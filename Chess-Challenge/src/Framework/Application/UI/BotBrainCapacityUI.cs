using Raylib_cs;

namespace ChessChallenge.Application
{
    public static class BotBrainCapacityUI
    {
        public static BoardTheme theme = new();
        public static void Draw(int totalTokenCount, int debugTokenCount, int tokenLimit)
        {
            int activeTokenCount = totalTokenCount - debugTokenCount;

            int screenWidth = Raylib.GetScreenWidth();
            int screenHeight = Raylib.GetScreenHeight();
            int height = UIHelper.ScaleInt(30);
            int fontSize = UIHelper.ScaleInt(25);

            // Bg
            Raylib.DrawRectangle(0, screenHeight - height, screenWidth, height, theme.BorderCol);
            // Bar
            double t = (double)activeTokenCount / tokenLimit;
            t = .7;

            Color col;
            col = LerpRGB(theme.positiveTextColor, theme.negativeTextColor, (float)t);
            Raylib.DrawRectangle(0, screenHeight - height, (int)(screenWidth * t), height, col);

            var textPos = new System.Numerics.Vector2(screenWidth / 2, screenHeight - height / 2);
            string text = $"Bot Brain Capacity: {activeTokenCount}/{tokenLimit}";
            if (activeTokenCount > tokenLimit)
            {
                text += " [LIMIT EXCEEDED]";
            }
            else if (debugTokenCount != 0)
            {
                text += $"    ({totalTokenCount} with Debugs included)";
            }
            UIHelper.DrawText(text, textPos, fontSize, 1, theme.strongNeutralTextColor, UIHelper.AlignH.Centre);
        }
        private static Color LerpRGB(Color startColor, Color endColor, float t)
        {
            if (t > 1){
                t = 1;
            }

            float r = startColor.r + (endColor.r - startColor.r) * t;
            float g = startColor.g + (endColor.g - startColor.g) * t;
            float b = startColor.b + (endColor.b - startColor.b) * t;
            float a = startColor.a + (endColor.a - startColor.a) * t;

            return new Color((byte)r, (byte)g, (byte)b, (byte)a);
        }
    }
}