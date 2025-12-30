using System;
using System.Diagnostics;
using System.Threading;

// Token: 0x02000322 RID: 802
[DebuggerDisplay("{X},{Z} {StateInfo}")]
public class DynamicMeshData
{
	// Token: 0x0600172E RID: 5934 RVA: 0x00089820 File Offset: 0x00087A20
	public string GetByteLengthString()
	{
		if (this.Bytes == null)
		{
			return "null";
		}
		return this.Bytes.Length.ToString();
	}

	// Token: 0x0600172F RID: 5935 RVA: 0x0008984B File Offset: 0x00087A4B
	public string Path(bool isRegionQueue)
	{
		return DynamicMeshFile.MeshLocation + string.Format("{0},{1}.{2}", this.X, this.Z, isRegionQueue ? "region" : "mesh");
	}

	// Token: 0x06001730 RID: 5936 RVA: 0x00089886 File Offset: 0x00087A86
	public bool Exists(bool isRegionQueue)
	{
		return SdFile.Exists(DynamicMeshFile.MeshLocation + string.Format("{0},{1}.{2}", this.X, this.Z, isRegionQueue ? "region" : "mesh"));
	}

	// Token: 0x06001731 RID: 5937 RVA: 0x000898C8 File Offset: 0x00087AC8
	public bool GetLock(string debug)
	{
		DateTime now = DateTime.Now;
		bool flag = false;
		while (!this.TryTakeLock(debug))
		{
			if (DynamicMeshThread.RequestThreadStop)
			{
				Log.Out(this.ToDebugLocation() + " World is unloading so lock attempt failed " + debug);
				return false;
			}
			double totalSeconds = (DateTime.Now - now).TotalSeconds;
			if (!flag && totalSeconds > 5.0)
			{
				flag = true;
				if (DynamicMeshManager.DoLog)
				{
					Log.Out(this.ToDebugLocation() + " Waiting for lock to release: " + this.lastLock);
				}
			}
			if (totalSeconds > 60.0 && Monitor.IsEntered(this._lock))
			{
				Log.Warning(string.Concat(new string[]
				{
					"Forcing lock release to ",
					debug,
					" from ",
					this.lastLock,
					" after ",
					totalSeconds.ToString(),
					" seconds"
				}));
				this.ReleaseLock();
			}
		}
		return true;
	}

	// Token: 0x06001732 RID: 5938 RVA: 0x000899BC File Offset: 0x00087BBC
	public bool ReleaseLock()
	{
		return this.TryExit("releaseLock");
	}

	// Token: 0x06001733 RID: 5939 RVA: 0x000899CC File Offset: 0x00087BCC
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

	// Token: 0x06001734 RID: 5940 RVA: 0x00089A82 File Offset: 0x00087C82
	public bool ThreadHasLock()
	{
		return Monitor.IsEntered(this._lock);
	}

	// Token: 0x06001735 RID: 5941 RVA: 0x00089A8F File Offset: 0x00087C8F
	public bool TryGetBytes(out byte[] bytes, string debug)
	{
		if (this.TryTakeLock(debug))
		{
			bytes = this.Bytes;
			return true;
		}
		bytes = null;
		return false;
	}

	// Token: 0x06001736 RID: 5942 RVA: 0x00089AA8 File Offset: 0x00087CA8
	public bool TryExit(string debug)
	{
		if (!Monitor.IsEntered(this._lock))
		{
			if (DynamicMeshManager.DoLog)
			{
				Log.Warning(this.ToDebugLocation() + " Tried to release lock when not owner " + debug);
			}
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

	// Token: 0x06001737 RID: 5943 RVA: 0x00089B18 File Offset: 0x00087D18
	public bool IsAvailableToLoad()
	{
		return this.Bytes != null && !this.StateInfo.HasFlag(DynamicMeshStates.MarkedForDelete) && !this.StateInfo.HasFlag(DynamicMeshStates.ThreadUpdating) && !this.StateInfo.HasFlag(DynamicMeshStates.LoadRequired);
	}

	// Token: 0x06001738 RID: 5944 RVA: 0x00089B79 File Offset: 0x00087D79
	public string ToDebugLocation()
	{
		return string.Format("{0}:{1},{2}", this.IsRegion ? "R" : "C", this.X, this.Z);
	}

	// Token: 0x06001739 RID: 5945 RVA: 0x00089BAF File Offset: 0x00087DAF
	public void ClearUnloadMarks()
	{
		this.StateInfo &= ~DynamicMeshStates.UnloadMark1;
		this.StateInfo &= ~DynamicMeshStates.UnloadMark2;
		this.StateInfo &= ~DynamicMeshStates.UnloadMark3;
	}

	// Token: 0x0600173A RID: 5946 RVA: 0x00089BDE File Offset: 0x00087DDE
	public static DynamicMeshData Create(int x, int z, bool isRegion)
	{
		return new DynamicMeshData
		{
			X = x,
			Z = z,
			IsRegion = isRegion
		};
	}

	// Token: 0x04000E7A RID: 3706
	public byte[] Bytes;

	// Token: 0x04000E7B RID: 3707
	[PublicizedFrom(EAccessModifier.Private)]
	public object _lock = new object();

	// Token: 0x04000E7C RID: 3708
	public string lastLock;

	// Token: 0x04000E7D RID: 3709
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsRegion;

	// Token: 0x04000E7E RID: 3710
	public DynamicMeshStates StateInfo;

	// Token: 0x04000E7F RID: 3711
	public int X;

	// Token: 0x04000E80 RID: 3712
	public int Z;

	// Token: 0x04000E81 RID: 3713
	public int StreamLength;
}
