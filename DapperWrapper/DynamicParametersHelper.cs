using System.Data;
using Dapper;

namespace DapperWrapper
{
    public class DynamicParametersHelper : IDynamicParametersHelper
    {
        public void Add(DynamicParameters dynamicParameters, string name, DbType dbType, ParameterDirection direction, object value)
        {
            dynamicParameters.Add(name, dbType: dbType, direction: direction, value: value);
        }

        public void Add(DynamicParameters dynamicParameters, string name, DbType dbType, ParameterDirection direction)
        {
            dynamicParameters.Add(name, dbType: dbType, direction: direction);
        }

        public T Get<T>(DynamicParameters dynamicParameters, string name)
        {
            return dynamicParameters.Get<T>(name);
        }
    }
}