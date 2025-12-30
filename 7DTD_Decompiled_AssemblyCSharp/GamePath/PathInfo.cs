using System;
using UnityEngine;

namespace GamePath
{
	// Token: 0x020015D3 RID: 5587
	public class PathInfo
	{
		// Token: 0x0600AB89 RID: 43913 RVA: 0x00437690 File Offset: 0x00435890
		public PathInfo(EntityAlive _entity, Vector3 _targetPos, bool _canBreakBlocks, float _speed, EAIBase _aiTask)
		{
			this.entity = _entity;
			this.hasStart = false;
			this.targetPos = _targetPos;
			this.canBreakBlocks = _canBreakBlocks;
			this.speed = _speed;
			this.aiTask = _aiTask;
			this.path = null;
		}

		// Token: 0x0600AB8A RID: 43914 RVA: 0x004376CB File Offset: 0x004358CB
		public void SetStartPos(Vector3 _startPos)
		{
			this.startPos = _startPos;
			this.hasStart = true;
		}

		// Token: 0x040085A5 RID: 34213
		public static PathInfo Empty = new PathInfo(null, Vector3.zero, false, 0f, null);

		// Token: 0x040085A6 RID: 34214
		public EntityAlive entity;

		// Token: 0x040085A7 RID: 34215
		public PathInfo.State state;

		// Token: 0x040085A8 RID: 34216
		public bool hasStart;

		// Token: 0x040085A9 RID: 34217
		public Vector3 startPos;

		// Token: 0x040085AA RID: 34218
		public Vector3 targetPos;

		// Token: 0x040085AB RID: 34219
		public bool canBreakBlocks;

		// Token: 0x040085AC RID: 34220
		public float speed;

		// Token: 0x040085AD RID: 34221
		public EAIBase aiTask;

		// Token: 0x040085AE RID: 34222
		public ChunkCache chunkcache;

		// Token: 0x040085AF RID: 34223
		public PathEntity path;

		// Token: 0x020015D4 RID: 5588
		public enum State
		{
			// Token: 0x040085B1 RID: 34225
			Queued,
			// Token: 0x040085B2 RID: 34226
			Pathing,
			// Token: 0x040085B3 RID: 34227
			Done
		}
	}
}
