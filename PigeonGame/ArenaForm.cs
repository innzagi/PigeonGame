using System;
using System.Windows.Forms;
using PigeonGame.Dto;
using PigeonGame.Models;

namespace PigeonGame
{
    public partial class ArenaForm : Form
    {
        // TODO: Заменить на MainMenuForm
        private FirstLevel _firstLevel;
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
            MouseDown += OnMouseDown;
        }

        private void StartGame()
        {
            // TODO: Заменить на MainMenuForm
            _firstLevel = new FirstLevel(ClientSize.Width, ClientSize.Height);
            _timer.Interval = 20;
            _timer.Tick += OnTick;
            _timer.Start();
        }

        private void OnTick(object? sender, EventArgs e)
        {
            _firstLevel.Update(_movementInput);
            Invalidate();
        }

        private void OnPaint(object? sender, PaintEventArgs e)
        {
            _firstLevel.Draw(e.Graphics);
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
        private void OnMouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                _firstLevel.Shoot(e.X, e.Y);
        }
    }
}
