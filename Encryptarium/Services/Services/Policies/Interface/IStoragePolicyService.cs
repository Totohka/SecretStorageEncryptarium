using BusinessLogic.Entities;
using Model.Entities;

namespace BusinessLogic.Services.Policies.Interface
{
    public interface IStoragePolicyService
    {
        public Task<ServiceResponse<Guid>> CreateStoragePolicyAsync(string title, string description, DiscretionaryAccessModelStorage discretionaryAccessModelStorage) ;
        public Task<ServiceResponse<bool>> CreateLinkAsync(Guid roleUid, Guid storagePolicyUid, Guid storageUid);
        public Task<ServiceResponse<StoragePolicy>> GetStoragePolicyAsync(Guid uid);
        public Task<ServiceResponse<List<StoragePolicy>>> GetAllStoragePolicyAsync();
        public Task<ServiceResponse<StorageLinkResponse>> GetLinkAsync(Guid roleUid, Guid storageUid);
        public Task<ServiceResponse<bool>> UpdateStoragePolicyAsync(Guid uid, string title, string description);
        public Task<ServiceResponse<bool>> UpdateRigthAccessStoragePolicyAsync(Guid uid, DiscretionaryAccessModelStorage discretionaryAccessModelStorage);
        public Task<ServiceResponse<bool>> DeactivateStoragePolicyAsync(Guid uid);
        public Task<ServiceResponse<bool>> DeleteStoragePolicyAsync(Guid uid);
        public Task<ServiceResponse<bool>> DeleteLinkAsync(Guid roleUid, Guid storagePolicyUid, Guid storageUid);
        //public Task<ServiceResponse<RightStorageDTO>> GetRightStorageAsync(Guid userUid, Guid storageUid);
        public Task<ServiceResponse<RightEntityDTO>> GetRightStorageAsync(Guid userUid);
        public Task<ServiceResponse<RightUserForStorageDTO>> GetRightUserForStorage(Guid userUid, Guid storageUid);
    }
}
