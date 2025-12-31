using System;
using UnityEngine;

// Token: 0x020010D7 RID: 4311
[CreateAssetMenu(fileName = "HairColorSwatch", menuName = "Hair Color Management/Hair Color Swatch", order = 1)]
public class HairColorSwatch : ScriptableObject
{
	// Token: 0x060087C1 RID: 34753 RVA: 0x0036EED8 File Offset: 0x0036D0D8
	public void ApplyToMaterial(Material material)
	{
		material.SetColor("_Tint1", this.tint1);
		material.SetColor("_Tint2", this.tint2);
		material.SetColor("_Tint3", this.tint3);
		material.SetFloat("_TintSharpness", this.tintSharpness);
		material.SetFloat("_IDMapStrength", this.idMapStrength);
		material.SetFloat("_RootDarkening", this.rootDarkening);
		material.SetFloat("_Metallic", this.metallic);
		material.SetColor("_CuticleSpecularColor", this.cuticleSpecularColor);
		material.SetColor("_CortexSpecularColor", this.cortexSpecularColor);
		material.SetFloat("_IndirectSpecularStrength", this.indirectSpecularStrength);
		material.SetColor("_SubsurfaceAmbient", this.subsurfaceAmbient);
		material.SetColor("_SubsurfaceColor", this.subsurfaceColor);
	}

	// Token: 0x060087C2 RID: 34754 RVA: 0x0036EFB4 File Offset: 0x0036D1B4
	public void ApplySwatchToGameObject(GameObject targetGameObject)
	{
		Shader y = Shader.Find("Game/SDCS/Hair");
		if (targetGameObject != null)
		{
			foreach (Renderer renderer in targetGameObject.GetComponentsInChildren<Renderer>(true))
			{
				Material[] array;
				if (Application.isPlaying)
				{
					array = renderer.materials;
				}
				else
				{
					array = renderer.sharedMaterials;
				}
				foreach (Material material in array)
				{
					if (material.shader == y)
					{
						this.ApplyToMaterial(material);
					}
				}
			}
			return;
		}
		Debug.LogWarning("No target GameObject selected.");
	}

	// Token: 0x04006984 RID: 27012
	[ColorUsage(false, false)]
	public Color tint1 = Color.red;

	// Token: 0x04006985 RID: 27013
	[ColorUsage(false, false)]
	public Color tint2 = Color.green;

	// Token: 0x04006986 RID: 27014
	[ColorUsage(false, false)]
	public Color tint3 = Color.blue;

	// Token: 0x04006987 RID: 27015
	[Range(0f, 1f)]
	public float tintSharpness = 0.5f;

	// Token: 0x04006988 RID: 27016
	[Range(0f, 1f)]
	public float idMapStrength;

	// Token: 0x04006989 RID: 27017
	[Range(0f, 1f)]
	public float rootDarkening;

	// Token: 0x0400698A RID: 27018
	[Range(0f, 1f)]
	public float metallic;

	// Token: 0x0400698B RID: 27019
	[ColorUsage(false, true)]
	public Color cuticleSpecularColor = Color.white;

	// Token: 0x0400698C RID: 27020
	[ColorUsage(false, true)]
	public Color cortexSpecularColor = Color.white;

	// Token: 0x0400698D RID: 27021
	[Range(0f, 1f)]
	public float indirectSpecularStrength;

	// Token: 0x0400698E RID: 27022
	[ColorUsage(false, false)]
	public Color subsurfaceAmbient = Color.white;

	// Token: 0x0400698F RID: 27023
	[ColorUsage(false, false)]
	public Color subsurfaceColor = Color.white;
}
