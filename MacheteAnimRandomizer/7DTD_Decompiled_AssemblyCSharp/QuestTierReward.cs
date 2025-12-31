using System;
using System.Collections.Generic;

// Token: 0x02000914 RID: 2324
public class QuestTierReward
{
	// Token: 0x06004542 RID: 17730 RVA: 0x001BBD54 File Offset: 0x001B9F54
	public void GiveRewards(EntityPlayer player)
	{
		for (int i = 0; i < this.Rewards.Count; i++)
		{
			this.Rewards[i].GiveReward(player);
		}
	}

	// Token: 0x04003637 RID: 13879
	public int Tier;

	// Token: 0x04003638 RID: 13880
	public List<BaseReward> Rewards = new List<BaseReward>();
}
