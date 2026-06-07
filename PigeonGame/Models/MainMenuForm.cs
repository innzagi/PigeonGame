using System.Drawing.Drawing2D;
using PigeonGame.Dto;
using PigeonGame.Interfaces;

namespace PigeonGame.Models;

public class MainMenuForm : IArena
{
    public int Width { get; }
    public int Height { get; }

    private readonly Bitmap _background;
    private readonly Bitmap _playButton;
    private readonly Rectangle _playButtonBounds;

    private IArena? _nextArena;

    public MainMenuForm(int width, int height)
    {
        Width = width;
        Height = height;

        _background = LoadBackground(width, height);
        _playButton = new Bitmap("Resources/GameLaunchButton.png");

        // Кнопка по центру экрана. Высота вычисляется пропорционально картинке.
        int btnWidth = 400;
        int btnHeight = (int)(_playButton.Height * (btnWidth / (float)_playButton.Width));
        _playButtonBounds = new Rectangle(
            (width - btnWidth) / 2,
            (height - btnHeight) / 2,
            btnWidth,
            btnHeight);
    }

    public void Update(MovementInput movementInput)
    {
        // На главном экране ничего обновлять не нужно
    }

    public void Shoot(int targetX, int targetY)
    {
        // На главном экране стрелять нельзя
    }

    public void OnLeftClick(int x, int y)
    {
        // Если клик попал в кнопку «Играть» — готовим переход на первый уровень
        if (_playButtonBounds.Contains(x, y) && _nextArena == null)
            _nextArena = new FirstLevel(Width, Height);
    }

    public IArena? GetNextArena() => _nextArena;

    public void Draw(Graphics graphics)
    {
        graphics.DrawImageUnscaled(_background, 0, 0);
        graphics.DrawImage(_playButton, _playButtonBounds);
    }

    private static Bitmap LoadBackground(int width, int height)
    {
        var raw = Image.FromFile("Resources/BackgroundForTheMenu.png");
        var bmp = new Bitmap(width, height);
        using var g = Graphics.FromImage(bmp);
        g.InterpolationMode = InterpolationMode.NearestNeighbor;
        g.PixelOffsetMode = PixelOffsetMode.Half;
        g.DrawImage(raw, 0, 0, width, height);
        raw.Dispose();
        return bmp;
    }
}
