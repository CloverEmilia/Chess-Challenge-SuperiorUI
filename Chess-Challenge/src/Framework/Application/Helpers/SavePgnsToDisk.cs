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

namespace ChessChallenge.Application
{
    public static class SavePgnsToDisk
    {

        /* old save system
        string pgns = controller.AllPGNs;
        string directoryPath = Path.Combine(FileHelper.AppDataPath, "Games");
        Directory.CreateDirectory(directoryPath);
        string fileName = FileHelper.GetUniqueFileName(directoryPath, "games", ".txt");
        string fullPath = Path.Combine(directoryPath, fileName);
        File.WriteAllText(fullPath, pgns);
        ConsoleHelper.Log("Saved games to " + fullPath, false, ConsoleColor.Blue);
        */
    

    static string GetWhitePlayerName(string pgn)
    {
        // look through pgns for [White "Player Name"]
        string pattern = @"\[White ""(.*?)""\]";
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

