# Chess Coding Challenge SuperiorUI
# NOTICE, this is currently pre-release, you're welcome to use it, but, this information is not guarenteed to be correct, and anything may change at any time.


## Features
-Entirely reworked and unified UI

-Extremely simplified process for adding (see the .txt in MyBot) and playing bots, feauturing a no-code-needed, infinitely scrolling duel-sided bot selector

-Non-hanging fast-forward

-Tourneyment mode for automatically round-robin comparison of all currently installed bots

-Stockfish evaluation bar (//see known issues)

-Overhauled save system, featuring bot uniqueness checks via hashing code, to automatically distinguish between old and new versions of bots (//not yet comparing uniqueness between bot names)

-Material Difference UI (🤏that tiny thing with the pieces that have been taken)

-Squishy Buttons :D

## Integrations
-Geda's UCI implimentation

-MoonWalker's MatchStatsUI

-Aug's Tier 1 elo bot

-JW's 2 elo bot

-Odin's pgn marking

-Typin's duck piece textures (quack)

-Countless bug fixes and suggestions from Toanth

## Changes
-Everything (afaik) now uses BoardTheme.cs, so changing it should result in more pleasant and cohesive results, also expanded with text color options

-Screen size can now be changed incrimentally

-Added (mS spent)TimePerMove to bot stats

-Saving now happens per a customizable number of games

-Removed .examples namespace so evilbot can be safely removed

## Known Issues
-Eval bar backend at least 85% working, eval bar frontend 10-65% working

-BoardUI doesn't scale correctly (need to raylib.scaleint each value only once)

-the Grey border under the bots name, and positioning for the time, is messed up, this is fixable, just not highest-priority

-I only put moderate effort into confirming I integrated UCI support in correctly, I think I did, but, I can only confirm I *think* it worked.

## TODO and Version History
----Coming Soon----

-Fix weird piecelist errors when selecting bots weirdly (as soon as I know what that is, top priority)

-Cleaned up and commented code, cleaned usings;,,

-Save tournament results seperately (parralel to normal saves)

-Setup board from fenn string (if I can get windows.forms or something 3rd party working), and/or a fen string.txt selector

-Minor to Moderate prettification, depending on how many remaing ideas work out.

-Easter eggs tab (for fun little rendering hehe's)

----Probably Coming----

-UI hook for realtime changing of integer bot variables

-Undo button

-I'll port in my "Champion" custom fen start list, to be used in a mini-tournament

-Move history

----Don't get your hopes up----

-Example Bot with four-phased (I think, I think enemy thinks, I think enemy thinks, I think enemy thinks I think), for testing/demonstrating "weirdo" bots with odd preferences.

-Bot brain capacity by function

-Sound

-Right Click Markup arrows

-Integrated stats explorer

-Online bot database (just an alternative to the current public github repos and #post my bot channel, not online play)

-A fun little minigame I won't spoil

-Integrated stats explorer
