using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.AuthorizationMiddleware
{
    public static class SkipAuthorizationActions
    {
        private static readonly HashSet<(string Controller, string Action)> SkipList = new()
        {
            ("Auth", "Login"),
            ("Auth","Register")
        };

        public static bool ShouldSkip(String controller, String action)
        {
            return SkipList.Contains((controller, action));
        }
    }
}
