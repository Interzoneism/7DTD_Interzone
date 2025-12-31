using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001666 RID: 5734
	[Preserve]
	public class ActionEnemyToCrawler : ActionBaseTargetAction
	{
		// Token: 0x0600AF50 RID: 44880 RVA: 0x00447854 File Offset: 0x00445A54
		public override BaseAction.ActionCompleteStates PerformTargetAction(Entity target)
		{
			EntityAlive entityAlive = target as EntityAlive;
			if (entityAlive != null && !(entityAlive is EntityPlayer) && entityAlive is EntityHuman)
			{
				DamageResponse damageResponse = DamageResponse.New(false);
				damageResponse.Source = new DamageSource(EnumDamageSource.External, EnumDamageTypes.Bashing);
				damageResponse.Source.DismemberChance = 100000f;
				damageResponse.Strength = 1;
				damageResponse.CrippleLegs = true;
				damageResponse.Dismember = true;
				damageResponse.TurnIntoCrawler = true;
				damageResponse.HitBodyPart = EnumBodyPartHit.UpperLegs;
				entityAlive.ProcessDamageResponse(damageResponse);
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600AF51 RID: 44881 RVA: 0x004478D7 File Offset: 0x00445AD7
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionEnemyToCrawler
			{
				targetGroup = this.targetGroup
			};
		}
	}
}
