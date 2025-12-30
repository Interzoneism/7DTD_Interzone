using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000149 RID: 329
[Preserve]
public class BlockTrapDoor : Block
{
	// Token: 0x17000082 RID: 130
	// (get) Token: 0x06000926 RID: 2342 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowBlockTriggers
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000927 RID: 2343 RVA: 0x0003F760 File Offset: 0x0003D960
	public BlockTrapDoor()
	{
		this.IsCheckCollideWithEntity = true;
	}

	// Token: 0x06000928 RID: 2344 RVA: 0x0003F7B8 File Offset: 0x0003D9B8
	public override void Init()
	{
		base.Init();
		if (base.Properties.Values.ContainsKey(BlockTrapDoor.PropTriggerDelay))
		{
			this.TriggerDelay = StringParsers.ParseFloat(base.Properties.Values[BlockTrapDoor.PropTriggerDelay], 0, -1, NumberStyles.Any);
		}
		if (base.Properties.Values.ContainsKey(BlockTrapDoor.PropTriggerSound))
		{
			this.TriggerSound = base.Properties.Values[BlockTrapDoor.PropTriggerSound];
		}
	}

	// Token: 0x06000929 RID: 2345 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool HasBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return true;
	}

	// Token: 0x0600092A RID: 2346 RVA: 0x0003F83B File Offset: 0x0003DA3B
	public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		this.cmds[0].enabled = (_world.IsEditor() && !GameUtils.IsWorldEditor());
		return this.cmds;
	}

	// Token: 0x0600092B RID: 2347 RVA: 0x0003F867 File Offset: 0x0003DA67
	public override bool OnBlockActivated(string _commandName, WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		XUiC_TriggerProperties.Show(_player.PlayerUI.xui, _cIdx, _blockPos, true, true);
		return true;
	}

	// Token: 0x0600092C RID: 2348 RVA: 0x0003F880 File Offset: 0x0003DA80
	public override bool OnEntityCollidedWithBlock(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, Entity _targetEntity)
	{
		if (_targetEntity as EntityAlive == null)
		{
			return false;
		}
		EntityAlive entityAlive = (EntityAlive)_targetEntity;
		if (entityAlive.IsDead())
		{
			return false;
		}
		if (_blockValue.meta == 1)
		{
			return false;
		}
		Vector3 vector = _blockPos.ToVector3() + new Vector3(0.5f, 0f, 0.5f);
		vector.y = 0f;
		Vector3 position = entityAlive.position;
		position.y = 0f;
		float num = Utils.FastAbs(vector.x - position.x);
		float num2 = Utils.FastAbs(vector.z - position.z);
		float num3 = (_blockValue.Block != null && _blockValue.Block.isMultiBlock) ? ((float)_blockValue.Block.multiBlockPos.dim.x / 2f * 0.45f) : 0.45f;
		if (num > num3 || num2 > num3)
		{
			return false;
		}
		_blockValue.meta = 1;
		_world.SetBlockRPC(_blockPos, _blockValue);
		float value = EffectManager.GetValue(PassiveEffects.TrapDoorTriggerDelay, null, this.TriggerDelay, _targetEntity as EntityAlive, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		if (value > 0f)
		{
			GameManager.Instance.PlaySoundAtPositionServer(_blockPos.ToVector3(), this.TriggerSound, AudioRolloffMode.Linear, 5, _targetEntity.entityId);
			GameManager.Instance.StartCoroutine(this.damageBlock(value, _world, _clrIdx, _blockPos, _blockValue, _targetEntity as EntityPlayer, _targetEntity.entityId));
		}
		return true;
	}

	// Token: 0x0600092D RID: 2349 RVA: 0x0003FA00 File Offset: 0x0003DC00
	public override Block.DestroyedResult OnBlockDestroyedBy(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _entityId, bool _bUseHarvestTool)
	{
		EntityPlayer entityPlayer = ((World)_world).GetEntity(_entityId) as EntityPlayer;
		if (entityPlayer != null)
		{
			base.HandleTrigger(entityPlayer, (World)_world, _clrIdx, _blockPos, _blockValue);
		}
		return base.OnBlockDestroyedBy(_world, _clrIdx, _blockPos, _blockValue, _entityId, _bUseHarvestTool);
	}

	// Token: 0x0600092E RID: 2350 RVA: 0x0003FA44 File Offset: 0x0003DC44
	public override Block.DestroyedResult OnBlockDestroyedByExplosion(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _playerThatStartedExpl)
	{
		EntityPlayer entityPlayer = ((World)_world).GetEntity(_playerThatStartedExpl) as EntityPlayer;
		if (entityPlayer != null)
		{
			base.HandleTrigger(entityPlayer, (World)_world, _clrIdx, _blockPos, _blockValue);
		}
		return base.OnBlockDestroyedByExplosion(_world, _clrIdx, _blockPos, _blockValue, _playerThatStartedExpl);
	}

	// Token: 0x0600092F RID: 2351 RVA: 0x0003FA88 File Offset: 0x0003DC88
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator damageBlock(float time, WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, EntityPlayer _player, int _entityId)
	{
		yield return new WaitForSeconds(time);
		if (_player != null)
		{
			base.HandleTrigger(_player, (World)_world, _clrIdx, _blockPos, _blockValue);
		}
		this.DamageBlock(_world, _clrIdx, _blockPos, _blockValue, _blockValue.Block.MaxDamage - _blockValue.damage, _entityId, null, false, false);
		yield break;
	}

	// Token: 0x06000930 RID: 2352 RVA: 0x0003FAD8 File Offset: 0x0003DCD8
	public override void OnTriggered(EntityPlayer _player, WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, List<BlockChangeInfo> _blockChanges, BlockTrigger _triggeredBy)
	{
		base.OnTriggered(_player, _world, _cIdx, _blockPos, _blockValue, _blockChanges, _triggeredBy);
		this.DamageBlock(_world, _cIdx, _blockPos, _blockValue, _blockValue.Block.MaxDamage - _blockValue.damage, -1, null, false, false);
	}

	// Token: 0x040008CB RID: 2251
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropTriggerDelay = "TriggerDelay";

	// Token: 0x040008CC RID: 2252
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropTriggerSound = "TriggerSound";

	// Token: 0x040008CD RID: 2253
	[PublicizedFrom(EAccessModifier.Private)]
	public new BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("trigger", "wrench", true, false, null)
	};

	// Token: 0x040008CE RID: 2254
	[PublicizedFrom(EAccessModifier.Private)]
	public float TriggerDelay = 0.6f;

	// Token: 0x040008CF RID: 2255
	[PublicizedFrom(EAccessModifier.Private)]
	public string TriggerSound = "trapdoor_trigger";
}
