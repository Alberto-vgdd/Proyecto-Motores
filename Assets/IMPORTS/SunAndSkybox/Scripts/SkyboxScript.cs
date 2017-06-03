using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxScript : MonoBehaviour {

	public  Material currentSkybox;
    public Transform target;
	private static AnimationClip sunAnimation;
	private static GameObject fatherGameobject;
	private GameObject objectSkyboxTest;

	//Colores referencia para animación, cambiar por otros que sean adecuados.

	private Color amanecer = new Color (0/255f, 255f/255f, 200f/255f, 255f/255f);
	private Color dia = new Color (60f/255f,124f/255f,233f/255f, 255f/255f);
	private Color noche = new Color(0f/255f,0f/255f,0f/255f, 255f/255f);

	//Valores de prueba
//	private Color noche = new Color (0f/255f, 0f/255f, 255f/255f);
//	private Color dia = new Color (255f/255f, 255f/255f, 255f/255f);
//	private Color noche = new Color (128f/255f, 128f/255f, 128f/255f);

	private Color ColorInicioUpdate;
	private Color ColorFinalUpdate;

	private Color colorCalculated;

	private float atmosphereThicknessNoche = 0.1f;
	private float atmosphereThicknessDía = 0.6f;
	private float atmosphereThicknessAtardecer = 0.3f;
	private float actualAT = 0.6f;

	private int shaderIDAtmosphereThickness;
	private int shaderIDSkyTint; //Es para obtener el ID del color del skybox

	private float tiempoTransicionUpdate;
	private int estadoLastFrame = 0;
	private int estadoActual = 0;

	private static AnimationEvent amanecerEvent; //Dura 4 segundos
	private static AnimationEvent mediodiaEvent; //Dura 16 segundos
	private static AnimationEvent atardecerEvent; //Dura 4 segundos
	private static AnimationEvent nocheEvent; //Dura 16 segundos

	private float timeTest = 0.0f; //A ver cuanto tiempo tarda en hacer toda la mierda

	void Awake()
	{	
		fatherGameobject = GameObject.Find ("SunAndSkybox");
		//Nombre objeto padre: SunAndSkybox

		objectSkyboxTest = GameObject.Find("SphereSkyboxTest");
		currentSkybox = objectSkyboxTest.GetComponent<MeshRenderer>().material;

        sunAnimation = fatherGameobject.GetComponent<Animator> ().GetCurrentAnimatorClipInfo (0)[0].clip;

		shaderIDSkyTint = Shader.PropertyToID ("_SkyTint");
		shaderIDAtmosphereThickness = Shader.PropertyToID ("_AtmosphereThickness");

		colorCalculated = currentSkybox.GetColor (shaderIDSkyTint);

		ColorInicioUpdate = noche;
		ColorFinalUpdate = amanecer;

		//Nombre camara: Camera
		//Nombre esfera para testear material skybox: SphereSkyboxTest

		//print (sunAnimation.name);
	}

	// Use this for initialization
	void Start () 	{	SetSkyboxEvents ();	}



	void SetSkyboxEvents()
	{
		amanecerEvent = new AnimationEvent ();
		mediodiaEvent = new AnimationEvent ();
		atardecerEvent = new AnimationEvent ();
		nocheEvent = new AnimationEvent ();

		mediodiaEvent.time = 0.0f;
		atardecerEvent.time = (sunAnimation.length / 2.0f) - 4.0f;
		nocheEvent.time = (sunAnimation.length / 2.0f) + 1.0f;
		amanecerEvent.time = sunAnimation.length - 2.0f;

		mediodiaEvent.intParameter = 1;
		atardecerEvent.intParameter = 2;
		nocheEvent.intParameter = 3;
		amanecerEvent.intParameter = 4;


		amanecerEvent.functionName = "ChangeSkybox";
		mediodiaEvent.functionName = "ChangeSkybox";
		atardecerEvent.functionName = "ChangeSkybox";
		nocheEvent.functionName = "ChangeSkybox";

		sunAnimation.AddEvent (amanecerEvent);
		sunAnimation.AddEvent (mediodiaEvent);
		sunAnimation.AddEvent (atardecerEvent);
		sunAnimation.AddEvent (nocheEvent);
	}

	void ChangeSkybox(int estado)
	{

		//De amanecer a mediodia
		if (estado == 1) 
		{
			estadoActual = 1;
			ColorInicioUpdate = amanecer;
			ColorFinalUpdate = dia;
			tiempoTransicionUpdate = 4.0f * 2;
			StageData.currentData.UpdateAllLights (false);
			//print ("Va a ser mediodía");		
		}

		//Si va a atardecer
		if (estado == 2) 
		{	
			estadoActual = 2;
			ColorInicioUpdate = dia;
			ColorFinalUpdate = amanecer;
			tiempoTransicionUpdate = 4.0f;
			//print ("Va a atardecer");		
		}

		//De atardecer a noche
		if (estado == 3) 
		{	
			estadoActual = 3;
			ColorInicioUpdate = amanecer;
			ColorFinalUpdate = noche;
			tiempoTransicionUpdate = 1.0f;
			StageData.currentData.UpdateAllLights (true);
			//print ("Va a hacerse de noche");	
		}

		//De noche a amanecer
		if (estado == 4) 
		{	
			estadoActual = 4;
			ColorInicioUpdate = noche;
			ColorFinalUpdate = amanecer;
			tiempoTransicionUpdate = 4.0f;
			//print ("Va a amanecer");		
		}


	}
		
	void UpdateMaterialColor (Color color1, Color color2, float time)
	{	
		colorCalculated = Color.Lerp (color1, color2, time);	

		if (estadoActual == 2)	 //De normal a atardecer
		{	actualAT = Mathf.Lerp (atmosphereThicknessDía, atmosphereThicknessAtardecer, time / 2);} 
		else if (estadoActual == 3) //De atardecer a anochecer
		{	actualAT = Mathf.Lerp (atmosphereThicknessAtardecer, atmosphereThicknessNoche, time / 100);	} 
		else if (estadoActual == 4 ) //De noche a amanecer 
		{	actualAT = Mathf.Lerp (atmosphereThicknessNoche, atmosphereThicknessDía, time); }

	}

	void UpdateState(int estadoLastFrame, int estadoActual)
	{
		timeTest += Time.deltaTime;
		if (estadoLastFrame == estadoActual) 
		{	
			UpdateMaterialColor (ColorInicioUpdate, ColorFinalUpdate, timeTest / tiempoTransicionUpdate);	
		} 
		else 
		{	timeTest = 0f;	}			
			
		objectSkyboxTest.GetComponent<MeshRenderer>().material.SetColor(shaderIDSkyTint, colorCalculated);
		objectSkyboxTest.GetComponent<MeshRenderer> ().material.SetFloat (shaderIDAtmosphereThickness, actualAT);
	}


	void updateEnvironmentEffects()
	{			
		RenderSettings.skybox = currentSkybox;
		RenderSettings.fogColor =  colorCalculated;
	}


	
	// Update is called once per frame
	void Update () 
	{	
		UpdateState (estadoLastFrame, estadoActual);

		updateEnvironmentEffects ();

		estadoLastFrame = estadoActual;

        transform.parent.position = new Vector3(target.position.x, 0f, target.position.z);
	}

}
