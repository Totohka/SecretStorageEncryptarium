using DAL.Repositories.Users.Interface;
using Model.Entities;
using BusinessLogic.Services.Users.Interface;
using BusinessLogic.Entities;
using BusinessLogic.Services.Base;
using Microsoft.Extensions.Logging;
using DAL.Repositories.Roles.Interface;
using DAL.Repositories.UserRoles.Interface;
using DAL.Repositories.RoleTypes.Interface;
using DAL.Repositories.StoragePolicies.Interface;
using DAL.Repositories.StorageLinkPolicies.Interface;
using DAL.Repositories.Storages.Interface;
using System.Linq.Expressions;
using Model;
using System.Reflection.Metadata;

namespace BusinessLogic.Services.Users.Implementation
{
    public class UserService : BaseService, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IRoleTypeRepository _roleTypeRepository;
        private readonly IStoragePolicyRepository _storagePolicyRepository;
        private readonly IStorageLinkPolicyRepository _storageLinkPolicyRepository;
        private readonly IStorageRepository _storageRepository;
        public UserService(IUserRepository userRepository,
                           IRoleRepository roleRepository,
                           IUserRoleRepository userRoleRepository,
                           IRoleTypeRepository roleTypeRepository,
                           IStoragePolicyRepository storagePolicyRepository,
                           IStorageLinkPolicyRepository storageLinkPolicyRepository,
                           IStorageRepository storageRepository,
                           ILogger<UserService> logger) : base(logger)
        { 
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _roleTypeRepository = roleTypeRepository;
            _userRoleRepository = userRoleRepository;
            _storagePolicyRepository = storagePolicyRepository;
            _storageLinkPolicyRepository = storageLinkPolicyRepository;
            _storageRepository = storageRepository;
        }

        public async Task<ServiceResponse<bool>> ChangeEmailAsync(Guid uid, string email)
        {
            try
            {
                var userWithEmail = await _userRepository.GetAsync(u => u.Email == email);
                if (userWithEmail is not null)
                    return Ok(false);
                var user = await _userRepository.GetAsync(uid);
                user.Email = email;
                await _userRepository.UpdateAsync(user);
                return Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Ok(false);
            }
        }

        public async Task<ServiceResponse<bool>> ChangeLoginAsync(Guid uid, string login)
        {
            try
            {
                var userWithLogin = await _userRepository.GetAsync(u => u.Login == login);
                if (userWithLogin is not null)
                    return Ok(false);
                var user = await _userRepository.GetAsync(uid);
                user.Login = login;
                await _userRepository.UpdateAsync(user);
                return Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Ok(false);
            }
        }

        private async Task<bool> CheckExistsUser(string login, string email, bool isOAuth)
        {
            if (isOAuth)
            {
                User user = await _userRepository.GetAsync(u => u.Email == email && u.Login == login);
                if (user is null)
                    return false;
                return true;
            }
            else
            {
                User user = await _userRepository.GetAsync(u => u.Email == email || u.Login == login);
                if (user is null)
                    return false;
                return true;
            }
        }

        #region Create

        public async Task<ServiceResponse<bool>> CreateUserPassAsync(string login, string email, string password, Guid refreshTokenUid)
        {
            _logger.LogInformation("Вызван метод UserService.CreateUserPassAsync()");
            bool isExistsUser = await CheckExistsUser(login, email, false);
            if (!isExistsUser)
            {
                var user = new User()
                {
                    Login = login,
                    Email = email,
                    PasswordHash = password,
                    IsUserPass = true,
                    IsOAuth = false,
                    IsApiKey = false,
                    IsActive = true,
                    IsAdmin = false,
                    Code2FA = "000000",
                    IsActiveCode = false,
                    IsCreateStorage = false,
                    DateCreate = DateTime.UtcNow,
                    RefreshTokenUid = refreshTokenUid
                };

                Guid userUid = await _userRepository.CreateAsync(user);

                // Создаём личную роль зарегистрировшегося пользователя
                Guid roleUid = await CreatePersonalRole(userUid, login);
                await CreatePersonalStorageAndPolicy(roleUid, login);
                await AddUserWithCommonStoragesAndPolicies(roleUid);
                return Ok(true);
            }
            _logger.LogError("Метод UserService.CreateUserPassAsync(). Пользователь уже существует");
            return Error("Пользователь уже существует");
        }

        public async Task<ServiceResponse<bool>> CreateUserOAuthAsync(GitHubUser gitHubUser, Guid refreshTokenUid)
        {
            _logger.LogInformation("Вызван метод UserService.CreateUserOAuthAsync()");

            bool isExistsUser = await CheckExistsUser(gitHubUser.login, gitHubUser.email, true);
            if (!isExistsUser)
            {
                var user = new User()
                {
                    Login = gitHubUser.login,
                    Email = gitHubUser.email,
                    PasswordHash = "",
                    IsUserPass = false,
                    IsOAuth = true,
                    IsApiKey = false,
                    IsActive = true,
                    IsAdmin = false,
                    Code2FA = "000000",
                    IsActiveCode = false,
                    IsCreateStorage = false,
                    DateCreate = DateTime.UtcNow,
                    RefreshTokenUid = refreshTokenUid
                };

                Guid userUid = await _userRepository.CreateAsync(user);

                // Создаём личную роль зарегистрировшегося пользователя
                Guid roleUid = await CreatePersonalRole(userUid, gitHubUser.login);
                await CreatePersonalStorageAndPolicy(roleUid, gitHubUser.login);
                await AddUserWithCommonStoragesAndPolicies(roleUid);
                return Ok(true);
            }
            else
            {
                User user = await _userRepository.GetAsync(u => u.Email == gitHubUser.email && u.Login == gitHubUser.login);
                user.IsOAuth = true;
                await _userRepository.UpdateAsync(user);
                return Ok(false);
            }
        }

        #endregion

        #region Get

        public async Task<ServiceResponse<UsersByStorageResponseDTO>> GetUsersByStorageAsync(Guid storageUid)
        {
            _logger.LogInformation("Вызван метод UserService.GetUsersByStorageAsync()");

            var slps = await _storageLinkPolicyRepository.GetAllAsync(slp => slp.StorageUid == storageUid);
            var roleUids = slps.Select(slp => slp.RoleUid).Distinct();
            var userRoles = await _userRoleRepository.GetAllAsync(ur => roleUids.Contains(ur.RoleUid));
            var userUids = userRoles.Select(ur => ur.UserUid).Distinct();
            var users = await _userRepository.GetAllAsync(u => userUids.Contains(u.Uid));

            var userDTOs = new List<UserDTO>();
            var rightDTOs = new List<RightForGroupDTO>();
            foreach (var user in users)
            {
                var userRolesForUser = await _userRoleRepository.GetAllAsync(ur => ur.UserUid == user.Uid);
                var slpsForUser = await _storageLinkPolicyRepository.GetAllAsync(slp => userRolesForUser.Select(ur => ur.RoleUid).Contains(slp.RoleUid) && slp.StorageUid == storageUid);
                var policies = await _storagePolicyRepository.GetAllAsync(sp => slpsForUser.Select(slp => slp.StoragePolicyUid).Contains(sp.Uid));
                var roles = await _roleRepository.GetAllAsync(r => slpsForUser.Select(slp => slp.RoleUid).Contains(r.Uid));
                bool isGroup = false;
                if (roles.Count == 1)
                {
                    var roleType = await _roleTypeRepository.GetAsync(roles.First().RoleTypeUid);
                    if (roleType.Code == 1)
                        isGroup = false;
                    if (roleType.Code == 2)
                        isGroup = true;
                    if (roleType.Code == 3)
                        throw new ApplicationException();
                }

                var right = new RightForGroupDTO()
                {
                    IsGroup = isGroup,
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
                rightDTOs.Add(right);
                userDTOs.Add(new UserDTO
                {
                    Login = user.Login,
                    Email = user.Email,
                    DateCreate = user.DateCreate,
                    IsActive = user.IsActive,
                    IsAdmin = user.IsAdmin,
                    IsCreateGroupRole = user.IsCreateGroupRole,
                    IsCreateStorage = user.IsCreateStorage,
                    Uid = user.Uid
                });
            }
            return Ok(new UsersByStorageResponseDTO { Users = userDTOs, Rights = rightDTOs });
        }

        public async Task<ServiceResponse<User>> GetUserByUidAsync(Guid uid)
        {
            _logger.LogInformation("Вызван метод UserService.GetUserByUidAsync()");

            return Ok(await _userRepository.GetAsync(uid));
        }

        public async Task<ServiceResponse<User>> GetUserPassAsync(string login, string password)
        {
            _logger.LogInformation("Вызван метод UserService.GetUserPassAsync()");

            // нужно переделать с хешированием
            User user = await _userRepository.GetAsync(u => u.Login == login && u.PasswordHash == password); 
            return Ok(user);
        }

        public async Task<ServiceResponse<User>> GetUserByEmailAsync(string email)
        {
            _logger.LogInformation("Вызван метод UserService.GetUserByEmailAsync()");

            User user = await _userRepository.GetAsync(u => u.Email == email);
            return Ok(user);
        }

        public async Task<ServiceResponse<List<User>>> GetAllUsersAsync()
        {
            _logger.LogInformation("Вызван метод UserService.GetAllUsersAsync()");

            return Ok(await _userRepository.GetAllAsync());
        }

        public async Task<ServiceResponse<ResponseUserDTOs>> GetAllUsersAsync(Guid myUid, FilterUser filterUser, bool isRemoveMe = true)
        {
            _logger.LogInformation("Вызван метод UserService.GetAllUsersAsync()");
            Expression<Func<User, bool>> lambda;

            var argument = Expression.Parameter(typeof(User), "user");

            var conditions = new List<Expression>();

            if (filterUser.Login is not null)
            {
                var property = Expression.Property(argument, nameof(User.Login));
                var value = Expression.Constant(filterUser.Login);
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var condition = Expression.Call(property, containsMethod, value);
                conditions.Add(condition);
            }
            
            if (filterUser.Email is not null)
            {
                var property = Expression.Property(argument, nameof(User.Email));
                var value = Expression.Constant(filterUser.Email);
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var condition = Expression.Call(property, containsMethod, value);
                conditions.Add(condition);
            }
            List<User> users = null;
            if (conditions.Count > 0)
            {
                var body = conditions.Aggregate(Expression.Or);
                lambda = Expression.Lambda<Func<User, bool>>(body, argument);
                users = await _userRepository.GetAllAsync(lambda);
            }
            else
            {
                users = await _userRepository.GetAllAsync();
            }

            if (users.FirstOrDefault(u => u.Uid == myUid) is not null && isRemoveMe)
            {
                users.Remove(users.First(u => u.Uid == myUid));
            }
            int countPage = users.Count() / Constants.Take;
            countPage += users.Count() % Constants.Take == 0 ? 0 : 1;
            users = users.Skip(filterUser.Skip).Take(filterUser.Take).ToList();
            var userDTOs = new List<UserDTO>();
            foreach (var user in users)
            {
                userDTOs.Add(new UserDTO()
                {
                    Uid = user.Uid,
                    Login = user.Login,
                    DateCreate = DateTime.Now,
                    Email = user.Email,
                    IsActive = user.IsActive,
                    IsAdmin = user.IsAdmin,
                    IsCreateGroupRole = user.IsCreateGroupRole,
                    IsCreateStorage = user.IsCreateStorage
                });
            }
            var responseUserDTOs = new ResponseUserDTOs()
            {
                Users = userDTOs,
                Count = countPage,
                CurrentPage = filterUser.Take / Constants.Take
            };

            return Ok(responseUserDTOs);
        }

        #endregion

        public async Task<ServiceResponse<bool>> SetCode2FAAsync(Guid userUid, string code)
        {
            _logger.LogInformation("Вызван метод UserService.SetCode2FAAsync()");

            User user = await _userRepository.GetAsync(userUid);
            user.Code2FA = code;
            user.IsActiveCode = true;
            await _userRepository.UpdateAsync(user);
            return Ok(true);
        }

        public async Task<ServiceResponse<bool>> UpdateRefreshTokenByUserAsync(Guid userUid, Guid refreshToken)
        {
            _logger.LogInformation("Вызван метод UserService.UpdateRefreshTokenByUserAsync()");

            User user = await _userRepository.GetAsync(userUid);
            user.RefreshTokenUid = refreshToken;
            await _userRepository.UpdateAsync(user);
            return Ok(true);
        }

        public async Task<ServiceResponse<bool>> DeactivateUserAsync(Guid userUid)
        {
            _logger.LogInformation("Вызван метод UserService.DeactivateUserAsync()");

            User user = await _userRepository.GetAsync(userUid);
            user.IsActive = false;
            await _userRepository.UpdateAsync(user);
            return Ok(true);
        }

        public async Task<ServiceResponse<bool>> DeleteUserAsync(Guid userUid)
        {
            _logger.LogInformation("Вызван метод UserService.DeleteUserAsync()");

            await _userRepository.DeleteAsync(userUid);
            return Ok(true);
        }

        public async Task<ServiceResponse<bool>> ChangeAuthMethodAsync(Guid userUid, ChangeMethodAuthDTO changeMethodAuthDTO)
        {
            _logger.LogInformation("Вызван метод UserService.ChangeAuthMethodAsync()");

            User user = await _userRepository.GetAsync(userUid);

            if (changeMethodAuthDTO.IsApiKey is not null)
                user.IsApiKey = (bool)changeMethodAuthDTO.IsApiKey;

            if (changeMethodAuthDTO.IsOAuth is not null)
                user.IsOAuth = (bool)changeMethodAuthDTO.IsOAuth;

            if (changeMethodAuthDTO.IsUserPass is not null)
                user.IsUserPass = (bool)changeMethodAuthDTO.IsUserPass;

            await _userRepository.UpdateAsync(user);
            return Ok(true);
        }

        public async Task<ServiceResponse<bool>> SetIsCreateAsync(Guid userUid, bool isCreate)
        {
            var user = await _userRepository.GetAsync(userUid);
            user.IsCreateStorage = isCreate;
            await _userRepository.UpdateAsync(user);
            return Ok(true);
        }

        public async Task<ServiceResponse<bool>> SetIsAdminAsync(Guid userUid, bool isAdmin)
        {
            var user = await _userRepository.GetAsync(userUid);
            user.IsAdmin = isAdmin;
            await _userRepository.UpdateAsync(user);
            return Ok(true);
        }
        public async Task<ServiceResponse<bool>> LinkTelegramNotificationAsync(string message, long chatId)
        {
            var data = message.Split(' ');
            if (data.Length != 3)
                return Ok(false);
            string login = data[1];
            string password = data[2];
            var user = await _userRepository.GetAsync(u => u.Login == login && u.PasswordHash == password);
            if (user is null)
                return Ok(false);
            user.ChatId = chatId;
            await _userRepository.UpdateAsync(user);
            return Ok(true);
        }
        #region CreateUserMethods
        private async Task<Guid> CreatePersonalRole(Guid userUid, string login)
        {
            RoleType roleType = await _roleTypeRepository.GetAsync(rt => rt.Code == 1); // Получаем личный тип ролей
            Role role = new Role()
            {
                Name = $"Личная роль пользователя {login}",
                Description = "Личная роль пользователя, созданная автоматически.",
                RoleTypeUid = roleType.Uid,
                IsActive = true
            };
            Guid roleUid = await _roleRepository.CreateAsync(role);
            UserRole userRole = new UserRole()
            {
                RoleUid = roleUid,
                UserUid = userUid,
                IsMain = true
            };
            await _userRoleRepository.CreateAsync(userRole);
            return roleUid;
        }

        private async Task CreatePersonalStorageAndPolicy(Guid roleUid, string login)
        {
            Storage storage = new Storage()
            {
                Title = $"Личное хранилище для {login}",
                Description = "Хранилище созданное автоматически.",
                IsPersonal = true,
                IsActive = true,
                DateCreate = DateTime.UtcNow
            };
            StoragePolicy storagePolicy = new StoragePolicy()
            {
                Title = $"Политика доступа к личному хранилищу {login}.",
                Description = $"Политика созданная автоматически.",
                DateCreate = DateTime.UtcNow,
                IsCreate = true,
                IsRead = true,
                IsUpdate = true,
                IsDelete = true,
                IsActive = true
            };

            Guid storagePolicyUid = await _storagePolicyRepository.CreateAsync(storagePolicy);
            Guid storageUid = await _storageRepository.CreateAsync(storage);
            StorageLinkPolicy storageLinkPolicy = new StorageLinkPolicy()
            {
                RoleUid = roleUid,
                StoragePolicyUid = storagePolicyUid,
                StorageUid = storageUid,
                IsActive = true
            };
            await _storageLinkPolicyRepository.CreateAsync(storageLinkPolicy);
        }

        private async Task AddUserWithCommonStoragesAndPolicies(Guid roleUid)
        {
            List<Storage> storages = await _storageRepository.GetAllAsync(s => s.IsCommon);
            StoragePolicy storagePolicy = await _storagePolicyRepository.GetAsync(sp => sp.IsCommon); // по задумке должна быть всегда одна
            foreach (Storage storage in storages)
            {
                await _storageLinkPolicyRepository.CreateAsync(new StorageLinkPolicy()
                {
                    StorageUid = storage.Uid,
                    RoleUid = roleUid,
                    StoragePolicyUid = storagePolicy.Uid,
                    IsActive = true
                });
            }
        }

        #endregion
    }
}
