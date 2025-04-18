using BusinessLogic.Entities;
using BusinessLogic.Services.Base;
using BusinessLogic.Services.Secrets.Interface;
using DAL.Repositories.Roles.Implementation;
using DAL.Repositories.Roles.Interface;
using DAL.Repositories.RoleTypes.Interface;
using DAL.Repositories.SecretLinkPolicies.Interface;
using DAL.Repositories.Secrets.Interface;
using DAL.Repositories.StorageLinkPolicies.Interface;
using DAL.Repositories.StoragePolicies.Interface;
using DAL.Repositories.Storages.Implementation;
using DAL.Repositories.Storages.Interface;
using DAL.Repositories.UserRoles.Implementation;
using DAL.Repositories.UserRoles.Interface;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Model;
using Model.Entities;
using System.Security.Cryptography;
using System.Text;

namespace BusinessLogic.Services.Secrets.Implementation
{
    public class SecretService : BaseService, ISecretService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ISecretRepository _secretRepository;
        private readonly IConfiguration _configuration;
        private readonly IStorageLinkPolicyRepository _storageLinkPolicyRepository;
        private readonly IStoragePolicyRepository _storagePolicyRepository;
        private readonly IStorageRepository _storageRepository;
        private readonly ISecretLinkPolicyRepository _secretLinkPolicyRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IRoleTypeRepository _roleTypeRepository;
        public SecretService(IMemoryCache memoryCache,
                             ISecretRepository secretRepository,
                             IConfiguration configuration,
                             ILogger<SecretService> logger) : base(logger)
        {
            _memoryCache = memoryCache;
            _secretRepository = secretRepository;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<Guid>> CreateSecretAsync(Guid userUid, SecretDTO secretDTO)
        {
            //var storage = await _storageRepository.GetAsync(secretDTO.StorageUid);
            var key = _memoryCache.Get(_configuration.GetSection(Constants.MasterKeyId).Value) as string;
            if (secretDTO.Secret is null || secretDTO.StorageUid is null)
            {
                return Error<Guid>("Значение секрета или uid сейфа равно null");
            }
            Secret secret = new Secret()
            {
                DateCreate = DateTime.UtcNow,
                Name = secretDTO.Name,
                Value = Encrypt(secretDTO.Secret),
                StorageUid = (Guid)secretDTO.StorageUid
            };
            return Ok(await _secretRepository.CreateAsync(secret));

            //var userRoles = await _userRoleRepository.GetAllAsync(ur => ur.UserUid == userUid);
            //var userRolesUid = userRoles.Select(ur => ur.RoleUid).ToList();
            //var personalRoleType = await _roleTypeRepository.GetAsync(rt => rt.Code == 1);
            //var personalRole = _roleRepository.GetAsync(r => r.RoleTypeUid == personalRoleType.Uid && userRolesUid.Contains(r.Uid));

            //var links = await _storageLinkPolicyRepository.GetAllAsync(slp => slp.StorageUid == secretDTO.StorageUid);
            //var idRoles = links.Select(slp => slp.RoleUid).ToList();
            //var storagePolicies = await _storagePolicyRepository.GetAllAsync(sp => links.Select(l => l.StoragePolicyUid).Contains(sp.Uid));

            //foreach ( var uid in idRoles)
            //{

            //}
            //var secretLink = new List<SecretLinkPolicy>();

        }

        public async Task<ServiceResponse<bool>> DeleteSecretAsync(Guid secretUid)
        {
            await _secretRepository.DeleteAsync(secretUid);
            return Ok(true);
        }

        public async Task<ServiceResponse<Secret>> GetSecretAsync(Guid secretUid)
        {
            var secret = await _secretRepository.GetAsync(secretUid);
            secret.Value = Decrypt(secret.Value);
            return Ok(secret);
        }

        public async Task<ServiceResponse<List<Secret>>> GetAllSecretAsync(Guid storageUid)
        {
            return Ok(await _secretRepository.GetAllAsync(s => s.StorageUid == storageUid));
        }
        public async Task<ServiceResponse<bool>> UpdateSecretAsync(Guid secretUid, SecretDTO secretDTO)
        {
            var key = _memoryCache.Get(_configuration.GetSection(Constants.MasterKeyId)) as string;
            Secret secret = await _secretRepository.GetAsync(secretUid);
            secret.Name = secretDTO.Name;
            secret.Value = secretDTO.Secret is null ? secret.Value : Encrypt(secretDTO.Secret);
            secret.StorageUid = secretDTO.StorageUid is null ? secret.StorageUid : (Guid)secretDTO.StorageUid;
            await _secretRepository.UpdateAsync(secret);
            return Ok(true);
        }

        #region Crypt
        private string Encrypt(string clearText)
        {
            string EncryptionKey = _memoryCache.Get(_configuration.GetSection(Constants.MasterKeyId).Value) as string;
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new
                    Rfc2898DeriveBytes(EncryptionKey, new byte[]
                    { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        private string Decrypt(string cipherText)
        {
            string EncryptionKey = _memoryCache.Get(_configuration.GetSection(Constants.MasterKeyId).Value) as string;
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new
                    Rfc2898DeriveBytes(EncryptionKey, new byte[]
                    { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        #endregion
    }
}
