namespace BusinessLogic.Entities
{
    public class ChangeAccessForGroupStorageDTO
    {
        public Dictionary<Guid, ChangeRightDTO> Create { get; set; } = new Dictionary<Guid, ChangeRightDTO>();
        public Dictionary<Guid, ChangeRightDTO> Update { get; set; } = new Dictionary<Guid, ChangeRightDTO>();
        public List<Guid> Delete { get; set; } = new List<Guid>();
    }
    public class ChangeRightDTO
    {
        public bool IsGroup { get; set; }
        public bool IsCreate { get; set; }
        public bool IsRead { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsDelete { get; set; }
    }
}
