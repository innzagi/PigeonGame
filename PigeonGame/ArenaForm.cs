using System;
using System.Windows.Forms;
using PigeonGame.Dto;
using PigeonGame.Models;

namespace PigeonGame
{
    public partial class ArenaForm : Form
    {
        private Arena _arena;
        private readonly System.Windows.Forms.Timer _timer = new();
        private MovementInput _movementInput = new();

        public ArenaForm()
        {
            InitializeComponent();
            SetupForm();
            StartGame();
        }

        private void SetupForm()
        {
            Text = "Голубиный дозор";
            Width = 1920;
            Height = 1080;
            DoubleBuffered = true;
            Paint += OnPaint;
            KeyDown += OnKeyDown;
            KeyUp += OnKeyUp;
        }

        private void StartGame()
        {
            _arena = new Arena(ClientSize.Width, ClientSize.Height);
            _timer.Interval = 20;
            _timer.Tick += OnTick;
            _timer.Start();
        }

        private void OnTick(object? sender, EventArgs e)
        {
            _arena.Update(_movementInput);
            Invalidate();
        }

        private void OnPaint(object? sender, PaintEventArgs e)
        {
            _arena.Draw(e.Graphics);
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W) _movementInput.Up    = true;
            if (e.KeyCode == Keys.S) _movementInput.Down  = true;
            if (e.KeyCode == Keys.A) _movementInput.Left  = true;
            if (e.KeyCode == Keys.D) _movementInput.Right = true;
        }

        private void OnKeyUp(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W) _movementInput.Up    = false;
            if (e.KeyCode == Keys.S) _movementInput.Down  = false;
            if (e.KeyCode == Keys.A) _movementInput.Left  = false;
            if (e.KeyCode == Keys.D) _movementInput.Right = false;
        }
    }
}
