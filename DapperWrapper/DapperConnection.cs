using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;

namespace DapperWrapper
{

    public class DapperConnection : IDapperConnection
    {
        private bool disposed;
        protected readonly IDbConnection Connection;

        public DapperConnection(IDbConnection connection)
        {
            Connection = connection;
        }

        public void Open()
        {
            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }
        }

        public void Close()
        {
            if (Connection.State != ConnectionState.Closed)
            {
                Connection.Close();
            }
        }

        public ConnectionState State => Connection.State;

        public virtual int Execute(string sql, object param = null, IDbTransaction transaction = null, int? timeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            return Run((() => Connection.Execute(sql, param, transaction, timeout, commandType)), sql, param);
        }

        public virtual Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, int? timeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            return Run(() => Connection.ExecuteAsync(sql, param, transaction, timeout, commandType), sql, param);
        }

        public virtual IEnumerable<T> Query<T>(string sql, object param = null, IDbTransaction transaction = null, bool buffered = true,
            int? commandTimeout = null, CommandType? commandType = null)
        {
            return Run(() => Connection.Query<T>(sql, param, transaction, buffered, commandTimeout, commandType), sql, param);
        }

        public virtual Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Run(() => Connection.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType), sql, param);
        }

        public virtual IEnumerable<dynamic> Query(string sql, object param = null, IDbTransaction transaction = null, bool buffered = true,
            int? commandTimeout = null, CommandType? commandType = null)
        {
            return Run(() => Connection.Query(sql, param, transaction, buffered, commandTimeout, commandType), sql, param);
        }

        public virtual Task<IEnumerable<dynamic>> QueryAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Run(() => Connection.QueryAsync(sql, param, transaction, commandTimeout, commandType), sql, param);
        }

        public virtual IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param = null,
            IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Run(() => Connection.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType), sql, param);
        }

        public virtual Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param = null,
            IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Run(() => Connection.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType), sql, param);
        }

        public virtual T QuerySingle<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Run(() => Connection.QuerySingle<T>(sql, param, transaction, commandTimeout, commandType), sql, param);
        }

        public virtual Task<T> QuerySingleAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Run(() => Connection.QuerySingleAsync<T>(sql, param, transaction, commandTimeout, commandType), sql, param);
        }

        public virtual Task<T> QueryFirstAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Run(() => Connection.QueryFirstAsync<T>(sql, param, transaction, commandTimeout, commandType), sql, param);
        }

        public virtual Task<dynamic> QueryFirstOrDefaultAsync(string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null)
        {
            return Run(() => Connection.QueryFirstOrDefaultAsync(sql, param, transaction, commandTimeout, commandType), sql, param);
        }

        public virtual Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null)
        {
            return Run(() => Connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction, commandTimeout, commandType), sql, param);
        }

        public virtual IGridReader QueryMultiple(string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null)
        {
            return Run(() => new GridReader(Connection.QueryMultiple(sql, param, transaction, commandTimeout, commandType)), sql, param);
        }

        public virtual Task<IGridReader> QueryMultipleAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return GeneralizeTask<IGridReader, GridReader>(Run(async () => new GridReader(await Connection.QueryMultipleAsync(sql, param, transaction, commandTimeout, commandType)), sql, param));
        }

        public void EnlistTransaction(Transaction transaction)
        {
            ((SqlConnection)Connection).EnlistTransaction(transaction);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private static async Task<TBase> GeneralizeTask<TBase, TDerived>(Task<TDerived> task) where TDerived : TBase
        {
            return await task;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                if (Connection.State != ConnectionState.Closed)
                {
                    Connection.Dispose();
                }
            }

            disposed = true;
        }

        private static async Task<T> Run<T>(Func<Task<T>> func, string sql, object param)
        {
            try
            {
                return await func();
            }
            catch (Exception e)
            {
                throw ExceptionBuilder.BuildException(e, sql, param);
            }
        }

        private static T Run<T>(Func<T> func, string sql, object param)
        {
            try
            {
                return func();
            }
            catch (Exception e)
            {
                throw ExceptionBuilder.BuildException(e, sql, param);
            }
        }
    }
}

