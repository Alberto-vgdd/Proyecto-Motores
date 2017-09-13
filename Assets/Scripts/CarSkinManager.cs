using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSkinManager : MonoBehaviour {

	public List<MeshRenderer> MRs;

	public void ChangeBaseSkin(int id)
	{
		for (int i = 0; i < MRs.Count; i++) {
			MRs[i].material = GlobalGameData.currentInstance.m_carSkins [id];
		}
	}

}
