using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using DynamicWebServer.special_html;

namespace DynamicWebServer.FormToHtml
{
    public class HTMLLabel : HtmlControl
    {
        public delegate byte[] ResponseRequested(string Variable, string Value);
        public event ResponseRequested OnResponseRequested;
        public CssScript cssScript = new CssScript();
        string HTMLCode = "";
        public HTMLLabel(Label control)
            : base(control)
        {
            this.Thiscontrol = control;
            cssScript.html_Type = "div";
            cssScript.type_Name = Thiscontrol.Name;
            cssScript.sxml = special_xml.css;
            cssScript.mime_Type = type.css;
            cssScript.values = new string[] { (Thiscontrol.Bounds.X + (Thiscontrol.Bounds.X / 2) + 10).ToString() + "px", (Thiscontrol.Bounds.Y + (Thiscontrol.Bounds.Y / 2) - 28).ToString() + "px", "absolute" };
            cssScript.variables = new css_variables[] { css_variables.left, css_variables.top, css_variables.position};
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
            string cssSRipt = Make_Special.MakeCssScript(cssScript);
            string TempStr = CurrentPage;
            if (TempStr.IndexOf("<body>") != -1)
            {
                int test = TempStr.IndexOf("<body>");
                if (this.Thiscontrol.Text.IndexOf('*') == -1)
                {

                    string BeforeBody = TempStr.Substring(0, (TempStr.IndexOf("<body>") - 1));
                    string AfterBodyCode = "<div class=" + this.Thiscontrol.Name + "><p class=" + this.Thiscontrol.Name + ">" + this.Thiscontrol.Text + "</p></div>";// +"<body>";

                    string HtmlAfterCode = TempStr.Substring((TempStr.IndexOf("<body>") + 7));

                    TempStr = cssSRipt + BeforeBody + AfterBodyCode + HtmlAfterCode;
                }
            }
            return TempStr;
        }
    }
}
