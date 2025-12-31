using System;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000463 RID: 1123
[Preserve]
public class EntitySupplyPlane : Entity
{
	// Token: 0x0600245C RID: 9308 RVA: 0x000DB50B File Offset: 0x000D970B
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		base.Awake();
	}

	// Token: 0x0600245D RID: 9309 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsDeadIfOutOfWorld()
	{
		return false;
	}

	// Token: 0x0600245E RID: 9310 RVA: 0x000E799B File Offset: 0x000E5B9B
	public void SetDirectionToFly(Vector3 _directionToFly, int _ticksToFly)
	{
		this.ticksToFly = _ticksToFly;
		this.motion = _directionToFly * 6f;
		this.IsMovementReplicated = false;
	}

	// Token: 0x0600245F RID: 9311 RVA: 0x000E79BC File Offset: 0x000E5BBC
	[PublicizedFrom(EAccessModifier.Private)]
	public void MoveBoundsInsideFrustrum(Transform _parentT)
	{
		if (!this.planeMesh)
		{
			return;
		}
		float magnitude = (this.mainCamera.transform.position - _parentT.position).magnitude;
		Vector3 size = Vector3.one * (magnitude * 1.25f);
		Vector3 zero = Vector3.zero;
		this.planeMesh.bounds = new Bounds(zero, size);
	}

	// Token: 0x06002460 RID: 9312 RVA: 0x000E7A28 File Offset: 0x000E5C28
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateFarDraw()
	{
		if (!this.mainCamera)
		{
			this.mainCamera = Camera.main;
			if (!this.mainCamera)
			{
				return;
			}
		}
		if (!this.planeMesh)
		{
			this.planeMF = base.transform.GetComponentInChildren<MeshFilter>();
			if (this.planeMF)
			{
				this.planeMesh = this.planeMF.mesh;
			}
		}
		this.MoveBoundsInsideFrustrum(base.transform);
	}

	// Token: 0x06002461 RID: 9313 RVA: 0x000E7AA4 File Offset: 0x000E5CA4
	public override void OnUpdatePosition(float _partialTicks)
	{
		base.OnUpdatePosition(_partialTicks);
		this.UpdateFarDraw();
		this.interpolateTargetRot = 0;
		this.position += this.motion * _partialTicks;
		if (!this.isEntityRemote)
		{
			int num = this.ticksToFly - 1;
			this.ticksToFly = num;
			if (num <= 0)
			{
				this.MarkToUnload();
			}
		}
		if (!this.isPlayedSound)
		{
			Manager.Play(this, "SupplyDrops/Supply_Crate_Plane_lp", 1f, false);
			this.isPlayedSound = true;
		}
		base.SetAirBorne(true);
	}

	// Token: 0x06002462 RID: 9314 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsSavedToFile()
	{
		return false;
	}

	// Token: 0x06002463 RID: 9315 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool CanCollideWithBlocks()
	{
		return false;
	}

	// Token: 0x04001B4B RID: 6987
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int ticksToFly;

	// Token: 0x04001B4C RID: 6988
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isPlayedSound;

	// Token: 0x04001B4D RID: 6989
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Camera mainCamera;

	// Token: 0x04001B4E RID: 6990
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public MeshFilter planeMF;

	// Token: 0x04001B4F RID: 6991
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Mesh planeMesh;
}
