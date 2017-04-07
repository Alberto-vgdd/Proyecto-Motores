using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {

	public GameObject cocheObjetivo;

	float driftMultiplier = 0.0f;
	float distanceMultiplier = 0.0f;
	float timeMultiplier = 0.0f;
	float maxSpeedMultiplier = 0.0f;
	float localDriftDegreeMultiplier = 0.2f;
	float localDriftSpeedMultiplier = 0.1f;

	float dummyDistance = 0.0f;
	float maxSpeedRate = 0.5f; //Cuan cerca de la vel maxima queremos que cuente que 
							   //este como tal.
	float actualSpeed = 0.0f;  //Pues eso, que viene del rigid body.
	Vector3 lastFramePosition;

	float localDriftDistance = 0.0f;
	float localDrift = 0.0f;
	float localMaxSpeedTime = 0.0f;
	float localDriftTime = 0.0f;
	float localTimeDifference = 0.0f;

	float globalDistance = 0.0f;
	float globalDrift = 0.0f;
	float globalMaxSpeed = 0.0f;
	float globalDriftTime = 0.0f;
	float globalTimeDifference = 0.0f;

	float globalTime = 0f;
	float gameScore = 0.0f;

	void LevelEndScore() //Añade las estadísticas acumuladas del nivel
	{
		gameScore += 
			globalDrift * driftMultiplier +
			globalMaxSpeed * maxSpeedMultiplier +
			globalTime * timeMultiplier + 
			globalDistance * distanceMultiplier;
	}

	void EndGameScore()
	{
		LevelEndScore ();
		print ("Puntuación final del nivel: " + gameScore);		
	}

	void resetLocalScore()
	{
		localDrift = 0f;	localMaxSpeedTime = 0f;	
		localDriftTime = 0f; localTimeDifference = 0f;		
	}

	void resetGlobalScore()
	{
		globalDistance = 0f; globalDrift = 0f;
		globalDriftTime = 0f; globalMaxSpeed = 0f;
		globalTime = 0f; globalTimeDifference = 0f;
	}
		
	 public float getScoreLocalDrift()
	{
		//return (cocheObjetivo.GetComponent<PlayerMovement> ().driftDegree * localDriftDegreeMultiplier) +
		//			(actualSpeed * localDriftSpeedMultiplier);
		return 0.0f; //Esto es un dummy para que deje compilar, quitalo al ponerlo en el proyecto bueno.
	}

	 public float getScoreMaxSpeed()
	{	return localMaxSpeedTime * maxSpeedMultiplier;		}

	public float getLocalDriftDistance()
	{	return localDriftDistance;	}


	public float updatelocalDriftDistance()
	{
//		if (cocheObjetivo.GetComponent<PlayerMovement> ().drifting) 
//		{	localDriftDistance += actualSpeed / localDriftTime;		} 
//		else 
//		{	localDriftDistance = 0.0f;	}
		return 0.0f; //Otro dummy para que debuguee
	}


	void driftScoreManager()
	{
//		if (cocheObjetivo.GetComponent<PlayerMovement> ().drifting) 
//		{			
//			localDriftTime += Time.deltaTime;
//			localDrift = 
//				(cocheObjetivo.GetComponent<PlayerMovement> ().driftDegree * localDriftDegreeMultiplier) +
//			(actualSpeed * localDriftSpeedMultiplier);
//			globalDrift += localDrift * Time.deltaTime;
//			localDrift = 0f;
//		} 
//		else 
//		{
//			globalDriftTime += localDriftTime;
//			localDriftTime = 0f;
//		}
		updatelocalDriftDistance();
	}

	void maxSpeedManager()
	{
//		if (cocheObjetivo.GetComponent<PlayerMovement> ().maxFwdSpeed - maxSpeedRate  <= actualSpeed) {
//			localMaxSpeedTime += Time.deltaTime;
//		} 
//		else 
//		{
//			globalMaxSpeed += localMaxSpeedTime;
//			localMaxSpeedTime = 0.0f;
//		}
	}








	void Awake()
	{
		cocheObjetivo = GameObject.FindGameObjectWithTag ("Player");
		resetLocalScore ();
		cocheObjetivo.transform.position = new Vector3(0f, 0f, 0f);
	}


	void LateUpdate()
	{
		dummyDistance = Vector3.Distance (cocheObjetivo.transform.position, lastFramePosition);

		globalDistance += dummyDistance;

		lastFramePosition = cocheObjetivo.transform.position;
	}
		
	// Update is called once per frame
	void Update () 
	{
		actualSpeed = cocheObjetivo.GetComponent<Rigidbody> ().velocity.magnitude; 
			
		driftScoreManager ();
		maxSpeedManager ();
	}
}
