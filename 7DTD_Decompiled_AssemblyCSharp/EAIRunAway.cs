using System;
using GamePath;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020003EA RID: 1002
[Preserve]
public abstract class EAIRunAway : EAIBase
{
	// Token: 0x06001E53 RID: 7763 RVA: 0x000BD58F File Offset: 0x000BB78F
	public EAIRunAway()
	{
	}

	// Token: 0x06001E54 RID: 7764 RVA: 0x000BD59F File Offset: 0x000BB79F
	public override void SetData(DictionarySave<string, string> data)
	{
		base.SetData(data);
		base.GetData(data, "fleeDistance", ref this.fleeDistance);
	}

	// Token: 0x06001E55 RID: 7765 RVA: 0x000BD5BA File Offset: 0x000BB7BA
	public override bool CanExecute()
	{
		return this.FindFleePos(this.GetFleeFromPos());
	}

	// Token: 0x06001E56 RID: 7766 RVA: 0x000BD5C8 File Offset: 0x000BB7C8
	public override void Start()
	{
		this.timeoutTicks = (30 + base.GetRandom(20)) * 20;
		PathFinderThread.Instance.RemovePathsFor(this.theEntity.entityId);
	}

	// Token: 0x06001E57 RID: 7767 RVA: 0x000BD5F3 File Offset: 0x000BB7F3
	public override bool Continue()
	{
		return this.timeoutTicks > 0;
	}

	// Token: 0x06001E58 RID: 7768 RVA: 0x000BD600 File Offset: 0x000BB800
	public override void Update()
	{
		this.timeoutTicks--;
		PathEntity path = this.theEntity.navigator.getPath();
		if (!this.checkedPath && !PathFinderThread.Instance.IsCalculatingPath(this.theEntity.entityId))
		{
			this.checkedPath = true;
			if (path != null)
			{
				Vector3 rawEndPos = path.rawEndPos;
				if (this.theEntity.GetDistanceSq(rawEndPos) < 1.21f)
				{
					this.FindRandomPos();
				}
			}
		}
		if (this.checkedPath && path != null && path.getCurrentPathLength() >= 2 && path.NodeCountRemaining() <= 2)
		{
			this.fleeTicks = 0;
		}
		int num = this.fleeTicks - 1;
		this.fleeTicks = num;
		if (num <= 0)
		{
			Vector3 fleeFromPos = this.GetFleeFromPos();
			this.FindFleePos(fleeFromPos);
		}
		num = this.pathTicks - 1;
		this.pathTicks = num;
		if (num <= 0 && !PathFinderThread.Instance.IsCalculatingPath(this.theEntity.entityId))
		{
			this.pathTicks = 60;
			this.theEntity.FindPath(this.targetPos, this.theEntity.GetMoveSpeed(), false, this);
			this.checkedPath = false;
		}
	}

	// Token: 0x06001E59 RID: 7769 RVA: 0x000BD718 File Offset: 0x000BB918
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool FindFleePos(Vector3 fleeFromPos)
	{
		Vector3 dirV = this.theEntity.position - fleeFromPos;
		Vector3 vector = RandomPositionGenerator.CalcPositionInDirection(this.theEntity, this.theEntity.position, dirV, (float)this.fleeDistance, 80f);
		if (vector.y == 0f)
		{
			return false;
		}
		this.targetPos = vector;
		this.fleeTicks = 80;
		this.pathTicks = 0;
		this.checkedPath = false;
		return true;
	}

	// Token: 0x06001E5A RID: 7770 RVA: 0x000BD788 File Offset: 0x000BB988
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool FindRandomPos()
	{
		Vector3 vector = RandomPositionGenerator.CalcAround(this.theEntity, this.fleeDistance, 0);
		if (vector.y == 0f)
		{
			this.fleeTicks = 0;
			return false;
		}
		this.targetPos = vector;
		this.fleeTicks = 80;
		this.pathTicks = 0;
		this.checkedPath = false;
		return true;
	}

	// Token: 0x06001E5B RID: 7771
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract Vector3 GetFleeFromPos();

	// Token: 0x06001E5C RID: 7772 RVA: 0x000BD7DC File Offset: 0x000BB9DC
	public override string ToString()
	{
		return string.Format("{0}, flee {1}, timeout {2}", base.ToString(), this.fleeTicks, this.timeoutTicks);
	}

	// Token: 0x040014F0 RID: 5360
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 targetPos;

	// Token: 0x040014F1 RID: 5361
	[PublicizedFrom(EAccessModifier.Private)]
	public int timeoutTicks;

	// Token: 0x040014F2 RID: 5362
	[PublicizedFrom(EAccessModifier.Private)]
	public int fleeTicks;

	// Token: 0x040014F3 RID: 5363
	[PublicizedFrom(EAccessModifier.Private)]
	public int pathTicks;

	// Token: 0x040014F4 RID: 5364
	[PublicizedFrom(EAccessModifier.Private)]
	public bool checkedPath;

	// Token: 0x040014F5 RID: 5365
	[PublicizedFrom(EAccessModifier.Private)]
	public int fleeDistance = 20;
}
