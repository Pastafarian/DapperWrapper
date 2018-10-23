using System;
using System.Data.SqlClient;
using System.Text;

namespace DapperWrapper
{
    internal class ExceptionBuilder
    {
        public static Exception BuildException(Exception exception, string sql, object param)
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

            if (exception is SqlException sqlException)
            {
                message.Append($"Sql database engine error number: {sqlException.Number}");
            }

            exception.Data.Add("DapperErrorDetails", message.ToString());

            throw exception;
        }
    }
}