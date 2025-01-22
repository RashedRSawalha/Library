using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;



namespace LibraryManagementApplication.Authorization
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class AuthorizeRoleAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public AuthorizeRoleAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var role = context.HttpContext.Items["Role"]?.ToString();

            // Log the role for debugging
            Console.WriteLine($"Role in HttpContext: {role}");

            if (string.IsNullOrEmpty(role) || !_roles.Contains(role))
            {
                Console.WriteLine("Access denied: Role not authorized.");
                context.Result = new ForbidResult(); // Return 403 Forbidden if not authorized
                return;
            }

            Console.WriteLine($"Access granted: Role {role} is authorized.");
        }
    }

}