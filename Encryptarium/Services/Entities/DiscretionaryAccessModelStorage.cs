namespace BusinessLogic.Entities
{
    public class DiscretionaryAccessModelStorage
    {
        /// <summary>
        /// Разрешает ли политика читать хранилище
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// Разрешается ли создавать новые секреты внутри хранилища
        /// </summary>
        public bool? IsCreate { get; set; }

        /// <summary>
        /// Разрешает ли политика редактировать хранилище
        /// </summary>
        public bool IsUpdate { get; set; }

        /// <summary>
        /// Разрешает ли политика удалять хранилище
        /// </summary>
        public bool IsDelete { get; set; }
    }
}
