using System.Linq;
using Devq.Bids.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;

namespace Devq.Bids.Drivers
{
    public class BidsPartDriver : ContentPartDriver<BidsPart> {

        private readonly IContentManager _contentManager;

        public BidsPartDriver(IContentManager contentManager) {
            _contentManager = contentManager;
        }

        protected override DriverResult Display(BidsPart part, string displayType, dynamic shapeHelper) {
            return Combined(
                ContentShape("Parts_ListOfBids", () => {

                    var list = shapeHelper.List();
                    list.AddRange(part.Bids.Select(b => _contentManager.BuildDisplay(b)));

                    return shapeHelper.Parts_ListOfBids(List: list);
                }),
                ContentShape("Parts_BidForm", () => {

                    var newBid = _contentManager.New("Bid");
                    if (newBid.Has<BidPart>()) newBid.As<BidPart>().BidedOn = part.Id;

                    var editorShape = _contentManager.BuildEditor(newBid);

                    return shapeHelper.Parts_BidForm(EditorShape: editorShape, CanStillBid: true);
                }));
        }

        protected override DriverResult Editor(BidsPart part, dynamic shapeHelper) {
            return ContentShape("Parts_Comments_Enable",
                () => shapeHelper.EditorTemplate(TemplateName: "Parts.Bids.Bids", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(BidsPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}