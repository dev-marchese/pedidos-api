
using PedidoAPI.Models;

namespace PedidoAPI.Services
{
	public interface IUsuarioService
	{
		Task<Usuario?> CreateAsync(Usuario entity);

		Task<bool> UpdateAsync(Usuario entity);

		Task<bool> DeleteAsync(int id);

		Task<Usuario?> GetUsuarioByIdAsync(int id);

		Task<List<Usuario>> GetAllAsync();
	}
}
