using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendCollisionToPlayer : MonoBehaviour {

	public PlayerMovement pm;
	public string CollisionSide;

	void OnTriggerStay(Collider other)
	{
		if (other.tag != "Untagged")
			return;
		pm.SendCollisionStayFrom (CollisionSide);
	}
	void OnTriggerEnter(Collider other)
	{
		if (other.tag != "Untagged")
			return;
		pm.SendCollisionEnterFrom (CollisionSide);
	}
	void OnTriggerExit(Collider other)
	{
		if (other.tag != "Untagged")
			return;
		if (CollisionSide == "GROUND")
			pm.SetDetectingGrounded(false);
	}
}
