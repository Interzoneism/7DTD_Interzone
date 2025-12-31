using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001680 RID: 5760
	[Preserve]
	public class ActionPrimeEntity : ActionBaseTargetAction
	{
		// Token: 0x0600AFAD RID: 44973 RVA: 0x0044ADB0 File Offset: 0x00448FB0
		public override BaseAction.ActionCompleteStates PerformTargetAction(Entity target)
		{
			EntityZombieCop entityZombieCop = target as EntityZombieCop;
			if (entityZombieCop != null)
			{
				entityZombieCop.HandlePrimingDetonator(GameEventManager.GetFloatValue(base.Owner.Target as EntityAlive, this.overrideTimeText, -1f));
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600AFAE RID: 44974 RVA: 0x0044ADF4 File Offset: 0x00448FF4
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionPrimeEntity.PropOverrideTime, ref this.overrideTimeText);
		}

		// Token: 0x0600AFAF RID: 44975 RVA: 0x0044AE0E File Offset: 0x0044900E
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionPrimeEntity
			{
				targetGroup = this.targetGroup,
				overrideTimeText = this.overrideTimeText
			};
		}

		// Token: 0x0400893A RID: 35130
		[PublicizedFrom(EAccessModifier.Protected)]
		public string overrideTimeText;

		// Token: 0x0400893B RID: 35131
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropOverrideTime = "override_time";
	}
}
