namespace Encryptarium.Storage.Middlewares
{
    public class CorsMiddleware
    {
        private readonly RequestDelegate _next;

        public CorsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Вызов следующего middleware
            await _next(context);

            // Добавление заголовков CORS к ответу
            if (context.Response.StatusCode == 401 || context.Response.StatusCode == 500)
            {
                context.Response.Headers.Add("Access-Control-Allow-Origin", "https://localhost:3000"); // Или укажите конкретный домен
                context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
                context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            }
        }
    }
}
