using System;
using UnityEngine.Scripting;

namespace UAI
{
	// Token: 0x020014A0 RID: 5280
	[Preserve]
	public class UAIConsiderationSelfVisible : UAIConsiderationBase
	{
		// Token: 0x0600A32D RID: 41773 RVA: 0x0040FC48 File Offset: 0x0040DE48
		public override float GetScore(Context _context, object target)
		{
			EntityAlive entityAlive = UAIUtils.ConvertToEntityAlive(target);
			if (entityAlive != null)
			{
				float num = _context.Self.GetSeeDistance();
				num *= num;
				float num2 = 1f - UAIUtils.DistanceSqr(_context.Self.getHeadPosition(), entityAlive.getHeadPosition()) / num;
				return (float)(entityAlive.CanEntityBeSeen(_context.Self) ? 1 : 0) * num2;
			}
			return 0f;
		}
	}
}
