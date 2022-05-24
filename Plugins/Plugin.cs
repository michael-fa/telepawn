using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace telepawn.Plugins
{
    public class Plugin
    {
        public string _File;
        Assembly m_Plugin = null;
        object m_Instance = null;
        public Plugin(string _filename)
        {
            _File = _filename;
            try
            {
                m_Plugin = Assembly.LoadFile(Environment.CurrentDirectory + "\\" + _filename);
                m_Instance = Activator.CreateInstance(m_Plugin.GetExportedTypes()[0]);
                Program.m_Plugins.Add(this);
            }
            catch (Exception ex)
            {
                Utils.Log.Error("=========================\nPlugin " + _filename + " threw an error:\n" + ex.Message + ex.StackTrace + "\n        =========================\n");
            }

            
        }

        public void Unload(int exitcode)
        {
            m_Plugin.GetExportedTypes()[0].InvokeMember("Unload", BindingFlags.InvokeMethod, null, m_Instance, new object[] { exitcode });

           
           
            m_Instance = null;
            m_Plugin = null;
            GC.Collect();
          
        }
    }
}
