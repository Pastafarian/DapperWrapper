using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Transactions;

namespace DapperWrapper
{
    public interface IDapperConnection : IDisposable
    {
        void Open();

        ConnectionState State { get; }

        int Execute(string sql, object param = null, IDbTransaction transaction = null, int? timeout = default(int?), CommandType? commandType = default(CommandType?));

        Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, int? timeout = default(int?), CommandType? commandType = default(CommandType?));

        IEnumerable<T> Query<T>(string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);

        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null,
            IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

        IEnumerable<dynamic> Query(string sql, object param = null,
            IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null,
            CommandType? commandType = null);

        Task<IEnumerable<dynamic>> QueryAsync(string sql, object param = null,
            IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

        IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(string sql,
            Func<TFirst, TSecond, TReturn> map, object param = null, IDbTransaction transaction = null,
            bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);

        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(string sql,
            Func<TFirst, TSecond, TReturn> map, object param = null, IDbTransaction transaction = null,
            bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);

        T QuerySingle<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null,
            CommandType? commandType = null);

        Task<T> QuerySingleAsync<T>(string sql, object param = null,
            IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

        Task<T> QueryFirstAsync<T>(string sql, object param = null,
            IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

        Task<dynamic> QueryFirstOrDefaultAsync(string sql, object param = null,
            IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null,
            IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

        IGridReader QueryMultiple(string sql, object param = null,
            IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

        Task<IGridReader> QueryMultipleAsync(string sql, object param = null,
            IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

        void EnlistTransaction(Transaction transation);
  }
}