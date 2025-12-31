using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001625 RID: 5669
	[Preserve]
	public class RequirementInVehicle : BaseRequirement
	{
		// Token: 0x0600AE21 RID: 44577 RVA: 0x00002914 File Offset: 0x00000B14
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600AE22 RID: 44578 RVA: 0x00440A3E File Offset: 0x0043EC3E
		public override bool CanPerform(Entity target)
		{
			if (target == null)
			{
				return false;
			}
			if (target.AttachedToEntity)
			{
				return !this.Invert;
			}
			return this.Invert;
		}

		// Token: 0x0600AE23 RID: 44579 RVA: 0x00440A68 File Offset: 0x0043EC68
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementInVehicle
			{
				Invert = this.Invert
			};
		}
	}
}
