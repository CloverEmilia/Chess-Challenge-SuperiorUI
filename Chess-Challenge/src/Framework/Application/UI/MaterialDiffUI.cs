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
        public static List<int> startingPieces = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 2, 2, 3, 3, 4, 5 };
        public static List<int> blackPiecesNotTakenbyWhite = new();
        public static List<int> whitePiecesNotTakenbyBlack = new();


        public static List<int> matchingPieces = new();

        public static void DrawMaterialDiff()
        {
            foreach(int displayInt in blackPiecesNotTakenbyWhite)
            {
                //Console.Write(displayInt + " ");
            }
                //Console.WriteLine(" left nottaken ");
            foreach (int displayInt in startingPieces)
            {
                //Console.Write(displayInt + " ");
            }
            //.WriteLine(" left starting ");
            foreach (int displayInt in blackPiecesNotTakenbyWhite.Except(startingPieces).ToList())
            {
                //Console.Write(displayInt + " ");
            }
            //Console.WriteLine(" left total ");

        }

        public static void CalculateMaterialDiff(ChessChallenge.API.Board board)
        {
            whitePiecesNotTakenbyBlack.Clear();
            blackPiecesNotTakenbyWhite.Clear();
            API.PieceList[] arrayOfAllPieceLists = board.GetAllPieceLists();

            for (int i = 0; i < arrayOfAllPieceLists.Length; i++)
            {
                if (i < 6){
                    for (int y = 0; y < arrayOfAllPieceLists[i].Count; y++)
                    {
                        whitePiecesNotTakenbyBlack.Add(i);
                    }

                } else{
                    for (int y = 0; y < arrayOfAllPieceLists[i].Count; y++)
                    {
                        blackPiecesNotTakenbyWhite.Add(i-6);
                    }

                }
            }
        }
    }
}