using System.Collections.Generic;
using Devq.Bids.Models;
using Orchard.ContentManagement;

namespace Devq.Bids.ViewModels {
    public class BidsIndexViewModel {
        public IList<BidEntry> Bids { get; set; }
        public BidIndexOptions Options { get; set; }
        public dynamic Pager { get; set; }
    }

    public class BidEntry {
        public BidPartRecord Bid { get; set; }
        public IContent BidedOn { get; set; }
        public bool IsChecked { get; set; }
        public bool IsHeighest { get; set; }
    }

    public class BidIndexOptions {
        public BidIndexFilter Filter { get; set; }
        public BidIndexBulkAction BulkAction { get; set; }
    }

    public enum BidIndexBulkAction {
        None,
        Delete
    }

    public enum BidIndexFilter {
        All
    }
}
