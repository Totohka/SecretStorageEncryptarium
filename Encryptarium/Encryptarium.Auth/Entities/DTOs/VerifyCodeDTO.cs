namespace Encryptarium.Auth.Entities.DTOs
{
    public class VerifyCodeDTO
    {
        public string Password { get; set; }
        public string Login { get; set; }
        public string Code { get; set; }
    }
}
