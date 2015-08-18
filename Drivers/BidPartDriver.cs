using Devq.Bids.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;

namespace Devq.Bids.Drivers
{
    public class BidPartDriver : ContentPartDriver<BidPart> {

        protected override DriverResult Display(BidPart part, string displayType, dynamic shapeHelper) {
            return ContentShape("Parts_Bid", () => shapeHelper.Parts_Bid());
        }

        protected override DriverResult Editor(BidPart part, dynamic shapeHelper) {
            return ContentShape("Parts_Bid_Edit",
                () => shapeHelper.EditorTemplate(TemplateName: "Parts.Bid", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(BidPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);

            return Editor(part, shapeHelper);
        }
    }
}