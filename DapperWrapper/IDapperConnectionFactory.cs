namespace DapperWrapper
{
    public interface IDapperConnectionFactory
    {
        IDapperConnection CreateConnection();
    }
}