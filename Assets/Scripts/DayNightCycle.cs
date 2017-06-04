using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour 
{


    [Header("Day Night Cycle Configuration")]
    public float m_DayNightCycleDuration;
    private float m_TimeBetweenSkyboxes;
    private float m_Timer;

    [Header("Different Skyboxes")]
    public Material[] m_Skyboxes;
    private int m_SelectedSkybox;
    private float m_DaySkyboxTime;
    private float m_NightSkyboxTime;

    [Header("Directional Light")]
    public Transform m_DirectionalLight;
    public Vector3 m_StartRotation;
    private float m_RotationSpeed;

	// Use this for initialization
	void Start () 
    {
        //Set Skyboxes Time values
        m_DaySkyboxTime =  m_DayNightCycleDuration / (m_Skyboxes.Length*1.5f);
        m_NightSkyboxTime = m_DaySkyboxTime  *  m_Skyboxes.Length/2f;

        //Set Default Skybox
        m_Timer = 0f;
        m_SelectedSkybox = 1;
        RenderSettings.skybox = m_Skyboxes[m_SelectedSkybox];
        m_TimeBetweenSkyboxes = m_DaySkyboxTime;

        //Set Default Light Rotation
        m_DirectionalLight.rotation = Quaternion.Euler(m_StartRotation);
        m_RotationSpeed = 360/m_DayNightCycleDuration;


	}
	
	// Update is called once per frame
	void Update () 
    {
        //Increase timer value
        m_Timer += Time.deltaTime;

        //Reset timer value and change selected Skybox
        if (m_Timer > m_TimeBetweenSkyboxes)
        {
            m_Timer = 0f;
            m_TimeBetweenSkyboxes = m_DaySkyboxTime;

            m_SelectedSkybox += 1;
            if (m_SelectedSkybox > m_Skyboxes.Length - 1)
            {
                m_SelectedSkybox = 0;
                m_TimeBetweenSkyboxes = m_NightSkyboxTime;
            }
            RenderSettings.skybox = m_Skyboxes[m_SelectedSkybox];

            if (m_SelectedSkybox == m_Skyboxes.Length - 2)
            {
                StageData.currentData.UpdateAllLights(true);
                m_DirectionalLight.GetComponent<Light>().color = new Color(0, 0, 0);

            }
            if (m_SelectedSkybox == 2)
            {
                StageData.currentData.UpdateAllLights(false);
                m_DirectionalLight.GetComponent<Light>().color = new Color(1f, 0.9411f,0.713f);
            }

        }

        //Set the Directional Light rotation
        m_DirectionalLight.Rotate(m_RotationSpeed*Time.deltaTime,0f, 0f);
       
	}
}
