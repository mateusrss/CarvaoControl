using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Globalization;
using CarvaoControl.Application.Services;
using CarvaoControl.Domain.Entities;

namespace MeuAppWinForms
{
    public class EstoqueForm : Form
    {
        private readonly VendaAppService _app;
        private readonly AuthService _auth;
        private readonly EstoqueAppService _estoqueService;
        private DataGridView dgvEstoque = null!;
        private Button btnAjustarEstoque = null!;
        private TextBox txtPesquisa = null!;
        private StatusStrip statusStrip = null!;
        private ToolStripStatusLabel statusLabel = null!;

        public EstoqueForm(VendaAppService app, AuthService auth, EstoqueAppService estoqueService)
        {
            _app = app ?? throw new ArgumentNullException(nameof(app));
            _auth = auth ?? throw new ArgumentNullException(nameof(auth));
            _estoqueService = estoqueService ?? throw new ArgumentNullException(nameof(estoqueService));

            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Controle de Estoque";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            
            txtPesquisa = new TextBox
            {
                Location = new Point(12, 12),
                Size = new Size(250, 23),
                PlaceholderText = "Pesquisar produto...",
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            txtPesquisa.TextChanged += (s, e) => FilterGrid();

            
            dgvEstoque = new DataGridView
            {
                Location = new Point(12, 41),
                Size = new Size(760, 450),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false
            };

            dgvEstoque.Columns.Add("Id", "ID");
            dgvEstoque.Columns.Add("Nome", "Nome");
            dgvEstoque.Columns.Add("Preco", "Preço");
            dgvEstoque.Columns.Add("Quantidade", "Quantidade");
            dgvEstoque.Columns.Add("ValorTotal", "Valor Total");

            
            btnAjustarEstoque = new Button
            {
                Text = "Ajustar Estoque",
                Location = new Point(12, 500),
                Size = new Size(120, 30),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            btnAjustarEstoque.Click += BtnAjustarEstoque_Click;

            
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            statusStrip.Items.Add(statusLabel);

            
            this.Controls.Add(txtPesquisa);
            this.Controls.Add(dgvEstoque);
            this.Controls.Add(btnAjustarEstoque);
            this.Controls.Add(statusStrip);

            
            var menu = new ContextMenuStrip();
            var editPrice = new ToolStripMenuItem("Editar Preço");
            editPrice.Click += (s, e) => EditSelectedPrice();
            menu.Items.Add(editPrice);
            dgvEstoque.ContextMenuStrip = menu;
        }

        private void LoadData()
        {
            dgvEstoque.Rows.Clear();
            var culture = new CultureInfo("pt-BR");

            foreach (var p in _app.ListarProdutos())
            {
                decimal valorTotal = p.Preco * p.Quantidade;
                dgvEstoque.Rows.Add(
                    p.Id,
                    p.Nome,
                    p.Preco.ToString("C2", culture),
                    p.Quantidade,
                    valorTotal.ToString("C2", culture)
                );
            }

            UpdateStatus();
        }

        private void FilterGrid()
        {
            var search = txtPesquisa.Text.Trim().ToLower();
            foreach (DataGridViewRow row in dgvEstoque.Rows)
            {
                var nome = row.Cells["Nome"].Value?.ToString()?.ToLower() ?? "";
                row.Visible = string.IsNullOrEmpty(search) || nome.Contains(search);
            }
        }

        private void UpdateStatus()
        {
            var total = _app.ObterEstoqueTotal();
            var valorTotal = _app.ListarProdutos().Sum(p => p.Preco * p.Quantidade);
            statusLabel.Text = $"Total de itens: {total} | Valor total em estoque: {valorTotal:C2}";
        }

        private void BtnAjustarEstoque_Click(object? sender, EventArgs e)
        {
            if (dgvEstoque.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione um produto.", "Ajustar Estoque", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var auth = new AdminAuthForm())
            {
                if (auth.ShowDialog(this) != DialogResult.OK) return;
                if (!_auth.ValidateAdmin(auth.Password))
                {
                    MessageBox.Show("Senha inválida.", "Autenticação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            var row = dgvEstoque.SelectedRows[0];
            var id = Convert.ToInt32(row.Cells["Id"].Value);
            var produto = _app.ListarProdutos().FirstOrDefault(p => p.Id == id);
            if (produto == null) return;

            using (var form = new AjusteEstoqueForm(_estoqueService, _auth, produto))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        private void EditSelectedPrice()
        {
            if (dgvEstoque.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione um produto.", "Editar Preço", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var auth = new AdminAuthForm())
            {
                if (auth.ShowDialog(this) != DialogResult.OK) return;
                if (!_auth.ValidateAdmin(auth.Password))
                {
                    MessageBox.Show("Senha inválida.", "Autenticação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            var row = dgvEstoque.SelectedRows[0];
            var id = Convert.ToInt32(row.Cells["Id"].Value);
            var produto = _app.ListarProdutos().FirstOrDefault(p => p.Id == id);
            if (produto == null) return;

            using (var edit = new EditPriceForm(produto.Preco))
            {
                if (edit.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    _app.AtualizarPrecoProduto(id, edit.NewPrice);
                    LoadData();
                    MessageBox.Show("Preço atualizado.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }

    // Helper form for numeric input
    public class NumericInputForm : Form
    {
        private NumericUpDown numericUpDown;
        public decimal Value => numericUpDown.Value;

        public NumericInputForm(string title, string prompt, decimal currentValue)
        {
            this.Text = title;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(300, 150);
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var label = new Label
            {
                Text = prompt,
                Location = new Point(10, 20),
                AutoSize = true
            };

            numericUpDown = new NumericUpDown
            {
                Location = new Point(10, 50),
                Size = new Size(120, 23),
                Minimum = 0,
                Maximum = 1000000,
                Value = currentValue
            };

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

            this.Controls.AddRange(new Control[] { label, numericUpDown, btnOk, btnCancel });
            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }
    }
}