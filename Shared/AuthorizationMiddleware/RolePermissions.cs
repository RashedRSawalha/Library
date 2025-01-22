using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kernal;

namespace Shared.AuthorizationMiddleware
{
    public static class RolePermissions
    {
        public static readonly Dictionary<string, List<string>> RolePermissionsMapping = new()
        {
            {"Admin", new List<string>{"ManageUsers", "ViewReports", "EditData"} },
            { "User", new List<string> { "ViewReports" } }
        };
    }
}
