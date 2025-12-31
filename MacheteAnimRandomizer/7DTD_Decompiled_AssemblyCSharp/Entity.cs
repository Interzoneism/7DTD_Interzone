using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020003A5 RID: 933
[Preserve]
public class Entity : MonoBehaviour
{
	// Token: 0x17000327 RID: 807
	// (get) Token: 0x06001BB6 RID: 7094 RVA: 0x000ADBB6 File Offset: 0x000ABDB6
	public FastTags<TagGroup.Global> EntityTags
	{
		get
		{
			return this.cachedTags;
		}
	}

	// Token: 0x17000328 RID: 808
	// (get) Token: 0x06001BB7 RID: 7095 RVA: 0x000ADBC0 File Offset: 0x000ABDC0
	public EntityClass EntityClass
	{
		get
		{
			EntityClass result;
			EntityClass.list.TryGetValue(this.entityClass, out result);
			return result;
		}
	}

	// Token: 0x17000329 RID: 809
	// (get) Token: 0x06001BB8 RID: 7096 RVA: 0x000ADBE4 File Offset: 0x000ABDE4
	public EntityActivationCommand[] CustomCmds
	{
		get
		{
			if (this.customCmds == null)
			{
				EntityClass entityClass = EntityClass.list[this.entityClass];
				int num = 0;
				int num2 = 1;
				while (num2 <= 10 && entityClass.Properties.Values.ContainsKey(string.Format("{0}{1}", EntityClass.PropCustomCommandName, num2)))
				{
					num++;
					num2++;
				}
				this.customCmds = new EntityActivationCommand[num];
				if (num > 0)
				{
					for (int i = 1; i <= num; i++)
					{
						if (entityClass.Properties.Values.ContainsKey(string.Format("{0}{1}", EntityClass.PropCustomCommandName, i)))
						{
							EntityActivationCommand entityActivationCommand = default(EntityActivationCommand);
							entityActivationCommand.text = entityClass.Properties.Values[string.Format("{0}{1}", EntityClass.PropCustomCommandName, i)];
							entityActivationCommand.icon = entityClass.Properties.Values[string.Format("{0}{1}", EntityClass.PropCustomCommandIcon, i)];
							entityActivationCommand.eventName = entityClass.Properties.Values[string.Format("{0}{1}", EntityClass.PropCustomCommandEvent, i)];
							entityActivationCommand.enabled = true;
							string text = string.Format("{0}{1}", EntityClass.PropCustomCommandIconColor, i);
							if (this.EntityClass.Properties.Values.ContainsKey(text))
							{
								entityActivationCommand.iconColor = StringParsers.ParseHexColor(this.EntityClass.Properties.Values[text]);
							}
							else
							{
								entityActivationCommand.iconColor = Color.white;
							}
							this.customCmds[i - 1] = entityActivationCommand;
						}
					}
				}
			}
			return this.customCmds;
		}
	}

	// Token: 0x1700032A RID: 810
	// (get) Token: 0x06001BB9 RID: 7097 RVA: 0x000ADDA1 File Offset: 0x000ABFA1
	public virtual string LocalizedEntityName
	{
		get
		{
			return Localization.Get(EntityClass.list[this.entityClass].entityClassName, false);
		}
	}

	// Token: 0x1700032B RID: 811
	// (get) Token: 0x06001BBA RID: 7098 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual Entity.EnumPositionUpdateMovementType positionUpdateMovementType
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return Entity.EnumPositionUpdateMovementType.Lerp;
		}
	}

	// Token: 0x06001BBB RID: 7099 RVA: 0x000ADDC0 File Offset: 0x000ABFC0
	public static bool CheckDistance(int entityID_A, int entityID_B)
	{
		if (GameManager.Instance == null)
		{
			return false;
		}
		if (GameManager.Instance.World == null)
		{
			return false;
		}
		Entity entity = GameManager.Instance.World.GetEntity(entityID_A);
		if (entity == null)
		{
			return false;
		}
		Entity entity2 = GameManager.Instance.World.GetEntity(entityID_B);
		return !(entity2 == null) && Entity.CheckDistance(entity, entity2);
	}

	// Token: 0x06001BBC RID: 7100 RVA: 0x000ADE29 File Offset: 0x000AC029
	public static bool CheckDistance(Entity entityB, int entityID_A)
	{
		return Entity.CheckDistance(entityID_A, entityB);
	}

	// Token: 0x06001BBD RID: 7101 RVA: 0x000ADE34 File Offset: 0x000AC034
	public static bool CheckDistance(int entityID_A, Entity entityB)
	{
		if (GameManager.Instance == null)
		{
			return false;
		}
		if (GameManager.Instance.World == null)
		{
			return false;
		}
		if (entityB == null)
		{
			return false;
		}
		Entity entity = GameManager.Instance.World.GetEntity(entityID_A);
		return !(entity == null) && Entity.CheckDistance(entity, entityB);
	}

	// Token: 0x06001BBE RID: 7102 RVA: 0x000ADE8C File Offset: 0x000AC08C
	public static bool CheckDistance(Entity A, Vector3 B)
	{
		return !(A == null) && Entity.CheckDistance(A.transform.position, B);
	}

	// Token: 0x06001BBF RID: 7103 RVA: 0x000ADEAA File Offset: 0x000AC0AA
	public static bool CheckDistance(Vector3 A, Entity B)
	{
		return !(B == null) && Entity.CheckDistance(A, B.transform.position);
	}

	// Token: 0x06001BC0 RID: 7104 RVA: 0x000ADEC8 File Offset: 0x000AC0C8
	public static bool CheckDistance(Vector3 A, int entityID_B)
	{
		if (GameManager.Instance == null)
		{
			return false;
		}
		if (GameManager.Instance.World == null)
		{
			return false;
		}
		Entity entity = GameManager.Instance.World.GetEntity(entityID_B);
		return !(entity == null) && Entity.CheckDistance(A - Origin.position, entity.transform.position);
	}

	// Token: 0x06001BC1 RID: 7105 RVA: 0x000ADF2C File Offset: 0x000AC12C
	public static bool CheckDistance(Vector3 A, Vector3 B)
	{
		return (A - B).magnitude < 256f;
	}

	// Token: 0x06001BC2 RID: 7106 RVA: 0x000ADF4F File Offset: 0x000AC14F
	public static bool CheckDistance(Entity listenerEntity, Entity sourceEntity)
	{
		return Entity.CheckDistance(sourceEntity.transform.position, listenerEntity.transform.position);
	}

	// Token: 0x06001BC3 RID: 7107 RVA: 0x000ADF6C File Offset: 0x000AC16C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Awake()
	{
		Entity.InstanceCount++;
		this.world = GameManager.Instance.World;
		this.isEntityRemote = !SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer;
		this.WorldTimeBorn = this.world.worldTime;
		this.rand = this.world.GetGameRandom();
		this.SetupBounds();
	}

	// Token: 0x06001BC4 RID: 7108 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Start()
	{
	}

	// Token: 0x06001BC5 RID: 7109 RVA: 0x000ADFD0 File Offset: 0x000AC1D0
	[PublicizedFrom(EAccessModifier.Protected)]
	public ~Entity()
	{
		Entity.InstanceCount--;
	}

	// Token: 0x06001BC6 RID: 7110 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnXMLChanged()
	{
	}

	// Token: 0x06001BC7 RID: 7111 RVA: 0x000AE004 File Offset: 0x000AC204
	[PublicizedFrom(EAccessModifier.Protected)]
	public void SetupBounds()
	{
		BoxCollider boxCollider;
		if (base.TryGetComponent<BoxCollider>(out boxCollider))
		{
			this.nativeCollider = boxCollider;
			Vector3 localScale = base.transform.localScale;
			this.scaledExtent = Vector3.Scale(boxCollider.size, localScale) * 0.5f;
			Vector3 b = Vector3.Scale(boxCollider.center, localScale);
			this.boundingBox = BoundsUtils.BoundsForMinMax(-this.scaledExtent, this.scaledExtent);
			this.boundingBox.center = this.boundingBox.center + b;
			if (this.isDetailedHeadBodyColliders())
			{
				boxCollider.enabled = false;
				return;
			}
		}
		else
		{
			CharacterController characterController;
			if (base.TryGetComponent<CharacterController>(out characterController))
			{
				Vector3 localScale2 = base.transform.localScale;
				float radius = characterController.radius;
				this.scaledExtent = new Vector3(radius * localScale2.x, characterController.height * localScale2.y * 0.5f, radius * localScale2.z);
				this.boundingBox = BoundsUtils.BoundsForMinMax(-this.scaledExtent, this.scaledExtent);
				return;
			}
			this.boundingBox = BoundsUtils.BoundsForMinMax(Vector3.zero, Vector3.one);
		}
	}

	// Token: 0x06001BC8 RID: 7112 RVA: 0x000AE124 File Offset: 0x000AC324
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Update()
	{
		this.bWasDead = this.IsDead();
		this.animateYaw();
		if (this.physicsMasterTargetTime > 0f)
		{
			this.PhysicsMasterTargetFrameUpdate();
		}
		else
		{
			this.updateTransform();
		}
		if (this.bIsChunkObserver && !this.isEntityRemote)
		{
			if (this.movableChunkObserver == null)
			{
				this.movableChunkObserver = new MovableSharedChunkObserver(this.world.m_SharedChunkObserverCache);
			}
			this.movableChunkObserver.SetPosition(this.position);
		}
		else if (!this.bIsChunkObserver && this.movableChunkObserver != null)
		{
			this.movableChunkObserver.Dispose();
			this.movableChunkObserver = null;
		}
		if (this.animatorAudioMonitoringDictionary.Count > 0)
		{
			List<Entity.StopAnimatorAudioType> list = new List<Entity.StopAnimatorAudioType>();
			foreach (KeyValuePair<Entity.StopAnimatorAudioType, Handle> keyValuePair in this.animatorAudioMonitoringDictionary)
			{
				if (!keyValuePair.Value.IsPlaying())
				{
					keyValuePair.Value.Stop(this.entityId);
					list.Add(keyValuePair.Key);
				}
			}
			foreach (Entity.StopAnimatorAudioType key in list)
			{
				this.animatorAudioMonitoringDictionary.Remove(key);
			}
		}
	}

	// Token: 0x06001BC9 RID: 7113 RVA: 0x000AE28C File Offset: 0x000AC48C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void updateTransform()
	{
		if (this.AttachedToEntity != null)
		{
			return;
		}
		this.ApplyFixedUpdate();
		if (!this.emodel || !this.emodel.IsRagdollOn)
		{
			float y;
			if (this.physicsRB)
			{
				Vector3 b = this.physicsRBT.position - this.physicsBasePos;
				Vector3 vector = Vector3.Lerp(base.transform.position, b, this.physicsPosMoveDistance * Time.deltaTime / Time.fixedDeltaTime);
				base.transform.position = vector;
				y = this.physicsRBT.eulerAngles.y;
			}
			else
			{
				Vector3 b2 = this.position - Origin.position;
				base.transform.position = Vector3.Lerp(base.transform.position, b2, Time.deltaTime * Entity.updatePositionLerpTimeScale);
				y = this.rotation.y;
			}
			if (this.isRotateToGround)
			{
				Vector3 vector2 = this.groundSurface.normal;
				float num = Vector3.Dot(vector2, Vector3.up);
				if (this.IsRotateToGroundFlat)
				{
					num = 1f;
				}
				if (num > 0.99f || num < 0.7f)
				{
					vector2 = Vector3.up;
				}
				Vector3 vector3 = Quaternion.AngleAxis(-y, Vector3.up) * vector2;
				float target = 90f - Mathf.Atan2(vector3.y, vector3.z) * 57.29578f;
				this.rotateToGroundPitchVel *= 0.86f;
				this.rotateToGroundPitchVel += Mathf.DeltaAngle(this.rotateToGroundPitch, target) * 0.8f * Time.deltaTime;
				this.rotateToGroundPitch += this.rotateToGroundPitchVel;
				base.transform.eulerAngles = new Vector3(this.rotateToGroundPitch, y, 0f);
			}
			else
			{
				base.transform.eulerAngles = new Vector3(0f, Mathf.LerpAngle(base.transform.eulerAngles.y, y, Time.deltaTime * Entity.updateRotationLerpTimeScale), 0f);
			}
		}
		if (this.isEntityRemote && this.PhysicsTransform != null)
		{
			this.PhysicsTransform.position = Vector3.Lerp(this.PhysicsTransform.position, this.position - Origin.position, Time.deltaTime * Entity.updateRotationLerpTimeScale);
		}
	}

	// Token: 0x06001BCA RID: 7114 RVA: 0x000AE4EC File Offset: 0x000AC6EC
	[PublicizedFrom(EAccessModifier.Private)]
	public void FixedUpdate()
	{
		this.ApplyFixedUpdate();
		this.wasFixedUpdate = true;
		if (this.physicsRB)
		{
			this.physicsRB.velocity *= 0.9f;
			this.physicsRB.angularVelocity *= 0.9f;
			Transform transform = this.physicsRBT;
			Vector3 b = this.physicsTargetPos + this.physicsBasePos;
			Vector3 vector = Vector3.Lerp(transform.position, b, 0.4f);
			this.physicsPos = vector;
			this.physicsRot = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, this.rotation.y, 0f), 0.3f);
			transform.SetPositionAndRotation(vector, this.physicsRot);
			if (this.physicsCapsuleCollider)
			{
				EntityAlive entityAlive = this as EntityAlive;
				if (entityAlive)
				{
					entityAlive.CrouchHeightFixedUpdate();
				}
			}
		}
	}

	// Token: 0x06001BCB RID: 7115 RVA: 0x000AE5E0 File Offset: 0x000AC7E0
	[PublicizedFrom(EAccessModifier.Private)]
	public void ApplyFixedUpdate()
	{
		if (!this.wasFixedUpdate)
		{
			return;
		}
		this.wasFixedUpdate = false;
		if (this.physicsRB)
		{
			Transform transform = this.physicsRBT;
			Vector3 a = transform.position;
			if ((a - this.physicsPos).sqrMagnitude > 0.0001f)
			{
				Vector3 vector = this.position;
				Vector3 a2 = a - this.physicsBasePos;
				this.physicsPos = a;
				this.SetPosition(a2 + Origin.position, false);
				this.PhysicsTransform.position = a2;
			}
			this.physicsPosMoveDistance = Vector3.Distance(this.physicsPos, base.transform.position);
			if (Mathf.Abs(Quaternion.Angle(transform.rotation, this.physicsRot)) > 0.1f)
			{
				Quaternion quaternion = transform.rotation;
				this.physicsRot = quaternion;
				this.rotation = quaternion.eulerAngles;
				this.qrotation = quaternion;
			}
		}
	}

	// Token: 0x06001BCC RID: 7116 RVA: 0x000AE6CD File Offset: 0x000AC8CD
	public virtual void OriginChanged(Vector3 _deltaPos)
	{
		this.physicsPos += _deltaPos;
		this.physicsTargetPos += _deltaPos;
		if (this.emodel)
		{
			this.emodel.OriginChanged(_deltaPos);
		}
	}

	// Token: 0x06001BCD RID: 7117 RVA: 0x000AE70C File Offset: 0x000AC90C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void AddCharacterController()
	{
		if (!this.PhysicsTransform)
		{
			return;
		}
		float num = 0.08f;
		Vector3 center = Vector3.zero;
		bool flag = false;
		GameObject gameObject = this.PhysicsTransform.gameObject;
		float num2;
		float num3;
		if (this is EntityPlayer)
		{
			CharacterController component = gameObject.GetComponent<CharacterController>();
			if (!component)
			{
				Log.Error("Player !cc");
				return;
			}
			center = component.center;
			num2 = component.height;
			num3 = component.radius;
			this.m_characterController = new CharacterControllerUnity(component);
			if (!this.isEntityRemote)
			{
				gameObject.AddComponent<ColliderHitCallForward>().Entity = this;
			}
			BoxCollider boxCollider = this.nativeCollider as BoxCollider;
			if (boxCollider)
			{
				num2 = Utils.FastMax(boxCollider.size.y - num, this.stepHeight);
				center = boxCollider.center;
				center.y = num2 * 0.5f;
				if (boxCollider.size.x > boxCollider.size.y)
				{
					center.y += (boxCollider.size.x - boxCollider.size.y) * 0.5f;
				}
				num3 = boxCollider.size.x * 0.5f - num;
				flag = true;
			}
		}
		else
		{
			flag = true;
			CapsuleCollider component2 = gameObject.GetComponent<CapsuleCollider>();
			if (component2)
			{
				center = component2.center;
				num2 = component2.height;
				num3 = component2.radius;
			}
			else
			{
				gameObject.AddComponent<CapsuleCollider>();
				center.y = 0.9f;
				num2 = 1.8f;
				num3 = 0.3f;
			}
			if (this.physicsCapsuleCollider)
			{
				num2 = this.physicsBaseHeight;
				center.y = num2 * 0.5f;
			}
			CharacterController characterController;
			if (gameObject.TryGetComponent<CharacterController>(out characterController))
			{
				center = characterController.center;
				num2 = characterController.height;
				num3 = characterController.radius;
				UnityEngine.Object.Destroy(characterController);
				Log.Warning("{0} has old CC", new object[]
				{
					this.ToString()
				});
			}
			this.m_characterController = new CharacterControllerKinematic(this);
		}
		if (num2 <= 0f)
		{
			return;
		}
		if (flag)
		{
			center.y /= this.physicsHeightScale;
			this.m_characterController.SetSize(center, num2 / this.physicsHeightScale, num3);
			this.physicsBaseHeight = num2;
			this.physicsHeight = num2;
			if (this.physicsCapsuleCollider)
			{
				this.PhysicsSetHeight(num2);
			}
		}
		this.m_characterController.SetStepOffset(this.stepHeight);
		Vector3 localScale = base.transform.localScale;
		this.scaledExtent = new Vector3(num3 * localScale.x, num2 * localScale.y * 0.5f, num3 * localScale.z);
		this.boundingBox = BoundsUtils.BoundsForMinMax(-this.scaledExtent, this.scaledExtent);
		if (this.nativeCollider)
		{
			this.nativeCollider.enabled = false;
		}
	}

	// Token: 0x06001BCE RID: 7118 RVA: 0x000AE9F8 File Offset: 0x000ACBF8
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetCCScale(float scale)
	{
		CharacterControllerAbstract characterController = this.m_characterController;
		if (characterController == null)
		{
			return;
		}
		this.PhysicsTransform.localScale = Vector3.one;
		Vector3 center = characterController.GetCenter() * scale;
		float num = characterController.GetHeight() * scale;
		if (num < 2.2f && num > 1.89f)
		{
			num = 1.89f;
			center.y = num * 0.5f;
		}
		float num2 = Utils.FastMax(scale, 1f);
		characterController.SetSize(center, num, characterController.GetRadius() * num2);
	}

	// Token: 0x06001BCF RID: 7119 RVA: 0x000AEA76 File Offset: 0x000ACC76
	public virtual void Init(int _entityClass)
	{
		this.entityClass = _entityClass;
		this.InitCommon();
		this.InitEModel();
		this.PhysicsInit();
	}

	// Token: 0x06001BD0 RID: 7120 RVA: 0x000AEA91 File Offset: 0x000ACC91
	public virtual void InitFromPrefab(int _entityClass)
	{
		this.entityClass = _entityClass;
		this.InitCommon();
		this.InitEModelFromPrefab();
		this.PhysicsInit();
	}

	// Token: 0x06001BD1 RID: 7121 RVA: 0x000AEAAC File Offset: 0x000ACCAC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void InitCommon()
	{
		EntityClass entityClass = EntityClass.list[this.entityClass];
		this.cachedTags = entityClass.Tags;
		this.bIsChunkObserver = entityClass.bIsChunkObserver;
		this.CopyPropertiesFromEntityClass();
		if (this.PhysicsTransform)
		{
			this.PhysicsTransform.gameObject.tag = "Physics";
		}
	}

	// Token: 0x06001BD2 RID: 7122 RVA: 0x000AEB0C File Offset: 0x000ACD0C
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitEModel()
	{
		Type modelType = EntityClass.list[this.entityClass].modelType;
		this.emodel = (base.gameObject.AddComponent(modelType) as EModelBase);
		this.emodel.Init(this.world, this);
	}

	// Token: 0x06001BD3 RID: 7123 RVA: 0x000AEB58 File Offset: 0x000ACD58
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitEModelFromPrefab()
	{
		Type modelType = EntityClass.list[this.entityClass].modelType;
		this.emodel = (base.gameObject.GetComponent(modelType) as EModelBase);
		this.emodel.InitFromPrefab(this.world, this);
	}

	// Token: 0x06001BD4 RID: 7124 RVA: 0x000AEBA4 File Offset: 0x000ACDA4
	public virtual void PostInit()
	{
		if (this.emodel != null)
		{
			this.emodel.PostInit();
			this.HandleNavObject();
		}
	}

	// Token: 0x06001BD5 RID: 7125 RVA: 0x000AEBC8 File Offset: 0x000ACDC8
	[PublicizedFrom(EAccessModifier.Private)]
	public void PhysicsInit()
	{
		Transform transform = GameUtils.FindTagInChilds(this.ModelTransform, "Physics");
		if (transform)
		{
			this.PhysicsTransform = this.RootTransform.Find("Physics");
			if (this.PhysicsTransform)
			{
				UnityEngine.Object.Destroy(this.PhysicsTransform.gameObject);
				Log.Warning("{0} has old Physics", new object[]
				{
					this.ToString()
				});
			}
			this.PhysicsTransform = transform;
			transform.SetParent(this.RootTransform, false);
		}
		else if (!this.PhysicsTransform)
		{
			this.PhysicsTransform = this.RootTransform.Find("Physics");
		}
		this.physicsRBT = GameUtils.FindTagInChilds(this.RootTransform, "LargeEntityBlocker");
		if (this.physicsRBT)
		{
			Transform transform2 = base.transform;
			Transform parent = this.physicsRBT.parent;
			this.physicsPos = this.physicsRBT.position;
			this.physicsRot = transform2.rotation;
			if (parent != transform2.parent)
			{
				Vector3 vector = this.physicsRBT.localPosition;
				float x = parent.lossyScale.x;
				vector += parent.localPosition * (1f / x);
				Collider[] componentsInChildren = this.physicsRBT.GetComponentsInChildren<Collider>();
				for (int i = componentsInChildren.Length - 1; i >= 0; i--)
				{
					Collider collider = componentsInChildren[i];
					CapsuleCollider capsuleCollider;
					BoxCollider boxCollider;
					SphereCollider sphereCollider;
					if (capsuleCollider = (collider as CapsuleCollider))
					{
						capsuleCollider.center = (capsuleCollider.center + vector) * x;
						capsuleCollider.height *= x;
						capsuleCollider.radius *= x;
					}
					else if (boxCollider = (collider as BoxCollider))
					{
						boxCollider.center = (boxCollider.center + vector) * x;
						boxCollider.size *= x;
					}
					else if (sphereCollider = (collider as SphereCollider))
					{
						sphereCollider.center = (sphereCollider.center + vector) * x;
						sphereCollider.radius *= x;
					}
				}
				this.physicsBasePos = Vector3.zero;
				this.physicsRBT.SetParent(transform2.parent, true);
				this.physicsRBT.localScale = Vector3.one;
			}
			else
			{
				this.physicsBasePos = Vector3.Scale(this.physicsRBT.localPosition, parent.lossyScale);
			}
			this.physicsRB = this.physicsRBT.gameObject.AddComponent<Rigidbody>();
			this.physicsRB.useGravity = false;
			float v = EntityClass.list[this.entityClass].MassKg * 0.6f;
			this.physicsRB.mass = Utils.FastMax(30f, v);
			this.physicsRB.constraints = (RigidbodyConstraints)80;
			this.physicsRB.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			this.physicsTargetPos = this.physicsPos;
			CapsuleCollider component = this.physicsRBT.GetComponent<CapsuleCollider>();
			if (component && component.direction == 1)
			{
				this.physicsCapsuleCollider = component;
				this.physicsColliderRadius = component.radius;
				this.physicsHeightScale = 1.09f;
				float height = component.height;
				float y = component.center.y;
				float num = y + height * 0.5f;
				this.physicsBaseHeight = num * this.physicsHeightScale;
				this.physicsColliderLowerY = y - height * 0.5f;
				if ((double)this.physicsBaseHeight > 1.95)
				{
					this.physicsBaseHeight = 1.95f;
				}
			}
		}
	}

	// Token: 0x06001BD6 RID: 7126 RVA: 0x000AEF7C File Offset: 0x000AD17C
	public void PhysicsSetRB(Rigidbody rb)
	{
		this.physicsRB = rb;
	}

	// Token: 0x06001BD7 RID: 7127 RVA: 0x000AEF85 File Offset: 0x000AD185
	public void PhysicsPause()
	{
		if (this.physicsRBT)
		{
			this.physicsRBT.gameObject.SetActive(false);
		}
	}

	// Token: 0x06001BD8 RID: 7128 RVA: 0x000AEFA8 File Offset: 0x000AD1A8
	public virtual void PhysicsResume(Vector3 pos, float rotY)
	{
		this.rotation = new Vector3(0f, rotY, 0f);
		if (this.physicsRBT)
		{
			this.physicsRBT.gameObject.SetActive(true);
			this.physicsRBT.eulerAngles = this.rotation;
			this.physicsPosMoveDistance = 0f;
		}
		this.SetPosition(pos, true);
		base.transform.SetPositionAndRotation(pos - Origin.position, Quaternion.Euler(this.rotation));
	}

	// Token: 0x06001BD9 RID: 7129 RVA: 0x000AF030 File Offset: 0x000AD230
	public virtual void PhysicsPush(Vector3 forceVec, Vector3 forceWorldPos, bool affectLocalPlayerController = false)
	{
		if (forceVec.sqrMagnitude > 0f)
		{
			Rigidbody rigidbody = this.physicsRB;
			if (rigidbody)
			{
				if (!this.emodel.IsRagdollActive)
				{
					forceVec *= 5f;
				}
				if (forceWorldPos.sqrMagnitude > 0f)
				{
					rigidbody.AddForceAtPosition(forceVec, forceWorldPos - Origin.position, ForceMode.Impulse);
					return;
				}
				rigidbody.AddForce(forceVec, ForceMode.Impulse);
			}
		}
	}

	// Token: 0x06001BDA RID: 7130 RVA: 0x000AF0A0 File Offset: 0x000AD2A0
	public void PhysicsSetHeight(float _height)
	{
		this.physicsHeight = _height;
		float num = this.physicsColliderLowerY;
		if (_height - num < this.physicsColliderRadius)
		{
			num = _height - this.physicsColliderRadius;
			if (num < 0f)
			{
				num = 0f;
			}
		}
		this.physicsCapsuleCollider.height = _height - num;
		Vector3 center = this.physicsCapsuleCollider.center;
		center.y = (_height + num) * 0.5f;
		if (center.y < this.physicsColliderRadius)
		{
			center.y = this.physicsColliderRadius;
		}
		this.physicsCapsuleCollider.center = center;
	}

	// Token: 0x06001BDB RID: 7131 RVA: 0x000AF130 File Offset: 0x000AD330
	public virtual void PhysicsMasterBecome()
	{
		this.isPhysicsMaster = true;
		this.physicsMasterTargetTime = 0f;
		this.SetPosition(this.physicsMasterTargetPos, false);
		this.qrotation = this.physicsMasterTargetRot;
		if (this.physicsRB)
		{
			this.physicsRB.position = this.position - Origin.position;
			this.physicsRB.rotation = this.qrotation;
			this.physicsRB.velocity = this.physicsVel;
			this.physicsRB.angularVelocity = this.physicsAngVel;
		}
	}

	// Token: 0x06001BDC RID: 7132 RVA: 0x000AF1C4 File Offset: 0x000AD3C4
	public NetPackageEntityPhysics PhysicsMasterSetupBroadcast()
	{
		if ((this.position - this.physicsMasterSendPos).sqrMagnitude < 0.0025000002f && Quaternion.Angle(this.qrotation, this.physicsMasterSendRot) < 1f)
		{
			return null;
		}
		this.physicsMasterSendPos = this.position;
		this.physicsMasterSendRot = this.qrotation;
		return NetPackageManager.GetPackage<NetPackageEntityPhysics>().Setup(this);
	}

	// Token: 0x06001BDD RID: 7133 RVA: 0x000AF230 File Offset: 0x000AD430
	public void PhysicsMasterSendToServer(Transform t)
	{
		if (this.clientEntityId != 0)
		{
			return;
		}
		this.position = t.position + Origin.position;
		this.qrotation = t.rotation;
		if (this.GetVelocityPerSecond().sqrMagnitude < 0.16000001f)
		{
			this.isPhysicsMaster = false;
		}
		NetPackageEntityPhysics package = NetPackageManager.GetPackage<NetPackageEntityPhysics>().Setup(this);
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(package, false);
	}

	// Token: 0x06001BDE RID: 7134 RVA: 0x000AF29C File Offset: 0x000AD49C
	public Vector3 PhysicsMasterGetFinalPosition()
	{
		if (this.physicsMasterTargetTime > 0f)
		{
			return this.physicsMasterTargetPos;
		}
		return this.position;
	}

	// Token: 0x06001BDF RID: 7135 RVA: 0x000AF2B8 File Offset: 0x000AD4B8
	public void PhysicsMasterSetTargetOrientation(Vector3 pos, Quaternion rot)
	{
		this.physicsMasterFromPos = this.position;
		this.physicsMasterFromRot = this.qrotation;
		this.physicsMasterTargetElapsed = 0f;
		this.physicsMasterTargetTime = 0.1f;
		this.physicsMasterTargetPos = pos;
		this.physicsMasterTargetRot = rot;
	}

	// Token: 0x06001BE0 RID: 7136 RVA: 0x000AF2F8 File Offset: 0x000AD4F8
	public void PhysicsMasterTargetFrameUpdate()
	{
		this.physicsMasterTargetElapsed += Time.deltaTime;
		float t = this.physicsMasterTargetElapsed / this.physicsMasterTargetTime;
		Vector3 vector = Vector3.Lerp(this.physicsMasterFromPos, this.physicsMasterTargetPos, t);
		this.SetPosition(vector, true);
		Quaternion quaternion = Quaternion.Lerp(this.physicsMasterFromRot, this.physicsMasterTargetRot, t);
		this.qrotation = quaternion;
		this.physicsRB.position = vector - Origin.position;
		this.physicsRB.rotation = quaternion;
		if (this.physicsMasterTargetElapsed >= this.physicsMasterTargetTime)
		{
			this.physicsMasterTargetTime = 0f;
		}
	}

	// Token: 0x06001BE1 RID: 7137 RVA: 0x000AF395 File Offset: 0x000AD595
	public void SetHeight(float _height)
	{
		this.m_characterController.SetHeight(_height / this.physicsHeightScale);
		this.PhysicsSetHeight(_height);
	}

	// Token: 0x06001BE2 RID: 7138 RVA: 0x000AF3B4 File Offset: 0x000AD5B4
	public void SetMaxHeight(float _maxHeight)
	{
		this.physicsBaseHeight = _maxHeight;
		if (this.m_characterController != null)
		{
			this.m_characterController.SetHeight(_maxHeight / this.physicsHeightScale);
		}
		if (this.physicsCapsuleCollider)
		{
			this.PhysicsSetHeight(_maxHeight);
			float y = this.physicsCapsuleCollider.center.y;
			float num = this.physicsCapsuleCollider.height * 0.5f;
			this.physicsBaseHeight = y + num;
			this.physicsColliderLowerY = y - num;
		}
	}

	// Token: 0x06001BE3 RID: 7139 RVA: 0x000AF42C File Offset: 0x000AD62C
	public void SetScale(float scale)
	{
		Vector3 localScale = new Vector3(scale, scale, scale);
		this.ModelTransform.localScale = localScale;
		foreach (CharacterJoint characterJoint in this.ModelTransform.GetComponentsInChildren<CharacterJoint>())
		{
			if (characterJoint.autoConfigureConnectedAnchor)
			{
				characterJoint.autoConfigureConnectedAnchor = false;
				characterJoint.autoConfigureConnectedAnchor = true;
			}
		}
		if (this.physicsRBT)
		{
			this.physicsBaseHeight *= scale;
			this.physicsHeight *= scale;
			this.physicsColliderLowerY *= scale;
			Collider[] componentsInChildren2 = this.physicsRBT.GetComponentsInChildren<Collider>();
			for (int j = componentsInChildren2.Length - 1; j >= 0; j--)
			{
				Collider collider = componentsInChildren2[j];
				CapsuleCollider capsuleCollider = collider as CapsuleCollider;
				if (capsuleCollider != null)
				{
					capsuleCollider.center *= scale;
					capsuleCollider.height *= scale;
					capsuleCollider.radius *= scale;
				}
				else
				{
					BoxCollider boxCollider = collider as BoxCollider;
					if (boxCollider != null)
					{
						boxCollider.center *= scale;
						boxCollider.size *= scale;
					}
					else
					{
						SphereCollider sphereCollider = collider as SphereCollider;
						if (sphereCollider != null)
						{
							sphereCollider.center *= scale;
							sphereCollider.radius *= scale;
						}
					}
				}
			}
		}
		this.SetCCScale(scale);
	}

	// Token: 0x06001BE4 RID: 7140 RVA: 0x000AF598 File Offset: 0x000AD798
	[PublicizedFrom(EAccessModifier.Protected)]
	public void ReplicateSpeeds()
	{
		int num = this.speedSentTicks - 1;
		this.speedSentTicks = num;
		if (num > 0)
		{
			return;
		}
		float num2 = this.speedForward - this.speedForwardSent;
		float num3 = this.speedStrafe - this.speedStrafeSent;
		if (num2 * num2 + num3 * num3 >= 4.0000004E-06f)
		{
			this.speedSentTicks = 3;
			this.speedForwardSent = this.speedForward;
			this.speedStrafeSent = this.speedStrafe;
			if (this.world.IsRemote())
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEntitySpeeds>().Setup(this), false);
				return;
			}
			this.world.entityDistributer.SendPacketToTrackedPlayers(this.entityId, this.entityId, NetPackageManager.GetPackage<NetPackageEntitySpeeds>().Setup(this), false);
		}
	}

	// Token: 0x06001BE5 RID: 7141 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void SetMovementState()
	{
	}

	// Token: 0x06001BE6 RID: 7142 RVA: 0x000AF650 File Offset: 0x000AD850
	[PublicizedFrom(EAccessModifier.Private)]
	public void animateYaw()
	{
		if (this.yawSeekTimeMax > 0f)
		{
			this.yawSeekTime += Time.deltaTime;
			if (this.yawSeekTime < this.yawSeekTimeMax)
			{
				this.rotation.y = Mathf.Lerp(this.yawSeekAngle, this.yawSeekAngleEnd, this.yawSeekTime / this.yawSeekTimeMax);
				return;
			}
			this.yawSeekTimeMax = 0f;
			this.rotation.y = this.yawSeekAngleEnd;
		}
	}

	// Token: 0x06001BE7 RID: 7143 RVA: 0x000AF6D0 File Offset: 0x000AD8D0
	public void SeekYawToPos(Vector3 _pos, float _yawSlowAt)
	{
		float num = _pos.x - this.position.x;
		float num2 = _pos.z - this.position.z;
		if (num * num + num2 * num2 > 0.0001f)
		{
			float yaw = Mathf.Atan2(num, num2) * 57.29578f;
			this.SeekYaw(yaw, 0f, _yawSlowAt);
		}
	}

	// Token: 0x06001BE8 RID: 7144 RVA: 0x000AF730 File Offset: 0x000AD930
	public float SeekYaw(float yaw, float _, float yawSlowAt)
	{
		if (yaw < 0f)
		{
			yaw += 360f;
		}
		if (yaw > 360f)
		{
			yaw -= 360f;
		}
		if (this.rotation.y < 0f)
		{
			this.rotation.y = this.rotation.y + 360f;
		}
		if (this.rotation.y > 360f)
		{
			this.rotation.y = this.rotation.y - 360f;
		}
		float num = EntityClass.list[this.entityClass].MaxTurnSpeed;
		if (this.inWaterPercent > 0.3f)
		{
			num *= 1f - this.inWaterPercent * 0.5f;
		}
		if (num > 0f)
		{
			float num2 = yaw - this.rotation.y;
			if (num2 != 0f)
			{
				if (num2 < -180f)
				{
					num2 += 360f;
				}
				if (num2 > 180f)
				{
					num2 -= 360f;
				}
				float num3 = Utils.FastAbs(num2);
				if (num3 < yawSlowAt)
				{
					float num4 = num3 / yawSlowAt;
					num = num * num4 * num4;
					num = Utils.FastMax(num, 20f);
				}
				this.yawSeekTime = 0f;
				this.yawSeekTimeMax = num3 / num;
				this.yawSeekAngle = this.rotation.y;
				this.yawSeekAngleEnd = this.rotation.y + num2;
				return num2;
			}
		}
		this.rotation.y = yaw;
		this.yawSeekTimeMax = 0f;
		return 0f;
	}

	// Token: 0x06001BE9 RID: 7145 RVA: 0x000AF89B File Offset: 0x000ADA9B
	public virtual void KillLootContainer()
	{
		this.Kill(DamageResponse.New(true));
	}

	// Token: 0x06001BEA RID: 7146 RVA: 0x000AF8AC File Offset: 0x000ADAAC
	public virtual void Kill(DamageResponse _dmResponse)
	{
		this.SetDead();
		if (this.attachedEntities != null)
		{
			for (int i = 0; i < this.attachedEntities.Length; i++)
			{
				Entity entity = this.attachedEntities[i];
				if (entity != null)
				{
					entity.Kill(_dmResponse);
					entity.Detach();
				}
			}
		}
	}

	// Token: 0x06001BEB RID: 7147 RVA: 0x000AF8FC File Offset: 0x000ADAFC
	[PublicizedFrom(EAccessModifier.Private)]
	public void TickInWater()
	{
		this.inWaterLevel = this.CalcWaterLevel();
		this.inWaterPercent = this.inWaterLevel / (this.GetHeight() * 1.1f);
		this.isInWater = (this.inWaterPercent >= 0.25f);
		bool flag = this.isSwimming;
		this.isSwimming = this.CalcIfSwimming();
		if (this.isSwimming != flag)
		{
			this.SwimChanged();
		}
		bool flag2 = this.isHeadUnderwater;
		this.isHeadUnderwater = this.IsHeadUnderwater();
		if (this.isHeadUnderwater != flag2)
		{
			this.OnHeadUnderwaterStateChanged(this.isHeadUnderwater);
		}
	}

	// Token: 0x06001BEC RID: 7148 RVA: 0x000AF990 File Offset: 0x000ADB90
	public float CalcWaterLevel()
	{
		float num = this.GetHeight() * 1.1f;
		int num2 = Utils.Fastfloor(this.position.y + num);
		int num3 = Utils.Fastfloor(this.position.y);
		int num4 = num2 - num3 + 1;
		int num5 = Utils.Fastfloor(this.position.x);
		int num6 = Utils.Fastfloor(this.position.z);
		int i = -2;
		while (i < 6)
		{
			Vector3i vector3i;
			if (i < 0)
			{
				vector3i.x = num5;
				vector3i.z = num6;
				goto IL_E4;
			}
			vector3i.x = Utils.Fastfloor(this.position.x + Entity.waterLevelDirOffsets[i] * 0.28f);
			vector3i.z = Utils.Fastfloor(this.position.z + Entity.waterLevelDirOffsets[i + 1] * 0.28f);
			if (vector3i.x != num5 || vector3i.z != num6)
			{
				goto IL_E4;
			}
			IL_184:
			i += 2;
			continue;
			IL_E4:
			vector3i.y = num2;
			int num7 = num4;
			float num8;
			for (;;)
			{
				num8 = this.world.GetWaterPercent(vector3i);
				if (num8 > 0f)
				{
					break;
				}
				vector3i.y--;
				if (--num7 <= 0)
				{
					goto IL_184;
				}
			}
			if (num7 == num4)
			{
				vector3i.y++;
				if (this.world.GetWaterPercent(vector3i) == 0f)
				{
					num8 = 0.6f;
				}
				vector3i.y--;
			}
			else
			{
				num8 = 0.6f;
			}
			return Mathf.Clamp((float)vector3i.y + num8 - this.position.y, 0f, num);
		}
		return 0f;
	}

	// Token: 0x06001BED RID: 7149 RVA: 0x000AFB34 File Offset: 0x000ADD34
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool CalcIfSwimming()
	{
		return this.inWaterPercent >= 0.5f;
	}

	// Token: 0x06001BEE RID: 7150 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SwimChanged()
	{
	}

	// Token: 0x06001BEF RID: 7151 RVA: 0x000AFB46 File Offset: 0x000ADD46
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool IsHeadUnderwater()
	{
		return this.inWaterPercent >= 0.9f;
	}

	// Token: 0x06001BF0 RID: 7152 RVA: 0x000AFB58 File Offset: 0x000ADD58
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnHeadUnderwaterStateChanged(bool _bUnderwater)
	{
		if (!_bUnderwater)
		{
			Manager.Play(this, "water_emerge", 1f, false);
		}
	}

	// Token: 0x06001BF1 RID: 7153 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnCollisionForward(Transform t, Collision collision, bool isStay)
	{
	}

	// Token: 0x06001BF2 RID: 7154 RVA: 0x000AFB70 File Offset: 0x000ADD70
	public void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (hit.normal.y > 0.707f && hit.normal.y > this.groundSurface.normal.y && hit.moveDirection.y < 0f)
		{
			if ((double)(hit.point - this.groundSurface.lastHitPoint).sqrMagnitude > 0.001 || this.groundSurface.lastNormal == Vector3.zero)
			{
				this.groundSurface.normal = hit.normal;
			}
			else
			{
				this.groundSurface.normal = this.groundSurface.lastNormal;
			}
			this.groundSurface.hitPoint = hit.point;
		}
	}

	// Token: 0x06001BF3 RID: 7155 RVA: 0x000AFC3F File Offset: 0x000ADE3F
	[PublicizedFrom(EAccessModifier.Private)]
	public void ccEntityCollision(Vector3 _vel)
	{
		this.canCCMove = true;
		this.ccEntityCollisionStart(_vel);
		if (!this.isCCDelayed)
		{
			this.ccEntityCollisionResults();
		}
	}

	// Token: 0x06001BF4 RID: 7156 RVA: 0x000AFC60 File Offset: 0x000ADE60
	[PublicizedFrom(EAccessModifier.Private)]
	public void ccEntityCollisionStart(Vector3 _vel)
	{
		this.groundSurface.lastHitPoint = this.groundSurface.hitPoint;
		this.groundSurface.lastNormal = this.groundSurface.normal;
		this.groundSurface.normal = Vector3.up;
		this.ySize *= this.ConditionalScalePhysicsMulConstant(0.4f);
		if (this.isMotionSlowedDown)
		{
			this.isMotionSlowedDown = false;
			_vel.x *= this.motionMultiplier;
			if (!this.isCollidedVertically)
			{
				_vel.y *= this.motionMultiplier;
			}
			_vel.z *= this.motionMultiplier;
		}
		this.hitMove = _vel;
		this.collisionFlags = CollisionFlags.None;
		if (this.IsStuck)
		{
			this.PhysicsTransform.position += this.hitMove;
			return;
		}
		this.collisionFlags = this.m_characterController.Move(this.hitMove);
	}

	// Token: 0x06001BF5 RID: 7157 RVA: 0x000AFD58 File Offset: 0x000ADF58
	[PublicizedFrom(EAccessModifier.Private)]
	public void ccEntityCollisionResults()
	{
		Vector3 a = this.PhysicsTransform.position;
		this.physicsTargetPos = a;
		a += Origin.position;
		Vector3 vector = a - this.position;
		this.position = a;
		this.boundingBox.center = this.boundingBox.center + vector;
		Vector3 lhs = new Vector3(vector.x, 0f, vector.z);
		Vector3 vector2 = new Vector3(this.motion.x, 0f, this.motion.z);
		this.projectedMove = 0f;
		if (vector2 != Vector3.zero)
		{
			this.projectedMove = Utils.FastClamp01(Vector3.Dot(lhs, vector2) / vector2.sqrMagnitude);
			vector2 *= this.projectedMove;
		}
		if (this.motion.y > 0f)
		{
			if (vector.y >= 0f && vector.y < this.motion.y * 0.95f)
			{
				this.motion.y = 0f;
			}
			else
			{
				this.motion.y = Utils.FastClamp(vector.y, 0f, this.motion.y);
			}
		}
		else
		{
			this.motion.y = Utils.FastClamp(vector.y, this.motion.y, 0f);
		}
		this.motion.x = vector2.x;
		this.motion.z = vector2.z;
		this.isCollidedHorizontally = ((this.collisionFlags & CollisionFlags.Sides) > CollisionFlags.None);
		this.isCollidedVertically = ((this.collisionFlags & (CollisionFlags)6) > CollisionFlags.None);
		this.onGround = this.m_characterController.IsGrounded();
		if (this.onGround)
		{
			this.groundSurface.normal = this.m_characterController.GroundNormal;
		}
		this.world.CheckEntityCollisionWithBlocks(this);
		this.UpdateFall(this.hitMove.y);
	}

	// Token: 0x06001BF6 RID: 7158 RVA: 0x000AFF50 File Offset: 0x000AE150
	[PublicizedFrom(EAccessModifier.Private)]
	public void aabbEntityCollision(Vector3 _vel)
	{
		this.ySize *= 0.4f;
		if (this.isMotionSlowedDown)
		{
			this.isMotionSlowedDown = false;
			_vel.x *= this.motionMultiplier;
			if (!this.isCollidedVertically)
			{
				_vel.y *= this.motionMultiplier;
			}
			_vel.z *= this.motionMultiplier;
			this.motion = Vector3.zero;
		}
		Vector3 vector = _vel;
		Bounds bounds = this.boundingBox;
		if (Math.Abs(_vel.x) <= 0.0001f)
		{
			Math.Abs(_vel.z);
		}
		this.collAABB.Clear();
		Bounds aabb = BoundsUtils.ExpandDirectional(this.boundingBox, vector);
		this.world.GetCollidingBounds(this, aabb, this.collAABB);
		Vector3 vector2 = BoundsUtils.ClipBoundsMove(this.boundingBox, vector, this.collAABB, this.collAABB.Count);
		this.boundingBox.center = this.boundingBox.center + vector2;
		bool flag = this.onGround || (vector.y != vector2.y && vector.y < 0f);
		if (this.stepHeight > 0f && flag && this.ySize < 0.05f && (vector.x != vector2.x || vector.z != vector2.z))
		{
			Vector3 vector3 = vector2;
			vector2 = vector;
			vector2.y = this.stepHeight;
			Bounds bounds2 = this.boundingBox;
			this.boundingBox = bounds;
			this.collAABB.Clear();
			aabb = BoundsUtils.ExpandDirectional(this.boundingBox, new Vector3(vector2.x, 0f, vector2.z));
			this.world.GetCollidingBounds(this, aabb, this.collAABB);
			vector2 = BoundsUtils.ClipBoundsMove(this.boundingBox, vector2, this.collAABB, this.collAABB.Count);
			this.boundingBox.center = this.boundingBox.center + vector2;
			float y = BoundsUtils.ClipBoundsMoveY(this.boundingBox.min, this.boundingBox.max, -this.stepHeight, this.collAABB, this.collAABB.Count);
			this.boundingBox.center = this.boundingBox.center + new Vector3(0f, y, 0f);
			vector2.y = y;
			if (vector3.x * vector3.x + vector3.z * vector3.z >= vector2.x * vector2.x + vector2.z * vector2.z)
			{
				vector2 = vector3;
				this.boundingBox = bounds2;
			}
			else if (this.boundingBox.min.y - (float)((int)this.boundingBox.min.y) > 0f)
			{
				this.ySize += this.boundingBox.min.y - bounds2.min.y;
			}
		}
		Vector3 center = this.boundingBox.center;
		this.position.x = center.x;
		this.position.y = this.boundingBox.min.y + this.yOffset - this.ySize;
		this.position.z = center.z;
		if (this.PhysicsTransform != null && (this.PhysicsTransform.position - (this.position - Origin.position)).sqrMagnitude > 0.0001f)
		{
			this.PhysicsTransform.position = this.position - Origin.position;
		}
		this.isCollidedHorizontally = (vector.x != vector2.x || vector.z != vector2.z);
		this.isCollidedVertically = (vector.y != vector2.y);
		this.onGround = (vector.y != vector2.y && vector.y < 0f);
		this.world.CheckEntityCollisionWithBlocks(this);
		this.UpdateFall(vector2.y);
		if (vector.x != vector2.x)
		{
			this.motion.x = 0f;
		}
		if (vector.y != vector2.y)
		{
			this.motion.y = 0f;
		}
		if (vector.z != vector2.z)
		{
			this.motion.z = 0f;
		}
	}

	// Token: 0x06001BF7 RID: 7159 RVA: 0x000B03E3 File Offset: 0x000AE5E3
	[PublicizedFrom(EAccessModifier.Protected)]
	public void CalcFixedUpdateTimeScaleConstants()
	{
		this.kAddFixedUpdateTimeScale = Time.deltaTime / 0.05f;
	}

	// Token: 0x06001BF8 RID: 7160 RVA: 0x000B03F6 File Offset: 0x000AE5F6
	public float ScalePhysicsMulConstant(float tickMulDelta)
	{
		return Mathf.Pow(tickMulDelta, this.kAddFixedUpdateTimeScale);
	}

	// Token: 0x06001BF9 RID: 7161 RVA: 0x000B0404 File Offset: 0x000AE604
	public float ScalePhysicsAddConstant(float tickAddDelta)
	{
		return this.kAddFixedUpdateTimeScale * tickAddDelta;
	}

	// Token: 0x06001BFA RID: 7162 RVA: 0x000B040E File Offset: 0x000AE60E
	public float ConditionalScalePhysicsMulConstant(float tickMulDelta)
	{
		return tickMulDelta;
	}

	// Token: 0x06001BFB RID: 7163 RVA: 0x000B040E File Offset: 0x000AE60E
	public float ConditionalScalePhysicsAddConstant(float tickAddDelta)
	{
		return tickAddDelta;
	}

	// Token: 0x06001BFC RID: 7164 RVA: 0x000B0414 File Offset: 0x000AE614
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void entityCollision(Vector3 _motion)
	{
		if (this.emodel.IsRagdollMovement)
		{
			if (this.emodel.pelvisRB)
			{
				float num = this.emodel.bipedPelvisTransform.position.y + Origin.position.y;
				Vector3 velocity = this.emodel.pelvisRB.velocity;
				if (velocity.y < -1f)
				{
					this.fallVelY = Utils.FastMin(this.fallVelY, velocity.y);
					float num2 = this.fallLastY - num;
					if (num2 > 0f)
					{
						this.fallDistance += num2;
					}
				}
				else if (this.fallDistance > 0f)
				{
					this.fallLastMotion.y = this.fallVelY * 0.05f;
					this.onGround = true;
					this.UpdateFall(0f);
				}
				this.fallLastY = num;
			}
			return;
		}
		this.ApplyFixedUpdate();
		if (this.m_characterController != null)
		{
			this.ccEntityCollision(_motion);
			return;
		}
		this.aabbEntityCollision(_motion);
	}

	// Token: 0x06001BFD RID: 7165 RVA: 0x000B0518 File Offset: 0x000AE718
	public virtual void SetMotionMultiplier(float _motionMultiplier)
	{
		this.isMotionSlowedDown = true;
		this.motionMultiplier = _motionMultiplier;
		if (this.motionMultiplier < 0.5f)
		{
			this.fallDistance = 0f;
		}
	}

	// Token: 0x06001BFE RID: 7166 RVA: 0x000B0540 File Offset: 0x000AE740
	public float GetDistance(Entity _other)
	{
		return (this.position - _other.position).magnitude;
	}

	// Token: 0x06001BFF RID: 7167 RVA: 0x000B0568 File Offset: 0x000AE768
	public float GetDistanceSq(Entity _other)
	{
		return (this.position - _other.position).sqrMagnitude;
	}

	// Token: 0x06001C00 RID: 7168 RVA: 0x000B0590 File Offset: 0x000AE790
	public float GetDistanceSq(Vector3 _pos)
	{
		return (this.position - _pos).sqrMagnitude;
	}

	// Token: 0x06001C01 RID: 7169 RVA: 0x000B05B4 File Offset: 0x000AE7B4
	public float GetSoundTravelTime(Vector3 _otherPos)
	{
		return (this.position - _otherPos).magnitude / 343f;
	}

	// Token: 0x06001C02 RID: 7170 RVA: 0x000B05DB File Offset: 0x000AE7DB
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsInWater()
	{
		return this.isInWater;
	}

	// Token: 0x06001C03 RID: 7171 RVA: 0x000B05E3 File Offset: 0x000AE7E3
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsSwimming()
	{
		return this.isSwimming;
	}

	// Token: 0x06001C04 RID: 7172 RVA: 0x000B05EB File Offset: 0x000AE7EB
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsInElevator()
	{
		return this.bInElevator;
	}

	// Token: 0x06001C05 RID: 7173 RVA: 0x000B05F3 File Offset: 0x000AE7F3
	public void SetInElevator(bool _b)
	{
		this.bInElevator = _b;
	}

	// Token: 0x06001C06 RID: 7174 RVA: 0x000B05FC File Offset: 0x000AE7FC
	public virtual bool IsAirBorne()
	{
		return this.bAirBorne || !this.onGround;
	}

	// Token: 0x06001C07 RID: 7175 RVA: 0x000B0611 File Offset: 0x000AE811
	public void SetAirBorne(bool _b)
	{
		this.bAirBorne = _b;
	}

	// Token: 0x1700032C RID: 812
	// (get) Token: 0x06001C08 RID: 7176 RVA: 0x000B061A File Offset: 0x000AE81A
	public float width
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.scaledExtent.x * 2f;
		}
	}

	// Token: 0x1700032D RID: 813
	// (get) Token: 0x06001C09 RID: 7177 RVA: 0x000B062D File Offset: 0x000AE82D
	public float depth
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.scaledExtent.z * 2f;
		}
	}

	// Token: 0x1700032E RID: 814
	// (get) Token: 0x06001C0A RID: 7178 RVA: 0x000B0640 File Offset: 0x000AE840
	public float height
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.scaledExtent.y * 2f;
		}
	}

	// Token: 0x06001C0B RID: 7179 RVA: 0x0002E7B0 File Offset: 0x0002C9B0
	public virtual float GetEyeHeight()
	{
		return 0f;
	}

	// Token: 0x06001C0C RID: 7180 RVA: 0x000B0653 File Offset: 0x000AE853
	public virtual float GetHeight()
	{
		if (this.m_characterController != null)
		{
			return this.m_characterController.GetHeight();
		}
		return this.height;
	}

	// Token: 0x1700032F RID: 815
	// (get) Token: 0x06001C0D RID: 7181 RVA: 0x000B066F File Offset: 0x000AE86F
	public float radius
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.scaledExtent.x;
		}
	}

	// Token: 0x06001C0E RID: 7182 RVA: 0x000B067C File Offset: 0x000AE87C
	public virtual void Move(Vector3 _direction, bool _isDirAbsolute, float _velocity, float _maxVelocity)
	{
		if (!this.IsClientControlled() && (GamePrefs.GetBool(EnumGamePrefs.DebugStopEnemiesMoving) || GameStats.GetInt(EnumGameStats.GameState) == 2))
		{
			return;
		}
		float y = _direction.y;
		_direction.y = 0f;
		_direction.Normalize();
		if (_isDirAbsolute)
		{
			float num = Mathf.Clamp(_maxVelocity - Mathf.Max(0f, Vector3.Dot(this.motion, _direction)), 0f, _velocity);
			this.motion.x = this.motion.x + this.ConditionalScalePhysicsAddConstant(_direction.x * num);
			this.motion.y = this.motion.y + this.ConditionalScalePhysicsAddConstant(_direction.y * _velocity);
			this.motion.z = this.motion.z + this.ConditionalScalePhysicsAddConstant(_direction.z * num);
			return;
		}
		Vector3 rhs = base.transform.forward * _direction.z + base.transform.right * _direction.x;
		rhs.Normalize();
		float num2 = Mathf.Clamp(_maxVelocity - Mathf.Max(0f, Vector3.Dot(this.motion, rhs)), 0f, _velocity);
		this.motion += base.transform.forward * this.ConditionalScalePhysicsAddConstant(_direction.z * num2) + base.transform.right * this.ConditionalScalePhysicsAddConstant(_direction.x * num2) + base.transform.up * this.ConditionalScalePhysicsAddConstant(y * _velocity);
	}

	// Token: 0x06001C0F RID: 7183 RVA: 0x000B080C File Offset: 0x000AEA0C
	public bool IsAlive()
	{
		return !this.IsDead();
	}

	// Token: 0x06001C10 RID: 7184 RVA: 0x000B0817 File Offset: 0x000AEA17
	public bool WasAlive()
	{
		return !this.WasDead();
	}

	// Token: 0x06001C11 RID: 7185 RVA: 0x000B0822 File Offset: 0x000AEA22
	public virtual bool IsDead()
	{
		return this.bDead;
	}

	// Token: 0x06001C12 RID: 7186 RVA: 0x000B082A File Offset: 0x000AEA2A
	public bool WasDead()
	{
		return this.bWasDead;
	}

	// Token: 0x06001C13 RID: 7187 RVA: 0x000B0834 File Offset: 0x000AEA34
	public virtual void SetDead()
	{
		this.bDead = true;
		Manager.DestroySoundsForEntity(this.entityId);
		if (this.m_marker != null)
		{
			this.m_marker.Release();
			this.m_marker = null;
		}
		if (this.PhysicsTransform != null)
		{
			if (this.emodel.HasRagdoll())
			{
				this.PhysicsTransform.gameObject.layer = 17;
			}
			else
			{
				this.PhysicsTransform.gameObject.layer = 14;
			}
		}
		if (this.physicsRBT)
		{
			this.physicsRBT.gameObject.SetActive(false);
		}
		if (this.emodel != null)
		{
			this.emodel.SetDead();
		}
	}

	// Token: 0x06001C14 RID: 7188 RVA: 0x000B08E8 File Offset: 0x000AEAE8
	public virtual void SetAlive()
	{
		this.bDead = false;
		if (this.PhysicsTransform != null)
		{
			if (this is EntityPlayerLocal)
			{
				this.PhysicsTransform.gameObject.layer = 20;
				return;
			}
			this.PhysicsTransform.gameObject.layer = 15;
		}
	}

	// Token: 0x06001C15 RID: 7189 RVA: 0x000B0938 File Offset: 0x000AEB38
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateFall(float mY)
	{
		if (this.onGround)
		{
			if (this.fallDistance > 0f)
			{
				this.fallHitGround(this.fallDistance, this.fallLastMotion);
				this.fallDistance = 0f;
				return;
			}
		}
		else if (mY < 0f)
		{
			this.fallLastMotion = this.motion;
			this.fallDistance -= mY;
		}
	}

	// Token: 0x06001C16 RID: 7190 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void fallHitGround(float _v, Vector3 _fallMotion)
	{
	}

	// Token: 0x06001C17 RID: 7191 RVA: 0x000B099C File Offset: 0x000AEB9C
	public virtual void OnRagdoll(bool isActive)
	{
		if (isActive && this.emodel.bipedPelvisTransform)
		{
			this.fallLastY = this.emodel.bipedPelvisTransform.position.y + Origin.position.y;
			this.fallVelY = 0f;
		}
	}

	// Token: 0x06001C18 RID: 7192 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool CanDamageEntity(int _sourceEntityId)
	{
		return true;
	}

	// Token: 0x06001C19 RID: 7193 RVA: 0x000B09EF File Offset: 0x000AEBEF
	public virtual int DamageEntity(DamageSource _damageSource, int _strength, bool _criticalHit, float impulseScale = 1f)
	{
		this.setBeenAttacked();
		return 0;
	}

	// Token: 0x06001C1A RID: 7194 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public void setBeenAttacked()
	{
	}

	// Token: 0x06001C1B RID: 7195 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void FireAttackedEvents(DamageResponse dmResponse)
	{
	}

	// Token: 0x06001C1C RID: 7196 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void ProcessDamageResponse(DamageResponse _dmResponse)
	{
	}

	// Token: 0x06001C1D RID: 7197 RVA: 0x000B09F8 File Offset: 0x000AEBF8
	public Bounds getBoundingBox()
	{
		return this.boundingBox;
	}

	// Token: 0x06001C1E RID: 7198 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnDamagedByExplosion()
	{
	}

	// Token: 0x06001C1F RID: 7199 RVA: 0x000B0A00 File Offset: 0x000AEC00
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnPushEntity(Entity _entity)
	{
		Vector3 vector = _entity.position - this.position;
		float num = Utils.FastMax(Mathf.Abs(vector.x), Mathf.Abs(vector.z));
		if (num >= 0.01f)
		{
			num = Mathf.Sqrt(num);
			float num2 = 1f / num;
			vector.x *= num2;
			vector.z *= num2;
			if (num2 < 1f)
			{
				vector.x *= num2;
				vector.z *= num2;
			}
			float num3 = 0.05f * (1f - this.entityCollisionReduction);
			num3 *= Utils.FastMin(_entity.GetWeight(), this.GetWeight()) / Utils.FastMax(_entity.GetWeight(), this.GetWeight());
			vector.x *= num3;
			vector.z *= num3;
			this.AddVelocity(new Vector3(-vector.x, 0f, -vector.z));
			if (_entity.CanBePushed())
			{
				_entity.AddVelocity(new Vector3(vector.x, 0f, vector.z));
			}
		}
	}

	// Token: 0x06001C20 RID: 7200 RVA: 0x000B0B20 File Offset: 0x000AED20
	public virtual void AddVelocity(Vector3 _vel)
	{
		this.motion += _vel;
		this.SetAirBorne(true);
	}

	// Token: 0x06001C21 RID: 7201 RVA: 0x000B0B3C File Offset: 0x000AED3C
	public virtual Vector3 GetVelocityPerSecond()
	{
		if (this.AttachedToEntity)
		{
			return this.AttachedToEntity.GetVelocityPerSecond();
		}
		if (this.physicsRB)
		{
			return this.physicsRB.velocity;
		}
		return this.motion * 20f;
	}

	// Token: 0x06001C22 RID: 7202 RVA: 0x000B0B8B File Offset: 0x000AED8B
	public virtual Vector3 GetAngularVelocityPerSecond()
	{
		if (this.AttachedToEntity)
		{
			return this.AttachedToEntity.GetAngularVelocityPerSecond();
		}
		if (this.physicsRB)
		{
			return this.physicsRB.angularVelocity;
		}
		return Vector3.zero;
	}

	// Token: 0x06001C23 RID: 7203 RVA: 0x000B0BC4 File Offset: 0x000AEDC4
	public virtual void SetVelocityPerSecond(Vector3 vel, Vector3 angularVel)
	{
		if (this.AttachedToEntity)
		{
			this.AttachedToEntity.SetVelocityPerSecond(vel, angularVel);
			return;
		}
		this.physicsVel = vel;
		this.physicsAngVel = angularVel;
		if (this.isPhysicsMaster && this.physicsRB)
		{
			this.physicsRB.velocity = vel;
			this.physicsRB.angularVelocity = angularVel;
		}
		this.motion = vel * 0.05f;
	}

	// Token: 0x06001C24 RID: 7204 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool CanBePushed()
	{
		return false;
	}

	// Token: 0x06001C25 RID: 7205 RVA: 0x0002E7B0 File Offset: 0x0002C9B0
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual float GetPushBoundsVertical()
	{
		return 0f;
	}

	// Token: 0x06001C26 RID: 7206 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool CanCollideWith(Entity _other)
	{
		return true;
	}

	// Token: 0x06001C27 RID: 7207 RVA: 0x000B0C38 File Offset: 0x000AEE38
	public virtual void OnUpdatePosition(float _partialTicks)
	{
		this.ticksExisted++;
		this.prevPos = this.position;
		this.prevRotation = this.rotation;
		if (this.isUpdatePosition)
		{
			if (this.AttachedToEntity || (this.emodel && this.emodel.IsRagdollOn))
			{
				this.isUpdatePosition = false;
			}
			else
			{
				switch (this.positionUpdateMovementType)
				{
				case Entity.EnumPositionUpdateMovementType.Lerp:
					this.SetPosition(Vector3.Lerp(this.position, this.targetPos, Time.deltaTime / Time.fixedDeltaTime * Entity.tickPositionLerpMultiplier), false);
					goto IL_D6;
				case Entity.EnumPositionUpdateMovementType.MoveTowards:
					this.SetPosition(Vector3.MoveTowards(this.position, this.targetPos, Entity.tickPositionMoveTowardsMaxDistance), false);
					goto IL_D6;
				}
				this.SetPosition(this.targetPos, false);
				IL_D6:
				if (this.position == this.targetPos)
				{
					this.isUpdatePosition = false;
				}
				if (this.PhysicsTransform != null)
				{
					this.physicsTargetPos = this.position - Origin.position;
					this.PhysicsTransform.position = this.physicsTargetPos;
				}
			}
		}
		if (this.interpolateTargetQRot > 0)
		{
			this.qrotation = Quaternion.Lerp(this.qrotation, this.targetQRot, 1f / (float)this.interpolateTargetQRot);
			this.interpolateTargetQRot--;
		}
		if (this.interpolateTargetRot > 0)
		{
			float t = 1f / (float)this.interpolateTargetRot;
			this.SetRotation(new Vector3(Mathf.LerpAngle(this.rotation.x, this.targetRot.x, t), Mathf.LerpAngle(this.rotation.y, this.targetRot.y, t), Mathf.LerpAngle(this.rotation.z, this.targetRot.z, t)));
			this.interpolateTargetRot--;
		}
		if (!this.isEntityRemote && !this.IsDead() && !this.IsClientControlled() && this.position.y < 0f && this.IsDeadIfOutOfWorld())
		{
			EntityDrone entityDrone = this as EntityDrone;
			if (entityDrone)
			{
				entityDrone.NotifyOffTheWorld();
				return;
			}
			Log.Warning(string.Concat(new string[]
			{
				"Entity ",
				(this != null) ? this.ToString() : null,
				" fell off the world, id=",
				this.entityId.ToString(),
				" pos=",
				this.position.ToCultureInvariantString()
			}));
			this.MarkToUnload();
		}
	}

	// Token: 0x06001C28 RID: 7208 RVA: 0x000B0ED4 File Offset: 0x000AF0D4
	public virtual void CheckPosition()
	{
		if (float.IsNaN(this.position.x) || float.IsInfinity(this.position.x))
		{
			this.position.x = this.lastTickPos[0].x;
		}
		if (float.IsNaN(this.position.y) || float.IsInfinity(this.position.y))
		{
			this.position.y = this.lastTickPos[0].y;
		}
		if (float.IsNaN(this.position.z) || float.IsInfinity(this.position.z))
		{
			this.position.z = this.lastTickPos[0].z;
		}
		if (float.IsNaN(this.rotation.x) || float.IsInfinity(this.rotation.x))
		{
			this.rotation.x = this.prevRotation.x;
		}
		if (float.IsNaN(this.rotation.y) || float.IsInfinity(this.rotation.y))
		{
			this.rotation.y = this.prevRotation.y;
		}
		if (float.IsNaN(this.rotation.z) || float.IsInfinity(this.rotation.z))
		{
			this.rotation.z = this.prevRotation.z;
		}
	}

	// Token: 0x06001C29 RID: 7209 RVA: 0x000B1050 File Offset: 0x000AF250
	public virtual void OnUpdateEntity()
	{
		bool flag = this.isInWater;
		if (!this.isEntityStatic())
		{
			this.TickInWater();
		}
		if (this.isEntityRemote)
		{
			return;
		}
		if (this.isInWater)
		{
			if (!flag && !this.firstUpdate && this.fallDistance > 1f)
			{
				this.PlayOneShot("waterfallinginto", false, false, false, null);
			}
			this.fallDistance = 0f;
		}
		if (!this.RootMotion && !this.IsDead() && this.CanBePushed())
		{
			List<Entity> entitiesInBounds = this.world.GetEntitiesInBounds(this, BoundsUtils.ExpandBounds(this.boundingBox, 0.2f, this.GetPushBoundsVertical(), 0.2f));
			if (entitiesInBounds != null && entitiesInBounds.Count > 0)
			{
				for (int i = 0; i < entitiesInBounds.Count; i++)
				{
					Entity entity = entitiesInBounds[i];
					this.OnPushEntity(entity);
				}
			}
		}
		this.firstUpdate = false;
	}

	// Token: 0x06001C2A RID: 7210 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnAddedToWorld()
	{
	}

	// Token: 0x06001C2B RID: 7211 RVA: 0x000B1128 File Offset: 0x000AF328
	public virtual void OnEntityUnload()
	{
		if (this.isUnloaded)
		{
			Log.Warning("OnEntityUnload already unloaded {0} ", new object[]
			{
				this.GetDebugName()
			});
			return;
		}
		this.isUnloaded = true;
		Manager.DestroySoundsForEntity(this.entityId);
		if (this.movableChunkObserver != null)
		{
			this.movableChunkObserver.Dispose();
			this.movableChunkObserver = null;
		}
		if (this.attachedEntities != null)
		{
			for (int i = 0; i < this.attachedEntities.Length; i++)
			{
				Entity entity = this.attachedEntities[i];
				if (entity != null)
				{
					entity.Detach();
				}
			}
		}
		if (this.AttachedToEntity != null)
		{
			this.Detach();
		}
		if (this.emodel != null)
		{
			this.emodel.OnUnload();
		}
		try
		{
			UnityEngine.Object.Destroy(this.RootTransform.gameObject);
		}
		catch (Exception e)
		{
			Log.Error("OnEntityUnload: {0}", new object[]
			{
				this.GetDebugName()
			});
			Log.Exception(e);
		}
	}

	// Token: 0x06001C2C RID: 7212 RVA: 0x000B1228 File Offset: 0x000AF428
	public virtual float GetLightBrightness()
	{
		Vector3i blockPosition = this.GetBlockPosition();
		Vector3i blockPos = blockPosition;
		blockPos.y += Mathf.RoundToInt(this.height + 0.5f);
		return Utils.FastMax(this.world.GetLightBrightness(blockPosition), this.world.GetLightBrightness(blockPos));
	}

	// Token: 0x06001C2D RID: 7213 RVA: 0x000B1277 File Offset: 0x000AF477
	public Vector3i GetBlockPosition()
	{
		return World.worldToBlockPos(this.position);
	}

	// Token: 0x06001C2E RID: 7214 RVA: 0x000B1284 File Offset: 0x000AF484
	public virtual void InitLocation(Vector3 _pos, Vector3 _rot)
	{
		this.serverPos = NetEntityDistributionEntry.EncodePos(_pos);
		this.SetPosition(_pos, true);
		this.SetRotation(_rot);
		this.SetPosAndRotFromNetwork(_pos, _rot, 0);
		this.ResetLastTickPos(_pos);
		base.transform.SetPositionAndRotation(this.position - Origin.position, Quaternion.Euler(this.rotation));
	}

	// Token: 0x06001C2F RID: 7215 RVA: 0x000B12E2 File Offset: 0x000AF4E2
	public Vector3 GetPosition()
	{
		return this.position;
	}

	// Token: 0x06001C30 RID: 7216 RVA: 0x000B12EC File Offset: 0x000AF4EC
	public virtual void SetPosition(Vector3 _pos, bool _bUpdatePhysics = true)
	{
		this.position = _pos;
		float num = this.width * 0.5f;
		float num2 = this.depth * 0.5f;
		float num3 = _pos.y - this.yOffset + this.ySize;
		this.boundingBox = BoundsUtils.BoundsForMinMax(_pos.x - num, num3, _pos.z - num2, _pos.x + num, num3 + this.height, _pos.z + num2);
		if (this.attachedEntities != null)
		{
			for (int i = 0; i < this.attachedEntities.Length; i++)
			{
				Entity entity = this.attachedEntities[i];
				if (entity != null)
				{
					entity.SetPosition(_pos, false);
				}
			}
		}
		if (_bUpdatePhysics && this.PhysicsTransform != null)
		{
			this.PhysicsTransform.position = _pos - Origin.position;
			if (this.physicsRBT)
			{
				this.physicsPos = _pos - Origin.position + this.physicsBasePos;
				this.physicsRBT.position = this.physicsPos;
				this.physicsTargetPos = this.PhysicsTransform.position;
			}
		}
	}

	// Token: 0x06001C31 RID: 7217 RVA: 0x000B140D File Offset: 0x000AF60D
	public void SetRotationAndStopTurning(Vector3 _rot)
	{
		this.SetRotation(_rot);
		this.yawSeekTimeMax = 0f;
		this.interpolateTargetQRot = 0;
		this.interpolateTargetRot = 0;
	}

	// Token: 0x06001C32 RID: 7218 RVA: 0x000B142F File Offset: 0x000AF62F
	public virtual void SetRotation(Vector3 _rot)
	{
		this.rotation = _rot;
		this.qrotation = Quaternion.Euler(_rot);
	}

	// Token: 0x06001C33 RID: 7219 RVA: 0x000B1444 File Offset: 0x000AF644
	public void SetPosAndRotFromNetwork(Vector3 _pos, Vector3 _rot, int _steps)
	{
		this.targetPos = _pos;
		this.targetRot = _rot;
		this.isUpdatePosition = true;
		this.interpolateTargetRot = _steps;
	}

	// Token: 0x06001C34 RID: 7220 RVA: 0x000B1462 File Offset: 0x000AF662
	public void SetPosAndQRotFromNetwork(Vector3 _pos, Quaternion _rot, int _steps)
	{
		this.targetPos = _pos;
		this.targetQRot = _rot;
		this.isUpdatePosition = true;
		this.interpolateTargetQRot = _steps;
	}

	// Token: 0x06001C35 RID: 7221 RVA: 0x000B1480 File Offset: 0x000AF680
	public void SetRotFromNetwork(Vector3 _rot, int _steps)
	{
		this.targetRot = _rot;
		this.interpolateTargetRot = _steps;
	}

	// Token: 0x06001C36 RID: 7222 RVA: 0x000B1490 File Offset: 0x000AF690
	public void SetQRotFromNetwork(Quaternion _qrot, int _steps)
	{
		this.targetQRot = _qrot;
		this.interpolateTargetQRot = _steps;
	}

	// Token: 0x06001C37 RID: 7223 RVA: 0x000B14A0 File Offset: 0x000AF6A0
	public float GetBrightness(float _t)
	{
		int num = Utils.Fastfloor(this.position.x);
		int num2 = Utils.Fastfloor(this.position.z);
		if (this.world.GetChunkSync(World.toChunkXZ(num), World.toChunkXZ(num2)) != null)
		{
			float num3 = (this.boundingBox.max.y - this.boundingBox.min.y) * 0.66f;
			int y = Utils.Fastfloor((double)this.position.y - (double)this.yOffset + (double)num3);
			return this.world.GetLightBrightness(new Vector3i(num, y, num2));
		}
		return 0f;
	}

	// Token: 0x06001C38 RID: 7224 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void VisiblityCheck(float _distanceSqr, bool _masterIsZooming)
	{
	}

	// Token: 0x06001C39 RID: 7225 RVA: 0x000B1546 File Offset: 0x000AF746
	public void SetIgnoredByAI(bool ignore)
	{
		this.isIgnoredByAI = ignore;
	}

	// Token: 0x06001C3A RID: 7226 RVA: 0x000B154F File Offset: 0x000AF74F
	public virtual bool IsIgnoredByAI()
	{
		return this.isIgnoredByAI;
	}

	// Token: 0x06001C3B RID: 7227 RVA: 0x000B1557 File Offset: 0x000AF757
	public virtual Vector3 getHeadPosition()
	{
		if (this.emodel == null)
		{
			return this.position + new Vector3(0f, this.GetEyeHeight(), 0f);
		}
		return this.emodel.GetHeadPosition();
	}

	// Token: 0x06001C3C RID: 7228 RVA: 0x000B1593 File Offset: 0x000AF793
	public virtual Vector3 getNavObjectPosition()
	{
		if (this.emodel == null)
		{
			return this.position + new Vector3(0f, this.GetEyeHeight(), 0f);
		}
		return this.emodel.GetNavObjectPosition();
	}

	// Token: 0x06001C3D RID: 7229 RVA: 0x000B15D0 File Offset: 0x000AF7D0
	public virtual Vector3 getBellyPosition()
	{
		if (this.emodel == null)
		{
			return this.position + new Vector3(0f, this.GetEyeHeight() / 2f, 0f);
		}
		return this.emodel.GetBellyPosition();
	}

	// Token: 0x06001C3E RID: 7230 RVA: 0x000B1620 File Offset: 0x000AF820
	public virtual Vector3 getHipPosition()
	{
		if (this.emodel == null)
		{
			return this.position + new Vector3(0f, this.GetEyeHeight() / 2f, 0f);
		}
		return this.emodel.GetHipPosition();
	}

	// Token: 0x06001C3F RID: 7231 RVA: 0x000B1670 File Offset: 0x000AF870
	public virtual Vector3 getChestPosition()
	{
		if (this.emodel == null)
		{
			return this.position + new Vector3(0f, this.GetEyeHeight() / 2.4f, 0f);
		}
		return this.emodel.GetChestPosition();
	}

	// Token: 0x06001C40 RID: 7232 RVA: 0x000B16BD File Offset: 0x000AF8BD
	public void SetVelocity(Vector3 _vel)
	{
		this.motion = _vel;
	}

	// Token: 0x06001C41 RID: 7233 RVA: 0x0003E2E0 File Offset: 0x0003C4E0
	public virtual float GetWeight()
	{
		return 1f;
	}

	// Token: 0x06001C42 RID: 7234 RVA: 0x0003E2E0 File Offset: 0x0003C4E0
	public virtual float GetPushFactor()
	{
		return 1f;
	}

	// Token: 0x06001C43 RID: 7235 RVA: 0x0003E2E0 File Offset: 0x0003C4E0
	public virtual float GetSightDetectionScale()
	{
		return 1f;
	}

	// Token: 0x06001C44 RID: 7236 RVA: 0x000B16C6 File Offset: 0x000AF8C6
	public virtual void OnLoadedFromEntityCache(EntityCreationData _ed)
	{
		if (this.bIsChunkObserver && !this.isEntityRemote)
		{
			this.movableChunkObserver = new MovableSharedChunkObserver(this.world.m_SharedChunkObserverCache);
			this.movableChunkObserver.SetPosition(this.position);
		}
	}

	// Token: 0x06001C45 RID: 7237 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool IsSavedToNetwork()
	{
		return true;
	}

	// Token: 0x06001C46 RID: 7238 RVA: 0x000B16FF File Offset: 0x000AF8FF
	public virtual bool IsSavedToFile()
	{
		return !this.world.IsEditor() || !GameManager.Instance.GetDynamicPrefabDecorator().IsEntityInPrefab(this.entityId);
	}

	// Token: 0x06001C47 RID: 7239 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SetEntityName(string _name)
	{
	}

	// Token: 0x06001C48 RID: 7240 RVA: 0x000B1728 File Offset: 0x000AF928
	public virtual void CopyPropertiesFromEntityClass()
	{
		EntityClass entityClass = EntityClass.list[this.entityClass];
		this.RootMotion = entityClass.RootMotion;
		this.HasDeathAnim = entityClass.HasDeathAnim;
		this.entityFlags = entityClass.entityFlags;
		this.entityType = EntityType.Unknown;
		entityClass.Properties.ParseEnum<EntityType>(EntityClass.PropEntityType, ref this.entityType);
		entityClass.Properties.ParseFloat(EntityClass.PropLootDropProb, ref this.lootDropProb);
		this.lootListOnDeath = entityClass.Properties.GetString(EntityClass.PropLootListOnDeath);
		entityClass.Properties.ParseString(EntityClass.PropLootListAlive, ref this.lootListAlive);
		entityClass.Properties.ParseString(EntityClass.PropMapIcon, ref this.mapIcon);
		entityClass.Properties.ParseString(EntityClass.PropCompassIcon, ref this.compassIcon);
		entityClass.Properties.ParseString(EntityClass.PropCompassUpIcon, ref this.compassUpIcon);
		entityClass.Properties.ParseString(EntityClass.PropCompassDownIcon, ref this.compassDownIcon);
		entityClass.Properties.ParseString(EntityClass.PropTrackerIcon, ref this.trackerIcon);
		entityClass.Properties.ParseBool(EntityClass.PropRotateToGround, ref this.isRotateToGround);
	}

	// Token: 0x06001C49 RID: 7241 RVA: 0x000B184D File Offset: 0x000AFA4D
	public virtual string GetLootList()
	{
		return this.lootListAlive;
	}

	// Token: 0x06001C4A RID: 7242 RVA: 0x000B1855 File Offset: 0x000AFA55
	public virtual void MarkToUnload()
	{
		this.markedForUnload = true;
	}

	// Token: 0x06001C4B RID: 7243 RVA: 0x000B185E File Offset: 0x000AFA5E
	public virtual bool IsMarkedForUnload()
	{
		return this.markedForUnload || this.IsDead();
	}

	// Token: 0x06001C4C RID: 7244 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool IsSpawned()
	{
		return true;
	}

	// Token: 0x06001C4D RID: 7245 RVA: 0x000B1870 File Offset: 0x000AFA70
	public void ResetLastTickPos(Vector3 _pos)
	{
		for (int i = 0; i < this.lastTickPos.Length; i++)
		{
			this.lastTickPos[i] = _pos;
		}
	}

	// Token: 0x06001C4E RID: 7246 RVA: 0x000B18A0 File Offset: 0x000AFAA0
	public void SetLastTickPos(Vector3 _pos)
	{
		for (int i = this.lastTickPos.Length - 1; i > 0; i--)
		{
			this.lastTickPos[i] = this.lastTickPos[i - 1];
		}
		this.lastTickPos[0] = _pos;
	}

	// Token: 0x06001C4F RID: 7247 RVA: 0x0000FB42 File Offset: 0x0000DD42
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool isDetailedHeadBodyColliders()
	{
		return false;
	}

	// Token: 0x06001C50 RID: 7248 RVA: 0x00019766 File Offset: 0x00017966
	public virtual Transform GetModelTransform()
	{
		return null;
	}

	// Token: 0x06001C51 RID: 7249 RVA: 0x000B18E9 File Offset: 0x000AFAE9
	public virtual Vector3 GetMapIconScale()
	{
		return new Vector3(1f, 1f, 1f);
	}

	// Token: 0x06001C52 RID: 7250 RVA: 0x000B18FF File Offset: 0x000AFAFF
	public virtual string GetMapIcon()
	{
		return this.mapIcon;
	}

	// Token: 0x06001C53 RID: 7251 RVA: 0x000B1907 File Offset: 0x000AFB07
	public virtual string GetCompassIcon()
	{
		if (this.compassIcon == null)
		{
			return this.mapIcon;
		}
		return this.compassIcon;
	}

	// Token: 0x06001C54 RID: 7252 RVA: 0x000B191E File Offset: 0x000AFB1E
	public virtual string GetCompassUpIcon()
	{
		return this.compassUpIcon;
	}

	// Token: 0x06001C55 RID: 7253 RVA: 0x000B1926 File Offset: 0x000AFB26
	public virtual string GetCompassDownIcon()
	{
		return this.compassDownIcon;
	}

	// Token: 0x06001C56 RID: 7254 RVA: 0x000B192E File Offset: 0x000AFB2E
	public virtual string GetTrackerIcon()
	{
		return this.trackerIcon;
	}

	// Token: 0x06001C57 RID: 7255 RVA: 0x000B1936 File Offset: 0x000AFB36
	public virtual bool HasUIIcon()
	{
		return this.mapIcon != null || this.trackerIcon != null || this.compassIcon != null;
	}

	// Token: 0x06001C58 RID: 7256 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual EnumMapObjectType GetMapObjectType()
	{
		return EnumMapObjectType.Entity;
	}

	// Token: 0x06001C59 RID: 7257 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool IsMapIconBlinking()
	{
		return false;
	}

	// Token: 0x06001C5A RID: 7258 RVA: 0x000B1953 File Offset: 0x000AFB53
	public virtual bool IsDrawMapIcon()
	{
		return this.IsSpawned();
	}

	// Token: 0x06001C5B RID: 7259 RVA: 0x000B195B File Offset: 0x000AFB5B
	public virtual Color GetMapIconColor()
	{
		return Color.white;
	}

	// Token: 0x06001C5C RID: 7260 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool CanMapIconBeSelected()
	{
		return false;
	}

	// Token: 0x06001C5D RID: 7261 RVA: 0x000282C0 File Offset: 0x000264C0
	public virtual int GetLayerForMapIcon()
	{
		return 2;
	}

	// Token: 0x06001C5E RID: 7262 RVA: 0x000B1962 File Offset: 0x000AFB62
	public virtual bool IsClientControlled()
	{
		return this.attachedEntities != null && this.attachedEntities.Length != 0 && this.attachedEntities[0] != null;
	}

	// Token: 0x06001C5F RID: 7263 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool IsDeadIfOutOfWorld()
	{
		return true;
	}

	// Token: 0x06001C60 RID: 7264 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool CanCollideWithBlocks()
	{
		return true;
	}

	// Token: 0x06001C61 RID: 7265 RVA: 0x000B1985 File Offset: 0x000AFB85
	public void SetSpawnerSource(EnumSpawnerSource _spawnerSource)
	{
		this.SetSpawnerSource(_spawnerSource, 0L, 0);
	}

	// Token: 0x06001C62 RID: 7266 RVA: 0x000B1991 File Offset: 0x000AFB91
	public void SetSpawnerSource(EnumSpawnerSource _spawnerSource, long _chunkKey, int _biomeIdHash)
	{
		this.spawnerSource = _spawnerSource;
		this.spawnerSourceChunkKey = _chunkKey;
		this.spawnerSourceBiomeIdHash = _biomeIdHash;
	}

	// Token: 0x06001C63 RID: 7267 RVA: 0x000B19A8 File Offset: 0x000AFBA8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public EnumSpawnerSource GetSpawnerSource()
	{
		return this.spawnerSource;
	}

	// Token: 0x06001C64 RID: 7268 RVA: 0x000B19B0 File Offset: 0x000AFBB0
	public long GetSpawnerSourceChunkKey()
	{
		return this.spawnerSourceChunkKey;
	}

	// Token: 0x06001C65 RID: 7269 RVA: 0x000B19B8 File Offset: 0x000AFBB8
	public int GetSpawnerSourceBiomeIdHash()
	{
		return this.spawnerSourceBiomeIdHash;
	}

	// Token: 0x06001C66 RID: 7270 RVA: 0x0002E7B0 File Offset: 0x0002C9B0
	public float CalculateAudioOcclusion()
	{
		return 0f;
	}

	// Token: 0x06001C67 RID: 7271 RVA: 0x000B19C0 File Offset: 0x000AFBC0
	public virtual void PlayOneShot(string clipName, bool sound_in_head = false, bool serverSignalOnly = false, bool isUnique = false, AnimationEvent _animEvent = null)
	{
		if (!sound_in_head)
		{
			if (!serverSignalOnly)
			{
				Manager.BroadcastPlay(this, clipName, serverSignalOnly);
				return;
			}
			Handle handle = Manager.Play(this, clipName, 1f, true);
			if (_animEvent != null)
			{
				int intParameter = _animEvent.intParameter;
				if (intParameter > 0)
				{
					this.addAnimatorAudioToMonitor((Entity.StopAnimatorAudioType)intParameter, handle);
					return;
				}
			}
		}
		else
		{
			Manager.PlayInsidePlayerHead(clipName, -1, 0f, false, isUnique);
		}
	}

	// Token: 0x06001C68 RID: 7272 RVA: 0x000B1A14 File Offset: 0x000AFC14
	[PublicizedFrom(EAccessModifier.Private)]
	public void addAnimatorAudioToMonitor(Entity.StopAnimatorAudioType _sat, Handle _handle)
	{
		Handle handle;
		if (this.animatorAudioMonitoringDictionary.TryGetValue(_sat, out handle))
		{
			handle.Stop(this.entityId);
		}
		this.animatorAudioMonitoringDictionary[_sat] = _handle;
	}

	// Token: 0x06001C69 RID: 7273 RVA: 0x000B1A4C File Offset: 0x000AFC4C
	public void StopAnimatorAudio(Entity.StopAnimatorAudioType _sat)
	{
		Handle handle;
		if (this.animatorAudioMonitoringDictionary.TryGetValue(_sat, out handle))
		{
			handle.Stop(this.entityId);
			this.animatorAudioMonitoringDictionary.Remove(_sat);
		}
	}

	// Token: 0x06001C6A RID: 7274 RVA: 0x000B1A82 File Offset: 0x000AFC82
	public void StopOneShot(string clipName)
	{
		Manager.BroadcastStop(this.entityId, clipName);
	}

	// Token: 0x06001C6B RID: 7275 RVA: 0x000B1A90 File Offset: 0x000AFC90
	public virtual EntityActivationCommand[] GetActivationCommands(Vector3i _tePos, EntityAlive _entityFocusing)
	{
		if (this.lootContainer == null)
		{
			this.cmds[0].enabled = false;
		}
		return this.cmds;
	}

	// Token: 0x06001C6C RID: 7276 RVA: 0x000B1AB4 File Offset: 0x000AFCB4
	public virtual bool OnEntityActivated(int _indexInBlockActivationCommands, Vector3i _tePos, EntityAlive _entityFocusing)
	{
		if (_indexInBlockActivationCommands == 0)
		{
			EntityClass entityClass = EntityClass.list[this.entityClass];
			if (entityClass.onActivateEvent != "")
			{
				GameEventManager.Current.HandleAction(entityClass.onActivateEvent, null, this, false, _tePos, "", "", false, true, "", null);
			}
			GameManager.Instance.TELockServer(0, _tePos, this.entityId, _entityFocusing.entityId, null);
			return true;
		}
		return false;
	}

	// Token: 0x06001C6D RID: 7277 RVA: 0x000B1B30 File Offset: 0x000AFD30
	public void SetAttachMaxCount(int maxCount)
	{
		if (this.attachedEntities != null)
		{
			if (this.attachedEntities.Length == maxCount)
			{
				return;
			}
			for (int i = maxCount; i < this.attachedEntities.Length; i++)
			{
				Entity entity = this.attachedEntities[i];
				if (entity)
				{
					entity.Detach();
				}
			}
		}
		Entity[] array = this.attachedEntities;
		this.attachedEntities = null;
		if (maxCount > 0)
		{
			this.attachedEntities = new Entity[maxCount];
			if (array != null)
			{
				int num = Utils.FastMin(array.Length, maxCount);
				for (int j = 0; j < num; j++)
				{
					this.attachedEntities[j] = array[j];
				}
			}
		}
	}

	// Token: 0x06001C6E RID: 7278 RVA: 0x000B1BC3 File Offset: 0x000AFDC3
	public int GetAttachMaxCount()
	{
		if (this.attachedEntities != null)
		{
			return (int)((byte)this.attachedEntities.Length);
		}
		return 0;
	}

	// Token: 0x06001C6F RID: 7279 RVA: 0x000B1BD8 File Offset: 0x000AFDD8
	public int GetAttachFreeCount()
	{
		int num = 0;
		if (this.attachedEntities != null)
		{
			for (int i = 0; i < this.attachedEntities.Length; i++)
			{
				if (this.attachedEntities[i] == null)
				{
					num++;
				}
			}
		}
		return num;
	}

	// Token: 0x06001C70 RID: 7280 RVA: 0x000B1C17 File Offset: 0x000AFE17
	public Entity GetAttached(int slot)
	{
		if (this.attachedEntities != null && slot < this.attachedEntities.Length)
		{
			return this.attachedEntities[slot];
		}
		return null;
	}

	// Token: 0x17000330 RID: 816
	// (get) Token: 0x06001C71 RID: 7281 RVA: 0x000B1C36 File Offset: 0x000AFE36
	public Entity AttachedMainEntity
	{
		get
		{
			if (this.attachedEntities == null)
			{
				return null;
			}
			return this.attachedEntities[0];
		}
	}

	// Token: 0x06001C72 RID: 7282 RVA: 0x000B1C4C File Offset: 0x000AFE4C
	public Entity GetFirstAttached()
	{
		if (this.attachedEntities != null)
		{
			for (int i = 0; i < this.attachedEntities.Length; i++)
			{
				Entity entity = this.attachedEntities[i];
				if (entity)
				{
					return entity;
				}
			}
		}
		return null;
	}

	// Token: 0x06001C73 RID: 7283 RVA: 0x000B1C88 File Offset: 0x000AFE88
	public EntityPlayerLocal GetAttachedPlayerLocal()
	{
		if (this.attachedEntities != null)
		{
			for (int i = 0; i < this.attachedEntities.Length; i++)
			{
				EntityPlayerLocal entityPlayerLocal = this.attachedEntities[i] as EntityPlayerLocal;
				if (entityPlayerLocal)
				{
					return entityPlayerLocal;
				}
			}
		}
		return null;
	}

	// Token: 0x06001C74 RID: 7284 RVA: 0x000B1CC9 File Offset: 0x000AFEC9
	public bool CanAttach(Entity _entity)
	{
		return this.FindAttachSlot(_entity) < 0 && this.FindAttachSlot(null) >= 0;
	}

	// Token: 0x06001C75 RID: 7285 RVA: 0x000B1CE4 File Offset: 0x000AFEE4
	public int FindAttachSlot(Entity _entity)
	{
		if (this.attachedEntities != null)
		{
			for (int i = 0; i < this.attachedEntities.Length; i++)
			{
				if (this.attachedEntities[i] == _entity)
				{
					return i;
				}
			}
		}
		return -1;
	}

	// Token: 0x06001C76 RID: 7286 RVA: 0x000B1D1F File Offset: 0x000AFF1F
	public bool IsAttached(Entity _entity)
	{
		return this.FindAttachSlot(_entity) >= 0;
	}

	// Token: 0x06001C77 RID: 7287 RVA: 0x000B1D2E File Offset: 0x000AFF2E
	public bool IsDriven()
	{
		return this.attachedEntities != null && this.attachedEntities[0];
	}

	// Token: 0x06001C78 RID: 7288 RVA: 0x000B1D48 File Offset: 0x000AFF48
	public virtual int AttachEntityToSelf(Entity _other, int slot)
	{
		int num = this.FindAttachSlot(_other);
		if (num >= 0)
		{
			if (slot < 0 || slot == num)
			{
				return num;
			}
			this.DetachEntity(_other);
		}
		if (slot < 0)
		{
			slot = this.FindAttachSlot(null);
			if (slot < 0)
			{
				return -1;
			}
		}
		if (slot >= this.attachedEntities.Length)
		{
			return -1;
		}
		if (slot == 0)
		{
			this.serverPos = NetEntityDistributionEntry.EncodePos(this.position);
			this.isEntityRemote = _other.isEntityRemote;
		}
		this.attachedEntities[slot] = _other;
		return slot;
	}

	// Token: 0x06001C79 RID: 7289 RVA: 0x000B1DBC File Offset: 0x000AFFBC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void DetachEntity(Entity _other)
	{
		int num = this.FindAttachSlot(_other);
		if (num < 0)
		{
			return;
		}
		if (num == 0)
		{
			this.isEntityRemote = this.world.IsRemote();
		}
		this.attachedEntities[num] = null;
	}

	// Token: 0x06001C7A RID: 7290 RVA: 0x000B1DF4 File Offset: 0x000AFFF4
	public virtual void StartAttachToEntity(Entity _other, int slot = -1)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEntityAttach>().Setup(NetPackageEntityAttach.AttachType.AttachServer, this.entityId, _other.entityId, slot), false);
			return;
		}
		slot = this.AttachToEntity(_other, slot);
		if (slot >= 0)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityAttach>().Setup(NetPackageEntityAttach.AttachType.AttachClient, this.entityId, _other.entityId, slot), false, -1, -1, -1, null, 192, false);
		}
	}

	// Token: 0x06001C7B RID: 7291 RVA: 0x000B1E74 File Offset: 0x000B0074
	public virtual int AttachToEntity(Entity _other, int slot = -1)
	{
		if (_other.IsAttached(this))
		{
			return -1;
		}
		slot = _other.AttachEntityToSelf(this, slot);
		if (slot < 0)
		{
			return slot;
		}
		AttachedToEntitySlotInfo attachedToInfo = _other.GetAttachedToInfo(slot);
		this.RootTransform.SetParent(attachedToInfo.enterParentTransform, false);
		this.RootTransform.localPosition = Vector3.zero;
		this.RootTransform.localEulerAngles = Vector3.zero;
		this.ModelTransform.localPosition = attachedToInfo.enterPosition;
		this.ModelTransform.localEulerAngles = attachedToInfo.enterRotation;
		this.rotation = attachedToInfo.enterRotation;
		if (this.isEntityRemote && !attachedToInfo.bKeep3rdPersonModelVisible)
		{
			this.emodel.SetVisible(false, false);
		}
		this.AttachedToEntity = _other;
		return slot;
	}

	// Token: 0x06001C7C RID: 7292 RVA: 0x000B1F2C File Offset: 0x000B012C
	public void SendDetach()
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEntityAttach>().Setup(NetPackageEntityAttach.AttachType.DetachServer, this.entityId, -1, -1), false);
		}
		else
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityAttach>().Setup(NetPackageEntityAttach.AttachType.DetachClient, this.entityId, -1, -1), false, -1, -1, -1, null, 192, false);
		}
		this.Detach();
	}

	// Token: 0x06001C7D RID: 7293 RVA: 0x000B1F9C File Offset: 0x000B019C
	public virtual void Detach()
	{
		this.RootTransform.parent = EntityFactory.ParentNameToTransform[EntityClass.list[this.entityClass].parentGameObjectName];
		if (this.AttachedToEntity == null)
		{
			return;
		}
		int num = this.AttachedToEntity.FindAttachSlot(this);
		if (num < 0)
		{
			num = 0;
		}
		AttachedToEntitySlotInfo attachedToInfo = this.AttachedToEntity.GetAttachedToInfo(num);
		AttachedToEntitySlotExit attachedToEntitySlotExit = this.FindValidExitPosition(attachedToInfo.exits);
		Entity attachedToEntity = this.AttachedToEntity;
		this.AttachedToEntity = null;
		this.isUpdatePosition = false;
		if (attachedToEntitySlotExit.position != Vector3.zero)
		{
			this.SetPosition(attachedToEntitySlotExit.position, true);
			this.SetRotation(attachedToEntitySlotExit.rotation);
		}
		this.ResetLastTickPos(base.transform.position + Origin.position);
		attachedToEntity.DetachEntity(this);
		if (this.isEntityRemote && !attachedToInfo.bKeep3rdPersonModelVisible)
		{
			this.emodel.SetVisible(true, false);
		}
	}

	// Token: 0x06001C7E RID: 7294 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void MoveByAttachedEntity(EntityPlayerLocal _player)
	{
	}

	// Token: 0x06001C7F RID: 7295 RVA: 0x000B208C File Offset: 0x000B028C
	public virtual AttachedToEntitySlotExit FindValidExitPosition(List<AttachedToEntitySlotExit> candidatePositions)
	{
		AttachedToEntitySlotExit attachedToEntitySlotExit;
		attachedToEntitySlotExit.position = Vector3.zero;
		attachedToEntitySlotExit.rotation = Vector3.zero;
		if (this.m_characterController == null)
		{
			return attachedToEntitySlotExit;
		}
		this.AttachedToEntity.SetPhysicsCollidersLayer(14);
		float radius = this.m_characterController.GetRadius();
		float num = this.m_characterController.GetHeight() - radius * 2f;
		Vector3 vector = base.transform.position + this.m_characterController.GetCenter();
		vector.y -= num * 0.5f;
		for (int i = 0; i < candidatePositions.Count; i++)
		{
			for (float num2 = 0f; num2 < 0.75f; num2 += 0.24f)
			{
				Vector3 vector2 = vector;
				vector2.y += num2;
				attachedToEntitySlotExit = candidatePositions[i];
				attachedToEntitySlotExit.position.y = attachedToEntitySlotExit.position.y + num2;
				Vector3 vector3 = attachedToEntitySlotExit.position - Origin.position - vector2;
				vector3.y += radius;
				Vector3 normalized = vector3.normalized;
				float num3 = vector3.magnitude;
				if (normalized.y < 0f)
				{
					float num4 = normalized.y;
					if (num4 < -0.707f)
					{
						break;
					}
					num4 *= -1.6f;
					num3 += num4;
					attachedToEntitySlotExit.position += normalized * num4;
				}
				bool flag = false;
				Vector3 origin = vector2;
				for (float num5 = -radius * 0.5f; num5 < num; num5 += 0.2f)
				{
					origin.y = vector2.y + num5;
					flag = Physics.Raycast(origin, normalized, num3, 1084817408);
					if (flag)
					{
						break;
					}
				}
				Vector3 vector4 = vector2 - normalized * 0.1f;
				Vector3 point = vector4;
				point.y += num;
				if (!flag && !Physics.CapsuleCast(vector4, point, radius, normalized, num3, 1084817408))
				{
					this.AttachedToEntity.SetPhysicsCollidersLayer(21);
					return attachedToEntitySlotExit;
				}
			}
		}
		this.AttachedToEntity.SetPhysicsCollidersLayer(21);
		attachedToEntitySlotExit.position = Vector3.zero;
		return attachedToEntitySlotExit;
	}

	// Token: 0x06001C80 RID: 7296 RVA: 0x000B22BC File Offset: 0x000B04BC
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetPhysicsCollidersLayer(int layer)
	{
		Collider[] componentsInChildren = this.PhysicsTransform.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.layer = layer;
		}
	}

	// Token: 0x06001C81 RID: 7297 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public void DebugCapsuleCast()
	{
	}

	// Token: 0x06001C82 RID: 7298 RVA: 0x00019766 File Offset: 0x00017966
	public virtual AttachedToEntitySlotInfo GetAttachedToInfo(int _slotIdx)
	{
		return null;
	}

	// Token: 0x06001C83 RID: 7299 RVA: 0x000B22F4 File Offset: 0x000B04F4
	public virtual bool CanUpdateEntity()
	{
		Vector3i vector3i = World.worldToBlockPos(this.position);
		IChunk chunkFromWorldPos = this.world.GetChunkFromWorldPos(vector3i.x, vector3i.y, vector3i.z);
		if (chunkFromWorldPos == null || !chunkFromWorldPos.GetAvailable())
		{
			return false;
		}
		for (int i = 0; i < this.adjacentPositions.Length; i++)
		{
			int num = World.toChunkXZ(vector3i.x + this.adjacentPositions[i].x);
			int num2 = World.toChunkXZ(vector3i.z + this.adjacentPositions[i].z);
			if (num != chunkFromWorldPos.X || num2 != chunkFromWorldPos.Z)
			{
				IChunk chunkSync = this.world.GetChunkSync(num, num2);
				if (chunkSync == null || !chunkSync.GetAvailable())
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x06001C84 RID: 7300 RVA: 0x000B23BB File Offset: 0x000B05BB
	public virtual Transform GetThirdPersonCameraTransform()
	{
		return this.emodel.GetThirdPersonCameraTransform();
	}

	// Token: 0x06001C85 RID: 7301 RVA: 0x000B23C8 File Offset: 0x000B05C8
	public virtual void Write(BinaryWriter _bw, bool _bNetworkWrite)
	{
		_bw.Write((byte)this.spawnerSource);
		if (this.spawnerSource == EnumSpawnerSource.Biome)
		{
			_bw.Write(this.spawnerSourceBiomeIdHash);
			_bw.Write(this.spawnerSourceChunkKey);
		}
		_bw.Write(this.WorldTimeBorn);
	}

	// Token: 0x06001C86 RID: 7302 RVA: 0x000B2404 File Offset: 0x000B0604
	public virtual void Read(byte _version, BinaryReader _br)
	{
		if (_version >= 11)
		{
			this.spawnerSource = (EnumSpawnerSource)_br.ReadByte();
			if (this.spawnerSource == EnumSpawnerSource.Biome)
			{
				if (_version >= 28)
				{
					this.spawnerSourceBiomeIdHash = _br.ReadInt32();
				}
				else
				{
					_br.ReadString();
					this.spawnerSource = EnumSpawnerSource.Delete;
				}
				this.spawnerSourceChunkKey = _br.ReadInt64();
			}
		}
		if (_version >= 15)
		{
			this.WorldTimeBorn = _br.ReadUInt64();
		}
	}

	// Token: 0x06001C87 RID: 7303 RVA: 0x0000FB42 File Offset: 0x0000DD42
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool isEntityStatic()
	{
		return false;
	}

	// Token: 0x06001C88 RID: 7304 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void AddUIHarvestingItem(ItemStack _is, bool _bAddOnlyIfNotExisting = false)
	{
	}

	// Token: 0x06001C89 RID: 7305 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool IsQRotationUsed()
	{
		return false;
	}

	// Token: 0x06001C8A RID: 7306 RVA: 0x000B2469 File Offset: 0x000B0669
	public bool HasAnyTags(FastTags<TagGroup.Global> tags)
	{
		return this.cachedTags.Test_AnySet(tags);
	}

	// Token: 0x06001C8B RID: 7307 RVA: 0x000B2477 File Offset: 0x000B0677
	public bool HasAllTags(FastTags<TagGroup.Global> tags)
	{
		return this.cachedTags.Test_AllSet(tags);
	}

	// Token: 0x06001C8C RID: 7308 RVA: 0x000B2488 File Offset: 0x000B0688
	public void SetTransformActive(string partName, bool active)
	{
		Transform transform = base.transform.FindInChilds(partName, false);
		if (transform != null)
		{
			transform.gameObject.SetActive(active);
		}
	}

	// Token: 0x06001C8D RID: 7309 RVA: 0x000B24B8 File Offset: 0x000B06B8
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void HandleNavObject()
	{
		if (EntityClass.list[this.entityClass].NavObject != "")
		{
			this.NavObject = NavObjectManager.Instance.RegisterNavObject(EntityClass.list[this.entityClass].NavObject, this, "", false);
		}
	}

	// Token: 0x06001C8E RID: 7310 RVA: 0x000B2514 File Offset: 0x000B0714
	public void AddNavObject(string navObjectName, string overrideSprite, string overrideText)
	{
		if (this.NavObject == null)
		{
			NavObjectManager.Instance.RegisterNavObject(navObjectName, this, overrideSprite, false).name = overrideText;
			return;
		}
		NavObjectClass navObjectClass = NavObjectClass.GetNavObjectClass(navObjectName);
		this.NavObject.name = overrideText;
		this.NavObject.AddNavObjectClass(navObjectClass);
	}

	// Token: 0x06001C8F RID: 7311 RVA: 0x000B2560 File Offset: 0x000B0760
	public void RemoveNavObject(string navObjectName)
	{
		NavObjectClass navObjectClass = NavObjectClass.GetNavObjectClass(navObjectName);
		if (this.NavObject != null && this.NavObject.RemoveNavObjectClass(navObjectClass))
		{
			this.NavObject = null;
		}
	}

	// Token: 0x06001C90 RID: 7312 RVA: 0x000B2594 File Offset: 0x000B0794
	public string GetDebugName()
	{
		EntityAlive entityAlive = this as EntityAlive;
		if (entityAlive != null)
		{
			return entityAlive.EntityName;
		}
		return base.GetType().ToString();
	}

	// Token: 0x04001297 RID: 4759
	public const int EntityIdInvalid = -1;

	// Token: 0x04001298 RID: 4760
	public const int cIdCreatorIsServer = -2;

	// Token: 0x04001299 RID: 4761
	public const int cClientIdStart = -2;

	// Token: 0x0400129A RID: 4762
	public const int cClientIdCreate = -1;

	// Token: 0x0400129B RID: 4763
	public const int cClientIdNone = 0;

	// Token: 0x0400129C RID: 4764
	public const int cKillAnythingDamage = 99999;

	// Token: 0x0400129D RID: 4765
	public const int cIgnoreDamage = -1;

	// Token: 0x0400129E RID: 4766
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public FastTags<TagGroup.Global> cachedTags;

	// Token: 0x0400129F RID: 4767
	public bool RootMotion;

	// Token: 0x040012A0 RID: 4768
	public bool HasDeathAnim;

	// Token: 0x040012A1 RID: 4769
	public World world;

	// Token: 0x040012A2 RID: 4770
	public Transform PhysicsTransform;

	// Token: 0x040012A3 RID: 4771
	public Transform RootTransform;

	// Token: 0x040012A4 RID: 4772
	public Transform ModelTransform;

	// Token: 0x040012A5 RID: 4773
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 scaledExtent;

	// Token: 0x040012A6 RID: 4774
	public Bounds boundingBox;

	// Token: 0x040012A7 RID: 4775
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Collider nativeCollider;

	// Token: 0x040012A8 RID: 4776
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int interpolateTargetRot;

	// Token: 0x040012A9 RID: 4777
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int interpolateTargetQRot;

	// Token: 0x040012AA RID: 4778
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isUpdatePosition;

	// Token: 0x040012AB RID: 4779
	public int entityId;

	// Token: 0x040012AC RID: 4780
	public int clientEntityId;

	// Token: 0x040012AD RID: 4781
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float yOffset;

	// Token: 0x040012AE RID: 4782
	public bool onGround;

	// Token: 0x040012AF RID: 4783
	public bool isCollided;

	// Token: 0x040012B0 RID: 4784
	public bool isCollidedHorizontally;

	// Token: 0x040012B1 RID: 4785
	public bool isCollidedVertically;

	// Token: 0x040012B2 RID: 4786
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool isMotionSlowedDown;

	// Token: 0x040012B3 RID: 4787
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float motionMultiplier;

	// Token: 0x040012B4 RID: 4788
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool firstUpdate = true;

	// Token: 0x040012B5 RID: 4789
	public Vector3 prevRotation;

	// Token: 0x040012B6 RID: 4790
	public Vector3 rotation;

	// Token: 0x040012B7 RID: 4791
	public Quaternion qrotation = Quaternion.identity;

	// Token: 0x040012B8 RID: 4792
	public Vector3 position;

	// Token: 0x040012B9 RID: 4793
	public Vector3 prevPos;

	// Token: 0x040012BA RID: 4794
	public Vector3 targetPos;

	// Token: 0x040012BB RID: 4795
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 targetRot;

	// Token: 0x040012BC RID: 4796
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Quaternion targetQRot = Quaternion.identity;

	// Token: 0x040012BD RID: 4797
	public Vector3i chunkPosAddedEntityTo;

	// Token: 0x040012BE RID: 4798
	public Vector3i serverPos;

	// Token: 0x040012BF RID: 4799
	public Vector3i serverRot;

	// Token: 0x040012C0 RID: 4800
	public Vector3[] lastTickPos = new Vector3[5];

	// Token: 0x040012C1 RID: 4801
	public Vector3 motion;

	// Token: 0x040012C2 RID: 4802
	public bool IsMovementReplicated = true;

	// Token: 0x040012C3 RID: 4803
	public bool IsStuck;

	// Token: 0x040012C4 RID: 4804
	public bool addedToChunk;

	// Token: 0x040012C5 RID: 4805
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isInWater;

	// Token: 0x040012C6 RID: 4806
	public bool isSwimming;

	// Token: 0x040012C7 RID: 4807
	public float inWaterLevel;

	// Token: 0x040012C8 RID: 4808
	public float inWaterPercent;

	// Token: 0x040012C9 RID: 4809
	public bool isHeadUnderwater;

	// Token: 0x040012CA RID: 4810
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool bInElevator;

	// Token: 0x040012CB RID: 4811
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool bAirBorne;

	// Token: 0x040012CC RID: 4812
	public float stepHeight;

	// Token: 0x040012CD RID: 4813
	public float ySize;

	// Token: 0x040012CE RID: 4814
	public float distanceWalked;

	// Token: 0x040012CF RID: 4815
	public float distanceSwam;

	// Token: 0x040012D0 RID: 4816
	public float distanceClimbed;

	// Token: 0x040012D1 RID: 4817
	public float fallDistance;

	// Token: 0x040012D2 RID: 4818
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float fallLastY;

	// Token: 0x040012D3 RID: 4819
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float fallVelY;

	// Token: 0x040012D4 RID: 4820
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 fallLastMotion;

	// Token: 0x040012D5 RID: 4821
	public float entityCollisionReduction = 0.9f;

	// Token: 0x040012D6 RID: 4822
	public bool isEntityRemote;

	// Token: 0x040012D7 RID: 4823
	public GameRandom rand;

	// Token: 0x040012D8 RID: 4824
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int ticksExisted;

	// Token: 0x040012D9 RID: 4825
	public static float updatePositionLerpTimeScale = 8f;

	// Token: 0x040012DA RID: 4826
	public static float updateRotationLerpTimeScale = 8f;

	// Token: 0x040012DB RID: 4827
	public static float tickPositionMoveTowardsMaxDistance = 3f;

	// Token: 0x040012DC RID: 4828
	public static float tickPositionLerpMultiplier = 0.5f;

	// Token: 0x040012DD RID: 4829
	public int entityClass;

	// Token: 0x040012DE RID: 4830
	public float lifetime;

	// Token: 0x040012DF RID: 4831
	public int count;

	// Token: 0x040012E0 RID: 4832
	public int belongsPlayerId;

	// Token: 0x040012E1 RID: 4833
	public bool bWillRespawn;

	// Token: 0x040012E2 RID: 4834
	public ulong WorldTimeBorn;

	// Token: 0x040012E3 RID: 4835
	public DataItem<bool> IsFlyMode = new DataItem<bool>();

	// Token: 0x040012E4 RID: 4836
	public DataItem<bool> IsGodMode = new DataItem<bool>();

	// Token: 0x040012E5 RID: 4837
	public DataItem<bool> IsNoCollisionMode = new DataItem<bool>();

	// Token: 0x040012E6 RID: 4838
	public EntityFlags entityFlags;

	// Token: 0x040012E7 RID: 4839
	public EntityType entityType;

	// Token: 0x040012E8 RID: 4840
	public float lootDropProb;

	// Token: 0x040012E9 RID: 4841
	public string lootListOnDeath;

	// Token: 0x040012EA RID: 4842
	public string lootListAlive;

	// Token: 0x040012EB RID: 4843
	public TileEntityLootContainer lootContainer;

	// Token: 0x040012EC RID: 4844
	public float speedForward;

	// Token: 0x040012ED RID: 4845
	public float speedStrafe;

	// Token: 0x040012EE RID: 4846
	public float speedVertical;

	// Token: 0x040012EF RID: 4847
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int speedSentTicks;

	// Token: 0x040012F0 RID: 4848
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float speedForwardSent = float.MaxValue;

	// Token: 0x040012F1 RID: 4849
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float speedStrafeSent = float.MaxValue;

	// Token: 0x040012F2 RID: 4850
	public int MovementState;

	// Token: 0x040012F3 RID: 4851
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float yawSeekTime;

	// Token: 0x040012F4 RID: 4852
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float yawSeekTimeMax;

	// Token: 0x040012F5 RID: 4853
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float yawSeekAngle;

	// Token: 0x040012F6 RID: 4854
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float yawSeekAngleEnd;

	// Token: 0x040012F7 RID: 4855
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public IAIDirectorMarker m_marker;

	// Token: 0x040012F8 RID: 4856
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public string mapIcon;

	// Token: 0x040012F9 RID: 4857
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public string compassIcon;

	// Token: 0x040012FA RID: 4858
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public string compassUpIcon;

	// Token: 0x040012FB RID: 4859
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public string compassDownIcon;

	// Token: 0x040012FC RID: 4860
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public string trackerIcon;

	// Token: 0x040012FD RID: 4861
	public bool bDead;

	// Token: 0x040012FE RID: 4862
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bWasDead;

	// Token: 0x040012FF RID: 4863
	[Preserve]
	public EModelBase emodel;

	// Token: 0x04001300 RID: 4864
	public CharacterControllerAbstract m_characterController;

	// Token: 0x04001301 RID: 4865
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isCCDelayed;

	// Token: 0x04001302 RID: 4866
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool canCCMove;

	// Token: 0x04001303 RID: 4867
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public CollisionFlags collisionFlags;

	// Token: 0x04001304 RID: 4868
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Entity.MoveHitSurface groundSurface;

	// Token: 0x04001305 RID: 4869
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 hitMove;

	// Token: 0x04001306 RID: 4870
	public float projectedMove;

	// Token: 0x04001307 RID: 4871
	public bool IsRotateToGroundFlat;

	// Token: 0x04001308 RID: 4872
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isRotateToGround;

	// Token: 0x04001309 RID: 4873
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float rotateToGroundPitch;

	// Token: 0x0400130A RID: 4874
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float rotateToGroundPitchVel;

	// Token: 0x0400130B RID: 4875
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EnumSpawnerSource spawnerSource;

	// Token: 0x0400130C RID: 4876
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int spawnerSourceBiomeIdHash;

	// Token: 0x0400130D RID: 4877
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public long spawnerSourceChunkKey;

	// Token: 0x0400130E RID: 4878
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityActivationCommand[] cmds = new EntityActivationCommand[]
	{
		new EntityActivationCommand("Search", "search", true, null)
	};

	// Token: 0x0400130F RID: 4879
	public static int InstanceCount;

	// Token: 0x04001310 RID: 4880
	public bool IsDespawned;

	// Token: 0x04001311 RID: 4881
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool markedForUnload;

	// Token: 0x04001312 RID: 4882
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public MovableSharedChunkObserver movableChunkObserver;

	// Token: 0x04001313 RID: 4883
	public bool bIsChunkObserver;

	// Token: 0x04001314 RID: 4884
	public NavObject NavObject;

	// Token: 0x04001315 RID: 4885
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool isIgnoredByAI;

	// Token: 0x04001316 RID: 4886
	public Entity AttachedToEntity;

	// Token: 0x04001317 RID: 4887
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Entity[] attachedEntities;

	// Token: 0x04001318 RID: 4888
	public const int cPhysicsMasterTickRate = 2;

	// Token: 0x04001319 RID: 4889
	public bool usePhysicsMaster;

	// Token: 0x0400131A RID: 4890
	public bool isPhysicsMaster;

	// Token: 0x0400131B RID: 4891
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 physicsMasterFromPos;

	// Token: 0x0400131C RID: 4892
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion physicsMasterFromRot;

	// Token: 0x0400131D RID: 4893
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float physicsMasterTargetElapsed;

	// Token: 0x0400131E RID: 4894
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float physicsMasterTargetTime;

	// Token: 0x0400131F RID: 4895
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 physicsMasterTargetPos;

	// Token: 0x04001320 RID: 4896
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion physicsMasterTargetRot;

	// Token: 0x04001321 RID: 4897
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 physicsMasterSendPos;

	// Token: 0x04001322 RID: 4898
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion physicsMasterSendRot;

	// Token: 0x04001323 RID: 4899
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float physicsHeightScale = 1f;

	// Token: 0x04001324 RID: 4900
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform physicsRBT;

	// Token: 0x04001325 RID: 4901
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public CapsuleCollider physicsCapsuleCollider;

	// Token: 0x04001326 RID: 4902
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float physicsColliderRadius;

	// Token: 0x04001327 RID: 4903
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float physicsColliderLowerY;

	// Token: 0x04001328 RID: 4904
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float physicsBaseHeight;

	// Token: 0x04001329 RID: 4905
	public float physicsHeight;

	// Token: 0x0400132A RID: 4906
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Rigidbody physicsRB;

	// Token: 0x0400132B RID: 4907
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 physicsPos;

	// Token: 0x0400132C RID: 4908
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 physicsBasePos;

	// Token: 0x0400132D RID: 4909
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 physicsTargetPos;

	// Token: 0x0400132E RID: 4910
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float physicsPosMoveDistance;

	// Token: 0x0400132F RID: 4911
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion physicsRot;

	// Token: 0x04001330 RID: 4912
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool wasFixedUpdate;

	// Token: 0x04001331 RID: 4913
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 physicsVel;

	// Token: 0x04001332 RID: 4914
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 physicsAngVel;

	// Token: 0x04001333 RID: 4915
	public bool spawnByAllowShare;

	// Token: 0x04001334 RID: 4916
	public int spawnById = -1;

	// Token: 0x04001335 RID: 4917
	public string spawnByName;

	// Token: 0x04001336 RID: 4918
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityActivationCommand[] customCmds;

	// Token: 0x04001337 RID: 4919
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cWaterHeightScale = 1.1f;

	// Token: 0x04001338 RID: 4920
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static float[] waterLevelDirOffsets = new float[]
	{
		Mathf.Cos(0f),
		Mathf.Sin(0f),
		Mathf.Cos(2.0943952f),
		Mathf.Sin(2.0943952f),
		Mathf.Cos(4.1887903f),
		Mathf.Sin(4.1887903f)
	};

	// Token: 0x04001339 RID: 4921
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public List<Bounds> collAABB = new List<Bounds>();

	// Token: 0x0400133A RID: 4922
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float kAddFixedUpdateTimeScale = 1f;

	// Token: 0x0400133B RID: 4923
	public EnumRemoveEntityReason unloadReason;

	// Token: 0x0400133C RID: 4924
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isUnloaded;

	// Token: 0x0400133D RID: 4925
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public const int cAttachSlotNone = -1;

	// Token: 0x0400133E RID: 4926
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3i[] adjacentPositions = new Vector3i[]
	{
		Vector3i.forward,
		Vector3i.back,
		Vector3i.left,
		Vector3i.right
	};

	// Token: 0x0400133F RID: 4927
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Dictionary<Entity.StopAnimatorAudioType, Handle> animatorAudioMonitoringDictionary = new Dictionary<Entity.StopAnimatorAudioType, Handle>();

	// Token: 0x020003A6 RID: 934
	public struct MoveHitSurface
	{
		// Token: 0x04001340 RID: 4928
		public Vector3 hitPoint;

		// Token: 0x04001341 RID: 4929
		public Vector3 lastHitPoint;

		// Token: 0x04001342 RID: 4930
		public Vector3 normal;

		// Token: 0x04001343 RID: 4931
		public Vector3 lastNormal;
	}

	// Token: 0x020003A7 RID: 935
	public enum EnumPositionUpdateMovementType
	{
		// Token: 0x04001345 RID: 4933
		Lerp,
		// Token: 0x04001346 RID: 4934
		MoveTowards,
		// Token: 0x04001347 RID: 4935
		Instant
	}

	// Token: 0x020003A8 RID: 936
	public enum StopAnimatorAudioType
	{
		// Token: 0x04001349 RID: 4937
		StopOnReloadCancel = 1,
		// Token: 0x0400134A RID: 4938
		StopOnStopHolding
	}
}
