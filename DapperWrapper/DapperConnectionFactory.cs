using System.Data.SqlClient;

namespace DapperWrapper
{
    public class DapperConnectionFactory : IDapperConnectionFactory
    {
        private readonly ConnectionStringProvider connectionStringProvider;

        public DapperConnectionFactory(ConnectionStringProvider connectionStringProvider)
        {
            this.connectionStringProvider = connectionStringProvider;
        }

        public IDapperConnection CreateConnection()
        {
            var connection = new DapperConnection(new SqlConnection(connectionStringProvider.ConnectionString));
            connection.Open();
            return connection;
        }
    }
}
