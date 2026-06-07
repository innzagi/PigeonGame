using System;
using System.Drawing;

namespace PigeonGame.Models;

public enum GroundItemType { Crumb, Cigarette, Beer }

public class GroundItem
{
    public float X    { get; }
    public float Y    { get; }
    public GroundItemType Type { get; }

    private float _currentY;
    private float _angle;
    private int   _glowTick;

    private const float FallSpeed     = 6f;
    private const float RotationSpeed = 4f;

    // Крошка — свой спрайт; окурок и пиво берут размеры из классов снарядов
    private const int CrumbSize = 64;

    private static Bitmap? _crumbSprite;
    private static Bitmap? _cigaretteSprite;
    private static Bitmap? _beerSprite;
    private static Bitmap CrumbSprite      => _crumbSprite     ??= new Bitmap("Resources/Сhit.png");
    private static Bitmap CigaretteSprite  => _cigaretteSprite ??= new Bitmap("Resources/Cigarette.png");
    private static Bitmap BeerSprite       => _beerSprite      ??= new Bitmap("Resources/Beer.png");

    public bool IsOnGround => _currentY >= Y;

    public RectangleF Bounds
    {
        get
        {
            var (w, h) = SizeOf(Type);
            return new RectangleF(X - w / 2f, _currentY - h / 2f, w, h);
        }
    }

    public GroundItem(float x, float y, GroundItemType type)
    {
        X = x;
        Y = y;
        Type = type;
        _currentY = y - 420f;
    }

    // Размеры на земле совпадают с размерами снаряда — меняй в одном месте
    private static (int W, int H) SizeOf(GroundItemType t) => t switch
    {
        GroundItemType.Cigarette => (CigaretteProjectile.W, CigaretteProjectile.H),
        GroundItemType.Beer      => (BeerProjectile.W,      BeerProjectile.H),
        _                        => (CrumbSize, CrumbSize),
    };

    public void Update()
    {
        if (!IsOnGround)
        {
            _currentY += FallSpeed;
            if (_currentY > Y) _currentY = Y;
        }

        // вращается всегда: и падая, и лёжа, и когда лежит
        _angle = (_angle + RotationSpeed) % 360f;

        if (IsOnGround)
            _glowTick++;
    }

    public void Draw(Graphics graphics)
    {
        if (IsOnGround)
            DrawGlow(graphics);

        DrawItem(graphics);
    }

    private void DrawItem(Graphics graphics)
    {
        var (w, h) = SizeOf(Type);
        var state = graphics.Save();

        graphics.TranslateTransform(X, _currentY);
        graphics.RotateTransform(_angle);

        switch (Type)
        {
            case GroundItemType.Cigarette:
                graphics.DrawImage(CigaretteSprite, -w / 2f, -h / 2f, w, h);
                break;
            case GroundItemType.Beer:
                graphics.DrawImage(BeerSprite, -w / 2f, -h / 2f, w, h);
                break;
            default:
                graphics.DrawImage(CrumbSprite, -w / 2f, -h / 2f, w, h);
                break;
        }

        graphics.Restore(state);
    }

    private void DrawGlow(Graphics graphics)
    {
        float pulse = (float)Math.Sin(_glowTick * 0.08);
        var (w, h) = SizeOf(Type);

        Color glowColor = Type switch
        {
            GroundItemType.Cigarette => Color.FromArgb(255, 100, 20),
            GroundItemType.Beer      => Color.FromArgb(210, 165, 20),
            _                        => Color.FromArgb(255, 220, 60),
        };

        DrawGlowRing(graphics, w, h, 44 + pulse * 10, 22, glowColor);
        DrawGlowRing(graphics, w, h, 24 + pulse * 6,  50, glowColor);
        DrawGlowRing(graphics, w, h, 11 + pulse * 3,  85, glowColor);
    }

    private void DrawGlowRing(Graphics graphics, int w, int h, float pad, int alpha, Color color)
    {
        using var brush = new SolidBrush(Color.FromArgb(Math.Clamp(alpha, 0, 255), color));
        graphics.FillEllipse(brush,
            X - w / 2f - pad,
            _currentY - h / 2f - pad * 0.55f,
            w + pad * 2f,
            h + pad * 1.1f);
    }
}
