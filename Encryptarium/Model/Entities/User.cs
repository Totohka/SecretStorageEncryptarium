using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities
{
    /// <summary>
    /// Модель учётки
    /// </summary>
    public class User : BaseEntity
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Uid { get; set; }

        /// <summary>
        /// Электронная почта
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// ID чата в телеграм 
        /// </summary>
        public long? ChatId { get; set; }

        /// <summary>
        /// Логин
        /// </summary>
        public string Login {  get; set; }

        /// <summary>
        /// Хеш пароля
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Дата создания учётки
        /// </summary>
        public DateTime DateCreate { get; set; }

        /// <summary>
        /// Идентификатор рефреш токена
        /// </summary>
        public Guid? RefreshTokenUid { get; set; }

        /// <summary>
        /// Рефреш токен
        /// </summary>
        public RefreshToken RefreshToken { get; set; }

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

        /// <summary>
        /// Может ли учётка создавать групповые роли
        /// </summary>
        public bool IsCreateGroupRole { get; set; }

        /// <summary>
        /// Код для 2FA 
        /// </summary>
        public string Code2FA { get; set; }

        /// <summary>
        /// Активен ли код
        /// </summary>
        public bool IsActiveCode { get; set; }

        /// <summary>
        /// Активная ли учётка
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Список задокументированных действий
        /// </summary>
        public List<Audit> Audits { get; set; }

        /// <summary>
        /// Список Api-ключей, по которым можно авторизироваться
        /// </summary>
        public List<ApiKey> ApiKeys { get; set; }
    }
}
