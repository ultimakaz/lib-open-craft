using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Windows.Forms;

namespace DynamicWebServer.FormToHtml
{
    public abstract class HtmlControl
    {
        public delegate byte[] FormItemResponse(string Variable, string Value);
        public event FormItemResponse OnItemResponse;
        public Control Thiscontrol;
        public FormEnums FormType;
        public HtmlControl(Control control)
        {
            Thiscontrol = control;
            switch (control.GetType().FullName)
            {
                case "System.Windows.Forms.TextBox":
                    FormType = FormEnums.TextBox;
                    break;
                case "System.Windows.Forms.PictureBox":
                    FormType = FormEnums.PictureBox;
                    break;
                case "System.Windows.Forms.Label":
                    FormType = FormEnums.Label;
                    break;
                case "System.Windows.Forms.Button":
                    FormType = FormEnums.Button;
                    break;
            }
        }
        public virtual byte[] Response(string Variable, string Value)
        {
            return OnItemResponse(Variable, Value);
        }
        public virtual string GetPageCode(string CurrentPage)
        {
            return "";
        }
        public virtual string PrintAllHTML()
        {
            return "";
        }
    }
    public class FormUtility
    {
        public Dictionary<string, HtmlControl> HtmlControls = new Dictionary<string, HtmlControl>();
        public FormUtility(Control.ControlCollection Controls)
        {
            foreach (Control control in Controls)
            {
                switch (control.GetType().FullName)
                {
                    case "System.Windows.Forms.TextBox":
                        HtmlControls.Add(control.Name ,new HTMLTextBox((TextBox)control));
                        break;
                    case "System.Windows.Forms.PictureBox":
                        HtmlControls.Add(control.Name, new HTMLPictureBox((PictureBox)control));
                        break;
                    case "System.Windows.Forms.Label":
                        HtmlControls.Add(control.Name, new HTMLLabel((Label)control));
                        break;
                    case "System.Windows.Forms.Button":
                        HtmlControls.Add(control.Name, new HTMLButton((Button)control));
                        break;
                }
                if (control != null)
                {
                    //control.PrintAllHTML();
                }
            }
        }
        
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
        public static string GetEnum(string en)
        {
            

            return en.ToString();

        }
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
    public enum FormTypes
    {
        [StringValueAttribute("button")]
        button,
        [StringValueAttribute("text")]
        textbox
    }
    public enum HtmlTypes
    {
    }
    public enum FormEnums
    {
        [StringValueAttribute("System.Windows.Forms.TestBox")]
        TextBox,
        [StringValueAttribute("System.Windows.Forms.PictureBox")]
        PictureBox,
        [StringValueAttribute("System.Windows.Forms.Label")]
        Label,
        [StringValueAttribute("System.Windows.Forms.Button")]
        Button,
    }
}
