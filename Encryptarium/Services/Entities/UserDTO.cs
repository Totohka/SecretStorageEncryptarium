using Model.Entities;

namespace BusinessLogic.Entities
{
    public class UserDTO
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
        /// Логин
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Дата создания учётки
        /// </summary>
        public DateTime DateCreate { get; set; }

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
        /// Активная ли учётка
        /// </summary>
        public bool IsActive { get; set; }
    }
}
