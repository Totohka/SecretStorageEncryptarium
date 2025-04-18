using Microsoft.AspNetCore.Authorization;

namespace Encryptarium.Storage.Requirements
{
    public class SecretRequirement : IAuthorizationRequirement
    {
        public SecretRequirement() { }
    }
}
