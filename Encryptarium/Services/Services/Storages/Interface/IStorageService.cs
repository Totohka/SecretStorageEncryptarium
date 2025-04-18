using BusinessLogic.Entities;
using Model.Entities;

namespace BusinessLogic.Services.Storages.Interface
{
    public interface IStorageService
    {
        public Task<ServiceResponse<Guid>> CreateStorage(Guid userUid, string title, string description);
        public Task<ServiceResponse<bool>> DeleteStorage(Guid storageUid);
        public Task<ServiceResponse<Storage>> GetStorage(Guid storageUid);
        public Task<ServiceResponse<Guid>> GetStorageUserAsync(Guid userUid);
        public Task<ServiceResponse<Guid>> GetStorageCommonAsync();
        public Task<ServiceResponse<bool>> UpdateStorage(Guid storageUid, string title, string description);
        public Task<ServiceResponse<List<Storage>>> GetAllStorage(Guid userUid, int code);
        public Task<ServiceResponse<bool>> DeactivatedStorage(Guid storageUid);
    }
}
