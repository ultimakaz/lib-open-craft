using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

namespace LibOpenCraft.MajongProtocol
{
    [Export(typeof(CoreModule))]
    [ExportMetadata("Name", "Client Listener")]
    public class ClientListener : CoreModule
    {
        private Thread listener;
        private ParameterizedThreadStart listener_start;
        private AsyncCallback listener_async;
        private TcpListener _listener;
        string name = "";
        public ClientListener()
            : base()
        {
            listener_async = new AsyncCallback(AsyncResult_listener);
            listener_start = new ParameterizedThreadStart(Listener);
            listener_start.BeginInvoke(this, listener_async, this);
            listener = new Thread(listener_start);
            base.Start();
        }

        public override void Start()
        {
            base.Start();
            listener.Start(this);
        }

        public override void Stop()
        {
            base.Stop();
            listener.Abort();
        }

        private void AsyncResult_listener(IAsyncResult IAR)
        {

        }
        private void AsyncResult_newcon(IAsyncResult IAR)
        {
            if (IAR.AsyncState == (object)"New Connection")
            {
                Random r = new Random();

                int id = r.Next(0, (int)Config.Configuration["MaxPlayers"]);
                while (GridServer.ContainsPlayer(id) == true)
                {
                    id = r.Next(0, (int)Config.Configuration["MaxPlayers"]);
                }
                GridServer.player_list[id] = new ClientManager(_listener.EndAcceptTcpClient(IAR), id);
            }
        }
        protected void Listener(object obj)
        {
            _listener = new TcpListener(IPAddress.Parse((string)Config.Configuration["IPAddress"]), (int)Config.Configuration["Port"]);
            _listener.Start(100);
            while (true)
            {
                if (_listener.Pending())
                    _listener.BeginAcceptTcpClient(new AsyncCallback(AsyncResult_newcon), (object)"New Connection");
                else
                    Thread.Sleep(1);
            }
            
        }
    }
}
