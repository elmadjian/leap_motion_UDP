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
	private bool isBeingMoved;

	private List<Atom> children;

	public static List<Atom> allAtoms = new List<Atom> ();

	// Use this for initialization
	void Start () {
		minimumProximity = 2.5f;
		isBeingMoved = false;

		children = new List<Atom> ();

		allAtoms.Add (this);
	}
}
