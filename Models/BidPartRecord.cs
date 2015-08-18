using System;
using Orchard.ContentManagement.Records;

namespace Devq.Bids.Models {
    public class BidPartRecord : ContentPartRecord {
        public virtual int BidedOn { get; set; }
        public virtual decimal BidPrice { get; set; }
        public virtual string Bider { get; set; }
        public virtual DateTime? BidDateUtc { get; set; }
    }
}