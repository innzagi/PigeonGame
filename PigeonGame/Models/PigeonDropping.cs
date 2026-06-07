using System.Drawing;

namespace PigeonGame.Models;

public class PigeonDropping : BaseProjectile
{
    private const int Size = 16;
    public override int Damage => 1;

    public PigeonDropping(float startX, float startY, float targetX, float targetY)
        : base(startX, startY, targetX, targetY) { }

    public override void Draw(Graphics graphics)
    {
        using var dark = new SolidBrush(Color.FromArgb(80, 45, 10));
        graphics.FillEllipse(dark, X - Size / 2f, Y - Size / 2f, Size, Size);

        using var mid = new SolidBrush(Color.FromArgb(120, 75, 20));
        graphics.FillEllipse(mid, X - Size / 2f + 2, Y - Size / 2f + 2, Size - 4, Size - 4);

        using var hi = new SolidBrush(Color.FromArgb(160, 255, 255, 255));
        graphics.FillEllipse(hi, X - Size / 2f + 3, Y - Size / 2f + 2, 4, 3);
    }
}
