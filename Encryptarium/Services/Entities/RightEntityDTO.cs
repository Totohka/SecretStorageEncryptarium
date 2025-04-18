using Model.Entities;

namespace BusinessLogic.Entities
{
    public class RightEntityDTO
    {
        public Dictionary<Guid, List<Tuple<RoleDTO, DiscretionaryAccessModelStorage>>> AccessModel { get; set; } = new();
    }
}
