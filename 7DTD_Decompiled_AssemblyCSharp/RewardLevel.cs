using System;
using UnityEngine.Scripting;

// Token: 0x0200091B RID: 2331
[Preserve]
public class RewardLevel : BaseReward
{
	// Token: 0x06004588 RID: 17800 RVA: 0x001BCC91 File Offset: 0x001BAE91
	public override void SetupReward()
	{
		base.Description = string.Format("{0}", Localization.Get("RewardLevel_keyword", false));
		base.ValueText = base.Value;
		base.Icon = "ui_game_symbol_trophy";
	}

	// Token: 0x06004589 RID: 17801 RVA: 0x001BCCC8 File Offset: 0x001BAEC8
	public override void GiveReward(EntityPlayer player)
	{
		int num = Convert.ToInt32(base.Value);
		float levelProgressPercentage = player.Progression.GetLevelProgressPercentage();
		for (int i = 0; i < num; i++)
		{
			player.Progression.AddLevelExp(player.Progression.ExpToNextLevel, "_xpOther", Progression.XPTypes.Other, true, true);
		}
		player.Progression.AddLevelExp((int)(levelProgressPercentage * (float)player.Progression.GetExpForNextLevel()), "_xpOther", Progression.XPTypes.Other, true, true);
	}

	// Token: 0x0600458A RID: 17802 RVA: 0x001BCD3C File Offset: 0x001BAF3C
	public override BaseReward Clone()
	{
		RewardLevel rewardLevel = new RewardLevel();
		base.CopyValues(rewardLevel);
		return rewardLevel;
	}
}
