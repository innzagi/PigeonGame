using System.Drawing;
using System.Drawing.Drawing2D;
using PigeonGame.Dto;
using PigeonGame.Interfaces;

namespace PigeonGame.Models;

public class Arena : IArena
{
    public int Width { get; }
    public int Height { get; }

    private readonly Bitmap _background;
    private readonly Pigeon _pigeon;
    private readonly Nest _nest;
    private readonly Crow _crow;

    // пиксельная маска сердечка 7×6: 1 = закрашенный пиксель
    private static readonly int[,] HeartMask =
    {
        { 0, 1, 1, 0, 1, 1, 0 },
        { 1, 1, 1, 1, 1, 1, 1 },
        { 1, 1, 1, 1, 1, 1, 1 },
        { 0, 1, 1, 1, 1, 1, 0 },
        { 0, 0, 1, 1, 1, 0, 0 },
        { 0, 0, 0, 1, 0, 0, 0 },
    };

    private const int PixelSize  = 8;   // размер одного «пикселя» сердечка
    private const int HeartCols  = 7;
    private const int HeartRows  = 6;
    private const int HeartGap   = 12; // отступ между сердечками

    public Arena(int width, int height)
    {
        Width = width;
        Height = height;

        _background = LoadBackground(width, height);
        _pigeon = new Pigeon(100, 100, width, height);
        _nest = new Nest(20, 20, width, height);
        // ворона стартует у правого края экрана
        _crow = new Crow(width - 200, height / 2f);
    }

    private static Bitmap LoadBackground(int width, int height)
    {
        var raw = Image.FromFile("Resources/Background.png");
        var bmp = new Bitmap(width, height);
        using var g = Graphics.FromImage(bmp);
        g.InterpolationMode = InterpolationMode.NearestNeighbor;
        g.PixelOffsetMode = PixelOffsetMode.Half;
        g.DrawImage(raw, 0, 0, width, height);
        raw.Dispose();
        return bmp;
    }

    public void Update(MovementInput movementInput)
    {
        _pigeon.Move(movementInput);
        _crow.Update(_pigeon);
    }

    public void Draw(Graphics graphics)
    {
        graphics.DrawImageUnscaled(_background, 0, 0);
        _nest.Draw(graphics);
        _crow.Draw(graphics);
        _pigeon.Draw(graphics);
        DrawHealthPanel(graphics, _pigeon.Health, _pigeon.MaxHealth,
            panelX: 20, panelY: 20,
            fillColor: Color.Crimson, borderColor: Color.FromArgb(180, 0, 0));

        int crowPanelH = HeartRows * PixelSize + 32; // panelPad*2 + heartHeight
        DrawHealthPanel(graphics, _crow.Health, _crow.MaxHealth,
            panelX: (int)_crow.X + _crow.Width + 8,
            panelY: (int)_crow.Y + (_crow.Height - crowPanelH) / 2,
            fillColor: Color.MediumPurple, borderColor: Color.FromArgb(80, 0, 120));
    }

    private void DrawHealthPanel(Graphics graphics, int health, int maxHealth,
        int panelX, int panelY, Color fillColor, Color borderColor)
    {
        int heartWidth  = HeartCols * PixelSize;
        int heartHeight = HeartRows * PixelSize;
        int totalWidth  = maxHealth * heartWidth + (maxHealth - 1) * HeartGap;

        const int panelPad = 16;
        int panelW = totalWidth  + panelPad * 2;
        int panelH = heartHeight + panelPad * 2;

        using var bg = new SolidBrush(Color.FromArgb(140, 0, 0, 0));
        graphics.FillRectangle(bg, panelX, panelY, panelW, panelH);
        graphics.DrawRectangle(Pens.White, panelX, panelY, panelW, panelH);

        int originX = panelX + panelPad;
        int originY = panelY + panelPad;

        for (int i = 0; i < maxHealth; i++)
        {
            int hx = originX + i * (heartWidth + HeartGap);
            DrawPixelHeart(graphics, hx, originY, filled: i < health, fillColor, borderColor);
        }
    }

    private static void DrawPixelHeart(Graphics graphics, int x, int y, bool filled,
        Color fillColor, Color borderColor)
    {
        var fc = filled ? fillColor         : Color.FromArgb(60, 60, 60);
        var bc = filled ? borderColor       : Color.FromArgb(100, 100, 100);

        using var fillBrush   = new SolidBrush(fc);
        using var borderBrush = new SolidBrush(bc);

        for (int row = 0; row < HeartRows; row++)
        {
            for (int col = 0; col < HeartCols; col++)
            {
                if (HeartMask[row, col] == 0) continue;

                graphics.FillRectangle(borderBrush,
                    x + col * PixelSize - 1,
                    y + row * PixelSize - 1,
                    PixelSize + 2, PixelSize + 2);

                graphics.FillRectangle(fillBrush,
                    x + col * PixelSize,
                    y + row * PixelSize,
                    PixelSize, PixelSize);
            }
        }
    }
    
}
