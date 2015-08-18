using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Devq.Bids.Models;
using Devq.Bids.Services;
using Devq.Bids.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Mvc;
using Orchard.Mvc.Extensions;
using Orchard.Settings;
using Orchard.UI.Navigation;

namespace Devq.Bids.Controllers {
    [ValidateInput(false)]
    public class AdminController : Controller, IUpdateModel {
        private readonly IOrchardServices _orchardServices;
        private readonly IBidService _bidService;
        private readonly ISiteService _siteService;
        private readonly IContentManager _contentManager;

        public AdminController(
            IOrchardServices orchardServices,
            IBidService BidService,
            ISiteService siteService,
            IShapeFactory shapeFactory) {
            _orchardServices = orchardServices;
            _bidService = BidService;
            _siteService = siteService;
            _contentManager = _orchardServices.ContentManager;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
            Shape = shapeFactory;
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }
        dynamic Shape { get; set; }

        public ActionResult Index(BidIndexOptions options, PagerParameters pagerParameters) {
            Pager pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);

            // Default options
            if (options == null)
                options = new BidIndexOptions();

            // Filtering
            IContentQuery<BidPart, BidPartRecord> bidsQuery;
            switch (options.Filter) {
                case BidIndexFilter.All:
                    bidsQuery = _bidService.GetBids();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var pagerShape = Shape.Pager(pager).TotalItemCount(bidsQuery.Count());
            var entries = bidsQuery
                .OrderByDescending<BidPartRecord>(b => b.BidedOn)
                .Slice(pager.GetStartIndex(), pager.PageSize)
                .ToList()
                .Select(CreateBidEntry);

            var model = new BidsIndexViewModel {
                Bids = entries.ToList(),
                Options = options,
                Pager = pagerShape
            };

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("submit.BulkEdit")]
        public ActionResult Index(FormCollection input) {
            var viewModel = new BidsIndexViewModel { Bids = new List<BidEntry>(), Options = new BidIndexOptions() };
            UpdateModel(viewModel);

            IEnumerable<BidEntry> checkedEntries = viewModel.Bids.Where(c => c.IsChecked);
            switch (viewModel.Options.BulkAction) {
                case BidIndexBulkAction.None:
                    break;
                case BidIndexBulkAction.Delete:
                    if (!_orchardServices.Authorizer.Authorize(Permissions.ManageBids, T("Couldn't delete Bid")))
                        return new HttpUnauthorizedResult();

                    foreach (BidEntry entry in checkedEntries) {
                        _bidService.DeleteBid(entry.Bid.Id);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Details(int id, BidDetailsOptions options) {
            // Default options
            if (options == null)
                options = new BidDetailsOptions();

            // Filtering
            IContentQuery<BidPart, BidPartRecord> Bids = _bidService.GetBidsForBidedContent(id);
            
            var entries = Bids.List().Select(Bid => CreateBidEntry(Bid)).ToList();
            var model = new BidsDetailsViewModel {
                Bids = entries,
                Options = options,
                DisplayNameForBidedItem = _bidService.GetDisplayForBidedContent(id) == null ? "" : _bidService.GetDisplayForBidedContent(id).DisplayText,
                BidedItemId = id,
                BidsClosedOnItem = _bidService.BidsDisabledForBidedContent(id),
            };
            return View(model);
        }

        [HttpPost]
        [FormValueRequired("submit.BulkEdit")]
        public ActionResult Details(FormCollection input) {
            var viewModel = new BidsDetailsViewModel { Bids = new List<BidEntry>(), Options = new BidDetailsOptions() };
            UpdateModel(viewModel);

            IEnumerable<BidEntry> checkedEntries = viewModel.Bids.Where(c => c.IsChecked);
            if (viewModel.Options.BulkAction == BidDetailsBulkAction.Delete) {
                if (!_orchardServices.Authorizer.Authorize(Permissions.ManageBids, T("Couldn't delete Bid")))
                    return new HttpUnauthorizedResult();

                foreach (BidEntry entry in checkedEntries) {
                    _bidService.DeleteBid(entry.Bid.Id);
                }

            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Disable(int BidedItemId, string returnUrl) {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ManageBids, T("Couldn't disable Bids")))
                return new HttpUnauthorizedResult();

            _bidService.DisableBidsForBidedContent(BidedItemId);
            return this.RedirectLocal(returnUrl, () => RedirectToAction("Index"));
        }

        [HttpPost]
        public ActionResult Enable(int BidedItemId, string returnUrl) {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ManageBids, T("Couldn't enable Bids")))
                return new HttpUnauthorizedResult();

            _bidService.EnableBidsForBidedContent(BidedItemId);

            return this.RedirectLocal(returnUrl, () => RedirectToAction("Index"));
        }

        [HttpPost]
        public ActionResult Delete(int id, string returnUrl) {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ManageBids, T("Couldn't delete Bid")))
                return new HttpUnauthorizedResult();

            var BidPart = _contentManager.Get<BidPart>(id);
            if (BidPart == null)
                return new HttpNotFoundResult();

            int BidedOn = BidPart.Record.BidedOn;
            _bidService.DeleteBid(id);

            return this.RedirectLocal(returnUrl, () => RedirectToAction("Details", new { id = BidedOn }));
        }

        private BidEntry CreateBidEntry(BidPart item) {
            return new BidEntry {
                Bid = item.Record,
                BidedOn = _bidService.GetBidedContent(item.BidedOn),
                IsChecked = false,
                IsHeighest = _bidService.IsHeighestBid(item.Record.Id)
            };
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}
