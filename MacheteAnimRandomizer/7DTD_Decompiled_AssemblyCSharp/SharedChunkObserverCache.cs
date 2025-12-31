using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000A3B RID: 2619
public class SharedChunkObserverCache
{
	// Token: 0x17000827 RID: 2087
	// (get) Token: 0x06005012 RID: 20498 RVA: 0x001FC7C4 File Offset: 0x001FA9C4
	public IThreadingSemantics ThreadingSemantics
	{
		get
		{
			return this.threadingSemantics;
		}
	}

	// Token: 0x06005013 RID: 20499 RVA: 0x001FC7CC File Offset: 0x001FA9CC
	public SharedChunkObserverCache(ChunkManager _chunkManager, int _viewDim, IThreadingSemantics _threadingSemantics)
	{
		this.chunkManager = _chunkManager;
		this.viewDim = _viewDim;
		this.threadingSemantics = _threadingSemantics;
	}

	// Token: 0x06005014 RID: 20500 RVA: 0x001FC7F4 File Offset: 0x001FA9F4
	public ISharedChunkObserver GetSharedObserverForChunk(Vector2i chunkPos)
	{
		return this.threadingSemantics.Synchronize<SharedChunkObserverCache.SharedChunkObserver>(delegate()
		{
			SharedChunkObserverCache.SharedChunkObserver sharedChunkObserver;
			if (this.observers.TryGetValue(chunkPos, out sharedChunkObserver) && sharedChunkObserver.refCount < 1)
			{
				sharedChunkObserver = null;
			}
			if (sharedChunkObserver != null)
			{
				sharedChunkObserver.Reference();
			}
			else
			{
				sharedChunkObserver = new SharedChunkObserverCache.SharedChunkObserver(this, this.chunkManager.AddChunkObserver(new Vector3((float)(chunkPos.x << 4), 0f, (float)(chunkPos.y << 4)), false, this.viewDim, -1), new SharedChunkObserverCache.SharedChunkObserver.RemoveObserver(this.removeChunkObserver), chunkPos);
				this.observers[chunkPos] = sharedChunkObserver;
			}
			return sharedChunkObserver;
		});
	}

	// Token: 0x06005015 RID: 20501 RVA: 0x001FC82C File Offset: 0x001FAA2C
	[PublicizedFrom(EAccessModifier.Private)]
	public void removeChunkObserver(SharedChunkObserverCache.SharedChunkObserver observer)
	{
		this.threadingSemantics.Synchronize(delegate()
		{
			if (this.observers[observer.chunkPos] == observer)
			{
				this.observers.Remove(observer.chunkPos);
			}
		});
		this.chunkManager.RemoveChunkObserver(observer.chunkRef);
	}

	// Token: 0x04003D3D RID: 15677
	[PublicizedFrom(EAccessModifier.Private)]
	public IThreadingSemantics threadingSemantics;

	// Token: 0x04003D3E RID: 15678
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<Vector2i, SharedChunkObserverCache.SharedChunkObserver> observers = new Dictionary<Vector2i, SharedChunkObserverCache.SharedChunkObserver>();

	// Token: 0x04003D3F RID: 15679
	[PublicizedFrom(EAccessModifier.Private)]
	public ChunkManager chunkManager;

	// Token: 0x04003D40 RID: 15680
	[PublicizedFrom(EAccessModifier.Private)]
	public int viewDim;

	// Token: 0x02000A3C RID: 2620
	[PublicizedFrom(EAccessModifier.Private)]
	public class SharedChunkObserver : ISharedChunkObserver, IDisposable
	{
		// Token: 0x06005016 RID: 20502 RVA: 0x001FC87A File Offset: 0x001FAA7A
		public SharedChunkObserver(SharedChunkObserverCache _cache, ChunkManager.ChunkObserver _chunkRef, SharedChunkObserverCache.SharedChunkObserver.RemoveObserver _removeObserverDelegate, Vector2i _chunkPos)
		{
			this.cache = _cache;
			this.chunkRef = _chunkRef;
			this.removeObserver = _removeObserverDelegate;
			this.chunkPos = _chunkPos;
			this.refCount = 1;
		}

		// Token: 0x06005017 RID: 20503 RVA: 0x001FC8A6 File Offset: 0x001FAAA6
		public void Reference()
		{
			if (this.cache.ThreadingSemantics.InterlockedAdd(ref this.refCount, 1) < 2)
			{
				throw new Exception("Synchronization error: shared chunk observer was already disposed with a ref count of zero!");
			}
		}

		// Token: 0x06005018 RID: 20504 RVA: 0x001FC8CD File Offset: 0x001FAACD
		public void Dispose()
		{
			if (this.cache.ThreadingSemantics.InterlockedAdd(ref this.refCount, -1) == 0)
			{
				this.removeObserver(this);
			}
		}

		// Token: 0x17000828 RID: 2088
		// (get) Token: 0x06005019 RID: 20505 RVA: 0x001FC8F4 File Offset: 0x001FAAF4
		public Vector2i ChunkPos
		{
			get
			{
				return this.chunkPos;
			}
		}

		// Token: 0x17000829 RID: 2089
		// (get) Token: 0x0600501A RID: 20506 RVA: 0x001FC8FC File Offset: 0x001FAAFC
		public SharedChunkObserverCache Owner
		{
			get
			{
				return this.cache;
			}
		}

		// Token: 0x04003D41 RID: 15681
		public Vector2i chunkPos;

		// Token: 0x04003D42 RID: 15682
		public int refCount;

		// Token: 0x04003D43 RID: 15683
		public ChunkManager.ChunkObserver chunkRef;

		// Token: 0x04003D44 RID: 15684
		[PublicizedFrom(EAccessModifier.Private)]
		public SharedChunkObserverCache cache;

		// Token: 0x04003D45 RID: 15685
		[PublicizedFrom(EAccessModifier.Private)]
		public SharedChunkObserverCache.SharedChunkObserver.RemoveObserver removeObserver;

		// Token: 0x02000A3D RID: 2621
		// (Invoke) Token: 0x0600501C RID: 20508
		public delegate void RemoveObserver(SharedChunkObserverCache.SharedChunkObserver observer);
	}
}
