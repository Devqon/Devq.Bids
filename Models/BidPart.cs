using System;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Security;

namespace Devq.Bids.Models
{
    public class BidPart : ContentPart<BidPartRecord> {
        public int BidedOn {
            get { return Retrieve(r => r.BidedOn); }
            set { Store(r => r.BidedOn, value); }
        }

        public decimal BidPrice {
            get { return Retrieve(r => r.BidPrice); }
            set { Store(r => r.BidPrice, value); }
        }

        public DateTime? Date
        {
            get { return this.As<CommonPart>().CreatedUtc; }
        }

        public IUser Owner {
            get { return this.As<CommonPart>().Owner; }
        }
    }
}