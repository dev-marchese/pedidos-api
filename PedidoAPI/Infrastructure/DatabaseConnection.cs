using Microsoft.Data.SqlClient;
using System.Data;

namespace PedidoAPI.Infrastructure
{
	public class DatabaseConnection
	{
		private readonly IConfiguration _configuration;

		public DatabaseConnection(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task<IDbConnection> CreateConnectionAsync()
		{
			var connectionString = _configuration.GetConnectionString("conn");
			var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();
			return connection;
		}
	}
}
