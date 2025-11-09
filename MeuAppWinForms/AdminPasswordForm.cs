using System;
using System.Windows.Forms;
using System.Drawing;
using CarvaoControl.Application.Services;

namespace MeuAppWinForms
{
    public class AdminPasswordForm : Form
    {
        private readonly AuthService _auth;
    private TextBox txtCurrentPassword = null!;
    private TextBox txtNewPassword = null!;
    private TextBox txtConfirmPassword = null!;
    private Button btnSave = null!;
    private Button btnCancel = null!;

        public AdminPasswordForm(AuthService auth)
        {
            _auth = auth ?? throw new ArgumentNullException(nameof(auth));
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Configurar Senha Administrativa";
            this.Size = new Size(400, 250);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var lblCurrentPassword = new Label
            {
                Text = "Senha Atual:",
                Location = new Point(20, 20),
                AutoSize = true
            };

            txtCurrentPassword = new TextBox
            {
                Location = new Point(20, 40),
                Size = new Size(340, 23),
                UseSystemPasswordChar = true
            };

            var lblNewPassword = new Label
            {
                Text = "Nova Senha:",
                Location = new Point(20, 70),
                AutoSize = true
            };

            txtNewPassword = new TextBox
            {
                Location = new Point(20, 90),
                Size = new Size(340, 23),
                UseSystemPasswordChar = true
            };

            var lblConfirmPassword = new Label
            {
                Text = "Confirmar Nova Senha:",
                Location = new Point(20, 120),
                AutoSize = true
            };

            txtConfirmPassword = new TextBox
            {
                Location = new Point(20, 140),
                Size = new Size(340, 23),
                UseSystemPasswordChar = true
            };

            btnSave = new Button
            {
                Text = "Salvar",
                DialogResult = DialogResult.OK,
                Location = new Point(190, 180),
                Size = new Size(80, 30)
            };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Cancelar",
                DialogResult = DialogResult.Cancel,
                Location = new Point(280, 180),
                Size = new Size(80, 30)
            };

            this.Controls.AddRange(new Control[] {
                lblCurrentPassword,
                txtCurrentPassword,
                lblNewPassword,
                txtNewPassword,
                lblConfirmPassword,
                txtConfirmPassword,
                btnSave,
                btnCancel
            });

            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCurrentPassword.Text) ||
                string.IsNullOrWhiteSpace(txtNewPassword.Text) ||
                string.IsNullOrWhiteSpace(txtConfirmPassword.Text))
            {
                MessageBox.Show("Preencha todos os campos.", 
                    "Campos Obrigatórios", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            if (!_auth.ValidateAdmin(txtCurrentPassword.Text))
            {
                MessageBox.Show("Senha atual incorreta.", 
                    "Erro", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
                txtCurrentPassword.SelectAll();
                txtCurrentPassword.Focus();
                this.DialogResult = DialogResult.None;
                return;
            }

            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("As senhas não conferem.", 
                    "Erro", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
                txtNewPassword.Clear();
                txtConfirmPassword.Clear();
                txtNewPassword.Focus();
                this.DialogResult = DialogResult.None;
                return;
            }

            if (txtNewPassword.Text.Length < 4)
            {
                MessageBox.Show("A nova senha deve ter pelo menos 4 caracteres.", 
                    "Senha Inválida", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Warning);
                txtNewPassword.SelectAll();
                txtNewPassword.Focus();
                this.DialogResult = DialogResult.None;
                return;
            }

            try
            {
                _auth.SetAdminPassword(txtNewPassword.Text);
                MessageBox.Show("Senha alterada com sucesso!", 
                    "Sucesso", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Erro ao salvar a nova senha.", 
                    "Erro", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
            }
        }
    }
}