using System;
using System.Collections;
using UnityEngine;

// Token: 0x02001276 RID: 4726
public static class ProfilerGameUtils
{
	// Token: 0x060093FB RID: 37883 RVA: 0x003B12E0 File Offset: 0x003AF4E0
	public static bool TryGetFlyingPlayer(out EntityPlayerLocal player)
	{
		World world = GameManager.Instance.World;
		if (world == null)
		{
			player = null;
			return false;
		}
		player = world.GetPrimaryPlayer();
		if (player == null)
		{
			player = null;
			return false;
		}
		if (player.AttachedToEntity)
		{
			player.Detach();
		}
		player.IsGodMode.Value = true;
		player.IsNoCollisionMode.Value = true;
		player.IsFlyMode.Value = true;
		player.Buffs.AddBuff("god", -1, true, false, -1f);
		return true;
	}

	// Token: 0x060093FC RID: 37884 RVA: 0x003B136E File Offset: 0x003AF56E
	public static IEnumerator WaitForSingleChunkToLoad(int chunkX, int chunkZ, ChunkConditions.Delegate chunkCondition)
	{
		ChunkCluster cc = GameManager.Instance.World.ChunkClusters[0];
		Chunk chunk = null;
		while (chunk == null)
		{
			chunk = cc.GetChunkSync(chunkX, chunkZ);
			yield return null;
		}
		ProfilerGameUtils.WaitForSingleChunkToLoad(chunk, chunkCondition);
		yield break;
	}

	// Token: 0x060093FD RID: 37885 RVA: 0x003B138B File Offset: 0x003AF58B
	public static IEnumerator WaitForSingleChunkToLoad(Chunk chunk, ChunkConditions.Delegate chunkCondition)
	{
		if (chunk == null)
		{
			yield break;
		}
		float startTime = Time.realtimeSinceStartup;
		while (!chunkCondition(chunk))
		{
			if (Time.realtimeSinceStartup - startTime > 30f)
			{
				yield break;
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x060093FE RID: 37886 RVA: 0x003B13A1 File Offset: 0x003AF5A1
	public static IEnumerator WaitForChunksAroundObserverToLoad(ChunkManager.ChunkObserver observer, ChunkConditions.Delegate chunkCondition)
	{
		ChunkCluster cc = GameManager.Instance.World.ChunkClusters[0];
		bool isLoaded = false;
		float startTime = Time.realtimeSinceStartup;
		while (!isLoaded)
		{
			isLoaded = true;
			for (int i = 0; i <= observer.viewDim - 2; i++)
			{
				foreach (long key in observer.chunksAround.buckets.array[i])
				{
					Chunk chunkSync = cc.GetChunkSync(key);
					if (chunkSync == null)
					{
						isLoaded = false;
						break;
					}
					if (!chunkCondition(chunkSync))
					{
						isLoaded = false;
						break;
					}
				}
			}
			if (Time.realtimeSinceStartup - startTime > 30f)
			{
				isLoaded = true;
				Debug.LogErrorFormat("Could not load Chunks at player chunk pos ({0},{1}). Forcing continue", new object[]
				{
					observer.curChunkPos.x,
					observer.curChunkPos.z
				});
			}
			yield return false;
		}
		yield break;
	}
}
