using System;
using System.Diagnostics;
using System.Threading;

// Token: 0x02000351 RID: 849
[DebuggerDisplay("{X},{Z} {StateInfo}")]
public class DynamicMeshRegionDataWrapper
{
	// Token: 0x060018EB RID: 6379 RVA: 0x00098D5C File Offset: 0x00096F5C
	public void Reset()
	{
		this.StateInfo = DynamicMeshStates.None;
	}

	// Token: 0x060018EC RID: 6380 RVA: 0x00098D68 File Offset: 0x00096F68
	public bool IsReadyForRelease()
	{
		if (this.StateInfo.HasFlag(DynamicMeshStates.SaveRequired))
		{
			if (DynamicMeshManager.DebugReleases)
			{
				Log.Out(string.Format("{0},{1} save required", this.X, this.Z));
			}
			return false;
		}
		if (this.StateInfo.HasFlag(DynamicMeshStates.ThreadUpdating) || this.StateInfo.HasFlag(DynamicMeshStates.Generating))
		{
			if (DynamicMeshManager.DebugReleases)
			{
				Log.Out(string.Format("{0},{1} thread updating", this.X, this.Z));
			}
			return false;
		}
		return true;
	}

	// Token: 0x060018ED RID: 6381 RVA: 0x00098E1E File Offset: 0x0009701E
	public string Path()
	{
		return DynamicMeshFile.MeshLocation + string.Format("{0}.group", WorldChunkCache.MakeChunkKey(World.toChunkXZ(this.X), World.toChunkXZ(this.Z)));
	}

	// Token: 0x060018EE RID: 6382 RVA: 0x00098E54 File Offset: 0x00097054
	public string RawPath()
	{
		return DynamicMeshFile.MeshLocation + string.Format("{0}.raw", WorldChunkCache.MakeChunkKey(World.toChunkXZ(this.X), World.toChunkXZ(this.Z)));
	}

	// Token: 0x060018EF RID: 6383 RVA: 0x00098E8A File Offset: 0x0009708A
	public bool Exists()
	{
		return SdFile.Exists(this.Path());
	}

	// Token: 0x060018F0 RID: 6384 RVA: 0x00098E98 File Offset: 0x00097098
	public bool GetLock(string debug)
	{
		int num = 0;
		while (!this.TryTakeLock(debug))
		{
			if (DynamicMeshThread.RequestThreadStop)
			{
				Log.Out(this.ToDebugLocation() + " World is unloading so lock attempt failed " + debug);
				return false;
			}
			if (++num % 10 == 0)
			{
				Log.Out(this.ToDebugLocation() + " Waiting for lock to release: " + this.lastLock);
			}
			if (num > 600 && Monitor.IsEntered(this._lock))
			{
				Log.Warning(string.Concat(new string[]
				{
					"Forcing lock release to ",
					debug,
					" from ",
					this.lastLock,
					" after 60 seconds"
				}));
				this.ReleaseLock();
			}
			Thread.Sleep(100);
		}
		return true;
	}

	// Token: 0x060018F1 RID: 6385 RVA: 0x00098F55 File Offset: 0x00097155
	public bool ReleaseLock()
	{
		return this.TryExit("releaseLock");
	}

	// Token: 0x060018F2 RID: 6386 RVA: 0x00098F64 File Offset: 0x00097164
	public bool TryTakeLock(string debug)
	{
		bool flag = false;
		if (Monitor.IsEntered(this._lock))
		{
			if (DynamicMeshManager.DoLog)
			{
				Log.Warning(this.ToDebugLocation() + " Lock kept by " + debug);
			}
			this.lastLock = debug;
			return true;
		}
		Monitor.TryEnter(this._lock, ref flag);
		if (flag)
		{
			if (DynamicMeshManager.DoLog)
			{
				Log.Warning(this.ToDebugLocation() + " Lock taken by " + debug);
			}
			this.lastLock = debug;
		}
		else if (DynamicMeshManager.DoLog)
		{
			Log.Warning(string.Concat(new string[]
			{
				this.ToDebugLocation(),
				" Lock failed on ",
				debug,
				" : ",
				this.lastLock
			}));
		}
		return flag;
	}

	// Token: 0x060018F3 RID: 6387 RVA: 0x0009901A File Offset: 0x0009721A
	public bool ThreadHasLock()
	{
		return Monitor.IsEntered(this._lock);
	}

	// Token: 0x060018F4 RID: 6388 RVA: 0x00099028 File Offset: 0x00097228
	public bool TryExit(string debug)
	{
		if (!Monitor.IsEntered(this._lock))
		{
			Log.Warning(this.ToDebugLocation() + " Tried to release lock when not owner " + debug);
			return false;
		}
		while (Monitor.IsEntered(this._lock))
		{
			Monitor.Exit(this._lock);
		}
		if (DynamicMeshManager.DoLog)
		{
			Log.Out(this.ToDebugLocation() + " Lock released " + debug);
		}
		return true;
	}

	// Token: 0x060018F5 RID: 6389 RVA: 0x00099090 File Offset: 0x00097290
	public string ToDebugLocation()
	{
		return string.Format("{0},{1}", this.X, this.Z);
	}

	// Token: 0x060018F6 RID: 6390 RVA: 0x000990B2 File Offset: 0x000972B2
	public void ClearUnloadMarks()
	{
		this.StateInfo &= ~DynamicMeshStates.UnloadMark1;
		this.StateInfo &= ~DynamicMeshStates.UnloadMark2;
		this.StateInfo &= ~DynamicMeshStates.UnloadMark3;
	}

	// Token: 0x060018F7 RID: 6391 RVA: 0x000990E1 File Offset: 0x000972E1
	public static DynamicMeshRegionDataWrapper Create(long key)
	{
		return new DynamicMeshRegionDataWrapper
		{
			X = DynamicMeshUnity.GetWorldXFromKey(key),
			Z = DynamicMeshUnity.GetWorldZFromKey(key),
			Key = key
		};
	}

	// Token: 0x04000FF1 RID: 4081
	[PublicizedFrom(EAccessModifier.Private)]
	public object _lock = new object();

	// Token: 0x04000FF2 RID: 4082
	public string lastLock;

	// Token: 0x04000FF3 RID: 4083
	public DynamicMeshStates StateInfo;

	// Token: 0x04000FF4 RID: 4084
	public int X;

	// Token: 0x04000FF5 RID: 4085
	public int Z;

	// Token: 0x04000FF6 RID: 4086
	public long Key;
}
