using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Scripting;

// Token: 0x020007BD RID: 1981
[Preserve]
public class NetPackageWorldInfo : NetPackage
{
	// Token: 0x0600393B RID: 14651 RVA: 0x00173020 File Offset: 0x00171220
	public NetPackageWorldInfo Setup(string _gameMode, string _levelName, string _gameName, string _guid, PersistentPlayerList _playerList, ulong _ticks, bool _fixedSizeCC, bool _firstTimeJoin, List<WallVolume> wallVolumeData)
	{
		this.gameMode = _gameMode;
		this.levelName = _levelName;
		this.gameName = _gameName;
		this.ppList = _playerList;
		this.ticks = _ticks;
		this.guid = _guid;
		this.fixedSizeCC = _fixedSizeCC;
		this.firstTimeJoin = _firstTimeJoin;
		this.wallVolumes = wallVolumeData;
		return this;
	}

	// Token: 0x170005CC RID: 1484
	// (get) Token: 0x0600393C RID: 14652 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x0600393D RID: 14653 RVA: 0x00173074 File Offset: 0x00171274
	public override void read(PooledBinaryReader _reader)
	{
		this.gameMode = _reader.ReadString();
		this.levelName = _reader.ReadString();
		this.gameName = _reader.ReadString();
		this.guid = _reader.ReadString();
		this.ppList = (_reader.ReadBoolean() ? PersistentPlayerList.Read(_reader) : new PersistentPlayerList());
		this.ticks = _reader.ReadUInt64();
		this.fixedSizeCC = _reader.ReadBoolean();
		this.firstTimeJoin = _reader.ReadBoolean();
		int num = _reader.ReadInt32();
		this.worldFileHashes = new Dictionary<string, uint>();
		for (int i = 0; i < num; i++)
		{
			string key = _reader.ReadString();
			uint value = _reader.ReadUInt32();
			this.worldFileHashes.Add(key, value);
		}
		NetPackageWorldInfo.worldDataSize = _reader.ReadInt64();
		this.wallVolumes = new List<WallVolume>();
		for (uint num2 = (uint)_reader.ReadInt32(); num2 > 0U; num2 -= 1U)
		{
			WallVolume item = WallVolume.Read(_reader);
			this.wallVolumes.Add(item);
		}
	}

	// Token: 0x0600393E RID: 14654 RVA: 0x00173168 File Offset: 0x00171368
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.gameMode);
		_writer.Write(this.levelName);
		_writer.Write(this.gameName);
		_writer.Write(this.guid);
		_writer.Write(this.ppList != null);
		PersistentPlayerList persistentPlayerList = this.ppList;
		if (persistentPlayerList != null)
		{
			persistentPlayerList.Write(_writer);
		}
		_writer.Write(this.ticks);
		_writer.Write(this.fixedSizeCC);
		_writer.Write(this.firstTimeJoin);
		_writer.Write(NetPackageWorldInfo.worldHashesData);
		_writer.Write(NetPackageWorldInfo.worldDataSize);
		uint count = (uint)this.wallVolumes.Count;
		_writer.Write(count);
		foreach (WallVolume wallVolume in this.wallVolumes)
		{
			wallVolume.Write(_writer);
		}
	}

	// Token: 0x0600393F RID: 14655 RVA: 0x00173260 File Offset: 0x00171460
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		_callbacks.WorldInfo(this.gameMode, this.levelName, this.gameName, this.guid, this.ppList, this.ticks, this.fixedSizeCC, this.firstTimeJoin, this.worldFileHashes, NetPackageWorldInfo.worldDataSize, this.wallVolumes);
	}

	// Token: 0x06003940 RID: 14656 RVA: 0x001732B4 File Offset: 0x001714B4
	public override int GetLength()
	{
		return 48 + NetPackageWorldInfo.worldHashesData.Length + 4 + this.wallVolumes.Count * 25 + 8;
	}

	// Token: 0x06003941 RID: 14657 RVA: 0x001732D4 File Offset: 0x001714D4
	public static void PrepareWorldHashes()
	{
		NetPackageWorldInfo.worldHashesData = null;
		ChunkProviderGenerateWorldFromRaw chunkProviderGenerateWorldFromRaw = GameManager.Instance.World.ChunkCache.ChunkProvider as ChunkProviderGenerateWorldFromRaw;
		Dictionary<string, uint> dictionary = (chunkProviderGenerateWorldFromRaw != null) ? chunkProviderGenerateWorldFromRaw.worldFileCrcs : null;
		NetPackageWorldInfo.worldDataSize = ((chunkProviderGenerateWorldFromRaw != null) ? chunkProviderGenerateWorldFromRaw.worldFileTotalSize : 0L);
		List<string> list = null;
		if (dictionary != null)
		{
			list = GameUtils.GetWorldFilesToTransmitToClient(dictionary.Keys);
		}
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				if (dictionary != null)
				{
					binaryWriter.Write(list.Count);
					using (List<string>.Enumerator enumerator = list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							string text = enumerator.Current;
							binaryWriter.Write(text);
							binaryWriter.Write(dictionary[text]);
						}
						goto IL_B5;
					}
				}
				binaryWriter.Write(0);
				IL_B5:
				NetPackageWorldInfo.worldHashesData = memoryStream.ToArray();
			}
		}
	}

	// Token: 0x04002E5D RID: 11869
	[PublicizedFrom(EAccessModifier.Private)]
	public string gameMode;

	// Token: 0x04002E5E RID: 11870
	[PublicizedFrom(EAccessModifier.Private)]
	public string levelName;

	// Token: 0x04002E5F RID: 11871
	[PublicizedFrom(EAccessModifier.Private)]
	public string gameName;

	// Token: 0x04002E60 RID: 11872
	[PublicizedFrom(EAccessModifier.Private)]
	public string guid;

	// Token: 0x04002E61 RID: 11873
	[PublicizedFrom(EAccessModifier.Private)]
	public PersistentPlayerList ppList;

	// Token: 0x04002E62 RID: 11874
	[PublicizedFrom(EAccessModifier.Private)]
	public ulong ticks;

	// Token: 0x04002E63 RID: 11875
	[PublicizedFrom(EAccessModifier.Private)]
	public bool fixedSizeCC;

	// Token: 0x04002E64 RID: 11876
	[PublicizedFrom(EAccessModifier.Private)]
	public bool firstTimeJoin;

	// Token: 0x04002E65 RID: 11877
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, uint> worldFileHashes;

	// Token: 0x04002E66 RID: 11878
	[PublicizedFrom(EAccessModifier.Private)]
	public List<WallVolume> wallVolumes;

	// Token: 0x04002E67 RID: 11879
	[PublicizedFrom(EAccessModifier.Private)]
	public static byte[] worldHashesData;

	// Token: 0x04002E68 RID: 11880
	[PublicizedFrom(EAccessModifier.Private)]
	public static long worldDataSize;
}
