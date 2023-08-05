using ChessChallenge.Chess;
using Raylib_cs;
using System;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using static ChessChallenge.Application.Settings;
using static ChessChallenge.Application.ConsoleHelper;

namespace ChessChallenge.Application
{
    public static class MaterialDiffUI
    {
        public static List<int> blackPiecesTakenbyWhite = new();
        public static List<int> whitePiecesTakenbyBlack = new();

        public static void DrawMaterialDiff(Application.ChallengeController controller)
        {
            List<int> matchingPieces = blackPiecesTakenbyWhite.Intersect(whitePiecesTakenbyBlack).ToList();

            foreach (int tradedPiece in matchingPieces){
                blackPiecesTakenbyWhite.Remove(tradedPiece);
                whitePiecesTakenbyBlack.Remove(tradedPiece);
            }

            foreach(int piecesToDisplay in blackPiecesTakenbyWhite){
                Console.WriteLine("black" + piecesToDisplay);
            }
            foreach (int piecesToDisplay in whitePiecesTakenbyBlack)
            {
                Console.WriteLine("white" + piecesToDisplay);
            }
        }

        public static void UpdateMaterialDiff(Chess.Board board)
        {
            blackPiecesTakenbyWhite.Clear();
            whitePiecesTakenbyBlack.Clear();
            string currentlyExploredFenString = (FenUtility.CurrentFen(board));

            for (int i = 0; i < currentlyExploredFenString.Length; i++)
            {
                char currentChar = currentlyExploredFenString[i];
                // Switch statement to handle different actions based on the character
                switch (currentChar)
                {
                    case ' ':
                        //done reading pieces
                        i = 9999; //stops the loop from evaluating after the next break
                        break;
                    case 'P':
                        blackPiecesTakenbyWhite.Add(1);
                        break;
                    case 'N':
                        blackPiecesTakenbyWhite.Add(2);
                        break;
                    case 'B':
                        blackPiecesTakenbyWhite.Add(3);
                        break;
                    case 'R':
                        blackPiecesTakenbyWhite.Add(4);
                        break;
                    case 'Q':
                        blackPiecesTakenbyWhite.Add(5);
                        break;
                    case 'K':
                        blackPiecesTakenbyWhite.Add(6); //technically not needed, I guess, keep for posterity
                        break;
                    case 'p':
                        whitePiecesTakenbyBlack.Add(1);
                        break;
                    case 'n':
                        whitePiecesTakenbyBlack.Add(2);
                        break;
                    case 'b':
                        whitePiecesTakenbyBlack.Add(3);
                        break;
                    case 'r':
                        whitePiecesTakenbyBlack.Add(4);
                        break;
                    case 'q':
                        whitePiecesTakenbyBlack.Add(5);
                        break;
                    case 'k':
                        whitePiecesTakenbyBlack.Add(6);
                        break;
                }
            }
        }
    }
}