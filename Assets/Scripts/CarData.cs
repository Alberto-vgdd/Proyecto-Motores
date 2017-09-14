using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CarData {

	// Base car stats.

	private float m_turnRate;
	private float m_acceleration;
	private float m_maxSpeed;
	private float m_driftStrenght;
	private float m_maxDriftDegree;
	private float m_carWeight;

	// Bonus car stats (upgrades)

	private float m_upgraded_turnRate;
	private float m_upgraded_acceleration;
	private float m_upgraded_maxSpeed;
	private float m_upgraded_driftStrenght;
	private float m_upgraded_maxDriftDegree;
	private float m_upgraded_carWeight;

	// Other car data

	private int m_modelID;
	private int m_skinID;
	private string m_name;
	private int m_MaterialIndex;

	// Constructor sin parametros, coche de prueba.
	public CarData(int preset = 0)
	{
		switch (preset) {
		case 0:
			{
				m_turnRate = 3.5f;
				m_acceleration = 5f;
				m_maxSpeed = 4.5f;
				m_maxDriftDegree = 5f;
				m_driftStrenght = 6f;
				m_carWeight = 6.5f;

				m_upgraded_turnRate = 1f;
				m_upgraded_acceleration = 2.7f;
				m_upgraded_maxSpeed = 0.3f;
				m_upgraded_driftStrenght = 1.25f;
				m_upgraded_maxDriftDegree = 0;
				m_upgraded_carWeight = 0f;

				m_modelID = 1;
				m_skinID = 0;
				m_name = "Wave86 Sport";
				m_MaterialIndex = 0;
				break;
			}
		case 1:
			{
				m_turnRate = 3.5f;
				m_acceleration = 5f;
				m_maxSpeed = 4.5f;
				m_maxDriftDegree = 5f;
				m_driftStrenght = 6f;
				m_carWeight = 6.5f;

				m_upgraded_turnRate = 2f;
				m_upgraded_acceleration = 3f;
				m_upgraded_maxSpeed = 0.65f;
				m_upgraded_driftStrenght = 0.25f;
				m_upgraded_maxDriftDegree = -0.25f;
				m_upgraded_carWeight = 1f;

				m_modelID = 1;
				m_skinID = 1;
				m_name = "Wave86 Type-R";
				m_MaterialIndex = 1;
				break;
			}
		case 2:
			{
				m_turnRate = 3.5f;
				m_acceleration = 5f;
				m_maxSpeed = 4.5f;
				m_maxDriftDegree = 5f;
				m_driftStrenght = 6f;
				m_carWeight = 6.5f;

				m_upgraded_turnRate = -1f;
				m_upgraded_acceleration = 1.75f;
				m_upgraded_maxSpeed = 3f;
				m_upgraded_driftStrenght = 1f;
				m_upgraded_maxDriftDegree = -1;
				m_upgraded_carWeight = 2f;

				m_modelID = 1;
				m_skinID = 2;
				m_name = "Wave86 Turbo";
				m_MaterialIndex = 2;

				break;
			}
		case 3:
			{
				m_turnRate = 3.5f;
				m_acceleration = 5f;
				m_maxSpeed = 4.5f;
				m_maxDriftDegree = 5f;
				m_driftStrenght = 6f;
				m_carWeight = 6.5f;

				m_upgraded_turnRate = 3.5f;
				m_upgraded_acceleration = 4.5f;
				m_upgraded_maxSpeed = -2f;
				m_upgraded_driftStrenght = 3f;
				m_upgraded_maxDriftDegree = 3;
				m_upgraded_carWeight = -2f;

				m_modelID = 1;
				m_skinID = 3;
				m_name = "Wave86 Rally edition";
				m_MaterialIndex = 3;
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
	public int GetMaterialIndex()
	{
		return m_MaterialIndex;
	}

	// COMBINED STAT GETTERS
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
	public float GetCarWeight()
	{
		return Mathf.Clamp (m_carWeight + m_upgraded_carWeight, 0, 10);
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
	public float GetBaseWeight()
	{
		return m_carWeight;
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
	public float GetUpgradedWeight()
	{
		return m_upgraded_carWeight;
	}
}
