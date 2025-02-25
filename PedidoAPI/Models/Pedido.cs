using Dapper.Contrib.Extensions;

namespace PedidoAPI.Models
{
	[Table("Pedido")]
	public class Pedido
	{
		[Key]
		public int Id { get; set; }
		public int UsuarioId { get; set; }
		public string? Descricao { get; set; }
		public decimal Valor { get; set; }

		[Computed]
		public DateTime DataCadastro { get; set; }
	}
}
