using System.Collections.Generic;

namespace Devq.Bids.ViewModels {
    public class BidsDetailsViewModel {
        public IList<BidEntry> Bids { get; set; }
        public BidDetailsOptions Options { get; set; }
        public string DisplayNameForBidedItem { get; set; }
        public int BidedItemId { get; set; }
        public bool BidsClosedOnItem { get; set; }
    }

    public class BidDetailsOptions {
        public BidDetailsFilter Filter { get; set; }
        public BidDetailsBulkAction BulkAction { get; set; }
    }

    public enum BidDetailsBulkAction {
        None,
        Delete
    }

    public enum BidDetailsFilter {
        All
    }
}
