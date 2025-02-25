using PedidoAPI.Models;
using PedidoAPI.Repositories;

namespace PedidoAPI.Services
{
	public class PedidoService : IPedidoService
	{
		private readonly IPedidoRepository _pedidoRepository;

		public PedidoService(IPedidoRepository pedidoRepository)
		{
			_pedidoRepository = pedidoRepository;
		}

		public async Task<Pedido?> CreateAsync(Pedido entity)
		{
			return await _pedidoRepository.CreateAsync(entity);
		}

		public async Task<bool> UpdateAsync(Pedido entity)
		{
			return await _pedidoRepository.UpdateAsync(entity);
		}

		public async Task<bool> DeleteAsync(int id)
		{
			return await _pedidoRepository.DeleteAsync(id);
		}

		public async Task<List<Pedido>> GetAllAsync(int usuarioId, int page, int pageSize)
		{
			return await _pedidoRepository.GetAllAsync(usuarioId, page, pageSize);
		}

		public async Task<int> CountAsync(int usuarioId)
		{
			return await _pedidoRepository.CountAsync(usuarioId);
		}
	}
}
