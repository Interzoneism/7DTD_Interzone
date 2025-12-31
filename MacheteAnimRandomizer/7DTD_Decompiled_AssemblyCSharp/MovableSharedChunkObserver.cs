using System;
using UnityEngine;

// Token: 0x02000A3A RID: 2618
public class MovableSharedChunkObserver : IDisposable
{
	// Token: 0x0600500E RID: 20494 RVA: 0x001FC6F8 File Offset: 0x001FA8F8
	public MovableSharedChunkObserver(SharedChunkObserverCache _observerCache)
	{
		this.cache = _observerCache;
	}

	// Token: 0x0600500F RID: 20495 RVA: 0x001FC708 File Offset: 0x001FA908
	[PublicizedFrom(EAccessModifier.Protected)]
	public ~MovableSharedChunkObserver()
	{
		this.Dispose();
	}

	// Token: 0x06005010 RID: 20496 RVA: 0x001FC734 File Offset: 0x001FA934
	public void SetPosition(Vector3 newPosition)
	{
		Vector2i vector2i = new Vector2i(World.toChunkXZ(Utils.Fastfloor(newPosition.x)), World.toChunkXZ(Utils.Fastfloor(newPosition.z)));
		if (this.observer == null || this.observer.ChunkPos != vector2i)
		{
			if (this.observer != null)
			{
				this.observer.Dispose();
			}
			this.observer = this.cache.GetSharedObserverForChunk(vector2i);
		}
	}

	// Token: 0x06005011 RID: 20497 RVA: 0x001FC7A8 File Offset: 0x001FA9A8
	public void Dispose()
	{
		if (this.observer != null)
		{
			this.observer.Dispose();
			this.observer = null;
		}
	}

	// Token: 0x04003D3B RID: 15675
	[PublicizedFrom(EAccessModifier.Private)]
	public ISharedChunkObserver observer;

	// Token: 0x04003D3C RID: 15676
	[PublicizedFrom(EAccessModifier.Private)]
	public SharedChunkObserverCache cache;
}
