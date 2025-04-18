using BusinessLogic.Entities;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Services.Base
{
    public class BaseService
    {
        private protected readonly ILogger _logger;

        public BaseService(ILogger logger)
        {
            _logger = logger;
        }

        public ServiceResponse<T> Ok<T>(T value)
        {
            return new ServiceResponse<T>(value, true);
        }

        public ServiceResponse<T> Error<T>(string errorMsg)
        {
            return new ServiceResponse<T>(errorMsg);
        }

        public ServiceResponse<T> Error<T>(string[] errorMsgs)
        {
            return new ServiceResponse<T>(errorMsgs);
        }

        public ServiceResponse<bool> Error(string errorMsg)
        {
            return new ServiceResponse<bool>(errorMsg);
        }
    }
}
