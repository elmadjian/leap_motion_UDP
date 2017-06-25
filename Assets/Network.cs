using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System;
using System.Globalization;
using System.Text;

public class Network : MonoBehaviour {

	private string inputBuffer;
	private IPAddress host;
	private byte[] data;
	private int bufferCount;
	UdpClient socket;
	IPEndPoint ipep;
	IPEndPoint sender;
	string receivedData;

	public GameObject rightHand;
	public GameObject leftHand;

	// Use this for initialization
	void Start () {
		data = new byte[1024];
		host = Dns.GetHostAddresses(Dns.GetHostName())[0];
		ipep = new IPEndPoint(host, 9988);
		sender = new IPEndPoint (IPAddress.Any, 0);
		socket = new UdpClient (ipep);
		socket.Client.ReceiveTimeout = 2000;
		bufferCount = 0;
	}

	// Update is called once per frame
	void Update() {
		if (bufferCount < 30)
			bufferCount += 1;
		else {
			data = socket.Receive (ref sender);
			receivedData = Encoding.ASCII.GetString (data, 0, data.Length);
			if (receivedData != "") {
				string[] rawdata = receivedData.Split (';');
				if (rawdata [0] == "right")
					rightHand.GetComponent<Hand> ().ParseData (rawdata);
				else if (rawdata [0] == "left")
					leftHand.GetComponent<Hand> ().ParseData (rawdata);
			}
		}
	}
}
