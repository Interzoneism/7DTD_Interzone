using System;
using System.Collections.Generic;
using System.ComponentModel;
using Platform;

// Token: 0x020009E2 RID: 2530
public interface IMapChunkDatabase
{
	// Token: 0x06004D8A RID: 19850
	void Clear();

	// Token: 0x06004D8B RID: 19851
	ushort[] GetMapColors(long _chunkKey);

	// Token: 0x06004D8C RID: 19852 RVA: 0x001EA4B0 File Offset: 0x001E86B0
	void Add(Vector3i _chunkPos, World _world)
	{
		int num = _world.IsEditor() ? 7 : 4;
		for (int i = -num; i <= num; i++)
		{
			for (int j = -num; j <= num; j++)
			{
				Chunk chunk = (Chunk)_world.GetChunkSync(_chunkPos.x + i, _chunkPos.z + j);
				if (chunk != null && !chunk.NeedsDecoration)
				{
					this.Add(_chunkPos.x + i, _chunkPos.z + j, chunk.GetMapColors());
				}
			}
		}
	}

	// Token: 0x06004D8D RID: 19853
	void Add(int _chunkX, int _chunkZ, ushort[] _mapColors);

	// Token: 0x06004D8E RID: 19854
	void Add(List<int> _chunks, List<ushort[]> _mapPieces);

	// Token: 0x06004D8F RID: 19855
	bool Contains(long _chunkKey);

	// Token: 0x06004D90 RID: 19856
	bool IsNetworkDataAvail();

	// Token: 0x06004D91 RID: 19857
	void ResetNetworkDataAvail();

	// Token: 0x06004D92 RID: 19858
	NetPackage GetMapChunkPackagesToSend();

	// Token: 0x06004D93 RID: 19859
	void LoadAsync(ThreadManager.TaskInfo _taskInfo);

	// Token: 0x06004D94 RID: 19860
	void SaveAsync(ThreadManager.TaskInfo _taskInfo);

	// Token: 0x06004D95 RID: 19861
	void SetClientMapMiddlePosition(Vector2i _pos);

	// Token: 0x06004D96 RID: 19862 RVA: 0x001EA52C File Offset: 0x001E872C
	public static bool TryCreateOrLoad(int _entityId, out IMapChunkDatabase _mapDatabase, Func<IMapChunkDatabase.DirectoryPlayerId> _parameterSupplier)
	{
		if ((DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent())
		{
			bool flag;
			if (_entityId != -1 && GameManager.Instance)
			{
				World world = GameManager.Instance.World;
				if (world == null)
				{
					flag = false;
				}
				else
				{
					EntityPlayerLocal primaryPlayer = world.GetPrimaryPlayer();
					int? num = (primaryPlayer != null) ? new int?(primaryPlayer.entityId) : null;
					flag = (num.GetValueOrDefault() == _entityId & num != null);
				}
			}
			else
			{
				flag = false;
			}
			if (!flag)
			{
				_mapDatabase = null;
				return false;
			}
		}
		MapChunkDatabaseType value = LaunchPrefs.MapChunkDatabase.Value;
		IMapChunkDatabase mapChunkDatabase;
		if (value != MapChunkDatabaseType.Fixed)
		{
			if (value != MapChunkDatabaseType.Region)
			{
				throw new InvalidEnumArgumentException(string.Format("Unknown {0}: {1}", "MapChunkDatabaseType", LaunchPrefs.MapChunkDatabase.Value));
			}
			mapChunkDatabase = new MapChunkDatabaseByRegion(_entityId);
		}
		else
		{
			mapChunkDatabase = new MapChunkDatabase(_entityId);
		}
		IMapChunkDatabase mapChunkDatabase2 = mapChunkDatabase;
		IMapChunkDatabase.DirectoryPlayerId parameter = _parameterSupplier();
		ThreadManager.AddSingleTask(new ThreadManager.TaskFunctionDelegate(mapChunkDatabase2.LoadAsync), parameter, null, true);
		_mapDatabase = mapChunkDatabase2;
		return true;
	}

	// Token: 0x06004D97 RID: 19863 RVA: 0x001EA614 File Offset: 0x001E8814
	public static int ToChunkDBKey(long _worldChunkKey)
	{
		return IMapChunkDatabase.ToChunkDBKey(WorldChunkCache.extractX(_worldChunkKey), WorldChunkCache.extractZ(_worldChunkKey));
	}

	// Token: 0x06004D98 RID: 19864 RVA: 0x001EA627 File Offset: 0x001E8827
	public static int ToChunkDBKey(int _chunkX, int _chunkZ)
	{
		return (_chunkZ & 65535) << 16 | (_chunkX & 65535);
	}

	// Token: 0x06004D99 RID: 19865 RVA: 0x001EA63B File Offset: 0x001E883B
	public static void FromChunkDBKey(int _chunkDBKey, out int _chunkX, out int _chunkZ)
	{
		_chunkX = (_chunkDBKey & 65535);
		if (_chunkX > 32767)
		{
			_chunkX |= -65536;
		}
		_chunkZ = (_chunkDBKey >> 16 & 65535);
		if (_chunkZ > 32767)
		{
			_chunkZ |= -65536;
		}
	}

	// Token: 0x020009E3 RID: 2531
	public class DirectoryPlayerId
	{
		// Token: 0x06004D9A RID: 19866 RVA: 0x001EA678 File Offset: 0x001E8878
		public DirectoryPlayerId(string _dir, string _file)
		{
			this.file = _file;
			this.dir = _dir;
		}

		// Token: 0x04003B2C RID: 15148
		public string file;

		// Token: 0x04003B2D RID: 15149
		public string dir;
	}
}
