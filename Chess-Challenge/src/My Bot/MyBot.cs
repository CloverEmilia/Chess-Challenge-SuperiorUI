using System.Reflection;
using ChessChallenge.API;

public class MyBot : IChessBot
{
    public Move Think(Board board, Timer timer)
    {
        string expectedAssemblyName = Assembly.GetExecutingAssembly().FullName; // or specify the correct assembly
        System.Console.WriteLine($"Expected Assembly: {expectedAssemblyName}");

        Move[] moves = board.GetLegalMoves();
        return moves[0];
    }
}