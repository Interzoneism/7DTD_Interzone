using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Platform;
using UnityEngine;

// Token: 0x0200099A RID: 2458
public class Chunk : IChunk, IBlockAccess, IMemoryPoolableObject
{
	// Token: 0x06004A15 RID: 18965 RVA: 0x001D4025 File Offset: 0x001D2225
	public void AssignWaterSimHandle(WaterSimulationNative.ChunkHandle handle)
	{
		this.waterSimHandle = handle;
	}

	// Token: 0x06004A16 RID: 18966 RVA: 0x001D402E File Offset: 0x001D222E
	public void ResetWaterSimHandle()
	{
		this.waterSimHandle.Reset();
	}

	// Token: 0x06004A17 RID: 18967 RVA: 0x001D403B File Offset: 0x001D223B
	public void AssignWaterDebugRenderer(WaterDebugManager.RendererHandle handle)
	{
		this.waterDebugHandle = handle;
	}

	// Token: 0x06004A18 RID: 18968 RVA: 0x00002914 File Offset: 0x00000B14
	public void ResetWaterDebugHandle()
	{
	}

	// Token: 0x06004A19 RID: 18969 RVA: 0x001D4044 File Offset: 0x001D2244
	public byte[] GetTopSoil()
	{
		return this.m_bTopSoilBroken;
	}

	// Token: 0x06004A1A RID: 18970 RVA: 0x001D404C File Offset: 0x001D224C
	public void SetTopSoil(IList<byte> soil)
	{
		for (int i = 0; i < this.m_bTopSoilBroken.Length; i++)
		{
			this.m_bTopSoilBroken[i] = soil[i];
		}
	}

	// Token: 0x06004A1B RID: 18971 RVA: 0x001D407C File Offset: 0x001D227C
	public Chunk()
	{
		this.m_X = 0;
		this.m_Y = 0;
		this.Z = 0;
		for (int i = 0; i < this.trisInMesh.GetLength(0); i++)
		{
			this.trisInMesh[i] = new int[MeshDescription.meshes.Length];
			this.sizeOfMesh[i] = new int[MeshDescription.meshes.Length];
		}
		for (int j = 0; j < 16; j++)
		{
			this.entityLists[j] = new List<Entity>();
		}
		this.NeedsLightCalculation = true;
		this.NeedsDecoration = true;
		this.hasEntities = false;
		this.isModified = false;
		this.m_BlockLayers = new ChunkBlockLayer[64];
		this.chnLight = new ChunkBlockChannel(0L, 1);
		this.chnDensity = new ChunkBlockChannel((long)((ulong)((byte)MarchingCubes.DensityAir)), 1);
		this.chnStability = new ChunkBlockChannel(0L, 1);
		this.chnDamage = new ChunkBlockChannel(0L, 2);
		this.chnTextures = new ChunkBlockChannel[1];
		for (int k = 0; k < 1; k++)
		{
			this.chnTextures[k] = new ChunkBlockChannel(0L, 6);
		}
		this.chnWater = new ChunkBlockChannel(0L, 2);
		this.m_HeightMap = new byte[256];
		this.m_TerrainHeight = new byte[256];
		this.m_bTopSoilBroken = new byte[32];
		this.m_Biomes = new byte[256];
		this.m_BiomeIntensities = new byte[1536];
		this.m_NormalX = new byte[256];
		this.m_NormalY = new byte[256];
		this.m_NormalZ = new byte[256];
		Chunk.InstanceCount++;
	}

	// Token: 0x06004A1C RID: 18972 RVA: 0x001D4313 File Offset: 0x001D2513
	public Chunk(int _x, int _z) : this()
	{
		this.m_X = _x;
		this.m_Y = 0;
		this.m_Z = _z;
		this.ResetStability();
		this.RefreshSunlight();
		this.NeedsLightCalculation = true;
		this.NeedsDecoration = false;
	}

	// Token: 0x06004A1D RID: 18973 RVA: 0x001D4350 File Offset: 0x001D2550
	[PublicizedFrom(EAccessModifier.Protected)]
	public ~Chunk()
	{
		Chunk.InstanceCount--;
	}

	// Token: 0x06004A1E RID: 18974 RVA: 0x001D4384 File Offset: 0x001D2584
	public void ResetLights(byte _lightValue = 0)
	{
		this.chnLight.Clear((long)((ulong)_lightValue));
	}

	// Token: 0x06004A1F RID: 18975 RVA: 0x001D4394 File Offset: 0x001D2594
	public void Reset()
	{
		if (this.InProgressSaving)
		{
			Log.Warning("Unloading: chunk while saving " + ((this != null) ? this.ToString() : null));
		}
		this.cachedToString = null;
		this.m_X = 0;
		this.m_Y = 0;
		this.Z = 0;
		this.MeshLayerCount = 0;
		for (int i = 0; i < 16; i++)
		{
			this.entityLists[i].Clear();
		}
		this.entityStubs.Clear();
		this.blockEntityStubs.Clear();
		this.sleeperVolumes.Clear();
		this.triggerVolumes.Clear();
		this.tileEntities.Clear();
		this.IndexedBlocks.Clear();
		this.triggerData.Clear();
		this.insideDevices.Clear();
		this.insideDevicesHashSet.Clear();
		this.NeedsRegeneration = false;
		this.NeedsDecoration = true;
		this.NeedsLightDecoration = false;
		this.NeedsLightCalculation = true;
		this.hasEntities = false;
		this.isModified = false;
		this.InProgressRegeneration = false;
		this.InProgressSaving = false;
		this.InProgressCopying = false;
		this.InProgressDecorating = false;
		this.InProgressLighting = false;
		this.InProgressUnloading = false;
		this.NeedsOnlyCollisionMesh = false;
		this.IsCollisionMeshGenerated = false;
		this.SavedInWorldTicks = 0UL;
		MemoryPools.poolCBL.FreeSync(this.m_BlockLayers);
		this.chnDensity.FreeLayers();
		this.chnStability.FreeLayers();
		this.chnLight.FreeLayers();
		this.chnDamage.FreeLayers();
		for (int j = 0; j < 1; j++)
		{
			this.chnTextures[j].FreeLayers();
		}
		this.chnWater.FreeLayers();
		this.ResetLights(0);
		Array.Clear(this.m_HeightMap, 0, this.m_HeightMap.GetLength(0));
		Array.Clear(this.m_TerrainHeight, 0, this.m_TerrainHeight.GetLength(0));
		Array.Clear(this.m_bTopSoilBroken, 0, this.m_bTopSoilBroken.GetLength(0));
		Array.Clear(this.m_Biomes, 0, this.m_Biomes.GetLength(0));
		Array.Clear(this.m_NormalX, 0, this.m_NormalX.GetLength(0));
		Array.Clear(this.m_NormalY, 0, this.m_NormalY.GetLength(0));
		Array.Clear(this.m_NormalZ, 0, this.m_NormalZ.GetLength(0));
		this.ResetBiomeIntensity(BiomeIntensity.Default);
		this.DominantBiome = 0;
		this.AreaMasterDominantBiome = byte.MaxValue;
		this.biomeSpawnData = null;
		if (this.m_DecoBiomeArray != null)
		{
			Array.Clear(this.m_DecoBiomeArray, 0, this.m_DecoBiomeArray.GetLength(0));
		}
		this.ChunkCustomData.Clear();
		this.bMapDirty = true;
		DictionaryKeyList<Vector3i, int> obj = this.tickedBlocks;
		lock (obj)
		{
			this.tickedBlocks.Clear();
		}
		this.bEmptyDirty = true;
		this.StopStabilityCalculation = true;
		this.waterSimHandle.Reset();
	}

	// Token: 0x06004A20 RID: 18976 RVA: 0x001D402E File Offset: 0x001D222E
	public void Cleanup()
	{
		this.waterSimHandle.Reset();
	}

	// Token: 0x170007C9 RID: 1993
	// (get) Token: 0x06004A21 RID: 18977 RVA: 0x001D46A0 File Offset: 0x001D28A0
	// (set) Token: 0x06004A22 RID: 18978 RVA: 0x001D46A8 File Offset: 0x001D28A8
	public int X
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.m_X;
		}
		set
		{
			this.cachedToString = null;
			this.m_X = value;
			this.updateBounds();
		}
	}

	// Token: 0x170007CA RID: 1994
	// (get) Token: 0x06004A23 RID: 18979 RVA: 0x001D46BE File Offset: 0x001D28BE
	public int Y
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.m_Y;
		}
	}

	// Token: 0x170007CB RID: 1995
	// (get) Token: 0x06004A24 RID: 18980 RVA: 0x001D46C6 File Offset: 0x001D28C6
	// (set) Token: 0x06004A25 RID: 18981 RVA: 0x001D46CE File Offset: 0x001D28CE
	public int Z
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.m_Z;
		}
		set
		{
			this.cachedToString = null;
			this.m_Z = value;
			this.updateBounds();
		}
	}

	// Token: 0x170007CC RID: 1996
	// (get) Token: 0x06004A26 RID: 18982 RVA: 0x001D46E4 File Offset: 0x001D28E4
	// (set) Token: 0x06004A27 RID: 18983 RVA: 0x001D46FD File Offset: 0x001D28FD
	public Vector3i ChunkPos
	{
		get
		{
			return new Vector3i(this.m_X, this.m_Y, this.m_Z);
		}
		set
		{
			this.cachedToString = null;
			this.m_X = value.x;
			this.m_Z = value.z;
			this.updateBounds();
		}
	}

	// Token: 0x170007CD RID: 1997
	// (get) Token: 0x06004A28 RID: 18984 RVA: 0x001D4724 File Offset: 0x001D2924
	public long Key
	{
		get
		{
			return WorldChunkCache.MakeChunkKey(this.m_X, this.m_Z);
		}
	}

	// Token: 0x170007CE RID: 1998
	// (get) Token: 0x06004A29 RID: 18985 RVA: 0x001D4738 File Offset: 0x001D2938
	public bool IsLocked
	{
		get
		{
			return this.InProgressCopying || this.InProgressDecorating || this.InProgressLighting || this.InProgressRegeneration || this.InProgressUnloading || this.InProgressSaving || this.InProgressNetworking || this.InProgressWaterSim;
		}
	}

	// Token: 0x170007CF RID: 1999
	// (get) Token: 0x06004A2A RID: 18986 RVA: 0x001D4798 File Offset: 0x001D2998
	public bool IsLockedExceptUnloading
	{
		get
		{
			return this.InProgressCopying || this.InProgressDecorating || this.InProgressLighting || this.InProgressRegeneration || this.InProgressSaving || this.InProgressNetworking || this.InProgressWaterSim;
		}
	}

	// Token: 0x170007D0 RID: 2000
	// (get) Token: 0x06004A2B RID: 18987 RVA: 0x001D47EB File Offset: 0x001D29EB
	public bool IsInitialized
	{
		get
		{
			return !this.NeedsLightCalculation && !this.InProgressDecorating && !this.InProgressUnloading;
		}
	}

	// Token: 0x06004A2C RID: 18988 RVA: 0x001D480E File Offset: 0x001D2A0E
	public bool GetAvailable()
	{
		return this.IsCollisionMeshGenerated;
	}

	// Token: 0x170007D1 RID: 2001
	// (get) Token: 0x06004A2D RID: 18989 RVA: 0x001D4818 File Offset: 0x001D2A18
	// (set) Token: 0x06004A2E RID: 18990 RVA: 0x001D485C File Offset: 0x001D2A5C
	public bool NeedsRegeneration
	{
		get
		{
			bool result;
			lock (this)
			{
				result = (this.m_NeedsRegenerationAtY != 0);
			}
			return result;
		}
		set
		{
			Queue<int> layerIndexQueue = this.m_layerIndexQueue;
			lock (layerIndexQueue)
			{
				this.MeshLayerCount = 0;
				this.m_layerIndexQueue.Clear();
				MemoryPools.poolVML.FreeSync(this.m_meshLayers);
			}
			lock (this)
			{
				if (value)
				{
					this.m_NeedsRegenerationAtY = 65535;
				}
				else
				{
					this.m_NeedsRegenerationAtY = 0;
				}
			}
			this.NeedsRegenerationDebug = this.m_NeedsRegenerationAtY;
		}
	}

	// Token: 0x06004A2F RID: 18991 RVA: 0x001D4908 File Offset: 0x001D2B08
	public void ClearNeedsRegenerationAt(int _idx)
	{
		lock (this)
		{
			this.m_NeedsRegenerationAtY &= ~(1 << _idx);
			this.NeedsRegenerationDebug = this.m_NeedsRegenerationAtY;
		}
	}

	// Token: 0x170007D2 RID: 2002
	// (get) Token: 0x06004A30 RID: 18992 RVA: 0x001D4964 File Offset: 0x001D2B64
	public bool NeedsCopying
	{
		get
		{
			return this.HasMeshLayer();
		}
	}

	// Token: 0x170007D3 RID: 2003
	// (get) Token: 0x06004A31 RID: 18993 RVA: 0x001D496C File Offset: 0x001D2B6C
	// (set) Token: 0x06004A32 RID: 18994 RVA: 0x001D49AC File Offset: 0x001D2BAC
	public int NeedsRegenerationAt
	{
		get
		{
			int needsRegenerationAtY;
			lock (this)
			{
				needsRegenerationAtY = this.m_NeedsRegenerationAtY;
			}
			return needsRegenerationAtY;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			lock (this)
			{
				this.m_NeedsRegenerationAtY |= 1 << (value >> 4);
			}
		}
	}

	// Token: 0x06004A33 RID: 18995 RVA: 0x001D49FC File Offset: 0x001D2BFC
	public void SetNeedsRegenerationRaw(int _v)
	{
		this.m_NeedsRegenerationAtY = _v;
	}

	// Token: 0x170007D4 RID: 2004
	// (get) Token: 0x06004A34 RID: 18996 RVA: 0x001D4A07 File Offset: 0x001D2C07
	public bool NeedsSaving
	{
		get
		{
			return this.isModified || this.hasEntities || this.tileEntities.Count > 0 || this.triggerData.Count > 0;
		}
	}

	// Token: 0x06004A35 RID: 18997 RVA: 0x001D4A39 File Offset: 0x001D2C39
	public void load(PooledBinaryReader stream, uint _version)
	{
		this.read(stream, _version, false);
		this.isModified = false;
	}

	// Token: 0x06004A36 RID: 18998 RVA: 0x001D4A4B File Offset: 0x001D2C4B
	public void read(PooledBinaryReader stream, uint _version)
	{
		this.read(stream, _version, true);
	}

	// Token: 0x06004A37 RID: 18999 RVA: 0x001D4A58 File Offset: 0x001D2C58
	[PublicizedFrom(EAccessModifier.Private)]
	public void read(PooledBinaryReader _br, uint _version, bool _bNetworkRead)
	{
		this.cachedToString = null;
		this.m_X = _br.ReadInt32();
		this.m_Y = _br.ReadInt32();
		this.Z = _br.ReadInt32();
		if (_version > 30U)
		{
			this.SavedInWorldTicks = _br.ReadUInt64();
		}
		this.LastTimeRandomTicked = this.SavedInWorldTicks;
		MemoryPools.poolCBL.FreeSync(this.m_BlockLayers);
		Array.Clear(this.m_HeightMap, 0, 256);
		if (_version < 28U)
		{
			throw new Exception("Chunk version " + _version.ToString() + " not supported any more!");
		}
		for (int i = 0; i < 64; i++)
		{
			if (_br.ReadBoolean())
			{
				ChunkBlockLayer chunkBlockLayer = MemoryPools.poolCBL.AllocSync(false);
				chunkBlockLayer.Read(_br, _version, _bNetworkRead);
				this.m_BlockLayers[i] = chunkBlockLayer;
				this.bEmptyDirty = true;
			}
		}
		if (_version < 28U)
		{
			ChunkBlockLayerLegacy[] blockLayers = new ChunkBlockLayerLegacy[256];
			this.chnStability.Convert(blockLayers);
		}
		else if (!_bNetworkRead)
		{
			this.chnStability.Read(_br, _version, _bNetworkRead);
		}
		_br.Flush();
		this.recalcIndexedBlocks();
		BinaryFormatter binaryFormatter = null;
		if (_version < 10U)
		{
			binaryFormatter = new BinaryFormatter();
			byte[,] array = (byte[,])binaryFormatter.Deserialize(_br.BaseStream);
			for (int j = 0; j < 16; j++)
			{
				for (int k = 0; k < 16; k++)
				{
					this.m_HeightMap[j + k * 16] = array[j, k];
				}
			}
		}
		else
		{
			_br.Read(this.m_HeightMap, 0, 256);
		}
		if (_version >= 7U && _version < 8U)
		{
			if (binaryFormatter == null)
			{
				binaryFormatter = new BinaryFormatter();
			}
			byte[,] array2 = (byte[,])binaryFormatter.Deserialize(_br.BaseStream);
			this.m_TerrainHeight = new byte[array2.GetLength(0) * array2.GetLength(1)];
			for (int l = 0; l < array2.GetLength(0); l++)
			{
				for (int m = 0; m < array2.GetLength(1); m++)
				{
					this.SetTerrainHeight(l, m, array2[l, m]);
				}
			}
		}
		else if (_version > 21U)
		{
			_br.Read(this.m_TerrainHeight, 0, this.m_TerrainHeight.Length);
		}
		if (_version > 41U)
		{
			_br.Read(this.m_bTopSoilBroken, 0, 32);
		}
		if (_version > 8U && _version < 15U)
		{
			if (binaryFormatter == null)
			{
				binaryFormatter = new BinaryFormatter();
			}
			byte[,] array3 = (byte[,])binaryFormatter.Deserialize(_br.BaseStream);
			this.m_Biomes = new byte[array3.GetLength(0) * array3.GetLength(1)];
			for (int n = 0; n < array3.GetLength(0); n++)
			{
				for (int num = 0; num < array3.GetLength(1); num++)
				{
					this.SetBiomeId(n, num, array3[n, num]);
				}
			}
		}
		else
		{
			_br.Read(this.m_Biomes, 0, 256);
		}
		if (_version > 19U)
		{
			_br.Read(this.m_BiomeIntensities, 0, 1536);
		}
		else
		{
			for (int num2 = 0; num2 < this.m_BiomeIntensities.Length; num2 += 6)
			{
				BiomeIntensity.Default.ToArray(this.m_BiomeIntensities, num2);
			}
		}
		if (_version > 23U)
		{
			this.DominantBiome = _br.ReadByte();
		}
		if (_version > 24U)
		{
			this.AreaMasterDominantBiome = _br.ReadByte();
		}
		if (_version > 25U)
		{
			int num3 = (int)_br.ReadUInt16();
			this.ChunkCustomData.Clear();
			for (int num4 = 0; num4 < num3; num4++)
			{
				ChunkCustomData chunkCustomData = new ChunkCustomData();
				chunkCustomData.Read(_br);
				this.ChunkCustomData.Set(chunkCustomData.key, chunkCustomData);
			}
		}
		if (_version > 22U)
		{
			_br.Read(this.m_NormalX, 0, 256);
		}
		if (_version > 20U)
		{
			_br.Read(this.m_NormalY, 0, 256);
		}
		if (_version > 22U)
		{
			_br.Read(this.m_NormalZ, 0, 256);
		}
		if (_version > 12U && _version < 27U)
		{
			throw new Exception("Chunk version " + _version.ToString() + " not supported any more!");
		}
		this.chnDensity.Read(_br, _version, _bNetworkRead);
		if (_version < 27U)
		{
			SmartArray smartArray = new SmartArray(4, 8, 4);
			smartArray.read(_br);
			SmartArray smartArray2 = new SmartArray(4, 8, 4);
			smartArray2.read(_br);
			this.chnLight.Convert(smartArray, 0);
			this.chnLight.Convert(smartArray2, 4);
		}
		else
		{
			this.chnLight.Read(_br, _version, _bNetworkRead);
		}
		if (_version >= 33U && _version < 36U)
		{
			ChunkBlockChannel chunkBlockChannel = new ChunkBlockChannel(0L, 1);
			chunkBlockChannel.Read(_br, _version, _bNetworkRead);
			chunkBlockChannel.Read(_br, _version, _bNetworkRead);
		}
		if (_version >= 36U)
		{
			this.chnDamage.Read(_br, _version, _bNetworkRead);
		}
		if (_version >= 47U)
		{
			for (int num5 = 0; num5 < 1; num5++)
			{
				this.chnTextures[num5].Read(_br, _version, _bNetworkRead);
			}
		}
		else if (_version >= 35U)
		{
			this.chnTextures[0].Read(_br, _version, _bNetworkRead);
		}
		if (_version >= 46U)
		{
			this.chnWater.Read(_br, _version, _bNetworkRead);
		}
		else if (WaterSimulationNative.Instance.IsInitialized)
		{
			throw new Exception("Serialized data incompatible with new water simulation");
		}
		this.NeedsDecoration = false;
		this.NeedsLightCalculation = false;
		if (_version >= 6U)
		{
			this.NeedsLightCalculation = _br.ReadBoolean();
		}
		int num6 = _br.ReadInt32();
		for (int num7 = 0; num7 < 16; num7++)
		{
			this.entityLists[num7].Clear();
		}
		this.entityStubs.Clear();
		for (int num8 = 0; num8 < num6; num8++)
		{
			EntityCreationData entityCreationData = new EntityCreationData();
			entityCreationData.read(_br, _bNetworkRead);
			this.entityStubs.Add(entityCreationData);
		}
		this.hasEntities = (this.entityStubs.Count > 0);
		if (_version > 13U && _version < 32U)
		{
			num6 = _br.ReadInt32();
		}
		num6 = _br.ReadInt32();
		this.tileEntities.Clear();
		for (int num9 = 0; num9 < num6; num9++)
		{
			TileEntity tileEntity = TileEntity.Instantiate((TileEntityType)_br.ReadInt32(), this);
			if (tileEntity != null)
			{
				tileEntity.read(_br, _bNetworkRead ? TileEntity.StreamModeRead.FromServer : TileEntity.StreamModeRead.Persistency);
				tileEntity.OnReadComplete();
				this.tileEntities.Set(tileEntity.localChunkPos, tileEntity);
			}
		}
		if (_version > 10U && _version < 43U && !_bNetworkRead)
		{
			_br.ReadUInt16();
			_br.ReadByte();
		}
		if (_version > 33U && _br.ReadBoolean())
		{
			for (int num10 = 0; num10 < 16; num10++)
			{
				_br.ReadUInt16();
			}
		}
		if (!_bNetworkRead && _version == 37U)
		{
			byte b = _br.ReadByte();
			for (int num11 = 0; num11 < (int)b; num11++)
			{
				SleeperVolume.Read(_br);
			}
		}
		if (!_bNetworkRead && _version > 37U)
		{
			this.sleeperVolumes.Clear();
			int num12 = (int)_br.ReadByte();
			for (int num13 = 0; num13 < num12; num13++)
			{
				int num14 = _br.ReadInt32();
				if (num14 < 0)
				{
					Log.Error("chunk sleeper volumeId invalid {0}", new object[]
					{
						num14
					});
				}
				else
				{
					this.AddSleeperVolumeId(num14);
				}
			}
		}
		if (!_bNetworkRead && _version >= 44U)
		{
			this.triggerVolumes.Clear();
			int num15 = (int)_br.ReadByte();
			for (int num16 = 0; num16 < num15; num16++)
			{
				int num17 = _br.ReadInt32();
				if (num17 < 0)
				{
					Log.Error("chunk trigger volumeId invalid {0}", new object[]
					{
						num17
					});
				}
				else
				{
					this.AddTriggerVolumeId(num17);
				}
			}
		}
		if (_version >= 45U)
		{
			this.wallVolumes.Clear();
			int num18 = (int)_br.ReadByte();
			for (int num19 = 0; num19 < num18; num19++)
			{
				int num20 = _br.ReadInt32();
				if (num20 < 0)
				{
					Log.Error("chunk wall volumeId invalid {0}", new object[]
					{
						num20
					});
				}
				else
				{
					this.AddWallVolumeId(num20);
				}
			}
		}
		if (_bNetworkRead)
		{
			_br.ReadBoolean();
		}
		DictionaryKeyList<Vector3i, int> obj = this.tickedBlocks;
		lock (obj)
		{
			this.tickedBlocks.Clear();
			for (int num21 = 0; num21 < 64; num21++)
			{
				ChunkBlockLayer chunkBlockLayer2 = this.m_BlockLayers[num21];
				if (chunkBlockLayer2 != null)
				{
					for (int num22 = 0; num22 < 1024; num22++)
					{
						int idAt = chunkBlockLayer2.GetIdAt(num22);
						if (idAt != 0 && Block.BlocksLoaded && idAt < Block.list.Length && Block.list[idAt] != null && Block.list[idAt].IsRandomlyTick && !chunkBlockLayer2.GetAt(num22).ischild)
						{
							int x = num22 % 256 % 16;
							int y = num21 * 4 + num22 / 256;
							int z = num22 % 256 / 16;
							this.tickedBlocks.Add(this.ToWorldPos(x, y, z), 0);
						}
					}
				}
			}
		}
		this.insideDevices.Clear();
		if (_version > 39U)
		{
			int num23 = (int)_br.ReadInt16();
			this.insideDevices.Capacity = num23;
			byte x2 = 0;
			byte z2 = 0;
			int num24 = 0;
			for (int num25 = 0; num25 < num23; num25++)
			{
				if (num24 == 0)
				{
					x2 = _br.ReadByte();
					z2 = _br.ReadByte();
					num24 = (int)_br.ReadByte();
				}
				Vector3b item = new Vector3b(x2, _br.ReadByte(), z2);
				this.insideDevices.Add(item);
				this.insideDevicesHashSet.Add(item.GetHashCode());
				num24--;
			}
		}
		if (_version > 40U)
		{
			this.IsInternalBlocksCulled = _br.ReadBoolean();
		}
		if (_version > 42U && !_bNetworkRead)
		{
			this.triggerData.Clear();
			int num26 = (int)_br.ReadInt16();
			for (int num27 = 0; num27 < num26; num27++)
			{
				Vector3i vector3i = StreamUtils.ReadVector3i(_br);
				BlockTrigger blockTrigger = new BlockTrigger(this);
				blockTrigger.LocalChunkPos = vector3i;
				blockTrigger.Read(_br);
				this.triggerData.Add(vector3i, blockTrigger);
			}
		}
		if (_bNetworkRead)
		{
			this.ResetStabilityToBottomMost();
			this.NeedsLightCalculation = true;
		}
		this.bMapDirty = true;
		this.StopStabilityCalculation = false;
	}

	// Token: 0x06004A38 RID: 19000 RVA: 0x001D5414 File Offset: 0x001D3614
	public void save(PooledBinaryWriter stream)
	{
		this.saveBlockIds();
		this.write(stream, false);
		this.isModified = false;
		this.SavedInWorldTicks = GameTimer.Instance.ticks;
	}

	// Token: 0x06004A39 RID: 19001 RVA: 0x001D543C File Offset: 0x001D363C
	[PublicizedFrom(EAccessModifier.Private)]
	public void saveBlockIds()
	{
		if (Block.nameIdMapping != null)
		{
			NameIdMapping nameIdMapping = Block.nameIdMapping;
			NameIdMapping obj = nameIdMapping;
			lock (obj)
			{
				for (int i = 0; i < 256; i += 4)
				{
					int num = i >> 2;
					ChunkBlockLayer chunkBlockLayer = this.m_BlockLayers[num];
					if (chunkBlockLayer == null)
					{
						Block block = BlockValue.Air.Block;
						nameIdMapping.AddMapping(block.blockID, block.GetBlockName(), false);
					}
					else
					{
						chunkBlockLayer.SaveBlockMappings(nameIdMapping);
					}
				}
			}
		}
	}

	// Token: 0x06004A3A RID: 19002 RVA: 0x001D54D0 File Offset: 0x001D36D0
	public void write(PooledBinaryWriter stream)
	{
		this.write(stream, true);
	}

	// Token: 0x06004A3B RID: 19003 RVA: 0x001D54DC File Offset: 0x001D36DC
	[PublicizedFrom(EAccessModifier.Private)]
	public void write(PooledBinaryWriter _bw, bool _bNetworkWrite)
	{
		byte[] array = MemoryPools.poolByte.Alloc(256);
		_bw.Write(this.m_X);
		_bw.Write(this.m_Y);
		_bw.Write(this.m_Z);
		_bw.Write(this.SavedInWorldTicks);
		for (int i = 0; i < 64; i++)
		{
			bool flag = this.m_BlockLayers[i] != null;
			_bw.Write(flag);
			if (flag)
			{
				this.m_BlockLayers[i].Write(_bw, _bNetworkWrite);
			}
		}
		if (!_bNetworkWrite)
		{
			this.chnStability.Write(_bw, _bNetworkWrite, array);
		}
		_bw.Write(this.m_HeightMap);
		_bw.Write(this.m_TerrainHeight);
		_bw.Write(this.m_bTopSoilBroken);
		_bw.Write(this.m_Biomes);
		_bw.Write(this.m_BiomeIntensities);
		_bw.Write(this.DominantBiome);
		_bw.Write(this.AreaMasterDominantBiome);
		int num = 0;
		if (_bNetworkWrite)
		{
			for (int j = 0; j < this.ChunkCustomData.valueList.Count; j++)
			{
				if (this.ChunkCustomData.valueList[j].isSavedToNetwork)
				{
					num++;
				}
			}
		}
		else
		{
			num = this.ChunkCustomData.valueList.Count;
		}
		_bw.Write((ushort)num);
		for (int k = 0; k < this.ChunkCustomData.valueList.Count; k++)
		{
			if (!_bNetworkWrite || this.ChunkCustomData.valueList[k].isSavedToNetwork)
			{
				this.ChunkCustomData.valueList[k].Write(_bw);
			}
		}
		_bw.Write(this.m_NormalX);
		_bw.Write(this.m_NormalY);
		_bw.Write(this.m_NormalZ);
		this.chnDensity.Write(_bw, _bNetworkWrite, array);
		this.chnLight.Write(_bw, _bNetworkWrite, array);
		this.chnDamage.Write(_bw, _bNetworkWrite, array);
		for (int l = 0; l < 1; l++)
		{
			this.chnTextures[l].Write(_bw, _bNetworkWrite, array);
		}
		this.chnWater.Write(_bw, _bNetworkWrite, array);
		_bw.Write(this.NeedsLightCalculation);
		int num2 = 0;
		for (int m = 0; m < 16; m++)
		{
			List<Entity> list = this.entityLists[m];
			for (int n = 0; n < list.Count; n++)
			{
				Entity entity = list[n];
				if (!(entity is EntityVehicle) && !(entity is EntityDrone) && ((!_bNetworkWrite && entity.IsSavedToFile()) || (_bNetworkWrite && entity.IsSavedToNetwork())))
				{
					num2++;
				}
			}
		}
		_bw.Write(num2);
		for (int num3 = 0; num3 < 16; num3++)
		{
			List<Entity> list2 = this.entityLists[num3];
			for (int num4 = 0; num4 < list2.Count; num4++)
			{
				Entity entity2 = list2[num4];
				if (!(entity2 is EntityVehicle) && !(entity2 is EntityDrone) && ((!_bNetworkWrite && entity2.IsSavedToFile()) || (_bNetworkWrite && entity2.IsSavedToNetwork())))
				{
					new EntityCreationData(entity2, true).write(_bw, _bNetworkWrite);
				}
			}
		}
		_bw.Write(this.tileEntities.Count);
		for (int num5 = 0; num5 < this.tileEntities.list.Count; num5++)
		{
			_bw.Write((int)this.tileEntities.list[num5].GetTileEntityType());
			this.tileEntities.list[num5].write(_bw, _bNetworkWrite ? TileEntity.StreamModeWrite.ToClient : TileEntity.StreamModeWrite.Persistency);
		}
		_bw.Write(false);
		if (!_bNetworkWrite)
		{
			int count = this.sleeperVolumes.Count;
			_bw.Write((byte)count);
			for (int num6 = 0; num6 < count; num6++)
			{
				_bw.Write(this.sleeperVolumes[num6]);
			}
		}
		if (!_bNetworkWrite)
		{
			int count2 = this.triggerVolumes.Count;
			_bw.Write((byte)count2);
			for (int num7 = 0; num7 < count2; num7++)
			{
				_bw.Write(this.triggerVolumes[num7]);
			}
		}
		int count3 = this.wallVolumes.Count;
		_bw.Write((byte)count3);
		for (int num8 = 0; num8 < count3; num8++)
		{
			_bw.Write(this.wallVolumes[num8]);
		}
		if (_bNetworkWrite)
		{
			_bw.Write(false);
		}
		List<byte> list3 = new List<byte>();
		int num9 = int.MaxValue;
		int num10 = int.MaxValue;
		_bw.Write((short)this.insideDevices.Count);
		foreach (Vector3b vector3b in this.insideDevices)
		{
			if (list3.Count > 254 || num9 != (int)vector3b.x || num10 != (int)vector3b.z)
			{
				if (list3.Count > 0)
				{
					_bw.Write((byte)num9);
					_bw.Write((byte)num10);
					_bw.Write((byte)list3.Count);
					for (int num11 = 0; num11 < list3.Count; num11++)
					{
						_bw.Write(list3[num11]);
					}
					list3.Clear();
				}
				num9 = (int)vector3b.x;
				num10 = (int)vector3b.z;
			}
			list3.Add(vector3b.y);
		}
		if (list3.Count > 0)
		{
			_bw.Write((byte)num9);
			_bw.Write((byte)num10);
			_bw.Write((byte)list3.Count);
			for (int num12 = 0; num12 < list3.Count; num12++)
			{
				_bw.Write(list3[num12]);
			}
		}
		_bw.Write(this.IsInternalBlocksCulled);
		if (!_bNetworkWrite)
		{
			int count4 = this.triggerData.Count;
			_bw.Write((short)count4);
			for (int num13 = 0; num13 < count4; num13++)
			{
				StreamUtils.Write(_bw, this.triggerData.list[num13].LocalChunkPos);
				this.triggerData.list[num13].Write(_bw);
			}
		}
		MemoryPools.poolByte.Free(array);
	}

	// Token: 0x06004A3C RID: 19004 RVA: 0x001D5AE0 File Offset: 0x001D3CE0
	[PublicizedFrom(EAccessModifier.Private)]
	public void recalcIndexedBlocks()
	{
		this.IndexedBlocks.Clear();
		for (int i = 0; i < 64; i++)
		{
			ChunkBlockLayer chunkBlockLayer = this.m_BlockLayers[i];
			if (chunkBlockLayer != null)
			{
				chunkBlockLayer.AddIndexedBlocks(i, this.IndexedBlocks);
			}
		}
	}

	// Token: 0x06004A3D RID: 19005 RVA: 0x001D5B1E File Offset: 0x001D3D1E
	public void AddEntityStub(EntityCreationData _ecd)
	{
		this.entityStubs.Add(_ecd);
	}

	// Token: 0x06004A3E RID: 19006 RVA: 0x001D5B2C File Offset: 0x001D3D2C
	public BlockEntityData GetBlockEntity(Vector3i _worldPos)
	{
		BlockEntityData result;
		this.blockEntityStubs.dict.TryGetValue(GameUtils.Vector3iToUInt64(_worldPos), out result);
		return result;
	}

	// Token: 0x06004A3F RID: 19007 RVA: 0x001D5B54 File Offset: 0x001D3D54
	public BlockEntityData GetBlockEntity(Transform _transform)
	{
		for (int i = 0; i < this.blockEntityStubs.list.Count; i++)
		{
			if (this.blockEntityStubs.list[i].transform == _transform)
			{
				return this.blockEntityStubs.list[i];
			}
		}
		return null;
	}

	// Token: 0x06004A40 RID: 19008 RVA: 0x001D5BB0 File Offset: 0x001D3DB0
	public void AddEntityBlockStub(BlockEntityData _ecd)
	{
		ulong key = GameUtils.Vector3iToUInt64(_ecd.pos);
		BlockEntityData item;
		if (this.blockEntityStubs.dict.TryGetValue(key, out item))
		{
			this.blockEntityStubsToRemove.Add(item);
		}
		this.blockEntityStubs.Set(key, _ecd);
	}

	// Token: 0x06004A41 RID: 19009 RVA: 0x001D5BF8 File Offset: 0x001D3DF8
	public void RemoveEntityBlockStub(Vector3i _pos)
	{
		ulong key = GameUtils.Vector3iToUInt64(_pos);
		BlockEntityData item;
		if (this.blockEntityStubs.dict.TryGetValue(key, out item))
		{
			this.blockEntityStubsToRemove.Add(item);
			this.blockEntityStubs.Remove(key);
			return;
		}
		string str = "Entity block on pos ";
		Vector3i vector3i = _pos;
		Log.Warning(str + vector3i.ToString() + " not found!");
	}

	// Token: 0x06004A42 RID: 19010 RVA: 0x001D5C60 File Offset: 0x001D3E60
	public int EnableEntityBlocks(bool _on, string _name)
	{
		_name = _name.ToLower();
		int num = 0;
		for (int i = 0; i < this.blockEntityStubs.list.Count; i++)
		{
			BlockEntityData blockEntityData = this.blockEntityStubs.list[i];
			if (blockEntityData.transform)
			{
				string text = blockEntityData.transform.name.ToLower();
				if (_name.Length == 0 || text.Contains(_name))
				{
					blockEntityData.transform.gameObject.SetActive(_on);
					num++;
				}
			}
		}
		return num;
	}

	// Token: 0x06004A43 RID: 19011 RVA: 0x001D5CEC File Offset: 0x001D3EEC
	public void AddInsideDevicePosition(int _blockX, int _blockY, int _blockZ, BlockValue _bv)
	{
		Vector3b item = new Vector3b(_blockX, _blockY, _blockZ);
		this.insideDevices.Add(item);
		this.insideDevicesHashSet.Add(item.GetHashCode());
		this.IsInternalBlocksCulled = true;
	}

	// Token: 0x06004A44 RID: 19012 RVA: 0x001D5D30 File Offset: 0x001D3F30
	public int EnableInsideBlockEntities(bool _bOn)
	{
		int num = 0;
		foreach (Vector3b vector3b in this.insideDevices)
		{
			ulong key = GameUtils.Vector3iToUInt64(this.ToWorldPos(vector3b.ToVector3i()));
			BlockEntityData blockEntityData;
			if (this.blockEntityStubs.dict.TryGetValue(key, out blockEntityData) && blockEntityData.bHasTransform)
			{
				blockEntityData.transform.gameObject.SetActive(_bOn);
				num++;
			}
		}
		return num;
	}

	// Token: 0x06004A45 RID: 19013 RVA: 0x001D5DC8 File Offset: 0x001D3FC8
	public void ResetStability()
	{
		this.chnStability.Clear(-1L);
		for (int i = 0; i < 16; i++)
		{
			for (int j = 0; j < 16; j++)
			{
				for (int k = 0; k < 256; k++)
				{
					int blockId = this.GetBlockId(i, k, j);
					if (blockId == 0)
					{
						break;
					}
					if (!Block.list[blockId].StabilitySupport)
					{
						this.chnStability.Set(i, k, j, 1L);
						break;
					}
					this.chnStability.Set(i, k, j, 15L);
				}
			}
		}
	}

	// Token: 0x06004A46 RID: 19014 RVA: 0x001D5E4C File Offset: 0x001D404C
	public void ResetStabilityToBottomMost()
	{
		this.chnStability.Clear(-1L);
		for (int i = 0; i < 16; i++)
		{
			int j = 0;
			IL_96:
			while (j < 16)
			{
				for (int k = 0; k < 256; k++)
				{
					int blockId = this.GetBlockId(j, k, i);
					if (blockId != 0 && Block.list[blockId].StabilitySupport)
					{
						IL_8A:
						while (k < 256)
						{
							int blockId2 = this.GetBlockId(j, k, i);
							if (blockId2 == 0)
							{
								break;
							}
							if (!Block.list[blockId2].StabilitySupport)
							{
								this.chnStability.Set(j, k, i, 1L);
								break;
							}
							this.chnStability.Set(j, k, i, 15L);
							k++;
						}
						j++;
						goto IL_96;
					}
				}
				goto IL_8A;
			}
		}
	}

	// Token: 0x06004A47 RID: 19015 RVA: 0x001D5F04 File Offset: 0x001D4104
	public void RefreshSunlight()
	{
		this.chnLight.SetHalf(false, 15);
		for (int i = 0; i < 16; i++)
		{
			for (int j = 0; j < 16; j++)
			{
				int num = 15;
				bool flag = true;
				int k = 255;
				while (k >= 0)
				{
					int blockId = this.GetBlockId(i, k, j);
					if (!flag)
					{
						goto IL_3F;
					}
					if (blockId != 0)
					{
						flag = false;
						goto IL_3F;
					}
					IL_8D:
					k--;
					continue;
					IL_3F:
					Block block = Block.list[blockId];
					bool flag2 = block.shape.IsTerrain();
					if (!flag2)
					{
						num -= block.lightOpacity;
						if (num <= 0)
						{
							break;
						}
					}
					this.chnLight.Set(i, k, j, (long)((ulong)((byte)num)));
					if (!flag2)
					{
						goto IL_8D;
					}
					num -= block.lightOpacity;
					if (num > 0)
					{
						goto IL_8D;
					}
					break;
				}
				for (k--; k >= 0; k--)
				{
					this.chnLight.Set(i, k, j, 0L);
				}
			}
		}
		this.isModified = true;
	}

	// Token: 0x06004A48 RID: 19016 RVA: 0x001D5FEC File Offset: 0x001D41EC
	public void SetFullSunlight()
	{
		this.chnLight.SetHalf(false, 15);
	}

	// Token: 0x06004A49 RID: 19017 RVA: 0x001D5FFC File Offset: 0x001D41FC
	public void CopyLightsFrom(Chunk _other)
	{
		this.chnLight.CopyFrom(_other.chnLight);
		this.isModified = true;
	}

	// Token: 0x06004A4A RID: 19018 RVA: 0x001D6018 File Offset: 0x001D4218
	public bool CanMobsSpawnAtPos(int _x, int _y, int _z, bool _ignoreCanMobsSpawnOn = false, bool _checkWater = true)
	{
		if (_y < 2 || _y > 251)
		{
			return false;
		}
		if (this.IsTraderArea(_x, _z))
		{
			return false;
		}
		if (_checkWater || !this.IsWater(_x, _y - 1, _z))
		{
			Block block = this.GetBlockNoDamage(_x, _y - 1, _z).Block;
			if (!_ignoreCanMobsSpawnOn && !block.CanMobsSpawnOn)
			{
				return false;
			}
			if (!block.IsCollideMovement)
			{
				return false;
			}
		}
		Block block2 = this.GetBlockNoDamage(_x, _y, _z).Block;
		if (!block2.IsCollideMovement || !block2.shape.IsSolidSpace)
		{
			Block block3 = this.GetBlockNoDamage(_x, _y + 1, _z).Block;
			if ((!block3.IsCollideMovement || !block3.shape.IsSolidSpace) && (!_checkWater || !this.IsWater(_x, _y, _z)))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06004A4B RID: 19019 RVA: 0x001D60E0 File Offset: 0x001D42E0
	public bool CanSleeperSpawnAtPos(int _x, int _y, int _z, bool _checkBelow)
	{
		if (_checkBelow && !this.GetBlockNoDamage(_x, _y - 1, _z).Block.IsCollideMovement)
		{
			return false;
		}
		Block block = this.GetBlockNoDamage(_x, _y, _z).Block;
		return !block.IsCollideMovement && !block.shape.IsSolidSpace;
	}

	// Token: 0x06004A4C RID: 19020 RVA: 0x001D6138 File Offset: 0x001D4338
	public bool CanPlayersSpawnAtPos(int _x, int _y, int _z, bool _allowOnAirPos = false)
	{
		if (_y < 2 || _y > 251)
		{
			return false;
		}
		Block block = this.GetBlockNoDamage(_x, _y - 1, _z).Block;
		if (!block.CanPlayersSpawnOn)
		{
			return false;
		}
		Block block2 = this.GetBlockNoDamage(_x, _y, _z).Block;
		Block block3 = this.GetBlockNoDamage(_x, _y + 1, _z).Block;
		return ((_allowOnAirPos && block.blockID == 0) || block.IsCollideMovement) && (!block2.IsCollideMovement || !block2.shape.IsSolidSpace) && !this.IsWater(_x, _y, _z) && (!block3.IsCollideMovement || !block3.shape.IsSolidSpace);
	}

	// Token: 0x06004A4D RID: 19021 RVA: 0x001D61E4 File Offset: 0x001D43E4
	public bool IsPositionOnTerrain(int _x, int _y, int _z)
	{
		return _y >= 1 && this.GetBlockNoDamage(_x, _y - 1, _z).Block.shape.IsTerrain();
	}

	// Token: 0x06004A4E RID: 19022 RVA: 0x001D6214 File Offset: 0x001D4414
	public bool FindRandomTopSoilPoint(World _world, out int x, out int y, out int z, int numTrys)
	{
		x = 0;
		y = 0;
		z = 0;
		while (numTrys-- > 0)
		{
			x = _world.GetGameRandom().RandomRange(15);
			z = _world.GetGameRandom().RandomRange(15);
			y = (int)this.GetHeight(x, z);
			if (y >= 2 && this.CanMobsSpawnAtPos(x, y, z, false, true))
			{
				x += this.m_X * 16;
				y++;
				z += this.m_Z * 16;
				return true;
			}
		}
		return false;
	}

	// Token: 0x06004A4F RID: 19023 RVA: 0x001D62A0 File Offset: 0x001D44A0
	public bool FindRandomCavePoint(World _world, out int x, out int y, out int z, int numTrys, int relMinY)
	{
		x = 0;
		y = 0;
		z = 0;
		while (numTrys-- > 0)
		{
			x = _world.GetGameRandom().RandomRange(15);
			z = _world.GetGameRandom().RandomRange(15);
			int height = (int)this.GetHeight(x, z);
			y = height;
			while (y > height - relMinY && y > 2)
			{
				if (this.CanMobsSpawnAtPos(x, y, z, false, true))
				{
					x += this.m_X * 16;
					y++;
					z += this.m_Z * 16;
					return true;
				}
				y--;
			}
		}
		return false;
	}

	// Token: 0x06004A50 RID: 19024 RVA: 0x001D6344 File Offset: 0x001D4544
	public bool FindSpawnPointAtXZ(int x, int z, out int y, int _maxLightV, int _darknessV, int startY, int endY, bool _bIgnoreCanMobsSpawnOn = false)
	{
		endY = Utils.FastClamp(endY, 1, 255);
		startY = Utils.FastClamp(startY - 1, 1, 255);
		y = endY;
		while (y > startY)
		{
			if (this.GetLightValue(x, y, z, _darknessV) <= _maxLightV)
			{
				if (this.CanMobsSpawnAtPos(x, y, z, _bIgnoreCanMobsSpawnOn, true))
				{
					y++;
					return true;
				}
				y--;
			}
		}
		return false;
	}

	// Token: 0x06004A51 RID: 19025 RVA: 0x001D63AB File Offset: 0x001D45AB
	public float GetLightBrightness(int x, int y, int z, int _ss)
	{
		return (float)this.GetLightValue(x, y, z, _ss) / 15f;
	}

	// Token: 0x06004A52 RID: 19026 RVA: 0x001D63C0 File Offset: 0x001D45C0
	public int GetLightValue(int x, int y, int z, int _darknessValue)
	{
		int num = (int)this.GetLight(x, y, z, Chunk.LIGHT_TYPE.SUN);
		num -= _darknessValue;
		if (num == 15)
		{
			return num;
		}
		int light = (int)this.GetLight(x, y, z, Chunk.LIGHT_TYPE.BLOCK);
		if (num > light)
		{
			return num;
		}
		return light;
	}

	// Token: 0x06004A53 RID: 19027 RVA: 0x001D63F8 File Offset: 0x001D45F8
	public byte GetLight(int x, int y, int z, Chunk.LIGHT_TYPE type)
	{
		x &= 15;
		z &= 15;
		int @byte = (int)this.chnLight.GetByte(x, y, z);
		if (type == Chunk.LIGHT_TYPE.SUN)
		{
			return (byte)(@byte & 15);
		}
		return (byte)(@byte >> 4);
	}

	// Token: 0x06004A54 RID: 19028 RVA: 0x001D6430 File Offset: 0x001D4630
	public void SetLight(int x, int y, int z, byte intensity, Chunk.LIGHT_TYPE type)
	{
		x &= 15;
		z &= 15;
		int @byte = (int)this.chnLight.GetByte(x, y, z);
		int num = (int)intensity;
		if (type == Chunk.LIGHT_TYPE.SUN)
		{
			num |= (@byte & 240);
		}
		else if (type == Chunk.LIGHT_TYPE.BLOCK)
		{
			num = (num << 4 | (@byte & 15));
		}
		if (num != @byte)
		{
			this.chnLight.Set(x, y, z, (long)((ulong)((byte)num)));
			this.NeedsRegenerationAt = y;
		}
		this.isModified = true;
	}

	// Token: 0x06004A55 RID: 19029 RVA: 0x001D649C File Offset: 0x001D469C
	public void CheckSameLight()
	{
		this.chnLight.CheckSameValue();
	}

	// Token: 0x06004A56 RID: 19030 RVA: 0x001D64A9 File Offset: 0x001D46A9
	public void CheckSameStability()
	{
		this.chnStability.CheckSameValue();
	}

	// Token: 0x06004A57 RID: 19031 RVA: 0x001D64B8 File Offset: 0x001D46B8
	public static bool IsNeighbourChunksDecorated(Chunk[] _neighbours)
	{
		foreach (Chunk chunk in _neighbours)
		{
			if (chunk == null || chunk.NeedsDecoration)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06004A58 RID: 19032 RVA: 0x001D64E8 File Offset: 0x001D46E8
	public static bool IsNeighbourChunksLit(Chunk[] _neighbours)
	{
		foreach (Chunk chunk in _neighbours)
		{
			if (chunk == null || chunk.NeedsLightCalculation)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06004A59 RID: 19033 RVA: 0x001D6517 File Offset: 0x001D4717
	public Vector3i GetWorldPos()
	{
		return new Vector3i(this.m_X << 4, this.m_Y << 8, this.m_Z << 4);
	}

	// Token: 0x06004A5A RID: 19034 RVA: 0x001D6536 File Offset: 0x001D4736
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetBlockWorldPosX(int _x)
	{
		return (this.m_X << 4) + _x;
	}

	// Token: 0x06004A5B RID: 19035 RVA: 0x001D6542 File Offset: 0x001D4742
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetBlockWorldPosZ(int _z)
	{
		return (this.m_Z << 4) + _z;
	}

	// Token: 0x06004A5C RID: 19036 RVA: 0x001D654E File Offset: 0x001D474E
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public byte GetHeight(int _x, int _z)
	{
		return this.m_HeightMap[_x + _z * 16];
	}

	// Token: 0x06004A5D RID: 19037 RVA: 0x001D655D File Offset: 0x001D475D
	public void SetHeight(int _x, int _z, byte _h)
	{
		this.m_HeightMap[_x + _z * 16] = _h;
	}

	// Token: 0x06004A5E RID: 19038 RVA: 0x001D6570 File Offset: 0x001D4770
	public byte GetMaxHeight()
	{
		byte b = 0;
		for (int i = this.m_HeightMap.Length - 1; i >= 0; i--)
		{
			byte b2 = this.m_HeightMap[i];
			if (b2 > b)
			{
				b = b2;
			}
		}
		return b;
	}

	// Token: 0x06004A5F RID: 19039 RVA: 0x001D65A4 File Offset: 0x001D47A4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public byte GetTerrainHeight(int _x, int _z)
	{
		return this.m_TerrainHeight[_x + _z * 16];
	}

	// Token: 0x06004A60 RID: 19040 RVA: 0x001D65B3 File Offset: 0x001D47B3
	public void SetTerrainHeight(int _x, int _z, byte _h)
	{
		this.m_TerrainHeight[_x + _z * 16] = _h;
	}

	// Token: 0x06004A61 RID: 19041 RVA: 0x001D65C4 File Offset: 0x001D47C4
	public byte GetTopMostTerrainHeight()
	{
		byte b = 0;
		for (int i = 0; i < this.m_TerrainHeight.Length; i++)
		{
			if (this.m_TerrainHeight[i] > b)
			{
				b = this.m_TerrainHeight[i];
			}
		}
		return b;
	}

	// Token: 0x06004A62 RID: 19042 RVA: 0x001D65FC File Offset: 0x001D47FC
	public bool IsTopSoil(int _x, int _z)
	{
		int num = (_x + _z * 16) / 8;
		int num2 = (_x + _z * 16) % 8;
		return ((int)this.m_bTopSoilBroken[num] & 1 << num2) == 0;
	}

	// Token: 0x06004A63 RID: 19043 RVA: 0x001D6630 File Offset: 0x001D4830
	public void SetTopSoilBroken(int _x, int _z)
	{
		int num = (_x + _z * 16) / 8;
		int num2 = (_x + _z * 16) % 8;
		int num3 = (int)this.m_bTopSoilBroken[num];
		num3 |= 1 << num2;
		this.m_bTopSoilBroken[num] = (byte)num3;
	}

	// Token: 0x06004A64 RID: 19044 RVA: 0x001D666C File Offset: 0x001D486C
	public BlockValue GetBlock(Vector3i _pos)
	{
		BlockValue result = BlockValue.Air;
		try
		{
			ChunkBlockLayer chunkBlockLayer = this.m_BlockLayers[_pos.y >> 2];
			if (chunkBlockLayer != null)
			{
				result = chunkBlockLayer.GetAt(_pos.x, _pos.y, _pos.z);
			}
		}
		catch (IndexOutOfRangeException)
		{
			Log.Error(string.Concat(new string[]
			{
				"GetBlock failed: _y = ",
				_pos.y.ToString(),
				", len = ",
				this.m_BlockLayers.Length.ToString(),
				" (chunk ",
				this.m_X.ToString(),
				"/",
				this.m_Z.ToString(),
				")"
			}));
			throw;
		}
		result.damage = this.GetDamage(_pos.x, _pos.y, _pos.z);
		return result;
	}

	// Token: 0x06004A65 RID: 19045 RVA: 0x001D6758 File Offset: 0x001D4958
	public BlockValue GetBlock(int _x, int _y, int _z)
	{
		if (this.IsInternalBlocksCulled && this.isInside(_x, _y, _z))
		{
			if (Chunk.bvPOIFiller.isair)
			{
				Chunk.bvPOIFiller = new BlockValue((uint)Block.GetBlockByName(Constants.cPOIFillerBlock, false).blockID);
			}
			return Chunk.bvPOIFiller;
		}
		BlockValue result = BlockValue.Air;
		try
		{
			ChunkBlockLayer chunkBlockLayer = this.m_BlockLayers[_y >> 2];
			if (chunkBlockLayer != null)
			{
				result = chunkBlockLayer.GetAt(_x, _y, _z);
			}
		}
		catch (IndexOutOfRangeException)
		{
			Log.Error(string.Concat(new string[]
			{
				"GetBlock failed: _y = ",
				_y.ToString(),
				", len = ",
				this.m_BlockLayers.Length.ToString(),
				" (chunk ",
				this.m_X.ToString(),
				"/",
				this.m_Z.ToString(),
				")"
			}));
			throw;
		}
		result.damage = this.GetDamage(_x, _y, _z);
		return result;
	}

	// Token: 0x06004A66 RID: 19046 RVA: 0x001D685C File Offset: 0x001D4A5C
	public BlockValue GetBlockNoDamage(int _x, int _y, int _z)
	{
		BlockValue result = BlockValue.Air;
		try
		{
			ChunkBlockLayer chunkBlockLayer = this.m_BlockLayers[_y >> 2];
			if (chunkBlockLayer != null)
			{
				result = chunkBlockLayer.GetAt(_x, _y, _z);
			}
		}
		catch (IndexOutOfRangeException)
		{
			Log.Error(string.Concat(new string[]
			{
				"GetBlockNoDamage failed: _y = ",
				_y.ToString(),
				", len = ",
				this.m_BlockLayers.Length.ToString(),
				" (chunk ",
				this.m_X.ToString(),
				"/",
				this.m_Z.ToString(),
				")"
			}));
			throw;
		}
		return result;
	}

	// Token: 0x06004A67 RID: 19047 RVA: 0x001D6910 File Offset: 0x001D4B10
	public int GetBlockId(int _x, int _y, int _z)
	{
		int result = 0;
		ChunkBlockLayer chunkBlockLayer = this.m_BlockLayers[_y >> 2];
		if (chunkBlockLayer != null)
		{
			result = chunkBlockLayer.GetIdAt(_x, _y, _z);
		}
		return result;
	}

	// Token: 0x06004A68 RID: 19048 RVA: 0x001D6938 File Offset: 0x001D4B38
	public void CopyMeshDataFrom(Chunk _other)
	{
		for (int i = 0; i < this.m_BlockLayers.Length; i++)
		{
			if (_other.m_BlockLayers[i] == null)
			{
				if (this.m_BlockLayers[i] != null)
				{
					MemoryPools.poolCBL.FreeSync(this.m_BlockLayers[i]);
					this.m_BlockLayers[i] = null;
				}
			}
			else
			{
				if (this.m_BlockLayers[i] == null)
				{
					this.m_BlockLayers[i] = MemoryPools.poolCBL.AllocSync(true);
				}
				this.m_BlockLayers[i].CopyFrom(_other.m_BlockLayers[i]);
			}
		}
		this.chnDensity.CopyFrom(_other.chnDensity);
		this.chnDamage.CopyFrom(_other.chnDamage);
	}

	// Token: 0x06004A69 RID: 19049 RVA: 0x001D69DD File Offset: 0x001D4BDD
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public byte GetBiomeId(int _x, int _z)
	{
		return this.m_Biomes[_x + _z * 16];
	}

	// Token: 0x06004A6A RID: 19050 RVA: 0x001D69EC File Offset: 0x001D4BEC
	public void SetBiomeId(int _x, int _z, byte _biomeId)
	{
		this.m_Biomes[_x + _z * 16] = _biomeId;
	}

	// Token: 0x06004A6B RID: 19051 RVA: 0x001D69FC File Offset: 0x001D4BFC
	public void FillBiomeId(byte _biomeId)
	{
		for (int i = 0; i < this.m_Biomes.Length; i++)
		{
			this.m_Biomes[i] = _biomeId;
		}
	}

	// Token: 0x06004A6C RID: 19052 RVA: 0x001D6A25 File Offset: 0x001D4C25
	public BiomeIntensity GetBiomeIntensity(int _x, int _z)
	{
		if (this.m_BiomeIntensities == null)
		{
			return BiomeIntensity.Default;
		}
		return new BiomeIntensity(this.m_BiomeIntensities, (_x + _z * 16) * 6);
	}

	// Token: 0x06004A6D RID: 19053 RVA: 0x001D6A48 File Offset: 0x001D4C48
	public void CalcBiomeIntensity(Chunk[] _neighbours)
	{
		int[] array = new int[50];
		for (int i = 0; i < 16; i++)
		{
			for (int j = 0; j < 16; j++)
			{
				Array.Clear(array, 0, array.Length);
				for (int k = -16; k < 16; k++)
				{
					int num = i + k;
					int num2 = j + k;
					Chunk chunk = this;
					if (num < 0)
					{
						if (num2 < 0)
						{
							chunk = _neighbours[5];
						}
						else if (num2 >= 16)
						{
							chunk = _neighbours[6];
						}
						else
						{
							chunk = _neighbours[1];
						}
					}
					else if (num >= 16)
					{
						if (num2 < 0)
						{
							chunk = _neighbours[3];
						}
						else if (num2 >= 16)
						{
							chunk = _neighbours[4];
						}
						else
						{
							chunk = _neighbours[0];
						}
					}
					else if (num2 >= 16)
					{
						chunk = _neighbours[2];
					}
					else if (num2 < 0)
					{
						chunk = _neighbours[3];
					}
					int biomeId = (int)chunk.GetBiomeId(World.toBlockXZ(num), World.toBlockXZ(num2));
					if (biomeId >= 0 && biomeId < array.Length)
					{
						array[biomeId]++;
					}
				}
				BiomeIntensity.FromArray(array).ToArray(this.m_BiomeIntensities, (i + j * 16) * 6);
			}
		}
	}

	// Token: 0x06004A6E RID: 19054 RVA: 0x001D6B64 File Offset: 0x001D4D64
	public void CalcDominantBiome()
	{
		int[] array = new int[50];
		for (int i = 0; i < this.m_Biomes.Length; i++)
		{
			array[(int)this.m_Biomes[i]]++;
		}
		int num = 0;
		for (int j = 0; j < array.Length; j++)
		{
			if (array[j] > num)
			{
				this.DominantBiome = (byte)j;
				num = array[j];
			}
		}
	}

	// Token: 0x06004A6F RID: 19055 RVA: 0x001D6BC4 File Offset: 0x001D4DC4
	public void ResetBiomeIntensity(BiomeIntensity _v)
	{
		for (int i = 0; i < this.m_BiomeIntensities.Length; i += 6)
		{
			_v.ToArray(this.m_BiomeIntensities, i);
		}
	}

	// Token: 0x06004A70 RID: 19056 RVA: 0x001D6BF2 File Offset: 0x001D4DF2
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public byte GetStability(int _x, int _y, int _z)
	{
		return (byte)this.chnStability.Get(_x, _y, _z);
	}

	// Token: 0x06004A71 RID: 19057 RVA: 0x001D6C03 File Offset: 0x001D4E03
	public void SetStability(int _x, int _y, int _z, byte _v)
	{
		this.chnStability.Set(_x, _y, _z, (long)((ulong)_v));
	}

	// Token: 0x06004A72 RID: 19058 RVA: 0x001D6C16 File Offset: 0x001D4E16
	public void SetDensity(int _x, int _y, int _z, sbyte _density)
	{
		this.chnDensity.Set(_x, _y, _z, (long)((ulong)((byte)_density)));
	}

	// Token: 0x06004A73 RID: 19059 RVA: 0x001D6C2A File Offset: 0x001D4E2A
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sbyte GetDensity(int _x, int _y, int _z)
	{
		return (sbyte)this.chnDensity.Get(_x, _y, _z);
	}

	// Token: 0x06004A74 RID: 19060 RVA: 0x001D6C3B File Offset: 0x001D4E3B
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool HasSameDensityValue(int _y)
	{
		return this.chnDensity.HasSameValue(_y);
	}

	// Token: 0x06004A75 RID: 19061 RVA: 0x001D6C49 File Offset: 0x001D4E49
	public sbyte GetSameDensityValue(int _y)
	{
		if (_y < 0)
		{
			return MarchingCubes.DensityTerrain;
		}
		if (_y >= 256)
		{
			return MarchingCubes.DensityAir;
		}
		return (sbyte)this.chnDensity.GetSameValue(_y);
	}

	// Token: 0x06004A76 RID: 19062 RVA: 0x001D6C70 File Offset: 0x001D4E70
	public void CheckSameDensity()
	{
		this.chnDensity.CheckSameValue();
	}

	// Token: 0x06004A77 RID: 19063 RVA: 0x001D6C80 File Offset: 0x001D4E80
	public bool IsOnlyTerrain(int _y)
	{
		int idx = _y >> 2;
		return this.IsOnlyTerrainLayer(idx);
	}

	// Token: 0x06004A78 RID: 19064 RVA: 0x001D6C98 File Offset: 0x001D4E98
	public bool IsOnlyTerrainLayer(int _idx)
	{
		return _idx < 0 || _idx >= this.m_BlockLayers.Length || (this.m_BlockLayers[_idx] != null && this.m_BlockLayers[_idx].IsOnlyTerrain());
	}

	// Token: 0x06004A79 RID: 19065 RVA: 0x001D6CC4 File Offset: 0x001D4EC4
	public void CheckOnlyTerrain()
	{
		for (int i = 0; i < this.m_BlockLayers.Length; i++)
		{
			ChunkBlockLayer chunkBlockLayer = this.m_BlockLayers[i];
			if (chunkBlockLayer != null)
			{
				chunkBlockLayer.CheckOnlyTerrain();
			}
		}
	}

	// Token: 0x06004A7A RID: 19066 RVA: 0x001D6CF6 File Offset: 0x001D4EF6
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public long GetTextureFull(int _x, int _y, int _z, int channel = 0)
	{
		if (!Chunk.IgnorePaintTextures)
		{
			return this.chnTextures[channel].Get(_x, _y, _z);
		}
		return 0L;
	}

	// Token: 0x06004A7B RID: 19067 RVA: 0x001D6D14 File Offset: 0x001D4F14
	public TextureFullArray GetTextureFullArray(int _x, int _y, int _z, bool applyIgnore = true)
	{
		TextureFullArray result;
		for (int i = 0; i < 1; i++)
		{
			result[i] = ((applyIgnore && Chunk.IgnorePaintTextures) ? 0L : this.chnTextures[i].Get(_x, _y, _z));
		}
		return result;
	}

	// Token: 0x06004A7C RID: 19068 RVA: 0x001D6D55 File Offset: 0x001D4F55
	public void SetTextureFull(int _x, int _y, int _z, long _texturefull, int channel = 0)
	{
		this.chnTextures[channel].Set(_x, _y, _z, _texturefull);
		this.isModified = true;
	}

	// Token: 0x06004A7D RID: 19069 RVA: 0x001D6D74 File Offset: 0x001D4F74
	public TextureFullArray GetSetTextureFullArray(int _x, int _y, int _z, TextureFullArray _texturefullArray)
	{
		TextureFullArray result;
		for (int i = 0; i < 1; i++)
		{
			result[i] = this.chnTextures[i].GetSet(_x, _y, _z, _texturefullArray[i]);
		}
		this.isModified = true;
		return result;
	}

	// Token: 0x06004A7E RID: 19070 RVA: 0x001D6DB5 File Offset: 0x001D4FB5
	public int GetBlockFaceTexture(int _x, int _y, int _z, BlockFace _face, int channel)
	{
		return (int)(this.chnTextures[channel].Get(_x, _y, _z) >> (int)(_face * (BlockFace)8) & 255L);
	}

	// Token: 0x06004A7F RID: 19071 RVA: 0x001D6DD8 File Offset: 0x001D4FD8
	public long SetBlockFaceTexture(int _x, int _y, int _z, BlockFace _face, int _texture, int channel = 0)
	{
		long num;
		long result = num = this.chnTextures[channel].Get(_x, _y, _z);
		int num2 = (int)(_face * (BlockFace)8);
		num &= ~(255L << num2);
		num |= (long)(_texture & 255) << num2;
		this.chnTextures[channel].Set(_x, _y, _z, num);
		this.isModified = true;
		return result;
	}

	// Token: 0x06004A80 RID: 19072 RVA: 0x001D6E36 File Offset: 0x001D5036
	public static int Value64FullToIndex(long _valueFull, BlockFace _blockFace)
	{
		return (int)(_valueFull >> (int)(_blockFace * (BlockFace)8) & 255L);
	}

	// Token: 0x06004A81 RID: 19073 RVA: 0x001D6E48 File Offset: 0x001D5048
	public static long TextureIdxToTextureFullValue64(int _idx)
	{
		long num = (long)_idx;
		return (num & 255L) << 40 | (num & 255L) << 32 | (num & 255L) << 24 | (num & 255L) << 16 | (num & 255L) << 8 | (num & 255L);
	}

	// Token: 0x06004A82 RID: 19074 RVA: 0x001D6E9B File Offset: 0x001D509B
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetDamage(int _x, int _y, int _z, int _damage)
	{
		this.chnDamage.Set(_x, _y, _z, (long)_damage);
	}

	// Token: 0x06004A83 RID: 19075 RVA: 0x001D6EAE File Offset: 0x001D50AE
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetDamage(int _x, int _y, int _z)
	{
		return (int)this.chnDamage.Get(_x, _y, _z);
	}

	// Token: 0x06004A84 RID: 19076 RVA: 0x001D6EC0 File Offset: 0x001D50C0
	public bool IsAir(int _x, int _y, int _z)
	{
		return !this.IsWater(_x, _y, _z) && this.GetBlockNoDamage(_x, _y, _z).isair;
	}

	// Token: 0x06004A85 RID: 19077 RVA: 0x001D6EEB File Offset: 0x001D50EB
	public void ClearWater()
	{
		this.chnWater.Clear(0L);
	}

	// Token: 0x06004A86 RID: 19078 RVA: 0x001D6EFC File Offset: 0x001D50FC
	public bool IsWater(int _x, int _y, int _z)
	{
		return this.GetWater(_x, _y, _z).HasMass();
	}

	// Token: 0x06004A87 RID: 19079 RVA: 0x001D6F1A File Offset: 0x001D511A
	public WaterValue GetWater(int _x, int _y, int _z)
	{
		return WaterValue.FromRawData(this.chnWater.Get(_x, _y, _z));
	}

	// Token: 0x06004A88 RID: 19080 RVA: 0x001D6F2F File Offset: 0x001D512F
	public void SetWater(int _x, int _y, int _z, WaterValue _data)
	{
		this.SetWaterRaw(_x, _y, _z, _data);
		this.waterSimHandle.WakeNeighbours(_x, _y, _z);
	}

	// Token: 0x06004A89 RID: 19081 RVA: 0x001D6F4C File Offset: 0x001D514C
	public void SetWaterRaw(int _x, int _y, int _z, WaterValue _data)
	{
		if (!WaterUtils.CanWaterFlowThrough(this.GetBlockNoDamage(_x, _y, _z)))
		{
			_data.SetMass(0);
		}
		this.chnWater.Set(_x, _y, _z, _data.RawData);
		this.bEmptyDirty = true;
		this.bMapDirty = true;
		this.isModified = true;
		this.waterSimHandle.SetWaterMass(_x, _y, _z, _data.GetMass());
		if (_data.HasMass())
		{
			int num = ChunkBlockLayerLegacy.CalcOffset(_x, _z);
			if ((int)this.m_HeightMap[num] < _y)
			{
				this.m_HeightMap[num] = (byte)_y;
			}
		}
	}

	// Token: 0x06004A8A RID: 19082 RVA: 0x001D6FD8 File Offset: 0x001D51D8
	public void SetWaterSimUpdate(int _x, int _y, int _z, WaterValue _data, out WaterValue _lastData)
	{
		if (!WaterUtils.CanWaterFlowThrough(this.GetBlockNoDamage(_x, _y, _z)))
		{
			_lastData = WaterValue.FromRawData(this.chnWater.Get(_x, _y, _z));
			return;
		}
		long set = this.chnWater.GetSet(_x, _y, _z, _data.RawData);
		_lastData = WaterValue.FromRawData(set);
		if (_lastData.GetMass() == _data.GetMass())
		{
			return;
		}
		GameManager.Instance.World.HandleWaterLevelChanged(this.ToWorldPos(_x, _y, _z), _data.GetMassPercent());
		this.bEmptyDirty = true;
		this.bMapDirty = true;
		this.isModified = true;
		if (_data.HasMass())
		{
			int num = ChunkBlockLayerLegacy.CalcOffset(_x, _z);
			if ((int)this.m_HeightMap[num] < _y)
			{
				this.m_HeightMap[num] = (byte)_y;
			}
		}
	}

	// Token: 0x06004A8B RID: 19083 RVA: 0x001D70A0 File Offset: 0x001D52A0
	public bool IsEmpty()
	{
		if (this.bEmptyDirty)
		{
			this.bEmpty = true;
			for (int i = 0; i < this.m_BlockLayers.Length; i++)
			{
				if (this.m_BlockLayers[i] != null)
				{
					this.bEmpty = false;
					break;
				}
			}
			if (this.bEmpty)
			{
				this.bEmpty = this.chnWater.IsDefault();
			}
			this.bEmptyDirty = false;
		}
		return this.bEmpty;
	}

	// Token: 0x06004A8C RID: 19084 RVA: 0x001D7108 File Offset: 0x001D5308
	public bool IsEmpty(int _y)
	{
		int idx = _y >> 2;
		return this.IsEmptyLayer(idx);
	}

	// Token: 0x06004A8D RID: 19085 RVA: 0x001D7120 File Offset: 0x001D5320
	public bool IsEmptyLayer(int _idx)
	{
		return (ulong)_idx >= (ulong)((long)this.m_BlockLayers.Length) || (this.m_BlockLayers[_idx] == null && this.chnWater.IsDefaultLayer(_idx));
	}

	// Token: 0x06004A8E RID: 19086 RVA: 0x001D714C File Offset: 0x001D534C
	public int RecalcHeights()
	{
		int num = 0;
		for (int i = 0; i < 16; i++)
		{
			for (int j = 0; j < 16; j++)
			{
				int num2 = ChunkBlockLayerLegacy.CalcOffset(j, i);
				this.m_HeightMap[num2] = 0;
				for (int k = 255; k >= 0; k--)
				{
					ChunkBlockLayer chunkBlockLayer = this.m_BlockLayers[k >> 2];
					if ((chunkBlockLayer != null && !chunkBlockLayer.GetAt(j, k, i).isair) || this.IsWater(j, k, i))
					{
						this.m_HeightMap[num2] = (byte)k;
						num = Utils.FastMax(num, k);
						break;
					}
				}
			}
		}
		return num;
	}

	// Token: 0x06004A8F RID: 19087 RVA: 0x001D71F4 File Offset: 0x001D53F4
	public byte RecalcHeightAt(int _x, int _yMaxStart, int _z)
	{
		int num = ChunkBlockLayerLegacy.CalcOffset(_x, _z);
		for (int i = _yMaxStart; i >= 0; i--)
		{
			ChunkBlockLayer chunkBlockLayer = this.m_BlockLayers[i >> 2];
			if ((chunkBlockLayer != null && !chunkBlockLayer.GetAt(_x, i, _z).isair) || this.IsWater(_x, i, _z))
			{
				this.m_HeightMap[num] = (byte)i;
				return (byte)i;
			}
		}
		return 0;
	}

	// Token: 0x06004A90 RID: 19088 RVA: 0x001D7258 File Offset: 0x001D5458
	public BlockValue SetBlock(WorldBase _world, int x, int y, int z, BlockValue _blockValue, bool _notifyAddChange = true, bool _notifyRemove = true, bool _fromReset = false, bool _poiOwned = false, int _changedByEntityId = -1)
	{
		Vector3i vector3i = new Vector3i((this.m_X << 4) + x, y, (this.m_Z << 4) + z);
		BlockValue blockValue = this.SetBlockRaw(x, y, z, _blockValue);
		bool flag = !blockValue.isair && _blockValue.isair;
		if (flag)
		{
			this.waterSimHandle.WakeNeighbours(x, y, z);
			if (blockValue.Block.StabilitySupport)
			{
				MultiBlockManager.Instance.SetOversizedStabilityDirty(vector3i);
			}
		}
		if (!_blockValue.ischild)
		{
			MultiBlockManager.Instance.UpdateTrackedBlockData(vector3i, _blockValue, _poiOwned);
		}
		_blockValue = this.GetBlock(x, y, z);
		if (_notifyRemove && !blockValue.isair && blockValue.type != _blockValue.type)
		{
			Block block = blockValue.Block;
			if (block != null)
			{
				block.OnBlockRemoved(_world, this, vector3i, blockValue);
			}
		}
		if (_notifyAddChange)
		{
			Block block2 = _blockValue.Block;
			if (block2 != null)
			{
				if (blockValue.type != _blockValue.type)
				{
					if (!_blockValue.isair)
					{
						PlatformUserIdentifierAbs addedByPlayer = null;
						if (_changedByEntityId != -1)
						{
							addedByPlayer = GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(_changedByEntityId).PrimaryId;
						}
						block2.OnBlockAdded(_world, this, vector3i, _blockValue, addedByPlayer);
					}
				}
				else
				{
					block2.OnBlockValueChanged(_world, this, 0, vector3i, blockValue, _blockValue);
					if (_fromReset)
					{
						block2.OnBlockReset(_world, this, vector3i, _blockValue);
					}
				}
			}
		}
		if (flag)
		{
			this.RemoveBlockTrigger(new Vector3i(x, y, z));
			GameEventManager.Current.BlockRemoved(vector3i);
		}
		if ((DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent() && SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && !GameManager.Instance.IsEditMode() && BlockLimitTracker.instance != null && !blockValue.Equals(_blockValue))
		{
			BlockLimitTracker.instance.TryRemoveOrReplaceBlock(blockValue, _blockValue, vector3i);
			if (!flag)
			{
				BlockLimitTracker.instance.TryAddTrackedBlock(_blockValue, vector3i, _changedByEntityId);
			}
			BlockLimitTracker.instance.ServerUpdateClients();
		}
		return blockValue;
	}

	// Token: 0x06004A91 RID: 19089 RVA: 0x001D7418 File Offset: 0x001D5618
	public BlockValue SetBlockRaw(int _x, int _y, int _z, BlockValue _blockValue)
	{
		if (_y >= 255)
		{
			return BlockValue.Air;
		}
		Block block = _blockValue.Block;
		if (block == null)
		{
			return BlockValue.Air;
		}
		if (_blockValue.isWater)
		{
			ChunkBlockLayer chunkBlockLayer = this.m_BlockLayers[_y >> 2];
			BlockValue blockValue = (chunkBlockLayer != null) ? chunkBlockLayer.GetAt(_x, _y, _z) : BlockValue.Air;
			if (!WaterUtils.CanWaterFlowThrough(blockValue))
			{
				this.SetBlockRaw(_x, _y, _z, BlockValue.Air);
			}
			this.SetWater(_x, _y, _z, WaterValue.Full);
			return blockValue;
		}
		if (!WaterUtils.CanWaterFlowThrough(_blockValue))
		{
			this.SetWater(_x, _y, _z, WaterValue.Empty);
		}
		this.waterSimHandle.SetVoxelSolid(_x, _y, _z, BlockFaceFlags.RotateFlags(block.WaterFlowMask, _blockValue.rotation));
		BlockValue result = BlockValue.Air;
		int num = _y >> 2;
		ChunkBlockLayer chunkBlockLayer2 = this.m_BlockLayers[num];
		if (chunkBlockLayer2 != null)
		{
			int offs = ChunkBlockLayer.CalcOffset(_x, _y, _z);
			result = chunkBlockLayer2.GetAt(offs);
			chunkBlockLayer2.SetAt(offs, _blockValue.rawData);
			if (!result.ischild)
			{
				result.damage = this.GetDamage(_x, _y, _z);
			}
		}
		else if (!_blockValue.isair)
		{
			chunkBlockLayer2 = MemoryPools.poolCBL.AllocSync(true);
			this.m_BlockLayers[num] = chunkBlockLayer2;
			chunkBlockLayer2.SetAt(_x, _y, _z, _blockValue.rawData);
		}
		if (!_blockValue.ischild)
		{
			this.SetDamage(_x, _y, _z, _blockValue.damage);
		}
		Block block2 = result.Block;
		if (result.type != _blockValue.type)
		{
			if (!result.ischild && block2.IndexName != null && this.IndexedBlocks.ContainsKey(block2.IndexName))
			{
				this.IndexedBlocks[block2.IndexName].Remove(new Vector3i(_x, _y, _z));
				if (this.IndexedBlocks[block2.IndexName].Count == 0)
				{
					this.IndexedBlocks.Remove(block2.IndexName);
				}
			}
			if (!_blockValue.ischild && block.IndexName != null && block.FilterIndexType(_blockValue))
			{
				if (!this.IndexedBlocks.ContainsKey(block.IndexName))
				{
					this.IndexedBlocks[block.IndexName] = new List<Vector3i>();
				}
				this.IndexedBlocks[block.IndexName].Add(new Vector3i(_x, _y, _z));
			}
		}
		int num2 = ChunkBlockLayerLegacy.CalcOffset(_x, _z);
		if (_blockValue.isair)
		{
			if ((int)this.m_HeightMap[num2] == _y)
			{
				this.RecalcHeightAt(_x, _y - 1, _z);
			}
		}
		else if ((int)this.m_HeightMap[num2] < _y)
		{
			this.m_HeightMap[num2] = (byte)_y;
		}
		if (result.isair && !_blockValue.isair && !_blockValue.ischild)
		{
			if (!block.IsRandomlyTick)
			{
				goto IL_3CF;
			}
			DictionaryKeyList<Vector3i, int> obj = this.tickedBlocks;
			lock (obj)
			{
				this.tickedBlocks.Replace(this.ToWorldPos(_x, _y, _z), 0);
				goto IL_3CF;
			}
		}
		if (!result.isair && _blockValue.isair && !result.ischild)
		{
			if (!block2.IsRandomlyTick)
			{
				goto IL_3CF;
			}
			DictionaryKeyList<Vector3i, int> obj = this.tickedBlocks;
			lock (obj)
			{
				this.tickedBlocks.Remove(this.ToWorldPos(_x, _y, _z));
				goto IL_3CF;
			}
		}
		if (block2.IsRandomlyTick && !block.IsRandomlyTick && !result.ischild)
		{
			DictionaryKeyList<Vector3i, int> obj = this.tickedBlocks;
			lock (obj)
			{
				this.tickedBlocks.Remove(this.ToWorldPos(_x, _y, _z));
				goto IL_3CF;
			}
		}
		if (!block2.IsRandomlyTick && block.IsRandomlyTick && !_blockValue.ischild)
		{
			DictionaryKeyList<Vector3i, int> obj = this.tickedBlocks;
			lock (obj)
			{
				this.tickedBlocks.Replace(this.ToWorldPos(_x, _y, _z), 0);
			}
		}
		IL_3CF:
		this.bMapDirty = true;
		this.isModified = true;
		this.bEmptyDirty = true;
		return result;
	}

	// Token: 0x06004A92 RID: 19090 RVA: 0x001D7840 File Offset: 0x001D5A40
	public bool FillBlockRaw(int _heightIncl, BlockValue _blockValue)
	{
		if (_heightIncl >= 255)
		{
			return false;
		}
		if (_blockValue.isair || _blockValue.ischild)
		{
			return false;
		}
		Block block = _blockValue.Block;
		if (block == null)
		{
			return false;
		}
		if (_blockValue.isWater)
		{
			return false;
		}
		if (!this.IsEmpty())
		{
			return false;
		}
		uint rawData = _blockValue.rawData;
		sbyte density = block.shape.IsTerrain() ? MarchingCubes.DensityTerrain : MarchingCubes.DensityAir;
		int damage = _blockValue.damage;
		int i;
		for (i = 0; i <= _heightIncl - 4; i += 4)
		{
			int num = i >> 2;
			if (this.m_BlockLayers[num] == null)
			{
				this.m_BlockLayers[num] = MemoryPools.poolCBL.AllocSync(true);
			}
			this.m_BlockLayers[num].Fill(rawData);
		}
		while (i <= _heightIncl)
		{
			for (int j = 0; j < 16; j++)
			{
				for (int k = 0; k < 16; k++)
				{
					int num2 = i >> 2;
					if (this.m_BlockLayers[num2] == null)
					{
						this.m_BlockLayers[num2] = MemoryPools.poolCBL.AllocSync(true);
					}
					this.m_BlockLayers[num2].SetAt(j, i, k, rawData);
				}
			}
			i++;
		}
		List<Vector3i> list = null;
		if (block.IndexName != null)
		{
			if (!this.IndexedBlocks.ContainsKey(block.IndexName))
			{
				this.IndexedBlocks[block.IndexName] = new List<Vector3i>();
			}
			list = this.IndexedBlocks[block.IndexName];
			list.Clear();
		}
		DictionaryKeyList<Vector3i, int> obj = this.tickedBlocks;
		lock (obj)
		{
			this.tickedBlocks.Clear();
			for (i = 0; i <= _heightIncl; i++)
			{
				for (int l = 0; l < 16; l++)
				{
					for (int m = 0; m < 16; m++)
					{
						this.SetDensity(l, i, m, density);
						this.SetDamage(l, i, m, damage);
						if (list != null)
						{
							list.Add(new Vector3i(l, i, m));
						}
						if (block.IsRandomlyTick)
						{
							this.tickedBlocks.Replace(this.ToWorldPos(l, i, m), 0);
						}
					}
				}
			}
		}
		for (int n = 0; n < 16; n++)
		{
			for (int num3 = 0; num3 < 16; num3++)
			{
				int num4 = ChunkBlockLayerLegacy.CalcOffset(n, num3);
				this.m_HeightMap[num4] = (byte)_heightIncl;
			}
		}
		this.bMapDirty = true;
		this.isModified = true;
		this.bEmptyDirty = true;
		return true;
	}

	// Token: 0x06004A93 RID: 19091 RVA: 0x001D7AC0 File Offset: 0x001D5CC0
	public DictionaryKeyList<Vector3i, int> GetTickedBlocks()
	{
		return this.tickedBlocks;
	}

	// Token: 0x06004A94 RID: 19092 RVA: 0x001D7AC8 File Offset: 0x001D5CC8
	public void RemoveTileEntityAt<T>(World world, Vector3i _posInChunk)
	{
		TileEntity tileEntity;
		if (this.tileEntities.dict.TryGetValue(_posInChunk, out tileEntity) && tileEntity is T)
		{
			tileEntity.IsRemoving = true;
			tileEntity.OnRemove(world);
			this.tileEntities.Remove(_posInChunk);
			tileEntity.IsRemoving = false;
		}
		this.isModified = true;
	}

	// Token: 0x06004A95 RID: 19093 RVA: 0x001D7B1B File Offset: 0x001D5D1B
	public void RemoveAllTileEntities()
	{
		this.isModified = (this.tileEntities.Count > 0);
		this.tileEntities.Clear();
	}

	// Token: 0x06004A96 RID: 19094 RVA: 0x001D7B3C File Offset: 0x001D5D3C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public byte GetHeight(int _blockOffset)
	{
		return this.m_HeightMap[_blockOffset];
	}

	// Token: 0x06004A97 RID: 19095 RVA: 0x001D7B46 File Offset: 0x001D5D46
	public void AddTileEntity(TileEntity _te)
	{
		this.tileEntities.Set(_te.localChunkPos, _te);
	}

	// Token: 0x06004A98 RID: 19096 RVA: 0x001D7B5C File Offset: 0x001D5D5C
	public void RemoveTileEntity(World world, TileEntity _te)
	{
		TileEntity tileEntity;
		if (this.tileEntities.dict.TryGetValue(_te.localChunkPos, out tileEntity) && tileEntity != null)
		{
			tileEntity.IsRemoving = true;
			tileEntity.OnRemove(world);
			this.tileEntities.Remove(_te.localChunkPos);
			tileEntity.IsRemoving = false;
			this.isModified = true;
		}
	}

	// Token: 0x06004A99 RID: 19097 RVA: 0x001D7BB4 File Offset: 0x001D5DB4
	public TileEntity GetTileEntity(Vector3i _blockPosInChunk)
	{
		TileEntity result;
		if (!this.tileEntities.dict.TryGetValue(_blockPosInChunk, out result))
		{
			return null;
		}
		return result;
	}

	// Token: 0x06004A9A RID: 19098 RVA: 0x001D7BD9 File Offset: 0x001D5DD9
	public DictionaryList<Vector3i, TileEntity> GetTileEntities()
	{
		return this.tileEntities;
	}

	// Token: 0x06004A9B RID: 19099 RVA: 0x001D7BE1 File Offset: 0x001D5DE1
	public void AddSleeperVolumeId(int id)
	{
		if (!this.sleeperVolumes.Contains(id))
		{
			if (this.sleeperVolumes.Count < 255)
			{
				this.sleeperVolumes.Add(id);
				return;
			}
			Log.Error("Chunk AddSleeperVolumeId at max");
		}
	}

	// Token: 0x06004A9C RID: 19100 RVA: 0x001D7C1A File Offset: 0x001D5E1A
	public List<int> GetSleeperVolumes()
	{
		return this.sleeperVolumes;
	}

	// Token: 0x06004A9D RID: 19101 RVA: 0x001D7C22 File Offset: 0x001D5E22
	public void AddTriggerVolumeId(int id)
	{
		if (!this.triggerVolumes.Contains(id))
		{
			if (this.triggerVolumes.Count < 255)
			{
				this.triggerVolumes.Add(id);
				return;
			}
			Log.Error("Chunk AddTriggerVolumeId at max");
		}
	}

	// Token: 0x06004A9E RID: 19102 RVA: 0x001D7C5B File Offset: 0x001D5E5B
	public List<int> GetTriggerVolumes()
	{
		return this.triggerVolumes;
	}

	// Token: 0x06004A9F RID: 19103 RVA: 0x001D7C63 File Offset: 0x001D5E63
	public void AddWallVolumeId(int id)
	{
		if (!this.wallVolumes.Contains(id))
		{
			if (this.wallVolumes.Count < 255)
			{
				this.wallVolumes.Add(id);
				return;
			}
			Log.Error("Chunk AddWallVolume at max");
		}
	}

	// Token: 0x06004AA0 RID: 19104 RVA: 0x001D7C9C File Offset: 0x001D5E9C
	public List<int> GetWallVolumes()
	{
		return this.wallVolumes;
	}

	// Token: 0x06004AA1 RID: 19105 RVA: 0x001D7CA4 File Offset: 0x001D5EA4
	public int GetTickRefCount(int _layerIdx)
	{
		if (this.m_BlockLayers[_layerIdx] == null)
		{
			return 0;
		}
		return this.m_BlockLayers[_layerIdx].GetTickRefCount();
	}

	// Token: 0x06004AA2 RID: 19106 RVA: 0x001D7CBF File Offset: 0x001D5EBF
	public DictionaryList<Vector3i, BlockTrigger> GetBlockTriggers()
	{
		return this.triggerData;
	}

	// Token: 0x06004AA3 RID: 19107 RVA: 0x001D7CC7 File Offset: 0x001D5EC7
	public void AddBlockTrigger(BlockTrigger _td)
	{
		this.triggerData.Set(_td.LocalChunkPos, _td);
		this.isModified = true;
	}

	// Token: 0x06004AA4 RID: 19108 RVA: 0x001D7CE4 File Offset: 0x001D5EE4
	public void RemoveBlockTrigger(BlockTrigger _td)
	{
		BlockTrigger blockTrigger;
		if (this.triggerData.dict.TryGetValue(_td.LocalChunkPos, out blockTrigger) && blockTrigger != null)
		{
			this.triggerData.Remove(_td.LocalChunkPos);
			this.isModified = true;
		}
	}

	// Token: 0x06004AA5 RID: 19109 RVA: 0x001D7D27 File Offset: 0x001D5F27
	public void RemoveBlockTrigger(Vector3i _blockPos)
	{
		if (this.triggerData.dict.ContainsKey(_blockPos))
		{
			this.triggerData.Remove(_blockPos);
			this.isModified = true;
		}
	}

	// Token: 0x06004AA6 RID: 19110 RVA: 0x001D7D50 File Offset: 0x001D5F50
	public BlockTrigger GetBlockTrigger(Vector3i _blockPosInChunk)
	{
		BlockTrigger result;
		this.triggerData.dict.TryGetValue(_blockPosInChunk, out result);
		return result;
	}

	// Token: 0x06004AA7 RID: 19111 RVA: 0x001D7D74 File Offset: 0x001D5F74
	public void UpdateTick(World _world, bool _bSpawnEnemies)
	{
		this.ProfilerBegin("TeTick");
		for (int i = 0; i < this.tileEntities.list.Count; i++)
		{
			this.tileEntities.list[i].UpdateTick(_world);
		}
		this.ProfilerEnd();
	}

	// Token: 0x170007D5 RID: 2005
	// (get) Token: 0x06004AA8 RID: 19112 RVA: 0x001D7DC4 File Offset: 0x001D5FC4
	public bool NeedsTicking
	{
		get
		{
			return this.tileEntities.Count > 0 || this.sleeperVolumes.Count > 0;
		}
	}

	// Token: 0x06004AA9 RID: 19113 RVA: 0x001D7DE4 File Offset: 0x001D5FE4
	public bool IsOpenSkyAbove(int _x, int _y, int _z)
	{
		return _y >= (int)this.GetHeight(_x, _z);
	}

	// Token: 0x06004AAA RID: 19114 RVA: 0x001D7DF4 File Offset: 0x001D5FF4
	public void GetLivingEntitiesInBounds(EntityAlive _excludeEntity, Bounds _aabb, List<EntityAlive> _entityOutputList)
	{
		int num = Utils.Fastfloor((double)(_aabb.min.y - 5f) / 16.0);
		int num2 = Utils.Fastfloor((double)(_aabb.max.y + 5f) / 16.0);
		if (num < 0)
		{
			num = 0;
		}
		if (num2 >= 16)
		{
			num2 = 15;
		}
		for (int i = num; i <= num2; i++)
		{
			List<Entity> list = this.entityLists[i];
			for (int j = 0; j < list.Count; j++)
			{
				EntityAlive entityAlive = list[j] as EntityAlive;
				if (!(entityAlive == null) && !(entityAlive == _excludeEntity) && !entityAlive.IsDead() && entityAlive.boundingBox.Intersects(_aabb) && (!(_excludeEntity != null) || _excludeEntity.CanCollideWith(entityAlive)))
				{
					_entityOutputList.Add(entityAlive);
				}
			}
		}
	}

	// Token: 0x06004AAB RID: 19115 RVA: 0x001D7ED8 File Offset: 0x001D60D8
	public void GetEntitiesInBounds(Entity _excludeEntity, Bounds _aabb, List<Entity> _entityOutputList, bool isAlive)
	{
		int num = Utils.Fastfloor((double)(_aabb.min.y - 5f) / 16.0);
		int num2 = Utils.Fastfloor((double)(_aabb.max.y + 5f) / 16.0);
		if (num < 0)
		{
			num = 0;
		}
		if (num2 >= 16)
		{
			num2 = 15;
		}
		for (int i = num; i <= num2; i++)
		{
			List<Entity> list = this.entityLists[i];
			for (int j = 0; j < list.Count; j++)
			{
				Entity entity = list[j];
				if (!(entity == _excludeEntity) && isAlive == entity.IsAlive() && entity.boundingBox.Intersects(_aabb) && (!(_excludeEntity != null) || _excludeEntity.CanCollideWith(entity)))
				{
					_entityOutputList.Add(entity);
				}
			}
		}
	}

	// Token: 0x06004AAC RID: 19116 RVA: 0x001D7FAC File Offset: 0x001D61AC
	public void GetEntitiesInBounds(FastTags<TagGroup.Global> _tags, Bounds _bb, List<Entity> _list)
	{
		int num = Utils.Fastfloor((double)(_bb.min.y - 5f) / 16.0);
		int num2 = Utils.Fastfloor((double)(_bb.max.y + 5f) / 16.0);
		if (num < 0)
		{
			num = 0;
		}
		else if (num >= 16)
		{
			num = 15;
		}
		if (num2 >= 16)
		{
			num2 = 15;
		}
		else if (num2 < 0)
		{
			num2 = 0;
		}
		for (int i = num; i <= num2; i++)
		{
			List<Entity> list = this.entityLists[i];
			for (int j = 0; j < list.Count; j++)
			{
				Entity entity = list[j];
				if (entity.HasAnyTags(_tags) && entity.boundingBox.Intersects(_bb))
				{
					_list.Add(entity);
				}
			}
		}
	}

	// Token: 0x06004AAD RID: 19117 RVA: 0x001D8074 File Offset: 0x001D6274
	public void GetEntitiesInBounds(Type _class, Bounds _bb, List<Entity> _list)
	{
		int num = Utils.Fastfloor((double)(_bb.min.y - 5f) / 16.0);
		int num2 = Utils.Fastfloor((double)(_bb.max.y + 5f) / 16.0);
		if (num < 0)
		{
			num = 0;
		}
		else if (num >= 16)
		{
			num = 15;
		}
		if (num2 >= 16)
		{
			num2 = 15;
		}
		else if (num2 < 0)
		{
			num2 = 0;
		}
		for (int i = num; i <= num2; i++)
		{
			List<Entity> list = this.entityLists[i];
			for (int j = 0; j < list.Count; j++)
			{
				Entity entity = list[j];
				if (_class.IsAssignableFrom(entity.GetType()) && entity.boundingBox.Intersects(_bb))
				{
					_list.Add(entity);
				}
			}
		}
	}

	// Token: 0x06004AAE RID: 19118 RVA: 0x001D8144 File Offset: 0x001D6344
	public void GetEntitiesAround(EntityFlags _mask, Vector3 _pos, float _radius, List<Entity> _list)
	{
		int num = Utils.Fastfloor((double)(_pos.y - _radius) / 16.0);
		int num2 = Utils.Fastfloor((double)(_pos.y + _radius) / 16.0);
		if (num < 0)
		{
			num = 0;
		}
		else if (num >= 16)
		{
			num = 15;
		}
		if (num2 >= 16)
		{
			num2 = 15;
		}
		else if (num2 < 0)
		{
			num2 = 0;
		}
		float num3 = _radius * _radius;
		for (int i = num; i <= num2; i++)
		{
			List<Entity> list = this.entityLists[i];
			for (int j = 0; j < list.Count; j++)
			{
				Entity entity = list[j];
				if ((entity.entityFlags & _mask) != EntityFlags.None && (entity.position - _pos).sqrMagnitude <= num3)
				{
					_list.Add(entity);
				}
			}
		}
	}

	// Token: 0x06004AAF RID: 19119 RVA: 0x001D820C File Offset: 0x001D640C
	public void GetEntitiesAround(EntityFlags _flags, EntityFlags _mask, Vector3 _pos, float _radius, List<Entity> _list)
	{
		int num = Utils.Fastfloor((double)(_pos.y - _radius) / 16.0);
		int num2 = Utils.Fastfloor((double)(_pos.y + _radius) / 16.0);
		if (num < 0)
		{
			num = 0;
		}
		else if (num >= 16)
		{
			num = 15;
		}
		if (num2 >= 16)
		{
			num2 = 15;
		}
		else if (num2 < 0)
		{
			num2 = 0;
		}
		float num3 = _radius * _radius;
		for (int i = num; i <= num2; i++)
		{
			List<Entity> list = this.entityLists[i];
			for (int j = 0; j < list.Count; j++)
			{
				Entity entity = list[j];
				if ((entity.entityFlags & _mask) == _flags && (entity.position - _pos).sqrMagnitude <= num3)
				{
					_list.Add(entity);
				}
			}
		}
	}

	// Token: 0x06004AB0 RID: 19120 RVA: 0x001D82D8 File Offset: 0x001D64D8
	public void RemoveEntityFromChunk(Entity _entity)
	{
		int y = _entity.chunkPosAddedEntityTo.y;
		this.entityLists[y].Remove(_entity);
		this.isModified = true;
		bool flag = false;
		for (int i = 0; i < 16; i++)
		{
			if (this.entityLists[i].Count > 0)
			{
				flag = true;
				break;
			}
		}
		this.hasEntities = flag;
	}

	// Token: 0x06004AB1 RID: 19121 RVA: 0x001D8334 File Offset: 0x001D6534
	public void AddEntityToChunk(Entity _entity)
	{
		this.hasEntities = true;
		int num = World.toChunkXZ(Utils.Fastfloor(_entity.position.x));
		int num2 = World.toChunkXZ(Utils.Fastfloor(_entity.position.z));
		if (num != this.m_X || num2 != this.m_Z)
		{
			Log.Error(string.Concat(new string[]
			{
				"Wrong entity chunk position! ",
				(_entity != null) ? _entity.ToString() : null,
				" x=",
				num.ToString(),
				" z=",
				num2.ToString(),
				"/",
				(this != null) ? this.ToString() : null
			}));
		}
		int num3 = Utils.Fastfloor((double)_entity.position.y / 16.0);
		if (num3 < 0)
		{
			num3 = 0;
		}
		if (num3 >= 16)
		{
			num3 = 15;
		}
		_entity.addedToChunk = true;
		_entity.chunkPosAddedEntityTo.x = this.m_X;
		_entity.chunkPosAddedEntityTo.y = num3;
		_entity.chunkPosAddedEntityTo.z = this.m_Z;
		this.entityLists[num3].Add(_entity);
	}

	// Token: 0x06004AB2 RID: 19122 RVA: 0x001D845C File Offset: 0x001D665C
	public void AdJustEntityTracking(Entity _entity)
	{
		if (!_entity.addedToChunk)
		{
			return;
		}
		int num = Utils.Fastfloor((double)_entity.position.y / 16.0);
		if (num < 0)
		{
			num = 0;
		}
		if (num >= 16)
		{
			num = 15;
		}
		if (_entity.chunkPosAddedEntityTo.y != num)
		{
			this.entityLists[_entity.chunkPosAddedEntityTo.y].Remove(_entity);
			_entity.chunkPosAddedEntityTo.y = num;
			this.entityLists[num].Add(_entity);
			this.isModified = true;
		}
	}

	// Token: 0x06004AB3 RID: 19123 RVA: 0x001D84E4 File Offset: 0x001D66E4
	public Bounds GetAABB()
	{
		return this.boundingBox;
	}

	// Token: 0x06004AB4 RID: 19124 RVA: 0x001D84EC File Offset: 0x001D66EC
	public static Bounds CalculateAABB(int _chunkX, int _chunkY, int _chunkZ)
	{
		return BoundsUtils.BoundsForMinMax((float)(_chunkX * 16), (float)(_chunkY * 256), (float)(_chunkZ * 16), (float)(_chunkX * 16 + 16), (float)(_chunkY * 256 + 256), (float)(_chunkZ * 16 + 16));
	}

	// Token: 0x06004AB5 RID: 19125 RVA: 0x001D8524 File Offset: 0x001D6724
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateBounds()
	{
		this.boundingBox = Chunk.CalculateAABB(this.m_X, this.m_Y, this.m_Z);
		this.worldPosIMin.x = this.m_X << 4;
		this.worldPosIMin.y = this.m_Y << 8;
		this.worldPosIMin.z = this.m_Z << 4;
		this.worldPosIMax.x = this.worldPosIMin.x + 15;
		this.worldPosIMax.y = this.worldPosIMin.y + 255;
		this.worldPosIMax.z = this.worldPosIMin.z + 15;
	}

	// Token: 0x06004AB6 RID: 19126 RVA: 0x001D85D5 File Offset: 0x001D67D5
	public int GetTris()
	{
		return this.totalTris;
	}

	// Token: 0x06004AB7 RID: 19127 RVA: 0x001D85E0 File Offset: 0x001D67E0
	public int GetTrisInMesh(int _idx)
	{
		int num = 0;
		for (int i = 0; i < this.trisInMesh.GetLength(0); i++)
		{
			num += this.trisInMesh[i][_idx];
		}
		return num;
	}

	// Token: 0x06004AB8 RID: 19128 RVA: 0x001D8614 File Offset: 0x001D6814
	public int GetSizeOfMesh(int _idx)
	{
		int num = 0;
		for (int i = 0; i < this.trisInMesh.GetLength(0); i++)
		{
			num += this.sizeOfMesh[i][_idx];
		}
		return num;
	}

	// Token: 0x06004AB9 RID: 19129 RVA: 0x001D8648 File Offset: 0x001D6848
	public int GetUsedMem()
	{
		this.TotalMemory = 0;
		for (int i = 0; i < this.m_BlockLayers.Length; i++)
		{
			this.TotalMemory += ((this.m_BlockLayers[i] != null) ? this.m_BlockLayers[i].GetUsedMem() : 0);
		}
		this.TotalMemory += 12;
		this.TotalMemory += this.m_TerrainHeight.Length;
		this.TotalMemory += this.m_HeightMap.Length;
		this.TotalMemory += this.m_Biomes.Length;
		this.TotalMemory += this.m_BiomeIntensities.Length;
		this.TotalMemory += this.m_NormalX.Length;
		this.TotalMemory += this.m_NormalY.Length;
		this.TotalMemory += this.m_NormalZ.Length;
		this.TotalMemory += this.chnStability.GetUsedMem();
		this.TotalMemory += this.chnLight.GetUsedMem();
		this.TotalMemory += this.chnDensity.GetUsedMem();
		this.TotalMemory += this.chnDamage.GetUsedMem();
		for (int j = 0; j < 1; j++)
		{
			this.TotalMemory += this.chnTextures[j].GetUsedMem();
		}
		this.TotalMemory += this.chnWater.GetUsedMem();
		return this.TotalMemory;
	}

	// Token: 0x06004ABA RID: 19130 RVA: 0x001D87DC File Offset: 0x001D69DC
	public void GetTextureChannelMemory(out int[] texMem)
	{
		texMem = new int[1];
		for (int i = 0; i < 1; i++)
		{
			texMem[i] = this.chnTextures[i].GetUsedMem();
		}
	}

	// Token: 0x06004ABB RID: 19131 RVA: 0x001D8810 File Offset: 0x001D6A10
	public void OnLoadedFromCache()
	{
		this.NeedsRegeneration = true;
		this.isModified = true;
		this.InProgressRegeneration = false;
		this.InProgressSaving = false;
		this.InProgressCopying = false;
		this.InProgressDecorating = false;
		this.InProgressLighting = false;
		this.InProgressUnloading = false;
		this.NeedsOnlyCollisionMesh = false;
		this.IsCollisionMeshGenerated = false;
		this.entityStubs.Clear();
		for (int i = 0; i < 16; i++)
		{
			for (int j = 0; j < this.entityLists[i].Count; j++)
			{
				if (this.entityLists[i][j].IsSavedToFile())
				{
					this.entityStubs.Add(new EntityCreationData(this.entityLists[i][j], true));
				}
			}
			this.entityLists[i].Clear();
		}
	}

	// Token: 0x06004ABC RID: 19132 RVA: 0x001D88E4 File Offset: 0x001D6AE4
	public void OnLoad(World _world)
	{
		if (!_world.IsRemote())
		{
			for (int i = 0; i < this.entityStubs.Count; i++)
			{
				EntityCreationData entityCreationData = this.entityStubs[i];
				if (!(_world.GetEntity(entityCreationData.id) != null))
				{
					_world.SpawnEntityInWorld(EntityFactory.CreateEntity(entityCreationData));
				}
			}
			this.removeExpiredCustomChunkDataEntries(_world.GetWorldTime());
		}
		if (!_world.IsEditor())
		{
			GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled);
		}
		for (int j = 0; j < this.m_BlockLayers.Length; j++)
		{
			if (this.m_BlockLayers[j] != null)
			{
				this.m_BlockLayers[j].OnLoad(_world, 0, this.X * 16, j * 4, this.Z * 16);
			}
		}
	}

	// Token: 0x06004ABD RID: 19133 RVA: 0x001D899C File Offset: 0x001D6B9C
	public void OnUnload(WorldBase _world)
	{
		this.ProfilerBegin("Chunk OnUnload");
		this.InProgressUnloading = true;
		if (this.biomeParticles != null)
		{
			this.ProfilerBegin("biome particles");
			for (int i = 0; i < this.biomeParticles.Count; i++)
			{
				UnityEngine.Object.Destroy(this.biomeParticles[i]);
			}
			this.biomeParticles = null;
			this.ProfilerEnd();
		}
		this.spawnedBiomeParticles = false;
		if (!_world.IsRemote())
		{
			this.ProfilerBegin("enities");
			for (int j = 0; j < 16; j++)
			{
				if (this.entityLists[j].Count != 0)
				{
					_world.UnloadEntities(this.entityLists[j], false);
				}
			}
			this.ProfilerEnd();
			this.removeExpiredCustomChunkDataEntries(_world.GetWorldTime());
		}
		this.ProfilerBegin("tile entities");
		for (int k = 0; k < this.tileEntities.list.Count; k++)
		{
			this.tileEntities.list[k].OnUnload(GameManager.Instance.World);
		}
		this.ProfilerEnd();
		this.RemoveBlockEntityTransforms();
		this.ProfilerBegin("block layers");
		for (int l = 0; l < this.m_BlockLayers.Length; l++)
		{
			if (this.m_BlockLayers[l] != null)
			{
				this.m_BlockLayers[l].OnUnload(_world, 0, this.X * 16, l * 4, this.Z * 16);
			}
		}
		this.ProfilerEnd();
		this.ProfilerBegin("water");
		this.waterSimHandle.Reset();
		this.ProfilerEnd();
		this.ProfilerEnd();
	}

	// Token: 0x06004ABE RID: 19134 RVA: 0x001D8B21 File Offset: 0x001D6D21
	[PublicizedFrom(EAccessModifier.Private)]
	public void SpawnBiomeParticles(Transform _parentForEntityBlocks)
	{
		if (!this.spawnedBiomeParticles)
		{
			this.biomeParticles = BiomeParticleManager.SpawnParticles(this, _parentForEntityBlocks);
			this.spawnedBiomeParticles = true;
		}
	}

	// Token: 0x06004ABF RID: 19135 RVA: 0x001D8B40 File Offset: 0x001D6D40
	public void OnDisplay(World _world, Transform _entityBlocksParentT, ChunkCluster _chunkCluster)
	{
		this.ProfilerBegin("Chunk OnDisplay");
		this.SpawnBiomeParticles(_entityBlocksParentT);
		this.displayState = Chunk.DisplayState.BlockEntities;
		this.blockEntitiesIndex = 0;
		this.blockEntityStubs.list.Sort((BlockEntityData a, BlockEntityData b) => a.pos.y.CompareTo(b.pos.y));
		this.ProfilerEnd();
	}

	// Token: 0x06004AC0 RID: 19136 RVA: 0x001D8BA4 File Offset: 0x001D6DA4
	public void OnDisplayBlockEntities(World _world, Transform _entityBlocksParentT, ChunkCluster _chunkCluster)
	{
		this.ProfilerBegin("Chunk OnDisplayBlockEntities");
		Vector3 b = new Vector3((float)(this.X * 16), 0f, (float)(this.Z * 16));
		int num = _chunkCluster.LayerMappingTable["nocollision"];
		int num2 = _chunkCluster.LayerMappingTable["terraincollision"];
		int num3 = 0;
		int num4 = Utils.FastMax(50, this.blockEntityStubs.list.Count / 3 + 8);
		while (this.blockEntitiesIndex < this.blockEntityStubs.list.Count)
		{
			BlockEntityData blockEntityData = this.blockEntityStubs.list[this.blockEntitiesIndex];
			if (blockEntityData.bHasTransform)
			{
				if (!this.NeedsOnlyCollisionMesh && !blockEntityData.bRenderingOn)
				{
					this.SetBlockEntityRendering(blockEntityData, true);
				}
			}
			else
			{
				if (++num3 > num4)
				{
					this.ProfilerEnd();
					return;
				}
				BlockValue block = _chunkCluster.GetBlock(blockEntityData.pos);
				if (!this.IsInternalBlocksCulled || block.type == blockEntityData.blockValue.type)
				{
					Block block2 = blockEntityData.blockValue.Block;
					BlockShapeModelEntity blockShapeModelEntity = block2.shape as BlockShapeModelEntity;
					if (blockShapeModelEntity == null)
					{
						this.RemoveEntityBlockStub(blockEntityData.pos);
					}
					else
					{
						float num5 = 0f;
						if (block2.IsTerrainDecoration && _world.GetBlock(blockEntityData.pos - Vector3i.up).Block.shape.IsTerrain())
						{
							num5 = _world.GetDecorationOffsetY(blockEntityData.pos);
						}
						Quaternion rotation = blockShapeModelEntity.GetRotation(block);
						Vector3 rotatedOffset = blockShapeModelEntity.GetRotatedOffset(block2, rotation);
						rotatedOffset.x += 0.5f;
						rotatedOffset.z += 0.5f;
						rotatedOffset.y += num5;
						Vector3 a = blockEntityData.pos.ToVector3() + rotatedOffset;
						GameObject objectForType = GameObjectPool.Instance.GetObjectForType(blockShapeModelEntity.modelName, out block2.defaultTintColor);
						if (objectForType)
						{
							this.ProfilerBegin("BE setup");
							Transform transform = objectForType.transform;
							blockEntityData.transform = transform;
							blockEntityData.bHasTransform = true;
							transform.SetParent(_entityBlocksParentT, false);
							transform.localScale = Vector3.one;
							transform.SetLocalPositionAndRotation(a - b, rotation);
							bool isCollideMovement = block2.IsCollideMovement;
							int newLayer = num;
							if (isCollideMovement)
							{
								int layer = objectForType.layer;
								if (layer == 30)
								{
									newLayer = _chunkCluster.LayerMappingTable["Glass"];
								}
								else if (layer != 4)
								{
									newLayer = num2;
								}
							}
							Utils.SetColliderLayerRecursively(objectForType, newLayer);
							Vector3i vector3i = Chunk.ToLocalPosition(blockEntityData.pos);
							this.ProfilerBegin("BE TBA");
							block2.OnBlockEntityTransformBeforeActivated(_world, blockEntityData.pos, this.GetBlock(vector3i.x, vector3i.y, vector3i.z), blockEntityData);
							this.ProfilerEnd();
							objectForType.SetActive(true);
							this.ProfilerBegin("BE TAA");
							block2.OnBlockEntityTransformAfterActivated(_world, blockEntityData.pos, 0, this.GetBlock(vector3i.x, vector3i.y, vector3i.z), blockEntityData);
							this.ProfilerEnd();
							if (this.NeedsOnlyCollisionMesh)
							{
								this.SetBlockEntityRendering(blockEntityData, false);
							}
							else
							{
								Chunk.occlusionTs.Add(blockEntityData.transform);
							}
							this.ProfilerEnd();
						}
					}
				}
			}
			this.blockEntitiesIndex++;
		}
		if (Chunk.occlusionTs.Count > 0)
		{
			if (OcclusionManager.Instance.cullChunkEntities)
			{
				this.ProfilerBegin("BE occlusion");
				OcclusionManager.Instance.AddChunkTransforms(this, Chunk.occlusionTs);
				this.ProfilerEnd();
			}
			Chunk.occlusionTs.Clear();
		}
		this.removeBlockEntitesMarkedForRemoval();
		AstarManager.AddBoundsToUpdate(this.boundingBox);
		this.displayState = Chunk.DisplayState.Done;
		DynamicMeshThread.AddChunkGameObject(this);
		this.ProfilerEnd();
	}

	// Token: 0x06004AC1 RID: 19137 RVA: 0x001D8F7F File Offset: 0x001D717F
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3i ToLocalPosition(Vector3i _pos)
	{
		_pos.x &= 15;
		_pos.y &= 255;
		_pos.z &= 15;
		return _pos;
	}

	// Token: 0x06004AC2 RID: 19138 RVA: 0x001D8FAC File Offset: 0x001D71AC
	[PublicizedFrom(EAccessModifier.Private)]
	public void removeBlockEntitesMarkedForRemoval()
	{
		if (OcclusionManager.Instance.cullChunkEntities)
		{
			for (int i = 0; i < this.blockEntityStubsToRemove.Count; i++)
			{
				BlockEntityData blockEntityData = this.blockEntityStubsToRemove[i];
				if (blockEntityData.bHasTransform)
				{
					Chunk.occlusionTs.Add(blockEntityData.transform);
				}
			}
			if (Chunk.occlusionTs.Count > 0)
			{
				OcclusionManager.Instance.RemoveChunkTransforms(this, Chunk.occlusionTs);
				Chunk.occlusionTs.Clear();
			}
		}
		for (int j = 0; j < this.blockEntityStubsToRemove.Count; j++)
		{
			BlockEntityData blockEntityData2 = this.blockEntityStubsToRemove[j];
			blockEntityData2.Cleanup();
			if (blockEntityData2.bHasTransform)
			{
				this.poolBlockEntityTransform(blockEntityData2);
			}
		}
		this.blockEntityStubsToRemove.Clear();
	}

	// Token: 0x06004AC3 RID: 19139 RVA: 0x001D906A File Offset: 0x001D726A
	public void OnHide()
	{
		this.RemoveBlockEntityTransforms();
		AstarManager.AddBoundsToUpdate(this.boundingBox);
	}

	// Token: 0x06004AC4 RID: 19140 RVA: 0x001D9080 File Offset: 0x001D7280
	public void RemoveBlockEntityTransforms()
	{
		this.ProfilerBegin("RemoveBlockEntityTransforms");
		if (OcclusionManager.Instance.cullChunkEntities)
		{
			this.ProfilerBegin("OcclusionManager RemoveChunk");
			OcclusionManager.Instance.RemoveChunk(this);
			this.ProfilerEnd();
		}
		for (int i = 0; i < this.blockEntityStubs.list.Count; i++)
		{
			BlockEntityData blockEntityData = this.blockEntityStubs.list[i];
			if (blockEntityData.bHasTransform)
			{
				this.poolBlockEntityTransform(blockEntityData);
			}
		}
		this.ProfilerEnd();
	}

	// Token: 0x06004AC5 RID: 19141 RVA: 0x001D9104 File Offset: 0x001D7304
	[PublicizedFrom(EAccessModifier.Private)]
	public void poolBlockEntityTransform(BlockEntityData _bed)
	{
		if (!_bed.bRenderingOn)
		{
			this.SetBlockEntityRendering(_bed, true);
		}
		if (_bed.transform)
		{
			GameObjectPool.Instance.PoolObject(_bed.transform.gameObject);
		}
		else
		{
			Log.Error("BlockEntity {0} at pos {1} null transform!", new object[]
			{
				_bed.ToString(),
				_bed.pos
			});
		}
		_bed.bHasTransform = false;
		_bed.transform = null;
	}

	// Token: 0x06004AC6 RID: 19142 RVA: 0x001D917C File Offset: 0x001D737C
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetBlockEntityRendering(BlockEntityData _bed, bool _bOn)
	{
		_bed.bRenderingOn = _bOn;
		if (!_bed.transform)
		{
			Log.Error(string.Format("2: {0} on pos {1} with empty transform/gameobject!", _bed.ToString(), _bed.pos));
			return;
		}
		this.ProfilerBegin("SetBlockEntityRendering set enable");
		_bed.transform.GetComponentsInChildren<MeshRenderer>(Chunk.tempMeshRenderers);
		for (int i = 0; i < Chunk.tempMeshRenderers.Count; i++)
		{
			Chunk.tempMeshRenderers[i].enabled = _bOn;
		}
		Chunk.tempMeshRenderers.Clear();
		this.ProfilerEnd();
		this.ProfilerBegin("SetBlockEntityRendering BroadcastMessage");
		if (_bOn)
		{
			_bed.transform.BroadcastMessage("SetRenderingOn", SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			_bed.transform.BroadcastMessage("SetRenderingOff", SendMessageOptions.DontRequireReceiver);
		}
		this.ProfilerEnd();
	}

	// Token: 0x06004AC7 RID: 19143 RVA: 0x001D9248 File Offset: 0x001D7448
	public static void ToTerrain(Chunk _chunk, Chunk _terrainChunk)
	{
		for (int i = 0; i < 16; i++)
		{
			for (int j = 0; j < 16; j++)
			{
				byte height = _chunk.GetHeight(i, j);
				for (int k = 0; k <= (int)height; k++)
				{
					if (!_chunk.GetBlock(i, k, j).isair)
					{
						_terrainChunk.SetBlockRaw(i, k, j, Constants.cTerrainBlockValue);
					}
				}
				for (int l = 0; l < 256; l++)
				{
					_terrainChunk.SetDensity(i, l, j, _chunk.GetDensity(i, l, j));
				}
				_terrainChunk.SetHeight(i, j, height);
				_terrainChunk.SetTerrainHeight(i, j, height);
			}
		}
		_terrainChunk.CopyLightsFrom(_chunk);
		_terrainChunk.isModified = true;
		_terrainChunk.NeedsLightCalculation = false;
	}

	// Token: 0x06004AC8 RID: 19144 RVA: 0x001D9300 File Offset: 0x001D7500
	public void AddMeshLayer(VoxelMeshLayer _vml)
	{
		for (int i = 0; i < MeshDescription.meshes.Length; i++)
		{
			this.trisInMesh[_vml.idx][i] = _vml.GetTrisInMesh(i);
			this.sizeOfMesh[_vml.idx][i] = _vml.GetSizeOfMesh(i);
		}
		this.totalTris = 0;
		for (int j = 0; j < this.trisInMesh.GetLength(0); j++)
		{
			for (int k = 0; k < MeshDescription.meshes.Length; k++)
			{
				this.totalTris += this.trisInMesh[j][k];
			}
		}
		Queue<int> layerIndexQueue = this.m_layerIndexQueue;
		lock (layerIndexQueue)
		{
			VoxelMeshLayer voxelMeshLayer = this.m_meshLayers[_vml.idx];
			if (voxelMeshLayer == null)
			{
				this.MeshLayerCount++;
				this.m_layerIndexQueue.Enqueue(_vml.idx);
			}
			else
			{
				MemoryPools.poolVML.FreeSync(voxelMeshLayer);
			}
			this.m_meshLayers[_vml.idx] = _vml;
		}
	}

	// Token: 0x06004AC9 RID: 19145 RVA: 0x001D9410 File Offset: 0x001D7610
	public bool HasMeshLayer()
	{
		Queue<int> layerIndexQueue = this.m_layerIndexQueue;
		bool result;
		lock (layerIndexQueue)
		{
			result = (this.m_layerIndexQueue.Count > 0);
		}
		return result;
	}

	// Token: 0x06004ACA RID: 19146 RVA: 0x001D945C File Offset: 0x001D765C
	public VoxelMeshLayer GetMeshLayer()
	{
		Queue<int> layerIndexQueue = this.m_layerIndexQueue;
		VoxelMeshLayer result;
		lock (layerIndexQueue)
		{
			if (this.m_layerIndexQueue.Count > 0)
			{
				this.MeshLayerCount--;
				int num = this.m_layerIndexQueue.Dequeue();
				VoxelMeshLayer voxelMeshLayer = this.m_meshLayers[num];
				this.m_meshLayers[num] = null;
				result = voxelMeshLayer;
			}
			else
			{
				result = null;
			}
		}
		return result;
	}

	// Token: 0x06004ACB RID: 19147 RVA: 0x001D94D8 File Offset: 0x001D76D8
	public EnumDecoAllowed GetDecoAllowedAt(int x, int z)
	{
		EnumDecoAllowed enumDecoAllowed = EnumDecoAllowed.Everything;
		if (this.m_DecoBiomeArray != null)
		{
			enumDecoAllowed = this.m_DecoBiomeArray[x + z * 16];
		}
		if (enumDecoAllowed.GetSize() < EnumDecoAllowedSize.NoBigOnlySmall)
		{
			EnumDecoOccupied decoOccupiedAt = DecoManager.Instance.GetDecoOccupiedAt(x + this.m_X * 16, z + this.m_Z * 16);
			if (decoOccupiedAt > EnumDecoOccupied.Perimeter && decoOccupiedAt != EnumDecoOccupied.POI)
			{
				enumDecoAllowed = enumDecoAllowed.WithSize(EnumDecoAllowedSize.NoBigNoSmall);
			}
		}
		return enumDecoAllowed;
	}

	// Token: 0x06004ACC RID: 19148 RVA: 0x001D9538 File Offset: 0x001D7738
	public EnumDecoAllowedSlope GetDecoAllowedSlopeAt(int x, int z)
	{
		return this.GetDecoAllowedAt(x, z).GetSlope();
	}

	// Token: 0x06004ACD RID: 19149 RVA: 0x001D9547 File Offset: 0x001D7747
	public EnumDecoAllowedSize GetDecoAllowedSizeAt(int x, int z)
	{
		return this.GetDecoAllowedAt(x, z).GetSize();
	}

	// Token: 0x06004ACE RID: 19150 RVA: 0x001D9556 File Offset: 0x001D7756
	public bool GetDecoAllowedStreetOnlyAt(int x, int z)
	{
		return this.GetDecoAllowedAt(x, z).GetStreetOnly();
	}

	// Token: 0x06004ACF RID: 19151 RVA: 0x001D9565 File Offset: 0x001D7765
	[PublicizedFrom(EAccessModifier.Private)]
	public void EnsureDecoBiomeArray()
	{
		if (this.m_DecoBiomeArray == null)
		{
			this.m_DecoBiomeArray = new EnumDecoAllowed[256];
		}
	}

	// Token: 0x06004AD0 RID: 19152 RVA: 0x001D9580 File Offset: 0x001D7780
	public void SetDecoAllowedAt(int x, int z, EnumDecoAllowed _newVal)
	{
		this.EnsureDecoBiomeArray();
		int num = x + z * 16;
		EnumDecoAllowed decoAllowed = this.m_DecoBiomeArray[num];
		EnumDecoAllowedSlope slope = decoAllowed.GetSlope();
		if (slope > _newVal.GetSlope())
		{
			_newVal = _newVal.WithSlope(slope);
		}
		EnumDecoAllowedSize size = decoAllowed.GetSize();
		if (size > _newVal.GetSize())
		{
			_newVal = _newVal.WithSize(size);
		}
		if (decoAllowed.GetStreetOnly() && !_newVal.GetStreetOnly())
		{
			_newVal = _newVal.WithStreetOnly(true);
		}
		this.m_DecoBiomeArray[num] = _newVal;
	}

	// Token: 0x06004AD1 RID: 19153 RVA: 0x001D95F8 File Offset: 0x001D77F8
	public void SetDecoAllowedSlopeAt(int x, int z, EnumDecoAllowedSlope _newVal)
	{
		this.EnsureDecoBiomeArray();
		int num = x + z * 16;
		this.SetDecoAllowedAt(x, z, this.m_DecoBiomeArray[num].WithSlope(_newVal));
	}

	// Token: 0x06004AD2 RID: 19154 RVA: 0x001D9628 File Offset: 0x001D7828
	public void SetDecoAllowedSizeAt(int x, int z, EnumDecoAllowedSize _newVal)
	{
		this.EnsureDecoBiomeArray();
		int num = x + z * 16;
		this.SetDecoAllowedAt(x, z, this.m_DecoBiomeArray[num].WithSize(_newVal));
	}

	// Token: 0x06004AD3 RID: 19155 RVA: 0x001D9658 File Offset: 0x001D7858
	public void SetDecoAllowedStreetOnlyAt(int x, int z, bool _newVal)
	{
		this.EnsureDecoBiomeArray();
		int num = x + z * 16;
		this.SetDecoAllowedAt(x, z, this.m_DecoBiomeArray[num].WithStreetOnly(_newVal));
	}

	// Token: 0x06004AD4 RID: 19156 RVA: 0x001D9688 File Offset: 0x001D7888
	public Vector3 GetTerrainNormal(int _x, int _z)
	{
		int num = _x + _z * 16;
		Vector3 result;
		result.x = (float)((sbyte)this.m_NormalX[num]) / 127f;
		result.y = (float)((sbyte)this.m_NormalY[num]) / 127f;
		result.z = (float)((sbyte)this.m_NormalZ[num]) / 127f;
		return result;
	}

	// Token: 0x06004AD5 RID: 19157 RVA: 0x001D96E4 File Offset: 0x001D78E4
	public float GetTerrainNormalY(int _x, int _z)
	{
		int num = _x + _z * 16;
		return (float)((sbyte)this.m_NormalY[num]) / 127f;
	}

	// Token: 0x06004AD6 RID: 19158 RVA: 0x001D9708 File Offset: 0x001D7908
	public void SetTerrainNormal(int x, int z, Vector3 _v)
	{
		int num = x + z * 16;
		this.m_NormalX[num] = (byte)Utils.FastClamp(_v.x * 127f, -128f, 127f);
		this.m_NormalY[num] = (byte)Utils.FastClamp(_v.y * 127f, -128f, 127f);
		this.m_NormalZ[num] = (byte)Utils.FastClamp(_v.z * 127f, -128f, 127f);
	}

	// Token: 0x06004AD7 RID: 19159 RVA: 0x001D9788 File Offset: 0x001D7988
	public Vector3i ToWorldPos()
	{
		return new Vector3i(this.m_X * 16, this.m_Y * 256, this.m_Z * 16);
	}

	// Token: 0x06004AD8 RID: 19160 RVA: 0x001D97AD File Offset: 0x001D79AD
	public Vector3i ToWorldPos(int _x, int _y, int _z)
	{
		return new Vector3i(this.m_X * 16 + _x, this.m_Y * 256 + _y, this.m_Z * 16 + _z);
	}

	// Token: 0x06004AD9 RID: 19161 RVA: 0x001D97D8 File Offset: 0x001D79D8
	public Vector3i ToWorldPos(Vector3i _pos)
	{
		return new Vector3i(this.m_X * 16, this.m_Y * 256, this.m_Z * 16) + _pos;
	}

	// Token: 0x06004ADA RID: 19162 RVA: 0x001D9804 File Offset: 0x001D7A04
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateFullMap()
	{
		if (this.mapColors == null)
		{
			this.mapColors = new ushort[256];
		}
		for (int i = 0; i < 16; i++)
		{
			for (int j = 0; j < 16; j++)
			{
				int num = i + j * 16;
				int num2 = (int)this.m_HeightMap[num];
				int num3 = num2 >> 2;
				BlockValue blockValue = (this.m_BlockLayers[num3] != null) ? this.m_BlockLayers[num3].GetAt(i, num2, j) : BlockValue.Air;
				WaterValue water = this.GetWater(i, num2, j);
				while (num2 > 0 && (blockValue.isair || blockValue.Block.IsTerrainDecoration) && !water.HasMass())
				{
					num2--;
					blockValue = ((this.m_BlockLayers[num3] != null) ? this.m_BlockLayers[num3].GetAt(i, num2, j) : BlockValue.Air);
					water = this.GetWater(i, num2, j);
				}
				Color col = BlockLiquidv2.Color;
				if (!water.HasMass())
				{
					float x = (float)((sbyte)this.m_NormalX[num]) / 127f;
					float y = (float)((sbyte)this.m_NormalY[num]) / 127f;
					float z = (float)((sbyte)this.m_NormalZ[num]) / 127f;
					col = blockValue.Block.GetMapColor(blockValue, new Vector3(x, y, z), num2);
				}
				this.mapColors[num] = Utils.ToColor5(col);
			}
		}
		this.bMapDirty = false;
		ModEvents.SCalcChunkColorsDoneData scalcChunkColorsDoneData = new ModEvents.SCalcChunkColorsDoneData(this);
		ModEvents.CalcChunkColorsDone.Invoke(ref scalcChunkColorsDoneData);
	}

	// Token: 0x06004ADB RID: 19163 RVA: 0x001D9984 File Offset: 0x001D7B84
	public ushort[] GetMapColors()
	{
		if (this.mapColors == null || this.bMapDirty)
		{
			this.updateFullMap();
		}
		return this.mapColors;
	}

	// Token: 0x06004ADC RID: 19164 RVA: 0x001D99A2 File Offset: 0x001D7BA2
	public void OnDecorated()
	{
		this.CheckSameDensity();
		this.CheckOnlyTerrain();
	}

	// Token: 0x06004ADD RID: 19165 RVA: 0x001D99B0 File Offset: 0x001D7BB0
	public bool IsAreaMaster()
	{
		return this.m_X % 5 == 0 && this.m_Z % 5 == 0;
	}

	// Token: 0x06004ADE RID: 19166 RVA: 0x001D99CC File Offset: 0x001D7BCC
	public bool IsAreaMasterCornerChunksLoaded(ChunkCluster _cc)
	{
		return _cc.GetChunkSync(this.m_X - 2, this.m_Z) != null && _cc.GetChunkSync(this.m_X, this.m_Z + 2) != null && _cc.GetChunkSync(this.m_X + 2, this.m_Z + 2) != null && _cc.GetChunkSync(this.m_X - 2, this.m_Z - 2) != null;
	}

	// Token: 0x06004ADF RID: 19167 RVA: 0x001D9A38 File Offset: 0x001D7C38
	public static Vector3i ToAreaMasterChunkPos(Vector3i _worldBlockPos)
	{
		return new Vector3i(World.toChunkXZ(_worldBlockPos.x) / 5 * 5, World.toChunkY(_worldBlockPos.y), World.toChunkXZ(_worldBlockPos.z) / 5 * 5);
	}

	// Token: 0x06004AE0 RID: 19168 RVA: 0x001D9A68 File Offset: 0x001D7C68
	public bool IsAreaMasterDominantBiomeInitialized(ChunkCluster _cc)
	{
		if (this.AreaMasterDominantBiome != 255)
		{
			return true;
		}
		if (_cc == null)
		{
			return false;
		}
		for (int i = 0; i < 50; i++)
		{
			Chunk.biomeCnt[i] = 0;
		}
		for (int j = this.m_X - 2; j < this.m_X + 2; j++)
		{
			for (int k = this.m_Z - 2; k < this.m_Z + 2; k++)
			{
				Chunk chunkSync = _cc.GetChunkSync(j, k);
				if (chunkSync == null)
				{
					return false;
				}
				if (chunkSync.DominantBiome > 0)
				{
					Chunk.biomeCnt[(int)chunkSync.DominantBiome]++;
				}
			}
		}
		int num = 0;
		for (int l = 1; l < Chunk.biomeCnt.Length; l++)
		{
			if (Chunk.biomeCnt[l] > num)
			{
				this.AreaMasterDominantBiome = (byte)l;
				num = Chunk.biomeCnt[l];
			}
		}
		return true;
	}

	// Token: 0x06004AE1 RID: 19169 RVA: 0x001D9B38 File Offset: 0x001D7D38
	public ChunkAreaBiomeSpawnData GetChunkBiomeSpawnData()
	{
		if (this.AreaMasterDominantBiome == 255)
		{
			return null;
		}
		if (this.biomeSpawnData == null)
		{
			ChunkCustomData chunkCustomData;
			if (!this.ChunkCustomData.dict.TryGetValue("bspd.main", out chunkCustomData) || chunkCustomData == null)
			{
				chunkCustomData = new ChunkCustomData("bspd.main", ulong.MaxValue, false);
				this.ChunkCustomData.Set(chunkCustomData.key, chunkCustomData);
			}
			this.biomeSpawnData = new ChunkAreaBiomeSpawnData(this, this.AreaMasterDominantBiome, chunkCustomData);
		}
		return this.biomeSpawnData;
	}

	// Token: 0x06004AE2 RID: 19170 RVA: 0x001D9BB4 File Offset: 0x001D7DB4
	[PublicizedFrom(EAccessModifier.Private)]
	public void removeExpiredCustomChunkDataEntries(ulong _worldTime)
	{
		List<string> list = null;
		for (int i = 0; i < this.ChunkCustomData.valueList.Count; i++)
		{
			if (this.ChunkCustomData.valueList[i].expiresInWorldTime <= _worldTime)
			{
				if (list == null)
				{
					list = new List<string>();
				}
				list.Add(this.ChunkCustomData.keyList[i]);
				this.ChunkCustomData.valueList[i].OnRemove(this);
			}
		}
		if (list != null)
		{
			for (int j = 0; j < list.Count; j++)
			{
				this.ChunkCustomData.Remove(list[j]);
			}
		}
	}

	// Token: 0x06004AE3 RID: 19171 RVA: 0x001D9C54 File Offset: 0x001D7E54
	public bool IsTraderArea(int _x, int _z)
	{
		Vector3i worldBlockPos = this.worldPosIMin;
		worldBlockPos.x += _x;
		worldBlockPos.z += _z;
		return GameManager.Instance.World.IsWithinTraderArea(worldBlockPos);
	}

	// Token: 0x06004AE4 RID: 19172 RVA: 0x001D9C90 File Offset: 0x001D7E90
	public override int GetHashCode()
	{
		return 31 * this.m_X + this.m_Z;
	}

	// Token: 0x06004AE5 RID: 19173 RVA: 0x001D9CA2 File Offset: 0x001D7EA2
	public void EnterReadLock()
	{
		this.sync.EnterReadLock();
	}

	// Token: 0x06004AE6 RID: 19174 RVA: 0x001D9CAF File Offset: 0x001D7EAF
	public void EnterWriteLock()
	{
		this.sync.EnterWriteLock();
	}

	// Token: 0x06004AE7 RID: 19175 RVA: 0x001D9CBC File Offset: 0x001D7EBC
	public void ExitReadLock()
	{
		this.sync.ExitReadLock();
	}

	// Token: 0x06004AE8 RID: 19176 RVA: 0x001D9CC9 File Offset: 0x001D7EC9
	public void ExitWriteLock()
	{
		this.sync.ExitWriteLock();
	}

	// Token: 0x06004AE9 RID: 19177 RVA: 0x001D9CD6 File Offset: 0x001D7ED6
	public override bool Equals(object obj)
	{
		return base.Equals(obj) && obj.GetHashCode() == this.GetHashCode();
	}

	// Token: 0x06004AEA RID: 19178 RVA: 0x001D9CF1 File Offset: 0x001D7EF1
	public override string ToString()
	{
		if (this.cachedToString == null)
		{
			this.cachedToString = string.Format("Chunk_{0},{1}", this.m_X, this.m_Z);
		}
		return this.cachedToString;
	}

	// Token: 0x06004AEB RID: 19179 RVA: 0x001D9D28 File Offset: 0x001D7F28
	public List<Chunk.DensityMismatchInformation> CheckDensities(bool _logAllMismatches = false)
	{
		Vector3i vector3i = new Vector3i(0, 0, 0);
		Vector3i vector3i2 = new Vector3i(16, 256, 16);
		int num = this.m_X << 4;
		int num2 = this.m_Y << 8;
		int num3 = this.m_Z << 4;
		bool flag = true;
		List<Chunk.DensityMismatchInformation> list = new List<Chunk.DensityMismatchInformation>();
		for (int i = vector3i.x; i < vector3i2.x; i++)
		{
			for (int j = vector3i.z; j < vector3i2.z; j++)
			{
				for (int k = vector3i.y; k < vector3i2.y; k++)
				{
					sbyte density = this.GetDensity(i, k, j);
					BlockValue block = this.GetBlock(i, k, j);
					bool flag2 = block.Block.shape.IsTerrain();
					bool flag3;
					if (flag2)
					{
						flag3 = (density < 0);
					}
					else
					{
						flag3 = (density >= 0);
					}
					if (!flag3)
					{
						Chunk.DensityMismatchInformation item = new Chunk.DensityMismatchInformation(num + i, num2 + k, num3 + j, density, block.type, flag2);
						list.Add(item);
						if (flag || _logAllMismatches)
						{
							Log.Warning(item.ToString());
							flag = false;
						}
					}
				}
			}
		}
		return list;
	}

	// Token: 0x06004AEC RID: 19180 RVA: 0x001D9E68 File Offset: 0x001D8068
	public bool RepairDensities()
	{
		Vector3i vector3i = new Vector3i(0, 0, 0);
		Vector3i vector3i2 = new Vector3i(16, 256, 16);
		bool result = false;
		for (int i = vector3i.x; i < vector3i2.x; i++)
		{
			for (int j = vector3i.z; j < vector3i2.z; j++)
			{
				for (int k = vector3i.y; k < vector3i2.y; k++)
				{
					Block block = this.GetBlock(i, k, j).Block;
					sbyte density = this.GetDensity(i, k, j);
					if (block.shape.IsTerrain())
					{
						if (density >= 0)
						{
							this.SetDensity(i, k, j, -1);
							result = true;
						}
					}
					else if (density < 0)
					{
						this.SetDensity(i, k, j, 1);
						result = true;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06004AED RID: 19181 RVA: 0x001D9F38 File Offset: 0x001D8138
	public void LoopOverAllBlocks(ChunkBlockLayer.LoopBlocksDelegate _delegate, bool _bIncludeChilds = false, bool _bIncludeAirBlocks = false)
	{
		for (int i = 0; i < this.m_BlockLayers.Length; i++)
		{
			ChunkBlockLayer chunkBlockLayer = this.m_BlockLayers[i];
			if (chunkBlockLayer != null)
			{
				chunkBlockLayer.LoopOverAllBlocks(this, i << 2, _delegate, _bIncludeChilds, _bIncludeAirBlocks);
			}
		}
	}

	// Token: 0x06004AEE RID: 19182 RVA: 0x001D9F74 File Offset: 0x001D8174
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isInside(int _x, int _y, int _z)
	{
		Vector3b vector3b = new Vector3b(_x, _y, _z);
		return this.insideDevicesHashSet.Contains(vector3b.GetHashCode());
	}

	// Token: 0x06004AEF RID: 19183 RVA: 0x001D9FA4 File Offset: 0x001D81A4
	public BlockFaceFlag RestoreCulledBlocks(World _world)
	{
		BlockFaceFlag blockFaceFlag = BlockFaceFlag.None;
		for (int i = this.insideDevices.Count - 1; i >= 0; i--)
		{
			Vector3b vector3b = this.insideDevices[i];
			if (vector3b.x == 0)
			{
				blockFaceFlag |= BlockFaceFlag.West;
			}
			else if (vector3b.x == 15)
			{
				blockFaceFlag |= BlockFaceFlag.East;
			}
			if (vector3b.z == 0)
			{
				blockFaceFlag |= BlockFaceFlag.North;
			}
			else if (vector3b.z == 15)
			{
				blockFaceFlag |= BlockFaceFlag.South;
			}
		}
		this.IsInternalBlocksCulled = false;
		return blockFaceFlag;
	}

	// Token: 0x06004AF0 RID: 19184 RVA: 0x001DA01C File Offset: 0x001D821C
	public bool HasFallingBlocks()
	{
		foreach (List<Entity> list in this.entityLists)
		{
			for (int j = 0; j < list.Count; j++)
			{
				if (list[j] is EntityFallingBlock)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06004AF1 RID: 19185 RVA: 0x00002914 File Offset: 0x00000B14
	[Conditional("DEBUG_CHUNK_PROFILE")]
	[PublicizedFrom(EAccessModifier.Private)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ProfilerBegin(string _name)
	{
	}

	// Token: 0x06004AF2 RID: 19186 RVA: 0x00002914 File Offset: 0x00000B14
	[Conditional("DEBUG_CHUNK_PROFILE")]
	[PublicizedFrom(EAccessModifier.Private)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ProfilerEnd()
	{
	}

	// Token: 0x06004AF3 RID: 19187 RVA: 0x001DA064 File Offset: 0x001D8264
	[Conditional("DEBUG_CHUNK_RWCHECK")]
	[PublicizedFrom(EAccessModifier.Private)]
	public void RWCheck(PooledBinaryReader stream)
	{
		if (stream.ReadInt32() != 1431655765)
		{
			Log.Error("Chunk !RWCheck");
		}
	}

	// Token: 0x06004AF4 RID: 19188 RVA: 0x001DA07D File Offset: 0x001D827D
	[Conditional("DEBUG_CHUNK_RWCHECK")]
	[PublicizedFrom(EAccessModifier.Private)]
	public void RWCheck(PooledBinaryWriter stream)
	{
		stream.Write(1431655765);
	}

	// Token: 0x06004AF5 RID: 19189 RVA: 0x001DA08A File Offset: 0x001D828A
	[Conditional("DEBUG_CHUNK_TRIGGERLOG")]
	public void LogTrigger(string _format = "", params object[] _args)
	{
		_format = string.Format("{0} Chunk {1} trigger {2}", GameManager.frameCount, this.ChunkPos, _format);
		Log.Warning(_format, _args);
	}

	// Token: 0x06004AF6 RID: 19190 RVA: 0x001DA0B8 File Offset: 0x001D82B8
	[Conditional("DEBUG_CHUNK_CHUNK")]
	public static void LogChunk(long _key, string _format = "", params object[] _args)
	{
		int num = WorldChunkCache.extractX(_key);
		int num2 = WorldChunkCache.extractZ(_key);
		if (num == 136 && num2 == 25)
		{
			_format = string.Format("{0} Chunk pos {1} {2}, {3}", new object[]
			{
				GameManager.frameCount,
				num,
				num2,
				_format
			});
			Log.Warning(_format, _args);
		}
	}

	// Token: 0x06004AF7 RID: 19191 RVA: 0x001DA11C File Offset: 0x001D831C
	[Conditional("DEBUG_CHUNK_ENTITY")]
	public void LogEntity(string _format = "", params object[] _args)
	{
		if (this.m_X == 136 && this.m_Z == 25)
		{
			_format = string.Format("{0} Chunk {1} entity {2}", GameManager.frameCount, this.ChunkPos, _format);
			Log.Warning(_format, _args);
		}
	}

	// Token: 0x06004AF8 RID: 19192 RVA: 0x001DA16C File Offset: 0x001D836C
	public void LogChunkState()
	{
		Log.Out(string.Concat(new string[]
		{
			string.Format("[FELLTHROUGHWORLD] Chunk {0} State\n", this.Key),
			string.Format("  Displayed: {0}\n", this.IsDisplayed),
			string.Format("  IsCollisionMeshGenerated: {0}\n", this.IsCollisionMeshGenerated),
			string.Format("  NeedsDecoration: {0}\n", this.NeedsDecoration),
			string.Format("  NeedsLightDecoration: {0}\n", this.NeedsLightDecoration),
			string.Format("  NeedsLightCalculation: {0}\n", this.NeedsLightCalculation),
			string.Format("  NeedsRegeneration: {0} {1}\n", this.NeedsRegeneration, this.FormatRegenerationLayers(this.m_NeedsRegenerationAtY)),
			string.Format("  NeedsCopying: {0} (layers: {1})", this.NeedsCopying, this.m_layerIndexQueue.Count)
		}));
	}

	// Token: 0x06004AF9 RID: 19193 RVA: 0x001DA274 File Offset: 0x001D8474
	[PublicizedFrom(EAccessModifier.Private)]
	public string FormatRegenerationLayers(int mask)
	{
		if (mask == 0)
		{
			return "(none)";
		}
		if (mask == 65535)
		{
			return "(all layers)";
		}
		List<int> list = new List<int>();
		for (int i = 0; i < 16; i++)
		{
			if ((mask & 1 << i) != 0)
			{
				list.Add(i);
			}
		}
		return "(Y layers: " + string.Join<int>(", ", list) + ")";
	}

	// Token: 0x0400391D RID: 14621
	public static uint CurrentSaveVersion = 47U;

	// Token: 0x0400391E RID: 14622
	public const int cAreaMasterSizeChunks = 5;

	// Token: 0x0400391F RID: 14623
	public const int cAreaMasterSizeBlocks = 80;

	// Token: 0x04003920 RID: 14624
	public const int cTextureChannelCount = 1;

	// Token: 0x04003921 RID: 14625
	[PublicizedFrom(EAccessModifier.Private)]
	public ChunkBlockLayer[] m_BlockLayers;

	// Token: 0x04003922 RID: 14626
	[PublicizedFrom(EAccessModifier.Private)]
	public ChunkBlockChannel chnStability;

	// Token: 0x04003923 RID: 14627
	[PublicizedFrom(EAccessModifier.Private)]
	public ChunkBlockChannel chnDensity;

	// Token: 0x04003924 RID: 14628
	[PublicizedFrom(EAccessModifier.Private)]
	public ChunkBlockChannel chnLight;

	// Token: 0x04003925 RID: 14629
	[PublicizedFrom(EAccessModifier.Private)]
	public ChunkBlockChannel chnDamage;

	// Token: 0x04003926 RID: 14630
	[PublicizedFrom(EAccessModifier.Private)]
	public ChunkBlockChannel[] chnTextures;

	// Token: 0x04003927 RID: 14631
	[PublicizedFrom(EAccessModifier.Private)]
	public ChunkBlockChannel chnWater;

	// Token: 0x04003928 RID: 14632
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_X;

	// Token: 0x04003929 RID: 14633
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_Y;

	// Token: 0x0400392A RID: 14634
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_Z;

	// Token: 0x0400392B RID: 14635
	public Vector3i worldPosIMin;

	// Token: 0x0400392C RID: 14636
	public Vector3i worldPosIMax;

	// Token: 0x0400392D RID: 14637
	[PublicizedFrom(EAccessModifier.Private)]
	public const double cEntityListHeight = 16.0;

	// Token: 0x0400392E RID: 14638
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cEntityListCount = 16;

	// Token: 0x0400392F RID: 14639
	public List<Entity>[] entityLists = new List<Entity>[16];

	// Token: 0x04003930 RID: 14640
	[PublicizedFrom(EAccessModifier.Private)]
	public DictionaryList<Vector3i, TileEntity> tileEntities = new DictionaryList<Vector3i, TileEntity>();

	// Token: 0x04003931 RID: 14641
	[PublicizedFrom(EAccessModifier.Private)]
	public List<int> sleeperVolumes = new List<int>();

	// Token: 0x04003932 RID: 14642
	[PublicizedFrom(EAccessModifier.Private)]
	public List<int> triggerVolumes = new List<int>();

	// Token: 0x04003933 RID: 14643
	[PublicizedFrom(EAccessModifier.Private)]
	public List<int> wallVolumes = new List<int>();

	// Token: 0x04003934 RID: 14644
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] m_HeightMap;

	// Token: 0x04003935 RID: 14645
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] m_bTopSoilBroken;

	// Token: 0x04003936 RID: 14646
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] m_Biomes;

	// Token: 0x04003937 RID: 14647
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] m_BiomeIntensities;

	// Token: 0x04003938 RID: 14648
	public byte DominantBiome;

	// Token: 0x04003939 RID: 14649
	public byte AreaMasterDominantBiome = byte.MaxValue;

	// Token: 0x0400393A RID: 14650
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] m_NormalX;

	// Token: 0x0400393B RID: 14651
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] m_NormalY;

	// Token: 0x0400393C RID: 14652
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] m_NormalZ;

	// Token: 0x0400393D RID: 14653
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] m_TerrainHeight;

	// Token: 0x0400393E RID: 14654
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EntityCreationData> entityStubs = new List<EntityCreationData>();

	// Token: 0x0400393F RID: 14655
	public DictionaryKeyValueList<string, ChunkCustomData> ChunkCustomData = new DictionaryKeyValueList<string, ChunkCustomData>();

	// Token: 0x04003940 RID: 14656
	public ulong SavedInWorldTicks;

	// Token: 0x04003941 RID: 14657
	public ulong LastTimeRandomTicked;

	// Token: 0x04003942 RID: 14658
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Vector3b> insideDevices = new List<Vector3b>();

	// Token: 0x04003943 RID: 14659
	[PublicizedFrom(EAccessModifier.Private)]
	public HashSet<int> insideDevicesHashSet = new HashSet<int>();

	// Token: 0x04003944 RID: 14660
	[PublicizedFrom(EAccessModifier.Private)]
	public DictionaryList<Vector3i, BlockTrigger> triggerData = new DictionaryList<Vector3i, BlockTrigger>();

	// Token: 0x04003945 RID: 14661
	[PublicizedFrom(EAccessModifier.Private)]
	public DictionaryList<ulong, BlockEntityData> blockEntityStubs = new DictionaryList<ulong, BlockEntityData>();

	// Token: 0x04003946 RID: 14662
	[PublicizedFrom(EAccessModifier.Private)]
	public List<BlockEntityData> blockEntityStubsToRemove = new List<BlockEntityData>();

	// Token: 0x04003947 RID: 14663
	[PublicizedFrom(EAccessModifier.Private)]
	public ChunkAreaBiomeSpawnData biomeSpawnData;

	// Token: 0x04003948 RID: 14664
	[PublicizedFrom(EAccessModifier.Private)]
	public Queue<int> m_layerIndexQueue = new Queue<int>();

	// Token: 0x04003949 RID: 14665
	[PublicizedFrom(EAccessModifier.Private)]
	public VoxelMeshLayer[] m_meshLayers = new VoxelMeshLayer[16];

	// Token: 0x0400394A RID: 14666
	public volatile bool hasEntities;

	// Token: 0x0400394B RID: 14667
	public bool isModified;

	// Token: 0x0400394C RID: 14668
	[PublicizedFrom(EAccessModifier.Private)]
	public Bounds boundingBox;

	// Token: 0x0400394D RID: 14669
	public DictionarySave<string, List<Vector3i>> IndexedBlocks = new DictionarySave<string, List<Vector3i>>();

	// Token: 0x0400394E RID: 14670
	[PublicizedFrom(EAccessModifier.Private)]
	public volatile int m_NeedsRegenerationAtY;

	// Token: 0x0400394F RID: 14671
	[PublicizedFrom(EAccessModifier.Private)]
	public EnumDecoAllowed[] m_DecoBiomeArray;

	// Token: 0x04003950 RID: 14672
	[PublicizedFrom(EAccessModifier.Private)]
	public ushort[] mapColors;

	// Token: 0x04003951 RID: 14673
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bMapDirty;

	// Token: 0x04003952 RID: 14674
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bEmpty;

	// Token: 0x04003953 RID: 14675
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bEmptyDirty = true;

	// Token: 0x04003954 RID: 14676
	[PublicizedFrom(EAccessModifier.Private)]
	public DictionaryKeyList<Vector3i, int> tickedBlocks = new DictionaryKeyList<Vector3i, int>();

	// Token: 0x04003955 RID: 14677
	public bool IsInternalBlocksCulled;

	// Token: 0x04003956 RID: 14678
	public bool StopStabilityCalculation;

	// Token: 0x04003957 RID: 14679
	public OcclusionManager.OccludeeZone occludeeZone;

	// Token: 0x04003958 RID: 14680
	public readonly ReaderWriterLockSlim sync = new ReaderWriterLockSlim();

	// Token: 0x04003959 RID: 14681
	[PublicizedFrom(EAccessModifier.Private)]
	public WaterSimulationNative.ChunkHandle waterSimHandle;

	// Token: 0x0400395A RID: 14682
	public static int InstanceCount;

	// Token: 0x0400395B RID: 14683
	public int TotalMemory;

	// Token: 0x0400395C RID: 14684
	[PublicizedFrom(EAccessModifier.Private)]
	public int totalTris;

	// Token: 0x0400395D RID: 14685
	[PublicizedFrom(EAccessModifier.Private)]
	public int[][] trisInMesh = new int[16][];

	// Token: 0x0400395E RID: 14686
	[PublicizedFrom(EAccessModifier.Private)]
	public int[][] sizeOfMesh = new int[16][];

	// Token: 0x0400395F RID: 14687
	[PublicizedFrom(EAccessModifier.Private)]
	public WaterDebugManager.RendererHandle waterDebugHandle;

	// Token: 0x04003960 RID: 14688
	public readonly int ClrIdx;

	// Token: 0x04003961 RID: 14689
	public volatile bool InProgressCopying;

	// Token: 0x04003962 RID: 14690
	public volatile bool InProgressDecorating;

	// Token: 0x04003963 RID: 14691
	public volatile bool InProgressLighting;

	// Token: 0x04003964 RID: 14692
	public volatile bool InProgressRegeneration;

	// Token: 0x04003965 RID: 14693
	public volatile bool InProgressUnloading;

	// Token: 0x04003966 RID: 14694
	public volatile bool InProgressSaving;

	// Token: 0x04003967 RID: 14695
	public volatile bool InProgressNetworking;

	// Token: 0x04003968 RID: 14696
	public volatile bool InProgressWaterSim;

	// Token: 0x04003969 RID: 14697
	public volatile bool IsDisplayed;

	// Token: 0x0400396A RID: 14698
	public volatile bool IsCollisionMeshGenerated;

	// Token: 0x0400396B RID: 14699
	public volatile bool NeedsOnlyCollisionMesh;

	// Token: 0x0400396C RID: 14700
	public int NeedsRegenerationDebug;

	// Token: 0x0400396D RID: 14701
	public volatile bool NeedsDecoration;

	// Token: 0x0400396E RID: 14702
	public volatile bool NeedsLightDecoration;

	// Token: 0x0400396F RID: 14703
	public volatile bool NeedsLightCalculation;

	// Token: 0x04003970 RID: 14704
	[PublicizedFrom(EAccessModifier.Private)]
	public static BlockValue bvPOIFiller;

	// Token: 0x04003971 RID: 14705
	public static bool IgnorePaintTextures = false;

	// Token: 0x04003972 RID: 14706
	[PublicizedFrom(EAccessModifier.Private)]
	public bool spawnedBiomeParticles;

	// Token: 0x04003973 RID: 14707
	[PublicizedFrom(EAccessModifier.Private)]
	public List<GameObject> biomeParticles;

	// Token: 0x04003974 RID: 14708
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<Transform> occlusionTs = new List<Transform>(200);

	// Token: 0x04003975 RID: 14709
	public Chunk.DisplayState displayState;

	// Token: 0x04003976 RID: 14710
	[PublicizedFrom(EAccessModifier.Private)]
	public int blockEntitiesIndex;

	// Token: 0x04003977 RID: 14711
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<MeshRenderer> tempMeshRenderers = new List<MeshRenderer>();

	// Token: 0x04003978 RID: 14712
	public int MeshLayerCount;

	// Token: 0x04003979 RID: 14713
	[PublicizedFrom(EAccessModifier.Private)]
	public static int[] biomeCnt = new int[50];

	// Token: 0x0400397A RID: 14714
	[PublicizedFrom(EAccessModifier.Private)]
	public string cachedToString;

	// Token: 0x0400397B RID: 14715
	[PublicizedFrom(EAccessModifier.Private)]
	public const int dbChunkX = 136;

	// Token: 0x0400397C RID: 14716
	[PublicizedFrom(EAccessModifier.Private)]
	public const int dbChunkZ = 25;

	// Token: 0x0200099B RID: 2459
	public enum LIGHT_TYPE
	{
		// Token: 0x0400397E RID: 14718
		BLOCK,
		// Token: 0x0400397F RID: 14719
		SUN
	}

	// Token: 0x0200099C RID: 2460
	public enum DisplayState
	{
		// Token: 0x04003981 RID: 14721
		Start,
		// Token: 0x04003982 RID: 14722
		BlockEntities,
		// Token: 0x04003983 RID: 14723
		Done
	}

	// Token: 0x0200099D RID: 2461
	public struct DensityMismatchInformation
	{
		// Token: 0x06004AFB RID: 19195 RVA: 0x001DA30A File Offset: 0x001D850A
		public DensityMismatchInformation(int _x, int _y, int _z, sbyte _density, int _bvType, bool _isTerrain)
		{
			this.x = _x;
			this.y = _y;
			this.z = _z;
			this.density = _density;
			this.bvType = _bvType;
			this.isTerrain = _isTerrain;
		}

		// Token: 0x06004AFC RID: 19196 RVA: 0x001DA33C File Offset: 0x001D853C
		public string ToJsonString()
		{
			return string.Format("{{\"x\":{0}, \"y\":{1}, \"z\":{2}, \"density\":{3}, \"bvtype\":{4}, \"terrain\":{5}}}", new object[]
			{
				this.x,
				this.y,
				this.z,
				this.density,
				this.bvType,
				this.isTerrain.ToString().ToLower()
			});
		}

		// Token: 0x06004AFD RID: 19197 RVA: 0x001DA3B4 File Offset: 0x001D85B4
		public override string ToString()
		{
			return string.Format("DENSITYMISMATCH;{0};{1};{2};{3};{4};{5}", new object[]
			{
				this.x,
				this.y,
				this.z,
				this.density,
				this.isTerrain,
				this.bvType
			});
		}

		// Token: 0x04003984 RID: 14724
		public int x;

		// Token: 0x04003985 RID: 14725
		public int y;

		// Token: 0x04003986 RID: 14726
		public int z;

		// Token: 0x04003987 RID: 14727
		public sbyte density;

		// Token: 0x04003988 RID: 14728
		public int bvType;

		// Token: 0x04003989 RID: 14729
		public bool isTerrain;
	}
}
