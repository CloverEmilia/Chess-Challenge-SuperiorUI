using System.Numerics;

namespace ChessChallenge.Application
{
    public static class Settings
    {
        public const string Version = "1.20";

        // Game settings
        public const int GameDurationMilliseconds = 60 * 1000;
        public const int IncrementMilliseconds = 0 * 1000;
        public const float MinMoveDelay = 0;
        public static bool RunBotsOnSeparateThread = true;

        // Screen Settings
        public const bool DisplayBoardCoordinates = true;
        public const int defaultScreenX = (int)(1280 / 1.2f / 1.2f);
        public const int defaultScreenY = (int)(720 / 1.2f / 1.2f);

        // Other settings
        public const int MaxTokenCount = 1024;
        public const LogType MessagesToLog = LogType.None;

        public enum LogType
        {
            None,
            ErrorOnly,
            All
        }
    }
}
