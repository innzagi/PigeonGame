using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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

    private const int PixelSize = 8;
    private const int HeartGap = 12;

    // Таблички экрана Game Over
    private readonly Bitmap _gameOverImage;
    private readonly Bitmap _restartImage;
    private readonly Rectangle _gameOverRect;     // куда рисуем картинку Game Over
    private readonly Rectangle _restartRect;       // куда рисуем картинку Restart
    private readonly Rectangle _restartButtonBounds; // кликабельная зона (видимая часть таблички)

    // Если игрок нажал Restart — сюда кладётся новый уровень
    private IArena? _nextArena;

    public FirstLevel(int width, int height)
    {
        Width = width;
        Height = height;

        _background = LoadBackground(width, height);
        _pigeon = new Pigeon(100, 100, width, height);
        _nest = new Nest(20, 20, width, height);
        _crow = new Crow(width - 200, height / 2f);

        _gameOverImage = new Bitmap("Resources/GameOver.png");
        _restartImage = new Bitmap("Resources/Restart.png");

        // Картинка Game Over — по центру, выше середины экрана.
        // У картинок большие прозрачные поля, поэтому позиционируем
        // по центру видимой таблички (она ~в центре PNG).
        const int goW = 700;
        int goH = (int)(_gameOverImage.Height * (goW / (float)_gameOverImage.Width));
        _gameOverRect = new Rectangle((width - goW) / 2, height / 2 - 110 - goH / 2, goW, goH);

        // Картинка Restart — ниже середины экрана
        const int rsW = 460;
        int rsH = (int)(_restartImage.Height * (rsW / (float)_restartImage.Width));
        _restartRect = new Rectangle((width - rsW) / 2, height / 2 + 110 - rsH / 2, rsW, rsH);

        // Видимая табличка занимает примерно центр PNG (по ширине ~90%, по высоте ~43%).
        // По этой зоне ловим клик, чтобы не реагировать на прозрачные поля.
        _restartButtonBounds = new Rectangle(
            _restartRect.X + (int)(rsW * 0.05f),
            _restartRect.Y + (int)(rsH * 0.28f),
            (int)(rsW * 0.90f),
            (int)(rsH * 0.43f));
    }

    private static Bitmap LoadBackground(int width, int height)
    {
        var raw = Image.FromFile("Resources/BackgroundFirstLevel.png");
        var bmp = new Bitmap(width, height);
        using var g = Graphics.FromImage(bmp);
        g.InterpolationMode = InterpolationMode.NearestNeighbor;
        g.PixelOffsetMode = PixelOffsetMode.Half;
        g.DrawImage(raw, 0, 0, width, height);
        raw.Dispose();
        return bmp;
    }

    public void Shoot(int targetX, int targetY)
    {
        // Стрелять можно только живым и когда прошла перезарядка
        if (_pigeon.IsDead()) return;
        if (!_pigeon.TryShoot()) return;

        float cx = _pigeon.X + _pigeon.Width / 2f;
        float cy = _pigeon.Y + _pigeon.Height / 2f;
        _droppings.Add(new PigeonDropping(cx, cy, targetX, targetY));
    }

    public void OnLeftClick(int x, int y)
    {
        // Restart работает только на экране Game Over
        if (_pigeon.IsDead() && _restartButtonBounds.Contains(x, y) && _nextArena == null)
            _nextArena = new FirstLevel(Width, Height);
    }

    public IArena? GetNextArena() => _nextArena;

    public void Update(MovementInput movementInput)
    {
        // Голубь мёртв — игра заморожена, ждём нажатия Restart
        if (_pigeon.IsDead())
            return;

        _pigeon.Move(movementInput);

        if (!_crow.IsDead())
            _crow.Update(_pigeon);

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
            DrawGameOver(graphics);
    }

    private void DrawGameOver(Graphics graphics)
    {
        // Затемняем весь экран
        using var overlay = new SolidBrush(Color.FromArgb(180, 0, 0, 0));
        graphics.FillRectangle(overlay, 0, 0, Width, Height);

        // Готовые таблички из Resources
        graphics.DrawImage(_gameOverImage, _gameOverRect);
        graphics.DrawImage(_restartImage, _restartRect);
    }
}
