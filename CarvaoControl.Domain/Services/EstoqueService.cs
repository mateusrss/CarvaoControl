using CarvaoControl.Domain.Entities;
using CarvaoControl.Domain.Exceptions;

namespace CarvaoControl.Domain.Services
{
    public class EstoqueService
    {
        private readonly List<Produto> _produtos = new();

        public void AdicionarProduto(Produto produto)
        {
            if (_produtos.Any(p => p.Nome == produto.Nome))
                throw new DomainException("Produto já cadastrado.");

            _produtos.Add(produto);
        }

        public void ReduzirEstoque(int produtoId, int quantidade)
        {
            var produto = _produtos.FirstOrDefault(p => p.Id == produtoId)
                ?? throw new DomainException("Produto não encontrado.");

            produto.ReduzirEstoque(quantidade);
        }

        public IEnumerable<Produto> ListarProdutos() => _produtos;

        public void AtualizarPreco(int produtoId, decimal novoPreco)
        {
            var produto = _produtos.FirstOrDefault(p => p.Id == produtoId)
                ?? throw new DomainException("Produto não encontrado.");

            produto.AtualizarPreco(novoPreco);
        }
    }
}
