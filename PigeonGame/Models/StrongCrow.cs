using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using PigeonGame.Helpers;
using PigeonGame.Interfaces;

namespace PigeonGame.Models;

public class StrongCrow : ICrow
{
    public float X { get; private set; }
    public float Y { get; private set; }

    public int Width  { get; } = 250;
    public int Height { get; } = 250;
    public int Health     { get; private set; }
    public int MaxHealth  { get; } = 10;

    private const float ChaseSpeed     = 9.0f;
    private const int   AttackInterval = 90;
    private const int   AttackDamage   = 2;
    private const float AttackDistance = 80f;

    private CrowState _state = CrowState.Chasing;

    private readonly Bitmap[] _flyFrames;
    private readonly Bitmap[] _stayFrames;

    private int _frameIndex;
    private int _tickCounter;
    private const int TicksPerFrame = 6;

    private int  _attackCooldown;
    private bool _facingLeft;

    private const int SheetCols = 4;
    private const int SheetRows = 2;

    private const int PixelSize = 8;
    private const int HeartCols = 7;
    private const int HeartRows = 6;
    private const int HeartGap  = 12;

    public StrongCrow(float startX, float startY)
    {
        X = startX;
        Y = startY;
        Health = MaxHealth;

        _flyFrames  = SliceSheet("Resources/StrongCrowFly.png",  SheetCols, SheetRows);
        _stayFrames = SliceSheet("Resources/StrongCrowStay.png", SheetCols, SheetRows);
    }

    private Bitmap[] SliceSheet(string path, int cols, int rows)
    {
        var sheet  = new Bitmap(path);
        float frameW = sheet.Width  / (float)cols;
        float frameH = sheet.Height / (float)rows;

        var frames = new Bitmap[cols * rows];
        var index  = 0;

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                var frame = new Bitmap(Width, Height);
                using var g = Graphics.FromImage(frame);
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode   = PixelOffsetMode.Half;
                g.DrawImage(sheet,
                    new RectangleF(0, 0, Width, Height),
                    new RectangleF(col * frameW, row * frameH, frameW, frameH),
                    GraphicsUnit.Pixel);
                frames[index++] = frame;
            }
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
                pigeon.TakeDamage(AttackDamage);
                _attackCooldown = AttackInterval;
            }
            else
            {
                _attackCooldown--;
            }
        }
        else
        {
            _state          = CrowState.Chasing;
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
        var frames = _state == CrowState.Attacking ? _stayFrames : _flyFrames;
        _frameIndex = (_frameIndex + 1) % frames.Length;
    }

    public void Draw(Graphics graphics)
    {
        if (IsDead()) return;

        var frames = _state == CrowState.Attacking ? _stayFrames : _flyFrames;
        var frame  = frames[_frameIndex % frames.Length];

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

        const int crowPixel = PixelSize / 2;
        const int crowGap   = HeartGap  / 2;
        const int crowPad   = 8;
        int panelW = MaxHealth * (HeartCols * crowPixel) + (MaxHealth - 1) * crowGap + crowPad * 2;
        int panelH = HeartRows * crowPixel + crowPad * 2;

        DrawHelper.DrawHealthPanel(graphics, Health, MaxHealth,
            panelX: (int)X + (Width - panelW) / 2,
            panelY: (int)Y - panelH + 80,
            fillColor: Color.OrangeRed, borderColor: Color.FromArgb(160, 40, 0),
            pixelSize: crowPixel, heartGap: crowGap, panelPad: crowPad);
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health < 0) Health = 0;
    }

    public bool IsDead() => Health <= 0;
}
