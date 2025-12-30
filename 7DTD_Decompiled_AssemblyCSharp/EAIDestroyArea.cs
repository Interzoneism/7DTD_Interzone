using System;
using System.Diagnostics;
using GamePath;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020003E2 RID: 994
[Preserve]
public class EAIDestroyArea : EAIBase
{
	// Token: 0x06001E11 RID: 7697 RVA: 0x000BB051 File Offset: 0x000B9251
	public override void Init(EntityAlive _theEntity)
	{
		base.Init(_theEntity);
		this.MutexBits = 3;
		this.executeDelay = 1f + base.RandomFloat * 0.9f;
	}

	// Token: 0x06001E12 RID: 7698 RVA: 0x000BB07C File Offset: 0x000B927C
	public override bool CanExecute()
	{
		EntityMoveHelper moveHelper = this.theEntity.moveHelper;
		if (!moveHelper.CanBreakBlocks)
		{
			return false;
		}
		EntityAlive attackTarget = this.theEntity.GetAttackTarget();
		if (!attackTarget)
		{
			return false;
		}
		if (this.theEntity.bodyDamage.CurrentStun != EnumEntityStunType.None)
		{
			return false;
		}
		bool flag = this.isLookFar;
		if (moveHelper.IsDestroyAreaTryUnreachable)
		{
			moveHelper.IsDestroyAreaTryUnreachable = false;
			float num = moveHelper.UnreachablePercent;
			if (num > 0f)
			{
				if (base.RandomFloat < num)
				{
					flag = true;
					num = 0f;
				}
				moveHelper.UnreachablePercent = num * 0.5f;
			}
		}
		if (this.manager.pathCostScale < 0.65f)
		{
			float num2 = (1f - this.manager.pathCostScale * 1.5384616f) * 0.6f;
			if (base.RandomFloat < num2)
			{
				PathEntity path = this.theEntity.navigator.getPath();
				if (path != null && path.NodeCountRemaining() > 18 && (attackTarget.position - this.theEntity.position).sqrMagnitude <= 81f)
				{
					flag = true;
				}
			}
		}
		if (!flag && !moveHelper.IsUnreachableAbove)
		{
			return false;
		}
		Vector3 vector = this.theEntity.position;
		Vector3 vector2 = moveHelper.IsUnreachableSide ? moveHelper.UnreachablePos : attackTarget.position;
		Vector3 a = vector - vector2;
		float sqrMagnitude = a.sqrMagnitude;
		if (sqrMagnitude > 25f)
		{
			vector = vector2 + a * (5f / Mathf.Sqrt(sqrMagnitude));
		}
		vector.x += -3f + base.RandomFloat * 6f;
		vector.z += -3f + base.RandomFloat * 6f;
		if (!moveHelper.FindDestroyPos(ref vector, this.isLookFar))
		{
			return false;
		}
		this.seekPos = vector;
		this.seekBlockPos = World.worldToBlockPos(vector);
		this.isLookFar = false;
		this.state = EAIDestroyArea.eState.FindPath;
		this.theEntity.navigator.clearPath();
		this.theEntity.FindPath(vector, this.theEntity.GetMoveSpeedAggro(), true, this);
		moveHelper.IsDestroyArea = true;
		return true;
	}

	// Token: 0x06001E13 RID: 7699 RVA: 0x000BB29C File Offset: 0x000B949C
	public override void Start()
	{
		this.isAtPathEnd = false;
		this.delayTime = 3f;
		this.attackTimeout = 0;
	}

	// Token: 0x06001E14 RID: 7700 RVA: 0x000BB2B7 File Offset: 0x000B94B7
	public void Stop()
	{
		this.delayTime = 0f;
	}

	// Token: 0x06001E15 RID: 7701 RVA: 0x000BB2C4 File Offset: 0x000B94C4
	public override bool Continue()
	{
		if (this.theEntity.bodyDamage.CurrentStun != EnumEntityStunType.None)
		{
			return false;
		}
		if (this.delayTime <= 0f)
		{
			return false;
		}
		EntityMoveHelper moveHelper = this.theEntity.moveHelper;
		if (this.state == EAIDestroyArea.eState.FindPath && this.theEntity.navigator.HasPath())
		{
			moveHelper.CalcIfUnreachablePos();
			if (moveHelper.IsUnreachableAbove || moveHelper.IsUnreachableSide)
			{
				this.isLookFar = true;
				return false;
			}
			moveHelper.IsUnreachableAbove = true;
			this.state = EAIDestroyArea.eState.HasPath;
			this.delayTime = 15f;
			this.theEntity.navigator.ShortenEnd(0.2f);
		}
		if (this.state == EAIDestroyArea.eState.HasPath)
		{
			PathEntity path = this.theEntity.navigator.getPath();
			if (path != null && path.NodeCountRemaining() <= 1)
			{
				this.state = EAIDestroyArea.eState.EndPath;
				this.delayTime = 5f + base.RandomFloat * 5f;
				this.isAtPathEnd = true;
			}
		}
		if (this.state == EAIDestroyArea.eState.EndPath && moveHelper.BlockedFlags == 0)
		{
			if (!Voxel.BlockHit(this.hitInfo, this.seekBlockPos))
			{
				return false;
			}
			this.state = EAIDestroyArea.eState.Attack;
			this.theEntity.SeekYawToPos(this.seekPos, 10f);
		}
		return this.isAtPathEnd || !this.theEntity.navigator.noPathAndNotPlanningOne();
	}

	// Token: 0x06001E16 RID: 7702 RVA: 0x000BB414 File Offset: 0x000B9614
	public override void Update()
	{
		this.delayTime -= 0.05f;
		if (this.state == EAIDestroyArea.eState.Attack)
		{
			int num = this.attackTimeout - 1;
			this.attackTimeout = num;
			if (num <= 0)
			{
				ItemActionAttackData itemActionAttackData = this.theEntity.inventory.holdingItemData.actionData[0] as ItemActionAttackData;
				if (itemActionAttackData != null)
				{
					this.theEntity.SetLookPosition(Vector3.zero);
					if (this.theEntity.Attack(false))
					{
						this.attackTimeout = this.theEntity.GetAttackTimeoutTicks();
						itemActionAttackData.hitDelegate = new ItemActionAttackData.HitDelegate(this.GetHitInfo);
						this.theEntity.Attack(true);
						this.state = EAIDestroyArea.eState.EndPath;
					}
				}
			}
		}
	}

	// Token: 0x06001E17 RID: 7703 RVA: 0x000BB4CC File Offset: 0x000B96CC
	[PublicizedFrom(EAccessModifier.Private)]
	public WorldRayHitInfo GetHitInfo(out float damageScale)
	{
		damageScale = 1f;
		return this.hitInfo;
	}

	// Token: 0x06001E18 RID: 7704 RVA: 0x000BB4DB File Offset: 0x000B96DB
	public override void Reset()
	{
		EntityMoveHelper moveHelper = this.theEntity.moveHelper;
		moveHelper.Stop();
		moveHelper.IsUnreachableAbove = false;
		moveHelper.IsDestroyArea = false;
	}

	// Token: 0x06001E19 RID: 7705 RVA: 0x000BB4FB File Offset: 0x000B96FB
	public override string ToString()
	{
		return string.Format("{0}, {1}, delayTime {2}", base.ToString(), this.state.ToStringCached<EAIDestroyArea.eState>(), this.delayTime.ToCultureInvariantString("0.00"));
	}

	// Token: 0x06001E1A RID: 7706 RVA: 0x000BB528 File Offset: 0x000B9728
	[Conditional("DEBUG_AIDESTROY")]
	[PublicizedFrom(EAccessModifier.Private)]
	public void LogDestroy(string _format = "", params object[] _args)
	{
		_format = string.Format("{0} EAIDestroyArea {1} {2}, {3}", new object[]
		{
			GameManager.frameCount,
			this.theEntity.EntityName,
			this.theEntity.entityId,
			_format
		});
		Log.Warning(_format, _args);
	}

	// Token: 0x040014A9 RID: 5289
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cDumbDistance = 9f;

	// Token: 0x040014AA RID: 5290
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 seekPos;

	// Token: 0x040014AB RID: 5291
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i seekBlockPos;

	// Token: 0x040014AC RID: 5292
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isLookFar;

	// Token: 0x040014AD RID: 5293
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isAtPathEnd;

	// Token: 0x040014AE RID: 5294
	[PublicizedFrom(EAccessModifier.Private)]
	public float delayTime;

	// Token: 0x040014AF RID: 5295
	[PublicizedFrom(EAccessModifier.Private)]
	public int attackTimeout;

	// Token: 0x040014B0 RID: 5296
	[PublicizedFrom(EAccessModifier.Private)]
	public EAIDestroyArea.eState state;

	// Token: 0x040014B1 RID: 5297
	[PublicizedFrom(EAccessModifier.Private)]
	public WorldRayHitInfo hitInfo = new WorldRayHitInfo();

	// Token: 0x020003E3 RID: 995
	[PublicizedFrom(EAccessModifier.Private)]
	public enum eState
	{
		// Token: 0x040014B3 RID: 5299
		FindPath,
		// Token: 0x040014B4 RID: 5300
		HasPath,
		// Token: 0x040014B5 RID: 5301
		EndPath,
		// Token: 0x040014B6 RID: 5302
		Attack
	}
}
