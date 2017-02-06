using UnityEngine;
using System.Collections;

public class FollowTarget : MonoBehaviour {

	public GameObject target;
	private Vector3 offset;
	private Vector3 targetPos;
	private PlayerMovement pm;

	[Header("Camera Parameters")]
	public float camDegree;
	[Range(0.1f, 2)]
	public float driftDegreeMultiplier;
	[Range(4,12)]
	public float camDistance;
	[Range(0,10)]
	public float camHeight;

	private float camDegreeTemp;
	private float camDegreeRads;
	private float camCos;
	private float camSin;

	// Use this for initialization
	void Start () {
		camDegreeTemp = 0;
		offset = new Vector3 (0, 1, -2);
		pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {


		lookAtTarget ();
		sphericalPositionLock ();
		updateFov ();
		camDegree = 270 - target.transform.rotation.eulerAngles.y + pm.driftDegree * driftDegreeMultiplier;
	}

	void lookAtTarget()
	{
		transform.LookAt (target.transform.position);
	}
	void sphericalPositionLock()
	{
		camDegreeTemp = Mathf.MoveTowardsAngle(camDegreeTemp, camDegree, Time.smoothDeltaTime*250);
		camDegreeRads = camDegreeTemp * Mathf.Deg2Rad;
		camCos = Mathf.Cos (camDegreeRads);
		camSin = Mathf.Sin (camDegreeRads);

		transform.position = new Vector3 (camCos * camDistance, camHeight, camSin * camDistance) + target.transform.position;
	}
	void updateFov()
	{
		GetComponent<Camera> ().fieldOfView = pm.cameraFov;
	}
}
