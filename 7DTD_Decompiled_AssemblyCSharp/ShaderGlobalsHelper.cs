using System;
using UnityEngine;

// Token: 0x020010FC RID: 4348
[ExecuteInEditMode]
public class ShaderGlobalsHelper : MonoBehaviour
{
	// Token: 0x06008861 RID: 34913 RVA: 0x003732FC File Offset: 0x003714FC
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		Shader.SetGlobalFloat("_ElectricShockIntensity", this.electricShockIntensity);
		Shader.SetGlobalColor("_ElectricShockColor", this.electricShockColor);
		Shader.SetGlobalFloat("_ElectricShockSpeed", this.electricShockSpeed);
		Shader.SetGlobalFloat("_ElectricShockScale", this.electricShockScale);
		Shader.SetGlobalTexture("_ElectricShockTexture", this.electricShockTexture);
		Shader.SetGlobalVector("_ElectricShockTexture_ST", this.electricShockTexture_ST);
		Shader.SetGlobalFloat("_ElectricShockTexturePanSpeed", this.electricShockTexturePanSpeed);
	}

	// Token: 0x06008862 RID: 34914 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
	}

	// Token: 0x04006A48 RID: 27208
	[Header("Electric Shock Shader Properties")]
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public float electricShockIntensity = 1f;

	// Token: 0x04006A49 RID: 27209
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public Color electricShockColor = new Color(0.5f, 0.8f, 1f, 1f);

	// Token: 0x04006A4A RID: 27210
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public float electricShockSpeed = 5f;

	// Token: 0x04006A4B RID: 27211
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public float electricShockScale = 2.5f;

	// Token: 0x04006A4C RID: 27212
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public Texture electricShockTexture;

	// Token: 0x04006A4D RID: 27213
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector4 electricShockTexture_ST;

	// Token: 0x04006A4E RID: 27214
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public float electricShockTexturePanSpeed = 10f;

	// Token: 0x04006A4F RID: 27215
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public bool updateEveryFrame;
}
