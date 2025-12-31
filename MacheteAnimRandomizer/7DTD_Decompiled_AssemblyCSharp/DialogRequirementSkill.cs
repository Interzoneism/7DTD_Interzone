using System;
using UnityEngine.Scripting;

// Token: 0x020002B6 RID: 694
[Preserve]
public class DialogRequirementSkill : BaseDialogRequirement
{
	// Token: 0x1700021E RID: 542
	// (get) Token: 0x0600136C RID: 4972 RVA: 0x00076E19 File Offset: 0x00075019
	public override BaseDialogRequirement.RequirementTypes RequirementType
	{
		get
		{
			return BaseDialogRequirement.RequirementTypes.Skill;
		}
	}

	// Token: 0x0600136D RID: 4973 RVA: 0x00076E1C File Offset: 0x0007501C
	public override string GetRequiredDescription(EntityPlayer player)
	{
		ProgressionValue progressionValue = player.Progression.GetProgressionValue(base.ID);
		return string.Format("({0} {1})", Localization.Get(progressionValue.ProgressionClass.NameKey, false), Convert.ToInt32(base.Value));
	}

	// Token: 0x0600136E RID: 4974 RVA: 0x00076E66 File Offset: 0x00075066
	public override bool CheckRequirement(EntityPlayer player, EntityNPC talkingTo)
	{
		return player.Progression.GetProgressionValue(base.ID).Level > Convert.ToInt32(base.Value);
	}
}
