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
	string[] leftHandData;
	string[] rightHandData;

	public GameObject rightHand;
	public GameObject leftHand;

	// Use this for initialization
	void Start () {
		try {
			data = new byte[1024];
			sender = new IPEndPoint (IPAddress.Any, 0);
			socket = new UdpClient(9988);
			socket.Client.ReceiveTimeout = 2000;
			bufferCount = 0;
			Thread connection = new Thread(new ThreadStart(ProcessConnection));
			connection.Start();
		} catch (SocketException) {
			print ("Warning: failed to initialize network.");
		}
	}

	private void ProcessConnection() {
		this.bufferCount = 10;
		while (true) {
			try {
				data = socket.Receive (ref sender);
				receivedData = Encoding.ASCII.GetString (data, 0, data.Length);
				//Debug.Log (receivedData);
				if (receivedData != "") {
					this.bufferCount = 10;
					string[] rawdata = receivedData.Split (';');
					if (rawdata [0] == "right")
						this.rightHandData = rawdata;
					else if (rawdata [0] == "left")
						this.leftHandData = rawdata;
				}
			} catch(SocketException) {
				this.bufferCount--;
				sender = new IPEndPoint (IPAddress.Any, 0);
			}
			if (this.bufferCount <= 0)
				break;
		}
	}

	// Update is called once per frame
	void Update() {
		rightHand.GetComponent<Hand> ().ParseData (rightHandData);
		leftHand.GetComponent<Hand> ().ParseData (leftHandData);
	}
}
