
using Dapper;
using Microsoft.Extensions.Caching.Distributed;
using PedidoAPI.Infrastructure;
using PedidoAPI.Models;

namespace PedidoAPI.Repositories
{
	public class PedidoRepository : BaseRepository<Pedido>, IPedidoRepository
	{
		private readonly ILogger<UsuarioRepository> _logger;

		public PedidoRepository(DatabaseConnection dbContext, ILogger<UsuarioRepository> logger, IDistributedCache cache) : base(dbContext, logger, cache)
		{
			_logger = logger;
		}

		public async Task<Pedido?> CreateAsync(Pedido entity)
		{
			return await Insert(entity);
		}

		public async Task<bool> UpdateAsync(Pedido entity)
		{
			return await Update(entity);
		}

		public async Task<bool> DeleteAsync(int id)
		{
			return await Delete(new Pedido() { Id = id });
		}

		/// <summary>
		/// Recupera uma lista paginada de pedidos associados a um usuário específico. 
		/// O método implementa armazenamento em cache baseado no ID do usuário fornecido,
		/// permitindo a reutilização de consultas anteriores e demonstrando a flexibilidade do cache
		/// para diferentes tipos de consultas conforme a necessidade.
		/// </summary>
		/// <param name="usuarioId">Chave do usuário</param>
		/// <param name="page">Número da página</param>
		/// <param name="pageSize">Número de pedidos a serem retornados por página. O número de pedidos retorna para a página poderá ser igual o menor que o valor fornecido.</param>
		/// <returns>Uma lista de pedidos relacionados ao usuário especificado e paginada conforme com os parâmetros de paginação fornecidos.</returns>
		/// <remarks>
		/// O método utiliza caching para armazenar os resultados da consulta, evitando a execução repetida 
		/// da mesma query no banco de dados, melhorando o desempenho.
		/// Os dados são armazenados no cache com a chave formatada como "Pedidos:ByUser:{usuarioId}".
		/// </remarks>
		public async Task<List<Pedido>> GetAllAsync(int usuarioId, int page, int pageSize)
		{
			DynamicParameters param = new DynamicParameters();
			param.Add("@usuarioid", usuarioId);
			param.Add("@offset", (page - 1) * pageSize);
			param.Add("@pageSize", pageSize);

			string cacheKey = $"Pedidos:ByUser:{usuarioId}";
			return await Query<Pedido>("select * from Pedido where usuarioid = @usuarioid order by Id offset @offset rows fetch next @pagesize rows only", param, cacheKey);
		}

		public async Task<int> CountAsync(int usuarioId)
		{
			DynamicParameters param = new DynamicParameters();
			param.Add("@usuarioid", usuarioId);

			return await QuerySingle<int>("select count(*) from Pedido where usuarioid = @usuarioid", param);
		}
	}
}
