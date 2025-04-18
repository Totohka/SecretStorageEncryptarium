namespace Model.Entities
{
    /// <summary>
    /// Модель для рефреш токена
    /// </summary>
    public class RefreshToken : BaseEntity
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Uid { get; set; }

        /// <summary>
        /// Дата создания токена (дата+время без зоны)
        /// </summary>
        public DateTime DateCreate { get; set; }

        /// <summary>
        /// Дата истечения срока токена (дата+время без зоны)
        /// </summary>
        public DateTime DateExpireToken { get; set; }

        /// <summary>
        /// Дата смерти токена (дата+время без зоны)
        /// </summary>
        public DateTime? DateDeadToken { get; set; }

        /// <summary>
        /// Активен ли токен
        /// </summary>
        public bool IsActive { get; set; } 

        /// <summary>
        /// Список юзеров
        /// </summary>
        public List<User> Users { get; set; }
    }
}
