using System;
using UnityEngine.Scripting;

// Token: 0x020002AE RID: 686
[Preserve]
public class DialogRequirementBuff : BaseDialogRequirement
{
	// Token: 0x17000217 RID: 535
	// (get) Token: 0x06001352 RID: 4946 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override BaseDialogRequirement.RequirementTypes RequirementType
	{
		get
		{
			return BaseDialogRequirement.RequirementTypes.Buff;
		}
	}

	// Token: 0x06001353 RID: 4947 RVA: 0x0007685C File Offset: 0x00074A5C
	public override void SetupRequirement()
	{
		string arg = Localization.Get("RequirementBuff_keyword", false);
		base.Description = string.Format("{0} {1}", arg, BuffManager.GetBuff(base.ID).Name);
	}

	// Token: 0x06001354 RID: 4948 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool CheckRequirement(EntityPlayer player, EntityNPC talkingTo)
	{
		return false;
	}

	// Token: 0x04000CC8 RID: 3272
	[PublicizedFrom(EAccessModifier.Private)]
	public string name = "";
}
