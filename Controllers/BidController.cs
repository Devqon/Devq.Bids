using System.Web.Mvc;
using Devq.Bids.Models;
using Devq.Bids.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Mvc.Extensions;
using Orchard.UI.Notify;

namespace Devq.Bids.Controllers {
    public class BidController : Controller, IUpdateModel {
        private readonly IOrchardServices _services;
        private readonly IBidService _bidService;
        private readonly INotifier _notifier;

        public Localizer T { get; set; }

        public BidController(IOrchardServices services, IBidService bidService) {
            _services = services;
            _bidService = bidService;
            _notifier = _services.Notifier;

            T = NullLocalizer.Instance;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Create(string returnUrl) {
            if (!_services.Authorizer.Authorize(Permissions.AddBid, T("Couldn't add bid")))
                return this.RedirectLocal(returnUrl, "~/");

            var bid = _services.ContentManager.New<BidPart>("Bid");
            var editorShape = _services.ContentManager.UpdateEditor(bid, this);

            if (ModelState.IsValid) {
                _services.ContentManager.Create(bid);

                var bidPart = bid.As<BidPart>();

                // Check if bid is higher than others
                var heighestBid = _bidService.GetHeighestBid(bid.BidedOn);
                if (bidPart.BidPrice <= heighestBid.BidPrice && bidPart.Id != heighestBid.Id)
                {
                    _services.TransactionManager.Cancel();
                    _notifier.Error(T("Bid must be higher than {0}", heighestBid.BidPrice.ToString("c")));
                    return this.RedirectLocal(returnUrl, "~/");
                }

                // Check if bid is higher than minimum bid
                var bidsPart = _bidService.GetContainer(bidPart);
                var minimumPrice = bidsPart.MinimumBidPrice;
                if (bidPart.BidPrice <= minimumPrice)
                {
                    _services.TransactionManager.Cancel();
                    _notifier.Error(T("Bid must be higher than {0}", minimumPrice.ToString("c")));
                    return this.RedirectLocal(returnUrl, "~/");
                }

                // Ensure the bids are not closed on the container, as the html could have been tampered manually
                if (!_bidService.CanCreateBid(bidPart))
                {
                    _services.TransactionManager.Cancel();
                    return this.RedirectLocal(returnUrl, "~/");
                }

                _notifier.Information(T("Your Bid has been added."));

                if (bidsPart.NotificationEmail) {
                    _bidService.SendNotificationEmail(bidPart);
                }
            }
            else {
                _services.TransactionManager.Cancel();

                TempData["Bids.InvalidBidEditorShape"] = editorShape;
            }

            return this.RedirectLocal(returnUrl, "~/");
        }

        public ActionResult Delete(string nonce) {
            int id;
            if (_bidService.DecryptNonce(nonce, out id))
            {
                _bidService.DeleteBid(id);
            }

            _notifier.Information(T("Bid deleted successfully"));
            return Redirect("~/");
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}