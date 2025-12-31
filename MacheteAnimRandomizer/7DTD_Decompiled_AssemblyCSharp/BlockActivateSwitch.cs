using System;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020000F0 RID: 240
[Preserve]
public class BlockActivateSwitch : Block
{
	// Token: 0x1700006F RID: 111
	// (get) Token: 0x06000624 RID: 1572 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowBlockTriggers
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000626 RID: 1574 RVA: 0x0002C815 File Offset: 0x0002AA15
	public override void Init()
	{
		base.Init();
		base.Properties.ParseBool(BlockActivateSwitch.PropSingleUse, ref this.singleUse);
		base.Properties.ParseString(BlockActivateSwitch.PropActivateSound, ref this.activateSound);
	}

	// Token: 0x06000627 RID: 1575 RVA: 0x0002C313 File Offset: 0x0002A513
	public override void LateInit()
	{
		base.LateInit();
	}

	// Token: 0x06000628 RID: 1576 RVA: 0x0002C31B File Offset: 0x0002A51B
	public override void OnBlockValueChanged(WorldBase _world, Chunk _chunk, int _clrIdx, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
		base.OnBlockValueChanged(_world, _chunk, _clrIdx, _blockPos, _oldBlockValue, _newBlockValue);
		this.Refresh(_world, _chunk, _clrIdx, _blockPos, _newBlockValue);
	}

	// Token: 0x06000629 RID: 1577 RVA: 0x0002C339 File Offset: 0x0002A539
	public override void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, int _cIdx, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		base.OnBlockEntityTransformAfterActivated(_world, _blockPos, _cIdx, _blockValue, _ebcd);
		this.Refresh(_world, null, _cIdx, _blockPos, _blockValue);
	}

	// Token: 0x0600062A RID: 1578 RVA: 0x0002C354 File Offset: 0x0002A554
	public override void OnBlockRemoved(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		BlockUtilityNavIcon.RemoveNavObject(_blockPos);
		base.OnBlockRemoved(_world, _chunk, _blockPos, _blockValue);
	}

	// Token: 0x0600062B RID: 1579 RVA: 0x0002C367 File Offset: 0x0002A567
	public override void OnBlockUnloaded(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		BlockUtilityNavIcon.RemoveNavObject(_blockPos);
		base.OnBlockUnloaded(_world, _clrIdx, _blockPos, _blockValue);
	}

	// Token: 0x0600062C RID: 1580 RVA: 0x0002C84C File Offset: 0x0002AA4C
	public override string GetActivationText(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		if (!_world.IsEditor())
		{
			if (this.singleUse && (_blockValue.meta & 2) != 0)
			{
				return "";
			}
			if ((_blockValue.meta & 1) == 0)
			{
				return "";
			}
		}
		PlayerActionsLocal playerInput = ((EntityPlayerLocal)_entityFocusing).playerInput;
		string arg = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
		string localizedBlockName = _blockValue.Block.GetLocalizedBlockName();
		return string.Format(Localization.Get("questBlockActivate", false), arg, localizedBlockName);
	}

	// Token: 0x0600062D RID: 1581 RVA: 0x0002C8E0 File Offset: 0x0002AAE0
	public override void OnBlockAdded(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, PlatformUserIdentifierAbs _addedByPlayer)
	{
		base.OnBlockAdded(_world, _chunk, _blockPos, _blockValue, _addedByPlayer);
		BlockTrigger blockTrigger = _chunk.GetBlockTrigger(World.toBlock(_blockPos));
		if (blockTrigger != null)
		{
			bool flag = blockTrigger.HasAnyTriggeredBy();
			_blockValue.meta = (byte)(((int)_blockValue.meta & -2) | (flag ? 0 : 1));
			_world.SetBlockRPC(_chunk.ClrIdx, _blockPos, _blockValue);
		}
		this.Refresh(_world, _chunk, _chunk.ClrIdx, _blockPos, _blockValue);
	}

	// Token: 0x0600062E RID: 1582 RVA: 0x0002C94C File Offset: 0x0002AB4C
	public override void OnTriggerAddedFromPrefab(BlockTrigger _trigger, Vector3i _blockPos, BlockValue _blockValue, FastTags<TagGroup.Global> _questTags)
	{
		if (GameManager.Instance.World.IsEditor())
		{
			return;
		}
		World world = GameManager.Instance.World;
		base.OnTriggerAddedFromPrefab(_trigger, _blockPos, _blockValue, _questTags);
		bool flag = _trigger.HasAnyTriggeredBy();
		_blockValue.meta = (byte)(((int)_blockValue.meta & -2) | (flag ? 0 : 1));
		world.SetBlock(_trigger.Chunk.ClrIdx, _trigger.ToWorldPos(), _blockValue, true, false);
		this.Refresh(GameManager.Instance.World, _trigger.Chunk, _trigger.Chunk.ClrIdx, _trigger.LocalChunkPos, _blockValue);
	}

	// Token: 0x0600062F RID: 1583 RVA: 0x0002C9E4 File Offset: 0x0002ABE4
	public override bool OnBlockActivated(string _commandName, WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (!_world.IsEditor())
		{
			if (this.singleUse && (_blockValue.meta & 2) != 0)
			{
				return false;
			}
			if ((_blockValue.meta & 1) == 0)
			{
				return false;
			}
			if (_player.prefab == null && SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				return false;
			}
		}
		if (!(_commandName == "activate"))
		{
			if (_commandName == "trigger")
			{
				XUiC_TriggerProperties.Show(_player.PlayerUI.xui, _clrIdx, _blockPos, true, true);
			}
		}
		else if (!_world.IsEditor())
		{
			bool flag = (_blockValue.meta & 2) > 0;
			if ((_blockValue.meta & 1) > 0 && (!flag || !this.singleUse))
			{
				base.HandleTrigger(_player, (World)_world, _clrIdx, _blockPos, _blockValue);
				Manager.BroadcastPlay(_blockPos.ToVector3() + Vector3.one * 0.5f, this.activateSound, 0f);
				flag = !flag;
				_blockValue.meta = (byte)(((int)_blockValue.meta & -3) | (flag ? 2 : 0));
				_world.SetBlockRPC(_clrIdx, _blockPos, _blockValue);
				this.Refresh(_world, null, _clrIdx, _blockPos, _blockValue);
			}
			return true;
		}
		return false;
	}

	// Token: 0x06000630 RID: 1584 RVA: 0x0002CB18 File Offset: 0x0002AD18
	public override void OnTriggerRefresh(BlockTrigger _trigger, BlockValue _bv, FastTags<TagGroup.Global> questTag)
	{
		bool flag = GameManager.Instance.World.triggerManager.CheckPowerState(_trigger, questTag);
		_bv.meta = (byte)(((int)_bv.meta & -2) | (flag ? 1 : 0));
		GameManager.Instance.World.SetBlockRPC(_trigger.Chunk.ClrIdx, _trigger.ToWorldPos(), _bv);
	}

	// Token: 0x06000631 RID: 1585 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool HasBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return true;
	}

	// Token: 0x06000632 RID: 1586 RVA: 0x0002CB78 File Offset: 0x0002AD78
	public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		((Chunk)_world.ChunkClusters[_clrIdx].GetChunkSync(World.toChunkXZ(_blockPos.x), _blockPos.y, World.toChunkXZ(_blockPos.z))).GetBlockTrigger(World.toBlock(_blockPos));
		this.cmds[0].enabled = !_world.IsEditor();
		this.cmds[1].enabled = (_world.IsEditor() && !GameUtils.IsWorldEditor());
		return this.cmds;
	}

	// Token: 0x06000633 RID: 1587 RVA: 0x0002CC0C File Offset: 0x0002AE0C
	public override void Refresh(WorldBase _world, Chunk _chunk, int _cIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.Refresh(_world, _chunk, _cIdx, _blockPos, _blockValue);
		bool flag = (_blockValue.meta & 2) > 0;
		bool flag2 = (_blockValue.meta & 1) > 0;
		BlockUtilityNavIcon.UpdateNavIcon(flag2 && !flag, _blockPos);
		IChunk chunk = _chunk;
		if (chunk == null)
		{
			ChunkCluster chunkCluster = _world.ChunkClusters[_cIdx];
			if (chunkCluster == null)
			{
				return;
			}
			chunk = chunkCluster.GetChunkSync(World.toChunkXZ(_blockPos.x), World.toChunkY(_blockPos.y), World.toChunkXZ(_blockPos.z));
			if (chunk == null)
			{
				return;
			}
		}
		if (chunk == null)
		{
			return;
		}
		BlockEntityData blockEntity = chunk.GetBlockEntity(_blockPos);
		if (blockEntity == null || !blockEntity.bHasTransform)
		{
			return;
		}
		BlockSwitchController component = blockEntity.transform.GetComponent<BlockSwitchController>();
		if (component)
		{
			component.SetState(flag2, flag);
		}
	}

	// Token: 0x06000634 RID: 1588 RVA: 0x0002CCD1 File Offset: 0x0002AED1
	public override void OnTriggered(EntityPlayer _player, WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, List<BlockChangeInfo> _blockChanges, BlockTrigger _triggeredBy)
	{
		base.OnTriggered(_player, _world, _cIdx, _blockPos, _blockValue, _blockChanges, _triggeredBy);
		_blockValue.meta = (byte)(((int)_blockValue.meta & -2) | 1);
		_blockChanges.Add(new BlockChangeInfo(_cIdx, _blockPos, _blockValue));
	}

	// Token: 0x04000779 RID: 1913
	public const int cMetaPowered = 1;

	// Token: 0x0400077A RID: 1914
	public const int cMetaOn = 2;

	// Token: 0x0400077B RID: 1915
	[PublicizedFrom(EAccessModifier.Private)]
	public bool singleUse;

	// Token: 0x0400077C RID: 1916
	[PublicizedFrom(EAccessModifier.Private)]
	public string activateSound;

	// Token: 0x0400077D RID: 1917
	[PublicizedFrom(EAccessModifier.Private)]
	public new BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("activate", "electric_switch", true, false, null),
		new BlockActivationCommand("trigger", "wrench", true, false, null)
	};

	// Token: 0x0400077E RID: 1918
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropSingleUse = "SingleUse";

	// Token: 0x0400077F RID: 1919
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropActivateSound = "ActivateSound";
}
