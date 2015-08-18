using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace Devq.Bids {
    public class Permissions : IPermissionProvider {
        public static readonly Permission AddBid = new Permission { Description = "Add Bid", Name = "AddBid" };
        public static readonly Permission ManageBids = new Permission { Description = "Manage Bids", Name = "ManageBids" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions() {
            return new[] {
                AddBid,
                ManageBids,
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes() {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {ManageBids, AddBid}
                },
                new PermissionStereotype {
                    Name = "Anonymous",
                    Permissions = new[] {AddBid}
                },
                new PermissionStereotype {
                    Name = "Authenticated",
                    Permissions = new[] {AddBid}
                },
                new PermissionStereotype {
                    Name = "Editor",
                    Permissions = new[] {AddBid}
                },
                new PermissionStereotype {
                    Name = "Moderator",
                    Permissions = new[] {ManageBids, AddBid}
                },
                new PermissionStereotype {
                    Name = "Author",
                    Permissions = new[] {AddBid}
                },
                new PermissionStereotype {
                    Name = "Contributor",
                    Permissions = new[] {AddBid}
                },
            };
        }
    }
}
