using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum State {Selectable, LeavingAtom, Attachable, RecentlyAttached}; 

public class ConnectedAtom : MonoBehaviour {

	public GameObject rhand;
	public GameObject lhand;
	public GameObject connection;
	public Material selected;
	public Material unselected;
	//public bool fixedAtom;
	private float proximity;
	private Behaviour halo;
	private State state = State.Selectable;

	// Use this for initialization
	void Start () {
		proximity = 2.5f;
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

		if (this.state == State.RecentlyAttached) {
			Transform lht = lhand.GetComponent<Transform> ();
			Transform rht = rhand.GetComponent<Transform> ();
			if (Vector3.Distance (lht.position, transform.position) > proximity &&
			    Vector3.Distance (rht.position, transform.position) > proximity) {

				this.state = State.Selectable;
				print ("Recently Attached -> Selectable");
			}
		}
	}

	private bool Manipulate(GameObject hand) {
		Transform ht = hand.GetComponent<Transform> ();

		if (this.state == State.RecentlyAttached) {
			return false;
		}

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

				if (hstate.isMarkerBased) {
					if (this.state == State.Selectable) {
						if (connected) {
							// Hand picked atom up while close to other attachable atoms
							this.state = State.LeavingAtom;
							print ("Selectable -> Leaving Atom");
						} else {
							// Hand picked atom up while far to other attachable atoms
							this.state = State.Attachable;
							print ("Selectable -> Attachable");
						}
					} else if (this.state == State.LeavingAtom) {
						if (!connected) {
							// If we got far enough away from previously attached atoms
							this.state = State.Attachable;
							print ("Leaving Atom -> Attachable");
						}
					} else if (this.state == State.Attachable) {
						if (connected) {
							// If we got close enough to new attachable atoms
							this.state = State.RecentlyAttached;
							print ("Attachable -> Recently Attached");
							// attach the atom automatically
							print ("Attaching!");
							hstate.atom = null;
							return true;
						}
					}
				}

				// No other connectable atoms are close
				if (!connected) {
					connection.SetActive (false);
				}

			} else {
				//release the atom
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
