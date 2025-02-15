﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.AuthorizationMiddleware
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AuthorizePermissionAttribute : Attribute
    {
        public string Permission { get; }

        public AuthorizePermissionAttribute(string permission)
        {
            Permission = permission;
        }
    }
}
