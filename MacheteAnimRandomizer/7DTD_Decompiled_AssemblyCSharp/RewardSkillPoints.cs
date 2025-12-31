using System;
using System.Globalization;
using UnityEngine.Scripting;

// Token: 0x02000921 RID: 2337
[Preserve]
public class RewardSkillPoints : BaseReward
{
	// Token: 0x060045AD RID: 17837 RVA: 0x001BD6EC File Offset: 0x001BB8EC
	public override void SetupReward()
	{
		string description = Localization.Get("RewardSkillPoint_keyword", false);
		base.Description = description;
		base.ValueText = string.Format("+{0}", base.Value);
		base.Icon = "ui_game_symbol_skills";
	}

	// Token: 0x060045AE RID: 17838 RVA: 0x001BD72D File Offset: 0x001BB92D
	public override void GiveReward(EntityPlayer player)
	{
		player.Progression.SkillPoints += StringParsers.ParseSInt32(base.Value, 0, -1, NumberStyles.Integer);
	}

	// Token: 0x060045AF RID: 17839 RVA: 0x001BD750 File Offset: 0x001BB950
	public override BaseReward Clone()
	{
		RewardSkillPoints rewardSkillPoints = new RewardSkillPoints();
		base.CopyValues(rewardSkillPoints);
		return rewardSkillPoints;
	}

	// Token: 0x060045B0 RID: 17840 RVA: 0x001BD76B File Offset: 0x001BB96B
	public override string GetRewardText()
	{
		return string.Format("{0} {1}", base.Description, base.ValueText);
	}
}
