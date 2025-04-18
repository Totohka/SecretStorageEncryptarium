namespace Model.Entities
{
    /// <summary>
    /// Модель для секретных данных
    /// </summary>
    public class Secret : BaseEntity
    {
        /// <summary>
        /// Идентификатор 
        /// </summary>
        public Guid Uid { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Значение секрета
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Дата создания (дата+время без зоны)
        /// </summary>
        public DateTime DateCreate { get; set; }

        /// <summary>
        /// Идентификатор хранилища
        /// </summary>
        public Guid StorageUid { get; set; }

        /// <summary>
        /// Хранилище
        /// </summary>
        public Storage Storage { get; set; }

        /// <summary>
        /// Список связей с политиками доступа и секретами
        /// </summary>
        public List<SecretLinkPolicy> SecretLinkPolicies { get; set; }
    }
}
