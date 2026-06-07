using System.Drawing;

namespace PigeonGame.Interfaces;

public interface IProjectile
{
    float X { get; }
    float Y { get; }
    bool IsExpired { get; }
    int Damage { get; }

    void Update();
    void Draw(Graphics graphics);
    void Expire();
    bool Hits(ICrow crow);
}
