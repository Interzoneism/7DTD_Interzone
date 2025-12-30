using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001624 RID: 5668
	[Preserve]
	public class RequirementInTraderArea : BaseRequirement
	{
		// Token: 0x0600AE1E RID: 44574 RVA: 0x004409E8 File Offset: 0x0043EBE8
		public override bool CanPerform(Entity target)
		{
			if (!GameManager.Instance.World.IsWithinTraderArea(new Vector3i((target == null) ? this.Owner.TargetPosition : target.position)))
			{
				return !this.Invert;
			}
			return this.Invert;
		}

		// Token: 0x0600AE1F RID: 44575 RVA: 0x00440A37 File Offset: 0x0043EC37
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementInTraderArea();
		}
	}
}
