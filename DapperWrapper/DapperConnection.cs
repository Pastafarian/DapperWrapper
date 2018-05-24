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

        public ConnectionState State => connection.State;

        public int Execute(string sql, object param = null, IDbTransaction transaction = null, int? timeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            return Run(() => connection.Execute(sql, param, transaction, timeout, commandType), sql, param);
        }

        public Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, int? timeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            return Run(()=> connection.ExecuteAsync(sql, param, transaction, timeout, commandType), sql, param);
        }

        public IEnumerable<T> Query<T>(string sql, object param = null, IDbTransaction transaction = null, bool buffered = true,
            int? commandTimeout = null, CommandType? commandType = null)
        {
            return Run(() => connection.Query<T>(sql, param, transaction, buffered, commandTimeout, commandType), sql, param);
        }

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Run(() => connection.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType), sql, param);
        }

        public IEnumerable<dynamic> Query(string sql, object param = null, IDbTransaction transaction = null, bool buffered = true,
            int? commandTimeout = null, CommandType? commandType = null)
        {
            return Run(() => connection.Query(sql, param, transaction, buffered, commandTimeout, commandType), sql, param);
        }

        public Task<IEnumerable<dynamic>> QueryAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Run(() => connection.QueryAsync(sql, param, transaction, commandTimeout, commandType), sql, param);
        }

        public IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param = null,
            IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Run(() => connection.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType), sql, param);
        }

        public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param = null,
            IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Run(() => connection.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType), sql, param);
        }

        public T QuerySingle<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Run(() => connection.QuerySingle<T>(sql, param, transaction, commandTimeout, commandType), sql, param);
        }

        public Task<T> QuerySingleAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Run(() => connection.QuerySingleAsync<T>(sql, param, transaction, commandTimeout, commandType), sql, param);
        }

        public Task<T> QueryFirstAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Run(() => connection.QueryFirstAsync<T>(sql, param, transaction, commandTimeout, commandType), sql, param);
        }

        public Task<dynamic> QueryFirstOrDefaultAsync(string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null)
        {
            return Run(() => connection.QueryFirstOrDefaultAsync(sql, param, transaction, commandTimeout, commandType), sql, param);
        }

        public Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null)
        {
            return Run(() => connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction, commandTimeout, commandType), sql, param);
        }

        public IGridReader QueryMultiple(string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null)
        {
            return Run(() => new GridReader(connection.QueryMultiple(sql, param, transaction, commandTimeout, commandType)), sql, param);
        }

        public Task<IGridReader> QueryMultipleAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return GeneralizeTask<IGridReader, GridReader>(Run(async () => new GridReader(await connection.QueryMultipleAsync(sql, param, transaction, commandTimeout, commandType)), sql, param));
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
