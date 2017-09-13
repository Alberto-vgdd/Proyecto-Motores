using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMaterials : MonoBehaviour
{
	public static CarMaterials currentInstance;

	public Material[] m_Materials;

	void Awake()
	{
		if (currentInstance == null)
		{
			DontDestroyOnLoad(this.gameObject);
			currentInstance = this;
		}
		else
		{
			DestroyObject(this.gameObject);
		}
	}

	public Material GetMaterial(int index)
	{
		return m_Materials[index];
	}
}
