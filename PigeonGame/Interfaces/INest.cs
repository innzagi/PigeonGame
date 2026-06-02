namespace PigeonGame.Interfaces;

public interface INest
{
    int X { get; }
    int Y { get; }

    int Width { get; }
    int Height { get; }
    
    Rectangle Bounds { get; }
    
    void Draw(Graphics graphics);
}