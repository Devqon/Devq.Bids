using System.Linq;
using Devq.Bids.Models;
using Devq.Bids.Services;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;

namespace Devq.Bids.Handlers
{
    public class BidsPartHandler : ContentHandler {
        private readonly IBidService _bidService;

        public BidsPartHandler(IRepository<BidsPartRecord> repository, 
            IBidService bidService) {

            _bidService = bidService;

            Filters.Add(StorageFilter.For(repository));

            OnLoading<BidsPart>(LoadBids);
        }

        private void LoadBids(LoadContentContext context, BidsPart part) {
            part._bidsFields.Loader(field => {

                var bids = _bidService.GetBidsForBidedContent(context.ContentItem.Id);
                return bids.List();
            });

            part._heighestBidField.Loader(field => part
                ._bidsFields
                .Value
                .OrderByDescending(b => b.BidPrice)
                .FirstOrDefault());
        }
    }
}