using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000157 RID: 343
[Preserve]
public abstract class TEFeatureAbs : ITileEntityFeature, ITileEntity
{
	// Token: 0x170000B0 RID: 176
	// (get) Token: 0x060009F3 RID: 2547 RVA: 0x0004248B File Offset: 0x0004068B
	// (set) Token: 0x060009F4 RID: 2548 RVA: 0x00042493 File Offset: 0x00040693
	public TileEntityFeatureData FeatureData { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

	// Token: 0x170000B1 RID: 177
	// (get) Token: 0x060009F5 RID: 2549 RVA: 0x0004249C File Offset: 0x0004069C
	// (set) Token: 0x060009F6 RID: 2550 RVA: 0x000424A4 File Offset: 0x000406A4
	public TileEntityComposite Parent { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

	// Token: 0x060009F7 RID: 2551 RVA: 0x000424AD File Offset: 0x000406AD
	public virtual void Init(TileEntityComposite _parent, TileEntityFeatureData _featureData)
	{
		this.Parent = _parent;
		this.FeatureData = _featureData;
	}

	// Token: 0x060009F8 RID: 2552 RVA: 0x000424BD File Offset: 0x000406BD
	public virtual void CopyFrom(TileEntityComposite _other)
	{
		throw new NotImplementedException();
	}

	// Token: 0x060009F9 RID: 2553 RVA: 0x00002914 File Offset: 0x00000B14
	public void OnRemove(World _world)
	{
	}

	// Token: 0x060009FA RID: 2554 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnUnload(World _world)
	{
	}

	// Token: 0x060009FB RID: 2555 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnDestroy()
	{
	}

	// Token: 0x060009FC RID: 2556 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void PlaceBlock(WorldBase _world, BlockPlacement.Result _result, EntityAlive _placingEntity)
	{
	}

	// Token: 0x060009FD RID: 2557 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SetBlockEntityData(BlockEntityData _blockEntityData)
	{
	}

	// Token: 0x060009FE RID: 2558 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void UpgradeDowngradeFrom(TileEntityComposite _other)
	{
	}

	// Token: 0x060009FF RID: 2559 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void ReplacedBy(BlockValue _bvOld, BlockValue _bvNew, TileEntity _teNew)
	{
	}

	// Token: 0x06000A00 RID: 2560 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void Reset(FastTags<TagGroup.Global> _questTags)
	{
	}

	// Token: 0x06000A01 RID: 2561 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void UpdateTick(World _world)
	{
	}

	// Token: 0x06000A02 RID: 2562 RVA: 0x00019766 File Offset: 0x00017966
	public virtual string GetActivationText(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityAlive _entityFocusing, string _activateHotkeyMarkup, string _focusedTileEntityName)
	{
		return null;
	}

	// Token: 0x06000A03 RID: 2563 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void InitBlockActivationCommands(Action<BlockActivationCommand, TileEntityComposite.EBlockCommandOrder, TileEntityFeatureData> _addCallback)
	{
	}

	// Token: 0x06000A04 RID: 2564 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void UpdateBlockActivationCommands(ref BlockActivationCommand _command, ReadOnlySpan<char> _commandName, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityAlive _entityFocusing)
	{
	}

	// Token: 0x06000A05 RID: 2565 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool OnBlockActivated(ReadOnlySpan<char> _commandName, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		return false;
	}

	// Token: 0x06000A06 RID: 2566 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void Read(PooledBinaryReader _br, TileEntity.StreamModeRead _eStreamMode, int _readVersion)
	{
	}

	// Token: 0x06000A07 RID: 2567 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void Write(PooledBinaryWriter _bw, TileEntity.StreamModeWrite _eStreamMode)
	{
	}

	// Token: 0x14000003 RID: 3
	// (add) Token: 0x06000A08 RID: 2568 RVA: 0x000424C4 File Offset: 0x000406C4
	// (remove) Token: 0x06000A09 RID: 2569 RVA: 0x000424D2 File Offset: 0x000406D2
	public event XUiEvent_TileEntityDestroyed Destroyed
	{
		add
		{
			this.Parent.Destroyed += value;
		}
		remove
		{
			this.Parent.Destroyed -= value;
		}
	}

	// Token: 0x170000B2 RID: 178
	// (get) Token: 0x06000A0A RID: 2570 RVA: 0x000424E0 File Offset: 0x000406E0
	public List<ITileEntityChangedListener> listeners
	{
		get
		{
			return this.Parent.listeners;
		}
	}

	// Token: 0x06000A0B RID: 2571 RVA: 0x000424ED File Offset: 0x000406ED
	public void SetUserAccessing(bool _bUserAccessing)
	{
		this.Parent.SetUserAccessing(_bUserAccessing);
	}

	// Token: 0x06000A0C RID: 2572 RVA: 0x000424FB File Offset: 0x000406FB
	public bool IsUserAccessing()
	{
		return this.Parent.IsUserAccessing();
	}

	// Token: 0x06000A0D RID: 2573 RVA: 0x00042508 File Offset: 0x00040708
	public void SetModified()
	{
		this.Parent.SetModified();
	}

	// Token: 0x06000A0E RID: 2574 RVA: 0x00042515 File Offset: 0x00040715
	public Chunk GetChunk()
	{
		return this.Parent.GetChunk();
	}

	// Token: 0x06000A0F RID: 2575 RVA: 0x00042522 File Offset: 0x00040722
	public Vector3i ToWorldPos()
	{
		return this.Parent.ToWorldPos();
	}

	// Token: 0x06000A10 RID: 2576 RVA: 0x0004252F File Offset: 0x0004072F
	public Vector3 ToWorldCenterPos()
	{
		return this.Parent.ToWorldCenterPos();
	}

	// Token: 0x170000B3 RID: 179
	// (get) Token: 0x06000A11 RID: 2577 RVA: 0x0004253C File Offset: 0x0004073C
	public BlockValue blockValue
	{
		get
		{
			return this.Parent.blockValue;
		}
	}

	// Token: 0x170000B4 RID: 180
	// (get) Token: 0x06000A12 RID: 2578 RVA: 0x00042549 File Offset: 0x00040749
	// (set) Token: 0x06000A13 RID: 2579 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual int EntityId
	{
		get
		{
			return this.Parent.EntityId;
		}
		set
		{
		}
	}

	// Token: 0x170000B5 RID: 181
	// (get) Token: 0x06000A14 RID: 2580 RVA: 0x00042556 File Offset: 0x00040756
	// (set) Token: 0x06000A15 RID: 2581 RVA: 0x00042563 File Offset: 0x00040763
	public virtual bool IsRemoving
	{
		get
		{
			return this.Parent.IsRemoving;
		}
		set
		{
			this.Parent.IsRemoving = value;
		}
	}

	// Token: 0x06000A16 RID: 2582 RVA: 0x00042571 File Offset: 0x00040771
	public int GetClrIdx()
	{
		return this.Parent.GetClrIdx();
	}

	// Token: 0x06000A17 RID: 2583 RVA: 0x0004257E File Offset: 0x0004077E
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool CommandIs(ReadOnlySpan<char> _givenCommand, string _compareCommand)
	{
		return _givenCommand.Equals(_compareCommand, StringComparison.Ordinal);
	}

	// Token: 0x06000A18 RID: 2584 RVA: 0x0000A7E3 File Offset: 0x000089E3
	[PublicizedFrom(EAccessModifier.Protected)]
	public TEFeatureAbs()
	{
	}
}
