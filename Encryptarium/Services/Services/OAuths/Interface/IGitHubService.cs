using BusinessLogic.Entities;
using Model.Entities;

namespace BusinessLogic.Services.OAuths.Interface
{
    public interface IGitHubService
    {
        public Task<ServiceResponse<User>> GetUserAsync(string code);  
    }
}
