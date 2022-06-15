# TelePawn

## [Read the Wiki for help](https://github.com/michael-fa/TelePawn/wiki) (W.I.P)


A .NET Console App that lets you script your Telegram Bots using the awesome [PAWN](https://github.com/pawn-lang) scripting language.
The core goal is to make Telegram Bots more easier for everyone - given enough time for people to put out scripts.
For more about PAWN [click here](https://www.compuphase.com/pawn/pawn.htm).
This project depends heavily on [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot). A .NET package allowing me to interact with Telegram's API as easy as possible.
Mirroring the project and idea from my previous project idea (discordamx), I was still able to get quickly into their (telegram) stuff, I am thankful for the work!
```
main()
{
    print("Test TelePawn Script loaded!\n");
}

public OnInit()
{
    printf("OnInit\n");
    return 1;
}

public OnUnload()
{
    print("OnUnload\n");
    return 1;
}

public OnChatMemberAdded(chatid[], userid[])
{
    new str[123];
    GetUserBio(userid, str);
    printf("%s joined the chat %s, %s\n", GetName(userid), chatid, str);
    return 1;
}

public OnChatMemberLeft(chatid[], userid[])
{
    printf("%s left the chat %s\n", GetName(userid), chatid);
    return 1;
}

public OnTelegramMessage(chatid[], messageid, userid[], message[]) //Userid can be longer than an integer, so we pass it as a string array.
{
        //Here you can see whats possible(really basic)

    /*if(IsChatPrivate(chatid))
        SendChatMessage(chatid, "Private chat message..");
    else
        SendChatMessage(chatid, "Public chat message");*/

    printf("Message(id: %d) from %s(id: %s) in chat %s: %s\n", messageid, userid, GetName(userid), chatid, message);

   //Another example (pins a message, but not really, just pinning the comment itself)

   /*if(strequals(message, "pin"))
        PinChatMessage(chatid, messageid);
    else if(strequals(message, "unpin"))
        PinChatMessage(chatid, messageid);
    else SendChatMessage(chatid, "What do you want to test? pin | unpin");*/


    return 1;
}

public OnTelegramError(errorcode, error_source[], error_message[], error[])
{
    printf("Telegram Error: %s(%d) at %s: %s", error, errorcode, error_source, error_message);
    return 1;
}

public OnConsoleInput(input[])
{
    return 1;
}
```

---

## :construction: EARLY ALPHA

The whole project is still in its beta, meaning there will be many changes and if you are not somewhat familiar with PAWN Scripting you should come back later. You can test stuff, read the [Wiki](https://github.com/michael-fa/telepawn/wiki) for easy entry. ** Just keep in mind that functions and other things may change are be removed completely, so keep the bot updated.

## Testers
Even though I may forget to mention things or stuff will be changed over time, it should be kinda easy for anyone having done anything with scripting, game servers or something like that to set this up. Scripting is another thing, but it's basic, simple and almost plain logic.  
Run it on windows and make sure you have <b>[.NET Runtime 4.8.*](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48)</b> ! 

**If you are an experienced PAWN coder, and you're interested in testing this (given you have some knowledge about telegram bots too)
I'll offer to host a bot for TESTERS -> I am currently looking for 2-3 official project testers that are able to test files asap when I send them. Asap meaning a test-result in 24 hours.**

Linux support is coming soon!


## Current Features
* Load & unload filterscripts 
* Send messages to users or delete them
* Pin messages
* Get user bio and channel description
* A few more tools
* Load plugins ( **extending the internal bot functionality... nothing script-related other then that it's adding script functions.** )
* Set and delete timers


## Planned for first release
* Too much. We aim for basic chat functionality, and moving to image and audio handling soon too.
* Media files just have to be put in scriptfiles folder allowing scripters direct access and work with the media.

### Third party info
* Using [PAWN Wrapper by ikkentim](https://github.com/ikkentim/AMXWrapper)
* Using [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot)

[Visit CompuPhase Pawn Language](https://www.compuphase.com/pawn/pawn.htm) site for more info!
