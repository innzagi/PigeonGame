using System.Drawing;

namespace PigeonGame.Models;

public class CigaretteProjectile : BaseProjectile
{
    private static Bitmap? _sprite;
    private static Bitmap Sprite => _sprite ??= new Bitmap("Resources/Cigarette.png");

    public const int W = 30 * 4;
    public const int H = 20 * 4;

    public override int Damage => 2;
    protected override float RotationSpeed => 5f;

    public CigaretteProjectile(float startX, float startY, float targetX, float targetY)
        : base(startX, startY, targetX, targetY, speed: 12f) { }

    public override void Draw(Graphics graphics)
    {
        var state = graphics.Save();
        graphics.TranslateTransform(X, Y);
        graphics.RotateTransform(_angle);
        graphics.DrawImage(Sprite, -W / 2f, -H / 2f, W, H);
        graphics.Restore(state);
    }
}
