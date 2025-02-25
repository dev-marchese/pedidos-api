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
	public class PedidosController : Controller
	{
		private readonly ILogger<PedidoRepository> _logger;
		private readonly IPedidoService _pedidoService;
		private readonly IMapper _mapper;

		public PedidosController(ILogger<PedidoRepository> logger, IPedidoService pedidoService, IMapper mapper)
		{
			_logger = logger;
			_pedidoService = pedidoService;
			_mapper = mapper;
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] PedidoDTO model)
		{
			if (model == null)
			{
				return BadRequest(new { success = false, message = "Pedido não pode ser nulo." });
			}

			var entity = _mapper.Map<Pedido>(model);
			var entityInserted = await _pedidoService.CreateAsync(entity);
			model = _mapper.Map<PedidoDTO>(entityInserted);
			if (entityInserted != null && entityInserted.Id > 0)
			{
				return CreatedAtAction(nameof(GetByUsuarioId), new { model.Id }, model);
			}

			return BadRequest(new { success = false, message = "Erro ao cadastrar o Registro." });
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetByUsuarioId(int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 2)
		{
			if (id <= 0)
			{
				return BadRequest(new { success = false, message = "Parâmetro inválido." });
			}

			if (page < 1 || pageSize < 1)
			{
				return BadRequest(new { success = false, message = "Os parâmetros de paginação devem ser maiores que zero." });
			}

			var totalRecords = await _pedidoService.CountAsync(id);
			var entitys = await _pedidoService.GetAllAsync(id, page, pageSize);
			var entitysAll = _mapper.Map<List<PedidoDTO>>(entitys);
			if (entitysAll == null || !entitysAll.Any())
			{
				return NotFound(new { success = false, message = "Registro não encontrado." });
			}

			var response = new
			{
				success = true,
				data = entitysAll,
				pagination = new
				{
					currentPage = page,
					pageSize = pageSize,
					totalRecords = totalRecords,
					totalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
				}
			};

			return Ok(response);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update([FromBody] PedidoDTO model)
		{
			if (model == null || model.Id == 0)
			{
				return BadRequest(new { success = false, message = "Parâmetro inválido." });
			}

			var entity = _mapper.Map<Pedido>(model);
			var isUpdated = await _pedidoService.UpdateAsync(entity);
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

			var isDeleted = await _pedidoService.DeleteAsync(id);
			if (!isDeleted)
			{
				return BadRequest(new { success = false, message = "Não foi possível deletar o registro." });
			}

			return Ok(new { success = true, message = "Registro deletado com sucesso." });
		}
	}
}
