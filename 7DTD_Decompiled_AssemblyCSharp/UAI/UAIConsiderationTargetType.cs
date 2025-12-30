using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace UAI
{
	// Token: 0x020014A4 RID: 5284
	[Preserve]
	public class UAIConsiderationTargetType : UAIConsiderationBase
	{
		// Token: 0x0600A337 RID: 41783 RVA: 0x0040FF58 File Offset: 0x0040E158
		public override void Init(Dictionary<string, string> parameters)
		{
			base.Init(parameters);
			if (parameters.ContainsKey("type"))
			{
				this.type = parameters["type"].Split(',', StringSplitOptions.None);
				for (int i = 0; i < this.type.Length; i++)
				{
					this.type[i] = this.type[i].Trim();
				}
			}
		}

		// Token: 0x0600A338 RID: 41784 RVA: 0x0040FFBC File Offset: 0x0040E1BC
		public override float GetScore(Context _context, object target)
		{
			for (int i = 0; i < this.type.Length; i++)
			{
				Type type = Type.GetType(this.type[i]);
				if (type.IsAssignableFrom(target.GetType()))
				{
					return 1f;
				}
				if (target.GetType() == typeof(Vector3) && type.IsAssignableFrom(_context.World.GetBlock(new Vector3i((Vector3)target)).Block.GetType()))
				{
					return 1f;
				}
			}
			return 0f;
		}

		// Token: 0x04007E72 RID: 32370
		public string[] type;
	}
}
