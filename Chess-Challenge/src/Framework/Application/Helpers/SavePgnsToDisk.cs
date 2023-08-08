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
using System.Security.Cryptography;
using System.Text;

namespace ChessChallenge.Application
{
    public class SavePgnsToDisk
    {
        private string pgnToSave;

        static public void SavePgnToDisk(string pgnToSave, string botName)
        {
            string scriptHash = GetPlayerHash(botName);

            string directoryPath;
            if (MenuUI.customSaveFilePath == @"C:\replace this with your desired directory, goes to documents by default")
            {
                directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            else
            {
                directoryPath = MenuUI.customSaveFilePath;
            }
            directoryPath = Path.Combine(directoryPath, "ChessChallenge-SUI/" + botName);
            Directory.CreateDirectory(directoryPath);
            try
            {
                File.AppendAllText(Path.Combine(directoryPath, scriptHash + ".txt"), pgnToSave);
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public static string GetPlayerHash(string botToGetCodeOf)
        {
            string botCodeInPlainText = Path.Combine(Directory.GetCurrentDirectory(), "src", "My Bot", $"{botToGetCodeOf}.cs");
            if(botCodeInPlainText.Contains("Human")){
                return " Hhuman Doesn'tMatterWhatWeReturnHere";
            }
            botCodeInPlainText = File.ReadAllText(botCodeInPlainText);
            //Console.WriteLine(botCodeInPlainText);
            return GetSHA256Hash(botCodeInPlainText);
        }

        static string GetSHA256Hash(string stringToHash) //this name is a lie unless I actually need Sha like I thought, change back eventually
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(stringToHash);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                // Convert the byte array to a hexadecimal string representation
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2")); // "x2" means the byte will be converted to a two-digit hexadecimal number
                }

                return builder.ToString();
            }
        }
        
        public static string GetPlayerName(string pgn, bool isWhitenotBlack)
        {
            string pattern;
            // look through pgns for [White "Player Name"]
            if (isWhitenotBlack)
            {
                pattern = @"\[White ""(.*?)""\]";
            }
            else
            {
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
    }
}