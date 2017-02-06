using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplaceWithDrift : MonoBehaviour {

	public float displaceMultiplier;

	private PlayerMovement pm;
	// Use this for initialization
	void Start () {
		pm = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerMovement>();
	}
	
	// Update is called once per frame
	void Update () {
		transform.localPosition = pm.driftDegree * Vector3.right * displaceMultiplier;
		
	}
}
