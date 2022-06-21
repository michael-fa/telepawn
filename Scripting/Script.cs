using AMXWrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace telepawn.Scripting
{
    public class Script
    {
        public string m_AmxFile = null;
        public AMX m_Amx;
        public bool m_isFs;


        public Script(string _amxFile, bool _isFilterscript = false)
        {
            this.m_AmxFile = _amxFile;
            try
            {
                this.m_Amx = new AMX("Scripts/" + _amxFile + ".amx");
            }
            catch (Exception e)
            {
                Utils.Log.Exception(e);
                return;
            }

            this.m_Amx.LoadLibrary(AMXDefaultLibrary.Core | AMXDefaultLibrary.Float | AMXDefaultLibrary.String | AMXDefaultLibrary.Console | AMXDefaultLibrary.DGram |AMXDefaultLibrary.Time);

            this.RegisterNatives();

            if (_isFilterscript)
            {
                this.m_isFs = _isFilterscript;
                Utils.Log.Debug("Loading filterscript as ID: " + Program.m_Scripts.Count, this);
                AMXPublic p = this.m_Amx.FindPublic("OnFilterscriptInit");
                if (p != null)
                {
                    var x = p.AMX.Push(Program.m_TelegramHandle.m_TelegramBotClient.BotId.ToString());

                    p.Execute();
                    p.AMX.Release(x);

                }
            }
            else
            {
                try
                {
                    Utils.Log.Debug("Loading script as main script, ID: " + Program.m_Scripts.Count, this);

                    this.m_Amx.ExecuteMain();

                    AMXPublic p = this.m_Amx.FindPublic("OnInit");
                    if (p != null)
                    {
                        var x = p.AMX.Push(Program.m_TelegramHandle.m_TelegramBotClient.BotId.ToString());

                        p.AMX.Release(x);
                        p.Execute();
                    }
                }
                catch(Exception ex)
                {
                    Utils.Log.Exception(ex);
                }
            }

            Program.m_Scripts.Add(this);

            return;
        }

        public void StopAllTimers()
        {
            foreach (ScriptTimer timer in Program.m_ScriptTimers)
            {
                timer.KillTimer();
            }

        }

        public bool RegisterNatives()
        {
            m_Amx.Register("printc", (amx1, args1) => Natives.printc(amx1, args1, this));

            m_Amx.Register("Loadscript", (amx1, args1) => Natives.Loadscript(amx1, args1, this));
            m_Amx.Register("Unloadscript", (amx1, args1) => Natives.Unloadscript(amx1, args1, this));
            m_Amx.Register("SetTimer", (amx1, args1) => Natives.SetTimer(amx1, args1, this));
            m_Amx.Register("KillTimer", (amx1, args1) => Natives.KillTimer(amx1, args1, this));
            m_Amx.Register("gettimestamp", (amx1, args1) => Natives.gettimestamp(amx1, args1, this));
            m_Amx.Register("CallRemoteFunction", (amx1, args1) => Natives.CallRemoteFunction(amx1, args1, this));
            
            m_Amx.Register("GetChatType", (amx1, args1) => Natives.GetChatType(amx1, args1, this));
            m_Amx.Register("GetChatSlowModeDelay", (amx1, args1) => Natives.GetChatSlowModeDelay(amx1, args1, this));
            m_Amx.Register("GetChatDescription", (amx1, args1) => Natives.GetChatDescription(amx1, args1, this));
            m_Amx.Register("SendChatMessage", (amx1, args1) => Natives.SendChatMessage(amx1, args1, this));
            m_Amx.Register("DeleteChatMessage", (amx1, args1) => Natives.DeleteChatMessage(amx1, args1, this));
            m_Amx.Register("PinChatMessage", (amx1, args1) => Natives.PinChatMessage(amx1, args1, this));
            m_Amx.Register("UnpinChatMessage", (amx1, args1) => Natives.UnpinChatMessage(amx1, args1, this));
            m_Amx.Register("UnpinAllChatMessages", (amx1, args1) => Natives.UnpinAllChatMessages(amx1, args1, this));

            m_Amx.Register("GetUserName", (amx1, args1) => Natives.GetUserName(amx1, args1, this));

            m_Amx.Register("GetBotUserName", (amx1, args1) => Natives.GetBotUserName(amx1, args1, this));
            m_Amx.Register("GetBotName", (amx1, args1) => Natives.GetBotName(amx1, args1, this));
            m_Amx.Register("GetUserBio", (amx1, args1) => Natives.GetUserBio(amx1, args1, this));

            //Very ugly, but it works.
            foreach (Plugins.PluginNatives pln in Program.m_PluginNatives)
            {
                foreach (string nat in pln.m_Natives)
                {
                    m_Amx.Register(nat, (amx1, args1) =>
                    (int)pln.m_SourcePlugin.m_Plugin.GetExportedTypes()[0].InvokeMember(nat, BindingFlags.InvokeMethod, null, pln.m_SourcePlugin.m_Instance, new object[] { amx1, args1 })
                );
                }
                
            }
            return true;
        }
    }
}
