using System;
using UnityEngine.Scripting;

// Token: 0x02000919 RID: 2329
[Preserve]
public class RewardExp : BaseReward
{
	// Token: 0x06004577 RID: 17783 RVA: 0x001BC5E9 File Offset: 0x001BA7E9
	public override void SetupReward()
	{
		base.Description = Localization.Get("RewardExp_keyword", false);
		this.SetupValueText();
		base.Icon = "ui_game_symbol_trophy";
	}

	// Token: 0x06004578 RID: 17784 RVA: 0x001BC610 File Offset: 0x001BA810
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupValueText()
	{
		float num = Convert.ToSingle(base.Value) * (float)GameStats.GetInt(EnumGameStats.XPMultiplier) / 100f;
		if (num > 214748370f)
		{
			num = 214748370f;
		}
		base.ValueText = num.ToString("{0}");
	}

	// Token: 0x06004579 RID: 17785 RVA: 0x001BC658 File Offset: 0x001BA858
	public override void GiveReward(EntityPlayer player)
	{
		int exp = Convert.ToInt32(base.Value);
		player.Progression.AddLevelExp(exp, "_xpFromQuest", Progression.XPTypes.Quest, true, true);
	}

	// Token: 0x0600457A RID: 17786 RVA: 0x001BC688 File Offset: 0x001BA888
	public override BaseReward Clone()
	{
		RewardExp rewardExp = new RewardExp();
		base.CopyValues(rewardExp);
		return rewardExp;
	}

	// Token: 0x0600457B RID: 17787 RVA: 0x001BC6A3 File Offset: 0x001BA8A3
	public override void ParseProperties(DynamicProperties properties)
	{
		base.ParseProperties(properties);
		if (properties.Values.ContainsKey(RewardExp.PropExp))
		{
			base.Value = properties.Values[RewardExp.PropExp];
		}
	}

	// Token: 0x04003664 RID: 13924
	public static string PropExp = "xp";

	// Token: 0x04003665 RID: 13925
	[PublicizedFrom(EAccessModifier.Private)]
	public static FastTags<TagGroup.Global> questTag = FastTags<TagGroup.Global>.Parse("quest");
}
