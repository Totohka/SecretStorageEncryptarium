using BusinessLogic.Entities;
using BusinessLogic.Services.Base;
using BusinessLogic.Services.Storages.Interface;
using DAL.Repositories.Roles.Interface;
using DAL.Repositories.RoleTypes.Interface;
using DAL.Repositories.StorageLinkPolicies.Implementation;
using DAL.Repositories.StorageLinkPolicies.Interface;
using DAL.Repositories.StoragePolicies.Interface;
using DAL.Repositories.Storages.Interface;
using DAL.Repositories.UserRoles.Interface;
using Microsoft.Extensions.Logging;
using Model.Entities;
using Model.Enums;

namespace BusinessLogic.Services.Storages.Implementation
{
    public class StorageService : BaseService, IStorageService
    {
        private readonly IStorageRepository _storageRepository;
        private readonly IStorageLinkPolicyRepository _storageLinkPolicyRepository;
        private readonly IStoragePolicyRepository _storagePolicyRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IRoleTypeRepository _roleTypeRepository;

        public StorageService(IStorageRepository storageRepository,
                              IStorageLinkPolicyRepository storageLinkPolicyRepository,
                              IStoragePolicyRepository storagePolicyRepository,
                              IUserRoleRepository userRoleRepository,
                              IRoleRepository roleRepository,
                              IRoleTypeRepository roleTypeRepository,
                              ILogger<StorageService> logger) : base(logger)
        {
            _storageRepository = storageRepository;
            _storageLinkPolicyRepository = storageLinkPolicyRepository;
            _storagePolicyRepository = storagePolicyRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _roleTypeRepository = roleTypeRepository;
        }

        public async Task<ServiceResponse<Guid>> CreateStorage(Guid userUid, string title, string description)
        {
            Storage storage = new Storage()
            {
                Title = title,
                Description = description,
                DateCreate = DateTime.UtcNow,
                
                IsActive = true
            };
            StoragePolicy storagePolicy = new StoragePolicy()
            {
                Title = $"Политика доступа для пользователя {userUid}",
                Description = "Политика доступа, созданная автоматически при создании контейнера.",
                DateCreate = DateTime.UtcNow,
                IsCreate = true,
                IsRead = true,
                IsDelete = true,
                IsUpdate = true,
                IsActive = true
            };

            Guid storageUid = await _storageRepository.CreateAsync(storage);
            Guid storagePolicyUid = await _storagePolicyRepository.CreateAsync(storagePolicy);
            Guid roleUid = Guid.Empty;
            List<UserRole> userRoles = await _userRoleRepository.GetAsync(ur => ur.UserUid == userUid);

            foreach (var userRole in userRoles)
            {
                Role role = await _roleRepository.GetAsync(userRole.RoleUid);
                RoleType roleType = await _roleTypeRepository.GetAsync(role.RoleTypeUid);
                if (roleType.Code == 1)
                {
                    roleUid = role.Uid;
                    break;
                }
            }
            StorageLinkPolicy storageLinkPolicy = new StorageLinkPolicy()
            {
                RoleUid = roleUid,
                StorageUid = storageUid,
                StoragePolicyUid = storagePolicyUid,
            };
            await _storageLinkPolicyRepository.CreateAsync(storageLinkPolicy);
            return Ok(storageUid);
        }

        public async Task<ServiceResponse<bool>> DeleteStorage(Guid storageUid)
        {
            await _storageRepository.DeleteAsync(storageUid);
            return Ok(true);
        }

        public async Task<ServiceResponse<Guid>> GetStorageUserAsync(Guid userUid)
        {
            var userRoles = await _userRoleRepository.GetAllAsync(ur => ur.UserUid == userUid);
            var roles = await _roleRepository.GetAllAsync(r => userRoles.Select(ur => ur.RoleUid).Contains(r.Uid));
            var roleType = await _roleTypeRepository.GetAsync(rt => rt.Code == 1);
            var role = roles.First(r => r.RoleTypeUid == roleType.Uid);
            var storageLinkPolicies = await _storageLinkPolicyRepository.GetAllAsync(slp => slp.RoleUid == role.Uid);
            var storages = await _storageRepository.GetAllAsync(s => storageLinkPolicies.Select(slp => slp.StorageUid).Contains(s.Uid));
            return Ok(storages.First(s => s.IsPersonal).Uid);
        }

        public async Task<ServiceResponse<Guid>> GetStorageCommonAsync()
        {
            var roleType = await _roleTypeRepository.GetAsync(rt => rt.Code == 3);
            var role = await _roleRepository.GetAsync(r => r.RoleTypeUid == roleType.Uid);
            var storageLinkPolicy = await _storageLinkPolicyRepository.GetAsync(slp => slp.RoleUid == role.Uid);
            var storage = await _storageRepository.GetAsync(storageLinkPolicy.First().StorageUid);
            return Ok(storage.Uid);
        }

        public async Task<ServiceResponse<List<Storage>>> GetAllStorage(Guid userUid, int code)
        {
            Guid roleUid = Guid.Empty;
            List<UserRole> userRoles = await _userRoleRepository.GetAllAsync(ur => ur.UserUid == userUid);
            List<Storage> storages = new List<Storage>();
            foreach (var userRole in userRoles)
            {
                Role role = await _roleRepository.GetAsync(userRole.RoleUid);
                List<StorageLinkPolicy> storageLinkPolicies = await _storageLinkPolicyRepository.GetAllAsync();
                storageLinkPolicies = storageLinkPolicies.Where(slp => slp.RoleUid == role.Uid).ToList();
                foreach (var storageLinkPolicy in storageLinkPolicies)
                {
                    StoragePolicy storagePolicy = await _storagePolicyRepository.GetAsync(storageLinkPolicy.StoragePolicyUid);
                    if (storagePolicy.IsRead)
                    {
                        var storage = await _storageRepository.GetAsync(storageLinkPolicy.StorageUid);
                        switch (code)
                        {
                            case (int)TypeStorageEnum.None:
                                storages.Add(storage);
                                break;
                            case (int)TypeStorageEnum.Personal:
                                if (storage.IsPersonal)
                                    storages.Add(storage);
                                break;
                            case (int)TypeStorageEnum.Group:
                                if (!storage.IsPersonal && !storage.IsCommon)
                                    storages.Add(storage);
                                break;
                            case (int)TypeStorageEnum.Common:
                                if (storage.IsCommon)
                                    storages.Add(storage);
                                break;
                        }
                    }
                }
            }
            return Ok(storages);
        }

        public async Task<ServiceResponse<Storage>> GetStorage(Guid storageUid)
        {
            return Ok(await _storageRepository.GetAsync(storageUid));
        }

        public async Task<ServiceResponse<bool>> UpdateStorage(Guid storageUid, string title, string description)
        {
            var storage = await _storageRepository.GetAsync(storageUid);
            storage.Title = title;
            storage.Description = description;
            await _storageRepository.UpdateAsync(storage);
            return Ok(true);
        }

        public async Task<ServiceResponse<bool>> DeactivatedStorage(Guid storageUid)
        {
            var storage = await _storageRepository.GetAsync(storageUid);
            storage.IsActive = false;
            await _storageRepository.UpdateAsync(storage);
            return Ok(true);
        }
    }
}
