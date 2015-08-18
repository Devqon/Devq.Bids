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

        public string Bider {
            get { return Retrieve(r => r.Bider); }
            set { Store(r => r.Bider, value); }
        }

        public DateTime? BidDateUtc {
            get { return Retrieve(r => r.BidDateUtc); }
            set { Store(r => r.BidDateUtc, value); }
        }

        public IUser Owner {
            get { return this.As<CommonPart>().Owner; }
        }
    }
}