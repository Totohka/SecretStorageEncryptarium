using BusinessLogic.Entities;
using BusinessLogic.Services.RabbitMQ.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Model.Enums;
using System.Text.Json;
using System.Text;
using Model;
using System.Security.Claims;
using BusinessLogic.Services.Tokens.Interface;
using Encryptarium.Auth.Attributes;
using BusinessLogic.Services.Users.Interface;
using Newtonsoft.Json.Linq;

namespace Encryptarium.Auth.Middlewares
{
    public class MonitoringMiddleware
    {
        private RequestDelegate _next;

        public MonitoringMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IRabbitMQService rabbitMQService, IAccessTokenService accessTokenService, IUserService userService)
        {
            Stream originalBody = context.Response.Body;
            using var memStream = new MemoryStream();
            context.Response.Body = memStream;

            var request = context.Request;
            string requestContent = "";
            if (request.ContentLength > 0)
            {
                var buffer = new byte[Convert.ToInt32(request.ContentLength)];
                await request.Body.ReadAsync(buffer, 0, buffer.Length);
                requestContent = Encoding.UTF8.GetString(buffer);

                request.Body.Position = 0;  //rewinding the stream to 0
            }


            await _next(context);
            var now = DateTime.UtcNow;
            #region userUid
            Guid userUid = Guid.Empty;
            string? accessToken = context.Request.Cookies[Constants.AccessToken];
            string? refreshTokenStr = context.Request.Cookies[Constants.RefreshToken];
            if (accessToken is not null && refreshTokenStr is not null)
            {
                Guid refreshToken = new Guid(refreshTokenStr);

                ServiceResponse<ClaimsPrincipal> serviceResponseToken = accessTokenService.GetPrincipalFromExpiredToken(accessToken, refreshToken);
                var claimsPrincipal = serviceResponseToken.Data;
                userUid = new Guid(claimsPrincipal.Claims.First(c => c.Type == Constants.ClaimUserUid).Value);
            }
            #endregion

            var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            var attributeAuth = endpoint?.Metadata.GetMetadata<AuthorizeAttribute>();
            var attribute = endpoint?.Metadata.GetMetadata<MonitoringAttribute>();
            if (attribute != null)
            {
                var microservice = attribute.Microservice;
                var controller = attribute.Controller;
                var method = attribute.Method;
                var entity = attribute.Entity;
                var partHttpContext = attribute.PartHttpContext;
                var nameParameter = attribute.NameParameter;
                Guid? entityUid = Guid.Empty;

                #region GetEntityUid
                switch (partHttpContext)
                {
                    case PartHttpContextEnum.RequestParameter:
                        if (context.Request.Query.TryGetValue(nameParameter, out var uid))
                        {
                            entityUid = Guid.Parse(uid);
                        }
                        else entityUid = Guid.Parse(context.Request.RouteValues[nameParameter].ToString());
                        break;
                    case PartHttpContextEnum.ResponseBody:
                        var responseParts = nameParameter.Split('.');
                        try
                        {
                            memStream.Position = 0;
                            string responseBody = new StreamReader(memStream, Encoding.UTF8, true, 1024, true).ReadToEnd();
                            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                            ServiceResponse<object> serviceResponse = JsonSerializer.Deserialize<ServiceResponse<object>>(responseBody, options);
                            if (responseParts.Count() == 1)
                            {
                                if (serviceResponse.Data is not null)
                                    entityUid = Guid.Parse(serviceResponse.Data.ToString());
                                else entityUid = null;
                            }
                            else
                            {
                                if (serviceResponse.Data is not null)
                                {
                                    dynamic json = JObject.Parse(serviceResponse.Data.ToString());
                                    entityUid = (Guid)json.uid;
                                }
                                else entityUid = null;

                            }
                            memStream.Position = 0;
                        }
                        finally
                        {
                        }
                        break;
                    case PartHttpContextEnum.RequestBody:
                        try
                        {
                            if (requestContent is not null && requestContent != "")
                            {
                                dynamic json = JObject.Parse(requestContent);
                                entityUid = json.uid;
                            }
                            else entityUid = null;
                        }
                        finally
                        {
                            //context.Response.Body = originalBody;
                        }
                        //entityUid = Guid.Parse(context.Request.Query[nameParameter]);
                        break;
                    default:
                        entityUid = null;
                        break;
                }
                #endregion

                AuthorizePoliciesEnum policy = AuthorizePoliciesEnum.None;
                if (attributeAuth is not null)
                    policy = (AuthorizePoliciesEnum)Enum.Parse(typeof(AuthorizePoliciesEnum), attributeAuth.Policy);

                bool isAdmin = false;
                if (attributeAuth is not null && attributeAuth.Roles is not null)
                    isAdmin = attributeAuth.Roles.Any() && attributeAuth.Roles.Contains(Constants.Admin);

                int statusCode = context.Response.StatusCode;
                StatusCodeEnum status = StatusCodeEnum.Error;
                if (statusCode < 400)
                {
                    try
                    {
                        memStream.Position = 0;
                        string responseBody = new StreamReader(memStream, Encoding.UTF8, true, 1024, true).ReadToEnd();
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        ServiceResponse<object> serviceResponse = JsonSerializer.Deserialize<ServiceResponse<object>>(responseBody, options);
                        if (serviceResponse.IsSuccess)
                        {
                            status = StatusCodeEnum.Success;
                        }
                        else
                        {
                            status = StatusCodeEnum.Warning;
                        }
                        memStream.Position = 0;
                        await memStream.CopyToAsync(originalBody);
                    }
                    finally
                    {
                        context.Response.Body = originalBody;
                    }
                }
                var serviceResponseUser = await userService.GetUserByUidAsync(userUid);
                var user = serviceResponseUser.Data;
                string nameUser = "Аноним";
                if (user is not null)
                {
                    nameUser = user.Login;
                }
                string action = $"Пользователь {nameUser} в {now} попытался получить доступ к " +
                                $"{microservice}.{controller}.{method} " +
                                $"c политикой доступа {policy}. Было произведено действие над сущностью {entity} " +
                                $"c uid равным {entityUid}" +
                                $" Результат: {status}.";
                MonitorMessage message = new MonitorMessage()
                {
                    UserUid = userUid,
                    SourceMicroservice = microservice,
                    SourceController = controller,
                    AuthorizePolice = policy,
                    SourceMethod = method,
                    StatusCode = status,
                    DateAct = now,
                    IsSourceRightAdmin = isAdmin,
                    Action = action,
                    Entity = entity,
                    EntityUid = entityUid
                };
                await rabbitMQService.SendMessage(message);
            }
        }
    }
}
