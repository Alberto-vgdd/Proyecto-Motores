using UnityEngine;
using System.Collections;

public class CameraControllerScript : MonoBehaviour
{

    public Transform m_Target;
    public Transform m_Camera;

    public float m_CameraDistance;
    public float m_CameraHeight;
    public float m_CameraDamping;
    public bool m_RearViewCamera;
    public float m_CameraRotationDamping;

    private Vector3 m_DesiredPosition;
    private Quaternion m_DesiredRotation;

    void Start()
    {
        m_CameraDistance = 3.5f;
        m_CameraHeight = 3.0f;
        m_CameraDamping = 15.0f;
        m_RearViewCamera = false;
        m_CameraRotationDamping = 10.0f;
    }

    void Update()
    {
        m_RearViewCamera = Input.GetButton("Rear View Camera");
        MoveCamera();
        RotateCamera();
    }


    void MoveCamera()
    {
        m_DesiredPosition = DesiredPosition();
        m_Camera.position = m_DesiredPosition;
    }



    Vector3 DesiredPosition()
    {
        if (m_RearViewCamera)
        {
            return m_Target.TransformPoint(0, m_CameraHeight, m_CameraDistance);
        }
        else
        {
            return m_Target.TransformPoint(0, m_CameraHeight, -m_CameraDistance);
        }
    }




    void RotateCamera()
    {
        m_DesiredRotation = DesiredRotation();
        m_Camera.rotation = Quaternion.Slerp(m_Camera.rotation, m_DesiredRotation, Time.deltaTime * m_CameraRotationDamping);
    }

    Quaternion DesiredRotation()
    {
        return Quaternion.LookRotation(m_Target.position - m_Camera.position, m_Target.up);
    }





}