using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020000F6 RID: 246
[Preserve]
public class BlockCarExplodeLoot : BlockLoot
{
	// Token: 0x17000072 RID: 114
	// (get) Token: 0x06000650 RID: 1616 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowBlockTriggers
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000073 RID: 115
	// (get) Token: 0x06000652 RID: 1618 RVA: 0x0002D119 File Offset: 0x0002B319
	public ExplosionData Explosion
	{
		get
		{
			return this.explosion;
		}
	}

	// Token: 0x06000653 RID: 1619 RVA: 0x0002D121 File Offset: 0x0002B321
	public override void Init()
	{
		base.Init();
		this.explosion = new ExplosionData(base.Properties, null);
	}

	// Token: 0x06000654 RID: 1620 RVA: 0x0002D13C File Offset: 0x0002B33C
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

	// Token: 0x06000655 RID: 1621 RVA: 0x0002D172 File Offset: 0x0002B372
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

	// Token: 0x06000656 RID: 1622 RVA: 0x0002D195 File Offset: 0x0002B395
	public override void OnBlockStartsToFall(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockStartsToFall(_world, _blockPos, _blockValue);
		this.explode(_blockPos);
	}

	// Token: 0x06000657 RID: 1623 RVA: 0x0002D1A8 File Offset: 0x0002B3A8
	[PublicizedFrom(EAccessModifier.Private)]
	public void explode(Vector3i _blockPos)
	{
		if (this.explosion.ParticleIndex == 0)
		{
			return;
		}
		GameManager instance = GameManager.Instance;
		if (!instance)
		{
			return;
		}
		Chunk chunk = (Chunk)instance.World.GetChunkFromWorldPos(_blockPos);
		if (chunk == null)
		{
			return;
		}
		Vector3 worldPos = _blockPos.ToVector3();
		Quaternion rotation = Quaternion.identity;
		BlockEntityData blockEntity = chunk.GetBlockEntity(_blockPos);
		if (blockEntity != null && blockEntity.transform)
		{
			worldPos = blockEntity.transform.position + Origin.position;
			rotation = blockEntity.transform.rotation;
		}
		instance.ExplosionServer(0, worldPos, _blockPos, rotation, this.explosion, -1, 0.1f, false, null);
	}

	// Token: 0x06000658 RID: 1624 RVA: 0x0002D00B File Offset: 0x0002B20B
	public override void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, int _cIdx, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		base.OnBlockEntityTransformAfterActivated(_world, _blockPos, _cIdx, _blockValue, _ebcd);
	}

	// Token: 0x06000659 RID: 1625 RVA: 0x0002C086 File Offset: 0x0002A286
	public override void OnBlockValueChanged(WorldBase _world, Chunk _chunk, int _clrIdx, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
		base.OnBlockValueChanged(_world, _chunk, _clrIdx, _blockPos, _oldBlockValue, _newBlockValue);
	}

	// Token: 0x0600065A RID: 1626 RVA: 0x0002D250 File Offset: 0x0002B450
	public override int OnBlockDamaged(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _damagePoints, int _entityIdThatDamaged, ItemActionAttack.AttackHitInfo _attackHitInfo, bool _bUseHarvestTool, bool _bBypassMaxDamage, int _recDepth = 0)
	{
		return base.OnBlockDamaged(_world, _clrIdx, _blockPos, _blockValue, _damagePoints, _entityIdThatDamaged, _attackHitInfo, _bUseHarvestTool, _bBypassMaxDamage, _recDepth);
	}

	// Token: 0x0600065B RID: 1627 RVA: 0x0002D040 File Offset: 0x0002B240
	public override BlockValue OnBlockPlaced(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, GameRandom _rnd)
	{
		_blockValue.rotation = (byte)(_rnd.RandomFloat * 4f);
		return _blockValue;
	}

	// Token: 0x0600065C RID: 1628 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool HasBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return true;
	}

	// Token: 0x0600065D RID: 1629 RVA: 0x0002D274 File Offset: 0x0002B474
	public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		this.cmds[0].enabled = true;
		this.cmds[1].enabled = (_world.IsEditor() && !GameUtils.IsWorldEditor());
		return this.cmds;
	}

	// Token: 0x0600065E RID: 1630 RVA: 0x0002D2B4 File Offset: 0x0002B4B4
	public override bool OnBlockActivated(string _commandName, WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (!(_commandName == "Search"))
		{
			if (_commandName == "trigger")
			{
				XUiC_TriggerProperties.Show(_player.PlayerUI.xui, _cIdx, _blockPos, false, true);
			}
			return false;
		}
		return this.OnBlockActivated(_world, _cIdx, _blockPos, _blockValue, _player);
	}

	// Token: 0x0600065F RID: 1631 RVA: 0x0002D303 File Offset: 0x0002B503
	public override void OnTriggered(EntityPlayer _player, WorldBase _world, int cIdx, Vector3i _blockPos, BlockValue _blockValue, List<BlockChangeInfo> _blockChanges, BlockTrigger _triggeredBy)
	{
		base.OnTriggered(_player, _world, cIdx, _blockPos, _blockValue, _blockChanges, _triggeredBy);
		this.explode(_blockPos);
	}

	// Token: 0x04000782 RID: 1922
	[PublicizedFrom(EAccessModifier.Private)]
	public ExplosionData explosion;

	// Token: 0x04000783 RID: 1923
	[PublicizedFrom(EAccessModifier.Private)]
	public new BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("Search", "search", true, false, null),
		new BlockActivationCommand("trigger", "wrench", true, false, null)
	};
}
