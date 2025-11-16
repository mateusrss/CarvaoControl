using CarvaoControl.Domain.Entities;
using CarvaoControl.Domain.Services;
using CarvaoControl.Application.Services;
using CarvaoControl.Infrastructure.Services;
using System.Diagnostics;
using System.Threading;

namespace MeuAppWinForms;

static class Program
{
    private static Mutex? _instanceMutex;

    [STAThread]
    static void Main()
    {
        const string mutexName = "CarvaoControl_SingleInstance_Mutex";
        bool createdNew;

        _instanceMutex = new Mutex(true, mutexName, out createdNew);

        if (!createdNew)
        {
            MessageBox.Show(
                "Já existe uma instância do Carvão Control em execução.\n\n" +
                "Feche a outra instância antes de abrir uma nova.",
                "Aplicação já em execução",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
            return;
        }

        ApplicationConfiguration.Initialize();

        // Splash screen rápido
        try
        {
            using (var splash = new SplashForm())
            {
                splash.Show();
                // processa mensagens enquanto inicializa serviços pesados
                Application.DoEvents();
                Thread.Sleep(600); // pequeno delay para percepção visual
            }
        }
        catch { }

        // Inicializa serviços
        var estoqueService = new EstoqueService();
        var vendaService = new VendaService(estoqueService);
        var persistence = new PersistenceService(AppDomain.CurrentDomain.BaseDirectory);

        // Carrega produtos existentes
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
                    // Ignorado
                }
            }
        }

        // Carrega vendas existentes
        var vendasDto = persistence.LoadVendas();
        if (vendasDto != null)
        {
            foreach (var vd in vendasDto)
            {
                try
                {
                    // Reidrata a venda preservando a data e o valor total originais
                    var venda = new Venda(
                        id: vd.Id,
                        produtoId: vd.ProdutoId,
                        quantidade: vd.Quantidade,
                        valorTotal: vd.ValorTotal,
                        pagamento: vd.Pagamento,
                        data: vd.Data);
                    vendaService.AdicionarVendaExistente(venda);
                }
                catch
                {
                    // Ignorado
                }
            }
        }

        // Inicializa app services
        var vendaAppService = new VendaAppService(vendaService, estoqueService, persistence);
        var authService = new AuthService();
        var loggingService = new LoggingService();
        var estoqueAppService = new EstoqueAppService(estoqueService, loggingService, persistence);

    // Executa formulário principal
    Application.Run(new Form1(vendaAppService, authService, estoqueAppService));

        // Libera mutex ao fechar
        _instanceMutex?.ReleaseMutex();
        _instanceMutex?.Dispose();
    }
}
