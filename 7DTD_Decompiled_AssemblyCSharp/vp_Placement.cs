using System;
using UnityEngine;

// Token: 0x02001326 RID: 4902
public class vp_Placement
{
	// Token: 0x0600987F RID: 39039 RVA: 0x003CA5BC File Offset: 0x003C87BC
	public static bool AdjustPosition(vp_Placement p, float physicsRadius, int attempts = 1000)
	{
		attempts--;
		if (attempts > 0)
		{
			if (p.IsObstructed(physicsRadius))
			{
				Vector3 insideUnitSphere = UnityEngine.Random.insideUnitSphere;
				p.Position.x = p.Position.x + insideUnitSphere.x;
				p.Position.z = p.Position.z + insideUnitSphere.z;
				vp_Placement.AdjustPosition(p, physicsRadius, attempts);
			}
			return true;
		}
		Debug.LogWarning("(vp_Placement.AdjustPosition) Failed to find valid placement.");
		return false;
	}

	// Token: 0x06009880 RID: 39040 RVA: 0x003CA623 File Offset: 0x003C8823
	public virtual bool IsObstructed(float physicsRadius = 1f)
	{
		return Physics.CheckSphere(this.Position, physicsRadius, 2260992);
	}

	// Token: 0x06009881 RID: 39041 RVA: 0x003CA63C File Offset: 0x003C883C
	public static void SnapToGround(vp_Placement p, float radius, float snapDistance)
	{
		if (snapDistance == 0f)
		{
			return;
		}
		RaycastHit raycastHit;
		Physics.SphereCast(new Ray(p.Position + Vector3.up * snapDistance, Vector3.down), radius, out raycastHit, snapDistance * 2f, 1084850176);
		if (raycastHit.collider != null)
		{
			p.Position.y = raycastHit.point.y + 0.05f;
		}
	}

	// Token: 0x040074FA RID: 29946
	public Vector3 Position = Vector3.zero;

	// Token: 0x040074FB RID: 29947
	public Quaternion Rotation = Quaternion.identity;
}
