using BusinessLogic.Entities;
using BusinessLogic.Services.Base;
using BusinessLogic.Services.Roles.Interface;
using DAL.Repositories.Roles.Interface;
using DAL.Repositories.RoleTypes.Interface;
using DAL.Repositories.StorageLinkPolicies.Interface;
using DAL.Repositories.StoragePolicies.Interface;
using DAL.Repositories.Storages.Interface;
using DAL.Repositories.UserRoles.Interface;
using Microsoft.Extensions.Logging;
using Model.Entities;
using System.Linq;

namespace BusinessLogic.Services.Roles.Implementation
{
    public class RoleService : BaseService, IRoleService
    {
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IRoleTypeRepository _roleTypeRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IStorageRepository _storageRepository;
        private readonly IStoragePolicyRepository _storagePolicyRepository;
        private readonly IStorageLinkPolicyRepository _storageLinkPolicyRepository;
        public RoleService(IRoleRepository roleRepository,
                           IRoleTypeRepository roleTypeRepository,
                           IStorageRepository storageRepository,
                           IStoragePolicyRepository storagePolicyRepository,
                           IStorageLinkPolicyRepository storageLinkPolicyRepository,
                           IUserRoleRepository userRoleRepository,
                           ILogger<RoleService> logger) : base(logger)
        {
            _userRoleRepository = userRoleRepository;
            _roleTypeRepository = roleTypeRepository;
            _roleRepository = roleRepository;
            _storageRepository = storageRepository;
            _storagePolicyRepository = storagePolicyRepository;
            _storageLinkPolicyRepository = storageLinkPolicyRepository;
        }

        public async Task<ServiceResponse<bool>> CreateRoleAsync(string name, string description, Guid roleTypeUid)
        {
            await _roleRepository.CreateAsync(new Role()  { Name = name, Description = description, RoleTypeUid = roleTypeUid });
            return Ok(true);
        }

        public async Task<ServiceResponse<bool>> AddUserInGroupRole(Guid roleUid, Guid ownerUid, Guid userUid, bool isMain)
        {
            var role = await _roleRepository.GetAsync(roleUid);
            var roleType = await _roleTypeRepository.GetAsync(role.RoleTypeUid);
            if (role is null || roleType.Code != 2)
            {
                return Ok(false);
            }
            var userRoles = await _userRoleRepository.GetAsync(ur => ur.RoleUid == roleUid && ur.UserUid == ownerUid);
            var userRole = userRoles.FirstOrDefault();
            if (userRole is null || !userRole.IsMain)
            {
                return Ok(false);
            }

            var userRoleNew = new UserRole()
            {
                RoleUid = roleUid,
                UserUid = userUid,
                IsMain = isMain
            };

            await _userRoleRepository.CreateAsync(userRoleNew);
            return Ok(true);
        }

        public async Task<ServiceResponse<bool>> ChangeAccessInGroupStorage(Guid storageUid, ChangeAccessForGroupStorageDTO dto)
        {
            var storage = await _storageRepository.GetAsync(storageUid);
            if (storage.IsPersonal || storage.IsCommon)
                return Ok(false);

            var roleTypePersonal = await _roleTypeRepository.GetAsync(rt => rt.Code == 1);
            var roleTypeGroup = await _roleTypeRepository.GetAsync(rt => rt.Code == 2);

            // Связи подключенные к хранилищу
            var slps = await _storageLinkPolicyRepository.GetAsync(slp => slp.StorageUid == storageUid);

            // Роли подключенные к хранилищу
            var roles = await _roleRepository.GetAllAsync(r => slps.Select(slp => slp.RoleUid).Contains(r.Uid));
            //var rolesByUser = await _roleRepository.GetAllAsync(r => r);
            var userRoles = await _userRoleRepository.GetAllAsync(ur => roles.Select(r => r.Uid).Contains(ur.RoleUid));

            var userRolesByUsers = await _userRoleRepository.GetAllAsync(ur => dto.Create.Select(kv => kv.Key).Contains(ur.UserUid) ||
                                                                            dto.Update.Select(kv => kv.Key).Contains(ur.UserUid) ||
                                                                            dto.Delete.Contains(ur.UserUid));
            var rolesPersonalByUsers = await _roleRepository.GetAllAsync(r => userRolesByUsers.Select(ur => ur.RoleUid).Contains(r.Uid) &&
                                                                            r.RoleTypeUid == roleTypePersonal.Uid);
            foreach (var userRightCreate in dto.Create)
            {
                if (userRightCreate.Value.IsGroup)
                {
                    await _userRoleRepository.CreateAsync(new UserRole
                    {
                        UserUid = userRightCreate.Key,
                        RoleUid = roles.First(r => r.RoleTypeUid == roleTypeGroup.Uid).Uid,
                        IsMain = false
                    });
                }
                else
                {
                    var policyUid = await _storagePolicyRepository.CreateAsync(new StoragePolicy
                    {
                        Title = $"{storage.Title} Личная для {userRightCreate.Key}",
                        Description = $"Политика группы для хранилища {storage.Title}",
                        DateCreate = DateTime.UtcNow,
                        IsRead = userRightCreate.Value.IsRead,
                        IsCreate = userRightCreate.Value.IsCreate,
                        IsDelete = userRightCreate.Value.IsDelete,
                        IsUpdate = userRightCreate.Value.IsUpdate,
                        IsCommon = false,
                        IsActive = true,
                    });
                    await _storageLinkPolicyRepository.CreateAsync(new StorageLinkPolicy
                    {
                        StoragePolicyUid = policyUid,
                        StorageUid = storageUid,
                        // Тут персональной роли в roles не будет, нужно искать её по общему списку персональных ролей
                        RoleUid = rolesPersonalByUsers.First(r => userRolesByUsers.Where(ur => ur.UserUid == userRightCreate.Key)
                                                .Select(ur => ur.RoleUid).Contains(r.Uid) && 
                                                r.RoleTypeUid == roleTypePersonal.Uid).Uid
                    });
                }
            }

            foreach (var userRightUpdate in dto.Update)
            {
                if (userRightUpdate.Value.IsGroup)
                {
                    // Раз апдейт на групповую роль, то персональная роль будет в списке
                    var roleUidPersonal = roles.First(r => userRoles.Where(ur => ur.UserUid == userRightUpdate.Key)
                                                .Select(ur => ur.RoleUid).Contains(r.Uid) &&
                                                r.RoleTypeUid == roleTypePersonal.Uid).Uid;
                    var slpPersonal = slps.First(slp => slp.RoleUid == roleUidPersonal);
                    // Удаляем старую политику и все сопуствующие связи
                    await _storagePolicyRepository.DeleteAsync(slpPersonal.StoragePolicyUid);

                    // Подключаем к групповой роли роли
                    await _userRoleRepository.CreateAsync(new UserRole
                    {
                        UserUid = userRightUpdate.Key,
                        RoleUid = roles.First(r => r.RoleTypeUid == roleTypeGroup.Uid).Uid,
                        IsMain = false
                    });
                }
                else
                {
                    // Тут обновление на персональную роль может происходить с групповой, поэтому также нужно искать по общему списку персональных ролей
                    var roleUidPersonal = rolesPersonalByUsers.First(r => userRolesByUsers.Where(ur => ur.UserUid == userRightUpdate.Key)
                                                .Select(ur => ur.RoleUid).Contains(r.Uid) &&
                                                r.RoleTypeUid == roleTypePersonal.Uid).Uid;
                    // Получаем связь для персональной роли, если она есть
                    var slpPersonal = slps.FirstOrDefault(slp => slp.RoleUid == roleUidPersonal);

                    // Если персональной роли нет в связях, значит пользователь подключен к групповой роли
                    if (slpPersonal is null)
                    {
                        var roleGroup = roles.First(r => r.RoleTypeUid == roleTypeGroup.Uid);
                        // Отключаем от групповой роли
                        await _userRoleRepository.DeleteAsync(ur => ur.UserUid == userRightUpdate.Key && ur.RoleUid == roleGroup.Uid);
                        // Создаём политику
                        var policyUid = await _storagePolicyRepository.CreateAsync(new StoragePolicy
                        {
                            Title = $"{storage.Title} Личная для {userRightUpdate.Key}",
                            Description = $"Политика группы для хранилища {storage.Title}",
                            DateCreate = DateTime.UtcNow,
                            IsRead = userRightUpdate.Value.IsRead,
                            IsCreate = userRightUpdate.Value.IsCreate,
                            IsDelete = userRightUpdate.Value.IsDelete,
                            IsUpdate = userRightUpdate.Value.IsUpdate,
                            IsCommon = false,
                            IsActive = true,
                        });
                        // Подключаем политику, персональную роль и хранилище
                        await _storageLinkPolicyRepository.CreateAsync(new StorageLinkPolicy
                        {
                            StoragePolicyUid = policyUid,
                            StorageUid = storageUid,
                            RoleUid = roleUidPersonal
                        });
                    }
                    // Если пользователь подключен к персональной роли
                    else
                    {
                        // Если надо подлючать к групповой роли
                        if (userRightUpdate.Value.IsGroup)
                        {
                            // Удаляем персональную политику
                            await _storagePolicyRepository.DeleteAsync(slpPersonal.StoragePolicyUid);
                            // Подключаем к групповой роли
                            await _userRoleRepository.CreateAsync(new UserRole
                            {
                                UserUid = userRightUpdate.Key,
                                RoleUid = roles.First(r => r.RoleTypeUid == roleTypeGroup.Uid).Uid,
                                IsMain = false,
                            });
                        }
                        // Если уже была персональная политика и надо её менять
                        else 
                        {
                            // Получаем персональную политику
                            var policyPersonal = await _storagePolicyRepository.GetAsync(slpPersonal.StoragePolicyUid);

                            // Меняем персональную политику
                            policyPersonal.IsCreate = userRightUpdate.Value.IsCreate;
                            policyPersonal.IsRead = userRightUpdate.Value.IsRead;
                            policyPersonal.IsUpdate = userRightUpdate.Value.IsUpdate;
                            policyPersonal.IsDelete = userRightUpdate.Value.IsDelete;

                            // Обновляем персональную политику 
                            await _storagePolicyRepository.UpdateAsync(policyPersonal);
                        }
                    }
                }
            }

            foreach(var userUid in dto.Delete)
            {
                // Персональной роли может не быть в подключенных, нужно также искать в общем списке персональных ролей
                var roleUidPersonal = rolesPersonalByUsers.First(r => userRolesByUsers.Where(ur => ur.UserUid == userUid)
                        .Select(ur => ur.RoleUid).Contains(r.Uid) &&
                        r.RoleTypeUid == roleTypePersonal.Uid).Uid;
                // Получаем связь для персональной роли, если она есть
                var slpPersonal = slps.FirstOrDefault(slp => slp.RoleUid == roleUidPersonal);
                // Если нет персональная связь, значит пользователь подключен к групповой роли
                if (slpPersonal is null)
                {
                    var roleGroup = roles.First(r => r.RoleTypeUid == roleTypeGroup.Uid);
                    // Отключаем пользователя от роли
                    await _userRoleRepository.DeleteAsync(ur => ur.UserUid == userUid && ur.RoleUid == roleGroup.Uid);
                }
                // Если персональной связи есть
                else
                {
                    // Отключаем роль от связи
                    await _userRoleRepository.DeleteAsync(ur => ur.UserUid == userUid && ur.RoleUid == slpPersonal.RoleUid);
                }
            }

            return Ok(true);
        }

        public async Task<ServiceResponse<bool>> RemoveUserInGroupRole(Guid roleUid, Guid ownerUid, Guid userUid)
        {
            var role = await _roleRepository.GetAsync(roleUid);
            var roleType = await _roleTypeRepository.GetAsync(role.RoleTypeUid);
            if (role is null || roleType.Code != 2)
            {
                return Ok(false);
            }
            var userRoles = await _userRoleRepository.GetAsync(ur => ur.RoleUid == roleUid && ur.UserUid == ownerUid);
            var userRole = userRoles.FirstOrDefault();
            if (userRole is null || !userRole.IsMain)
            {
                return Ok(false);
            }

            await _userRoleRepository.DeleteAsync(ur => ur.UserUid == userUid && ur.RoleUid == roleUid);
            return Ok(true);
        }

        public async Task<ServiceResponse<Guid>> CreateGroupRoleAndStorage(GroupRoleDTO groupRoleDTO, Guid userUid)
        {
            #region CreateGroup
            // групповое хранилище
            var storage = new Storage()
            {
                Title = groupRoleDTO.StorageTitle,
                DateCreate = DateTime.UtcNow,
                Description = groupRoleDTO.StorageDescription,
            };
            // групповая политика
            var storagePolicy = new StoragePolicy()
            {
                Title = groupRoleDTO.StorageTitle,
                Description = $"Политика группы для хранилища {groupRoleDTO.StorageTitle}",
                DateCreate = DateTime.UtcNow,
                IsCreate = false,
                IsRead = true,
                IsDelete = false,
                IsUpdate = false
            };
            var roleTypeGroup = await _roleTypeRepository.GetAsync(rt => rt.Code == 2);
            var roleTypePersonal = await _roleTypeRepository.GetAsync(rt => rt.Code == 1);
            // групповая роль
            var role = new Role()
            {
                Name = groupRoleDTO.RoleName,
                Description = groupRoleDTO.RoleDescription,
                RoleTypeUid = roleTypeGroup.Uid
            };

            var storageUid = await _storageRepository.CreateAsync(storage);
            var storagePolicyUid = await _storagePolicyRepository.CreateAsync(storagePolicy);
            var roleUid = await _roleRepository.CreateAsync(role);

            await _storageLinkPolicyRepository.CreateAsync(new StorageLinkPolicy
            {
                StorageUid = storageUid,
                StoragePolicyUid = storagePolicyUid,
                RoleUid = roleUid,
                IsActive = true
            });
            #endregion

            var myUserRoles = await _userRoleRepository.GetAsync(ur => ur.UserUid == userUid);
            var myUserRoleUids = myUserRoles.Select(ur => ur.RoleUid);
            var myRolePersonal = await _roleRepository.GetAsync(r => r.RoleTypeUid == roleTypePersonal.Uid && myUserRoleUids.Contains(r.Uid));
            var myStoragePolicy = new StoragePolicy()
            {
                Title = groupRoleDTO.StorageTitle,
                Description = $"Политика создателя для хранилища {groupRoleDTO.StorageTitle}",
                DateCreate = DateTime.UtcNow,
                IsCreate = true,
                IsRead = true,
                IsDelete = true,
                IsUpdate = true
            };
            var myStoragePolicyUid = await _storagePolicyRepository.CreateAsync(myStoragePolicy);
            await _storageLinkPolicyRepository.CreateAsync(new StorageLinkPolicy
            {
                StorageUid = storageUid,
                StoragePolicyUid = myStoragePolicyUid,
                RoleUid = myRolePersonal.Uid,
                IsActive = true
            });
            foreach (var rightUser in groupRoleDTO.RightUsers)
            {
                // Подключаем к групповой роли
                if (rightUser.Value.IsGroup)
                {
                    await _userRoleRepository.CreateAsync(new UserRole
                    {
                        UserUid = rightUser.Key,
                        RoleUid = roleUid,
                        IsMain = false,
                    });
                }
                else
                {
                    // Подключаем персональные роли
                    var policyUidForUser = await _storagePolicyRepository.CreateAsync(new StoragePolicy
                    {
                        Title = groupRoleDTO.StorageTitle + $" (Личная для {rightUser.Key})",
                        Description = $"Политика группы для хранилища {groupRoleDTO.StorageTitle}",
                        DateCreate = DateTime.UtcNow,
                        IsCreate = rightUser.Value.IsCreate,
                        IsRead = rightUser.Value.IsRead,
                        IsDelete = rightUser.Value.IsDelete,
                        IsUpdate = rightUser.Value.IsUpdate
                    });
                    var userRoles = await _userRoleRepository.GetAsync(ur => ur.UserUid == rightUser.Key);
                    var userRoleUids = userRoles.Select(ur => ur.RoleUid);
                    var rolePersonal = await _roleRepository.GetAsync(r => r.RoleTypeUid == roleTypePersonal.Uid && userRoleUids.Contains(r.Uid));
                    
                    await _storageLinkPolicyRepository.CreateAsync(new StorageLinkPolicy
                    {
                        StorageUid = storageUid,
                        StoragePolicyUid = policyUidForUser,
                        RoleUid = rolePersonal.Uid,
                        IsActive = true
                    });
                }
            }

            return Ok(storageUid);
        }

        public async Task<ServiceResponse<bool>> DeactivateRoleAsync(Guid uid)
        {
            Role role = await _roleRepository.GetAsync(uid);
            role.IsActive = false;
            await _roleRepository.UpdateAsync(role);
            return Ok(true);
        }

        public async Task<ServiceResponse<bool>> DeleteRoleAsync(Guid uid)
        {
            await _roleRepository.DeleteAsync(uid);
            return Ok(true);
        }

        public async Task<ServiceResponse<List<Role>>> GetAllRoleAsync(string? name)
        {
            if (name is null)
                return Ok(await _roleRepository.GetAllAsync());
            return Ok(await _roleRepository.GetAllAsync(r => r.Name == name));
        }

        public async Task<ServiceResponse<Role>> GetRoleAsync(Guid uid)
        {
            return Ok(await _roleRepository.GetAsync(uid));
        }

        public async Task<ServiceResponse<bool>> UpdateRoleAsync(Guid uid, string name, string description)
        {
            Role role = await _roleRepository.GetAsync(uid);
            role.Description = description;
            role.Name = name;
            await _roleRepository.UpdateAsync(role);
            return Ok(true);
        }
    }
}
