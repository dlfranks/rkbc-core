using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace rkbc.models.extension
{
    public static class IdentityExtensions
    {
        public static string GetDepartment(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("department");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }
    }
}
