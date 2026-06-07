using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using PigeonGame.Dto;
using PigeonGame.Helpers;
using PigeonGame.Interfaces;

namespace PigeonGame.Models;

public class SecondLevel : IArena
{
    public int Width { get; }
    public int Height { get; }

    private readonly Bitmap _background;
    private readonly Pigeon _pigeon;
    private readonly Nest _nest;
    private readonly List<Crow> _crows = new();

    private readonly List<PigeonDropping> _droppings = new();

    private const int PixelSize = 8;
    private const int HeartGap = 12;

    // на втором уровне вороны быстрее и бьют чаще, чем на первом
    private const float CrowChaseSpeed = 4.0f;
    private const int CrowAttackInterval = 50;

    public SecondLevel(int width, int height)
    {
        Width = width;
        Height = height;

        _background = LoadBackground(width, height);
        _pigeon = new Pigeon(100, 100, width, height);
        _nest = new Nest(20, 20, width, height);

        _crows.Add(new Crow(width - 200, height / 4f,         CrowChaseSpeed, CrowAttackInterval));
        _crows.Add(new Crow(width - 200, height / 2f,         CrowChaseSpeed, CrowAttackInterval));
        _crows.Add(new Crow(width - 200, height * 3f / 4f,    CrowChaseSpeed, CrowAttackInterval));
    }

    private static Bitmap LoadBackground(int width, int height)
    {
        var raw = Image.FromFile("Resources/BackgroundTwoLewel.png");
        var bmp = new Bitmap(width, height);
        using var g = Graphics.FromImage(bmp);
        g.InterpolationMode = InterpolationMode.NearestNeighbor;
        g.PixelOffsetMode = PixelOffsetMode.Half;
        g.DrawImage(raw, 0, 0, width, height);
        raw.Dispose();
        return bmp;
    }

    public void Shoot(int targetX, int targetY)
    {
        // Стрелять можно только когда прошла перезарядка
        if (!_pigeon.TryShoot()) return;

        float cx = _pigeon.X + _pigeon.Width / 2f;
        float cy = _pigeon.Y + _pigeon.Height / 2f;
        _droppings.Add(new PigeonDropping(cx, cy, targetX, targetY));
    }

    public void OnLeftClick(int x, int y)
    {
        // На втором уровне ЛКМ ничего не делает
    }

    public IArena? GetNextArena() => null;

    public void Update(MovementInput movementInput)
    {
        _pigeon.Move(movementInput);

        foreach (var crow in _crows)
            if (!crow.IsDead())
                crow.Update(_pigeon);

        for (int i = _droppings.Count - 1; i >= 0; i--)
        {
            var d = _droppings[i];
            d.Update();

            foreach (var crow in _crows)
            {
                if (!crow.IsDead() && d.Hits(crow))
                {
                    crow.TakeDamage(1);
                    d.Expire();
                    break;
                }
            }

            if (d.IsExpired)
                _droppings.RemoveAt(i);
        }
    }

    public void Draw(Graphics graphics)
    {
        graphics.DrawImageUnscaled(_background, 0, 0);
        _nest.Draw(graphics);

        foreach (var crow in _crows)
            crow.Draw(graphics);

        foreach (var d in _droppings)
            d.Draw(graphics);

        _pigeon.Draw(graphics);

        DrawHelper.DrawHealthPanel(graphics, _pigeon.Health, _pigeon.MaxHealth,
            panelX: 20, panelY: 20,
            fillColor: Color.Crimson, borderColor: Color.FromArgb(180, 0, 0),
            pixelSize: PixelSize, heartGap: HeartGap, panelPad: 16);
    }
}
