using System;
using System.Collections.Generic;
using System.Linq;
using CarvaoControl.Domain.Entities;
using CarvaoControl.Domain.Enums;
using CarvaoControl.Domain.Services;

namespace CarvaoControl.Application.Services
{
    public class VendaAppService
    {
        private readonly VendaService _vendaService;
        private readonly EstoqueService _estoqueService;
        private readonly PersistenceService? _persistence;

        public VendaAppService(VendaService vendaService, EstoqueService estoqueService)
        {
            _vendaService = vendaService;
            _estoque_service_guard(estoqueService);
            _estoqueService = estoqueService;
        }

        // Overload that receives persistence to save changes
        public VendaAppService(VendaService vendaService, EstoqueService estoqueService, PersistenceService persistence)
        {
            _vendaService = vendaService;
            _estoqueService = estoqueService;
            _persistence = persistence;
        }

        public void RegistrarVenda(int produtoId, int quantidade, TipoPagamento pagamento)
        {
            _vendaService.RegistrarVenda(produtoId, quantidade, pagamento);
            try
            {
                // salva vendas e produtos (estoque mudou)
                _persistence?.SaveVendas(_vendaService.ListarVendas());
                _persistence?.SaveProdutos(_estoqueService.ListarProdutos());
            }
            catch { }
        }

        public IEnumerable<Venda> ListarVendas() => _vendaService.ListarVendas();

        public IEnumerable<Venda> ListarVendasDoDia(DateTime data) => _vendaService.ListarVendasDoDia(data);

        public IEnumerable<Produto> ListarProdutos() => _estoqueService.ListarProdutos();

        // Application-level method to add a product to the estoque (orquestração DDD)
        public void AdicionarProduto(string nome, decimal preco, int quantidade)
        {
            var produto = new Produto(nome, preco, quantidade);
            produto.DefinirId(_estoqueService.ListarProdutos().Count() + 1);
            _estoqueService.AdicionarProduto(produto);
            // salva produtos persistidos
            try { _persistence?.SaveProdutos(_estoqueService.ListarProdutos()); } catch { }
        }

        public void AtualizarPrecoProduto(int produtoId, decimal novoPreco)
        {
            _estoqueService.AtualizarPreco(produtoId, novoPreco);
            try { _persistence?.SaveProdutos(_estoqueService.ListarProdutos()); } catch { }
        }

        // Returns total items in stock (sum of Quantidade)
        public int ObterEstoqueTotal()
        {
            return _estoqueService.ListarProdutos().Sum(p => p.Quantidade);
        }

        private void _estoque_service_guard(EstoqueService estoqueService)
        {
            if (estoqueService == null) throw new ArgumentNullException(nameof(estoqueService));
        }
    }
}
