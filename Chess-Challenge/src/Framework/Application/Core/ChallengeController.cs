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
    public class ChallengeController
    {
        public enum PlayerType
        {
            Human,
            MyBot,
            EvilBot,
            NegamaxBasic,
            AdvancedNegamax,
            //ect.
        }

        public static readonly Dictionary<PlayerType, Type> BotTypeMap = new Dictionary<PlayerType, Type>
    {
        //_ => new ChessPlayer(new HumanPlayer(boardUI), type) I removed this line at some point and I don't know why, but re-adding it now throws an error
        { PlayerType.MyBot, typeof(MyBot) },
        { PlayerType.EvilBot, typeof(EvilBot) },
        { PlayerType.NegamaxBasic, typeof(NegamaxBasic) },
        { PlayerType.AdvancedNegamax, typeof(AdvancedNegamax) },
        //ect.  
    };

        // Game state
        public Board board; //I am sorry for making this public, should this not be public? https://imgur.com/a/PsgtdWw
        readonly Random rng;
        int gameID;
        bool isPlaying;
        public bool isTournamentActive;
        int tournamentCurrentBlackPlayerIndex;
        int tournamentCurrentWhitePlayerIndex;
        public ChessPlayer PlayerWhite { get; private set; }
        public ChessPlayer PlayerBlack {get;private set;}

        float lastMoveMadeTime;
        bool isWaitingToPlayMove;
        Move moveToPlay;
        float playMoveTime;
        public bool HumanWasWhiteLastGame { get; private set; }

        // Bot match state
        readonly string[] botMatchStartFens;
        int botMatchGameIndex;
        public BotMatchStats BotStatsA { get; private set; }
        public BotMatchStats BotStatsB {get;private set;}
        bool botAPlaysWhite;
        float totalTimeTakenByBotA;
        float totalTimeTakenByBotB;


        // Bot task
        AutoResetEvent botTaskWaitHandle;
        bool hasBotTaskException;
        ExceptionDispatchInfo botExInfo;

        // Other
        readonly BoardUI boardUI;
        readonly MoveGenerator moveGenerator;
        readonly int tokenCount;
        readonly int debugTokenCount;
        readonly StringBuilder pgns; //what *are* you?
        public readonly List<string> listOfPgns = new(); //I am like you, but better.

        int totalMovesPlayed = 0;
        public int trueTotalMovesPlayed = 0;
        public bool fastForward = false;
        StockFish stockFishInstance = new StockFish();


        public ChallengeController()
        {
            Log($"Launching Chess-Challenge version {Settings.Version}");
            (tokenCount, debugTokenCount) = GetTokenCount();
            Warmer.Warm();

            rng = new Random();
            moveGenerator = new();
            boardUI = new BoardUI();
            board = new Board();
            pgns = new();
            fastForward = false;
            stockFishInstance.InitializeStockFish();

            BotStatsA = new BotMatchStats("IBot");
            BotStatsB = new BotMatchStats("IBot");
            botMatchStartFens = FileHelper.ReadResourceFile("Fens.txt").Split('\n').Where(fen => fen.Length > 0).ToArray();
            botTaskWaitHandle = new AutoResetEvent(false);

            StartNewGame(PlayerType.Human, PlayerType.MyBot);
        }

        public void StartNewGame(PlayerType whiteType, PlayerType blackType)
        {
            // End any ongoing game
            EndGame(GameResult.DrawByArbiter, log: false, autoStartNextBotMatch: false);
            gameID = rng.Next();

            // Stop prev task and create a new one
            if (RunBotsOnSeparateThread)
            {
                // Allow task to terminate
                botTaskWaitHandle.Set();
                // Create new task
                botTaskWaitHandle = new AutoResetEvent(false);
                Task.Factory.StartNew(BotThinkerThread, TaskCreationOptions.LongRunning);
            }
            // Board Setup
            board = new Board();
            bool isGameWithHuman = whiteType is PlayerType.Human || blackType is PlayerType.Human;
            int fenIndex = isGameWithHuman ? 0 : botMatchGameIndex / 2;
            board.LoadPosition(botMatchStartFens[fenIndex]);

            // Player Setup
            PlayerWhite = CreatePlayer(whiteType);
            PlayerBlack = CreatePlayer(blackType);
            PlayerWhite.SubscribeToMoveChosenEventIfHuman(OnMoveChosen);
            PlayerBlack.SubscribeToMoveChosenEventIfHuman(OnMoveChosen);

            // UI Setup
            boardUI.UpdatePosition(board);
            boardUI.ResetSquareColours();
            SetBoardPerspective();

            // Start
            isPlaying = true;
            NotifyTurnToMove();
        }

        void BotThinkerThread()
        {
            int threadID = gameID;
            //Console.WriteLine("Starting thread: " + threadID);

            while (true)
            {
                // Sleep thread until notified
                botTaskWaitHandle.WaitOne();
                // Get bot move
                if (threadID == gameID)
                {
                    var move = GetBotMove();

                    if (threadID == gameID)
                    {
                        OnMoveChosen(move);
                    }
                }
                // Terminate if no longer playing this game
                if (threadID != gameID)
                {
                    break;
                }
            }
            //Console.WriteLine("Exitting thread: " + threadID);
        }

        Move GetBotMove()
        {
            //Console.WriteLine("ehhehe, hehehe, ahahaha, HAHHAHAAHHHAA");
            
            API.Board botBoard = new(board);
            try
            {
                API.Timer timer = new(PlayerToMove.TimeRemainingMs, PlayerNotOnMove.TimeRemainingMs, GameDurationMilliseconds, IncrementMilliseconds);
                API.Move move = PlayerToMove.Bot.Think(botBoard, timer);
                return new Move(move.RawValue);
            }
            catch (Exception e)
            {
                Log("An error occurred while bot was thinking.\n" + e.ToString(), true, ConsoleColor.Red);
                hasBotTaskException = true;
                botExInfo = ExceptionDispatchInfo.Capture(e);
            }
            return Move.NullMove;
        }



        void NotifyTurnToMove()
        {
            //playerToMove.NotifyTurnToMove(board);
            if (PlayerToMove.IsHuman)
            {
                PlayerToMove.Human.SetPosition(FenUtility.CurrentFen(board));
                PlayerToMove.Human.NotifyTurnToMove();
            }
            else
            {
                if (RunBotsOnSeparateThread)
                {
                    botTaskWaitHandle.Set();
                }
                else
                {
                    double startThinkTime = Raylib.GetTime();
                    var move = GetBotMove();
                    double thinkDuration = Raylib.GetTime() - startThinkTime;
                    PlayerToMove.UpdateClock(thinkDuration);
                    OnMoveChosen(move);
                }
            }
        }

        void SetBoardPerspective()
        {
            // Board perspective
            if (PlayerWhite.IsHuman || PlayerBlack.IsHuman)
            {
                boardUI.SetPerspective(PlayerWhite.IsHuman);
                HumanWasWhiteLastGame = PlayerWhite.IsHuman;
            }
            else if (PlayerWhite.Bot is MyBot && PlayerBlack.Bot is MyBot)
            {
                boardUI.SetPerspective(true);
            }
            else
            {
                boardUI.SetPerspective(PlayerWhite.Bot is MyBot);
            }
        }
        ChessPlayer CreatePlayer(PlayerType type)
        {
            if (BotTypeMap.TryGetValue(type, out var botType))
            {
                return new ChessPlayer(Activator.CreateInstance(botType), type, GameDurationMilliseconds);
            }
            else
            {
                return new ChessPlayer(new HumanPlayer(boardUI), type);
            }
        }
        static (int totalTokenCount, int debugTokenCount) GetTokenCount()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "src", "My Bot", "MyBot.cs");

            using StreamReader reader = new(path);
            string txt = reader.ReadToEnd();
            return TokenCounter.CountTokens(txt);
        }

        void OnMoveChosen(Move chosenMove)
        {
            if (IsLegal(chosenMove))
            {
                totalMovesPlayed++;
                if(fastForward == false){
                    API.Board stockBoard = new(board);
                    stockFishInstance.EvaluateScore(stockBoard, 800);
                }

                PlayerToMove.AddIncrement(IncrementMilliseconds);
                if (PlayerToMove.IsBot)
                {
                    moveToPlay = chosenMove;
                    isWaitingToPlayMove = true;
                    playMoveTime = lastMoveMadeTime + MinMoveDelay;
                }
                else
                {
                    PlayMove(chosenMove);
                }
            }
            else
            {
                string moveName = MoveUtility.GetMoveNameUCI(chosenMove);
                string log = $"Illegal move: {moveName} in position: {FenUtility.CurrentFen(board)}";
                Log(log, true, ConsoleColor.Red);
                GameResult result = PlayerToMove == PlayerWhite ? GameResult.WhiteIllegalMove : GameResult.BlackIllegalMove;
                EndGame(result);
            }
        }

        void PlayMove(Move move)
        {
            if (isPlaying)
            {
                bool animate = PlayerToMove.IsBot;
                lastMoveMadeTime = (float)Raylib.GetTime();

                board.MakeMove(move, false);
                boardUI.UpdatePosition(board, move, animate);

                GameResult result = Arbiter.GetGameState(board);
                if (result == GameResult.InProgress)
                {
                    NotifyTurnToMove();
                }
                else
                {
                    EndGame(result);
                }
            }
        }

        void EndGame(GameResult result, bool log = true, bool autoStartNextBotMatch = true)
        {
            trueTotalMovesPlayed += totalMovesPlayed;
            totalMovesPlayed = 0;
            if (isPlaying)
            {
                isPlaying = false;
                isWaitingToPlayMove = false;

                if (log)
                {
                        //Adds the name of the bot to the result, for easy identification
                    string adjustedResults = result.ToString();
                    if(adjustedResults.Contains("Black")){
                        adjustedResults = PlayerBlack.Bot.ToString() + "" + adjustedResults;
                    }
                    if (adjustedResults.Contains("White"))
                    {
                        adjustedResults = PlayerWhite.Bot.ToString() + "" + adjustedResults;
                    }

                    Log("Game Over: " + adjustedResults, false, ConsoleColor.Blue);
                }

                string pgn = PGNCreator.CreatePGN(board, result, GetPlayerName(PlayerWhite), GetPlayerName(PlayerBlack));
                listOfPgns.Add(pgn);
                pgns.AppendLine(pgn);

                // If 2 bots playing each other, start next game automatically.
                if (PlayerWhite.IsBot && PlayerBlack.IsBot && isTournamentActive == false)
                {
                    UpdateBotMatchStats(result);
                    botMatchGameIndex++;
                    int numGamesToPlay = botMatchStartFens.Length * 2;

                    if (botMatchGameIndex < numGamesToPlay && autoStartNextBotMatch)
                    {
                        botAPlaysWhite = !botAPlaysWhite;

                        if (fastForward)
                        {
                            StartNewGame(PlayerBlack.PlayerType, PlayerWhite.PlayerType);
                        } else{
                        const int startNextGameDelayMs = 60; //originally 600, 60 is still viewable but more practical for the fastest of bots
                        System.Timers.Timer autoNextTimer = new(startNextGameDelayMs);
                        int originalGameID = gameID;
                        autoNextTimer.Elapsed += (s, e) => AutoStartNextBotMatchGame(originalGameID, autoNextTimer);
                        autoNextTimer.AutoReset = false;
                        autoNextTimer.Start();
                        }
                    }
                    else if (autoStartNextBotMatch)
                    {
                        fastForward = false;
                        Log("Match finished", false, ConsoleColor.Blue);
                    }
                }

                // If it is a Tournament
                if (isTournamentActive)
                {
                    bool matchHasBeenSelected = false;

                    while(matchHasBeenSelected == false){
                        if(tournamentCurrentBlackPlayerIndex < Enum.GetValues(typeof(PlayerType)).Length)
                        {
                            if (tournamentCurrentWhitePlayerIndex < Enum.GetValues(typeof(PlayerType)).Length)
                            {
                                if(tournamentCurrentBlackPlayerIndex != 0 && tournamentCurrentWhitePlayerIndex != 0 && tournamentCurrentBlackPlayerIndex != tournamentCurrentWhitePlayerIndex)
                                {
                                    Console.WriteLine(tournamentCurrentBlackPlayerIndex + " Inner " + tournamentCurrentWhitePlayerIndex);
                                    StartNewGame((PlayerType)tournamentCurrentBlackPlayerIndex, (PlayerType)tournamentCurrentWhitePlayerIndex);
                                    matchHasBeenSelected = true;
                                }
                                tournamentCurrentWhitePlayerIndex++;
                            } else{
                                tournamentCurrentWhitePlayerIndex = 0;
                                tournamentCurrentBlackPlayerIndex++;
                            }
                        } else{
                            tournamentCurrentWhitePlayerIndex = 0;
                            tournamentCurrentBlackPlayerIndex = 0;
                            Console.WriteLine("TTT Over!");
                            matchHasBeenSelected = true;
                            //StartNewGame(PlayerType.Human, PlayerType.Human); //can be used to make tournament mode not loop
                        }
                    }
                }
            }
        }

        private void AutoStartNextBotMatchGame(int originalGameID, System.Timers.Timer timer)
        {
            if (originalGameID == gameID)
            {
                StartNewGame(PlayerBlack.PlayerType, PlayerWhite.PlayerType);
            }
            timer.Close();
        }


        void UpdateBotMatchStats(GameResult result)
        {
            UpdateStats(BotStatsA, botAPlaysWhite);
            UpdateStats(BotStatsB, !botAPlaysWhite);

            void UpdateStats(BotMatchStats stats, bool isWhiteStats)
            {
                // Draw
                if (Arbiter.IsDrawResult(result))
                {
                    stats.NumDraws++;
                }
                // Win
                else if (Arbiter.IsWhiteWinsResult(result) == isWhiteStats)
                {
                    stats.NumWins++;
                }
                // Loss
                else
                {
                    stats.NumLosses++;
                    stats.NumTimeouts += (result is GameResult.WhiteTimeout or GameResult.BlackTimeout) ? 1 : 0;
                    stats.NumIllegalMoves += (result is GameResult.WhiteIllegalMove or GameResult.BlackIllegalMove) ? 1 : 0;
                }

                if(isWhiteStats){
                    totalTimeTakenByBotA -= (PlayerWhite.TimeRemainingMs - GameDurationMilliseconds);
                    stats.timePerTurn = totalTimeTakenByBotA / (trueTotalMovesPlayed / 2);
                } else{
                    totalTimeTakenByBotB -= (PlayerBlack.TimeRemainingMs - GameDurationMilliseconds);
                    stats.timePerTurn = totalTimeTakenByBotB / (trueTotalMovesPlayed / 2);
                }
            }
        }

        static int tempControlVariable;

        public void Update()
        {
            do
            {
                //Console.WriteLine(tempControlVariable);
                tempControlVariable++;
                if (isPlaying)
                {
                    PlayerWhite.Update();
                    PlayerBlack.Update();

                    PlayerToMove.UpdateClock(Raylib.GetFrameTime() + MinMoveDelay);
                    if (PlayerToMove.TimeRemainingMs <= 0)
                    {
                        EndGame(PlayerToMove == PlayerWhite ? GameResult.WhiteTimeout : GameResult.BlackTimeout);
                    }
                    else
                    {
                        if (isWaitingToPlayMove && (Raylib.GetTime() >= playMoveTime || fastForward))
                        {
                            isWaitingToPlayMove = false;
                            PlayMove(moveToPlay);
                        }
                    }
                }

                if (hasBotTaskException)
                {
                    hasBotTaskException = false;
                    botExInfo.Throw();
                }

                if (PlayerWhite.IsHuman || PlayerBlack.IsHuman)
                {
                    fastForward = false;
                }

            } while (fastForward && isWaitingToPlayMove && tempControlVariable < 3000);
            tempControlVariable = 0;
        }

        public void Draw()
        {
            boardUI.Draw();
            string nameW = GetPlayerName(PlayerWhite);
            string nameB = GetPlayerName(PlayerBlack);
            boardUI.DrawPlayerNames(nameW, nameB, PlayerWhite.TimeRemainingMs, PlayerBlack.TimeRemainingMs, isPlaying);
        }
        

        public void DrawOverlay()
        {
            BotBrainCapacityUI.Draw(tokenCount, debugTokenCount, MaxTokenCount);
            MenuUI.DrawButtons(this);
            MaterialDiffUI.currentlyExploredFenString = FenUtility.CurrentFen(board);
            MaterialDiffUI.DrawMaterialDiff(this, boardUI);
            EvalBarUI.DrawEvalBar(this);
            MatchStatsUI.DrawMatchStats(this);
        }

        static string GetPlayerName(ChessPlayer player) => GetPlayerName(player.PlayerType);
        static string GetPlayerName(PlayerType type) => type.ToString();

        public void StartNewBotMatch(PlayerType botTypeA, PlayerType botTypeB)
        {
            EndGame(GameResult.DrawByArbiter, log: false, autoStartNextBotMatch: false);
            botMatchGameIndex = 0;
            totalMovesPlayed = 0;
            trueTotalMovesPlayed = 0;
            totalTimeTakenByBotA = 0;
            totalTimeTakenByBotB = 0;
            string nameA = GetPlayerName(botTypeA);
            string nameB = GetPlayerName(botTypeB);
            if (nameA == nameB)
            {
                nameA += " (A)";
                nameB += " (B)";
            }
            BotStatsA = new BotMatchStats(nameA);
            BotStatsB = new BotMatchStats(nameB);
            botAPlaysWhite = true;
            Log($"Starting new match: {nameA} vs {nameB}", false, ConsoleColor.Blue);
            StartNewGame(botTypeA, botTypeB);
        }


        ChessPlayer PlayerToMove => board.IsWhiteToMove ? PlayerWhite : PlayerBlack;
        ChessPlayer PlayerNotOnMove => board.IsWhiteToMove ? PlayerBlack : PlayerWhite;

        public int TotalGameCount => botMatchStartFens.Length * 2;
        public int CurrGameNumber => Math.Min(TotalGameCount, botMatchGameIndex + 1);
        public string AllPGNs => pgns.ToString();


        bool IsLegal(Move givenMove)
        {
            var moves = moveGenerator.GenerateMoves(board);
            foreach (var legalMove in moves)
            {
                if (givenMove.Value == legalMove.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public class BotMatchStats
        {
            public string BotName;
            public int NumWins;
            public int NumLosses;
            public int NumDraws;
            public int NumTimeouts;
            public int NumIllegalMoves;
            public float timePerTurn;

            public BotMatchStats(string name) => BotName = name;
        }

        public void Release()
        {
            boardUI.Release();
        }
    }
}
