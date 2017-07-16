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

	private bool camRaceMode = false;
	private float camDegreeTemp;
	private float camDegreeRads;
	private float camT;
	private float camCos;
	private float camSin;
	private float tiltSpeed = 15;
	private float tiltMultiplier = 0.15f;
	private float tiltCurrent = 0;
	private float camTurnSpeed = 200f;
	private float camRaceModeHeight = 2.2f;
	private float camRaceModeDistance = 3.5f;

	void Start () {
		camDegreeTemp = 0;
		// TODO: Eh...solucion spaghetti, cambiar esto.
		pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement> ();
	}

	void FixedUpdate () {


		if (camRaceMode) {
			lookAtTarget ();
			camDegree = 270 - target.transform.rotation.eulerAngles.y + pm.GetDriftDegree() * driftDegreeMultiplier;
			sphericalPositionLock ();
			updateFov ();
		} else {
			lookAtTarget ();
			transform.Rotate (new Vector3(0,-32.5f,20));
			camDegree = 240f;
			sphericalPositionLock ();
		}

	}

	void lookAtTarget()
	{
        transform.LookAt (target.transform.position+new Vector3(0f,1f,0f));
		tiltCurrent = Mathf.MoveTowards (tiltCurrent, tiltMultiplier * pm.GetDriftDegree(), Time.deltaTime * tiltSpeed);
		transform.Rotate (Vector3.forward * tiltCurrent);
	}
	void sphericalPositionLock()
	{
		if (camRaceMode) {
			camDegreeTemp = Mathf.MoveTowardsAngle (camDegreeTemp, camDegree, Time.smoothDeltaTime * camTurnSpeed);
			camDegreeRads = camDegreeTemp * Mathf.Deg2Rad;
			camCos = Mathf.Cos (camDegreeRads);
			camSin = Mathf.Sin (camDegreeRads);

			transform.position = new Vector3 (camCos * camDistance, camHeight, camSin * camDistance) + target.transform.position;
		} else {
			// TODO: No spaghetti?
			camT = (1 - (Mathf.Abs (camDegreeTemp) / Mathf.Abs (camDegree))) - 0.5f;
			if (Mathf.Abs (camT) < 0.005f)
				camT = 0;
			camDegreeTemp = Mathf.MoveTowardsAngle (camDegreeTemp, camDegree, Time.smoothDeltaTime * camT * 300f);
			//camDegreeTemp = camDegree;
			camDegreeRads = camDegreeTemp * Mathf.Deg2Rad;
			camCos = Mathf.Cos (camDegreeRads);
			camSin = Mathf.Sin (camDegreeRads);

			transform.position = new Vector3 (camCos * camDistance, camHeight, camSin * camDistance) + target.transform.position;
		}

	}
	void updateFov()
	{
		GetComponent<Camera> ().fieldOfView = Mathf.Clamp(90 + pm.GetCurrentSpeed() * speedToFov, minFov, maxFov);
	}
	public void SetRaceMode()
	{
		StartCoroutine ("changeToRaceMode");
		camRaceMode = true;
	}
	IEnumerator changeToRaceMode()
	{
		while (camHeight != camRaceModeHeight || camDistance != camRaceModeDistance) {
			camHeight = Mathf.MoveTowards (camHeight, camRaceModeHeight, Time.deltaTime * 2f);
			camDistance = Mathf.MoveTowards (camDistance, camRaceModeDistance, Time.deltaTime * 2f);
			yield return null;
		}
	}
}
