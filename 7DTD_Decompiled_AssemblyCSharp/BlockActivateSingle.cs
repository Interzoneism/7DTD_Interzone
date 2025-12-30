using System;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020000EF RID: 239
[Preserve]
public class BlockActivateSingle : Block
{
	// Token: 0x1700006E RID: 110
	// (get) Token: 0x06000613 RID: 1555 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowBlockTriggers
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000615 RID: 1557 RVA: 0x0002C2A8 File Offset: 0x0002A4A8
	public override void Init()
	{
		base.Init();
		base.Properties.ParseString(BlockActivateSingle.PropActivateSound, ref this.activateSound);
		base.Properties.ParseString(BlockActivateSingle.PropActivatedAnimBool, ref this.AnimActivatedBool);
		base.Properties.ParseString(BlockActivateSingle.PropActivatedAnimTrigger, ref this.AnimActivatedTrigger);
		base.Properties.ParseString(BlockActivateSingle.PropActivatedAnimState, ref this.AnimActivatedState);
	}

	// Token: 0x06000616 RID: 1558 RVA: 0x0002C313 File Offset: 0x0002A513
	public override void LateInit()
	{
		base.LateInit();
	}

	// Token: 0x06000617 RID: 1559 RVA: 0x0002C31B File Offset: 0x0002A51B
	public override void OnBlockValueChanged(WorldBase _world, Chunk _chunk, int _clrIdx, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
		base.OnBlockValueChanged(_world, _chunk, _clrIdx, _blockPos, _oldBlockValue, _newBlockValue);
		this.Refresh(_world, _chunk, _clrIdx, _blockPos, _newBlockValue);
	}

	// Token: 0x06000618 RID: 1560 RVA: 0x0002C339 File Offset: 0x0002A539
	public override void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, int _cIdx, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		base.OnBlockEntityTransformAfterActivated(_world, _blockPos, _cIdx, _blockValue, _ebcd);
		this.Refresh(_world, null, _cIdx, _blockPos, _blockValue);
	}

	// Token: 0x06000619 RID: 1561 RVA: 0x0002C354 File Offset: 0x0002A554
	public override void OnBlockRemoved(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		BlockUtilityNavIcon.RemoveNavObject(_blockPos);
		base.OnBlockRemoved(_world, _chunk, _blockPos, _blockValue);
	}

	// Token: 0x0600061A RID: 1562 RVA: 0x0002C367 File Offset: 0x0002A567
	public override void OnBlockUnloaded(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		BlockUtilityNavIcon.RemoveNavObject(_blockPos);
		base.OnBlockUnloaded(_world, _clrIdx, _blockPos, _blockValue);
	}

	// Token: 0x0600061B RID: 1563 RVA: 0x0002C37C File Offset: 0x0002A57C
	public override string GetActivationText(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		if (!_world.IsEditor() && (_blockValue.meta & 2) != 0)
		{
			return "";
		}
		PlayerActionsLocal playerInput = ((EntityPlayerLocal)_entityFocusing).playerInput;
		string arg = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
		string localizedBlockName = _blockValue.Block.GetLocalizedBlockName();
		return string.Format(Localization.Get("questBlockActivate", false), arg, localizedBlockName);
	}

	// Token: 0x0600061C RID: 1564 RVA: 0x0002C3F5 File Offset: 0x0002A5F5
	public override void OnBlockAdded(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, PlatformUserIdentifierAbs _addedByPlayer)
	{
		base.OnBlockAdded(_world, _chunk, _blockPos, _blockValue, _addedByPlayer);
		_chunk.GetBlockTrigger(World.toBlock(_blockPos));
		this.Refresh(_world, _chunk, _chunk.ClrIdx, _blockPos, _blockValue);
	}

	// Token: 0x0600061D RID: 1565 RVA: 0x0002C424 File Offset: 0x0002A624
	public override bool OnBlockActivated(string _commandName, WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (!_world.IsEditor())
		{
			if ((_blockValue.meta & 2) != 0)
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
				XUiC_TriggerProperties.Show(_player.PlayerUI.xui, _clrIdx, _blockPos, true, false);
			}
		}
		else if (!_world.IsEditor())
		{
			bool flag = (_blockValue.meta & 2) > 0;
			if (!flag)
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

	// Token: 0x0600061E RID: 1566 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool HasBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return true;
	}

	// Token: 0x0600061F RID: 1567 RVA: 0x0002C528 File Offset: 0x0002A728
	public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		((Chunk)_world.ChunkClusters[_clrIdx].GetChunkSync(World.toChunkXZ(_blockPos.x), _blockPos.y, World.toChunkXZ(_blockPos.z))).GetBlockTrigger(World.toBlock(_blockPos));
		this.cmds[0].enabled = !_world.IsEditor();
		this.cmds[1].enabled = (_world.IsEditor() && !GameUtils.IsWorldEditor());
		return this.cmds;
	}

	// Token: 0x06000620 RID: 1568 RVA: 0x0002C5BC File Offset: 0x0002A7BC
	public override void Refresh(WorldBase _world, Chunk _chunk, int _cIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.Refresh(_world, _chunk, _cIdx, _blockPos, _blockValue);
		bool flag = (_blockValue.meta & 2) > 0;
		BlockUtilityNavIcon.UpdateNavIcon(!flag, _blockPos);
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
		BlockSwitchSingleController component = blockEntity.transform.GetComponent<BlockSwitchSingleController>();
		if (component)
		{
			component.SetState(flag);
		}
		this.updateAnimState(blockEntity, flag);
	}

	// Token: 0x06000621 RID: 1569 RVA: 0x0002C674 File Offset: 0x0002A874
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateAnimState(BlockEntityData _ebcd, bool _bOpen)
	{
		if (this.AnimActivatedBool == "" || this.AnimActivatedTrigger == "")
		{
			return;
		}
		Animator[] componentsInChildren;
		if (_ebcd != null && _ebcd.bHasTransform && (componentsInChildren = _ebcd.transform.GetComponentsInChildren<Animator>()) != null)
		{
			foreach (Animator animator in componentsInChildren)
			{
				animator.SetBool(this.AnimActivatedBool, _bOpen);
				animator.SetTrigger(this.AnimActivatedTrigger);
			}
		}
	}

	// Token: 0x06000622 RID: 1570 RVA: 0x0002C6F0 File Offset: 0x0002A8F0
	public override void ForceAnimationState(BlockValue _blockValue, BlockEntityData _ebcd)
	{
		if (this.AnimActivatedState == "" || this.AnimActivatedBool == "")
		{
			return;
		}
		Animator[] componentsInChildren;
		if (_ebcd != null && _ebcd.bHasTransform && (componentsInChildren = _ebcd.transform.GetComponentsInChildren<Animator>(false)) != null)
		{
			bool flag = (_blockValue.meta & 2) > 0;
			foreach (Animator animator in componentsInChildren)
			{
				animator.SetBool(this.AnimActivatedBool, flag);
				if (flag)
				{
					animator.CrossFade(this.AnimActivatedState, 0f);
				}
				else
				{
					animator.CrossFade(this.AnimActivatedState, 0f);
				}
			}
		}
	}

	// Token: 0x0400076F RID: 1903
	public const int cMetaOn = 2;

	// Token: 0x04000770 RID: 1904
	[PublicizedFrom(EAccessModifier.Private)]
	public string activateSound;

	// Token: 0x04000771 RID: 1905
	[PublicizedFrom(EAccessModifier.Protected)]
	public string AnimActivatedBool = "";

	// Token: 0x04000772 RID: 1906
	[PublicizedFrom(EAccessModifier.Protected)]
	public string AnimActivatedTrigger = "";

	// Token: 0x04000773 RID: 1907
	[PublicizedFrom(EAccessModifier.Protected)]
	public string AnimActivatedState = "";

	// Token: 0x04000774 RID: 1908
	[PublicizedFrom(EAccessModifier.Private)]
	public new BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("activate", "electric_switch", true, false, null),
		new BlockActivationCommand("trigger", "wrench", true, false, null)
	};

	// Token: 0x04000775 RID: 1909
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropActivateSound = "ActivateSound";

	// Token: 0x04000776 RID: 1910
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropActivatedAnimBool = "ActivatedAnimBool";

	// Token: 0x04000777 RID: 1911
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropActivatedAnimTrigger = "ActivatedAnimTrigger";

	// Token: 0x04000778 RID: 1912
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropActivatedAnimState = "ActivatedAnimState";
}
