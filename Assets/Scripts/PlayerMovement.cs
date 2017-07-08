using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	// Controla el movimiento del jugador, para que el script funcione, el objeto necesita:
	// - RigidBody (No kinematico, sin constantes)
	// - Collider (Cubo)

	[Header("Car properties")]
	[Range(5.5f,8f)]
	public float turnRate;												// Fuerza de giro (sin drift) del vehiculo
	[Range(0.5f,6)]
	public float acceleration;											// Aceleracion del vehiculo
	[Range(25, 45)]
	public float maxFwdSpeed;											// Velocidad maxima
	[Range(-25,-5f)]
	public float maxBwdSpeed;											// Velocidad maxima marcha atras
	[Range(3,15)]
	public float driftStrenght;											// Fuerza de derrape
	[Range(10,50)]
	public float maxDrift;												// Maximo angulo de derrape
	[Range(0.5f,2.5f)]
	public float driftSpeedLoss;										// Perdida de velocidad al derrapar
	[Range(0.5f,12)]
	public float driftStabilization;									// Auto-Estabilizacion del derrape
	[Range(1f, 6f)]
	public float speedFalloffReductionFwd;								// Velocidad a la que se empieza a dejar de acelerar, cuanto mas alto sea, mas tardara en activarse (hacia delante)
	[Range(1f, 6f)]
	public float speedFalloffReductionBwd;								// Velocidad a la que se empieza a dejar de acelerar, cuanto mas alto sea, mas tardara en activarse (hacia delante)

	[Header("Debug info")]
	//TODO: convertir en privadas y preasignar los valores.
	public bool grounded;												// Esta en el suelo?
	public bool groundedHitbox;											// Detecta colision con el suelo?
	public bool drifting;												// Esta derrapando?
	public float forwInput;												// Almacena el input de acelerar/frenar
	public float turnInput;												// Almacena el input de giro
	public float accumulatedAcceleration;								// Velocidad
	public float ungroundedTime;										// Tiempo sin tocar el suelo
	public float groundingTime;											// Tiempo de transicion aire-suelo	
	public bool cleanAir;												// En el aire sin colisionar con nada?
	public bool cleanSection;											// Seccion limpia? (Sin recibir daños)
	public float driftDegree;											// Angulo de derrape

	// Constantes

	private float groundToAirThs = 0.05f;								// Margen de tiempo para dejar de tocar suelo (en seg.)
	private float airToGroundThs = 0.05f;								// Margen de tiempo para volver a tocar el suelo (en seg.)
	private float ungroundedRespawnTime = 6f;							// Tiempo sin tocar suelo necesario para auto-reaparecer
	private float Tr2Vel = 3f;											// Proporcion traslacion-velocidad aplicado al objeto al dejar de tocar el suelo
	private float downforce = 5000f;								    // Fuerza aplicada en Vector3.down RELATIVO al coche para pegarlo al suelo.
	private float inclinationMaxSpeedModifier = 30f;					// Intensidad de la modificacion de la velocidad maxima por inclinacion de terreno.
	private float inclinationAccelerationModifier = 25f;				// Intensidad de la modificacion de la aceleracion por inclinacion de terreno.
	private float driftOutsideForce = 1.75f;							// Multiplicador de apertura de drift
	private float driftCorrectionStr = 1.75f;								// Fuerza de correccion del drift al intentar estabilizar.
	private float frictionFactorFront = 0.85f;							// Factor de friccion frontal (menor = mas velocidad perdida al colisionar)
	private float frictionFactorSide = 0.99f;							// Factor de friccion lateral (menor = mas velocidad perdida al colisionar)

	// Valores auxiliares privados

	private bool currentEventFinished = false;
	private bool firstFrameUngrounded = false;							// Auxiliar para aplicar la fuerza al saltar SOLO una vez.
	private float respawnCooldown;										// (Temp) Reutilizacion del respawn.
	private int lastNodeCrossedID;										// ID del ultimo nodo cruzado (Se inicia en -1).
	private RoadNode nodeCrossedParams;									// Ultimo nodo cruzado (NodeProperties)
	private float extraForwInput;										// Aceleracion extra por inclinacion de terreno.
	private float extraFwdSpeed;										// Velocidad limite extra por inclinacion de terreno.
	private bool respawnEnabled = false;								// Permite el uso de R, se activa al cruzar el primer CP pasivo.
	private Vector3 savedResetPosition = new Vector3(0,3,0);			// Posicion de reset (respawn)
	private Quaternion savedResetRotation = Quaternion.identity;		// Rotacion de reset (respawn)
	private float turnMultiplier = 0;									// Auxiliar para evitar que se pueda girar a 0 km/h
	private Rigidbody rb;

	private bool gameStarted;

	void Start()
	{
		lastNodeCrossedID = -1;
		rb = GetComponent<Rigidbody> ();
		rb.velocity = new Vector3 (0, 0, 0);
		driftDegree = 0;
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

		// Leemos todos los inputs.
		//if (Input.GetKeyDown (KeyCode.R) && respawnCooldown <= 0 && respawnEnabled) {
		//	ResetCar ();
		//}

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
			extraForwInput = Mathf.Clamp( Mathf.DeltaAngle(0,transform.rotation.eulerAngles.x) / inclinationAccelerationModifier, -1, 2);
			extraFwdSpeed = Mathf.DeltaAngle (0, transform.rotation.eulerAngles.x) / inclinationMaxSpeedModifier;

			// Downforce (Empuje hacia el suelo para "pegar" el coche al suelo)
			rb.AddForce(-transform.up * Mathf.Abs(accumulatedAcceleration) * downforce * Time.fixedDeltaTime);


			if (forwInput == 0)
				// En el caso de que el jugador no de inputs.
				accumulatedAcceleration = Mathf.MoveTowards (accumulatedAcceleration, extraFwdSpeed * 20, Time.fixedDeltaTime * 5);
			else {
				// En el caso de que el jugador de algun input de aceleracion.
				if (forwInput < 0 && accumulatedAcceleration > 0) { 
					// Frenando
					accumulatedAcceleration += (forwInput+extraForwInput) * Time.fixedDeltaTime * 20;
				} else if (accumulatedAcceleration < 0 && forwInput < 0){ 
					// Marcha atras
					accumulatedAcceleration += (forwInput + extraForwInput) * Time.fixedDeltaTime * 3 * acceleration * (Mathf.Clamp01 ((1-(accumulatedAcceleration / maxBwdSpeed)) * speedFalloffReductionBwd)); 
				} else { 
					// Acelerando
					accumulatedAcceleration += (forwInput + extraForwInput) * Time.fixedDeltaTime * 3 * acceleration * (Mathf.Clamp01 ((1-(accumulatedAcceleration / maxFwdSpeed)) * speedFalloffReductionFwd)); 
				}
			}

		} else {			   // Acciones a realizar sin tocar suelo
			forwInput = turnInput = 0;
			accumulatedAcceleration = Mathf.MoveTowards (accumulatedAcceleration, 0, Time.fixedDeltaTime * 5);

			if (!firstFrameUngrounded) {
				rb.velocity = transform.forward * accumulatedAcceleration * Tr2Vel;
				firstFrameUngrounded = true;
				cleanAir = true;
			}

			driftDegree = 0;
			drifting = false;
			accumulatedAcceleration = transform.InverseTransformDirection(rb.velocity).z / Tr2Vel;

			if (ungroundedTime > ungroundedRespawnTime)
				ResetCar ();
		}
		// Acciones a realizar en ambos casos

		if (accumulatedAcceleration > maxFwdSpeed + extraFwdSpeed)
			accumulatedAcceleration = Mathf.MoveTowards (accumulatedAcceleration, maxFwdSpeed + extraFwdSpeed, Time.fixedDeltaTime);
		else if (accumulatedAcceleration < maxBwdSpeed + extraFwdSpeed) 
			accumulatedAcceleration = Mathf.MoveTowards (accumulatedAcceleration, maxBwdSpeed + extraFwdSpeed, Time.fixedDeltaTime * 500);

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
				if (groundingTime > airToGroundThs) {
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

		if (ungroundedTime > groundToAirThs) { grounded = false; } else
		{ 
			grounded = true; 
			firstFrameUngrounded = false; 
			if (firstFrameUngrounded) {
				firstFrameUngrounded = false;
				accumulatedAcceleration = rb.velocity.x * Tr2Vel;
			}
			rb.velocity = Vector3.zero; 
		}
	}

	// Simula la aceleracion.

	void MoveFwd()
	{
		accumulatedAcceleration = Mathf.MoveTowards(accumulatedAcceleration, 0, ((Mathf.Abs(driftDegree)*driftSpeedLoss) / 10) * Time.fixedDeltaTime);
		if (!grounded)
			return;
		// TODO: Ajustar la apertura del derrape? en "-driftDegree", multiplicar por valor.
		rb.MovePosition(transform.TransformPoint( (Quaternion.Euler(0,-driftDegree * driftOutsideForce,0) * Vector3.forward * accumulatedAcceleration * Time.fixedDeltaTime)));
		if (accumulatedAcceleration < 3) {
			EndDrift();
		}
	}

	// Simula el giro/derrape

	void MoveTrn()
	{
		// Para evitar que se pueda girar a 0km/h como un tanque.
		turnMultiplier = Mathf.Clamp (accumulatedAcceleration/10, -1,1);
		// Giro normal (Sin derrape)
		transform.Rotate (new Vector3(0,((turnInput*0.7f) + driftDegree/20) * turnRate * 10 * Time.fixedDeltaTime * turnMultiplier,0));

		if (drifting) {
			if ((driftDegree > 0 && turnInput > 0) || (driftDegree < 0 && turnInput < 0)) {
				driftDegree += turnInput * Time.fixedDeltaTime * driftStrenght * 10 * (Mathf.Clamp01 ((1-(Mathf.Abs(driftDegree) / maxDrift)) * 1f)); // *1 es para recordar donde poner el ajuste
			} else {
				driftDegree += turnInput * Time.fixedDeltaTime * driftStrenght * 10 * driftCorrectionStr; 
			}
			// Asegurar que no derrape por encima del limite.
			driftDegree = Mathf.Clamp (driftDegree, -maxDrift,maxDrift);
			// Estabilizacion si se deja el input de giro en 0.
			if (turnInput == 0) {
				driftDegree = Mathf.MoveTowards (driftDegree, 0, Time.fixedDeltaTime * driftStabilization * 10);
				if (driftDegree == 0)
					EndDrift ();
			}
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
		respawnEnabled = true;
		savedResetPosition = other.transform.position;
		savedResetRotation = other.transform.rotation;
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
					StageData.currentData.DamagePlayer (accumulatedAcceleration * 0.002f);
				else
					StageData.currentData.DamagePlayer (rb.velocity.magnitude * 0.0005f);
				accumulatedAcceleration *= frictionFactorSide;
				break;
			}
		case "FRONTAL":
			{
				cleanSection = false;
				cleanAir = false;
				EndDrift ();
				if (grounded)
					StageData.currentData.DamagePlayer (accumulatedAcceleration * 0.05f);
				else
					StageData.currentData.DamagePlayer (rb.velocity.magnitude * 0.002f);
				accumulatedAcceleration *= frictionFactorFront;
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
					StageData.currentData.DamagePlayer (accumulatedAcceleration * 0.05f);
				else
					StageData.currentData.DamagePlayer (rb.velocity.magnitude * 0.01f);
				accumulatedAcceleration *= frictionFactorSide;
				break;
			}
		case "FRONTAL":
			{
				cleanSection = false;
				cleanAir = false;
				EndDrift ();
				if (grounded)
					StageData.currentData.DamagePlayer (accumulatedAcceleration * 0.1f);
				else
					StageData.currentData.DamagePlayer (rb.velocity.magnitude * 0.02f);
				accumulatedAcceleration *= frictionFactorFront;
				break;
			}
		case "TOP":
			{
				cleanSection = false;
				cleanAir = false;
				StageData.currentData.DamagePlayer (rb.velocity.magnitude * 0.04f);
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
	void EndDrift()
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
		accumulatedAcceleration = 0;
		turnInput = 0;
		forwInput = 0;
		transform.position = savedResetPosition;
		transform.rotation = savedResetRotation * Quaternion.Euler(0,-90,0);
		rb.angularVelocity = Vector3.zero;
		rb.velocity = Vector3.zero;
	}
}
