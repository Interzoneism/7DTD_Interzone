using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016BE RID: 5822
	[Preserve]
	public class ActionUpdateBossGroup : BaseAction
	{
		// Token: 0x0600B0CA RID: 45258 RVA: 0x00450170 File Offset: 0x0044E370
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			GameEventManager.Current.UpdateBossGroupType(base.Owner.CurrentBossGroupID, this.bossGroupType);
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600B0CB RID: 45259 RVA: 0x0045018E File Offset: 0x0044E38E
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseEnum<BossGroup.BossGroupTypes>(ActionUpdateBossGroup.PropGroupType, ref this.bossGroupType);
		}

		// Token: 0x0600B0CC RID: 45260 RVA: 0x004501A8 File Offset: 0x0044E3A8
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionUpdateBossGroup
			{
				bossGroupType = this.bossGroupType
			};
		}

		// Token: 0x04008A53 RID: 35411
		[PublicizedFrom(EAccessModifier.Protected)]
		public BossGroup.BossGroupTypes bossGroupType;

		// Token: 0x04008A54 RID: 35412
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropGroupType = "group_type";
	}
}
