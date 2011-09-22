using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace DynamicWebServer
{
    public class SimpleWebServer
    {
        public delegate byte[] GotCommand(string[] Commands, string[] Variables);
        public event GotCommand OnCommand;
        List<string> Variables = new List<string>();
        List<string> Values = new List<string>();
        Dictionary<string, FormToHtml.HtmlControl> HtmlFormItems = new Dictionary<string, FormToHtml.HtmlControl>();
        private TcpListener myListener;
        private int port = 8080;  // Select any free port you wish 
        public void EndListener()
        {
            myListener.Stop();
        }
        public SimpleWebServer(int porT,ref Dictionary<string, FormToHtml.HtmlControl> HtmlItems)
        {
            HtmlFormItems = HtmlItems;
            System.IO.StreamWriter SW;
            System.IO.StreamWriter SWW;
            port = porT;
            string DFolder = AppDomain.CurrentDomain.BaseDirectory + "data\\";
            string DFolderRoot = AppDomain.CurrentDomain.BaseDirectory + "www\\";
            if (!Directory.Exists("www")) { Directory.CreateDirectory("www"); }
            if (!Directory.Exists("data")) { Directory.CreateDirectory("data"); }

            
            string DefaultSite = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\"><html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=iso-8859-1\"><meta name=\"GENERATOR\" content=\"Arachnophilia 3.9\"><meta name=\"description\" content=\"Comprehensive Documentation and information about HTML.\"><meta name=\"keywords\" content=\"HTML, tags, commands\"><title>ShaftCraft Manager</title><link href=\"style.css\" rel=\"stylesheet\" type=\"text/css\"></head><body>Test Test this is MCSharp <a href=\"index.html?ChangeRank=none\">Change Rank</a><br /><a href=\"index.html?GetButton\">Dynamicly Made Website.</a></body>";

            if (!File.Exists(DFolderRoot + "index.html"))
            {
                //File.Open(DFolderRoot + "index.html", FileMode.OpenOrCreate);
                SW = new StreamWriter(DFolderRoot + "index.html");
                Thread.Sleep(100);
                SW.Write(DefaultSite);
                SW.Flush();
                SW.Close();
                SW.Dispose();
                SW = null;
            }
            if (!File.Exists(DFolder + "Default.dat"))
            {
                SWW = new StreamWriter(DFolder + "Default.dat");
                Thread.Sleep(100);
                SWW.Write(StringArrayToString(new string[] { "default.html", "default.htm", "index.html", "index.htm;" }));
                Thread.Sleep(100);
                SWW.Flush();
                SWW.Close();
                SWW.Dispose();
                SW = null;
            }
            if (!File.Exists(DFolder + "Mime.dat"))
            {
                SWW = new StreamWriter(DFolder + "Mime.dat");
                Thread.Sleep(100);
                SWW.Write(StringArrayToString( new string[] 
                { ".html; text/html", ".htm; text/html ", ".gif; image/gif", ".bmp; image/bmp", ".png; image/png", ".jpg; image/jpg"
                
                }));
                SWW.Flush();
                SWW.Close();
                SWW.Dispose();
                SWW = null;
            }
            if (!File.Exists(DFolder + "VDirs.dat"))
            {
                SWW = new StreamWriter(DFolder + "VDirs.dat");
                Thread.Sleep(100);
                
                SWW.Write(StringArrayToString( new string[] 
                { "/; "+DFolderRoot//, "//test//; "+DFolderRoot
                
                }));
                SWW.Flush();
                SWW.Close();
                SWW.Dispose();
                SWW = null;
            }
            try
            {
                IPAddress[] TempIPs = Dns.GetHostEntry(System.Environment.MachineName).AddressList;
                int Ti = 0;
                IPAddress TheIP = new IPAddress(TempIPs[0].GetAddressBytes());
                foreach (IPAddress IP in TempIPs)
                {
                    if (!TempIPs[Ti].AddressFamily.Equals("127.0.0.1") && !TempIPs[Ti].IsIPv6LinkLocal && !TempIPs[Ti].IsIPv6Multicast && !TempIPs[Ti].IsIPv6SiteLocal)
                    {
                        TheIP = TempIPs[Ti];
                    }
                    Ti++;
                }
                myListener = new TcpListener(IPAddress.Any ,port);
                myListener.Start();
                Thread th = new Thread(new ThreadStart(StartListen));
                th.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("An Exception Occurred while Listening :"
                                   + e.ToString());
            }

        }
        public string StringArrayToString(string[] str)
        {
            string temp = "";
            foreach (string TStr in str)
            {
                temp += TStr + "\n";
            }
            return temp;
        }
        public void StartListen()
        {
            string DFolderRoot = AppDomain.CurrentDomain.BaseDirectory + "www\\";
            int iStartPos = 0;
            String sRequest;
            String sDirName;
            String sRequestedFile;
            String sErrorMessage;
            String sLocalDir;
            String sMyWebServerRoot = DFolderRoot;
            String sPhysicalFilePath = "";
            String sFormattedMessage = "";
            String sResponse = "";
            while (true)
            {
                //Accept a new connection
                TcpClient mySocket = myListener.AcceptTcpClient();
                StreamReader WBSReader = new StreamReader(mySocket.GetStream());
                
                Console.WriteLine("Socket Type " + mySocket.Client.SocketType);
                if (mySocket.Connected)
                {
                    try
                    {
                        Console.WriteLine("\nClient Connected!!\n==================\n CLient IP {0}\n", mySocket.Client.RemoteEndPoint);


                        //make a byte array and receive data from the client 
                        Byte[] bReceive = new Byte[1024];
                        string temstr = ASCIIEncoding.ASCII.GetString(bReceive);
                        //int i = WBSReader.Read(temstr.ToCharArray() ,0,  bReceive.Length);
                        int i = mySocket.GetStream().Read(bReceive, 0, bReceive.Length);
                        //string sBuffer = WBSReader.ReadLine();

                        //Convert Byte to String
                        string sBuffer = Encoding.ASCII.GetString(bReceive);

                        Thread.Sleep(1);
                        //At present we will only deal with GET type
                        if (sBuffer.Substring(0, 3) != "GET")
                        {
                            Console.WriteLine("Only Get Method is supported..");
                            //mySocket.Close();
                            return;
                        }


                        // Look for HTTP request
                        iStartPos = sBuffer.IndexOf("HTTP", 1);


                        // Get the HTTP text and version e.g. it will return "HTTP/1.1"
                        string sHttpVersion = sBuffer.Substring(iStartPos, 8);


                        // Extract the Requested Type and Requested file/directory
                        sRequest = sBuffer.Substring(0, iStartPos - 1);


                        //Replace backslash with Forward Slash, if Any
                        sRequest.Replace("\\", "/");

                        Thread.Sleep(1);
                        //If file name is not supplied add forward slash to indicate 
                        //that it is a directory and then we will look for the 
                        //default file name..
                        //sRequest.IndexOf(
                        if ((sRequest.IndexOf(".") < 1) && (!sRequest.EndsWith("/")))
                        {
                            sRequest = sRequest + "/";
                        }
                        //Extract the requested file name
                        iStartPos = sRequest.LastIndexOf("/") + 1;
                        sRequestedFile = sRequest.Substring(iStartPos);


                        //Extract The directory Name
                        sDirName = sRequest.Substring(sRequest.IndexOf("/"),
                                   sRequest.LastIndexOf("/") - 3);
                        //The code is self-explanatory. It receives the request, converts it into string from bytes then looks for the request type, extracts the HTTP Version, file and directory information.

                        //sDirName = GetLocalPath(DFolderRoot, sDirName);
                        /////////////////////////////////////////////////////////////////////

                        // Identify the Physical Directory

                        /////////////////////////////////////////////////////////////////////
                        Thread.Sleep(1);
                        if (sDirName == "/")
                            sLocalDir = sMyWebServerRoot;
                        else
                        {
                            //Get the Virtual Directory

                            sLocalDir = GetLocalPath(sMyWebServerRoot, sDirName);
                        }


                        Console.WriteLine("Directory Requested : " + sLocalDir);

                        //If the physical directory does not exists then

                        // dispaly the error message
                        Thread.Sleep(1);
                        if (sLocalDir.Length == 0)
                        {
                            sErrorMessage = "<H2>Error!! Requested Directory does not exists</H2><Br>";
                            sErrorMessage = sErrorMessage + "Please check data\\Vdirs.dat";


                            //Format The Message

                            SendHeader(sHttpVersion, "", sErrorMessage.Length, " 404 Not Found", ref mySocket);

                            //Send to the browser

                            SendToBrowser(sErrorMessage, ref mySocket);

                            //mySocket.Close();

                            continue;
                        }
                        //Note: Microsoft Internet Explorer usually displays a 'friendly' HTTP Error Page if you want to display our error message then you need to disable the 'Show friendly HTTP error messages' option under the 'Advanced' tab in Tools->Internet Options. Next we look if the directory name is supplied, we call GetLocalPath function to get the physical directory information, if the directory not found (or does not mapped with entry in Vdir.dat) error message is sent to the browser.. Next we will identify the file name, if the filename is not supplied by the user we will call the GetTheDefaultFileName function to retrieve the filename, if error occurred it is thrown to browser.

                        /////////////////////////////////////////////////////////////////////

                        // Identify the File Name

                        /////////////////////////////////////////////////////////////////////


                        //If The file name is not supplied then look in the default file list
                        Thread.Sleep(1);
                        if (sRequestedFile.Length == 0)
                        {
                            // Get the default filename

                            sRequestedFile = GetTheDefaultFileName(sLocalDir);

                            if (sRequestedFile == "")
                            {
                                sErrorMessage = "<H2>Error!! No Default File Name Specified</H2>";
                                SendHeader(sHttpVersion, "", sErrorMessage.Length, " 404 Not Found", ref mySocket);
                                SendToBrowser(sErrorMessage, ref mySocket);

                                //mySocket.Close();

                                return;

                            }
                        }

                        //////////////////////////////////////////////////

                        // Get TheMime Type

                        //////////////////////////////////////////////////


                        String sMimeType = GetMimeType(sRequestedFile);


                        //Build the physical path

                        sPhysicalFilePath = sLocalDir + sRequestedFile;
                        Console.WriteLine("File Requested : " + sPhysicalFilePath);
                        Thread.Sleep(1);
                        //Now the final steps of opening the requested file and sending it to the browser.

                        if (sPhysicalFilePath.IndexOf('?') != -1)
                        {
                            string[] FirstSpl = sPhysicalFilePath.Split('?')[1].Split('&');


                            Values.Clear();
                            Variables.Clear();
                            int ii = 0;
                            byte[] bytes = new byte[] { };
                            foreach (string tempStr in FirstSpl)
                            {
                                string[] TempValVars = tempStr.Split('=');
                                Variables.Add(TempValVars[0]);
                                try
                                {
                                    if (HtmlFormItems.ContainsKey(TempValVars[0]))
                                    {
                                        bytes = HtmlFormItems[TempValVars[0]].Response(TempValVars[0], TempValVars[ii]);
                                    }
                                    Values.Add(TempValVars[ii]);
                                }
                                catch { }
                                ii++;
                                #region Old Dynamic Code
                                //OnCommand(Variables.ToArray(), Values.ToArray());
                                //Window.TempPerm_OnClosedOne(new string[] { TempValVars[1], SelectedName });

                                /*if (TempValVars[0] == "ChangeRank")
                                {
                                    if (TempValVars[1] == "none")
                                    {
                                        ProcTwo = true;
                                    }
                                    else if (TempValVars[1] == "Admin")
                                    {
                                        //OnCommand(Variables.ToArray(), Values.ToArray
                                        Window.TempPerm_OnClosedOne(new string[] { TempValVars[1], SelectedName });
                                        byte[] bytes = ASCIIEncoding.ASCII.GetBytes("<body>Rank set, click here to go to the home page.<a href=index.html" + ">Admin</a><br /></body>");
                                        sResponse = Encoding.ASCII.GetString(bytes, 0, bytes.Length); //bytes.ToString();
                                        int iTotBytes = bytes.Length;
                                        SendHeader(sHttpVersion, sMimeType, iTotBytes, " 200 OK", ref mySocket);
                                        SendToBrowser(bytes, ref mySocket);
                                        mySocket.GetStream().Write(bytes, 0, iTotBytes);
                                    }
                                    else if (TempValVars[1] == "Builder")
                                    {
                                        Window.TempPerm_OnClosedOne(new string[] { TempValVars[1], SelectedName });
                                        byte[] bytes = ASCIIEncoding.ASCII.GetBytes("<body>Rank set, click here to go to the home page.<a href=index.html" + ">Home</a><br /></body>");
                                        sResponse = Encoding.ASCII.GetString(bytes, 0, bytes.Length); //bytes.ToString();
                                        int iTotBytes = bytes.Length;
                                        SendHeader(sHttpVersion, sMimeType, iTotBytes, " 200 OK", ref mySocket);
                                        SendToBrowser(bytes, ref mySocket);
                                        mySocket.GetStream().Write(bytes, 0, iTotBytes);
                                    }
                                    else if (TempValVars[1] == "Guest")
                                    {
                                        Window.TempPerm_OnClosedOne(new string[] { TempValVars[1], SelectedName });
                                        byte[] bytes = ASCIIEncoding.ASCII.GetBytes("<body>Rank set, click here to go to the home page.<a href=index.html" + ">Admin</a><br /></body>");
                                        sResponse = Encoding.ASCII.GetString(bytes, 0, bytes.Length); //bytes.ToString();
                                        int iTotBytes = bytes.Length;
                                        SendHeader(sHttpVersion, sMimeType, iTotBytes, " 200 OK", ref mySocket);
                                        SendToBrowser(bytes, ref mySocket);
                                        mySocket.GetStream().Write(bytes, 0, iTotBytes);
                                    }
                                }
                                if (TempValVars[0] == "Name")
                                {
                                    ProcThree = true;
                                    SelectedName = TempValVars[1];
                                }
                            }
                            //if(
                            if (ProcTwo == true)
                            {
                                byte[] bytes = ASCIIEncoding.ASCII.GetBytes("<body>Please Select a name " + tempNames);
                                sResponse = Encoding.ASCII.GetString(bytes, 0, bytes.Length); //bytes.ToString();
                                int iTotBytes = bytes.Length;
                                SendHeader(sHttpVersion, sMimeType, iTotBytes, " 200 OK", ref mySocket);
                                SendToBrowser(bytes, ref mySocket);
                                mySocket.GetStream().Write(bytes, 0, iTotBytes);
                            }
                            else if (ProcThree == true)
                            {
                                byte[] bytes = ASCIIEncoding.ASCII.GetBytes("<body>Please Select a rank <a href=index.html?ChangeRank=Admin >Admin</a><br /> <a href=index.html?ChangeRank=Builder>Builder</a><br /> <a href=index.html?ChangeRank=Guest>Guest</a></body>");
                                sResponse = Encoding.ASCII.GetString(bytes, 0, bytes.Length); //bytes.ToString();
                                int iTotBytes = bytes.Length;
                                SendHeader(sHttpVersion, sMimeType, iTotBytes, " 200 OK", ref mySocket);
                                SendToBrowser(bytes, ref mySocket);
                                mySocket.GetStream().Write(bytes, 0, iTotBytes);
                            }*/
                                #endregion
                            }
                            byte[] Tbytes = OnCommand(Variables.ToArray(), Values.ToArray());
                            if (Tbytes == null || Tbytes.Length == 0 || Tbytes.Length == 1)
                            {

                            }
                            else
                            {
                                bytes = Tbytes;
                            }
                             //ASCIIEncoding.ASCII.GetBytes("<body>Rank set, click here to go to the home page.<a href=index.html" + ">Admin</a><br /></body>");
                            sResponse = Encoding.ASCII.GetString(bytes, 0, bytes.Length); //bytes.ToString();
                            int iTotBytes = bytes.Length;
                            SendHeader(sHttpVersion, sMimeType, iTotBytes, " 200 OK", ref mySocket);
                            SendToBrowser(bytes, ref mySocket);
                            mySocket.GetStream().Write(bytes, 0, iTotBytes);
                        }
                        else if (File.Exists(sPhysicalFilePath) == false)
                        {

                            sErrorMessage = "<H2>404 Error! File Does Not Exists...</H2>";
                            SendHeader(sHttpVersion, "", sErrorMessage.Length, " 404 Not Found", ref mySocket);
                            SendToBrowser(sErrorMessage, ref mySocket);

                            Console.WriteLine(sFormattedMessage);
                        }
                        else
                        {
                            int iTotBytes = 0;

                            sResponse = "";

                            //FileStream fs = new FileStream(sPhysicalFilePath,
                            //FileMode.Open, FileAccess.Read,
                            //FileShare.Read);
                            // Create a reader that can read bytes from the FileStream.



                            //BinaryReader reader = new BinaryReader(fs);
                            byte[] bytes = File.ReadAllBytes(sPhysicalFilePath);
                            // Read from the file and write the data to the network

                            sResponse = Encoding.ASCII.GetString(bytes, 0, bytes.Length); //bytes.ToString();

                            iTotBytes = bytes.Length;


                            SendHeader(sHttpVersion, sMimeType, iTotBytes, " 200 OK", ref mySocket);
                            SendToBrowser(bytes, ref mySocket);
                            mySocket.GetStream().Write(bytes, 0, iTotBytes);
                            //WBSWriter.WriteLine(bytes);
                            //WBSWriter.Flush();
                        }
                    }
                    catch { }
                }
                //catch {  }
                try
                {
                    if (mySocket.Connected == true)
                    {
                        mySocket.Close();
                        WBSReader.Close();
                    }
                }
                catch { }
            }
        }
        public string GetTheDefaultFileName(string sLocalDirectory)
        {
            StreamReader sr;
            String sLine = "";

            try
            {
                //Open the default.dat to find out the list

                // of default file

                sr = new StreamReader("data\\Default.dat");

                while ((sLine = sr.ReadLine()) != null)
                {
                    //Look for the default file in the web server root folder

                    if (File.Exists(sLocalDirectory + sLine) == true)
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An Exception Occurred : " + e.ToString());
            }
            if (File.Exists(sLocalDirectory + sLine) == true)
                return sLine;
            else
                return "";
        }
        public string GetLocalPath(string sMyWebServerRoot, string sDirName)
        {

            StreamReader sr;
            String sLine = "";
            String sVirtualDir = "";
            String sRealDir = "";
            int iStartPos = 0;


            //Remove extra spaces

            sDirName.Trim();



            // Convert to lowercase

            sMyWebServerRoot = sMyWebServerRoot.ToLower();

            // Convert to lowercase

            sDirName = sDirName.ToLower();


            try
            {
                //Open the Vdirs.dat to find out the list virtual directories

                sr = new StreamReader("data\\VDirs.dat");

                while ((sLine = sr.ReadLine()) != null)
                {
                    //Remove extra Spaces

                    sLine.Trim();

                    if (sLine.Length > 0)
                    {
                        //find the separator

                        iStartPos = sLine.IndexOf(";");

                        // Convert to lowercase

                        sLine = sLine.ToLower();

                        sVirtualDir = sLine.Substring(0, iStartPos);
                        sRealDir = sLine.Substring(iStartPos + 1);

                        if (sVirtualDir == sDirName)
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An Exception Occurred : " + e.ToString());
            }


            if (sVirtualDir == sDirName)
                return sRealDir;
            else
                return "";
        }
        public string GetMimeType(string sRequestedFile)
        {


            StreamReader sr;
            String sLine = "";
            String sMimeType = "";
            String sFileExt = "";
            String sMimeExt = "";

            // Convert to lowercase

            sRequestedFile = sRequestedFile.ToLower();

            int iStartPos = sRequestedFile.IndexOf(".");

            sFileExt = sRequestedFile.Substring(iStartPos);

            try
            {
                //Open the Vdirs.dat to find out the list virtual directories

                sr = new StreamReader("data\\Mime.dat");

                while ((sLine = sr.ReadLine()) != null)
                {

                    sLine.Trim();

                    if (sLine.Length > 0)
                    {
                        //find the separator

                        iStartPos = sLine.IndexOf(";");

                        // Convert to lower case

                        sLine = sLine.ToLower();

                        sMimeExt = sLine.Substring(0, iStartPos);
                        sMimeType = sLine.Substring(iStartPos + 1);

                        if (sMimeExt == sFileExt)
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An Exception Occurred : " + e.ToString());
            }

            if (sMimeExt == sFileExt)
                return sMimeType;
            else
                return "";
        }
        public void SendToBrowser(String sData, ref TcpClient mySocket)
        {
            SendToBrowser(Encoding.ASCII.GetBytes(sData), ref mySocket);
        }


        public void SendToBrowser(Byte[] bSendData, ref TcpClient mySocket)
        {
            //int numBytes = 0;
            try
            {
                mySocket.GetStream().Write(bSendData, 0, bSendData.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Occurred : {0} ", e);
            }
        }
        public void SendHeader(string sHttpVersion, string sMIMEHeader,
            int iTotBytes, string sStatusCode, ref TcpClient mySocket)
        {

            String sBuffer = "";

            // if Mime type is not provided set default to text/html

            if (sMIMEHeader.Length == 0)
            {
                sMIMEHeader = "text/html";  // Default Mime Type is text/html

            }

            sBuffer = sBuffer + sHttpVersion + sStatusCode + "\r\n";
            sBuffer = sBuffer + "Server: cx1193719-b\r\n";
            sBuffer = sBuffer + "Content-Type: " + sMIMEHeader + "\r\n";
            sBuffer = sBuffer + "Accept-Ranges: bytes\r\n";
            sBuffer = sBuffer + "Content-Length: " + iTotBytes + "\r\n\r\n";

            Byte[] bSendData = Encoding.ASCII.GetBytes(sBuffer);

            SendToBrowser(bSendData, ref mySocket);

            Console.WriteLine("Total Bytes : " + iTotBytes.ToString());

        }
    }
}
