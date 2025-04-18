namespace Model.Entities
{
    /// <summary>
    /// Модуль для связи политики доступа с юзером и секретом
    /// </summary>
    public class SecretLinkPolicy : BaseEntity
    {

        /// <summary>
        /// Индитификатор политики доступа
        /// </summary>
        public Guid SecretPolicyUid { get; set; }

        /// <summary>
        /// Политика доступа к секрету
        /// </summary>
        public SecretPolicy SecretPolicy { get; set; }

        /// <summary>
        /// Индитификатор секрета
        /// </summary>
        public Guid SecretUid { get; set; }

        /// <summary>
        /// Секрет
        /// </summary>
        public Secret Secret { get; set; }

        /// <summary>
        /// Индитификатор юзера
        /// </summary>
        public Guid RoleUid { get; set; }

        /// <summary>
        /// Юзер
        /// </summary>
        public Role Role { get; set; }

        /// <summary>
        /// Активна ли связь
        /// </summary>
        public bool IsActive { get; set; }
    }
}
