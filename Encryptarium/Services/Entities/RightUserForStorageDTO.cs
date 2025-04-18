namespace BusinessLogic.Entities
{
    public class RightUserForStorageDTO
    {
        public bool IsRead { get; set; }
        public bool IsDelete { get; set; }
        public bool IsCreate { get; set; }
        public bool IsUpdate { get; set; }
    }
}
