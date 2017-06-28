using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

	public GameObject rightHand;
	public GameObject leftHand;
	private Transform rht;
	private Transform lht;
	private Hand rh;
	private Hand lh;
	private Vector3 direction;
	private bool start;

	// Use this for initialization
	void Start () {
		rht = rightHand.GetComponent<Transform> ();
		lht = leftHand.GetComponent<Transform> ();
		rh = rightHand.GetComponent<Hand> ();
		lh = leftHand.GetComponent<Hand> ();
		start = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (rh.pinch && lh.pinch && lh.atom == null && rh.atom == null) {
			if (!start) {
				direction = lht.position - rht.position;
				start = true;
			}
			else {
				//horizontally rotating the world
				Vector3 currentPos = lht.position - rht.position;
				Vector3 normal = Vector3.Cross (direction, currentPos);
				float sign = Mathf.Sign (normal.y - currentPos.y);
				float angle = Vector3.Angle (currentPos, direction) * sign;
				transform.RotateAround (Vector3.zero, Vector3.up, angle * Time.deltaTime);

				//Globally scaling the world
				float mag = currentPos.magnitude - direction.magnitude;
				if (Mathf.Abs (mag) > 0.2f) {
					mag /= 500f;
					transform.localScale += new Vector3 (mag, mag, mag);
				}

			}
		} 
		else {
			start = false;
		}
	}
}
