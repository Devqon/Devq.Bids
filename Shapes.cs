using System.IO;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Orchard;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Mvc.Html;
using Orchard.Utility.Extensions;

namespace Devq.Bids {
    public class Shapes : IDependency {
        public Shapes() {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        [Shape]
        public void BidsummaryLinks(dynamic display, TextWriter output, HtmlHelper html, ContentItem item, int count, int pendingCount) {
            var bidText = "";

            if (item.Id != 0) {
                var totalBidCount = count + pendingCount;
                var totalBidText = T.Plural("1 Bid", "{0} Bids", totalBidCount);
                if (totalBidCount == 0) {
                    bidText += totalBidText.ToString();
                }
                else {
                    bidText +=
                        html.ActionLink(
                            totalBidText.ToString(),
                            "Details",
                            new {
                                Area = "Devq.Bids",
                                Controller = "Admin",
                                id = item.Id,
                                returnUrl = html.ViewContext.HttpContext.Request.ToUrlString()
                            });
                }

                if (pendingCount > 0) {
                    bidText += " " + html.ActionLink(T("({0} pending)", pendingCount).ToString(),
                                                   "Details",
                                                   new {
                                                       Area = "Devq.Bids",
                                                       Controller = "Admin",
                                                       id = item.Id,
                                                       returnUrl = html.ViewContext.HttpContext.Request.Url
                                                   });
                }
            }

            output.Write(bidText);
        }
    }
}
