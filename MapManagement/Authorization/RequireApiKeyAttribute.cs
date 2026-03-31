using Microsoft.AspNetCore.Authorization;

namespace MapManagement.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RequireApiKeyAttribute : Attribute
    {
    }
}
