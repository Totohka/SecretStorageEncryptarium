namespace BusinessLogic.Entities
{
    /// <summary>
    /// DTO для изменения методов авторизации
    /// </summary>
    public class ChangeMethodAuthDTO
    {
        /// <summary>
        /// Авторизация через UserPass
        /// </summary>
        public bool? IsUserPass { get; set; }

        /// <summary>
        /// Авторизация через ApiKey
        /// </summary>
        public bool? IsApiKey { get; set; }

        /// <summary>
        /// Авторизация через OAuth GitHub
        /// </summary>
        public bool? IsOAuth { get; set; }
    }
}
