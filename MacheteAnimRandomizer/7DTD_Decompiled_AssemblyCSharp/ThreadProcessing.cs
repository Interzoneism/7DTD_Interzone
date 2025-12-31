using System;
using System.Collections.Generic;

// Token: 0x02000AC0 RID: 2752
public class ThreadProcessing
{
	// Token: 0x060054C4 RID: 21700 RVA: 0x0022A3A4 File Offset: 0x002285A4
	public ThreadProcessing(List<ThreadInfoParam> _JobList)
	{
		this.JobList = new List<ThreadInfoParam>(100);
		this.Init(_JobList);
	}

	// Token: 0x060054C5 RID: 21701 RVA: 0x0022A3C0 File Offset: 0x002285C0
	public ThreadProcessing()
	{
		this.JobList = new List<ThreadInfoParam>(100);
		this.IsCancelled = false;
		this.IsFinished = false;
	}

	// Token: 0x060054C6 RID: 21702 RVA: 0x0022A3E4 File Offset: 0x002285E4
	public void Init(List<ThreadInfoParam> _JobList)
	{
		this.RemoveTreatedElement(_JobList);
		if (this.JobList.Count == 0)
		{
			this.IsFinished = true;
			return;
		}
		this.IsCancelled = false;
		this.IsFinished = false;
		for (int i = 0; i < this.JobList.Count; i++)
		{
			for (int j = 0; j < this.JobList[i].LengthThreadContList; j++)
			{
				DistantChunk dchunk = this.JobList[i].ThreadContListA[j].DChunk;
				dchunk.CellMeshData = DistantChunk.SMPool.GetObject(dchunk.BaseChunkMap, dchunk.ResLevel);
			}
		}
		this.TaskInfo = ThreadManager.AddSingleTask(new ThreadManager.TaskFunctionDelegate(this.ThreadJob), this.JobList, null, true);
	}

	// Token: 0x060054C7 RID: 21703 RVA: 0x0022A4A4 File Offset: 0x002286A4
	public void RemoveTreatedElement(List<ThreadInfoParam> _JobList)
	{
		this.JobList.Clear();
		for (int i = 0; i < _JobList.Count; i++)
		{
			if (!_JobList[i].IsThreadDone)
			{
				this.JobList.Add(_JobList[i]);
			}
		}
	}

	// Token: 0x060054C8 RID: 21704 RVA: 0x0022A4F0 File Offset: 0x002286F0
	[PublicizedFrom(EAccessModifier.Private)]
	public void ThreadJob(ThreadManager.TaskInfo _InfoJob)
	{
		List<ThreadInfoParam> list = (List<ThreadInfoParam>)_InfoJob.parameter;
		object lockObjectThread;
		for (int i = 0; i < list.Count; i++)
		{
			for (int j = 0; j < list[i].LengthThreadContList; j++)
			{
				lockObjectThread = ThreadProcessing.LockObjectThread;
				lock (lockObjectThread)
				{
					if (this.IsCancelled)
					{
						break;
					}
					list[i].ThreadContListA[j].ThreadExtraWork();
				}
			}
			lockObjectThread = ThreadProcessing.LockObjectThread;
			lock (lockObjectThread)
			{
				if (this.IsCancelled)
				{
					break;
				}
				list[i].IsThreadDone = true;
			}
		}
		lockObjectThread = ThreadProcessing.LockObjectThread;
		lock (lockObjectThread)
		{
			this.IsFinished = true;
		}
	}

	// Token: 0x060054C9 RID: 21705 RVA: 0x0022A5F4 File Offset: 0x002287F4
	public void CancelThread()
	{
		object lockObjectThread = ThreadProcessing.LockObjectThread;
		lock (lockObjectThread)
		{
			this.IsCancelled = true;
		}
	}

	// Token: 0x060054CA RID: 21706 RVA: 0x0022A634 File Offset: 0x00228834
	public long CancelThreadAndWaitFinished()
	{
		object lockObjectThread = ThreadProcessing.LockObjectThread;
		lock (lockObjectThread)
		{
			this.IsCancelled = true;
		}
		if (this.TaskInfo != null)
		{
			this.TaskInfo.WaitForEnd();
		}
		return 0L;
	}

	// Token: 0x060054CB RID: 21707 RVA: 0x0022A68C File Offset: 0x0022888C
	public bool IsThreadFinished()
	{
		return this.IsFinished;
	}

	// Token: 0x060054CC RID: 21708 RVA: 0x0022A694 File Offset: 0x00228894
	public bool IsThreadDone(int ThreadInfoParamId)
	{
		object lockObjectThread = ThreadProcessing.LockObjectThread;
		bool isThreadDone;
		lock (lockObjectThread)
		{
			isThreadDone = this.JobList[ThreadInfoParamId].IsThreadDone;
		}
		return isThreadDone;
	}

	// Token: 0x040041A7 RID: 16807
	public bool IsFinished;

	// Token: 0x040041A8 RID: 16808
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsCancelled;

	// Token: 0x040041A9 RID: 16809
	public ThreadManager.TaskInfo TaskInfo;

	// Token: 0x040041AA RID: 16810
	[PublicizedFrom(EAccessModifier.Private)]
	public static object LockObjectThread = new object();

	// Token: 0x040041AB RID: 16811
	public List<ThreadInfoParam> JobList;
}
