using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace NETLibrary {
	public class NET {
		private Socket Listen = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
		private Socket Send   = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);

		public NET() {}
		public void SetListenData(Address addr) {
			Listen.Bind(addr.GetAddress());
			Listen.Listen(1);
		}
		public void StartListen(ref String returndata) {
			Socket ListenData = Listen.Accept();
			byte[] bytes = new byte[4294967296];
			ListenData.Receive(bytes,bytes.Length,0);
			returndata = Encoding.UTF8.GetString(bytes);
		}
		public void SetSendData(Address addr) {
			Send.Connect(addr.GetAddress());
		}
		public void StartSend(String data) {
			Send.Send(Encoding.UTF8.GetBytes(data));
		}
		public void Clear() {
			Listen = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
			Send   = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
		}
		public void Close() {
			Listen.Shutdown(SocketShutdown.Both);
			Listen.Close();
			Send.Shutdown(SocketShutdown.Both);
			Send.Close();
		}
	}
	public class Communication {
		private NET[]     ListenAndSend = new NET[256];
		private Address[] SendAddress   = new Address[256];
		private int       SendKey       = 0;
		private Address[] ListenAddress = new Address[256];
		private int       ListenKey     = 0;

		public Communication() {}
		public Communication(int Number) {
			Number = Number < 0 ? Number : 0;
			ListenAndSend = new NET[Number];
			SendAddress   = new Address[Number];
			ListenAddress = new Address[Number];
		}
		public int AddListenData(Address addr) {
			ListenAddress[ListenKey] = addr;
			ListenAndSend[ListenKey] = ListenAndSend[ListenKey] == null ? new NET() : ListenAndSend[ListenKey];
			ListenAndSend[ListenKey].SetListenData(addr);
			ListenKey += 1;
			return ListenKey - 1;
		}
		public void SetListenData(int Key,Address addr) {
			ListenAddress[Key] = addr;
			ListenAndSend[Key].SetListenData(addr);
		}
		public void ListenData(int Key,ref String returndata) {
			ListenAndSend[Key].StartListen(ref returndata);
		}
		public int AddSendData(Address addr) {
			SendAddress[SendKey] = addr;
			ListenAndSend[SendKey] = ListenAndSend[SendKey] == null ? new NET() : ListenAndSend[SendKey] ;
			ListenAndSend[SendKey].SetSendData(addr);
			SendKey += 1;
			return SendKey - 1;
		}
		public void SetSendData(int Key,Address addr) {
			SendAddress[Key] = addr;
			ListenAndSend[Key].SetSendData(addr);
		}
		public void SendData(int Key,String data) {
			ListenAndSend[Key].StartSend(data);
		}
		public void Clear() {
			for (int i = 0;i < SendKey && i < ListenKey;i ++) {
				ListenAndSend[i].Clear();
			}
		}
		public void Close() {
			for (int i = 0;i < SendKey && i < ListenKey;i ++) {
				ListenAndSend[i].Close();
			}
		}
	}
	public class Connect {
		private Socket ConnectSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
		private bool   ConnectStatus = false;

		public Connect() {}
		public void ConnectTo(Address addr) {
			if (!ConnectStatus) {return;}
			ConnectSocket.Connect(addr.GetAddress());
			ConnectStatus = true;
		}
		public void AcceptConnection(Address addr) {
			if (!ConnectStatus) {return;}
			ConnectSocket.Bind(addr.GetAddress());
			ConnectSocket.Listen(1);
			ConnectSocket = ConnectSocket.Accept();
			ConnectStatus = true;
		}
		public void Send(String data) {
			ConnectSocket.Send(Encoding.UTF8.GetBytes(data));
		}
		public void Listen(ref String returndata) {
			byte[] bytes = new byte[4294967296];
			ConnectSocket.Receive(bytes,bytes.Length,0);
			returndata = Encoding.UTF8.GetString(bytes);
		}
		public void Clear() {
			ConnectSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
			ConnectStatus = false;
		}
		public void Close() {
			ConnectSocket.Shutdown(SocketShutdown.Both);
			ConnectSocket.Close();
		}
	}
	/*public class Connects {
		private Connect[] ListenAndSend = new Connect[256];
		private Address[] Address       = new Address[256];
		private int       ConnectKey    = 0;

		public Connects() {}
		public Connects(int Number) {
			ListenAndSend = new Connect[Number];
			Address       = new Address[Number];
		}
	}*/
	public class Address {
		private IPEndPoint addr;

		public Address(String IP,int Port) {
			addr = new IPEndPoint(IPAddress.Parse(IP),Port);
		}
		public Address(int Port) {
			addr = new IPEndPoint(IPAddress.Any,Port);
		}
		public Address(String IP) {
			addr = new IPEndPoint(IPAddress.Parse(IP),0);
		}
		public Address() {
			addr = new IPEndPoint(IPAddress.Any,0);
		}
		public IPEndPoint GetAddress() {
			return addr;
		}
		public void GetAddress(ref IPEndPoint returnaddr) {
			returnaddr = addr;
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