using Raylib_cs;
using System.Numerics;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace ChessChallenge.Application
{
    public static class MaterialDiffUI
    {
        public static List<int> blackPiecesTakenbyWhite = new();
        public static List<int> whitePiecesTakenbyBlack = new();

        public static void DrawMaterialDiff(ChallengeController controller)
        {
            UpdateMaterialDiff(controller);
            List<int> matchingPieces = blackPiecesTakenbyWhite.Intersect(whitePiecesTakenbyBlack).ToList();

            foreach (int equalizedPiece in matchingPieces)
            {
                blackPiecesTakenbyWhite.Remove(equalizedPiece);
                whitePiecesTakenbyBlack.Remove(equalizedPiece);
            }

            //where we are now
            //Console.WriteLine(blackPiecesTakenbyWhite + );
        }

        public static void UpdateMaterialDiff(ChallengeController controller)
        {
            //get starting fen string
            //current fen string
            //find out what's changed, piecewise
        }
    }
}