using BusinessLogic.Entities;
using BusinessLogic.Services.Base;
using BusinessLogic.Services.Policies.Interface;
using BusinessLogic.Services.Roles.Implementation;
using DAL.Repositories.Roles.Interface;
using DAL.Repositories.RoleTypes.Interface;
using DAL.Repositories.SecretLinkPolicies.Interface;
using DAL.Repositories.SecretPolicies.Interface;
using DAL.Repositories.Secrets.Interface;
using DAL.Repositories.StorageLinkPolicies.Interface;
using DAL.Repositories.StoragePolicies.Interface;
using DAL.Repositories.UserRoles.Interface;
using DAL.Repositories.Users.Interface;
using Microsoft.Extensions.Logging;
using Model.Entities;

namespace BusinessLogic.Services.Policies.Implementation
{
    public class SecretPolicyService : BaseService, ISecretPolicyService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IRoleTypeRepository _roleTypeRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISecretRepository _secretRepository;
        private readonly ISecretPolicyRepository _secretPolicyRepository;
        private readonly IStoragePolicyRepository _storagePolicyRepository;
        private readonly ISecretLinkPolicyRepository _secretLinkPolicyRepository;
        private readonly IStorageLinkPolicyRepository _storageLinkPolicyRepository;
        public SecretPolicyService(ISecretLinkPolicyRepository secretLinkPolicyRepository,
                                   IStorageLinkPolicyRepository storageLinkPolicyRepository,
                                   ISecretPolicyRepository secretPolicyRepository,
                                   IStoragePolicyRepository storagePolicyRepository,
                                   IRoleRepository roleRepository,
                                   IRoleTypeRepository roleTypeRepository,
                                   IUserRoleRepository userRoleRepository,
                                   IUserRepository userRepository,
                                   ISecretRepository secretRepository,
                                   ILogger<SecretPolicyService> logger) : base(logger)
        {
            _storageLinkPolicyRepository = storageLinkPolicyRepository;
            _secretRepository = secretRepository;
            _storagePolicyRepository = storagePolicyRepository;
            _secretLinkPolicyRepository = secretLinkPolicyRepository;
            _secretPolicyRepository = secretPolicyRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _userRepository = userRepository;
        }

        public async Task<ServiceResponse<RightEntityDTO>> GetRightSecretAsync(Guid userUid)
        {
            var user = await _userRepository.GetAsync(userUid);
            var userRoles = await _userRoleRepository.GetAllAsync(ur => ur.UserUid == userUid && ur.IsMain);
            var roles = await _roleRepository.GetAllAsync(r => userRoles.Select(ur => ur.RoleUid).Contains(r.Uid));
            var roleDTOs = new List<RoleDTO>();
            // Получение и маппинг ролей
            foreach (var role in roles)
            {
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
            var secretLinkPolicies = await _secretLinkPolicyRepository.GetAllAsync(slp => roles.Select(r => r.Uid).Contains(slp.RoleUid));
            var secretUids = secretLinkPolicies.Select(slp => slp.SecretUid).Distinct().ToList();

            // Собираем ответ, параллельно получаем разрешения на каждое хранилище для каждой роли
            var result = new RightEntityDTO();
            foreach (var secretUid in secretUids)
            {
                if (!result.AccessModel.ContainsKey(secretUid))
                {
                    result.AccessModel.Add(secretUid, new List<Tuple<RoleDTO, DiscretionaryAccessModelStorage>>());
                }

                // Получаем secretLinkPolicies относящихся к хранилищу и получаем из него нужные данные
                var linkPoliciesForStorage = secretLinkPolicies.Where(slp => slp.SecretUid == secretUid);
                foreach (var linkPolicy in linkPoliciesForStorage)
                {
                    var roleDTO = roleDTOs.First(r => r.Uid == linkPolicy.RoleUid);
                    var policy = await _secretPolicyRepository.GetAsync(linkPolicy.SecretPolicyUid);
                    var right = new DiscretionaryAccessModelStorage()
                    {
                        IsCreate = null,
                        IsDelete = policy.IsDelete,
                        IsRead = policy.IsRead,
                        IsUpdate = policy.IsUpdate
                    };
                    var tuple = new Tuple<RoleDTO, DiscretionaryAccessModelStorage>(roleDTO, right);
                    result.AccessModel[secretUid].Add(tuple);
                }
            }
            return Ok(result);
        }

        public async Task<ServiceResponse<RightUserForSecretDTO>> GetRightUserForSecret(Guid userUid, Guid secretUid)
        {
            var userRoles = await _userRoleRepository.GetAllAsync(ur => ur.UserUid == userUid);
            var slps = await _secretLinkPolicyRepository.GetAllAsync(slp => userRoles.Select(ur => ur.RoleUid).Contains(slp.RoleUid) && slp.SecretUid == secretUid);
            List<SecretPolicy> secretPolicies = null;
            List<StoragePolicy> storagePolicies = null;
            if (slps.Count > 0)
                secretPolicies = await _secretPolicyRepository.GetAllAsync(sp => slps.Select(slp => slp.SecretPolicyUid).Contains(sp.Uid));
            else
            {
                var secret = await _secretRepository.GetAsync(secretUid);
                var storagelps = await _storageLinkPolicyRepository.GetAllAsync(slp => userRoles.Select(ur => ur.RoleUid).Contains(slp.RoleUid) && slp.StorageUid == secret.StorageUid);
                storagePolicies = await _storagePolicyRepository.GetAllAsync(sp => storagelps.Select(slp => slp.StoragePolicyUid).Contains(sp.Uid));
            }

            var right = new RightUserForSecretDTO()
            {
                IsDelete = false,
                IsRead = false,
                IsUpdate = false
            };

            if (secretPolicies is not null)
            {
                foreach (var policy in secretPolicies)
                {
                    if (policy.IsDelete) right.IsDelete = true;
                    if (policy.IsRead) right.IsRead = true;
                    if (policy.IsUpdate) right.IsUpdate = true;
                }
            }
            else if (storagePolicies is not null)
            {
                foreach (var policy in storagePolicies)
                {
                    if (policy.IsDelete) right.IsDelete = true;
                    if (policy.IsRead) right.IsRead = true;
                    if (policy.IsUpdate) right.IsUpdate = true;
                }
            }
            
            return Ok(right);
        }

        public async Task<ServiceResponse<bool>> CreateLinkAsync(Guid roleUid, Guid secretPolicyUid, Guid secretUid)
        {
            var link = await _secretLinkPolicyRepository.GetAsync(l => l.RoleUid == roleUid && l.SecretPolicyUid == secretPolicyUid);
            if (link is null)
            {
                await _secretLinkPolicyRepository.CreateAsync(new SecretLinkPolicy { RoleUid = roleUid, SecretUid = secretUid, SecretPolicyUid = secretPolicyUid });
                return Ok(true);
            }
            return Ok(false);
        }

        public async Task<ServiceResponse<Guid>> CreateSecretPolicyAsync(string title, string description, DiscretionaryAccessModelSecret discretionaryAccessModelSecret)
        {
            var secretPolicy = new SecretPolicy()
            {
                Title = title,
                Description = description,
                DateCreate = DateTime.UtcNow,
                IsDelete = discretionaryAccessModelSecret.IsDelete,
                IsRead = discretionaryAccessModelSecret.IsRead,
                IsUpdate = discretionaryAccessModelSecret.IsUpdate
            };
            return Ok(await _secretPolicyRepository.CreateAsync(secretPolicy));
        }

        public async Task<ServiceResponse<bool>> DeactivateSecretPolicyAsync(Guid uid)
        {
            SecretPolicy secretPolicy = await _secretPolicyRepository.GetAsync(uid);
            secretPolicy.IsActive = false;
            await _secretPolicyRepository.UpdateAsync(secretPolicy);
            return Ok(true);
        }

        public async Task<ServiceResponse<bool>> DeleteLinkAsync(Guid roleUid, Guid secretUid)
        {
            await _secretLinkPolicyRepository.DeleteAsync(secretUid, roleUid);
            return Ok(true);
        }

        public async Task<ServiceResponse<bool>> DeleteSecretPolicyAsync(Guid uid)
        {
            await _secretPolicyRepository.DeleteAsync(uid);
            return Ok(true);
        }

        public async Task<ServiceResponse<List<SecretPolicy>>> GetAllSecretPolicyAsync()
        {
            return Ok(await _secretPolicyRepository.GetAllAsync());
        }
        public async Task<ServiceResponse<SecretLinkResponse>> GetLinkAsync(Guid roleUid, Guid secretUid)
        {
            var link = await _secretLinkPolicyRepository.GetAsync(secretUid, roleUid);
            var linkResponse = new SecretLinkResponse()
            {
                Role = await _roleRepository.GetAsync(roleUid),
                SecretUid = secretUid,
                SecretPolicy = await _secretPolicyRepository.GetAsync(link.SecretPolicyUid)
            };
            return Ok(linkResponse);
        }

        public async Task<ServiceResponse<SecretPolicy>> GetSecretPolicyAsync(Guid uid)
        {
            return Ok(await _secretPolicyRepository.GetAsync(uid));
        }

        public async Task<ServiceResponse<bool>> UpdateRigthAccessSecretPolicyAsync(Guid uid, DiscretionaryAccessModelSecret discretionaryAccessModelSecret)
        {
            SecretPolicy secretPolicy = await _secretPolicyRepository.GetAsync(uid);
            secretPolicy.IsRead = discretionaryAccessModelSecret.IsRead;
            secretPolicy.IsDelete = discretionaryAccessModelSecret.IsDelete;
            secretPolicy.IsUpdate = discretionaryAccessModelSecret.IsUpdate;
            await _secretPolicyRepository.UpdateAsync(secretPolicy);
            return Ok(true);
        }

        public async Task<ServiceResponse<bool>> UpdateSecretPolicyAsync(Guid uid, string title, string description)
        {
            SecretPolicy secretPolicy = await _secretPolicyRepository.GetAsync(uid);
            secretPolicy.Title = title;
            secretPolicy.Description = description;
            await _secretPolicyRepository.UpdateAsync(secretPolicy);
            return Ok(true);
        }
    }
}
