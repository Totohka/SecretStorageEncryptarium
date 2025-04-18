using Model.Enums;

namespace Model.Entities
{
    /// <summary>
    /// Модель для записи аудита
    /// </summary>
    public class Audit : BaseEntity
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Uid { get; set; }

        /// <summary>
        /// Пользователь совершивший событие
        /// </summary>
        public Guid? UserUid { get; set; }

        /// <summary>
        /// Юзер
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Источник микросервис
        /// </summary>
        public MicroservicesEnum SourceMicroservice { get; set; }

        /// <summary>
        /// Источник контроллер
        /// </summary>
        public ControllersEnum SourceController { get; set; }

        /// <summary>
        /// Uid сущности над которой совершались действия, если была
        /// </summary>
        public Guid? EntityUid { get; set; }

        /// <summary>
        /// Над какой сущностью работали
        /// </summary>
        public EntitiesEnum Entity { get; set; }

        /// <summary>
        /// Метод источника
        /// </summary>
        public string SourceMethod { get; set; }
        
        /// <summary>
        /// Нужны ли права админа в источнике
        /// </summary>
        public bool IsSourceRightAdmin { get; set; }

        /// <summary>
        /// Политики безопасности в источнике
        /// </summary>
        public AuthorizePoliciesEnum AuthorizePolice { get; set; }

        /// <summary>
        /// Событие
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Статус действия
        /// </summary>
        public StatusCodeEnum StatusCode { get; set; }
        
        /// <summary>
        /// Дата события (дата+время без зоны)
        /// </summary>
        public DateTime DateAct { get; set; }
    }
}
