using System;
using System.Collections.Generic;
using System.IO;
using Platform;

// Token: 0x0200010C RID: 268
public class BlockLimitTracker
{
	// Token: 0x0600072E RID: 1838 RVA: 0x00032754 File Offset: 0x00030954
	public static void Init()
	{
		BlockLimitTracker.instance = new BlockLimitTracker();
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			BlockLimitTracker.instance.Load();
		}
	}

	// Token: 0x0600072F RID: 1839 RVA: 0x00032776 File Offset: 0x00030976
	public BlockLimitTracker()
	{
		this.poweredBlockTracker = new BlockTracker(10000);
		this.playerStorageTracker = new BlockTracker(10000);
		this.clientAmounts = new List<int>();
	}

	// Token: 0x06000730 RID: 1840 RVA: 0x000327AC File Offset: 0x000309AC
	public bool CanAddBlock(BlockValue _blockValue, Vector3i _blockPosition, out eSetBlockResponse _response)
	{
		_response = eSetBlockResponse.Success;
		if ((DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX).IsCurrent() || GameManager.Instance.IsEditMode())
		{
			return true;
		}
		if (_blockValue.Block.isMultiBlock && _blockValue.ischild)
		{
			return true;
		}
		if (_blockValue.Block is BlockPowered || _blockValue.Block is BlockPowerSource)
		{
			if (!this.poweredBlockTracker.CanAdd(_blockPosition))
			{
				_response = eSetBlockResponse.PowerBlockLimitExceeded;
				return false;
			}
		}
		else
		{
			if (!(_blockValue.Block is BlockLoot) && !(_blockValue.Block is BlockSecureLoot))
			{
				BlockCompositeTileEntity blockCompositeTileEntity = _blockValue.Block as BlockCompositeTileEntity;
				if (blockCompositeTileEntity == null || !blockCompositeTileEntity.CompositeData.HasFeature<ITileEntityLootable>())
				{
					return true;
				}
			}
			if (!this.playerStorageTracker.CanAdd(_blockPosition))
			{
				_response = eSetBlockResponse.StorageBlockLimitExceeded;
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000731 RID: 1841 RVA: 0x00032868 File Offset: 0x00030A68
	public void TryAddTrackedBlock(BlockValue _blockValue, Vector3i _blockPosition, int _entityId)
	{
		if (_entityId == -1)
		{
			return;
		}
		if (_blockValue.isair)
		{
			return;
		}
		if (_blockValue.Block.isMultiBlock && _blockValue.ischild)
		{
			return;
		}
		Entity entity = GameManager.Instance.World.GetEntity(_entityId);
		if (entity == null || !(entity is EntityPlayer))
		{
			return;
		}
		Log.Out("TryAddTrackedBlock {0} from entity {1}", new object[]
		{
			_blockValue.Block.GetBlockName(),
			_entityId
		});
		if (_blockValue.Block is BlockPowered || _blockValue.Block is BlockPowerSource)
		{
			if (this.poweredBlockTracker.TryAddBlock(_blockPosition))
			{
				this.TriggerSave();
				Log.Out("{0}/{1} Powered Blocks", new object[]
				{
					this.poweredBlockTracker.blockLocations.Count,
					this.poweredBlockTracker.limit
				});
				return;
			}
		}
		else
		{
			if (!(_blockValue.Block is BlockLoot) && !(_blockValue.Block is BlockSecureLoot))
			{
				BlockCompositeTileEntity blockCompositeTileEntity = _blockValue.Block as BlockCompositeTileEntity;
				if (blockCompositeTileEntity == null || !blockCompositeTileEntity.CompositeData.HasFeature<ITileEntityLootable>())
				{
					return;
				}
			}
			if (this.playerStorageTracker.TryAddBlock(_blockPosition))
			{
				this.TriggerSave();
				Log.Out("{0}/{1} Storage Blocks", new object[]
				{
					this.playerStorageTracker.blockLocations.Count,
					this.playerStorageTracker.limit
				});
			}
		}
	}

	// Token: 0x06000732 RID: 1842 RVA: 0x000329E0 File Offset: 0x00030BE0
	public void TryRemoveOrReplaceBlock(BlockValue _oldBlockValue, BlockValue _newBlockValue, Vector3i _blockPosition)
	{
		if (_oldBlockValue.Block.isMultiBlock && _oldBlockValue.ischild)
		{
			return;
		}
		if (!(_oldBlockValue.Block is BlockPowered) && !(_oldBlockValue.Block is BlockPowerSource))
		{
			if (!(_oldBlockValue.Block is BlockLoot) && !(_oldBlockValue.Block is BlockSecureLoot))
			{
				BlockCompositeTileEntity blockCompositeTileEntity = _oldBlockValue.Block as BlockCompositeTileEntity;
				if (blockCompositeTileEntity == null || !blockCompositeTileEntity.CompositeData.HasFeature<ITileEntityLootable>())
				{
					return;
				}
			}
			if (!(_newBlockValue.Block is BlockLoot) && !(_newBlockValue.Block is BlockSecureLoot))
			{
				BlockCompositeTileEntity blockCompositeTileEntity2 = _newBlockValue.Block as BlockCompositeTileEntity;
				if (blockCompositeTileEntity2 == null || !blockCompositeTileEntity2.CompositeData.HasFeature<ITileEntityLootable>())
				{
					if (this.playerStorageTracker.RemoveBlock(_blockPosition))
					{
						this.TriggerSave();
						Log.Out("{0}/{1} Storage Blocks", new object[]
						{
							this.playerStorageTracker.blockLocations.Count,
							this.playerStorageTracker.limit
						});
						return;
					}
					return;
				}
			}
			return;
		}
		if (_newBlockValue.Block is BlockPowered || _newBlockValue.Block is BlockPowerSource)
		{
			return;
		}
		if (this.poweredBlockTracker.RemoveBlock(_blockPosition))
		{
			this.TriggerSave();
			Log.Out("{0}/{1} powered Blocks", new object[]
			{
				this.poweredBlockTracker.blockLocations.Count,
				this.poweredBlockTracker.limit
			});
			return;
		}
	}

	// Token: 0x06000733 RID: 1843 RVA: 0x00032B5C File Offset: 0x00030D5C
	public void ServerUpdateClients()
	{
		if (this.clientAmounts.Count == 0 || this.clientAmounts[0] != this.poweredBlockTracker.blockLocations.Count || this.clientAmounts[1] != this.playerStorageTracker.blockLocations.Count)
		{
			this.clientAmounts.Clear();
			this.clientAmounts.Add(this.poweredBlockTracker.blockLocations.Count);
			this.clientAmounts.Add(this.playerStorageTracker.blockLocations.Count);
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageBlockLimitTracking>().Setup(this.clientAmounts), false, -1, -1, -1, null, 192, false);
		}
	}

	// Token: 0x06000734 RID: 1844 RVA: 0x00032C20 File Offset: 0x00030E20
	public void UpdateClientAmounts(List<int> _amounts)
	{
		if (_amounts.Count != 2)
		{
			Log.Error("Client block limit count not exepcted amount");
			return;
		}
		this.poweredBlockTracker.clientAmount = _amounts[0];
		this.playerStorageTracker.clientAmount = _amounts[1];
	}

	// Token: 0x06000735 RID: 1845 RVA: 0x00032C5A File Offset: 0x00030E5A
	[PublicizedFrom(EAccessModifier.Private)]
	public void ClearAll()
	{
		this.poweredBlockTracker.Clear();
		this.playerStorageTracker.Clear();
	}

	// Token: 0x06000736 RID: 1846 RVA: 0x00032C72 File Offset: 0x00030E72
	public static void Cleanup()
	{
		if (BlockLimitTracker.instance != null)
		{
			BlockLimitTracker.instance.Save();
			BlockLimitTracker.instance.ClearAll();
			BlockLimitTracker.instance = null;
		}
	}

	// Token: 0x06000737 RID: 1847 RVA: 0x00032C98 File Offset: 0x00030E98
	public void Load()
	{
		string path = string.Format("{0}/{1}", GameIO.GetSaveGameDir(), "blockLimits.dat");
		if (SdFile.Exists(path))
		{
			try
			{
				using (Stream stream = SdFile.OpenRead(path))
				{
					using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
					{
						pooledBinaryReader.SetBaseStream(stream);
						this.read(pooledBinaryReader);
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error("BlockLimitTracker Load Exception: " + ex.Message);
				path = string.Format("{0}/{1}", GameIO.GetSaveGameDir(), "blockLimits.dat.bak");
				if (SdFile.Exists(path))
				{
					using (Stream stream2 = SdFile.OpenRead(path))
					{
						using (PooledBinaryReader pooledBinaryReader2 = MemoryPools.poolBinaryReader.AllocSync(false))
						{
							pooledBinaryReader2.SetBaseStream(stream2);
							this.read(pooledBinaryReader2);
						}
					}
				}
			}
		}
	}

	// Token: 0x06000738 RID: 1848 RVA: 0x00032DB8 File Offset: 0x00030FB8
	[PublicizedFrom(EAccessModifier.Private)]
	public void read(PooledBinaryReader _reader)
	{
		if (_reader.ReadByte() != 1)
		{
			Log.Error("BlockLimitTracker Read bad version");
			return;
		}
		this.ClearAll();
		this.poweredBlockTracker.Read(_reader);
		this.playerStorageTracker.Read(_reader);
	}

	// Token: 0x06000739 RID: 1849 RVA: 0x00032DEC File Offset: 0x00030FEC
	[PublicizedFrom(EAccessModifier.Private)]
	public void TriggerSave()
	{
		if (this.saveThread == null || this.saveThread.HasTerminated())
		{
			this.Save();
		}
	}

	// Token: 0x0600073A RID: 1850 RVA: 0x00032E0C File Offset: 0x0003100C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Save()
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && (this.saveThread == null || !ThreadManager.ActiveThreads.ContainsKey("blockLimitSaveData")))
		{
			PooledExpandableMemoryStream pooledExpandableMemoryStream = MemoryPools.poolMemoryStream.AllocSync(true);
			using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
			{
				pooledBinaryWriter.SetBaseStream(pooledExpandableMemoryStream);
				this.write(pooledBinaryWriter);
			}
			this.saveThread = ThreadManager.StartThread("blockLimitSaveData", null, new ThreadManager.ThreadFunctionLoopDelegate(this.SaveThread), null, pooledExpandableMemoryStream, null, false, true);
		}
	}

	// Token: 0x0600073B RID: 1851 RVA: 0x00032EA4 File Offset: 0x000310A4
	[PublicizedFrom(EAccessModifier.Private)]
	public int SaveThread(ThreadManager.ThreadInfo _threadInfo)
	{
		PooledExpandableMemoryStream pooledExpandableMemoryStream = (PooledExpandableMemoryStream)_threadInfo.parameter;
		string text = string.Format("{0}/{1}", GameIO.GetSaveGameDir(), "blockLimits.dat");
		if (SdFile.Exists(text))
		{
			SdFile.Copy(text, string.Format("{0}/{1}.dat.bak", GameIO.GetSaveGameDir(), "blockLimits"), true);
		}
		pooledExpandableMemoryStream.Position = 0L;
		StreamUtils.WriteStreamToFile(pooledExpandableMemoryStream, text);
		MemoryPools.poolMemoryStream.FreeSync(pooledExpandableMemoryStream);
		return -1;
	}

	// Token: 0x0600073C RID: 1852 RVA: 0x00032F10 File Offset: 0x00031110
	[PublicizedFrom(EAccessModifier.Private)]
	public void write(PooledBinaryWriter _writer)
	{
		_writer.Write(1);
		this.poweredBlockTracker.Write(_writer);
		this.playerStorageTracker.Write(_writer);
	}

	// Token: 0x040007ED RID: 2029
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cVersion = 1;

	// Token: 0x040007EE RID: 2030
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cMaxPowerBlocks = 10000;

	// Token: 0x040007EF RID: 2031
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cMaxPlayerStorageBlocks = 10000;

	// Token: 0x040007F0 RID: 2032
	public static BlockLimitTracker instance;

	// Token: 0x040007F1 RID: 2033
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockTracker poweredBlockTracker;

	// Token: 0x040007F2 RID: 2034
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockTracker playerStorageTracker;

	// Token: 0x040007F3 RID: 2035
	[PublicizedFrom(EAccessModifier.Private)]
	public ThreadManager.ThreadInfo saveThread;

	// Token: 0x040007F4 RID: 2036
	[PublicizedFrom(EAccessModifier.Private)]
	public List<int> clientAmounts;

	// Token: 0x040007F5 RID: 2037
	[PublicizedFrom(EAccessModifier.Private)]
	public const string cNameKey = "blockLimits";

	// Token: 0x040007F6 RID: 2038
	[PublicizedFrom(EAccessModifier.Private)]
	public const string cThreadKey = "blockLimitSaveData";
}
