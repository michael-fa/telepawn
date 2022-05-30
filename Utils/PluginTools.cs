using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telepawn.Utils
{
    public static class PluginTools
    {
        public static int LoadAllPlugins()
        {
            foreach(Plugins.Plugin pl in Program.m_Plugins)
            {
                pl.InvokeLoad();
            }
            return Program.m_Plugins.Count;
        }
    }
}
