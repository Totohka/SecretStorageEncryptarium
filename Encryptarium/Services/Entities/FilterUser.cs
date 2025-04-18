namespace BusinessLogic.Entities
{
    public class FilterUser
    {
        public string? Login { get; set; }
        public string? Email { get; set; }
        public int Take { get; set; } = 10;
        public int Skip { get; set; } = 0;
    }
}
