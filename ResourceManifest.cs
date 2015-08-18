using Orchard.UI.Resources;

namespace Devq.Bids {
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();
            manifest.DefineStyle("Admin").SetUrl("devq-bids-admin.css");
        }
    }
}