using System.Drawing;

namespace PigeonGame.Helpers;

public class GameOverHelper
{
    private readonly int _width;
    private readonly int _height;
    private readonly Bitmap _gameOverImage;
    private readonly Bitmap _restartImage;
    private readonly Rectangle _gameOverRect;
    private readonly Rectangle _restartRect;
    private readonly Rectangle _restartClickZone;

    public GameOverHelper(int width, int height)
    {
        _width  = width;
        _height = height;

        _gameOverImage = new Bitmap("Resources/GameOver.png");
        _restartImage  = new Bitmap("Resources/Restart.png");

        const int goW = 700;
        int goH = (int)(_gameOverImage.Height * (goW / (float)_gameOverImage.Width));
        _gameOverRect = new Rectangle((width - goW) / 2, height / 2 - 110 - goH / 2, goW, goH);

        const int rsW = 460;
        int rsH = (int)(_restartImage.Height * (rsW / (float)_restartImage.Width));
        _restartRect = new Rectangle((width - rsW) / 2, height / 2 + 110 - rsH / 2, rsW, rsH);

        _restartClickZone = new Rectangle(
            _restartRect.X + (int)(rsW * 0.05f),
            _restartRect.Y + (int)(rsH * 0.28f),
            (int)(rsW * 0.90f),
            (int)(rsH * 0.43f));
    }

    public bool IsRestartClicked(int x, int y) => _restartClickZone.Contains(x, y);

    public void Draw(Graphics graphics)
    {
        using var overlay = new SolidBrush(Color.FromArgb(180, 0, 0, 0));
        graphics.FillRectangle(overlay, 0, 0, _width, _height);
        graphics.DrawImage(_gameOverImage, _gameOverRect);
        graphics.DrawImage(_restartImage, _restartRect);
    }
}
