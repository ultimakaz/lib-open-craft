using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicWebServer;
using DynamicWebServer.FormToHtml;
using DynamicWebServer.special_html;

using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Reflection;

namespace LibOpenCraft
{
    public class Program
    {
        public static GridServer g;
        public static AdminPanel panel;
        public static DynamicWebServer.SimpleWebServer webserver;
        public static FormUtility f_utility;
        public static Assembly[] this_functions = new Assembly[1];
        public static void Main(string[] args)
        {
            g = new GridServer();
            panel = new AdminPanel();
            f_utility = new FormUtility(panel.Controls);
            
            webserver = new DynamicWebServer.SimpleWebServer(8080, ref f_utility.HtmlControls);
            webserver.OnCommand += new SimpleWebServer.GotCommand(webserver_OnCommand);
            panel.OnRestart += new NewEvent(panel_OnRestart);
            /*List<Assembly> temp_a = new List<Assembly>();
            int i = 0;
            for (; i < f_utility.HtmlControls.Values.Count; i++)
            {
                temp_a.Add(Assembly.GetAssembly(f_utility.HtmlControls.Values.ElementAt(i).GetType()));
            }
            this_functions = temp_a.ToArray();*/
        }

        static byte[] webserver_OnCommand(string[] Commands, string[] Variables)
        {
            if (Commands == null && Variables == null)
            {
                return new byte[1] { 0x00 };
            }
            else
            {

            }
            return new byte[1] { 0x01 };
        }

        static void panel_OnRestart(string type)
        {
            ModuleHandler._CoreModules["ClientListener"].Stop();
            ModuleHandler._CoreModules = null;
            ModuleHandler._CoreEventModules = null;
            GridServer.player_list = null;
            World.chunks = null;
            World.chunk_b = null;
            g = new GridServer();
        }
    }
}
