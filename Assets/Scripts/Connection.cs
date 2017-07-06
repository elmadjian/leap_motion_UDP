using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System;
using System.Globalization;

public class Connection : MonoBehaviour {

	private string inputBuffer;
	private IPAddress host;
	private int port;
	TcpListener server;
	TcpClient socket;
	NetworkStream netStream;
	StreamWriter socketWriter;
	StreamReader socketReader;

	// Use this for initialization
	void Start () {
		Listen ();
	}

	//Connect to remote server
	void Listen() {
		port = 9998;
		//host = IPAddress.Parse("127.0.0.1");
		host = Dns.GetHostAddresses(Dns.GetHostName())[0];
		server = new TcpListener (host, port);
		server.Start();
		socket = server.AcceptTcpClient();
		Debug.Log("Client connected to server");
		netStream = socket.GetStream ();
		socketWriter = new StreamWriter (netStream);
		socketReader = new StreamReader (netStream);
	}

	// Send a message to other device
	void SendMsg (string message) {
		string line = message + "\r\n";
		socketWriter.Write (line);
		socketWriter.Flush ();
	}

	// Read message from other device
	string ReadMsg () {
		if (netStream.DataAvailable)
			return socketReader.ReadLine ();
		return "";
	}

	// Update is called once per frame
	void Update() {
		string receivedData = "";
		if (socket != null)
			receivedData = ReadMsg ();
		else {
			Listen ();
		}
		if (receivedData != "") {
			string[] data = receivedData.Split (',');
			float x = Convert.ToSingle (data [0], CultureInfo.InvariantCulture);
			float y = Convert.ToSingle (data [1], CultureInfo.InvariantCulture);
			float z = Convert.ToSingle (data [2], CultureInfo.InvariantCulture);
			Vector3 pos = new Vector3 (x, y, z);
			transform.localPosition = pos;
//			string[] data = receivedData.Split (',');
//			for (int i = 0; i < data.Length; i += 2) {
//				if (data [i+1] == "down") {
//					Debug.Log ("key down:" + data [i]);
//					int val = Int32.Parse (data [i]);
//				} else {
//					Debug.Log ("key up:" + data [i]);
//					int val = Int32.Parse (data [i]);
//				}	
//			}
		}
	}
}