using System.Data;

namespace Desic.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection Create();
        IDbConnection CreateAndOpen();
    }
}
