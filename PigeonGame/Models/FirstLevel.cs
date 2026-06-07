using System.Collections.Generic;
using System.Drawing;
using PigeonGame.Dto;
using PigeonGame.Helpers;
using PigeonGame.Interfaces;

namespace PigeonGame.Models;

public class FirstLevel : IArena
{
    public int Width  { get; }
    public int Height { get; }

    private readonly Bitmap _background;
    private readonly Pigeon _pigeon;
    private readonly Nest   _nest;
    private readonly Crow   _crow;
    private readonly List<IProjectile> _projectiles = new();
    private readonly List<GroundItem>  _groundItems = new();
    private readonly GameOverHelper    _gameOver;

    private const int PixelSize          = 8;
    private const int HeartGap           = 12;
    private const int LevelTransitionDelay = 60;

    private IArena? _nextArena;
    private int     _crowDeadTicks;
    private int     _cigaretteAmmo;
    private int     _beerAmmo;
    private bool    _gameOverMusicPlayed;

    public FirstLevel(int width, int height)
    {
        Width  = width;
        Height = height;

        MusicPlayer.Play("Fight");

        _background = BitmapHelper.LoadScaledBitmap("Resources/BackgroundFirstLevel.png", width, height);
        _pigeon  = new Pigeon(100, 100, width, height);
        _nest    = new Nest(20, 20, width, height);
        _crow    = new Crow(width - 200, height / 2f);
        _gameOver = new GameOverHelper(width, height);

        float groundY = height * 0.72f;
        _groundItems.Add(new GroundItem(width * 0.25f, groundY, GroundItemType.Crumb));
        _groundItems.Add(new GroundItem(width * 0.45f, groundY, GroundItemType.Crumb));
        _groundItems.Add(new GroundItem(width * 0.65f, groundY, GroundItemType.Cigarette));
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
            _nextArena = new FirstLevel(Width, Height);
    }

    public IArena? GetNextArena() => _nextArena;

    public void Update(MovementInput movementInput)
    {
        if (_pigeon.IsDead())
        {
            if (!_gameOverMusicPlayed)
            {
                MusicPlayer.Play("Intro");
                _gameOverMusicPlayed = true;
            }
            return;
        }

        _pigeon.Move(movementInput);

        // анимация предметов (падение, свечение)
        foreach (var item in _groundItems)
            item.Update();

        // подбор предметов с земли
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

        if (!_crow.IsDead())
        {
            _crow.Update(_pigeon);
        }
        else
        {
            _crowDeadTicks++;
            if (_crowDeadTicks >= LevelTransitionDelay)
                _nextArena = new SecondLevel(Width, Height, _pigeon.Health);
        }

        for (int i = _projectiles.Count - 1; i >= 0; i--)
        {
            var p = _projectiles[i];
            p.Update();

            if (!_crow.IsDead() && p.Hits(_crow))
            {
                _crow.TakeDamage(p.Damage);
                p.Expire();
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

        _crow.Draw(graphics);

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
