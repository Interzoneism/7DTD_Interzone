using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001684 RID: 5764
	[Preserve]
	public class ActionRagdoll : ActionBaseTargetAction
	{
		// Token: 0x0600AFBE RID: 44990 RVA: 0x0044B358 File Offset: 0x00449558
		public override BaseAction.ActionCompleteStates PerformTargetAction(Entity target)
		{
			EntityAlive entityAlive = target as EntityAlive;
			if (entityAlive != null)
			{
				if (entityAlive.IsInElevator())
				{
					return BaseAction.ActionCompleteStates.InComplete;
				}
				if (entityAlive.AttachedToEntity != null)
				{
					entityAlive.Detach();
				}
				DamageResponse dmResponse = DamageResponse.New(false);
				dmResponse.StunDuration = GameEventManager.GetFloatValue(entityAlive, this.stunDurationText, 1f);
				dmResponse.Source = new DamageSource(EnumDamageSource.External, EnumDamageTypes.Bashing);
				entityAlive.DoRagdoll(dmResponse);
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600AFBF RID: 44991 RVA: 0x0044B3C8 File Offset: 0x004495C8
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionRagdoll.PropStunDuration, ref this.stunDurationText);
		}

		// Token: 0x0600AFC0 RID: 44992 RVA: 0x0044B3E2 File Offset: 0x004495E2
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionRagdoll
			{
				targetGroup = this.targetGroup,
				stunDurationText = this.stunDurationText
			};
		}

		// Token: 0x04008950 RID: 35152
		[PublicizedFrom(EAccessModifier.Protected)]
		public string stunDurationText;

		// Token: 0x04008951 RID: 35153
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropStunDuration = "stun_duration";
	}
}
