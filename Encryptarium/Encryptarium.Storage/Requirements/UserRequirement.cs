using Microsoft.AspNetCore.Authorization;

namespace Encryptarium.Storage.Requirements
{
    public class UserRequirement : IAuthorizationRequirement
    {
        public UserRequirement() { }
    }
}
