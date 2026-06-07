using System.Drawing;

namespace PigeonGame.Models;

public class BeerProjectile : BaseProjectile
{
    private static Bitmap? _sprite;
    private static Bitmap Sprite => _sprite ??= new Bitmap("Resources/Beer.png");

    public const int W = 32 * 5;
    public const int H = 40 * 5;

    public override int Damage => 5;
    protected override float RotationSpeed => 3f;

    public BeerProjectile(float startX, float startY, float targetX, float targetY)
        : base(startX, startY, targetX, targetY, speed: 10f) { }

    public override void Draw(Graphics graphics)
    {
        var state = graphics.Save();
        graphics.TranslateTransform(X, Y);
        graphics.RotateTransform(_angle);
        graphics.DrawImage(Sprite, -W / 2f, -H / 2f, W, H);
        graphics.Restore(state);
    }
}
