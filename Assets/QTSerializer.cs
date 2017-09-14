using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QTSerializer  {

	float x;
	float y;
	float z;
	float w;

	public QTSerializer(Quaternion qt)
	{
		x = qt.x;
		y = qt.y;
		z = qt.z;
		w = qt.w;
	}
	public Quaternion Get()
	{
		return new Quaternion (x, y, z, w);
	}
}
