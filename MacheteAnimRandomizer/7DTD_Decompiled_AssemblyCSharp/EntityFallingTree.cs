using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

// Token: 0x02000442 RID: 1090
[Preserve]
public class EntityFallingTree : Entity
{
	// Token: 0x06002237 RID: 8759 RVA: 0x000D7AF9 File Offset: 0x000D5CF9
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		base.Awake();
		this.treeRB = base.GetComponent<Rigidbody>();
		this.treeRB.useGravity = !this.isEntityRemote;
		this.treeRB.isKinematic = this.isEntityRemote;
	}

	// Token: 0x06002238 RID: 8760 RVA: 0x000D7B32 File Offset: 0x000D5D32
	public Vector3i GetBlockPos()
	{
		return this.treeBlockPos;
	}

	// Token: 0x06002239 RID: 8761 RVA: 0x000D7B3A File Offset: 0x000D5D3A
	public Vector3 GetFallTreeDir()
	{
		return this.fallTreeDir;
	}

	// Token: 0x0600223A RID: 8762 RVA: 0x000D7B44 File Offset: 0x000D5D44
	public void SetBlockPos(Vector3i _blockPos, Vector3 _fallTreeDir)
	{
		this.treeBlockPos = _blockPos;
		this.fallTreeDir = _fallTreeDir;
		if (!this.isEntityRemote)
		{
			base.SetAirBorne(true);
		}
		Chunk chunk = (Chunk)this.world.GetChunkFromWorldPos(this.treeBlockPos);
		if (chunk == null)
		{
			return;
		}
		this.treeBV = chunk.GetBlock(World.toBlock(_blockPos));
		if (DecoManager.Instance.IsEnabled && this.treeBV.Block.IsDistantDecoration)
		{
			this.treeTransform = DecoManager.Instance.GetDecorationTransform(this.treeBlockPos, true);
		}
		else
		{
			BlockEntityData blockEntity = chunk.GetBlockEntity(this.treeBlockPos);
			if (blockEntity != null && blockEntity.bHasTransform)
			{
				this.treeTransform = blockEntity.transform;
				blockEntity.transform = null;
				blockEntity.bHasTransform = false;
			}
		}
		this.collHeight = 3f;
		if (this.treeTransform)
		{
			foreach (Collider collider in this.treeTransform.GetComponentsInChildren<Collider>())
			{
				collider.enabled = false;
				CapsuleCollider capsuleCollider = collider as CapsuleCollider;
				if (capsuleCollider != null)
				{
					this.collHeight = Utils.FastMax(this.collHeight, capsuleCollider.height);
				}
			}
		}
		this.collHeight *= 0.9f;
	}

	// Token: 0x0600223B RID: 8763 RVA: 0x000D7C75 File Offset: 0x000D5E75
	[PublicizedFrom(EAccessModifier.Protected)]
	public void OnCollisionEnter(Collision collision)
	{
		this.Collide(collision);
	}

	// Token: 0x0600223C RID: 8764 RVA: 0x000D7C75 File Offset: 0x000D5E75
	[PublicizedFrom(EAccessModifier.Protected)]
	public void OnCollisionStay(Collision collision)
	{
		this.Collide(collision);
	}

	// Token: 0x0600223D RID: 8765 RVA: 0x000D7C80 File Offset: 0x000D5E80
	[PublicizedFrom(EAccessModifier.Private)]
	public void Collide(Collision collision)
	{
		if (this.isEntityRemote)
		{
			return;
		}
		if (collision.contactCount == 0)
		{
			return;
		}
		float magnitude = collision.relativeVelocity.magnitude;
		if (magnitude > 1f)
		{
			this.collidedWith(collision.gameObject.transform);
		}
		if (magnitude > 0.2f && collision.impulse.magnitude / this.treeRB.mass > 1.5f)
		{
			Vector3 a = base.transform.position;
			float num = -1f;
			for (int i = 0; i < collision.contactCount; i++)
			{
				ContactPoint contact = collision.GetContact(i);
				float magnitude2 = contact.impulse.magnitude;
				if (magnitude2 > num)
				{
					num = magnitude2;
					a = contact.point;
				}
			}
			Manager.BroadcastPlay(this, "treefallimpact", false);
			ParticleEffect pe = new ParticleEffect("treefall", a + Origin.position, base.transform.rotation * Quaternion.AngleAxis(90f, Vector3.forward), 1f, Color.white, null, null);
			GameManager.Instance.SpawnParticleEffectServer(pe, this.entityId, false, false);
		}
	}

	// Token: 0x0600223E RID: 8766 RVA: 0x000D7DA8 File Offset: 0x000D5FA8
	[PublicizedFrom(EAccessModifier.Private)]
	public void collidedWith(Transform _other)
	{
		if (this.timeToEnableDamage > 0f)
		{
			return;
		}
		Transform transform = _other;
		string tag = _other.tag;
		if (tag.StartsWith("E_BP_"))
		{
			transform = GameUtils.GetHitRootTransform(tag, transform);
			tag = transform.tag;
		}
		if (tag.StartsWith("E_"))
		{
			Entity component = transform.GetComponent<Entity>();
			if (component && !component.IsDead() && this.treeCanDamageEntity(component))
			{
				this.hitEntities.Add(component.entityId);
				int damage = (int)(this.treeRB.mass * 0.35999998f);
				base.StartCoroutine(this.onEntityDamageLater(component, damage));
			}
		}
	}

	// Token: 0x0600223F RID: 8767 RVA: 0x000D7E48 File Offset: 0x000D6048
	[PublicizedFrom(EAccessModifier.Private)]
	public bool treeCanDamageEntity(Entity _entity)
	{
		return !this.hitEntities.Contains(_entity.entityId) && !(_entity is EntityPlayer) && !(_entity is EntitySupplyCrate);
	}

	// Token: 0x06002240 RID: 8768 RVA: 0x000D7E75 File Offset: 0x000D6075
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator onEntityDamageLater(Entity _entity, int _damage)
	{
		yield return new WaitForSeconds(0.05f);
		if (!_entity.IsDead() && _damage > 10)
		{
			_entity.DamageEntity(new DamageSource(EnumDamageSource.External, EnumDamageTypes.Crushing), _damage, false, 1f);
		}
		yield break;
	}

	// Token: 0x06002241 RID: 8769 RVA: 0x000D7E8C File Offset: 0x000D608C
	[PublicizedFrom(EAccessModifier.Private)]
	public void CreateMesh()
	{
		if (!this.isEntityRemote && this.world.GetBlock(this.treeBlockPos).type == this.treeBV.type)
		{
			this.world.SetBlockRPC(this.treeBlockPos, BlockValue.Air);
		}
		if (this.treeBV.isair)
		{
			if (this.treeTransform)
			{
				UnityEngine.Object.Destroy(this.treeTransform.gameObject);
				this.treeTransform = null;
			}
			return;
		}
		Transform transform = base.transform;
		this.SetPosition(this.treeTransform.position + Origin.position, true);
		this.SetRotation(this.treeTransform.eulerAngles);
		transform.SetPositionAndRotation(this.treeTransform.position, this.treeTransform.rotation);
		this.treeTransform.SetParent(transform, false);
		this.treeTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		Transform transform2 = this.treeTransform.Find("rootBall");
		if (transform2)
		{
			transform2.gameObject.SetActive(true);
			transform2.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
		}
		this.treeRB.useGravity = !this.isEntityRemote;
		this.treeRB.isKinematic = this.isEntityRemote;
		this.treeRB.position = this.treeTransform.position;
		this.treeRB.rotation = this.treeTransform.rotation;
		Block block = this.treeBV.Block;
		BlockShapeModelEntity blockShapeModelEntity = block.shape as BlockShapeModelEntity;
		float num = (blockShapeModelEntity != null) ? blockShapeModelEntity.modelOffset.y : 0f;
		RaycastHit raycastHit;
		if (Physics.SphereCast(new Ray(transform.position + 3f * Vector3.up, Vector3.down), 0.25f, out raycastHit, 5f, -538750981))
		{
			num = transform.position.y - raycastHit.point.y;
		}
		transform.gameObject.layer = 23;
		CapsuleCollider component = transform.GetComponent<CapsuleCollider>();
		component.height = this.collHeight;
		component.center = new Vector3(0f, this.collHeight * 0.5f - num, 0f);
		component.enabled = true;
		this.treeRB.mass = (15f + 7f * this.collHeight) * 5f;
		this.treeRB.centerOfMass = new Vector3(0f, this.collHeight * 0.3f - num, 0f);
		if (!this.isEntityRemote)
		{
			this.treeRB.velocity = Vector3.zero;
			this.treeRB.angularVelocity = Vector3.zero;
			this.treeRB.solverIterations = 10;
			this.treeRB.solverVelocityIterations = 3;
			this.treeRB.AddForceAtPosition(this.fallTreeDir * ((80f + this.collHeight * 8f) * 5f), transform.position + Vector3.up * (this.collHeight * 0.65f - num), ForceMode.Impulse);
			block.SpawnDestroyParticleEffect(this.world, this.treeBV, this.treeBlockPos, this.world.GetLightBrightness(this.treeBlockPos), block.GetColorForSide(this.treeBV, BlockFace.Top), -1);
			this.lifetime = 3f;
			this.timeToEnableDamage = 1.5f;
		}
		this.rendererList.Clear();
		foreach (MeshRenderer item in transform.GetComponentsInChildren<MeshRenderer>())
		{
			this.rendererList.Add(item);
		}
	}

	// Token: 0x06002242 RID: 8770 RVA: 0x000D823C File Offset: 0x000D643C
	public override void OnUpdateEntity()
	{
		base.OnUpdateEntity();
		if (!this.isMeshCreated && this.treeTransform)
		{
			this.isMeshCreated = true;
			this.CreateMesh();
		}
		if (this.isEntityRemote)
		{
			return;
		}
		this.timeToEnableDamage -= 0.05f;
		if (this.lifetime > 0f)
		{
			this.lifetime -= 0.05f;
			return;
		}
		if (this.timeToRemoveTree < 0f)
		{
			if (this.treeRB.angularVelocity.sqrMagnitude < 0.1f && this.treeRB.velocity.sqrMagnitude < 0.1f)
			{
				this.timeToRemoveTree = 1f;
				this.targetFade = 0f;
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<EntityFallingTree.NetPackageTreeFade>().Setup(this), false, -1, -1, -1, null, 192, false);
			}
		}
		else
		{
			this.timeToRemoveTree -= 0.05f;
			if (this.timeToRemoveTree < 0f)
			{
				this.DestroyTree();
			}
		}
		if (base.transform.position.y + Origin.position.y < 1f)
		{
			this.DestroyTree();
		}
	}

	// Token: 0x06002243 RID: 8771 RVA: 0x000D837C File Offset: 0x000D657C
	[PublicizedFrom(EAccessModifier.Private)]
	public void DestroyTree()
	{
		this.SetDead();
		if (!this.isEntityRemote && this.world.GetBlock(this.treeBlockPos).type == this.treeBV.type)
		{
			this.world.SetBlockRPC(this.treeBlockPos, BlockValue.Air);
		}
		if (this.treeTransform != null)
		{
			UnityEngine.Object.Destroy(this.treeTransform.gameObject);
			this.treeTransform = null;
		}
	}

	// Token: 0x06002244 RID: 8772 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool CanCollideWith(Entity _other)
	{
		return false;
	}

	// Token: 0x06002245 RID: 8773 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsQRotationUsed()
	{
		return true;
	}

	// Token: 0x06002246 RID: 8774 RVA: 0x000D83F8 File Offset: 0x000D65F8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateTransform()
	{
		float deltaTime = Time.deltaTime;
		this.fade = Mathf.MoveTowards(this.fade, this.targetFade, deltaTime);
		if (this.fade < 1f)
		{
			float num = this.fade;
			for (int i = 0; i < this.rendererList.Count; i++)
			{
				Renderer renderer = this.rendererList[i];
				if (renderer)
				{
					if (this.fade < 0.5f && renderer.shadowCastingMode == ShadowCastingMode.ShadowsOnly)
					{
						renderer.gameObject.SetActive(false);
					}
					renderer.GetMaterials(this.mats);
					for (int j = 0; j < this.mats.Count; j++)
					{
						if (num < 1f)
						{
							this.mats[j].EnableKeyword("ENABLE_FADEOUT");
						}
						else
						{
							this.mats[j].DisableKeyword("ENABLE_FADEOUT");
						}
						this.mats[j].SetFloat("_FadeOut", num);
					}
				}
			}
			this.mats.Clear();
		}
		Transform transform = base.transform;
		Vector3 vector = transform.position;
		if (this.isEntityRemote)
		{
			float t = deltaTime * 20f;
			vector = Vector3.Lerp(vector, this.targetPos - Origin.position, t);
			Quaternion rotation = Quaternion.Slerp(transform.rotation, this.targetQRot, t);
			transform.SetPositionAndRotation(vector, rotation);
			return;
		}
		this.SetPosition(vector + Origin.position, true);
		this.SetRotation(transform.eulerAngles);
	}

	// Token: 0x06002247 RID: 8775 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsSavedToFile()
	{
		return false;
	}

	// Token: 0x06002248 RID: 8776 RVA: 0x000D8590 File Offset: 0x000D6790
	public override void OnEntityUnload()
	{
		if (this.treeTransform != null)
		{
			UnityEngine.Object.Destroy(this.treeTransform.gameObject);
			this.treeTransform = null;
		}
		base.OnEntityUnload();
	}

	// Token: 0x06002249 RID: 8777 RVA: 0x000D85BD File Offset: 0x000D67BD
	public override void MarkToUnload()
	{
		base.MarkToUnload();
		if (!this.isEntityRemote)
		{
			this.world.SetBlockRPC(this.treeBlockPos, BlockValue.Air);
		}
	}

	// Token: 0x04001991 RID: 6545
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cMassScale = 5f;

	// Token: 0x04001992 RID: 6546
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3i treeBlockPos;

	// Token: 0x04001993 RID: 6547
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public BlockValue treeBV;

	// Token: 0x04001994 RID: 6548
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 fallTreeDir;

	// Token: 0x04001995 RID: 6549
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform treeTransform;

	// Token: 0x04001996 RID: 6550
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Rigidbody treeRB;

	// Token: 0x04001997 RID: 6551
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<MeshRenderer> rendererList = new List<MeshRenderer>();

	// Token: 0x04001998 RID: 6552
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float collHeight;

	// Token: 0x04001999 RID: 6553
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isMeshCreated;

	// Token: 0x0400199A RID: 6554
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float timeToRemoveTree = -1f;

	// Token: 0x0400199B RID: 6555
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float timeToEnableDamage;

	// Token: 0x0400199C RID: 6556
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<int> hitEntities = new List<int>();

	// Token: 0x0400199D RID: 6557
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float fade = 1f;

	// Token: 0x0400199E RID: 6558
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float targetFade = 1f;

	// Token: 0x0400199F RID: 6559
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<Material> mats = new List<Material>();

	// Token: 0x02000443 RID: 1091
	[Preserve]
	public class NetPackageTreeFade : NetPackage
	{
		// Token: 0x170003B4 RID: 948
		// (get) Token: 0x0600224B RID: 8779 RVA: 0x000282C0 File Offset: 0x000264C0
		public override NetPackageDirection PackageDirection
		{
			get
			{
				return NetPackageDirection.ToClient;
			}
		}

		// Token: 0x0600224C RID: 8780 RVA: 0x000D8639 File Offset: 0x000D6839
		public EntityFallingTree.NetPackageTreeFade Setup(Entity entity)
		{
			this.entityId = entity.entityId;
			return this;
		}

		// Token: 0x0600224D RID: 8781 RVA: 0x000D8648 File Offset: 0x000D6848
		public override void read(PooledBinaryReader _reader)
		{
			this.entityId = _reader.ReadInt32();
		}

		// Token: 0x0600224E RID: 8782 RVA: 0x000D8656 File Offset: 0x000D6856
		public override void write(PooledBinaryWriter _writer)
		{
			base.write(_writer);
			_writer.Write(this.entityId);
		}

		// Token: 0x0600224F RID: 8783 RVA: 0x000D866C File Offset: 0x000D686C
		public override void ProcessPackage(World _world, GameManager _callbacks)
		{
			if (_world == null)
			{
				return;
			}
			EntityFallingTree entityFallingTree = _world.GetEntity(this.entityId) as EntityFallingTree;
			if (entityFallingTree != null)
			{
				entityFallingTree.targetFade = 0f;
			}
		}

		// Token: 0x06002250 RID: 8784 RVA: 0x00075CC0 File Offset: 0x00073EC0
		public override int GetLength()
		{
			return 4;
		}

		// Token: 0x040019A0 RID: 6560
		[PublicizedFrom(EAccessModifier.Private)]
		public int entityId;
	}
}
