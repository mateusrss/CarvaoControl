using System.Windows.Forms;

namespace MeuAppWinForms
{
    partial class AjusteEstoqueForm
    {
        private System.ComponentModel.IContainer components = null;

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
            this.lblProduto = new System.Windows.Forms.Label();
            this.lblEstoqueAtual = new System.Windows.Forms.Label();
            this.lblNovaQuantidade = new System.Windows.Forms.Label();
            this.numQuantidade = new System.Windows.Forms.NumericUpDown();
            this.lblMotivo = new System.Windows.Forms.Label();
            this.txtMotivo = new System.Windows.Forms.TextBox();
            this.btnSalvar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numQuantidade)).BeginInit();
            this.SuspendLayout();

            
            this.lblProduto.AutoSize = true;
            this.lblProduto.Location = new System.Drawing.Point(12, 9);
            this.lblProduto.Name = "lblProduto";
            this.lblProduto.Size = new System.Drawing.Size(50, 15);
            this.lblProduto.TabIndex = 0;
            this.lblProduto.Text = "Produto:";

            
            this.lblEstoqueAtual.AutoSize = true;
            this.lblEstoqueAtual.Location = new System.Drawing.Point(12, 33);
            this.lblEstoqueAtual.Name = "lblEstoqueAtual";
            this.lblEstoqueAtual.Size = new System.Drawing.Size(82, 15);
            this.lblEstoqueAtual.TabIndex = 1;
            this.lblEstoqueAtual.Text = "Estoque Atual:";

            
            this.lblNovaQuantidade.AutoSize = true;
            this.lblNovaQuantidade.Location = new System.Drawing.Point(12, 64);
            this.lblNovaQuantidade.Name = "lblNovaQuantidade";
            this.lblNovaQuantidade.Size = new System.Drawing.Size(102, 15);
            this.lblNovaQuantidade.TabIndex = 2;
            this.lblNovaQuantidade.Text = "Nova Quantidade:";

            
            this.numQuantidade.Location = new System.Drawing.Point(120, 62);
            this.numQuantidade.Name = "numQuantidade";
            this.numQuantidade.Size = new System.Drawing.Size(120, 23);
            this.numQuantidade.TabIndex = 3;

            
            this.lblMotivo.AutoSize = true;
            this.lblMotivo.Location = new System.Drawing.Point(12, 98);
            this.lblMotivo.Name = "lblMotivo";
            this.lblMotivo.Size = new System.Drawing.Size(48, 15);
            this.lblMotivo.TabIndex = 4;
            this.lblMotivo.Text = "Motivo:";

            
            this.txtMotivo.Location = new System.Drawing.Point(12, 116);
            this.txtMotivo.Multiline = true;
            this.txtMotivo.Name = "txtMotivo";
            this.txtMotivo.Size = new System.Drawing.Size(360, 60);
            this.txtMotivo.TabIndex = 5;

            
            this.btnSalvar.Location = new System.Drawing.Point(216, 182);
            this.btnSalvar.Name = "btnSalvar";
            this.btnSalvar.Size = new System.Drawing.Size(75, 23);
            this.btnSalvar.TabIndex = 6;
            this.btnSalvar.Text = "Salvar";
            this.btnSalvar.UseVisualStyleBackColor = true;
            this.btnSalvar.Click += new System.EventHandler(this.btnSalvar_Click);

            
            this.btnCancelar.Location = new System.Drawing.Point(297, 182);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(75, 23);
            this.btnCancelar.TabIndex = 7;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);

            
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 217);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnSalvar);
            this.Controls.Add(this.txtMotivo);
            this.Controls.Add(this.lblMotivo);
            this.Controls.Add(this.numQuantidade);
            this.Controls.Add(this.lblNovaQuantidade);
            this.Controls.Add(this.lblEstoqueAtual);
            this.Controls.Add(this.lblProduto);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AjusteEstoqueForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Ajuste de Estoque";
            ((System.ComponentModel.ISupportInitialize)(this.numQuantidade)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblProduto;
        private System.Windows.Forms.Label lblEstoqueAtual;
        private System.Windows.Forms.Label lblNovaQuantidade;
        private System.Windows.Forms.NumericUpDown numQuantidade;
        private System.Windows.Forms.Label lblMotivo;
        private System.Windows.Forms.TextBox txtMotivo;
        private System.Windows.Forms.Button btnSalvar;
        private System.Windows.Forms.Button btnCancelar;
    }
}