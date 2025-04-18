namespace BusinessLogic.Entities
{
    public class StorageDTOResponse
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
    }
}
