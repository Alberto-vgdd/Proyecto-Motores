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
	public float frictionFactor;										// Factor de friccion, cuando menor sea, mas velocidad se perdera al colisionar

	[Header("Input Variables")]
	public float forwInput;												// Almacena el input de acelerar/frenar
	public float turnInput;												// Almacena el input de giro

	[Header("Car properties")]
	[Range(3,10)]
	public float turnRate;												// Fuerza de giro (sin drift) del vehiculo
	[Range(1,5)]
	public float acceleration;											// Aceleracion del vehiculo
	[Range(5,20)]
	public float maxFwdSpeed;											// Velocidad maxima
	[Range(-2,-0.5f)]
	public float maxBwdSpeed;											// Velocidad maxima marcha atras
	[Range(3,10)]
	public float driftStrenght;											// Fuerza de derrape
	[Range(10,60)]
	public float maxDrift;												// Maximo angulo de derrape
	[Range(0.5f,2.5f)]
	public float driftSpeedLoss;										// Perdida de velocidad al derrapar
	[Range(0.5f,8)]
	public float driftStabilization;									// Auto-Estabilizacion del derrape

	private Vector3 savedResetPosition = new Vector3(0,3,0);			// Posicion de reset (respawn)
	private Quaternion savedResetRotation = Quaternion.identity;		// Rotacion de reset (respawn)
	private float turnMultiplier = 0;									// Auxiliar para evitar que se pueda girar a 0 km/h

	[Header("Advanced (DONT TOUCH)")]
	public float hoverHeight = 0.45f;
	public float hoverForce = 30;
	public float groundedThs = 0.2f;
	public float rotationCorrectionStrenght = 250;

	[Header("Debug info")]
	public bool grounded;												// Esta en el suelo?
	public bool drifting;												// Esta derrapando?
	public float accumulatedAcceleration;								// Velocidad
	public float ungroundedTime;										// Tiempo sin tocar el suelo
	public float driftDegree;											// Angulo de derrape

	void Start()
	{
		rb = GetComponent<Rigidbody> ();
		rb.velocity = new Vector3 (0, 0, 0);
		driftDegree = 0;
	}

	// Los inputs del jugador son leidos en Update, mientras que las fisicas son procesadas en FixedUpdate, para asi mejorar la respuesta
	// de los controles y evitar que se pierdan inputs.

	void Update()
	{
		// Si el jugador ha destruido su vehiculo, ignoramos todos los inputs.
		if (StageData.currentData.playerHealth <= 0) {
			forwInput = 0;
			turnInput = 0;
			return;
		}
		// Si el tiempo se ha agotado, solo escuchamos el input de giro.
		if (StageData.currentData.remainingSec <= 0) {
			forwInput = 0;
			turnInput = Input.GetAxis ("Horizontal");
			if (Input.GetKeyDown (KeyCode.Space))
				drifting = true;
			return;
		}
		// Leemos todos los inputs.
		if (Input.GetKeyDown (KeyCode.R)) {
			ResetCar ();
		}
		forwInput = Input.GetAxis ("Vertical");
		turnInput = Input.GetAxis ("Horizontal");
		if (Input.GetKeyDown (KeyCode.Space))
			drifting = true;
	}

	// Procesa las fisicas del coche.

	void FixedUpdate()
	{
		Hover ();
		if (!grounded) {
			MoveTowardsZeroRotation ();
			driftDegree = 0; drifting = false;
			forwInput = turnInput = 0;
			if (ungroundedTime > 5)
				ResetCar();
		}
			
		if (forwInput < 0 && accumulatedAcceleration > 0) {
			accumulatedAcceleration += forwInput * Time.fixedDeltaTime * 6;
		} else {
			accumulatedAcceleration += forwInput * Time.fixedDeltaTime * 3;
		}
		accumulatedAcceleration = Mathf.MoveTowards (accumulatedAcceleration, 0, Time.fixedDeltaTime);
		accumulatedAcceleration = Mathf.Clamp (accumulatedAcceleration, maxBwdSpeed, maxFwdSpeed);

		MoveFwd ();
		MoveTrn ();

	}

	// (Obsoleto/innecesario?) Simula la suspension

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

	// Simula la aceleracion.

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

	// Simula el giro/derrape

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

	// Asegura que el coche no pueda volcar.

	void MoveTowardsZeroRotation()
	{
		Vector3 currentRotation = transform.rotation.eulerAngles;
		transform.rotation = Quaternion.Euler (Mathf.MoveTowardsAngle (currentRotation.x, 0, Time.fixedDeltaTime * rotationCorrectionStrenght), currentRotation.y, Mathf.MoveTowardsAngle (currentRotation.x, 0, Time.fixedDeltaTime * 100));
	}

	// Administra la colision con triggers.

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "checkPointPasive") {
			savedResetPosition = other.transform.position;
			savedResetRotation = other.transform.rotation;
			other.GetComponent<Collider> ().enabled = false;
			MapGeneration.currentData.SpawnNode ();
			print ("CheckPoint");
			StageData.currentData.nodesCrossed++;
		} else if (other.gameObject.tag == "checkPointActive") {
			StageData.currentData.CrossCheckPoint ();
			savedResetPosition = other.transform.position;
			savedResetRotation = other.transform.rotation;
			other.GetComponent<Collider> ().enabled = false;
			MapGeneration.currentData.SpawnNode ();
			StageData.currentData.nodesCrossed++;
		} else if (other.gameObject.tag == "Respawn") {
			ResetCar ();
		}
	}

	// Administra la colision con muros.

	void OnCollisionStay(Collision other)
	{
		if (other.gameObject.tag == "wall") {
			print ("Collision: speed reduced from " + accumulatedAcceleration + " -> " + accumulatedAcceleration * frictionFactor);
			StageData.currentData.playerHealth -= Mathf.Clamp((accumulatedAcceleration-0.8f) * 0.1f, 0, 10);
			accumulatedAcceleration *= frictionFactor;
			drifting = false;
			driftDegree = 0;
		} 
	}

	// Termina el drift (llamado al colisionar o al no estar en el suelo)

	void EndDrift()
	{
		drifting = false;
		driftDegree = 0;
	}

	// Respawn (llamado al caer al vacio, o al ir en direccion contraria.

	void ResetCar()
	{
		ungroundedTime = 0;
		accumulatedAcceleration = 0;
		turnInput = 0;
		forwInput = 0;
		transform.position = savedResetPosition;
		transform.rotation = savedResetRotation;
		rb.angularVelocity = Vector3.zero;
		rb.velocity = Vector3.zero;
	}
}
