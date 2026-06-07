using System.Drawing;

namespace PigeonGame.Helpers;

public class VictoryHelper
{
    private readonly int _width;
    private readonly int _height;
    private readonly Bitmap _background;
    private readonly Bitmap _restartImage;
    private readonly Rectangle _restartRect;
    private readonly Rectangle _restartClickZone;

    public VictoryHelper(int width, int height)
    {
        _width  = width;
        _height = height;

        _background   = BitmapHelper.LoadScaledBitmap("Resources/FinalBackground.png", width, height);
        _restartImage = new Bitmap("Resources/Restart.png");

        const int rsW = 460;
        int rsH = (int)(_restartImage.Height * (rsW / (float)_restartImage.Width));

        // кнопка рестарта у нижнего края
        _restartRect = new Rectangle((width - rsW) / 2, height - rsH - 50, rsW, rsH);

        _restartClickZone = new Rectangle(
            _restartRect.X + (int)(rsW * 0.05f),
            _restartRect.Y + (int)(rsH * 0.28f),
            (int)(rsW * 0.90f),
            (int)(rsH * 0.43f));
    }

    public bool IsRestartClicked(int x, int y) => _restartClickZone.Contains(x, y);

    public void Draw(Graphics graphics)
    {
        graphics.DrawImage(_background, 0, 0, _width, _height);
        graphics.DrawImage(_restartImage, _restartRect);
    }
}
