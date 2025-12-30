using System;
using UnityEngine.Scripting;

// Token: 0x02000920 RID: 2336
[Preserve]
public class RewardSkill : BaseReward
{
	// Token: 0x060045A9 RID: 17833 RVA: 0x001BD5A8 File Offset: 0x001BB7A8
	public override void SetupReward()
	{
		string arg = Localization.Get("RewardSkill_keyword", false);
		base.Description = string.Format("{0} {1}", base.ID, arg);
		base.ValueText = base.Value;
		base.Icon = base.OwnerQuest.OwnerJournal.OwnerPlayer.Progression.GetProgressionValue(base.ID).ProgressionClass.Icon;
	}

	// Token: 0x060045AA RID: 17834 RVA: 0x001BD614 File Offset: 0x001BB814
	public override void GiveReward(EntityPlayer player)
	{
		ProgressionValue progressionValue = player.Progression.GetProgressionValue(base.ID);
		int num = Convert.ToInt32(base.Value);
		if (progressionValue != null)
		{
			if (progressionValue.Level + num > progressionValue.ProgressionClass.MaxLevel)
			{
				progressionValue.Level = progressionValue.ProgressionClass.MaxLevel;
			}
			else
			{
				progressionValue.Level += num;
			}
			if (progressionValue.ProgressionClass.IsPerk)
			{
				player.MinEventContext.ProgressionValue = progressionValue;
				player.FireEvent(MinEventTypes.onPerkLevelChanged, true);
			}
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEntitySetSkillLevelServer>().Setup(player.entityId, progressionValue.Name, progressionValue.Level), false);
			}
		}
	}

	// Token: 0x060045AB RID: 17835 RVA: 0x001BD6D0 File Offset: 0x001BB8D0
	public override BaseReward Clone()
	{
		RewardSkill rewardSkill = new RewardSkill();
		base.CopyValues(rewardSkill);
		return rewardSkill;
	}
}
