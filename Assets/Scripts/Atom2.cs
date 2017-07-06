using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Always move realtive to root atom
// TODO: Allow removing atoms
// TODO: Create visible connections

public class Atom2 : MonoBehaviour {

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

	private Atom2 parent;
	private List<Atom2> children;

	public static List<Atom2> allAtoms = new List<Atom2> ();

	// Use this for initialization
	void Start () {
		minimumProximity = 2.5f;

		isBeingHeld = false;

		children = new List<Atom2> ();

		allAtoms.Add (this);
	}

	public Atom2 rootAtom() {
		if (parent == null) {
			return this;
		}

		return parent.rootAtom ();
	}

	public void makeRoot() {
		if (parent == null) {
			return;
		}

		parent.setParent (this);

		parent = null;
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
		Atom2 otherAtom = getOtherHeldAtom ();
		if (otherAtom == null) {
			return;
		}

		float distance = Vector3.Distance (transform.position, otherAtom.transform.position);
		if (distance <= minimumConnectionProximity) {
			otherAtom.setParent (this);
		}
	}

	private void setParent(Atom2 newParent) {
		if (parent != null) {
			if (parent.gameObject.GetInstanceID () != newParent.gameObject.GetInstanceID ()) {
				return;
			} else {
				parent.setParent (this);
			}
		}
		this.parent = newParent;
		newParent.transform.parent = null;
		this.transform.SetParent (newParent.transform, true);
		newParent.children.Add (this);
		this.children.Remove (newParent);
	}

	private Atom2 getOtherHeldAtom () {
		Atom2 otherAtom = null;
		if (rhand.atom != null && (rhand.atom.GetInstanceID () != this.gameObject.GetInstanceID ())) {
			print ("Other is right");
			print (rhand.atom);
			print (rhand.atom.GetInstanceID ());
			print (this.GetInstanceID ());
			otherAtom = rhand.atom.GetComponent<Atom2> ();
		} else if (lhand.atom != null && (lhand.atom.GetInstanceID () != this.gameObject.GetInstanceID ())) {
			print ("Other is left");
			print (lhand.atom);
			print (this);
			print (lhand.atom.GetInstanceID ());
			print (this.GetInstanceID ());
			otherAtom = lhand.atom.GetComponent<Atom2> ();
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
