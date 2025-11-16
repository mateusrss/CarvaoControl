using System;
using System.Windows.Forms;
using CarvaoControl.Application.Services;
using CarvaoControl.Domain.Entities;

namespace MeuAppWinForms
{
    public partial class AjusteEstoqueForm : Form
    {
        private readonly EstoqueAppService _estoqueService;
        private readonly AuthService _authService;
        private Produto _produto;

        public AjusteEstoqueForm(EstoqueAppService estoqueService, AuthService authService, Produto produto)
        {
            _estoqueService = estoqueService ?? throw new ArgumentNullException(nameof(estoqueService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _produto = produto ?? throw new ArgumentNullException(nameof(produto));

            InitializeComponent();
            ConfigurarForm();
        }

        private void ConfigurarForm()
        {
            lblProduto.Text = $"Produto: {_produto.Nome}";
            lblEstoqueAtual.Text = $"Estoque Atual: {_produto.Quantidade}";
            // Ajusta limites antes de definir o valor para evitar exceção
            numQuantidade.Minimum = 0;
            // Garante que o máximo comporta a quantidade atual (evita erro ao abrir com estoque > 100)
            numQuantidade.Maximum = Math.Max(999999, _produto.Quantidade);
            numQuantidade.Value = _produto.Quantidade;
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtMotivo.Text))
                {
                    MessageBox.Show("Por favor, informe o motivo do ajuste.", "Aviso", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var novaQuantidade = (int)numQuantidade.Value;
                _estoqueService.AjustarEstoque(_produto.Id, novaQuantidade, txtMotivo.Text);

                MessageBox.Show("Estoque ajustado com sucesso!", "Sucesso", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao ajustar estoque: {ex.Message}", "Erro", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}