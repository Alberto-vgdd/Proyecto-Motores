using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour {

	public static PrefabManager currentInstance;

	public GameObject[] envDecoCity;

	void Awake()
	{
		currentInstance = this;
	}
	public GameObject GetRandomDeco(string biome)
	{
		return envDecoCity [Random.Range (0, envDecoCity.Length)];
	}
}
