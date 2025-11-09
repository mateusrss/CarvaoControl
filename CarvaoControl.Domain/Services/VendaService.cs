using CarvaoControl.Domain.Entities;
using CarvaoControl.Domain.Enums;
using CarvaoControl.Domain.Exceptions;

namespace CarvaoControl.Domain.Services
{
    public class VendaService
    {
        private readonly List<Venda> _vendas = new();
        private readonly EstoqueService _estoqueService;

        public VendaService(EstoqueService estoqueService)
        {
            _estoqueService = estoqueService;
        }

        public void RegistrarVenda(int produtoId, int quantidade, TipoPagamento pagamento)
        {
            var produto = _estoqueService
                .ListarProdutos()
                .FirstOrDefault(p => p.Id == produtoId)
                ?? throw new DomainException("Produto não encontrado.");

            if (produto.Quantidade < quantidade)
                throw new DomainException("Estoque insuficiente para realizar a venda.");

            // Cria a venda de forma segura, validada pelo construtor
            var venda = new Venda(produtoId, quantidade, produto.Preco, pagamento);
            venda.DefinirId(_vendas.Count + 1);

            _vendas.Add(venda);

            // Atualiza o estoque depois da venda
            _estoqueService.ReduzirEstoque(produtoId, quantidade);
        }

        public IEnumerable<Venda> ListarVendas() => _vendas;

        public IEnumerable<Venda> ListarVendasDoDia(DateTime dia)
        {
            return _vendas.Where(v => v.Data.Date == dia.Date);
        }

        // Permite adicionar vendas existentes (por exemplo ao carregar de persistência)
        public void AdicionarVendaExistente(Venda venda)
        {
            if (venda == null) throw new ArgumentNullException(nameof(venda));
            _vendas.Add(venda);
        }
    }
}
