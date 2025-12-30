using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000A0B RID: 2571
public class MultiBlockManager
{
	// Token: 0x1700080F RID: 2063
	// (get) Token: 0x06004EB8 RID: 20152 RVA: 0x001F2D1A File Offset: 0x001F0F1A
	public static MultiBlockManager Instance
	{
		get
		{
			if (MultiBlockManager.m_Instance == null)
			{
				MultiBlockManager.m_Instance = new MultiBlockManager();
			}
			return MultiBlockManager.m_Instance;
		}
	}

	// Token: 0x06004EB9 RID: 20153 RVA: 0x001F2D34 File Offset: 0x001F0F34
	[PublicizedFrom(EAccessModifier.Private)]
	public MultiBlockManager()
	{
	}

	// Token: 0x06004EBA RID: 20154 RVA: 0x001F2D89 File Offset: 0x001F0F89
	[PublicizedFrom(EAccessModifier.Private)]
	public bool CheckFeatures(MultiBlockManager.FeatureFlags targetFeatures, MultiBlockManager.FeatureRequirement requirement = MultiBlockManager.FeatureRequirement.AllEnabled)
	{
		this.DoCurrentModeSanityChecks();
		switch (requirement)
		{
		case MultiBlockManager.FeatureRequirement.OneOrMoreEnabled:
			return (this.enabledFeatures & targetFeatures) > MultiBlockManager.FeatureFlags.None;
		case MultiBlockManager.FeatureRequirement.AllDisabled:
			return (this.enabledFeatures & targetFeatures) == MultiBlockManager.FeatureFlags.None;
		}
		return (this.enabledFeatures & targetFeatures) == targetFeatures;
	}

	// Token: 0x17000810 RID: 2064
	// (get) Token: 0x06004EBB RID: 20155 RVA: 0x001F2DC8 File Offset: 0x001F0FC8
	public bool NoFeaturesEnabled
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.CheckFeatures(MultiBlockManager.FeatureFlags.All, MultiBlockManager.FeatureRequirement.AllDisabled);
		}
	}

	// Token: 0x06004EBC RID: 20156 RVA: 0x001F2DD4 File Offset: 0x001F0FD4
	[PublicizedFrom(EAccessModifier.Private)]
	public MultiBlockManager.FeatureFlags GetFeaturesForMode(MultiBlockManager.Mode mode)
	{
		MultiBlockManager.FeatureFlags result;
		switch (mode)
		{
		case MultiBlockManager.Mode.Normal:
			result = MultiBlockManager.FeatureFlags.All;
			break;
		case MultiBlockManager.Mode.WorldEditor:
			result = (MultiBlockManager.FeatureFlags.POIMBTracking | MultiBlockManager.FeatureFlags.CrossChunkMBTracking | MultiBlockManager.FeatureFlags.OversizedStability | MultiBlockManager.FeatureFlags.TerrainAlignment);
			break;
		case MultiBlockManager.Mode.PrefabPlaytest:
			result = (MultiBlockManager.FeatureFlags.POIMBTracking | MultiBlockManager.FeatureFlags.OversizedStability | MultiBlockManager.FeatureFlags.TerrainAlignment);
			break;
		case MultiBlockManager.Mode.PrefabEditor:
			result = (MultiBlockManager.FeatureFlags.POIMBTracking | MultiBlockManager.FeatureFlags.TerrainAlignment);
			break;
		case MultiBlockManager.Mode.Client:
			result = MultiBlockManager.FeatureFlags.TerrainAlignment;
			break;
		default:
			result = MultiBlockManager.FeatureFlags.None;
			break;
		}
		return result;
	}

	// Token: 0x17000811 RID: 2065
	// (get) Token: 0x06004EBD RID: 20157 RVA: 0x001F2E1B File Offset: 0x001F101B
	public bool POIMBTrackingEnabled
	{
		get
		{
			return this.CheckFeatures(MultiBlockManager.FeatureFlags.POIMBTracking, MultiBlockManager.FeatureRequirement.AllEnabled);
		}
	}

	// Token: 0x06004EBE RID: 20158 RVA: 0x001F2E28 File Offset: 0x001F1028
	public void Initialize(RegionFileManager regionFileManager)
	{
		object obj = this.lockObj;
		lock (obj)
		{
			this.regionFileManager = regionFileManager;
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				if (GameManager.Instance.IsEditMode())
				{
					this.currentMode = ((regionFileManager == null) ? MultiBlockManager.Mode.PrefabEditor : MultiBlockManager.Mode.WorldEditor);
				}
				else
				{
					this.currentMode = ((regionFileManager == null) ? MultiBlockManager.Mode.PrefabPlaytest : MultiBlockManager.Mode.Normal);
				}
			}
			else
			{
				this.currentMode = MultiBlockManager.Mode.Client;
			}
			this.enabledFeatures = this.GetFeaturesForMode(this.currentMode);
			this.world = GameManager.Instance.World;
			this.cc = this.world.ChunkCache;
			this.chunkManager = this.world.m_ChunkManager;
			if (this.CheckFeatures(MultiBlockManager.FeatureFlags.OversizedStability | MultiBlockManager.FeatureFlags.TerrainAlignment, MultiBlockManager.FeatureRequirement.OneOrMoreEnabled))
			{
				ChunkManager chunkManager = this.chunkManager;
				chunkManager.OnChunkInitialized = (Action<Chunk>)Delegate.Combine(chunkManager.OnChunkInitialized, new Action<Chunk>(this.OnChunkInitialized));
			}
			ChunkManager chunkManager2 = this.chunkManager;
			chunkManager2.OnChunkRegenerated = (Action<Chunk>)Delegate.Combine(chunkManager2.OnChunkRegenerated, new Action<Chunk>(this.OnChunkRegeneratedOrDisplayed));
			ChunkManager chunkManager3 = this.chunkManager;
			chunkManager3.OnChunkCopiedToUnity = (Action<Chunk>)Delegate.Combine(chunkManager3.OnChunkCopiedToUnity, new Action<Chunk>(this.OnChunkRegeneratedOrDisplayed));
			this.filePath = Path.Combine(GameIO.GetSaveGameDir(), "multiblocks.7dt");
			this.<Initialize>g__TryLoad|33_0();
			this.isDirty = false;
		}
	}

	// Token: 0x06004EBF RID: 20159 RVA: 0x001F2F9C File Offset: 0x001F119C
	public void Cleanup()
	{
		if (this.NoFeaturesEnabled)
		{
			return;
		}
		object obj = this.lockObj;
		lock (obj)
		{
			ChunkManager chunkManager = this.chunkManager;
			chunkManager.OnChunkInitialized = (Action<Chunk>)Delegate.Remove(chunkManager.OnChunkInitialized, new Action<Chunk>(this.OnChunkInitialized));
			ChunkManager chunkManager2 = this.chunkManager;
			chunkManager2.OnChunkRegenerated = (Action<Chunk>)Delegate.Remove(chunkManager2.OnChunkRegenerated, new Action<Chunk>(this.OnChunkRegeneratedOrDisplayed));
			ChunkManager chunkManager3 = this.chunkManager;
			chunkManager3.OnChunkCopiedToUnity = (Action<Chunk>)Delegate.Remove(chunkManager3.OnChunkCopiedToUnity, new Action<Chunk>(this.OnChunkRegeneratedOrDisplayed));
			this.SaveIfDirty();
			this.trackedDataMap.Clear();
			this.oversizedBlocksWithDirtyStability.Clear();
			this.blocksWithDirtyAlignment.Clear();
			this.deregisteredMultiBlockCount = 0;
			this.filePath = null;
			this.regionFileManager = null;
			this.cc = null;
			this.currentMode = MultiBlockManager.Mode.Disabled;
			this.enabledFeatures = MultiBlockManager.FeatureFlags.None;
		}
	}

	// Token: 0x06004EC0 RID: 20160 RVA: 0x001F30A8 File Offset: 0x001F12A8
	public void SaveIfDirty()
	{
		if (!this.CheckFeatures(MultiBlockManager.FeatureFlags.Serialization, MultiBlockManager.FeatureRequirement.AllEnabled))
		{
			return;
		}
		object obj = this.lockObj;
		lock (obj)
		{
			if (this.isDirty)
			{
				if (string.IsNullOrEmpty(this.filePath))
				{
					Log.Error("[MultiBlockManager] Failed to save MultiBlock data; MultiBlockManager has not been initialized with a valid filepath.");
				}
				else
				{
					this.CullCompletePOIPlacements();
					using (Stream stream = SdFile.OpenWrite(this.filePath))
					{
						using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
						{
							pooledBinaryWriter.SetBaseStream(stream);
							pooledBinaryWriter.Write(6);
							pooledBinaryWriter.Write(this.trackedDataMap.TrackedDataByPosition.Count);
							foreach (KeyValuePair<Vector3i, MultiBlockManager.TrackedBlockData> keyValuePair in this.trackedDataMap.TrackedDataByPosition)
							{
								StreamUtils.Write(pooledBinaryWriter, keyValuePair.Key);
								MultiBlockManager.TrackedBlockData value = keyValuePair.Value;
								pooledBinaryWriter.Write(value.rawData);
								pooledBinaryWriter.Write((byte)value.trackingTypeFlags);
							}
							stream.SetLength(stream.Position);
							pooledBinaryWriter.Flush();
						}
					}
					this.isDirty = false;
				}
			}
		}
	}

	// Token: 0x06004EC1 RID: 20161 RVA: 0x001F3240 File Offset: 0x001F1440
	public void CullChunklessData()
	{
		if (!this.CheckFeatures(MultiBlockManager.FeatureFlags.CrossChunkMBTracking, MultiBlockManager.FeatureRequirement.AllEnabled))
		{
			return;
		}
		object obj = this.lockObj;
		lock (obj)
		{
			foreach (KeyValuePair<Vector3i, MultiBlockManager.TrackedBlockData> keyValuePair in this.trackedDataMap.TrackedDataByPosition)
			{
				Vector3i key = keyValuePair.Key;
				RectInt flatChunkBounds = keyValuePair.Value.flatChunkBounds;
				if (!this.<CullChunklessData>g__AnyOverlappedChunkIsSyncedOrInSaveDir|36_0(flatChunkBounds))
				{
					this.keysToRemove.Enqueue(key);
				}
			}
			Vector3i worldPos;
			while (this.keysToRemove.TryDequeue(out worldPos))
			{
				this.DeregisterTrackedBlockDataInternal(worldPos);
			}
			this.ProcessDeregistrationCleanup();
		}
	}

	// Token: 0x06004EC2 RID: 20162 RVA: 0x001F330C File Offset: 0x001F150C
	public void UpdateTrackedBlockData(Vector3i worldPos, BlockValue blockValue, bool poiOwned)
	{
		if (this.NoFeaturesEnabled)
		{
			return;
		}
		object obj = this.lockObj;
		lock (obj)
		{
			MultiBlockManager.TrackingTypeFlags trackingTypeFlags = MultiBlockManager.TrackingTypeFlags.None;
			MultiBlockManager.TrackedBlockData trackedBlockData;
			if (this.trackedDataMap.TryGetValue(worldPos, out trackedBlockData))
			{
				if (trackedBlockData.rawData != blockValue.rawData)
				{
					this.DeregisterTrackedBlockDataInternal(worldPos);
					this.ProcessDeregistrationCleanup();
				}
				else
				{
					trackingTypeFlags = trackedBlockData.trackingTypeFlags;
					if (poiOwned)
					{
						if ((trackedBlockData.trackingTypeFlags & MultiBlockManager.TrackingTypeFlags.CrossChunkMultiBlock) != MultiBlockManager.TrackingTypeFlags.None)
						{
							this.trackedDataMap.RemoveTrackedData(worldPos, MultiBlockManager.TrackingTypeFlags.CrossChunkMultiBlock);
							this.deregisteredMultiBlockCount++;
							this.ProcessDeregistrationCleanup();
							trackingTypeFlags &= ~MultiBlockManager.TrackingTypeFlags.CrossChunkMultiBlock;
						}
					}
					else if ((trackedBlockData.trackingTypeFlags & MultiBlockManager.TrackingTypeFlags.PoiMultiBlock) != MultiBlockManager.TrackingTypeFlags.None)
					{
						this.trackedDataMap.RemoveTrackedData(worldPos, MultiBlockManager.TrackingTypeFlags.PoiMultiBlock);
						trackingTypeFlags &= ~MultiBlockManager.TrackingTypeFlags.PoiMultiBlock;
					}
				}
			}
			if (blockValue.Block.isMultiBlock)
			{
				if (poiOwned)
				{
					if ((trackingTypeFlags & MultiBlockManager.TrackingTypeFlags.PoiMultiBlock) == MultiBlockManager.TrackingTypeFlags.None)
					{
					}
				}
				else if ((trackingTypeFlags & MultiBlockManager.TrackingTypeFlags.CrossChunkMultiBlock) == MultiBlockManager.TrackingTypeFlags.None)
				{
					this.TryRegisterCrossChunkMultiBlock(worldPos, blockValue);
				}
			}
			if (blockValue.Block.isOversized && (trackingTypeFlags & MultiBlockManager.TrackingTypeFlags.OversizedBlock) == MultiBlockManager.TrackingTypeFlags.None)
			{
				this.TryRegisterOversizedBlock(worldPos, blockValue);
			}
			if (blockValue.Block.terrainAlignmentMode != TerrainAlignmentMode.None && (trackingTypeFlags & MultiBlockManager.TrackingTypeFlags.TerrainAlignedBlock) == MultiBlockManager.TrackingTypeFlags.None)
			{
				this.TryRegisterTerrainAlignedBlockInternal(worldPos, blockValue);
			}
		}
	}

	// Token: 0x06004EC3 RID: 20163 RVA: 0x001F343C File Offset: 0x001F163C
	public void DeregisterTrackedBlockData(Vector3i worldPos)
	{
		if (this.NoFeaturesEnabled)
		{
			return;
		}
		object obj = this.lockObj;
		lock (obj)
		{
			this.DeregisterTrackedBlockDataInternal(worldPos);
			this.ProcessDeregistrationCleanup();
		}
	}

	// Token: 0x06004EC4 RID: 20164 RVA: 0x001F348C File Offset: 0x001F168C
	[PublicizedFrom(EAccessModifier.Private)]
	public void DeregisterTrackedBlockDataInternal(Vector3i worldPos)
	{
		MultiBlockManager.TrackedBlockData trackedBlockData;
		if (this.trackedDataMap.TryGetValue(worldPos, out trackedBlockData))
		{
			if ((trackedBlockData.trackingTypeFlags & MultiBlockManager.TrackingTypeFlags.CrossChunkMultiBlock) != MultiBlockManager.TrackingTypeFlags.None)
			{
				this.deregisteredMultiBlockCount++;
			}
			this.trackedDataMap.RemoveTrackedData(worldPos, MultiBlockManager.TrackingTypeFlags.All);
			this.blocksWithDirtyAlignment.Remove(worldPos);
			this.oversizedBlocksWithDirtyStability.Remove(worldPos);
			this.isDirty = true;
		}
	}

	// Token: 0x06004EC5 RID: 20165 RVA: 0x001F34F0 File Offset: 0x001F16F0
	public void DeregisterTrackedBlockDatas(Bounds bounds)
	{
		if (this.NoFeaturesEnabled)
		{
			return;
		}
		object obj = this.lockObj;
		lock (obj)
		{
			List<Vector3i> list = null;
			foreach (Vector3i vector3i in this.trackedDataMap.TrackedDataByPosition.Keys)
			{
				if (bounds.Contains(vector3i))
				{
					if (list == null)
					{
						list = new List<Vector3i>();
					}
					list.Add(vector3i);
				}
			}
			if (list != null)
			{
				foreach (Vector3i worldPos in list)
				{
					this.DeregisterTrackedBlockDataInternal(worldPos);
				}
				this.ProcessDeregistrationCleanup();
			}
		}
	}

	// Token: 0x06004EC6 RID: 20166 RVA: 0x001F35E4 File Offset: 0x001F17E4
	public void MainThreadUpdate()
	{
		this.UpdateAlignment();
		this.UpdateOversizedStability();
	}

	// Token: 0x06004EC7 RID: 20167 RVA: 0x001F35F2 File Offset: 0x001F17F2
	public bool TryRegisterTerrainAlignedBlock(Vector3i worldPos, BlockValue blockValue)
	{
		return this.TryRegisterTerrainAlignedBlockInternal(worldPos, blockValue);
	}

	// Token: 0x06004EC8 RID: 20168 RVA: 0x001F35FC File Offset: 0x001F17FC
	[PublicizedFrom(EAccessModifier.Private)]
	public bool TryRegisterTerrainAlignedBlockInternal(Vector3i worldPos, BlockValue blockValue)
	{
		if (!this.CheckFeatures(MultiBlockManager.FeatureFlags.TerrainAlignment, MultiBlockManager.FeatureRequirement.AllEnabled))
		{
			return false;
		}
		if (blockValue.Block.terrainAlignmentMode == TerrainAlignmentMode.None)
		{
			Log.Error(string.Format("[MultiBlockManager] TryRegisterTerrainAlignedBlock failed: target block of type {0} at {1} is not a terrain-aligned block.", blockValue.Block.GetBlockName(), worldPos));
			return false;
		}
		if (blockValue.ischild)
		{
			Log.Error(string.Format("[MultiBlockManager] TryRegisterTerrainAlignedBlock failed: target block is not a parent at position {0}.", worldPos));
			return false;
		}
		object obj = this.lockObj;
		bool result;
		lock (obj)
		{
			MultiBlockManager.TrackedBlockData trackedBlockData;
			if (this.trackedDataMap.TryGetValue(worldPos, out trackedBlockData))
			{
				if (blockValue.rawData != trackedBlockData.rawData)
				{
					Log.Error(string.Format("Unexpected condition in TryRegisterTerrainAlignedBlock: encountered raw data mismatch at position {0}.", worldPos));
					result = false;
				}
				else if ((trackedBlockData.trackingTypeFlags & MultiBlockManager.TrackingTypeFlags.TerrainAlignedBlock) != MultiBlockManager.TrackingTypeFlags.None)
				{
					result = true;
				}
				else
				{
					this.<TryRegisterTerrainAlignedBlockInternal>g__RegisterTerrainAlignedBlock|43_0(worldPos, blockValue, trackedBlockData.flatChunkBounds);
					result = true;
				}
			}
			else
			{
				bool flag2 = false;
				if (blockValue.Block.isMultiBlock)
				{
					Vector3i v;
					Vector3i v2;
					MultiBlockManager.GetMinMaxWorldPositions(worldPos, blockValue, out v, out v2);
					Vector2i vector2i = World.toChunkXZ(v);
					Vector2i one = World.toChunkXZ(v2);
					RectInt flatChunkBounds = new RectInt(vector2i, one - vector2i);
					this.<TryRegisterTerrainAlignedBlockInternal>g__RegisterTerrainAlignedBlock|43_0(worldPos, blockValue, flatChunkBounds);
					flag2 = true;
				}
				if (blockValue.Block.isOversized)
				{
					Vector3i v3;
					Vector3i v4;
					OversizedBlockUtils.GetWorldAlignedBoundsExtents(worldPos, blockValue.Block.shape.GetRotation(blockValue), blockValue.Block.oversizedBounds, out v3, out v4);
					Vector2i vector2i2 = World.toChunkXZ(v3);
					Vector2i one2 = World.toChunkXZ(v4);
					RectInt flatChunkBounds2 = new RectInt(vector2i2, one2 - vector2i2);
					this.<TryRegisterTerrainAlignedBlockInternal>g__RegisterTerrainAlignedBlock|43_0(worldPos, blockValue, flatChunkBounds2);
					flag2 = true;
				}
				result = flag2;
			}
		}
		return result;
	}

	// Token: 0x06004EC9 RID: 20169 RVA: 0x001F37D0 File Offset: 0x001F19D0
	public void SetTerrainAlignmentDirty(Vector3i worldPos)
	{
		if (!this.CheckFeatures(MultiBlockManager.FeatureFlags.TerrainAlignment, MultiBlockManager.FeatureRequirement.AllEnabled))
		{
			return;
		}
		object obj = this.lockObj;
		lock (obj)
		{
			if (!this.blocksWithDirtyAlignment.Contains(worldPos))
			{
				if (!this.trackedDataMap.TerrainAlignedBlocks.ContainsKey(worldPos))
				{
					Log.Warning(string.Format("[MultiBlockManager][Alignment] SetTerrainAlignmentDirty failed; no terrain-aligned block has been registered at the specified world position: {0}", worldPos));
				}
				else
				{
					this.blocksWithDirtyAlignment.Add(worldPos);
				}
			}
		}
	}

	// Token: 0x06004ECA RID: 20170 RVA: 0x001F3860 File Offset: 0x001F1A60
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateAlignment()
	{
		if (!this.CheckFeatures(MultiBlockManager.FeatureFlags.TerrainAlignment, MultiBlockManager.FeatureRequirement.AllEnabled))
		{
			return;
		}
		object obj = this.lockObj;
		lock (obj)
		{
			foreach (Vector3i vector3i in this.blocksWithDirtyAlignment)
			{
				MultiBlockManager.TrackedBlockData blockData;
				if (!this.trackedDataMap.TerrainAlignedBlocks.TryGetValue(vector3i, out blockData))
				{
					Log.Warning(string.Format("[MultiBlockManager][Alignment] Failed to retrieve registered terrain-aligned block at expected location: {0}", vector3i));
				}
				else
				{
					this.<UpdateAlignment>g__TryAlignBlock|45_1(vector3i, blockData);
				}
			}
			this.blocksWithDirtyAlignment.Clear();
		}
	}

	// Token: 0x06004ECB RID: 20171 RVA: 0x001F3928 File Offset: 0x001F1B28
	public void CullChunklessDataOnClient(List<long> removedChunks)
	{
		MultiBlockManager.<>c__DisplayClass46_0 CS$<>8__locals1;
		CS$<>8__locals1.removedChunks = removedChunks;
		CS$<>8__locals1.<>4__this = this;
		if (this.currentMode != MultiBlockManager.Mode.Client)
		{
			return;
		}
		object obj = this.lockObj;
		lock (obj)
		{
			foreach (KeyValuePair<Vector3i, MultiBlockManager.TrackedBlockData> keyValuePair in this.trackedDataMap.TrackedDataByPosition)
			{
				Vector3i key = keyValuePair.Key;
				RectInt flatChunkBounds = keyValuePair.Value.flatChunkBounds;
				if (!this.<CullChunklessDataOnClient>g__AnyOverlappedChunkIsSynced|46_0(flatChunkBounds, ref CS$<>8__locals1))
				{
					this.keysToRemove.Enqueue(key);
				}
			}
			Vector3i worldPos;
			while (this.keysToRemove.TryDequeue(out worldPos))
			{
				this.DeregisterTrackedBlockDataInternal(worldPos);
			}
		}
	}

	// Token: 0x06004ECC RID: 20172 RVA: 0x001F3A00 File Offset: 0x001F1C00
	public bool TryRegisterPOIMultiBlock(Vector3i parentWorldPos, BlockValue blockValue)
	{
		if (!this.CheckFeatures(MultiBlockManager.FeatureFlags.POIMBTracking, MultiBlockManager.FeatureRequirement.AllEnabled))
		{
			return false;
		}
		object obj = this.lockObj;
		bool result;
		lock (obj)
		{
			MultiBlockManager.TrackedBlockData trackedBlockData;
			MultiBlockManager.TrackedBlockData trackedBlockData2;
			if (this.trackedDataMap.CrossChunkMultiBlocks.TryGetValue(parentWorldPos, out trackedBlockData))
			{
				Log.Error(string.Format("[MultiBlockManager] Failed to register POI multiblock at {0} due to previously registered CrossChunk data.", parentWorldPos) + string.Format("\nOld value: {0} ", trackedBlockData.rawData) + string.Format("\nNew value: {0}", blockValue.rawData));
				result = false;
			}
			else if (this.trackedDataMap.PoiMultiBlocks.TryGetValue(parentWorldPos, out trackedBlockData2))
			{
				Log.Error(string.Format("[MultiBlockManager] Duplicate multiblock placement at {0}. New value will not be applied.", parentWorldPos) + string.Format("\nOld value: {0} ", trackedBlockData2.rawData) + string.Format("\nNew value: {0}", blockValue.rawData));
				result = false;
			}
			else if (blockValue.ischild)
			{
				Log.Error("[MultiBlockManager] TryAddPOIMultiBlock failed: target block is not a parent.");
				result = false;
			}
			else
			{
				RectInt flatChunkBounds;
				if (!blockValue.Block.isMultiBlock)
				{
					Vector2i v2i = World.toChunkXZ(parentWorldPos);
					flatChunkBounds = new RectInt(v2i, Vector2Int.zero);
				}
				else
				{
					Vector3i v;
					Vector3i v2;
					MultiBlockManager.GetMinMaxWorldPositions(parentWorldPos, blockValue, out v, out v2);
					Vector2i vector2i = World.toChunkXZ(v);
					Vector2i one = World.toChunkXZ(v2);
					flatChunkBounds = new RectInt(vector2i, one - vector2i);
				}
				this.trackedDataMap.AddOrMergeTrackedData(parentWorldPos, blockValue.rawData, flatChunkBounds, MultiBlockManager.TrackingTypeFlags.PoiMultiBlock);
				this.isDirty = true;
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06004ECD RID: 20173 RVA: 0x001F3BBC File Offset: 0x001F1DBC
	public static void GetMinMaxWorldPositions(Vector3i parentWorldPos, BlockValue blockValue, out Vector3i minPos, out Vector3i maxPos)
	{
		minPos = parentWorldPos;
		maxPos = parentWorldPos;
		for (int i = 0; i < blockValue.Block.multiBlockPos.Length; i++)
		{
			Vector3i v = parentWorldPos + blockValue.Block.multiBlockPos.Get(i, blockValue.type, (int)blockValue.rotation);
			minPos = Vector3i.Min(minPos, v);
			maxPos = Vector3i.Max(maxPos, v);
		}
	}

	// Token: 0x06004ECE RID: 20174 RVA: 0x001F3C40 File Offset: 0x001F1E40
	public bool TryGetPOIMultiBlock(Vector3i parentWorldPos, out MultiBlockManager.TrackedBlockData poiMultiBlock)
	{
		if (!this.CheckFeatures(MultiBlockManager.FeatureFlags.POIMBTracking, MultiBlockManager.FeatureRequirement.AllEnabled))
		{
			poiMultiBlock = default(MultiBlockManager.TrackedBlockData);
			return false;
		}
		object obj = this.lockObj;
		bool result;
		lock (obj)
		{
			result = this.trackedDataMap.PoiMultiBlocks.TryGetValue(parentWorldPos, out poiMultiBlock);
		}
		return result;
	}

	// Token: 0x06004ECF RID: 20175 RVA: 0x001F3CA4 File Offset: 0x001F1EA4
	public void CullCompletePOIPlacements()
	{
		if (!this.CheckFeatures(MultiBlockManager.FeatureFlags.POIMBTracking | MultiBlockManager.FeatureFlags.Serialization, MultiBlockManager.FeatureRequirement.AllEnabled))
		{
			return;
		}
		object obj = this.lockObj;
		lock (obj)
		{
			foreach (KeyValuePair<Vector3i, MultiBlockManager.TrackedBlockData> keyValuePair in this.trackedDataMap.PoiMultiBlocks)
			{
				Vector3i key = keyValuePair.Key;
				RectInt flatChunkBounds = keyValuePair.Value.flatChunkBounds;
				if (this.<CullCompletePOIPlacements>g__AllOverlappedChunksAreSavedAndDormant|50_0(flatChunkBounds))
				{
					this.keysToRemove.Enqueue(key);
				}
			}
			Vector3i worldPos;
			while (this.keysToRemove.TryDequeue(out worldPos))
			{
				this.trackedDataMap.RemoveTrackedData(worldPos, MultiBlockManager.TrackingTypeFlags.PoiMultiBlock);
				this.isDirty = true;
			}
		}
	}

	// Token: 0x06004ED0 RID: 20176 RVA: 0x001F3D84 File Offset: 0x001F1F84
	[PublicizedFrom(EAccessModifier.Private)]
	public bool TryRegisterCrossChunkMultiBlock(Vector3i parentWorldPos, BlockValue parentBlockValue)
	{
		if (!this.CheckFeatures(MultiBlockManager.FeatureFlags.CrossChunkMBTracking, MultiBlockManager.FeatureRequirement.AllEnabled))
		{
			return false;
		}
		object obj = this.lockObj;
		bool result;
		lock (obj)
		{
			if (this.trackedDataMap.PoiMultiBlocks.ContainsKey(parentWorldPos))
			{
				result = false;
			}
			else if (!parentBlockValue.Block.isMultiBlock)
			{
				Log.Error(string.Format("[MultiBlockManager] TryRegisterCrossChunkMultiBlock failed: target block of type {0} at {1} is not a MultiBlock.", parentBlockValue.Block.GetBlockName(), parentWorldPos));
				result = false;
			}
			else if (parentBlockValue.ischild)
			{
				Log.Error("[MultiBlockManager] TryRegisterCrossChunkMultiBlock failed: target block is not a parent.");
				result = false;
			}
			else
			{
				Vector3 vector = parentBlockValue.Block.shape.GetRotation(parentBlockValue) * parentBlockValue.Block.multiBlockPos.dim;
				if (Mathf.Approximately(Mathf.Abs(vector.x), 1f) && Mathf.Approximately(Mathf.Abs(vector.z), 1f))
				{
					result = false;
				}
				else
				{
					Vector3i vector3i;
					Vector3i vector3i2;
					MultiBlockManager.GetMinMaxWorldPositions(parentWorldPos, parentBlockValue, out vector3i, out vector3i2);
					if (vector3i.x == vector3i2.x && vector3i.z == vector3i2.z)
					{
						result = false;
					}
					else
					{
						Vector2i vector2i = World.toChunkXZ(vector3i);
						Vector2i vector2i2 = World.toChunkXZ(vector3i2);
						if (vector2i == vector2i2)
						{
							result = false;
						}
						else
						{
							RectInt flatChunkBounds = new RectInt(vector2i, vector2i2 - vector2i);
							this.trackedDataMap.AddOrMergeTrackedData(parentWorldPos, parentBlockValue.rawData, flatChunkBounds, MultiBlockManager.TrackingTypeFlags.CrossChunkMultiBlock);
							this.isDirty = true;
							this.tempChunksToGroup.Clear();
							for (int i = flatChunkBounds.yMin; i <= flatChunkBounds.yMax; i++)
							{
								for (int j = flatChunkBounds.xMin; j <= flatChunkBounds.xMax; j++)
								{
									long item = WorldChunkCache.MakeChunkKey(j, i);
									this.tempChunksToGroup.Add(item);
								}
							}
							this.regionFileManager.AddGroupedChunks(this.tempChunksToGroup);
							this.tempChunksToGroup.Clear();
							result = true;
						}
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06004ED1 RID: 20177 RVA: 0x001F3FB4 File Offset: 0x001F21B4
	[PublicizedFrom(EAccessModifier.Private)]
	public void ProcessDeregistrationCleanup()
	{
		if (!this.CheckFeatures(MultiBlockManager.FeatureFlags.CrossChunkMBTracking, MultiBlockManager.FeatureRequirement.AllEnabled))
		{
			return;
		}
		if (this.deregisteredMultiBlockCount <= 20)
		{
			return;
		}
		this.regionFileManager.RebuildChunkGroupsFromPOIs();
		this.tempChunksToGroup.Clear();
		foreach (KeyValuePair<Vector3i, MultiBlockManager.TrackedBlockData> keyValuePair in this.trackedDataMap.CrossChunkMultiBlocks)
		{
			RectInt flatChunkBounds = keyValuePair.Value.flatChunkBounds;
			for (int i = flatChunkBounds.yMin; i <= flatChunkBounds.yMax; i++)
			{
				for (int j = flatChunkBounds.xMin; j <= flatChunkBounds.xMax; j++)
				{
					long item = WorldChunkCache.MakeChunkKey(j, i);
					this.tempChunksToGroup.Add(item);
				}
			}
			this.regionFileManager.AddGroupedChunks(this.tempChunksToGroup);
			this.tempChunksToGroup.Clear();
		}
		this.deregisteredMultiBlockCount = 0;
	}

	// Token: 0x06004ED2 RID: 20178 RVA: 0x001F40BC File Offset: 0x001F22BC
	[PublicizedFrom(EAccessModifier.Private)]
	public bool TryRegisterOversizedBlock(Vector3i worldPos, BlockValue blockValue)
	{
		if (!this.CheckFeatures(MultiBlockManager.FeatureFlags.OversizedStability, MultiBlockManager.FeatureRequirement.AllEnabled))
		{
			return false;
		}
		object obj = this.lockObj;
		lock (obj)
		{
			if (!blockValue.Block.isOversized)
			{
				Log.Error(string.Format("[MultiBlockManager] TryRegisterOversizedBlock failed: target block of type {0} at {1} is not an Oversized Block.", blockValue.Block.GetBlockName(), worldPos));
				return false;
			}
			Vector3i v;
			Vector3i v2;
			OversizedBlockUtils.GetWorldAlignedBoundsExtents(worldPos, blockValue.Block.shape.GetRotation(blockValue), blockValue.Block.oversizedBounds, out v, out v2);
			Vector2i vector2i = World.toChunkXZ(v);
			Vector2i one = World.toChunkXZ(v2);
			RectInt flatChunkBounds = new RectInt(vector2i, one - vector2i);
			this.trackedDataMap.AddOrMergeTrackedData(worldPos, blockValue.rawData, flatChunkBounds, MultiBlockManager.TrackingTypeFlags.OversizedBlock);
			this.oversizedBlocksWithDirtyStability.Add(worldPos);
			this.isDirty = true;
		}
		return true;
	}

	// Token: 0x06004ED3 RID: 20179 RVA: 0x001F41BC File Offset: 0x001F23BC
	public void SetOversizedStabilityDirty(Vector3i worldPos)
	{
		if (!this.CheckFeatures(MultiBlockManager.FeatureFlags.OversizedStability, MultiBlockManager.FeatureRequirement.AllEnabled))
		{
			return;
		}
		object obj = this.lockObj;
		lock (obj)
		{
			Vector2i key = World.toChunkXZ(worldPos);
			HashSet<Vector3i> hashSet;
			if (this.trackedDataMap.OversizedBlocksByChunkPosition.TryGetValue(key, out hashSet))
			{
				foreach (Vector3i vector3i in hashSet)
				{
					if (!this.oversizedBlocksWithDirtyStability.Contains(vector3i))
					{
						MultiBlockManager.TrackedBlockData trackedBlockData;
						if (!this.trackedDataMap.OversizedBlocks.TryGetValue(vector3i, out trackedBlockData))
						{
							Log.Error("Tracked data mismatch: Oversized block chunk bin contains position without valid oversized block data.");
						}
						else
						{
							BlockValue blockValue = new BlockValue(trackedBlockData.rawData);
							Quaternion rotation = blockValue.Block.shape.GetRotation(blockValue);
							Bounds localStabilityBounds = OversizedBlockUtils.GetLocalStabilityBounds(blockValue.Block.oversizedBounds, rotation);
							localStabilityBounds.extents += Vector3Int.one;
							Matrix4x4 blockWorldToLocalMatrix = OversizedBlockUtils.GetBlockWorldToLocalMatrix(vector3i, rotation);
							if (OversizedBlockUtils.IsBlockCenterWithinBounds(worldPos, localStabilityBounds, blockWorldToLocalMatrix))
							{
								this.oversizedBlocksWithDirtyStability.Add(vector3i);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06004ED4 RID: 20180 RVA: 0x001F4328 File Offset: 0x001F2528
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnChunkRegeneratedOrDisplayed(Chunk chunk)
	{
		if (!this.CheckFeatures(MultiBlockManager.FeatureFlags.TerrainAlignment, MultiBlockManager.FeatureRequirement.AllEnabled))
		{
			return;
		}
		Vector2i chunkPos = new Vector2i(chunk.X, chunk.Z);
		object obj = this.lockObj;
		lock (obj)
		{
			MultiBlockManager.AddChunkOverlappingBlocksToSet(chunkPos, this.trackedDataMap.TerrainAlignedBlocks, this.blocksWithDirtyAlignment);
		}
	}

	// Token: 0x06004ED5 RID: 20181 RVA: 0x001F4398 File Offset: 0x001F2598
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnChunkInitialized(Chunk chunk)
	{
		if (!this.CheckFeatures(MultiBlockManager.FeatureFlags.OversizedStability | MultiBlockManager.FeatureFlags.TerrainAlignment, MultiBlockManager.FeatureRequirement.OneOrMoreEnabled))
		{
			return;
		}
		Vector2i chunkPos = new Vector2i(chunk.X, chunk.Z);
		object obj = this.lockObj;
		lock (obj)
		{
			if (this.CheckFeatures(MultiBlockManager.FeatureFlags.OversizedStability, MultiBlockManager.FeatureRequirement.AllEnabled))
			{
				MultiBlockManager.AddChunkOverlappingBlocksToSet(chunkPos, this.trackedDataMap.OversizedBlocks, this.oversizedBlocksWithDirtyStability);
			}
			if (this.CheckFeatures(MultiBlockManager.FeatureFlags.TerrainAlignment, MultiBlockManager.FeatureRequirement.AllEnabled))
			{
				MultiBlockManager.AddChunkOverlappingBlocksToSet(chunkPos, this.trackedDataMap.TerrainAlignedBlocks, this.blocksWithDirtyAlignment);
			}
		}
	}

	// Token: 0x06004ED6 RID: 20182 RVA: 0x001F4434 File Offset: 0x001F2634
	[PublicizedFrom(EAccessModifier.Private)]
	public static void AddChunkOverlappingBlocksToSet(Vector2i chunkPos, MultiBlockManager.TrackedDataMap.SubsetAccessor blocksMap, HashSet<Vector3i> targetSet)
	{
		foreach (KeyValuePair<Vector3i, MultiBlockManager.TrackedBlockData> keyValuePair in blocksMap)
		{
			Vector3i key = keyValuePair.Key;
			if (!targetSet.Contains(key))
			{
				RectInt flatChunkBounds = keyValuePair.Value.flatChunkBounds;
				flatChunkBounds.max += Vector2Int.one;
				if (flatChunkBounds.Contains(chunkPos))
				{
					targetSet.Add(key);
				}
			}
		}
	}

	// Token: 0x06004ED7 RID: 20183 RVA: 0x001F44C8 File Offset: 0x001F26C8
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateOversizedStability()
	{
		if (!this.CheckFeatures(MultiBlockManager.FeatureFlags.OversizedStability, MultiBlockManager.FeatureRequirement.AllEnabled))
		{
			return;
		}
		object obj = this.lockObj;
		lock (obj)
		{
			foreach (Vector3i vector3i in this.oversizedBlocksWithDirtyStability)
			{
				MultiBlockManager.TrackedBlockData blockData;
				if (!this.trackedDataMap.OversizedBlocks.TryGetValue(vector3i, out blockData))
				{
					Log.Warning(string.Format("[MultiBlockManager][Stability] Failed to retrieve registered Oversized Block at expected location: {0}", vector3i));
				}
				else if (!this.<UpdateOversizedStability>g__IsOversizedBlockStable|58_1(vector3i, blockData))
				{
					GameManager.Instance.World.AddFallingBlock(vector3i, true);
				}
			}
			this.oversizedBlocksWithDirtyStability.Clear();
		}
	}

	// Token: 0x06004ED8 RID: 20184 RVA: 0x001F45A0 File Offset: 0x001F27A0
	[Conditional("MBM_ENABLE_GENERAL_LOG")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void DebugLogGeneral(string message)
	{
		Log.Out(message);
	}

	// Token: 0x06004ED9 RID: 20185 RVA: 0x001F45A0 File Offset: 0x001F27A0
	[Conditional("MBM_ENABLE_PLACEMENT_LOG")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void DebugLogPlacement(string message)
	{
		Log.Out(message);
	}

	// Token: 0x06004EDA RID: 20186 RVA: 0x001F45A0 File Offset: 0x001F27A0
	[Conditional("MBM_ENABLE_REGISTRATION_LOG")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void DebugLogRegistration(string message)
	{
		Log.Out(message);
	}

	// Token: 0x06004EDB RID: 20187 RVA: 0x001F45A0 File Offset: 0x001F27A0
	[Conditional("MBM_ENABLE_STABILITY_LOG")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void DebugLogStability(string message)
	{
		Log.Out(message);
	}

	// Token: 0x06004EDC RID: 20188 RVA: 0x001F45A0 File Offset: 0x001F27A0
	[Conditional("MBM_ENABLE_ALIGNMENT_LOG")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void DebugLogAlignment(string message)
	{
		Log.Out(message);
	}

	// Token: 0x06004EDD RID: 20189 RVA: 0x00002914 File Offset: 0x00000B14
	[Conditional("MBM_ENABLE_PROFILER_MARKERS")]
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateProfilerCounters()
	{
	}

	// Token: 0x06004EDE RID: 20190 RVA: 0x001F45A8 File Offset: 0x001F27A8
	[Conditional("MBM_ENABLED_SANITY_CHECKS")]
	[PublicizedFrom(EAccessModifier.Private)]
	public void DoCurrentModeSanityChecks()
	{
		MultiBlockManager.Mode mode = MultiBlockManager.Mode.Client;
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			if (GameManager.Instance.IsEditMode())
			{
				mode = ((this.regionFileManager == null) ? MultiBlockManager.Mode.PrefabEditor : MultiBlockManager.Mode.WorldEditor);
			}
			else
			{
				mode = ((this.regionFileManager == null) ? MultiBlockManager.Mode.PrefabPlaytest : MultiBlockManager.Mode.Normal);
			}
		}
		if (this.currentMode != mode)
		{
			Log.Error("[MultiBlockManager] Unexpected mode state. \n" + string.Format("Current mode: {0}. Expected mode: {1}. \n", this.currentMode, mode) + string.Format("GameManager.Instance.IsEditMode(): {0}, ", GameManager.Instance.IsEditMode()) + string.Format("ConnectionManager.Instance.IsServer: {0}, ", SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer));
		}
	}

	// Token: 0x06004EDF RID: 20191 RVA: 0x001F4650 File Offset: 0x001F2850
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Private)]
	public bool <Initialize>g__TryLoad|33_0()
	{
		this.trackedDataMap.Clear();
		this.deregisteredMultiBlockCount = 0;
		if (!this.CheckFeatures(MultiBlockManager.FeatureFlags.Serialization, MultiBlockManager.FeatureRequirement.AllEnabled))
		{
			return false;
		}
		if (!SdFile.Exists(this.filePath))
		{
			return false;
		}
		bool result;
		using (Stream stream = SdFile.OpenRead(this.filePath))
		{
			using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
			{
				pooledBinaryReader.SetBaseStream(stream);
				byte b = pooledBinaryReader.ReadByte();
				if (b != 6)
				{
					Log.Error(string.Format("[MultiBlockManager] Saved MultiBlock data is out of date. Saved version is ({0}). Current version is ({1}). ", b, 6) + "This data is no longer compatible and will be ignored. MultiBlock-related bugs are likely to occur if you continue with this save. Please start a fresh game to avoid these issues.");
					result = false;
				}
				else
				{
					int num = pooledBinaryReader.ReadInt32();
					for (int i = 0; i < num; i++)
					{
						Vector3i vector3i = StreamUtils.ReadVector3i(pooledBinaryReader);
						uint rawData = pooledBinaryReader.ReadUInt32();
						BlockValue blockValue = new BlockValue(rawData);
						byte b2 = pooledBinaryReader.ReadByte();
						if ((b2 & 1) != 0)
						{
							this.TryRegisterPOIMultiBlock(vector3i, blockValue);
						}
						if ((b2 & 2) != 0)
						{
							this.TryRegisterCrossChunkMultiBlock(vector3i, blockValue);
						}
						if ((b2 & 4) != 0)
						{
							this.TryRegisterOversizedBlock(vector3i, blockValue);
						}
						if ((b2 & 8) != 0)
						{
							this.TryRegisterTerrainAlignedBlockInternal(vector3i, blockValue);
						}
					}
					result = true;
				}
			}
		}
		return result;
	}

	// Token: 0x06004EE0 RID: 20192 RVA: 0x001F4790 File Offset: 0x001F2990
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Private)]
	public bool <CullChunklessData>g__AnyOverlappedChunkIsSyncedOrInSaveDir|36_0(RectInt flatChunkBounds)
	{
		for (int i = flatChunkBounds.yMin; i <= flatChunkBounds.yMax; i++)
		{
			for (int j = flatChunkBounds.xMin; j <= flatChunkBounds.xMax; j++)
			{
				long key = WorldChunkCache.MakeChunkKey(j, i);
				if (this.regionFileManager.ContainsChunkSync(key) || this.cc.ContainsChunkSync(key))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06004EE1 RID: 20193 RVA: 0x001F47F4 File Offset: 0x001F29F4
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Private)]
	public void <TryRegisterTerrainAlignedBlockInternal>g__RegisterTerrainAlignedBlock|43_0(Vector3i worldPos, BlockValue blockValue, RectInt flatChunkBounds)
	{
		this.trackedDataMap.AddOrMergeTrackedData(worldPos, blockValue.rawData, flatChunkBounds, MultiBlockManager.TrackingTypeFlags.TerrainAlignedBlock);
		this.blocksWithDirtyAlignment.Add(worldPos);
		this.isDirty = true;
	}

	// Token: 0x06004EE2 RID: 20194 RVA: 0x001F4820 File Offset: 0x001F2A20
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Private)]
	public bool <UpdateAlignment>g__AllOverlappedChunksAreReady|45_0(RectInt flatChunkBounds)
	{
		for (int i = flatChunkBounds.yMin; i <= flatChunkBounds.yMax; i++)
		{
			for (int j = flatChunkBounds.xMin; j <= flatChunkBounds.xMax; j++)
			{
				long key = WorldChunkCache.MakeChunkKey(j, i);
				Chunk chunkSync = this.cc.GetChunkSync(key);
				if (chunkSync == null || !chunkSync.IsInitialized || chunkSync.NeedsRegeneration || chunkSync.InProgressRegeneration || chunkSync.NeedsCopying || chunkSync.InProgressCopying)
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x06004EE3 RID: 20195 RVA: 0x001F48A4 File Offset: 0x001F2AA4
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Private)]
	public bool <UpdateAlignment>g__TryAlignBlock|45_1(Vector3i worldPos, MultiBlockManager.TrackedBlockData blockData)
	{
		RectInt flatChunkBounds = blockData.flatChunkBounds;
		if (!this.<UpdateAlignment>g__AllOverlappedChunksAreReady|45_0(flatChunkBounds))
		{
			return false;
		}
		BlockEntityData blockEntity = ((Chunk)this.world.GetChunkFromWorldPos(worldPos)).GetBlockEntity(worldPos);
		BlockValue blockValue = new BlockValue(blockData.rawData);
		Block block = blockValue.Block;
		TerrainAlignmentMode terrainAlignmentMode = block.terrainAlignmentMode;
		if (terrainAlignmentMode != TerrainAlignmentMode.None && terrainAlignmentMode - TerrainAlignmentMode.Vehicle <= 1)
		{
			return TerrainAlignmentUtils.AlignToTerrain(block, worldPos, blockValue, blockEntity, block.terrainAlignmentMode);
		}
		Log.Error(string.Format("[MultiBlockManager][Alignment] TryAlignBlock cannot align block with TerrainAlignmentMode \"{0}\" of type {1} at {2}", block.terrainAlignmentMode, block.GetBlockName(), worldPos));
		return false;
	}

	// Token: 0x06004EE4 RID: 20196 RVA: 0x001F493C File Offset: 0x001F2B3C
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Private)]
	public bool <CullChunklessDataOnClient>g__AnyOverlappedChunkIsSynced|46_0(RectInt flatChunkBounds, ref MultiBlockManager.<>c__DisplayClass46_0 A_2)
	{
		for (int i = flatChunkBounds.yMin; i <= flatChunkBounds.yMax; i++)
		{
			for (int j = flatChunkBounds.xMin; j <= flatChunkBounds.xMax; j++)
			{
				long num = WorldChunkCache.MakeChunkKey(j, i);
				if (!A_2.removedChunks.Contains(num) && this.cc.ContainsChunkSync(num))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06004EE5 RID: 20197 RVA: 0x001F49A0 File Offset: 0x001F2BA0
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Private)]
	public bool <CullCompletePOIPlacements>g__AllOverlappedChunksAreSavedAndDormant|50_0(RectInt flatChunkBounds)
	{
		for (int i = flatChunkBounds.yMin; i <= flatChunkBounds.yMax; i++)
		{
			for (int j = flatChunkBounds.xMin; j <= flatChunkBounds.xMax; j++)
			{
				long key = WorldChunkCache.MakeChunkKey(j, i);
				if (this.cc.ContainsChunkSync(key) || !this.regionFileManager.IsChunkSavedAndDormant(key))
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x06004EE6 RID: 20198 RVA: 0x001F4A04 File Offset: 0x001F2C04
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Private)]
	public bool <UpdateOversizedStability>g__AllOverlappedChunksAreSyncedAndInitialized|58_0(RectInt flatChunkBounds)
	{
		for (int i = flatChunkBounds.yMin; i <= flatChunkBounds.yMax; i++)
		{
			for (int j = flatChunkBounds.xMin; j <= flatChunkBounds.xMax; j++)
			{
				long key = WorldChunkCache.MakeChunkKey(j, i);
				Chunk chunkSync = this.cc.GetChunkSync(key);
				if (chunkSync == null || !chunkSync.IsInitialized)
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x06004EE7 RID: 20199 RVA: 0x001F4A64 File Offset: 0x001F2C64
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Private)]
	public bool <UpdateOversizedStability>g__IsOversizedBlockStable|58_1(Vector3i worldPos, MultiBlockManager.TrackedBlockData blockData)
	{
		RectInt flatChunkBounds = blockData.flatChunkBounds;
		if (!this.<UpdateOversizedStability>g__AllOverlappedChunksAreSyncedAndInitialized|58_0(flatChunkBounds))
		{
			return true;
		}
		BlockValue blockValue = new BlockValue(blockData.rawData);
		Quaternion rotation = blockValue.Block.shape.GetRotation(blockValue);
		Bounds localStabilityBounds = OversizedBlockUtils.GetLocalStabilityBounds(blockValue.Block.oversizedBounds, rotation);
		Vector3i vector3i;
		Vector3i vector3i2;
		OversizedBlockUtils.GetWorldAlignedBoundsExtents(worldPos, rotation, localStabilityBounds, out vector3i, out vector3i2);
		Matrix4x4 blockWorldToLocalMatrix = OversizedBlockUtils.GetBlockWorldToLocalMatrix(worldPos, rotation);
		World world = GameManager.Instance.World;
		for (int i = vector3i.x; i <= vector3i2.x; i++)
		{
			for (int j = vector3i.y; j <= vector3i2.y; j++)
			{
				for (int k = vector3i.z; k <= vector3i2.z; k++)
				{
					Vector3i vector3i3 = new Vector3i(i, j, k);
					if (OversizedBlockUtils.IsBlockCenterWithinBounds(vector3i3, localStabilityBounds, blockWorldToLocalMatrix) && world.GetStability(vector3i3) > 1)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x04003C5B RID: 15451
	[PublicizedFrom(EAccessModifier.Private)]
	public const byte FILEVERSION = 6;

	// Token: 0x04003C5C RID: 15452
	[PublicizedFrom(EAccessModifier.Private)]
	public const int c_deregisteredMultiBlockLimit = 20;

	// Token: 0x04003C5D RID: 15453
	[PublicizedFrom(EAccessModifier.Private)]
	public object lockObj = new object();

	// Token: 0x04003C5E RID: 15454
	[PublicizedFrom(EAccessModifier.Private)]
	public static MultiBlockManager m_Instance;

	// Token: 0x04003C5F RID: 15455
	[PublicizedFrom(EAccessModifier.Private)]
	public RegionFileManager regionFileManager;

	// Token: 0x04003C60 RID: 15456
	[PublicizedFrom(EAccessModifier.Private)]
	public MultiBlockManager.TrackedDataMap trackedDataMap = new MultiBlockManager.TrackedDataMap();

	// Token: 0x04003C61 RID: 15457
	[PublicizedFrom(EAccessModifier.Private)]
	public HashSet<Vector3i> oversizedBlocksWithDirtyStability = new HashSet<Vector3i>();

	// Token: 0x04003C62 RID: 15458
	[PublicizedFrom(EAccessModifier.Private)]
	public HashSet<Vector3i> blocksWithDirtyAlignment = new HashSet<Vector3i>();

	// Token: 0x04003C63 RID: 15459
	[PublicizedFrom(EAccessModifier.Private)]
	public Queue<Vector3i> keysToRemove = new Queue<Vector3i>();

	// Token: 0x04003C64 RID: 15460
	[PublicizedFrom(EAccessModifier.Private)]
	public List<long> tempChunksToGroup = new List<long>();

	// Token: 0x04003C65 RID: 15461
	[PublicizedFrom(EAccessModifier.Private)]
	public World world;

	// Token: 0x04003C66 RID: 15462
	[PublicizedFrom(EAccessModifier.Private)]
	public ChunkCluster cc;

	// Token: 0x04003C67 RID: 15463
	[PublicizedFrom(EAccessModifier.Private)]
	public ChunkManager chunkManager;

	// Token: 0x04003C68 RID: 15464
	[PublicizedFrom(EAccessModifier.Private)]
	public string filePath;

	// Token: 0x04003C69 RID: 15465
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;

	// Token: 0x04003C6A RID: 15466
	[PublicizedFrom(EAccessModifier.Private)]
	public int deregisteredMultiBlockCount;

	// Token: 0x04003C6B RID: 15467
	[PublicizedFrom(EAccessModifier.Private)]
	public MultiBlockManager.Mode currentMode;

	// Token: 0x04003C6C RID: 15468
	[PublicizedFrom(EAccessModifier.Private)]
	public MultiBlockManager.FeatureFlags enabledFeatures;

	// Token: 0x02000A0C RID: 2572
	[Flags]
	public enum TrackingTypeFlags : byte
	{
		// Token: 0x04003C6E RID: 15470
		None = 0,
		// Token: 0x04003C6F RID: 15471
		PoiMultiBlock = 1,
		// Token: 0x04003C70 RID: 15472
		CrossChunkMultiBlock = 2,
		// Token: 0x04003C71 RID: 15473
		OversizedBlock = 4,
		// Token: 0x04003C72 RID: 15474
		TerrainAlignedBlock = 8,
		// Token: 0x04003C73 RID: 15475
		All = 15
	}

	// Token: 0x02000A0D RID: 2573
	public struct TrackedBlockData
	{
		// Token: 0x06004EE8 RID: 20200 RVA: 0x001F4B53 File Offset: 0x001F2D53
		public TrackedBlockData(uint rawData, RectInt flatChunkBounds, MultiBlockManager.TrackingTypeFlags trackingTypeFlags)
		{
			this.rawData = rawData;
			this.flatChunkBounds = flatChunkBounds;
			this.trackingTypeFlags = trackingTypeFlags;
		}

		// Token: 0x04003C74 RID: 15476
		public readonly uint rawData;

		// Token: 0x04003C75 RID: 15477
		public readonly RectInt flatChunkBounds;

		// Token: 0x04003C76 RID: 15478
		public readonly MultiBlockManager.TrackingTypeFlags trackingTypeFlags;
	}

	// Token: 0x02000A0E RID: 2574
	[PublicizedFrom(EAccessModifier.Private)]
	public class TrackedDataMap
	{
		// Token: 0x06004EE9 RID: 20201 RVA: 0x001F4B6C File Offset: 0x001F2D6C
		public TrackedDataMap()
		{
			this.TrackedDataByPosition = new ReadOnlyDictionary<Vector3i, MultiBlockManager.TrackedBlockData>(this.trackedDataByPosition);
			this.OversizedBlocksByChunkPosition = new ReadOnlyDictionary<Vector2i, HashSet<Vector3i>>(this.oversizedBlocksByChunkPosition);
		}

		// Token: 0x06004EEA RID: 20202 RVA: 0x001F4BE3 File Offset: 0x001F2DE3
		public bool ContainsKey(Vector3i key)
		{
			return this.trackedDataByPosition.ContainsKey(key);
		}

		// Token: 0x17000812 RID: 2066
		// (get) Token: 0x06004EEB RID: 20203 RVA: 0x001F4BF1 File Offset: 0x001F2DF1
		public int Count
		{
			get
			{
				return this.trackedDataByPosition.Count;
			}
		}

		// Token: 0x06004EEC RID: 20204 RVA: 0x001F4BFE File Offset: 0x001F2DFE
		public bool TryGetValue(Vector3i key, out MultiBlockManager.TrackedBlockData value)
		{
			return this.trackedDataByPosition.TryGetValue(key, out value);
		}

		// Token: 0x06004EED RID: 20205 RVA: 0x001F4C10 File Offset: 0x001F2E10
		public void Clear()
		{
			this.trackedDataByPosition.Clear();
			this.poiMultiBlocks.Clear();
			this.crossChunkMultiBlocks.Clear();
			this.oversizedBlocks.Clear();
			this.terrainAlignedBlocks.Clear();
			this.oversizedBlocksByChunkPosition.Clear();
		}

		// Token: 0x06004EEE RID: 20206 RVA: 0x001F4C60 File Offset: 0x001F2E60
		public void AddOrMergeTrackedData(Vector3i worldPos, uint rawData, RectInt flatChunkBounds, MultiBlockManager.TrackingTypeFlags trackingTypeFlags)
		{
			if (trackingTypeFlags == MultiBlockManager.TrackingTypeFlags.None)
			{
				Log.Error(string.Format("AddOrMergeTrackedData failed: Cannot add or merge tracked data with no tracking flags set. At position {0}.", worldPos));
				return;
			}
			MultiBlockManager.TrackedBlockData trackedBlockData;
			MultiBlockManager.TrackingTypeFlags trackingTypeFlags2;
			if (this.trackedDataByPosition.TryGetValue(worldPos, out trackedBlockData))
			{
				if (rawData != trackedBlockData.rawData)
				{
					Log.Warning(string.Format("Unexpected condition in AddOrMergeTrackedData: encountered raw data mismatch at position {0}.", worldPos));
				}
				RectInt flatChunkBounds2 = flatChunkBounds;
				if (!flatChunkBounds2.Equals(trackedBlockData.flatChunkBounds))
				{
					flatChunkBounds2.SetMinMax(Vector2Int.Min(flatChunkBounds2.min, trackedBlockData.flatChunkBounds.min), Vector2Int.Max(flatChunkBounds2.max, trackedBlockData.flatChunkBounds.max));
					if ((trackedBlockData.trackingTypeFlags & MultiBlockManager.TrackingTypeFlags.OversizedBlock) != MultiBlockManager.TrackingTypeFlags.None)
					{
						this.AddOversizedBlockToChunkBins(worldPos);
					}
				}
				trackingTypeFlags2 = (trackingTypeFlags & ~trackedBlockData.trackingTypeFlags);
				if (trackingTypeFlags2 != trackingTypeFlags)
				{
					Log.Warning(string.Format("Unexpected condition in AddOrMergeTrackedData: tracked data already has one or more target flag(s) set at position {0}.", worldPos));
				}
				MultiBlockManager.TrackingTypeFlags trackingTypeFlags3 = trackingTypeFlags | trackedBlockData.trackingTypeFlags;
				this.trackedDataByPosition[worldPos] = new MultiBlockManager.TrackedBlockData(rawData, flatChunkBounds2, trackingTypeFlags3);
			}
			else
			{
				trackingTypeFlags2 = trackingTypeFlags;
				this.trackedDataByPosition[worldPos] = new MultiBlockManager.TrackedBlockData(rawData, flatChunkBounds, trackingTypeFlags);
			}
			if (trackingTypeFlags2 == MultiBlockManager.TrackingTypeFlags.None)
			{
				return;
			}
			if ((trackingTypeFlags2 & MultiBlockManager.TrackingTypeFlags.PoiMultiBlock) != MultiBlockManager.TrackingTypeFlags.None)
			{
				this.poiMultiBlocks.Add(worldPos);
			}
			if ((trackingTypeFlags2 & MultiBlockManager.TrackingTypeFlags.CrossChunkMultiBlock) != MultiBlockManager.TrackingTypeFlags.None)
			{
				this.crossChunkMultiBlocks.Add(worldPos);
			}
			if ((trackingTypeFlags2 & MultiBlockManager.TrackingTypeFlags.OversizedBlock) != MultiBlockManager.TrackingTypeFlags.None)
			{
				this.oversizedBlocks.Add(worldPos);
				this.AddOversizedBlockToChunkBins(worldPos);
			}
			if ((trackingTypeFlags2 & MultiBlockManager.TrackingTypeFlags.TerrainAlignedBlock) != MultiBlockManager.TrackingTypeFlags.None)
			{
				this.terrainAlignedBlocks.Add(worldPos);
			}
		}

		// Token: 0x06004EEF RID: 20207 RVA: 0x001F4DC8 File Offset: 0x001F2FC8
		[PublicizedFrom(EAccessModifier.Private)]
		public void AddOversizedBlockToChunkBins(Vector3i worldPos)
		{
			MultiBlockManager.TrackedBlockData trackedBlockData;
			if (!this.trackedDataByPosition.TryGetValue(worldPos, out trackedBlockData))
			{
				return;
			}
			RectInt flatChunkBounds = trackedBlockData.flatChunkBounds;
			for (int i = flatChunkBounds.min.y; i <= flatChunkBounds.max.y; i++)
			{
				for (int j = flatChunkBounds.min.x; j <= flatChunkBounds.max.x; j++)
				{
					Vector2i key = new Vector2i(j, i);
					HashSet<Vector3i> hashSet;
					if (!this.oversizedBlocksByChunkPosition.TryGetValue(key, out hashSet))
					{
						hashSet = new HashSet<Vector3i>();
						this.oversizedBlocksByChunkPosition[key] = hashSet;
					}
					hashSet.Add(worldPos);
				}
			}
		}

		// Token: 0x06004EF0 RID: 20208 RVA: 0x001F4E7C File Offset: 0x001F307C
		[PublicizedFrom(EAccessModifier.Private)]
		public void RemoveOversizedBlockFromChunkBins(Vector3i worldPos)
		{
			MultiBlockManager.TrackedBlockData trackedBlockData;
			if (!this.trackedDataByPosition.TryGetValue(worldPos, out trackedBlockData))
			{
				return;
			}
			RectInt flatChunkBounds = trackedBlockData.flatChunkBounds;
			for (int i = flatChunkBounds.min.y; i <= flatChunkBounds.max.y; i++)
			{
				for (int j = flatChunkBounds.min.x; j <= flatChunkBounds.max.x; j++)
				{
					Vector2i key = new Vector2i(j, i);
					HashSet<Vector3i> hashSet;
					if (this.oversizedBlocksByChunkPosition.TryGetValue(key, out hashSet))
					{
						hashSet.Remove(worldPos);
					}
				}
			}
		}

		// Token: 0x06004EF1 RID: 20209 RVA: 0x001F4F1C File Offset: 0x001F311C
		public void RemoveTrackedData(Vector3i worldPos, MultiBlockManager.TrackingTypeFlags flagsToRemove)
		{
			MultiBlockManager.TrackedBlockData trackedBlockData;
			if (!this.trackedDataByPosition.TryGetValue(worldPos, out trackedBlockData))
			{
				Log.Error(string.Format("RemoveTrackedData failed; no tracked data at position {0}.", worldPos));
				return;
			}
			MultiBlockManager.TrackingTypeFlags trackingTypeFlags = flagsToRemove & trackedBlockData.trackingTypeFlags;
			if (trackingTypeFlags == MultiBlockManager.TrackingTypeFlags.None)
			{
				Log.Error(string.Format("RemoveTrackedData failed; tracked data at position {0} does not have the target flag(s).", worldPos));
				return;
			}
			if ((trackingTypeFlags & MultiBlockManager.TrackingTypeFlags.PoiMultiBlock) != MultiBlockManager.TrackingTypeFlags.None)
			{
				this.poiMultiBlocks.Remove(worldPos);
			}
			if ((trackingTypeFlags & MultiBlockManager.TrackingTypeFlags.CrossChunkMultiBlock) != MultiBlockManager.TrackingTypeFlags.None)
			{
				this.crossChunkMultiBlocks.Remove(worldPos);
			}
			if ((trackingTypeFlags & MultiBlockManager.TrackingTypeFlags.OversizedBlock) != MultiBlockManager.TrackingTypeFlags.None)
			{
				this.RemoveOversizedBlockFromChunkBins(worldPos);
				this.oversizedBlocks.Remove(worldPos);
			}
			if ((trackingTypeFlags & MultiBlockManager.TrackingTypeFlags.TerrainAlignedBlock) != MultiBlockManager.TrackingTypeFlags.None)
			{
				this.terrainAlignedBlocks.Remove(worldPos);
			}
			MultiBlockManager.TrackingTypeFlags trackingTypeFlags2 = trackedBlockData.trackingTypeFlags & ~flagsToRemove;
			if (trackingTypeFlags2 == MultiBlockManager.TrackingTypeFlags.None)
			{
				this.trackedDataByPosition.Remove(worldPos);
				return;
			}
			this.trackedDataByPosition[worldPos] = new MultiBlockManager.TrackedBlockData(trackedBlockData.rawData, trackedBlockData.flatChunkBounds, trackingTypeFlags2);
		}

		// Token: 0x17000813 RID: 2067
		// (get) Token: 0x06004EF2 RID: 20210 RVA: 0x001F4FFA File Offset: 0x001F31FA
		public MultiBlockManager.TrackedDataMap.SubsetAccessor PoiMultiBlocks
		{
			get
			{
				return new MultiBlockManager.TrackedDataMap.SubsetAccessor(this.trackedDataByPosition, this.poiMultiBlocks);
			}
		}

		// Token: 0x17000814 RID: 2068
		// (get) Token: 0x06004EF3 RID: 20211 RVA: 0x001F500D File Offset: 0x001F320D
		public MultiBlockManager.TrackedDataMap.SubsetAccessor CrossChunkMultiBlocks
		{
			get
			{
				return new MultiBlockManager.TrackedDataMap.SubsetAccessor(this.trackedDataByPosition, this.crossChunkMultiBlocks);
			}
		}

		// Token: 0x17000815 RID: 2069
		// (get) Token: 0x06004EF4 RID: 20212 RVA: 0x001F5020 File Offset: 0x001F3220
		public MultiBlockManager.TrackedDataMap.SubsetAccessor OversizedBlocks
		{
			get
			{
				return new MultiBlockManager.TrackedDataMap.SubsetAccessor(this.trackedDataByPosition, this.oversizedBlocks);
			}
		}

		// Token: 0x17000816 RID: 2070
		// (get) Token: 0x06004EF5 RID: 20213 RVA: 0x001F5033 File Offset: 0x001F3233
		public MultiBlockManager.TrackedDataMap.SubsetAccessor TerrainAlignedBlocks
		{
			get
			{
				return new MultiBlockManager.TrackedDataMap.SubsetAccessor(this.trackedDataByPosition, this.terrainAlignedBlocks);
			}
		}

		// Token: 0x04003C77 RID: 15479
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<Vector3i, MultiBlockManager.TrackedBlockData> trackedDataByPosition = new Dictionary<Vector3i, MultiBlockManager.TrackedBlockData>();

		// Token: 0x04003C78 RID: 15480
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly HashSet<Vector3i> poiMultiBlocks = new HashSet<Vector3i>();

		// Token: 0x04003C79 RID: 15481
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly HashSet<Vector3i> crossChunkMultiBlocks = new HashSet<Vector3i>();

		// Token: 0x04003C7A RID: 15482
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly HashSet<Vector3i> oversizedBlocks = new HashSet<Vector3i>();

		// Token: 0x04003C7B RID: 15483
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly HashSet<Vector3i> terrainAlignedBlocks = new HashSet<Vector3i>();

		// Token: 0x04003C7C RID: 15484
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<Vector2i, HashSet<Vector3i>> oversizedBlocksByChunkPosition = new Dictionary<Vector2i, HashSet<Vector3i>>();

		// Token: 0x04003C7D RID: 15485
		public readonly ReadOnlyDictionary<Vector3i, MultiBlockManager.TrackedBlockData> TrackedDataByPosition;

		// Token: 0x04003C7E RID: 15486
		public readonly ReadOnlyDictionary<Vector2i, HashSet<Vector3i>> OversizedBlocksByChunkPosition;

		// Token: 0x02000A0F RID: 2575
		public struct SubsetAccessor : IEnumerator<KeyValuePair<Vector3i, MultiBlockManager.TrackedBlockData>>, IEnumerator, IDisposable
		{
			// Token: 0x06004EF6 RID: 20214 RVA: 0x001F5046 File Offset: 0x001F3246
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public SubsetAccessor(Dictionary<Vector3i, MultiBlockManager.TrackedBlockData> trackedData, HashSet<Vector3i> subset)
			{
				this._trackedData = trackedData;
				this._subset = subset;
				this._subsetEnumerator = subset.GetEnumerator();
			}

			// Token: 0x17000817 RID: 2071
			// (get) Token: 0x06004EF7 RID: 20215 RVA: 0x001F5064 File Offset: 0x001F3264
			public KeyValuePair<Vector3i, MultiBlockManager.TrackedBlockData> Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					Vector3i key = this._subsetEnumerator.Current;
					return new KeyValuePair<Vector3i, MultiBlockManager.TrackedBlockData>(key, this._trackedData[key]);
				}
			}

			// Token: 0x06004EF8 RID: 20216 RVA: 0x001F508F File Offset: 0x001F328F
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				return this._subsetEnumerator.MoveNext();
			}

			// Token: 0x06004EF9 RID: 20217 RVA: 0x0000E8AD File Offset: 0x0000CAAD
			public void Reset()
			{
				throw new NotSupportedException();
			}

			// Token: 0x06004EFA RID: 20218 RVA: 0x001F509C File Offset: 0x001F329C
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Dispose()
			{
				this._subsetEnumerator.Dispose();
			}

			// Token: 0x06004EFB RID: 20219 RVA: 0x001F50A9 File Offset: 0x001F32A9
			public bool ContainsKey(Vector3i key)
			{
				return this._subset.Contains(key);
			}

			// Token: 0x17000818 RID: 2072
			// (get) Token: 0x06004EFC RID: 20220 RVA: 0x001F50B7 File Offset: 0x001F32B7
			public int Count
			{
				get
				{
					return this._subset.Count;
				}
			}

			// Token: 0x06004EFD RID: 20221 RVA: 0x001F50C4 File Offset: 0x001F32C4
			public bool TryGetValue(Vector3i key, out MultiBlockManager.TrackedBlockData value)
			{
				if (this._subset.Contains(key) && this._trackedData.TryGetValue(key, out value))
				{
					return true;
				}
				value = default(MultiBlockManager.TrackedBlockData);
				return false;
			}

			// Token: 0x06004EFE RID: 20222 RVA: 0x001F50ED File Offset: 0x001F32ED
			public MultiBlockManager.TrackedDataMap.SubsetAccessor GetEnumerator()
			{
				return this;
			}

			// Token: 0x17000819 RID: 2073
			// (get) Token: 0x06004EFF RID: 20223 RVA: 0x001F50F5 File Offset: 0x001F32F5
			public object Current
			{
				[PublicizedFrom(EAccessModifier.Private)]
				get
				{
					return this.Current;
				}
			}

			// Token: 0x1700081A RID: 2074
			public MultiBlockManager.TrackedBlockData this[Vector3i key]
			{
				get
				{
					if (!this._subset.Contains(key))
					{
						throw new KeyNotFoundException(string.Format("The key \"{0}\" was not found in the subset.", key));
					}
					return this._trackedData[key];
				}
			}

			// Token: 0x04003C7F RID: 15487
			[PublicizedFrom(EAccessModifier.Private)]
			public readonly Dictionary<Vector3i, MultiBlockManager.TrackedBlockData> _trackedData;

			// Token: 0x04003C80 RID: 15488
			[PublicizedFrom(EAccessModifier.Private)]
			public readonly HashSet<Vector3i> _subset;

			// Token: 0x04003C81 RID: 15489
			[PublicizedFrom(EAccessModifier.Private)]
			public HashSet<Vector3i>.Enumerator _subsetEnumerator;
		}
	}

	// Token: 0x02000A10 RID: 2576
	public enum Mode
	{
		// Token: 0x04003C83 RID: 15491
		Disabled,
		// Token: 0x04003C84 RID: 15492
		Normal,
		// Token: 0x04003C85 RID: 15493
		WorldEditor,
		// Token: 0x04003C86 RID: 15494
		PrefabPlaytest,
		// Token: 0x04003C87 RID: 15495
		PrefabEditor,
		// Token: 0x04003C88 RID: 15496
		Client
	}

	// Token: 0x02000A11 RID: 2577
	[Flags]
	public enum FeatureFlags
	{
		// Token: 0x04003C8A RID: 15498
		None = 0,
		// Token: 0x04003C8B RID: 15499
		POIMBTracking = 1,
		// Token: 0x04003C8C RID: 15500
		Serialization = 2,
		// Token: 0x04003C8D RID: 15501
		CrossChunkMBTracking = 4,
		// Token: 0x04003C8E RID: 15502
		OversizedStability = 8,
		// Token: 0x04003C8F RID: 15503
		TerrainAlignment = 16,
		// Token: 0x04003C90 RID: 15504
		All = 31
	}

	// Token: 0x02000A12 RID: 2578
	[PublicizedFrom(EAccessModifier.Private)]
	public enum FeatureRequirement
	{
		// Token: 0x04003C92 RID: 15506
		AllEnabled,
		// Token: 0x04003C93 RID: 15507
		OneOrMoreEnabled,
		// Token: 0x04003C94 RID: 15508
		AllDisabled
	}
}
