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

    private readonly Image image;

    public Rectangle Bounds => new Rectangle(X, Y, Width, Height);

    public Pigeon(int x, int y, Image image, int windowWidth, int windowHeight)
    {
        X = x;
        Y = y;

        Width = 128;
        Height = 128;

        Speed = 5;

        MaxHealth = 3;
        Health = MaxHealth;

        this.image = image;
        this.windowWidth = windowWidth;
        this.windowHeight = windowHeight;
    }

    public void Move(MovementInput movementInput)
    {
        if (movementInput.Up)
            Y -= Speed;

        if (movementInput.Down)
            Y += Speed;

        if (movementInput.Left)
            X -= Speed;

        if (movementInput.Right)
            X += Speed;

        KeepInsideWindow();
    }

    private void KeepInsideWindow()
    {
        if (X < 0)
            X = 0;

        if (Y < 0)
            Y = 0;

        if (X + Width > windowWidth)
            X = windowWidth - Width;

        if (Y + Height > windowHeight)
            Y = windowHeight - Height;
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;

        if (Health < 0)
            Health = 0;
    }

    public void Heal(int value)
    {
        Health += value;

        if (Health > MaxHealth)
            Health = MaxHealth;
    }

    public bool IsDead()
    {
        return Health <= 0;
    }

    public void Draw(Graphics graphics)
    {
        graphics.DrawImage(image, X, Y, Width, Height);
    }
}