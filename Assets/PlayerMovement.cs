using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	private Rigidbody rb;

	[Header("Friction strenght")]
	[Range(0.5f, 0.98f)]
	public float frictionFactor;

	[Header("Input Variables")]
	public float forwInput;
	public float turnInput;

	[Header("Car properties")]
	public float turnRate;
	public float acceleration;
	public float maxFwdSpeed;
	public float maxBwdSpeed;
	public float driftStrenght;
	public float maxDrift;
	public float driftSpeedLoss;

	public float hoverHeight = 0.45f;
	public float hoverForce = 30;
	private Vector3 resetPosition = new Vector3(0,3,0);
	private float turnMultiplier = 0;

	[Header("Debug info")]
	public bool grounded;
	public bool drifting;
	public float cameraFov;
	public float accumulatedAcceleration;
	public float ungroundedTime;
	public float timeBelowThs;
	public float driftDegree;

	void Start()
	{
		rb = GetComponent<Rigidbody> ();
		rb.velocity = new Vector3 (0, 0, 0);
		driftDegree = 0;

		cameraFov = 90;
	}
	void Update()
	{
		forwInput = Input.GetAxis ("Accelerate") - Input.GetAxis("Brake");
		turnInput = Input.GetAxis ("Steer");

		if (Input.GetButton ("Hand Brake"))
			drifting = true;
		else {
			drifting = false;
			driftDegree = 0;
		}

		Hover ();
		if (!grounded) {
			driftDegree = 0; drifting = false;
			forwInput = turnInput = 0;
			ungroundedTime += Time.deltaTime;
			if (ungroundedTime > 7)
				ResetCar();
		} else {
			ungroundedTime = 0;
		}

		accumulatedAcceleration += forwInput * Time.deltaTime * 3;
		accumulatedAcceleration = Mathf.MoveTowards (accumulatedAcceleration, 0, Time.deltaTime);
		accumulatedAcceleration = Mathf.Clamp (accumulatedAcceleration, maxBwdSpeed, maxFwdSpeed);

		MoveFwd ();
		MoveTrn ();

	}
	void Hover()
	{
		Ray ray = new Ray (transform.position, -transform.up);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, hoverHeight)) {
			grounded = true;
			float proportionalHeight = (hoverHeight - hit.distance) / hoverHeight;
			Vector3 appliedHoverForce = Vector3.up * proportionalHeight * hoverForce;
			rb.AddForce (appliedHoverForce, ForceMode.Acceleration);
		} else
			grounded = false;
	}
	void MoveFwd()
	{
		accumulatedAcceleration = Mathf.MoveTowards(accumulatedAcceleration, 0, (driftDegree*driftSpeedLoss) / 20 * Time.deltaTime);
		transform.Translate (Quaternion.Euler(0,-driftDegree,0) * Vector3.forward * accumulatedAcceleration * Time.deltaTime * acceleration);
	}
	void MoveTrn()
	{
		turnMultiplier = Mathf.Clamp (accumulatedAcceleration, -1,1);
		transform.Rotate (new Vector3(0,(turnInput + driftDegree/20) * turnRate * Time.deltaTime * turnMultiplier,0));
		if (drifting) {
			driftDegree += turnInput * Time.deltaTime * driftStrenght * 10;
			driftDegree = Mathf.Clamp (driftDegree, -maxDrift,maxDrift);
			driftDegree = Mathf.MoveTowards (driftDegree, 0, Time.deltaTime * 40);
		}
	}
	void OnCollisionStay(Collision other)
	{
		if (other.gameObject.tag == "wall") {
			print ("Collision: speed reduced from " + accumulatedAcceleration + " -> " + accumulatedAcceleration * frictionFactor);
			accumulatedAcceleration *= frictionFactor;
			drifting = false; driftDegree = 0;
		}
	}
	void ResetCar()
	{
		ungroundedTime = 0;
		accumulatedAcceleration = 0;
		turnInput = 0;
		forwInput = 0;
		transform.position = resetPosition;
		transform.rotation = Quaternion.Euler (Vector3.zero);
		rb.velocity = Vector3.zero;
	}
}
