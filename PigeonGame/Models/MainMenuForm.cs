using System.Drawing.Drawing2D;
using PigeonGame.Dto;
using PigeonGame.Interfaces;

namespace PigeonGame.Models;

public class MainMenuForm : IArena
{
    public int Width { get; }
    public int Height { get; }

    private readonly Bitmap _background;
    
    
    public MainMenuForm(int width, int height)
    {
        var nextArena = new FirstLevel(width, height);
        NextArena = nextArena;
    }

    public IArena NextArena { get; set; }
    
    // TODO: Вынести в интерфейс UpdatableArena
    public void Update(MovementInput movementInput)
    {
    }

    public void Draw(Graphics graphics)
    {
        throw new NotImplementedException();
    }

    public void DrawNextArena(Graphics graphics)
    {
        throw new NotImplementedException();
    }
    
    private static Bitmap LoadBackground(int width, int height)
    {
        var raw = Image.FromFile("BackgroundForTheMenu.png");
        var bmp = new Bitmap(width, height);
        using var g = Graphics.FromImage(bmp);
        g.InterpolationMode = InterpolationMode.NearestNeighbor;
        g.PixelOffsetMode = PixelOffsetMode.Half;
        g.DrawImage(raw, 0, 0, width, height);
        raw.Dispose();
        return bmp;
    }
    
}