using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atom : MonoBehaviour {

	public GameObject rhand;
	public GameObject lhand;
	public Material blue;
	public Material yellow;
	public Material red;
	private Transform rht;
	private Transform lht;
	private float proximity;

	// Use this for initialization
	void Start () {
		rht = rhand.GetComponent<Transform> ();
		lht = lhand.GetComponent<Transform> ();
		proximity = 2.5f;
	}
	
	// Update is called once per frame
	void Update () {

		//Test distance between atom and hand
		if (Vector3.Distance (rht.position, transform.position) < proximity) {
			GetComponent<Renderer> ().material = yellow;
			//get the atom
			Hand hstate = rhand.GetComponent<Hand>();
			if (hstate.pinch && (hstate.atom == null || hstate.atom == gameObject)) {
				GetComponent<Renderer> ().material = red;
				transform.position = rht.position;
				transform.rotation = rht.rotation;
				hstate.atom = gameObject;
			//release the atom
			} else {
				hstate.atom = null;
			}
		} else {
			GetComponent<Renderer> ().material = blue;
		}
	}
}
