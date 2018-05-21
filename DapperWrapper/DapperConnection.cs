using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;

namespace DapperWrapper
{
    public class DapperConnection : IDapperConnection
    {
        private bool disposed;
        private readonly IDbConnection connection;

        public DapperConnection(string connectionString)
        {
            connection = new SqlConnection(connectionString);
        }

        public void Open()
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }

        public int Execute(string sql, object param = null, CommandType? commandType = default(CommandType?))
        {
            return Run(() => connection.Execute(sql, param, null, default(int?), commandType), sql, param);
        }

        public Task<int> ExecuteAsync(string sql, object param = null, CommandType? commandType = default(CommandType?))
        {
            return Run(()=> connection.ExecuteAsync(sql, param, null, default(int?), commandType), sql, param);
        }

        public IEnumerable<dynamic> Query(string sql, object param = null, CommandType? commandType = default(CommandType?), int? commandTimeout = null)
        {
            return Run(() => connection.Query(sql, param, null, true, commandTimeout, commandType), sql, param);
        }

        public Task<IEnumerable<dynamic>> QueryAsync(string sql, object param = null, CommandType? commandType = default(CommandType?), int? commandTimeout = null)
        {
            return Run(() => connection.QueryAsync(sql, param, null, commandTimeout, commandType), sql, param);
        }

        public IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, object param, CommandType? commandType, Func<TFirst, TSecond, TThird, TFourth, TReturn> func, string splitOn = "id")
        {
            return Run(() => connection.Query(sql, func, param, splitOn: "id", commandType: commandType), sql, param);
        }

        public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, object param, CommandType? commandType, Func<TFirst, TSecond, TThird, TFourth, TReturn> func, string splitOn = "id")
        {
            return Run(() => connection.QueryAsync(sql, func, param, splitOn: "id", commandType: commandType), sql, param);
        }

        public IEnumerable<T> Query<T>(string sql, object param = null, CommandType? commandType = default(CommandType?), int? commandTimeout = null)
        {
            return Run(() => connection.Query<T>(sql, param, null, true, commandTimeout, commandType), sql, param);
        }

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, CommandType? commandType = default(CommandType?), int? commandTimeout = null)
        {
            return Run(() => connection.QueryAsync<T>(sql, param, null, commandTimeout, commandType), sql, param);
        }

        public Task<T> QuerySingleAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Run(() => connection.QuerySingleAsync<T>(sql, param, transaction, commandTimeout, commandType), sql, param);
        }

        public Task<T> QueryFirstAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Run(() => connection.QueryFirstAsync<T>(sql, param, transaction, commandTimeout, commandType), sql, param);
        }

        public Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Run(() => connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction, commandTimeout, commandType), sql, param);
        }

        public IGridReader QueryMultiple(string sql, object param = null, CommandType? commandType = default(CommandType?))
        {
            return Run(() => new GridReader(connection.QueryMultiple(sql, param, null, default(int), commandType)), sql, param);
        }

        public Task<IGridReader> QueryMultipleAsync(string sql, object param = null, CommandType? commandType = default(CommandType?))
        {
            return GeneralizeTask<IGridReader, GridReader>(Run(async () => new GridReader(await connection.QueryMultipleAsync(sql, param, null, default(int), commandType)), sql, param));
        }

        public void EnlistTransaction(Transaction transation)
        {
            ((SqlConnection)connection).EnlistTransaction(transation);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Dispose();
                }
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private static T Run<T>(Func<T> func, string sql, object param)
        {
            try
            {
                return func();
            }
            catch (Exception e)
            {
                throw BuildException(e, sql, param);
            }
        }

        private static async Task<TBase> GeneralizeTask<TBase, TDerived>(Task<TDerived> task) where TDerived : TBase
        {
            return await task;
        }

        private static Exception BuildException(Exception baseException, string sql, object param)
        {
            var message = new StringBuilder("Error executing dapper query for sql '" + sql + "'. ");

            if (param == null)
            {
                message.Append("Parameters object null.");
            }
            else
            {
                var props = param.GetType().GetProperties();

                if (props.Length == 0)
                {
                    message.Append("Parameters object empty.");
                }
                else
                {
                    message.Append("Parameter values: ");
                    foreach (var p in props)
                    {
                        var name = p.Name;
                        var objValue = p.GetValue(param, null);
                        var value = objValue?.ToString() ?? "null";

                        message.Append("Name: " + name + ", value: " + value + ". ");
                    }
                }
            }

            return new Exception(message.ToString(), baseException);
        }
    }
}
