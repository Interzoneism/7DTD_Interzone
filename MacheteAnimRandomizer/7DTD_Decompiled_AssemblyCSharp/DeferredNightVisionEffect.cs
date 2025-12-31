using System;
using UnityEngine;

// Token: 0x0200006E RID: 110
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class DeferredNightVisionEffect : MonoBehaviour
{
	// Token: 0x1700003B RID: 59
	// (get) Token: 0x060001ED RID: 493 RVA: 0x0001100A File Offset: 0x0000F20A
	public Shader NightVisionShader
	{
		get
		{
			return this.m_Shader;
		}
	}

	// Token: 0x060001EE RID: 494 RVA: 0x00011012 File Offset: 0x0000F212
	[PublicizedFrom(EAccessModifier.Private)]
	public void DestroyMaterial(Material mat)
	{
		if (mat)
		{
			UnityEngine.Object.DestroyImmediate(mat);
			mat = null;
		}
	}

	// Token: 0x060001EF RID: 495 RVA: 0x00011028 File Offset: 0x0000F228
	[PublicizedFrom(EAccessModifier.Private)]
	public void CreateMaterials()
	{
		if (this.m_Shader == null)
		{
			this.m_Shader = Shader.Find("Custom/DeferredNightVisionShader");
		}
		if (this.m_Material == null && this.m_Shader != null && this.m_Shader.isSupported)
		{
			this.m_Material = this.CreateMaterial(this.m_Shader);
		}
	}

	// Token: 0x060001F0 RID: 496 RVA: 0x0001108E File Offset: 0x0000F28E
	[PublicizedFrom(EAccessModifier.Private)]
	public Material CreateMaterial(Shader shader)
	{
		if (!shader)
		{
			return null;
		}
		return new Material(shader)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
	}

	// Token: 0x060001F1 RID: 497 RVA: 0x000110A8 File Offset: 0x0000F2A8
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDisable()
	{
		this.DestroyMaterial(this.m_Material);
		this.m_Material = null;
		this.m_Shader = null;
	}

	// Token: 0x060001F2 RID: 498 RVA: 0x000110C4 File Offset: 0x0000F2C4
	[ContextMenu("UpdateShaderValues")]
	public void UpdateShaderValues()
	{
		if (this.m_Material == null)
		{
			return;
		}
		this.m_Material.SetVector("_NVColor", this.m_NVColor);
		this.m_Material.SetVector("_TargetWhiteColor", this.m_TargetBleachColor);
		this.m_Material.SetFloat("_BaseLightingContribution", this.m_baseLightingContribution);
		this.m_Material.SetFloat("_LightSensitivityMultiplier", this.m_LightSensitivityMultiplier);
		this.m_Material.shaderKeywords = null;
		if (this.useVignetting)
		{
			Shader.EnableKeyword("USE_VIGNETTE");
			return;
		}
		Shader.DisableKeyword("USE_VIGNETTE");
	}

	// Token: 0x060001F3 RID: 499 RVA: 0x0001116B File Offset: 0x0000F36B
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		this.CreateMaterials();
		this.UpdateShaderValues();
	}

	// Token: 0x060001F4 RID: 500 RVA: 0x00011179 File Offset: 0x0000F379
	public void ReloadShaders()
	{
		this.OnDisable();
	}

	// Token: 0x060001F5 RID: 501 RVA: 0x00011181 File Offset: 0x0000F381
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		this.UpdateShaderValues();
		this.CreateMaterials();
		Graphics.Blit(source, destination, this.m_Material);
	}

	// Token: 0x040002BB RID: 699
	[SerializeField]
	[Tooltip("The main color of the NV effect")]
	public Color m_NVColor = new Color(0f, 1f, 0.1724138f, 0f);

	// Token: 0x040002BC RID: 700
	[SerializeField]
	[Tooltip("The color that the NV effect will 'bleach' towards (white = default)")]
	public Color m_TargetBleachColor = new Color(1f, 1f, 1f, 0f);

	// Token: 0x040002BD RID: 701
	[Range(0f, 1f)]
	[Tooltip("How much base lighting does the NV effect pick up")]
	public float m_baseLightingContribution = 0.025f;

	// Token: 0x040002BE RID: 702
	[Range(0f, 128f)]
	[Tooltip("The higher this value, the more bright areas will get 'bleached out'")]
	public float m_LightSensitivityMultiplier = 100f;

	// Token: 0x040002BF RID: 703
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Material m_Material;

	// Token: 0x040002C0 RID: 704
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Shader m_Shader;

	// Token: 0x040002C1 RID: 705
	[Tooltip("Do we want to apply a vignette to the edges of the screen?")]
	public bool useVignetting = true;
}
