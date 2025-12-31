using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.Scripting;

namespace UAI
{
	// Token: 0x0200149F RID: 5279
	[Preserve]
	public class UAIConsiderationSelfHealth : UAIConsiderationBase
	{
		// Token: 0x0600A32A RID: 41770 RVA: 0x0040FB74 File Offset: 0x0040DD74
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
			this.max = float.NaN;
		}

		// Token: 0x0600A32B RID: 41771 RVA: 0x0040FBF8 File Offset: 0x0040DDF8
		public override float GetScore(Context _context, object _target)
		{
			if (float.IsNaN(this.max))
			{
				this.max = (float)_context.Self.GetMaxHealth();
			}
			return ((float)_context.Self.Health - this.min) / (this.max - this.min);
		}

		// Token: 0x04007E6C RID: 32364
		[PublicizedFrom(EAccessModifier.Private)]
		public float min;

		// Token: 0x04007E6D RID: 32365
		[PublicizedFrom(EAccessModifier.Private)]
		public float max;
	}
}
