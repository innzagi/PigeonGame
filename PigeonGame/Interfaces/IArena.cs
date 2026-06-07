using System.Drawing;
using PigeonGame.Dto;

namespace PigeonGame.Interfaces;

public interface IArena
{
    void Update(MovementInput movementInput);
    void Draw(Graphics graphics);

    // Правый клик — выпустить помёт по цели
    void Shoot(int targetX, int targetY);

    // Левый клик — обработать клик по UI (кнопки меню и т.п.)
    void OnLeftClick(int x, int y);

    // Если арена готова смениться на другую (например, меню → 1 уровень),
    // возвращает следующую арену. Иначе null.
    IArena? GetNextArena();
}
