using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using CarvaoControl.Application.Services;
using CarvaoControl.Domain.Enums;
using CarvaoControl.Domain.Entities;

namespace MeuAppWinForms
{
    public partial class VendasReportForm : Form
    {
        private readonly VendaAppService _app;
        private DateTimePicker dtInicio = null!;
        private DateTimePicker dtFim = null!;
        private Button btnFiltrar = null!;
        private Button btnExportarCsv = null!;
        private Button btnExportarPdf = null!;
        private DataGridView dgvVendas = null!;
        private StatusStrip statusStrip = null!;
        private ToolStripStatusLabel lblTotal = null!;

    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    [System.ComponentModel.Browsable(false)]
    public string ReportType { get; set; } = "Vendas";

        public VendasReportForm(VendaAppService app)
        {
            _app = app ?? throw new ArgumentNullException(nameof(app));
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = $"Relatório de {ReportType}";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 250, 252); // Fundo azul muito claro

            // Logo
            var logo = new PictureBox
            {
                Size = new Size(100, 60),
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(20, 10)
            };
            try
            {
                var logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CarvaoChama.png");
                if (File.Exists(logoPath))
                {
                    logo.Image = Image.FromFile(logoPath);
                }
            }
            catch { }

            // Título
            var lblTitle = new Label
            {
                Text = $"Relatório de {ReportType}",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(23, 43, 77), // Azul escuro elegante
                AutoSize = true,
                Location = new Point(130, 20)
            };

            // Data inicial
            var lblInicio = new Label
            {
                Text = "Data Inicial:",
                Location = new Point(50, 90),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Regular)
            };

            dtInicio = new DateTimePicker
            {
                Location = new Point(140, 85),
                Size = new Size(180, 23),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy"
            };
            dtInicio.Value = DateTime.Now.AddDays(-30); // Últimos 30 dias por padrão

            // Data final
            var lblFim = new Label
            {
                Text = "Data Final:",
                Location = new Point(380, 90),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Regular)
            };

            dtFim = new DateTimePicker
            {
                Location = new Point(470, 85),
                Size = new Size(180, 23),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy"
            };
            dtFim.Value = DateTime.Now;

            // Botões
            btnFiltrar = new Button
            {
                Text = "Filtrar",
                Location = new Point(650, 83),
                Size = new Size(80, 28),
                BackColor = Color.FromArgb(63, 81, 181), // Azul Material Design 3
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnFiltrar.Click += BtnFiltrar_Click;

            btnExportarCsv = new Button
            {
                Text = "Exportar CSV",
                Location = new Point(750, 83),
                Size = new Size(100, 28),
                BackColor = Color.FromArgb(76, 175, 80), // Verde sucesso
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnExportarCsv.Click += BtnExportarCsv_Click;

            btnExportarPdf = new Button
            {
                Text = "Exportar PDF",
                Location = new Point(870, 83),
                Size = new Size(100, 28),
                BackColor = Color.FromArgb(244, 67, 54), // Vermelho alerta
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnExportarPdf.Click += BtnExportarPdf_Click;

            // Grid
            dgvVendas = new DataGridView
            {
                Location = new Point(50, 130),
                Size = new Size(900, 480),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                GridColor = Color.FromArgb(224, 224, 224) // Cinza claro
            };

            dgvVendas.DefaultCellStyle.SelectionBackColor = Color.FromArgb(63, 81, 181);
            dgvVendas.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvVendas.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(63, 81, 181);
            dgvVendas.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvVendas.EnableHeadersVisualStyles = false;

            dgvVendas.Columns.Add("Data", "Data");
            dgvVendas.Columns.Add("Produto", "Produto");
            dgvVendas.Columns.Add("Quantidade", "Quantidade");
            dgvVendas.Columns.Add("ValorUnitario", "Valor Unitário");
            dgvVendas.Columns.Add("ValorTotal", "Valor Total");
            dgvVendas.Columns.Add("Pagamento", "Forma de Pagamento");

            // Definir larguras mínimas para evitar truncamento
            dgvVendas.Columns[0].MinimumWidth = 120; // Data
            dgvVendas.Columns[1].MinimumWidth = 150; // Produto
            dgvVendas.Columns[2].MinimumWidth = 80;  // Quantidade
            dgvVendas.Columns[3].MinimumWidth = 100; // Valor Unitário
            dgvVendas.Columns[4].MinimumWidth = 100; // Valor Total
            dgvVendas.Columns[5].MinimumWidth = 120; // Pagamento

            // Ajustes de alinhamento/formatacao
            dgvVendas.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft; // Data
            dgvVendas.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // Quantidade
            dgvVendas.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; // Valor Unitario
            dgvVendas.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; // Valor Total

            // Status strip
            statusStrip = new StatusStrip
            {
                BackColor = Color.FromArgb(63, 81, 181),
                ForeColor = Color.White,
                Dock = DockStyle.Bottom
            };

            lblTotal = new ToolStripStatusLabel
            {
                Font = new Font(this.Font.FontFamily, 10, FontStyle.Bold)
            };
            statusStrip.Items.Add(lblTotal);

            // Add controls
            this.Controls.AddRange(new Control[]
            {
                logo, lblTitle,
                lblInicio, dtInicio,
                lblFim, dtFim,
                btnFiltrar,
                btnExportarCsv,
                btnExportarPdf,
                dgvVendas,
                statusStrip
            });
        }

        public void LoadData()
        {
            try
            {
                dgvVendas.Rows.Clear();
                decimal total = 0;
                var culture = new CultureInfo("pt-BR");
                var produtosDict = _app.ListarProdutos().ToDictionary(p => p.Id);

                if (ReportType == "Vendas")
                {
                    var vendas = _app.ListarVendas()
                        .Where(v => v.Data >= dtInicio.Value && v.Data <= dtFim.Value)
                        .OrderByDescending(v => v.Data)
                        .ToList();

                    foreach (var venda in vendas)
                    {
                        if (produtosDict.TryGetValue(venda.ProdutoId, out var produto))
                        {
                            var valorUnitario = venda.Quantidade > 0 ? venda.ValorTotal / venda.Quantidade : 0m;
                            dgvVendas.Rows.Add(
                                venda.Data.ToString("dd/MM/yyyy HH:mm"),
                                produto.Nome,
                                venda.Quantidade,
                                valorUnitario.ToString("C2", culture),
                                venda.ValorTotal.ToString("C2", culture),
                                venda.Pagamento.ToString()
                            );
                            total += venda.ValorTotal;
                        }
                    }
                    lblTotal.Text = $"Total de vendas no período: {total:C2}";
                }
                else if (ReportType == "Estoque")
                {
                    foreach (var produto in produtosDict.Values)
                    {
                        var valorTotal = produto.Preco * produto.Quantidade;
                        dgvVendas.Rows.Add(
                            "-",
                            produto.Nome,
                            produto.Quantidade,
                            produto.Preco.ToString("C2", culture),
                            valorTotal.ToString("C2", culture),
                            "-"
                        );
                        total += valorTotal;
                    }
                    lblTotal.Text = $"Valor total em estoque: {total:C2}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnFiltrar_Click(object? sender, EventArgs e)
        {
            if (dtInicio.Value > dtFim.Value)
            {
                MessageBox.Show("A data inicial deve ser menor ou igual à data final.",
                    "Filtro Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            LoadData();
        }

        private void BtnExportarCsv_Click(object? sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Arquivos CSV (*.csv)|*.csv";
                dialog.Title = "Salvar Relatório";
                dialog.FileName = $"{ReportType.ToLower()}_{dtInicio.Value:yyyyMMdd}_a_{dtFim.Value:yyyyMMdd}.csv";

                if (dialog.ShowDialog() != DialogResult.OK) return;

                try
                {
                    using (var sw = new StreamWriter(dialog.FileName, false, Encoding.UTF8))
                    {
                        // Cabeçalho
                        var headers = dgvVendas.Columns.Cast<DataGridViewColumn>()
                            .Select(c => $"\"{c.HeaderText}\"")
                            .ToArray();
                        sw.WriteLine(string.Join(",", headers));

                        // Dados
                        foreach (DataGridViewRow row in dgvVendas.Rows)
                        {
                            var fields = row.Cells.Cast<DataGridViewCell>()
                                .Select(cell => $"\"{cell.Value}\"")
                                .ToArray();
                            sw.WriteLine(string.Join(",", fields));
                        }
                    }

                    MessageBox.Show("Arquivo CSV exportado com sucesso!",
                        "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao exportar: {ex.Message}",
                        "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnExportarPdf_Click(object? sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Arquivos PDF (*.pdf)|*.pdf";
                dialog.Title = "Salvar Relatório";
                dialog.FileName = $"{ReportType.ToLower()}_{dtInicio.Value:yyyyMMdd}_a_{dtFim.Value:yyyyMMdd}.pdf";

                if (dialog.ShowDialog() != DialogResult.OK) return;

                try
                {
                    // Cria documento PDF
                    using var document = new PdfDocument();
                    var page = document.AddPage();
                    page.Size = PdfSharpCore.PageSize.A4;
                    var gfx = XGraphics.FromPdfPage(page);

                    var fontHeader = new XFont("Arial", 14, XFontStyle.Bold);
                    var font = new XFont("Arial", 10, XFontStyle.Regular);
                    var fontSmall = new XFont("Arial", 9, XFontStyle.Regular);

                    double margin = 40;
                    double y = margin;
                    double lineHeight = 18;

                    // Título
                    gfx.DrawString($"Relatório de {ReportType}", fontHeader, XBrushes.Black, new XRect(margin, y, page.Width - margin * 2, lineHeight), XStringFormats.TopLeft);
                    y += lineHeight + 4;

                    // Período
                    gfx.DrawString($"Período: {dtInicio.Value:dd/MM/yyyy} a {dtFim.Value:dd/MM/yyyy}", font, XBrushes.Black, new XRect(margin, y, page.Width - margin * 2, lineHeight), XStringFormats.TopLeft);
                    y += lineHeight + 8;

                    // Colunas (ajuste simples)
                    double tableWidth = page.Width - margin * 2;
                    double[] colWidths;
                    string[] headers;

                    if (ReportType == "Vendas")
                    {
                        headers = new[] { "Data", "Produto", "Quantidade", "Valor Unitário", "Valor Total", "Pagamento" };
                        colWidths = new[] { 100.0, 250.0, 70.0, 90.0, 90.0, 100.0 };
                    }
                    else
                    {
                        headers = new[] { "-", "Produto", "Quantidade", "Valor Unitário", "Valor Total", "-" };
                        colWidths = new[] { 100.0, 250.0, 70.0, 90.0, 90.0, 100.0 };
                    }

                    // Cabeçalho
                    double x = margin;
                    for (int i = 0; i < headers.Length; i++)
                    {
                        gfx.DrawString(headers[i], fontSmall, XBrushes.Black, new XRect(x, y, colWidths[i], lineHeight), XStringFormats.TopLeft);
                        x += colWidths[i];
                    }
                    y += lineHeight + 4;

                    // Linhas
                    var culture = new CultureInfo("pt-BR");
                    decimal total = 0m;

                    foreach (DataGridViewRow row in dgvVendas.Rows)
                    {
                        x = margin;
                        // Verifica quebra de página
                        if (y + lineHeight > page.Height - margin)
                        {
                            // Nova página
                            page = document.AddPage();
                            page.Size = PdfSharpCore.PageSize.A4;
                            gfx = XGraphics.FromPdfPage(page);
                            y = margin;
                        }

                        for (int i = 0; i < row.Cells.Count; i++)
                        {
                            var cellValue = row.Cells[i].Value?.ToString() ?? string.Empty;
                            gfx.DrawString(cellValue, fontSmall, XBrushes.Black, new XRect(x, y, colWidths[Math.Min(i, colWidths.Length - 1)], lineHeight), XStringFormats.TopLeft);
                            x += colWidths[Math.Min(i, colWidths.Length - 1)];
                        }

                        // Soma (coluna ValorTotal é index 4)
                        if (row.Cells.Count > 4 && decimal.TryParse(row.Cells[4].Value?.ToString(), NumberStyles.Currency, culture, out var valor))
                        {
                            total += valor;
                        }

                        y += lineHeight + 4;
                    }

                    // Rodapé com total
                    y += 6;
                    gfx.DrawString($"Total: {total.ToString("C2", culture)}", font, XBrushes.Black, new XRect(margin, y, tableWidth, lineHeight), XStringFormats.TopLeft);

                    // Salva
                    using (var fs = File.Open(dialog.FileName, FileMode.Create))
                    {
                        document.Save(fs);
                    }

                    MessageBox.Show("Arquivo PDF exportado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao exportar: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}