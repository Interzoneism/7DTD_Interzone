using System;

namespace GamePath
{
	// Token: 0x020015D5 RID: 5589
	public class PathNavigate
	{
		// Token: 0x0600AB8C RID: 43916 RVA: 0x004376F4 File Offset: 0x004358F4
		public PathNavigate(EntityAlive _ea)
		{
			this.theEntity = _ea;
			this.inWater = false;
			this.canDrown = false;
		}

		// Token: 0x0600AB8D RID: 43917 RVA: 0x00437711 File Offset: 0x00435911
		public void setMoveSpeed(float _b)
		{
			this.speed = _b;
		}

		// Token: 0x0600AB8E RID: 43918 RVA: 0x0043771A File Offset: 0x0043591A
		public void setCanDrown(bool _b)
		{
			this.canDrown = _b;
		}

		// Token: 0x0600AB8F RID: 43919 RVA: 0x00437723 File Offset: 0x00435923
		public bool noPath()
		{
			return this.currentPath == null || this.currentPath.isFinished();
		}

		// Token: 0x0600AB90 RID: 43920 RVA: 0x0043773A File Offset: 0x0043593A
		public bool noPathAndNotPlanningOne()
		{
			return this.noPath() && !PathFinderThread.Instance.IsCalculatingPath(this.theEntity.entityId);
		}

		// Token: 0x0600AB91 RID: 43921 RVA: 0x0043775E File Offset: 0x0043595E
		public bool HasPath()
		{
			return this.currentPath != null && !this.currentPath.isFinished();
		}

		// Token: 0x0600AB92 RID: 43922 RVA: 0x00437778 File Offset: 0x00435978
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool canNavigate()
		{
			return this.theEntity.CanNavigatePath();
		}

		// Token: 0x0600AB93 RID: 43923 RVA: 0x00437785 File Offset: 0x00435985
		public void clearPath()
		{
			if (this.currentPath != null)
			{
				this.currentPath.Destruct();
			}
			this.currentPath = null;
		}

		// Token: 0x0600AB94 RID: 43924 RVA: 0x004377A1 File Offset: 0x004359A1
		public PathEntity getPath()
		{
			return this.currentPath;
		}

		// Token: 0x0600AB95 RID: 43925 RVA: 0x004377A9 File Offset: 0x004359A9
		public void ShortenEnd(float _distance)
		{
			if (this.currentPath != null)
			{
				this.currentPath.ShortenEnd(_distance);
			}
		}

		// Token: 0x0600AB96 RID: 43926 RVA: 0x00002914 File Offset: 0x00000B14
		public virtual void GetPathTo(PathInfo _pathInfo)
		{
		}

		// Token: 0x0600AB97 RID: 43927 RVA: 0x00002914 File Offset: 0x00000B14
		public virtual void GetPathToEntity(PathInfo _pathInfo, EntityAlive _entity)
		{
		}

		// Token: 0x0600AB98 RID: 43928 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public virtual bool SetPath(PathInfo _pathInfo, float _speed)
		{
			return false;
		}

		// Token: 0x0600AB99 RID: 43929 RVA: 0x00002914 File Offset: 0x00000B14
		public virtual void UpdateNavigation()
		{
		}

		// Token: 0x0600AB9A RID: 43930 RVA: 0x00002914 File Offset: 0x00000B14
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void CreatePath()
		{
		}

		// Token: 0x040085B4 RID: 34228
		public PathInfo pathInfo;

		// Token: 0x040085B5 RID: 34229
		[PublicizedFrom(EAccessModifier.Protected)]
		public EntityAlive theEntity;

		// Token: 0x040085B6 RID: 34230
		[PublicizedFrom(EAccessModifier.Protected)]
		public PathEntity currentPath;

		// Token: 0x040085B7 RID: 34231
		[PublicizedFrom(EAccessModifier.Protected)]
		public float speed;

		// Token: 0x040085B8 RID: 34232
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool canBreakBlocks;

		// Token: 0x040085B9 RID: 34233
		[PublicizedFrom(EAccessModifier.Protected)]
		public int curNavTicks;

		// Token: 0x040085BA RID: 34234
		[PublicizedFrom(EAccessModifier.Protected)]
		public int prevNavTicks;

		// Token: 0x040085BB RID: 34235
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool inWater;

		// Token: 0x040085BC RID: 34236
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool canDrown;

		// Token: 0x040085BD RID: 34237
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool? canPathThroughDoors;

		// Token: 0x040085BE RID: 34238
		[PublicizedFrom(EAccessModifier.Protected)]
		public int canPathThroughDoorsDecisionTime;
	}
}
