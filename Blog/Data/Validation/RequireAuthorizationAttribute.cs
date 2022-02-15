using Blog.Data.DbModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blog.Data.Validation
{
    public class RequireAuthorizationAttribute : TypeFilterAttribute
    {
        public RequireAuthorizationAttribute() : base(typeof(ClaimRequirementFilter))
        {
            Arguments = new object[] { new Claim(ClaimTypes.Role, nameof(Role.User)) };
        }

        public RequireAuthorizationAttribute(Role role) : base(typeof(ClaimRequirementFilter))
        {
            Arguments = new object[] { new Claim(ClaimTypes.Role, role.ToString()) };
        }
    }
}