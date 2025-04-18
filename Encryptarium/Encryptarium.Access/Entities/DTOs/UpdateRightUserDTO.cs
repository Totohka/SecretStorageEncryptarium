namespace Encryptarium.Access.Entities.DTOs
{
    public class UpdateRightUserDTO
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public Guid Uid { get; set; }

        /// <summary>
        /// Может ли пользователь создавать групповые роли
        /// </summary>
        public string IsCreateGroupRole { get; set; }

        /// <summary>
        /// Включён ли вход через логин-пароль
        /// </summary>
        public bool IsUserPass { get; set; }

        /// <summary>
        /// Включен ли вход через API-Key
        /// </summary>
        public bool IsApiKey { get; set; }

        /// <summary>
        /// Включён ли вход через OAuth
        /// </summary>
        public bool IsOAuth { get; set; }

        /// <summary>
        /// Является ли юзер админом
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// Может ли учётка создавать новые хранилища
        /// </summary>
        public bool IsCreateStorage { get; set; }
    }
}
