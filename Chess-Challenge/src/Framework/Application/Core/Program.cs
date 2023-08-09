using Raylib_cs;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System;
using ChessChallenge.API;
using ChessChallenge.UCI;

namespace ChessChallenge.Application
{
    static class Program
    {
        const bool hideRaylibLogs = true;
        static Camera2D cam;

        public static void Main(string[] args)
        {
            if (args.Length == 1 && args[0].Contains("cutechess"))
            {
                string argstr = args[0].Substring(args[0].IndexOf("uci"));
                string[] ccArgs = argstr.Split(" ");
                if (ccArgs.Length == 2 && ccArgs[0] == "uci")
                {
                    Console.WriteLine("Starting up in UCI mode...");
                    StartUCI(ccArgs);
                    return;
                }
                else
                {
                    Console.WriteLine("Improper CuteChess arg format; should be 'cutechess uci <botname>'");
                    return;
                }
            }
            if (args.Length > 1 && args[0] == "uci")
            {
                Console.WriteLine("Starting up in UCI mode...");
                StartUCI(args);
                return;
            }
            Console.WriteLine("Starting up in GUI mode...");
            int screenWidth = Settings.defaultScreenX;
            int screenHeight = Settings.defaultScreenY;

            if (hideRaylibLogs)
            {
                unsafe
                {
                    Raylib.SetTraceLogCallback(&LogCustom);
                }
            }

            Raylib.InitWindow(screenWidth, screenHeight, "Chess Coding Challenge");
            Raylib.SetTargetFPS(60);

            UpdateCamera(screenWidth, screenHeight);

            ChallengeController controller = new();

            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(new Color(22, 22, 22, 255));
                Raylib.BeginMode2D(cam);

                controller.Update();
                controller.Draw();

                Raylib.EndMode2D();

                controller.DrawOverlay();

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();

            controller.Release();
            UIHelper.Release();
        }

        public static void SetWindowSize(Vector2 size)
        {
            Raylib.SetWindowSize((int)size.X, (int)size.Y);
            UpdateCamera((int)size.X, (int)size.Y);
        }

        public static Vector2 ScreenToWorldPos(Vector2 screenPos) => Raylib.GetScreenToWorld2D(screenPos, cam);

        static void UpdateCamera(int screenWidth, int screenHeight)
        {
            cam = new Camera2D
            {
                target = new Vector2(0, 15),
                offset = new Vector2(screenWidth / 2f, screenHeight / 2f),
                zoom = screenWidth / 1280f * 0.7f
            };
        }

        public static void StartUCI(string[] args)
        {
            ChallengeController.PlayerType player;
            bool success = Enum.TryParse(args[1], out player);

            if (!success)
            {
                Console.Error.WriteLine($"Failed to start bot with player type {args[1]}");
                return;
            }

            IChessBot? bot = ChallengeController.CreateBot(player);
            if (bot == null)
            {
                Console.Error.WriteLine($"Cannot create bot of type {player.ToString()}");
                return;
            }

            UCIBot uci = new UCIBot(bot, player);
            uci.Run();
        }



        [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        private static unsafe void LogCustom(int logLevel, sbyte* text, sbyte* args)
        {
        }
      
    }


}