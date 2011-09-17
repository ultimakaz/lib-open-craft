using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LibOpenCraft
{
    public class Config
    {
        public static Dictionary<string, object> Configuration = new Dictionary<string, object>();
        public static void InitializeSettings()
        {
            Configuration.Clear();
            StreamReader _reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "config.txt");
            Console.Write("Reading the configuration....");
            int i = 0;
            while (!(_reader.EndOfStream))
            {
                Console.WriteLine("...." + ReapeatChar('.', i));
                string temp_r = _reader.ReadLine();
                if (temp_r.Contains("="))
                {
                    string[] temp_vars = temp_r.Split(new char[1] { '=' }, 2);
                    Configuration.Add(temp_vars[0], ReturnType(temp_vars[1]));
                }
                else
                {
                    Console.WriteLine("Error in the configuration file \"config.txt\" line " + i + " : " + temp_r);
                }
                i++;
            }
            Console.WriteLine("Done reading the configuration");
        }
        public static string ReapeatChar(char c, int amount)
        {
            string temp = "";
            int i = 0;
            while (i++ <= amount)
            {
                temp += c;
            }
            return temp;
        }
        public static object ReturnType(string var)
        {
            int count = 0;
            if (var.Substring(0, var.Length).ToLower() == "true" || var.Substring(0, var.Length).ToLower() == "false") return (object)bool.Parse(var);
            else
            {
                foreach (char chr in var)
                {
                    if (char.IsLetter(chr) || chr == '+' || chr == '-') break;
                    else
                        count++;
                }
                if (count >= var.Length)
                {
                    
                    bool p = var.Contains('.');
                    bool f = var.Contains('f');
                    if (p && f) 
                        return (object)float.Parse(var);
                    else if (p) 
                        return (object)double.Parse(var);
                    else
                        return (object)int.Parse(var);
                }
                else return (object)var;
            }
        }
    }
}
