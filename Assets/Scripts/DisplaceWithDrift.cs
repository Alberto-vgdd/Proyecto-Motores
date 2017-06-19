using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplaceWithDrift : MonoBehaviour {

	// Administra el desplazamiento del objetivo de la camara cuando el jugador este derrapando.

	[Range(0.5f, 3f)]
	public float displaceSpeed;
	[Range(0.1f, 1)]
	public float displaceMultiplier;
	[Range(-1, 1)]
	public float frontalOffset;
	private float displacementCurrent;
	private float displacementTarget;

	private PlayerMovement pm;

	void Start () {
		displacementCurrent = 0;
		pm = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerMovement>();
	}

	void FixedUpdate () {
		displacementTarget = pm.driftDegree * displaceMultiplier / 10;
		displacementCurrent = Mathf.MoveTowards (displacementCurrent, displacementTarget, Time.fixedDeltaTime * displaceSpeed);
		transform.localPosition = displacementCurrent * Vector3.right + Vector3.forward * frontalOffset;
		
	}
}
