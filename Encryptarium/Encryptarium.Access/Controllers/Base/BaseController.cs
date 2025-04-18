using BusinessLogic.Entities;
using BusinessLogic.Services.Tokens.Interface;
using Microsoft.AspNetCore.Mvc;
using Model;
using System.Security.Claims;

namespace Encryptarium.Access.Controllers.Base
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        private protected readonly IAccessTokenService _accessTokenService;
        private protected readonly ILogger _logger;
        private protected string? ipAddress => HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        public BaseController(IAccessTokenService accessTokenService, ILogger logger) 
        {
            _accessTokenService = accessTokenService;
            _logger = logger;
        }

        protected Guid? GetClaimUserUid() 
        {
            _logger.LogInformation("Вызван метод BaseController.GetClaimUserUid()");
            string? accessToken = HttpContext.Request.Cookies[Constants.AccessToken];
            string? refreshTokenStr = HttpContext.Request.Cookies[Constants.RefreshToken];

            if (accessToken is null || refreshTokenStr is null)
            {
                return null;
            }

            Guid refreshToken = new Guid(refreshTokenStr);

            ServiceResponse<ClaimsPrincipal> serviceResponse = _accessTokenService.GetPrincipalFromExpiredToken(accessToken, refreshToken);
            var claimsPrincipal = serviceResponse.Data;
            Guid userUid = new Guid(claimsPrincipal.Claims.First(c => c.Type == Constants.ClaimUserUid).Value);
            return userUid;
        }

        protected T GetEntity<T>(ServiceResponse<T> resp) 
        {
            _logger.LogInformation("Вызван метод BaseController.GetEntity()");
            if (!resp.IsSuccess)
                throw new Exception("Неизвестная ошибка");
            return resp.Data;
        }

        protected bool GetBool(ServiceResponse<bool> resp)
        {
            _logger.LogInformation("Вызван метод BaseController.GetBool()");
            if (!resp.IsSuccess)
                throw new Exception("Неизвестная ошибка");
            return resp.Data;
        }
    }
}
