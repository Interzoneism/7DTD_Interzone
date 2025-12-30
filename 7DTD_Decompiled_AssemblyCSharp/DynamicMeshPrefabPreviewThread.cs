using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

// Token: 0x02000311 RID: 785
public class DynamicMeshPrefabPreviewThread
{
	// Token: 0x1700027C RID: 636
	// (get) Token: 0x06001647 RID: 5703 RVA: 0x00081BF2 File Offset: 0x0007FDF2
	public bool HasStopBeenRequested
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.CancelToken.IsCancellationRequested;
		}
	}

	// Token: 0x06001648 RID: 5704 RVA: 0x00081C00 File Offset: 0x0007FE00
	public void StartThread()
	{
		this.StopThread();
		DynamicMeshPrefabPreviewThread.Instance = this;
		this.BuilderManager = DynamicMeshBuilderManager.GetOrCreate();
		this.TokenSource = new CancellationTokenSource();
		this.Wait = new AutoResetEvent(false);
		this.CancelToken = this.TokenSource.Token;
		this.WaitHandles[0] = this.CancelToken.WaitHandle;
		this.WaitHandles[1] = this.Wait;
		this.PreviewCheckThread = new Thread(new ThreadStart(this.ThreadLoop));
		this.PreviewCheckThread.Start();
	}

	// Token: 0x06001649 RID: 5705 RVA: 0x00081C90 File Offset: 0x0007FE90
	[PublicizedFrom(EAccessModifier.Private)]
	public void ThreadLoop()
	{
		while (!this.HasStopBeenRequested)
		{
			this.BuilderManager.CheckPreviews();
			bool flag = this.ProcessList();
			if (!this.ChunksToProcess.IsEmpty)
			{
				if (!flag)
				{
					Thread.Sleep(100);
				}
			}
			else
			{
				WaitHandle.WaitAny(this.WaitHandles);
			}
		}
	}

	// Token: 0x0600164A RID: 5706 RVA: 0x00081CE0 File Offset: 0x0007FEE0
	public void StopThread()
	{
		if (this.PreviewCheckThread == null)
		{
			return;
		}
		this.TokenSource.Cancel();
		DateTime t = DateTime.Now.AddSeconds(3.0);
		while (this.PreviewCheckThread.IsAlive || t > DateTime.Now)
		{
			Thread.Sleep(10);
		}
		Thread previewCheckThread = this.PreviewCheckThread;
		if (previewCheckThread != null && previewCheckThread.IsAlive)
		{
			try
			{
				Thread previewCheckThread2 = this.PreviewCheckThread;
				if (previewCheckThread2 != null)
				{
					previewCheckThread2.Abort();
				}
			}
			catch
			{
			}
		}
	}

	// Token: 0x0600164B RID: 5707 RVA: 0x00081D78 File Offset: 0x0007FF78
	public void AddChunk(DynamicMeshItem item)
	{
		this.LockGenerationUntil = DateTime.Now.AddMilliseconds(300.0);
		this.ChunksToProcess.TryAdd(item.Key, item);
		this.Wait.Set();
	}

	// Token: 0x0600164C RID: 5708 RVA: 0x00081DC0 File Offset: 0x0007FFC0
	public void ClearChunks()
	{
		this.ChunksToProcess.Clear();
	}

	// Token: 0x0600164D RID: 5709 RVA: 0x00081DCD File Offset: 0x0007FFCD
	public void CleanUp()
	{
		this.BuilderManager.StopThreads(true);
		this.ChunksToProcess.Clear();
	}

	// Token: 0x0600164E RID: 5710 RVA: 0x00081DE8 File Offset: 0x0007FFE8
	public bool ProcessList()
	{
		if (this.LockGenerationUntil > DateTime.Now)
		{
			return false;
		}
		KeyValuePair<long, DynamicMeshItem> keyValuePair = this.ChunksToProcess.FirstOrDefault<KeyValuePair<long, DynamicMeshItem>>();
		if (keyValuePair.Value == null)
		{
			return false;
		}
		DynamicMeshItem value = keyValuePair.Value;
		DynamicMeshChunkProcessor nextBuilder = this.BuilderManager.GetNextBuilder();
		if (((nextBuilder != null) ? nextBuilder.AddItemForMeshPreview(value, this.PreviewData) : 0) != 1)
		{
			return false;
		}
		DynamicMeshItem dynamicMeshItem;
		this.ChunksToProcess.TryRemove(value.Key, out dynamicMeshItem);
		return true;
	}

	// Token: 0x04000E09 RID: 3593
	public ConcurrentDictionary<long, DynamicMeshItem> ChunksToProcess = new ConcurrentDictionary<long, DynamicMeshItem>();

	// Token: 0x04000E0A RID: 3594
	[PublicizedFrom(EAccessModifier.Private)]
	public DynamicMeshBuilderManager BuilderManager;

	// Token: 0x04000E0B RID: 3595
	public static DynamicMeshPrefabPreviewThread Instance;

	// Token: 0x04000E0C RID: 3596
	public ChunkPreviewData PreviewData;

	// Token: 0x04000E0D RID: 3597
	[PublicizedFrom(EAccessModifier.Private)]
	public AutoResetEvent Wait;

	// Token: 0x04000E0E RID: 3598
	[PublicizedFrom(EAccessModifier.Private)]
	public CancellationTokenSource TokenSource;

	// Token: 0x04000E0F RID: 3599
	[PublicizedFrom(EAccessModifier.Private)]
	public CancellationToken CancelToken;

	// Token: 0x04000E10 RID: 3600
	[PublicizedFrom(EAccessModifier.Private)]
	public WaitHandle[] WaitHandles = new WaitHandle[2];

	// Token: 0x04000E11 RID: 3601
	[PublicizedFrom(EAccessModifier.Private)]
	public Thread PreviewCheckThread;

	// Token: 0x04000E12 RID: 3602
	[PublicizedFrom(EAccessModifier.Private)]
	public bool chunkAdded;

	// Token: 0x04000E13 RID: 3603
	[PublicizedFrom(EAccessModifier.Private)]
	public DateTime LockGenerationUntil;
}
