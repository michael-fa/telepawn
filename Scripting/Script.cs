using AMXWrapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace telepawn.Scripting
{
    public class Script
    {
        public string _amxFile = null;
        public AMX amx;


        public Script(string _amxFile, bool _isFilterscript = false)
        {
            this._amxFile = _amxFile;
            try
            {
                amx = new AMX("Scripts/" + _amxFile + ".amx");
            }
            catch (Exception e)
            {
                Utils.Log.Exception(e);
                return;
            }

            amx.LoadLibrary(AMXDefaultLibrary.Core);

            amx.LoadLibrary(AMXDefaultLibrary.String);
            amx.LoadLibrary(AMXDefaultLibrary.Console);
            amx.LoadLibrary(AMXDefaultLibrary.DGram);
            amx.LoadLibrary(AMXDefaultLibrary.Float);
            amx.LoadLibrary(AMXDefaultLibrary.Time);
            amx.LoadLibrary(AMXDefaultLibrary.Fixed);
            this.RegisterNatives();

            Program.m_Scripts.Add(this);

            if (_isFilterscript)
            {
                Utils.Log.Debug("Loading filterscript as ID: " + (Program.m_Scripts.Count - 1), this);
                AMXPublic p = null;
                p = amx.FindPublic("OnFilterscriptInit");
                if (p != null)
                {
                    p.Execute();

                }
                
            }
            else
            {
                Utils.Log.Debug("Loading script as main script, ID 0.", this);
                AMXWrapper.AMXPublic pub = Program.m_Scripts[0].amx.FindPublic("OnInit");
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
            amx.Register("printc", (amx1, args1) => Natives.printc(amx1, args1, this));

            amx.Register("Loadscript", (amx1, args1) => Natives.Loadscript(amx1, args1, this));
            amx.Register("Unloadscript", (amx1, args1) => Natives.Unloadscript(amx1, args1, this));
            amx.Register("SetTimer", (amx1, args1) => Natives.SetTimer(amx1, args1, this));
            amx.Register("KillTimer", (amx1, args1) => Natives.KillTimer(amx1, args1, this));
            amx.Register("gettimestamp", (amx1, args1) => Natives.gettimestamp(amx1, args1, this));
            amx.Register("strequals", (amx1, args1) => Natives.strequals(amx1, args1, this));

            amx.Register("SendChatMessage", (amx1, args1) => Natives.SendChatMessage(amx1, args1, this));
            return true;
        }
    }
}
