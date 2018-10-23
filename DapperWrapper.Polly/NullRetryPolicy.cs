using System;
using System.Threading;
using System.Threading.Tasks;

namespace DapperWrapper.Polly
{
    public class NullRetryPolicy : IRetryPolicy
    {
        public void Execute(Action operation)
        {
            operation();
        }

        public TResult Execute<TResult>(Func<TResult> operation)
        {
            return operation();
        }

        public async Task Execute(Func<Task> operation, CancellationToken cancellationToken)
        {
            await operation();
        }

        public async Task<TResult> Execute<TResult>(Func<Task<TResult>> operation, CancellationToken cancellationToken)
        {
            return await operation();
        }
    }
}