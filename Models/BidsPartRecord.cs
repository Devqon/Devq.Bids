using Orchard.ContentManagement.Records;

namespace Devq.Bids.Models
{
    public class BidsPartRecord : ContentPartRecord
    {
        public virtual decimal MinimumBidPrice { get; set; }
        public virtual BidType BidType { get; set; }
        public virtual bool BidsActive { get; set; }
    }
}