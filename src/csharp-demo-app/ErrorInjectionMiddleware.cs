using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace csharp_demo_app
{
    public class ErrorInjectionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Random _random = new Random();

        public ErrorInjectionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            const string keySleep = "rs.sleep";
            const string keyStatus = "rs.status";
            const string keyFailurePercentage = "rs.failure";

            var qKeySleep = context.Request.Query[keySleep];
            var qKeyStatus = context.Request.Query[keyStatus];
            var qKeyFailurePercentage = context.Request.Query[keyFailurePercentage];

            if (qKeySleep != StringValues.Empty && int.TryParse(qKeySleep.ToString(), out var keySleepValue))
                await Task.Delay(keySleepValue);

            if (qKeyStatus != StringValues.Empty && int.TryParse(qKeyStatus.ToString(), out var keyStatusValue))
            {
                context.Response.StatusCode = keyStatusValue;
                return;
            }

            if (qKeyFailurePercentage != StringValues.Empty && int.TryParse(qKeyFailurePercentage.ToString(), out var failurePercentage))
            {
                if (_random.Next(100) <= failurePercentage)
                {
                    context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                    return;
                }
            }

            await _next(context);
        }
    }
}