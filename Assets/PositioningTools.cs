using UnityEngine;
using System.Collections;

public class PositioningTools : MonoBehaviour {
	
	public static int test()
	{
		return 0;
	}
	public static float trackFromFront(Vector3 current, Vector3 target)
	{
		float difX = target.x - current.x;
		float difY = target.y - current.y;
		if (difX == 0) {
			if (difY > 0) { return 90; } 
			else { return 270; }
		}
		float resultAngle = Mathf.Atan(difY / difX) * Mathf.Rad2Deg;
		if (difX > 0) { return resultAngle; } 
		else { return resultAngle+180; }
	}
	public static float trackFromTop(Vector3 current, Vector3 target)
	{
		float difX = target.x - current.x;
		float difZ = target.z - current.z;
		if (difX == 0) {
			if (difZ > 0) { return 90; } 
			else { return 270; }
		}
		float resultAngle = Mathf.Atan(difZ / difX) * Mathf.Rad2Deg;
		if (difX > 0) { return resultAngle; } 
		else { return resultAngle+180; }
	}
}
