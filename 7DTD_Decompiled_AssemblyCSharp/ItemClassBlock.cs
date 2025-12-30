using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000565 RID: 1381
[Preserve]
public class ItemClassBlock : ItemClass
{
	// Token: 0x06002CAE RID: 11438 RVA: 0x0012A774 File Offset: 0x00128974
	public ItemClassBlock()
	{
		this.HoldType = new DataItem<int>(7);
		AnimationDelayData.AnimationDelay[this.HoldType.Value] = new AnimationDelayData.AnimationDelays(0f, 0f, 0.31f, 0.31f, true);
		this.Stacknumber = new DataItem<int>(500);
	}

	// Token: 0x06002CAF RID: 11439 RVA: 0x0012A7D4 File Offset: 0x001289D4
	public override void Init()
	{
		base.Init();
		Block block = Block.list[base.Id];
		this.DescriptionKey = block.DescriptionKey;
		this.MadeOfMaterial = block.blockMaterial;
		if (block.CustomIcon != null)
		{
			this.CustomIcon = new DataItem<string>(block.CustomIcon);
		}
		this.NoScrapping = block.NoScrapping;
		this.CustomIconTint = block.CustomIconTint;
		this.SortOrder = block.SortOrder;
		this.CreativeMode = block.CreativeMode;
		this.TraderStageTemplate = block.TraderStageTemplate;
		this.SoundPickup = block.SoundPickup;
		this.SoundPlace = block.SoundPlace;
	}

	// Token: 0x06002CB0 RID: 11440 RVA: 0x0012A879 File Offset: 0x00128A79
	public override bool IsActionRunning(ItemInventoryData _data)
	{
		return Time.time - (_data as ItemClassBlock.ItemBlockInventoryData).lastBuildTime < Constants.cBuildIntervall;
	}

	// Token: 0x06002CB1 RID: 11441 RVA: 0x0012A893 File Offset: 0x00128A93
	[PublicizedFrom(EAccessModifier.Protected)]
	public override ItemInventoryData createItemInventoryData(ItemStack _itemStack, IGameManager _gameManager, EntityAlive _holdingEntity, int _slotIdx)
	{
		return new ItemClassBlock.ItemBlockInventoryData(this, _itemStack, _gameManager, _holdingEntity, _slotIdx);
	}

	// Token: 0x06002CB2 RID: 11442 RVA: 0x00002914 File Offset: 0x00000B14
	public override void StopHolding(ItemInventoryData _data, Transform _modelTransform)
	{
	}

	// Token: 0x06002CB3 RID: 11443 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsBlock()
	{
		return true;
	}

	// Token: 0x06002CB4 RID: 11444 RVA: 0x0012A8A0 File Offset: 0x00128AA0
	public override Block GetBlock()
	{
		return Block.list[base.Id];
	}

	// Token: 0x06002CB5 RID: 11445 RVA: 0x0012A8AE File Offset: 0x00128AAE
	public override string GetItemName()
	{
		return this.GetBlock().GetBlockName();
	}

	// Token: 0x06002CB6 RID: 11446 RVA: 0x0012A8BB File Offset: 0x00128ABB
	public override string GetLocalizedItemName()
	{
		return this.GetBlock().GetLocalizedBlockName();
	}

	// Token: 0x06002CB7 RID: 11447 RVA: 0x0012A8C8 File Offset: 0x00128AC8
	public override bool HasAnyTags(FastTags<TagGroup.Global> _tags)
	{
		return this.GetBlock().Tags.Test_AnySet(_tags);
	}

	// Token: 0x06002CB8 RID: 11448 RVA: 0x0012A8DB File Offset: 0x00128ADB
	public override bool HasAllTags(FastTags<TagGroup.Global> _tags)
	{
		return this.GetBlock().Tags.Test_AllSet(_tags);
	}

	// Token: 0x06002CB9 RID: 11449 RVA: 0x0012A8F0 File Offset: 0x00128AF0
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockValue GetBlockValueFromItemValue(ItemValue _itemValue)
	{
		Block block = this.GetBlock();
		if (block.SelectAlternates)
		{
			return block.GetAltBlockValue(_itemValue.Meta);
		}
		return _itemValue.ToBlockValue();
	}

	// Token: 0x06002CBA RID: 11450 RVA: 0x0012A91F File Offset: 0x00128B1F
	public override Transform CloneModel(GameObject _go, World _world, BlockValue _blockValue, Vector3[] _vertices, Vector3 _position, Transform _parent, BlockShape.MeshPurpose _purpose, TextureFullArray _textureFullArray = default(TextureFullArray))
	{
		return ItemClassBlock.CreateMesh(_go, _world, _blockValue, _vertices, _position, _parent, _purpose, _textureFullArray);
	}

	// Token: 0x06002CBB RID: 11451 RVA: 0x0012A933 File Offset: 0x00128B33
	public override Transform CloneModel(World _world, ItemValue _itemValue, Vector3 _position, Transform _parent, BlockShape.MeshPurpose _purpose, TextureFullArray _textureFullArray = default(TextureFullArray))
	{
		return ItemClassBlock.CreateMesh(null, _world, this.GetBlockValueFromItemValue(_itemValue), null, _position, _parent, _purpose, _textureFullArray);
	}

	// Token: 0x06002CBC RID: 11452 RVA: 0x0012A94C File Offset: 0x00128B4C
	public static Transform CreateMesh(GameObject _go, World _world, BlockValue _blockValue, Vector3[] _vertices, Vector3 _worldPos, Transform _parent, BlockShape.MeshPurpose _purpose, TextureFullArray _textureFullArray = default(TextureFullArray))
	{
		if (_purpose == BlockShape.MeshPurpose.Drop)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(LoadManager.LoadAsset<GameObject>("@:Other/Items/Misc/sack_droppedPrefab.prefab", null, null, false, true).Asset);
			Transform transform = gameObject.transform;
			transform.SetParent(_parent, false);
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			return gameObject.transform;
		}
		Block block = _blockValue.Block;
		BlockShapeModelEntity blockShapeModelEntity = block.shape as BlockShapeModelEntity;
		if (blockShapeModelEntity != null)
		{
			if (_go == null)
			{
				_go = new GameObject();
				_go.transform.SetParent(_parent, false);
			}
			blockShapeModelEntity.CloneModel(_blockValue, _go.transform);
			return _go.transform;
		}
		int meshIndex = (int)block.MeshIndex;
		Transform transform2 = ItemClassBlock.CreateMeshOfType(_go, _world, _blockValue, _vertices, _worldPos, _parent, _purpose, _textureFullArray, meshIndex);
		if (meshIndex == 0 && (_purpose == BlockShape.MeshPurpose.Preview || _purpose == BlockShape.MeshPurpose.Local))
		{
			Transform transform3 = ItemClassBlock.CreateMeshOfType(_go, _world, _blockValue, _vertices, _worldPos, _parent, _purpose, _textureFullArray, 2);
			if (transform3)
			{
				if (transform2)
				{
					transform3.SetParent(transform2, false);
				}
				else
				{
					transform2 = transform3;
				}
			}
		}
		return transform2;
	}

	// Token: 0x06002CBD RID: 11453 RVA: 0x0012AA48 File Offset: 0x00128C48
	public static Transform CreateMeshOfType(GameObject _go, World _world, BlockValue _blockValue, Vector3[] _vertices, Vector3 _worldPos, Transform _parent, BlockShape.MeshPurpose _purpose, TextureFullArray _textureFullArray, int _meshIndex)
	{
		Vector3i vector3i = World.worldToBlockPos(_worldPos);
		byte sun;
		byte block;
		_world.GetSunAndBlockColors(vector3i, out sun, out block);
		VoxelMesh voxelMesh = VoxelMesh.Create(_meshIndex, MeshDescription.meshes[_meshIndex].meshType, 1);
		VoxelMesh[] array = new VoxelMesh[MeshDescription.meshes.Length];
		array[_meshIndex] = voxelMesh;
		_blockValue.Block.shape.renderFull(vector3i, _blockValue, ItemClassBlock.renderOffsetV, _vertices, new LightingAround(sun, block, 0), _textureFullArray, array, _purpose);
		if (voxelMesh.m_Vertices.Count == 0)
		{
			return null;
		}
		if (_go == null)
		{
			_go = new GameObject();
			_go.transform.SetParent(_parent, false);
			_go.AddComponent<UpdateLightOnChunkMesh>();
		}
		_go.name = "Block_" + _blockValue.type.ToString();
		MeshFilter[] array2;
		MeshRenderer[] mr;
		VoxelMesh.CreateMeshFilter(_meshIndex, 0, _go, "Item", false, out array2, out mr);
		if (array2[0] != null)
		{
			voxelMesh.CopyToMesh(array2, mr, 0, null);
		}
		return _go.transform;
	}

	// Token: 0x06002CBE RID: 11454 RVA: 0x000197A5 File Offset: 0x000179A5
	public override ItemClass.EnumCrosshairType GetCrosshairType(ItemInventoryData _holdingData)
	{
		return ItemClass.EnumCrosshairType.Plus;
	}

	// Token: 0x06002CBF RID: 11455 RVA: 0x0011934C File Offset: 0x0011754C
	public override RenderCubeType GetFocusType(ItemInventoryData _data)
	{
		return RenderCubeType.FullBlockBothSides;
	}

	// Token: 0x06002CC0 RID: 11456 RVA: 0x0012AB3F File Offset: 0x00128D3F
	public override float GetFocusRange()
	{
		return Constants.cDigAndBuildDistance;
	}

	// Token: 0x06002CC1 RID: 11457 RVA: 0x0012AB46 File Offset: 0x00128D46
	public override void ExecuteAction(int _actionIdx, ItemInventoryData _data, bool _bReleased, PlayerActionsLocal _playerActions)
	{
		if (_actionIdx == 0)
		{
			GameManager.Instance.GetActiveBlockTool().ExecuteAttackAction(_data, _bReleased, _playerActions);
			return;
		}
		GameManager.Instance.GetActiveBlockTool().ExecuteUseAction(_data, _bReleased, _playerActions);
	}

	// Token: 0x06002CC2 RID: 11458 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsFocusBlockInside()
	{
		return false;
	}

	// Token: 0x06002CC3 RID: 11459 RVA: 0x000470CA File Offset: 0x000452CA
	public override Vector3 GetDroppedCorrectionRotation()
	{
		return Vector3.zero;
	}

	// Token: 0x06002CC4 RID: 11460 RVA: 0x0012AB74 File Offset: 0x00128D74
	public override Vector3 GetCorrectionRotation()
	{
		return new Vector3(90f, 0f, 0f);
	}

	// Token: 0x06002CC5 RID: 11461 RVA: 0x000470CA File Offset: 0x000452CA
	public override Vector3 GetCorrectionPosition()
	{
		return Vector3.zero;
	}

	// Token: 0x06002CC6 RID: 11462 RVA: 0x0012AB8A File Offset: 0x00128D8A
	public override Vector3 GetCorrectionScale()
	{
		return new Vector3(0.1f, 0.1f, 0.1f);
	}

	// Token: 0x06002CC7 RID: 11463 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool CanHold()
	{
		return false;
	}

	// Token: 0x04002363 RID: 9059
	[PublicizedFrom(EAccessModifier.Private)]
	public const byte cNoRotation = 128;

	// Token: 0x04002364 RID: 9060
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Vector3 renderOffsetV = new Vector3(-0.5f, -0.5f, -0.5f);

	// Token: 0x02000566 RID: 1382
	public class ItemBlockInventoryData : ItemInventoryData
	{
		// Token: 0x06002CC9 RID: 11465 RVA: 0x0012ABBC File Offset: 0x00128DBC
		public ItemBlockInventoryData(ItemClass _item, ItemStack _itemStack, IGameManager _gameManager, EntityAlive _holdingEntity, int _slotIdx) : base(_item, _itemStack, _gameManager, _holdingEntity, _slotIdx)
		{
			this.lastBuildTime = 0f;
			this.rotation = 128;
			Block block = Block.list[_item.Id];
			if (block.HandleFace != BlockFace.None)
			{
				this.mode = BlockPlacement.EnumRotationMode.ToFace;
			}
			if (block.BlockPlacementHelper != BlockPlacement.None)
			{
				this.mode = BlockPlacement.EnumRotationMode.Auto;
			}
		}

		// Token: 0x04002365 RID: 9061
		public float lastBuildTime;

		// Token: 0x04002366 RID: 9062
		public byte rotation;

		// Token: 0x04002367 RID: 9063
		public BlockPlacement.EnumRotationMode mode = BlockPlacement.EnumRotationMode.Simple;

		// Token: 0x04002368 RID: 9064
		public int localRot;

		// Token: 0x04002369 RID: 9065
		public int damage;
	}
}
