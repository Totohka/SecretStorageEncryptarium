namespace Model.Entities
{
    /// <summary>
    /// Модель для хранения Ip-адреса
    /// </summary>
    public class WhiteListIp : BaseEntity
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Uid { get; set; }

        /// <summary>
        /// Ip адрес устройства, с которого можно использовать ключ api
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// Идентификатор api-ключа 
        /// </summary>
        public Guid ApiKeyUid { get; set; }

        /// <summary>
        /// API-ключ, которым можно воспользоваться с данного IP адреса
        /// </summary>
        public ApiKey ApiKey { get; set; }

        /// <summary>
        /// Активен ли ключ для данного Ip
        /// </summary>
        public bool IsActive { get; set; }
    }
}
