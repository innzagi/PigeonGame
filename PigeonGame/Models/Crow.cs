using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using PigeonGame.Interfaces;

namespace PigeonGame.Models;

public enum CrowState { Chasing, Attacking }

public class Crow : ICrow
{
    public float X { get; private set; }
    public float Y { get; private set; }

    public int Width     { get; } = 128;
    public int Height    { get; } = 128;
    public int Health    { get; private set; }
    public int MaxHealth { get; } = 5;

    private CrowState _state = CrowState.Chasing;

    private readonly Bitmap[] _flyFrames;
    private readonly Bitmap[] _attackFrames;

    private int _frameIndex;
    private int _tickCounter;
    private const int TicksPerFrame = 6;

    // атака: интервал между уронами
    private int _attackCooldown;
    private const int AttackInterval = 90; // ~1.8 сек при 20мс тике

    // дистанция от центра до центра, при которой начинается атака
    private const float AttackDistance = 80f;
    private const float ChaseSpeed = 2.5f;

    private bool _facingLeft;

    // спрайт: 4 строки × 4 столбца, каждый кадр 48×48
    private const int FrameSize = 48;
    private const int FramesPerRow = 4;
    private const int FlyRow    = 3;
    private const int AttackRow = 1;

    public Crow(float startX, float startY)
    {
        X = startX;
        Y = startY;
        Health = MaxHealth;

        _flyFrames    = SliceRow("Resources/Crow.png", FlyRow);
        _attackFrames = SliceRow("Resources/Crow.png", AttackRow);
    }

    private Bitmap[] SliceRow(string path, int row)
    {
        var sheet = new Bitmap(path);
        var frames = new Bitmap[FramesPerRow];

        for (var i = 0; i < FramesPerRow; i++)
        {
            var frame = new Bitmap(Width, Height);
            using var g = Graphics.FromImage(frame);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode   = PixelOffsetMode.Half;
            g.DrawImage(sheet,
                new Rectangle(0, 0, Width, Height),
                new Rectangle(i * FrameSize, row * FrameSize, FrameSize, FrameSize),
                GraphicsUnit.Pixel);
            frames[i] = frame;
        }

        sheet.Dispose();
        return frames;
    }

    public void Update(Pigeon pigeon)
    {
        float pigeonCx = pigeon.X + pigeon.Width  / 2f;
        float pigeonCy = pigeon.Y + pigeon.Height / 2f;
        float crowCx   = X + Width  / 2f;
        float crowCy   = Y + Height / 2f;

        float dx   = pigeonCx - crowCx;
        float dy   = pigeonCy - crowCy;
        float dist = MathF.Sqrt(dx * dx + dy * dy);

        if (dist <= AttackDistance)
        {
            _state = CrowState.Attacking;

            if (_attackCooldown <= 0)
            {
                pigeon.TakeDamage(1);
                _attackCooldown = AttackInterval;
            }
            else
            {
                _attackCooldown--;
            }
        }
        else
        {
            _state = CrowState.Chasing;
            _attackCooldown = 0;

            X += dx / dist * ChaseSpeed;
            Y += dy / dist * ChaseSpeed;

            _facingLeft = dx < 0;
        }

        AdvanceAnimation();
    }

    private void AdvanceAnimation()
    {
        _tickCounter++;
        if (_tickCounter < TicksPerFrame) return;

        _tickCounter = 0;
        var frames = _state == CrowState.Attacking ? _attackFrames : _flyFrames;
        _frameIndex = (_frameIndex + 1) % frames.Length;
    }

    public void Draw(Graphics graphics)
    {
        var frames = _state == CrowState.Attacking ? _attackFrames : _flyFrames;
        var frame  = frames[_frameIndex];

        int drawX = (int)X;
        int drawY = (int)Y;

        if (_facingLeft)
        {
            graphics.TranslateTransform(drawX + Width, drawY);
            graphics.ScaleTransform(-1, 1);
            graphics.DrawImageUnscaled(frame, 0, 0);
            graphics.ResetTransform();
        }
        else
        {
            graphics.DrawImageUnscaled(frame, drawX, drawY);
        }
    }
}
