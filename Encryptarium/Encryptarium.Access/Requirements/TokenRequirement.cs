using Microsoft.AspNetCore.Authorization;

namespace Encryptarium.Access.Requirements
{
    public class TokenRequirement : IAuthorizationRequirement
    {
        public TokenRequirement()
        {
        }
    }
}

