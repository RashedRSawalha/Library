using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementApplication.Authorization
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class AuthorizeRoleOrPermissionAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;
        private readonly string[] _permissions;

        public AuthorizeRoleOrPermissionAttribute(string[] roles = null, string[] permissions = null)
        {
            _roles = roles ?? Array.Empty<string>();
            _permissions = permissions ?? Array.Empty<string>();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var role = context.HttpContext.Items["Role"]?.ToString();
            var permissions = context.HttpContext.Items["Permissions"] as List<string>;

            var isAuthorized = false;

            // Check role-based authorization
            if (!string.IsNullOrEmpty(role) && _roles.Contains(role))
            {
                isAuthorized = true;
            }

            // Check permission-based authorization
            if (permissions != null && _permissions.Any(p => permissions.Contains(p)))
            {
                isAuthorized = true;
            }

            if (!isAuthorized)
            {
                context.Result = new ForbidResult(); // Return 403 Forbidden
            }
        }
    }
}
