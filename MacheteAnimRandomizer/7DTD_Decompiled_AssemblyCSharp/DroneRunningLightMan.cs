using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000399 RID: 921
[Preserve]
public class DroneRunningLightMan
{
	// Token: 0x06001B84 RID: 7044 RVA: 0x000AC7B9 File Offset: 0x000AA9B9
	public DroneRunningLightMan()
	{
		DroneRunningLightMan.instance = this;
		this.runningLights = new List<DroneRunningLight>();
	}

	// Token: 0x06001B85 RID: 7045 RVA: 0x000AC7D2 File Offset: 0x000AA9D2
	public void AddLight(DroneRunningLight _light)
	{
		this.runningLights.Add(_light);
	}

	// Token: 0x06001B86 RID: 7046 RVA: 0x00002914 File Offset: 0x00000B14
	public void QueueLight(DroneRunningLight _light)
	{
	}

	// Token: 0x04001259 RID: 4697
	[PublicizedFrom(EAccessModifier.Private)]
	public List<DroneRunningLight> runningLights;

	// Token: 0x0400125A RID: 4698
	public static DroneRunningLightMan instance;
}
