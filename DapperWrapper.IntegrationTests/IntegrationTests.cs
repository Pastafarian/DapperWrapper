using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using DapperWrapper.Polly;
using Xunit;

namespace DapperWrapper.IntegrationTests
{
    public class IntegrationTests
    {
        //DapperWrapperTests 

        private string sqlConnection = "Server=IT02090\\SQL2017;Integrated Security=True";

        private ResilientDapperConnection con;


        public IntegrationTests()
        {
            con = new ResilientDapperConnection(new SqlConnection(sqlConnection), new DefaultSqlRetryPolicy(new MockLogger()));
        }

        [Fact]
        public void Execute()
        {

            con.Execute("LogRunningStoredProcedure", new { }, null, 5, CommandType.StoredProcedure);
        }


        
    }


    public class MockLogger : ILogger
    {
        public void Error(Exception exception, string messageTemplate)
        {
        }
    }
}
