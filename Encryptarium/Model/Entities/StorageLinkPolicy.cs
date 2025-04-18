namespace Model.Entities
{
    /// <summary>
    /// Модель для связи политики доступа с юзером и хранилищем
    /// </summary>
    public class StorageLinkPolicy : BaseEntity
    {
        /// <summary>
        /// Идентификатор политики доступа
        /// </summary>
        public Guid StoragePolicyUid { get; set; }

        /// <summary>
        /// Политика доступа к хранилищу
        /// </summary>
        public StoragePolicy StoragePolicy { get; set; }

        /// <summary>
        /// Идентификатор хранилища
        /// </summary>
        public Guid StorageUid { get; set; }

        /// <summary>
        /// Хранилище
        /// </summary>
        public Storage Storage { get; set; }

        /// <summary>
        /// Идентификатор роли
        /// </summary>
        public Guid RoleUid { get; set; }

        /// <summary>
        /// Роль
        /// </summary>
        public Role Role { get; set; }
        /// <summary>
        /// Активна ли связь
        /// </summary>
        public bool IsActive { get; set; }
    }
}
