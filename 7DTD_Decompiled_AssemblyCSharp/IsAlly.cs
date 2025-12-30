using System;
using UnityEngine.Scripting;

// Token: 0x020005D9 RID: 1497
[Preserve]
public class IsAlly : TargetedCompareRequirementBase
{
	// Token: 0x06002F8B RID: 12171 RVA: 0x00145C5C File Offset: 0x00143E5C
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
		bool flag = entityPlayer.IsFriendOfLocalPlayer && !(this.target is EntityPlayerLocal);
		if (!this.invert)
		{
			return flag;
		}
		return !flag;
	}
}
