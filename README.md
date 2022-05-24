# TelePawn

## [Read the Wiki for help](https://github.com/michael-fa/TelePawn/wiki) (W.I.P)


A .NET Console App that lets you script your Telegram Bots using the awesome [PAWN](https://github.com/pawn-lang) scripting language.
The core goal is to make Telegram Bots more easier for everyone - given enough time for people to put out scripts.
For more about PAWN [click here](https://www.compuphase.com/pawn/pawn.htm).
This project depends heavily on [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot). A .NET package allowing me to interact with Telegram's API as easy as possible.
Mirroring the project and idea from my previous project idea (discordamx), I was still able to get quickly into their (telegram) stuff, I am thankful for the work!
```
#include <a_telepawn>

main()
{
	print(" => Awesome Telegram Bot starting!\n");
}

public OnInit()
{
    printc("OnInit");
    return 1;
}

public OnUnload()
{
    printc("OnUnload");
    return 1;
}

public OnTelegramMessage(userid[], message[])
{
    new str[32];
    strformat(str, sizeof(str), true, "Got message: %s from %s", message, userid);
    print(str);
    SendChatMessage(userid, "Whatever you said, I received it, and it's awesome!");
    return 1;
}
```

---

## :construction: EARLY ALPHA

The whole project is still in its super early alpha, meaning there will be many changes and if you are not experienced in PAWN Scripting you should come back later! Leave a watch! 

## Testers
The current RELEASE is meant for experienced scripters and is only for pure testing.
Run it on windows and make sure you have <b>[.NET Runtime 4.8.*](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48)</b> ! 

**If you are an experienced PAWN coder, and you're interested in testing this (given you have some knowledge about telegram bots too)
I'll offer to host a bot for TESTERS -> I am currently looking for 2-3 official project testers!**

:warning: The current aim of this is self-hosting - remember: NOT SELF-BOT. You need (to rent) a (v)Server running either Linux or Windows on it for 24/7 runtime!
Linux support is coming soon!


## Current Features
* Load & unload filterscripts 
* Send messages to users
* Load plugins ( **extending the internal bot functionality... nothing script-related other then that it's adding script functions.** )
* Set and delete timers


## Planned for first release
* Too much. We aim for basic chat functionality, and moving to image and audio handling soon too.
* Media files just have to be put in scriptfiles folder allowing scripters direct access and work with the media.

### Third party info
* Using [PAWN Wrapper by ikkentim](https://github.com/ikkentim/AMXWrapper)
* Using [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot)

[Visit CompuPhase Pawn Language](https://www.compuphase.com/pawn/pawn.htm) site for more info!
