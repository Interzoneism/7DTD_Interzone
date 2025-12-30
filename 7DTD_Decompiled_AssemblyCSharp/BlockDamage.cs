using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020000F8 RID: 248
[Preserve]
public class BlockDamage : Block
{
	// Token: 0x06000669 RID: 1641 RVA: 0x0002D632 File Offset: 0x0002B832
	public BlockDamage()
	{
		this.IsCheckCollideWithEntity = true;
	}

	// Token: 0x0600066A RID: 1642 RVA: 0x0002D648 File Offset: 0x0002B848
	public override void Init()
	{
		base.Init();
		if (base.Properties.Values.ContainsKey(Block.PropDamage))
		{
			int.TryParse(base.Properties.Values[Block.PropDamage], out this.damage);
		}
		else
		{
			Log.Error("Block " + base.GetBlockName() + " is a BlockDamage but does not specify a damage value");
			this.damage = 0;
		}
		if (base.Properties.Values.ContainsKey(BlockDamage.PropDamageReceived))
		{
			int.TryParse(base.Properties.Values[BlockDamage.PropDamageReceived], out this.damageReceived);
		}
		else
		{
			this.damageReceived = 0;
		}
		base.Properties.ParseEnum<EnumDamageTypes>(BlockDamage.PropDamageType, ref this.damageType);
	}

	// Token: 0x0600066B RID: 1643 RVA: 0x0002D710 File Offset: 0x0002B910
	public override void GetCollisionAABB(BlockValue _blockValue, int _x, int _y, int _z, float _distortedY, List<Bounds> _result)
	{
		base.GetCollisionAABB(_blockValue, _x, _y, _z, _distortedY, _result);
		Vector3 b = new Vector3(0.05f, 0.05f, 0.05f);
		for (int i = 0; i < _result.Count; i++)
		{
			Bounds value = _result[i];
			value.SetMinMax(value.min - b, value.max + b);
			_result[i] = value;
		}
	}

	// Token: 0x0600066C RID: 1644 RVA: 0x0002D787 File Offset: 0x0002B987
	public override IList<Bounds> GetClipBoundsList(BlockValue _blockValue, Vector3 _blockPos)
	{
		Block.staticList_IntersectRayWithBlockList.Clear();
		this.GetCollisionAABB(_blockValue, (int)_blockPos.x, (int)_blockPos.y, (int)_blockPos.z, 0f, Block.staticList_IntersectRayWithBlockList);
		return Block.staticList_IntersectRayWithBlockList;
	}

	// Token: 0x0600066D RID: 1645 RVA: 0x0002D7C0 File Offset: 0x0002B9C0
	public override bool OnEntityCollidedWithBlock(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, Entity _targetEntity)
	{
		if (!(_targetEntity is EntityAlive))
		{
			return false;
		}
		EntityAlive entityAlive = (EntityAlive)_targetEntity;
		if (entityAlive.IsDead())
		{
			return false;
		}
		DamageSourceEntity damageSourceEntity = new DamageSourceEntity(EnumDamageSource.External, this.damageType, -1);
		damageSourceEntity.AttackingItem = _blockValue.ToItemValue();
		damageSourceEntity.BlockPosition = _blockPos;
		damageSourceEntity.SetIgnoreConsecutiveDamages(true);
		bool flag;
		if (entityAlive is EntityHuman)
		{
			damageSourceEntity.hitTransformName = entityAlive.emodel.GetHitTransform(BodyPrimaryHit.Torso).name;
			flag = (_targetEntity.DamageEntity(damageSourceEntity, this.damage, false, 1f) > 0);
		}
		else
		{
			flag = (_targetEntity.DamageEntity(damageSourceEntity, this.damage, false, 1f) > 0);
		}
		bool bBypassMaxDamage = false;
		int num = entityAlive.CalculateBlockDamage(this, this.damageReceived, out bBypassMaxDamage);
		if (this.MovementFactor != 1f)
		{
			entityAlive.SetMotionMultiplier(EffectManager.GetValue(PassiveEffects.MovementFactorMultiplier, null, this.MovementFactor, entityAlive, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false));
		}
		if (flag && num > 0 && !((World)_world).IsWithinTraderArea(_blockPos))
		{
			this.DamageBlock(_world, _clrIdx, _blockPos, _blockValue, num, (_targetEntity != null) ? _targetEntity.entityId : -1, null, false, bBypassMaxDamage);
		}
		return flag;
	}

	// Token: 0x04000789 RID: 1929
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropDamageReceived = "Damage_received";

	// Token: 0x0400078A RID: 1930
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropDamageType = "DamageType";

	// Token: 0x0400078B RID: 1931
	[PublicizedFrom(EAccessModifier.Protected)]
	public int damage;

	// Token: 0x0400078C RID: 1932
	[PublicizedFrom(EAccessModifier.Protected)]
	public int damageReceived;

	// Token: 0x0400078D RID: 1933
	[PublicizedFrom(EAccessModifier.Private)]
	public EnumDamageTypes damageType = EnumDamageTypes.Piercing;
}
