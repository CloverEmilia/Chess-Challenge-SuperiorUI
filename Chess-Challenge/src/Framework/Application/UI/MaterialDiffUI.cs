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
using static ChessChallenge.Application.UIHelper;
using ChessChallenge.Application.APIHelpers;
using System.Numerics;

namespace ChessChallenge.Application
{
    public static class MaterialDiffUI
    {
        public static List<int> startingPieces = new();
        public static List<int> blackPiecesTakenbyWhite = new();
        public static List<int> whitePiecesTakenbyBlack = new();
        public static List<int> blackPiecesLastFrame = new();
        public static List<int> whitePiecesLastFrame = new();
        public static List<int> matchingPieces = new();
        public static List<int> blackPiecesActuallyDisplayed = new();
        public static List<int> whitePiecesActuallyDisplayed = new();
        static string currentlyExploredFenString = "hehe "; //any string to stop null reference, feel free to insert you own silly little phrase, I won't mind🤗
        private static BoardUI boardUI = new BoardUI();

        public static void InitializeStartingPieces()
        {
                for (int pawns = 0; pawns < 8; pawns++)
                {
                    startingPieces.Add(1);
                }
                for (int knights = 0; knights < 2; knights++)
                {
                    startingPieces.Add(2);
                }
                for (int bishops = 0; bishops < 2; bishops++)
                {
                    startingPieces.Add(3);
                }
                for (int Rooks = 0; Rooks < 2; Rooks++)
                {
                    startingPieces.Add(4);
                }
                for (int Queens = 0; Queens < 1; Queens++)
                {
                    startingPieces.Add(5);
                }
                for (int Kings = 0; Kings < 1; Kings++)
                {
                    startingPieces.Add(6);
                }
        }

        public static void DrawMaterialDiff(Application.ChallengeController controller, BoardUI boardUI)
        {
            //Console.WriteLine("2 " + blackPiecesTakenbyWhite.Count);
            //Console.WriteLine("1 " + whitePiecesTakenbyBlack.Count);

            if (startingPieces.Any(piece => piece > 1))
            {
            } else{
                InitializeStartingPieces();
            }

                //Calculate diff
                CalculateMaterialDiff();
                //Draw the material display itself
            foreach (int pieceTypeValueInteger in blackPiecesActuallyDisplayed)
            {
                boardUI.DrawPiece(pieceTypeValueInteger, Vector2.One * 50, 1);
            }
            foreach (int pieceTypeValueInteger in whitePiecesActuallyDisplayed)
            {
                boardUI.DrawPiece (pieceTypeValueInteger, Vector2.One * 50, 1);
            }

                //If the calculation changed something
            if (blackPiecesTakenbyWhite != blackPiecesLastFrame || whitePiecesTakenbyBlack != whitePiecesLastFrame){
                    //Update piece comparison variables
                blackPiecesLastFrame = blackPiecesTakenbyWhite;
                whitePiecesLastFrame = whitePiecesTakenbyBlack;
                    //Clear Display variable
                whitePiecesActuallyDisplayed.Clear();
                blackPiecesActuallyDisplayed.Clear();
                
                //trade off equivilent pieces
                matchingPieces = blackPiecesTakenbyWhite.Intersect(whitePiecesTakenbyBlack).ToList();
                foreach (int tradedPiece in matchingPieces)
                {
                    blackPiecesActuallyDisplayed.Remove(tradedPiece);
                    whitePiecesActuallyDisplayed.Remove(tradedPiece);
                }
                

                //for every piece left, add it to the displayed pieces
                //if it's different from starting position
                // Calculate pieces taken from each side
                var blackPiecesRemoved = startingPieces.Except(blackPiecesTakenbyWhite).ToList();
                var whitePiecesRemoved = startingPieces.Except(whitePiecesTakenbyBlack).ToList();

                // Update displayed pieces with the removed pieces
                foreach (int piecesToDisplay in blackPiecesRemoved)
                {
                    blackPiecesActuallyDisplayed.Add(piecesToDisplay);
                }
                foreach (int piecesToDisplay in whitePiecesRemoved)
                {
                    whitePiecesActuallyDisplayed.Add(piecesToDisplay);
                }
            }
        }

        public static void UpdateMaterialDiff(Chess.Board board)
        {
            currentlyExploredFenString = (FenUtility.CurrentFen(board));
        }

        public static void CalculateMaterialDiff()
        {
            blackPiecesTakenbyWhite.Clear();
            whitePiecesTakenbyBlack.Clear();
            for (int i = 0; i < MaterialDiffUI.currentlyExploredFenString.Length; i++)
            {
                char currentChar = currentlyExploredFenString[i];
                // Switch statement to handle different actions based on the character
                switch (currentChar)
                {
                    case ' ':
                        //done reading pieces
                        i = MaterialDiffUI.currentlyExploredFenString.Length; //stops the loop from evaluating after the next break
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