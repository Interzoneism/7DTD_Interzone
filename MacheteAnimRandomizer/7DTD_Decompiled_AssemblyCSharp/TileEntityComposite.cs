using System;
using System.Collections.Generic;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200015D RID: 349
[Preserve]
public class TileEntityComposite : TileEntity
{
	// Token: 0x170000C2 RID: 194
	// (get) Token: 0x06000A7B RID: 2683 RVA: 0x00044595 File Offset: 0x00042795
	public TileEntityCompositeData TeData
	{
		get
		{
			return this.teData;
		}
	}

	// Token: 0x170000C3 RID: 195
	// (get) Token: 0x06000A7C RID: 2684 RVA: 0x0004459D File Offset: 0x0004279D
	public bool PlayerPlaced
	{
		get
		{
			return this.Owner != null;
		}
	}

	// Token: 0x170000C4 RID: 196
	// (get) Token: 0x06000A7D RID: 2685 RVA: 0x000445A8 File Offset: 0x000427A8
	// (set) Token: 0x06000A7E RID: 2686 RVA: 0x000445B0 File Offset: 0x000427B0
	public PlatformUserIdentifierAbs Owner { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06000A7F RID: 2687 RVA: 0x000445B9 File Offset: 0x000427B9
	public override TileEntityType GetTileEntityType()
	{
		return TileEntityType.Composite;
	}

	// Token: 0x06000A80 RID: 2688 RVA: 0x000445BD File Offset: 0x000427BD
	public TileEntityComposite(Chunk _chunk) : base(_chunk)
	{
	}

	// Token: 0x06000A81 RID: 2689 RVA: 0x000445C6 File Offset: 0x000427C6
	public TileEntityComposite(Chunk _chunk, BlockCompositeTileEntity _block) : base(_chunk)
	{
		this.initModules(_block);
	}

	// Token: 0x06000A82 RID: 2690 RVA: 0x000445D6 File Offset: 0x000427D6
	[PublicizedFrom(EAccessModifier.Private)]
	public TileEntityComposite(TileEntityComposite _original) : base(null)
	{
	}

	// Token: 0x06000A83 RID: 2691 RVA: 0x000445E0 File Offset: 0x000427E0
	[PublicizedFrom(EAccessModifier.Private)]
	public void initModules(BlockCompositeTileEntity _block)
	{
		if (!TileEntityCompositeData.FeaturesByBlock.TryGetValue(_block, out this.teData))
		{
			throw new Exception("Trying to initialize TileEntityComposite for block " + _block.GetBlockName() + " failed, no feature definitions found");
		}
		this.modulesCustomOrder = new ITileEntityFeature[this.teData.Features.Count];
		this.modulesInternalOrder = new ITileEntityFeature[this.teData.Features.Count];
		for (int i = 0; i < this.teData.Features.Count; i++)
		{
			TileEntityFeatureData tileEntityFeatureData = this.teData.Features[i];
			ITileEntityFeature tileEntityFeature = tileEntityFeatureData.InstantiateModule();
			this.modulesCustomOrder[tileEntityFeatureData.CustomOrder] = tileEntityFeature;
			this.modulesInternalOrder[i] = tileEntityFeature;
		}
		for (int j = 0; j < this.teData.Features.Count; j++)
		{
			TileEntityFeatureData featureData = this.teData.Features[j];
			this.modulesInternalOrder[j].Init(this, featureData);
		}
	}

	// Token: 0x06000A84 RID: 2692 RVA: 0x000446DA File Offset: 0x000428DA
	public override TileEntity Clone()
	{
		TileEntityComposite tileEntityComposite = new TileEntityComposite(this);
		tileEntityComposite.CopyFrom(this);
		return tileEntityComposite;
	}

	// Token: 0x06000A85 RID: 2693 RVA: 0x000446EC File Offset: 0x000428EC
	public override void CopyFrom(TileEntity _other)
	{
		TileEntityComposite tileEntityComposite = _other as TileEntityComposite;
		if (tileEntityComposite == null)
		{
			Log.Warning(string.Format("TEC.CopyFrom with non TEC ({0})", _other));
			return;
		}
		this.initModules(tileEntityComposite.TeData.Block);
		this.bUserAccessing = _other.IsUserAccessing();
		this.Owner = tileEntityComposite.Owner;
		ITileEntityFeature[] array = this.modulesInternalOrder;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].CopyFrom(tileEntityComposite);
		}
	}

	// Token: 0x06000A86 RID: 2694 RVA: 0x0004475C File Offset: 0x0004295C
	public override void UpdateTick(World _world)
	{
		base.UpdateTick(_world);
		ITileEntityFeature[] array = this.modulesInternalOrder;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].UpdateTick(_world);
		}
	}

	// Token: 0x06000A87 RID: 2695 RVA: 0x00044790 File Offset: 0x00042990
	public void PlaceBlock(WorldBase _world, BlockPlacement.Result _result, EntityAlive _ea)
	{
		if (_ea != null && _ea.entityType == EntityType.Player)
		{
			this.Owner = PlatformManager.InternalLocalUserIdentifier;
			base.SetModified();
		}
		ITileEntityFeature[] array = this.modulesInternalOrder;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].PlaceBlock(_world, _result, _ea);
		}
	}

	// Token: 0x06000A88 RID: 2696 RVA: 0x000447E0 File Offset: 0x000429E0
	public void SetBlockEntityData(BlockEntityData _blockEntityData)
	{
		ITileEntityFeature[] array = this.modulesInternalOrder;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetBlockEntityData(_blockEntityData);
		}
	}

	// Token: 0x06000A89 RID: 2697 RVA: 0x0004480C File Offset: 0x00042A0C
	public override void OnRemove(World _world)
	{
		ITileEntityFeature[] array = this.modulesInternalOrder;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].OnRemove(_world);
		}
		base.OnRemove(_world);
	}

	// Token: 0x06000A8A RID: 2698 RVA: 0x00044840 File Offset: 0x00042A40
	public override void OnUnload(World _world)
	{
		ITileEntityFeature[] array = this.modulesInternalOrder;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].OnUnload(_world);
		}
		base.OnUnload(_world);
	}

	// Token: 0x06000A8B RID: 2699 RVA: 0x00044874 File Offset: 0x00042A74
	public override void OnDestroy()
	{
		ITileEntityFeature[] array = this.modulesInternalOrder;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].OnDestroy();
		}
		base.OnDestroy();
	}

	// Token: 0x06000A8C RID: 2700 RVA: 0x000448A4 File Offset: 0x00042AA4
	public override void UpgradeDowngradeFrom(TileEntity _other)
	{
		base.UpgradeDowngradeFrom(_other);
		TileEntityComposite tileEntityComposite = _other as TileEntityComposite;
		if (tileEntityComposite == null)
		{
			return;
		}
		this.Owner = tileEntityComposite.Owner;
		ITileEntityFeature[] array = this.modulesInternalOrder;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].UpgradeDowngradeFrom(tileEntityComposite);
		}
	}

	// Token: 0x06000A8D RID: 2701 RVA: 0x000448F0 File Offset: 0x00042AF0
	public override void ReplacedBy(BlockValue _bvOld, BlockValue _bvNew, TileEntity _teNew)
	{
		base.ReplacedBy(_bvOld, _bvNew, _teNew);
		ITileEntityFeature[] array = this.modulesInternalOrder;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ReplacedBy(_bvOld, _bvNew, _teNew);
		}
	}

	// Token: 0x06000A8E RID: 2702 RVA: 0x00044928 File Offset: 0x00042B28
	public override void Reset(FastTags<TagGroup.Global> _questTags)
	{
		base.Reset(_questTags);
		ITileEntityFeature[] array = this.modulesInternalOrder;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Reset(_questTags);
		}
	}

	// Token: 0x06000A8F RID: 2703 RVA: 0x0004495A File Offset: 0x00042B5A
	public override bool IsActive(World world)
	{
		return base.IsActive(world);
	}

	// Token: 0x06000A90 RID: 2704 RVA: 0x00044963 File Offset: 0x00042B63
	public void SetOwner(PlatformUserIdentifierAbs _userIdentifier)
	{
		this.Owner = _userIdentifier;
		base.SetModified();
	}

	// Token: 0x06000A91 RID: 2705 RVA: 0x00044974 File Offset: 0x00042B74
	public string GetActivationText(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityAlive _entityFocusing, string _activateHotkeyMarkup, string _focusedTileEntityName)
	{
		ITileEntityFeature[] array = this.modulesCustomOrder;
		for (int i = 0; i < array.Length; i++)
		{
			string activationText = array[i].GetActivationText(_world, _blockPos, _blockValue, _entityFocusing, _activateHotkeyMarkup, _focusedTileEntityName);
			if (activationText != null)
			{
				return activationText;
			}
		}
		return null;
	}

	// Token: 0x06000A92 RID: 2706 RVA: 0x000449B0 File Offset: 0x00042BB0
	public BlockActivationCommand[] InitBlockActivationCommands()
	{
		List<ValueTuple<BlockActivationCommand, int>> commands = new List<ValueTuple<BlockActivationCommand, int>>();
		int firsts = 100;
		int regular = 200;
		int last = 300;
		Action<BlockActivationCommand, TileEntityComposite.EBlockCommandOrder, TileEntityFeatureData> <>9__1;
		foreach (ITileEntityFeature tileEntityFeature in this.modulesCustomOrder)
		{
			Action<BlockActivationCommand, TileEntityComposite.EBlockCommandOrder, TileEntityFeatureData> addCallback;
			if ((addCallback = <>9__1) == null)
			{
				addCallback = (<>9__1 = delegate(BlockActivationCommand _bac, TileEntityComposite.EBlockCommandOrder _order, TileEntityFeatureData _featureData)
				{
					BlockActivationCommand item = new BlockActivationCommand(_featureData.Name + ":" + _bac.text, _bac.icon, _bac.enabled, _bac.highlighted, null);
					int num2;
					switch (_order)
					{
					case TileEntityComposite.EBlockCommandOrder.First:
					{
						int num = firsts;
						firsts = num + 1;
						num2 = num;
						break;
					}
					case TileEntityComposite.EBlockCommandOrder.Normal:
					{
						int num = regular;
						regular = num + 1;
						num2 = num;
						break;
					}
					case TileEntityComposite.EBlockCommandOrder.Last:
					{
						int num = last;
						last = num + 1;
						num2 = num;
						break;
					}
					default:
						throw new ArgumentOutOfRangeException("_order", _order, null);
					}
					int item2 = num2;
					commands.Add(new ValueTuple<BlockActivationCommand, int>(item, item2));
				});
			}
			tileEntityFeature.InitBlockActivationCommands(addCallback);
		}
		commands.Sort((ValueTuple<BlockActivationCommand, int> _a, ValueTuple<BlockActivationCommand, int> _b) => _a.Item2.CompareTo(_b.Item2));
		BlockActivationCommand[] array2 = new BlockActivationCommand[commands.Count];
		for (int j = 0; j < commands.Count; j++)
		{
			array2[j] = commands[j].Item1;
		}
		return array2;
	}

	// Token: 0x06000A93 RID: 2707 RVA: 0x00044A9C File Offset: 0x00042C9C
	public bool UpdateBlockActivationCommands(BlockActivationCommand[] _commands, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityAlive _entityFocusing)
	{
		int frameCount = Time.frameCount;
		if (frameCount <= this.lastUpdateFrameOfBlockActivationCommands)
		{
			return this.lastUpdateHadEnabledCommands;
		}
		bool flag = false;
		for (int i = 0; i < _commands.Length; i++)
		{
			BlockActivationCommand blockActivationCommand = _commands[i];
			ReadOnlyMemory<char> featureName;
			ReadOnlyMemory<char> readOnlyMemory;
			if (TileEntityComposite.SplitFullCommandName(blockActivationCommand.text, out featureName, out readOnlyMemory))
			{
				ITileEntityFeature feature = this.GetFeature(featureName);
				if (feature != null)
				{
					feature.UpdateBlockActivationCommands(ref blockActivationCommand, readOnlyMemory.Span, _world, _blockPos, _blockValue, _entityFocusing);
					flag |= blockActivationCommand.enabled;
					_commands[i] = blockActivationCommand;
				}
			}
		}
		this.lastUpdateFrameOfBlockActivationCommands = frameCount;
		this.lastUpdateHadEnabledCommands = flag;
		return flag;
	}

	// Token: 0x06000A94 RID: 2708 RVA: 0x00044B2C File Offset: 0x00042D2C
	public bool OnBlockActivated(BlockActivationCommand[] _commands, string _commandName, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		ReadOnlyMemory<char> featureName;
		ReadOnlyMemory<char> readOnlyMemory;
		if (!TileEntityComposite.SplitFullCommandName(_commandName, out featureName, out readOnlyMemory))
		{
			return false;
		}
		ITileEntityFeature feature = this.GetFeature(featureName);
		return feature != null && feature.OnBlockActivated(readOnlyMemory.Span, _world, _blockPos, _blockValue, _player);
	}

	// Token: 0x06000A95 RID: 2709 RVA: 0x00044B68 File Offset: 0x00042D68
	public ITileEntityFeature GetFeature(ReadOnlyMemory<char> _featureName)
	{
		int featureIndex = this.teData.GetFeatureIndex(_featureName);
		if (featureIndex < 0)
		{
			return null;
		}
		return this.modulesInternalOrder[featureIndex];
	}

	// Token: 0x06000A96 RID: 2710 RVA: 0x00044B90 File Offset: 0x00042D90
	public T GetFeature<T>() where T : class
	{
		int featureIndex = this.teData.GetFeatureIndex<T>();
		if (featureIndex < 0)
		{
			return default(T);
		}
		return (T)((object)this.modulesInternalOrder[featureIndex]);
	}

	// Token: 0x06000A97 RID: 2711 RVA: 0x00044BC4 File Offset: 0x00042DC4
	public static bool SplitFullCommandName(string _fullCommandName, out ReadOnlyMemory<char> _moduleName, out ReadOnlyMemory<char> _commandName)
	{
		int num = _fullCommandName.IndexOf(':');
		if (num < 0)
		{
			_moduleName = default(ReadOnlyMemory<char>);
			_commandName = default(ReadOnlyMemory<char>);
			return false;
		}
		_moduleName = _fullCommandName.AsMemory(0, num);
		_commandName = _fullCommandName.AsMemory(num + 1);
		return true;
	}

	// Token: 0x06000A98 RID: 2712 RVA: 0x00044C0C File Offset: 0x00042E0C
	public override string ToString()
	{
		return string.Format(string.Concat(new string[]
		{
			"[TEC] ",
			this.GetTileEntityType().ToStringCached<TileEntityType>(),
			"/",
			base.ToWorldPos().ToString(),
			"/",
			this.entityId.ToString(),
			" / ",
			this.teData.Block.GetBlockName()
		}), Array.Empty<object>());
	}

	// Token: 0x06000A99 RID: 2713 RVA: 0x00044C94 File Offset: 0x00042E94
	public override void read(PooledBinaryReader _br, TileEntity.StreamModeRead _eStreamMode)
	{
		base.read(_br, _eStreamMode);
		PooledBinaryReader.StreamReadSizeMarker streamReadSizeMarker = _br.ReadSizeMarker(PooledBinaryWriter.EMarkerSize.UInt32);
		int num = _br.ReadInt32();
		BlockCompositeTileEntity blockCompositeTileEntity = Block.list[num] as BlockCompositeTileEntity;
		if (blockCompositeTileEntity == null)
		{
			string str = "TileEntityComposite.read: Failed reading data, block ";
			Block block = Block.list[num];
			Log.Error(str + ((block != null) ? block.GetBlockName() : null) + " is not a BlockCompositeTileEntity");
			uint num2;
			_br.ValidateSizeMarker(ref streamReadSizeMarker, out num2, true);
			return;
		}
		try
		{
			this.Owner = PlatformUserIdentifierAbs.FromStream(_br, false, false);
			if (this.modulesCustomOrder == null)
			{
				this.initModules(blockCompositeTileEntity);
			}
			byte b = _br.ReadByte();
			if ((int)b != this.modulesCustomOrder.Length)
			{
				Log.Error(string.Format("{0}.{1}: Failed reading data for block {2}: Received {3} features, expected {4}", new object[]
				{
					"TileEntityComposite",
					"read",
					Block.list[num].GetBlockName(),
					b,
					this.modulesCustomOrder.Length
				}));
				uint num2;
				_br.ValidateSizeMarker(ref streamReadSizeMarker, out num2, true);
				return;
			}
			for (int i = 0; i < (int)b; i++)
			{
				ITileEntityFeature tileEntityFeature = this.modulesInternalOrder[i];
				int num3 = _br.ReadInt32();
				if (tileEntityFeature.FeatureData.NameHash != num3)
				{
					Log.Error(string.Format("{0}.{1}: Failed reading data for block {2}: Received hash {3:X8} does not equal expected hash {4:X8} for feature {5}", new object[]
					{
						"TileEntityComposite",
						"read",
						Block.list[num].GetBlockName(),
						num3,
						tileEntityFeature.FeatureData.NameHash,
						tileEntityFeature.FeatureData.Name
					}));
					uint num2;
					_br.ValidateSizeMarker(ref streamReadSizeMarker, out num2, true);
					return;
				}
				tileEntityFeature.Read(_br, _eStreamMode, this.readVersion);
			}
		}
		catch (Exception e)
		{
			Log.Error("TileEntityComposite.read: Failed reading data for block " + Block.list[num].GetBlockName() + ": Caught exception:");
			Log.Exception(e);
		}
		uint num4;
		if (!_br.ValidateSizeMarker(ref streamReadSizeMarker, out num4, true))
		{
			Log.Error(string.Format("{0}.{1}: Failed reading data for block {2}: Data received {3} B, read {4} B", new object[]
			{
				"TileEntityComposite",
				"read",
				Block.list[num].GetBlockName(),
				streamReadSizeMarker.ExpectedSize,
				num4
			}));
		}
	}

	// Token: 0x06000A9A RID: 2714 RVA: 0x00044EE8 File Offset: 0x000430E8
	public override void write(PooledBinaryWriter _bw, TileEntity.StreamModeWrite _eStreamMode)
	{
		base.write(_bw, _eStreamMode);
		PooledBinaryWriter.StreamWriteSizeMarker streamWriteSizeMarker = _bw.ReserveSizeMarker(PooledBinaryWriter.EMarkerSize.UInt32);
		_bw.Write(this.teData.Block.blockID);
		this.Owner.ToStream(_bw, false);
		_bw.Write((byte)this.modulesCustomOrder.Length);
		for (int i = 0; i < this.modulesInternalOrder.Length; i++)
		{
			ITileEntityFeature tileEntityFeature = this.modulesInternalOrder[i];
			_bw.Write(tileEntityFeature.FeatureData.NameHash);
			tileEntityFeature.Write(_bw, _eStreamMode);
		}
		_bw.FinalizeSizeMarker(ref streamWriteSizeMarker);
	}

	// Token: 0x0400093D RID: 2365
	[PublicizedFrom(EAccessModifier.Private)]
	public TileEntityCompositeData teData;

	// Token: 0x0400093E RID: 2366
	[PublicizedFrom(EAccessModifier.Private)]
	public ITileEntityFeature[] modulesCustomOrder;

	// Token: 0x0400093F RID: 2367
	[PublicizedFrom(EAccessModifier.Private)]
	public ITileEntityFeature[] modulesInternalOrder;

	// Token: 0x04000941 RID: 2369
	[PublicizedFrom(EAccessModifier.Private)]
	public int lastUpdateFrameOfBlockActivationCommands;

	// Token: 0x04000942 RID: 2370
	[PublicizedFrom(EAccessModifier.Private)]
	public bool lastUpdateHadEnabledCommands;

	// Token: 0x04000943 RID: 2371
	[PublicizedFrom(EAccessModifier.Private)]
	public const char ModuleCommandSeparator = ':';

	// Token: 0x0200015E RID: 350
	public enum EBlockCommandOrder
	{
		// Token: 0x04000945 RID: 2373
		First,
		// Token: 0x04000946 RID: 2374
		Normal,
		// Token: 0x04000947 RID: 2375
		Last
	}
}
