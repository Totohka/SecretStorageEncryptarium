namespace Model.Entities
{
    /// <summary>
    /// Модель для хранения API-ключа
    /// </summary>
    public class ApiKey : BaseEntity
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Uid { get; set; }

        /// <summary>
        /// Название ключа
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Хеш ключа
        /// </summary>
        public string KeyHash { get; set; }

        /// <summary>
        /// С каким юзером(и в последствии с какими правами доступа) ассоциируется ключ
        /// </summary>
        public Guid UserUid { get; set; }

        /// <summary>
        /// Сущность юзера, которому доступен API-ключ
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Активен ли api-ключ
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Список IP адресов, с которых можно воспользоваться API-ключом
        /// </summary>
        public List<WhiteListIp> WhiteListIps { get; set; }
    }
}
