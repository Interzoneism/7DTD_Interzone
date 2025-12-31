using System;
using UnityEngine;

// Token: 0x02001257 RID: 4695
public class WaterClippingVolume
{
	// Token: 0x06009339 RID: 37689 RVA: 0x003A9A0C File Offset: 0x003A7C0C
	public void Prepare(Plane waterClipPlane)
	{
		this.waterClipPlane = waterClipPlane;
		this.isSliced = WaterClippingUtils.GetCubePlaneIntersectionEdgeLoop(waterClipPlane, ref this.intersectionPoints, out this.count);
	}

	// Token: 0x0600933A RID: 37690 RVA: 0x003A9A30 File Offset: 0x003A7C30
	public void ApplyClipping(ref Vector3 vertLocalPos)
	{
		if (!this.isSliced)
		{
			return;
		}
		if (this.waterClipPlane.GetDistanceToPoint(vertLocalPos) > 0f)
		{
			vertLocalPos = this.waterClipPlane.ClosestPointOnPlane(vertLocalPos);
			if (!WaterClippingUtils.CubeBounds.Contains(vertLocalPos))
			{
				vertLocalPos = GeometryUtils.NearestPointOnEdgeLoop(vertLocalPos, this.intersectionPoints, this.count);
			}
			return;
		}
	}

	// Token: 0x04007084 RID: 28804
	[PublicizedFrom(EAccessModifier.Private)]
	public Plane waterClipPlane;

	// Token: 0x04007085 RID: 28805
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3[] intersectionPoints = new Vector3[6];

	// Token: 0x04007086 RID: 28806
	[PublicizedFrom(EAccessModifier.Private)]
	public int count = -1;

	// Token: 0x04007087 RID: 28807
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isSliced;
}
