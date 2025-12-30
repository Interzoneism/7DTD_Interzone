using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000422 RID: 1058
[Preserve]
public class EntityAnimalSnake : EntityHuman
{
	// Token: 0x060020B1 RID: 8369 RVA: 0x000CBC20 File Offset: 0x000C9E20
	public override Vector3 GetAttackTargetHitPosition()
	{
		Vector3 position = this.attackTarget.position;
		position.y += 0.5f;
		return position;
	}
}
