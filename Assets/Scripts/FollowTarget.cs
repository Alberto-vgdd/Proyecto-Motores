using UnityEngine;
using System.Collections;

public class FollowTarget : MonoBehaviour {

	// Sigue al objetivo, IMPORTANTE: EL OBJETIVO DEBE TENER DISPLACEWITHDRIFT.
	// PM (PlayerMovement) buscara automaticamente al objeto marcado como jugador en la escena.

	// TODO: Ajustar el FOV.

	public GameObject target;
	private Vector3 targetPos;
	private PlayerMovement pm;

	[Header("Positioning Parameters")]
	public float camDegree;
	[Range(0.1f, 2)]
	public float driftDegreeMultiplier;
	[Range(0,12)]
	public float camDistance;
	[Range(0,10)]
	public float camHeight;
	[Header("FOV Parameters")]
	[Range(40, 70)]
	public float minFov;
	[Range(70, 100)]
	public float maxFov;
	[Range(0.1f, 1)]
	public float speedToFov;

	// Variables auxiliares para calculos.

	private float camDegreeTemp;
	private float camDegreeRads;
	private float camCos;
	private float camSin;
	private float tiltSpeed = 15;
	private float tiltMultiplier = 0.125f;
	private float tiltCurrent = 0;
	private float camTurnSpeed = 100f;

	void Start () {
		camDegreeTemp = 0;
		// TODO: Eh...solucion spaghetti, cambiar esto.
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
        transform.LookAt (target.transform.position+new Vector3(0f,1f,0f));
		tiltCurrent = Mathf.MoveTowards (tiltCurrent, tiltMultiplier * pm.driftDegree, Time.deltaTime * tiltSpeed);
		transform.Rotate (Vector3.forward * tiltCurrent);
	}
	void sphericalPositionLock()
	{
		camDegreeTemp = Mathf.MoveTowardsAngle(camDegreeTemp, camDegree, Time.smoothDeltaTime*camTurnSpeed);
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
