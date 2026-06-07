using System.Drawing;
using PigeonGame.Models;

namespace PigeonGame.Interfaces;

public interface ICrow
{
    float X { get; }
    float Y { get; }
    int Width { get; }
    int Height { get; }
    void Update(Pigeon pigeon);
    void Draw(Graphics graphics);
    void TakeDamage(int damage);
    bool IsDead();
}
