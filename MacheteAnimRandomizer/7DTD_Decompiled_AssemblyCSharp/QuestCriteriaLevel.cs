using System;

// Token: 0x020008A2 RID: 2210
public class QuestCriteriaLevel : BaseQuestCriteria
{
	// Token: 0x06004069 RID: 16489 RVA: 0x001A4934 File Offset: 0x001A2B34
	public override bool CheckForPlayer(EntityPlayer player)
	{
		int num = 0;
		return int.TryParse(this.Value, out num) && player.Progression.Level >= num;
	}
}
