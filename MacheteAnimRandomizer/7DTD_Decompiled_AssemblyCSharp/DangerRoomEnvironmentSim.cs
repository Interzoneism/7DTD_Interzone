using System;
using UnityEngine;

// Token: 0x02000016 RID: 22
[ExecuteInEditMode]
public class DangerRoomEnvironmentSim : MonoBehaviour
{
	// Token: 0x0600008C RID: 140 RVA: 0x00009130 File Offset: 0x00007330
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (this.simulateWind)
		{
			Shader.SetGlobalVector("_Wind", new Vector4(this.wind, 0f, 0f, 0f));
			return;
		}
		Shader.SetGlobalVector("_Wind", new Vector4(0f, 0f, 0f, 0f));
	}

	// Token: 0x040000C2 RID: 194
	public bool simulateWind = true;

	// Token: 0x040000C3 RID: 195
	[Range(0f, 100f)]
	public float wind = 100f;
}
