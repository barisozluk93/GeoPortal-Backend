using Microsoft.AspNetCore.Authorization;

namespace SupportManagement.Authorization
{
    public class HasPermissionAttribute : AuthorizeAttribute
    {

        public HasPermissionAttribute(string permission) : base(policy: permission) 
        {
        
        }
    }
}
