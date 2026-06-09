using System.Drawing;

namespace PigeonGame.Models;

public class PigeonDropping : BaseProjectile
{
    private static Bitmap? _sprite;
    private static Bitmap Sprite => _sprite ??= new Bitmap("Resources/pop.png");

    public const int W = 16 * 3;
    public const int H = 16 * 3;

    public override int Damage => 1;

    public PigeonDropping(float startX, float startY, float targetX, float targetY)
        : base(startX, startY, targetX, targetY) { }

    public override void Draw(Graphics graphics)
    {
        var state = graphics.Save();
        graphics.TranslateTransform(X, Y);
        graphics.RotateTransform(_angle);
        graphics.DrawImage(Sprite, -W / 2f, -H / 2f, W, H);
        graphics.Restore(state);
    }
}
