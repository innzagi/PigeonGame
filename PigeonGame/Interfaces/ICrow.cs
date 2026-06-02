using System.Drawing;
using PigeonGame.Models;

namespace PigeonGame.Interfaces;

public interface ICrow
{
    void Update(Pigeon pigeon);
    void Draw(Graphics graphics);
}
