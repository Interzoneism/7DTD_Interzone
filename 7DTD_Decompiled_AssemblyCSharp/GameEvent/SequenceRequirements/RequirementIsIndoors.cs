using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001628 RID: 5672
	[Preserve]
	public class RequirementIsIndoors : BaseRequirement
	{
		// Token: 0x0600AE2F RID: 44591 RVA: 0x00440BEC File Offset: 0x0043EDEC
		public override bool CanPerform(Entity target)
		{
			EntityAlive entityAlive = target as EntityAlive;
			if (entityAlive == null)
			{
				return false;
			}
			if (!this.Invert)
			{
				return entityAlive.Stats.AmountEnclosed > 0f;
			}
			return entityAlive.Stats.AmountEnclosed <= 0f;
		}

		// Token: 0x0600AE30 RID: 44592 RVA: 0x00440C35 File Offset: 0x0043EE35
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementIsIndoors
			{
				Invert = this.Invert
			};
		}
	}
}
