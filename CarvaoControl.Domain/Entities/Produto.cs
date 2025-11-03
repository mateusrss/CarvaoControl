using CarvaoControl.Domain.Exceptions;
namespace CarvaoControl.Domain.Entities
{
    public class Produto
    {
        public int Id { get; private set; }
        public string Nome { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
        public int Quantity { get; private set; }

        public void ReduzirEstoque(int quantidade)
        {
            if (quantidade <= 0)
                throw new DomainException("A quantidade deve ser maior que zero.");

            if (quantidade > Quantity)
                throw new DomainException("Estoque insuficiente");

            Quantity -= quantidade;
        }
        public void AdicionarEstoque(int quantidade)
        {
            if (quantidade <= 0)
                throw new DomainException("A quantidade deve ser maior que zero.");
                
            Quantity += quantidade;
        }
        
    }
}

