using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using AMXWrapper;


using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Diagnostics;

namespace telepawn.Scripting
{
    public static class Natives
    {
        public static int printc(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            try
            {
                Utils.Log.WriteLine(args1[0].AsString());
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex);
            }


            return 1;
        }

        public static int Loadscript(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 1) return 0;
            if (args1[0].AsString().Length == 0)
            {
                Utils.Log.Error(" [command] You did not specify a correct script file!");
                return 0;
            }

            if (!System.IO.File.Exists("Scripts/" + args1[0].AsString() + ".amx"))
            {
                Utils.Log.Error(" [command] The script file " + args1[0].AsString() + ".amx does not exist in /Scripts/ folder.");
                return 0;
            }
            Script scr = new Script(args1[0].AsString(), true);
            AMXWrapper.AMXPublic pub = scr.m_Amx.FindPublic("OnInit");
            if (pub != null) pub.Execute();
            return 1;
        }

        public static int Unloadscript(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 1) return 1;
            if (args1[0].AsString().Length == 0)
            {
                Utils.Log.Error(" [command] You did not specify a correct script file!");
                return 0;
            }

            foreach (Script sc in Program.m_Scripts)
            {
                if (sc.m_AmxFile.Equals(args1[0].AsString()))
                {
                    AMXWrapper.AMXPublic pub = sc.m_Amx.FindPublic("OnUnload");
                    if (pub != null) pub.Execute();
                    sc.m_Amx.Dispose();
                    sc.m_Amx = null;
                    Program.m_Scripts.Remove(sc);
                    Utils.Log.Info("[CORE] Script '" + args1[0].AsString() + "' unloaded.");
                    return 1;
                }
            }
            Utils.Log.Error(" [command] The script '" + args1[0].AsString() + "' is not running.");
            return 1;
        }

        public static int gettimestamp(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            return (Int32)DateTimeOffset.Now.ToUnixTimeSeconds();
        }

        public static int SetTimer(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1[2].AsInt32() > 1 || args1[2].AsInt32() < 0)
            {
                Utils.Log.Error("SetTimer: Argument 'repeating' is boolean. Please pass 0 or 1 only!");
                return 1;
            }

            try
            {
                ScriptTimer timer = new ScriptTimer(args1[1].AsInt32(), Convert.ToBoolean(args1[2].AsInt32()), args1[0].AsString(), caller_script);
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
            }
            return (Program.m_ScriptTimers.Count);
        }

        public static int KillTimer(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            foreach (ScriptTimer scrt in Program.m_ScriptTimers)
            {
                if (scrt != null)
                {
                    if (scrt.ID == args1[0].AsInt32())
                    {
                        scrt.KillTimer();
                        return 1;
                    }
                }
            }
            return 1;
        }




        public static int GetBotUserName(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 1) return 0;
            try
            {
                Debug.WriteLine(Program.m_TelegramHandle.m_User.Username);
                AMX.SetString(args1[0].AsCellPtr(), Program.m_TelegramHandle.m_User.Username, true);
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'GetBotUserName' (dest_string must be an char array, or received invalid parameters!)" + caller_script);
            }
            return 1;
        }

        public static int GetBotName(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 2) return 0;
            try
            {
                AMX.SetString(args1[0].AsCellPtr(), Program.m_TelegramHandle.m_User.FirstName, true);
                AMX.SetString(args1[1].AsCellPtr(), Program.m_TelegramHandle.m_User.LastName, true);

                var res = Program.m_TelegramHandle.m_TelegramBotClient.GetMeAsync();
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'GetBotName' (dest_string must be an char array, or invalid parameters!)" + caller_script);
            }
            return 1;
        }



        public static int SendChatMessage(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 2) return 0;
            /*Task.Run(async () =>
            {
                await Program.m_TelegramHandle.m_TelegramBotClient.SendTextMessageAsync(Convert.ToString(args1[0].AsString()), Convert.ToString(args1[1].AsString())).ConfigureAwait(false);
            });*/
            Program.m_TelegramHandle.m_TelegramBotClient.SendTextMessageAsync(Convert.ToString(args1[0].AsString()), Convert.ToString(args1[1].AsString()));
            return 1;
        }

        public static int DeleteChatMessage(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 2) return 0;
            Task.Run(async () =>
            {
                await Program.m_TelegramHandle.m_TelegramBotClient.DeleteMessageAsync(Convert.ToString(args1[0].AsString()), args1[1].AsInt32()).ConfigureAwait(false);
            });
            return 1;
        }

        public static int GetChatType(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 1) return 0;
            try
            {
                var res = Task.Run(async () =>
                {
                    Chat _Chat = await Program.m_TelegramHandle.m_TelegramBotClient.GetChatAsync(args1[0].AsString()).ConfigureAwait(false);
                    return _Chat;
                });

                switch (res.Result.Type)
                {
                    case ChatType.Private:
                        return 0;
                    case ChatType.Group:
                        return 1;
                    case ChatType.Channel:
                        return 2;
                    case ChatType.Sender:
                        return 3;
                    case ChatType.Supergroup:
                        return 4;
                }
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'GetChatType' (invalid chatid?)" + caller_script);
            }
            return -1;
        }

        public static int GetChatDescription(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 2) return 0;
            try
            {
                var res = Task.Run(async () =>
                {
                    Chat _Chat = await Program.m_TelegramHandle.m_TelegramBotClient.GetChatAsync(args1[0].AsString()).ConfigureAwait(false);
                    return _Chat.Description;
                });
                AMX.SetString(args1[1].AsCellPtr(), res.Result, true);
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'GetChatDescription' (dest_string must be an char array, invalid parameters, or you try to get description from a private chat)" + caller_script);
            }
            return 1;
        }

        public static int GetChatSlowModeDelay(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 1) return 0;
            try
            {
                var res = Task.Run(async () =>
                {
                    Chat _Chat = await Program.m_TelegramHandle.m_TelegramBotClient.GetChatAsync(args1[0].AsString()).ConfigureAwait(false);
                    return _Chat;
                });
                return Convert.ToInt32(res.Result.SlowModeDelay);
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'GetChatSlowModeDelay' (invalid chat id, or not a supergroup)" + caller_script);
            }
            return 1;
        }

        public static int PinChatMessage(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 2) return 0;
            Task.Run(async () =>
            {
                await Program.m_TelegramHandle.m_TelegramBotClient.PinChatMessageAsync(Convert.ToString(args1[0].AsString()), args1[1].AsInt32()).ConfigureAwait(false);
            });
            return 1;
        }

        public static int UnpinChatMessage(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 2) return 0;
            Task.Run(async () =>
            {
                await Program.m_TelegramHandle.m_TelegramBotClient.UnpinChatMessageAsync(Convert.ToString(args1[0].AsString()), args1[1].AsInt32()).ConfigureAwait(false);
            });
            return 1;
        }

        public static int UnpinAllChatMessages(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 1) return 0;
            Task.Run(async () =>
            {
                await Program.m_TelegramHandle.m_TelegramBotClient.UnpinAllChatMessages(Convert.ToString(args1[0].AsString())).ConfigureAwait(false);
            });
            return 1;
        }

        public static int GetUserName(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 2) return 0;
            try
            {
                var res = Task.Run(async () =>
                {
                    Chat _Chat = await Program.m_TelegramHandle.m_TelegramBotClient.GetChatAsync(args1[0].AsString()).ConfigureAwait(false);
                    return _Chat.Username.ToString();
                });
                AMX.SetString(args1[1].AsCellPtr(), res.Result, true);
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'GetUserName' (dest_string must be an char array, or invalid parameters!)" + caller_script);
            }
            return 1;
        }

        public static int GetUserBio(AMX amx1, AMXArgumentList args1, Script caller_script)
        {
            if (args1.Length != 2) return 0;
            try
            {
                var res = Task.Run(async () =>
                {
                    Chat _Chat = await Program.m_TelegramHandle.m_TelegramBotClient.GetChatAsync(args1[0].AsString()).ConfigureAwait(false);
                    return _Chat.Bio.ToString();
                });
                AMX.SetString(args1[1].AsCellPtr(), res.Result, true);
            }
            catch (Exception ex)
            {
                Utils.Log.Exception(ex, caller_script);
                Utils.Log.Error("In native 'GetUserBio' (dest_string must be an char array, invalid parameters, or you try to get bio from an group chat)" + caller_script);
            }
            return 1;
        }
    }
}
