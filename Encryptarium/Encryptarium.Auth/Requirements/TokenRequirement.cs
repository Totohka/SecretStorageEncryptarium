using System.Text;
using System.IdentityModel.Tokens.Jwt;
using DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using Model;
using Model.Entities;

namespace Encryptarium.Auth.Requirements
{
    public class TokenRequirement : IAuthorizationRequirement
    {
        public TokenRequirement()
        {
        }
    }
}

