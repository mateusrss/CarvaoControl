using System;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CarvaoControl.Application;
using CarvaoControl.Application.Services;
using CarvaoControl.Domain.Entities;
using CarvaoControl.Infrastructure.Services;

namespace MeuAppWinForms
{
    public partial class Form1 : Form
    {
        private readonly VendaAppService _app;
        private readonly AuthService _auth;
        private readonly EstoqueAppService _estoqueService;

        private MenuStrip menuStrip = new();
        private StatusStrip statusStrip = new();
        private Panel mainPanel = new();
        private Button btnVendas = new();
        private Button btnEstoque = new();
        private Button btnRelatorios = new();
        private DataGridView dgvEstoque = new();

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
            // Form básico
            this.Text = AppConfig.AppTitle;
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1000, 700);
            this.BackColor = Color.White;

            // Header Panel
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(25, 118, 210) // Azul TOTVS
            };
            this.Controls.Add(headerPanel);

            // Logo no header
            var headerLogo = new PictureBox
            {
                Size = new Size(60, 60),
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(20, 10),
                BackColor = Color.Transparent
            };
            try
            {
                var resourceName = "MeuAppWinForms.Resources.CarvaoChama.png";
                using var stream = typeof(Form1).Assembly.GetManifestResourceStream(resourceName);
                if (stream != null) headerLogo.Image = Image.FromStream(stream);
                if (headerLogo.Image == null)
                {
                    var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "CarvaoChama.png");
                    if (File.Exists(filePath)) headerLogo.Image = Image.FromFile(filePath);
                }
            }
            catch { }
            headerPanel.Controls.Add(headerLogo);

            // Título no header
            var headerTitle = new Label
            {
                Text = "Carvão Chama Distribuidora",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(100, 20)
            };
            headerPanel.Controls.Add(headerTitle);

            // Definir ícone da janela
            try
            {
                // Preferir .ico se existir
                var icoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "AppIcon.ico");
                if (File.Exists(icoPath))
                {
                    this.Icon = new Icon(icoPath);
                }
                else
                {
                    // Fallback: converter PNG em ícone em runtime
                    var pngPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "CarvaoChama.png");
                    if (File.Exists(pngPath))
                    {
                        using var bmp = new Bitmap(pngPath);
                        var hIcon = bmp.GetHicon();
                        this.Icon = Icon.FromHandle(hIcon);
                    }
                }
            }
            catch { }

            // Menu Strip (estilizado)
            menuStrip.BackColor = Color.FromArgb(33, 150, 243);
            menuStrip.ForeColor = Color.White;
            menuStrip.Font = new Font("Segoe UI", 10);
            var menuVendas = new ToolStripMenuItem("Vendas");
            menuVendas.DropDownItems.AddRange(new ToolStripItem[] {
                new ToolStripMenuItem("Nova Venda", null, (s,e)=> AbrirTelaVendas()),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Histórico de Vendas", null, (s,e)=> AbrirTelaRelatorios())
            });
            var menuEstoque = new ToolStripMenuItem("Estoque");
            menuEstoque.DropDownItems.AddRange(new ToolStripItem[] {
                new ToolStripMenuItem("Controle de Estoque", null, (s,e)=> AbrirTelaEstoque()),
                new ToolStripMenuItem("Cadastrar Produto", null, (s,e)=> AbrirTelaCadastroProduto()),
                new ToolStripMenuItem("Renomear Produto", null, (s,e)=> AbrirTelaRenomearProduto())
            });
            var menuRelatorios = new ToolStripMenuItem("Relatórios");
            menuRelatorios.DropDownItems.AddRange(new ToolStripItem[] {
                new ToolStripMenuItem("Relatório de Vendas", null, (s,e)=> AbrirTelaRelatorios()),
                new ToolStripMenuItem("Relatório de Estoque", null, (s,e)=> AbrirTelaRelatorioEstoque())
            });
            var menuAdmin = new ToolStripMenuItem("Administrador");
            menuAdmin.DropDownItems.AddRange(new ToolStripItem[] {
                new ToolStripMenuItem("Ajustar Estoque", null, (s,e)=> AbrirTelaAjusteEstoque()),
                new ToolStripMenuItem("Alterar Senha", null, (s,e)=> AbrirTelaAlterarSenha()),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Limpar Produtos", null, (s,e)=> LimparProdutos()),
                new ToolStripMenuItem("Backup", null, (s,e)=> RealizarBackup())
            });
            menuStrip.Items.AddRange(new ToolStripItem[] { menuVendas, menuEstoque, menuRelatorios, menuAdmin });
            this.Controls.Add(menuStrip);

            // Status Strip
            statusStrip.BackColor = Color.FromArgb(245, 245, 245);
            statusStrip.ForeColor = Color.Gray;
            statusStrip.Font = new Font("Segoe UI", 9);
            statusStrip.Items.Add(new ToolStripStatusLabel("Sistema pronto"));
            this.Controls.Add(statusStrip);
            statusStrip.Dock = DockStyle.Bottom;

            // Painel principal
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.BackColor = Color.FromArgb(250, 250, 250);
            mainPanel.Padding = new Padding(20);
            this.Controls.Add(mainPanel);

            // Layout principal
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100)); // Dashboard buttons
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Grid
            mainPanel.Controls.Add(mainLayout);

            // Painel de botões do dashboard
            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(10),
                BackColor = Color.White
            };
            ConfigureButton(btnVendas, "Registrar Venda");
            ConfigureButton(btnEstoque, "Controle de Estoque");
            ConfigureButton(btnRelatorios, "Relatórios de Vendas");
            buttonPanel.Controls.Add(btnVendas);
            buttonPanel.Controls.Add(btnEstoque);
            buttonPanel.Controls.Add(btnRelatorios);
            mainLayout.Controls.Add(buttonPanel, 0, 0);

            // DataGridView estilizado
            dgvEstoque.Dock = DockStyle.Fill;
            dgvEstoque.BackgroundColor = Color.White;
            dgvEstoque.BorderStyle = BorderStyle.None;
            dgvEstoque.GridColor = Color.LightGray;
            dgvEstoque.RowHeadersVisible = false;
            dgvEstoque.AllowUserToAddRows = false;
            dgvEstoque.AllowUserToDeleteRows = false;
            dgvEstoque.AllowUserToResizeRows = false;
            dgvEstoque.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEstoque.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvEstoque.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvEstoque.DefaultCellStyle.BackColor = Color.White;
            dgvEstoque.DefaultCellStyle.ForeColor = Color.Black;
            dgvEstoque.DefaultCellStyle.SelectionBackColor = Color.FromArgb(33, 150, 243);
            dgvEstoque.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvEstoque.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvEstoque.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvEstoque.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvEstoque.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvEstoque.ColumnHeadersHeight = 40;
            dgvEstoque.DataSource = _app.ListarProdutos().Select(p => new
            {
                p.Nome,
                Preco = p.Preco.ToString("C"),
                p.Quantidade
            }).ToList();
            mainLayout.Controls.Add(dgvEstoque, 0, 1);
        }

        private void ConfigureButton(Button btn, string text)
        {
            btn.Text = text;
            btn.Font = new Font("Segoe UI Semibold", 12);
            btn.BackColor = Color.FromArgb(33, 150, 243); // Azul TOTVS
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Height = 50;
            btn.Width = 180;
            btn.Anchor = AnchorStyles.None;
            btn.Margin = new Padding(10);

            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(25, 118, 210);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(33, 150, 243);
        }

        private void ConfigureEvents()
        {
            btnVendas.Click += (s, e) => AbrirTelaVendas();
            btnEstoque.Click += (s, e) => AbrirTelaEstoque();
            btnRelatorios.Click += (s, e) => AbrirTelaRelatorios();
        }

        private void AbrirTelaVendas() => new VendasForm(_app).ShowDialog(this);
        private void AbrirTelaEstoque()
        {
            if (_app == null || _auth == null || _estoqueService == null) return;
            using var form = new EstoqueForm(_app, _auth, _estoqueService);
            form.ShowDialog(this);
        }
        private void AbrirTelaRelatorios() => new VendasReportForm(_app).ShowDialog(this);
        private void AbrirTelaCadastroProduto()
        {
            using var form = new RegisterProductForm(_app);
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                dgvEstoque.DataSource = _app.ListarProdutos().Select(p => new
                {
                    p.Nome,
                    Preco = p.Preco.ToString("C"),
                    p.Quantidade
                }).ToList();
            }
        }

        private void AbrirTelaRelatorioEstoque() { }
        private void AbrirTelaRenomearProduto()
        {
            var produtos = _app?.ListarProdutos();
            if (produtos == null || !produtos.Any())
            {
                MessageBox.Show("Não há produtos cadastrados.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // Garantimos não nulo: já verificado acima que existe pelo menos 1
            Produto? produto = produtos.FirstOrDefault();
            if (produtos.Count() > 1)
            {
                using var selectForm = new Form
                {
                    Text = "Selecionar Produto para Renomear",
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
                var btnOkSelect = new Button { Text = "OK", DialogResult = DialogResult.OK, Location = new Point(100, 80) };
                var btnCancelSelect = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, Location = new Point(180, 80) };
                selectForm.Controls.AddRange(new Control[] { label, selectCombo, btnOkSelect, btnCancelSelect });
                selectForm.AcceptButton = btnOkSelect;
                selectForm.CancelButton = btnCancelSelect;
                if (selectForm.ShowDialog(this) != DialogResult.OK) return;
                var selectedProduto = selectCombo.SelectedItem as CarvaoControl.Domain.Entities.Produto;
                if (selectedProduto == null)
                {
                    MessageBox.Show("Erro ao selecionar o produto.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                produto = selectedProduto;
            }
            // Now rename
            using var renameForm = new Form
            {
                Text = "Renomear Produto",
                Size = new Size(350, 150),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false
            };
            var txtNovoNome = new TextBox { Location = new Point(120, 20), Size = new Size(200, 25), Text = (produto?.Nome ?? string.Empty) };
            var lblNovoNome = new Label { Text = "Novo Nome:", Location = new Point(10, 25), AutoSize = true };
            var btnOkRename = new Button { Text = "Renomear", DialogResult = DialogResult.OK, Location = new Point(120, 60) };
            var btnCancelRename = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, Location = new Point(200, 60) };
            renameForm.Controls.AddRange(new Control[] { lblNovoNome, txtNovoNome, btnOkRename, btnCancelRename });
            renameForm.AcceptButton = btnOkRename;
            renameForm.CancelButton = btnCancelRename;
            if (renameForm.ShowDialog(this) == DialogResult.OK)
            {
                var novoNome = (txtNovoNome.Text ?? string.Empty).Trim();
                if (string.IsNullOrEmpty(novoNome))
                {
                    MessageBox.Show("Nome não pode ser vazio.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                try
                {
                    // produto não é nulo neste ponto (já validado anteriormente)
                    if (produto == null)
                    {
                        MessageBox.Show("Produto inválido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    // produto não é nulo aqui (checado acima). Supressão do aviso de nullabilidade no ponto específico.
#pragma warning disable CS8602 // Desreferência de uma referência possivelmente nula
                    _app.RenomearProduto(produto!.Id, novoNome);
#pragma warning restore CS8602
                    dgvEstoque.DataSource = _app.ListarProdutos().Select(p => new
                    {
                        p.Nome,
                        Preco = p.Preco.ToString("C"),
                        p.Quantidade
                    }).ToList();
                    MessageBox.Show("Produto renomeado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erro ao renomear produto", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void LimparProdutos()
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
                var result = MessageBox.Show("Tem certeza que deseja limpar todos os produtos? Esta ação não pode ser desfeita.", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        _app.LimparProdutos();
                        dgvEstoque.DataSource = _app.ListarProdutos().Select(p => new
                        {
                            p.Nome,
                            Preco = p.Preco.ToString("C"),
                            p.Quantidade
                        }).ToList();
                        MessageBox.Show("Produtos limpos com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao limpar produtos: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
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
                var produtos = _app?.ListarProdutos();
                if (produtos == null || !produtos.Any())
                {
                    MessageBox.Show("Não há produtos cadastrados.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var produto = produtos.First();
                if (produtos.Count() > 1)
                {
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
                    var btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Location = new Point(100, 80) };
                    var btnCancel = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, Location = new Point(180, 80) };
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
                using var ajusteForm = new AjusteEstoqueForm(_estoqueService, _auth, produto);
                ajusteForm.ShowDialog(this);
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
                        // Cria backup padrão (zip) na pasta de dados e copia para o destino escolhido
                        var backupService = new BackupService();
                        var zipPath = backupService.CreateBackup();
                        File.Copy(zipPath, dialog.FileName, overwrite: true);
                        MessageBox.Show("Backup realizado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao realizar backup: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
