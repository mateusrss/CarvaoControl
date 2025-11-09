using System;
using System.Windows.Forms;
using CarvaoControl.Application.Services;

namespace MeuAppWinForms
{
    public class EditPriceForm : Form
    {
        private NumericUpDown nudPrice;
        private Button btnOk;
        private Button btnCancel;
        private TextBox txtPassword;
        private readonly AuthService _authService;

        public decimal NewPrice => nudPrice.Value;

        public EditPriceForm(decimal currentPrice)
        {
            _authService = new AuthService();
            
            this.Text = "Editar Preço";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new System.Drawing.Size(320, 160);
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var lbl = new Label { Text = "Novo preço:", Left = 12, Top = 12, AutoSize = true };
            nudPrice = new NumericUpDown { Left = 12, Top = 36, Width = 200, DecimalPlaces = 2, Minimum = 0.01m, Maximum = 1000000, Value = currentPrice };

            var lblPassword = new Label { Text = "Senha do administrador:", Left = 12, Top = 70, AutoSize = true };
            txtPassword = new TextBox { Left = 12, Top = 94, Width = 200, UseSystemPasswordChar = true };

            btnOk = new Button { Text = "OK", Left = 140, Width = 70, Top = 124, DialogResult = DialogResult.None };
            btnCancel = new Button { Text = "Cancelar", Left = 220, Width = 70, Top = 124, DialogResult = DialogResult.Cancel };

            btnOk.Click += BtnOk_Click;

            this.Controls.Add(lbl);
            this.Controls.Add(nudPrice);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("Por favor digite a senha do administrador.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!_authService.ValidateAdmin(txtPassword.Text))
            {
                MessageBox.Show("Senha do administrador incorreta.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
