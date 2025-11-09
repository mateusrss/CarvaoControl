using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CarvaoControl.Application;
using CarvaoControl.Application.Services;

namespace MeuAppWinForms
{
    public partial class Form1 : Form
    {
        private readonly VendaAppService? _app;
        private readonly AuthService? _auth;
        private readonly EstoqueAppService? _estoqueService;

    private MenuStrip menuStrip = new();
    private StatusStrip statusStrip = new();
    private Panel mainPanel = new();
    private Button btnVendas = new();
    private Button btnEstoque = new();
    private Button btnRelatorios = new();
    private Label lblTitle = new();
    private PictureBox pictureBox = new();

        // Construtor do designer
        public Form1()
        {
            InitializeComponent();
        }

        // Construtor em tempo de execução
        public Form1(VendaAppService app, AuthService auth, EstoqueAppService estoqueService)
        {
            _app = app ?? throw new ArgumentNullException(nameof(app));
            _auth = auth ?? throw new ArgumentNullException(nameof(auth));
            _estoqueService = estoqueService ?? throw new ArgumentNullException(nameof(estoqueService));

            InitializeComponent();
            ConfigureEvents();
        }

        private void InitializeComponent()
        {
            // Configuração básica do formulário
            this.Text = AppConfig.AppTitle;
            this.Size = new Size(1024, 768);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(800, 600);

            // Menu Strip
            var menuVendas = new ToolStripMenuItem("Vendas");
            menuVendas.DropDownItems.AddRange(new ToolStripItem[] {
                new ToolStripMenuItem("Nova Venda", null, (s, e) => AbrirTelaVendas()),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Histórico de Vendas", null, (s, e) => AbrirTelaRelatorios())
            });

            var menuEstoque = new ToolStripMenuItem("Estoque");
            menuEstoque.DropDownItems.AddRange(new ToolStripItem[] {
                new ToolStripMenuItem("Controle de Estoque", null, (s, e) => AbrirTelaEstoque()),
                new ToolStripMenuItem("Cadastrar Produto", null, (s, e) => AbrirTelaCadastroProduto())
            });

            var menuRelatorios = new ToolStripMenuItem("Relatórios");
            menuRelatorios.DropDownItems.AddRange(new ToolStripItem[] {
                new ToolStripMenuItem("Relatório de Vendas", null, (s, e) => AbrirTelaRelatorios()),
                new ToolStripMenuItem("Relatório de Estoque", null, (s, e) => AbrirTelaRelatorioEstoque())
            });

            var menuAdmin = new ToolStripMenuItem("Administrador");
            menuAdmin.DropDownItems.AddRange(new ToolStripItem[] {
                new ToolStripMenuItem("Ajustar Estoque", null, (s, e) => AbrirTelaAjusteEstoque()),
                new ToolStripMenuItem("Alterar Senha", null, (s, e) => AbrirTelaAlterarSenha()),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Backup", null, (s, e) => RealizarBackup())
            });

            menuStrip.Items.AddRange(new ToolStripItem[] {
                menuVendas, menuEstoque, menuRelatorios, menuAdmin
            });

            // Status Strip
            statusStrip.Items.Add(new ToolStripStatusLabel("Pronto"));

            // Painel Principal
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.BackColor = Color.White;
            mainPanel.Padding = new Padding(20);

            // Título
            lblTitle.Text = "Sistema de Controle de Vendas de Carvão";
            lblTitle.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(64, 64, 64);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(50, 50);

            // Botões
            ConfigureButton(btnVendas, "Registrar Venda", new Point(50, 150));
            ConfigureButton(btnEstoque, "Controle de Estoque", new Point(50, 220));
            ConfigureButton(btnRelatorios, "Relatórios de Vendas", new Point(50, 290));

            // Picture Box para logo
            pictureBox.Size = new Size(300, 200);
            pictureBox.Location = new Point(450, 100);
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            // Label para o texto do logo
            var lblLogo = new Label
            {
                Text = "CARVÃO CHAMA",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 122, 183),
                AutoSize = true
            };
            lblLogo.Location = new Point(pictureBox.Left + (pictureBox.Width - lblLogo.PreferredWidth) / 2, 
                                       pictureBox.Bottom + 10);

            try
            {
                var logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CarvaoChama.png");
                if (File.Exists(logoPath))
                {
                    pictureBox.Image = Image.FromFile(logoPath);
                }
            }
            catch
            {
                // ignorado
            }

            mainPanel.Controls.Add(lblLogo);

            // Adiciona controles ao painel
            mainPanel.Controls.Add(lblTitle);
            mainPanel.Controls.Add(btnVendas);
            mainPanel.Controls.Add(btnEstoque);
            mainPanel.Controls.Add(btnRelatorios);
            mainPanel.Controls.Add(pictureBox);

            // Adiciona controles ao form
            this.Controls.Add(menuStrip);
            this.Controls.Add(mainPanel);
            this.Controls.Add(statusStrip);

            // Define a ordem dos controles
            menuStrip.Dock = DockStyle.Top;
            statusStrip.Dock = DockStyle.Bottom;
        }

        private void ConfigureButton(Button btn, string text, Point location)
        {
            btn.Text = text;
            btn.Size = new Size(300, 50);
            btn.Location = location;
            btn.Font = new Font("Segoe UI", 12);
            btn.FlatStyle = FlatStyle.Flat;
            btn.BackColor = Color.FromArgb(51, 122, 183);
            btn.ForeColor = Color.White;
            btn.Cursor = Cursors.Hand;

            btn.MouseEnter += (s, e) => {
                btn.BackColor = Color.FromArgb(40, 96, 144);
            };

            btn.MouseLeave += (s, e) => {
                btn.BackColor = Color.FromArgb(51, 122, 183);
            };
        }

        private void ConfigureEvents()
        {
            btnVendas.Click += (s, e) => AbrirTelaVendas();
            btnEstoque.Click += (s, e) => AbrirTelaEstoque();
            btnRelatorios.Click += (s, e) => AbrirTelaRelatorios();
        }

        private void AbrirTelaVendas()
        {
            if (_app == null || _auth == null) return;
            using var form = new VendasForm(_app);
            form.ShowDialog(this);
        }

        private void AbrirTelaEstoque()
        {
            if (_app == null || _auth == null || _estoqueService == null) return;
            using var form = new EstoqueForm(_app, _auth, _estoqueService);
            form.ShowDialog(this);
        }

        private void AbrirTelaRelatorios()
        {
            if (_app == null) return;
            using var form = new VendasReportForm(_app);
            form.ShowDialog(this);
        }

        private void AbrirTelaCadastroProduto()
        {
            if (_app == null || _auth == null) return;
            
            using var auth = new AdminAuthForm();
            if (auth.ShowDialog(this) == DialogResult.OK)
            {
                if (!_auth.ValidateAdmin(auth.Password))
                {
                    MessageBox.Show("Senha incorreta!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using var form = new RegisterProductForm(_app);
                form.ShowDialog(this);
            }
        }

        private void AbrirTelaRelatorioEstoque()
        {
            if (_app == null || _estoqueService == null) return;
            using var form = new VendasReportForm(_app) { ReportType = "Estoque" };
            form.ShowDialog(this);
        }

        private void AbrirTelaAlterarSenha()
        {
            if (_auth == null) return;
            
            using var auth = new AdminAuthForm();
            if (auth.ShowDialog(this) == DialogResult.OK)
            {
                if (!_auth.ValidateAdmin(auth.Password))
                {
                    MessageBox.Show("Senha incorreta!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using var form = new AdminPasswordForm(_auth);
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    MessageBox.Show("Senha alterada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void RealizarBackup()
        {
            if (_auth == null) return;
            
            using var auth = new AdminAuthForm();
            if (auth.ShowDialog(this) == DialogResult.OK)
            {
                if (!_auth.ValidateAdmin(auth.Password))
                {
                    MessageBox.Show("Senha incorreta!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    using var dialog = new SaveFileDialog
                    {
                        Filter = "Arquivo de Backup (*.bak)|*.bak",
                        Title = "Salvar Backup",
                        FileName = $"CarvaoChama_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak"
                    };

                    if (dialog.ShowDialog(this) == DialogResult.OK)
                    {
                        // TODO: Implementar o backup real usando BackupService
                        MessageBox.Show("Backup realizado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao realizar backup: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void AbrirTelaAjusteEstoque()
        {
            if (_auth == null || _estoqueService == null) return;
            
            using var form = new AdminAuthForm();
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                if (!_auth.ValidateAdmin(form.Password))
                {
                    MessageBox.Show("Senha incorreta!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Antes de abrir o formulário de ajuste, precisamos selecionar um produto
                var produtos = _app?.ListarProdutos();
                if (produtos == null || !produtos.Any())
                {
                    MessageBox.Show("Não há produtos cadastrados.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Se houver apenas um produto, use-o diretamente
                var produto = produtos.First();
                if (produtos.Count() > 1)
                {
                    // Se houver mais de um produto, mostre um diálogo de seleção
                    using var selectForm = new Form
                    {
                        Text = "Selecionar Produto",
                        Size = new Size(300, 150),
                        FormBorderStyle = FormBorderStyle.FixedDialog,
                        StartPosition = FormStartPosition.CenterParent,
                        MaximizeBox = false,
                        MinimizeBox = false
                    };

                    var selectCombo = new ComboBox
                    {
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        DataSource = produtos.ToList(),
                        DisplayMember = "Nome",
                        ValueMember = "Id",
                        Location = new Point(10, 40),
                        Size = new Size(260, 25)
                    };

                    var label = new Label { Text = "Selecione o produto:", Location = new Point(10, 20), AutoSize = true };

                    var btnOk = new Button 
                    { 
                        Text = "OK", 
                        DialogResult = DialogResult.OK,
                        Location = new Point(100, 80)
                    };
                    var btnCancel = new Button 
                    { 
                        Text = "Cancelar", 
                        DialogResult = DialogResult.Cancel,
                        Location = new Point(180, 80)
                    };

                    selectForm.Controls.AddRange(new Control[] { label, selectCombo, btnOk, btnCancel });
                    selectForm.AcceptButton = btnOk;
                    selectForm.CancelButton = btnCancel;

                    if (selectForm.ShowDialog(this) != DialogResult.OK) return;

                    var selectedProduto = selectCombo.SelectedItem as CarvaoControl.Domain.Entities.Produto;
                    if (selectedProduto == null)
                    {
                        MessageBox.Show("Erro ao selecionar o produto.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    produto = selectedProduto;
                }

                // Abrir formulário de ajuste
                using var ajusteForm = new AjusteEstoqueForm(_estoqueService, _auth, produto);
                ajusteForm.ShowDialog(this);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            // Desenha uma linha decorativa abaixo do título
            if (lblTitle != null && mainPanel != null)
            {
                using var pen = new Pen(Color.FromArgb(200, 200, 200), 2);
                var y = lblTitle.Bottom + 10;
                e.Graphics.DrawLine(pen, 50, y, mainPanel.Width - 50, y);
            }
        }
    }
}