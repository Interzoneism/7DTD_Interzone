using System;
using GamePath;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020003DA RID: 986
[Preserve]
public class EAIApproachSpot : EAIBase
{
	// Token: 0x06001DE2 RID: 7650 RVA: 0x000B929B File Offset: 0x000B749B
	public override void Init(EntityAlive _theEntity)
	{
		base.Init(_theEntity);
		this.MutexBits = 3;
		this.executeDelay = 0.1f;
	}

	// Token: 0x06001DE3 RID: 7651 RVA: 0x000BA700 File Offset: 0x000B8900
	public override bool CanExecute()
	{
		if (!this.theEntity.HasInvestigatePosition)
		{
			return false;
		}
		if (this.theEntity.IsSleeping)
		{
			return false;
		}
		this.investigatePos = this.theEntity.InvestigatePosition;
		this.seekPos = this.theEntity.world.FindSupportingBlockPos(this.investigatePos);
		return true;
	}

	// Token: 0x06001DE4 RID: 7652 RVA: 0x000BA759 File Offset: 0x000B8959
	public override void Start()
	{
		this.hadPath = false;
		this.updatePath();
	}

	// Token: 0x06001DE5 RID: 7653 RVA: 0x000BA768 File Offset: 0x000B8968
	public override bool Continue()
	{
		PathEntity path = this.theEntity.navigator.getPath();
		if (this.hadPath && path == null)
		{
			return false;
		}
		int num = this.investigateTicks + 1;
		this.investigateTicks = num;
		if (num > 40)
		{
			this.investigateTicks = 0;
			if (!this.theEntity.HasInvestigatePosition)
			{
				return false;
			}
			if ((this.investigatePos - this.theEntity.InvestigatePosition).sqrMagnitude >= 4f)
			{
				return false;
			}
		}
		if ((this.seekPos - this.theEntity.position).sqrMagnitude <= 4f || (path != null && path.isFinished()))
		{
			this.theEntity.ClearInvestigatePosition();
			return false;
		}
		return true;
	}

	// Token: 0x06001DE6 RID: 7654 RVA: 0x000BA824 File Offset: 0x000B8A24
	public override void Update()
	{
		if (this.theEntity.navigator.getPath() != null)
		{
			this.hadPath = true;
			this.theEntity.moveHelper.CalcIfUnreachablePos();
		}
		Vector3 lookPosition = this.investigatePos;
		lookPosition.y += 0.8f;
		this.theEntity.SetLookPosition(lookPosition);
		int num = this.pathRecalculateTicks - 1;
		this.pathRecalculateTicks = num;
		if (num <= 0)
		{
			this.updatePath();
		}
	}

	// Token: 0x06001DE7 RID: 7655 RVA: 0x000BA898 File Offset: 0x000B8A98
	[PublicizedFrom(EAccessModifier.Private)]
	public void updatePath()
	{
		if (this.theEntity.IsScoutZombie)
		{
			AstarManager.Instance.AddLocationLine(this.theEntity.position, this.seekPos, 32);
		}
		if (PathFinderThread.Instance.IsCalculatingPath(this.theEntity.entityId))
		{
			return;
		}
		this.pathRecalculateTicks = 40 + base.GetRandom(20);
		this.theEntity.FindPath(this.seekPos, this.theEntity.GetMoveSpeedAggro(), true, this);
	}

	// Token: 0x06001DE8 RID: 7656 RVA: 0x000BA918 File Offset: 0x000B8B18
	public override void Reset()
	{
		this.theEntity.moveHelper.Stop();
		this.theEntity.SetLookPosition(Vector3.zero);
		this.manager.lookTime = 5f + base.RandomFloat * 3f;
		this.manager.interestDistance = 2f;
	}

	// Token: 0x06001DE9 RID: 7657 RVA: 0x000BA974 File Offset: 0x000B8B74
	public override string ToString()
	{
		return string.Format("{0}, {1} dist{2}", base.ToString(), this.theEntity.navigator.noPathAndNotPlanningOne() ? "(-path)" : (this.theEntity.navigator.noPath() ? "(!path)" : ""), (this.theEntity.position - this.seekPos).magnitude.ToCultureInvariantString());
	}

	// Token: 0x04001487 RID: 5255
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cInvestigateChangeDist = 2f;

	// Token: 0x04001488 RID: 5256
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cCloseDist = 2f;

	// Token: 0x04001489 RID: 5257
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cLookTimeMin = 5f;

	// Token: 0x0400148A RID: 5258
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cLookTimeMax = 8f;

	// Token: 0x0400148B RID: 5259
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 investigatePos;

	// Token: 0x0400148C RID: 5260
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 seekPos;

	// Token: 0x0400148D RID: 5261
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hadPath;

	// Token: 0x0400148E RID: 5262
	[PublicizedFrom(EAccessModifier.Private)]
	public int investigateTicks;

	// Token: 0x0400148F RID: 5263
	[PublicizedFrom(EAccessModifier.Private)]
	public int pathRecalculateTicks;
}
