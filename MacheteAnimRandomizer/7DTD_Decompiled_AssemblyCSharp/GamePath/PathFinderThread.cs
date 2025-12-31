using System;
using UnityEngine;

namespace GamePath
{
	// Token: 0x020015D2 RID: 5586
	public class PathFinderThread
	{
		// Token: 0x0600AB7F RID: 43903 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public virtual int GetFinishedCount()
		{
			return 0;
		}

		// Token: 0x0600AB80 RID: 43904 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public virtual int GetQueueCount()
		{
			return 0;
		}

		// Token: 0x0600AB81 RID: 43905 RVA: 0x00002914 File Offset: 0x00000B14
		public virtual void StartWorkerThreads()
		{
		}

		// Token: 0x0600AB82 RID: 43906 RVA: 0x00002914 File Offset: 0x00000B14
		public virtual void Cleanup()
		{
		}

		// Token: 0x0600AB83 RID: 43907 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public virtual bool IsCalculatingPath(int _entityId)
		{
			return false;
		}

		// Token: 0x0600AB84 RID: 43908 RVA: 0x00002914 File Offset: 0x00000B14
		public virtual void FindPath(EntityAlive _entity, Vector3 _targetPos, float _speed, bool _canBreak, EAIBase _aiTask)
		{
		}

		// Token: 0x0600AB85 RID: 43909 RVA: 0x00002914 File Offset: 0x00000B14
		public virtual void FindPath(EntityAlive _entity, Vector3 _startPos, Vector3 _targetPos, float _speed, bool _canBreak, EAIBase _aiTask)
		{
		}

		// Token: 0x0600AB86 RID: 43910 RVA: 0x00437689 File Offset: 0x00435889
		public virtual PathInfo GetPath(int _entityId)
		{
			return PathInfo.Empty;
		}

		// Token: 0x0600AB87 RID: 43911 RVA: 0x00002914 File Offset: 0x00000B14
		public virtual void RemovePathsFor(int _entityId)
		{
		}

		// Token: 0x040085A4 RID: 34212
		public static PathFinderThread Instance;
	}
}
