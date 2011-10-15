using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using DynamicWebServer;
//using DynamicWebServer.FormToHtml;
//using DynamicWebServer.special_html;

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
        //public static AdminPanel panel;
        //public static DynamicWebServer.SimpleWebServer webserver;
        //public static FormUtility f_utility;
        public static Assembly[] this_functions = new Assembly[1];
        public static void Main(string[] args)
        {
            g = new GridServer();
            while(Console.Read() != '\n')
            {

            }
        }
    }
}
