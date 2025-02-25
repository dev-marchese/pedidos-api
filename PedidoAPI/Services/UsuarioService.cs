
using PedidoAPI.Models;
using PedidoAPI.Repositories;

namespace PedidoAPI.Services
{
	public class UsuarioService : IUsuarioService
	{
		private readonly IUsuarioRepository _usuarioRepository;

		public UsuarioService(IUsuarioRepository usuarioRepository)
		{
			_usuarioRepository = usuarioRepository;
		}

		public async Task<Usuario?> CreateAsync(Usuario entity)
		{
			return await _usuarioRepository.CreateAsync(entity);
		}

		public async Task<bool> DeleteAsync(int id)
		{
			return await _usuarioRepository.DeleteAsync(id);
		}

		public async Task<List<Usuario>> GetAllAsync()
		{
			return await _usuarioRepository.GetAllAsync();
		}

		public async Task<Usuario?> GetUsuarioByIdAsync(int id)
		{
			return await _usuarioRepository.GetUsuarioByIdAsync(id);
		}

		public async Task<bool> UpdateAsync(Usuario entity)
		{
			return await _usuarioRepository.UpdateAsync(entity);
		}
	}
}
