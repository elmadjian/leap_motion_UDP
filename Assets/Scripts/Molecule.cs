using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Molecule : MonoBehaviour {

	public GameObject rightHand;
	public GameObject leftHand;
	public bool rightPinch;
	public bool leftPinch;
	public bool selected;
	private float proximity;
	private Quaternion rotation;

	// Use this for initialization
	void Start () {
		rightPinch = false;
		leftPinch = false;
		proximity = 14.0f;
		rotation = transform.rotation;
		selected = false;
	}

	// Update is called once per frame
	void Update () {
		Hand lhstate = leftHand.GetComponent<Hand> ();
		Hand rhstate = rightHand.GetComponent<Hand> ();
		rightPinch = rhstate.pinch;
		leftPinch = lhstate.pinch;
		if ((rightPinch || leftPinch) && !(rightPinch && leftPinch)) {
			selected = true;
			Transform rht = rightHand.GetComponent<Transform> ();
			Transform lht = leftHand.GetComponent<Transform> ();
			if (Vector3.Distance (rht.position, transform.position) < proximity) {
				transform.position = rht.position;
				transform.rotation = rht.rotation * rotation;
			} else if (Vector3.Distance (transform.position, lht.position) < proximity) {
				transform.position = lht.position;
				transform.rotation = lht.rotation * rotation;
			} else {
				rotation = transform.rotation;
			}
		}
		selected = false;
	}
}
