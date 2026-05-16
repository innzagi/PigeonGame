using System.Drawing;

namespace PigeonGame.Interfaces;

public interface IPigeon
{
    int X { get; }
    int Y { get; }

    int Width { get; }
    int Height { get; }

    int Speed { get; }
    int Health { get; }
    int MaxHealth { get; }

    Rectangle Bounds { get; }

    void Move(bool up, bool down, bool left, bool right, int windowWidth, int windowHeight);

    void TakeDamage(int damage);

    void Heal(int value);

    bool IsDead();

    void Draw(Graphics graphics);
}