namespace MeuAppWinForms
{
    partial class RegisterProductForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtNome;
        private System.Windows.Forms.NumericUpDown nudPreco;
        private System.Windows.Forms.NumericUpDown nudQuantidade;
        private System.Windows.Forms.Button btnSalvar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Label lblNome;
        private System.Windows.Forms.Label lblPreco;
        private System.Windows.Forms.Label lblQuantidade;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtNome = new System.Windows.Forms.TextBox();
            this.nudPreco = new System.Windows.Forms.NumericUpDown();
            this.nudQuantidade = new System.Windows.Forms.NumericUpDown();
            this.btnSalvar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.lblNome = new System.Windows.Forms.Label();
            this.lblPreco = new System.Windows.Forms.Label();
            this.lblQuantidade = new System.Windows.Forms.Label();

            ((System.ComponentModel.ISupportInitialize)(this.nudPreco)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudQuantidade)).BeginInit();

            this.SuspendLayout();

            
            this.lblNome.Text = "Nome:";
            this.lblNome.Location = new System.Drawing.Point(12, 15);
            this.lblNome.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtNome.Location = new System.Drawing.Point(80, 12);
            this.txtNome.Width = 320;
            this.txtNome.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);

            this.lblPreco.Text = "Preço:";
            this.lblPreco.Location = new System.Drawing.Point(12, 50);
            this.lblPreco.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.nudPreco.Location = new System.Drawing.Point(80, 48);
            this.nudPreco.DecimalPlaces = 2;
            this.nudPreco.Maximum = 100000;
            this.nudPreco.Minimum = 0.01M;
            this.nudPreco.Value = 1.00M;

            this.lblQuantidade.Text = "Quantidade:";
            this.lblQuantidade.Location = new System.Drawing.Point(12, 85);
            this.lblQuantidade.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.nudQuantidade.Location = new System.Drawing.Point(80, 83);
            this.nudQuantidade.Minimum = 1;
            this.nudQuantidade.Maximum = 100000;
            this.nudQuantidade.Value = 1;

            
            this.btnSalvar.Text = "Salvar";
            this.btnSalvar.Location = new System.Drawing.Point(80, 120);
            this.btnSalvar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnSalvar.Click += new System.EventHandler(this.btnSalvar_Click);

            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Location = new System.Drawing.Point(170, 120);
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);

            
            this.ClientSize = new System.Drawing.Size(420, 170);
            this.Controls.Add(this.lblNome);
            this.Controls.Add(this.txtNome);
            this.Controls.Add(this.lblPreco);
            this.Controls.Add(this.nudPreco);
            this.Controls.Add(this.lblQuantidade);
            this.Controls.Add(this.nudQuantidade);
            this.Controls.Add(this.btnSalvar);
            this.Controls.Add(this.btnCancelar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Registrar Produto";
            this.AcceptButton = this.btnSalvar;
            this.CancelButton = this.btnCancelar;

            ((System.ComponentModel.ISupportInitialize)(this.nudPreco)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudQuantidade)).EndInit();

            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}