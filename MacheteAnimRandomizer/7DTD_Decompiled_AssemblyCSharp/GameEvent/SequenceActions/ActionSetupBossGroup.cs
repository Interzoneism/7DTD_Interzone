using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016A8 RID: 5800
	[Preserve]
	public class ActionSetupBossGroup : BaseAction
	{
		// Token: 0x0600B06E RID: 45166 RVA: 0x0044E7F0 File Offset: 0x0044C9F0
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			List<Entity> entityGroup = base.Owner.GetEntityGroup(this.bossGroupName);
			List<Entity> entityGroup2 = base.Owner.GetEntityGroup(this.minionGroupName);
			List<EntityAlive> list = new List<EntityAlive>();
			EntityAlive boss = entityGroup[0] as EntityAlive;
			for (int i = 0; i < entityGroup2.Count; i++)
			{
				EntityAlive entityAlive = entityGroup2[i] as EntityAlive;
				if (entityAlive != null)
				{
					list.Add(entityAlive);
				}
			}
			EntityPlayer entityPlayer = base.Owner.Target as EntityPlayer;
			if (entityPlayer != null)
			{
				base.Owner.CurrentBossGroupID = GameEventManager.Current.SetupBossGroup(entityPlayer, boss, list, this.bossGroupType, this.bossIcon1);
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600B06F RID: 45167 RVA: 0x0044E8A4 File Offset: 0x0044CAA4
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionSetupBossGroup.PropMinionGroupName, ref this.minionGroupName);
			properties.ParseString(ActionSetupBossGroup.PropBossGroupName, ref this.bossGroupName);
			properties.ParseString(ActionSetupBossGroup.PropBossIcon1, ref this.bossIcon1);
			properties.ParseEnum<BossGroup.BossGroupTypes>(ActionSetupBossGroup.PropGroupType, ref this.bossGroupType);
		}

		// Token: 0x0600B070 RID: 45168 RVA: 0x0044E8FC File Offset: 0x0044CAFC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionSetupBossGroup
			{
				minionGroupName = this.minionGroupName,
				bossGroupName = this.bossGroupName,
				bossGroupType = this.bossGroupType,
				bossIcon1 = this.bossIcon1
			};
		}

		// Token: 0x040089F3 RID: 35315
		[PublicizedFrom(EAccessModifier.Protected)]
		public string minionGroupName = "";

		// Token: 0x040089F4 RID: 35316
		[PublicizedFrom(EAccessModifier.Protected)]
		public string bossGroupName = "";

		// Token: 0x040089F5 RID: 35317
		[PublicizedFrom(EAccessModifier.Protected)]
		public string bossIcon1 = "";

		// Token: 0x040089F6 RID: 35318
		[PublicizedFrom(EAccessModifier.Protected)]
		public BossGroup.BossGroupTypes bossGroupType;

		// Token: 0x040089F7 RID: 35319
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropMinionGroupName = "minion_group_name";

		// Token: 0x040089F8 RID: 35320
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropBossGroupName = "boss_group_name";

		// Token: 0x040089F9 RID: 35321
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropBossIcon1 = "boss_icon1";

		// Token: 0x040089FA RID: 35322
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropGroupType = "group_type";
	}
}
