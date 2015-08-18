using System;
using System.Web.Mvc;
using Devq.Bids.Models;
using Devq.Bids.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Mvc.Extensions;
using Orchard.Tokens;

namespace Devq.Bids.Tokens {

    public class BidTokens : ITokenProvider {
        private readonly IContentManager _contentManager;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IBidService _bidService;

        public BidTokens(
            IContentManager contentManager,
            IWorkContextAccessor workContextAccessor,
            IBidService BidService) {
            _contentManager = contentManager;
            _workContextAccessor = workContextAccessor;
            _bidService = BidService;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void Describe(DescribeContext context) {
            context.For("Content", T("Bids"), T("Bids"))
                .Token("BidedOn", T("Bided On"), T("The content item this Bid was created on."))
                .Token("BidPrice", T("Bid Price"), T("The price of the bid itself"))
                .Token("BidAuthor", T("Bid Author"), T("The author of the bid."))
                .Token("BidDeleteUrl", T("Bid deletion Url"), T("The absolute url to follow in order to delete this Bid."))
                ;
        }

        public void Evaluate(EvaluateContext context) {
            context.For<IContent>("Content")
                .Token("BidedOn", content => content.As<BidPart>().BidedOn)
                .Chain("BidedOn", "Content", content => _contentManager.Get(content.As<BidPart>().BidedOn))
                .Token("BidPrice", content => content.As<BidPart>().BidPrice)
                .Chain("BidPrice", "Text", content => content.As<BidPart>().BidPrice)
                //.Token("BidAuthor", BidAuthor)
                //.Chain("BidAuthor", "Text", BidAuthor)
                .Token("BidDeleteUrl", content => CreateProtectedUrl("Delete", content.As<BidPart>()))
                ;
        }

        private string CreateProtectedUrl(string action, BidPart part) {
            var workContext = _workContextAccessor.GetContext();
            if (workContext.HttpContext != null) {
                var url = new UrlHelper(workContext.HttpContext.Request.RequestContext);
                return url.AbsoluteAction(action, "Bid", new {area = "Devq.Bids", nonce = _bidService.CreateNonce(part, TimeSpan.FromDays(7))});
            }

            return null;
        }

        //private static string BidAuthor(IContent Bid) {
        //    var BidPart = Bid.As<BidPart>();
        //    return String.IsNullOrWhiteSpace(BidPart.UserName) ? BidPart.Author : BidPart.UserName;
        //}
    }
}