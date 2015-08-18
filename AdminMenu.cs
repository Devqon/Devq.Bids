using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Devq.Bids {
    public class AdminMenu : INavigationProvider {
        public Localizer T { get; set; }

        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder) {
            builder.AddImageSet("Bids")
                .Add(T("Bids"), "4",
                    menu => menu.Add(T("List"), "0", item => item.Action("Index", "Admin", new { area = "Devq.Bids" })
                        .Permission(Permissions.ManageBids)));
        }
    }
}
