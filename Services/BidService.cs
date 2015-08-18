using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Devq.Bids.Models;
using Devq.Bids.Settings;
using JetBrains.Annotations;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Environment.Configuration;
using Orchard.Environment.Descriptor;
using Orchard.Environment.State;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Services;

namespace Devq.Bids.Services {
    [UsedImplicitly]
    public class BidService : IBidService {
        private readonly IOrchardServices _orchardServices;
        private readonly IClock _clock;
        private readonly IEncryptionService _encryptionService;
        private readonly IProcessingEngine _processingEngine;
        private readonly ShellSettings _shellSettings;
        private readonly IShellDescriptorManager _shellDescriptorManager;
        private readonly HashSet<int> _processedBidsParts = new HashSet<int>();

        public BidService(
            IOrchardServices orchardServices, 
            IClock clock, 
            IEncryptionService encryptionService,
            IProcessingEngine processingEngine,
            ShellSettings shellSettings,
            IShellDescriptorManager shellDescriptorManager) {
            _orchardServices = orchardServices;
            _clock = clock;
            _encryptionService = encryptionService;
            _processingEngine = processingEngine;
            _shellSettings = shellSettings;
            _shellDescriptorManager = shellDescriptorManager;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public BidPart GetBid(int id) {
            return _orchardServices.ContentManager.Get<BidPart>(id);
        }
        
        public IContentQuery<BidPart, BidPartRecord> GetBids() {
            return _orchardServices.ContentManager
                       .Query<BidPart, BidPartRecord>();
        }

        public IContentQuery<BidPart, BidPartRecord> GetBidsForBidedContent(int id) {
            return GetBids()
                       .Where(c => c.BidedOn == id);
        }

        public ContentItemMetadata GetDisplayForBidedContent(int id) {
            var content = GetBidedContent(id);
            if (content == null)
                return null;
            return _orchardServices.ContentManager.GetItemMetadata(content);
        }

        public IContentQuery<BidPart, BidPartRecord> GetBidsDescending(int id) {
            return GetBidsForBidedContent(id)
                        .OrderByDescending(b => b.BidPrice);
        }

        // Get highest bid
        public BidPart GetHeighestBid(int id) {
            return GetBidsDescending(id)
                    .List()
                    .FirstOrDefault();
        }

        // Check if the bid is the heighest bid (compared to the bided on content item)
        public bool IsHeighestBid(int id) {
            var bidPart = GetBid(id);
            var heighestBid = GetHeighestBid(bidPart.BidedOn);
            return id == heighestBid.Id;
        }

        public ContentItem GetBidedContent(int id) {
            var result = _orchardServices.ContentManager.Get(id, VersionOptions.Published);
            if (result == null)
                result = _orchardServices.ContentManager.Get(id, VersionOptions.Draft);
            return result;
        }

        public void ProcessBidsCount(int bidsPartId) {
            if (!_processedBidsParts.Contains(bidsPartId)) {
                _processedBidsParts.Add(bidsPartId);
                _processingEngine.AddTask(_shellSettings, _shellDescriptorManager.GetShellDescriptor(), "IBidsCountProcessor.Process", new Dictionary<string, object> { { "BidsPartId", bidsPartId } });
            }
        }

        public void DeleteBid(int bidId) {
            _orchardServices.ContentManager.Remove(_orchardServices.ContentManager.Get<BidPart>(bidId).ContentItem);
        }

        public bool BidsDisabledForBidedContent(int id) {
            return !_orchardServices.ContentManager.Get<BidsPart>(id, VersionOptions.Latest).BidsActive;
        }

        public void DisableBidsForBidedContent(int id) {
            _orchardServices.ContentManager.Get<BidsPart>(id, VersionOptions.Latest).BidsActive = false;
        }

        public void EnableBidsForBidedContent(int id) {
            _orchardServices.ContentManager.Get<BidsPart>(id, VersionOptions.Latest).BidsActive = true;
        }

        public string CreateNonce(BidPart bid, TimeSpan delay) {
            var challengeToken = new XElement("n", new XAttribute("c", bid.Id), new XAttribute("v", _clock.UtcNow.ToUniversalTime().Add(delay).ToString(CultureInfo.InvariantCulture))).ToString();
            var data = Encoding.UTF8.GetBytes(challengeToken);
            return Convert.ToBase64String(_encryptionService.Encode(data));
        }

        public bool DecryptNonce(string nonce, out int id) {
            id = 0;

            try {
                var data = _encryptionService.Decode(Convert.FromBase64String(nonce));
                var xml = Encoding.UTF8.GetString(data);
                var element = XElement.Parse(xml);
                id = Convert.ToInt32(element.Attribute("c").Value);
                var validateByUtc = DateTime.Parse(element.Attribute("v").Value, CultureInfo.InvariantCulture);
                return _clock.UtcNow <= validateByUtc;
            }
            catch {
                return false;
            }

        }

        public decimal GetMinimumBidPrice(BidPart bidPart) {
            var container = _orchardServices.ContentManager.Get(bidPart.BidedOn);
            var bidsPart = container.As<BidsPart>();
            return bidsPart.MinimumBidPrice;
        }

        public bool CanCreateBid(BidPart bidPart) {
            if (bidPart == null) {
                return false;
            }

            var container = _orchardServices.ContentManager.Get(bidPart.BidedOn);
            
            if (container == null) {
                return false;
            }

            var bidsPart = container.As<BidsPart>();
            if (bidsPart == null) {
                return false;
            }
            
            var settings = bidsPart.TypePartDefinition.Settings.GetModel<BidsPartSettings>();
            if (!bidsPart.BidsActive) {
                return false;
            }

            if (settings.MustBeAuthenticated && _orchardServices.WorkContext.CurrentUser == null) {
                return false;
            }

            if (bidPart.BidPrice < bidsPart.MinimumBidPrice) {
                return false;
            }
            
            if (!CanStillBidOn(bidsPart)) {
                return false;
            }

            return true;
        }

        public bool CanStillBidOn(BidsPart bidsPart) {
            var bidsettings = _orchardServices.WorkContext.CurrentSite.As<BidSettingsPart>();
            if (bidsettings == null)
            {
                return false;
            }

            if (bidsettings.ClosedBidsDelay > 0)
            {
                var commonPart = bidsPart.As<CommonPart>();
                if (bidsPart == null)
                {
                    return false;
                }

                if (!commonPart.CreatedUtc.HasValue)
                {
                    return false;
                }

                if (commonPart.CreatedUtc.Value.AddDays(bidsettings.ClosedBidsDelay) < _clock.UtcNow)
                {
                    return false;
                }
            }

            return true;
        }

        //private BidPart GetBidWithQueryHints(int id) {
        //    return _orchardServices.ContentManager.Get<BidPart>(id, VersionOptions.Latest, new QueryHints().ExpandParts<BidPart>());
        //}
    }
}
