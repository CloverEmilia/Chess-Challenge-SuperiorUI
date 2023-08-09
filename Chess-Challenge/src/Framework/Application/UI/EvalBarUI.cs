using Raylib_cs;
using System.Numerics;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using ChessChallenge.Chess;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ChessChallenge.Application
{
    public static class EvalBarUI
    {
        //eval bar settings (scaled automatically later)
        static int evalBarPositionX = 480;
        static int evalBarPositionY = 100;
        static int evalBarSizeX = 40;
        static int evalBarSizeY = 847; //I hate this number
        static float correctedHeight;
        static int currentFramesOfCorrectedHeightDifferenceAccumulated;


        public static void DrawEvalBar(ChallengeController controller)
        {
            //Console.WriteLine(StockFish.currentlyEvaluatedScore);
                //place a black rectangle over the entire area and then cover with white what's actually needed
            Raylib.DrawRectangle(UIHelper.ScaleInt(evalBarPositionX), UIHelper.ScaleInt(evalBarPositionY), UIHelper.ScaleInt(evalBarSizeX), UIHelper.ScaleInt(evalBarSizeY), BoardUI.theme.weakNeutralTextColor);
            float winChance = 2 / (1 + MathF.Exp(-0.00368208f * StockFish.savedEvaluatedScore)) - 1;
            float height = 100 - (winChance + 1) * 50;
            Raylib.DrawRectangle(UIHelper.ScaleInt(evalBarPositionX), UIHelper.ScaleInt(evalBarPositionY), UIHelper.ScaleInt(evalBarSizeX), (int)(UIHelper.ScaleInt(evalBarSizeY) * (correctedHeight / 100)), BoardUI.theme.strongNeutralTextColor);

            if (correctedHeight != ReverseAlongMidpoint((int)height)){
                currentFramesOfCorrectedHeightDifferenceAccumulated++;
                if(currentFramesOfCorrectedHeightDifferenceAccumulated > 15){
                    correctedHeight = ReverseAlongMidpoint((int)height);
                }
            } else{
                currentFramesOfCorrectedHeightDifferenceAccumulated = 0;
            }
        

            Console.WriteLine("A " + StockFish.currentlyEvaluatedScore);
            //Console.WriteLine("B " + winChance);
            //Console.WriteLine("C " + height);
        }

        static int ReverseAlongMidpoint(int value)
        {
            const int midpoint = 50;
            int difference = midpoint - value;
            int reversedValue = midpoint + difference;
            return reversedValue;
        }
    }


    public class StockFish
    {
        private Process stockfishProcess;
        private StreamWriter Ins() => stockfishProcess.StandardInput;
        private StreamReader Outs() => stockfishProcess.StandardOutput;

        private const int SKILL_LEVEL = 20;
        public static int currentlyEvaluatedScore;
        public static int savedEvaluatedScore;

        private const string stockfishExe = @"C:\Users\Clover\Desktop\stockfish-windows-x86-64-avx2.exe";
        
        public void InitializeStockFish(){
            if (stockfishExe == null)
            {
                throw new Exception("Missing environment variable: 'STOCKFISH_EXE'");
            }

            stockfishProcess = new();
            stockfishProcess.StartInfo.RedirectStandardOutput = true;
            stockfishProcess.StartInfo.RedirectStandardInput = true;
            stockfishProcess.StartInfo.FileName = stockfishExe;
            stockfishProcess.Start();

            Ins().WriteLine("uci");
            string? line;
            var isOk = false;

            while ((line = Outs().ReadLine()) != null)
            {
                if (line == "uciok")
                {
                    isOk = true;
                    break;
                }
            }

            if (!isOk)
            {
                throw new Exception("Failed to communicate with stockfish");
            }

            Ins().WriteLine($"setoption name Skill Level value {SKILL_LEVEL}");
        }

        public void EvaluateScore(API.Board board, int timeInMilliseconds)
        {
            Ins().WriteLine("ucinewgame");
            Ins().WriteLine($"position fen {board.GetFenString()}");
            var timeString = board.IsWhiteToMove ? "wtime" : "btime";
            Ins().WriteLine($"go {timeString} {timeInMilliseconds}");

            string? line;

            while ((line = Outs().ReadLine()) != null)
            {
                if(line.StartsWith("bestmove")){
                    savedEvaluatedScore = currentlyEvaluatedScore;
                    return;
                }
                ExtractScore(line);
            }
        }

        static void ExtractScore(string input)
        {
            string pattern = @"score cp (-?\d+)"; // Updated pattern to match "score cp" format

            Match match = Regex.Match(input, pattern);

            if (match.Success)
            {
                if (match.Groups.Count > 1 && int.TryParse(match.Groups[1].Value, out int score))
                {
                    currentlyEvaluatedScore = score;
                }
            }
        }
    }
}