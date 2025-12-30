using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
using Epic.OnlineServices.AntiCheatCommon;
using GamePath;
using Platform;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000FA6 RID: 4006
public class GameUtils
{
	// Token: 0x06007F87 RID: 32647 RVA: 0x0033C9C0 File Offset: 0x0033ABC0
	public static bool FindMasterBlockForEntityModelBlock(World _world, Vector3 _dirNormalized, string _phsxTag, Vector3 _hitPointPos, Transform _hitTransform, WorldRayHitInfo _hitInfo)
	{
		int num = 0;
		if (_phsxTag.Length > 2)
		{
			char c = _phsxTag[_phsxTag.Length - 2];
			char c2 = _phsxTag[_phsxTag.Length - 1];
			if (c >= '0' && c <= '9' && c2 >= '0' && c2 <= '9')
			{
				num += (int)((c - '0') * '\n');
				num += (int)(c2 - '0');
			}
		}
		ChunkCluster chunkCluster = _world.ChunkClusters[num];
		if (chunkCluster == null)
		{
			return false;
		}
		Vector3 vector = chunkCluster.ToLocalPosition(_hitPointPos);
		Vector3 vector2 = chunkCluster.ToLocalVector(_dirNormalized);
		Vector3i vector3i = World.worldToBlockPos(vector);
		Transform parentTransform = RootTransformRefParent.FindRoot(_hitTransform);
		int num2 = World.toBlockXZ(vector3i.x);
		int num3 = World.toBlockXZ(vector3i.z);
		int num4 = World.toChunkXZ(vector3i.x);
		int num5 = World.toChunkXZ(vector3i.z);
		BlockEntityData blockEntityData;
		if (GameUtils.checkChunk(chunkCluster, num4, num5, parentTransform, vector, vector2, _hitInfo, out blockEntityData))
		{
			_hitInfo.hit.pos = _hitPointPos;
			_hitInfo.lastBlockPos = World.worldToBlockPos(_hitInfo.hit.pos);
			if (!blockEntityData.blockValue.Block.isMultiBlock || blockEntityData.blockValue.Block.multiBlockPos.ContainsPos(_world, blockEntityData.pos, blockEntityData.blockValue, _hitInfo.lastBlockPos))
			{
				BlockValue blockValue;
				_hitInfo.lastBlockPos = Voxel.GoBackOnVoxels(chunkCluster, new Ray(chunkCluster.ToLocalPosition(_hitPointPos + vector2 * 0.01f), -vector2), out blockValue);
			}
			return true;
		}
		if (GameUtils.checkChunk(chunkCluster, (num2 < 8) ? (num4 - 1) : (num4 + 1), num5, parentTransform, vector, vector2, _hitInfo, out blockEntityData))
		{
			_hitInfo.hit.pos = _hitPointPos;
			_hitInfo.lastBlockPos = World.worldToBlockPos(_hitInfo.hit.pos);
			if (!blockEntityData.blockValue.Block.isMultiBlock || blockEntityData.blockValue.Block.multiBlockPos.ContainsPos(_world, blockEntityData.pos, blockEntityData.blockValue, _hitInfo.lastBlockPos))
			{
				BlockValue blockValue;
				_hitInfo.lastBlockPos = Voxel.GoBackOnVoxels(chunkCluster, new Ray(chunkCluster.ToLocalPosition(_hitPointPos + vector2 * 0.01f), -vector2), out blockValue);
			}
			return true;
		}
		if (GameUtils.checkChunk(chunkCluster, num4, (num3 < 8) ? (num5 - 1) : (num5 + 1), parentTransform, vector, vector2, _hitInfo, out blockEntityData))
		{
			_hitInfo.hit.pos = _hitPointPos;
			_hitInfo.lastBlockPos = World.worldToBlockPos(_hitInfo.hit.pos);
			if (!blockEntityData.blockValue.Block.isMultiBlock || blockEntityData.blockValue.Block.multiBlockPos.ContainsPos(_world, blockEntityData.pos, blockEntityData.blockValue, _hitInfo.lastBlockPos))
			{
				BlockValue blockValue;
				_hitInfo.lastBlockPos = Voxel.GoBackOnVoxels(chunkCluster, new Ray(chunkCluster.ToLocalPosition(_hitPointPos + vector2 * 0.01f), -vector2), out blockValue);
			}
			return true;
		}
		if (GameUtils.checkChunk(chunkCluster, (num2 < 8) ? (num4 - 1) : (num4 + 1), (num3 < 8) ? (num5 - 1) : (num5 + 1), parentTransform, vector, vector2, _hitInfo, out blockEntityData))
		{
			_hitInfo.hit.pos = _hitPointPos;
			_hitInfo.lastBlockPos = World.worldToBlockPos(_hitInfo.hit.pos);
			if (!blockEntityData.blockValue.Block.isMultiBlock || blockEntityData.blockValue.Block.multiBlockPos.ContainsPos(_world, blockEntityData.pos, blockEntityData.blockValue, _hitInfo.lastBlockPos))
			{
				BlockValue blockValue;
				_hitInfo.lastBlockPos = Voxel.GoBackOnVoxels(chunkCluster, new Ray(chunkCluster.ToLocalPosition(_hitPointPos + vector2 * 0.01f), -vector2), out blockValue);
			}
			return true;
		}
		return false;
	}

	// Token: 0x06007F88 RID: 32648 RVA: 0x0033CD7C File Offset: 0x0033AF7C
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool checkChunk(ChunkCluster cc, int cX, int cZ, Transform parentTransform, Vector3 localHitPos, Vector3 localDirNormalized, WorldRayHitInfo _hitInfo, out BlockEntityData _ebcd)
	{
		_ebcd = null;
		IChunk chunkSync = cc.GetChunkSync(cX, cZ);
		if (chunkSync == null)
		{
			return false;
		}
		BlockEntityData blockEntity;
		_ebcd = (blockEntity = chunkSync.GetBlockEntity(parentTransform));
		if (blockEntity != null)
		{
			_hitInfo.hit.clrIdx = 0;
			_hitInfo.hit.blockPos = _ebcd.pos;
			_hitInfo.hit.voxelData = HitInfoDetails.VoxelData.GetFrom(chunkSync, World.toBlockXZ(_ebcd.pos.x), World.toBlockY(_ebcd.pos.y), World.toBlockXZ(_ebcd.pos.z));
			Ray ray = new Ray(localHitPos, -1f * localDirNormalized);
			int num = 0;
			do
			{
				Vector3 a;
				BlockFace blockFace;
				_hitInfo.lastBlockPos = Voxel.OneVoxelStep(Vector3i.FromVector3Rounded(ray.origin), ray.origin, ray.direction, out a, out blockFace);
				ray.origin = a + localDirNormalized * 0.001f;
			}
			while (!cc.GetBlock(_hitInfo.lastBlockPos).isair && num++ < 3);
			return true;
		}
		return false;
	}

	// Token: 0x06007F89 RID: 32649 RVA: 0x0033CE94 File Offset: 0x0033B094
	public static void EnableRagdoll(GameObject _model, bool _bEnable, bool _bUseGravity)
	{
		foreach (Rigidbody rigidbody in _model.GetComponentsInChildren<Rigidbody>())
		{
			rigidbody.isKinematic = !_bEnable;
			rigidbody.useGravity = _bUseGravity;
		}
		Collider[] componentsInChildren2 = _model.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren2.Length; i++)
		{
			componentsInChildren2[i].enabled = _bEnable;
		}
	}

	// Token: 0x06007F8A RID: 32650 RVA: 0x0033CEE8 File Offset: 0x0033B0E8
	public static Entity GetHitRootEntity(string _tag, Transform _hitTransform)
	{
		if (_tag.StartsWith("E_BP_"))
		{
			RootTransformRefEntity rootTransformRefEntity;
			if (_hitTransform.TryGetComponent<RootTransformRefEntity>(out rootTransformRefEntity))
			{
				if (rootTransformRefEntity.RootTransform)
				{
					return rootTransformRefEntity.RootTransform.GetComponent<Entity>();
				}
			}
			else
			{
				Transform transform = RootTransformRefEntity.FindEntityUpwards(_hitTransform);
				if (transform)
				{
					return transform.GetComponent<Entity>();
				}
			}
		}
		else if (_tag.StartsWith("E_Vehicle"))
		{
			return CollisionCallForward.FindEntity(_hitTransform);
		}
		return null;
	}

	// Token: 0x06007F8B RID: 32651 RVA: 0x0033CF50 File Offset: 0x0033B150
	public static Transform GetHitRootTransform(string _tag, Transform _hitTransform)
	{
		if (!_tag.StartsWith("E_BP_"))
		{
			if (_tag.Equals("E_Vehicle"))
			{
				Entity entity = CollisionCallForward.FindEntity(_hitTransform);
				if (entity)
				{
					return entity.transform;
				}
			}
			return _hitTransform;
		}
		RootTransformRefEntity rootTransformRefEntity;
		if (_hitTransform.TryGetComponent<RootTransformRefEntity>(out rootTransformRefEntity))
		{
			return rootTransformRefEntity.RootTransform;
		}
		return RootTransformRefEntity.FindEntityUpwards(_hitTransform);
	}

	// Token: 0x06007F8C RID: 32652 RVA: 0x0033CFA6 File Offset: 0x0033B1A6
	public static string GetTransformPath(Transform _t)
	{
		if (!_t)
		{
			return "null";
		}
		if (!_t.parent)
		{
			return _t.name;
		}
		return GameUtils.GetTransformPath(_t.parent) + "/" + _t.name;
	}

	// Token: 0x06007F8D RID: 32653 RVA: 0x0033CFE8 File Offset: 0x0033B1E8
	public static string GetChildTransformPath(Transform _parent, Transform _child)
	{
		if (_child.parent == null)
		{
			throw new Exception(string.Concat(new string[]
			{
				"GetChildTransformPath: '",
				_child.name,
				"' is a root object and not in the path underneath '",
				_parent.name,
				"'"
			}));
		}
		if (_child.parent == _parent)
		{
			return _child.name;
		}
		return GameUtils.GetChildTransformPath(_parent, _child.parent) + "/" + _child.name;
	}

	// Token: 0x06007F8E RID: 32654 RVA: 0x0033D070 File Offset: 0x0033B270
	public static void FindTagInChilds(Transform _parent, string _tag, List<Transform> _list)
	{
		int childCount = _parent.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = _parent.GetChild(i);
			if (child.CompareTag(_tag))
			{
				_list.Add(child);
			}
			GameUtils.FindTagInChilds(child, _tag, _list);
		}
	}

	// Token: 0x06007F8F RID: 32655 RVA: 0x0033D0B0 File Offset: 0x0033B2B0
	public static Transform FindTagInChilds(Transform _parent, string _tag)
	{
		int childCount = _parent.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = _parent.GetChild(i);
			if (child.CompareTag(_tag))
			{
				return child;
			}
		}
		for (int j = 0; j < childCount; j++)
		{
			Transform transform = GameUtils.FindTagInChilds(_parent.GetChild(j), _tag);
			if (transform != null)
			{
				return transform;
			}
		}
		return null;
	}

	// Token: 0x06007F90 RID: 32656 RVA: 0x0033D10C File Offset: 0x0033B30C
	public static Transform FindTagInDirectChilds(Transform _parent, string _tag)
	{
		int childCount = _parent.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = _parent.GetChild(i);
			if (child.CompareTag(_tag))
			{
				return child;
			}
		}
		return null;
	}

	// Token: 0x06007F91 RID: 32657 RVA: 0x0033D140 File Offset: 0x0033B340
	public static Transform FindChildWithPartialName(Transform root, params string[] names)
	{
		foreach (string b in names)
		{
			if (root.name.ContainsCaseInsensitive(b))
			{
				return root;
			}
			for (int j = 0; j < root.childCount; j++)
			{
				Transform child = root.GetChild(j);
				if (child.name.ContainsCaseInsensitive(b))
				{
					return child;
				}
			}
		}
		return null;
	}

	// Token: 0x06007F92 RID: 32658 RVA: 0x0033D1A0 File Offset: 0x0033B3A0
	public static void FindDeepChildWithPartialName(Transform root, string name, ref List<Transform> found)
	{
		if (root.name.ContainsCaseInsensitive(name))
		{
			found.Add(root);
		}
		for (int i = 0; i < root.childCount; i++)
		{
			GameUtils.FindDeepChildWithPartialName(root.GetChild(i), name, ref found);
		}
	}

	// Token: 0x06007F93 RID: 32659 RVA: 0x0033D1E2 File Offset: 0x0033B3E2
	[Conditional("UNITY_EDITOR")]
	public static void HideObjectInEditor(GameObject _obj)
	{
		UnityEngine.Object.DontDestroyOnLoad(_obj);
	}

	// Token: 0x06007F94 RID: 32660 RVA: 0x0033D1EC File Offset: 0x0033B3EC
	public static bool IsColliderWithinBlock(Vector3i blockPosition, BlockValue blockValue)
	{
		int num = 3899392;
		Quaternion rotation = blockValue.Block.shape.GetRotation(blockValue);
		Bounds blockPlacementBounds = GameUtils.GetBlockPlacementBounds(blockValue.Block);
		Vector3 a = blockPlacementBounds.size;
		Vector3 vector = World.blockToTransformPos(blockPosition) - Origin.position + new Vector3(0f, 0.5f, 0f);
		if (blockPlacementBounds.center != Vector3.zero)
		{
			vector += rotation * blockPlacementBounds.center;
		}
		if (blockValue.Block.isOversized)
		{
			num |= 1082130432;
			a -= new Vector3(0.1f, 0.1f, 0.1f);
		}
		else if (blockValue.Block.shape.IsTerrain())
		{
			vector -= new Vector3(0f, 0.25f, 0f);
		}
		Vector3 halfExtents = a * 0.5f;
		int num2 = Physics.OverlapBoxNonAlloc(vector, halfExtents, GameUtils.overlapBoxHits, rotation, num);
		if (num2 == GameUtils.overlapBoxHits.Length)
		{
			UnityEngine.Debug.LogError(string.Format("OverlapBox reached maximum hit count ({0}); overlapBoxHits array size may be insufficient.", num2));
		}
		for (int i = 0; i < num2; i++)
		{
			if (blockValue.Block.isOversized)
			{
				if (!GameUtils.overlapBoxHits[i].CompareTag("T_Mesh"))
				{
					return true;
				}
			}
			else
			{
				if (!GameUtils.IsBlockOrTerrain(GameUtils.overlapBoxHits[i]) && !GameUtils.overlapBoxHits[i].CompareTag("Item"))
				{
					return true;
				}
				if (GameUtils.overlapBoxHits[i].CompareTag("T_Block"))
				{
					Transform entityParentTransform = RootTransformRefParent.FindRoot(GameUtils.overlapBoxHits[i].transform);
					BlockEntityData blockEntityData;
					if (GameUtils.TryFindEntityData(blockPosition, entityParentTransform, out blockEntityData) && blockEntityData.blockValue.Block.isOversized)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06007F95 RID: 32661 RVA: 0x0033D3C8 File Offset: 0x0033B5C8
	public static Vector3 GetMultiBlockBoundsOffset(Vector3 multiBlockDim)
	{
		return new Vector3((multiBlockDim.x % 2f == 0f) ? -0.5f : 0f, multiBlockDim.y / 2f - 0.5f, (multiBlockDim.z % 2f == 0f) ? -0.5f : 0f);
	}

	// Token: 0x06007F96 RID: 32662 RVA: 0x0033D42C File Offset: 0x0033B62C
	public static Bounds GetBlockPlacementBounds(Block block)
	{
		if (block.isOversized)
		{
			return block.oversizedBounds;
		}
		if (block.isMultiBlock)
		{
			Vector3 vector = block.multiBlockPos.dim;
			return new Bounds(GameUtils.GetMultiBlockBoundsOffset(vector), vector);
		}
		return new Bounds(Vector3.zero, Vector3.one);
	}

	// Token: 0x06007F97 RID: 32663 RVA: 0x0033D480 File Offset: 0x0033B680
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool TryFindEntityData(Vector3i entityWorldHitPosition, Transform entityParentTransform, out BlockEntityData ebcd)
	{
		int num = World.toBlockXZ(entityWorldHitPosition.x);
		int num2 = World.toBlockXZ(entityWorldHitPosition.z);
		int num3 = World.toChunkXZ(entityWorldHitPosition.x);
		int num4 = World.toChunkXZ(entityWorldHitPosition.z);
		int num5 = (num < 8) ? -1 : 1;
		int num6 = (num2 < 8) ? -1 : 1;
		ChunkCluster chunkCache = GameManager.Instance.World.ChunkCache;
		if (chunkCache != null)
		{
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					Chunk chunkSync = chunkCache.GetChunkSync(num3 + j * num5, num4 + i * num6);
					ebcd = ((chunkSync != null) ? chunkSync.GetBlockEntity(entityParentTransform) : null);
					if (ebcd != null)
					{
						return true;
					}
				}
			}
		}
		UnityEngine.Debug.LogWarning(string.Format("Failed to find entity data for Transform \"{0}\" with hit position \"{1}\"", entityParentTransform.name, entityWorldHitPosition), entityParentTransform);
		ebcd = null;
		return false;
	}

	// Token: 0x06007F98 RID: 32664 RVA: 0x00002914 File Offset: 0x00000B14
	public static void DebugDrawPathFromEntity(Entity _e, PathEntity _path, Color _color)
	{
	}

	// Token: 0x06007F99 RID: 32665 RVA: 0x0033D550 File Offset: 0x0033B750
	public static void CreateEmptyFlatLevel(string _worldName, int _worldSize, int _terrainHeight = 60)
	{
		PathAbstractions.AbstractedLocation worldLocation = new PathAbstractions.AbstractedLocation(PathAbstractions.EAbstractedLocationType.UserDataPath, _worldName, GameIO.GetUserGameDataDir() + "/GeneratedWorlds/" + _worldName, null, true, null);
		SdDirectory.CreateDirectory(worldLocation.FullPath);
		World world = new World();
		WorldState worldState = new WorldState();
		worldState.SetFrom(world, EnumChunkProviderId.ChunkDataDriven);
		worldState.ResetDynamicData();
		worldState.Save(worldLocation.FullPath + "/main.ttw");
		GameUtils.CreateWorldFilesForFlatLevel(_worldName, _worldSize, worldLocation, _terrainHeight);
	}

	// Token: 0x06007F9A RID: 32666 RVA: 0x0033D5C0 File Offset: 0x0033B7C0
	[PublicizedFrom(EAccessModifier.Private)]
	public static void CreateWorldFilesForFlatLevel(string _worldName, int _worldSize, PathAbstractions.AbstractedLocation _worldLocation, int _terrainHeight)
	{
		int num = _worldSize * 2;
		byte[] array = new byte[num];
		for (int i = 0; i < _worldSize; i++)
		{
			array[2 * i] = 0;
			array[2 * i + 1] = 60;
		}
		using (BufferedStream bufferedStream = new BufferedStream(SdFile.OpenWrite(_worldLocation.FullPath + "/dtm.raw")))
		{
			for (int j = 0; j < _worldSize; j++)
			{
				bufferedStream.Write(array, 0, num);
			}
		}
		new GameUtils.WorldInfo(_worldName, "Empty World", new string[]
		{
			"Survival",
			"Creative"
		}, new Vector2i(_worldSize, _worldSize), 1, false, false, Constants.cVersionInformation, null).Save(_worldLocation);
		new SpawnPointManager(true)
		{
			spawnPointList = 
			{
				new SpawnPoint(new Vector3i(0, _terrainHeight + 1, 0))
			}
		}.Save(_worldLocation.FullPath);
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.CreateXmlDeclaration();
		xmlDocument.AddXmlElement("WaterSources");
		xmlDocument.SdSave(_worldLocation.FullPath + "/water_info.xml");
		GameUtils.CreateForestBiomeMap(_worldSize, _worldLocation, BiomeDefinition.BiomeType.PineForest);
		XmlDocument xmlDocument2 = new XmlDocument();
		xmlDocument2.CreateXmlDeclaration();
		xmlDocument2.AddXmlElement("prefabs");
		xmlDocument2.SdSave(_worldLocation.FullPath + "/prefabs.xml");
		GameUtils.CreateSimpleRadiationMap(_worldSize, _worldLocation, (_worldSize <= 1024) ? 128 : ((_worldSize <= 2048) ? 256 : 512), 16);
		GameUtils.CreateEmptySplatMap(_worldSize, _worldLocation);
	}

	// Token: 0x06007F9B RID: 32667 RVA: 0x0033D740 File Offset: 0x0033B940
	[PublicizedFrom(EAccessModifier.Private)]
	public static void CreateSimpleRadiationMap(int _worldSize, PathAbstractions.AbstractedLocation _worldLocation, int _radiationBorderSize = 128, int _downScale = 16)
	{
		int num = _worldSize / _downScale;
		Color red = Color.red;
		Color32 color = new Color32(0, 0, 0, byte.MaxValue);
		Texture2D texture2D = new Texture2D(num, num, TextureFormat.RGBA32, false);
		texture2D.FillTexture(red, false, false);
		int num2 = _radiationBorderSize / _downScale;
		int num3 = num - 2 * num2;
		Color32[] array = new Color32[num3];
		for (int i = 0; i < num3; i++)
		{
			array[i] = color;
		}
		for (int j = num2; j < num - num2; j++)
		{
			texture2D.SetPixels32(num2, j, num3, 1, array);
		}
		texture2D.Apply();
		SdFile.WriteAllBytes(_worldLocation.FullPath + "/radiation.png", texture2D.EncodeToPNG());
	}

	// Token: 0x06007F9C RID: 32668 RVA: 0x0033D7F0 File Offset: 0x0033B9F0
	[PublicizedFrom(EAccessModifier.Private)]
	public static void CreateEmptySplatMap(int _worldSize, PathAbstractions.AbstractedLocation _worldLocation)
	{
		Color32 c = new Color32(0, 0, 0, 0);
		Texture2D texture2D = new Texture2D(_worldSize, _worldSize, TextureFormat.RGBA32, false);
		texture2D.FillTexture(c, false, false);
		texture2D.Apply();
		byte[] bytes = texture2D.EncodeToPNG();
		MicroStopwatch microStopwatch = new MicroStopwatch(true);
		SdFile.WriteAllBytes(_worldLocation.FullPath + "/splat1.png", bytes);
		SdFile.WriteAllBytes(_worldLocation.FullPath + "/splat2.png", bytes);
		SdFile.WriteAllBytes(_worldLocation.FullPath + "/splat3.png", bytes);
		Log.Out(string.Format("Write tex took {0} ms", microStopwatch.ElapsedMilliseconds));
	}

	// Token: 0x06007F9D RID: 32669 RVA: 0x0033D894 File Offset: 0x0033BA94
	[PublicizedFrom(EAccessModifier.Private)]
	public static void CreateForestBiomeMap(int _worldSize, PathAbstractions.AbstractedLocation _worldLocation, BiomeDefinition.BiomeType _biome = BiomeDefinition.BiomeType.PineForest)
	{
		Color32 c = GameUtils.UIntToColor(BiomeDefinition.GetBiomeColor(_biome), false);
		Texture2D texture2D = new Texture2D(_worldSize, _worldSize, TextureFormat.RGBA32, false);
		texture2D.FillTexture(c, false, false);
		texture2D.Apply();
		MicroStopwatch microStopwatch = new MicroStopwatch(true);
		SdFile.WriteAllBytes(_worldLocation.FullPath + "/biomes.png", texture2D.EncodeToPNG());
		Log.Out(string.Format("Write tex took {0} ms", microStopwatch.ElapsedMilliseconds));
	}

	// Token: 0x06007F9E RID: 32670 RVA: 0x0033D90C File Offset: 0x0033BB0C
	public static void DeleteWorld(PathAbstractions.AbstractedLocation _worldLocation)
	{
		if (string.IsNullOrEmpty(_worldLocation.Name))
		{
			return;
		}
		SdDirectory.Delete(_worldLocation.FullPath, true);
		string saveGameDir = GameIO.GetSaveGameDir(_worldLocation.Name);
		if (SdDirectory.Exists(saveGameDir))
		{
			SdDirectory.Delete(saveGameDir, true);
		}
	}

	// Token: 0x06007F9F RID: 32671 RVA: 0x0033D94F File Offset: 0x0033BB4F
	public static int WorldTimeToDays(ulong _worldTime)
	{
		return (int)(_worldTime / 24000UL + 1UL);
	}

	// Token: 0x06007FA0 RID: 32672 RVA: 0x0033D95D File Offset: 0x0033BB5D
	public static int WorldTimeToHours(ulong _worldTime)
	{
		return (int)(_worldTime / 1000UL) % 24;
	}

	// Token: 0x06007FA1 RID: 32673 RVA: 0x0033D96B File Offset: 0x0033BB6B
	public static int WorldTimeToMinutes(ulong _worldTime)
	{
		return (int)(_worldTime / 1000.0 * 60.0) % 60;
	}

	// Token: 0x06007FA2 RID: 32674 RVA: 0x0033D988 File Offset: 0x0033BB88
	public static float WorldTimeToTotalSeconds(float _worldTime)
	{
		return _worldTime * 3.6f;
	}

	// Token: 0x06007FA3 RID: 32675 RVA: 0x0033D991 File Offset: 0x0033BB91
	public static uint WorldTimeToTotalMinutes(ulong _worldTime)
	{
		return (uint)(_worldTime * 0.06);
	}

	// Token: 0x06007FA4 RID: 32676 RVA: 0x0033D9A1 File Offset: 0x0033BBA1
	public static int WorldTimeToTotalHours(ulong _worldTime)
	{
		return (int)(_worldTime / 1000UL);
	}

	// Token: 0x06007FA5 RID: 32677 RVA: 0x0033D9AC File Offset: 0x0033BBAC
	public static ulong TotalMinutesToWorldTime(uint _totalMinutes)
	{
		return (ulong)(_totalMinutes / 0.06);
	}

	// Token: 0x06007FA6 RID: 32678 RVA: 0x0033D9BC File Offset: 0x0033BBBC
	[return: TupleElementNames(new string[]
	{
		"Days",
		"Hours",
		"Minutes"
	})]
	public static ValueTuple<int, int, int> WorldTimeToElements(ulong _worldTime)
	{
		int item = (int)(_worldTime / 24000UL + 1UL);
		int item2 = (int)(_worldTime / 1000UL) % 24;
		int item3 = (int)(_worldTime * 0.06) % 60;
		return new ValueTuple<int, int, int>(item, item2, item3);
	}

	// Token: 0x06007FA7 RID: 32679 RVA: 0x0033D9FC File Offset: 0x0033BBFC
	public static string WorldTimeToString(ulong _worldTime)
	{
		ValueTuple<int, int, int> valueTuple = GameUtils.WorldTimeToElements(_worldTime);
		int item = valueTuple.Item1;
		int item2 = valueTuple.Item2;
		int item3 = valueTuple.Item3;
		return string.Format("{0} {1:D2}:{2:D2}", item, item2, item3);
	}

	// Token: 0x06007FA8 RID: 32680 RVA: 0x0033DA40 File Offset: 0x0033BC40
	public static string WorldTimeDeltaToString(ulong _worldTime)
	{
		ValueTuple<int, int, int> valueTuple = GameUtils.WorldTimeToElements(_worldTime);
		int item = valueTuple.Item1;
		int item2 = valueTuple.Item2;
		int item3 = valueTuple.Item3;
		return string.Format("{0} {1:D2}:{2:D2}", item - 1, item2, item3);
	}

	// Token: 0x06007FA9 RID: 32681 RVA: 0x0033DA85 File Offset: 0x0033BC85
	public static ulong DayTimeToWorldTime(int _day, int _hours, int _minutes)
	{
		if (_day < 1)
		{
			return 0UL;
		}
		return (ulong)((long)(_day - 1) * 24000L + (long)(_hours * 1000) + (long)(_minutes * 1000 / 60));
	}

	// Token: 0x06007FAA RID: 32682 RVA: 0x0033DAAE File Offset: 0x0033BCAE
	public static ulong DaysToWorldTime(int _day)
	{
		if (_day < 1)
		{
			return 0UL;
		}
		return (ulong)(((long)_day - 1L) * 24000L);
	}

	// Token: 0x06007FAB RID: 32683 RVA: 0x0033DAC3 File Offset: 0x0033BCC3
	public static ulong DaysToWorldTimeMidnight(int _day)
	{
		return GameUtils.DaysToWorldTime(_day) + 16000UL;
	}

	// Token: 0x06007FAC RID: 32684 RVA: 0x0033DAD4 File Offset: 0x0033BCD4
	[return: TupleElementNames(new string[]
	{
		"duskHour",
		"dawnHour"
	})]
	public static ValueTuple<int, int> CalcDuskDawnHours(int _dayLightLength)
	{
		ValueTuple<int, int> valueTuple;
		valueTuple.Item1 = 22;
		if (_dayLightLength > 22)
		{
			valueTuple.Item1 = Mathf.Clamp(_dayLightLength, 0, 23);
		}
		valueTuple.Item2 = Mathf.Clamp(valueTuple.Item1 - _dayLightLength, 0, 23);
		return valueTuple;
	}

	// Token: 0x06007FAD RID: 32685 RVA: 0x0033DB18 File Offset: 0x0033BD18
	public static bool IsBloodMoonTime(ulong _worldTime, [TupleElementNames(new string[]
	{
		"duskHour",
		"dawnHour"
	})] ValueTuple<int, int> _duskDawnTimes, int _bmDay)
	{
		ValueTuple<int, int, int> valueTuple = GameUtils.WorldTimeToElements(_worldTime);
		int item = valueTuple.Item1;
		int item2 = valueTuple.Item2;
		return GameUtils.IsBloodMoonTime(_duskDawnTimes, item2, _bmDay, item);
	}

	// Token: 0x06007FAE RID: 32686 RVA: 0x0033DB41 File Offset: 0x0033BD41
	public static bool IsBloodMoonTime([TupleElementNames(new string[]
	{
		"duskHour",
		"dawnHour"
	})] ValueTuple<int, int> _duskDawnTimes, int _hour, int _bmDay, int _day)
	{
		if (_day == _bmDay)
		{
			if (_hour >= _duskDawnTimes.Item1)
			{
				return true;
			}
		}
		else if (_day > 1 && _day == _bmDay + 1 && _hour < _duskDawnTimes.Item2)
		{
			return true;
		}
		return false;
	}

	// Token: 0x06007FAF RID: 32687 RVA: 0x0033DB68 File Offset: 0x0033BD68
	public static List<string> GetWorldFilesToTransmitToClient(string _worldFolder)
	{
		string[] files = SdDirectory.GetFiles(_worldFolder);
		for (int i = 0; i < files.Length; i++)
		{
			files[i] = GameIO.GetFilenameFromPath(files[i]);
		}
		return GameUtils.GetWorldFilesToTransmitToClient(files);
	}

	// Token: 0x06007FB0 RID: 32688 RVA: 0x0033DB9C File Offset: 0x0033BD9C
	public static List<string> GetWorldFilesToTransmitToClient(ICollection<string> _files)
	{
		HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (string filepath in _files)
		{
			hashSet.Add(GameIO.GetFilenameFromPathWithoutExtension(filepath));
		}
		List<string> list = new List<string>();
		foreach (string text in _files)
		{
			string filenameFromPathWithoutExtension = GameIO.GetFilenameFromPathWithoutExtension(text);
			if (true & !hashSet.Contains(filenameFromPathWithoutExtension + "_processed") & !text.ContainsCaseInsensitive("GenerationInfo") & !text.EndsWith(".bak", StringComparison.OrdinalIgnoreCase) & !text.ContainsCaseInsensitive("Version.txt") & !text.ContainsCaseInsensitive("checksums.txt"))
			{
				list.Add(text);
			}
		}
		return list;
	}

	// Token: 0x06007FB1 RID: 32689 RVA: 0x0033DC98 File Offset: 0x0033BE98
	public static void DebugOutputGamePrefs(GameUtils.OutputDelegate _output)
	{
		SortedList<string, string> sortedList = new SortedList<string, string>();
		for (int num = 0; num != 276; num++)
		{
			string text = ((EnumGamePrefs)num).ToStringCached<EnumGamePrefs>();
			if (!text.Contains("Password") && text != "ServerHistoryCache")
			{
				SortedList<string, string> sortedList2 = sortedList;
				string key = text;
				string str = text;
				string str2 = " = ";
				object @object = GamePrefs.GetObject((EnumGamePrefs)num);
				sortedList2.Add(key, str + str2 + ((@object != null) ? @object.ToString() : null));
			}
		}
		foreach (string key2 in sortedList.Keys)
		{
			_output(sortedList[key2]);
		}
	}

	// Token: 0x06007FB2 RID: 32690 RVA: 0x0033DD48 File Offset: 0x0033BF48
	public static void DebugOutputGameStats(GameUtils.OutputDelegate _output)
	{
		SortedList<string, string> sortedList = new SortedList<string, string>();
		for (int num = 0; num != 68; num++)
		{
			string text = ((EnumGameStats)num).ToStringCached<EnumGameStats>();
			SortedList<string, string> sortedList2 = sortedList;
			string key = text;
			string str = text;
			string str2 = " = ";
			object @object = GameStats.GetObject((EnumGameStats)num);
			sortedList2.Add(key, str + str2 + ((@object != null) ? @object.ToString() : null));
		}
		foreach (string key2 in sortedList.Keys)
		{
			_output(sortedList[key2]);
		}
	}

	// Token: 0x06007FB3 RID: 32691 RVA: 0x0033DDDC File Offset: 0x0033BFDC
	public static void KickPlayerForClientInfo(ClientInfo _cInfo, GameUtils.KickPlayerData _kickData)
	{
		_cInfo.SendPackage(NetPackageManager.GetPackage<NetPackagePlayerDenied>().Setup(_kickData));
		string str = _cInfo.ToString();
		string str2 = "Kicking player (";
		GameUtils.KickPlayerData kickPlayerData = _kickData;
		Log.Out(str2 + kickPlayerData.ToString() + "): " + str);
		ThreadManager.StartCoroutine(GameUtils.disconnectLater(0.5f, _cInfo));
	}

	// Token: 0x06007FB4 RID: 32692 RVA: 0x0033DE36 File Offset: 0x0033C036
	[PublicizedFrom(EAccessModifier.Protected)]
	public static IEnumerator disconnectLater(float _delayInSec, ClientInfo _clientInfo)
	{
		_clientInfo.disconnecting = true;
		yield return new WaitForSecondsRealtime(_delayInSec);
		SingletonMonoBehaviour<ConnectionManager>.Instance.DisconnectClient(_clientInfo, false, false);
		yield break;
	}

	// Token: 0x06007FB5 RID: 32693 RVA: 0x0033DE4C File Offset: 0x0033C04C
	public static void ForceDisconnect()
	{
		GameUtils.ForceDisconnect(new GameUtils.KickPlayerData(GameUtils.EKickReason.InternalNetConnectionError, 0, default(DateTime), ""));
	}

	// Token: 0x06007FB6 RID: 32694 RVA: 0x0033DE74 File Offset: 0x0033C074
	public static void ForceDisconnect(GameUtils.KickPlayerData _kickData)
	{
		ThreadManager.StartCoroutine(GameUtils.ForceDisconnectRoutine(0.5f, _kickData));
	}

	// Token: 0x06007FB7 RID: 32695 RVA: 0x0033DE87 File Offset: 0x0033C087
	[PublicizedFrom(EAccessModifier.Protected)]
	public static IEnumerator ForceDisconnectRoutine(float delay, GameUtils.KickPlayerData kickData)
	{
		yield return new WaitForSeconds(delay);
		GameManager.Instance.Disconnect();
		if (!GameManager.IsDedicatedServer)
		{
			yield return new WaitForSeconds(0.5f);
			GameManager.Instance.ShowMessagePlayerDenied(kickData);
		}
		yield break;
	}

	// Token: 0x06007FB8 RID: 32696 RVA: 0x0033DEA0 File Offset: 0x0033C0A0
	public static void WriteItemStack(BinaryWriter _bw, IList<ItemStack> _itemStack)
	{
		_bw.Write((ushort)_itemStack.Count);
		for (int i = 0; i < _itemStack.Count; i++)
		{
			_itemStack[i].Write(_bw);
		}
	}

	// Token: 0x06007FB9 RID: 32697 RVA: 0x0033DED8 File Offset: 0x0033C0D8
	public static ItemStack[] ReadItemStackOld(BinaryReader _br)
	{
		int num = (int)_br.ReadUInt16();
		ItemStack[] array = ItemStack.CreateArray(num);
		for (int i = 0; i < num; i++)
		{
			array[i].ReadOld(_br);
			if (ItemClass.GetForId(array[i].itemValue.type) == null)
			{
				array[i] = ItemStack.Empty.Clone();
			}
		}
		return array;
	}

	// Token: 0x06007FBA RID: 32698 RVA: 0x0033DF2C File Offset: 0x0033C12C
	public static ItemStack[] ReadItemStack(BinaryReader _br)
	{
		int num = (int)_br.ReadUInt16();
		ItemStack[] array = ItemStack.CreateArray(num);
		for (int i = 0; i < num; i++)
		{
			array[i].Read(_br);
			if (ItemClass.GetForId(array[i].itemValue.type) == null)
			{
				array[i] = ItemStack.Empty.Clone();
			}
		}
		return array;
	}

	// Token: 0x06007FBB RID: 32699 RVA: 0x0033DF80 File Offset: 0x0033C180
	public static void WriteItemValueArray(BinaryWriter _bw, ItemValue[] _items)
	{
		_bw.Write((ushort)_items.Length);
		foreach (ItemValue itemValue in _items)
		{
			bool flag = itemValue != null;
			_bw.Write(flag);
			if (flag)
			{
				itemValue.Write(_bw);
			}
		}
	}

	// Token: 0x06007FBC RID: 32700 RVA: 0x0033DFC0 File Offset: 0x0033C1C0
	public static ItemValue[] ReadItemValueArray(BinaryReader _br)
	{
		ItemValue[] array = new ItemValue[(int)_br.ReadUInt16()];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new ItemValue();
			if (_br.ReadBoolean())
			{
				array[i].Read(_br);
			}
		}
		return array;
	}

	// Token: 0x06007FBD RID: 32701 RVA: 0x0033E004 File Offset: 0x0033C204
	public static void HarvestOnAttack(ItemActionData _actionData, Dictionary<string, ItemActionAttack.Bonuses> ToolBonuses)
	{
		if (_actionData.invData.world.IsEditor() || !(_actionData.invData.holdingEntity is EntityPlayerLocal) || _actionData.attackDetails == null || _actionData.attackDetails.itemsToDrop == null)
		{
			return;
		}
		if (GameUtils.random == null)
		{
			GameUtils.random = GameRandomManager.Instance.CreateGameRandom();
			GameUtils.random.SetSeed((int)Stopwatch.GetTimestamp());
		}
		Block block = _actionData.attackDetails.blockBeingDamaged.Block;
		if (block.RepairItemsMeshDamage != null)
		{
			BlockValue blockBeingDamaged = _actionData.attackDetails.blockBeingDamaged;
			blockBeingDamaged.damage += _actionData.attackDetails.damageGiven;
			_actionData.attackDetails.bKilled = (blockBeingDamaged.damage < block.MaxDamage && block.shape.UseRepairDamageState(blockBeingDamaged));
		}
		if (_actionData.attackDetails.bKilled)
		{
			if (!_actionData.attackDetails.itemsToDrop.ContainsKey(EnumDropEvent.Destroy))
			{
				if (!_actionData.attackDetails.blockBeingDamaged.isair && _actionData.attackDetails.bBlockHit)
				{
					ItemValue iv = _actionData.attackDetails.blockBeingDamaged.ToItemValue();
					int count = 1;
					GameUtils.collectHarvestedItem(_actionData, iv, count, 1f, false);
				}
			}
			else
			{
				List<Block.SItemDropProb> list = _actionData.attackDetails.itemsToDrop[EnumDropEvent.Destroy];
				for (int i = 0; i < list.Count; i++)
				{
					if (_actionData.attackDetails.bBlockHit && list[i].name.Equals("[recipe]"))
					{
						List<Recipe> recipes = CraftingManager.GetRecipes(_actionData.attackDetails.blockBeingDamaged.Block.GetBlockName());
						if (recipes.Count > 0)
						{
							for (int j = 0; j < recipes[0].ingredients.Count; j++)
							{
								if (recipes[0].ingredients[j].count / 2 > 0)
								{
									GameUtils.collectHarvestedItem(_actionData, recipes[0].ingredients[j].itemValue, recipes[0].ingredients[j].count / 2, 1f, false);
								}
							}
						}
					}
					else
					{
						float num = 1f;
						if (list[i].toolCategory != null)
						{
							num = 0f;
							if (ToolBonuses != null && ToolBonuses.ContainsKey(list[i].toolCategory))
							{
								num = ToolBonuses[list[i].toolCategory].Tool;
							}
						}
						num = EffectManager.GetValue(PassiveEffects.HarvestCount, _actionData.invData.itemValue, num, _actionData.invData.holdingEntity, null, FastTags<TagGroup.Global>.Parse(list[i].tag), true, true, true, true, true, 1, true, false);
						ItemValue itemValue = list[i].name.Equals("*") ? _actionData.attackDetails.blockBeingDamaged.ToItemValue() : new ItemValue(ItemClass.GetItem(list[i].name, false).type, false);
						if (itemValue.type != 0 && ItemClass.list[itemValue.type] != null && (list[i].prob > 0.999f || GameUtils.random.RandomFloat <= list[i].prob))
						{
							int num2 = (int)((float)GameUtils.random.RandomRange(list[i].minCount, list[i].maxCount + 1) * num);
							if (num2 > 0)
							{
								GameUtils.collectHarvestedItem(_actionData, itemValue, num2, 1f, false);
							}
						}
					}
				}
			}
		}
		if (_actionData.attackDetails.bBlockHit)
		{
			_actionData.invData.holdingEntity.MinEventContext.BlockValue = _actionData.attackDetails.blockBeingDamaged;
			_actionData.invData.holdingEntity.FireEvent(MinEventTypes.onSelfHarvestBlock, true);
		}
		else
		{
			_actionData.invData.holdingEntity.MinEventContext.Other = (_actionData.attackDetails.entityHit as EntityAlive);
			_actionData.invData.holdingEntity.FireEvent(MinEventTypes.onSelfHarvestOther, true);
		}
		if (_actionData.attackDetails.itemsToDrop.ContainsKey(EnumDropEvent.Harvest))
		{
			List<Block.SItemDropProb> list2 = _actionData.attackDetails.itemsToDrop[EnumDropEvent.Harvest];
			for (int k = 0; k < list2.Count; k++)
			{
				float num3 = 0f;
				if (list2[k].toolCategory != null)
				{
					num3 = 0f;
					if (ToolBonuses != null && ToolBonuses.ContainsKey(list2[k].toolCategory))
					{
						num3 = ToolBonuses[list2[k].toolCategory].Tool;
					}
				}
				ItemValue itemValue2 = list2[k].name.Equals("*") ? _actionData.attackDetails.blockBeingDamaged.ToItemValue() : new ItemValue(ItemClass.GetItem(list2[k].name, false).type, false);
				if (itemValue2.type != 0 && ItemClass.list[itemValue2.type] != null)
				{
					num3 = EffectManager.GetValue(PassiveEffects.HarvestCount, _actionData.invData.itemValue, num3, _actionData.invData.holdingEntity, null, FastTags<TagGroup.Global>.Parse(list2[k].tag), true, true, true, true, true, 1, true, false);
					int num4 = (int)((float)GameUtils.random.RandomRange(list2[k].minCount, list2[k].maxCount + 1) * num3);
					int num5 = num4 - num4 / 3;
					if (num5 > 0)
					{
						GameUtils.collectHarvestedItem(_actionData, itemValue2, num5, list2[k].prob, true);
					}
					if (_actionData.attackDetails.bKilled)
					{
						num5 = num4 / 3;
						float num6 = list2[k].prob;
						float resourceScale = list2[k].resourceScale;
						if (resourceScale > 0f && resourceScale < 1f)
						{
							num6 /= resourceScale;
							num5 = (int)((float)num5 * resourceScale);
							if (num5 < 1)
							{
								num5++;
							}
						}
						if (num5 > 0)
						{
							GameUtils.collectHarvestedItem(_actionData, itemValue2, num5, num6, false);
						}
					}
				}
			}
		}
		_actionData.attackDetails.blockBeingDamaged = BlockValue.Air;
	}

	// Token: 0x06007FBE RID: 32702 RVA: 0x0033E64C File Offset: 0x0033C84C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void collectHarvestedItem(ItemActionData _actionData, ItemValue _iv, int _count, float _prob, bool _bScaleCountOnDamage = true)
	{
		if (GameUtils.random == null)
		{
			GameUtils.random = GameRandomManager.Instance.CreateGameRandom();
			GameUtils.random.SetSeed((int)Stopwatch.GetTimestamp());
		}
		if (_bScaleCountOnDamage)
		{
			float num = (float)_actionData.attackDetails.damageMax / (float)_count;
			int num2 = (int)((Utils.FastMin(_actionData.attackDetails.damageTotalOfTarget, (float)_actionData.attackDetails.damageMax) - (float)_actionData.attackDetails.damageGiven) / num + 0.5f);
			int num3 = Mathf.Min((int)(_actionData.attackDetails.damageTotalOfTarget / num + 0.5f), _count);
			int b = _count;
			_count = num3 - num2;
			if (_actionData.attackDetails.damageTotalOfTarget > (float)_actionData.attackDetails.damageMax)
			{
				_count = Mathf.Min(_count, b);
			}
		}
		if (GameUtils.random.RandomFloat <= _prob && _count > 0)
		{
			ItemStack itemStack = new ItemStack(_iv, _count);
			LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(_actionData.invData.holdingEntity as EntityPlayerLocal);
			XUiM_PlayerInventory playerInventory = uiforPlayer.xui.PlayerInventory;
			QuestEventManager.Current.HarvestedItem(_actionData.invData.itemValue, itemStack, _actionData.attackDetails.blockBeingDamaged);
			if (!playerInventory.AddItem(itemStack))
			{
				GameManager.Instance.ItemDropServer(new ItemStack(_iv, itemStack.count), GameManager.Instance.World.GetPrimaryPlayer().GetDropPosition(), new Vector3(0.5f, 0.5f, 0.5f), GameManager.Instance.World.GetPrimaryPlayerId(), 60f, false);
			}
			uiforPlayer.entityPlayer.Progression.AddLevelExp((int)(itemStack.itemValue.ItemClass.MadeOfMaterial.Experience * (float)_count), "_xpFromHarvesting", Progression.XPTypes.Harvesting, true, true);
		}
	}

	// Token: 0x06007FBF RID: 32703 RVA: 0x0033E7F8 File Offset: 0x0033C9F8
	public static void DrawCube(Vector3 _pos, Color _col)
	{
		UnityEngine.Debug.DrawLine(_pos + new Vector3(0.1f, 0.1f, 0.1f), _pos + new Vector3(0.9f, 0.1f, 0.1f), _col, 10f);
		UnityEngine.Debug.DrawLine(_pos + new Vector3(0.1f, 0.1f, 0.1f), _pos + new Vector3(0.1f, 0.1f, 0.9f), _col, 10f);
		UnityEngine.Debug.DrawLine(_pos + new Vector3(0.9f, 0.1f, 0.1f), _pos + new Vector3(0.9f, 0.1f, 0.9f), _col, 10f);
		UnityEngine.Debug.DrawLine(_pos + new Vector3(0.9f, 0.1f, 0.9f), _pos + new Vector3(0.1f, 0.1f, 0.9f), _col, 10f);
		UnityEngine.Debug.DrawLine(_pos + new Vector3(0.1f, 0.9f, 0.1f), _pos + new Vector3(0.9f, 0.9f, 0.1f), _col, 10f);
		UnityEngine.Debug.DrawLine(_pos + new Vector3(0.1f, 0.9f, 0.1f), _pos + new Vector3(0.1f, 0.9f, 0.9f), _col, 10f);
		UnityEngine.Debug.DrawLine(_pos + new Vector3(0.9f, 0.9f, 0.1f), _pos + new Vector3(0.9f, 0.9f, 0.9f), _col, 10f);
		UnityEngine.Debug.DrawLine(_pos + new Vector3(0.9f, 0.9f, 0.9f), _pos + new Vector3(0.1f, 0.9f, 0.9f), _col, 10f);
		UnityEngine.Debug.DrawLine(_pos + new Vector3(0.1f, 0.1f, 0.1f), _pos + new Vector3(0.1f, 0.9f, 0.1f), _col, 10f);
		UnityEngine.Debug.DrawLine(_pos + new Vector3(0.1f, 0.1f, 0.9f), _pos + new Vector3(0.1f, 0.9f, 0.9f), _col, 10f);
		UnityEngine.Debug.DrawLine(_pos + new Vector3(0.9f, 0.1f, 0.1f), _pos + new Vector3(0.9f, 0.9f, 0.1f), _col, 10f);
		UnityEngine.Debug.DrawLine(_pos + new Vector3(0.9f, 0.1f, 0.9f), _pos + new Vector3(0.9f, 0.9f, 0.9f), _col, 10f);
	}

	// Token: 0x06007FC0 RID: 32704 RVA: 0x0033EAF9 File Offset: 0x0033CCF9
	public static string SafeStringFormat(string _s)
	{
		return _s.Replace("{", "{{").Replace("}", "}}");
	}

	// Token: 0x06007FC1 RID: 32705 RVA: 0x0033EB1C File Offset: 0x0033CD1C
	public static Vector3 GetNormalFromHitInfo(Vector3i _blockPos, Collider _hitCollider, int _hitTriangleIdx, out Vector3 _hitFaceCenter)
	{
		_hitFaceCenter = Vector3.zero;
		if (_hitTriangleIdx < 0)
		{
			return Vector3.zero;
		}
		MeshCollider meshCollider = _hitCollider as MeshCollider;
		if (!(meshCollider != null) || !(meshCollider.sharedMesh != null) || !meshCollider.sharedMesh.isReadable)
		{
			return Vector3.zero;
		}
		Mesh sharedMesh = meshCollider.sharedMesh;
		GameUtils.tempVertices.Clear();
		sharedMesh.GetVertices(GameUtils.tempVertices);
		GameUtils.tempTriangles.Clear();
		sharedMesh.GetTriangles(GameUtils.tempTriangles, 0);
		int num = _hitTriangleIdx * 3;
		if (num >= GameUtils.tempTriangles.Count || GameUtils.tempTriangles[num] >= GameUtils.tempVertices.Count)
		{
			return Vector3.zero;
		}
		Vector3 a = GameUtils.tempVertices[GameUtils.tempTriangles[num]];
		Vector3 b = GameUtils.tempVertices[GameUtils.tempTriangles[num + 1]];
		Vector3 b2 = GameUtils.tempVertices[GameUtils.tempTriangles[num + 2]];
		Vector3 result = Vector3.Cross(a - b, a - b2);
		Vector3 a2 = (a + b + b2) / 3f;
		_hitFaceCenter = a2 + World.toChunkXyzWorldPos(_blockPos);
		return result;
	}

	// Token: 0x06007FC2 RID: 32706 RVA: 0x0033EC60 File Offset: 0x0033CE60
	public static BlockFace GetBlockFaceFromHitInfo(Vector3i _blockPos, BlockValue _blockValue, Collider _hitCollider, int _hitTriangleIdx, out Vector3 _hitFaceCenter, out Vector3 _hitFaceNormal)
	{
		_hitFaceCenter = Vector3.zero;
		_hitFaceNormal = Vector3.zero;
		if (_hitTriangleIdx < 0)
		{
			return BlockFace.None;
		}
		MeshCollider meshCollider = _hitCollider as MeshCollider;
		if (meshCollider != null && meshCollider.sharedMesh != null && meshCollider.sharedMesh.isReadable)
		{
			Mesh sharedMesh = meshCollider.sharedMesh;
			GameUtils.tempVertices.Clear();
			sharedMesh.GetVertices(GameUtils.tempVertices);
			GameUtils.tempTriangles.Clear();
			sharedMesh.GetTriangles(GameUtils.tempTriangles, 0);
			int num = _hitTriangleIdx * 3;
			if (num >= GameUtils.tempTriangles.Count || GameUtils.tempTriangles[num] >= GameUtils.tempVertices.Count)
			{
				return BlockFace.None;
			}
			Vector3 vector = GameUtils.tempVertices[GameUtils.tempTriangles[num]];
			Vector3 vector2 = GameUtils.tempVertices[GameUtils.tempTriangles[num + 1]];
			Vector3 vector3 = GameUtils.tempVertices[GameUtils.tempTriangles[num + 2]];
			_hitFaceNormal = Vector3.Cross(vector - vector2, vector - vector3);
			Vector3 a = (vector + vector2 + vector3) / 3f;
			_hitFaceCenter = a + World.toChunkXyzWorldPos(_blockPos);
			Vector3 b = World.toBlock(_blockPos).ToVector3();
			vector -= b;
			vector2 -= b;
			vector3 -= b;
			if (!_blockValue.Block.isMultiBlock)
			{
				if ((double)vector.x < -0.001)
				{
					vector.x += 16f;
				}
				else if (vector.x > 15f)
				{
					vector.x -= 16f;
				}
				if ((double)vector.y < -0.001)
				{
					vector.y += 16f;
				}
				else if (vector.y > 15f)
				{
					vector.y -= 16f;
				}
				if ((double)vector.z < -0.001)
				{
					vector.z += 16f;
				}
				else if (vector.z > 15f)
				{
					vector.z -= 16f;
				}
				if ((double)vector2.x < -0.001)
				{
					vector2.x += 16f;
				}
				else if (vector2.x > 15f)
				{
					vector2.x -= 16f;
				}
				if ((double)vector2.y < -0.001)
				{
					vector2.y += 16f;
				}
				else if (vector2.y > 15f)
				{
					vector2.y -= 16f;
				}
				if ((double)vector2.z < -0.001)
				{
					vector2.z += 16f;
				}
				else if (vector2.z > 15f)
				{
					vector2.z -= 16f;
				}
				if ((double)vector3.x < -0.001)
				{
					vector3.x += 16f;
				}
				else if (vector3.x > 15f)
				{
					vector3.x -= 16f;
				}
				if ((double)vector3.y < -0.001)
				{
					vector3.y += 16f;
				}
				else if (vector3.y > 15f)
				{
					vector3.y -= 16f;
				}
				if ((double)vector3.z < -0.001)
				{
					vector3.z += 16f;
				}
				else if (vector3.z > 15f)
				{
					vector3.z -= 16f;
				}
			}
			BlockShapeNew blockShapeNew = _blockValue.Block.shape as BlockShapeNew;
			if (blockShapeNew != null)
			{
				Vector3 b2 = Vector3.one * 0.5f;
				Quaternion rotation = Quaternion.Inverse(blockShapeNew.GetRotation(_blockValue));
				vector = rotation * (vector - b2) + b2;
				vector2 = rotation * (vector2 - b2) + b2;
				vector3 = rotation * (vector3 - b2) + b2;
				return blockShapeNew.GetBlockFaceFromColliderTriangle(_blockValue, vector, vector2, vector3);
			}
		}
		return BlockFace.None;
	}

	// Token: 0x06007FC3 RID: 32707 RVA: 0x0033F0CC File Offset: 0x0033D2CC
	public static string GetLaunchArgument(string _argumentName)
	{
		if (GameUtils.arguments == null)
		{
			GameUtils.arguments = new CaseInsensitiveStringDictionary<string>();
			string[] commandLineArgs = GameStartupHelper.GetCommandLineArgs();
			for (int i = 0; i < commandLineArgs.Length; i++)
			{
				if (!string.IsNullOrEmpty(commandLineArgs[i]) && commandLineArgs[i][0] == '-')
				{
					int num = commandLineArgs[i].IndexOf('=');
					string key;
					string value;
					if (num >= 0)
					{
						key = commandLineArgs[i].Substring(1, num - 1);
						value = commandLineArgs[i].Substring(num + 1);
					}
					else
					{
						key = commandLineArgs[i].Substring(1);
						value = string.Empty;
					}
					GameUtils.arguments[key] = value;
				}
			}
		}
		if (GameUtils.arguments.ContainsKey(_argumentName))
		{
			return GameUtils.arguments[_argumentName];
		}
		return null;
	}

	// Token: 0x06007FC4 RID: 32708 RVA: 0x0033F17C File Offset: 0x0033D37C
	public static bool IsBlockOrTerrain(string _tag)
	{
		return _tag == "B_Mesh" || _tag == "T_Mesh" || _tag == "T_Mesh_B" || _tag == "T_Block" || _tag == "T_Deco";
	}

	// Token: 0x06007FC5 RID: 32709 RVA: 0x0033F1CC File Offset: 0x0033D3CC
	public static bool IsBlockOrTerrain(Component component)
	{
		return component.CompareTag("B_Mesh") || component.CompareTag("T_Mesh") || component.CompareTag("T_Mesh_B") || component.CompareTag("T_Block") || component.CompareTag("T_Deco");
	}

	// Token: 0x06007FC6 RID: 32710 RVA: 0x0033F21C File Offset: 0x0033D41C
	public static ulong Vector3iToUInt64(Vector3i _v)
	{
		return (ulong)((long)(_v.x + 32768 & 65535) << 32 | (long)(_v.y + 32768 & 65535) << 16 | ((long)(_v.z + 32768) & 65535L));
	}

	// Token: 0x06007FC7 RID: 32711 RVA: 0x0033F26B File Offset: 0x0033D46B
	public static Vector3i UInt64ToVector3i(ulong _fullValue)
	{
		return new Vector3i((int)(_fullValue >> 32 & 65535UL) - 32768, (int)(_fullValue >> 16 & 65535UL) - 32768, (int)(_fullValue & 65535UL) - 32768);
	}

	// Token: 0x06007FC8 RID: 32712 RVA: 0x0033F2A8 File Offset: 0x0033D4A8
	public static char ValidateGameNameInput(string _text, int _charIndex, char _addedChar)
	{
		if (_addedChar >= 'Ā')
		{
			return _addedChar;
		}
		if ((_addedChar >= 'a' && _addedChar <= 'z') || (_addedChar >= 'A' && _addedChar <= 'Z') || (_addedChar >= '0' && _addedChar <= '9'))
		{
			return _addedChar;
		}
		if (_addedChar == '_' || _addedChar == '-')
		{
			return _addedChar;
		}
		if (_charIndex > 0 && (_addedChar == '.' || _addedChar == ' '))
		{
			return _addedChar;
		}
		return '\0';
	}

	// Token: 0x06007FC9 RID: 32713 RVA: 0x0033F2FC File Offset: 0x0033D4FC
	public static char ValidateHexInput(string _text, int _charIndex, char _addedChar)
	{
		if ((_addedChar >= 'a' && _addedChar <= 'f') || (_addedChar >= 'A' && _addedChar <= 'F') || (_addedChar >= '0' && _addedChar <= '9'))
		{
			return _addedChar;
		}
		return '\0';
	}

	// Token: 0x06007FCA RID: 32714 RVA: 0x0033F320 File Offset: 0x0033D520
	public static bool ValidateGameName(string _gameName)
	{
		string text = _gameName.Trim();
		if (string.IsNullOrEmpty(text) || text.Length != _gameName.Length)
		{
			return false;
		}
		for (int i = 0; i < _gameName.Length; i++)
		{
			if (GameUtils.ValidateGameNameInput(_gameName, i, _gameName[i]) == '\0')
			{
				return false;
			}
		}
		return !_gameName.EndsWith(".");
	}

	// Token: 0x06007FCB RID: 32715 RVA: 0x0033F380 File Offset: 0x0033D580
	public static PrefabInstance FindPrefabForBlockPos(List<PrefabInstance> prefabs, Vector3i hitPointBlockPos)
	{
		for (int i = 0; i < prefabs.Count; i++)
		{
			if (prefabs[i].boundingBoxPosition.x <= hitPointBlockPos.x && prefabs[i].boundingBoxPosition.x + prefabs[i].boundingBoxSize.x >= hitPointBlockPos.x && prefabs[i].boundingBoxPosition.z <= hitPointBlockPos.z && prefabs[i].boundingBoxPosition.z + prefabs[i].boundingBoxSize.z >= hitPointBlockPos.z)
			{
				return prefabs[i];
			}
		}
		return null;
	}

	// Token: 0x06007FCC RID: 32716 RVA: 0x0033F438 File Offset: 0x0033D638
	public static int FindPaintIdForBlockFace(BlockValue _bv, BlockFace blockFace, out string _name, int _channel)
	{
		int sideTextureId = _bv.Block.GetSideTextureId(_bv, blockFace, _channel);
		for (int i = 0; i < BlockTextureData.list.Length; i++)
		{
			if (BlockTextureData.list[i] != null && (int)BlockTextureData.list[i].TextureID == sideTextureId)
			{
				_name = BlockTextureData.list[i].Name;
				return i;
			}
		}
		_name = string.Empty;
		return 0;
	}

	// Token: 0x06007FCD RID: 32717 RVA: 0x0033F498 File Offset: 0x0033D698
	public static Vector3i Mirror(EnumMirrorAlong _axis, Vector3i _pos, Vector3i _prefabSize)
	{
		if (_axis == EnumMirrorAlong.XAxis)
		{
			return new Vector3i(_prefabSize.x - _pos.x - 1, _pos.y, _pos.z);
		}
		if (_axis != EnumMirrorAlong.YAxis)
		{
			return new Vector3i(_pos.x, _pos.y, _prefabSize.z - _pos.z - 1);
		}
		return new Vector3i(_pos.x, _prefabSize.y - _pos.y - 1, _pos.z);
	}

	// Token: 0x06007FCE RID: 32718 RVA: 0x0033F510 File Offset: 0x0033D710
	public static Vector3 Mirror(EnumMirrorAlong _axis, Vector3 _pos, Vector3i _prefabSize)
	{
		if (_axis == EnumMirrorAlong.XAxis)
		{
			return new Vector3((float)_prefabSize.x - _pos.x, _pos.y, _pos.z);
		}
		if (_axis != EnumMirrorAlong.YAxis)
		{
			return new Vector3(_pos.x, _pos.y, (float)_prefabSize.z - _pos.z);
		}
		return new Vector3(_pos.x, (float)_prefabSize.y - _pos.y, _pos.z);
	}

	// Token: 0x06007FCF RID: 32719 RVA: 0x0033F585 File Offset: 0x0033D785
	public static void TakeScreenShot(GameUtils.EScreenshotMode _screenshotMode, string _overrideScreenshotFilePath = null, float _borderPerc = 0f, bool _b4to3 = false, int _rescaleToW = 0, int _rescaleToH = 0, bool _isSaveTGA = false)
	{
		ThreadManager.StartCoroutine(GameUtils.TakeScreenshotEnum(_screenshotMode, _overrideScreenshotFilePath, _borderPerc, _b4to3, _rescaleToW, _rescaleToH, _isSaveTGA));
	}

	// Token: 0x06007FD0 RID: 32720 RVA: 0x0033F59C File Offset: 0x0033D79C
	public static IEnumerator TakeScreenshotEnum(GameUtils.EScreenshotMode _screenshotMode, string _overrideScreenshotFilePath = null, float _borderPerc = 0f, bool _b4to3 = false, int _rescaleToW = 0, int _rescaleToH = 0, bool _isSaveTGA = false)
	{
		yield return new WaitForEndOfFrame();
		Rect screenshotRect = GameUtils.GetScreenshotRect(_borderPerc, _b4to3);
		Texture2D texture2D = new Texture2D((int)screenshotRect.width, (int)screenshotRect.height, TextureFormat.RGB24, false);
		texture2D.ReadPixels(screenshotRect, 0, 0);
		if (_rescaleToW != 0 && _rescaleToH != 0)
		{
			TextureScale.Bilinear(texture2D, _rescaleToW, _rescaleToH);
		}
		texture2D.Apply();
		if (_screenshotMode != GameUtils.EScreenshotMode.File)
		{
			TextureUtils.CopyToClipboard(texture2D);
		}
		if (_screenshotMode != GameUtils.EScreenshotMode.Clipboard)
		{
			string text3;
			if (_overrideScreenshotFilePath == null)
			{
				string text = GameIO.GetUserGameDataDir() + "/Screenshots";
				if (!SdDirectory.Exists(text))
				{
					SdDirectory.CreateDirectory(text);
				}
				string text2 = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
				text3 = string.Concat(new string[]
				{
					text,
					"/",
					Constants.cVersionInformation.ShortString,
					"_",
					text2
				});
			}
			else
			{
				text3 = _overrideScreenshotFilePath;
			}
			text3 += (_isSaveTGA ? ".tga" : ".jpg");
			GameUtils.lastSavedScreenshotFilename = text3;
			if (_isSaveTGA)
			{
				SdFile.WriteAllBytes(text3, texture2D.EncodeToTGA());
			}
			else
			{
				SdFile.WriteAllBytes(text3, texture2D.EncodeToJPG());
			}
		}
		UnityEngine.Object.Destroy(texture2D);
		yield break;
	}

	// Token: 0x06007FD1 RID: 32721 RVA: 0x0033F5D8 File Offset: 0x0033D7D8
	public static Rect GetScreenshotRect(float _borderPerc = 0f, bool _b4to3 = false)
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = (float)Screen.width;
		float num4 = (float)Screen.height;
		if (_borderPerc > 0.001f)
		{
			float num5 = (float)Screen.width * _borderPerc;
			float num6 = (float)Screen.height * _borderPerc;
			num += num5;
			num3 -= num5 * 2f;
			num2 += num6;
			num4 -= num6 * 2f;
		}
		if (_b4to3)
		{
			num3 = 1.3333334f * num4;
			num = ((float)Screen.width - num3) / 2f;
		}
		return new Rect(num, num2, num3, num4);
	}

	// Token: 0x06007FD2 RID: 32722 RVA: 0x0033F65E File Offset: 0x0033D85E
	public static void StartPlaytesting()
	{
		if (string.IsNullOrEmpty(GamePrefs.GetString(EnumGamePrefs.LastLoadedPrefab)))
		{
			return;
		}
		GameManager.bHideMainMenuNextTime = true;
		GameManager.Instance.Disconnect();
		ThreadManager.StartCoroutine(GameUtils.startPlaytestLater());
	}

	// Token: 0x06007FD3 RID: 32723 RVA: 0x0033F68D File Offset: 0x0033D88D
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator startPlaytestLater()
	{
		yield return new WaitForSeconds(2f);
		string @string = GamePrefs.GetString(EnumGamePrefs.LastLoadedPrefab);
		GamePrefs.Set(EnumGamePrefs.GameWorld, "Playtesting");
		GamePrefs.Set(EnumGamePrefs.GameMode, EnumGameMode.Survival.ToStringCached<EnumGameMode>());
		GamePrefs.Set(EnumGamePrefs.GameName, @string);
		string saveGameDir = GameIO.GetSaveGameDir();
		if (SdDirectory.Exists(saveGameDir))
		{
			SdDirectory.Delete(saveGameDir, true);
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.StartServers(GamePrefs.GetString(EnumGamePrefs.ServerPassword), false);
		yield break;
	}

	// Token: 0x06007FD4 RID: 32724 RVA: 0x0033F695 File Offset: 0x0033D895
	public static bool IsPlaytesting()
	{
		return GamePrefs.GetString(EnumGamePrefs.GameWorld) == "Playtesting";
	}

	// Token: 0x06007FD5 RID: 32725 RVA: 0x0033F6A8 File Offset: 0x0033D8A8
	public static bool IsWorldEditor()
	{
		return GameModeEditWorld.TypeName.Equals(GamePrefs.GetString(EnumGamePrefs.GameMode)) && GamePrefs.GetString(EnumGamePrefs.GameName) == "WorldEditor";
	}

	// Token: 0x06007FD6 RID: 32726 RVA: 0x0033F6D0 File Offset: 0x0033D8D0
	public static void StartSinglePrefabEditing()
	{
		GameManager.bHideMainMenuNextTime = true;
		GameManager.Instance.Disconnect();
		ThreadManager.StartCoroutine(GameUtils.startSinglePrefabEditingLater());
	}

	// Token: 0x06007FD7 RID: 32727 RVA: 0x0033F6ED File Offset: 0x0033D8ED
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator startSinglePrefabEditingLater()
	{
		yield return new WaitForSeconds(2f);
		GamePrefs.Set(EnumGamePrefs.GameWorld, "Empty");
		GamePrefs.Set(EnumGamePrefs.GameMode, GameModeEditWorld.TypeName);
		GamePrefs.Set(EnumGamePrefs.GameName, "PrefabEditor");
		SingletonMonoBehaviour<ConnectionManager>.Instance.StartServers(GamePrefs.GetString(EnumGamePrefs.ServerPassword), false);
		yield break;
	}

	// Token: 0x06007FD8 RID: 32728 RVA: 0x0033F6F5 File Offset: 0x0033D8F5
	public static float GetOreNoiseAt(PerlinNoise _noise, int _x, int _y, int _z)
	{
		return ((float)_noise.Noise((double)((float)_x * 0.05f), (double)((float)_y * 0.05f), (double)((float)_z * 0.05f)) - 0.333f) * 3f;
	}

	// Token: 0x06007FD9 RID: 32729 RVA: 0x0033F725 File Offset: 0x0033D925
	public static bool CheckOreNoiseAt(PerlinNoise _noise, int _x, int _y, int _z)
	{
		return GameUtils.GetOreNoiseAt(_noise, _x, _y, _z) > 0f;
	}

	// Token: 0x06007FDA RID: 32730 RVA: 0x0033F737 File Offset: 0x0033D937
	public static Color32 UIntToColor(uint color, bool _includeAlpha = false)
	{
		if (_includeAlpha)
		{
			return new Color32((byte)(color >> 16), (byte)(color >> 8), (byte)color, (byte)(color >> 24));
		}
		return new Color32((byte)(color >> 16), (byte)(color >> 8), (byte)color, byte.MaxValue);
	}

	// Token: 0x06007FDB RID: 32731 RVA: 0x0033F768 File Offset: 0x0033D968
	public static uint ColorToUInt(Color32 color, bool _includeAlpha = false)
	{
		if (_includeAlpha)
		{
			return (uint)((int)color.r << 24 | (int)color.r << 16 | (int)color.g << 8 | (int)color.b);
		}
		return (uint)((int)color.r << 16 | (int)color.g << 8 | (int)color.b);
	}

	// Token: 0x06007FDC RID: 32732 RVA: 0x0033F7B8 File Offset: 0x0033D9B8
	public static void WaterFloodFill(GridCompressedData<byte> _cols, byte[] _waterChunks16x16Height, int _width, HeightMap _heightMap, int _posX, int _maxY, int _posZ, byte _colWater, byte _colBorder, List<Vector2i> _listPos, int _minX = -2147483648, int _maxX = 2147483647, int _minZ = -2147483648, int _maxZ = 2147483647, int _worldScale = 1)
	{
		int num = _heightMap.GetHeight() * _worldScale;
		do
		{
			int num2 = _posX + _width / 2;
			int num3 = _posZ + num / 2;
			if (_heightMap.GetAt(num2, num3) < (float)(_maxY + 1))
			{
				_cols.SetValue(num2, num3, _colWater);
				_waterChunks16x16Height[num2 / 16 + num3 / 16 * _width / 16] = (byte)_maxY;
				Vector2i vector2i;
				if (num2 < _width - 1 && _posX < _maxX && _cols.GetValue(num2 + 1, num3) == 0 && _listPos.Count < 100000)
				{
					vector2i.x = _posX + 1;
					vector2i.y = _posZ;
					_listPos.Add(vector2i);
				}
				if (num2 > 0 && _posX > _minX && _cols.GetValue(num2 - 1, num3) == 0 && _listPos.Count < 100000)
				{
					vector2i.x = _posX - 1;
					vector2i.y = _posZ;
					_listPos.Add(vector2i);
				}
				if (num3 > 0 && _posZ > _minZ && _cols.GetValue(num2, num3 - 1) == 0 && _listPos.Count < 100000)
				{
					vector2i.x = _posX;
					vector2i.y = _posZ - 1;
					_listPos.Add(vector2i);
				}
				if (num3 < num - 1 && _posZ < _maxZ && _cols.GetValue(num2, num3 + 1) == 0 && _listPos.Count < 100000)
				{
					vector2i.x = _posX;
					vector2i.y = _posZ + 1;
					_listPos.Add(vector2i);
				}
			}
			else
			{
				_cols.SetValue(num2, num3, _colBorder);
			}
			int count = _listPos.Count;
			if (count > 0)
			{
				Vector2i vector2i = _listPos[count - 1];
				_posX = vector2i.x;
				_posZ = vector2i.y;
				_listPos.RemoveAt(count - 1);
			}
		}
		while (_listPos.Count > 0);
	}

	// Token: 0x06007FDD RID: 32733 RVA: 0x0033F960 File Offset: 0x0033DB60
	public static GameUtils.EPlayerHomeType CheckForAnyPlayerHome(World world, Vector3i BoxMin, Vector3i BoxMax)
	{
		double num = (double)GameStats.GetInt(EnumGameStats.LandClaimExpiryTime) * 24.0;
		double num2 = (double)GameStats.GetInt(EnumGameStats.BedrollExpiryTime) * 24.0;
		int @int = GamePrefs.GetInt(EnumGamePrefs.BedrollDeadZoneSize);
		Vector3i other = new Vector3i(@int, @int, @int);
		Vector3i vector3i = BoxMin - other;
		Vector3i vector3i2 = BoxMax + other;
		int int2 = GameStats.GetInt(EnumGameStats.LandClaimSize);
		int num3 = int2 / 2;
		foreach (KeyValuePair<PlatformUserIdentifierAbs, PersistentPlayerData> keyValuePair in GameManager.Instance.GetPersistentPlayerList().Players)
		{
			if (keyValuePair.Value.OfflineHours < num2 && keyValuePair.Value.HasBedrollPos)
			{
				Vector3i bedrollPos = keyValuePair.Value.BedrollPos;
				if (bedrollPos.x >= vector3i.x && bedrollPos.x < vector3i2.x && bedrollPos.z >= vector3i.z && bedrollPos.z < vector3i2.z)
				{
					return GameUtils.EPlayerHomeType.Bedroll;
				}
			}
			List<Vector3i> lpblocks = keyValuePair.Value.LPBlocks;
			if (keyValuePair.Value.OfflineHours < num && lpblocks != null && lpblocks.Count > 0)
			{
				for (int i = 0; i < lpblocks.Count; i++)
				{
					Vector3i vector3i3 = lpblocks[i];
					vector3i3.x -= num3;
					vector3i3.z -= num3;
					if (vector3i3.x <= BoxMax.x && vector3i3.x + int2 >= BoxMin.x && vector3i3.z <= BoxMax.z && vector3i3.z + int2 >= BoxMin.z)
					{
						return GameUtils.EPlayerHomeType.Landclaim;
					}
				}
			}
		}
		return GameUtils.EPlayerHomeType.None;
	}

	// Token: 0x06007FDE RID: 32734 RVA: 0x0033FB50 File Offset: 0x0033DD50
	public static Transform FindDeepChild(Transform _parent, string _transformName)
	{
		Transform transform = _parent.Find(_transformName);
		if (transform != null)
		{
			return transform;
		}
		int childCount = _parent.childCount;
		for (int i = 0; i < childCount; i++)
		{
			transform = GameUtils.FindDeepChild(_parent.GetChild(i), _transformName);
			if (transform != null)
			{
				return transform;
			}
		}
		return transform;
	}

	// Token: 0x06007FDF RID: 32735 RVA: 0x0033FBA0 File Offset: 0x0033DDA0
	public static Transform FindDeepChildActive(Transform _parent, string _transformName)
	{
		Transform transform = _parent.Find(_transformName);
		if (transform != null && transform.gameObject.activeSelf)
		{
			return transform;
		}
		int childCount = _parent.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = _parent.GetChild(i);
			if (child.gameObject.activeSelf)
			{
				transform = GameUtils.FindDeepChildActive(child, _transformName);
				if (transform != null)
				{
					return transform;
				}
			}
		}
		return transform;
	}

	// Token: 0x06007FE0 RID: 32736 RVA: 0x0033FC09 File Offset: 0x0033DE09
	public static int GetViewDistance()
	{
		if (GamePrefs.GetString(EnumGamePrefs.GameWorld) == "Empty")
		{
			return 12;
		}
		return GameStats.GetInt(EnumGameStats.AllowedViewDistance);
	}

	// Token: 0x06007FE1 RID: 32737 RVA: 0x0033FC28 File Offset: 0x0033DE28
	public static Vector3 GetUpdatedNormalAtPosition(Vector3i _worldPos, int _clrIdx, bool _saveNrmToChunk = false)
	{
		int terrainHeight = (int)GameManager.Instance.World.GetTerrainHeight(_worldPos.x, _worldPos.z);
		int terrainHeight2 = (int)GameManager.Instance.World.GetTerrainHeight(_worldPos.x + 1, _worldPos.z);
		float terrainHeight3 = (float)GameManager.Instance.World.GetTerrainHeight(_worldPos.x, _worldPos.z + 1);
		float num = (float)GameManager.Instance.World.GetDensity(_clrIdx, _worldPos.x, terrainHeight, _worldPos.z) / -128f;
		float num2 = (float)GameManager.Instance.World.GetDensity(_clrIdx, _worldPos.x + 1, terrainHeight, _worldPos.z) / -128f;
		float num3 = (float)GameManager.Instance.World.GetDensity(_clrIdx, _worldPos.x, terrainHeight, _worldPos.z + 1) / -128f;
		float num4 = (float)GameManager.Instance.World.GetDensity(_clrIdx, _worldPos.x, terrainHeight + 1, _worldPos.z) / -128f;
		float num5 = (float)GameManager.Instance.World.GetDensity(_clrIdx, _worldPos.x + 1, terrainHeight + 1, _worldPos.z) / -128f;
		float num6 = (float)GameManager.Instance.World.GetDensity(_clrIdx, _worldPos.x, terrainHeight + 1, _worldPos.z + 1) / -128f;
		if (num > 0.999f && num4 > 0.999f)
		{
			num = 0.5f;
		}
		if (num2 > 0.999f && num5 > 0.999f)
		{
			num2 = 0.5f;
		}
		if (num3 > 0.999f && num6 > 0.999f)
		{
			num3 = 0.5f;
		}
		float y = (float)terrainHeight + num;
		float y2 = (float)terrainHeight2 + num2;
		float y3 = terrainHeight3 + num3;
		Vector3 lhs = new Vector3(0f, y3, 1f) - new Vector3(0f, y, 0f);
		Vector3 rhs = new Vector3(1f, y2, 0f) - new Vector3(0f, y, 0f);
		return Vector3.Cross(lhs, rhs).normalized;
	}

	// Token: 0x06007FE2 RID: 32738 RVA: 0x0033FE39 File Offset: 0x0033E039
	public static GameUtils.DirEightWay GetDirByNormal(Vector2 _normal)
	{
		_normal.Normalize();
		return GameUtils.GetDirByNormal(new Vector2i(Mathf.RoundToInt(_normal.x), Mathf.RoundToInt(_normal.y)));
	}

	// Token: 0x06007FE3 RID: 32739 RVA: 0x0033FE64 File Offset: 0x0033E064
	public static GameUtils.DirEightWay GetDirByNormal(Vector2i _normal)
	{
		for (int i = 0; i < GameUtils.NeighborsEightWay.Count; i++)
		{
			if (GameUtils.NeighborsEightWay[i] == _normal)
			{
				return (GameUtils.DirEightWay)i;
			}
		}
		return GameUtils.DirEightWay.None;
	}

	// Token: 0x06007FE4 RID: 32740 RVA: 0x0033FEA0 File Offset: 0x0033E0A0
	public static GameUtils.DirEightWay GetClosestDirection(float _rotation, bool _limitTo90Degress = false)
	{
		_rotation = MathUtils.Mod(_rotation, 360f);
		if (_limitTo90Degress)
		{
			if (_rotation > 315f || _rotation <= 45f)
			{
				return GameUtils.DirEightWay.N;
			}
			if (_rotation <= 135f)
			{
				return GameUtils.DirEightWay.E;
			}
			if (_rotation <= 225f)
			{
				return GameUtils.DirEightWay.S;
			}
			return GameUtils.DirEightWay.W;
		}
		else
		{
			if ((double)_rotation > 337.5 || (double)_rotation <= 22.5)
			{
				return GameUtils.DirEightWay.N;
			}
			if ((double)_rotation <= 67.5)
			{
				return GameUtils.DirEightWay.NE;
			}
			if ((double)_rotation <= 112.5)
			{
				return GameUtils.DirEightWay.E;
			}
			if ((double)_rotation <= 157.5)
			{
				return GameUtils.DirEightWay.SE;
			}
			if ((double)_rotation <= 202.5)
			{
				return GameUtils.DirEightWay.S;
			}
			if ((double)_rotation <= 247.5)
			{
				return GameUtils.DirEightWay.SW;
			}
			if ((double)_rotation <= 292.5)
			{
				return GameUtils.DirEightWay.W;
			}
			return GameUtils.DirEightWay.NW;
		}
	}

	// Token: 0x06007FE5 RID: 32741 RVA: 0x0033FF5C File Offset: 0x0033E15C
	public static void DestroyAllChildrenBut(Transform t, string _excluded)
	{
		bool isPlaying = Application.isPlaying;
		int num = 0;
		List<string> list = new List<string>(_excluded.Split(',', StringSplitOptions.None));
		while (t.childCount != num)
		{
			Transform child = t.GetChild(num);
			if (list.Contains(child.name))
			{
				num++;
			}
			else if (isPlaying)
			{
				child.parent = null;
				UnityEngine.Object.Destroy(child.gameObject);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(child.gameObject);
			}
		}
	}

	// Token: 0x06007FE6 RID: 32742 RVA: 0x0033FFCC File Offset: 0x0033E1CC
	public static void DestroyAllChildrenBut(Transform t, List<string> _excluded)
	{
		bool isPlaying = Application.isPlaying;
		for (int i = t.childCount - 1; i >= 0; i--)
		{
			Transform child = t.GetChild(i);
			if (!_excluded.Contains(child.name))
			{
				if (isPlaying)
				{
					child.SetParent(null, false);
					UnityEngine.Object.Destroy(child.gameObject);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(child.gameObject);
				}
			}
		}
	}

	// Token: 0x06007FE7 RID: 32743 RVA: 0x0034002C File Offset: 0x0033E22C
	public static void SetMeshVertexAttributes(Mesh mesh, bool compressPosition = true)
	{
		VertexAttributeDescriptor[] vertexAttributes = mesh.GetVertexAttributes();
		GameUtils.ApplyCompressedVertexAttributes(vertexAttributes, compressPosition);
		mesh.SetVertexBufferParams(mesh.vertexCount, vertexAttributes);
	}

	// Token: 0x06007FE8 RID: 32744 RVA: 0x0034005C File Offset: 0x0033E25C
	public unsafe static void ApplyCompressedVertexAttributes(Span<VertexAttributeDescriptor> attributes, bool compressPosition = true)
	{
		for (int i = 0; i < attributes.Length; i++)
		{
			VertexAttributeDescriptor vertexAttributeDescriptor = *attributes[i];
			VertexAttribute attribute = vertexAttributeDescriptor.attribute;
			if (attribute == VertexAttribute.Position && !compressPosition)
			{
				vertexAttributeDescriptor.format = VertexAttributeFormat.Float32;
				vertexAttributeDescriptor.dimension = 3;
			}
			else if (attribute == VertexAttribute.Position || attribute == VertexAttribute.Normal)
			{
				vertexAttributeDescriptor.format = VertexAttributeFormat.Float16;
				vertexAttributeDescriptor.dimension = 4;
			}
			else if (attribute == VertexAttribute.Color || attribute == VertexAttribute.Tangent)
			{
				vertexAttributeDescriptor.format = VertexAttributeFormat.Float16;
				vertexAttributeDescriptor.dimension = 4;
			}
			else if (attribute == VertexAttribute.TexCoord0 || attribute == VertexAttribute.TexCoord1 || attribute == VertexAttribute.TexCoord2 || attribute == VertexAttribute.TexCoord3)
			{
				vertexAttributeDescriptor.format = VertexAttributeFormat.Float16;
				vertexAttributeDescriptor.dimension = 2;
			}
			*attributes[i] = vertexAttributeDescriptor;
		}
	}

	// Token: 0x040062A5 RID: 25253
	[PublicizedFrom(EAccessModifier.Private)]
	public static Collider[] overlapBoxHits = new Collider[50];

	// Token: 0x040062A6 RID: 25254
	[PublicizedFrom(EAccessModifier.Private)]
	public static GameRandom random;

	// Token: 0x040062A7 RID: 25255
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<Vector3> tempVertices = new List<Vector3>(16384);

	// Token: 0x040062A8 RID: 25256
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<int> tempTriangles = new List<int>(16384);

	// Token: 0x040062A9 RID: 25257
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<string, string> arguments;

	// Token: 0x040062AA RID: 25258
	public static string lastSavedScreenshotFilename;

	// Token: 0x040062AB RID: 25259
	public static List<Vector2i> NeighborsEightWay = new List<Vector2i>
	{
		new Vector2i(0, 1),
		new Vector2i(1, 1),
		new Vector2i(1, 0),
		new Vector2i(1, -1),
		new Vector2i(0, -1),
		new Vector2i(-1, -1),
		new Vector2i(-1, 0),
		new Vector2i(-1, 1)
	};

	// Token: 0x02000FA7 RID: 4007
	public class WorldInfo
	{
		// Token: 0x17000D4D RID: 3405
		// (get) Token: 0x06007FEB RID: 32747 RVA: 0x003401B9 File Offset: 0x0033E3B9
		public Vector2i WorldSize
		{
			get
			{
				return this.HeightmapSize * this.Scale;
			}
		}

		// Token: 0x06007FEC RID: 32748 RVA: 0x003401CC File Offset: 0x0033E3CC
		public WorldInfo(string _name, string _description, string[] _modes, Vector2i _heightmapSize, int _scale, bool _fixedWaterLevel, bool _randomGeneratedWorld, VersionInformation _gameVersionCreated, DynamicProperties _dynamicProperties = null)
		{
			this.Valid = true;
			this.Name = _name;
			this.Description = _description;
			this.Modes = _modes;
			this.HeightmapSize = _heightmapSize;
			this.Scale = _scale;
			this.FixedWaterLevel = _fixedWaterLevel;
			this.RandomGeneratedWorld = _randomGeneratedWorld;
			this.GameVersionCreated = _gameVersionCreated;
			this.DynamicProperties = _dynamicProperties;
		}

		// Token: 0x06007FED RID: 32749 RVA: 0x0034022C File Offset: 0x0033E42C
		public void Save(PathAbstractions.AbstractedLocation _worldLocation)
		{
			if (_worldLocation.Type == PathAbstractions.EAbstractedLocationType.None)
			{
				Log.Warning("No world location given");
				return;
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.CreateXmlDeclaration();
			XmlElement node = xmlDocument.AddXmlElement("MapInfo");
			node.AddXmlElement("property").SetAttrib("name", "Name").SetAttrib("value", this.Name);
			node.AddXmlElement("property").SetAttrib("name", "Modes").SetAttrib("value", string.Join(",", this.Modes));
			node.AddXmlElement("property").SetAttrib("name", "Description").SetAttrib("value", this.Description);
			node.AddXmlElement("property").SetAttrib("name", "Scale").SetAttrib("value", this.Scale.ToString());
			node.AddXmlElement("property").SetAttrib("name", "HeightMapSize").SetAttrib("value", this.HeightmapSize.ToString());
			node.AddXmlElement("property").SetAttrib("name", "FixedWaterLevel").SetAttrib("value", this.FixedWaterLevel.ToString());
			node.AddXmlElement("property").SetAttrib("name", "RandomGeneratedWorld").SetAttrib("value", this.RandomGeneratedWorld.ToString());
			node.AddXmlElement("property").SetAttrib("name", "GameVersion").SetAttrib("value", this.GameVersionCreated.SerializableString);
			xmlDocument.SdSave(_worldLocation.FullPath + "/map_info.xml");
		}

		// Token: 0x06007FEE RID: 32750 RVA: 0x00340408 File Offset: 0x0033E608
		public static GameUtils.WorldInfo LoadWorldInfo(PathAbstractions.AbstractedLocation _worldLocation)
		{
			try
			{
				if (_worldLocation.Type == PathAbstractions.EAbstractedLocationType.None)
				{
					return null;
				}
				string text = _worldLocation.FullPath + "/map_info.xml";
				if (!SdFile.Exists(text))
				{
					return null;
				}
				IEnumerable<XElement> enumerable = from s in SdXDocument.Load(text).Elements("MapInfo")
				from p in s.Elements("property")
				select p;
				DynamicProperties dynamicProperties = new DynamicProperties();
				foreach (XElement propertyNode in enumerable)
				{
					dynamicProperties.Add(propertyNode, true, false);
				}
				string name = null;
				string description = null;
				string[] modes = null;
				int x = 4096;
				int y = 4096;
				int scale = 1;
				bool fixedWaterLevel = false;
				bool randomGeneratedWorld = false;
				VersionInformation gameVersionCreated = new VersionInformation(VersionInformation.EGameReleaseType.Alpha, -1, -1, -1);
				if (dynamicProperties.Values.ContainsKey("Name"))
				{
					name = dynamicProperties.Values["Name"];
				}
				if (dynamicProperties.Values.ContainsKey("Modes"))
				{
					modes = dynamicProperties.Values["Modes"].Replace(" ", "").Split(',', StringSplitOptions.None);
				}
				if (dynamicProperties.Values.ContainsKey("Description"))
				{
					description = Localization.Get(dynamicProperties.Values["Description"], false);
				}
				if (dynamicProperties.Values.ContainsKey("Scale"))
				{
					scale = int.Parse(dynamicProperties.Values["Scale"]);
				}
				if (dynamicProperties.Values.ContainsKey("HeightMapSize"))
				{
					Vector2i vector2i = StringParsers.ParseVector2i(dynamicProperties.Values["HeightMapSize"], ',');
					x = vector2i.x;
					y = vector2i.y;
				}
				if (dynamicProperties.Values.ContainsKey("FixedWaterLevel"))
				{
					fixedWaterLevel = StringParsers.ParseBool(dynamicProperties.Values["FixedWaterLevel"], 0, -1, true);
				}
				if (dynamicProperties.Values.ContainsKey("RandomGeneratedWorld"))
				{
					randomGeneratedWorld = StringParsers.ParseBool(dynamicProperties.Values["RandomGeneratedWorld"], 0, -1, true);
				}
				if (dynamicProperties.Values.ContainsKey("GameVersion") && !VersionInformation.TryParseSerializedString(dynamicProperties.Values["GameVersion"], out gameVersionCreated))
				{
					gameVersionCreated = new VersionInformation(VersionInformation.EGameReleaseType.Alpha, -1, -1, -1);
					Log.Warning("World '" + _worldLocation.Name + "' has an invalid GameVersion value: " + dynamicProperties.Values["GameVersion"]);
				}
				return new GameUtils.WorldInfo(name, description, modes, new Vector2i(x, y), scale, fixedWaterLevel, randomGeneratedWorld, gameVersionCreated, dynamicProperties);
			}
			catch (Exception e)
			{
				Log.Error("Error reading WorldInfo for " + (_worldLocation.FullPath ?? "<null>"));
				Log.Exception(e);
			}
			return null;
		}

		// Token: 0x040062AC RID: 25260
		public readonly bool Valid;

		// Token: 0x040062AD RID: 25261
		public readonly string Name;

		// Token: 0x040062AE RID: 25262
		public readonly string Description;

		// Token: 0x040062AF RID: 25263
		public readonly string[] Modes;

		// Token: 0x040062B0 RID: 25264
		public readonly Vector2i HeightmapSize;

		// Token: 0x040062B1 RID: 25265
		public readonly int Scale;

		// Token: 0x040062B2 RID: 25266
		public readonly bool FixedWaterLevel;

		// Token: 0x040062B3 RID: 25267
		public readonly bool RandomGeneratedWorld;

		// Token: 0x040062B4 RID: 25268
		public readonly VersionInformation GameVersionCreated;

		// Token: 0x040062B5 RID: 25269
		public readonly DynamicProperties DynamicProperties;
	}

	// Token: 0x02000FA9 RID: 4009
	// (Invoke) Token: 0x06007FF4 RID: 32756
	public delegate void OutputDelegate(string _text);

	// Token: 0x02000FAA RID: 4010
	public enum EKickReason
	{
		// Token: 0x040062BA RID: 25274
		EmptyNameOrPlayerID,
		// Token: 0x040062BB RID: 25275
		InvalidUserId,
		// Token: 0x040062BC RID: 25276
		DuplicatePlayerID,
		// Token: 0x040062BD RID: 25277
		InvalidAuthTicket,
		// Token: 0x040062BE RID: 25278
		VersionMismatch,
		// Token: 0x040062BF RID: 25279
		PlayerLimitExceeded,
		// Token: 0x040062C0 RID: 25280
		Banned,
		// Token: 0x040062C1 RID: 25281
		NotOnWhitelist,
		// Token: 0x040062C2 RID: 25282
		PlatformAuthenticationBeginFailed,
		// Token: 0x040062C3 RID: 25283
		PlatformAuthenticationFailed,
		// Token: 0x040062C4 RID: 25284
		ManualKick,
		// Token: 0x040062C5 RID: 25285
		EacViolation,
		// Token: 0x040062C6 RID: 25286
		EacBan,
		// Token: 0x040062C7 RID: 25287
		PlayerLimitExceededNonVIP,
		// Token: 0x040062C8 RID: 25288
		GameStillLoading,
		// Token: 0x040062C9 RID: 25289
		GamePaused,
		// Token: 0x040062CA RID: 25290
		ModDecision,
		// Token: 0x040062CB RID: 25291
		FriendsOnly,
		// Token: 0x040062CC RID: 25292
		UnknownNetPackage,
		// Token: 0x040062CD RID: 25293
		EncryptionFailure,
		// Token: 0x040062CE RID: 25294
		UnsupportedPlatform,
		// Token: 0x040062CF RID: 25295
		CrossPlatformAuthenticationBeginFailed,
		// Token: 0x040062D0 RID: 25296
		CrossPlatformAuthenticationFailed,
		// Token: 0x040062D1 RID: 25297
		WrongCrossPlatform,
		// Token: 0x040062D2 RID: 25298
		EosEacViolation,
		// Token: 0x040062D3 RID: 25299
		MultiplayerBlockedForHostAccount,
		// Token: 0x040062D4 RID: 25300
		BadMTUPackets,
		// Token: 0x040062D5 RID: 25301
		CrossplayDisabled,
		// Token: 0x040062D6 RID: 25302
		InternalNetConnectionError,
		// Token: 0x040062D7 RID: 25303
		InviteOnly,
		// Token: 0x040062D8 RID: 25304
		SessionClosed,
		// Token: 0x040062D9 RID: 25305
		PersistentPlayerDataExceeded,
		// Token: 0x040062DA RID: 25306
		PlatformPlayerLimitExceeded,
		// Token: 0x040062DB RID: 25307
		EncryptionAgreementInvalidSignature,
		// Token: 0x040062DC RID: 25308
		EncryptionAgreementError
	}

	// Token: 0x02000FAB RID: 4011
	public struct KickPlayerData
	{
		// Token: 0x06007FF7 RID: 32759 RVA: 0x0034072C File Offset: 0x0033E92C
		public KickPlayerData(GameUtils.EKickReason _kickReason, int _apiResponseEnum = 0, DateTime _banUntil = default(DateTime), string _customReason = "")
		{
			this.reason = _kickReason;
			this.apiResponseEnum = _apiResponseEnum;
			this.banUntil = _banUntil;
			this.customReason = (_customReason ?? string.Empty);
		}

		// Token: 0x06007FF8 RID: 32760 RVA: 0x00340754 File Offset: 0x0033E954
		public string LocalizedMessage()
		{
			switch (this.reason)
			{
			case GameUtils.EKickReason.EmptyNameOrPlayerID:
			case GameUtils.EKickReason.InvalidUserId:
			case GameUtils.EKickReason.DuplicatePlayerID:
			case GameUtils.EKickReason.InvalidAuthTicket:
			case GameUtils.EKickReason.NotOnWhitelist:
			case GameUtils.EKickReason.PersistentPlayerDataExceeded:
				return Localization.Get("auth_" + this.reason.ToStringCached<GameUtils.EKickReason>(), false);
			case GameUtils.EKickReason.VersionMismatch:
				return string.Format(Localization.Get("auth_VersionMismatch", false), Constants.cVersionInformation.LongStringNoBuild, this.customReason);
			case GameUtils.EKickReason.PlayerLimitExceeded:
				return string.Format(Localization.Get("auth_PlayerLimitExceeded", false), this.customReason);
			case GameUtils.EKickReason.Banned:
				return string.Format(Localization.Get("auth_Banned", false), this.banUntil.ToCultureInvariantString()) + (string.IsNullOrEmpty(this.customReason) ? string.Empty : ("\n" + string.Format(Localization.Get("auth_reason", false), this.customReason)));
			case GameUtils.EKickReason.PlatformAuthenticationBeginFailed:
			{
				EBeginUserAuthenticationResult ebeginUserAuthenticationResult = (EBeginUserAuthenticationResult)this.apiResponseEnum;
				if (ebeginUserAuthenticationResult - EBeginUserAuthenticationResult.InvalidTicket <= 4)
				{
					return string.Format(Localization.Get("platformauth_" + ebeginUserAuthenticationResult.ToStringCached<EBeginUserAuthenticationResult>(), false), PlatformManager.NativePlatform.PlatformDisplayName);
				}
				return string.Format(Localization.Get("platformauth_unknown", false), PlatformManager.NativePlatform.PlatformDisplayName);
			}
			case GameUtils.EKickReason.PlatformAuthenticationFailed:
			{
				EUserAuthenticationResult euserAuthenticationResult = (EUserAuthenticationResult)this.apiResponseEnum;
				if (euserAuthenticationResult - EUserAuthenticationResult.UserNotConnectedToPlatform <= 7)
				{
					return string.Format(Localization.Get("platformauth_" + euserAuthenticationResult.ToStringCached<EUserAuthenticationResult>(), false), PlatformManager.NativePlatform.PlatformDisplayName);
				}
				if (euserAuthenticationResult != EUserAuthenticationResult.PublisherIssuedBan)
				{
					return string.Format(Localization.Get("platformauth_unknown", false), PlatformManager.NativePlatform.PlatformDisplayName);
				}
				if (this.banUntil == default(DateTime))
				{
					return string.Format(Localization.Get("platformauth_" + euserAuthenticationResult.ToStringCached<EUserAuthenticationResult>(), false), PlatformManager.NativePlatform.PlatformDisplayName) + (string.IsNullOrEmpty(this.customReason) ? string.Empty : ("\n" + string.Format(Localization.Get("auth_reason", false), Array.Empty<object>())));
				}
				return string.Format("\n" + Localization.Get("auth_Banned", false), this.banUntil.ToCultureInvariantString()) + (string.IsNullOrEmpty(this.customReason) ? string.Empty : ("\n" + string.Format(Localization.Get("auth_reason", false), this.customReason)));
			}
			case GameUtils.EKickReason.ManualKick:
				return string.Format(Localization.Get("auth_ManualKick", false), Array.Empty<object>()) + (string.IsNullOrEmpty(this.customReason) ? string.Empty : ("\n" + string.Format(Localization.Get("auth_reason", false), this.customReason)));
			case GameUtils.EKickReason.PlayerLimitExceededNonVIP:
				return string.Format(Localization.Get("auth_PlayerLimitExceededNonVIP", false), this.customReason);
			case GameUtils.EKickReason.GameStillLoading:
				return Localization.Get("auth_stillloading", false);
			case GameUtils.EKickReason.GamePaused:
				return Localization.Get("auth_gamepaused", false);
			case GameUtils.EKickReason.ModDecision:
			{
				string text = Localization.Get("auth_mod", false);
				if (!string.IsNullOrEmpty(this.customReason))
				{
					text = text + "\n" + this.customReason;
				}
				return text;
			}
			case GameUtils.EKickReason.FriendsOnly:
				return Localization.Get("auth_friendsonly", false);
			case GameUtils.EKickReason.UnknownNetPackage:
				return Localization.Get("auth_unknownnetpackage", false);
			case GameUtils.EKickReason.EncryptionFailure:
				return Localization.Get("auth_encryptionfailure", false);
			case GameUtils.EKickReason.UnsupportedPlatform:
				return string.Format(Localization.Get("auth_unsupportedplatform", false), Localization.Get("platformName" + this.customReason, false));
			case GameUtils.EKickReason.CrossPlatformAuthenticationBeginFailed:
			{
				EBeginUserAuthenticationResult ebeginUserAuthenticationResult2 = (EBeginUserAuthenticationResult)this.apiResponseEnum;
				if (ebeginUserAuthenticationResult2 - EBeginUserAuthenticationResult.InvalidTicket <= 4)
				{
					return string.Format(Localization.Get("platformauth_" + ebeginUserAuthenticationResult2.ToStringCached<EBeginUserAuthenticationResult>(), false), PlatformManager.CrossplatformPlatform.PlatformDisplayName);
				}
				return string.Format(Localization.Get("platformauth_unknown", false), PlatformManager.CrossplatformPlatform.PlatformDisplayName);
			}
			case GameUtils.EKickReason.CrossPlatformAuthenticationFailed:
			{
				EUserAuthenticationResult euserAuthenticationResult2 = (EUserAuthenticationResult)this.apiResponseEnum;
				if (euserAuthenticationResult2 - EUserAuthenticationResult.UserNotConnectedToPlatform <= 8)
				{
					return string.Format(Localization.Get("platformauth_" + euserAuthenticationResult2.ToStringCached<EUserAuthenticationResult>(), false), PlatformManager.CrossplatformPlatform.PlatformDisplayName);
				}
				if (euserAuthenticationResult2 != EUserAuthenticationResult.EosTicketFailed)
				{
					return string.Format(Localization.Get("platformauth_unknown", false), PlatformManager.CrossplatformPlatform.PlatformDisplayName);
				}
				return string.Format(Localization.Get("platformauth_" + euserAuthenticationResult2.ToStringCached<EUserAuthenticationResult>(), false), PlatformManager.CrossplatformPlatform.PlatformDisplayName, this.customReason);
			}
			case GameUtils.EKickReason.WrongCrossPlatform:
				return string.Format(Localization.Get("auth_wrongcrossplatform", false), Localization.Get("platformName" + this.customReason, false));
			case GameUtils.EKickReason.EosEacViolation:
			{
				AntiCheatCommonClientActionReason antiCheatCommonClientActionReason = (AntiCheatCommonClientActionReason)this.apiResponseEnum;
				if (antiCheatCommonClientActionReason > AntiCheatCommonClientActionReason.PermanentBanned)
				{
					return Localization.Get("eacauth_unknown", false);
				}
				string arg = Localization.Get("eacauth_known_" + ((AntiCheatCommonClientActionReason)this.apiResponseEnum).ToStringCached<AntiCheatCommonClientActionReason>(), false);
				if (string.IsNullOrEmpty(this.customReason))
				{
					return string.Format(Localization.Get("eacauth_known", false), arg);
				}
				return string.Format(Localization.Get("eacauth_known_with_text", false), arg, this.customReason);
			}
			case GameUtils.EKickReason.MultiplayerBlockedForHostAccount:
				return Localization.Get("auth_multiplayerblocked", false);
			case GameUtils.EKickReason.BadMTUPackets:
				return Localization.Get("auth_badPackets", false);
			case GameUtils.EKickReason.CrossplayDisabled:
				return Localization.Get("auth_crossplaydisabled", false);
			case GameUtils.EKickReason.InternalNetConnectionError:
				return Localization.Get("auth_internalnetconnectionerror", false);
			case GameUtils.EKickReason.InviteOnly:
				return Localization.Get("auth_inviteOnly", false);
			case GameUtils.EKickReason.SessionClosed:
				return Localization.Get("auth_sessionClosed", false);
			case GameUtils.EKickReason.PlatformPlayerLimitExceeded:
				return string.Format(Localization.Get("auth_PlatformPlayerLimitExceeded", false), this.customReason);
			case GameUtils.EKickReason.EncryptionAgreementInvalidSignature:
				return Localization.Get("auth_encryptionagreementinvalidsignature", false);
			case GameUtils.EKickReason.EncryptionAgreementError:
				return Localization.Get("auth_encryptionagreementerror", false);
			}
			return Localization.Get("auth_unknown", false);
		}

		// Token: 0x06007FF9 RID: 32761 RVA: 0x00340D08 File Offset: 0x0033EF08
		public override string ToString()
		{
			switch (this.reason)
			{
			case GameUtils.EKickReason.EmptyNameOrPlayerID:
				return "Empty name or player ID";
			case GameUtils.EKickReason.InvalidUserId:
				return "Invalid SteamID";
			case GameUtils.EKickReason.DuplicatePlayerID:
				return "Duplicate player ID";
			case GameUtils.EKickReason.InvalidAuthTicket:
				return "Invalid authentication ticket";
			case GameUtils.EKickReason.VersionMismatch:
				return "Version mismatch";
			case GameUtils.EKickReason.PlayerLimitExceeded:
				return "Player limit exceeded";
			case GameUtils.EKickReason.Banned:
				return "Banned until: " + this.banUntil.ToCultureInvariantString() + (string.IsNullOrEmpty(this.customReason) ? "" : (", reason: " + this.customReason));
			case GameUtils.EKickReason.NotOnWhitelist:
				return "Not on whitelist";
			case GameUtils.EKickReason.PlatformAuthenticationBeginFailed:
				return "Platform auth failed: " + ((EBeginUserAuthenticationResult)this.apiResponseEnum).ToStringCached<EBeginUserAuthenticationResult>();
			case GameUtils.EKickReason.PlatformAuthenticationFailed:
				return "Platform auth failed: " + ((EUserAuthenticationResult)this.apiResponseEnum).ToStringCached<EUserAuthenticationResult>();
			case GameUtils.EKickReason.ManualKick:
				return "Kick: " + ((this.customReason != null) ? this.customReason : "no reason given");
			case GameUtils.EKickReason.PlayerLimitExceededNonVIP:
				return "Player limit for non VIPs / unreserved slots exceeded";
			case GameUtils.EKickReason.GameStillLoading:
				return "Server is still initializing";
			case GameUtils.EKickReason.GamePaused:
				return "Server is paused";
			case GameUtils.EKickReason.ModDecision:
				return "Denied by mod";
			case GameUtils.EKickReason.FriendsOnly:
				return "Friends Only host";
			case GameUtils.EKickReason.UnknownNetPackage:
				return "Unknown NetPackage";
			case GameUtils.EKickReason.EncryptionFailure:
				return "Encryption failure";
			case GameUtils.EKickReason.UnsupportedPlatform:
				return "Unsupported client platform: " + this.customReason;
			case GameUtils.EKickReason.CrossPlatformAuthenticationBeginFailed:
				return "Cross platform auth failed: " + ((EBeginUserAuthenticationResult)this.apiResponseEnum).ToStringCached<EBeginUserAuthenticationResult>() + (string.IsNullOrEmpty(this.customReason) ? "" : (" - " + this.customReason));
			case GameUtils.EKickReason.CrossPlatformAuthenticationFailed:
				return "Cross platform auth failed: " + ((EUserAuthenticationResult)this.apiResponseEnum).ToStringCached<EUserAuthenticationResult>() + (string.IsNullOrEmpty(this.customReason) ? "" : (" - " + this.customReason));
			case GameUtils.EKickReason.WrongCrossPlatform:
				return "Unsupported client cross platform: " + this.customReason;
			case GameUtils.EKickReason.EosEacViolation:
				return "EOS-ACS violation: " + ((AntiCheatCommonClientActionReason)this.apiResponseEnum).ToStringCached<AntiCheatCommonClientActionReason>() + (string.IsNullOrEmpty(this.customReason) ? "" : (" - " + this.customReason));
			case GameUtils.EKickReason.MultiplayerBlockedForHostAccount:
				return "Multiplayer blocked for host's account";
			case GameUtils.EKickReason.CrossplayDisabled:
				return "Crossplay disabled for host's account";
			case GameUtils.EKickReason.InviteOnly:
				return "Invite Only host";
			case GameUtils.EKickReason.SessionClosed:
				return "Session is Closed";
			case GameUtils.EKickReason.EncryptionAgreementInvalidSignature:
				return "Encryption key agreement authentication invalid";
			case GameUtils.EKickReason.EncryptionAgreementError:
				return "Error while performing encryption key agreement";
			}
			return "Unknown reason";
		}

		// Token: 0x040062DD RID: 25309
		public GameUtils.EKickReason reason;

		// Token: 0x040062DE RID: 25310
		public int apiResponseEnum;

		// Token: 0x040062DF RID: 25311
		public DateTime banUntil;

		// Token: 0x040062E0 RID: 25312
		public string customReason;
	}

	// Token: 0x02000FAC RID: 4012
	public enum EScreenshotMode
	{
		// Token: 0x040062E2 RID: 25314
		File,
		// Token: 0x040062E3 RID: 25315
		Clipboard,
		// Token: 0x040062E4 RID: 25316
		Both
	}

	// Token: 0x02000FAD RID: 4013
	public enum EPlayerHomeType
	{
		// Token: 0x040062E6 RID: 25318
		None,
		// Token: 0x040062E7 RID: 25319
		Landclaim,
		// Token: 0x040062E8 RID: 25320
		Bedroll
	}

	// Token: 0x02000FAE RID: 4014
	public enum DirEightWay : sbyte
	{
		// Token: 0x040062EA RID: 25322
		None = -1,
		// Token: 0x040062EB RID: 25323
		N,
		// Token: 0x040062EC RID: 25324
		NE,
		// Token: 0x040062ED RID: 25325
		E,
		// Token: 0x040062EE RID: 25326
		SE,
		// Token: 0x040062EF RID: 25327
		S,
		// Token: 0x040062F0 RID: 25328
		SW,
		// Token: 0x040062F1 RID: 25329
		W,
		// Token: 0x040062F2 RID: 25330
		NW,
		// Token: 0x040062F3 RID: 25331
		COUNT
	}
}
