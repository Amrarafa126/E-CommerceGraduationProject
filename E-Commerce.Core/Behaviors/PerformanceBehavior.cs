using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Behaviors
{
    public class PerformanceBehavior<TRequest, TResponse>(
     ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
     : IPipelineBehavior<TRequest, TResponse>
     where TRequest : notnull
    {
        private const int SlowRequestThresholdMs = 500;

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var response = await next();
            stopwatch.Stop();

            if (stopwatch.ElapsedMilliseconds > SlowRequestThresholdMs)
            {
                logger.LogWarning(
                    "Slow Request: {RequestName} took {ElapsedMs}ms - {@Request}",
                    typeof(TRequest).Name,
                    stopwatch.ElapsedMilliseconds,
                    request);
            }

            return response;
        }
    }
}
