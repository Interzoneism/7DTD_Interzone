using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.Scripting;

namespace UAI
{
	// Token: 0x020014A2 RID: 5282
	[Preserve]
	public class UAIConsiderationTargetFactionStanding : UAIConsiderationBase
	{
		// Token: 0x0600A332 RID: 41778 RVA: 0x0040FE08 File Offset: 0x0040E008
		public override void Init(Dictionary<string, string> parameters)
		{
			base.Init(parameters);
			if (parameters.ContainsKey("min"))
			{
				this.min = StringParsers.ParseFloat(parameters["min"], 0, -1, NumberStyles.Any);
			}
			else
			{
				this.min = 0f;
			}
			if (parameters.ContainsKey("max"))
			{
				this.max = StringParsers.ParseFloat(parameters["max"], 0, -1, NumberStyles.Any);
				return;
			}
			this.max = 255f;
		}

		// Token: 0x0600A333 RID: 41779 RVA: 0x0040FE8C File Offset: 0x0040E08C
		public override float GetScore(Context _context, object target)
		{
			if (target is EntityAlive)
			{
				EntityAlive targetEntity = UAIUtils.ConvertToEntityAlive(target);
				return (FactionManager.Instance.GetRelationshipValue(_context.Self, targetEntity) - this.min) / (this.max - this.min);
			}
			return 0f;
		}

		// Token: 0x04007E70 RID: 32368
		[PublicizedFrom(EAccessModifier.Private)]
		public float min;

		// Token: 0x04007E71 RID: 32369
		[PublicizedFrom(EAccessModifier.Private)]
		public float max;
	}
}
