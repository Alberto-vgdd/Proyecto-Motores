using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuspensionTiltSimulation : MonoBehaviour {

	public PlayerMovement pm;

	private float tiltTargetValue = 0;
	private float currentTiltValue = 0;
	private float tiltSpeed = 5;
	private float tiltMultiplier = 0.115f;
	
	// Update is called once per frame
	void Update () {
		if (pm.IsGrounded ()) {
			tiltTargetValue = ((-pm.GetDriftFwdDegree ()) + (pm.GetTurnInput() * 10)) * tiltMultiplier * Mathf.Abs(pm.GetCurrentSpeed()/30);
		} else {
			tiltTargetValue = 0;
		}

		currentTiltValue = Mathf.MoveTowards(currentTiltValue, tiltTargetValue, Time.deltaTime * tiltSpeed);
		transform.localRotation = Quaternion.Euler (0, 0, currentTiltValue);
	}
}
