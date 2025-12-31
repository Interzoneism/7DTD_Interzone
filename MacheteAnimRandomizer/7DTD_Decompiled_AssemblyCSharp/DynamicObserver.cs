using System;
using UnityEngine;

// Token: 0x02000341 RID: 833
public class DynamicObserver
{
	// Token: 0x06001874 RID: 6260 RVA: 0x00096158 File Offset: 0x00094358
	public void Start(Vector3 pos)
	{
		this.Position = pos;
		if (GameManager.Instance.World == null)
		{
			return;
		}
		if (this.Observer == null)
		{
			this.Observer = GameManager.Instance.AddChunkObserver(pos, false, DynamicObserver.ViewSize, GameManager.Instance.World.GetPrimaryPlayerId());
		}
		this.Observer.SetPosition(this.Position);
		this.StopTime = float.MaxValue;
	}

	// Token: 0x06001875 RID: 6261 RVA: 0x000961C4 File Offset: 0x000943C4
	public bool ContainsPoint(Vector3i pos)
	{
		int num = DynamicObserver.ViewSize * 16;
		return (float)pos.x >= this.Position.x - (float)num && (float)pos.x <= this.Position.x + (float)num && (float)pos.z >= this.Position.z - (float)num && (float)pos.z <= this.Position.z + (float)num;
	}

	// Token: 0x06001876 RID: 6262 RVA: 0x0009623C File Offset: 0x0009443C
	public bool HasFallingBlocks()
	{
		foreach (long key in this.Observer.chunksLoaded)
		{
			if (GameManager.Instance == null)
			{
				return false;
			}
			if (GameManager.Instance.World == null)
			{
				return false;
			}
			Chunk chunk = (Chunk)GameManager.Instance.World.GetChunkSync(key);
			if (chunk == null)
			{
				DynamicMeshManager.LogMsg("Observer couldn't load chunk so assuming falling");
				return true;
			}
			if (chunk.HasFallingBlocks())
			{
				DynamicMeshManager.Instance.AddUpdateData(chunk.Key, false, true, true, 3);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001877 RID: 6263 RVA: 0x000962FC File Offset: 0x000944FC
	public void Stop()
	{
		if (this.Observer == null)
		{
			return;
		}
		try
		{
			GameManager.Instance.RemoveChunkObserver(this.Observer);
		}
		catch (Exception ex)
		{
			if (DynamicMeshManager.DoLog)
			{
				DynamicMeshManager.LogMsg("Observer already destroyed: " + ex.Message);
			}
		}
		this.Observer = null;
	}

	// Token: 0x04000F97 RID: 3991
	public static int ViewSize = 3;

	// Token: 0x04000F98 RID: 3992
	public Vector3 Position;

	// Token: 0x04000F99 RID: 3993
	public ChunkManager.ChunkObserver Observer;

	// Token: 0x04000F9A RID: 3994
	public float StopTime = float.MaxValue;
}
