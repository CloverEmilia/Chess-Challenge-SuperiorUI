# Chess Coding Challenge SuperiorUI
# NOTICE, this is currently pre-release, you're welcome to use it, but, this information is not guarenteed to be correct, and anything may change at any time.


## Features
-Entirely reworked and unified UI

-Extremely simplified process for adding and playing bots (see the .txt in MyBot), featuring a codeless, infinitely scrolling bot selector

-Tourneyment mode for automatically running 

-Stockfish evaluation bar (//see known issues)

-Overhauled save system, featuring bot uniqueness checks via hashing code, to automatically distinguish between old and new versions of bots

-Material Difference UI (🤏that tiny thing with the pieces that have been taken)

-Squishy Buttons :D

## Integrations
-Geda's UCI implimentation

-MoonWalker's MatchStatsUI

-Aug's stockfishbot.cs is the indirect basis for the stockfish evaluation bar

-Aug's Tier 1 elo bot

-JW's 2 elo bot

-Odin's pgn marking

-Typin's duck piece textures (quack)


## Changes
-Everything (afaik) now uses BoardTheme.cs, so changing it should result in more pleasant and cohesive results, also expanded with text color options

-Screen size can now be changed incrimentally

-Added TimePerMove to bot stats

-Saving now happens per a customizable number of games

-Removed .examples namespace so evilbot can be safely removed

## Known Issues
-Eval Bar broke-y okey, should in theory work but I haven't been able to get someone to confirm if there's an issue with my implimentation or something purely on my end

-Fast Forward hangs the client, this isn't anything new to my mod but it is something I'm hoping to band-aid it by dropping out of it every few seconds for a short bit.

-BoardUI doesn't scale correctly (need to raylib.scaleint each value only once)

-Hashing for bots is broken, but I know how to fix it, just need to get around to it

-the Grey border under the bots name, and positioning for the time, is messed up, this is also obviously fixable, but just not highest-priority

-I have not personally confirmed I integrated UCI in correctly, I think I did, but, I personally wasn't able to get it to work

## TODO And Version History
----Coming Soon----

-Setup board from fenn string (if I can get windows.forms or something 3rd party working)

-Minor to Moderate prettification, depending on how many remaing ideas work out.

-Move history

-Torneyment Mode

----Probably Coming----

-UI hook for realtime changing of integer bot variables

-Undo button

----Do not get your hopes up----

-Bot brain capacity by function

-Sound

-Markup arrows

-Integrated stats explorer
