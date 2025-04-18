namespace Model.Entities
{
    /// <summary>
    /// Модель политики доступа к хранилищу
    /// </summary>
    public class StoragePolicy : BaseEntity
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
        /// Разрешает ли политика читать хранилище
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// Разрешается ли создавать новые секреты внутри хранилища
        /// </summary>
        public bool IsCreate { get; set; }

        /// <summary>
        /// Разрешает ли политика редактировать хранилище
        /// </summary>
        public bool IsUpdate { get; set; }

        /// <summary>
        /// Разрешает ли политика удалять хранилище
        /// </summary>
        public bool IsDelete { get; set; }

        /// <summary>
        /// Общая ли политика (применяется для контейнеров организации) 
        /// </summary>
        public bool IsCommon { get; set; }

        /// <summary>
        /// Активна ли политика доступа
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Список связей с политиками доступа и хранилищем
        /// </summary>
        public List<StorageLinkPolicy> StorageLinkPolicies { get; set; }
    }
}
