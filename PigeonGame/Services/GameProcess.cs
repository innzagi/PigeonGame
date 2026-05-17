using PigeonGame.Dto;
using PigeonGame.Interfaces;
using PigeonGame.Models;

namespace PigeonGame.Services;

public class GameProcess(Pigeon pigeon) : IGameProcess
{
    private Pigeon _pigeon = pigeon;

    public void Update(MovementInput movementInput)
    {
        _pigeon.Move(movementInput);
    }
}