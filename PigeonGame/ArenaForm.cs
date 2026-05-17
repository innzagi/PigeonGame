using System;
using System.Drawing;
using System.Windows.Forms;
using PigeonGame.Dto;
using PigeonGame.Models;
using PigeonGame.Services;

namespace PigeonGame
{
    public partial class ArenaForm : Form
    {
        private Image _pigeonImage;
        private GameProcess _gameProcess;
        private Pigeon _pigeon;
        private readonly System.Windows.Forms.Timer _timer = new();
        private MovementInput _movementInput = new();

        private Image _background;

        public ArenaForm()
        {
            InitializeComponent();
            SetupArena();
            StartGame();
        }

        private void StartGame()
        {
            _pigeon = new Pigeon(100, 100, _pigeonImage, Width, Height);
            _gameProcess = new GameProcess(_pigeon);
            _timer.Interval = 20;
            _timer.Tick += GameTimer_Tick;
            _timer.Start();
        }

        private void GameTimer_Tick(object? sender, EventArgs e)
        {
            _gameProcess.Update(_movementInput);
            Invalidate();
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
            this.KeyDown += ArenaForm_KeyDown;
            this.KeyUp += ArenaForm_KeyUp;
        }

        private void ArenaForm_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
                _movementInput.Up = true;

            if (e.KeyCode == Keys.S)
                _movementInput.Down = true;

            if (e.KeyCode == Keys.A)
                _movementInput.Left = true;

            if (e.KeyCode == Keys.D)
                _movementInput.Right = true;
        }

        private void ArenaForm_KeyUp(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
                _movementInput.Up = false;

            if (e.KeyCode == Keys.S)
                _movementInput.Down = false;

            if (e.KeyCode == Keys.A)
                _movementInput.Left = false;

            if (e.KeyCode == Keys.D)
                _movementInput.Right = false;
        }

        private void PaintFormArena(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(_background, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
            _pigeon.Draw(e.Graphics);
        }
    }
}