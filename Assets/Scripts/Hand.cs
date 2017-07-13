using System.Collections.Generic;
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

	public bool isMarkerBased = false;

	public Material selected;
	public Material unselected;
	public bool pinch;
	public GameObject atom;
	public bool isRightHand;

	List<String> commands;

	private Vector3 up = new Vector3 (0, 1, 0);
	private Vector3 forward = new Vector3 (0, 0, 1);
	private Vector3 right = new Vector3 (1, 0, 0);

	private bool pinchIsPressed = false;

	// Use this for initialization
	void Start () {

		if (isMarkerBased) {
			pinch = true;
		} else {
			pinch = false;
		}

		atom = null;

		if (isRightHand) {
			commands = RightHandCommands ();
		} else {
			commands = LeftHandCommands ();
		}
	}

	public void ParseData(string[] data) {
		if (data != null) {
			string[] pos_data = data [1].Split (',');
			string[] rot_data = data [2].Split (',');
			string pinchState = data [3];
			SetPosition (pos_data);
			SetRotation (rot_data);
			SetPinch (pinchState);
		}
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
		if (isMarkerBased) {
			return;
		}

		SetPinch(pinchState == "true");
	}

	private void SetPinch(bool pinchState) {
		if (isMarkerBased) {
			return;
		}

		if (pinchState) {
			GetComponent<Renderer> ().material = selected;
			pinch = true;
		} else {
			GetComponent<Renderer> ().material = unselected;
			pinch = false;
		}
	}

	void Update () {
		float translationSpeed = 0.4f;
		float rotationSpeed = 3.0f;

		// Translation
		if (!Input.GetKey ("space")) {
			if (Input.GetKey (commands [0])) {
				transform.Translate (forward * translationSpeed);
			}
			if (Input.GetKey (commands [1])) {
				transform.Translate (-forward * translationSpeed);
			}
			if (Input.GetKey (commands [2])) {
				transform.Translate (right * translationSpeed);
			}
			if (Input.GetKey (commands [3])) {
				transform.Translate (-right * translationSpeed);
			}
			if (Input.GetKey (commands [4])) {
				transform.Translate (up * translationSpeed);
			}
			if (Input.GetKey (commands [5])) {
				transform.Translate (-up * translationSpeed);
			}
		}
		// Rotation
		else {
			if (Input.GetKey (commands [0])) {
				transform.Rotate (new Vector3 (rotationSpeed, 0, 0));
			}
			if (Input.GetKey (commands [1])) {
				transform.Rotate (new Vector3 (-rotationSpeed, 0, 0));
			}
			if (Input.GetKey (commands [2])) {
				transform.Rotate (new Vector3 (0, 0, rotationSpeed));
			}
			if (Input.GetKey (commands [3])) {
				transform.Rotate (new Vector3 (0, 0, -rotationSpeed));
			}
			if (Input.GetKey (commands [6])) {
				transform.Rotate (new Vector3 (0, rotationSpeed, 0));
			}
			if (Input.GetKey (commands [7])) {
				transform.Rotate (new Vector3 (0, -rotationSpeed, 0));
			}
		}

		// Pinch
		if (Input.GetKey (commands [8])) {
			if (!pinchIsPressed) {
				SetPinch (!pinch);
			}
			pinchIsPressed = true;
		} else {
			pinchIsPressed = false;
		}
	}

	List<String> LeftHandCommands() {
		List<String> commands = new List<String> ();
		commands.Add ("w");
		commands.Add ("s");
		commands.Add ("d");
		commands.Add ("a");
		commands.Add ("2");
		commands.Add ("x");
		commands.Add ("q");
		commands.Add ("e");
		commands.Add ("z");
		return commands;
	}

	List<String> RightHandCommands() {
		List<String> commands = new List<String> ();
		commands.Add ("i");
		commands.Add ("k");
		commands.Add ("l");
		commands.Add ("j");
		commands.Add ("8");
		commands.Add (",");
		commands.Add ("u");
		commands.Add ("o");
		commands.Add (".");
		return commands;
	}
}
