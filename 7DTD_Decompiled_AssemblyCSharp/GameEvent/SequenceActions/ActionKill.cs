using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001674 RID: 5748
	[Preserve]
	public class ActionKill : ActionBaseTargetAction
	{
		// Token: 0x0600AF7F RID: 44927 RVA: 0x00449F58 File Offset: 0x00448158
		public override BaseAction.ActionCompleteStates PerformTargetAction(Entity target)
		{
			EntityAlive entityAlive = target as EntityAlive;
			if (entityAlive != null)
			{
				entityAlive.DamageEntity(new DamageSource(EnumDamageSource.Internal, EnumDamageTypes.Suicide), 99999, false, 1f);
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600AF80 RID: 44928 RVA: 0x00449F90 File Offset: 0x00448190
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionKill
			{
				targetGroup = this.targetGroup
			};
		}
	}
}
