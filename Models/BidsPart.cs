﻿using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;

namespace Devq.Bids.Models
{
    public class BidsPart : ContentPart<BidsPartRecord>
    {
        internal LazyField<IEnumerable<BidPart>> _bidsFields = new LazyField<IEnumerable<BidPart>>();
        internal LazyField<BidPart> _heighestBidField = new LazyField<BidPart>(); 

        public IEnumerable<BidPart> Bids {
            get { return _bidsFields.Value; }
        }

        public BidPart HeighestBid {
            get { return _heighestBidField.Value; }
        }

        public decimal MinimumBidPrice {
            get { return Retrieve(r => r.MinimumBidPrice); }
            set { Store(r => r.MinimumBidPrice, value); }
        }

        public BidType BidType {
            get { return Retrieve(r => r.BidType); }
            set { Store(r => r.BidType, value); }
        }

        public bool BidsActive {
            get { return Retrieve(r => r.BidsActive); }
            set { Store(r => r.BidsActive, value); }
        }

        public bool BidsDisabled {
            get { return BidType == BidType.Free || BidType == BidType.Static; }
        }

        public bool NotificationEmail
        {
            get { return Retrieve(x => x.NotificationEmail); }
            set { Store(x => x.NotificationEmail, value); }
        }
    }
}