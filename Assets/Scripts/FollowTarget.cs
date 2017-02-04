using UnityEngine;
using System.Collections;

public class FollowTarget : MonoBehaviour {

	public GameObject target;
	private Vector3 offset;
	private Vector3 camSpeed = Vector3.one * 4;
	private Vector3 targetPos;
	private PlayerMovement pm;
	// Use this for initialization
	void Start () {
		offset = new Vector3 (0, 1, -2);
		pm = target.GetComponent<PlayerMovement> ();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		GetComponent<Camera> ().fieldOfView = pm.cameraFov;
		targetPos = target.transform.TransformPoint (offset);
		//transform.position = Vector3.MoveTowards (transform.position, targetPos, 30 *Time.deltaTime);
		transform.position = targetPos;
		transform.LookAt (target.transform);
	}
}
