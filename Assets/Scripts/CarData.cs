using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarData {

	// Base car stats.

	private float m_turnRate;
	private float m_acceleration;
	private float m_maxSpeed;
	private float m_driftStrenght;
	private float m_maxDriftDegree;
	private float m_driftSpeedLoss;
	private float m_driftStabilization;
	private float m_speedFalloffReduction;

	// Bonus car stats (upgrades)

	private float m_upgraded_turnRate;
	private float m_upgraded_acceleration;
	private float m_upgraded_maxSpeed;
	private float m_upgraded_driftStrenght;
	private float m_upgraded_maxDriftDegree;
	private float m_upgraded_driftSpeedLoss;
	private float m_upgraded_driftStabilization;
	private float m_upgraded_speedFalloffReduction;

	// Other car data

	private int m_modelID;
	private int m_skinID;
	private string m_name;

	// Constant values

	private const float m_maxPossibleSpeed = 50f;
	private const float m_maxPossibleAcceleration = 6f;
	private const float m_maxPossibleTurnrate = 10f;
	private const float m_maxPossibleDriftDegree = 100f;
	private const float m_maxPossibleDriftStrenght = 10f;

	// Constructor sin parametros, coche de prueba.
	public CarData(int preset = 0)
	{
		switch (preset) {
		case 1:
			{
				m_turnRate = 4f;
				m_acceleration = 2.8f;
				m_maxSpeed = 41.5f;
				m_maxDriftDegree = 90f;
				m_driftStrenght = 5f;
				m_driftSpeedLoss = 6.5f;
				m_driftStabilization = 4f;
				m_speedFalloffReduction = 2f;

				m_upgraded_turnRate = 0;
				m_upgraded_acceleration = 0;
				m_upgraded_maxSpeed = 0;
				m_upgraded_driftStrenght = 0;
				m_upgraded_maxDriftDegree = 0;
				m_upgraded_driftSpeedLoss = 0;
				m_upgraded_driftStabilization = 0;
				m_upgraded_speedFalloffReduction = 0;

				m_modelID = 1;
				m_skinID = 1;
				m_name = "[TEST] SPORTS CAR";
				break;
			}
		case 2:
			{
				m_turnRate = 7f;
				m_acceleration = 3.4f;
				m_maxSpeed = 27f;
				m_maxDriftDegree = 20f;
				m_driftStrenght = 8f;
				m_driftSpeedLoss = 1f;
				m_driftStabilization = 2f;
				m_speedFalloffReduction = 4f;

				m_upgraded_turnRate = 0;
				m_upgraded_acceleration = 0;
				m_upgraded_maxSpeed = 0;
				m_upgraded_driftStrenght = 0;
				m_upgraded_maxDriftDegree = 0;
				m_upgraded_driftSpeedLoss = 0;
				m_upgraded_driftStabilization = 0;
				m_upgraded_speedFalloffReduction = 0;

				m_modelID = 1;
				m_skinID = 1;
				m_name = "[TEST] RALLY CAR";
				break;
			}
		case 3:
			{
				m_turnRate = 4f;
				m_acceleration = 2.5f;
				m_maxSpeed = 35f;
				m_maxDriftDegree = 100f;
				m_driftStrenght = 5f;
				m_driftSpeedLoss = 3f;
				m_driftStabilization = 1f;
				m_speedFalloffReduction = 1.5f;

				m_upgraded_turnRate = 0;
				m_upgraded_acceleration = 0;
				m_upgraded_maxSpeed = 0;
				m_upgraded_driftStrenght = 0;
				m_upgraded_maxDriftDegree = 0;
				m_upgraded_driftSpeedLoss = 0;
				m_upgraded_driftStabilization = 0;
				m_upgraded_speedFalloffReduction = 0;

				m_modelID = 1;
				m_skinID = 1;
				m_name = "[TEST] MUSCLECAR";
				break;
			}
		case 4:
			{
				m_turnRate = 5.5f;
				m_acceleration = 2f;
				m_maxSpeed = 50f;
				m_maxDriftDegree = 25f;
				m_driftStrenght = 5f;
				m_driftSpeedLoss = 4f;
				m_driftStabilization = 3f;
				m_speedFalloffReduction = 1.5f;

				m_upgraded_turnRate = 0;
				m_upgraded_acceleration = 0;
				m_upgraded_maxSpeed = 0;
				m_upgraded_driftStrenght = 0;
				m_upgraded_maxDriftDegree = 0;
				m_upgraded_driftSpeedLoss = 0;
				m_upgraded_driftStabilization = 0;
				m_upgraded_speedFalloffReduction = 0;

				m_modelID = 1;
				m_skinID = 1;
				m_name = "[TEST] LMP-ONE CAR";
				break;
			}
		case 5:
			{
				m_turnRate = 3.5f;
				m_acceleration = 3.2f;
				m_maxSpeed = 30f;
				m_maxDriftDegree = 45f;
				m_driftStrenght = 7f;
				m_driftSpeedLoss = 1.5f;
				m_driftStabilization = 1f;
				m_speedFalloffReduction = 1.5f;

				m_upgraded_turnRate = 0;
				m_upgraded_acceleration = 0;
				m_upgraded_maxSpeed = 0;
				m_upgraded_driftStrenght = 0;
				m_upgraded_maxDriftDegree = 0;
				m_upgraded_driftSpeedLoss = 0;
				m_upgraded_driftStabilization = 0;
				m_upgraded_speedFalloffReduction = 0;

				m_modelID = 1;
				m_skinID = 1;
				m_name = "[TEST] JGTC DRIFT RACECAR";
				break;
			}
		case 6:
			{
				m_turnRate = 3.5f;
				m_acceleration = 4f;
				m_maxSpeed = 40f;
				m_maxDriftDegree = 90f;
				m_driftStrenght = 4f;
				m_driftSpeedLoss = 7f;
				m_driftStabilization = 1f;
				m_speedFalloffReduction = 3f;

				m_upgraded_turnRate = 0;
				m_upgraded_acceleration = 0;
				m_upgraded_maxSpeed = 0;
				m_upgraded_driftStrenght = 0;
				m_upgraded_maxDriftDegree = 0;
				m_upgraded_driftSpeedLoss = 0;
				m_upgraded_driftStabilization = 0;
				m_upgraded_speedFalloffReduction = 0;

				m_modelID = 1;
				m_skinID = 1;
				m_name = "[TEST] SUPERCAR";
				break;
			}
		case 7:
			{
				m_turnRate = 7.5f;
				m_acceleration = 5f;
				m_maxSpeed = 35.5f;
				m_maxDriftDegree = 90f;
				m_driftStrenght = 8f;
				m_driftSpeedLoss = 10f;
				m_driftStabilization = 1f;
				m_speedFalloffReduction = 4.5f;

				m_upgraded_turnRate = 0;
				m_upgraded_acceleration = 0;
				m_upgraded_maxSpeed = 0;
				m_upgraded_driftStrenght = 0;
				m_upgraded_maxDriftDegree = 0;
				m_upgraded_driftSpeedLoss = 0;
				m_upgraded_driftStabilization = 0;
				m_upgraded_speedFalloffReduction = 0;

				m_modelID = 1;
				m_skinID = 1;
				m_name = "[TEST] F1-CAR";
				break;
			}
		default:
			{
				m_turnRate = 4.5f;
				m_acceleration = 2.5f;
				m_maxSpeed = 30.5f;
				m_maxDriftDegree = 28f;
				m_driftStrenght = 7f;
				m_driftSpeedLoss = 2.15f;
				m_driftStabilization = 3f;
				m_speedFalloffReduction = 1.3f;

				m_upgraded_turnRate = 0;
				m_upgraded_acceleration = 0;
				m_upgraded_maxSpeed = 0;
				m_upgraded_driftStrenght = 0;
				m_upgraded_maxDriftDegree = 0;
				m_upgraded_driftSpeedLoss = 0;
				m_upgraded_driftStabilization = 0;
				m_upgraded_speedFalloffReduction = 0;

				m_modelID = 1;
				m_skinID = 1;
				m_name = "[TEST] AE-86";
				break;
			}
		}
	}

	public string GetCarName()
	{
		return m_name;
	}
	public int GetModelId()
	{
		return m_modelID;
	}
	public int GetSkinId()
	{
		return m_skinID;
	}
	public float GetTurnRate()
	{
		return Mathf.Clamp(m_turnRate + m_upgraded_turnRate, 0, m_maxPossibleTurnrate);
	}
	public float GetAcceleration()
	{
		return Mathf.Clamp (m_acceleration + m_upgraded_acceleration, 0, m_maxPossibleAcceleration);
	}
	public float GetMaxSpeed()
	{
		return Mathf.Clamp(m_maxSpeed + m_upgraded_maxSpeed, 0, m_maxPossibleSpeed);
	}
	public float GetDriftStrenght()
	{
		return Mathf.Clamp(m_driftStrenght + m_upgraded_driftStrenght, 0, m_maxPossibleDriftStrenght);
	}
	public float GetMaxDriftDegree()
	{
		return Mathf.Clamp (m_maxDriftDegree + m_upgraded_maxDriftDegree, 0, m_maxPossibleDriftDegree);
	}
	public float GetSpeedLossOnDrift()
	{
		if (m_driftSpeedLoss - m_upgraded_driftSpeedLoss < 0) {
			return 0;
		}
		return m_driftSpeedLoss - m_upgraded_driftSpeedLoss;
	}
	public float GetDriftStabilization()
	{
		return m_driftStabilization + m_upgraded_driftStabilization;
	}
	public float GetSpeedFalloffStartingPoint()
	{
		return m_speedFalloffReduction + m_upgraded_speedFalloffReduction;
	}
	public float GetMaxPossibleSpeed()
	{
		return m_maxPossibleSpeed;
	}
	public float GetMaxPossibleTurnRate()
	{
		return m_maxPossibleTurnrate;
	}
	public float GetMaxPossibleDriftStrenght()
	{
		return m_maxPossibleDriftStrenght;
	}
	public float GetMaxPossibleDriftDegree()
	{
		return m_maxPossibleDriftDegree;
	}
	public float GetMaxPossibleAcceleration()
	{
		return m_maxPossibleAcceleration;
	}
}
