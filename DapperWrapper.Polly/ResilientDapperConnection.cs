using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DapperWrapper.Polly
{

    public class ResilientDapperConnection : DapperConnection
    {
        private readonly IRetryPolicy retryPolicy;

        public ResilientDapperConnection(IDbConnection connection, IRetryPolicy retryPolicy) : base(connection)
        {
            this.retryPolicy = retryPolicy;
        }

        public override int Execute(string sql, object param = null, IDbTransaction transaction = null, int? timeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            return retryPolicy.Execute(() => base.Execute(sql, param, transaction, timeout, commandType));
        }
        
        public override Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, int? timeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            return retryPolicy.Execute(() => base.ExecuteAsync(sql, param, transaction, timeout, commandType));
        }

        public override IEnumerable<T> Query<T>(string sql, object param = null, IDbTransaction transaction = null, bool buffered = true,
            int? commandTimeout = null, CommandType? commandType = null)
        {
            return retryPolicy.Execute(() => base.Query<T>(sql, param, transaction, buffered, commandTimeout, commandType));
        }

        public override Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return retryPolicy.Execute(()=> base.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType));
        }

        public override IEnumerable<dynamic> Query(string sql, object param = null, IDbTransaction transaction = null, bool buffered = true,
            int? commandTimeout = null, CommandType? commandType = null)
        {
            return retryPolicy.Execute(() => base.Query(sql, param, transaction, buffered, commandTimeout, commandType));
        }

        public override Task<IEnumerable<dynamic>> QueryAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return retryPolicy.Execute(() => base.QueryAsync(sql, param, transaction, commandTimeout, commandType));
        }

        public override IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param = null,
            IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return retryPolicy.Execute(() => base.Query(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType));
        }

        public override Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param = null,
            IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return retryPolicy.Execute(() => base.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType));
        }

        public override T QuerySingle<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return retryPolicy.Execute(() => base.QuerySingle<T>(sql, param, transaction, commandTimeout, commandType));
        }

        public override Task<T> QuerySingleAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return retryPolicy.Execute(() => base.QuerySingleAsync<T>(sql, param, transaction, commandTimeout, commandType));
        }

        public override Task<T> QueryFirstAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return retryPolicy.Execute(() => base.QueryFirstAsync<T>(sql, param, transaction, commandTimeout, commandType));
        }

        public override Task<dynamic> QueryFirstOrDefaultAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return retryPolicy.Execute(() => base.QueryFirstOrDefaultAsync(sql, param, transaction, commandTimeout, commandType));
        }

        public override Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return retryPolicy.Execute(() => base.QueryFirstOrDefaultAsync<T>(sql, param, transaction, commandTimeout, commandType));
        }

        public override IGridReader QueryMultiple(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return retryPolicy.Execute(() => base.QueryMultiple(sql, param, transaction, commandTimeout, commandType));
        }

        public override Task<IGridReader> QueryMultipleAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return retryPolicy.Execute(() => base.QueryMultipleAsync(sql, param, transaction, commandTimeout, commandType));
        }
    }
}
