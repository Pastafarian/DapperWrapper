using System;

namespace DapperWrapper.Polly
{
    public interface ILogger
    {
        void Error(Exception exception, string messageTemplate);
    }
}