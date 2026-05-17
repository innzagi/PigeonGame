using PigeonGame.Interfaces;
using PigeonGame.Models;

namespace PigeonGame.Services;

public class GameProcess(Pigeon pigeon) : IGameProcess
{
    private Pigeon _pigeon = pigeon;

    public void Update()
    {
        
    }

    public void SetMovement(bool up, bool down, bool left, bool right)
    {
        
    }
}