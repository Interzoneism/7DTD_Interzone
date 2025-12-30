using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x0200060F RID: 1551
[Preserve]
public class InSafeZone : TargetedCompareRequirementBase
{
	// Token: 0x06003037 RID: 12343 RVA: 0x001482D0 File Offset: 0x001464D0
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		EntityPlayer entityPlayer = this.target as EntityPlayer;
		if (entityPlayer == null)
		{
			return false;
		}
		if (!this.invert)
		{
			return entityPlayer.TwitchSafe;
		}
		return !entityPlayer.TwitchSafe;
	}

	// Token: 0x06003038 RID: 12344 RVA: 0x00148311 File Offset: 0x00146511
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("Is {0} In Safe Zone", this.invert ? "NOT " : ""));
	}
}
