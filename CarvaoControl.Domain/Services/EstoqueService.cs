using CarvaoControl.Domain.Entities;
using CarvaoControl.Domain.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace CarvaoControl.Domain.Services
{
    public class EstoqueService
    {
        // Lista privada e encapsulada
        private readonly List<Produto> _produtos = new();

        // Adiciona um produto, só se não existir duplicata
        public void AdicionarProduto(Produto produto)
        {
            if (_produtos.Any(p => p.Nome.Equals(produto.Nome, System.StringComparison.OrdinalIgnoreCase)))
                throw new DomainException($"O produto '{produto.Nome}' já está cadastrado.");

            _produtos.Add(produto);
        }

        // Reduz o estoque de um produto existente
        public void ReduzirEstoque(int produtoId, int quantidade)
        {
            var produto = ObterProdutoPorId(produtoId);
            produto.ReduzirEstoque(quantidade);
        }

        // Consulta produto pelo ID
        private Produto ObterProdutoPorId(int produtoId)
        {
            var produto = _produtos.FirstOrDefault(p => p.Id == produtoId);
            if (produto == null)
                throw new DomainException("Produto não encontrado.");

            return produto;
        }

        // Retorna uma cópia da lista de produtos (protege a lista interna)
        public IReadOnlyCollection<Produto> ListarProdutos() => _produtos.AsReadOnly();
    }
}
