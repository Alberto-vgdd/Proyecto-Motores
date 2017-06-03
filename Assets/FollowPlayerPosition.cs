using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerPosition : MonoBehaviour 
{

    public Transform m_PlayerTransform;




	
	// Update is called once per frame
	void Update () 
    {
        transform.position = new Vector3(m_PlayerTransform.position.x, -0.3f, m_PlayerTransform.position.z);
	}
}
