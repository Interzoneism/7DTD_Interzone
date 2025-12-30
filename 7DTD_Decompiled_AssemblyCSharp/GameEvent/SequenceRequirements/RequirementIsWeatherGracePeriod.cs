using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x0200162A RID: 5674
	[Preserve]
	public class RequirementIsWeatherGracePeriod : BaseRequirement
	{
		// Token: 0x0600AE35 RID: 44597 RVA: 0x00440C8C File Offset: 0x0043EE8C
		public override bool CanPerform(Entity target)
		{
			bool flag = GameManager.Instance.World.GetWorldTime() <= 30000UL;
			if (!this.Invert)
			{
				return flag;
			}
			return !flag;
		}

		// Token: 0x0600AE36 RID: 44598 RVA: 0x00440CC2 File Offset: 0x0043EEC2
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementIsWeatherGracePeriod
			{
				Invert = this.Invert
			};
		}
	}
}
