using UnityEngine;
using System.Collections;

public class FollowTarget : MonoBehaviour {

	// Sigue al objetivo, IMPORTANTE: EL OBJETIVO DEBE TENER DISPLACEWITHDRIFT.
	// PM (PlayerMovement) buscara automaticamente al objeto marcado como jugador en la escena.

	public GameObject target;
	private Vector3 targetPos;
	private PlayerMovement pm;

	[Header("Positioning Parameters")]
	public float camDegree;
	[Range(0.1f, 2)]
	public float driftDegreeMultiplier;
	[Range(4,12)]
	public float camDistance;
	[Range(0,10)]
	public float camHeight;
	[Header("FOV Parameters")]
	[Range(90, 100)]
	public float minFov;
	[Range(90, 100)]
	public float maxFov;
	[Range(0.1f, 1)]
	public float speedToFov;

	// Variables auxiliares para calculos.

	private float camDegreeTemp;
	private float camDegreeRads;
	private float camCos;
	private float camSin;

	void Start () {
		camDegreeTemp = 0;
		// Eh...solucion spaghetti.
		pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement> ();
	}

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
		camDegreeTemp = Mathf.MoveTowardsAngle(camDegreeTemp, camDegree, Time.smoothDeltaTime*200);
		camDegreeRads = camDegreeTemp * Mathf.Deg2Rad;
		camCos = Mathf.Cos (camDegreeRads);
		camSin = Mathf.Sin (camDegreeRads);

		transform.position = new Vector3 (camCos * camDistance, camHeight, camSin * camDistance) + target.transform.position;
	}
	void updateFov()
	{
		GetComponent<Camera> ().fieldOfView = Mathf.Clamp(90 + pm.accumulatedAcceleration * speedToFov, minFov, maxFov);
	}
}
