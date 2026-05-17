using PigeonGame.Dto;

namespace PigeonGame.Interfaces;

public interface IGameProcess
{
    void Update(MovementInput movementInput);
}