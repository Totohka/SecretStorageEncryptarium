using Model.Entities;

namespace BusinessLogic.Entities
{
    public class StorageLinkResponse
    {
        public Storage Storage { get; set; }
        public StoragePolicy StoragePolicy { get; set; }
        public Role Role { get; set; }
    }
}
