using System;

// Token: 0x02000156 RID: 342
public interface ITileEntityFeature
{
	// Token: 0x170000AE RID: 174
	// (get) Token: 0x060009E0 RID: 2528
	TileEntityFeatureData FeatureData { get; }

	// Token: 0x170000AF RID: 175
	// (get) Token: 0x060009E1 RID: 2529
	TileEntityComposite Parent { get; }

	// Token: 0x060009E2 RID: 2530
	void Init(TileEntityComposite _parent, TileEntityFeatureData _featureData);

	// Token: 0x060009E3 RID: 2531
	void CopyFrom(TileEntityComposite _other);

	// Token: 0x060009E4 RID: 2532
	void OnRemove(World _world);

	// Token: 0x060009E5 RID: 2533
	void OnUnload(World _world);

	// Token: 0x060009E6 RID: 2534
	void OnDestroy();

	// Token: 0x060009E7 RID: 2535
	void PlaceBlock(WorldBase _world, BlockPlacement.Result _result, EntityAlive _placingEntity);

	// Token: 0x060009E8 RID: 2536
	void SetBlockEntityData(BlockEntityData _blockEntityData);

	// Token: 0x060009E9 RID: 2537
	void UpgradeDowngradeFrom(TileEntityComposite _other);

	// Token: 0x060009EA RID: 2538
	void ReplacedBy(BlockValue _bvOld, BlockValue _bvNew, TileEntity _teNew);

	// Token: 0x060009EB RID: 2539
	void Reset(FastTags<TagGroup.Global> _questTags);

	// Token: 0x060009EC RID: 2540
	void UpdateTick(World _world);

	// Token: 0x060009ED RID: 2541
	string GetActivationText(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityAlive _entityFocusing, string _activateHotkeyMarkup, string _focusedTileEntityName);

	// Token: 0x060009EE RID: 2542
	void InitBlockActivationCommands(Action<BlockActivationCommand, TileEntityComposite.EBlockCommandOrder, TileEntityFeatureData> _addCallback);

	// Token: 0x060009EF RID: 2543
	void UpdateBlockActivationCommands(ref BlockActivationCommand _command, ReadOnlySpan<char> _commandName, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityAlive _entityFocusing);

	// Token: 0x060009F0 RID: 2544
	bool OnBlockActivated(ReadOnlySpan<char> _commandName, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player);

	// Token: 0x060009F1 RID: 2545
	void Read(PooledBinaryReader _br, TileEntity.StreamModeRead _eStreamMode, int _readVersion);

	// Token: 0x060009F2 RID: 2546
	void Write(PooledBinaryWriter _bw, TileEntity.StreamModeWrite _eStreamMode);
}
