namespace DapperWrapper.Polly
{
    public class RetryOptions
    {
        /// <summary>
        /// Defaults to 6
        /// </summary>
        public int RetryCount { get; set; }

        public RetryOptions()
        {
            RetryCount = 6;
        }
    }
}