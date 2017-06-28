using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Allow removing atoms

public class Atom : MonoBehaviour {

	public static float minimumConnectionProximity = 5;

	public GameObject link;
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

		link.transform.localPosition = new Vector3 (0, 1, 0);
		link.transform.localRotation = new Quaternion();
		link.GetComponent<Renderer> ().enabled = false;
	}

	public Atom rootAtom() {
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
		updateLink ();
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
			updateLink ();
		}
	}

	private void setParent(Atom newParent) {
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

		updateLink ();
	}

	private void updateLink() {
		if (parent == null) {
			link.GetComponent<Renderer> ().enabled = false;
		} else {
			link.GetComponent<Renderer> ().enabled = true;
			Vector3 relativePos = parent.transform.position - this.transform.position;
			Quaternion rotation = Quaternion.LookRotation(relativePos);
			link.transform.rotation = rotation;
			Vector3 position = rotation * new Vector3 (0, 0, 1);
			link.transform.localPosition = position;
		}
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

	private void UpdateHighlights () {
		if (isBeingHeld) {
			return;
		}
			
		if (isClosestToLeftHand || isClosestToRightHand) {
			GetComponent<Renderer> ().material = yellow;
		} else {
			GetComponent<Renderer> ().material = blue;
		}
	}

	void Update () {
		updateLink ();
	}
}
