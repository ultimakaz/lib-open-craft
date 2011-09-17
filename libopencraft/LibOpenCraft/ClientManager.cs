using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace LibOpenCraft
{
    public delegate void RecievedPackets(Dictionary<string, Variable> data, TcpClient client);
    
    public class ClientManager
    {
        public PlayerClass _player = new PlayerClass();
        #region Misc Variables
        public Dictionary<string, object> customAttributes = new Dictionary<string, object>();
        public int id = -1;
        public DateTime keep_alive = DateTime.Now;
        public RecievedPackets OnRecieved;
        public int PreChunkRan = 0;
        #endregion Misc Variables
        #region Networking and Threading
        public NetworkStream _stream;
        protected TcpClient _client;
        private Thread recieved;
        private ParameterizedThreadStart recieve_start;
        private AsyncCallback recieve_async;
        #endregion Networking and Threading

        private ClientManager _recieveClient;

        public ClientManager(TcpClient client)
        {
            try
            {
                _client = client;
                _stream = client.GetStream();
                recieve_async = new AsyncCallback(AsyncResult_recieve);
                recieve_start = new ParameterizedThreadStart(Recieve);
                recieve_start.BeginInvoke(this, recieve_async, this);
                recieved = new Thread(recieve_start);
                if (recieved == null) recieved = new Thread(recieve_start);
                recieved.Start(this);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR:" + e.Message);
            }
        }
        public void Stop(bool ClearData)
        {
            if (ClearData == true)
            {
                if (recieved != null)
                    recieved.Abort();
                recieve_async = null;
                recieve_start = null;
                recieved = null;
                _stream = null;
                _client = null;
                id = -1;
            }
            else
            {
                recieved.Abort();
            }
        }
        public void Start(TcpClient client)
        {
            _client = client;
            _stream = client.GetStream();
            recieve_async = new AsyncCallback(AsyncResult_recieve);
            recieve_start = new ParameterizedThreadStart(Recieve);
            recieve_start.BeginInvoke(this, recieve_async, this);
            recieved = new Thread(recieve_start);
            recieved.Start(this);
        }
        public bool WaitToRead = false;
        public void Start()
        {
            recieved.Start(this);
        }
        private void AsyncResult_recieve(IAsyncResult IAR)
        {
        }
        public void SendPacket(PacketHandler p, int id, bool PingType = false, ClientManager _cm = null, bool Waitread = false)
        {
            try
            {
                byte[] t_byte = p.GetBytes();
                if (GridServer.InvokeContainsKeyPlayer(id))
                {
                    ClientManager cm = GridServer.InvokeGetPlayer(id);
                    cm._stream.Write(t_byte, 0, t_byte.Length);
                    cm._stream.Flush();
                    WaitToRead = Waitread;
                    Console.WriteLine("Packet Sent: " + p._packetid.ToString() + " Length: " + t_byte.Length);
                    t_byte = null;
                    p = null;
                }
                else if (PingType == true)
                {
                    _cm._stream.Write(t_byte, 0, t_byte.Length);
                    t_byte = null;
                    p = null;
                }
                else
                {
                    t_byte = null;
                    p = null;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("ERROR: " + e.Message);
            }
        }
        protected void Recieve(object obj)
        {
            _recieveClient = (ClientManager)obj;
            PacketReader p_reader = new PacketReader(new System.IO.BinaryReader(_recieveClient._stream));
            bool connected = true;
            while (connected == true)
            {
                try
                {
                    if (_recieveClient.WaitToRead == false && _recieveClient._stream != null && !_recieveClient.customAttributes.ContainsKey("InPrechunk") && _recieveClient._client.Available > 0)
                    {
                        WaitToRead = true;
                        Thread.Sleep(3);
                        PacketType p_type = (PacketType)p_reader.ReadByte();
                        if (ModuleHandler.Eventmodules.ContainsKey(p_type))
                            ModuleHandler.Eventmodules[p_type](ref p_reader, p_type, ref _recieveClient);
                        else
                        {
                            System.Console.WriteLine(p_type.ToString() + " is not implemented:" + (byte)p_type);
                            //byte[] temp = new byte[_recieveClient._stream.Length];
                            //_recieveClient._stream.Read(temp, 0, (int)_recieveClient._stream.Length);
                            //Console.Write(temp);
                        }

                    }
                    else
                    {
                        if (_recieveClient.WaitToRead == false && keep_alive.Second > DateTime.Now.Second + 1 && !_recieveClient.customAttributes.ContainsKey("InPrechunk"))
                        {
                            LibOpenCraft.ServerPackets.KeepAlivePacket p = new LibOpenCraft.ServerPackets.KeepAlivePacket(PacketType.KeepAlive);
                            p.AddInt(id);
                            SendPacket(p, id);
                            p = null;
                            keep_alive = DateTime.Now;
                        }
                        if (_player.name == "")
                            Thread.Sleep(10);
                        else
                            Thread.Sleep(3);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: " + e.Message);
                }
            }
        }
    }
    
}
