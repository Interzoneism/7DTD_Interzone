using System;
using GamePath;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020003E5 RID: 997
[Preserve]
public class EAILeap : EAIBase
{
	// Token: 0x06001E27 RID: 7719 RVA: 0x000BB920 File Offset: 0x000B9B20
	public override void Init(EntityAlive _theEntity)
	{
		base.Init(_theEntity);
		this.MutexBits = 3;
		this.executeDelay = 1f + base.RandomFloat;
	}

	// Token: 0x06001E28 RID: 7720 RVA: 0x000BB942 File Offset: 0x000B9B42
	public override void SetData(DictionarySave<string, string> data)
	{
		base.SetData(data);
		base.GetData(data, "legs", ref this.legCount);
	}

	// Token: 0x06001E29 RID: 7721 RVA: 0x000BB960 File Offset: 0x000B9B60
	public override bool CanExecute()
	{
		if (this.theEntity.IsDancing)
		{
			return false;
		}
		if (!this.theEntity.GetAttackTarget())
		{
			return false;
		}
		if (this.theEntity.Jumping)
		{
			return false;
		}
		if ((this.legCount <= 2) ? this.theEntity.bodyDamage.IsAnyLegMissing : this.theEntity.bodyDamage.IsAnyArmOrLegMissing)
		{
			return false;
		}
		if (this.theEntity.moveHelper.BlockedFlags > 0)
		{
			return false;
		}
		PathEntity path = this.theEntity.navigator.getPath();
		if (path == null)
		{
			return false;
		}
		float jumpMaxDistance = this.theEntity.jumpMaxDistance;
		this.leapV = path.GetEndPos() - this.theEntity.position;
		if (this.leapV.y < -5f || this.leapV.y > 0.5f + jumpMaxDistance * 0.5f)
		{
			return false;
		}
		this.leapDist = Mathf.Sqrt(this.leapV.x * this.leapV.x + this.leapV.z * this.leapV.z);
		if (this.leapDist < 2.8f || this.leapDist > jumpMaxDistance)
		{
			return false;
		}
		Vector3 position = this.theEntity.position;
		position.y += 1.5f;
		RaycastHit raycastHit;
		return !Physics.Raycast(position - Origin.position, this.leapV, out raycastHit, this.leapDist - 0.5f, 1082195968);
	}

	// Token: 0x06001E2A RID: 7722 RVA: 0x000BBAEC File Offset: 0x000B9CEC
	public override void Start()
	{
		this.abortTime = 5f;
		this.theEntity.moveHelper.Stop();
		this.leapYaw = Mathf.Atan2(this.leapV.x, this.leapV.z) * 57.29578f;
	}

	// Token: 0x06001E2B RID: 7723 RVA: 0x000BBB3C File Offset: 0x000B9D3C
	public override bool Continue()
	{
		if (this.theEntity.bodyDamage.CurrentStun != EnumEntityStunType.None)
		{
			return false;
		}
		if (this.abortTime <= 0f)
		{
			return false;
		}
		EntityMoveHelper moveHelper = this.theEntity.moveHelper;
		this.theEntity.SeekYaw(this.leapYaw, 0f, 10f);
		if (Utils.FastAbs(Mathf.DeltaAngle(this.theEntity.rotation.y, this.leapYaw)) < 1f)
		{
			moveHelper.StartJump(false, this.leapDist, this.leapV.y);
			return false;
		}
		return true;
	}

	// Token: 0x06001E2C RID: 7724 RVA: 0x000BBBD6 File Offset: 0x000B9DD6
	public override void Update()
	{
		this.abortTime -= 0.05f;
	}

	// Token: 0x06001E2D RID: 7725 RVA: 0x00002914 File Offset: 0x00000B14
	public override void Reset()
	{
	}

	// Token: 0x040014C2 RID: 5314
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cCollisionMask = 1082195968;

	// Token: 0x040014C3 RID: 5315
	[PublicizedFrom(EAccessModifier.Private)]
	public int legCount = 2;

	// Token: 0x040014C4 RID: 5316
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 leapV;

	// Token: 0x040014C5 RID: 5317
	[PublicizedFrom(EAccessModifier.Private)]
	public float leapDist;

	// Token: 0x040014C6 RID: 5318
	[PublicizedFrom(EAccessModifier.Private)]
	public float leapYaw;

	// Token: 0x040014C7 RID: 5319
	[PublicizedFrom(EAccessModifier.Private)]
	public float abortTime;
}
