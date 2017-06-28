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
	
	public Material blue;
	public Material yellow;
	public Material red;
	public bool pinch;
	public GameObject atom;
	public bool isRightHand;

	List<String> commands;

	private Vector3 up = new Vector3 (0, 1, 0);
	private Vector3 forward = new Vector3 (0, 0, 1);
	private Vector3 right = new Vector3 (1, 0, 0);

	private Atom closestAtom;

	private bool pinchIsPressed = false;

	// Use this for initialization
	void Start () {
		pinch = false;
		atom = null;

		if (isRightHand) {
			commands = RightHandCommands ();
		} else {
			commands = LeftHandCommands ();
		}
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
		SetPinch(pinchState == "true");
	}

	private void SetPinch(bool pinchState) {
		if (pinch == pinchState) { return; }

		if (pinchState) {
			GetComponent<Renderer> ().material = red;
			pinch = true;
			GrabAtom ();
		} else {
			GetComponent<Renderer> ().material = blue;
			pinch = false;
			ReleaseAtom ();
		}
	}

	private Atom FindClosestAtom() {
		float smallestDistance = float.PositiveInfinity;
		Atom closestAtom = null;
		foreach (Atom anAtom in Atom.allAtoms) {
			// Ignore atoms being held by the other hand.
			if (anAtom.IsBeingHeld () && anAtom != this.atom) {
				continue;
			}

			float distance = Vector3.Distance (transform.position, anAtom.transform.position);

			// If we're grabbing further than the atom's proximity boundary, ignore it
			if (distance > anAtom.minimumProximity) {
				continue;
			}
				
			// If this is the closest atom so far
			if (distance < smallestDistance) {
				smallestDistance = distance;
				closestAtom = anAtom;
			}
		}

		foreach (Atom anAtom in Atom.allAtoms) {
			anAtom.SetIsClosest (isRightHand, closestAtom == anAtom);
		}

		return closestAtom;
	}

	private void GrabAtom () {
		if (closestAtom != null) {
			closestAtom.SetIsBeingHeld(true);
			this.atom = closestAtom.gameObject;
			this.atom.GetComponent<Renderer> ().material = red;
		}
	}

	private void ReleaseAtom () {
		this.atom.GetComponent<Atom> ().SetIsBeingHeld(false);
		this.atom = null;
	}

	private void SetColorInAtom(Material material, GameObject atom) {
		Renderer atomRenderer = atom.GetComponent<Renderer> ();
		if (atomRenderer.material != material) {
			atomRenderer.material = material;
		}
	}

	void Update () {
		HandleKeyPresses ();
		HandleAtoms ();

		if (atom == null) {
			closestAtom = FindClosestAtom ();
		}
	}

	private void HandleAtoms () {
		if (this.atom != null) {
			this.atom.GetComponent<Atom> ().makeRoot ();
			this.atom.transform.position = this.transform.position;
			this.atom.transform.rotation = transform.rotation;
		}
	}

	private void HandleKeyPresses () {
		float translationSpeed = 0.1f;
		float rotationSpeed = 3.0f;

		// Translation
		if (!Input.GetKey ("space")) {
			if (Input.GetKey (commands [0])) {
				Translate (forward * translationSpeed);
			}
			if (Input.GetKey (commands [1])) {
				Translate (-forward * translationSpeed);
			}
			if (Input.GetKey (commands [2])) {
				Translate (right * translationSpeed);
			}
			if (Input.GetKey (commands [3])) {
				Translate (-right * translationSpeed);
			}
			if (Input.GetKey (commands [4])) {
				Translate (up * translationSpeed);
			}
			if (Input.GetKey (commands [5])) {
				Translate (-up * translationSpeed);
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

	private void Translate (Vector3 vector) {
		transform.position += vector;
	}

	private List<String> LeftHandCommands() {
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

	private List<String> RightHandCommands() {
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
