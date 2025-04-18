using BusinessLogic.Services.OAuths.Interface;
using Model.Entities;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using Newtonsoft.Json;
using BusinessLogic.Entities;
using Microsoft.Extensions.Configuration;
using BusinessLogic.Services.Base;
using BusinessLogic.Services.Tokens.Interface;
using BusinessLogic.Services.Users.Interface;

namespace BusinessLogic.Services.OAuths.Implementation
{
    public class GitHubService : BaseService, IGitHubService
    {
        private readonly IUserService _userService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly HttpClient _httpClient;
        private readonly string _clientId;
        private readonly string _clientSecret;
        public GitHubService(HttpClient httpClient,
                             IUserService userService, 
                             IRefreshTokenService refreshTokenService,
                             IConfiguration configuration, 
                             ILogger<GitHubService> logger) : base(logger)
        {
            _clientId = configuration["OAuth:ClientId"];
            _clientSecret = configuration["OAuth:ClientSecret"];
            _httpClient = httpClient;
            _refreshTokenService = refreshTokenService;
            _userService = userService;
        }

        public async Task<ServiceResponse<User>> GetUserAsync(string code)
        {
            _logger.LogInformation("Вызван метод GitHubService.GetUserAsync()");

            HttpResponseMessage httpResponseMessage = await SendRequestCode(code);
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                var error = await httpResponseMessage.Content.ReadAsStringAsync();
                _logger.LogError($"Метод GitHubService.GetUserAsync(). {error}");
                return Error<User>("Ошибка при получении токена");
            }

            var resultString = await httpResponseMessage.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<GitHubTokenResponse>(resultString);

            User user = await GetUserByTokenAsync(result.access_token);
            if (user is null)
            {
                _logger.LogError($"Метод GitHubService.GetUserAsync(). Ошибка при получении или создании пользователя");
                return Error<User>("Ошибка при получении или создании пользователя");
            }

            return Ok(user);
        }

        private async Task<HttpResponseMessage> SendRequestCode(string code)
        {
            _logger.LogInformation("Вызван метод GitHubService.SendRequestCode()");

            var requestContent = new Dictionary<string, string>
            {
                { "client_id", _clientId },
                { "client_secret", _clientSecret },
                { "code", code },
                { "redirect_uri", "https://localhost:7192/api/OAuth/Github" }
            };
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token");
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requestMessage.Content = new FormUrlEncodedContent(requestContent);

            return await _httpClient.SendAsync(requestMessage);
        }

        private async Task<User> GetUserByTokenAsync(string accessToken)
        {
            _logger.LogInformation("Вызван метод GitHubService.GetUserByTokenAsync()");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Encryptarium"); 

            var response = await _httpClient.GetAsync("https://api.github.com/user");
            var email = await GetUserPrimaryEmailAsync(_httpClient);

            if (response.IsSuccessStatusCode)
            {
                var resultString = await response.Content.ReadAsStringAsync();
                var userInfo = JsonConvert.DeserializeObject<GitHubUser>(resultString);
                userInfo.email = email;

                ServiceResponse<Guid> respGuid = await _refreshTokenService.GenerateRefreshTokenAsync();
                Guid refreshTokenUid = respGuid.Data;

                ServiceResponse<bool> respCreateUser = await _userService.CreateUserOAuthAsync(userInfo, refreshTokenUid);

                //Если пользователь был уже создан, то рефреш токен нужно удалить
                if (!respCreateUser.Data)
                    await _refreshTokenService.DeleteRefreshTokenAsync(refreshTokenUid);

                ServiceResponse<User> respUser = await _userService.GetUserByEmailAsync(email);
                User user = respUser.Data;
                if (respCreateUser.Data)
                    user.RefreshTokenUid = refreshTokenUid;

                return user;
            }
            else
            {
                return null;
            }
        }

        private async Task<string> GetUserPrimaryEmailAsync(HttpClient http)
        {
            _logger.LogInformation("Вызван метод GitHubService.GetUserPrimaryEmailAsync()");

            var response = await http.GetFromJsonAsync<List<GitHubUserEmails>>("https://api.github.com/user/emails");
            return response.First(e => e.primary && e.verified).email;
        }
    }
}
