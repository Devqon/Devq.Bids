using System;
using Devq.Bids.Models;
using Orchard;
using Orchard.ContentManagement;

namespace Devq.Bids.Services {
    public interface IBidService : IDependency {
        IContentQuery<BidPart, BidPartRecord> GetBids();
        IContentQuery<BidPart, BidPartRecord> GetBidsForBidedContent(int id);
        BidPart GetBid(int id);
        ContentItemMetadata GetDisplayForBidedContent(int id);
        ContentItem GetBidedContent(int id);
        IContentQuery<BidPart, BidPartRecord> GetBidsDescending(int id);
        BidPart GetHeighestBid(int id);
        bool IsHeighestBid(int id);
        void ProcessBidsCount(int bidsPartId);
        void DeleteBid(int bidId);
        bool BidsDisabledForBidedContent(int id);
        void DisableBidsForBidedContent(int id);
        void EnableBidsForBidedContent(int id);
        bool DecryptNonce(string nonce, out int id);
        string CreateNonce(BidPart bid, TimeSpan delay);
        decimal GetMinimumBidPrice(BidPart bidPart);
        bool CanStillBidOn(BidsPart bidsPart);
        bool CanCreateBid(BidPart bidPart);

        BidsPart GetContainer(BidPart bidPart);
    }
}