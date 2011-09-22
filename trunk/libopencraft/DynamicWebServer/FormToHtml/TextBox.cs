using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using DynamicWebServer.special_html;

namespace DynamicWebServer.FormToHtml
{
    public class HTMLTextBox : HtmlControl
    {
        public delegate byte[] ResponseRequested(string Variable, string Value);
        public event ResponseRequested OnResponseRequested;
        public CssScript cssScript = new CssScript();
        string HTMLCode = "";
        public HTMLTextBox(TextBox control)
            : base(control)
        {
            this.Thiscontrol = control;
            cssScript.html_Type = "div";
            cssScript.type_Name = Thiscontrol.Name;
            cssScript.sxml = special_xml.css;
            cssScript.mime_Type = type.css;
            cssScript.values = new string[] { (Thiscontrol.Bounds.X + (Thiscontrol.Bounds.X / 2)).ToString(), (Thiscontrol.Bounds.Y + (Thiscontrol.Bounds.Y / 2)).ToString(), "absolute", Thiscontrol.ClientRectangle.Height.ToString(), Thiscontrol.ClientRectangle.Width.ToString() };
            cssScript.variables = new css_variables[] { css_variables.left, css_variables.top, css_variables.position, css_variables.height, css_variables.width };
            //this.Thiscontrol.ClientRectangle.X
            OnItemResponse += new FormItemResponse(HTMLTextBox_OnItemResponse);
        }

        byte[] HTMLTextBox_OnItemResponse(string Variable, string Value)
        {
           return OnResponseRequested(Variable, Value);
        }
        
        public override string PrintAllHTML()
        {
            return base.PrintAllHTML();
        }
        public string GetPageCode(string CurrentPage)
        {
            string cssSRipt = special_html.Make_Special.MakeCssScript(cssScript);
            string TempStr = CurrentPage;
            if (TempStr.IndexOf("<body>") != -1)
            {
                int test = TempStr.IndexOf("<body>");

                string BeforeBody = TempStr.Substring(0, (TempStr.IndexOf("<body>") - 1));
                string AfterBodyCode = "<div class=" + Thiscontrol.Name + "><input class=" + Thiscontrol.Name + " type=" + FormTypes.textbox + " value=\"" + Thiscontrol.Text + "\" /></div>" + "<body>";

                string HtmlAfterCode = TempStr.Substring((TempStr.IndexOf("<body>") + 7));

                TempStr = cssSRipt + BeforeBody + AfterBodyCode + HtmlAfterCode;
            }
            return TempStr;
        }
    }
}
