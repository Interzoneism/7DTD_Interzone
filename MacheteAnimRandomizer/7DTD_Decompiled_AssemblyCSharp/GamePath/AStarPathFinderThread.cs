using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace GamePath
{
	// Token: 0x020015C9 RID: 5577
	public class AStarPathFinderThread : PathFinderThread
	{
		// Token: 0x0600AB3C RID: 43836 RVA: 0x004361C6 File Offset: 0x004343C6
		public AStarPathFinderThread()
		{
			PathFinderThread.Instance = this;
		}

		// Token: 0x0600AB3D RID: 43837 RVA: 0x004361F6 File Offset: 0x004343F6
		public override int GetFinishedCount()
		{
			return this.finishedPaths.Count;
		}

		// Token: 0x0600AB3E RID: 43838 RVA: 0x00436203 File Offset: 0x00434403
		public override int GetQueueCount()
		{
			return this.entityWaitQueue.list.Count;
		}

		// Token: 0x0600AB3F RID: 43839 RVA: 0x00436218 File Offset: 0x00434418
		public override void StartWorkerThreads()
		{
			this.threadInfo = ThreadManager.StartThread("Pathfinder", null, new ThreadManager.ThreadFunctionLoopDelegate(this.thread_Pathfinder), null, null, null, false, false);
		}

		// Token: 0x0600AB40 RID: 43840 RVA: 0x00436248 File Offset: 0x00434448
		public override void Cleanup()
		{
			this.threadInfo.RequestTermination();
			this.writerThreadWaitHandle.Set();
			this.threadInfo.WaitForEnd(30);
			this.threadInfo = null;
			this.entityWaitQueue.Clear();
			this.finishedPaths.Clear();
			this.writerThreadWaitHandle = null;
		}

		// Token: 0x0600AB41 RID: 43841 RVA: 0x004362A0 File Offset: 0x004344A0
		[PublicizedFrom(EAccessModifier.Protected)]
		public int thread_Pathfinder(ThreadManager.ThreadInfo _threadInfo)
		{
			while (!_threadInfo.TerminationRequested())
			{
				try
				{
					if (this.entityWaitQueue.list.Count == 0)
					{
						this.writerThreadWaitHandle.WaitOne();
					}
					PathInfo pathInfo = PathInfo.Empty;
					Dictionary<int, PathInfo> obj = this.finishedPaths;
					lock (obj)
					{
						if (this.entityWaitQueue.list.Count <= 0)
						{
							continue;
						}
						int num = this.entityWaitQueue.list[0];
						this.entityWaitQueue.Remove(num);
						if (!this.finishedPaths.ContainsKey(num))
						{
							continue;
						}
						pathInfo = this.finishedPaths[num];
					}
					pathInfo.entity.navigator.GetPathTo(pathInfo);
					obj = this.finishedPaths;
					lock (obj)
					{
						if (pathInfo.path == null)
						{
							this.finishedPaths.Remove(pathInfo.entity.entityId);
						}
						else
						{
							this.finishedPaths[pathInfo.entity.entityId] = pathInfo;
						}
					}
				}
				catch (Exception ex)
				{
					Log.Error("Exception in PathFinder thread: " + ex.Message);
					Log.Error(ex.StackTrace);
				}
			}
			return -1;
		}

		// Token: 0x0600AB42 RID: 43842 RVA: 0x00436434 File Offset: 0x00434634
		public override bool IsCalculatingPath(int _entityId)
		{
			Dictionary<int, PathInfo> obj = this.finishedPaths;
			bool result;
			lock (obj)
			{
				result = this.finishedPaths.ContainsKey(_entityId);
			}
			return result;
		}

		// Token: 0x0600AB43 RID: 43843 RVA: 0x0043647C File Offset: 0x0043467C
		public override void FindPath(EntityAlive _entity, Vector3 _target, float _speed, bool _canBreak, EAIBase _aiTask)
		{
			Dictionary<int, PathInfo> obj = this.finishedPaths;
			lock (obj)
			{
				if (!this.entityWaitQueue.hashSet.Contains(_entity.entityId))
				{
					this.entityWaitQueue.Add(_entity.entityId);
				}
				this.finishedPaths[_entity.entityId] = new PathInfo(_entity, _target, _canBreak, _speed, _aiTask);
			}
			this.writerThreadWaitHandle.Set();
		}

		// Token: 0x0600AB44 RID: 43844 RVA: 0x00436508 File Offset: 0x00434708
		public override PathInfo GetPath(int _entityId)
		{
			Dictionary<int, PathInfo> obj = this.finishedPaths;
			lock (obj)
			{
				PathInfo pathInfo;
				if (this.finishedPaths.TryGetValue(_entityId, out pathInfo) && pathInfo.path != null)
				{
					this.finishedPaths.Remove(_entityId);
					return pathInfo;
				}
			}
			return PathInfo.Empty;
		}

		// Token: 0x0600AB45 RID: 43845 RVA: 0x00436574 File Offset: 0x00434774
		public override void RemovePathsFor(int _entityId)
		{
			Dictionary<int, PathInfo> obj = this.finishedPaths;
			lock (obj)
			{
				this.finishedPaths.Remove(_entityId);
				if (this.entityWaitQueue.hashSet.Contains(_entityId))
				{
					this.entityWaitQueue.Remove(_entityId);
				}
			}
		}

		// Token: 0x0400858A RID: 34186
		[PublicizedFrom(EAccessModifier.Private)]
		public ThreadManager.ThreadInfo threadInfo;

		// Token: 0x0400858B RID: 34187
		[PublicizedFrom(EAccessModifier.Private)]
		public AutoResetEvent writerThreadWaitHandle = new AutoResetEvent(false);

		// Token: 0x0400858C RID: 34188
		[PublicizedFrom(EAccessModifier.Private)]
		public HashSetList<int> entityWaitQueue = new HashSetList<int>();

		// Token: 0x0400858D RID: 34189
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<int, PathInfo> finishedPaths = new Dictionary<int, PathInfo>();
	}
}
