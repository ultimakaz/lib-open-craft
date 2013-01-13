using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using LibOpenCraft;
using LibOpenCraft.ServerPackets;

namespace LibOpenCraft.Query
{
	[Export(typeof(CoreModule))]
	[ExportMetadata("Name", "Query")]
	public class Query : CoreModule
	{
		private static Socket s = null;
		private int port = 0;
		private string ipaddress = null;
		private System.Net.EndPoint _endpoint;
		public Query()
			: base()
		{

		}
		
		public override void Start()
		{
			base.Start();
			port = (int)Config.Configuration["Query.port"];
			ipaddress = (string)Config.Configuration["IPAddress"];
			s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			_endpoint = new IPEndPoint(IPAddress.Parse(ipaddress), port);
			s.Bind(_endpoint);
			s.Listen(100);
			Thread QueryListener = new Thread(Query.Listen);
			QueryListener.Start(s);
			//ModuleHandler.InvokeAddModuleAddon(PacketType.PlayerBlockPlacement, OnBlockChange);
			base.RunModuleCache();
		}

		public static void Listen (object sock)
		{
			Socket _s = sock;
			while (true) {
				UdpClient client = _s.Accept();
				new QueryHandler(client);
			}
		}
		
		public PacketHandler OnBlockChange(PacketType p_type, string CustomPacketType, ref PacketReader packet_reader, PacketHandler _p, ref ClientManager cm)
		{
			return _p;
		}
		
		public override void Stop()
		{
			base.Stop();
		}
	}
}

