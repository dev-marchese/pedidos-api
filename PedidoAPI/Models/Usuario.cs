using Dapper.Contrib.Extensions;

namespace PedidoAPI.Models
{
    [Table("Usuario")]
	public class Usuario
	{
        [Key]
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Email { get; set; }

        [Computed]
        public DateTime DataCadastro { get; set; }
    }
}
