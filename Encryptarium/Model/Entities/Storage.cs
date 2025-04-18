namespace Model.Entities
{
    /// <summary>
    /// Модель хранилища
    /// </summary>
    public class Storage : BaseEntity
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Uid { get; set; }

        /// <summary>
        /// Название хранилища
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Описание хранилища
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Дата создания хранилища (дата+время без зоны)
        /// </summary>
        public DateTime DateCreate { get; set; }

        /// <summary>
        /// Общее ли хранилище 
        /// </summary>
        public bool IsCommon { get; set; }

        /// <summary>
        /// Личное ли хранилище
        /// </summary>
        public bool IsPersonal { get; set; }

        /// <summary>
        /// Активно хранилище
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Список связей с политиками доступа и хранилищем
        /// </summary>
        public List<StorageLinkPolicy> StorageLinkPolicies { get; set; }
    }
}
