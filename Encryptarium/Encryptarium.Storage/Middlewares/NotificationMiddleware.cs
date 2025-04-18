using BusinessLogic.Entities;
using BusinessLogic.Services.NotificationSenders.Entities;
using BusinessLogic.Services.NotificationSenders.Interface;
using BusinessLogic.Services.Users.Interface;
using BusinessLogic.Utils;
using DAL.Repositories.Secrets.Interface;
using DAL.Repositories.Storages.Interface;
using DAL.Repositories.Users.Interface;
using Model.Entities;
using Model.Enums;

namespace Encryptarium.Storage.Middlewares
{
    public class NotificationMiddleware
    {
        private RequestDelegate _next;

        public NotificationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, 
                                 INotificationSenterService notificationSenterService, 
                                 IUserService userService, 
                                 ISecretRepository secretRepository, 
                                 IStorageRepository storageRepository, 
                                 IUserRepository userRepository)
        {
            await _next(context);
            var entity = context.Items["Entity"] as EntitiesEnum?;
            var entityUid = context.Items["EntityUid"] as Guid?;
            var method = context.Items["Method"] as string;
            var userUid = context.Items["UserUid"] as Guid?;
            var statusCode = context.Items["StatusCode"] as StatusCodeEnum?;
            if (entity is not null && entityUid is not null && method is not null)
            {
                ServiceResponse<UsersByStorageResponseDTO> users = new();
                string subject = "";
                string message = "";
                if (entity == EntitiesEnum.Storage)
                {
                    users = await userService.GetUsersByStorageAsync(entityUid.Value);
                    var storage = await storageRepository.GetAsync(entityUid.Value);
                    string titleStorage = storage is null ? "(сейф удалён)" : storage.Title;
                    message = CreateMessage(entity.Value, method, titleStorage, userUid, statusCode.Value);
                    subject = CreateSubject(entity.Value, method, titleStorage, userUid, statusCode.Value);
                }
                else if (entity == EntitiesEnum.Secret)
                {
                    var secret = await secretRepository.GetAsync(entityUid.Value);
                    users = await userService.GetUsersByStorageAsync(secret.StorageUid);
                    string titleSecret = secret is null ? "(секрет удалён)" : secret.Name;
                    message = CreateMessage(entity.Value, method, titleSecret, userUid, statusCode.Value);
                    subject = CreateSubject(entity.Value, method, titleSecret, userUid, statusCode.Value);
                }

                foreach (var user in users.Data.Users) 
                {
                    // Декорируем 
                    MessageBase messageNotification = new MessageNotification(message);
                    messageNotification = new TelegramMessage(user.Uid, messageNotification);
                    messageNotification = new EmailMessage(user.Email, subject, messageNotification);

                    await notificationSenterService.SendNotification(messageNotification);
                }
            }
        }

        private string CreateSubject(EntitiesEnum entityType, string method, string titleEntity, Guid? userUid, StatusCodeEnum status)
        {
            string action = method == "POST" ? "создать" : method == "PUT" ? "изменить" : "удалить";
            string entity = entityType == EntitiesEnum.Storage ? "сейф" : "секрет";
            if (status == StatusCodeEnum.Error || status == StatusCodeEnum.Warning)
            {
                return $"Пользователь {(userUid is null ? "аноним" : userUid.Value)} попытался {action} {entity} {titleEntity}.";
            }
            else
            {
                return $"Пользователь {(userUid is null ? "аноним" : userUid.Value)} успешно смог {action} {entity} {titleEntity}.";
            }
        }

        private string CreateMessage(EntitiesEnum entityType, string method, string titleEntity, Guid? userUid, StatusCodeEnum status) 
        {
            string action = method == "POST" ? "создать" : method == "PUT" ? "изменить" : "удалить";
            string entity = entityType == EntitiesEnum.Storage ? "сейф" : "секрет";
            if (status == StatusCodeEnum.Error || status == StatusCodeEnum.Warning)
            {
                return $"Метод {method}. Пользователь {(userUid is null ? "аноним" : userUid.Value)} попытался {action} {entity} {titleEntity}. " +
                    $"Статус код операции: {status}.";
            }
            else
            {
                return $"Метод {method}. Пользователь {(userUid is null ? "аноним" : userUid.Value)} успешно смог {action} {entity} {titleEntity}. " +
                    $"Статус код операции: {status}.";
            }
        }
    }
}
