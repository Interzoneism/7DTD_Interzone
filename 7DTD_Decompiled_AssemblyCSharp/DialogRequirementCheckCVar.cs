using System;
using System.Globalization;
using UnityEngine.Scripting;

// Token: 0x020002AF RID: 687
[Preserve]
public class DialogRequirementCheckCVar : BaseDialogRequirement
{
	// Token: 0x17000218 RID: 536
	// (get) Token: 0x06001356 RID: 4950 RVA: 0x000768A9 File Offset: 0x00074AA9
	public override BaseDialogRequirement.RequirementTypes RequirementType
	{
		get
		{
			return BaseDialogRequirement.RequirementTypes.CVar;
		}
	}

	// Token: 0x06001357 RID: 4951 RVA: 0x000768AD File Offset: 0x00074AAD
	public override bool CheckRequirement(EntityPlayer player, EntityNPC talkingTo)
	{
		int num = (int)player.GetCVar(base.ID);
		LocalPlayerUI.GetUIForPlayer(player as EntityPlayerLocal);
		return num == StringParsers.ParseSInt32(base.Value, 0, -1, NumberStyles.Integer);
	}
}
