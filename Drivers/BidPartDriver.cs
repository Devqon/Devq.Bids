using Devq.Bids.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Services;

namespace Devq.Bids.Drivers
{
    public class BidPartDriver : ContentPartDriver<BidPart> {

        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IClock _clock;

        public BidPartDriver(IWorkContextAccessor workContextAccessor, IClock clock) {
            _workContextAccessor = workContextAccessor;
            _clock = clock;
        }

        protected override DriverResult Display(BidPart part, string displayType, dynamic shapeHelper) {
            return ContentShape("Parts_Bid", () => shapeHelper.Parts_Bid());
        }

        protected override DriverResult Editor(BidPart part, dynamic shapeHelper) {
            return ContentShape("Parts_Bid_Edit",
                () => shapeHelper.EditorTemplate(TemplateName: "Parts.Bid", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(BidPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);

            var workContext = _workContextAccessor.GetContext();
            var currentUser = workContext.CurrentUser;
            part.Bider = currentUser != null ? currentUser.UserName : null;

            part.BidDateUtc = _clock.UtcNow;

            return Editor(part, shapeHelper);
        }
    }
}