using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Polly;

namespace DapperWrapper.Polly
{
    // Retry a specified number of times, using a function to 
    // calculate the duration to wait between retries based on 
    // the current retry attempt (allows for exponential back-off)
    // In this case will wait for
    //  2 ^ 1 = 2 seconds then
    //  2 ^ 2 = 4 seconds then
    //  2 ^ 3 = 8 seconds etc...
    public class SlidingSqlRetryPolicy : IRetryPolicy
    {
        private const int TimeOutError = -2;
        private const int NetworkError = 53;
        private const int TransportLevelError = 121;
        private const int RetryCount = 6;

        private readonly int[] sqlExceptions = { NetworkError, TimeOutError, TransportLevelError };
        private readonly Policy retryPolicy;

        public SlidingSqlRetryPolicy(ILogger logger, RetryOptions options)
        {
            retryPolicy = Policy
                .Handle<SqlException>(exception => sqlExceptions.Contains(exception.Number))
                .WaitAndRetry(options.RetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (exception, timeSpan, retryCount, context) =>
                {
                    logger.Error(exception, $"Database call failed. Waiting {timeSpan} before next retry. Retry attempt {retryCount}");
                });
        }

        public void Execute(Action operation)
        {
            retryPolicy.Execute(operation.Invoke);
        }

        public TResult Execute<TResult>(Func<TResult> operation)
        {
            return retryPolicy.Execute(operation.Invoke);
        }

        public async Task Execute(Func<Task> operation, CancellationToken cancellationToken)
        {
            await retryPolicy.ExecuteAsync(operation.Invoke);
        }

        public async Task<TResult> Execute<TResult>(Func<Task<TResult>> operation, CancellationToken cancellationToken)
        {
            return await retryPolicy.ExecuteAsync(operation.Invoke);
        }
    }
}