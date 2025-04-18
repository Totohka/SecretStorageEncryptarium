using Microsoft.AspNetCore.Authorization;

namespace Encryptarium.Storage.Requirements
{
    public class TokenRequirement : IAuthorizationRequirement
    {
        public TokenRequirement()
        {
        }
    }
}

