using ChessChallenge.API;

namespace ChessChallenge.Application
{
    public static class DirectoriesSetLocation
    {
        public const string stockFishDirectory = "//put the full (including C:/) directory of stockfish here";
        public const bool isStockFishDirectoryDefault = (stockFishDirectory == "//put the full (from C:/) directory of stockfish here") ? true : false;
    }
}
