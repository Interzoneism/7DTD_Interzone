using System;
using System.Collections.Generic;
using System.Globalization;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200044A RID: 1098
[Preserve]
public class EntityItem : Entity
{
	// Token: 0x170003B9 RID: 953
	// (get) Token: 0x06002283 RID: 8835 RVA: 0x000282C0 File Offset: 0x000264C0
	public override Entity.EnumPositionUpdateMovementType positionUpdateMovementType
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return Entity.EnumPositionUpdateMovementType.Instant;
		}
	}

	// Token: 0x06002284 RID: 8836 RVA: 0x000D9B50 File Offset: 0x000D7D50
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		this.usePhysicsMaster = true;
		this.isPhysicsMaster = SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer;
		base.Awake();
		this.yOffset = 0.15f;
		EntityItem.ItemInstanceCount++;
		Collider component = base.GetComponent<Collider>();
		if (component)
		{
			component.enabled = false;
		}
	}

	// Token: 0x06002285 RID: 8837 RVA: 0x000D9BA8 File Offset: 0x000D7DA8
	[PublicizedFrom(EAccessModifier.Protected)]
	public ~EntityItem()
	{
		EntityItem.ItemInstanceCount--;
	}

	// Token: 0x06002286 RID: 8838 RVA: 0x000D9BDC File Offset: 0x000D7DDC
	public override void Init(int _entityClass)
	{
		base.Init(_entityClass);
		this.itemRB = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06002287 RID: 8839 RVA: 0x000D9BF4 File Offset: 0x000D7DF4
	public override void PostInit()
	{
		base.PostInit();
		base.PhysicsSetRB(this.itemRB);
		base.transform.eulerAngles = this.rotation;
		if (this.itemClass != null)
		{
			this.stickPercent = this.itemClass.Properties.GetFloat("StickPercent");
			if (this.itemStack != null)
			{
				this.itemWorldData = this.itemClass.CreateWorldData(GameManager.Instance, this, this.itemStack.itemValue, this.belongsPlayerId);
			}
		}
	}

	// Token: 0x06002288 RID: 8840 RVA: 0x000D9C78 File Offset: 0x000D7E78
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void HandleNavObject()
	{
		if (!this.bWasThrown)
		{
			return;
		}
		if (this.itemClass != null && this.OwnerId != -1 && this.world.GetEntity(this.OwnerId) as EntityPlayerLocal != null && this.itemClass.NavObject != "")
		{
			this.NavObject = NavObjectManager.Instance.RegisterNavObject(this.itemClass.NavObject, base.transform, "", false);
		}
	}

	// Token: 0x06002289 RID: 8841 RVA: 0x000D9CFC File Offset: 0x000D7EFC
	public void SetItemStack(ItemStack _itemStack)
	{
		if (this.itemStack == null)
		{
			this.itemStack = ItemStack.Empty.Clone();
		}
		this.lastCachedItemStack = this.itemStack.Clone();
		this.itemStack = _itemStack;
		this.itemClass = ItemClass.GetForId(this.itemStack.itemValue.type);
		this.distractionRadiusSq = EffectManager.GetValue(PassiveEffects.DistractionRadius, this.itemStack.itemValue, 0f, null, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		this.distractionRadiusSq *= this.distractionRadiusSq;
		this.distractionLifetime = Mathf.FloorToInt(EffectManager.GetValue(PassiveEffects.DistractionLifetime, this.itemStack.itemValue, 0f, null, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false));
		this.distractionEatTicks = Mathf.FloorToInt(EffectManager.GetValue(PassiveEffects.DistractionEatTicks, this.itemStack.itemValue, 0f, null, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false));
		this.distractionStrength = EffectManager.GetValue(PassiveEffects.DistractionStrength, this.itemStack.itemValue, 0f, null, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
	}

	// Token: 0x0600228A RID: 8842 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Private)]
	public new void FixedUpdate()
	{
	}

	// Token: 0x0600228B RID: 8843 RVA: 0x000D9E34 File Offset: 0x000D8034
	public override void OnUpdateEntity()
	{
		base.OnUpdateEntity();
		if (!this.bMeshCreated)
		{
			this.createMesh();
		}
		if (this.itemWorldData != null)
		{
			this.itemClass.OnDroppedUpdate(this.itemWorldData);
		}
		if (Utils.FastAbs(this.position.y - this.prevPos.y) < 0.1f)
		{
			this.onGroundCounter++;
			if (this.onGroundCounter > 10)
			{
				this.onGround = true;
			}
		}
		if (this.isPhysicsMaster && !SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && (this.ticksExisted & 1) != 0)
		{
			base.PhysicsMasterSendToServer(base.transform);
		}
		this.checkGravitySetting(this.isPhysicsMaster);
		if (this.isEntityRemote)
		{
			return;
		}
		if (!this.itemTransform)
		{
			this.lifetime = 0f;
		}
		this.lifetime -= 0.05f;
		if (this.lifetime <= 0f)
		{
			this.SetDead();
		}
		if (this.itemClass != null && this.itemClass.IsEatDistraction && this.distractionLifetime > 0 && this.distractionEatTicks <= 0)
		{
			this.SetDead();
		}
		if (base.transform.position.y + Origin.position.y < 0f)
		{
			this.SetDead();
		}
		if (!this.IsDead())
		{
			this.tickDistraction();
		}
	}

	// Token: 0x0600228C RID: 8844 RVA: 0x000D9F8F File Offset: 0x000D818F
	[PublicizedFrom(EAccessModifier.Private)]
	public void playThrowSound(string _name)
	{
		Manager.Play(this, "throw" + _name, 1f, false);
		Manager.Play(this, "throwdefault", 1f, false);
	}

	// Token: 0x0600228D RID: 8845 RVA: 0x000D9FBB File Offset: 0x000D81BB
	public override int DamageEntity(DamageSource _damageSource, int _strength, bool _criticalHit, float impulseScale = 1f)
	{
		if (_strength >= 99999)
		{
			this.lifetime = 0f;
		}
		return base.DamageEntity(_damageSource, _strength, _criticalHit, impulseScale);
	}

	// Token: 0x0600228E RID: 8846 RVA: 0x000D9FDC File Offset: 0x000D81DC
	public override void OnDamagedByExplosion()
	{
		if (this.itemWorldData != null)
		{
			ItemClass forId = ItemClass.GetForId(this.itemStack.itemValue.type);
			if (forId != null)
			{
				forId.OnDamagedByExplosion(this.itemWorldData);
			}
		}
	}

	// Token: 0x0600228F RID: 8847 RVA: 0x000DA018 File Offset: 0x000D8218
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void createMesh()
	{
		if (this.itemStack.itemValue.type == 0 || this.itemClass == null)
		{
			Log.Error(string.Format("Could not create item with id {0}", this.itemStack.itemValue.type));
			this.SetDead();
			return;
		}
		if (this.meshGameObject != null && this.lastCachedItemStack.itemValue.type != 0)
		{
			UnityEngine.Object.Destroy(this.meshGameObject);
			this.meshGameObject = null;
		}
		this.itemTransform = null;
		float num = 0f;
		Vector3 zero = Vector3.zero;
		if (this.itemClass.IsBlock())
		{
			BlockValue blockValue = this.itemStack.itemValue.ToBlockValue();
			if (this.itemTransform == null)
			{
				this.itemTransform = this.itemClass.CloneModel(this.meshGameObject, this.world, blockValue, null, Vector3.zero, base.transform, BlockShape.MeshPurpose.Drop, default(TextureFullArray));
			}
			Block block = blockValue.Block;
			if (block.Properties.Values.ContainsKey("DropScale"))
			{
				num = StringParsers.ParseFloat(block.Properties.Values["DropScale"], 0, -1, NumberStyles.Any);
			}
		}
		else
		{
			if (this.itemTransform == null)
			{
				this.itemTransform = this.itemClass.CloneModel(this.world, this.itemStack.itemValue, Vector3.zero, base.transform, BlockShape.MeshPurpose.Drop, default(TextureFullArray));
			}
			if (this.itemClass.Properties.Values.ContainsKey("DropScale"))
			{
				num = StringParsers.ParseFloat(this.itemClass.Properties.Values["DropScale"], 0, -1, NumberStyles.Any);
			}
		}
		if (num != 0f)
		{
			this.itemTransform.localScale = new Vector3(num, num, num);
		}
		this.itemTransform.localEulerAngles = this.itemClass.GetDroppedCorrectionRotation();
		this.itemTransform.localPosition = zero;
		bool enabled = true;
		Collider[] componentsInChildren = this.itemTransform.GetComponentsInChildren<Collider>();
		int num2 = 0;
		while (componentsInChildren != null && num2 < componentsInChildren.Length)
		{
			Collider collider = componentsInChildren[num2];
			Rigidbody component = collider.gameObject.GetComponent<Rigidbody>();
			if ((component && component.isKinematic) || (collider is MeshCollider && !((MeshCollider)collider).convex))
			{
				collider.enabled = false;
			}
			else
			{
				collider.gameObject.layer = 13;
				collider.enabled = true;
				enabled = false;
				collider.gameObject.AddMissingComponent<RootTransformRefEntity>();
			}
			num2++;
		}
		base.transform.GetComponent<Collider>().enabled = enabled;
		this.meshGameObject = this.itemTransform.gameObject;
		this.meshGameObject.SetActive(true);
		if (this.itemWorldData != null)
		{
			this.itemClass.OnMeshCreated(this.itemWorldData);
		}
		this.bMeshCreated = true;
		this.meshRenderers = this.itemTransform.GetComponentsInChildren<Renderer>(true);
		this.VisiblityCheck(0f, false);
	}

	// Token: 0x06002290 RID: 8848 RVA: 0x000DA324 File Offset: 0x000D8524
	[PublicizedFrom(EAccessModifier.Private)]
	public void checkGravitySetting(bool hasGravity)
	{
		bool flag = hasGravity && !this.stickT;
		if (this.useGravity != flag)
		{
			this.useGravity = flag;
			Rigidbody rigidbody = this.itemRB;
			if (flag)
			{
				rigidbody.useGravity = true;
				rigidbody.isKinematic = false;
				rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
				return;
			}
			rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
			rigidbody.useGravity = false;
			rigidbody.isKinematic = true;
		}
	}

	// Token: 0x06002291 RID: 8849 RVA: 0x000DA38C File Offset: 0x000D858C
	public override void AddVelocity(Vector3 _vel)
	{
		this.bWasThrown = true;
		base.SetAirBorne(true);
		if (!this.bMeshCreated)
		{
			this.createMesh();
		}
		this.checkGravitySetting(this.isPhysicsMaster);
		if (this.isPhysicsMaster)
		{
			this.itemRB.angularVelocity = this.rand.RandomOnUnitSphere * (1f + _vel.magnitude * this.rand.RandomFloat * 8f);
			this.itemRB.AddForce(_vel * 6f, ForceMode.Impulse);
		}
	}

	// Token: 0x06002292 RID: 8850 RVA: 0x000DA41C File Offset: 0x000D861C
	public override void VisiblityCheck(float _distanceSqr, bool _masterIsZooming)
	{
		if (this.itemTransform && this.meshRenderers != null)
		{
			bool enabled = _distanceSqr < (float)(_masterIsZooming ? 8100 : 3600);
			if (this.ticksExisted < 3 && _distanceSqr < 0.64000005f)
			{
				enabled = false;
			}
			for (int i = 0; i < this.meshRenderers.Length; i++)
			{
				this.meshRenderers[i].enabled = enabled;
			}
		}
	}

	// Token: 0x06002293 RID: 8851 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool CanCollideWith(Entity _other)
	{
		return true;
	}

	// Token: 0x06002294 RID: 8852 RVA: 0x000DA486 File Offset: 0x000D8686
	public virtual bool CanCollect()
	{
		return this.itemClass != null && this.itemClass.CanCollect(this.itemStack.itemValue);
	}

	// Token: 0x06002295 RID: 8853 RVA: 0x000DA4A8 File Offset: 0x000D86A8
	public override void OnLoadedFromEntityCache(EntityCreationData _ecd)
	{
		this.markedForUnload = false;
		base.transform.name = "Item_" + _ecd.id.ToString();
		this.SetItemStack(_ecd.itemStack);
		this.bMeshCreated = false;
		if (this.meshRenderers != null)
		{
			for (int i = 0; i < this.meshRenderers.Length; i++)
			{
				this.meshRenderers[i].enabled = false;
			}
		}
		UpdateLightOnChunkMesh component;
		if (this.meshGameObject != null && (component = this.meshGameObject.GetComponent<UpdateLightOnChunkMesh>()) != null)
		{
			component.Reset();
		}
		this.itemWorldData = null;
		this.bDead = false;
		this.motion = Vector3.zero;
		this.addedToChunk = false;
		this.fallDistance = 0f;
	}

	// Token: 0x06002296 RID: 8854 RVA: 0x000DA56E File Offset: 0x000D876E
	public override Transform GetModelTransform()
	{
		return this.itemTransform;
	}

	// Token: 0x06002297 RID: 8855 RVA: 0x000DA576 File Offset: 0x000D8776
	public override void PhysicsMasterBecome()
	{
		this.checkGravitySetting(true);
		base.PhysicsMasterBecome();
	}

	// Token: 0x06002298 RID: 8856 RVA: 0x000DA588 File Offset: 0x000D8788
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateTransform()
	{
		if (!this.isPhysicsMaster)
		{
			float deltaTime = Time.deltaTime;
			base.transform.position = Vector3.Lerp(base.transform.position, this.position - Origin.position, deltaTime * 7f);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this.qrotation, deltaTime * 3f);
			return;
		}
		Vector3 position = base.transform.position;
		if (!float.IsNaN(position.x) && !float.IsNaN(position.y) && !float.IsNaN(position.z) && !float.IsInfinity(position.x) && !float.IsInfinity(position.y) && !float.IsInfinity(position.z))
		{
			this.SetPosition(position + Origin.position, true);
		}
		Quaternion rotation = base.transform.rotation;
		Vector3 eulerAngles = rotation.eulerAngles;
		if (!float.IsNaN(eulerAngles.x) && !float.IsNaN(eulerAngles.y) && !float.IsNaN(eulerAngles.z) && !float.IsInfinity(eulerAngles.x) && !float.IsInfinity(eulerAngles.y) && !float.IsInfinity(eulerAngles.z))
		{
			this.SetRotation(eulerAngles);
			this.qrotation = rotation;
		}
	}

	// Token: 0x06002299 RID: 8857 RVA: 0x000DA6DC File Offset: 0x000D88DC
	[PublicizedFrom(EAccessModifier.Private)]
	public void LateUpdate()
	{
		if (this.stickT)
		{
			Vector3 position = this.stickT.TransformPoint(this.stickRelativePos);
			base.transform.position = position;
			base.transform.rotation = this.stickT.rotation * this.stickRot;
		}
	}

	// Token: 0x0600229A RID: 8858 RVA: 0x000DA738 File Offset: 0x000D8938
	[PublicizedFrom(EAccessModifier.Private)]
	public void tickDistraction()
	{
		if (this.itemClass == null || this.distractionLifetime <= 0)
		{
			return;
		}
		if ((this.isCollided || !this.itemClass.IsRequireContactDistraction) && this.distractionRadiusSq > 0f)
		{
			int num = this.nextDistractionTick + 1;
			this.nextDistractionTick = num;
			if (num > 20)
			{
				this.nextDistractionTick = 0;
				Vector3 position = this.position;
				Bounds bb = new Bounds(position, new Vector3(this.distractionRadiusSq, this.distractionRadiusSq, this.distractionRadiusSq));
				this.world.GetEntitiesInBounds(typeof(EntityAlive), bb, EntityItem.distractionTargets);
				for (int i = 0; i < EntityItem.distractionTargets.Count; i++)
				{
					EntityAlive entityAlive = (EntityAlive)EntityItem.distractionTargets[i];
					if (!entityAlive.IsSleeping && entityAlive.distraction == null)
					{
						EntityClass entityClass = EntityClass.list[entityAlive.entityClass];
						if (this.itemClass.DistractionTags.IsEmpty || this.itemClass.DistractionTags.Test_AnySet(entityClass.Tags))
						{
							float distanceSq = base.GetDistanceSq(entityAlive);
							if (distanceSq <= this.distractionRadiusSq && (entityAlive.pendingDistraction == null || distanceSq < entityAlive.pendingDistractionDistanceSq))
							{
								float num2 = entityAlive.distractionResistance - this.distractionStrength;
								if (num2 <= 0f || num2 < this.rand.RandomFloat * 100f)
								{
									entityAlive.pendingDistraction = this;
									entityAlive.pendingDistractionDistanceSq = distanceSq;
								}
							}
						}
					}
				}
				EntityItem.distractionTargets.Clear();
				if (this.distractionLifetime > 0)
				{
					this.distractionLifetime--;
				}
			}
		}
	}

	// Token: 0x0600229B RID: 8859 RVA: 0x000DA8F8 File Offset: 0x000D8AF8
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnCollisionEnter(Collision collision)
	{
		if (!this.CanCollide(collision))
		{
			return;
		}
		this.CheckStick(collision);
		if (!this.isCollided)
		{
			this.isCollided = true;
			if (this.isPhysicsMaster && !SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				base.PhysicsMasterSendToServer(base.transform);
			}
		}
		if (this.contactPoints == null)
		{
			this.contactPoints = new List<ContactPoint>(2);
		}
		for (int i = collision.GetContacts(this.contactPoints) - 1; i >= 0; i--)
		{
			ContactPoint contactPoint = this.contactPoints[i];
			if (Utils.FastAbs(Vector3.Dot(collision.relativeVelocity, contactPoint.normal)) >= 1f)
			{
				Entity hitRootEntity = GameUtils.GetHitRootEntity(contactPoint.otherCollider.transform.tag, contactPoint.otherCollider.transform);
				if (hitRootEntity != null)
				{
					string text = EntityClass.list[hitRootEntity.entityClass].Properties.Values["SurfaceCategory"];
					if (!string.IsNullOrEmpty(text) && this.itemClass != null && this.itemClass.MadeOfMaterial != null)
					{
						this.playThrowSound(this.itemClass.MadeOfMaterial.id + "hit" + text);
						return;
					}
					break;
				}
				else
				{
					Vector3i pos = World.worldToBlockPos(contactPoint.point - 0.25f * contactPoint.normal + Origin.position);
					BlockValue block = this.world.GetBlock(pos);
					if (block.isair)
					{
						WorldRayHitInfo worldRayHitInfo = new WorldRayHitInfo();
						GameUtils.FindMasterBlockForEntityModelBlock(this.world, -contactPoint.normal, contactPoint.otherCollider.transform.tag, contactPoint.point + Origin.position, contactPoint.otherCollider.transform, worldRayHitInfo);
						pos = worldRayHitInfo.hit.blockPos;
						block = this.world.GetBlock(pos);
						if (block.isair)
						{
							goto IL_300;
						}
					}
					float num = Utils.FastAbs(contactPoint.normal.x);
					float num2 = Utils.FastAbs(contactPoint.normal.y);
					float num3 = Utils.FastAbs(contactPoint.normal.z);
					BlockFace side = BlockFace.Top;
					if (num >= num2 && num >= num3)
					{
						if (contactPoint.normal.x < 0f)
						{
							side = BlockFace.East;
						}
						else if (contactPoint.normal.x > 0f)
						{
							side = BlockFace.West;
						}
					}
					else if (num3 >= num && num3 >= num2)
					{
						if (contactPoint.normal.z < 0f)
						{
							side = BlockFace.North;
						}
						else if (contactPoint.normal.z > 0f)
						{
							side = BlockFace.South;
						}
					}
					else if (contactPoint.normal.y < 0f)
					{
						side = BlockFace.Bottom;
					}
					string surfaceCategory = block.Block.GetMaterialForSide(block, side).SurfaceCategory;
					if (this.itemClass != null && this.itemClass.MadeOfMaterial != null)
					{
						this.playThrowSound(this.itemClass.MadeOfMaterial.id + "hit" + surfaceCategory);
						return;
					}
					break;
				}
			}
			IL_300:;
		}
	}

	// Token: 0x0600229C RID: 8860 RVA: 0x000DAC10 File Offset: 0x000D8E10
	[PublicizedFrom(EAccessModifier.Private)]
	public bool CanCollide(Collision collision)
	{
		if (GameManager.Instance.World == null)
		{
			return false;
		}
		if (!this.bWasThrown && this.itemClass is ItemClassTimeBomb)
		{
			return false;
		}
		Transform transform = collision.transform;
		if (!transform)
		{
			return false;
		}
		string tag = transform.tag;
		if (tag != null && tag.StartsWith("E_"))
		{
			Transform hitRootTransform = GameUtils.GetHitRootTransform(tag, transform);
			if (hitRootTransform != null)
			{
				Entity component = hitRootTransform.GetComponent<Entity>();
				if (component != null && component.entityId == this.belongsPlayerId)
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x0600229D RID: 8861 RVA: 0x000DAC9C File Offset: 0x000D8E9C
	[PublicizedFrom(EAccessModifier.Private)]
	public void CheckStick(Collision collision)
	{
		if (this.stickPercent <= 0f)
		{
			return;
		}
		float d = 1f - this.stickPercent;
		this.itemRB.velocity *= d;
		this.itemRB.angularVelocity *= d;
		if (this.stickPercent >= 1f && !this.stickT)
		{
			this.stickT = collision.transform;
			this.stickRelativePos = this.stickT.InverseTransformPoint(base.transform.position);
			this.stickRot = Quaternion.Inverse(this.stickT.rotation) * base.transform.rotation;
			Collider[] componentsInChildren = this.itemRB.GetComponentsInChildren<Collider>();
			for (int i = componentsInChildren.Length - 1; i >= 0; i--)
			{
				componentsInChildren[i].gameObject.layer = 0;
			}
			this.checkGravitySetting(this.isPhysicsMaster);
			this.PlayOneShot(this.itemClass.SoundStick, false, false, false, null);
		}
	}

	// Token: 0x0600229E RID: 8862 RVA: 0x000DADAC File Offset: 0x000D8FAC
	public override string ToString()
	{
		if (this.itemStack.itemValue.HasQuality)
		{
			return string.Format("[type={0}, name={1}, cnt={2}, quality={3}]", new object[]
			{
				base.GetType().Name,
				this.itemClass.Name,
				this.itemStack.count,
				this.itemStack.itemValue.Quality
			});
		}
		return string.Format("[type={0}, name={1}, cnt={2}]", base.GetType().Name, (this.itemClass != null) ? this.itemClass.Name : string.Empty, this.itemStack.count);
	}

	// Token: 0x0600229F RID: 8863 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsQRotationUsed()
	{
		return true;
	}

	// Token: 0x170003BA RID: 954
	// (get) Token: 0x060022A0 RID: 8864 RVA: 0x000DAE62 File Offset: 0x000D9062
	public bool IsDistractionActive
	{
		get
		{
			return this.distractionLifetime > 0;
		}
	}

	// Token: 0x040019C4 RID: 6596
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Rigidbody itemRB;

	// Token: 0x040019C5 RID: 6597
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool useGravity;

	// Token: 0x040019C6 RID: 6598
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Renderer[] meshRenderers;

	// Token: 0x040019C7 RID: 6599
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameObject meshGameObject;

	// Token: 0x040019C8 RID: 6600
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform itemTransform;

	// Token: 0x040019C9 RID: 6601
	public ItemStack itemStack = ItemStack.Empty.Clone();

	// Token: 0x040019CA RID: 6602
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ItemStack lastCachedItemStack = ItemStack.Empty.Clone();

	// Token: 0x040019CB RID: 6603
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bMeshCreated;

	// Token: 0x040019CC RID: 6604
	public ItemClass itemClass;

	// Token: 0x040019CD RID: 6605
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float stickPercent;

	// Token: 0x040019CE RID: 6606
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform stickT;

	// Token: 0x040019CF RID: 6607
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 stickRelativePos;

	// Token: 0x040019D0 RID: 6608
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion stickRot;

	// Token: 0x040019D1 RID: 6609
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ItemWorldData itemWorldData;

	// Token: 0x040019D2 RID: 6610
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bWasThrown;

	// Token: 0x040019D3 RID: 6611
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int onGroundCounter;

	// Token: 0x040019D4 RID: 6612
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int distractionLifetime;

	// Token: 0x040019D5 RID: 6613
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float distractionStrength;

	// Token: 0x040019D6 RID: 6614
	public int distractionEatTicks;

	// Token: 0x040019D7 RID: 6615
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int nextDistractionTick;

	// Token: 0x040019D8 RID: 6616
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float distractionRadiusSq;

	// Token: 0x040019D9 RID: 6617
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static List<Entity> distractionTargets = new List<Entity>();

	// Token: 0x040019DA RID: 6618
	public int OwnerId = -1;

	// Token: 0x040019DB RID: 6619
	public static int ItemInstanceCount;

	// Token: 0x040019DC RID: 6620
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<ContactPoint> contactPoints;
}
