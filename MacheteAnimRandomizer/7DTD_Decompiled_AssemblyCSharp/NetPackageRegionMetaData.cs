using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000345 RID: 837
[Preserve]
public class NetPackageRegionMetaData : DynamicMeshServerData
{
	// Token: 0x0600187C RID: 6268 RVA: 0x00096377 File Offset: 0x00094577
	public NetPackageRegionMetaData()
	{
	}

	// Token: 0x0600187D RID: 6269 RVA: 0x0009638A File Offset: 0x0009458A
	public NetPackageRegionMetaData(DynamicMeshRegion region)
	{
		this.X = region.WorldPosition.x;
		this.Z = region.WorldPosition.z;
	}

	// Token: 0x0600187E RID: 6270 RVA: 0x000963C0 File Offset: 0x000945C0
	public override bool Prechecks()
	{
		if (DynamicMeshManager.DoLog)
		{
			DynamicMeshManager.LogMsg(string.Concat(new string[]
			{
				"Sending region data: ",
				this.X.ToString(),
				",",
				this.Z.ToString(),
				"  Items: ",
				this.ChunksWithData.Count.ToString(),
				"   length: ",
				this.GetLength().ToString()
			}));
		}
		return true;
	}

	// Token: 0x0600187F RID: 6271 RVA: 0x00096448 File Offset: 0x00094648
	public override int GetLength()
	{
		return 12 + this.ChunksWithData.Count * 8;
	}

	// Token: 0x06001880 RID: 6272 RVA: 0x0009645C File Offset: 0x0009465C
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (!DynamicMeshManager.CONTENT_ENABLED)
		{
			return;
		}
		if (DynamicMeshManager.Instance == null)
		{
			return;
		}
		DynamicMeshRegion region = DynamicMeshManager.Instance.GetRegion(this.X, this.Z);
		if (DynamicMeshManager.DoLog)
		{
			string str = "Recieved Region meta data ";
			Vector3i worldPosition = region.WorldPosition;
			DynamicMeshManager.LogMsg(str + worldPosition.ToString() + " items: " + this.ChunksWithData.Count.ToString());
		}
		foreach (Vector2i vector2i in this.ChunksWithData)
		{
			DynamicMeshManager.Instance.AddChunk(DynamicMeshUnity.GetItemKey(vector2i.x, vector2i.y), false, false, null);
		}
	}

	// Token: 0x06001881 RID: 6273 RVA: 0x0009653C File Offset: 0x0009473C
	public override void read(PooledBinaryReader reader)
	{
		this.X = reader.ReadInt32();
		this.Z = reader.ReadInt32();
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			this.ChunksWithData.Add(new Vector2i(reader.ReadInt32(), reader.ReadInt32()));
		}
	}

	// Token: 0x170002BB RID: 699
	// (get) Token: 0x06001882 RID: 6274 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool FlushQueue
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001883 RID: 6275 RVA: 0x00096590 File Offset: 0x00094790
	public override void write(PooledBinaryWriter writer)
	{
		base.write(writer);
		writer.Write(this.X);
		writer.Write(this.Z);
		writer.Write(this.ChunksWithData.Count);
		for (int i = 0; i < this.ChunksWithData.Count; i++)
		{
			writer.Write(this.ChunksWithData[i].x);
			writer.Write(this.ChunksWithData[i].y);
		}
	}

	// Token: 0x04000FA7 RID: 4007
	public List<Vector2i> ChunksWithData = new List<Vector2i>();
}
