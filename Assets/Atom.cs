using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atom : MonoBehaviour {

	public static float minimumConnectionProximity = 2.5f;

	public Hand rhand;
	public Hand lhand;
	public Material blue;
	public Material yellow;
	public Material red;
	public float minimumProximity;

	private bool isBeingHeld;
	private bool isClosestToRightHand;
	private bool isClosestToLeftHand;

	private Atom parent;
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
			attemptToCreateNewConnection ();
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

	private void attemptToCreateNewConnection () {
		Atom otherAtom = getOtherHeldAtom ();
		if (otherAtom == null) {
			return;
		}

		float distance = Vector3.Distance (transform.position, otherAtom.transform.position);
		if (distance <= minimumConnectionProximity) {
			otherAtom.setParent (this);
			otherAtom.transform.SetParent (this.transform, true);
			children.Add (otherAtom);
		}
	}

	private void setParent(Atom newParent) {
		if (parent != null) {
			parent.setParent (this);
			children.Add (parent);
		}
		this.parent = newParent;
		parent.transform.SetParent (this.transform, true);
	}

	private Atom getOtherHeldAtom () {
		Atom otherAtom = null;
		if (rhand.atom != null && (rhand.atom.GetInstanceID () != this.gameObject.GetInstanceID ())) {
			print ("Other is right");
			print (rhand.atom);
			print (rhand.atom.GetInstanceID ());
			print (this.GetInstanceID ());
			otherAtom = rhand.atom.GetComponent<Atom> ();
		} else if (lhand.atom != null && (lhand.atom.GetInstanceID () != this.gameObject.GetInstanceID ())) {
			print ("Other is left");
			print (lhand.atom);
			print (this);
			print (lhand.atom.GetInstanceID ());
			print (this.GetInstanceID ());
			otherAtom = lhand.atom.GetComponent<Atom> ();
		}
		return otherAtom;
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
