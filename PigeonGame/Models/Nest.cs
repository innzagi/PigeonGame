using PigeonGame.Interfaces;

namespace PigeonGame.Models;

public class Nest : INest
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Width { get; }
    public int Height { get; }
    
    private readonly int windowWidth;
    private readonly int windowHeight;
    
    public Rectangle Bounds => new Rectangle(X, Y, Width, Height); // отрисовка 

    public Nest(int x, int y, int windowWidth, int windowHeight)
    {
        X = x;
        Y = y;

        Width = 128;
        Height = 128;

        this.windowWidth = windowWidth;
        this.windowHeight = windowHeight;
    }
    
    public void Draw(Graphics graphics)
    {
        graphics.DrawImageUnscaled(Image.FromFile("Resources/nest.svg"), X, Y);
    }
}