using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace csharp_demo_app
{
    public class ErrorInjectionMiddleware
    {
        private const string KeySleep = "rs.sleep";
        private const string KeyStatus = "rs.status";
        
        private readonly RequestDelegate _next;

        public ErrorInjectionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var keySleep = context.Request.Query[KeySleep];
            var keyStatus = context.Request.Query[KeyStatus];

            if (keySleep != StringValues.Empty && int.TryParse(keySleep.ToString(), out var keySleepValue))
                await Task.Delay(keySleepValue);

            if (keyStatus != StringValues.Empty && int.TryParse(keyStatus.ToString(), out var keyStatusValue))
            {
                context.Response.StatusCode = keyStatusValue;
                return;
            }
            
            await _next(context);
        }
    }
}