using System.Collections.Generic;
using System.Globalization;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;
using Orchard.Localization;

namespace Devq.Bids.Settings {
    public class BidsPartSettingsEvents : ContentDefinitionEditorEventsBase {

        public Localizer T { get; set; }

        public override IEnumerable<TemplateViewModel> TypePartEditor(ContentTypePartDefinition definition) {
            if (definition.PartDefinition.Name != "BidsPart")
                yield break;

            var settings = definition.Settings.GetModel<BidsPartSettings>();

            yield return DefinitionTemplate(settings);
        }

        public override IEnumerable<TemplateViewModel> TypePartEditorUpdate(ContentTypePartDefinitionBuilder builder, IUpdateModel updateModel) {
            if (builder.Name != "BidsPart")
                yield break;

            var settings = new BidsPartSettings {
            };

            if (updateModel.TryUpdateModel(settings, "BidsPartSettings", null, null)) {
                builder.WithSetting("BidsPartSettings.DefaultBidsShown", settings.DefaultBidsShown.ToString(CultureInfo.InvariantCulture));
                builder.WithSetting("BidsPartSettings.MustBeAuthenticated", settings.MustBeAuthenticated.ToString(CultureInfo.InvariantCulture));
            }

            yield return DefinitionTemplate(settings);
        }
    }
}
