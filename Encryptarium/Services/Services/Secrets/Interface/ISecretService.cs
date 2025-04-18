using BusinessLogic.Entities;
using Model.Entities;

namespace BusinessLogic.Services.Secrets.Interface
{
    public interface ISecretService
    {
        public Task<ServiceResponse<Guid>> CreateSecretAsync(Guid userUid, SecretDTO secretDTO);
        public Task<ServiceResponse<bool>> DeleteSecretAsync(Guid secretUid);
        public Task<ServiceResponse<bool>> UpdateSecretAsync(Guid secretUid, SecretDTO secretDTO);
        public Task<ServiceResponse<Secret>> GetSecretAsync(Guid secretUid);
        public Task<ServiceResponse<List<Secret>>> GetAllSecretAsync(Guid storageUid);
    }
}
