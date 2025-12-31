using System;
using UnityEngine;

// Token: 0x02000109 RID: 265
public class BlockProjectileMoveScript : ProjectileMoveScript
{
	// Token: 0x06000712 RID: 1810 RVA: 0x00031A74 File Offset: 0x0002FC74
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void checkCollision()
	{
		GameManager instance = GameManager.Instance;
		if (!instance)
		{
			return;
		}
		if (instance.World == null)
		{
			return;
		}
		Vector3 vector;
		if (this.isOnIdealPos)
		{
			vector = base.transform.position;
		}
		else
		{
			vector = this.idealPosition;
		}
		Vector3 direction = vector - this.previousPosition;
		float magnitude = direction.magnitude;
		if (magnitude < 0.04f)
		{
			return;
		}
		Ray ray = new Ray(this.previousPosition, direction);
		this.previousPosition = vector;
		int layerId = 0;
		EntityAlive entityAlive = (EntityAlive)this.firingEntity;
		if (entityAlive != null && entityAlive.emodel != null)
		{
			layerId = entityAlive.GetModelLayer();
			entityAlive.SetModelLayer(2, false, null);
		}
		this.hitMask = 32;
		bool flag = Voxel.Raycast(instance.World, ray, magnitude, -538750981, this.hitMask, 0f);
		if (entityAlive != null && entityAlive.emodel != null)
		{
			entityAlive.SetModelLayer(layerId, false, null);
		}
		if (flag && (GameUtils.IsBlockOrTerrain(Voxel.voxelRayHitInfo.tag) || Voxel.voxelRayHitInfo.tag.StartsWith("E_")))
		{
			base.enabled = false;
			UnityEngine.Object.Destroy(base.transform.gameObject);
			Transform transform = Voxel.voxelRayHitInfo.transform;
			string text = null;
			if (Voxel.voxelRayHitInfo.tag.StartsWith("E_BP_"))
			{
				text = Voxel.voxelRayHitInfo.tag.Substring("E_BP_".Length).ToLower();
				transform = GameUtils.GetHitRootTransform(Voxel.voxelRayHitInfo.tag, Voxel.voxelRayHitInfo.transform);
			}
			if (Voxel.voxelRayHitInfo.tag.StartsWith("E_"))
			{
				Entity component = transform.GetComponent<Entity>();
				if (component == null)
				{
					return;
				}
				DamageSourceEntity damageSourceEntity = new DamageSourceEntity(EnumDamageSource.External, EnumDamageTypes.Piercing, this.ProjectileOwnerID);
				damageSourceEntity.AttackingItem = this.itemValueProjectile;
				int strength = (int)this.GetProjectileDamageEntity();
				bool flag2 = component.IsDead();
				component.DamageEntity(damageSourceEntity, strength, false, 1f);
				if (this.itemActionProjectile.BuffActions != null && component is EntityAlive)
				{
					string context = (text != null) ? GameUtils.GetChildTransformPath(component.transform, Voxel.voxelRayHitInfo.transform) : null;
					ItemAction.ExecuteBuffActions(this.itemActionProjectile.BuffActions, -1, component as EntityAlive, false, damageSourceEntity.GetEntityDamageBodyPart(component), context);
				}
				if (!flag2 && component.IsDead())
				{
					EntityPlayer entityPlayer = instance.World.GetEntity(this.ProjectileOwnerID) as EntityPlayer;
					if (entityPlayer != null && EntityClass.list.ContainsKey(component.entityClass))
					{
						float value = EffectManager.GetValue(PassiveEffects.ElectricalTrapXP, entityPlayer.inventory.holdingItemItemValue, 0f, entityPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
						if (value > 0f)
						{
							entityPlayer.AddKillXP(component as EntityAlive, value);
						}
					}
				}
			}
			if (this.itemActionProjectile.Explosion.ParticleIndex > 0)
			{
				Vector3 vector2 = Voxel.voxelRayHitInfo.hit.pos - direction.normalized * 0.1f;
				Vector3i vector3i = World.worldToBlockPos(vector2);
				if (!instance.World.GetBlock(vector3i).isair)
				{
					BlockFace blockFace;
					vector3i = Voxel.OneVoxelStep(vector3i, vector2, -direction.normalized, out vector2, out blockFace);
				}
				instance.ExplosionServer(Voxel.voxelRayHitInfo.hit.clrIdx, vector2, vector3i, Quaternion.identity, this.itemActionProjectile.Explosion, this.ProjectileOwnerID, 0f, false, this.itemValueProjectile);
			}
		}
	}

	// Token: 0x06000713 RID: 1811 RVA: 0x00031E1E File Offset: 0x0003001E
	public float GetProjectileDamageEntity()
	{
		return this.itemActionProjectile.GetDamageEntity(this.itemValueProjectile, null, 0);
	}
}
