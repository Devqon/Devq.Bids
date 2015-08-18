using Devq.Bids.Models;
using Orchard.ContentManagement;

namespace Devq.Bids.ViewModels {
    public class BidCountViewModel {
        public BidCountViewModel() {
        }

        public BidCountViewModel(BidsPart part) {
            Item = part.ContentItem;
            //BidCount = part.Bids.Count;
        }

        public ContentItem Item { get; set; }
        public int BidCount { get; set; }
    }
}