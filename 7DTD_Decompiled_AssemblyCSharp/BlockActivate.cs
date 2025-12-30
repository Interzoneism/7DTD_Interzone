using System;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020000EE RID: 238
[Preserve]
public class BlockActivate : Block
{
	// Token: 0x1700006D RID: 109
	// (get) Token: 0x06000609 RID: 1545 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowBlockTriggers
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600060B RID: 1547 RVA: 0x0002BFF8 File Offset: 0x0002A1F8
	public override void Init()
	{
		base.Init();
		if (base.Properties.Values.ContainsKey("ActivateSound"))
		{
			this.activateSound = base.Properties.Values["ActivateSound"];
		}
	}

	// Token: 0x0600060C RID: 1548 RVA: 0x0002C034 File Offset: 0x0002A234
	public override void LateInit()
	{
		base.LateInit();
		if (base.Properties.Values.ContainsKey(BlockActivate.PropBlockChangeTo))
		{
			this.blockChangeTo = Block.GetBlockValue(base.Properties.Values[BlockActivate.PropBlockChangeTo], false);
			this.useChangeTo = true;
		}
	}

	// Token: 0x0600060D RID: 1549 RVA: 0x0002C086 File Offset: 0x0002A286
	public override void OnBlockValueChanged(WorldBase _world, Chunk _chunk, int _clrIdx, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
		base.OnBlockValueChanged(_world, _chunk, _clrIdx, _blockPos, _oldBlockValue, _newBlockValue);
	}

	// Token: 0x0600060E RID: 1550 RVA: 0x0002C098 File Offset: 0x0002A298
	public override string GetActivationText(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		PlayerActionsLocal playerInput = ((EntityPlayerLocal)_entityFocusing).playerInput;
		string arg = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
		string localizedBlockName = _blockValue.Block.GetLocalizedBlockName();
		return string.Format(Localization.Get("questBlockActivate", false), arg, localizedBlockName);
	}

	// Token: 0x0600060F RID: 1551 RVA: 0x0002C0F8 File Offset: 0x0002A2F8
	public override bool OnBlockActivated(string _commandName, WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (!(_commandName == "activate"))
		{
			if (_commandName == "trigger")
			{
				XUiC_TriggerProperties.Show(_player.PlayerUI.xui, _clrIdx, _blockPos, true, false);
			}
		}
		else if (!_world.IsEditor())
		{
			base.HandleTrigger(_player, (World)_world, _clrIdx, _blockPos, _blockValue);
			Manager.BroadcastPlay(_blockPos.ToVector3() + Vector3.one * 0.5f, this.activateSound, 0f);
			if (this.useChangeTo)
			{
				this.blockChangeTo.rotation = _blockValue.rotation;
			}
			return true;
		}
		return false;
	}

	// Token: 0x06000610 RID: 1552 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool HasBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return true;
	}

	// Token: 0x06000611 RID: 1553 RVA: 0x0002C19C File Offset: 0x0002A39C
	public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		((Chunk)_world.ChunkClusters[_clrIdx].GetChunkSync(World.toChunkXZ(_blockPos.x), _blockPos.y, World.toChunkXZ(_blockPos.z))).GetBlockTrigger(World.toBlock(_blockPos));
		this.cmds[0].enabled = true;
		this.cmds[1].enabled = (_world.IsEditor() && !GameUtils.IsWorldEditor());
		return this.cmds;
	}

	// Token: 0x0400076A RID: 1898
	[PublicizedFrom(EAccessModifier.Private)]
	public string activateSound;

	// Token: 0x0400076B RID: 1899
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockValue blockChangeTo = BlockValue.Air;

	// Token: 0x0400076C RID: 1900
	[PublicizedFrom(EAccessModifier.Private)]
	public bool useChangeTo;

	// Token: 0x0400076D RID: 1901
	[PublicizedFrom(EAccessModifier.Private)]
	public new BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("activate", "electric_switch", true, false, null),
		new BlockActivationCommand("trigger", "wrench", true, false, null)
	};

	// Token: 0x0400076E RID: 1902
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropBlockChangeTo = "BlockChangeTo";
}
