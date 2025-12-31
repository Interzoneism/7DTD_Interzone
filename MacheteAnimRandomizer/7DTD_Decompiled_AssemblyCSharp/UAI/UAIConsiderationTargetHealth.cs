using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace UAI
{
	// Token: 0x020014A3 RID: 5283
	[Preserve]
	public class UAIConsiderationTargetHealth : UAIConsiderationBase
	{
		// Token: 0x0600A335 RID: 41781 RVA: 0x0040FED4 File Offset: 0x0040E0D4
		public override float GetScore(Context _context, object target)
		{
			EntityAlive entityAlive = UAIUtils.ConvertToEntityAlive(target);
			if (entityAlive != null)
			{
				return (float)entityAlive.Health / (float)entityAlive.GetMaxHealth();
			}
			if (target.GetType() == typeof(Vector3))
			{
				BlockValue block = _context.Self.world.GetBlock(new Vector3i((Vector3)target));
				Block block2 = block.Block;
				return (float)(block2.MaxDamage - block.damage) / (float)block2.MaxDamage;
			}
			return 0f;
		}
	}
}
