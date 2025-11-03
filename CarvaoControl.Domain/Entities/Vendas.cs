using CarvaoControl.Domain.Enums;
namespace CarvaoControl.Domain.Entities
{
    public class Venda
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorTotal { get; set; }
        public TipoPagamento Pagamento { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
    }

}