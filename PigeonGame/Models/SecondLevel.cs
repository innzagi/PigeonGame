using System.Collections.Generic;
using System.Drawing;
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
    private readonly GameOverHelper _gameOver;

    private const int PixelSize = 8;
    private const int HeartGap = 12;
    private const float CrowChaseSpeed = 7.0f;
    private const int CrowAttackInterval = 50;
    private const int SpawnInterval = 300; // 10 сек при 20мс/тик

    private IArena? _nextArena;

    private readonly float[] _spawnYPositions;
    private int _spawnIndex;
    private int _spawnTick;

    public SecondLevel(int width, int height)
    {
        Width = width;
        Height = height;

        _background = BitmapHelper.LoadScaledBitmap("Resources/BackgroundTwoLewel.png", width, height);
        _pigeon = new Pigeon(100, 100, width, height);
        _nest = new Nest(20, 20, width, height);
        _gameOver = new GameOverHelper(width, height);

        _spawnYPositions = [height / 4f, height / 2f, height * 3f / 4f];

        SpawnNextCrow();
    }

    public void Shoot(int targetX, int targetY)
    {
        if (_pigeon.IsDead()) return;
        if (!_pigeon.TryShoot()) return;

        float cx = _pigeon.X + _pigeon.Width / 2f;
        float cy = _pigeon.Y + _pigeon.Height / 2f;
        _droppings.Add(new PigeonDropping(cx, cy, targetX, targetY));
    }

    public void OnLeftClick(int x, int y)
    {
        if (_pigeon.IsDead() && _gameOver.IsRestartClicked(x, y) && _nextArena == null)
            _nextArena = new SecondLevel(Width, Height);
    }

    public IArena? GetNextArena() => _nextArena;

    private void SpawnNextCrow()
    {
        if (_spawnIndex >= _spawnYPositions.Length) return;
        _crows.Add(new Crow(Width - 200, _spawnYPositions[_spawnIndex], CrowChaseSpeed, CrowAttackInterval));
        _spawnIndex++;
        _spawnTick = 0;
    }

    public void Update(MovementInput movementInput)
    {
        if (_pigeon.IsDead())
            return;

        _pigeon.Move(movementInput);

        if (_spawnIndex < _spawnYPositions.Length)
        {
            _spawnTick++;
            if (_spawnTick >= SpawnInterval)
                SpawnNextCrow();
        }

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

        if (_pigeon.IsDead())
            _gameOver.Draw(graphics);
    }
}
