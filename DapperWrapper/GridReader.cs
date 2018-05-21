using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace DapperWrapper
{
    public class GridReader : IGridReader
    {
        private readonly SqlMapper.GridReader gridReader;

        public GridReader(SqlMapper.GridReader gridReader)
        {
            this.gridReader = gridReader;
        }

        public IList<dynamic> Read(bool buffered = true)
        {
            return gridReader.Read<dynamic>().ToList();
        }

        public Task<IEnumerable<dynamic>> ReadAsync(bool buffered = true)
        {
            return gridReader.ReadAsync<dynamic>(buffered);
        }

        public IList<T> Read<T>(bool buffered = true)
        {
            return gridReader.Read<T>().ToList();
        }

        public Task<IEnumerable<T>> ReadAsync<T>(bool buffered = true)
        {
            return gridReader.ReadAsync<T>(buffered);
        }

        public IList<TReturn> Read<TFirst, TSecond, TReturn>(Func<TFirst, TSecond, TReturn> func, string splitOn = "id")
        {
            return gridReader.Read(func, splitOn).ToList();
        }

        public IList<TReturn> Read<TFirst, TSecond, TThird, TReturn>(Func<TFirst, TSecond, TThird, TReturn> func, string splitOn = "id")
        {
            return gridReader.Read(func, splitOn).ToList();
        }

        public IList<TReturn> Read<TFirst, TSecond, TThird, TFourth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TReturn> func, string splitOn = "id")
        {
            return gridReader.Read(func, splitOn).ToList();
        }

        public void Dispose()
        {
            gridReader.Dispose();
        }
    }
}
    