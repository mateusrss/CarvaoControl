using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CarvaoControl.Application.Services;
using CarvaoControl.Domain.Entities;
using CarvaoControl.Domain.Enums;

namespace MeuAppWinForms
{
    public partial class VendasForm : Form
    {
        private readonly VendaAppService _app;
    private ComboBox cbProdutos = null!;
    private NumericUpDown numQuantidade = null!;
    private ComboBox cbPagamento = null!;
    private Button btnVender = null!;
    private Label lblTotal = null!;
    private Label lblPrecoUnitario = null!;

        public VendasForm(VendaAppService app)
        {
            _app = app ?? throw new ArgumentNullException(nameof(app));
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Registrar Venda";
            this.Size = new Size(400, 300);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var lblProduto = new Label
            {
                Text = "Produto:",
                Location = new Point(20, 20),
                AutoSize = true
            };

            cbProdutos = new ComboBox
            {
                Location = new Point(20, 40),
                Width = 340,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cbProdutos.SelectedIndexChanged += CbProdutos_SelectedIndexChanged;

            lblPrecoUnitario = new Label
            {
                Location = new Point(20, 70),
                AutoSize = true
            };

            var lblQuantidade = new Label
            {
                Text = "Quantidade:",
                Location = new Point(20, 100),
                AutoSize = true
            };

            numQuantidade = new NumericUpDown
            {
                Location = new Point(20, 120),
                Width = 100,
                Minimum = 1,
                Maximum = 999,
                Value = 1
            };
            numQuantidade.ValueChanged += (s, e) => AtualizarTotal();

            var lblPagamento = new Label
            {
                Text = "Forma de Pagamento:",
                Location = new Point(20, 150),
                AutoSize = true
            };

            cbPagamento = new ComboBox
            {
                Location = new Point(20, 170),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cbPagamento.Items.AddRange(Enum.GetNames(typeof(TipoPagamento)));
            cbPagamento.SelectedIndex = 0;

            lblTotal = new Label
            {
                Location = new Point(20, 200),
                AutoSize = true,
                Font = new Font(this.Font.FontFamily, 12, FontStyle.Bold)
            };

            btnVender = new Button
            {
                Text = "Confirmar Venda",
                Location = new Point(240, 220),
                Size = new Size(120, 30)
            };
            btnVender.Click += BtnVender_Click;

            this.Controls.AddRange(new Control[]
            {
                lblProduto,
                cbProdutos,
                lblPrecoUnitario,
                lblQuantidade,
                numQuantidade,
                lblPagamento,
                cbPagamento,
                lblTotal,
                btnVender
            });
        }

        private void LoadData()
        {
            var produtos = _app.ListarProdutos().ToList();
            cbProdutos.DisplayMember = "Nome";
            cbProdutos.ValueMember = "Id";
            cbProdutos.DataSource = produtos;

            if (produtos.Any())
            {
                cbProdutos.SelectedIndex = 0;
                AtualizarPrecoUnitario();
                AtualizarTotal();
            }
            else
            {
                MessageBox.Show("Não há produtos cadastrados.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
            }
        }

    private void CbProdutos_SelectedIndexChanged(object? sender, EventArgs e)
        {
            AtualizarPrecoUnitario();
            AtualizarTotal();
        }

        private void AtualizarPrecoUnitario()
        {
            if (cbProdutos.SelectedItem is Produto produto)
            {
                lblPrecoUnitario.Text = $"Preço unitário: {produto.Preco:C2}";
            }
        }

        private void AtualizarTotal()
        {
            if (cbProdutos.SelectedItem is Produto produto)
            {
                var total = produto.Preco * numQuantidade.Value;
                lblTotal.Text = $"Total: {total:C2}";
            }
        }

    private void BtnVender_Click(object? sender, EventArgs e)
        {
            try
            {
                if (cbProdutos.SelectedItem is not Produto produto)
                {
                    MessageBox.Show("Selecione um produto.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(cbPagamento.Text))
                {
                    MessageBox.Show("Selecione a forma de pagamento.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var quantidade = (int)numQuantidade.Value;
                var tipoPagamento = (TipoPagamento)Enum.Parse(typeof(TipoPagamento), cbPagamento.Text);

                _app.RegistrarVenda(produto.Id, quantidade, tipoPagamento);

                MessageBox.Show("Venda registrada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao registrar venda: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}