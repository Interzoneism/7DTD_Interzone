using System;
using UnityEngine;

// Token: 0x0200133D RID: 4925
public class vp_HealthPickup : vp_Pickup
{
	// Token: 0x06009906 RID: 39174 RVA: 0x003CDD50 File Offset: 0x003CBF50
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool TryGive(vp_FPPlayerEventHandler player)
	{
		if (player.Health.Get() < 0f)
		{
			return false;
		}
		if (player.Health.Get() >= player.MaxHealth.Get())
		{
			return false;
		}
		player.Health.Set(Mathf.Min(player.MaxHealth.Get(), player.Health.Get() + this.Health));
		return true;
	}

	// Token: 0x040075A4 RID: 30116
	public float Health = 1f;
}
