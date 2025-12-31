using System;
using System.Collections.Generic;
using System.IO;

// Token: 0x020009EA RID: 2538
public class MapChunkDatabase : DatabaseWithFixedDS<int, ushort[]>, IMapChunkDatabase
{
	// Token: 0x06004DBC RID: 19900 RVA: 0x001EB00C File Offset: 0x001E920C
	public MapChunkDatabase(int _playerId) : base(7364973, 4, GamePrefs.GetInt(EnumGamePrefs.MaxUncoveredMapChunksPerPlayer), 512, 0, 131072)
	{
		this.playerId = _playerId;
	}

	// Token: 0x06004DBD RID: 19901 RVA: 0x001EB078 File Offset: 0x001E9278
	public void Add(int _chunkX, int _chunkZ, ushort[] _mapColors)
	{
		int key = IMapChunkDatabase.ToChunkDBKey(_chunkX, _chunkZ);
		ushort[] array = base.GetDS(key);
		if (array == null)
		{
			array = new ushort[_mapColors.Length];
		}
		Array.Copy(_mapColors, array, _mapColors.Length);
		base.SetDS(key, array);
	}

	// Token: 0x06004DBE RID: 19902 RVA: 0x001EB0B3 File Offset: 0x001E92B3
	public override void Clear()
	{
		base.Clear();
		this.chunksSent.Clear();
		this.bNetworkDataAvail = false;
	}

	// Token: 0x06004DBF RID: 19903 RVA: 0x001EB0CD File Offset: 0x001E92CD
	public ushort[] GetMapColors(long _chunkKey)
	{
		return base.GetDS(IMapChunkDatabase.ToChunkDBKey(_chunkKey));
	}

	// Token: 0x06004DC0 RID: 19904 RVA: 0x001EB0DC File Offset: 0x001E92DC
	public void Add(List<int> _chunks, List<ushort[]> _mapPieces)
	{
		for (int i = 0; i < _chunks.Count; i++)
		{
			base.SetDS(_chunks[i], _mapPieces[i]);
		}
		this.bNetworkDataAvail = true;
	}

	// Token: 0x06004DC1 RID: 19905 RVA: 0x001EB115 File Offset: 0x001E9315
	public bool Contains(long _chunkKey)
	{
		return base.ContainsDS(IMapChunkDatabase.ToChunkDBKey(_chunkKey));
	}

	// Token: 0x06004DC2 RID: 19906 RVA: 0x001EB123 File Offset: 0x001E9323
	public bool IsNetworkDataAvail()
	{
		return this.bNetworkDataAvail;
	}

	// Token: 0x06004DC3 RID: 19907 RVA: 0x001EB12B File Offset: 0x001E932B
	public void ResetNetworkDataAvail()
	{
		this.bNetworkDataAvail = false;
	}

	// Token: 0x06004DC4 RID: 19908 RVA: 0x001EB134 File Offset: 0x001E9334
	public NetPackage GetMapChunkPackagesToSend()
	{
		if (!this.bClientMapMiddlePositionUpdated)
		{
			return null;
		}
		this.bClientMapMiddlePositionUpdated = false;
		this.toSendList.Clear();
		this.mapPiecesList.Clear();
		int num = World.toChunkXZ(this.clientMapMiddlePosition.x);
		int num2 = World.toChunkXZ(this.clientMapMiddlePosition.y);
		int num3 = 8;
		for (int i = -num3; i <= num3; i++)
		{
			for (int j = -num3; j <= num3; j++)
			{
				int num4 = IMapChunkDatabase.ToChunkDBKey(WorldChunkCache.MakeChunkKey(num + i, num2 + j));
				if (!this.chunksSent.Contains(num4) && base.ContainsDS(num4))
				{
					this.toSendList.Add(num4);
					this.chunksSent.Add(num4);
					this.mapPiecesList.Add(base.GetDS(num4));
				}
			}
		}
		if (this.toSendList.Count == 0)
		{
			return null;
		}
		return NetPackageManager.GetPackage<NetPackageMapChunks>().Setup(this.playerId, this.toSendList, this.mapPiecesList);
	}

	// Token: 0x06004DC5 RID: 19909 RVA: 0x001EB22F File Offset: 0x001E942F
	public void LoadAsync(ThreadManager.TaskInfo _taskInfo)
	{
		base.Load(((IMapChunkDatabase.DirectoryPlayerId)_taskInfo.parameter).dir, ((IMapChunkDatabase.DirectoryPlayerId)_taskInfo.parameter).file + ".map");
	}

	// Token: 0x06004DC6 RID: 19910 RVA: 0x001EB261 File Offset: 0x001E9461
	public void SaveAsync(ThreadManager.TaskInfo _taskInfo)
	{
		base.Save(((IMapChunkDatabase.DirectoryPlayerId)_taskInfo.parameter).dir, ((IMapChunkDatabase.DirectoryPlayerId)_taskInfo.parameter).file + ".map");
	}

	// Token: 0x06004DC7 RID: 19911 RVA: 0x001EB293 File Offset: 0x001E9493
	public void SetClientMapMiddlePosition(Vector2i _pos)
	{
		if (!this.clientMapMiddlePosition.Equals(_pos))
		{
			this.clientMapMiddlePosition = _pos;
			this.bClientMapMiddlePositionUpdated = true;
		}
	}

	// Token: 0x06004DC8 RID: 19912 RVA: 0x001D01A2 File Offset: 0x001CE3A2
	[PublicizedFrom(EAccessModifier.Protected)]
	public override int readKey(BinaryReader _br)
	{
		return _br.ReadInt32();
	}

	// Token: 0x06004DC9 RID: 19913 RVA: 0x001D0199 File Offset: 0x001CE399
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void writeKey(BinaryWriter _bw, int _key)
	{
		_bw.Write(_key);
	}

	// Token: 0x06004DCA RID: 19914 RVA: 0x001EB2B4 File Offset: 0x001E94B4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void copyFromRead(byte[] _dataRead, ushort[] _data)
	{
		for (int i = 0; i < _data.Length; i++)
		{
			_data[i] = (ushort)((int)_dataRead[i * 2] | (int)_dataRead[i * 2 + 1] << 8);
		}
	}

	// Token: 0x06004DCB RID: 19915 RVA: 0x001EB2E4 File Offset: 0x001E94E4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void copyToWrite(ushort[] _data, byte[] _dataWrite)
	{
		for (int i = 0; i < _data.Length; i++)
		{
			_dataWrite[i * 2] = (byte)(_data[i] & 255);
			_dataWrite[i * 2 + 1] = (byte)(_data[i] >> 8 & 255);
		}
	}

	// Token: 0x06004DCC RID: 19916 RVA: 0x001EB321 File Offset: 0x001E9521
	[PublicizedFrom(EAccessModifier.Protected)]
	public override ushort[] allocateDataStorage()
	{
		return new ushort[256];
	}

	// Token: 0x04003B4A RID: 15178
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cMaxMapChunks = 131072;

	// Token: 0x04003B4B RID: 15179
	[PublicizedFrom(EAccessModifier.Private)]
	public const string EXT = "map";

	// Token: 0x04003B4C RID: 15180
	[PublicizedFrom(EAccessModifier.Private)]
	public HashSet<int> chunksSent = new HashSet<int>();

	// Token: 0x04003B4D RID: 15181
	[PublicizedFrom(EAccessModifier.Private)]
	public int playerId;

	// Token: 0x04003B4E RID: 15182
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2i clientMapMiddlePosition = new Vector2i(int.MaxValue, int.MaxValue);

	// Token: 0x04003B4F RID: 15183
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bClientMapMiddlePositionUpdated;

	// Token: 0x04003B50 RID: 15184
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bNetworkDataAvail;

	// Token: 0x04003B51 RID: 15185
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<int> toSendList = new List<int>();

	// Token: 0x04003B52 RID: 15186
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<ushort[]> mapPiecesList = new List<ushort[]>();
}
