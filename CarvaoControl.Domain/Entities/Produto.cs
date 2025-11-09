using CarvaoControl.Domain.Exceptions;

namespace CarvaoControl.Domain.Entities
{
    public class Produto
    {
        public int Id { get; private set; }
        public string Nome { get; private set; }
        public decimal Preco { get; private set; }
        public int Quantidade { get; private set; }

        // ***Construtor principal***
        public Produto(string nome, decimal preco, int quantidade)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new DomainException("O nome do produto é obrigatório.");

            if (preco <= 0)
                throw new DomainException("O preço deve ser maior que zero.");

            if (quantidade < 0)
                throw new DomainException("A quantidade não pode ser negativa.");

            Nome = nome;
            Preco = preco;
            Quantidade = quantidade;
        }

        // Permite atualizar o estoque depois que o produto já existe
        public void AdicionarEstoque(int quantidade)
        {
            if (quantidade <= 0)
                throw new DomainException("A quantidade deve ser maior que zero.");
            
            Quantidade += quantidade;
        }

        public void ReduzirEstoque(int quantidade)
        {
            if (quantidade <= 0)
                throw new DomainException("A quantidade deve ser maior que zero.");

            if (quantidade > Quantidade)
                throw new DomainException("Estoque insuficiente.");

            Quantidade -= quantidade;
        }

        // Permite alterar o preço com validação
        public void AtualizarPreco(decimal novoPreco)
        {
            if (novoPreco <= 0)
                throw new DomainException("O preço deve ser maior que zero.");

            Preco = novoPreco;
        }

        // Permite renomear o produto
        public void Renomear(string novoNome)
        {
            if (string.IsNullOrWhiteSpace(novoNome))
                throw new DomainException("O nome do produto é obrigatório.");

            Nome = novoNome;
        }

        // Define o Id depois (ex: quando for salvo em banco)
        public void DefinirId(int id)
        {
            if (id <= 0)
                throw new DomainException("Id inválido.");
            
            Id = id;
        }
    }
}
