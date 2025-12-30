using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020000F5 RID: 245
[Preserve]
public class BlockCarExplode : Block
{
	// Token: 0x17000070 RID: 112
	// (get) Token: 0x06000640 RID: 1600 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowBlockTriggers
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000071 RID: 113
	// (get) Token: 0x06000642 RID: 1602 RVA: 0x0002CF04 File Offset: 0x0002B104
	public ExplosionData Explosion
	{
		get
		{
			return this.explosion;
		}
	}

	// Token: 0x06000643 RID: 1603 RVA: 0x0002CF0C File Offset: 0x0002B10C
	public override void Init()
	{
		base.Init();
		this.explosion = new ExplosionData(base.Properties, null);
	}

	// Token: 0x06000644 RID: 1604 RVA: 0x0002CF28 File Offset: 0x0002B128
	public override Block.DestroyedResult OnBlockDestroyedBy(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _entityId, bool _bUseHarvestTool)
	{
		Block.DestroyedResult result = base.OnBlockDestroyedBy(_world, _clrIdx, _blockPos, _blockValue, _entityId, _bUseHarvestTool);
		if (_blockValue.ischild)
		{
			return Block.DestroyedResult.Keep;
		}
		if (!_bUseHarvestTool)
		{
			this.explode(_blockPos);
			return Block.DestroyedResult.Remove;
		}
		return result;
	}

	// Token: 0x06000645 RID: 1605 RVA: 0x0002CF5E File Offset: 0x0002B15E
	public override Block.DestroyedResult OnBlockDestroyedByExplosion(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _playerThatStartedExpl)
	{
		base.OnBlockDestroyedByExplosion(_world, _clrIdx, _blockPos, _blockValue, _playerThatStartedExpl);
		if (_blockValue.ischild)
		{
			return Block.DestroyedResult.Keep;
		}
		this.explode(_blockPos);
		return Block.DestroyedResult.Remove;
	}

	// Token: 0x06000646 RID: 1606 RVA: 0x0002CF81 File Offset: 0x0002B181
	public override void OnBlockStartsToFall(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockStartsToFall(_world, _blockPos, _blockValue);
		this.explode(_blockPos);
	}

	// Token: 0x06000647 RID: 1607 RVA: 0x0002CF94 File Offset: 0x0002B194
	[PublicizedFrom(EAccessModifier.Private)]
	public void explode(Vector3i _blockPos)
	{
		if (this.explosion.ParticleIndex == 0)
		{
			return;
		}
		GameManager instance = GameManager.Instance;
		Quaternion rotation = Quaternion.identity;
		BlockEntityData blockEntity = ((Chunk)instance.World.GetChunkFromWorldPos(_blockPos)).GetBlockEntity(_blockPos);
		if (blockEntity != null && blockEntity.transform)
		{
			rotation = blockEntity.transform.rotation;
		}
		instance.ExplosionServer(0, _blockPos.ToVector3(), _blockPos, rotation, this.explosion, -1, 0.1f, false, null);
	}

	// Token: 0x06000648 RID: 1608 RVA: 0x0002D00B File Offset: 0x0002B20B
	public override void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, int _cIdx, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		base.OnBlockEntityTransformAfterActivated(_world, _blockPos, _cIdx, _blockValue, _ebcd);
	}

	// Token: 0x06000649 RID: 1609 RVA: 0x0002C086 File Offset: 0x0002A286
	public override void OnBlockValueChanged(WorldBase _world, Chunk _chunk, int _clrIdx, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
		base.OnBlockValueChanged(_world, _chunk, _clrIdx, _blockPos, _oldBlockValue, _newBlockValue);
	}

	// Token: 0x0600064A RID: 1610 RVA: 0x0002D01C File Offset: 0x0002B21C
	public override int OnBlockDamaged(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _damagePoints, int _entityIdThatDamaged, ItemActionAttack.AttackHitInfo _attackHitInfo, bool _bUseHarvestTool, bool _bBypassMaxDamage, int _recDepth = 0)
	{
		return base.OnBlockDamaged(_world, _clrIdx, _blockPos, _blockValue, _damagePoints, _entityIdThatDamaged, _attackHitInfo, _bUseHarvestTool, _bBypassMaxDamage, _recDepth);
	}

	// Token: 0x0600064B RID: 1611 RVA: 0x0002D040 File Offset: 0x0002B240
	public override BlockValue OnBlockPlaced(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, GameRandom _rnd)
	{
		_blockValue.rotation = (byte)(_rnd.RandomFloat * 4f);
		return _blockValue;
	}

	// Token: 0x0600064C RID: 1612 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool HasBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return true;
	}

	// Token: 0x0600064D RID: 1613 RVA: 0x0002D059 File Offset: 0x0002B259
	public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		this.cmds[0].enabled = (_world.IsEditor() && !GameUtils.IsWorldEditor());
		return this.cmds;
	}

	// Token: 0x0600064E RID: 1614 RVA: 0x0002D085 File Offset: 0x0002B285
	public override bool OnBlockActivated(string _commandName, WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (_commandName == "trigger")
		{
			XUiC_TriggerProperties.Show(_player.PlayerUI.xui, _cIdx, _blockPos, false, true);
		}
		return false;
	}

	// Token: 0x0600064F RID: 1615 RVA: 0x0002D0AB File Offset: 0x0002B2AB
	public override void OnTriggered(EntityPlayer _player, WorldBase _world, int cIdx, Vector3i _blockPos, BlockValue _blockValue, List<BlockChangeInfo> _blockChanges, BlockTrigger _triggeredBy)
	{
		base.OnTriggered(_player, _world, cIdx, _blockPos, _blockValue, _blockChanges, _triggeredBy);
		this.explode(_blockPos);
	}

	// Token: 0x04000780 RID: 1920
	[PublicizedFrom(EAccessModifier.Private)]
	public ExplosionData explosion;

	// Token: 0x04000781 RID: 1921
	[PublicizedFrom(EAccessModifier.Private)]
	public new BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("trigger", "wrench", true, false, null)
	};
}
