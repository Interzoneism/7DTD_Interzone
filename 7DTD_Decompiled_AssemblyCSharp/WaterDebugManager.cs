using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

// Token: 0x02000B75 RID: 2933
public class WaterDebugManager
{
	// Token: 0x1700094C RID: 2380
	// (get) Token: 0x06005B06 RID: 23302 RVA: 0x00247AD1 File Offset: 0x00245CD1
	// (set) Token: 0x06005B07 RID: 23303 RVA: 0x00247AD9 File Offset: 0x00245CD9
	public bool RenderingEnabled { get; set; } = true;

	// Token: 0x06005B08 RID: 23304 RVA: 0x00247AE4 File Offset: 0x00245CE4
	public void InitializeDebugRender(Chunk chunk)
	{
		WaterDebugRenderer waterDebugRenderer = WaterDebugPools.rendererPool.AllocSync(true);
		waterDebugRenderer.LoadFromChunk(chunk);
		chunk.AssignWaterDebugRenderer(new WaterDebugManager.RendererHandle(chunk, this));
		this.newRenderers.Enqueue(new WaterDebugManager.InitializedRenderer
		{
			chunkKey = chunk.Key,
			renderer = waterDebugRenderer
		});
	}

	// Token: 0x06005B09 RID: 23305 RVA: 0x00247B3A File Offset: 0x00245D3A
	[PublicizedFrom(EAccessModifier.Private)]
	public void ReturnRenderer(long key)
	{
		this.renderersToRemove.Enqueue(key);
	}

	// Token: 0x06005B0A RID: 23306 RVA: 0x00247B48 File Offset: 0x00245D48
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateRenderers()
	{
		WaterDebugManager.InitializedRenderer initializedRenderer;
		while (this.newRenderers.TryDequeue(out initializedRenderer))
		{
			WaterDebugRenderer t;
			if (this.activeRenderers.TryGetValue(initializedRenderer.chunkKey, out t))
			{
				WaterDebugPools.rendererPool.FreeSync(t);
				this.activeRenderers.Remove(initializedRenderer.chunkKey);
			}
			this.activeRenderers.Add(initializedRenderer.chunkKey, initializedRenderer.renderer);
		}
		long key;
		while (this.renderersToRemove.TryDequeue(out key))
		{
			WaterDebugRenderer t2;
			if (this.activeRenderers.TryGetValue(key, out t2))
			{
				WaterDebugPools.rendererPool.FreeSync(t2);
				this.activeRenderers.Remove(key);
			}
		}
	}

	// Token: 0x06005B0B RID: 23307 RVA: 0x00247BE8 File Offset: 0x00245DE8
	public void DebugDraw()
	{
		this.UpdateRenderers();
		if (this.RenderingEnabled)
		{
			foreach (WaterDebugRenderer waterDebugRenderer in this.activeRenderers.Values)
			{
				waterDebugRenderer.Draw();
			}
		}
	}

	// Token: 0x06005B0C RID: 23308 RVA: 0x00247C4C File Offset: 0x00245E4C
	public void Cleanup()
	{
		WaterDebugManager.InitializedRenderer initializedRenderer;
		while (this.newRenderers.TryDequeue(out initializedRenderer))
		{
			WaterDebugPools.rendererPool.FreeSync(initializedRenderer.renderer);
		}
		foreach (WaterDebugRenderer t in this.activeRenderers.Values)
		{
			WaterDebugPools.rendererPool.FreeSync(t);
		}
		this.activeRenderers.Clear();
	}

	// Token: 0x0400459E RID: 17822
	[PublicizedFrom(EAccessModifier.Private)]
	public ConcurrentQueue<WaterDebugManager.InitializedRenderer> newRenderers = new ConcurrentQueue<WaterDebugManager.InitializedRenderer>();

	// Token: 0x0400459F RID: 17823
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<long, WaterDebugRenderer> activeRenderers = new Dictionary<long, WaterDebugRenderer>();

	// Token: 0x040045A0 RID: 17824
	[PublicizedFrom(EAccessModifier.Private)]
	public ConcurrentQueue<long> renderersToRemove = new ConcurrentQueue<long>();

	// Token: 0x02000B76 RID: 2934
	[PublicizedFrom(EAccessModifier.Private)]
	public struct InitializedRenderer
	{
		// Token: 0x040045A1 RID: 17825
		public long chunkKey;

		// Token: 0x040045A2 RID: 17826
		public WaterDebugRenderer renderer;
	}

	// Token: 0x02000B77 RID: 2935
	public struct RendererHandle
	{
		// Token: 0x1700094D RID: 2381
		// (get) Token: 0x06005B0E RID: 23310 RVA: 0x00247D04 File Offset: 0x00245F04
		public bool IsValid
		{
			get
			{
				return this.manager != null && this.key != null;
			}
		}

		// Token: 0x06005B0F RID: 23311 RVA: 0x00247D1B File Offset: 0x00245F1B
		public RendererHandle(Chunk _chunk, WaterDebugManager _manager)
		{
			this.manager = _manager;
			this.key = new long?(_chunk.Key);
		}

		// Token: 0x06005B10 RID: 23312 RVA: 0x00247D38 File Offset: 0x00245F38
		[Conditional("UNITY_EDITOR")]
		public void SetChunkOrigin(Vector3i _origin)
		{
			if (!this.IsValid)
			{
				return;
			}
			WaterDebugRenderer waterDebugRenderer;
			if (this.manager.activeRenderers.TryGetValue(this.key.Value, out waterDebugRenderer))
			{
				waterDebugRenderer.SetChunkOrigin(_origin);
			}
		}

		// Token: 0x06005B11 RID: 23313 RVA: 0x00247D7C File Offset: 0x00245F7C
		[Conditional("UNITY_EDITOR")]
		public void SetWater(int _x, int _y, int _z, float mass)
		{
			if (!this.IsValid)
			{
				return;
			}
			WaterDebugRenderer waterDebugRenderer;
			if (this.manager.activeRenderers.TryGetValue(this.key.Value, out waterDebugRenderer))
			{
				waterDebugRenderer.SetWater(_x, _y, _z, mass);
			}
		}

		// Token: 0x06005B12 RID: 23314 RVA: 0x00247DBC File Offset: 0x00245FBC
		[Conditional("UNITY_EDITOR")]
		public void Reset()
		{
			if (this.IsValid)
			{
				this.manager.ReturnRenderer(this.key.Value);
			}
			this.manager = null;
			this.key = null;
		}

		// Token: 0x040045A3 RID: 17827
		[PublicizedFrom(EAccessModifier.Private)]
		public WaterDebugManager manager;

		// Token: 0x040045A4 RID: 17828
		[PublicizedFrom(EAccessModifier.Private)]
		public long? key;
	}
}
