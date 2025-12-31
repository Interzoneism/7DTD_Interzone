using System;
using UnityEngine;

// Token: 0x020004CF RID: 1231
public class PrefabInstanceGizmo : MonoBehaviour
{
	// Token: 0x06002816 RID: 10262 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
	}

	// Token: 0x06002817 RID: 10263 RVA: 0x001046BC File Offset: 0x001028BC
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDrawGizmos()
	{
		if (this.pi != null)
		{
			Gizmos.color = ((!this.bSelected) ? Color.green : Color.yellow);
			if (!this.bSelected && base.transform.hasChanged)
			{
				Gizmos.color = Color.cyan;
			}
			Vector3 vector = this.pi.boundingBoxSize.ToVector3();
			Gizmos.DrawSphere(base.transform.position + new Vector3(0f, vector.y + 1f, 0f), 1.5f);
			Gizmos.DrawWireCube(base.transform.position + new Vector3(0f, (vector.y - (float)this.pi.prefab.yOffset) / 2f, 0f), this.pi.boundingBoxSize.ToVector3() + new Vector3(0f, (float)this.pi.prefab.yOffset, 0f));
			if (this.pi.prefab.yOffset != 0)
			{
				Gizmos.color = ((!this.bSelected) ? new Color(0f, 0.5f, 0f, 0.5f) : new Color(0.7f, 0.7f, 0f, 0.5f));
				if (!this.bSelected && base.transform.hasChanged)
				{
					Gizmos.color = Color.cyan;
				}
				Gizmos.DrawCube(base.transform.position + new Vector3(0f, (float)(-1 * this.pi.prefab.yOffset) / 2f, 0f), new Vector3((float)this.pi.boundingBoxSize.x, (float)(-1 * this.pi.prefab.yOffset) - 0.1f, (float)this.pi.boundingBoxSize.z));
				Gizmos.color = ((!this.bSelected) ? Color.green : Color.yellow);
				if (!this.bSelected && base.transform.hasChanged)
				{
					Gizmos.color = Color.cyan;
				}
				Gizmos.DrawWireCube(base.transform.position + new Vector3(0f, (float)(-1 * this.pi.prefab.yOffset) / 2f, 0f), new Vector3((float)this.pi.boundingBoxSize.x, (float)(-1 * this.pi.prefab.yOffset) - 0.1f, (float)this.pi.boundingBoxSize.z));
			}
			this.pos = base.transform.position - new Vector3((float)this.pi.boundingBoxSize.x * 0.5f, 0f, (float)this.pi.boundingBoxSize.z * 0.5f);
			Vector3 zero = Vector3.zero;
			switch (this.pi.rotation)
			{
			case 0:
				zero = new Vector3(-0.5f, 0f, -0.5f);
				break;
			case 1:
				zero = new Vector3(-0.5f, 0f, 0.5f);
				break;
			case 2:
				zero = new Vector3(0.5f, 0f, 0.5f);
				break;
			case 3:
				zero = new Vector3(0.5f, 0f, -0.5f);
				break;
			}
			if (Utils.FastAbs(this.pos.x - (float)((int)this.pos.x)) > 0.001f)
			{
				this.pos.x = this.pos.x - zero.x;
			}
			if (Utils.FastAbs(this.pos.z - (float)((int)this.pos.z)) > 0.001f)
			{
				this.pos.z = this.pos.z - zero.z;
			}
			this.rot = (int)this.pi.rotation;
			if (PrefabInstanceGizmo.Selected == this.pi)
			{
				this.pi.boundingBoxPosition = World.worldToBlockPos(this.pos);
				this.pi.rotation = (byte)this.rot;
			}
		}
		this.bSelected = false;
	}

	// Token: 0x06002818 RID: 10264 RVA: 0x00104B07 File Offset: 0x00102D07
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDrawGizmosSelected()
	{
		this.bSelected = true;
		PrefabInstanceGizmo.Selected = this.pi;
	}

	// Token: 0x04001ED4 RID: 7892
	public static PrefabInstance Selected;

	// Token: 0x04001ED5 RID: 7893
	public PrefabInstance pi;

	// Token: 0x04001ED6 RID: 7894
	public Vector3 pos;

	// Token: 0x04001ED7 RID: 7895
	public int rot;

	// Token: 0x04001ED8 RID: 7896
	public bool bSelected;
}
