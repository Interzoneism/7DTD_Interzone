using System;
using UnityEngine;

// Token: 0x0200109D RID: 4253
public class WorldRayHitInfo
{
	// Token: 0x06008629 RID: 34345 RVA: 0x00367CDC File Offset: 0x00365EDC
	public virtual void Clear()
	{
		this.ray = new Ray(Vector3.zero, Vector3.zero);
		this.bHitValid = false;
		this.tag = string.Empty;
		this.transform = null;
		this.lastBlockPos = Vector3i.zero;
		this.hit.Clear();
		this.fmcHit.Clear();
		this.hitCollider = null;
		this.hitTriangleIdx = 0;
	}

	// Token: 0x0600862A RID: 34346 RVA: 0x00367D48 File Offset: 0x00365F48
	public virtual void CopyFrom(WorldRayHitInfo _other)
	{
		this.ray = _other.ray;
		this.bHitValid = _other.bHitValid;
		this.tag = _other.tag;
		this.transform = _other.transform;
		this.lastBlockPos = _other.lastBlockPos;
		this.hit.CopyFrom(_other.hit);
		this.fmcHit.CopyFrom(_other.fmcHit);
		this.hitCollider = _other.hitCollider;
		this.hitTriangleIdx = _other.hitTriangleIdx;
	}

	// Token: 0x0600862B RID: 34347 RVA: 0x00367DCC File Offset: 0x00365FCC
	public virtual WorldRayHitInfo Clone()
	{
		return new WorldRayHitInfo
		{
			ray = this.ray,
			bHitValid = this.bHitValid,
			tag = this.tag,
			transform = this.transform,
			lastBlockPos = this.lastBlockPos,
			hit = this.hit.Clone(),
			fmcHit = this.fmcHit.Clone(),
			hitCollider = this.hitCollider,
			hitTriangleIdx = this.hitTriangleIdx
		};
	}

	// Token: 0x04006839 RID: 26681
	public Ray ray;

	// Token: 0x0400683A RID: 26682
	public bool bHitValid;

	// Token: 0x0400683B RID: 26683
	public string tag;

	// Token: 0x0400683C RID: 26684
	public Transform transform;

	// Token: 0x0400683D RID: 26685
	public HitInfoDetails hit;

	// Token: 0x0400683E RID: 26686
	public HitInfoDetails fmcHit;

	// Token: 0x0400683F RID: 26687
	public Vector3i lastBlockPos;

	// Token: 0x04006840 RID: 26688
	public Collider hitCollider;

	// Token: 0x04006841 RID: 26689
	public int hitTriangleIdx;
}
