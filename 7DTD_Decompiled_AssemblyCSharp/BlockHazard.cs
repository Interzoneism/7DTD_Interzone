using System;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000104 RID: 260
[Preserve]
public class BlockHazard : BlockParticle
{
	// Token: 0x17000076 RID: 118
	// (get) Token: 0x060006D3 RID: 1747 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowBlockTriggers
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060006D5 RID: 1749 RVA: 0x0002FFD8 File Offset: 0x0002E1D8
	public override void Init()
	{
		base.Init();
		if (base.Properties.Values.ContainsKey(BlockHazard.PropDamageBuffs))
		{
			if (this.buffActions == null)
			{
				this.buffActions = new List<string>();
			}
			string[] array = base.Properties.Values[BlockHazard.PropDamageBuffs].Split(',', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				this.buffActions.Add(array[i]);
			}
		}
		base.Properties.ParseVec(BlockHazard.PropDamageOffset, ref this.DamageOffset);
		base.Properties.ParseVec(BlockHazard.PropDamageSize, ref this.DamageSize);
		if (base.Properties.Values.ContainsKey(BlockHazard.PropSecondaryBuffs))
		{
			if (this.buffSecondaryActions == null)
			{
				this.buffSecondaryActions = new List<string>();
			}
			string[] array2 = base.Properties.Values[BlockHazard.PropSecondaryBuffs].Split(',', StringSplitOptions.None);
			for (int j = 0; j < array2.Length; j++)
			{
				this.buffSecondaryActions.Add(array2[j]);
			}
		}
		base.Properties.ParseVec(BlockHazard.PropSecondaryOffset, ref this.SecondaryOffset);
		base.Properties.ParseVec(BlockHazard.PropSecondarySize, ref this.SecondarySize);
		if (base.Properties.Values.ContainsKey("Model"))
		{
			DataLoader.PreloadBundle(base.Properties.Values["Model"]);
		}
		base.Properties.ParseString(BlockHazard.PropStartSound, ref this.StartSound);
		base.Properties.ParseString(BlockHazard.PropStopSound, ref this.StopSound);
	}

	// Token: 0x060006D6 RID: 1750 RVA: 0x00030168 File Offset: 0x0002E368
	public override byte GetLightValue(BlockValue _blockValue)
	{
		if ((_blockValue.meta & 2) == 0)
		{
			return 0;
		}
		return base.GetLightValue(_blockValue);
	}

	// Token: 0x060006D7 RID: 1751 RVA: 0x00030180 File Offset: 0x0002E380
	public override string GetActivationText(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		if (!_world.IsEditor())
		{
			return null;
		}
		PlayerActionsLocal playerInput = ((EntityPlayerLocal)_entityFocusing).playerInput;
		string arg = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
		if ((_blockValue.meta & 2) != 0)
		{
			return string.Format(Localization.Get("useSwitchLightOff", false), arg);
		}
		return string.Format(Localization.Get("useSwitchLightOn", false), arg);
	}

	// Token: 0x060006D8 RID: 1752 RVA: 0x000301FC File Offset: 0x0002E3FC
	public override bool OnBlockActivated(string _commandName, WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (!(_commandName == "light"))
		{
			if (_commandName == "trigger")
			{
				XUiC_TriggerProperties.Show(_player.PlayerUI.xui, _cIdx, _blockPos, false, true);
			}
		}
		else if (_world.IsEditor() && this.toggleHazardStateForEditor(_world, _cIdx, _blockPos, _blockValue))
		{
			return true;
		}
		return false;
	}

	// Token: 0x060006D9 RID: 1753 RVA: 0x00030254 File Offset: 0x0002E454
	[PublicizedFrom(EAccessModifier.Private)]
	public bool toggleHazardStateForEditor(WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		bool flag = (_blockValue.meta & 2) > 0;
		flag = !flag;
		_blockValue.meta = (byte)(((int)_blockValue.meta & -3) | (flag ? 2 : 0));
		_blockValue.meta = (byte)(((int)_blockValue.meta & -2) | (flag ? 1 : 0));
		_world.SetBlockRPC(_cIdx, _blockPos, _blockValue);
		return true;
	}

	// Token: 0x060006DA RID: 1754 RVA: 0x000302B4 File Offset: 0x0002E4B4
	public bool IsHazardOn(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (this.isMultiBlock && _blockValue.ischild)
		{
			Vector3i parentPos = this.multiBlockPos.GetParentPos(_blockPos, _blockValue);
			BlockValue block = _world.GetBlock(parentPos);
			return this.IsHazardOn(_world, parentPos, block);
		}
		return (_blockValue.meta & 2) > 0;
	}

	// Token: 0x060006DB RID: 1755 RVA: 0x00030300 File Offset: 0x0002E500
	public bool OriginalHazardState(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (this.isMultiBlock && _blockValue.ischild)
		{
			Vector3i parentPos = this.multiBlockPos.GetParentPos(_blockPos, _blockValue);
			BlockValue block = _world.GetBlock(parentPos);
			return this.IsHazardOn(_world, parentPos, block);
		}
		return (_blockValue.meta & 1) > 0;
	}

	// Token: 0x060006DC RID: 1756 RVA: 0x0003034A File Offset: 0x0002E54A
	public BlockValue SetHazardState(BlockValue _blockValue, bool isOn)
	{
		_blockValue.meta = (byte)(((int)_blockValue.meta & -3) | (isOn ? 2 : 0));
		return _blockValue;
	}

	// Token: 0x060006DD RID: 1757 RVA: 0x00030368 File Offset: 0x0002E568
	public override void OnBlockValueChanged(WorldBase _world, Chunk _chunk, int _clrIdx, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
		if (_newBlockValue.ischild)
		{
			return;
		}
		this.IsHazardOn(_world, _blockPos, _newBlockValue);
		this.OriginalHazardState(_world, _blockPos, _newBlockValue);
		base.OnBlockValueChanged(_world, _chunk, _clrIdx, _blockPos, _oldBlockValue, _newBlockValue);
		this.updateHazardState(_world, _chunk, _clrIdx, _blockPos, _newBlockValue);
		this.checkParticles(_world, _clrIdx, _blockPos, _newBlockValue);
	}

	// Token: 0x060006DE RID: 1758 RVA: 0x000303C0 File Offset: 0x0002E5C0
	[PublicizedFrom(EAccessModifier.Private)]
	public bool updateHazardState(WorldBase _world, Chunk _chunk, int _cIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		IChunk chunk = _chunk;
		if (chunk == null)
		{
			ChunkCluster chunkCluster = _world.ChunkClusters[_cIdx];
			if (chunkCluster == null)
			{
				return false;
			}
			chunk = chunkCluster.GetChunkSync(World.toChunkXZ(_blockPos.x), World.toChunkY(_blockPos.y), World.toChunkXZ(_blockPos.z));
			if (chunk == null)
			{
				return false;
			}
		}
		if (chunk == null)
		{
			return false;
		}
		BlockEntityData blockEntity = chunk.GetBlockEntity(_blockPos);
		if (blockEntity == null || !blockEntity.bHasTransform)
		{
			return false;
		}
		Transform transform = blockEntity.transform.Find("HazardDamage");
		if (transform == null)
		{
			GameObject gameObject = new GameObject("HazardDamage");
			gameObject.AddComponent<HazardDamageController>();
			transform = gameObject.transform;
			gameObject.AddComponent<BoxCollider>().isTrigger = true;
			transform.SetParent(blockEntity.transform);
		}
		transform.GetComponent<BoxCollider>().size = this.DamageSize;
		transform.localPosition = this.DamageOffset;
		transform.localRotation = Quaternion.identity;
		HazardDamageController component = transform.GetComponent<HazardDamageController>();
		if (component)
		{
			component.IsActive = this.IsHazardOn(_world, _blockPos, _blockValue);
			component.buffActions = this.buffActions;
		}
		if (this.buffSecondaryActions != null && this.buffSecondaryActions.Count != 0)
		{
			transform = blockEntity.transform.Find("SecondaryDamage");
			if (transform == null)
			{
				GameObject gameObject2 = new GameObject("SecondaryDamage");
				gameObject2.AddComponent<HazardDamageController>();
				transform = gameObject2.transform;
				gameObject2.AddComponent<BoxCollider>().isTrigger = true;
				transform.SetParent(blockEntity.transform);
			}
			transform.GetComponent<BoxCollider>().size = this.SecondarySize;
			transform.localPosition = this.SecondaryOffset;
			transform.localRotation = Quaternion.identity;
			component = transform.GetComponent<HazardDamageController>();
			if (component)
			{
				component.IsActive = this.IsHazardOn(_world, _blockPos, _blockValue);
				component.buffActions = this.buffSecondaryActions;
			}
		}
		return true;
	}

	// Token: 0x060006DF RID: 1759 RVA: 0x00030588 File Offset: 0x0002E788
	public override void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, int _cIdx, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		if (_blockValue.ischild)
		{
			return;
		}
		base.OnBlockEntityTransformAfterActivated(_world, _blockPos, _cIdx, _blockValue, _ebcd);
		this.updateHazardState(_world, null, _cIdx, _blockPos, _blockValue);
		this.checkParticles(_world, _cIdx, _blockPos, _blockValue);
	}

	// Token: 0x060006E0 RID: 1760 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool HasBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return true;
	}

	// Token: 0x060006E1 RID: 1761 RVA: 0x000305BC File Offset: 0x0002E7BC
	public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		this.cmds[0].enabled = _world.IsEditor();
		this.cmds[1].enabled = (_world.IsEditor() && !GameUtils.IsWorldEditor());
		return this.cmds;
	}

	// Token: 0x060006E2 RID: 1762 RVA: 0x0003060C File Offset: 0x0002E80C
	public override void OnBlockReset(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (_blockValue.ischild)
		{
			return;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			bool flag = this.IsHazardOn(_world, _blockPos, _blockValue);
			bool flag2 = this.OriginalHazardState(_world, _blockPos, _blockValue);
			if (flag2 != flag)
			{
				_blockValue = this.SetHazardState(_blockValue, flag2);
				_world.SetBlockRPC(_chunk.ClrIdx, _blockPos, _blockValue);
			}
		}
	}

	// Token: 0x060006E3 RID: 1763 RVA: 0x00030664 File Offset: 0x0002E864
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void checkParticles(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (_blockValue.ischild)
		{
			return;
		}
		bool flag = _world.GetGameManager().HasBlockParticleEffect(_blockPos);
		if (this.IsHazardOn(_world, _blockPos, _blockValue) && !flag)
		{
			this.addParticles(_world, _clrIdx, _blockPos.x, _blockPos.y, _blockPos.z, _blockValue);
			return;
		}
		if (!this.IsHazardOn(_world, _blockPos, _blockValue) && flag)
		{
			this.removeParticles(_world, _blockPos.x, _blockPos.y, _blockPos.z, _blockValue);
		}
	}

	// Token: 0x060006E4 RID: 1764 RVA: 0x000306E0 File Offset: 0x0002E8E0
	public override void OnTriggered(EntityPlayer _player, WorldBase _world, int cIdx, Vector3i _blockPos, BlockValue _blockValue, List<BlockChangeInfo> _blockChanges, BlockTrigger _triggeredBy)
	{
		base.OnTriggered(_player, _world, cIdx, _blockPos, _blockValue, _blockChanges, _triggeredBy);
		bool flag = !this.IsHazardOn(_world, _blockPos, _blockValue);
		if (flag)
		{
			Manager.BroadcastPlayByLocalPlayer(_blockPos.ToVector3() + Vector3.one * 0.5f, this.StartSound);
		}
		else
		{
			Manager.BroadcastPlayByLocalPlayer(_blockPos.ToVector3() + Vector3.one * 0.5f, this.StopSound);
		}
		_blockValue = this.SetHazardState(_blockValue, flag);
		_blockChanges.Add(new BlockChangeInfo(cIdx, _blockPos, _blockValue));
	}

	// Token: 0x060006E5 RID: 1765 RVA: 0x0003077C File Offset: 0x0002E97C
	public override int OnBlockDamaged(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _damagePoints, int _entityIdThatDamaged, ItemActionAttack.AttackHitInfo _attackHitInfo, bool _bUseHarvestTool, bool _bBypassMaxDamage, int _recDepth = 0)
	{
		if (this.IsHazardOn(_world, _blockPos, _blockValue) && _damagePoints > 0 && this.buffActions != null && this.buffActions.Count > 0)
		{
			EntityAlive entityAlive = _world.GetEntity(_entityIdThatDamaged) as EntityAlive;
			if (entityAlive != null && entityAlive as EntityTurret == null)
			{
				ItemAction itemAction = entityAlive.inventory.holdingItemData.item.Actions[0];
				if (entityAlive != null)
				{
					if (itemAction is ItemActionRanged)
					{
						ItemActionRanged itemActionRanged = itemAction as ItemActionRanged;
						if (itemActionRanged == null || (itemActionRanged.Hitmask & 128) == 0)
						{
							goto IL_D2;
						}
					}
					for (int i = 0; i < this.buffActions.Count; i++)
					{
						entityAlive.Buffs.AddBuff(this.buffActions[i], _blockPos, entityAlive.entityId, true, false, -1f);
					}
				}
			}
		}
		IL_D2:
		return base.OnBlockDamaged(_world, _clrIdx, _blockPos, _blockValue, _damagePoints, _entityIdThatDamaged, _attackHitInfo, _bUseHarvestTool, _bBypassMaxDamage, _recDepth);
	}

	// Token: 0x040007C9 RID: 1993
	public const int cMetaOriginalState = 1;

	// Token: 0x040007CA RID: 1994
	public const int cMetaOn = 2;

	// Token: 0x040007CB RID: 1995
	public Vector3 DamageOffset = Vector3.zero;

	// Token: 0x040007CC RID: 1996
	public Vector3 DamageSize = Vector3.one;

	// Token: 0x040007CD RID: 1997
	public Vector3 SecondaryOffset = Vector3.zero;

	// Token: 0x040007CE RID: 1998
	public Vector3 SecondarySize = Vector3.one;

	// Token: 0x040007CF RID: 1999
	[PublicizedFrom(EAccessModifier.Private)]
	public List<string> buffActions;

	// Token: 0x040007D0 RID: 2000
	[PublicizedFrom(EAccessModifier.Private)]
	public List<string> buffSecondaryActions;

	// Token: 0x040007D1 RID: 2001
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropDamageOffset = "DamageOffset";

	// Token: 0x040007D2 RID: 2002
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropDamageSize = "DamageSize";

	// Token: 0x040007D3 RID: 2003
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropDamageBuffs = "DamageBuffs";

	// Token: 0x040007D4 RID: 2004
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropSecondaryOffset = "SecondaryOffset";

	// Token: 0x040007D5 RID: 2005
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropSecondarySize = "SecondarySize";

	// Token: 0x040007D6 RID: 2006
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropSecondaryBuffs = "SecondaryBuffs";

	// Token: 0x040007D7 RID: 2007
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropStartSound = "StartSound";

	// Token: 0x040007D8 RID: 2008
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropStopSound = "StopSound";

	// Token: 0x040007D9 RID: 2009
	[PublicizedFrom(EAccessModifier.Protected)]
	public string StartSound;

	// Token: 0x040007DA RID: 2010
	[PublicizedFrom(EAccessModifier.Protected)]
	public string StopSound;

	// Token: 0x040007DB RID: 2011
	[PublicizedFrom(EAccessModifier.Private)]
	public new BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("light", "electric_switch", true, false, null),
		new BlockActivationCommand("trigger", "wrench", true, false, null)
	};
}
