using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DapperWrapper
{
    public interface IGridReader
    {
        IList<object> Read(bool buffered = true);

        Task<IEnumerable<dynamic>> ReadAsync(bool buffered = true);

        IList<T> Read<T>(bool buffered = true);

        Task<IEnumerable<T>> ReadAsync<T>(bool buffered = true);

        IList<TReturn> Read<TFirst, TSecond, TReturn>(Func<TFirst, TSecond, TReturn> func, string splitOn = "id");

        IList<TReturn> Read<TFirst, TSecond, TThird, TReturn>(Func<TFirst, TSecond, TThird, TReturn> func, string splitOn = "id");

        IList<TReturn> Read<TFirst, TSecond, TThird, TFourth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TReturn> func, string splitOn = "id");

        void Dispose();
    }
}