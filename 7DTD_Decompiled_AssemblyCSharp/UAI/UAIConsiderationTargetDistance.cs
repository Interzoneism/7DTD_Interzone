using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Scripting;

namespace UAI
{
	// Token: 0x020014A1 RID: 5281
	[Preserve]
	public class UAIConsiderationTargetDistance : UAIConsiderationBase
	{
		// Token: 0x0600A32F RID: 41775 RVA: 0x0040FCB0 File Offset: 0x0040DEB0
		public override void Init(Dictionary<string, string> _parameters)
		{
			base.Init(_parameters);
			if (_parameters.ContainsKey("min"))
			{
				this.min = StringParsers.ParseFloat(_parameters["min"], 0, -1, NumberStyles.Any);
				this.min *= this.min;
			}
			if (_parameters.ContainsKey("max"))
			{
				this.max = StringParsers.ParseFloat(_parameters["max"], 0, -1, NumberStyles.Any);
				this.max *= this.max;
			}
		}

		// Token: 0x0600A330 RID: 41776 RVA: 0x0040FD40 File Offset: 0x0040DF40
		public override float GetScore(Context _context, object target)
		{
			EntityAlive entityAlive = UAIUtils.ConvertToEntityAlive(target);
			if (entityAlive != null)
			{
				float num = UAIUtils.DistanceSqr(_context.Self.position, entityAlive.position);
				return Mathf.Clamp01(Mathf.Max(0f, num - this.min) / (this.max - this.min));
			}
			if (target.GetType() == typeof(Vector3))
			{
				Vector3 pointB = (Vector3)target;
				float num2 = UAIUtils.DistanceSqr(_context.Self.position, pointB);
				return Mathf.Clamp01(Mathf.Max(0f, num2 - this.min) / (this.max - this.min));
			}
			return 0f;
		}

		// Token: 0x04007E6E RID: 32366
		[PublicizedFrom(EAccessModifier.Private)]
		public float min;

		// Token: 0x04007E6F RID: 32367
		[PublicizedFrom(EAccessModifier.Private)]
		public float max = 9126f;
	}
}
