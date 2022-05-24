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
        public Plugin(string _filename)
        {
            try
            {
                var DLL = Assembly.LoadFile(Environment.CurrentDirectory + "\\" + _filename);
                foreach (Type type in DLL.GetExportedTypes())
                {
                    var c = Activator.CreateInstance(type);
                    type.InvokeMember("Output", BindingFlags.InvokeMethod, null, c, new object[] { @"Hello" });
                }
            }
            catch(Exception ex)
            {
                Utils.Log.Error("Plugin " + _filename + " could not be loaded:\n" + ex.Message);
            }

           
            

        }
    }
}
