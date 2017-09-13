using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSkinManager : MonoBehaviour {

	public List<MeshRenderer> MeshRenderers;
	public MeshRenderer specialCase1;
	public MeshRenderer specialCase2;

	public void ChangeBaseSkin(int id)
	{
		for (int i = 0; i < MeshRenderers.Count; i++) {
			MeshRenderers[i].material = GlobalGameData.currentInstance.m_carSkins [id];
		}

		// TODO: me duele escribir esto pero unity no quiere otra solucion.
		if (specialCase1 != null) {
			Material[] tempMat = specialCase1.materials;
			tempMat [1] = GlobalGameData.currentInstance.m_carSkins [id];
			specialCase1.materials = tempMat;
		}
		if (specialCase2 != null) {
			Material[] tempMat = specialCase2.materials;
			tempMat [1] = GlobalGameData.currentInstance.m_carSkins [id];
			specialCase2.materials = tempMat;
		}

	}

}
