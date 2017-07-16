using System.Collections;
using System.Collections.Generic;

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
				break;
			}
		}
	}

	public float GetTurnRate()
	{
		return m_turnRate + m_upgraded_turnRate;
	}
	public float GetAcceleration()
	{
		return m_acceleration + m_upgraded_acceleration;
	}
	public float GetMaxSpeed()
	{
		return m_maxSpeed + m_upgraded_maxSpeed;
	}
	public float GetDriftStrenght()
	{
		return m_driftStrenght + m_upgraded_driftStrenght;
	}
	public float GetMaxDriftDegree()
	{
		return m_maxDriftDegree + m_upgraded_maxDriftDegree;
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
}
