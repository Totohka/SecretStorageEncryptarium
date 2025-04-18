namespace BusinessLogic.Entities
{
    public class ResponseUserDTOs
    {
        public List<UserDTO> Users { get; set; }
        public int CurrentPage { get; set; }
        public int Count { get; set; }
    }
}
