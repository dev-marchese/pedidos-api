using Microsoft.Extensions.Caching.Distributed;
using PedidoAPI.Infrastructure;
using PedidoAPI.Models;
using static Dapper.SqlMapper;

namespace PedidoAPI.Repositories
{
	public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
	{
		private readonly ILogger<UsuarioRepository> _logger;

		public UsuarioRepository(DatabaseConnection dbContext, ILogger<UsuarioRepository> logger, IDistributedCache cache) : base(dbContext, logger, cache)
		{
			_logger = logger;
		}

		public async Task<Usuario?> CreateAsync(Usuario entity)
		{
			return await Insert(entity);
		}

		public async Task<bool> UpdateAsync(Usuario entity)
		{
			return await Update(entity);
		}

		public async Task<bool> DeleteAsync(int id)
		{
			return await Delete(new Usuario() { Id = id });
		}

		public async Task<Usuario?> GetUsuarioByIdAsync(int id)
		{
			return await GetById(id);
		}

		public async Task<List<Usuario>> GetAllAsync()
		{
			return await GetAll();
		}
	}
}
