using System;
using UnityEngine;

// Token: 0x02000393 RID: 915
public class DroneLightManager : MonoBehaviour
{
	// Token: 0x06001B4E RID: 6990 RVA: 0x000AB138 File Offset: 0x000A9338
	public void InitMaterials(string key)
	{
		DroneLightManager.LightEffect lightEffect = this.getLightEffect(key);
		if (lightEffect == null)
		{
			Debug.LogWarning("Failed to find drone light with name: " + key, this);
			return;
		}
		for (int i = 0; i < lightEffect.linkedObjects.Length; i++)
		{
			lightEffect.linkedObjects[i].SetActive(true);
		}
		for (int j = 0; j < base.transform.childCount; j++)
		{
			SkinnedMeshRenderer component = base.transform.GetChild(j).GetComponent<SkinnedMeshRenderer>();
			if (component)
			{
				Material[] materials = component.materials;
				for (int k = materials.Length - 1; k >= 0; k--)
				{
					if (materials[k].name.Replace(" (Instance)", "") == lightEffect.material.name)
					{
						materials[k].SetColor("_EmissionColor", lightEffect.material.GetColor("_EmissionColor"));
						break;
					}
				}
			}
		}
	}

	// Token: 0x06001B4F RID: 6991 RVA: 0x000AB224 File Offset: 0x000A9424
	public void DisableMaterials(string key)
	{
		DroneLightManager.LightEffect lightEffect = this.getLightEffect(key);
		if (lightEffect == null)
		{
			Debug.LogWarning("Failed to find drone light with name: " + key, this);
			return;
		}
		for (int i = 0; i < lightEffect.linkedObjects.Length; i++)
		{
			lightEffect.linkedObjects[i].SetActive(false);
		}
		for (int j = 0; j < base.transform.childCount; j++)
		{
			SkinnedMeshRenderer component = base.transform.GetChild(j).GetComponent<SkinnedMeshRenderer>();
			if (component)
			{
				Material[] materials = component.materials;
				for (int k = materials.Length - 1; k >= 0; k--)
				{
					if (materials[k].name.Replace(" (Instance)", "") == lightEffect.material.name)
					{
						materials[k].SetColor("_EmissionColor", Color.black);
						break;
					}
				}
			}
		}
	}

	// Token: 0x06001B50 RID: 6992 RVA: 0x000AB300 File Offset: 0x000A9500
	[PublicizedFrom(EAccessModifier.Private)]
	public DroneLightManager.LightEffect getLightEffect(string key)
	{
		for (int i = 0; i < this.LightEffects.Length; i++)
		{
			if (this.LightEffects[i].material.name == key)
			{
				return this.LightEffects[i];
			}
		}
		return null;
	}

	// Token: 0x04001231 RID: 4657
	public DroneLightManager.LightEffect[] LightEffects;

	// Token: 0x02000394 RID: 916
	[Serializable]
	public class LightEffect
	{
		// Token: 0x04001232 RID: 4658
		public bool startsOn;

		// Token: 0x04001233 RID: 4659
		public Material material;

		// Token: 0x04001234 RID: 4660
		public GameObject[] linkedObjects;
	}
}
