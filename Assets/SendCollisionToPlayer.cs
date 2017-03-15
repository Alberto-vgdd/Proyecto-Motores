using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendCollisionToPlayer : MonoBehaviour {

	public PlayerMovement pm;
	public string CollisionSide;

	void OnTriggerStay(Collider other)
	{
		pm.SendCollisionStayFrom (CollisionSide);
	}
	void OnTriggerEnter(Collider other)
	{
		pm.SendCollisionEnterFrom (CollisionSide);
	}
	void OnTriggerExit(Collider other)
	{
		if (CollisionSide == "GROUND")
			pm.groundedHitbox = false;
	}
}
