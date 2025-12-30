using System;
using System.Globalization;
using UnityEngine.Scripting;

// Token: 0x020002B4 RID: 692
[Preserve]
public class DialogRequirementQuestTier : BaseDialogRequirement
{
	// Token: 0x1700021C RID: 540
	// (get) Token: 0x06001364 RID: 4964 RVA: 0x00075C39 File Offset: 0x00073E39
	public override BaseDialogRequirement.RequirementTypes RequirementType
	{
		get
		{
			return BaseDialogRequirement.RequirementTypes.QuestTier;
		}
	}

	// Token: 0x06001365 RID: 4965 RVA: 0x0002B133 File Offset: 0x00029333
	public override string GetRequiredDescription(EntityPlayer player)
	{
		return "";
	}

	// Token: 0x06001366 RID: 4966 RVA: 0x00076CA8 File Offset: 0x00074EA8
	public override bool CheckRequirement(EntityPlayer player, EntityNPC talkingTo)
	{
		EntityTrader entityTrader = talkingTo as EntityTrader;
		if (entityTrader == null)
		{
			return false;
		}
		int num = StringParsers.ParseSInt32(base.Value, 0, -1, NumberStyles.Integer);
		if (player.QuestJournal.GetCurrentFactionTier(entityTrader.NPCInfo.QuestFaction, 0, false) < num)
		{
			return false;
		}
		for (int i = 0; i < entityTrader.activeQuests.Count; i++)
		{
			if ((int)entityTrader.activeQuests[i].QuestClass.DifficultyTier == num && entityTrader.activeQuests[i].QuestClass.UniqueKey == base.Tag)
			{
				return true;
			}
		}
		return false;
	}
}
