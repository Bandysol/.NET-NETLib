using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NETLibrary {
	public class NET {
		private Socket Listen;
		private Socket Send;

		public NET() {
			Listen = null;
			Send   = null;
		}
		public NET(String Mod,String Mod2) {
			if (Mod.Equals("TCP")) {
				Listen = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
			} else if (Mod.Equals("UDP")) {
				Listen = new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);
			} else {
				throw new NETLibraryException.NETException("输入了错误的初始化值。");
			}
			if (Mod2.Equals("TCP")) {
				Send = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
			} else if (Mod2.Equals("UDP")) {
				Send = new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);
			} else {
				throw new NETLibraryException.NETException("输入了错误的初始化值。");
			}
		}
		public NET(String Mod) {
			if (Mod.Equals("TCP")) {
				Listen = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
				Send   = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
			} else if (Mod.Equals("UDP")) {
				Listen = new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);
				Send   = new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);
			} else {
				throw new NETLibraryException.NETException("输入了错误的初始化值。");
			}
		}
		public void SetListenData(String IP,String port) {
			if (Listen == null) {
				throw new NETLibraryException.NETException("尚未初始化或初始化不正确。");
			}
			try {
				Listen.Bind(new IPEndPoint(IPAddress.Parse(IP),int.Parse(port)));
			} catch(SocketException e) {
				if (e.Message.StartsWith("由于连接方在一段时间后没有正确答复或连接的主机没有反应，连接尝试失败。")) {
					throw new NETLibraryException.NETException("目标无响应。",e);
				} else if (e.Message.StartsWith("在其上下文中，该请求的地址无效。")) {
					throw new NETLibraryException.NETException("指定的端口号或域名不正确。",e);
				} else {
					throw e;
				}
			} catch(FormatException e) {
				if (e.Message.StartsWith("指定了无效的 IP 地址。")) {
					throw new NETLibraryException.NETException("指定了无效的IP地址。",e);
				} else if (e.Message.StartsWith("输入字符串的格式不正确。")) {
					throw new NETLibraryException.NETException("指定了无效的端口号。",e);
				} else {
					throw e;
				}
			} catch(ArgumentOutOfRangeException e) {
				if (e.Message.StartsWith("指定的参数已超出有效值的范围。")) {
					throw new NETLibraryException.NETException("端口号过大或过小。",e);
				} else {
					throw e;
				}
			}
			Listen.Listen(1);
		}
		public String StartListen() {
			Socket ListenData = Listen.Accept();
			byte[] bytes = new byte[4294967296];
			ListenData.Receive(bytes,bytes.Length,0);
			return Encoding.UTF8.GetString(bytes);
		}
		public void SetSendData(String IP,String port) {
			if (Send == null) {
				throw new NETLibraryException.NETException("尚未初始化或初始化不正确。");
			}
			try {
				Send.Connect(new IPEndPoint(IPAddress.Parse(IP),int.Parse(port)));
			} catch(SocketException e) {
				if (e.Message.StartsWith("由于连接方在一段时间后没有正确答复或连接的主机没有反应，连接尝试失败。")) {
					throw new NETLibraryException.NETException("目标无响应。",e);
				} else if (e.Message.StartsWith("向一个无法连接的网络尝试了一个套接字操作。")) {
					throw new NETLibraryException.NETException("指定的目标无法连接。",e);
				} else {
					throw e;
				}
			} catch(FormatException e) {
				if (e.Message.StartsWith("指定了无效的 IP 地址。")) {
					throw new NETLibraryException.NETException("指定了无效的IP地址。",e);
				} else if (e.Message.StartsWith("输入字符串的格式不正确。")) {
					throw new NETLibraryException.NETException("指定了无效的端口号。",e);
				} else {
					throw e;
				}
			} catch(ArgumentOutOfRangeException e) {
				if (e.Message.StartsWith("指定的参数已超出有效值的范围。")) {
					throw new NETLibraryException.NETException("端口号过大或过小。",e);
				} else {
					throw e;
				}
			}
		}
		public void StartSend(String data) {
			Send.Send(Encoding.UTF8.GetBytes(data));
		}
		public void Close() {
			Listen.Shutdown(SocketShutdown.Both);
			Listen.Close();
			Send.Shutdown(SocketShutdown.Both);
			Send.Close();
		}
	}
	public class Communication {
		private NETLibrary.NET[] ListenAndSend;
		private String[] SendIP;
		private String[] SendPort;
		private int SendKey = 0;
		private String[] ListenIP;
		private String[] ListenPort;
		private int ListenKey = 0;
		private int ArrayLength;

		public Communication() {
			new Communication(0);
		}
		public Communication(int Number) {
			if (Number < 0) {
				Number = 0;
			}
			ArrayLength   = Number + 1;
			SendKey       = Number;
			ListenKey     = Number;
			ListenAndSend = new NET[Number];
			SendIP        = new String[Number];
			SendPort      = new String[Number];
			ListenIP      = new String[Number];
			ListenPort    = new String[Number];
		}
		public int AddListenData(String IP,String Port) {
			try {
				ListenIP[ListenKey] = IP;
				ListenPort[ListenKey] = Port;
				if (ListenAndSend[ListenKey] == null) {
					ListenAndSend[ListenKey] = new NET("TCP");
				}
				ListenAndSend[ListenKey].SetListenData(IP,Port);
			} catch(IndexOutOfRangeException e) {
				if (e.Message.StartsWith("索引超出了数组界限。")) {
					throw new NETLibraryException.CommunicationException("指定的值超过了限定的最大值。",e);
				} else {
					throw e;
				}
			}
			ListenKey += 1;
			return ListenKey - 1;
		}
		public void SetListenData(int Key,String IP,String Port) {
			ListenIP[Key] = IP;
			ListenPort[Key] = Port;
			ListenAndSend[Key].SetListenData(IP,Port);
		}
		public String ListenData(int Key) {
			return ListenAndSend[Key].StartListen();
		}
		public int AddSendData(String IP,String Port) {
			try {
				SendIP[SendKey] = IP;
				SendPort[SendKey] = Port;
				if (ListenAndSend[SendKey] == null) {
					ListenAndSend[SendKey] = new NET("TCP");
				}
				ListenAndSend[SendKey].SetSendData(IP,Port);
			} catch(IndexOutOfRangeException e) {
				if (e.Message.StartsWith("索引超出了数组界限。")) {
					throw new NETLibraryException.CommunicationException("指定的值超过了限定的最大值。",e);
				} else {
					throw e;
				}
			}
			SendKey += 1;
			return SendKey - 1;
		}
		public void SetSendData(int Key,String IP,String Port) {
			if (ListenAndSend[Key] == null) {
				throw new NETLibraryException.CommunicationException("应当调用 NETLibrary.Communication.AddSendData(String IP,String Port) 方法。");
			}
			SendIP[Key]   = IP;
			SendPort[Key] = Port;
			ListenAndSend[Key].SetSendData(IP,Port);
		}
		public void SendData(int Key,String data) {
			ListenAndSend[Key].StartSend(data);
		}
		public void Close() {
			for (int i = 0;i < SendKey & i < ListenKey;i += 1) {
				ListenAndSend[i].Close();
			}
		}
	}
}
namespace NETLibraryException {
	public class NETException : Exception {
		public NETException(String message) : base(message) {}
		public NETException(String message,Exception inner) : base(message,inner) {}
	}
	public class CommunicationException : Exception {
		public CommunicationException(String message) : base(message) {}
		public CommunicationException(String message,Exception inner) : base(message,inner) {}
	}
}