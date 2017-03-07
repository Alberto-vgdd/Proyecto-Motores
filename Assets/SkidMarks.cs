using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkidMarks : MonoBehaviour {

	public GameObject objTR_L;
	public GameObject objTR_R;

	private TrailRenderer TR_L;
	private TrailRenderer TR_R;
	public PlayerMovement pm;

	// Use this for initialization
	void Start () {
		TR_L = objTR_L.GetComponent<TrailRenderer> ();
		TR_R = objTR_R.GetComponent<TrailRenderer> ();
		
	}
	
	// Update is called once per frame
	void Update () {
		if (pm.drifting && pm.grounded) {
			TR_R.time = 4;
			TR_L.time = 4;
		} else {
			TR_R.time = 0;
			TR_L.time = 0;
		}

	}
}
