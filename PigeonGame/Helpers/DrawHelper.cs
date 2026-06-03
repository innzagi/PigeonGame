namespace PigeonGame.Helpers;

public class DrawHelper

{   
    private static readonly  int HeartCols  = 7;
    private static readonly  int HeartRows  = 6;
    
    // пиксельная маска сердечка 7×6: 1 = закрашенный пиксель
    private static readonly int[,] HeartMask =
    {
        { 0, 1, 1, 0, 1, 1, 0 },
        { 1, 1, 1, 1, 1, 1, 1 },
        { 1, 1, 1, 1, 1, 1, 1 },
        { 0, 1, 1, 1, 1, 1, 0 },
        { 0, 0, 1, 1, 1, 0, 0 },
        { 0, 0, 0, 1, 0, 0, 0 },
    };
    
    public static void DrawHealthPanel(Graphics graphics, int health, int maxHealth,
        int panelX, int panelY, Color fillColor, Color borderColor,
        int pixelSize, int heartGap, int panelPad)
    {
        int heartWidth  = HeartCols * pixelSize;
        int heartHeight = HeartRows * pixelSize;
        int totalWidth  = maxHealth * heartWidth + (maxHealth - 1) * heartGap;

        int panelW = totalWidth  + panelPad * 2;
        int panelH = heartHeight + panelPad * 2;

        using var bg = new SolidBrush(Color.FromArgb(140, 0, 0, 0));
        graphics.FillRectangle(bg, panelX, panelY, panelW, panelH);
        graphics.DrawRectangle(Pens.White, panelX, panelY, panelW, panelH);

        int originX = panelX + panelPad;
        int originY = panelY + panelPad;

        for (int i = 0; i < maxHealth; i++)
        {
            int hx = originX + i * (heartWidth + heartGap);
            DrawPixelHeart(graphics, hx, originY, filled: i < health, fillColor, borderColor, pixelSize);
        }
        
    }
    

    public static void DrawPixelHeart(Graphics graphics, int x, int y, bool filled,
        Color fillColor, Color borderColor, int pixelSize)
    {
        
        var fc = filled ? fillColor   : Color.FromArgb(60, 60, 60);
        var bc = filled ? borderColor : Color.FromArgb(100, 100, 100);

        using var fillBrush   = new SolidBrush(fc);
        using var borderBrush = new SolidBrush(bc);

        for (int row = 0; row < HeartRows; row++)
        {
            for (int col = 0; col < HeartCols; col++)
            {
                if (HeartMask[row, col] == 0) continue;

                graphics.FillRectangle(borderBrush,
                    x + col * pixelSize - 1,
                    y + row * pixelSize - 1,
                    pixelSize + 2, pixelSize + 2);

                graphics.FillRectangle(fillBrush,
                    x + col * pixelSize,
                    y + row * pixelSize,
                    pixelSize, pixelSize);
            }
        }
    }
}