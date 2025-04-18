using Model.Entities;

namespace Encryptarium.Auth.Entities.DTOs
{
    public class WhiteListIpDTO
    {
        /// <summary>
        /// Ip адрес устройства, с которого можно использовать ключ api
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// Идентификатор api-ключа 
        /// </summary>
        public Guid ApiKeyUid { get; set; }
    }
}
