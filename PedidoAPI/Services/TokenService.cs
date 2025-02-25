using Microsoft.IdentityModel.Tokens;
using PedidoAPI.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PedidoAPI.Services
{
	public class TokenService
	{
		private readonly IConfiguration _configuration;

		public TokenService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		/// <summary>
		/// Gera um token jwt para autenticação de usuários para acesso aos endpoints da api.
		/// OBS: Para essa api a secretkey foi armazenada no appsettings se forma ilustrativa à autenticação jwt. Como alternativas mais seguras podem ser utilizados o 
		/// Azure Key Vault, AWS Secrets Manager, Google Cloud Secret Manager, entre outros.
		/// </summary>
		/// <param name="username">O nome de usuário para o qual o token é gerado</param>
		/// <param name="role"></param>
		/// <returns>Retorna o token gerado como uma string.</returns>
		/// <exception cref="Exception">Lança uma exceção se as configurações de autenticação não forem carregadas corretamente</exception>
		public string GenerateToken(string username, string role)
		{
			var secretKey = _configuration["Jwt:SecretKey"];
			var issuer = _configuration["Jwt:Issuer"];
			var audience = _configuration["Jwt:Audience"];

			if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
			{
				throw new Exception("As configurações de autenticação não foram carregadas corretamente.");
			}

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
			var tokenHandler = new JwtSecurityTokenHandler();
			var token = new JwtSecurityToken(
				issuer: issuer,
				audience: audience,
				claims:
				[
					new Claim(ClaimTypes.Name, username),
					new Claim(ClaimTypes.Role, role)
				],
				expires: DateTime.UtcNow.AddHours(1),
				signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
			);

			return tokenHandler.WriteToken(token);
		}
	}
}
