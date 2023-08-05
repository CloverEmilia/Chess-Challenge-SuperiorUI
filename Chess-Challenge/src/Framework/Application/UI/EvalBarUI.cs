using Raylib_cs;
using System.Numerics;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Stockfish.NET;
using ChessChallenge.Chess;

namespace ChessChallenge.Application
{
    public static class EvalBarUI
    {
            //Stockfish settings
        const int STOCKFISH_LEVEL = 20;
        static IStockfish mStockFish;
        //eval bar settings (scaled automatically later)
        static int evalBarPositionX = 480;
        static int evalBarPositionY = 100;
        static int evalBarSizeX = 40;
        static int evalBarSizeY = 847; //I hate this number

        static int tempTestVariable; //remove
        public static void DrawEvalBar(ChallengeController controller)
        {
                //place a black rectangle over the entire area and then cover with white what's actually needed
            Raylib.DrawRectangle(UIHelper.ScaleInt(evalBarPositionX), UIHelper.ScaleInt(evalBarPositionY), UIHelper.ScaleInt(evalBarSizeX), UIHelper.ScaleInt(evalBarSizeY), BoardUI.theme.weakNeutralTextColor);
            Raylib.DrawRectangle(UIHelper.ScaleInt(evalBarPositionX), UIHelper.ScaleInt(evalBarPositionY), UIHelper.ScaleInt(evalBarSizeX), UIHelper.ScaleInt(evalBarSizeY) /((Raylib.GetMouseY() / 10) + 1), BoardUI.theme.strongNeutralTextColor);

            tempTestVariable++;
            if(tempTestVariable > 1000){
                //GetStockfishEval(controller, controller.board, 1);
                tempTestVariable = 0;
            }
        }

        public static void GetStockfishEval (ChallengeController controller, Board board, int forTime) //change to what time incriment
        {
                //for now stockfish just goes here,
                //but we can break off if we want to add stockfishbot by default
            string fen = FenUtility.CurrentFen(board);
            mStockFish.SetFenPosition(fen);
            Console.WriteLine(mStockFish.Depth);

            Stockfish.NET.Models.Evaluation eval = mStockFish.GetEvaluation();
            Console.WriteLine(eval.Value);
        }

        public static void InitializeStockfish()
        {
            Stockfish.NET.Models.Settings stockfishSettings = new Stockfish.NET.Models.Settings();
            stockfishSettings.SkillLevel = STOCKFISH_LEVEL;
            int depth = 2; // Set your desired depth value here
            //mStockFish = new Stockfish.NET.Stockfish(@"C:\Users\Clover\Desktop\stockfish_20090216_x64.exe", depth, stockfishSettings);
        }
    }
}