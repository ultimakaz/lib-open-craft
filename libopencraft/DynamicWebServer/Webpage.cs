using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace DynamicWebServer
{
    public static class Webpage
    {
        public static byte[] AddMenuToSite(Menu PBE, string HtmlCode)
        {
            return new byte[] { };
        }
        public static byte[] AddImageToSite(HttpPictureBox PBE, string HtmlCode)
        {
            return PBE.DrawImage(ASCIIEncoding.ASCII.GetBytes(HtmlCode));
        }
        public static byte[] AddButtonToSite(System.Windows.Forms.Button PBE, string HtmlCode)
        {
            return ASCIIEncoding.ASCII.GetBytes(new FormToHtml.HTMLButton(PBE).GetPageCode(HtmlCode));
        }
    }
}
