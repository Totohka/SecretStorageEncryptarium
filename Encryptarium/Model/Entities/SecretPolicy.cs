namespace Model.Entities
{
    /// <summary>
    /// Модель для политики доступа к секрету
    /// </summary>
    public class SecretPolicy : BaseEntity
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Uid { get; set; }

        /// <summary>
        /// Название политики доступа
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Описание политики доступа
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Дата создания политики доступа (дата+время без зоны)
        /// </summary>
        public DateTime DateCreate { get; set; }

        /// <summary>
        /// Можно ли читать этот секрет
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// Можно ли редактировать этот секрет
        /// </summary>
        public bool IsUpdate { get; set; }

        /// <summary>
        /// Можно ли удалить этот секрет
        /// </summary>
        public bool IsDelete { get; set; }

        /// <summary>
        /// Активна ли политика доступа
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Список связей с политиками доступа и секретами
        /// </summary>
        public List<SecretLinkPolicy> SecretLinkPolicies { get; set; }
    }
}
