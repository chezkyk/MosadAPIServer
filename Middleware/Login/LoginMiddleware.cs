namespace MosadAPIServer.Middleware.Login
{
    public class LoginMiddleware
    {
        private readonly RequestDelegate _next;

        public LoginMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;
            Console.WriteLine("Inside the login middleware");
            await this._next(context);
        }
    }
}
