using System.Drawing;
using PigeonGame.Dto;

namespace PigeonGame.Interfaces;

public interface IArena
{
    void Update(MovementInput movementInput);
    void Draw(Graphics graphics);
    void DrawNextArena(Graphics graphics);

    
    
}