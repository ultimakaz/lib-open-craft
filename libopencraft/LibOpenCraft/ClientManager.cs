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

        public ClientManager(TcpClient client, int _id)
        {
            try
            {
                id = _id;
                _client = client;
                _stream = client.GetStream();
                recieve_async = new AsyncCallback(AsyncResult_recieve);
                recieve_start = new ParameterizedThreadStart(Recieve);
                recieved = new Thread(recieve_start);
                customAttributes.Add("PayLoad", null);
                if (recieved == null) recieved = new Thread(recieve_start);
                recieved.Start(id);
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
            recieved = new Thread(recieve_start);
            recieved.Start(id);
        }
        public bool WaitToRead = false;
        public void Start()
        {
            recieved.Start(id);
        }
        private void AsyncResult_recieve(IAsyncResult IAR)
        {
        }
        public void SendPacket(PacketHandler p, int id, bool PingType = false, ClientManager _cm = null, bool Waitread = false)
        {
            try
            {
                byte[] t_byte = p.GetBytes();
                if (GridServer.player_list.ContainsKey(id) && PingType == false)
                {
                    ClientManager cm = GridServer.player_list[id];
                    cm._stream.Write(t_byte, 0, t_byte.Length);
                    cm._stream.Flush();
                    WaitToRead = Waitread;
                    Console.WriteLine("Packet Sent: " + p._packetid.ToString() + " Length: " + t_byte.Length);
                    t_byte = null;
                    p = null;
                }
                else if (PingType == true)
                {
                    byte[] temp = new byte[256];
                    t_byte.CopyTo(temp, 0);
                    _cm._stream.Write(temp, 0, temp.Length);
                    _cm._stream.Flush();
                    _cm._client.Close();
                    _recieveClient.Stop(true);
                    Console.WriteLine("Packet Sent: " + p._packetid.ToString() + " Length: " + t_byte.Length);
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
                _stream.Close();
                _client.Close();
                Stop(true);
                Console.WriteLine("ERROR: " + e.Message + " Source:" + e.Source);
                Random r = new Random();
                GridServer.player_list.Remove(id);
            }
        }
        protected void Recieve(object obj)
        {
            int _id = (int)obj;
            if (!GridServer.player_list.ContainsKey(_id))
                _id = this.id;
            if (!GridServer.player_list.ContainsKey(_id))
                return;
            _recieveClient = GridServer.player_list[_id];
            PacketReader p_reader = new PacketReader(new System.IO.BinaryReader(_recieveClient._stream));
            bool connected = true;
            while (connected == true)
            {
                try
                {
                    if (_recieveClient._client == null || id == -1)
                    {
                        _recieveClient.Stop(true);
                        break;
                    }
                    if (!_recieveClient._client.Connected == true)
                    {
                        _recieveClient._client.Close();
                        _recieveClient.Stop(true);
                        break;
                    }
                    if (_recieveClient.WaitToRead == false && _recieveClient._stream != null && !_recieveClient.customAttributes.ContainsKey("InPrechunk") && _recieveClient._client.Available > 0)
                    {
                        if (_recieveClient._stream != null && DateTime.Now.Minute > _recieveClient.keep_alive.Minute || _recieveClient.keep_alive.Second + 1 < DateTime.Now.Second)
                        {
                            Random r = new Random(1789);
                            _recieveClient.keep_alive = DateTime.Now;
                            _recieveClient.customAttributes["PayLoad"] = r.Next(1024, 4096);
                            LibOpenCraft.ServerPackets.KeepAlivePacket p = new LibOpenCraft.ServerPackets.KeepAlivePacket(PacketType.KeepAlive);
                            p.AddInt(id);
                            SendPacket(p, id);
                        }
                        _recieveClient.WaitToRead = true;
                        Thread.Sleep(3);
                        PacketType p_type = (PacketType)p_reader.ReadByte();
                        if (ModuleHandler.Eventmodules.ContainsKey(p_type))
                            ModuleHandler.Eventmodules[p_type](ref p_reader, p_type, ref _recieveClient);
                        else
                        {
                            System.Console.WriteLine(p_type.ToString() + " is not implemented:" + (byte)p_type);
                            //int b = 0;
                            //List<char> c_buffer = new List<char>();
                            //string temp_write = "";
                            //while ((b = _recieveClient._stream.ReadByte()) != '\0' || b != -1) { c_buffer.Add((char)b); temp_write += (char)b; }
                            //Console.Write(temp_write);
                            _recieveClient.WaitToRead = false;
                        }

                    }
                    else
                    {
                        if (_recieveClient._stream != null && DateTime.Now.Minute > _recieveClient.keep_alive.Minute || _recieveClient.keep_alive.Second + 1 < DateTime.Now.Second)
                        {
                            Random r = new Random(1789);
                            _recieveClient.keep_alive = DateTime.Now;
                            _recieveClient.customAttributes["PayLoad"] = r.Next(1024, 4096);
                            LibOpenCraft.ServerPackets.KeepAlivePacket p = new LibOpenCraft.ServerPackets.KeepAlivePacket(PacketType.KeepAlive);
                            p.AddInt(id);
                            SendPacket(p, id);
                        }
                        if (_player.name == "")
                            Thread.Sleep(10);
                        else
                            Thread.Sleep(3);
                    }
                }
                catch (Exception e)
                {
                    if (_recieveClient._stream != null)
                        _recieveClient._stream.Close();
                    if (_recieveClient._client != null)
                        _recieveClient._client.Close();
                    _recieveClient.Stop(true);
                    GridServer.player_list.Remove(id);
                    Console.WriteLine("ERROR: " + e.Message + " Source:" + e.Source);
                    break;
                }
            }
        }
    }

}
#region  working non async
/* in Theory non async */
/*
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

        public ClientManager(TcpClient client, int _id)
        {
            try
            {
                id = _id;
                _client = client;
                _stream = client.GetStream();
                recieve_async = new AsyncCallback(AsyncResult_recieve);
                recieve_start = new ParameterizedThreadStart(Recieve);
                recieved = new Thread(recieve_start);
                customAttributes.Add("PayLoad", null);
                recieved = new Thread(recieve_start);
                recieved.Start(id);
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
            recieved = new Thread(recieve_start);
            recieved.Start(id);
        }
        public bool WaitToRead = false;
        public void Start()
        {
            recieved.Start(id);
        }
        private void AsyncResult_recieve(IAsyncResult IAR)
        {

        }
        public void SendPacket(PacketHandler p, int id, bool PingType = false, ClientManager _cm = null, bool Waitread = false)
        {
            try
            {
                byte[] t_byte = p.GetBytes();
                if (GridServer.player_list.ContainsKey(id) && PingType == false)
                {
                    GridServer.player_list[id]._stream.Write(t_byte, 0, t_byte.Length);
                    GridServer.player_list[id]._stream.Flush();
                    GridServer.player_list[id].WaitToRead = Waitread;
                    Console.WriteLine("Packet Sent: " + p._packetid.ToString() + " Length: " + t_byte.Length);
                    t_byte = null;
                    p = null;
                }
                else if (PingType == true)
                {
                    byte[] temp = new byte[256];
                    t_byte.CopyTo(temp, 0);
                    GridServer.player_list[id]._stream.Write(temp, 0, temp.Length);
                    GridServer.player_list[id]._stream.Flush();
                    GridServer.player_list[id]._client.Close();
                    GridServer.player_list[id]._recieveClient.Stop(true);
                    GridServer.player_list[id].WaitToRead = Waitread;
                    Console.WriteLine("Packet Sent: " + p._packetid.ToString() + " Length: " + t_byte.Length);
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
                GridServer.player_list[id]._stream.Close();
                GridServer.player_list[id]._client.Close();
                GridServer.player_list[id].Stop(true);
                GridServer.player_list[id].WaitToRead = Waitread;
                Console.WriteLine("ERROR: " + e.Message + " Source:" + e.Source);
                Random r = new Random();
                GridServer.player_list.Remove(id);
            }
        }
        protected void Recieve(object obj)
        {
            int id = (int)obj;
            GridServer.player_list[id]._stream = new NetworkStream(GridServer.player_list[id]._client.Client);
            PacketReader p_reader = new PacketReader(new System.IO.BinaryReader(GridServer.player_list[id]._stream));
            bool connected = true;
            while (connected == true)
            {
                try
                {
                    Thread.Sleep(50);
                    if (GridServer.player_list[id]._client == null || id == -1)
                    {
                        GridServer.player_list[id].Stop(true);
                        break;
                    }
                    Thread.Sleep(50);
                    if (!GridServer.player_list[id]._client.Connected == true)
                    {
                        GridServer.player_list[id]._client.Close();
                        GridServer.player_list[id].Stop(true);
                        break;
                    }
                    Thread.Sleep(50);
                    if (GridServer.player_list[id].WaitToRead == false && GridServer.player_list[id]._stream != null && !GridServer.player_list[id].customAttributes.ContainsKey("InPrechunk") && GridServer.player_list[id]._client.Available >= 1)
                    {
                        GridServer.player_list[id].WaitToRead = true;
                        Thread.Sleep(50);
                        PacketType p_type = (PacketType)p_reader.ReadByte();
                        Thread.Sleep(50);
                        if (ModuleHandler.Eventmodules.ContainsKey(p_type))
                        {
                            ClientManager client = GridServer.player_list[id];
                            ModuleHandler.Eventmodules[p_type](ref p_reader, p_type, ref client);
                            GridServer.player_list[id] = client;
                        }
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
                        if (_recieveClient.WaitToRead == false && _recieveClient._stream != null && keep_alive.Second + 1 < DateTime.Now.Second && !_recieveClient.customAttributes.ContainsKey("InPrechunk"))
                        {
                            GridServer.player_list[id].WaitToRead = true;
                            Random r = new Random(1789);
                            GridServer.player_list[id].customAttributes["PayLoad"] = r.Next(1024, 4096);
                            LibOpenCraft.ServerPackets.KeepAlivePacket p = new LibOpenCraft.ServerPackets.KeepAlivePacket(PacketType.KeepAlive);
                            p.AddInt(id);
                            GridServer.player_list[id].SendPacket(p, id);
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
                    if (GridServer.player_list[id]._stream != null)
                        GridServer.player_list[id]._stream.Close();
                    if (GridServer.player_list[id]._client != null)
                        GridServer.player_list[id]._client.Close();
                    GridServer.player_list[id].Stop(true);
                    GridServer.player_list.Remove(id);
                    Console.WriteLine("ERROR: " + e.Message + " Source:" + e.Source);
                    break;
                }
            }
        }
    }
    
}
*/
#endregion working non async