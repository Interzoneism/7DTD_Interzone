using System;
using System.Collections.Generic;
using Audio;
using UnityEngine;

// Token: 0x0200038C RID: 908
public class SpinningBladeTrapBladeController : MonoBehaviour
{
	// Token: 0x06001B0F RID: 6927 RVA: 0x000A9900 File Offset: 0x000A7B00
	public void Init(DynamicProperties _properties, Block _block)
	{
		this.entityDamage = 20f;
		if (_block.Damage > 0f)
		{
			this.entityDamage = _block.Damage;
		}
		this.selfDamage = 0.1f;
		_properties.ParseFloat("DamageReceived", ref this.selfDamage);
		_properties.ParseString("ImpactSound", ref this.bladeImpactSound);
		this.brokenPercentage = 0.25f;
		_properties.ParseFloat("BrokenPercentage", ref this.brokenPercentage);
		this.brokenPercentage = Mathf.Clamp01(this.brokenPercentage);
		this.blockDamage = 0f;
		if (_properties.Values.ContainsKey("Buff"))
		{
			this.buffActions = new List<string>();
			string[] collection = _properties.Values["Buff"].Replace(" ", "").Split(',', StringSplitOptions.None);
			this.buffActions.AddRange(collection);
		}
	}

	// Token: 0x06001B10 RID: 6928 RVA: 0x000A99E7 File Offset: 0x000A7BE7
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnTriggerEnter(Collider other)
	{
		if (this.CollidersThisFrame == null)
		{
			this.CollidersThisFrame = new List<Collider>();
		}
		if (!this.CollidersThisFrame.Contains(other))
		{
			this.CollidersThisFrame.Add(other);
		}
	}

	// Token: 0x06001B11 RID: 6929 RVA: 0x000A9A16 File Offset: 0x000A7C16
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnTriggerExit(Collider other)
	{
		if (this.CollidersThisFrame == null)
		{
			this.CollidersThisFrame = new List<Collider>();
		}
		if (!this.CollidersThisFrame.Contains(other))
		{
			this.CollidersThisFrame.Remove(other);
		}
	}

	// Token: 0x06001B12 RID: 6930 RVA: 0x000A9A48 File Offset: 0x000A7C48
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return;
		}
		if (this.entityHitCount == null)
		{
			this.entityHitCount = new Dictionary<int, float>();
		}
		if (!this.IsOn)
		{
			this.entityHitCount.Clear();
			return;
		}
		if (this.controller.HealthRatio <= this.brokenPercentage)
		{
			return;
		}
		this.entityHitList.Clear();
		GameManager.Instance.World.GetEntitiesInBounds(typeof(EntityAlive), new Bounds(base.transform.position + Origin.position, new Vector3(3f, 3f, 3f)), this.entityHitList);
		if (this.entityHitList.Count == 0)
		{
			return;
		}
		DamageMultiplier damageMultiplier = new DamageMultiplier();
		bool flag = false;
		Vector3 vector = this.BladeCenter.position + Origin.position + new Vector3(0f, 0.2f, 0f);
		for (int i = 0; i < this.Blades.Length; i++)
		{
			Vector3 direction = this.Blades[i].position + Origin.position - vector;
			Ray ray = new Ray(vector, direction);
			Voxel.Raycast(GameManager.Instance.World, ray, 1.24f, -538750981, 128, 0.1f);
			WorldRayHitInfo hitInfo = Voxel.voxelRayHitInfo.Clone();
			EntityAlive entityFromCollider = this.GetEntityFromCollider(Voxel.voxelRayHitInfo.hitCollider);
			if (entityFromCollider != null && entityFromCollider.IsAlive())
			{
				bool flag2;
				if (this.entityHitCount.ContainsKey(entityFromCollider.entityId))
				{
					Dictionary<int, float> dictionary = this.entityHitCount;
					int entityId = entityFromCollider.entityId;
					dictionary[entityId] += Time.deltaTime;
					flag2 = (this.entityHitCount[entityFromCollider.entityId] >= this.entityHitTime);
					if (flag2)
					{
						this.entityHitCount[entityFromCollider.entityId] = 0f;
					}
				}
				else
				{
					this.entityHitCount.Add(entityFromCollider.entityId, 0f);
					flag2 = true;
				}
				if (flag2)
				{
					flag = true;
					ItemActionAttack.Hit(hitInfo, (this.OwnerTE.OwnerEntityID == entityFromCollider.entityId) ? -1 : this.OwnerTE.OwnerEntityID, EnumDamageTypes.Slashing, this.blockDamage, this.entityDamage, 1f, 1f, 0f, 0.05f, "metal", damageMultiplier, this.buffActions, new ItemActionAttack.AttackHitInfo(), 3, 0, 0f, null, null, ItemActionAttack.EnumAttackMode.RealNoHarvesting, null, -2, this.OwnerTE.blockValue.ToItemValue());
				}
			}
		}
		if (flag)
		{
			this.controller.DamageSelf(this.selfDamage);
			Manager.BroadcastPlay(this.controller.BlockPosition.ToVector3(), this.bladeImpactSound, 0f);
		}
	}

	// Token: 0x06001B13 RID: 6931 RVA: 0x000A9D24 File Offset: 0x000A7F24
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityAlive GetEntityFromCollider(Collider collider)
	{
		if (collider == null || collider.transform == null)
		{
			return null;
		}
		EntityAlive entityAlive = collider.transform.GetComponent<EntityAlive>();
		if (entityAlive == null)
		{
			entityAlive = collider.transform.GetComponentInParent<EntityAlive>();
		}
		if (entityAlive == null && collider.transform.parent != null)
		{
			entityAlive = collider.transform.parent.GetComponentInChildren<EntityAlive>();
		}
		if (entityAlive == null)
		{
			entityAlive = collider.transform.GetComponentInChildren<EntityAlive>();
		}
		return entityAlive;
	}

	// Token: 0x040011E6 RID: 4582
	public SpinningBladeTrapController controller;

	// Token: 0x040011E7 RID: 4583
	public Transform[] Blades;

	// Token: 0x040011E8 RID: 4584
	public Transform BladeCenter;

	// Token: 0x040011E9 RID: 4585
	public bool IsOn;

	// Token: 0x040011EA RID: 4586
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string bladeImpactSound = "Electricity/BladeTrap/bladetrap_impact";

	// Token: 0x040011EB RID: 4587
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float entityDamage;

	// Token: 0x040011EC RID: 4588
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float blockDamage;

	// Token: 0x040011ED RID: 4589
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float selfDamage;

	// Token: 0x040011EE RID: 4590
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float brokenPercentage;

	// Token: 0x040011EF RID: 4591
	public TileEntityPoweredMeleeTrap OwnerTE;

	// Token: 0x040011F0 RID: 4592
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<string> buffActions;

	// Token: 0x040011F1 RID: 4593
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<Collider> CollidersThisFrame;

	// Token: 0x040011F2 RID: 4594
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Dictionary<int, float> entityHitCount;

	// Token: 0x040011F3 RID: 4595
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float entityHitTime = 0.05f;

	// Token: 0x040011F4 RID: 4596
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<Entity> entityHitList = new List<Entity>();
}
