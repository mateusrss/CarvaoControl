using CarvaoControl.Domain.Entities;
using CarvaoControl.Domain.Services;
using CarvaoControl.Application.Services;
using CarvaoControl.Infrastructure.Services;

namespace MeuAppWinForms;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Inicializa serviços e dados do aplicativo
        var estoqueService = new EstoqueService();
        var vendaService = new VendaService(estoqueService);
        var persistence = new PersistenceService(AppDomain.CurrentDomain.BaseDirectory);

        var produtos = persistence.LoadProdutos();
        if (produtos != null)
        {
            foreach (var dto in produtos)
            {
                try
                {
                    var prod = new Produto(dto.Nome, dto.Preco, dto.Quantidade);
                    prod.DefinirId(dto.Id);
                    estoqueService.AdicionarProduto(prod);
                }
                catch
                {
                    // ignorado
                }
            }
        }

        if (!estoqueService.ListarProdutos().Any())
        {
            var p1 = new Produto("Carvão 5kg", 20.0m, 10);
            p1.DefinirId(1);
            var p2 = new Produto("Carvão 10kg", 35.0m, 5);
            p2.DefinirId(2);
            var p3 = new Produto("Brasa 1kg", 8.5m, 20);
            p3.DefinirId(3);

            estoqueService.AdicionarProduto(p1);
            estoqueService.AdicionarProduto(p2);
            estoqueService.AdicionarProduto(p3);
        }

        var vendasDto = persistence.LoadVendas();
        if (vendasDto != null)
        {
            foreach (var vd in vendasDto)
            {
                try
                {
                    var produto = estoqueService.ListarProdutos().FirstOrDefault(p => p.Id == vd.ProdutoId);
                    decimal unitPrice = 0m;
                    if (produto != null && vd.Quantidade > 0)
                        unitPrice = Math.Round(vd.ValorTotal / vd.Quantidade, 2);
                    var venda = new Venda(vd.ProdutoId, vd.Quantidade, unitPrice, vd.Pagamento);
                    venda.DefinirId(vd.Id);

                    vendaService.AdicionarVendaExistente(venda);
                }
                catch
                {
                    // ignorado
                }
            }
        }

        var vendaAppService = new VendaAppService(vendaService, estoqueService, persistence);

        var authService = new CarvaoControl.Application.Services.AuthService();
        var loggingService = new LoggingService();
        var estoqueAppService = new EstoqueAppService(estoqueService, loggingService, persistence);

        Application.Run(new Form1(vendaAppService, authService, estoqueAppService));
    }
}