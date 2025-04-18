using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.AdminKeys.Interface
{
    public interface IAdminKeyRepository
    {
        public Task SetAdminKeyAsync(string key, string value);
        public Task SetMasterKeyAsync(string value);
        public Task SetMasterKeyAsync();
        public Task<bool> CheckAdminKeysForMasterKeyAsync();
        public Task RemoveAdminKeyAsync();
    }
}
