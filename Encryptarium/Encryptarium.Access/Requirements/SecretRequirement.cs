using Microsoft.AspNetCore.Authorization;

namespace Encryptarium.Access.Requirements
{
    public class SecretRequirement : IAuthorizationRequirement
    {
        public SecretRequirement() { }
    }
}
