using System;
using UnityEngine;

// Token: 0x02000086 RID: 134
public class AnimatorDrawbridgeState : AnimatorDoorState
{
	// Token: 0x0600026E RID: 622 RVA: 0x00013E03 File Offset: 0x00012003
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		this.mask = LayerMask.GetMask(new string[]
		{
			"Physics",
			"CC Physics",
			"CC Local Physics"
		});
	}

	// Token: 0x0600026F RID: 623 RVA: 0x00013E34 File Offset: 0x00012034
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool CheckForObstacles()
	{
		if (this.colliders == null)
		{
			return false;
		}
		for (int i = 0; i < this.colliders.Length; i++)
		{
			EntityCollisionRules entityCollisionRules = this.rules[i];
			if (!entityCollisionRules || !entityCollisionRules.IsStatic)
			{
				Vector3 halfExtents = this.colliders[i].bounds.extents;
				if (this.colliders[i] is MeshCollider)
				{
					halfExtents = Vector3.Scale(((MeshCollider)this.colliders[i]).sharedMesh.bounds.extents, this.colliders[i].transform.localScale);
				}
				if (Physics.OverlapBoxNonAlloc(this.colliders[i].bounds.center, halfExtents, AnimatorDrawbridgeState.overlapBoxHits, this.colliders[i].transform.rotation, this.mask) > 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000270 RID: 624 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void PushPlayers(float _normalizedTime)
	{
	}

	// Token: 0x04000311 RID: 785
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Collider[] overlapBoxHits = new Collider[20];

	// Token: 0x04000312 RID: 786
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public LayerMask mask;
}
