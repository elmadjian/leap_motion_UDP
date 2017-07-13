using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atom : MonoBehaviour {

	public GameObject rhand;
	public GameObject lhand;
	public GameObject molecule;
	public Material selected;
	public Material unselected;
	private float proximity;
	private Behaviour halo;

	// Use this for initialization
	void Start () {
		proximity = 3f;
		halo = (Behaviour)GetComponent ("Halo");
		halo.enabled = false;
	}

	// Update is called once per frame
	void Update () {
		bool rhandState = Manipulate (rhand);
		bool lhandState = Manipulate (lhand);
		if (!rhandState && !lhandState) {
			GetComponent<Renderer> ().material = unselected;
			halo.enabled = false;
		}
	}

	private bool Manipulate(GameObject hand) {
		Transform ht = hand.GetComponent<Transform> ();
		//check distance
		if (Vector3.Distance (ht.position, transform.position) < proximity) {
			halo.enabled = true;
			//GetComponent<Renderer>().material = selected;
			Hand hstate = hand.GetComponent<Hand> ();
			Molecule mol = molecule.GetComponent<Molecule> ();
			//get the atom
			if (hstate.pinch && (hstate.atom == null || hstate.atom == gameObject) && !mol.selected) {
				GetComponent<Renderer> ().material = selected;
				//transform.position = ht.position;
				//transform.rotation = ht.rotation;
				hstate.atom = gameObject;
				//release the atom
			} else {
				hstate.atom = null;
			}
			return true;
		}
		return false;
	}
		
//	void OnTriggerEnter(Collider col) {
//		if (col.tag == "atom") {
//			GameObject mol = col.gameObject.GetComponent<Atom> ().molecule;
//
//			//there's no molecule yet
//			if (mol == null) {
//				mol = new GameObject ();
//				mol.name = "Molecule";
//				col.gameObject.transform.SetParent (mol.GetComponent<Transform>());
//				col.gameObject.GetComponent<Atom> ().molecule = mol;
//			}
//
//			//make the atom join the molecule
//			if (molecule != null) {
//				transform.SetParent (mol.GetComponent<Transform>());
//				molecule = mol;
//			}
//		}
//	}
}
