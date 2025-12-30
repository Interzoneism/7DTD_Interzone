using System;
using UnityEngine;

// Token: 0x020009B4 RID: 2484
public class ChunkCoordinates
{
	// Token: 0x06004BF5 RID: 19445 RVA: 0x001E0357 File Offset: 0x001DE557
	public ChunkCoordinates(int _x, int _y, int _z)
	{
		this.position = new Vector3i(_x, _y, _z);
	}

	// Token: 0x06004BF6 RID: 19446 RVA: 0x001E036D File Offset: 0x001DE56D
	public ChunkCoordinates(ChunkCoordinates _cc)
	{
		this.position = _cc.position;
	}

	// Token: 0x06004BF7 RID: 19447 RVA: 0x001E0381 File Offset: 0x001DE581
	public override bool Equals(object _obj)
	{
		return _obj is ChunkCoordinates && this.position.Equals(((ChunkCoordinates)_obj).position);
	}

	// Token: 0x06004BF8 RID: 19448 RVA: 0x001E03A3 File Offset: 0x001DE5A3
	public override int GetHashCode()
	{
		return this.position.x + this.position.z << 8 + this.position.y << 16;
	}

	// Token: 0x06004BF9 RID: 19449 RVA: 0x001E03D0 File Offset: 0x001DE5D0
	public float getDistance(int _x, int _y, int _z)
	{
		int num = this.position.x - _x;
		int num2 = this.position.y - _y;
		int num3 = this.position.z - _z;
		return Mathf.Sqrt((float)(num * num + num2 * num2 + num3 * num3));
	}

	// Token: 0x06004BFA RID: 19450 RVA: 0x001E0418 File Offset: 0x001DE618
	public float getDistanceSquared(int _x, int _y, int _z)
	{
		int num = this.position.x - _x;
		int num2 = this.position.y - _y;
		int num3 = this.position.z - _z;
		return (float)(num * num + num2 * num2 + num3 * num3);
	}

	// Token: 0x040039F1 RID: 14833
	public Vector3i position;
}
