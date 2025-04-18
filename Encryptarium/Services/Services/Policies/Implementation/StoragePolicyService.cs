using BusinessLogic.Entities;
using BusinessLogic.Services.Base;
using BusinessLogic.Services.Policies.Interface;
using DAL.Repositories.Roles.Interface;
using DAL.Repositories.RoleTypes.Interface;
using DAL.Repositories.StorageLinkPolicies.Interface;
using DAL.Repositories.StoragePolicies.Interface;
using DAL.Repositories.Storages.Interface;
using DAL.Repositories.UserRoles.Interface;
using DAL.Repositories.Users.Interface;
using Microsoft.Extensions.Logging;
using Model.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BusinessLogic.Services.Policies.Implementation
{
    public class StoragePolicyService : BaseService, IStoragePolicyService
    {
        private readonly IStoragePolicyRepository _storagePolicyRepository;
        private readonly IStorageLinkPolicyRepository _storageLinkPolicyRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IStorageRepository _storageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IRoleTypeRepository _roleTypeRepository;

        public StoragePolicyService(IStorageLinkPolicyRepository storageLinkPolicyRepository, 
                                    IStoragePolicyRepository storagePolicyRepository,
                                    IRoleRepository roleRepository,
                                    IStorageRepository storageRepository,
                                    IUserRepository userRepository,
                                    IUserRoleRepository userRoleRepository,
                                    IRoleTypeRepository roleTypeRepository,
                                    ILogger<StoragePolicyService> logger) : base(logger)
        {
            _storageLinkPolicyRepository = storageLinkPolicyRepository;
            _storagePolicyRepository = storagePolicyRepository; 
            _roleRepository = roleRepository;
            _storageRepository = storageRepository;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _roleTypeRepository = roleTypeRepository;
        }

        public async Task<ServiceResponse<bool>> CreateLinkAsync(Guid roleUid, Guid storagePolicyUid, Guid storageUid)
        {
            await _storageLinkPolicyRepository.CreateAsync(new StorageLinkPolicy { RoleUid = roleUid, StorageUid = storageUid, StoragePolicyUid = storagePolicyUid });
            return Ok(true);
        }

        public async Task<ServiceResponse<Guid>> CreateStoragePolicyAsync(string title, string description, DiscretionaryAccessModelStorage discretionaryAccessModelStorage)
        {
            var storagePolicy = new StoragePolicy()
            {
                Title = title,
                Description = description,
                DateCreate = DateTime.UtcNow,
                IsCreate = (bool)discretionaryAccessModelStorage.IsCreate,
                IsDelete = discretionaryAccessModelStorage.IsDelete,
                IsRead = discretionaryAccessModelStorage.IsRead,
                IsUpdate = discretionaryAccessModelStorage.IsUpdate
            };
            return Ok(await _storagePolicyRepository.CreateAsync(storagePolicy));
        }

        public async Task<ServiceResponse<bool>> DeactivateStoragePolicyAsync(Guid uid)
        {
            StoragePolicy storagePolicy = await _storagePolicyRepository.GetAsync(uid);
            storagePolicy.IsActive = false;
            await _storagePolicyRepository.UpdateAsync(storagePolicy);
            return Ok(true);
        }

        public async Task<ServiceResponse<bool>> DeleteLinkAsync(Guid roleUid, Guid storagePolicyUid, Guid storageUid)
        {
            await _storageLinkPolicyRepository.DeleteAsync(storageUid, roleUid);
            return Ok(true);
        }

        public async Task<ServiceResponse<bool>> DeleteStoragePolicyAsync(Guid uid)
        {
            await _storagePolicyRepository.DeleteAsync(uid);
            return Ok(true);
        }

        public async Task<ServiceResponse<List<StoragePolicy>>> GetAllStoragePolicyAsync()
        {
            return Ok(await _storagePolicyRepository.GetAllAsync());
        }

        public async Task<ServiceResponse<StorageLinkResponse>> GetLinkAsync(Guid roleUid, Guid storageUid)
        {
            var linkStorage = await _storageLinkPolicyRepository.GetAsync(storageUid, roleUid);
            var linkResponse = new StorageLinkResponse()
            {
                Role = await _roleRepository.GetAsync(roleUid),
                Storage = await _storageRepository.GetAsync(storageUid),
                StoragePolicy = await _storagePolicyRepository.GetAsync(linkStorage.StoragePolicyUid)
            };
            return Ok(linkResponse);
        }

        public async Task<ServiceResponse<StoragePolicy>> GetStoragePolicyAsync(Guid uid)
        {
            return Ok(await _storagePolicyRepository.GetAsync(uid));
        }

        public async Task<ServiceResponse<bool>> UpdateRigthAccessStoragePolicyAsync(Guid uid, DiscretionaryAccessModelStorage discretionaryAccessModelStorage)
        {
            StoragePolicy storagePolicy = await _storagePolicyRepository.GetAsync(uid);
            storagePolicy.IsRead = discretionaryAccessModelStorage.IsRead;
            storagePolicy.IsDelete = discretionaryAccessModelStorage.IsDelete;
            storagePolicy.IsCreate = (bool)discretionaryAccessModelStorage.IsCreate;
            storagePolicy.IsUpdate = discretionaryAccessModelStorage.IsUpdate;
            await _storagePolicyRepository.UpdateAsync(storagePolicy);
            return Ok(true);
        }

        public async Task<ServiceResponse<bool>> UpdateStoragePolicyAsync(Guid uid, string title, string description)
        {
            StoragePolicy storagePolicy = await _storagePolicyRepository.GetAsync(uid);
            storagePolicy.Title = title;
            storagePolicy.Description = description;
            await _storagePolicyRepository.UpdateAsync(storagePolicy);
            return Ok(true);
        }
        public async Task<ServiceResponse<RightUserForStorageDTO>> GetRightUserForStorage(Guid userUid, Guid storageUid)
        {
            var userRoles = await _userRoleRepository.GetAllAsync(ur => ur.UserUid == userUid);
            var slps = await _storageLinkPolicyRepository.GetAllAsync(slp => userRoles.Select(ur => ur.RoleUid).Contains(slp.RoleUid) && slp.StorageUid == storageUid);
            var policies = await _storagePolicyRepository.GetAllAsync(sp => slps.Select(slp => slp.StoragePolicyUid).Contains(sp.Uid));

            var right = new RightUserForStorageDTO()
            {
                IsDelete = false,
                IsRead = false,
                IsUpdate = false,
                IsCreate = false
            };

            foreach (var policy in policies)
            {
                if (policy.IsDelete) right.IsDelete = true;
                if (policy.IsRead) right.IsRead = true;
                if (policy.IsUpdate) right.IsUpdate = true;
                if (policy.IsCreate) right.IsCreate = true;
            }

            return Ok(right);
        }

        public async Task<ServiceResponse<RightEntityDTO>> GetRightStorageAsync(Guid userUid)
        {
            var user = await _userRepository.GetAsync(userUid);
            var userRoles = await _userRoleRepository.GetAllAsync(ur => ur.UserUid == userUid && ur.IsMain);
            var roles = await _roleRepository.GetAllAsync(r => userRoles.Select(ur => ur.RoleUid).Contains(r.Uid));
            var roleDTOs = new List<RoleDTO>();
            // Получение и маппинг ролей
            foreach (var role in roles) {
                var roleType = await _roleTypeRepository.GetAsync(role.RoleTypeUid);
                roleDTOs.Add(new RoleDTO()
                {
                    Uid = role.Uid,
                    Name = role.Name,
                    Description = role.Description,
                    CodeType = roleType.Code,
                    IsActive = role.IsActive
                });
            }
            var storageLinkPolicies = await _storageLinkPolicyRepository.GetAllAsync(slp => roles.Select(r => r.Uid).Contains(slp.RoleUid));
            var storageUids = storageLinkPolicies.Select(slp => slp.StorageUid).Distinct().ToList();

            // Собираем ответ, параллельно получаем разрешения на каждое хранилище для каждой роли
            var result = new RightEntityDTO();
            foreach (var storageUid in storageUids)
            {
                if (!result.AccessModel.ContainsKey(storageUid))
                {
                    result.AccessModel.Add(storageUid, new List<Tuple<RoleDTO, DiscretionaryAccessModelStorage>>());
                }

                // Получаем storageLinkPolicies относящихся к хранилищу и получаем из него нужные данные
                var linkPoliciesForStorage = storageLinkPolicies.Where(slp => slp.StorageUid == storageUid);
                foreach (var linkPolicy in linkPoliciesForStorage)
                {
                    var roleDTO = roleDTOs.First(r => r.Uid == linkPolicy.RoleUid);
                    var policy = await _storagePolicyRepository.GetAsync(linkPolicy.StoragePolicyUid);
                    var right = new DiscretionaryAccessModelStorage()
                    {
                        IsCreate = policy.IsCreate,
                        IsDelete = policy.IsDelete,
                        IsRead = policy.IsRead,
                        IsUpdate = policy.IsUpdate
                    };
                    var tuple = new Tuple<RoleDTO, DiscretionaryAccessModelStorage>(roleDTO, right);
                    result.AccessModel[storageUid].Add(tuple);
                }
            }
            return Ok(result);
        } 
    }
}
