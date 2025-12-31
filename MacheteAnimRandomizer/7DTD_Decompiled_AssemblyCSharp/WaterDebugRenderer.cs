using System;
using UnityEngine;

// Token: 0x02000B74 RID: 2932
public class WaterDebugRenderer : IMemoryPoolableObject
{
	// Token: 0x06005AFD RID: 23293 RVA: 0x00247894 File Offset: 0x00245A94
	public void SetChunkOrigin(Vector3 _origin)
	{
		this.chunkOrigin = _origin;
		for (int i = 0; i < this.numActiveLayers; i++)
		{
			int num = this.activeLayers[i];
			Vector3 layerOrigin = this.chunkOrigin + Vector3.up * (float)num * 16f;
			this.layers[num].SetLayerOrigin(layerOrigin);
		}
	}

	// Token: 0x06005AFE RID: 23294 RVA: 0x002478F4 File Offset: 0x00245AF4
	public void SetWater(int _x, int _y, int _z, float mass)
	{
		int layerIndex = _y / 16;
		int y = _y % 16;
		this.GetOrCreateLayer(layerIndex).SetWater(_x, y, _z, mass);
	}

	// Token: 0x06005AFF RID: 23295 RVA: 0x0024791C File Offset: 0x00245B1C
	public void LoadFromChunk(Chunk chunk)
	{
		this.SetChunkOrigin(chunk.GetWorldPos());
		for (int i = 0; i < 16; i++)
		{
			for (int j = 0; j < 16; j++)
			{
				for (int k = 0; k < 256; k++)
				{
					float num = (float)chunk.GetWater(i, k, j).GetMass();
					if (num > 195f)
					{
						this.SetWater(i, k, j, num);
					}
				}
			}
		}
	}

	// Token: 0x06005B00 RID: 23296 RVA: 0x0024798C File Offset: 0x00245B8C
	[PublicizedFrom(EAccessModifier.Private)]
	public WaterDebugRendererLayer GetOrCreateLayer(int layerIndex)
	{
		WaterDebugRendererLayer waterDebugRendererLayer = this.layers[layerIndex];
		if (waterDebugRendererLayer == null)
		{
			waterDebugRendererLayer = WaterDebugPools.layerPool.AllocSync(false);
			Vector3 layerOrigin = this.chunkOrigin + Vector3.up * (float)layerIndex * 16f;
			waterDebugRendererLayer.SetLayerOrigin(layerOrigin);
			this.layers[layerIndex] = waterDebugRendererLayer;
			this.activeLayers[this.numActiveLayers] = layerIndex;
			this.numActiveLayers++;
			Array.Sort<int>(this.activeLayers, 0, this.numActiveLayers);
		}
		return waterDebugRendererLayer;
	}

	// Token: 0x06005B01 RID: 23297 RVA: 0x00247A14 File Offset: 0x00245C14
	public void Draw()
	{
		for (int i = 0; i < this.numActiveLayers; i++)
		{
			int num = this.activeLayers[i];
			this.layers[num].Draw();
		}
	}

	// Token: 0x06005B02 RID: 23298 RVA: 0x00247A48 File Offset: 0x00245C48
	public void Clear()
	{
		for (int i = 0; i < this.numActiveLayers; i++)
		{
			int num = this.activeLayers[i];
			WaterDebugRendererLayer t = this.layers[num];
			WaterDebugPools.layerPool.FreeSync(t);
			this.layers[num] = null;
			this.activeLayers[i] = 0;
		}
		this.numActiveLayers = 0;
	}

	// Token: 0x06005B03 RID: 23299 RVA: 0x00247A9C File Offset: 0x00245C9C
	public void Cleanup()
	{
		this.Clear();
	}

	// Token: 0x06005B04 RID: 23300 RVA: 0x00247A9C File Offset: 0x00245C9C
	public void Reset()
	{
		this.Clear();
	}

	// Token: 0x04004598 RID: 17816
	[PublicizedFrom(EAccessModifier.Private)]
	public const int numLayers = 16;

	// Token: 0x04004599 RID: 17817
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 chunkOrigin = Vector3.zero;

	// Token: 0x0400459A RID: 17818
	[PublicizedFrom(EAccessModifier.Private)]
	public WaterDebugRendererLayer[] layers = new WaterDebugRendererLayer[16];

	// Token: 0x0400459B RID: 17819
	[PublicizedFrom(EAccessModifier.Private)]
	public int[] activeLayers = new int[16];

	// Token: 0x0400459C RID: 17820
	[PublicizedFrom(EAccessModifier.Private)]
	public int numActiveLayers;
}
