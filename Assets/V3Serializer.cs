using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class V3Serializer {
	float x;
	float y;
	float z;

	public V3Serializer(Vector3 v3)
	{
		x = v3.x;
		y = v3.y;
		z = v3.z;
	}
	public Vector3 Get()
	{
		return new Vector3 (x, y, z);
	}

}
