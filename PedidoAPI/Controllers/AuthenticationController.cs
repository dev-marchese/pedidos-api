using Microsoft.AspNetCore.Mvc;
using PedidoAPI.DTOs;
using PedidoAPI.Services;

namespace PedidoAPI.Controllers
{
	[Route("api/v1/[controller]")]
	[ApiController]
	public class AuthenticationController : Controller
	{
		private readonly TokenService _tokenService;

		public AuthenticationController(TokenService tokenService)
		{
			_tokenService = tokenService;
		}

		[HttpPost]
		public IActionResult Login([FromBody] LoginDTO login)
		{
			if (login.UserName == "admin" && login.Password == "admin")
			{
				var token = _tokenService.GenerateToken(login.UserName, "Admin");
				return Ok(new { token });
			}

			return Unauthorized(new { message = "Usuário ou senha inválidos." });
		}
	}
}
