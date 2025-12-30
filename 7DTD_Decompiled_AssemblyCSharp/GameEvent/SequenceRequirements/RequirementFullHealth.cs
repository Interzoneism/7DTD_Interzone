using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001613 RID: 5651
	[Preserve]
	public class RequirementFullHealth : BaseRequirement
	{
		// Token: 0x0600ADC6 RID: 44486 RVA: 0x00002914 File Offset: 0x00000B14
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600ADC7 RID: 44487 RVA: 0x0043FF84 File Offset: 0x0043E184
		public override bool CanPerform(Entity target)
		{
			EntityAlive entityAlive = target as EntityAlive;
			if (entityAlive == null)
			{
				return false;
			}
			if (entityAlive.Stats.Health.Value == entityAlive.Stats.Health.Max)
			{
				return !this.Invert;
			}
			return this.Invert;
		}

		// Token: 0x0600ADC8 RID: 44488 RVA: 0x0043FFD1 File Offset: 0x0043E1D1
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementFullHealth
			{
				Invert = this.Invert
			};
		}
	}
}
