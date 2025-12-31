using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200053E RID: 1342
[Preserve]
public class ItemActionSpawnEntity : ItemAction
{
	// Token: 0x06002B61 RID: 11105 RVA: 0x0011FE8C File Offset: 0x0011E08C
	public override ItemActionData CreateModifierData(ItemInventoryData _invData, int _indexInEntityOfAction)
	{
		return new ItemActionSpawnEntity.ItemActionDataSpawnEntity(_invData, _indexInEntityOfAction);
	}

	// Token: 0x06002B62 RID: 11106 RVA: 0x0011FE98 File Offset: 0x0011E098
	public override void ReadFrom(DynamicProperties _props)
	{
		base.ReadFrom(_props);
		this.animType = _props.GetInt("AnimType");
		this.animWait = _props.GetFloat("AnimWait");
		this.soundWarn = _props.GetString("SoundWarn");
		this.soundAttack = _props.GetString("SoundAttack");
		this.entityToSpawn = _props.GetString("Entity");
		_props.ParseVec("EntityOffset", ref this.entityOffset);
	}

	// Token: 0x06002B63 RID: 11107 RVA: 0x00002914 File Offset: 0x00000B14
	public override void StartHolding(ItemActionData _actionData)
	{
	}

	// Token: 0x06002B64 RID: 11108 RVA: 0x00002914 File Offset: 0x00000B14
	public override void StopHolding(ItemActionData _actionData)
	{
	}

	// Token: 0x06002B65 RID: 11109 RVA: 0x0011FF14 File Offset: 0x0011E114
	public override void OnHoldingUpdate(ItemActionData _actionData)
	{
		ItemActionSpawnEntity.ItemActionDataSpawnEntity itemActionDataSpawnEntity = (ItemActionSpawnEntity.ItemActionDataSpawnEntity)_actionData;
		itemActionDataSpawnEntity.stateTime += 0.05f;
		ItemActionSpawnEntity.ItemActionDataSpawnEntity.State state = itemActionDataSpawnEntity.state;
		if (state != ItemActionSpawnEntity.ItemActionDataSpawnEntity.State.Anim)
		{
			if (state != ItemActionSpawnEntity.ItemActionDataSpawnEntity.State.Spawn)
			{
				return;
			}
			this.Spawn(itemActionDataSpawnEntity);
			itemActionDataSpawnEntity.state = ItemActionSpawnEntity.ItemActionDataSpawnEntity.State.End;
			return;
		}
		else
		{
			if (itemActionDataSpawnEntity.stateTime < this.animWait)
			{
				return;
			}
			itemActionDataSpawnEntity.state = ItemActionSpawnEntity.ItemActionDataSpawnEntity.State.Spawn;
			return;
		}
	}

	// Token: 0x06002B66 RID: 11110 RVA: 0x0011FF6F File Offset: 0x0011E16F
	public override void CancelAction(ItemActionData _actionData)
	{
		((ItemActionSpawnEntity.ItemActionDataSpawnEntity)_actionData).state = ItemActionSpawnEntity.ItemActionDataSpawnEntity.State.None;
	}

	// Token: 0x06002B67 RID: 11111 RVA: 0x0011FF80 File Offset: 0x0011E180
	public override void ExecuteAction(ItemActionData _actionData, bool _bReleased)
	{
		EntityAlive holdingEntity = _actionData.invData.holdingEntity;
		if (!holdingEntity)
		{
			return;
		}
		ItemActionSpawnEntity.ItemActionDataSpawnEntity itemActionDataSpawnEntity = (ItemActionSpawnEntity.ItemActionDataSpawnEntity)_actionData;
		if (!_bReleased)
		{
			if (itemActionDataSpawnEntity.state == ItemActionSpawnEntity.ItemActionDataSpawnEntity.State.None)
			{
				itemActionDataSpawnEntity.state = ItemActionSpawnEntity.ItemActionDataSpawnEntity.State.Anim;
				itemActionDataSpawnEntity.stateTime = 0f;
				holdingEntity.StartAnimAction(this.animType + 3000);
				holdingEntity.PlayOneShot(this.soundWarn, false, false, false, null);
			}
			return;
		}
		itemActionDataSpawnEntity.state = ItemActionSpawnEntity.ItemActionDataSpawnEntity.State.None;
	}

	// Token: 0x06002B68 RID: 11112 RVA: 0x0011FFF0 File Offset: 0x0011E1F0
	public override bool IsActionRunning(ItemActionData _actionData)
	{
		return ((ItemActionSpawnEntity.ItemActionDataSpawnEntity)_actionData).state > ItemActionSpawnEntity.ItemActionDataSpawnEntity.State.None;
	}

	// Token: 0x06002B69 RID: 11113 RVA: 0x00120000 File Offset: 0x0011E200
	[PublicizedFrom(EAccessModifier.Private)]
	public void Spawn(ItemActionData _actionData)
	{
		EntityAlive holdingEntity = _actionData.invData.holdingEntity;
		if (!holdingEntity)
		{
			return;
		}
		if (holdingEntity.IsAttackValid())
		{
			Vector3 vector = holdingEntity.getHeadPosition();
			vector += holdingEntity.qrotation * this.entityOffset;
			Entity entity = EntityFactory.CreateEntity(EntityClass.GetId(this.entityToSpawn), vector, new Vector3(0f, holdingEntity.rotation.y, 0f));
			entity.SetSpawnerSource(EnumSpawnerSource.StaticSpawner);
			GameManager.Instance.World.SpawnEntityInWorld(entity);
			EntityAlive entityAlive = entity as EntityAlive;
			if (entityAlive != null)
			{
				entityAlive.SetAttackTarget(holdingEntity.GetAttackTarget(), 600);
			}
			holdingEntity.PlayOneShot(this.soundAttack, false, false, false, null);
		}
	}

	// Token: 0x040021D8 RID: 8664
	[PublicizedFrom(EAccessModifier.Private)]
	public int animType;

	// Token: 0x040021D9 RID: 8665
	[PublicizedFrom(EAccessModifier.Private)]
	public float animWait;

	// Token: 0x040021DA RID: 8666
	[PublicizedFrom(EAccessModifier.Private)]
	public string soundWarn;

	// Token: 0x040021DB RID: 8667
	[PublicizedFrom(EAccessModifier.Private)]
	public string soundAttack;

	// Token: 0x040021DC RID: 8668
	[PublicizedFrom(EAccessModifier.Private)]
	public string entityToSpawn;

	// Token: 0x040021DD RID: 8669
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 entityOffset;

	// Token: 0x0200053F RID: 1343
	[PublicizedFrom(EAccessModifier.Protected)]
	public class ItemActionDataSpawnEntity : ItemActionAttackData
	{
		// Token: 0x06002B6A RID: 11114 RVA: 0x00112618 File Offset: 0x00110818
		public ItemActionDataSpawnEntity(ItemInventoryData _invData, int _indexInEntityOfAction) : base(_invData, _indexInEntityOfAction)
		{
		}

		// Token: 0x040021DE RID: 8670
		public ItemActionSpawnEntity.ItemActionDataSpawnEntity.State state;

		// Token: 0x040021DF RID: 8671
		public float stateTime;

		// Token: 0x02000540 RID: 1344
		public enum State
		{
			// Token: 0x040021E1 RID: 8673
			None,
			// Token: 0x040021E2 RID: 8674
			Anim,
			// Token: 0x040021E3 RID: 8675
			Spawn,
			// Token: 0x040021E4 RID: 8676
			End
		}
	}
}
