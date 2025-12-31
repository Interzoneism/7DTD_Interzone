using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001623 RID: 5667
	[Preserve]
	public class RequirementInSafeZone : BaseRequirement
	{
		// Token: 0x0600AE1B RID: 44571 RVA: 0x00440990 File Offset: 0x0043EB90
		public override bool CanPerform(Entity target)
		{
			if (!GameManager.Instance.World.CanPlaceBlockAt(new Vector3i((target == null) ? this.Owner.TargetPosition : target.position), null, false))
			{
				return !this.Invert;
			}
			return this.Invert;
		}

		// Token: 0x0600AE1C RID: 44572 RVA: 0x004409E1 File Offset: 0x0043EBE1
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementInSafeZone();
		}
	}
}
