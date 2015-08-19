using System.Linq;
using Devq.Bids.Models;
using Devq.Bids.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;

namespace Devq.Bids.Drivers
{
    public class BidsPartDriver : ContentPartDriver<BidsPart> {

        private readonly IContentManager _contentManager;
        private readonly IBidService _bidService;

        public BidsPartDriver(IContentManager contentManager, IBidService bidService) {
            _contentManager = contentManager;
            _bidService = bidService;
        }

        protected override DriverResult Display(BidsPart part, string displayType, dynamic shapeHelper) {
            return Combined(
                ContentShape("Parts_ListOfBids", () => {

                    var list = shapeHelper.List();
                    var bids = part
                        .Bids
                        .OrderByDescending(b => b.BidPrice);

                    list.AddRange(bids.Select(b => _contentManager.BuildDisplay(b)));

                    return shapeHelper.Parts_ListOfBids(List: list);
                }),
                ContentShape("Parts_BidForm", () => {

                    var newBid = _contentManager.New("Bid");
                    if (newBid.Has<BidPart>()) newBid.As<BidPart>().BidedOn = part.Id;

                    var editorShape = _contentManager.BuildEditor(newBid);

                    return shapeHelper.Parts_BidForm(EditorShape: editorShape, CanStillBid: _bidService.CanStillBidOn(part));
                }));
        }

        protected override DriverResult Editor(BidsPart part, dynamic shapeHelper) {
            return ContentShape("Parts_Bids_Enable",
                () => shapeHelper.EditorTemplate(TemplateName: "Parts.Bids.Bids", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(BidsPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);

            if (part.BidType == BidType.Free || part.BidType == BidType.Open) {
                part.MinimumBidPrice = 0;
            }

            return Editor(part, shapeHelper);
        }
    }
}