using System;
using System.Drawing;
using PigeonGame.Interfaces;

namespace PigeonGame.Models;

public class PigeonDropping
{
    public float X { get; private set; }
    public float Y { get; private set; }

    private readonly float _velX;
    private readonly float _velY;

    private const float Speed       = 14f;
    private const int   Size        = 16;
    private const int   MaxLifetime = 150;

    private int  _lifetime;
    public  bool IsExpired { get; private set; }

    public PigeonDropping(float startX, float startY, float targetX, float targetY)
    {
        X = startX;
        Y = startY;

        float dx   = targetX - startX;
        float dy   = targetY - startY;
        float dist = MathF.Sqrt(dx * dx + dy * dy);
        if (dist < 1f) dist = 1f;

        _velX = dx / dist * Speed;
        _velY = dy / dist * Speed;
    }

    public void Update()
    {
        X += _velX;
        Y += _velY;
        _lifetime++;
        if (_lifetime >= MaxLifetime)
            IsExpired = true;
    }

    public bool Hits(ICrow crow)
    {
        var bounds = new RectangleF(crow.X + crow.Width * 0.075f,
            crow.Y + crow.Height * 0.075f,
            crow.Width  * 0.85f,
            crow.Height * 0.85f);
        return bounds.Contains(X, Y);
    }

    public void Expire() => IsExpired = true;

    public void Draw(Graphics graphics)
    {
        using var dark = new SolidBrush(Color.FromArgb(80, 45, 10));
        graphics.FillEllipse(dark, X - Size / 2f, Y - Size / 2f, Size, Size);

        using var mid = new SolidBrush(Color.FromArgb(120, 75, 20));
        graphics.FillEllipse(mid, X - Size / 2f + 2, Y - Size / 2f + 2, Size - 4, Size - 4);

        using var hi = new SolidBrush(Color.FromArgb(160, 255, 255, 255));
        graphics.FillEllipse(hi, X - Size / 2f + 3, Y - Size / 2f + 2, 4, 3);
    }
}