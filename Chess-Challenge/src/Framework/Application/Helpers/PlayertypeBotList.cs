using ChessChallenge.API;
using System.Collections.Generic;
using System;


namespace ChessChallenge.Application
{
    
    public static class PlayertypeBotList
    {
        //As far as I know
        //There's no legitimate reason whatsoever to manually edit this file
        //but you do you

        public enum BotPlayerType
        {
            //Begin ListA
            Human,
            MyBot,
            EvilBot,
            //ect.
        }

        public static readonly Dictionary<PlayerType, Type> BotTypeMap = new Dictionary<PlayerType, Type>
        {
            //Humans aren't real
            //Begin ListB
            { PlayerType.MyBot, typeof(MyBot) },
            { PlayerType.EvilBot, typeof(EvilBot) },
            //ect.
        };

    }
}
