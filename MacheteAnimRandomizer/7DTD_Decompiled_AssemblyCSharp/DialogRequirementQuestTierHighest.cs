using System;
using System.Globalization;
using UnityEngine.Scripting;

// Token: 0x020002B5 RID: 693
[Preserve]
public class DialogRequirementQuestTierHighest : BaseDialogRequirement
{
	// Token: 0x1700021D RID: 541
	// (get) Token: 0x06001368 RID: 4968 RVA: 0x00075C39 File Offset: 0x00073E39
	public override BaseDialogRequirement.RequirementTypes RequirementType
	{
		get
		{
			return BaseDialogRequirement.RequirementTypes.QuestTier;
		}
	}

	// Token: 0x06001369 RID: 4969 RVA: 0x0002B133 File Offset: 0x00029333
	public override string GetRequiredDescription(EntityPlayer player)
	{
		return "";
	}

	// Token: 0x0600136A RID: 4970 RVA: 0x00076D48 File Offset: 0x00074F48
	public override bool CheckRequirement(EntityPlayer player, EntityNPC talkingTo)
	{
		int num = StringParsers.ParseSInt32(base.Value, 0, -1, NumberStyles.Integer);
		EntityTrader entityTrader = talkingTo as EntityTrader;
		if (entityTrader == null)
		{
			return false;
		}
		if (player.QuestJournal.GetCurrentFactionTier(entityTrader.NPCInfo.QuestFaction, 0, false) < num)
		{
			return false;
		}
		int num2 = -1;
		if (entityTrader.activeQuests == null)
		{
			return false;
		}
		bool flag = player.GetCVar("DisableQuesting") == 0f;
		for (int i = 0; i < entityTrader.activeQuests.Count; i++)
		{
			QuestClass questClass = entityTrader.activeQuests[i].QuestClass;
			if ((int)questClass.DifficultyTier > num2 && questClass.UniqueKey == base.Tag && (flag || questClass.AlwaysAllow))
			{
				num2 = (int)questClass.DifficultyTier;
			}
		}
		return num == num2;
	}
}
