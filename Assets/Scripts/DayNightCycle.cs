using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour 
{

	public static DayNightCycle currentInstance;

    [Header("Day Night Cycle Configuration")]
	public float dayCycleTimescale;
	public float currentHour;


    [Header("Different Skyboxes")]
    public Material[] m_Skyboxes;
    private int m_SelectedSkybox;

    [Header("Directional Light")]

	private float rotationOffset = 250f;
	private bool lightsOn = false;
	private float lightsOnStart = 19.5f;
	private float lightsOnEnd = 7f;

	private Light directionalLight;

	public Color illumMidday;
	public Color illumMidnight;
	public Color illumSunrise;
	public Color illumSunset;

	private Color currentIllumColor;
	private float currentIllumT;

	// Use this for initialization
	void Awake ()
	{
		currentInstance = this;
	}
	void Start () 
    {
		//RenderSettings.skybox = m_Skyboxes [1];
		directionalLight = GetComponent<Light> ();
	}
	
	// Update is called once per frame
	void Update () 
    {
        //Increase timer value
		currentHour += Time.deltaTime * dayCycleTimescale;
		if (currentHour > 24)
			currentHour = 0;
		transform.rotation = Quaternion.Euler (((currentHour/24) * 360) + rotationOffset,0,0);
		UpdateIllumColor ();

		if (currentHour > 12) { directionalLight.intensity = 1 - (currentHour-12)/12f; }
		else { directionalLight.intensity = currentHour / 12f; }
		RenderSettings.ambientIntensity = directionalLight.intensity;

		if (lightsOn) {
			if (currentHour > lightsOnEnd && currentHour < lightsOnStart) {
				lightsOn = false;
				StageData.currentData.UpdateAllLights ();
				//print ("turning off lights");

			}
		} else {
			if (!(currentHour > lightsOnEnd && currentHour < lightsOnStart)) {
				lightsOn = true;
				StageData.currentData.UpdateAllLights ();
				//print ("turning on lights");
			}
		}

	}
	void UpdateIllumColor()
	{
		if (currentHour < 5) {
			currentIllumT = currentHour / 5f;
			currentIllumColor = Color.Lerp (illumMidnight, illumSunrise, currentIllumT);
		} else if (currentHour < 10) {
			currentIllumT = (currentHour-5) / 5f;
			currentIllumColor = Color.Lerp (illumSunrise, illumMidday, currentIllumT);
		} else if (currentHour < 19) {
			currentIllumT = (currentHour-10) / 9f;
			currentIllumColor = Color.Lerp (illumMidday, illumSunset, currentIllumT);
		} else if (currentHour < 21) {
			currentIllumT = (currentHour-19) / 2f;
			currentIllumColor = Color.Lerp (illumSunset, illumMidnight, currentIllumT);
		} else{
			currentIllumT = (currentHour-21) / 3f;
			currentIllumColor = Color.Lerp (illumMidnight, illumMidnight, currentIllumT);
		}
		directionalLight.color = currentIllumColor;
			
	}
	public void SetTimeAndTimescale(float time, float timescale)
	{
		currentHour = Mathf.Clamp (time, 0f, 24f);
		dayCycleTimescale = timescale;
	}
	public bool getLightsOn()
	{
		return lightsOn;
	}
}
