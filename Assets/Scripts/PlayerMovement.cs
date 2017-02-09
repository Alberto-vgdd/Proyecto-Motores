using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	// Controla el movimiento del jugador, todas las fisicas de movimiento se basan en un RigidBody, 
	// es necesario que el objeto al que se le coloque este script lo tenga.
	// Los valores recomendados para CarHeight son:
	// hoverForce: 30
	// hoverHeight: 0.45f
	// groundedThs: 0.2f

	private Rigidbody rb;

	[Header("Friction strenght")]
	[Range(0.5f, 0.98f)]
	public float frictionFactor;

	[Header("Input Variables")]
	public float forwInput;
	public float turnInput;

	[Header("Car properties")]
	[Range(3,10)]
	public float turnRate;
	[Range(1,5)]
	public float acceleration;
	[Range(5,20)]
	public float maxFwdSpeed;
	[Range(-2,-0.5f)]
	public float maxBwdSpeed;
	[Range(3,10)]
	public float driftStrenght;
	[Range(10,60)]
	public float maxDrift;
	[Range(0.5f,2)]
	public float driftSpeedLoss;
	[Range(0.5f,8)]
	public float driftStabilization;

	private Vector3 resetPosition = new Vector3(0,3,0);
	private float turnMultiplier = 0;

	[Header("Car Height Adjustment(DONT TOUCH)")]
	public float hoverHeight = 0.45f;
	public float hoverForce = 30;
	public float groundedThs = 0.2f;

	[Header("Debug info")]
	public bool grounded;
	public bool drifting;
	public float accumulatedAcceleration;
	public float ungroundedTime;
	public float timeBelowThs;
	public float driftDegree;

	public Vector3 positionOnLastUpdate;
	public Vector3 positionOnCurrentUpdate;

	void Start()
	{
		rb = GetComponent<Rigidbody> ();
		rb.velocity = new Vector3 (0, 0, 0);
		driftDegree = 0;
	}
	void Update()
	{
		forwInput = Input.GetAxis ("Vertical");
		turnInput = Input.GetAxis ("Horizontal");
		if (Input.GetKeyDown (KeyCode.Space))
			drifting = true;
	}
	void FixedUpdate()
	{
		positionOnLastUpdate = positionOnCurrentUpdate;
		positionOnCurrentUpdate = transform.position;
		Hover ();
		if (!grounded) {
			MoveTowardsZeroRotation ();
			driftDegree = 0; drifting = false;
			forwInput = turnInput = 0;
			if (ungroundedTime > 5)
				ResetCar();
		}

		accumulatedAcceleration += forwInput * Time.fixedDeltaTime * 3;
		accumulatedAcceleration = Mathf.MoveTowards (accumulatedAcceleration, 0, Time.fixedDeltaTime);
		accumulatedAcceleration = Mathf.Clamp (accumulatedAcceleration, maxBwdSpeed, maxFwdSpeed);

		MoveFwd ();
		MoveTrn ();

	}
	void Hover()
	{
		Ray ray = new Ray (transform.position, -transform.up);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, hoverHeight)) {
			ungroundedTime = 0;
			float proportionalHeight = (hoverHeight - hit.distance) / hoverHeight;
			Vector3 appliedHoverForce = Vector3.up * proportionalHeight * hoverForce;
			rb.AddForce (appliedHoverForce, ForceMode.Acceleration);
		} else {
			ungroundedTime += Time.fixedDeltaTime;
		}
		if (ungroundedTime > groundedThs) {
			grounded = false;
		} else {
			grounded = true;
		}
	}
	void MoveFwd()
	{
		accumulatedAcceleration = Mathf.MoveTowards(accumulatedAcceleration, 0, ((Mathf.Abs(driftDegree)*driftSpeedLoss) / 20) * Time.fixedDeltaTime);
		rb.MovePosition(transform.TransformPoint( (Quaternion.Euler(0,-driftDegree,0) * Vector3.forward * accumulatedAcceleration * Time.fixedDeltaTime * acceleration)));
		if (!grounded)
			return;
		if (Mathf.Abs(accumulatedAcceleration) < 1) {
			EndDrift();
		}
	}
	void MoveTrn()
	{
		turnMultiplier = Mathf.Clamp (accumulatedAcceleration, -1,1);
		transform.Rotate (new Vector3(0,((turnInput*0.7f) + driftDegree/20) * turnRate * 10 * Time.fixedDeltaTime * turnMultiplier,0));
		if (drifting) {
			driftDegree += turnInput * Time.fixedDeltaTime * driftStrenght * 10;
			driftDegree = Mathf.Clamp (driftDegree, -maxDrift,maxDrift);
			if (turnInput == 0) {
				driftDegree = Mathf.MoveTowards (driftDegree, 0, Time.fixedDeltaTime * driftStabilization * 10);
				if (driftDegree == 0)
					EndDrift ();
			}
		}
	}
	void MoveTowardsZeroRotation()
	{
		Vector3 currentRotation = transform.rotation.eulerAngles;
		transform.rotation = Quaternion.Euler (Mathf.MoveTowardsAngle (currentRotation.x, 0, Time.fixedDeltaTime * 100), currentRotation.y, Mathf.MoveTowardsAngle (currentRotation.x, 0, Time.fixedDeltaTime * 100));
	}
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "checkpoint") {
			StageData.currentData.CrossCheckPoint ();
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
	void EndDrift()
	{
		drifting = false;
		driftDegree = 0;
	}
	void ResetCar()
	{
		ungroundedTime = 0;
		accumulatedAcceleration = 0;
		turnInput = 0;
		forwInput = 0;
		transform.position = resetPosition;
		transform.rotation = Quaternion.Euler (Vector3.zero);
		rb.angularVelocity = Vector3.zero;
		rb.velocity = Vector3.zero;
	}
}
