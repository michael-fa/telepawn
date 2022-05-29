using AMXWrapper;
using System;
using System.Collections.Generic;
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
                m_Amx = new AMX("Scripts/" + _amxFile + ".amx");
            }
            catch (Exception e)
            {
                Utils.Log.Exception(e);
                return;
            }

            m_Amx.LoadLibrary(AMXDefaultLibrary.Core);

            m_Amx.LoadLibrary(AMXDefaultLibrary.String);
            m_Amx.LoadLibrary(AMXDefaultLibrary.Console);
            m_Amx.LoadLibrary(AMXDefaultLibrary.DGram);
            m_Amx.LoadLibrary(AMXDefaultLibrary.Float);
            m_Amx.LoadLibrary(AMXDefaultLibrary.Time);
            m_Amx.LoadLibrary(AMXDefaultLibrary.Fixed);
            this.RegisterNatives();

            Program.m_Scripts.Add(this);

            if (_isFilterscript)
            {
                this.m_isFs = _isFilterscript;
                Utils.Log.Debug("Loading filterscript as ID: " + (Program.m_Scripts.Count - 1), this);
                AMXPublic p = null;
                p = m_Amx.FindPublic("OnFilterscriptInit");
                if (p != null)
                {
                    p.Execute();

                }
                
            }
            else
            {
                Utils.Log.Debug("Loading script as main script, ID 0.", this);
                AMXWrapper.AMXPublic pub = Program.m_Scripts[0].m_Amx.FindPublic("OnInit");
                if (pub != null) pub.Execute();
            }
            
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
            m_Amx.Register("strequals", (amx1, args1) => Natives.strequals(amx1, args1, this));

            m_Amx.Register("SendChatMessage", (amx1, args1) => Natives.SendChatMessage(amx1, args1, this));
            m_Amx.Register("GetBotUserName", (amx1, args1) => Natives.GetBotUserName(amx1, args1, this));
            m_Amx.Register("GetBotFirstLastName", (amx1, args1) => Natives.GetBotUserName(amx1, args1, this));
            return true;
        }
    }
}
