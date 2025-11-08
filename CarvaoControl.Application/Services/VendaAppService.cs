using CarvaoControl.Domain.Entities;
using CarvaoControl.Domain.Enums;
using CarvaoControl.Domain.Services;

namespace CarvaoControl.Application.Services
{
    public class VendaAppService
    {
        private readonly VendaService _vendaService;
        private readonly EstoqueService _estoqueService;

        public VendaAppService(VendaService vendaService, EstoqueService estoqueService)
        {
            _vendaService = vendaService;
            _estoqueService = estoqueService;
        }

        public void RegistrarVenda(int produtoId, int quantidade, TipoPagamento pagamento)
        {
            _vendaService.RegistrarVenda(produtoId, quantidade, pagamento);
        }

        public IEnumerable<Venda> ListarVendas()
        {
            return _vendaService.ListarVendas();
        }

        public IEnumerable<Venda> ListarVendasDoDia(DateTime data)
        {
            return _vendaService.ListarVendasDoDia(data);
        }

        public IEnumerable<Produto> ListarProdutos()
        {
            return _estoqueService.ListarProdutos();
        }
    }
}
