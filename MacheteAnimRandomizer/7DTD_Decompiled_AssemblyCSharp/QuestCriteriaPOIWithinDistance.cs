using System;

// Token: 0x020008A1 RID: 2209
public class QuestCriteriaPOIWithinDistance : BaseQuestCriteria
{
	// Token: 0x06004067 RID: 16487 RVA: 0x001A490C File Offset: 0x001A2B0C
	public override bool CheckForQuestGiver(EntityNPC entity)
	{
		int num = 0;
		int.TryParse(this.Value, out num);
		return false;
	}
}
