using System;
using Pathfinding;
using UnityEngine;

namespace GamePath
{
	// Token: 0x020015CF RID: 5583
	[PublicizedFrom(EAccessModifier.Internal)]
	public class ASPPathNavigate : PathNavigate
	{
		// Token: 0x0600AB62 RID: 43874 RVA: 0x00437027 File Offset: 0x00435227
		public ASPPathNavigate(EntityAlive _ea) : base(_ea)
		{
		}

		// Token: 0x0600AB63 RID: 43875 RVA: 0x00437030 File Offset: 0x00435230
		public override void GetPathTo(PathInfo _pathInfo)
		{
			if (this.pathFinder != null)
			{
				this.pathFinder.Cancel();
				this.pathFinder = null;
			}
			this.pathInfo = _pathInfo;
			if (!base.canNavigate())
			{
				return;
			}
			this.CreatePath();
		}

		// Token: 0x0600AB64 RID: 43876 RVA: 0x00437064 File Offset: 0x00435264
		public override bool SetPath(PathInfo _pathInfo, float _speed)
		{
			PathEntity pathEntity = (_pathInfo != null) ? _pathInfo.path : null;
			if (pathEntity == null)
			{
				if (this.currentPath != null)
				{
					this.currentPath.Destruct();
				}
				this.currentPath = null;
				return false;
			}
			if (this.currentPath != null)
			{
				this.currentPath.Destruct();
			}
			this.currentPath = pathEntity;
			if (this.currentPath.getCurrentPathLength() == 0)
			{
				return true;
			}
			this.ImprovePath();
			this.speed = _speed;
			this.canBreakBlocks = _pathInfo.canBreakBlocks;
			return true;
		}

		// Token: 0x0600AB65 RID: 43877 RVA: 0x004370E0 File Offset: 0x004352E0
		[PublicizedFrom(EAccessModifier.Private)]
		public void ImprovePath()
		{
			PathPoint[] points = this.currentPath.points;
			int num = points.Length;
			for (int i = 0; i < num; i++)
			{
				points[i].ProjectToGround(this.theEntity);
			}
			if (num >= 2)
			{
				Vector3 projectedLocation = points[0].projectedLocation;
				Vector3 projectedLocation2 = points[1].projectedLocation;
				if (projectedLocation2.y - projectedLocation.y < 0.6f)
				{
					points[0].projectedLocation = VectorMath.ClosestPointOnSegment(projectedLocation, projectedLocation2, this.theEntity.position);
				}
			}
		}

		// Token: 0x0600AB66 RID: 43878 RVA: 0x00437160 File Offset: 0x00435360
		public override void UpdateNavigation()
		{
			this.canPathThroughDoorsDecisionTime++;
			if (base.noPath())
			{
				return;
			}
			this.pathFollow();
			if (base.noPath())
			{
				return;
			}
			this.theEntity.moveHelper.SetMoveTo(this.currentPath, this.speed, this.canBreakBlocks);
		}

		// Token: 0x0600AB67 RID: 43879 RVA: 0x004371B8 File Offset: 0x004353B8
		[PublicizedFrom(EAccessModifier.Private)]
		public void pathFollow()
		{
			Vector3 vector = this.currentPath.CurrentPoint.ProjectToGround(this.theEntity);
			Vector3 position = this.theEntity.position;
			Vector3 b = VectorMath.ClosestPointOnSegment(this.theEntity.prevPos, position, vector);
			Vector3 vector2 = vector - b;
			float num = Utils.FastAbs(vector2.y);
			vector2.y = 0f;
			float num2 = this.theEntity.radius * 0.6f;
			float v = 0.15f;
			float num3 = 2f;
			int currentPathIndex = this.currentPath.getCurrentPathIndex();
			int currentPathLength = this.currentPath.getCurrentPathLength();
			if (currentPathIndex + 1 < currentPathLength)
			{
				v = ((this.theEntity.moveHelper.SideStepAngle != 0f) ? 0.49f : 0.33f);
			}
			if (this.theEntity.isSwimming)
			{
				v = 0.9f;
				num3 = 0.7f;
			}
			if (this.theEntity.IsInElevator())
			{
				num3 = 0.2f;
			}
			num2 = Utils.FastMax(v, num2);
			bool flag = false;
			PathPoint nextPoint = this.currentPath.NextPoint;
			if (nextPoint != null)
			{
				Vector3 vector3 = nextPoint.ProjectToGround(this.theEntity);
				if ((VectorMath.ClosestPointOnSegment(vector, vector3, position) - position).sqrMagnitude < 0.040000003f)
				{
					flag = true;
				}
				if (vector.y - vector3.y > 2f)
				{
					Plane plane = new Plane(vector3 - vector, vector);
					if (plane.SameSide(position, vector3))
					{
						flag = true;
					}
				}
			}
			if (flag || (vector2.sqrMagnitude <= num2 * num2 && num <= num3))
			{
				if (currentPathIndex + 1 < currentPathLength)
				{
					this.currentPath.setCurrentPathIndex(currentPathIndex + 1, this.theEntity, position);
					return;
				}
				this.currentPath.setCurrentPathIndex(currentPathLength, this.theEntity, position);
			}
		}

		// Token: 0x0600AB68 RID: 43880 RVA: 0x00437384 File Offset: 0x00435584
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void CreatePath()
		{
			EntityAlive theEntity = this.theEntity;
			this.pathFinder = new ASPPathFinder(this.pathInfo, this.canDrown, theEntity.bCanClimbLadders, theEntity.bCanClimbVertical);
			if (this.pathInfo.hasStart)
			{
				this.pathFinder.Calculate(this.pathInfo.startPos, this.pathInfo.targetPos);
				return;
			}
			Vector3 fromPos = theEntity.position + theEntity.motion * 2.5f;
			this.pathFinder.Calculate(fromPos, this.pathInfo.targetPos);
		}

		// Token: 0x0600AB69 RID: 43881 RVA: 0x0043741D File Offset: 0x0043561D
		[PublicizedFrom(EAccessModifier.Private)]
		public void CreatePathToEntity(EntityAlive _fromEntity, EntityAlive _toEntity)
		{
			this.pathFinder = new ASPPathFinder(this.pathInfo, this.canDrown, _fromEntity.bCanClimbLadders, _fromEntity.bCanClimbVertical);
			this.pathFinder.Calculate(_fromEntity.position, _toEntity.position);
		}

		// Token: 0x0400859A RID: 34202
		[PublicizedFrom(EAccessModifier.Private)]
		public ASPPathFinder pathFinder;
	}
}
