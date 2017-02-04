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
    public PlayerMovement m_CarMovementScript;

    //Input values [-1, 1]
    public float m_CarSpeed;
    public float m_CarSpeedMultiplier;
    public float m_SteerInput;

    //Car variables.
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

    }

    void updateCarState()
    {
        m_CarSpeed = m_CarMovementScript.accumulatedAcceleration;
        m_SteerInput = Input.GetAxis("Steer");
    }

    void rotateMeshes( )
    {
        //The rotation equals linear velocity / wheel radius
        for (int i = 0; i < 4; i++)
        {
          m_TireMeshes[i].Rotate(Vector3.right, m_CarSpeed *m_CarSpeedMultiplier/  m_WheelRadius);
        }
    }

    void steerWheels()
    {
            //Direction wheels (FR, FL)
            for (int i = 0; i < 2; i++)
            {
              m_Wheels[i].localRotation = Quaternion.Euler(0f, m_SteerInput * m_MaxWheelSteerAngle, 0f);
            }
    }
}
