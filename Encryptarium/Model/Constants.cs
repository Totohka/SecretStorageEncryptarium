namespace Model
{
    public static class Constants
    {
        public const string AccessToken = "AccessToken";
        public const string RefreshToken = "RefreshToken";
        public const string ClaimUserUid = "Uid";
        public const string ClaimUserRole = "Role";
        public const string PrefixApiKey = "ENC-";
        public const int NumberOfSecureBytesToGenerate = 32;
        public const int LengthOfKey = 32;
        public const string MasterKeyId = "MasterKeyId";
        public const string Admin = "Admin";
        public const string TokenPolicy = "TokenPolicy";
        public const string StoragePolicy = "StoragePolicy";
        public const string SecretPolicy = "SecretPolicy";
        public const string UserPolicy = "UserPolicy";
        public const string PrefixLog = "Вызван метод";
        public const string ActionMonitoring = "Пользователь {userUid} в {now} попытался получить доступ к " +
                                                "{microservice}.{controller}.{method} " +
                                                "c политикой доступа {policy}. Было произведено действие над сущностью {entity}" +
                                                "c uid равным {entityUid}" +
                                                " Результат: {status}.";
        public const int Take = 10;
    }
}
