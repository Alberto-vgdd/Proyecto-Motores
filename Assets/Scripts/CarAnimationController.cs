using UnityEngine;
using System.Collections;

public class CarAnimationController : MonoBehaviour
{
    // 0 -> FR
    // 1 -> FL
    // 2 -> BR
    // 3 -> BL
    public Transform[] m_Wheels;
    public Transform[] m_TireMeshes;

    //Input values [-1, 1]
    public float m_AccelerationInput;
    public float m_SteerInput;

    //Car variables.
    public float m_MaxCarSpeed; 
    public float m_MaxWheelSteerAngle;
    public float m_WheelRadius;



    void Start ()
    {
        initializeVariables();
    }
	
	void FixedUpdate ()
    {
        updateCarState();
        rotateMeshes();
        steerWheels();
    }





    void initializeVariables()
    {
        m_AccelerationInput = 0.0f;
        m_SteerInput = 0.0f;

        m_MaxCarSpeed = 100.0f;
        m_MaxWheelSteerAngle = 30.0f;
        m_WheelRadius = 1.0f;

    }

    void updateCarState()
    {
        m_AccelerationInput = Input.GetAxis("Vertical");
        m_SteerInput = Input.GetAxis("Horizontal");
    }

    void rotateMeshes( )
    {
        //The rotation equals linear velocity / wheel radius
        for (int i = 0; i < 4; i++)
        {
          m_TireMeshes[i].Rotate(Vector3.right, m_AccelerationInput * m_MaxCarSpeed / m_WheelRadius);
        }
    }

    void steerWheels()
    {
            //Direction wheels (FR, FL)
            for (int i = 0; i < 2; i++)
            {
			m_Wheels[i].localRotation = Quaternion.RotateTowards(m_Wheels[i].localRotation, Quaternion.Euler(0f, m_SteerInput * m_MaxWheelSteerAngle, 0f), Time.deltaTime*100);
            }
    }
}
