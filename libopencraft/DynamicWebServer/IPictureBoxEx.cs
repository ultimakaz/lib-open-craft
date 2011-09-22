using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using DynamicWebServer.special_html;

namespace DynamicWebServer
{
    interface IPictureBoxEx
    {
        string ID
        {
            get;
            set;
        }
        string HoverText
        {
            get;
            set;
        }
        Bitmap source
        {
            get;
            set;
        }
        int Width
        {
            get;
            set;
        }
        int Height
        {
            get;
            set;
        }
        int PositionX
        {
            get;
            set;
        }
        int PositionY
        {
            get;
            set;
        }
        int BorderWidth
        {
            get;
            set;
        }
        string HttpLink
        {
            get;
            set;
        }
        bool LinkEnabled
        {
            get;
            set;
        }
        string href
        {
            get;
            set;
        }
    }
    public class PictureBoxEx : IPictureBoxEx
    {
        public PictureBoxEx()
        {
           
        }
        public string href
        {
            get;
            set;
        }
        public string ID
        {
            get;
            set;
        }
        public string HoverText
        {
            get;
            set;
        }
        public Bitmap source
        {
            get;
            set;
        }
        public int Width
        {
            get;
            set;
        }
        public int Height
        {
            get;
            set;
        }
        public int PositionX
        {
            get;
            set;
        }
        public int PositionY
        {
            get;
            set;
        }
        public int BorderWidth
        {
            get;
            set;
        }
        public string HttpLink
        {
            get;
            set;
        }
        public bool LinkEnabled
        {
            get;
            set;
        }
    }
    public class HttpPictureBox : PictureBoxEx
    {
        public CssScript cssScript = new CssScript();
        public HttpPictureBox(string id, string hovertext, int width, int height, int pX, int pY, int BDWidth, string httpLink, bool linkEnabled, string Href)
        {
            cssScript = new CssScript();
            ID = id;
            HoverText = hovertext;
            //source = bmap;
            href = Href;
            Width = width;
            Height = height;
            PositionX = pX;
            PositionY = pY;
            BorderWidth = BDWidth;
            HttpLink = httpLink;
            LinkEnabled = linkEnabled;
            cssScript.html_Type = "div";
            cssScript.type_Name = id;
            cssScript.sxml = special_xml.css;
            cssScript.mime_Type = type.css;
            cssScript.values = new string[] { height.ToString(), width.ToString(), PositionX.ToString(), PositionY.ToString(), "absolute" };
            cssScript.variables = new css_variables[] { css_variables.height, css_variables.width, css_variables.left, css_variables.top, css_variables.position };
        }
        public byte[] DrawImage(byte[] PageData)
        {
            string cssSRipt = Make_Special.MakeCssScript(cssScript);
            string TempStr = ASCIIEncoding.ASCII.GetString(PageData);
            if (TempStr.IndexOf("<body>") != -1)
            {
                int test = TempStr.IndexOf("<body>");
                if (LinkEnabled == false)
                {
                    string BeforeBody = TempStr.Substring(0, (TempStr.IndexOf("<body>") - 1));
                    string AfterBodyCode = "<div class=" + ID + "><img src=" + HttpLink + " alt=" + ID + " /></div>" + "<body>";

                    string HtmlAfterCode = TempStr.Substring((TempStr.IndexOf("<body>") + 6));

                    TempStr = cssSRipt + BeforeBody + AfterBodyCode + HtmlAfterCode;
                }
                else
                {
                    string BeforeBody = TempStr.Substring(0, TempStr.IndexOf("<body>") - 1);
                    string AfterBodyCode = "<div class=" + ID + "><a target=_blank href=" + href + "><img src=" + HttpLink + " alt=" + ID + " /></a></div>" + "<body>";
                    string HtmlAfterCode = TempStr.Substring(TempStr.IndexOf("<body>") + 5, -1);
                    TempStr = cssSRipt + BeforeBody + AfterBodyCode + HtmlAfterCode;
                    
                }
            }
            return ASCIIEncoding.ASCII.GetBytes(TempStr);
        }
    }
}
