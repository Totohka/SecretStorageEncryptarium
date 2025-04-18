namespace BusinessLogic.Entities
{
    public class DiscretionaryAccessModelSecret
    {
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
    }
}
