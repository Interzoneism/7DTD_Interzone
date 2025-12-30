using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000101 RID: 257
[Preserve]
public class BlockGameEvent : Block
{
	// Token: 0x17000075 RID: 117
	// (get) Token: 0x060006BF RID: 1727 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowBlockTriggers
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060006C1 RID: 1729 RVA: 0x0002F970 File Offset: 0x0002DB70
	public override void Init()
	{
		base.Init();
		base.Properties.ParseString(BlockGameEvent.PropOnActivateEvent, ref this.onActivateEvent);
		base.Properties.ParseString(BlockGameEvent.PropOnDamageEvent, ref this.onDamageEvent);
		base.Properties.ParseString(BlockGameEvent.PropOnTriggeredEvent, ref this.onTriggeredEvent);
		base.Properties.ParseString(BlockGameEvent.PropOnAddedEvent, ref this.onAddedEvent);
		base.Properties.ParseBool(BlockGameEvent.PropDestroyOnEvent, ref this.destroyOnEvent);
		base.Properties.ParseBool(BlockGameEvent.PropSendDamageUpdate, ref this.sendDamageUpdate);
	}

	// Token: 0x060006C2 RID: 1730 RVA: 0x0002FA08 File Offset: 0x0002DC08
	public override string GetActivationText(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		if (this.onActivateEvent == "" && !_world.IsEditor())
		{
			return "";
		}
		PlayerActionsLocal playerInput = ((EntityPlayerLocal)_entityFocusing).playerInput;
		string arg = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
		string localizedBlockName = _blockValue.Block.GetLocalizedBlockName();
		return string.Format(Localization.Get("questBlockActivate", false), arg, localizedBlockName);
	}

	// Token: 0x060006C3 RID: 1731 RVA: 0x0002FA88 File Offset: 0x0002DC88
	public override void OnBlockAdded(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, PlatformUserIdentifierAbs _addedByPlayer)
	{
		base.OnBlockAdded(_world, _chunk, _blockPos, _blockValue, _addedByPlayer);
		GameEventManager.Current.HandleAction(this.onAddedEvent, null, null, false, _blockPos, "", "", false, true, "", null);
		if (this.destroyOnEvent)
		{
			this.DamageBlock(_world, 0, _blockPos, _blockValue, _blockValue.Block.MaxDamage - _blockValue.damage, -1, null, false, false);
		}
	}

	// Token: 0x060006C4 RID: 1732 RVA: 0x0002FAFC File Offset: 0x0002DCFC
	public override bool OnBlockActivated(string _commandName, WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (_commandName == "activate")
		{
			if (this.onActivateEvent != "")
			{
				GameEventManager.Current.HandleAction(this.onActivateEvent, null, _player, false, _blockPos, "", "", false, true, "", null);
				if (this.destroyOnEvent)
				{
					this.DamageBlock(_world, _cIdx, _blockPos, _blockValue, _blockValue.Block.MaxDamage - _blockValue.damage, -1, null, false, false);
				}
			}
			return true;
		}
		if (!(_commandName == "trigger"))
		{
			return false;
		}
		XUiC_TriggerProperties.Show(_player.PlayerUI.xui, _cIdx, _blockPos, false, true);
		return true;
	}

	// Token: 0x060006C5 RID: 1733 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool HasBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return true;
	}

	// Token: 0x060006C6 RID: 1734 RVA: 0x0002FBB0 File Offset: 0x0002DDB0
	public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		this.cmds[0].enabled = (!_world.IsEditor() && this.onActivateEvent != "");
		this.cmds[1].enabled = (_world.IsEditor() && !GameUtils.IsWorldEditor());
		return this.cmds;
	}

	// Token: 0x060006C7 RID: 1735 RVA: 0x0002FC14 File Offset: 0x0002DE14
	public override int OnBlockDamaged(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _damagePoints, int _entityIdThatDamaged, ItemActionAttack.AttackHitInfo _attackHitInfo, bool _bUseHarvestTool, bool _bBypassMaxDamage, int _recDepth = 0)
	{
		if (this.onDamageEvent != "")
		{
			if (GameEventManager.Current.GetTargetType(this.onDamageEvent) != GameEventActionSequence.TargetTypes.Block)
			{
				Debug.LogError("Game Event Target Type must be set to 'Block' to be used in BlockGameEvent.");
			}
			else
			{
				Entity entity = _world.GetEntity(_entityIdThatDamaged);
				EntityVehicle entityVehicle = entity as EntityVehicle;
				if (entityVehicle != null)
				{
					entity = entityVehicle.GetFirstAttached();
				}
				GameEventManager.Current.HandleAction(this.onDamageEvent, null, entity as EntityPlayer, false, _blockPos, "", "", false, true, "", null);
				if (this.destroyOnEvent)
				{
					this.DamageBlock(_world, _clrIdx, _blockPos, _blockValue, _blockValue.Block.MaxDamage - _blockValue.damage, -1, null, false, false);
				}
			}
		}
		int num = base.OnBlockDamaged(_world, _clrIdx, _blockPos, _blockValue, _damagePoints, _entityIdThatDamaged, _attackHitInfo, _bUseHarvestTool, _bBypassMaxDamage, _recDepth);
		if (num > 0 && this.sendDamageUpdate && GameManager.Instance.World.GetEntity(_entityIdThatDamaged) is EntityPlayer)
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				GameEventManager.Current.SendBlockDamageUpdate(_blockPos);
				return num;
			}
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageGameEventResponse>().Setup(NetPackageGameEventResponse.ResponseTypes.BlockDamaged, _blockPos), false);
		}
		return num;
	}

	// Token: 0x060006C8 RID: 1736 RVA: 0x0002FD38 File Offset: 0x0002DF38
	public override void OnTriggered(EntityPlayer _player, WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, List<BlockChangeInfo> _blockChanges, BlockTrigger _triggeredBy)
	{
		base.OnTriggered(_player, _world, _cIdx, _blockPos, _blockValue, _blockChanges, _triggeredBy);
		if (this.onTriggeredEvent != "")
		{
			if (GameEventManager.Current.GetTargetType(this.onTriggeredEvent) != GameEventActionSequence.TargetTypes.Block)
			{
				Debug.LogError("Game Event Target Type must be set to 'Block' to be used in BlockGameEvent.");
				return;
			}
			GameEventManager.Current.HandleAction(this.onTriggeredEvent, null, _player, false, _blockPos, "", "", false, true, "", null);
			if (this.destroyOnEvent)
			{
				this.DamageBlock(_world, _cIdx, _blockPos, _blockValue, _blockValue.Block.MaxDamage - _blockValue.damage, -1, null, false, false);
			}
		}
	}

	// Token: 0x040007BA RID: 1978
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropOnActivateEvent = "ActivateEvent";

	// Token: 0x040007BB RID: 1979
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropOnDamageEvent = "DamageEvent";

	// Token: 0x040007BC RID: 1980
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropOnTriggeredEvent = "TriggeredEvent";

	// Token: 0x040007BD RID: 1981
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropOnAddedEvent = "AddedEvent";

	// Token: 0x040007BE RID: 1982
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropDestroyOnEvent = "DestroyOnEvent";

	// Token: 0x040007BF RID: 1983
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropSendDamageUpdate = "SendDamageUpdate";

	// Token: 0x040007C0 RID: 1984
	[PublicizedFrom(EAccessModifier.Protected)]
	public string onActivateEvent = "";

	// Token: 0x040007C1 RID: 1985
	[PublicizedFrom(EAccessModifier.Protected)]
	public string onDamageEvent = "";

	// Token: 0x040007C2 RID: 1986
	[PublicizedFrom(EAccessModifier.Protected)]
	public string onTriggeredEvent = "";

	// Token: 0x040007C3 RID: 1987
	[PublicizedFrom(EAccessModifier.Protected)]
	public string onAddedEvent = "";

	// Token: 0x040007C4 RID: 1988
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool destroyOnEvent;

	// Token: 0x040007C5 RID: 1989
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool sendDamageUpdate;

	// Token: 0x040007C6 RID: 1990
	[PublicizedFrom(EAccessModifier.Private)]
	public new BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("activate", "electric_switch", false, false, null),
		new BlockActivationCommand("trigger", "wrench", true, false, null)
	};
}
