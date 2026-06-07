using System;
using System.Drawing;
using PigeonGame.Interfaces;

namespace PigeonGame.Models;

public abstract class BaseProjectile : IProjectile
{
    public float X { get; protected set; }
    public float Y { get; protected set; }
    public bool IsExpired { get; private set; }
    public abstract int Damage { get; }

    protected readonly float VelX;
    protected readonly float VelY;

    protected float _angle;
    protected virtual float RotationSpeed => 0f;

    private int _lifetime;
    private readonly int _maxLifetime;

    protected BaseProjectile(float startX, float startY, float targetX, float targetY,
        float speed = 14f, int maxLifetime = 150)
    {
        X = startX;
        Y = startY;
        _maxLifetime = maxLifetime;

        float dx   = targetX - startX;
        float dy   = targetY - startY;
        float dist = MathF.Sqrt(dx * dx + dy * dy);
        if (dist < 1f) dist = 1f;

        VelX = dx / dist * speed;
        VelY = dy / dist * speed;
    }

    public virtual void Update()
    {
        X += VelX;
        Y += VelY;
        _angle = (_angle + RotationSpeed) % 360f;
        _lifetime++;
        if (_lifetime >= _maxLifetime) IsExpired = true;
    }

    public bool Hits(ICrow crow)
    {
        var bounds = new RectangleF(
            crow.X + crow.Width  * 0.075f,
            crow.Y + crow.Height * 0.075f,
            crow.Width  * 0.85f,
            crow.Height * 0.85f);
        return bounds.Contains(X, Y);
    }

    public void Expire() => IsExpired = true;

    public abstract void Draw(Graphics graphics);
}
