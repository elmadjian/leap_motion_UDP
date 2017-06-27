using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atom : MonoBehaviour {

	public GameObject rhand;
	public GameObject lhand;
	public Material blue;
	public Material yellow;
	public Material red;
	public float minimumProximity;

	private bool isBeingHeld;
	private bool isClosestToRightHand;
	private bool isClosestToLeftHand;

	private List<Atom> children;

	public static List<Atom> allAtoms = new List<Atom> ();

	// Use this for initialization
	void Start () {
		minimumProximity = 2.5f;

		isBeingHeld = false;

		children = new List<Atom> ();

		allAtoms.Add (this);
	}

	public bool IsBeingHeld() {
		return isBeingHeld;
	}

	public void SetIsBeingHeld(bool newValue) {
		isBeingHeld = newValue;
		if (isBeingHeld) {
			GetComponent<Renderer> ().material = red;
		} else {
			UpdateHighlights ();
		}
	}

	public void SetIsClosest (bool isRightHand, bool isClosest) {
		if (isRightHand) {
			if (isClosestToRightHand != isClosest) {
				isClosestToRightHand = isClosest;
				UpdateHighlights ();
			}
		} else {
			if (isClosestToLeftHand != isClosest) {
				isClosestToLeftHand = isClosest;
				UpdateHighlights ();
			}
		}
	}

	void UpdateHighlights () {
		if (isBeingHeld) {
			return;
		}
			
		if (isClosestToLeftHand || isClosestToRightHand) {
			GetComponent<Renderer> ().material = yellow;
		} else {
			GetComponent<Renderer> ().material = blue;
		}
	}
}
