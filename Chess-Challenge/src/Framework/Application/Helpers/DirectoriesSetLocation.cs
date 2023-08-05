using ChessChallenge.API;

namespace ChessChallenge.Application
{
    public static class DirectoriesSetLocation
    {
        public const string stockFishDirectory = @"C:\Users\Clover\Desktop\stockfish_20090216_x64.exe";
        public const bool isStockFishDirectoryDefault = (stockFishDirectory == "//put the full (from C:/) directory of stockfish here") ? true : false;
    }
}
