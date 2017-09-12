using UnityEngine;
using System.Collections;

public class FollowTarget : MonoBehaviour {

	public static FollowTarget currentInstance;

	public GameObject cameraTarget;
	public Transform firstPersonCamPosition;
	public PlayerMovement pm;
	private Vector3 targetPos;

	private const float MIN_FOV = 40;
	private const float MAX_FOV = 90;
	private const float SPEED_TO_FOV = 0.9f;
	private const float DRIFT_DEGREE_MULTIPLIER = 0.1f;
	private const float DISTANCE_FARCAM = 2.85f;
	private const float DISTANCE_CLOSECAM = 2.65f;
	private const float DISTANCE_PRERACE = 4.1f;
	private const float HEIGHT_FARCAM = 1.9f;
	private const float HEIGHT_CLOSECAM = 1.65f;
	private const float HEIGHT_PRERACE = 0;
	private const float TILT_SPEED = 15;
	private const float TILT_EFFECT_MULTIPLIER = 0.15f;
	private const float CAMERA_TURN_SPEED = 200f;

	private float camDegree = 0;
	private float camDistance = 0;
	private float camHeight = 0;
	private bool camRaceMode = false;
	private float camDegreeTemp = 0;
	private float camDegreeRads = 0;
	private float camT = 0;
	private float camCos = 0;
	private float camSin = 0;
	private float tiltCurrent = 0;

	enum CamMode
	{
		CloseCam,
		FarCam,
		FirstPerson
	}
	private CamMode currentCameraMode;

	void Awake()
	{
		if (currentInstance == null) {
			DontDestroyOnLoad (this.gameObject);
			currentInstance = this;
			//InitializeData ();
		}
		else {
			Destroy (this.gameObject);
		}
	}

	void Start () {
		pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement> ();
		Camera.main.useOcclusionCulling = false;
		StartCoroutine ("PreRaceAnimation");
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.C) && camRaceMode) {
			ChangeCameraMode ();
		}
	}
	void FixedUpdate () {
		if (camRaceMode) {
			LookAtTarget ();
			UpdateFov ();
			if (currentCameraMode == CamMode.FirstPerson) {
				transform.position = firstPersonCamPosition.position;
				transform.rotation = firstPersonCamPosition.rotation;
			} else {
				SphericalPositionLock ();
			}
		}
	}

	void LookAtTarget()
	{
        transform.LookAt (cameraTarget.transform.position+new Vector3(0f,1f,0f));
		tiltCurrent = Mathf.MoveTowards (tiltCurrent, TILT_EFFECT_MULTIPLIER * pm.GetDriftDegree(), Time.deltaTime * TILT_SPEED);
		transform.Rotate (Vector3.forward * tiltCurrent);
		camDegree = 270 - cameraTarget.transform.rotation.eulerAngles.y + pm.GetDriftDegree() * DRIFT_DEGREE_MULTIPLIER;
	}
	void SphericalPositionLock()
	{
		camDegreeTemp = Mathf.MoveTowardsAngle (camDegreeTemp, camDegree, Time.smoothDeltaTime * CAMERA_TURN_SPEED);
		camDegreeRads = camDegreeTemp * Mathf.Deg2Rad;
		camCos = Mathf.Cos (camDegreeRads);
		camSin = Mathf.Sin (camDegreeRads);

		transform.position = new Vector3 (camCos * camDistance, camHeight, camSin * camDistance) + cameraTarget.transform.position;
	}
	void UpdateFov()
	{
		GetComponent<Camera> ().fieldOfView = Mathf.Clamp(90 + pm.GetCurrentSpeed() * SPEED_TO_FOV, MIN_FOV, MAX_FOV );
	}
	void ChangeCameraMode()
	{
		switch (currentCameraMode) {
			case CamMode.FarCam:
			{
				camHeight = HEIGHT_CLOSECAM;
				camDistance = DISTANCE_CLOSECAM;
				currentCameraMode = CamMode.CloseCam;
				break;
			}
			case CamMode.CloseCam:
			{
				camHeight = 0;
				camDistance = 0;
				currentCameraMode = CamMode.FirstPerson;
				break;
			}
			case CamMode.FirstPerson:
			{
				camHeight = HEIGHT_FARCAM;
				camDistance = DISTANCE_FARCAM;
				currentCameraMode = CamMode.FarCam;
				LookAtTarget ();
				camDegreeTemp = camDegree;
				SphericalPositionLock ();
				break;
			}
		}
	}
	public void SetRaceMode()
	{
		camDistance = DISTANCE_FARCAM;
		camHeight = HEIGHT_FARCAM;
		currentCameraMode = CamMode.FarCam;
		camRaceMode = true;
	}
	IEnumerator PreRaceAnimation()
	{
		camHeight = HEIGHT_PRERACE;
		camDistance = DISTANCE_PRERACE;

		while (!camRaceMode) {
			LookAtTarget ();
			transform.Rotate (new Vector3(0,-32.5f,20));
			camDegree = 240f;

			camT = (1 - (Mathf.Abs (camDegreeTemp) / Mathf.Abs (camDegree))) - 0.5f;
			if (Mathf.Abs (camT) < 0.005f)
				camT = 0;
			camDegreeTemp = Mathf.MoveTowardsAngle (camDegreeTemp, camDegree, Time.smoothDeltaTime * camT * 300f);
			//camDegreeTemp = camDegree;
			camDegreeRads = camDegreeTemp * Mathf.Deg2Rad;
			camCos = Mathf.Cos (camDegreeRads);
			camSin = Mathf.Sin (camDegreeRads);

			transform.position = new Vector3 (camCos * camDistance, camHeight, camSin * camDistance) + cameraTarget.transform.position;
			yield return null;
		}
	}
}
