
using PedidoAPI.Models;

namespace PedidoAPI.Repositories
{
	public interface IPedidoRepository
	{
		Task<Pedido?> CreateAsync(Pedido entity);

		Task<bool> UpdateAsync(Pedido entity);

		Task<bool> DeleteAsync(int id);

		Task<List<Pedido>> GetAllAsync(int usuarioId, int page, int pageSize);

		Task<int> CountAsync(int usuarioId);
	}
}
