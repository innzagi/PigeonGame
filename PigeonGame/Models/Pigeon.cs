using System.Drawing;
using PigeonGame.Dto;
using PigeonGame.Interfaces;

namespace PigeonGame.Models;

public class Pigeon : IPigeon
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public int Width { get; }
    public int Height { get; }

    public int Speed { get; }
    public int Health { get; private set; }
    public int MaxHealth { get; }

    private readonly int windowWidth;
    private readonly int windowHeight;

    private readonly Bitmap[] walkFrames;
    private readonly Bitmap[] flyFrames;

    private int frameIndex;
    private int tickCounter;
    private const int TicksPerFrame = 6;

    private bool _facingLeft;

    // неуязвимость после получения урона (i-frames)
    private int _invulnerableTicks;
    private const int InvulnerabilityDuration = 60; // ~1.2 сек при 20мс/тик
    public bool IsInvulnerable => _invulnerableTicks > 0;

    // перезарядка атаки (любым оружием)
    private int _shootCooldownTicks;
    private const int ShootCooldownDuration = 40;
    public bool CanShoot => _shootCooldownTicks <= 0;

    // ground level: bottom 25% of the window matches the pavement in the background
    private int GroundY => (int)(windowHeight * 0.75);
    private bool IsOnGround => Y + Height >= GroundY;

    public Rectangle Bounds => new Rectangle(X, Y, Width, Height);

    public Pigeon(int x, int y, int windowWidth, int windowHeight)
    {
        X = x;
        Y = y;

        Width = 128;
        Height = 128;

        Speed = 10;

        MaxHealth = 5;
        Health = MaxHealth;

        this.windowWidth = windowWidth;
        this.windowHeight = windowHeight;

        walkFrames = SliceSheet("Resources/pigeon_walking-Sheet.png", 4, 32, 32);
        flyFrames  = SliceSheet("Resources/pigeon_fiy-Sheet.png",     7, 32, 32);
    }

    private Bitmap[] SliceSheet(string path, int frameCount, int frameWidth, int frameHeight)
    {
        var sheet = new Bitmap(path);
        var frames = new Bitmap[frameCount];
        for (var i = 0; i < frameCount; i++)
        {
            var frame = new Bitmap(Width, Height);
            using var g = Graphics.FromImage(frame);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            g.DrawImage(sheet, new Rectangle(0, 0, Width, Height),
                new Rectangle(i * frameWidth, 0, frameWidth, frameHeight), GraphicsUnit.Pixel);
            frames[i] = frame;
        }
        sheet.Dispose();
        return frames;
    }

    public void Move(MovementInput movementInput)
    {
        if (_invulnerableTicks > 0)
            _invulnerableTicks--;

        if (_shootCooldownTicks > 0)
            _shootCooldownTicks--;

        if (movementInput.Left)  { X -= Speed; _facingLeft = true;  }
        if (movementInput.Right) { X += Speed; _facingLeft = false; }
        if (movementInput.Up)    Y -= Speed;
        if (movementInput.Down)  Y += Speed;

        KeepInsideWindow();
        AdvanceAnimation();
    }

    private void AdvanceAnimation()
    {
        tickCounter++;
        if (tickCounter < TicksPerFrame)
            return;

        tickCounter = 0;
        var frames = IsOnGround ? walkFrames : flyFrames;
        frameIndex = (frameIndex + 1) % frames.Length;
    }

    private void KeepInsideWindow()
    {
        if (X < 0) X = 0;
        if (Y < 0) Y = 0;
        if (X + Width > windowWidth)   X = windowWidth - Width;
        if (Y + Height > windowHeight) Y = windowHeight - Height;
    }

    public void TakeDamage(int damage)
    {
        // во время неуязвимости урон не проходит
        if (_invulnerableTicks > 0) return;

        Health -= damage;
        if (Health < 0) Health = 0;

        // запускаем неуязвимость, чтобы следующий удар (в т.ч. от других ворон)
        // не прошёл сразу в этом же или ближайших кадрах
        _invulnerableTicks = InvulnerabilityDuration;
    }

    public void Heal(int value)
    {
        Health += value;
        if (Health > MaxHealth) Health = MaxHealth;
    }

    public bool IsDead() => Health <= 0;

    // Пытается выстрелить: если перезарядка прошла — запускает её заново и
    // возвращает true. Иначе (ещё идёт перезарядка) возвращает false.
    public bool TryShoot()
    {
        if (_shootCooldownTicks > 0) return false;

        _shootCooldownTicks = ShootCooldownDuration;
        return true;
    }

    public void Draw(Graphics graphics)
    {
        if (_shootCooldownTicks > 0)
            DrawCooldownBar(graphics);

        // мигаем во время неуязвимости: пропускаем отрисовку каждые ~5 кадров
        if (_invulnerableTicks > 0 && (_invulnerableTicks / 5) % 2 == 0)
            return;

        var frames = IsOnGround ? walkFrames : flyFrames;
        var frame = frames[frameIndex % frames.Length];

        if (_facingLeft)
        {
            // отзеркаливаем по горизонтали через трансформацию
            graphics.TranslateTransform(X + Width, Y);
            graphics.ScaleTransform(-1, 1);
            graphics.DrawImageUnscaled(frame, 0, 0);
            graphics.ResetTransform();
        }
        else
        {
            graphics.DrawImageUnscaled(frame, X, Y);
        }
    }

    private void DrawCooldownBar(Graphics graphics)
    {
        const int barHeight = 4;
        const int barOffset = 10;

        int barX = X;
        int barY = Y - barOffset;
        float ratio = 1f - (float)_shootCooldownTicks / ShootCooldownDuration;
        int fillWidth = (int)(Width * ratio);

        using var bgBrush = new SolidBrush(Color.FromArgb(120, 0, 0, 0));
        graphics.FillRectangle(bgBrush, barX, barY, Width, barHeight);

        if (fillWidth > 0)
        {
            using var fillBrush = new SolidBrush(Color.FromArgb(255, 60, 210, 60));
            graphics.FillRectangle(fillBrush, barX, barY, fillWidth, barHeight);
        }
    }
}
