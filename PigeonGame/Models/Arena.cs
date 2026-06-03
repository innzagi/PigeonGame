using System.Drawing;
using System.Drawing.Drawing2D;
using PigeonGame.Dto;
using PigeonGame.Helpers;
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


    private const int PixelSize  = 8;   // размер одного «пикселя» сердечка
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
        DrawHelper.DrawHealthPanel(graphics, _pigeon.Health, _pigeon.MaxHealth,
            panelX: 20, panelY: 20,
            fillColor: Color.Crimson, borderColor: Color.FromArgb(180, 0, 0),
            pixelSize: PixelSize, heartGap: HeartGap, panelPad: 16);
    }

   
    
}
