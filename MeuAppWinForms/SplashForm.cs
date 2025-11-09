using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MeuAppWinForms
{
    public class SplashForm : Form
    {
    private System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();

        public SplashForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.Black;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Width = 420;
            this.Height = 420;
            this.Opacity = 0.96;

            var picture = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Black
            };
            try
            {
                // tenta abrir do output Resources
                var png = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "CarvaoChama.png");
                var jpg = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "CarvaoChama.jpg");
                if (File.Exists(png)) picture.Image = Image.FromFile(png);
                else if (File.Exists(jpg)) picture.Image = Image.FromFile(jpg);
            }
            catch { }
            this.Controls.Add(picture);

            _timer.Interval = 1500; // 1.5s
            _timer.Tick += (s, e) => { _timer.Stop(); this.Close(); };
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _timer.Start();
        }
    }
}
