using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePath
{
	// Token: 0x020015CD RID: 5581
	public class ASPPathFinderThread : PathFinderThread
	{
		// Token: 0x0600AB51 RID: 43857 RVA: 0x00436DB0 File Offset: 0x00434FB0
		public ASPPathFinderThread()
		{
			PathFinderThread.Instance = this;
		}

		// Token: 0x0600AB52 RID: 43858 RVA: 0x00436DD4 File Offset: 0x00434FD4
		public override int GetFinishedCount()
		{
			return this.finishedPaths.Count;
		}

		// Token: 0x0600AB53 RID: 43859 RVA: 0x00436DE1 File Offset: 0x00434FE1
		public override int GetQueueCount()
		{
			return this.entityWaitQueue.list.Count;
		}

		// Token: 0x0600AB54 RID: 43860 RVA: 0x00436DF3 File Offset: 0x00434FF3
		public override void StartWorkerThreads()
		{
			this.coroutine = GameManager.Instance.StartCoroutine(this.FindPaths());
		}

		// Token: 0x0600AB55 RID: 43861 RVA: 0x00436E0B File Offset: 0x0043500B
		public override void Cleanup()
		{
			GameManager.Instance.StopCoroutine(this.coroutine);
			this.entityWaitQueue.Clear();
			this.finishedPaths.Clear();
		}

		// Token: 0x0600AB56 RID: 43862 RVA: 0x00436E33 File Offset: 0x00435033
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator FindPaths()
		{
			for (;;)
			{
				int num = 0;
				while (num < 8 && this.entityWaitQueue.list.Count != 0)
				{
					int num2 = this.entityWaitQueue.list[0];
					this.entityWaitQueue.Remove(num2);
					PathInfo pathInfo;
					if (!this.finishedPaths.TryGetValue(num2, out pathInfo))
					{
						Log.Warning("{0} path dup id {1}", new object[]
						{
							GameManager.frameCount,
							num2
						});
					}
					else
					{
						pathInfo.entity.navigator.GetPathTo(pathInfo);
						if (pathInfo.state == PathInfo.State.Queued)
						{
							this.finishedPaths.Remove(num2);
						}
					}
					num++;
				}
				yield return null;
			}
			yield break;
		}

		// Token: 0x0600AB57 RID: 43863 RVA: 0x00436E42 File Offset: 0x00435042
		public override void FindPath(EntityAlive _entity, Vector3 _targetPos, float _speed, bool _canBreak, EAIBase _aiTask)
		{
			this.entityWaitQueue.Add(_entity.entityId);
			this.finishedPaths[_entity.entityId] = new PathInfo(_entity, _targetPos, _canBreak, _speed, _aiTask);
		}

		// Token: 0x0600AB58 RID: 43864 RVA: 0x00436E74 File Offset: 0x00435074
		public override void FindPath(EntityAlive _entity, Vector3 _startPos, Vector3 _targetPos, float _speed, bool _canBreak, EAIBase _aiTask)
		{
			this.entityWaitQueue.Add(_entity.entityId);
			PathInfo pathInfo = new PathInfo(_entity, _targetPos, _canBreak, _speed, _aiTask);
			pathInfo.SetStartPos(_startPos);
			this.finishedPaths[_entity.entityId] = pathInfo;
		}

		// Token: 0x0600AB59 RID: 43865 RVA: 0x00436EBC File Offset: 0x004350BC
		public override PathInfo GetPath(int _entityId)
		{
			PathInfo pathInfo;
			if (this.finishedPaths.TryGetValue(_entityId, out pathInfo) && pathInfo.state == PathInfo.State.Done)
			{
				this.finishedPaths.Remove(_entityId);
				return pathInfo;
			}
			return PathInfo.Empty;
		}

		// Token: 0x0600AB5A RID: 43866 RVA: 0x00436EF6 File Offset: 0x004350F6
		public override bool IsCalculatingPath(int _entityId)
		{
			return this.finishedPaths.ContainsKey(_entityId);
		}

		// Token: 0x0600AB5B RID: 43867 RVA: 0x00436F04 File Offset: 0x00435104
		public override void RemovePathsFor(int _entityId)
		{
			this.finishedPaths.Remove(_entityId);
			this.entityWaitQueue.Remove(_entityId);
		}

		// Token: 0x04008594 RID: 34196
		[PublicizedFrom(EAccessModifier.Private)]
		public Coroutine coroutine;

		// Token: 0x04008595 RID: 34197
		[PublicizedFrom(EAccessModifier.Private)]
		public HashSetList<int> entityWaitQueue = new HashSetList<int>();

		// Token: 0x04008596 RID: 34198
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<int, PathInfo> finishedPaths = new Dictionary<int, PathInfo>();
	}
}
