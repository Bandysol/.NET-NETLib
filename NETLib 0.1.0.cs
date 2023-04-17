using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace NETLibrary {
	public class NET {
		private Socket Listen;
		private Socket Send;

		public NET(String Mod,String Mod2) {
			if (Mod.Equals("TCP")) {
				Listen = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
			} else if (Mod.Equals("UDP")) {
				Listen = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Udp);
			} else {
				Listen = null;
			}
			if (Mod2.Equals("TCP")) {
				Send = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
			} else if (Mod2.Equals("UDP")) {
				Send = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Udp);
			} else {
				Send = null;
			}
		}
		public NET(String Mod) {
			if (Mod.Equals("TCP")) {
				Listen = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
				Send   = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
			} else if (Mod.Equals("UDP")) {
				Listen = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Udp);
				Send   = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Udp);
			} else {
				Listen = null;
				Send   = null;
			}
		}
		public NET() {
			Listen = null;
			Send   = null;
		}
		public String ListenData(String IP,String port) {
			Listen.Bind(new IPEndPoint(IPAddress.Parse(IP),int.Parse(port)));
			Listen.Listen(1);
			Socket ListenData = Listen.Accept();
			byte[] bytes = new byte[65536];
			ListenData.Receive(bytes,bytes.Length,0);
			return Encoding.UTF8.GetString(bytes);
		}
		public String ListenData(String IP,String port,int ListenNum) {
			if (ListenNum < 0) {
				return "";
			}
			Listen.Bind(new IPEndPoint(IPAddress.Parse(IP),int.Parse(port)));
			Listen.Listen(ListenNum);
			Socket ListenData = Listen.Accept();
			byte[] bytes = new byte[65536];
			ListenData.Receive(bytes,bytes.Length,0);
			return Encoding.UTF8.GetString(bytes);
		}
		public void SendData(String IP,String port,String data) {
			Send.Connect(new IPEndPoint(IPAddress.Parse(IP),int.Parse(port)));
			Send.Send(Encoding.UTF8.GetBytes(data));
		}
		public void Close() {
			Listen.Shutdown(SocketShutdown.Both);
			Listen.Close();
			Send.Shutdown(SocketShutdown.Both);
			Send.Close();
		}

		private bool SocketNull() {
			if (Listen == null & Send == null) {
				return true;
			}
			return false;
		}
	}
}
