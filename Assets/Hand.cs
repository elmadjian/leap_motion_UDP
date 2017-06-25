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

public class Hand : MonoBehaviour {
	
	public Material blue;
	public Material red;
	public bool pinch;
	public GameObject atom;


	// Use this for initialization
	void Start () {
		pinch = false;
		atom = null;
	}

	public void ParseData(string[] data) {
		string[] pos_data = data [1].Split (',');
		string[] rot_data = data [2].Split (',');
		string pinchState = data [3];
		SetPosition (pos_data);
		SetRotation (rot_data);
		SetPinch (pinchState);
	}

	private void SetPosition(string[] pos_data) {
		float x = Convert.ToSingle (pos_data [0], CultureInfo.InvariantCulture);
		float y = Convert.ToSingle (pos_data [1], CultureInfo.InvariantCulture);
		float z = Convert.ToSingle (pos_data [2], CultureInfo.InvariantCulture);
		Vector3 pos = new Vector3 (x, y, z);
		transform.localPosition = pos;
	}

	private void SetRotation(string[] rot_data) {
		float x = Convert.ToSingle (rot_data [0], CultureInfo.InvariantCulture);
		float y = Convert.ToSingle (rot_data [1], CultureInfo.InvariantCulture);
		float z = Convert.ToSingle (rot_data [2], CultureInfo.InvariantCulture);
		Quaternion target = Quaternion.Euler(x, y, z);
		transform.rotation = target;
	} 

	private void SetPinch(string pinchState) {
		if (pinchState == "true") {
			GetComponent<Renderer> ().material = red;
			pinch = true;
		} else {
			GetComponent<Renderer> ().material = blue;
			pinch = false;
		}
	}
}
