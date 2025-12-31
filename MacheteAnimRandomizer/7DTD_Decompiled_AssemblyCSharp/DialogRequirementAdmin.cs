using System;
using UnityEngine.Scripting;

// Token: 0x020002AD RID: 685
[Preserve]
public class DialogRequirementAdmin : BaseDialogRequirement
{
	// Token: 0x17000216 RID: 534
	// (get) Token: 0x0600134E RID: 4942 RVA: 0x000583BD File Offset: 0x000565BD
	public override BaseDialogRequirement.RequirementTypes RequirementType
	{
		get
		{
			return BaseDialogRequirement.RequirementTypes.Admin;
		}
	}

	// Token: 0x0600134F RID: 4943 RVA: 0x00076820 File Offset: 0x00074A20
	public override void SetupRequirement()
	{
		string description = Localization.Get("RequirementAdmin_keyword", false);
		base.Description = description;
	}

	// Token: 0x06001350 RID: 4944 RVA: 0x00076840 File Offset: 0x00074A40
	public override bool CheckRequirement(EntityPlayer player, EntityNPC talkingTo)
	{
		return GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled);
	}

	// Token: 0x04000CC7 RID: 3271
	[PublicizedFrom(EAccessModifier.Private)]
	public string name = "";
}
