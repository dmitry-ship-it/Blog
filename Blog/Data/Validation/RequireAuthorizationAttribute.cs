using Blog.Data.DbModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blog.Data.Validation
{
    public class RequireAuthorizationAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Checks if user is in <b>User</b> role
        /// </summary>
        public RequireAuthorizationAttribute() : base(typeof(ClaimRequirementFilter))
        {
            Arguments = new object[] { new Claim(ClaimTypes.Role, nameof(Role.User)) };
        }

        /// <summary>
        /// Checks if user is in specified role.
        /// </summary>
        /// <param name="role"></param>
        public RequireAuthorizationAttribute(Role role) : base(typeof(ClaimRequirementFilter))
        {
            Arguments = new object[] { new Claim(ClaimTypes.Role, role.ToString()) };
        }
    }
}