using System;
using System.Diagnostics;
using System.Threading;

// Token: 0x02000318 RID: 792
[DebuggerDisplay("{X},{Z} {StateInfo}")]
public class DynamicMeshChunkDataWrapper
{
	// Token: 0x060016C5 RID: 5829 RVA: 0x000846B0 File Offset: 0x000828B0
	public void Reset()
	{
		this.StateInfo = DynamicMeshStates.None;
	}

	// Token: 0x060016C6 RID: 5830 RVA: 0x000846BC File Offset: 0x000828BC
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

	// Token: 0x060016C7 RID: 5831 RVA: 0x00084772 File Offset: 0x00082972
	public string Path()
	{
		return DynamicMeshFile.MeshLocation + string.Format("{0}.update", WorldChunkCache.MakeChunkKey(World.toChunkXZ(this.X), World.toChunkXZ(this.Z)));
	}

	// Token: 0x060016C8 RID: 5832 RVA: 0x000847A8 File Offset: 0x000829A8
	public string RawPath()
	{
		return DynamicMeshFile.MeshLocation + string.Format("{0}.raw", WorldChunkCache.MakeChunkKey(World.toChunkXZ(this.X), World.toChunkXZ(this.Z)));
	}

	// Token: 0x060016C9 RID: 5833 RVA: 0x000847DE File Offset: 0x000829DE
	public bool Exists()
	{
		return SdFile.Exists(this.Path());
	}

	// Token: 0x060016CA RID: 5834 RVA: 0x000847EC File Offset: 0x000829EC
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
			if (num == 0 || ++num % 10 == 0)
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

	// Token: 0x060016CB RID: 5835 RVA: 0x000848AC File Offset: 0x00082AAC
	public bool ReleaseLock()
	{
		return this.TryExit("releaseLock");
	}

	// Token: 0x060016CC RID: 5836 RVA: 0x000848BC File Offset: 0x00082ABC
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

	// Token: 0x060016CD RID: 5837 RVA: 0x00084972 File Offset: 0x00082B72
	public bool ThreadHasLock()
	{
		return Monitor.IsEntered(this._lock);
	}

	// Token: 0x060016CE RID: 5838 RVA: 0x0008497F File Offset: 0x00082B7F
	public bool TryGetData(out DynamicMeshChunkData data, string debug)
	{
		if (this.TryTakeLock(debug))
		{
			data = this.Data;
			return true;
		}
		data = null;
		return false;
	}

	// Token: 0x060016CF RID: 5839 RVA: 0x00084998 File Offset: 0x00082B98
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

	// Token: 0x060016D0 RID: 5840 RVA: 0x00084A00 File Offset: 0x00082C00
	public bool IsAvailableToLoad()
	{
		return this.Data != null && !this.StateInfo.HasFlag(DynamicMeshStates.MarkedForDelete) && !this.StateInfo.HasFlag(DynamicMeshStates.ThreadUpdating) && !this.StateInfo.HasFlag(DynamicMeshStates.LoadRequired);
	}

	// Token: 0x060016D1 RID: 5841 RVA: 0x00084A61 File Offset: 0x00082C61
	public string ToDebugLocation()
	{
		return string.Format("{0},{1}", this.X, this.Z);
	}

	// Token: 0x060016D2 RID: 5842 RVA: 0x00084A83 File Offset: 0x00082C83
	public void ClearUnloadMarks()
	{
		this.StateInfo &= ~DynamicMeshStates.UnloadMark1;
		this.StateInfo &= ~DynamicMeshStates.UnloadMark2;
		this.StateInfo &= ~DynamicMeshStates.UnloadMark3;
	}

	// Token: 0x060016D3 RID: 5843 RVA: 0x00084AB2 File Offset: 0x00082CB2
	public static DynamicMeshChunkDataWrapper Create(long key)
	{
		return new DynamicMeshChunkDataWrapper
		{
			X = DynamicMeshUnity.GetWorldXFromKey(key),
			Z = DynamicMeshUnity.GetWorldZFromKey(key),
			Key = key
		};
	}

	// Token: 0x04000E46 RID: 3654
	public DynamicMeshChunkData Data;

	// Token: 0x04000E47 RID: 3655
	[PublicizedFrom(EAccessModifier.Private)]
	public object _lock = new object();

	// Token: 0x04000E48 RID: 3656
	public string lastLock;

	// Token: 0x04000E49 RID: 3657
	public DynamicMeshStates StateInfo;

	// Token: 0x04000E4A RID: 3658
	public int X;

	// Token: 0x04000E4B RID: 3659
	public int Z;

	// Token: 0x04000E4C RID: 3660
	public long Key;
}
