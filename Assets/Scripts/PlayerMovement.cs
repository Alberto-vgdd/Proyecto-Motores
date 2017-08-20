using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	// Controla el movimiento del jugador, para que el script funcione, el objeto necesita:
	// - RigidBody (No kinematico, sin constantes)
	// - Collider (Cubo)

	// Estadisticas del coche

	private float turnRate;												// Fuerza de giro (sin drift) del vehiculo
	private float acceleration;											// Aceleracion del vehiculo
	private float maxFwdSpeed;											// Velocidad maxima
	private float maxBwdSpeed;											// Velocidad maxima marcha atras
	private float driftStrenght;										// Fuerza de derrape
	private float maxDrift;												// Maximo angulo de derrape
	private float driftSpeedConservation;								// Conservacion de la velocidad derrapando
	private float driftStabilization;									// Auto-Estabilizacion del derrape
	private float speedFalloffReductionFwd;								// Velocidad a la que se empieza a dejar de acelerar, cuanto mas alto sea, mas tardara en activarse (hacia delante)
	private float speedFalloffReductionBwd;								// Velocidad a la que se empieza a dejar de acelerar, cuanto mas alto sea, mas tardara en activarse (hacia delante)

	// Parametros de estado

	private bool lockPreRaceRotation = true;
	private bool grounded;												// Esta en el suelo?
	private bool detectingGrounded;										// Detecta colision con el suelo?
	private bool drifting;												// Esta derrapando?
	private float forwInput;											// Almacena el input de acelerar/frenar
	private float turnInput;											// Almacena el input de giro
	private float accumulatedSpeed;										// Velocidad
	private float ungroundedTime;										// Tiempo sin tocar el suelo
	private float groundedTime;											// Tiempo tocando el suelo	
	private bool cleanAir;												// En el aire sin colisionar con nada?
	private bool cleanSection;											// Seccion limpia? (Sin recibir daños)
	private float driftDegree;											// Angulo de derrape

	// Constantes

	private const float STAT_TURNRATE_BASE = 2f;
	private const float STAT_TURNRATE_SCAL = 0.4f;
	private const float STAT_ACCELERATION_BASE = 0.5f;
	private const float STAT_ACCELERATION_SCAL = 0.225f;
	private const float STAT_MAXSPEED_BASE = 20f;
	private const float STAT_MAXSPEED_SCAL = 2.15f;
	private const float STAT_DRIFTSTR_BASE = 2.5f;
	private const float STAT_DRIFTSTR_SCAL = 0.175f;
	private const float STAT_MAXDRIFT_BASE = 15f;
	private const float STAT_MAXDRIFT_SCAL = 3.5f;
	private const float STAT_DRIFTSPDCONS_BASE = 0.2f;
	private const float STAT_DRIFTSPDCONS_SCAL = 0.08f;
	private const float STAT_SPDFALLOFF_BASE = 1f;
	private const float STAT_SPDFALLOFF_SCAL = 0.25f;

	private const float DRIFT_TURN_RATE = 6f;							// Turnrate utilizado en el drift, igual para todos los coches.
	private const float GROUND_TRANSITION_THS = 0.05f;					// Margen de tiempo para dejar de tocar suelo (en seg.)
	private const float UNGROUNDED_RESPAWN_DELAY = 5f;					// Tiempo sin tocar suelo necesario para auto-reaparecer
	private const float TRANSLATE_TO_VELOCITY = 3f;						// Proporcion traslacion-velocidad aplicado al objeto al dejar de tocar el suelo
	private const float DOWNFORCE = 10f;								// Fuerza aplicada en Vector3.down RELATIVO al coche para pegarlo al suelo.
	private const float INCLINATION_MAX_SPEED_MULTIPLIER = 0.1f;		// Intensidad de la modificacion de la velocidad maxima por inclinacion de terreno.
	private const float INCLINATION_ACCELERATION_MULTIPLIER = 0.02f;	// Intensidad de la modificacion de la aceleracion por inclinacion de terreno.
	private const float DRIFT_CORRECTION_STRENGHT = 1.75f;				// Fuerza de correccion del drift al intentar estabilizar.
	private const float FRICTION_SPD_CONSERVATION_FRONT_STAY = 0.85f;	// Conservacion de velocidad en colision frontal (STAY)
	private const float FRICTION_SPD_CONSERVATION_FRONT_ENTER = 0.8f;	// Conservacion de velocidad en colision frintal (ENTER)
	private const float FRICTION_SPD_CONSERVATION_SIDE_STAY = 0.994f;	// Conservacion de velocidad en colision lateral (STAY)
	private const float FRICTION_SPD_CONSERVATION_SIDE_ENTER = 0.9f;	// Conservacion de velocidad en colision lateral (ENTER)

	// Valores auxiliares privados

	private int lastNodeCrossedID;										// ID del ultimo nodo cruzado (Se inicia en -1).
	private RoadNode nodeCrossedParams;									// Ultimo nodo cruzado (NodeProperties)
	private float extraForwInput;										// Aceleracion extra por inclinacion de terreno.
	private float extraFwdSpeed;										// Velocidad limite extra por inclinacion de terreno.
	private float turnMultiplier = 0f;									// Auxiliar para evitar que se pueda girar a 0 km/h.
	private float driftMultiplier = 0.2f;
	private float driftOutsideForce = 0f;
	private float deltaEulerX;

	// Otras referencias

	private Rigidbody rb;												// Referencia al rigidbody
	private CarData carReferenced;
	public Transform resetTransform;
	private float respawnCooldown = 0;

	private bool allowPlayerControl;

	void Start()
	{
		lastNodeCrossedID = -1;
		rb = GetComponent<Rigidbody> ();
		SetCarStats ();
		rb.velocity = new Vector3 (0, 0, 0);
		driftDegree = 0;
		resetTransform.transform.position = transform.position;
		resetTransform.transform.rotation = transform.rotation;
		StartCoroutine ("LockRotation");
	}

	// Los inputs del jugador son leidos en Update, mientras que las fisicas son procesadas en FixedUpdate, para asi mejorar la respuesta.

	void Update()
	{
		if (!allowPlayerControl) {
			forwInput = 0;
			turnInput = 0;
			extraForwInput = 0;
			return;
		}
		if (respawnCooldown > 0)
			respawnCooldown -= Time.deltaTime;

		if (StageData.currentData.time_remainingSec > 0) {
			forwInput = Input.GetAxis ("Vertical");
		} else {
			forwInput = 0f;
		}
		turnInput = Input.GetAxis ("Horizontal");
		if (Input.GetKeyDown (KeyCode.Space))
			drifting = true;

		// Test only: 
//		if (Input.GetKeyDown (KeyCode.R)) {
//			ResetCar ();
//		}
	}

	// Procesa las fisicas del coche.

	void FixedUpdate()
	{
		CheckGrounded ();

		if (grounded) {	// Acciones a realizar tocando suelo

			// Parametros extra de velocidad y aceleracion asignados dependiendo de la inclinacion del coche si este esta tocando suelo.
			deltaEulerX = Mathf.DeltaAngle (0, transform.rotation.eulerAngles.x);
			extraForwInput = deltaEulerX * INCLINATION_ACCELERATION_MULTIPLIER;
			extraFwdSpeed = deltaEulerX * INCLINATION_MAX_SPEED_MULTIPLIER;

			// Downforce (Empuje hacia el suelo para "pegar" el coche al suelo) TODO: es necesario ahora?
			rb.AddForce(-transform.up * Mathf.Abs(accumulatedSpeed) * DOWNFORCE * Time.fixedDeltaTime);


			if (forwInput == 0) // Sin input de aceleracion.
				accumulatedSpeed = Mathf.MoveTowards (accumulatedSpeed, extraFwdSpeed * 20, Time.fixedDeltaTime * 5);
			else { // En el caso de que el jugador de algun input de aceleracion.
				if (forwInput < 0 && accumulatedSpeed > 0) { // Frenando
					accumulatedSpeed += (forwInput+extraForwInput) * Time.fixedDeltaTime * 20;
				} else if (accumulatedSpeed < 0 && forwInput < 0){ 	// Marcha atras
					accumulatedSpeed += (forwInput + extraForwInput) * Time.fixedDeltaTime * 3 * acceleration * (Mathf.Clamp01 ((1-(accumulatedSpeed / maxBwdSpeed)) * speedFalloffReductionBwd)); 
				} else { // Acelerando
					accumulatedSpeed += (forwInput + extraForwInput) * Time.fixedDeltaTime * 3 * acceleration * (Mathf.Clamp01 ((1-(accumulatedSpeed / maxFwdSpeed)) * speedFalloffReductionFwd)); 
				}
			}

		} else {	// Acciones a realizar sin tocar suelo
			forwInput = turnInput = 0;
			accumulatedSpeed = Mathf.MoveTowards (accumulatedSpeed, 0, Time.fixedDeltaTime * 5);

			driftDegree = 0;
			drifting = false;
			accumulatedSpeed = transform.InverseTransformDirection(rb.velocity).z / TRANSLATE_TO_VELOCITY;

			if (ungroundedTime > UNGROUNDED_RESPAWN_DELAY)
				ResetCar ();
		}
		// Acciones a realizar en ambos casos

		if (accumulatedSpeed > maxFwdSpeed + extraFwdSpeed)
			accumulatedSpeed = Mathf.MoveTowards (accumulatedSpeed, maxFwdSpeed + extraFwdSpeed, Time.fixedDeltaTime);
		else if (accumulatedSpeed < maxBwdSpeed + extraFwdSpeed) 
			accumulatedSpeed = Mathf.MoveTowards (accumulatedSpeed, maxBwdSpeed + extraFwdSpeed, Time.fixedDeltaTime * 500);

		MoveTrn ();
		MoveFwd ();


	}

	// Comprueba si esta colisionando con el suelo

	void CheckGrounded()
	{
		if (detectingGrounded) {
			groundedTime += Time.fixedDeltaTime;
			if (!grounded && groundedTime > GROUND_TRANSITION_THS) {
				accumulatedSpeed = transform.InverseTransformDirection(rb.velocity).z / TRANSLATE_TO_VELOCITY;
				rb.velocity = Vector3.zero;
				grounded = true;
				ungroundedTime = 0;
				groundedTime = 0;
			}
		} else {
			ungroundedTime += Time.fixedDeltaTime;
			if (grounded && ungroundedTime > GROUND_TRANSITION_THS) {
				rb.velocity = transform.forward * accumulatedSpeed * TRANSLATE_TO_VELOCITY;
				cleanAir = true;
				grounded = false;
				groundedTime = 0;
				ungroundedTime = 0;
			}
		}
	}

	// Simula la aceleracion.

	void MoveFwd()
	{
		if (!grounded) {
			accumulatedSpeed = transform.InverseTransformDirection(rb.velocity).z / TRANSLATE_TO_VELOCITY;
			return;
		}

		driftOutsideForce = 1 + accumulatedSpeed/30f;
		rb.MovePosition(transform.TransformPoint( (Quaternion.Euler(0,-driftDegree * driftOutsideForce,0) * Vector3.forward * accumulatedSpeed * Time.fixedDeltaTime)));
		if (accumulatedSpeed < 3) {
			EndDrift();
		}
	}

	// Simula el giro/derrape

	void MoveTrn()
	{
		// Para evitar que se pueda girar a 0km/h como un tanque.
		turnMultiplier = Mathf.Clamp (accumulatedSpeed/3.5f, -1.5f,1.5f);
		turnMultiplier = Mathf.MoveTowards (turnMultiplier, 0, accumulatedSpeed * 0.02f);

		if (drifting) {
			accumulatedSpeed -= (Mathf.Abs(driftDegree)+accumulatedSpeed) * 0.2f * (1-driftSpeedConservation) * Time.fixedDeltaTime;
			driftMultiplier = Mathf.MoveTowards (driftMultiplier, 1, Time.deltaTime * 2.5f);
			transform.Rotate (new Vector3(0,((turnInput*0.7f) + driftDegree/20) * DRIFT_TURN_RATE * 10 * Time.fixedDeltaTime,0));
			if ((driftDegree > 0 && turnInput > 0) || (driftDegree < 0 && turnInput < 0)) {
				driftDegree += turnInput * Time.fixedDeltaTime * driftMultiplier *  driftStrenght * 10 * (Mathf.Clamp01 ((1 - (Mathf.Abs (driftDegree) / maxDrift)) * 1f)); // *1 es para recordar donde poner el ajuste
			} else {
				driftDegree += turnInput * Time.fixedDeltaTime * driftMultiplier * driftStrenght * 10 * DRIFT_CORRECTION_STRENGHT; 
			}
			// Asegurar que no derrape por encima del limite. Innecesario?
			driftDegree = Mathf.Clamp (driftDegree, -maxDrift, maxDrift);

			// Estabilizacion si se deja el input de giro en 0.
			if (turnInput == 0) {
				driftDegree = Mathf.MoveTowards (driftDegree, 0, Time.fixedDeltaTime * driftStabilization * 5);
				if (driftDegree == 0)
					EndDrift ();
			}
		} else {
			transform.Rotate (new Vector3(0,((turnInput*0.7f)) * turnRate * 10 * Time.fixedDeltaTime * turnMultiplier,0));
		}
	}
		
	// Administra la colision con triggers.

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "CP_Passive") {
			CrossCheckPoint (other);
		} else if (other.gameObject.tag == "CP_Active") {
			CrossCheckPoint (other);
		} else if (other.gameObject.tag == "CP_WrongWay") {
			if (respawnCooldown > 0)
				return;
			NotificationManager.currentInstance.AddNotification(new GameNotification("Wrong way!", Color.red, 30));
			ResetCar ();
		}
	}

	// Administra los checkpoints

	void CrossCheckPoint(Collider other)
	{
		resetTransform.position = other.transform.position;
		resetTransform.rotation = other.transform.rotation;
		nodeCrossedParams = other.transform.parent.parent.GetComponent<RoadNode>();
		if (nodeCrossedParams == null) {
			print ("[ERROR] RoadNode not found on road parent.");
			return;
		}
		nodeCrossedParams.CrossCheckPoint();

		// En el caso de que el jugador de alguna forma se salte parte de la carretera, esto comprueba cuantos nodos se ha saltado.

		for (int i = 0; i < nodeCrossedParams.GetID() - lastNodeCrossedID; i++)
		{
			RoadGenerator.currentInstance.SpawnNextNode ();
			StageData.currentData.nodesCrossed++;
		}
		lastNodeCrossedID = nodeCrossedParams.GetID();

		// RETURN si no se trata de un punto de control marcado como activo.
		if (other.tag == "CP_Active") {
			StageData.currentData.PlayerCrossedCheckPoint (cleanSection, nodeCrossedParams.GetTimeAwarded());
			StageData.currentData.PlayerCrossedNode ();
			cleanSection = true;
		} else {
			StageData.currentData.PlayerCrossedNode ();
		}
	}

	// Administra las colisiones [OnCollisionStay].

	public void SendCollisionStayFrom(string side)
	{
		switch (side) {
		case "LATERAL":
			{
				cleanSection = false;
				cleanAir = false;
				EndDrift ();
				if (grounded)
					StageData.currentData.SendPlayerCollision (accumulatedSpeed * 0.003f);
				else
					StageData.currentData.SendPlayerCollision (rb.velocity.magnitude * 0.0005f);
				accumulatedSpeed *= FRICTION_SPD_CONSERVATION_SIDE_STAY;
				break;
			}
		case "FRONTAL":
			{
				cleanSection = false;
				cleanAir = false;
				EndDrift ();
				if (grounded)
					StageData.currentData.SendPlayerCollision (accumulatedSpeed * 0.06f);
				else
					StageData.currentData.SendPlayerCollision (rb.velocity.magnitude * 0.002f);
				accumulatedSpeed *= FRICTION_SPD_CONSERVATION_FRONT_STAY;
				break;
			}
		case "TOP":
			{
				cleanSection = false;
				cleanAir = false;
				break;
			}
		case "GROUND":
			{
				detectingGrounded = true;
				break;
			}
		}
	}

	// Administra las colisiones [OnCollisionEnter]

	public void SendCollisionEnterFrom(string side)
	{
		switch (side) {
		case "LATERAL":
			{
				cleanSection = false;
				cleanAir = false;
				EndDrift ();
				if (grounded)
					StageData.currentData.SendPlayerCollision (accumulatedSpeed * 0.075f);
				else
					StageData.currentData.SendPlayerCollision (rb.velocity.magnitude * 0.01f);
				accumulatedSpeed *= FRICTION_SPD_CONSERVATION_SIDE_ENTER;
				break;
			}
		case "FRONTAL":
			{
				cleanSection = false;
				cleanAir = false;
				EndDrift ();
				if (grounded)
					StageData.currentData.SendPlayerCollision (accumulatedSpeed * 0.135f);
				else
					StageData.currentData.SendPlayerCollision (rb.velocity.magnitude * 0.02f);
				accumulatedSpeed *= FRICTION_SPD_CONSERVATION_FRONT_ENTER;
				break;
			}
		case "TOP":
			{
				cleanSection = false;
				cleanAir = false;
				StageData.currentData.SendPlayerCollision (rb.velocity.magnitude * 0.04f);
				break;
			}
		case "GROUND":
			{
				detectingGrounded = true;
				break;
			}
		}
	}

	public void EndDrift()
	{
		drifting = false;
		driftDegree = 0;
		driftMultiplier = 0.2f;
	}

	// Respawn (llamado al caer al vacio, o al ir en direccion contraria.

	void ResetCar()
	{
		if (respawnCooldown > 0)
			return;
		respawnCooldown = 0.5f;
		StageData.currentData.RespawnDamage ();
		cleanSection = false;
		cleanAir = false;
		groundedTime = 0;
		ungroundedTime = 0;
		accumulatedSpeed = 0;
		turnInput = 0;
		forwInput = 0;
		transform.position = resetTransform.position + Vector3.up * 0.5f;
		transform.rotation = resetTransform.rotation * Quaternion.Euler(0,-90,0);
		rb.angularVelocity = Vector3.zero;
		rb.velocity = Vector3.zero;
	}

	void SetCarStats()
	{
		carReferenced = GlobalGameData.currentInstance.GetCarInUse();
		turnRate = STAT_TURNRATE_BASE + carReferenced.GetTurnRate () * STAT_TURNRATE_SCAL;
		acceleration = STAT_ACCELERATION_BASE  + carReferenced.GetAcceleration () * STAT_ACCELERATION_SCAL;
		maxFwdSpeed = STAT_MAXSPEED_BASE + carReferenced.GetMaxSpeed () * STAT_MAXSPEED_SCAL;
		maxBwdSpeed = -(maxFwdSpeed * 0.35f);
		driftStrenght = STAT_DRIFTSTR_BASE  + carReferenced.GetDriftStrenght () * STAT_DRIFTSTR_SCAL;
		maxDrift = STAT_MAXDRIFT_BASE + carReferenced.GetMaxDriftDegree () * STAT_MAXDRIFT_SCAL;
		driftSpeedConservation = STAT_DRIFTSPDCONS_BASE + carReferenced.GetSpeedLossOnDrift () * STAT_DRIFTSPDCONS_SCAL;
		driftStabilization = carReferenced.GetDriftStabilization ();
		speedFalloffReductionFwd = STAT_SPDFALLOFF_BASE + carReferenced.GetAcceleration() * STAT_SPDFALLOFF_SCAL;
		speedFalloffReductionBwd = speedFalloffReductionFwd * 2f;
	}
	IEnumerator LockRotation()
	{
		while (lockPreRaceRotation) {
			transform.localRotation = Quaternion.identity;
			yield return null;
		}
	}

	public bool IsDrifting()
	{
		return drifting;
	}
	public bool IsGrounded()
	{
		return grounded;
	}
	public bool IsPerformingCleanAir()
	{
		return cleanAir;
	}
	public float GetCurrentSpeed()
	{
		return accumulatedSpeed;
	}
	public float GetUngroundedTime()
	{
		return ungroundedTime;
	}
	public float GetDriftDegree()
	{
		return driftDegree;
	}
	public void SetDetectingGrounded(bool arg)
	{
		detectingGrounded = arg;
	}
	public void AllowPlayerControl(bool arg)
	{
		allowPlayerControl = arg;
		lockPreRaceRotation = false;
	}
	public float GetCurrentSpeedPercentage()
	{
		return Mathf.Abs(accumulatedSpeed) / maxFwdSpeed;
	}
	public float GetHorizontalCamDisplacementValue()
	{
		return driftDegree * 0.04f + turnInput * 0.065f;
	}
	public bool IsStopped()
	{
		return Mathf.Abs (accumulatedSpeed) < 0.5f;
	}
}
