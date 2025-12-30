using System;
using UnityEngine;

// Token: 0x020010EC RID: 4332
public class Polarizer : MonoBehaviour
{
	// Token: 0x0600881A RID: 34842 RVA: 0x00371630 File Offset: 0x0036F830
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Start()
	{
		Shader shader = GlobalAssets.FindShader("Custom/DebugView");
		if (shader != null)
		{
			this.material = new Material(shader);
		}
	}

	// Token: 0x0600881B RID: 34843 RVA: 0x0037165D File Offset: 0x0036F85D
	public static void SetDebugView(Polarizer.ViewEnums view)
	{
		Polarizer.currDebugView = view;
	}

	// Token: 0x0600881C RID: 34844 RVA: 0x00371665 File Offset: 0x0036F865
	public static Polarizer.ViewEnums GetDebugView()
	{
		return Polarizer.currDebugView;
	}

	// Token: 0x0600881D RID: 34845 RVA: 0x00002914 File Offset: 0x00000B14
	public void OnPreRender()
	{
	}

	// Token: 0x0600881E RID: 34846 RVA: 0x00002914 File Offset: 0x00000B14
	public void OnPostRender()
	{
	}

	// Token: 0x040069F6 RID: 27126
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Material material;

	// Token: 0x040069F7 RID: 27127
	public static Polarizer.ViewEnums currDebugView;

	// Token: 0x020010ED RID: 4333
	public enum ViewEnums
	{
		// Token: 0x040069F9 RID: 27129
		None,
		// Token: 0x040069FA RID: 27130
		Normals,
		// Token: 0x040069FB RID: 27131
		Albedo,
		// Token: 0x040069FC RID: 27132
		Specular
	}
}
