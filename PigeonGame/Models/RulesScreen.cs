using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using PigeonGame.Dto;
using PigeonGame.Helpers;
using PigeonGame.Interfaces;

namespace PigeonGame.Models;

// Пиксельная табличка с правилами. Показывается между меню и первым уровнем.
// Табличка рисуется в маленький offscreen-битмап без сглаживания, затем
// масштабируется ×Scale через NearestNeighbor — буквы и рамка остаются пиксельными.
public class RulesScreen : IArena
{
    public int Width  { get; }
    public int Height { get; }

    private readonly Bitmap    _background;
    private readonly Bitmap    _panel;
    private readonly Rectangle _panelRect;

    private IArena? _nextArena;

    private const int Scale = 4;   // во сколько раз увеличиваем логические пиксели
    private const int Lw    = 300; // логическая ширина таблички
    private const int Lh    = 240; // логическая высота таблички

    public RulesScreen(int width, int height)
    {
        Width  = width;
        Height = height;

        MusicPlayer.Play("Intro");

        _background = BitmapHelper.LoadScaledBitmap("Resources/BackgroundForTheMenu.png", width, height);
        _panel      = BuildPanel();

        int pw = Lw * Scale;
        int ph = Lh * Scale;
        _panelRect = new Rectangle((width - pw) / 2, (height - ph) / 2, pw, ph);
    }

    public void Update(MovementInput movementInput) { }

    public void Shoot(int targetX, int targetY) { }

    public void OnLeftClick(int x, int y)
    {
        // Любой клик — начинаем первый уровень
        if (_nextArena == null)
            _nextArena = new FirstLevel(Width, Height);
    }

    public IArena? GetNextArena() => _nextArena;

    public void Draw(Graphics graphics)
    {
        graphics.DrawImageUnscaled(_background, 0, 0);

        using (var overlay = new SolidBrush(Color.FromArgb(150, 0, 0, 0)))
            graphics.FillRectangle(overlay, 0, 0, Width, Height);

        var savedInterp = graphics.InterpolationMode;
        var savedOffset = graphics.PixelOffsetMode;
        graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
        graphics.PixelOffsetMode   = PixelOffsetMode.Half;
        graphics.DrawImage(_panel, _panelRect);
        graphics.InterpolationMode = savedInterp;
        graphics.PixelOffsetMode   = savedOffset;
    }

    // Рисует табличку в логическом разрешении (без сглаживания текста).
    private static Bitmap BuildPanel()
    {
        var bmp = new Bitmap(Lw, Lh);
        using var g = Graphics.FromImage(bmp);
        g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit; // без антиалиасинга → пиксельно
        g.SmoothingMode     = SmoothingMode.None;
        g.PixelOffsetMode   = PixelOffsetMode.Half;

        // фон таблички
        using (var fill = new SolidBrush(Color.FromArgb(38, 28, 52)))
            g.FillRectangle(fill, 0, 0, Lw, Lh);

        // пиксельная рамка в два слоя
        DrawFrame(g, 0, 0, Lw,     Lh,     3, Color.FromArgb(245, 210, 120));
        DrawFrame(g, 4, 4, Lw - 8, Lh - 8, 1, Color.FromArgb(120,  95,  55));

        var titleColor = Color.FromArgb(245, 220, 120);
        var textColor  = Color.FromArgb(235, 235, 235);
        var accentColor = Color.FromArgb(120, 230, 120);

        using var titleFont = new Font("Consolas", 22, FontStyle.Bold, GraphicsUnit.Pixel);
        using var bodyFont  = new Font("Consolas", 14, FontStyle.Bold, GraphicsUnit.Pixel);

        DrawCentered(g, "ПРАВИЛА", titleFont, titleColor, 16);

        string[] lines =
        {
            "WASD   — ДВИЖЕНИЕ",
            "ЛКМ    — ВЫСТРЕЛ",
            "КРОШКА — +1 ЖИЗНЬ",
            "ОКУРОК — УРОН x2",
            "ПИВО   — УРОН x5",
            "ЦЕЛЬ:  ПОБЕДИТЬ ВОРОН",
        };

        int y = 60;
        using (var brush = new SolidBrush(textColor))
        {
            foreach (var line in lines)
            {
                g.DrawString(line, bodyFont, brush, 22, y);
                y += 22;
            }
        }

        DrawCentered(g, "ЖМИ ЛКМ — В БОЙ!", bodyFont, accentColor, Lh - 30);

        return bmp;
    }

    private static void DrawCentered(Graphics g, string text, Font font, Color color, int y)
    {
        var size = g.MeasureString(text, font);
        using var brush = new SolidBrush(color);
        g.DrawString(text, font, brush, (Lw - size.Width) / 2f, y);
    }

    private static void DrawFrame(Graphics g, int x, int y, int w, int h, int t, Color color)
    {
        using var brush = new SolidBrush(color);
        g.FillRectangle(brush, x,         y,         w, t); // верх
        g.FillRectangle(brush, x,         y + h - t, w, t); // низ
        g.FillRectangle(brush, x,         y,         t, h); // лево
        g.FillRectangle(brush, x + w - t, y,         t, h); // право
    }
}
