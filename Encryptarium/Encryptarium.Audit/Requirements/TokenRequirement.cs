using Microsoft.AspNetCore.Authorization;

namespace Encryptarium.Audit.Requirements
{
    public class TokenRequirement : IAuthorizationRequirement
    {
        public TokenRequirement()
        {
        }
    }
}
