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
	private float m_driftSpeedConservation;
	private float m_driftStabilization;
	private float m_speedFalloffReduction;

	// Bonus car stats (upgrades)

	private float m_upgraded_turnRate;
	private float m_upgraded_acceleration;
	private float m_upgraded_maxSpeed;
	private float m_upgraded_driftStrenght;
	private float m_upgraded_maxDriftDegree;
	private float m_upgraded_driftSpeedConservation;
	private float m_upgraded_driftStabilization;
	private float m_upgraded_speedFalloffReduction;

	// Other car data

	private int m_modelID;
	private int m_skinID;
	private string m_name;

	// Constructor sin parametros, coche de prueba.
	public CarData(int preset = 0)
	{
		switch (preset) {
		case 1:
			{
				m_turnRate = 4f;
				m_acceleration = 6.5f;
				m_maxSpeed = 8f;
				m_maxDriftDegree = 8f;
				m_driftStrenght = 4f;
				m_driftSpeedConservation = 3f;
				m_driftStabilization = 4f;
				m_speedFalloffReduction = 2f;

				m_upgraded_turnRate = 0;
				m_upgraded_acceleration = 0;
				m_upgraded_maxSpeed = 0;
				m_upgraded_driftStrenght = 0;
				m_upgraded_maxDriftDegree = 0;
				m_upgraded_driftSpeedConservation = 0;
				m_upgraded_driftStabilization = 0;
				m_upgraded_speedFalloffReduction = 0;

				m_modelID = 1;
				m_skinID = 1;
				m_name = "[TEST] SPORTS CAR";
				break;
			}
		case 2:
			{
				m_turnRate = 3f;
				m_acceleration = 6f;
				m_maxSpeed = 6f;
				m_maxDriftDegree = 4f;
				m_driftStrenght = 7f;
				m_driftSpeedConservation = 5f;
				m_driftStabilization = 4f;
				m_speedFalloffReduction = 2f;

				m_upgraded_turnRate = -1f;
				m_upgraded_acceleration = 2.1f;
				m_upgraded_maxSpeed = 1.5f;
				m_upgraded_driftStrenght = -3.7f;
				m_upgraded_maxDriftDegree = 0.5f;
				m_upgraded_driftSpeedConservation = -2f;
				m_upgraded_driftStabilization = 0;
				m_upgraded_speedFalloffReduction = 0;

				m_modelID = 1;
				m_skinID = 1;
				m_name = "[TEST] MUSCLE CAR";
				break;
			}
		case 3:
			{
				m_turnRate = 4f;
				m_acceleration = 8f;
				m_maxSpeed = 9f;
				m_maxDriftDegree = 4f;
				m_driftStrenght = 5f;
				m_driftSpeedConservation = 5f;
				m_driftStabilization = 4f;
				m_speedFalloffReduction = 2f;

				m_upgraded_turnRate = 1f;
				m_upgraded_acceleration = 1.5f;
				m_upgraded_maxSpeed = 1f;
				m_upgraded_driftStrenght = -1.5f;
				m_upgraded_maxDriftDegree = -1.5f;
				m_upgraded_driftSpeedConservation = -3.5f;
				m_upgraded_driftStabilization = 0;
				m_upgraded_speedFalloffReduction = 0;

				m_modelID = 1;
				m_skinID = 1;
				m_name = "[TEST] LMP-ONE";
				break;
			}
		default:
			{
				m_turnRate = 3.5f;
				m_acceleration = 5f;
				m_maxSpeed = 4.5f;
				m_maxDriftDegree = 5f;
				m_driftStrenght = 6f;
				m_driftSpeedConservation = 8f;
				m_driftStabilization = 3f;
				m_speedFalloffReduction = 2f;

				m_upgraded_turnRate = 1f;
				m_upgraded_acceleration = 2.4f;
				m_upgraded_maxSpeed = -0.55f;
				m_upgraded_driftStrenght = 1.25f;
				m_upgraded_maxDriftDegree = 0;
				m_upgraded_driftSpeedConservation = -1.75f;
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
		return Mathf.Clamp(m_turnRate + m_upgraded_turnRate, 0, 10);
	}
	public float GetAcceleration()
	{
		return Mathf.Clamp (m_acceleration + m_upgraded_acceleration, 0, 10);
	}
	public float GetMaxSpeed()
	{
		return Mathf.Clamp(m_maxSpeed + m_upgraded_maxSpeed, 0,10);
	}
	public float GetDriftStrenght()
	{
		return Mathf.Clamp(m_driftStrenght + m_upgraded_driftStrenght, 0, 10);
	}
	public float GetMaxDriftDegree()
	{
		return Mathf.Clamp (m_maxDriftDegree + m_upgraded_maxDriftDegree, 0, 10);
	}
	public float GetSpeedLossOnDrift()
	{
		return Mathf.Clamp (m_driftSpeedConservation + m_upgraded_driftSpeedConservation, 0, 10);
	}
	public float GetDriftStabilization()
	{
		return m_driftStabilization + m_upgraded_driftStabilization;
	}
	public float GetSpeedFalloffStartingPoint()
	{
		return m_speedFalloffReduction + m_upgraded_speedFalloffReduction;
	}

	// BASE STAT GETTERS
	public float GetBaseMaxSpeed()
	{
		return m_maxSpeed;
	}
	public float GetBaseAcceleration()
	{
		return m_acceleration;
	}
	public float GetBaseTurnRate()
	{
		return m_turnRate;
	}
	public float GetBaseDriftStrenght()
	{
		return m_driftStrenght;
	}
	public float GetBaseDriftSpdCons()
	{
		return m_driftSpeedConservation;
	}
	// UPGRADED STAT GETTERS
	public float GetUpgradedMaxSpeed()
	{
		return m_upgraded_maxSpeed;
	}
	public float GetUpgradedAcceleration()
	{
		return m_upgraded_acceleration;
	}
	public float GetUpgradedTurnRate()
	{
		return m_upgraded_turnRate;
	}
	public float GetUpgradedDriftStrenght()
	{
		return m_upgraded_driftStrenght;
	}
	public float GetUpgradedDriftSpdCons()
	{
		return m_upgraded_driftSpeedConservation;
	}
}
