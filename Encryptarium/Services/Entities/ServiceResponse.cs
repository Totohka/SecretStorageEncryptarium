using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Entities
{
    public class ServiceResponse<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public List<string> Errors { get; set; }

        public ServiceResponse(T data, bool isSuccess = true)
        {
            IsSuccess = isSuccess;
            Data = data;
        }

        public ServiceResponse() //только для десериализации
        {
        }

        public ServiceResponse(string error)
        {
            IsSuccess = false;
            Errors = new List<string>() { error };
        }

        public ServiceResponse(IEnumerable<string> errors)
        {
            IsSuccess = false;
            Errors = errors.ToList();
        }
    }
}
