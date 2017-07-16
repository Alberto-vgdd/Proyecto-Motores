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
	private float driftSpeedLoss;										// Perdida de velocidad al derrapar
	private float driftStabilization;									// Auto-Estabilizacion del derrape
	private float speedFalloffReductionFwd;								// Velocidad a la que se empieza a dejar de acelerar, cuanto mas alto sea, mas tardara en activarse (hacia delante)
	private float speedFalloffReductionBwd;								// Velocidad a la que se empieza a dejar de acelerar, cuanto mas alto sea, mas tardara en activarse (hacia delante)

	// Parametros de estado

	private bool grounded;												// Esta en el suelo?
	private bool groundedHitbox;										// Detecta colision con el suelo?
	private bool drifting;												// Esta derrapando?
	private float forwInput;											// Almacena el input de acelerar/frenar
	private float turnInput;											// Almacena el input de giro
	private float accumulatedSpeed;										// Velocidad
	private float ungroundedTime;										// Tiempo sin tocar el suelo
	private float groundingTime;										// Tiempo de transicion aire-suelo	
	private bool cleanAir;												// En el aire sin colisionar con nada?
	private bool cleanSection;											// Seccion limpia? (Sin recibir daños)
	private float driftDegree;											// Angulo de derrape

	// Constantes

	private const float DRIFT_TURN_RATE = 6f;							// Turnrate utilizado en el drift, igual para todos los coches.
	private const float GROUND_TO_AIR_THS = 0.05f;						// Margen de tiempo para dejar de tocar suelo (en seg.)
	private const float AIR_TO_GROUND_THS = 0.05f;						// Margen de tiempo para volver a tocar el suelo (en seg.)
	private const float UNGROUNDED_RESPAWN_DELAY = 6f;					// Tiempo sin tocar suelo necesario para auto-reaparecer
	private const float TRANSLATE_TO_VELOCITY = 3f;						// Proporcion traslacion-velocidad aplicado al objeto al dejar de tocar el suelo
	private const float DOWNFORCE = 1000f;								// Fuerza aplicada en Vector3.down RELATIVO al coche para pegarlo al suelo.
	private const float INCLINATION_MAX_SPEED_MODIFIER = 30f;			// Intensidad de la modificacion de la velocidad maxima por inclinacion de terreno.
	private const float INCLINATION_ACCELERATION_MODIFIER = 25f;		// Intensidad de la modificacion de la aceleracion por inclinacion de terreno.
	private const float DRIFT_OUTSIDE_FORCE = 1.75f;					// Multiplicador de apertura de drift
	private const float DRIFT_CORRECTION_STRENGHT = 1.75f;				// Fuerza de correccion del drift al intentar estabilizar.
	private const float FRICTION_SPD_CONSERVATION_FRONT = 0.85f;		// Factor de friccion frontal (menor = mas velocidad perdida al colisionar)
	private const float FRICTION_SPD_CONSVERATION_SIDE = 0.99f;			// Factor de friccion lateral (menor = mas velocidad perdida al colisionar)

	// Valores auxiliares privados

	private bool currentEventFinished = false;
	private bool firstFrameUngrounded = false;							// Auxiliar para aplicar la fuerza al saltar SOLO una vez.
	private float respawnCooldown;										// (Temp) Reutilizacion del respawn.
	private int lastNodeCrossedID;										// ID del ultimo nodo cruzado (Se inicia en -1).
	private RoadNode nodeCrossedParams;									// Ultimo nodo cruzado (NodeProperties)
	private float extraForwInput;										// Aceleracion extra por inclinacion de terreno.
	private float extraFwdSpeed;										// Velocidad limite extra por inclinacion de terreno.
	private float turnMultiplier = 0;									// Auxiliar para evitar que se pueda girar a 0 km/h

	// Otras referencias

	private Rigidbody rb;												// Referencia al rigidbody
	private CarData carReferenced;
	public Transform resetTransform;

	private bool gameStarted;											// Se ha iniciado la partida? (TODO: innecesario, leer de stagedata.currentdata.gameStarted)

	void Start()
	{
		lastNodeCrossedID = -1;
		rb = GetComponent<Rigidbody> ();
		SetCarStats ();
		rb.velocity = new Vector3 (0, 0, 0);
		driftDegree = 0;
		resetTransform.transform.position = transform.position;
		resetTransform.transform.rotation = transform.rotation;
	}

	// Los inputs del jugador son leidos en Update, mientras que las fisicas son procesadas en FixedUpdate, para asi mejorar la respuesta
	// de los controles y evitar que se pierdan inputs.

	void Update()
	{
		// Si la partida no ha empezado, no leemos controles.
		if (!gameStarted) {
			gameStarted = StageData.currentData.gameStarted;
			forwInput = 0;
			turnInput = 0;
			extraForwInput = 0;
			return;
		}
		// Si el jugador ha destruido su vehiculo, ignoramos todos los inputs.
		if (currentEventFinished) {
			forwInput = 0;
			turnInput = 0;
			extraForwInput = 0;
			return;
		}
			
		// Si el tiempo se ha agotado, solo escuchamos el input de giro.
		if (StageData.currentData.time_remainingSec <= 0) {
			forwInput = 0;
			turnInput = Input.GetAxis ("Horizontal");
			if (Input.GetKeyDown (KeyCode.Space))
				drifting = true;
			return;
		}
			
//		if (Input.GetKeyDown (KeyCode.R)) {
//			ResetCar ();
//		}

		if (respawnCooldown > 0)
			respawnCooldown -= Time.deltaTime;

		forwInput = Input.GetAxis ("Vertical");
		turnInput = Input.GetAxis ("Horizontal");
		if (Input.GetKeyDown (KeyCode.Space))
			drifting = true;
	}

	// Procesa las fisicas del coche.

	void FixedUpdate()
	{
		CheckGrounded ();      // Comprobamos si esta tocando suelo
		if (grounded) {        // Acciones a realizar tocando suelo

			// Parametros extra de velocidad y aceleracion asignados dependiendo de la inclinacion del coche si este esta tocando suyelo.
			extraForwInput = Mathf.Clamp( Mathf.DeltaAngle(0,transform.rotation.eulerAngles.x) / INCLINATION_ACCELERATION_MODIFIER, -1, 2);
			extraFwdSpeed = Mathf.DeltaAngle (0, transform.rotation.eulerAngles.x) / INCLINATION_MAX_SPEED_MODIFIER;

			// Downforce (Empuje hacia el suelo para "pegar" el coche al suelo)
			rb.AddForce(-transform.up * Mathf.Abs(accumulatedSpeed) * DOWNFORCE * Time.fixedDeltaTime);


			if (forwInput == 0)
				// En el caso de que el jugador no de inputs.
				accumulatedSpeed = Mathf.MoveTowards (accumulatedSpeed, extraFwdSpeed * 20, Time.fixedDeltaTime * 5);
			else {
				// En el caso de que el jugador de algun input de aceleracion.
				if (forwInput < 0 && accumulatedSpeed > 0) { 
					// Frenando
					accumulatedSpeed += (forwInput+extraForwInput) * Time.fixedDeltaTime * 20;
				} else if (accumulatedSpeed < 0 && forwInput < 0){ 
					// Marcha atras
					accumulatedSpeed += (forwInput + extraForwInput) * Time.fixedDeltaTime * 3 * acceleration * (Mathf.Clamp01 ((1-(accumulatedSpeed / maxBwdSpeed)) * speedFalloffReductionBwd)); 
				} else { 
					// Acelerando
					accumulatedSpeed += (forwInput + extraForwInput) * Time.fixedDeltaTime * 3 * acceleration * (Mathf.Clamp01 ((1-(accumulatedSpeed / maxFwdSpeed)) * speedFalloffReductionFwd)); 
				}
			}

		} else {			   // Acciones a realizar sin tocar suelo
			forwInput = turnInput = 0;
			accumulatedSpeed = Mathf.MoveTowards (accumulatedSpeed, 0, Time.fixedDeltaTime * 5);

			if (!firstFrameUngrounded) {
				rb.velocity = transform.forward * accumulatedSpeed * TRANSLATE_TO_VELOCITY;
				firstFrameUngrounded = true;
				cleanAir = true;
			}

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

		MoveFwd ();
		MoveTrn ();

	}

	// Comprueba si esta colisionando con el suelo

	void CheckGrounded()
	{

		if (groundedHitbox) 
		{ 
			if (!grounded) {
				groundingTime += Time.deltaTime;
				if (groundingTime > AIR_TO_GROUND_THS) {
					ungroundedTime = 0;
					groundingTime = 0;
				}
			} else {
				ungroundedTime = 0; 
			}
		} 
		else 
		{ 
			ungroundedTime += Time.fixedDeltaTime; 
		}

		if (ungroundedTime > GROUND_TO_AIR_THS) { grounded = false; } else
		{ 
			grounded = true; 
			firstFrameUngrounded = false; 
			if (firstFrameUngrounded) {
				firstFrameUngrounded = false;
				accumulatedSpeed = rb.velocity.x * TRANSLATE_TO_VELOCITY;
			}
			rb.velocity = Vector3.zero; 
		}
	}

	// Simula la aceleracion.

	void MoveFwd()
	{
		accumulatedSpeed = Mathf.MoveTowards(accumulatedSpeed, 0, ((Mathf.Abs(driftDegree)*driftSpeedLoss) / 10) * Time.fixedDeltaTime);
		if (!grounded)
			return;
		// TODO: Ajustar la apertura del derrape? en "-driftDegree", multiplicar por valor.
		rb.MovePosition(transform.TransformPoint( (Quaternion.Euler(0,-driftDegree * DRIFT_OUTSIDE_FORCE,0) * Vector3.forward * accumulatedSpeed * Time.fixedDeltaTime)));
		if (accumulatedSpeed < 3) {
			EndDrift();
		}
	}

	// Simula el giro/derrape

	void MoveTrn()
	{
		// Para evitar que se pueda girar a 0km/h como un tanque.
		turnMultiplier = Mathf.Clamp (accumulatedSpeed/10, -1,1);

		if (drifting) {
			transform.Rotate (new Vector3(0,((turnInput*0.7f) + driftDegree/20) * DRIFT_TURN_RATE * 10 * Time.fixedDeltaTime * turnMultiplier,0));

			if ((driftDegree > 0 && turnInput > 0) || (driftDegree < 0 && turnInput < 0)) {
				driftDegree += turnInput * Time.fixedDeltaTime * driftStrenght * 10 * (Mathf.Clamp01 ((1 - (Mathf.Abs (driftDegree) / maxDrift)) * 1f)); // *1 es para recordar donde poner el ajuste
			} else {
				driftDegree += turnInput * Time.fixedDeltaTime * driftStrenght * 10 * DRIFT_CORRECTION_STRENGHT; 
			}
			// Asegurar que no derrape por encima del limite.
			driftDegree = Mathf.Clamp (driftDegree, -maxDrift, maxDrift);
			// Estabilizacion si se deja el input de giro en 0.
			if (turnInput == 0) {
				driftDegree = Mathf.MoveTowards (driftDegree, 0, Time.fixedDeltaTime * driftStabilization * 10);
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
				accumulatedSpeed *= FRICTION_SPD_CONSVERATION_SIDE;
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
				accumulatedSpeed *= FRICTION_SPD_CONSERVATION_FRONT;
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
				groundedHitbox = true;
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
				accumulatedSpeed *= FRICTION_SPD_CONSVERATION_SIDE;
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
				accumulatedSpeed *= FRICTION_SPD_CONSERVATION_FRONT;
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
				groundedHitbox = true;
				break;
			}
		}
	}

	// Termina el drift (llamado al colisionar o al no estar en el suelo)
	public void SetAsEventFinished()
	{
		currentEventFinished = true;	
	}
	public void EndDrift()
	{
		drifting = false;
		driftDegree = 0;
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
		groundingTime = 0;
		ungroundedTime = 0;
		accumulatedSpeed = 0;
		turnInput = 0;
		forwInput = 0;
		transform.position = resetTransform.position;
		transform.rotation = resetTransform.rotation * Quaternion.Euler(0,-90,0);
		rb.angularVelocity = Vector3.zero;
		rb.velocity = Vector3.zero;
	}

	void SetCarStats()
	{
		carReferenced = GlobalGameData.currentInstance.carInUse;
		turnRate = carReferenced.GetTurnRate ();
		acceleration = carReferenced.GetAcceleration ();
		maxFwdSpeed = carReferenced.GetMaxSpeed ();
		maxBwdSpeed = -(maxFwdSpeed * 0.35f);
		driftStrenght = carReferenced.GetDriftStrenght ();
		maxDrift = carReferenced.GetMaxDriftDegree ();
		driftSpeedLoss = carReferenced.GetSpeedLossOnDrift ();
		driftStabilization = carReferenced.GetDriftStabilization ();
		speedFalloffReductionFwd = carReferenced.GetSpeedFalloffStartingPoint ();
		speedFalloffReductionBwd = speedFalloffReductionFwd * 2f;
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
	public void SetGroundedHitboxDetection(bool arg)
	{
		groundedHitbox = arg;
	}
	public float GetCurrentSpeedPercentage()
	{
		return accumulatedSpeed / maxFwdSpeed;
	}
	public bool IsStopped()
	{
		return Mathf.Abs (accumulatedSpeed) < 0.5f;
	}
}
