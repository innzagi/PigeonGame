using System;
using System.Drawing;
using System.Windows.Forms;

namespace PigeonGame
{
    public partial class ArenaForm : Form
    {
        private Image background;
        public ArenaForm()
        {
            InitializeComponent();

            this.Text = "Голубиный дозор";
            this.Width = 1920;
            this.Height = 1080;

            this.DoubleBuffered = true;

            background = Image.FromFile("Resources/Background.png");

            this.Paint += PaintFormArena;
        }

        private void PaintFormArena(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(background, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
        }
    }
}