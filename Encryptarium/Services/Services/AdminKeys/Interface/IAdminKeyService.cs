using mtanksl.ShamirSecretSharing;

namespace BusinessLogic.Services.AdminKeys.Interface
{
    public interface IAdminKeyService
    {
        public Task RemoveAdminKeyAsync();
        public Task<bool> SetAdminKeyAsync(string value);
        public Task<List<string>> CreateAdminKeysAsync();
    }
}
