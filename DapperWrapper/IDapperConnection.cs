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

        int Execute(string sql, object param = null, CommandType? commandType = default(CommandType?));

        Task<int> ExecuteAsync(string sql, object param = null, CommandType? commandType = default(CommandType?));

        IEnumerable<object> Query(string sql, object param = null, CommandType? commandType = default(CommandType?), int? commandTimeout = null);

        Task<IEnumerable<dynamic>> QueryAsync(string sql, object param = null, CommandType? commandType = default(CommandType?), int? commandTimeout = null);

        IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, object param, CommandType? commandType, Func<TFirst, TSecond, TThird, TFourth,TReturn> func, string splitOn = "id");

        Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, object param, CommandType? commandType, Func<TFirst, TSecond, TThird, TFourth, TReturn> func, string splitOn = "id");

        IEnumerable<T> Query<T>(string sql, object param = null, CommandType? commandType = default(CommandType?), int? commandTimeout = null);

        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, CommandType? commandType = default(CommandType?), int? commandTimeout = null);

        Task<T> QuerySingleAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

        Task<T> QueryFirstAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

        IGridReader QueryMultiple(string sql, object param = null, CommandType? commandType = default(CommandType?));

        Task<IGridReader> QueryMultipleAsync(string sql, object param = null, CommandType? commandType = default(CommandType?));

        void EnlistTransaction(Transaction transation);
    }
}