
namespace PedidoAPI.DTOs
{
	public class PedidoDTO
	{
		public int Id { get; set; }
		public int UsuarioId { get; set; }
		public string? Descricao { get; set; }
		public decimal Valor { get; set; }
	}
}
