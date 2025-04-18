using Model.Entities;

namespace BusinessLogic.Entities
{
    public class SecretLinkResponse
    {
        public Guid SecretUid { get; set; }
        public SecretPolicy SecretPolicy { get; set; }
        public Role Role { get; set; }
    }
}
