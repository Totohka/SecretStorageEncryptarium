using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Encryptarium.Access.Entities.DTOs
{
    public class UpdateRoleDTO
    {
        public Guid Uid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
