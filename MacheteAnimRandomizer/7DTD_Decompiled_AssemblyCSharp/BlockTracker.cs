using System;
using System.Collections.Generic;

// Token: 0x0200010B RID: 267
public class BlockTracker
{
	// Token: 0x06000727 RID: 1831 RVA: 0x000325C5 File Offset: 0x000307C5
	public BlockTracker(int _limit)
	{
		this.limit = _limit;
		this.blockLocations = new List<Vector3i>();
	}

	// Token: 0x06000728 RID: 1832 RVA: 0x000325DF File Offset: 0x000307DF
	public bool TryAddBlock(Vector3i _position)
	{
		if (this.blockLocations.Contains(_position))
		{
			return true;
		}
		if (this.blockLocations.Count >= this.limit)
		{
			return false;
		}
		this.blockLocations.Add(_position);
		return true;
	}

	// Token: 0x06000729 RID: 1833 RVA: 0x00032613 File Offset: 0x00030813
	public bool RemoveBlock(Vector3i _position)
	{
		if (this.blockLocations.Contains(_position))
		{
			this.blockLocations.Remove(_position);
			return true;
		}
		return false;
	}

	// Token: 0x0600072A RID: 1834 RVA: 0x00032633 File Offset: 0x00030833
	public bool CanAdd(Vector3i _position)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return this.blockLocations.Count < this.limit || this.blockLocations.Contains(_position);
		}
		return this.clientAmount < this.limit;
	}

	// Token: 0x0600072B RID: 1835 RVA: 0x00032671 File Offset: 0x00030871
	public void Clear()
	{
		this.blockLocations.Clear();
	}

	// Token: 0x0600072C RID: 1836 RVA: 0x00032680 File Offset: 0x00030880
	public void Read(PooledBinaryReader _reader)
	{
		this.blockLocations = new List<Vector3i>();
		int num = _reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			this.blockLocations.Add(new Vector3i(_reader.ReadInt32(), _reader.ReadInt32(), _reader.ReadInt32()));
		}
	}

	// Token: 0x0600072D RID: 1837 RVA: 0x000326D0 File Offset: 0x000308D0
	public void Write(PooledBinaryWriter _writer)
	{
		_writer.Write(this.blockLocations.Count);
		foreach (Vector3i vector3i in this.blockLocations)
		{
			_writer.Write(vector3i.x);
			_writer.Write(vector3i.y);
			_writer.Write(vector3i.z);
		}
	}

	// Token: 0x040007EA RID: 2026
	public int limit;

	// Token: 0x040007EB RID: 2027
	public List<Vector3i> blockLocations;

	// Token: 0x040007EC RID: 2028
	public int clientAmount;
}
