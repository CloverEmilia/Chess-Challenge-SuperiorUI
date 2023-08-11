using System;
using System.Collections.Generic;

namespace ChessChallenge.Application
{
    public static class SelfWritingBotList
    {
        public static Dictionary<ChallengeController.PlayerType, Type> BotTypeMap = new Dictionary<ChallengeController.PlayerType, Type>
        {
            { ChallengeController.PlayerType.MyBot, typeof(MyBot) },
            { ChallengeController.PlayerType.EvilBot, typeof(EvilBot) },
            { ChallengeController.PlayerType.NegamaxBasic, typeof(NegamaxBasic) },
            { ChallengeController.PlayerType.AdvancedNegamax, typeof(AdvancedNegamax) },
        };
    }
}
