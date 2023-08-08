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
        public static List<int> matchingPieces = new();
        public static string currentlyExploredFenString = "hehe "; //any string to stop null reference, feel free to insert you own silly little phrase, I won't mind🤗
        private static BoardUI boardUI = new BoardUI();

        public static void DrawMaterialDiff(Application.ChallengeController controller, BoardUI boardUI)
        {

        }

        public static void AddPiece()
        {
            
        }

        public static void ResetMaterialDiffPieceList(){
            
        }
    }
}