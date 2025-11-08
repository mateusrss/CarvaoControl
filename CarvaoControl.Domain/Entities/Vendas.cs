using CarvaoControl.Domain.Enums;
using CarvaoControl.Domain.Exceptions;

namespace CarvaoControl.Domain.Entities
{
    public class Venda
    {
        public int Id { get; private set; }
        public int ProdutoId { get; private set; }
        public int Quantidade { get; private set; }
        public decimal ValorTotal { get; private set; }
        public TipoPagamento Pagamento { get; private set; }
        public DateTime Data { get; private set; } = DateTime.Now;

        // Construtor — define as regras de criação da venda
        public Venda(int produtoId, int quantidade, decimal precoProduto, TipoPagamento pagamento)
        {
            if (quantidade <= 0)
                throw new DomainException("A quantidade deve ser maior que zero.");

            if (precoProduto <= 0)
                throw new DomainException("O preço do produto deve ser maior que zero.");

            ProdutoId = produtoId;
            Quantidade = quantidade;
            ValorTotal = precoProduto * quantidade;
            Pagamento = pagamento;
            Data = DateTime.Now;
        }

        // Método para definir ID apenas internamente
        public void DefinirId(int id)
        {
            if (Id != 0)
                throw new DomainException("O ID da venda já foi definido.");

            Id = id;
        }
    }
}
