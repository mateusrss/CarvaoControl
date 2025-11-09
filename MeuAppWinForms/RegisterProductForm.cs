using CarvaoControl.Application.Services;
using System;
using System.Windows.Forms;

namespace MeuAppWinForms
{
    public partial class RegisterProductForm : Form
    {
        private readonly VendaAppService _app;

        public RegisterProductForm(VendaAppService app)
        {
            _app = app ?? throw new ArgumentNullException(nameof(app));
            InitializeComponent();
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                var nome = txtNome.Text.Trim();
                var preco = nudPreco.Value;
                var qtd = (int)nudQuantidade.Value;

                _app.AdicionarProduto(nome, preco, qtd);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro ao cadastrar produto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}