using System;
using System.Collections.Generic;
using System.Drawing;
using PigeonGame.Dto;
using PigeonGame.Helpers;
using PigeonGame.Interfaces;

namespace PigeonGame.Models;

public class ThirdLevel : IArena
{
    public int Width  { get; }
    public int Height { get; }

    private readonly Bitmap _background;
    private readonly Pigeon _pigeon;
    private readonly Nest   _nest;
    private readonly List<ICrow>       _crows      = new();
    private readonly List<IProjectile> _projectiles = new();
    private readonly List<GroundItem>  _groundItems = new();
    private readonly GameOverHelper    _gameOver;

    private const int   PixelSize          = 8;
    private const int   HeartGap           = 12;
    private const float CrowChaseSpeed     = 7.0f;
    private const int   CrowAttackInterval = 50;
    private const int   SpawnInterval      = 300;

    private IArena? _nextArena;
    private int     _cigaretteAmmo;
    private int     _beerAmmo;

    private readonly Queue<Func<ICrow>> _spawnQueue = new();
    private int _spawnTick;

    private readonly Queue<(int At, GroundItemType Type, float Xf)> _dropSchedule = new();
    private int _levelTick;

    public ThirdLevel(int width, int height)
    {
        Width  = width;
        Height = height;

        _background = BitmapHelper.LoadScaledBitmap("Resources/ВackgroundThirdLevel.png", width, height);
        _pigeon  = new Pigeon(100, 100, width, height);
        _nest    = new Nest(20, 20, width, height);
        _gameOver = new GameOverHelper(width, height);

        // порядок появления: 3 обычных → 2 сильных → 1 босс
        _spawnQueue.Enqueue(() => new Crow(width - 200, height / 4f,      CrowChaseSpeed, CrowAttackInterval));
        _spawnQueue.Enqueue(() => new Crow(width - 200, height * 3f / 4f, CrowChaseSpeed, CrowAttackInterval));
        _spawnQueue.Enqueue(() => new Crow(width - 200, height / 2f,      CrowChaseSpeed, CrowAttackInterval));
        _spawnQueue.Enqueue(() => new StrongCrow(width - 200, height / 3f));
        _spawnQueue.Enqueue(() => new StrongCrow(width - 200, height * 2f / 3f));
        _spawnQueue.Enqueue(() => new BossCrow(width - 200, height / 2f));

        SpawnNext();

        float groundY = height * 0.72f;
        _groundItems.Add(new GroundItem(width * 0.15f, groundY, GroundItemType.Crumb));
        _groundItems.Add(new GroundItem(width * 0.30f, groundY, GroundItemType.Crumb));
        _groundItems.Add(new GroundItem(width * 0.50f, groundY, GroundItemType.Cigarette));
        _groundItems.Add(new GroundItem(width * 0.70f, groundY, GroundItemType.Beer));

        _dropSchedule.Enqueue((400,  GroundItemType.Cigarette, 0.25f));
        _dropSchedule.Enqueue((700,  GroundItemType.Beer,      0.55f));
        _dropSchedule.Enqueue((1000, GroundItemType.Cigarette, 0.45f));
        _dropSchedule.Enqueue((1300, GroundItemType.Beer,      0.30f));
        _dropSchedule.Enqueue((1600, GroundItemType.Cigarette, 0.65f));
        _dropSchedule.Enqueue((1900, GroundItemType.Beer,      0.40f));
        _dropSchedule.Enqueue((2200, GroundItemType.Cigarette, 0.35f));
        _dropSchedule.Enqueue((2500, GroundItemType.Beer,      0.60f));
    }

    private void SpawnNext()
    {
        if (_spawnQueue.Count == 0) return;
        _crows.Add(_spawnQueue.Dequeue()());
        _spawnTick = 0;
    }

    public void Shoot(int targetX, int targetY)
    {
        if (_pigeon.IsDead()) return;
        if (!_pigeon.TryShoot()) return;

        float cx = _pigeon.X + _pigeon.Width  / 2f;
        float cy = _pigeon.Y + _pigeon.Height / 2f;

        IProjectile p;
        if (_beerAmmo > 0)           { p = new BeerProjectile(cx, cy, targetX, targetY);      _beerAmmo--;      }
        else if (_cigaretteAmmo > 0) { p = new CigaretteProjectile(cx, cy, targetX, targetY); _cigaretteAmmo--; }
        else                         { p = new PigeonDropping(cx, cy, targetX, targetY);                        }

        _projectiles.Add(p);
    }

    public void OnLeftClick(int x, int y)
    {
        if (_pigeon.IsDead() && _gameOver.IsRestartClicked(x, y) && _nextArena == null)
            _nextArena = new ThirdLevel(Width, Height);
    }

    public IArena? GetNextArena() => _nextArena;

    public void Update(MovementInput movementInput)
    {
        if (_pigeon.IsDead())
            return;

        _pigeon.Move(movementInput);

        _levelTick++;
        while (_dropSchedule.Count > 0 && _dropSchedule.Peek().At <= _levelTick)
        {
            var (_, type, xf) = _dropSchedule.Dequeue();
            _groundItems.Add(new GroundItem(Width * xf, Height * 0.72f, type));
        }

        foreach (var item in _groundItems)
            item.Update();

        var pigeonBounds = new RectangleF(_pigeon.X, _pigeon.Y, _pigeon.Width, _pigeon.Height);
        for (int i = _groundItems.Count - 1; i >= 0; i--)
        {
            var item = _groundItems[i];
            if (!item.IsOnGround || !pigeonBounds.IntersectsWith(item.Bounds)) continue;

            switch (item.Type)
            {
                case GroundItemType.Crumb:     _pigeon.Heal(1);  break;
                case GroundItemType.Cigarette: _cigaretteAmmo++; break;
                case GroundItemType.Beer:      _beerAmmo++;      break;
            }
            _groundItems.RemoveAt(i);
        }

        if (_spawnQueue.Count > 0)
        {
            _spawnTick++;
            if (_spawnTick >= SpawnInterval)
                SpawnNext();
        }

        foreach (var crow in _crows)
            if (!crow.IsDead())
                crow.Update(_pigeon);

        for (int i = _projectiles.Count - 1; i >= 0; i--)
        {
            var p = _projectiles[i];
            p.Update();

            foreach (var crow in _crows)
            {
                if (!crow.IsDead() && p.Hits(crow))
                {
                    crow.TakeDamage(p.Damage);
                    p.Expire();
                    break;
                }
            }

            if (p.IsExpired)
                _projectiles.RemoveAt(i);
        }
    }

    public void Draw(Graphics graphics)
    {
        graphics.DrawImageUnscaled(_background, 0, 0);
        _nest.Draw(graphics);

        foreach (var item in _groundItems)
            item.Draw(graphics);

        foreach (var crow in _crows)
            crow.Draw(graphics);

        foreach (var p in _projectiles)
            p.Draw(graphics);

        _pigeon.Draw(graphics);

        DrawHelper.DrawHealthPanel(graphics, _pigeon.Health, _pigeon.MaxHealth,
            panelX: 20, panelY: 20,
            fillColor: Color.Crimson, borderColor: Color.FromArgb(180, 0, 0),
            pixelSize: PixelSize, heartGap: HeartGap, panelPad: 16);

        if (_pigeon.IsDead())
            _gameOver.Draw(graphics);
    }
}
