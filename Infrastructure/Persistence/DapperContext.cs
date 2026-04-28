using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Infrastructure.Persistence
{
    public class DapperContext
    {
        private readonly string _con;
        public DapperContext(IConfiguration config)
        {
            _con = config.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection String not Found");
        }
        public IDbConnection CreateConection() => new SqlConnection(_con);
    }
}
