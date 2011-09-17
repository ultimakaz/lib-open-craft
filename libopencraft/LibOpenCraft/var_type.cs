using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gee_MUD.GSL.NewCore
{
    public enum var_type
    {
        Bool = 0x0001,
        Integer = 0x0010,
        Double = 0x0100,
        Float = 0x1000,
        String = 0x0011,
        Vector = 0x0101,
        UUID = 0x0111,
    }
    public static class var_char
    {
        public static var_type GetType(string var)
        {
            int count = 0;
            bool p = var.Contains('.');
            bool f = var.Contains('f');
            if (p && f) return var_type.Float;
            else if (p) return var_type.Double;
            else if (var.Length >= 32 && var.Length <= 37 && var.LastIndexOf('-') == 21 ) return var_type.UUID;
            else if (var.Contains("true") || var.Contains("false")) return var_type.Bool;
            else if (var.Contains('<') && var.Contains(',')) return var_type.Vector;
            else
            {
                foreach (char chr in var)
                {
                    if (char.IsLetter(chr)) break;
                    else
                        count++;
                }
                if (count >= var.Length) return var_type.Integer;
                else return var_type.String;
            }
        }
    }
}
