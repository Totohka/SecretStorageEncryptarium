using BusinessLogic.Entities;
using Model.Entities;

namespace BusinessLogic.Services.Policies.Interface
{
    public interface ISecretPolicyService
    {
        public Task<ServiceResponse<Guid>> CreateSecretPolicyAsync(string title, string description, DiscretionaryAccessModelSecret discretionaryAccessModelSecret);
        public Task<ServiceResponse<bool>> CreateLinkAsync(Guid roleUid, Guid secretPolicyUid, Guid secretUid);
        public Task<ServiceResponse<SecretPolicy>> GetSecretPolicyAsync(Guid uid);
        public Task<ServiceResponse<List<SecretPolicy>>> GetAllSecretPolicyAsync();
        public Task<ServiceResponse<SecretLinkResponse>> GetLinkAsync(Guid roleUid, Guid secretUid);
        public Task<ServiceResponse<bool>> UpdateSecretPolicyAsync(Guid uid, string title, string description);
        public Task<ServiceResponse<bool>> UpdateRigthAccessSecretPolicyAsync(Guid uid, DiscretionaryAccessModelSecret discretionaryAccessModelSecret);
        public Task<ServiceResponse<bool>> DeactivateSecretPolicyAsync(Guid uid);
        public Task<ServiceResponse<bool>> DeleteSecretPolicyAsync(Guid uid);
        public Task<ServiceResponse<bool>> DeleteLinkAsync(Guid roleUid, Guid secretUid);
        public Task<ServiceResponse<RightUserForSecretDTO>> GetRightUserForSecret(Guid userUid, Guid secretUid);
        public Task<ServiceResponse<RightEntityDTO>> GetRightSecretAsync(Guid userUid);
    }
}
