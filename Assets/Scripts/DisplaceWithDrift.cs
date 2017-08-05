using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplaceWithDrift : MonoBehaviour {

	// Administra el desplazamiento del objetivo de la camara cuando el jugador este derrapando.


	[Range(-1, 1)]
	public float frontalOffset;

	private float displacementMultiplier = 1f;
	private float displacementSpeedMultiplier = 1f;
	private float displacementCurrent;
	private float displacementTarget;

	private PlayerMovement pm;

	void Start () {
		displacementCurrent = 0;
		pm = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerMovement>();
	}

	void FixedUpdate () {
		displacementTarget = pm.GetHorizontalCamDisplacementValue() * displacementMultiplier;
		displacementSpeedMultiplier = 0.125f + Mathf.Abs (displacementCurrent - displacementTarget) * 10f;
		displacementCurrent = Mathf.MoveTowards (displacementCurrent, displacementTarget, Time.fixedDeltaTime * displacementSpeedMultiplier);
		transform.localPosition = displacementCurrent * Vector3.right + Vector3.forward * frontalOffset;
		
	}
}
