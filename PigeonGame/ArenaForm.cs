using System;
using System.Drawing;
using System.Windows.Forms;
using PigeonGame.Models;
using PigeonGame.Services;

namespace PigeonGame
{
    public partial class ArenaForm : Form
    {
        private Image _pigeonImage;
        private GameProcess _gameProcess;
        private Pigeon _pigeon;
        
        private Image _background;
        
        public ArenaForm()
        {
            InitializeComponent();
            SetupArena();
            StartGame();
        }
        private void StartGame()
        {
            _pigeon = new Pigeon(100, 100, _pigeonImage); 
            _gameProcess = new GameProcess(_pigeon);
        }

        private void SetupArena()
        {
            _pigeonImage = Image.FromFile("Resources/Pigeon.png");
            this.Text = "Голубиный дозор";
            this.Width = 1920;
            this.Height = 1080;

            this.DoubleBuffered = true;

            _background = Image.FromFile("Resources/Background.png");

            this.Paint += PaintFormArena;
        }

        private void PaintFormArena(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(_background, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
            _pigeon.Draw(e.Graphics);
        }
        
    }
}