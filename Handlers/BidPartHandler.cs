using Devq.Bids.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace Devq.Bids.Handlers
{
    public class BidPartHandler : ContentHandler
    {
        public BidPartHandler(IRepository<BidPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}