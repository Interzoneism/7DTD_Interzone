using System;
using GamePath;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020003D9 RID: 985
[Preserve]
public class EAIApproachDistraction : EAIBase
{
	// Token: 0x06001DDB RID: 7643 RVA: 0x000BA3A4 File Offset: 0x000B85A4
	public EAIApproachDistraction()
	{
		this.MutexBits = 3;
	}

	// Token: 0x06001DDC RID: 7644 RVA: 0x000BA3B4 File Offset: 0x000B85B4
	public override bool CanExecute()
	{
		EntityItem pendingDistraction = this.theEntity.pendingDistraction;
		if (!pendingDistraction || pendingDistraction.itemClass == null)
		{
			return false;
		}
		if (this.theEntity.GetAttackTarget())
		{
			if (!pendingDistraction.itemClass.IsEatDistraction)
			{
				this.theEntity.pendingDistraction = null;
			}
			return false;
		}
		if ((this.theEntity.position - pendingDistraction.position).sqrMagnitude < 2.25f && !pendingDistraction.itemClass.IsEatDistraction)
		{
			this.theEntity.pendingDistraction = null;
			return false;
		}
		return true;
	}

	// Token: 0x06001DDD RID: 7645 RVA: 0x000BA450 File Offset: 0x000B8650
	public override void Start()
	{
		this.theEntity.SetAttackTarget(null, 0);
		this.theEntity.IsEating = false;
		this.theEntity.distraction = this.theEntity.pendingDistraction;
		this.theEntity.pendingDistraction = null;
		this.hadPath = false;
		this.updatePath();
	}

	// Token: 0x06001DDE RID: 7646 RVA: 0x000BA4A8 File Offset: 0x000B86A8
	public override bool Continue()
	{
		PathEntity path = this.theEntity.navigator.getPath();
		if (this.hadPath && path == null)
		{
			return false;
		}
		EntityItem distraction = this.theEntity.distraction;
		return !(distraction == null) && distraction.itemClass != null && (((this.theEntity.position - distraction.position).sqrMagnitude > 2.25f && (path == null || !path.isFinished())) || (distraction.itemClass.IsEatDistraction && distraction.IsDistractionActive));
	}

	// Token: 0x06001DDF RID: 7647 RVA: 0x000BA53C File Offset: 0x000B873C
	public override void Update()
	{
		EntityItem distraction = this.theEntity.distraction;
		if (!distraction)
		{
			return;
		}
		PathEntity path = this.theEntity.getNavigator().getPath();
		if (path != null)
		{
			this.hadPath = true;
		}
		bool flag = false;
		if (path != null && !path.isFinished() && !this.theEntity.isCollidedHorizontally)
		{
			flag = true;
		}
		if (this.theEntity.IsSwimming())
		{
			flag = true;
		}
		if (Mathf.Abs(this.theEntity.speedForward) > 0.01f || Mathf.Abs(this.theEntity.speedStrafe) > 0.01f)
		{
			flag = true;
		}
		if (flag)
		{
			this.theEntity.SetLookPosition(distraction.position);
		}
		if ((this.theEntity.GetPosition() - distraction.position).sqrMagnitude <= 2.25f)
		{
			this.theEntity.IsEating = true;
			distraction.distractionEatTicks--;
			return;
		}
		int num = this.pathRecalculateTicks - 1;
		this.pathRecalculateTicks = num;
		if (num <= 0)
		{
			this.updatePath();
		}
	}

	// Token: 0x06001DE0 RID: 7648 RVA: 0x000BA648 File Offset: 0x000B8848
	[PublicizedFrom(EAccessModifier.Private)]
	public void updatePath()
	{
		if (PathFinderThread.Instance.IsCalculatingPath(this.theEntity.entityId))
		{
			return;
		}
		this.pathRecalculateTicks = 20 + base.GetRandom(20);
		this.theEntity.FindPath(this.theEntity.distraction.position, this.theEntity.GetMoveSpeedAggro(), true, this);
	}

	// Token: 0x06001DE1 RID: 7649 RVA: 0x000BA6A8 File Offset: 0x000B88A8
	public override void Reset()
	{
		this.theEntity.moveHelper.Stop();
		this.theEntity.SetLookPosition(Vector3.zero);
		this.theEntity.IsEating = false;
		this.theEntity.distraction = null;
		this.manager.lookTime = 2f;
	}

	// Token: 0x04001483 RID: 5251
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cCloseDist = 1.5f;

	// Token: 0x04001484 RID: 5252
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cLookTime = 2f;

	// Token: 0x04001485 RID: 5253
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hadPath;

	// Token: 0x04001486 RID: 5254
	[PublicizedFrom(EAccessModifier.Private)]
	public int pathRecalculateTicks;
}
