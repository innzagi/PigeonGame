using System.Collections.Generic;
using System.Drawing;
using PigeonGame.Dto;
using PigeonGame.Helpers;
using PigeonGame.Interfaces;

namespace PigeonGame.Models;

public class FirstLevel : IArena
{
    public int Width { get; }
    public int Height { get; }

    private readonly Bitmap _background;
    private readonly Pigeon _pigeon;
    private readonly Nest _nest;
    private readonly Crow _crow;
    private readonly List<PigeonDropping> _droppings = new();
    private readonly GameOverHelper _gameOver;

    private const int PixelSize = 8;
    private const int HeartGap = 12;
    private const int LevelTransitionDelay = 60;

    private IArena? _nextArena;
    private int _crowDeadTicks;

    public FirstLevel(int width, int height)
    {
        Width = width;
        Height = height;

        _background = BitmapHelper.LoadScaledBitmap("Resources/BackgroundFirstLevel.png", width, height);
        _pigeon = new Pigeon(100, 100, width, height);
        _nest = new Nest(20, 20, width, height);
        _crow = new Crow(width - 200, height / 2f);
        _gameOver = new GameOverHelper(width, height);
    }

    public void Shoot(int targetX, int targetY)
    {
        if (_pigeon.IsDead()) return;
        if (!_pigeon.TryShoot()) return;

        float cx = _pigeon.X + _pigeon.Width / 2f;
        float cy = _pigeon.Y + _pigeon.Height / 2f;
        _droppings.Add(new PigeonDropping(cx, cy, targetX, targetY));
    }

    public void OnLeftClick(int x, int y)
    {
        if (_pigeon.IsDead() && _gameOver.IsRestartClicked(x, y) && _nextArena == null)
            _nextArena = new FirstLevel(Width, Height);
    }

    public IArena? GetNextArena() => _nextArena;

    public void Update(MovementInput movementInput)
    {
        if (_pigeon.IsDead())
            return;

        _pigeon.Move(movementInput);

        if (!_crow.IsDead())
        {
            _crow.Update(_pigeon);
        }
        else
        {
            _crowDeadTicks++;
            if (_crowDeadTicks >= LevelTransitionDelay)
                _nextArena = new SecondLevel(Width, Height);
        }

        for (int i = _droppings.Count - 1; i >= 0; i--)
        {
            var d = _droppings[i];
            d.Update();

            if (!_crow.IsDead() && d.Hits(_crow))
            {
                _crow.TakeDamage(1);
                d.Expire();
            }

            if (d.IsExpired)
                _droppings.RemoveAt(i);
        }
    }

    public void Draw(Graphics graphics)
    {
        graphics.DrawImageUnscaled(_background, 0, 0);
        _nest.Draw(graphics);
        _crow.Draw(graphics);

        foreach (var d in _droppings)
            d.Draw(graphics);

        _pigeon.Draw(graphics);

        DrawHelper.DrawHealthPanel(graphics, _pigeon.Health, _pigeon.MaxHealth,
            panelX: 20, panelY: 20,
            fillColor: Color.Crimson, borderColor: Color.FromArgb(180, 0, 0),
            pixelSize: PixelSize, heartGap: HeartGap, panelPad: 16);

        if (_pigeon.IsDead())
            _gameOver.Draw(graphics);
    }
}
