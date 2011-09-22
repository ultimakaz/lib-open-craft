using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace DynamicWebServer.special_html
{
    public class CssScript
    {
        public special_xml sxml;
        public type mime_Type;
        public string html_Type;
        public string type_Name;
        public css_variables[] variables;
        public string[] values;

        public CssScript(special_xml Sxml, type Mime_Type, string html_type, string type_name, css_variables[] Variables, string[] Values)
        {
            sxml = Sxml;
            mime_Type = Mime_Type;
            html_Type = html_type;
            type_Name = type_name;
            variables = Variables;
            values = Values;
        }
        public CssScript()
        {
            sxml = new special_xml();
            mime_Type = new type();
            html_Type = "";
            type_Name = "";
            variables = new css_variables[] { };
            values = new string[] { };
        }
    }
    public static class Make_Special
    {
        public static string MakeJavaScript(string functionName, object[] Enums, string[] Values)
        {
            Enum[] TEnum = (Enum[])(Enums);
            int i = 0;

            string tStrOpen = "<" + EnumUtil.GetDescription( special_xml.javascript ) + " type=" + EnumUtil.GetDescription(type.javascript) + ">";
            string tStrMiddle = "";
            tStrMiddle = "function " + functionName + "() {" + "\n";
            bool VarObj = false;
            int VarObjPos = 0;
            foreach (Enum TempE in TEnum)
            {
                if (EnumUtil.GetDescription(TempE).IndexOf('(') != -1 && VarObjPos != 1)
                {
                    VarObj = false;
                    VarObjPos = 0;
                    tStrMiddle += EnumUtil.GetDescription(TempE).Substring(0, EnumUtil.GetDescription(TempE).IndexOf('(')) + "(\""+Values[i]+"\");\n";
                }
                else if (EnumUtil.GetDescription(TempE).IndexOf('=') != -1 && VarObjPos != 1)
                {
                    VarObj = false;
                    VarObjPos = 0;
                    tStrMiddle += EnumUtil.GetDescription(TempE) + "\"" + Values[i] + "\";" + "\n";
                }

                else if (EnumUtil.GetDescription(TempE).IndexOf(' ') != -1 && VarObjPos != 1)
                {
                    tStrMiddle += EnumUtil.GetDescription(TempE) + Values[i] + " =" + " ";
                    VarObjPos++;
                }
                else if (VarObjPos == 1)
                {
                    tStrMiddle += EnumUtil.GetDescription(TempE) + ";" + "\n";
                    VarObjPos = 0;
                }
                
                if (i == (Enums.Count() - 1))
                {
                    tStrMiddle += "}";
                }
                i++;
            }

            string tStrEnd = "</" + EnumUtil.GetDescription(special_xml.javascript) + ">";
            return tStrOpen + "\n" + tStrMiddle + "\n" + tStrEnd + "\n";
        }
        public static string MakeCssScript(special_xml Sxml, type Mime_Type, string html_type, string type_name, css_variables[] Variables, string[] Values)
        {
            string tStrOpen = "<" + EnumUtil.GetDescription(Sxml) + " type=" + EnumUtil.GetDescription(Mime_Type) + ">";
            string tStrMiddle = "";
            if (html_type != "#")
            {
                tStrMiddle = "" + html_type + "." + type_name + " {"+"\n";
            }
            else
            {
                
                tStrMiddle = "" + html_type + type_name + " {"+"\n";
            }
            int i = 0;

            foreach (css_variables str in Variables)
            {
                tStrMiddle += EnumUtil.GetDescription(str) + ":" + Values[i] + ";" + "\n";
                if(i == (Variables.Count() - 1))
                {
                    tStrMiddle += "}";
                }
                i++;
            }

            string tStrEnd = "</" + EnumUtil.GetDescription(Sxml) + ">";
            return tStrOpen + "\n" + tStrMiddle + "\n" + tStrEnd + "\n";
        }
        public static string MakeCssScript(CssScript css)
        {

            string tStrOpen = "<" + EnumUtil.GetDescription(css.sxml) + " type=" + EnumUtil.GetDescription(css.mime_Type) + ">";
            string tStrMiddle = "";
            if (css.html_Type != "#")
            {
                tStrMiddle = "" + css.html_Type + "." + css.type_Name + " {" + "\n";
            }
            else
            {
                tStrMiddle = "" + css.html_Type + css.type_Name + " {" + "\n";
            }
            int i = 0;

            foreach (css_variables str in css.variables)
            {
                tStrMiddle += EnumUtil.GetDescription(str) + ":" + css.values[i] + ";" + "\n";
                if (i == (css.variables.Count() - 1))
                {
                    tStrMiddle += "}";
                }
                i++;
            }

            string tStrEnd = "</" + EnumUtil.GetDescription(css.sxml) + ">";
            return tStrOpen + "\n" + tStrMiddle + "\n" + tStrEnd + "\n";
        }
    }
    public static class javascript
    {

    }
    public class StringValueAttribute : System.Attribute
    {
        private static Dictionary<Enum, object> _Values = new Dictionary<Enum, object>();
        private string _value;

        public StringValueAttribute(string value)
        {
            //_Values.Add((Enum)(this), value);
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }
    }
    public static class EnumUtil
    {
        public static string GetDescription(Enum en)
        {

            Type type = en.GetType();

            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {

                object[] attrs = memInfo[0].GetCustomAttributes(typeof(StringValueAttribute),
                                                                false);

                if (attrs != null && attrs.Length > 0)

                    return ((StringValueAttribute)attrs[0]).Value;

            }

            return en.ToString();

        }
    }
    public enum special_xml : int
    {
        [StringValueAttribute("style")]
        css = 0,
        [StringValueAttribute("script")]
        javascript = 1,
    };
    public enum css_variables
    {
        [StringValueAttribute("padding-bottom")]
        padding_bottom,
        [StringValueAttribute("padding-top")]
        padding_top,
        [StringValueAttribute("padding-right")]
        padding_right,
        [StringValueAttribute("padding-left")]
        padding_left,
        [StringValueAttribute("margin-left")]
        margin_left,
        [StringValueAttribute("margin-right")]
        margin_right,
        [StringValueAttribute("margin-top")]
        margin_top,
        [StringValueAttribute("margin-bottom")]
        margin_bottom,
        [StringValueAttribute("border")]
        border,
        [StringValueAttribute("height")]
        height,
        [StringValueAttribute("width")]
        width,
        [StringValueAttribute("left")]
        left,
        [StringValueAttribute("top")]
        top,
        [StringValueAttribute("bottom")]
        bottom,
        [StringValueAttribute("right")]
        right,
        [StringValueAttribute("float")]
        Float,
        [StringValueAttribute("text-align")]
        text_align,
        [StringValueAttribute("position")]
        position,
    };
    public enum type
    {
        [StringValueAttribute("text/css")]
        css = 0,
        [StringValueAttribute("text/javascript")]
        javascript = 1,
    }
    public enum jsDocument
    {
    }
    public enum ajaxCalls
    {
        [StringValueAttribute("XMLHttpRequest()")]
        XMLHttpRequest,
    }
    public enum jsWindow
    {
        [StringValueAttribute("window.location.href=")]
        window_location_href,
        [StringValueAttribute("window.location.reload()")]
        window_location_reload,
        [StringValueAttribute("window.location.reload()")]
        window_location_replace,
    };
    public enum CommonJs
    {
        [StringValueAttribute("location.href=")]
        location_href,
        [StringValueAttribute("var ")]
        var,
    }
}
