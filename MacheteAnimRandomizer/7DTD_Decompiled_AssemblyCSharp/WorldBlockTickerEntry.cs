using System;
using System.IO;

// Token: 0x02000A90 RID: 2704
public class WorldBlockTickerEntry
{
	// Token: 0x06005374 RID: 21364 RVA: 0x00217A24 File Offset: 0x00215C24
	public WorldBlockTickerEntry(int _clrIdx, Vector3i _pos, int _id, ulong _scheduledTime)
	{
		this.clrIdx = _clrIdx;
		long num = WorldBlockTickerEntry.nextTickEntryID;
		WorldBlockTickerEntry.nextTickEntryID = num + 1L;
		this.tickEntryID = num;
		this.worldPos = _pos;
		this.blockID = _id;
		this.scheduledTime = _scheduledTime;
	}

	// Token: 0x06005375 RID: 21365 RVA: 0x00217A60 File Offset: 0x00215C60
	public static WorldBlockTickerEntry Read(BinaryReader _br, int _chunkX, int _chunkZ, int _version)
	{
		Vector3i pos = new Vector3i((int)_br.ReadByte() + _chunkX * 16, (int)_br.ReadByte(), (int)_br.ReadByte() + _chunkZ * 16);
		int id = (int)_br.ReadUInt16();
		ulong num = _br.ReadUInt64();
		return new WorldBlockTickerEntry((int)_br.ReadUInt16(), pos, id, num);
	}

	// Token: 0x06005376 RID: 21366 RVA: 0x00217AAC File Offset: 0x00215CAC
	public void Write(BinaryWriter _bw)
	{
		_bw.Write((byte)World.toBlockXZ(this.worldPos.x));
		_bw.Write((byte)this.worldPos.y);
		_bw.Write((byte)World.toBlockXZ(this.worldPos.z));
		_bw.Write((ushort)this.blockID);
		_bw.Write(this.scheduledTime);
		_bw.Write((ushort)this.clrIdx);
	}

	// Token: 0x06005377 RID: 21367 RVA: 0x00217B20 File Offset: 0x00215D20
	public override bool Equals(object _obj)
	{
		WorldBlockTickerEntry worldBlockTickerEntry = _obj as WorldBlockTickerEntry;
		return worldBlockTickerEntry != null && (this.worldPos.Equals(worldBlockTickerEntry.worldPos) && this.blockID == worldBlockTickerEntry.blockID && this.clrIdx == worldBlockTickerEntry.clrIdx) && worldBlockTickerEntry.tickEntryID == this.tickEntryID;
	}

	// Token: 0x06005378 RID: 21368 RVA: 0x00217B7B File Offset: 0x00215D7B
	public override int GetHashCode()
	{
		return WorldBlockTickerEntry.ToHashCode(this.clrIdx, this.worldPos, this.blockID);
	}

	// Token: 0x06005379 RID: 21369 RVA: 0x00217B94 File Offset: 0x00215D94
	public static int ToHashCode(int _clrIdx, Vector3i _pos, int _blockID)
	{
		return (_pos.GetHashCode() * 397 ^ _blockID) * 397 ^ _clrIdx;
	}

	// Token: 0x0600537A RID: 21370 RVA: 0x00217BB3 File Offset: 0x00215DB3
	public long GetChunkKey()
	{
		return WorldChunkCache.MakeChunkKey(World.toChunkXZ(this.worldPos.x), World.toChunkXZ(this.worldPos.z), this.clrIdx);
	}

	// Token: 0x04003F9C RID: 16284
	public readonly Vector3i worldPos;

	// Token: 0x04003F9D RID: 16285
	public readonly int blockID;

	// Token: 0x04003F9E RID: 16286
	public readonly ulong scheduledTime;

	// Token: 0x04003F9F RID: 16287
	public readonly int clrIdx;

	// Token: 0x04003FA0 RID: 16288
	[PublicizedFrom(EAccessModifier.Private)]
	public static long nextTickEntryID;

	// Token: 0x04003FA1 RID: 16289
	public readonly long tickEntryID;
}
