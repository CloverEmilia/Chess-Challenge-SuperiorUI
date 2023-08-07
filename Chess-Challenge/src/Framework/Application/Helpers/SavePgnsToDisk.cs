using Raylib_cs;
using System.Numerics;
using System;
using System.IO;
using ChessChallenge.Chess;
using System.Text.RegularExpressions;
using static ChessChallenge.Application.ChallengeController;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Reflection;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Collections.Generic;

namespace ChessChallenge.Application
{
    public class SavePgnsToDisk
    {
        private string pgnToSave;

        static public void SavePgnToDisk(string pgnToSave, string innerFilePath)
        {
                string directoryPath;
            if (MenuUI.customSaveFilePath == @"C:\replace this with your desired directory, goes to documents by default")
            {
                directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            else
            {
                directoryPath = MenuUI.customSaveFilePath;
            }

            string scriptHash = "for now just hardcodedInsteadOfRealHash";
            directoryPath = Path.Combine(directoryPath, "ChessChallenge-SUI/" + innerFilePath);
            Directory.CreateDirectory(directoryPath);
            try
            {
                File.AppendAllText(Path.Combine(directoryPath, scriptHash + ".txt"), pgnToSave);
            }
            catch (UnauthorizedAccessException ex)
            {
                // Handle the exception, e.g., log the error, show an error message, etc.
                Console.WriteLine("Error: " + ex.Message);
            }

            /* old save system
            string fileName = FileHelper.GetUniqueFileName(directoryPath, "games", ".txt");
            string fullPath = Path.Combine(directoryPath, fileName);
            File.WriteAllText(fullPath, pgns);
            ConsoleHelper.Log("Saved games to " + fullPath, false, ConsoleColor.Blue);
            */

            //Assembly s = GetAssemblyFromPlayerType(GetPlayerTypeFromName(GetWhitePlayerName(examinedpgn)));
            //Console.WriteLine(s);
            //string uniqueBotID = GetSHA256Hash(GetAssemblyFromPlayerType(GetPlayerTypeFromName(GetWhitePlayerName(examinedpgn))));
            //Console.WriteLine(uniqueBotID);
            //}

        }

        public static string GetPlayerName(string pgn, bool isWhitenotBlack)
        {
            string pattern;
            // look through pgns for [White "Player Name"]
            if (isWhitenotBlack){
                pattern = @"\[White ""(.*?)""\]";
            } else{
                pattern = @"\[Black ""(.*?)""\]";
            }
            // Use Regex.Match to find the first match
            Match match = Regex.Match(pgn, pattern);
            // If a match is found and there is a captured group, return the player name
            if (match.Success && match.Groups.Count >= 2)
            {
                return match.Groups[1].Value;
            }
            return string.Empty; // this should never happen, if it does then it's not like we can error in a particularly more clear way I suppose?
        }

        static PlayerType GetPlayerTypeFromName(string playerName)
        {
            if (Enum.TryParse<PlayerType>(playerName, out var playerType))
            {
                return playerType;
            }
            return PlayerType.Human; // or any default value you prefer if the name doesn't match any PlayerType
        }

        static System.Reflection.Assembly GetAssemblyFromPlayerType(PlayerType playerType)
        {
            if (BotTypeMap.TryGetValue(playerType, out var botType))
            {
                string botSourceCode = GetBotSourceCodeFromType(botType);
                if (!string.IsNullOrEmpty(botSourceCode))
                {
                    // Compile the bot source code and load the assembly
                    return CompileAssemblyFromSource(botSourceCode);
                }
                else
                {
                    Console.WriteLine($"Bot source code not found for player type: {playerType}");
                }
            }
            else
            {
                Console.WriteLine($"Bot type not found in BotTypeMap for player type: {playerType}");
            }

            return null; // Return null if the bot type is not found in the BotTypeMap or if the assembly couldn't be loaded.
        }

        static string GetBotSourceCodeFromType(Type botType)
        {
            string botFileName = botType.Name + ".cs";
            string currentDirectory = Directory.GetCurrentDirectory();
            string parentDirectory = Directory.GetParent(currentDirectory)?.FullName;
            string chessChallengeDirectory = Directory.GetParent(parentDirectory)?.FullName;
            string chessChallengeSrcDirectory = Path.Combine(chessChallengeDirectory, "Chess-Challenge", "src");
            string botDirectory = Path.Combine(chessChallengeSrcDirectory, "My Bot");
            string botFilePath = Path.Combine(botDirectory, botFileName);

            Console.WriteLine($"Bot File Path: {botFilePath}");

            if (File.Exists(botFilePath))
            {
                return File.ReadAllText(botFilePath);
            }

            Console.WriteLine($"notexist");
            return null;
        }

        static System.Reflection.Assembly CompileAssemblyFromSource(string sourceCode)
        {
            CompilerParameters compilerParams = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true
            };

            CSharpCodeProvider codeProvider = new CSharpCodeProvider();

            // Compile the source code and get the results
            CompilerResults results = codeProvider.CompileAssemblyFromSource(compilerParams, sourceCode);

            // Check for any compilation errors
            if (results.Errors.HasErrors)
            {
                // Handle compilation errors
                foreach (CompilerError error in results.Errors)
                {
                    Console.WriteLine($"Error ({error.ErrorNumber}): {error.ErrorText}");
                }
                return null;
            }

            // Return the compiled assembly
            return results.CompiledAssembly;
        }

        static string GetSHA256Hash(System.Reflection.Assembly botAssembly) //this name is a lie unless I actually need Sha like I thought, change back eventually
        {
            int assemblyHashCode = botAssembly.GetHashCode();

            // Convert the hash code to a hexadecimal string
            string hashString = assemblyHashCode.ToString("X");

            return hashString;
        }

        static byte[] Serialize(object data)
        {
            // Convert the data to a JSON string
            string jsonString = JsonSerializer.Serialize(data);

            // Convert JSON string to a byte array
            return Encoding.UTF8.GetBytes(jsonString);
        }
    }
}