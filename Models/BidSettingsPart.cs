using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;

namespace Devq.Bids.Models
{
    public class BidSettingsPart : ContentPart
    {
        [Range(0, int.MaxValue, ErrorMessage = "Must be a positive number")]
        public int ClosedBidsDelay {
            get { return this.Retrieve(x => x.ClosedBidsDelay); }
            set { this.Store(r => r.ClosedBidsDelay, value); }
        }
    }
}