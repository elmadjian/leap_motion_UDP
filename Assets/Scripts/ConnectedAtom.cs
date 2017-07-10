using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectedAtom : MonoBehaviour {

	public GameObject rhand;
	public GameObject lhand;
	public GameObject connection;
	public Material selected;
	public Material unselected;
	//public bool fixedAtom;
	private float proximity;
	private Behaviour halo;


	// Use this for initialization
	void Start () {
		proximity = 3.8f;
		halo = (Behaviour)GetComponent ("Halo");
		halo.enabled = false;
		//connection.SetActive (false);
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
			Hand hstate = hand.GetComponent<Hand> ();

			//get the atom
			if (hstate.pinch && (hstate.atom == null || hstate.atom == gameObject)) {
				GetComponent<Renderer> ().material = selected;
				hstate.atom = gameObject;

				//it's not a carbon
				transform.position = ht.position;
				transform.rotation = ht.rotation;
				GameObject[] carbons = GameObject.FindGameObjectsWithTag ("carbon");
				bool connected = false;
				foreach (GameObject carbon in carbons) {
					if (Vector3.Distance (transform.position, carbon.transform.position) < 2 * proximity) {
						CreateConnection (gameObject, carbon);
						connected = true;
					}
				}
				if (!connected)
					connection.SetActive (false);


				//release the atom
			} else {
				hstate.atom = null;
			}
			return true;
		}
		return false;
	}

	private void CreateConnection(GameObject A, GameObject B) {
		Vector3 start = A.transform.position;
		Vector3 end = B.transform.position;
		connection.transform.position = (end - start) / 2.0f + start;
		connection.transform.rotation = Quaternion.FromToRotation (Vector3.up, end - start);
		connection.SetActive (true);
	}
}
