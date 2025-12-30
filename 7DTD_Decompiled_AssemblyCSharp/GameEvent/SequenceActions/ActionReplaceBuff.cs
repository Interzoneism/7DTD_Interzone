using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001691 RID: 5777
	[Preserve]
	public class ActionReplaceBuff : ActionBaseTargetAction
	{
		// Token: 0x0600AFFC RID: 45052 RVA: 0x0044C28C File Offset: 0x0044A48C
		public override BaseAction.ActionCompleteStates PerformTargetAction(Entity target)
		{
			EntityAlive entityAlive = target as EntityAlive;
			if (entityAlive != null && entityAlive.Buffs.HasBuff(this.replaceBuff))
			{
				entityAlive.Buffs.RemoveBuff(this.replaceBuff, true);
				entityAlive.Buffs.AddBuff(this.replaceWithBuff, -1, true, false, -1f);
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600AFFD RID: 45053 RVA: 0x0044C2E9 File Offset: 0x0044A4E9
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionReplaceBuff.PropReplaceBuffName, ref this.replaceBuff);
			properties.ParseString(ActionReplaceBuff.PropReplaceWithBuffName, ref this.replaceWithBuff);
		}

		// Token: 0x0600AFFE RID: 45054 RVA: 0x0044C314 File Offset: 0x0044A514
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionReplaceBuff
			{
				replaceBuff = this.replaceBuff,
				replaceWithBuff = this.replaceWithBuff,
				targetGroup = this.targetGroup
			};
		}

		// Token: 0x04008975 RID: 35189
		[PublicizedFrom(EAccessModifier.Protected)]
		public string replaceBuff = "";

		// Token: 0x04008976 RID: 35190
		[PublicizedFrom(EAccessModifier.Protected)]
		public string replaceWithBuff = "";

		// Token: 0x04008977 RID: 35191
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropReplaceBuffName = "replace_buff";

		// Token: 0x04008978 RID: 35192
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropReplaceWithBuffName = "replace_with_buff";
	}
}
