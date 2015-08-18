using System.Collections.Generic;
using Devq.Bids.Models;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Workflows.Models;
using Orchard.Workflows.Services;

namespace Devq.Bids.Activities {
    [OrchardFeature("Devq.Bids.Workflows")]
    public class CloseBidsActivity : Task {

        public CloseBidsActivity() {
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public override string Name {
            get { return "CloseBids"; }
        }

        public override LocalizedString Category {
            get { return T("Bids"); }
        }

        public override LocalizedString Description {
            get { return T("Closes the bids on the currently processed content item.");  }
        }

        public override string Form {
            get { return null; }
        }

        public override IEnumerable<LocalizedString> GetPossibleOutcomes(WorkflowContext workflowContext, ActivityContext activityContext) {
            return new[] {T("Done")};
        }

        public override IEnumerable<LocalizedString> Execute(WorkflowContext workflowContext, ActivityContext activityContext) {
            var content = workflowContext.Content;

            if (content != null) {
                var bids = content.As<BidsPart>();
                if (bids != null) {
                    bids.BidsActive = false;
                }
            }

            yield return T("Done");
        }
    }
}