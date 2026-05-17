namespace PigeonGame.Interfaces;

public interface IGameProcess
{
    void Update();

    void SetMovement(bool up, bool down, bool left, bool right);
}