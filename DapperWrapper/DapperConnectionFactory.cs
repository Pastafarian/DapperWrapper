namespace DapperWrapper
{
    public class DapperConnectionFactory : IDapperConnectionFactory
    {
        private readonly IConnectionStringProvider connectionStringProvider;

        public DapperConnectionFactory(IConnectionStringProvider connectionStringProvider)
        {
            this.connectionStringProvider = connectionStringProvider;
        }

        public IDapperConnection CreateConnection()
        {
            var connection = new DapperConnection(connectionStringProvider.ConnectionString);
            connection.Open();
            return connection;
        }
    }
}
