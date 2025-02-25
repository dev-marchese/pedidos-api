using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PedidoAPI.DTOs;
using PedidoAPI.Models;
using PedidoAPI.Repositories;
using PedidoAPI.Services;

namespace PedidoAPI.Controllers
{
	[Authorize]
	[Route("api/v1/[controller]")]
	[ApiController]
	public class UsuariosController : Controller
	{
		private readonly ILogger<UsuarioRepository> _logger;
		private readonly IUsuarioService _usuarioService;
		private readonly IMapper _mapper;

		public UsuariosController(ILogger<UsuarioRepository> logger, IUsuarioService usuarioService, IMapper mapper)
		{
			_logger = logger;
			_usuarioService = usuarioService;
			_mapper = mapper;
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] UsuarioDTO model)
		{
			if (model == null)
			{
				return BadRequest(new { success = false, message = "Usuário não pode ser nulo." });
			}

			var entity = _mapper.Map<Usuario>(model);
			var entityInserted = await _usuarioService.CreateAsync(entity);
			model = _mapper.Map<UsuarioDTO>(entityInserted);
			if (entityInserted != null && entityInserted.Id > 0)
			{
				return CreatedAtAction(nameof(GetUsuarioById), new { model.Id }, model);
			}

			return BadRequest(new { success = false, message = "Erro ao cadastrar o usuário." });
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetUsuarioById(int id)
		{
			if (id <= 0)
			{
				return BadRequest(new { success = false, message = "Parâmetro inválido." });
			}

			var usuario = await _usuarioService.GetUsuarioByIdAsync(id);
			if (usuario == null)
			{
				return NotFound(new { success = false, message = "Registro não encontrado." });
			}

			return Ok(usuario);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update([FromBody] UsuarioDTO model)
		{
			if (model == null || model.Id == 0)
			{
				return BadRequest(new { success = false, message = "Parâmetro inválido." });
			}

			var entity = _mapper.Map<Usuario>(model);
			var isUpdated = await _usuarioService.UpdateAsync(entity);
			if (!isUpdated)
			{
				return BadRequest(new { success = false, message = "Não foi possível atualizar o registro." });
			}

			return Ok(new { success = true, message = "Registro atualizado com sucesso." });
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			if (id == 0)
			{
				return BadRequest(new { success = true, message = "Parâmetro inválido." });
			}

			var isDeleted = await _usuarioService.DeleteAsync(id);
			if (!isDeleted)
			{
				return BadRequest(new { success = false, message = "Não foi possível deletar o registro." });
			}

			return Ok(new { success = true, message = "Registro deletado com sucesso." });
		}
	}
}
