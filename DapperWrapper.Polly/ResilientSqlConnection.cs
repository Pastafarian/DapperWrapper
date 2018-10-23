using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace DapperWrapper.Polly
{
    public class ResilientSqlConnection : DbConnection
    {
        private readonly SqlConnection underlyingConnection;
        private readonly IRetryPolicy retryPolicy;

        private string connectionString;

        public ResilientSqlConnection(string connectionString, IRetryPolicy retryPolicy)
        {
            this.connectionString = connectionString;
            this.retryPolicy = retryPolicy;
            underlyingConnection = new SqlConnection(connectionString);
        }

        public override string ConnectionString
        {
            get => connectionString;

            set
            {
                connectionString = value;
                underlyingConnection.ConnectionString = value;
            }
        }

        public override string Database => underlyingConnection.Database;

        public override string DataSource => underlyingConnection.DataSource;

        public override string ServerVersion => underlyingConnection.ServerVersion;

        public override ConnectionState State => underlyingConnection.State;

        public override void ChangeDatabase(string databaseName)
        {
            underlyingConnection.ChangeDatabase(databaseName);
        }

        public override void Close()
        {
            underlyingConnection.Close();
        }

        public override void Open()
        {
            retryPolicy.Execute(() =>
            {
                if (underlyingConnection.State != ConnectionState.Open)
                {
                    underlyingConnection.Open();
                }
            });
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return underlyingConnection.BeginTransaction(isolationLevel);
        }

        protected override DbCommand CreateDbCommand()
        {
            return underlyingConnection.CreateCommand();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (underlyingConnection.State == ConnectionState.Open)
                {
                    underlyingConnection.Close();
                }

                underlyingConnection.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}