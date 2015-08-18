using Devq.Bids.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;

namespace Devq.Bids.Handlers
{
    public class BidSettingsPartHandler : ContentHandler {

        public Localizer T { get; set; }

        public BidSettingsPartHandler() {
            T = NullLocalizer.Instance;

            Filters.Add(new ActivatingFilter<BidSettingsPart>("Site"));
            Filters.Add(new TemplateFilterForPart<BidSettingsPart>("BidSettings", "Parts.Bids.SiteSettings", "Bids"));
        }

        protected override void GetItemMetadata(GetContentItemMetadataContext context)
        {
            if (context.ContentItem.ContentType != "Site")
                return;
            base.GetItemMetadata(context);
            context.Metadata.EditorGroupInfo.Add(new GroupInfo(T("Bids")));
        }
    }
}