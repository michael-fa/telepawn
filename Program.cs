using System.Threading;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using telepawn.Utils;
using telepawn.Scripting;
using System.Text.RegularExpressions;

namespace telepawn
{
    public class Program
    {
        //Coreinfo
        static bool m_isWindows;
        static bool m_isLinux;

        public static Telegram.TelegramHandle m_TelegramHandle = null;
        public static List<Scripting.ScriptTimer> m_ScriptTimers = null;
        public static List<Plugins.Plugin> m_Plugins = null;
        public static List<Scripting.Script> m_Scripts = null;
        public static List<Plugins.PluginNatives> m_PluginNatives = null;
      
        public struct Settings
        {
            public string _botToken;
            public string _firstScript;
            public bool _clientMode;
        } public static Settings m_Settings;





        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig)
        {
            switch (sig)
            {
                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                    StopSafely();
                    return false;
                default:
                    return false;
            }
        }




        static void Main(string[] args)
        {
            //Environment - Set the OS
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) m_isWindows = true;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) m_isLinux = true;
            else StopSafely();

            //Set the console handler, catching events such as close.
            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);

            __InitialChecks();
            __InitialSetup();

            m_TelegramHandle.StartReceiving();

            //Print a time and date to log file
            File.AppendAllText("Logs/current.txt", "\n++++++++++++++++++++ | LOG " + DateTime.Now + " | ++++++++++++++++++++\n");
            
            //Console initial message
            Log.Info("INIT: -> Telegram AMX Bot © 2022 - www.fanter.eu <-");
            if (m_isWindows) Log.Info("INIT: -> Running on Windows.");
            else if (m_isLinux) Log.Info("INIT: Running on Linux. (Make sure you are always up to date!");

            //Check and warn about client mode.
            if (m_Settings._clientMode)
                Log.Info("INIT: -> Running in TELEGRAM CLIENT mode!");


            //PREPARE (not loading!) all plugins (extensions)
            try
            {
                foreach (string fl in Directory.GetFiles("Plugins/"))
                {
                    if (!fl.EndsWith(".dll")) continue;
                    Utils.Log.Info("[CORE] Found plugin: '" + fl + "' !");
                    new Plugins.Plugin(fl);
                }
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex);
                Program.StopSafely();
                return;
            }


            //Load main.amx, or error out if not available
            if (!File.Exists("Scripts/main.amx"))
            {
                Log.Error("No 'main.amx' file found. Make sure there is at least one script called main!");
                StopSafely();
                return;
            }
            else new Script(Program.m_Settings._firstScript);

            //Now add all filterscripts
            try
            {
                foreach (string fl in Directory.GetFiles("Scripts/"))
                {
                    Match mtch = Regex.Match(fl, "(?=/!).*(?=.amx)");
                    // demand load main.amx     ||  skip this file
                    if (fl.Contains("main.amx") || !fl.EndsWith(".amx") || mtch.Success) continue;
                    Log.Info("[CORE] Found filterscript: '" + mtch.Value.ToString().Remove(0, 1) + "' !");
                    new Script(mtch.Value.ToString().Remove(0, 1), true);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                StopSafely();
                return;
            }


            //We first want to prefetch (only call constructor methods, returning us the natives) and then, above, load all the scripts and finally "really load" the plugins.
            PluginTools.LoadAllPlugins();

        //Handle commands.
        _CMDLOOP:
            Utils.ConsoleCommand.Loop();
            goto _CMDLOOP;
        }

        //Checking for all different kinds of stuff so we're good to go.
        static private void __InitialChecks()
        {
            //Check if LOG Dir exists
            if (!Directory.Exists("Logs/"))
                Directory.CreateDirectory("Logs/");

            //Check if Plugins dir exists
            if (!Directory.Exists("Plugins/"))
                Directory.CreateDirectory("Plugins/");

            //Check if Scripts dir exists
            if (!Directory.Exists("Scripts/"))
                Directory.CreateDirectory("Scripts/");

            //Check if config.ini exists
            if (!File.Exists("config.ini"))
            {
                FileStream xx = File.Create("config.ini");
                xx.Close();
                xx.Dispose();
                File.AppendAllText("config.ini", "# (C) 2022 fanter.eu - TELEPAWN\n\n# This is your server config - the \"core\" section has to be set before you consider running the bot!");
            }

            //"Scan" the config file before using it.
            IniFile x = new IniFile("config.ini");

            if (!x.KeyExists("telegram_bot_token", "core"))
                x.Write("telegram_bot_token", "Not set", "core");

            if (!x.KeyExists("first_script", "core"))
                x.Write("first_script", "main", "core");

            if (!x.KeyExists("client_mode", "core"))
                x.Write("client_mode", "false", "core");

        }



        //Setting everything up AFTER InitialChecks are done!
        static private void __InitialSetup()
        {
            //Setting everything up
           
            Program.m_ScriptTimers = new List<Scripting.ScriptTimer>();
            Program.m_PluginNatives = new List<Plugins.PluginNatives>();   //Create list for scripts
            Program.m_Plugins = new List<Plugins.Plugin>();   //Create list for plugins
            Program.m_Scripts = new List<Scripting.Script>();   //Create list for scripts

            //Get data from config.ini
            IniFile x = new IniFile("config.ini");
            m_Settings._botToken = x.Read("telegram_bot_token", "core");
            m_Settings._firstScript = x.Read("first_script", "core");
            m_Settings._clientMode = Convert.ToBoolean(x.Read("client_mode", "core"));
            GC.Collect();


            //Start telegram client or bot api.
            if (m_Settings._clientMode)
            {

            }
            else Program.m_TelegramHandle = new Telegram.TelegramHandle();//This belongs here, because it accesses m_Settings, which is now accessable for the first time.
        }

        static public void StopSafely()
        {

            foreach (Plugins.Plugin pl in m_Plugins)
            {
                pl.Unload(0);

                Log.WriteLine("Script " + pl._File + " unloaded.");
            }

            foreach (Script script in m_Scripts)
            {
                if (script.m_Amx == null) continue;

                script.StopAllTimers();

                if (script.m_Amx.FindPublic("OnUnload") != null)
                    script.m_Amx.FindPublic("OnUnload").Execute();

                script.m_Amx.Dispose();
                script.m_Amx = null;
                Log.WriteLine("Script " + script.m_AmxFile + " unloaded.");
            }

            //copy current log txt to one with the date in name and delete the old one
            File.Copy("Logs/current.txt", ("Logs/" + DateTime.Now.ToString().Replace(':', '-') + ".txt"));
            if (File.Exists("Logs/current.txt")) File.Delete("Logs/current.txt");
            Environment.Exit(0);
            
        }

    }
}
