using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DapperWrapper.Polly;
using Microsoft.Extensions.Configuration;
using Serilog;
using SimpleInjector;
using SimpleInjector.Advanced;
using SimpleInjector.Lifestyles;
using Tpr.Logging;
using TPR.Logging.Serilog;
using ILogger = DapperWrapper.Polly.ILogger;

namespace Run
{
    class Program
    {
        public static Container BuildContainer()
        {
            return new Container
            {
                Options =
                {
                    DefaultLifestyle = Lifestyle.Scoped,
                    DefaultScopedLifestyle = new AsyncScopedLifestyle() ,
                    ConstructorResolutionBehavior = new LeastGreedyConstructorBehavior()
                }
            };
        }

        public class LeastGreedyConstructorBehavior : IConstructorResolutionBehavior
        {
            public ConstructorInfo GetConstructor(Type implementationType) => (
                    from ctor in implementationType.GetConstructors()
                    orderby ctor.GetParameters().Length
                    select ctor)
                .First();
        }
   
        static void Main(string[] args)
        {
            var container = BuildContainer();
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true);

            var configuration = builder.Build();
            var loggerSettings = configuration.GetSection("Logging").Get<LoggerSettings>();

            container.Register(() => loggerSettings, Lifestyle.Singleton);
            container.RegisterConditional(
                typeof(ITprLogger),
                c => typeof(TprLogger<>).MakeGenericType(typeof(Runner)),
                Lifestyle.Singleton,
                c => true);

            container.Verify();

            var runner = new Runner();

            var tprLogger = container.GetInstance<ITprLogger>();
            runner.Run(tprLogger);

            Console.WriteLine("Finished program");
            Console.ReadLine();
        }
    }

    public class Runner
    {
        private const string SqlConnection = "Server=IT02090\\SQL2017;Integrated Security=True;Database=DapperWrapperTests";

        private ITprLogger tprLogger;
        private static ResilientDapperConnection con;


        public async void Run(ITprLogger logger)
        {
            tprLogger = logger;

            con = new ResilientDapperConnection(new SqlConnection(SqlConnection), new DefaultSqlRetryPolicy(new RetryLogger(tprLogger)));

            await RunTests();
        }

        public async Task<int> RunTests()
        {

            TryLog(() => con.Execute("[dbo].[LogRunningStoredProcedure]", new { }, null, 5, CommandType.StoredProcedure));
            await TryLog(() => con.ExecuteAsync("[dbo].[LogRunningStoredProcedure]", new { }, null, 5, CommandType.StoredProcedure));
            Console.WriteLine("Finished running");

            return 1;
        }


        public int TryLog(Func<int> func)
        {
            try
            {
                return func();
            }
            catch (Exception e)
            {
                tprLogger.Error(e, string.Empty);
                Console.WriteLine(e.Message);
                return -1;
            }
        }

        public async Task<T> TryLog<T>(Func<Task<T>> func)
        {
            try
            {
                return await func();
            }
            catch (Exception e)
            {
                tprLogger.Error(e, string.Empty);
                Console.WriteLine(e.Message);
                return default(T);
            }
        }

    }


    public class RetryLogger : ILogger
    {
        private readonly ITprLogger tprLogger;

        public RetryLogger(ITprLogger tprLogger)
        {
            this.tprLogger = tprLogger;
        }

        public void Error(Exception exception, string messageTemplate)
        {
            tprLogger.Error(exception, messageTemplate);
            Console.WriteLine(exception.Message + " " + messageTemplate);
        }
    }
}
