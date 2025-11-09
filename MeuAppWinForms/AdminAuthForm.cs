using System;
using System.Windows.Forms;

namespace MeuAppWinForms
{
    public class AdminAuthForm : Form
    {
    private TextBox txtPassword = null!;
    private Button btnOk = null!;
    private Button btnCancel = null!;

        public string Password => txtPassword.Text ?? string.Empty;

        public AdminAuthForm()
        {
            this.Text = "Autenticação Admin";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new System.Drawing.Size(320, 110);
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var lbl = new Label { Text = "Senha de administrador:", Left = 12, Top = 12, AutoSize = true };
            txtPassword = new TextBox { Left = 12, Top = 36, Width = 280, UseSystemPasswordChar = true };

            btnOk = new Button { Text = "OK", Left = 140, Width = 70, Top = 70, DialogResult = DialogResult.OK };
            btnCancel = new Button { Text = "Cancelar", Left = 220, Width = 70, Top = 70, DialogResult = DialogResult.Cancel };

            this.Controls.Add(lbl);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }
    }
}
