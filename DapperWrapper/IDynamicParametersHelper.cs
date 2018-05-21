using System.Data;
using Dapper;

namespace DapperWrapper
{
    public interface IDynamicParametersHelper
    {
        void Add(DynamicParameters dynamicParameters, string name, DbType dbType, ParameterDirection direction, object value);
        void Add(DynamicParameters dynamicParameters, string name, DbType dbType, ParameterDirection direction);
        T Get<T>(DynamicParameters dynamicParameters, string name);
    }
}