using Microsoft.AspNetCore.Authorization;

namespace Encryptarium.Access.Requirements
{
    public class StorageRequirement : IAuthorizationRequirement
    {
        public StorageRequirement() { }
    }
}
