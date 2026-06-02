using PigeonGame.Interfaces;

namespace PigeonGame.Models;

public class Nest : INest
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Width { get; }
    public int Height { get; }
    
    private readonly Image image;

    private readonly int windowWidth;
    private readonly int windowHeight;
    
    public Rectangle Bounds => new Rectangle(X, Y, Width, Height); // отрисовка 

    public Nest(int x, int y, int windowWidth, int windowHeight)
    {
        X = x;
        Y = 800;

        Width = 300;
        Height = 300;

        this.windowWidth = windowWidth;
        this.windowHeight = windowHeight;
        image = Image.FromFile("Resources/nest.png");
    }
    
    public void Draw(Graphics graphics)
    {
        graphics.DrawImage(image, X, Y, Width, Height);
    }
}