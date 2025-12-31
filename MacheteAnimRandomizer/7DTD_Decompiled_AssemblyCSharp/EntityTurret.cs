using System;
using System.IO;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000467 RID: 1127
[Preserve]
public class EntityTurret : EntityAlive
{
	// Token: 0x170003EB RID: 1003
	// (get) Token: 0x06002489 RID: 9353 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsValidAimAssistSnapTarget
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170003EC RID: 1004
	// (get) Token: 0x0600248A RID: 9354 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsValidAimAssistSlowdownTarget
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170003ED RID: 1005
	// (get) Token: 0x0600248B RID: 9355 RVA: 0x000DB538 File Offset: 0x000D9738
	public override string LocalizedEntityName
	{
		get
		{
			return Localization.Get(this.EntityName, false);
		}
	}

	// Token: 0x170003EE RID: 1006
	// (get) Token: 0x0600248C RID: 9356 RVA: 0x000E916B File Offset: 0x000E736B
	// (set) Token: 0x0600248D RID: 9357 RVA: 0x000E9178 File Offset: 0x000E7378
	public int AmmoCount
	{
		get
		{
			return this.OriginalItemValue.Meta;
		}
		set
		{
			this.OriginalItemValue.Meta = value;
		}
	}

	// Token: 0x170003EF RID: 1007
	// (get) Token: 0x0600248E RID: 9358 RVA: 0x000E9186 File Offset: 0x000E7386
	public bool IsTurning
	{
		get
		{
			return this.IsOn && (this.YawController.IsTurning || this.PitchController.IsTurning);
		}
	}

	// Token: 0x0600248F RID: 9359 RVA: 0x000E91AC File Offset: 0x000E73AC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		this.bag = new Bag(this);
		base.Awake();
	}

	// Token: 0x170003F0 RID: 1008
	// (get) Token: 0x06002490 RID: 9360 RVA: 0x000E91C0 File Offset: 0x000E73C0
	// (set) Token: 0x06002491 RID: 9361 RVA: 0x000E91E5 File Offset: 0x000E73E5
	public override int Health
	{
		get
		{
			return (int)Mathf.Max((float)this.OriginalItemValue.MaxUseTimes - this.OriginalItemValue.UseTimes, 1f);
		}
		set
		{
			this.OriginalItemValue.UseTimes = (float)(this.OriginalItemValue.MaxUseTimes - value);
		}
	}

	// Token: 0x06002492 RID: 9362 RVA: 0x000E9200 File Offset: 0x000E7400
	public override void Init(int _entityClass)
	{
		base.Init(_entityClass);
		EntityClass entityClass = EntityClass.list[this.entityClass];
		base.transform.tag = "E_Vehicle";
		this.bag.SetupSlots(ItemStack.CreateArray(0));
		Transform transform = base.transform;
		this.thisRigidBody = transform.GetComponent<Rigidbody>();
		if (this.thisRigidBody)
		{
			this.thisRigidBody.centerOfMass = new Vector3(0f, 0.1f, 0f);
			this.thisRigidBody.sleepThreshold = this.thisRigidBody.mass * 0.01f * 0.01f * 0.5f;
			transform.gameObject.AddComponent<CollisionCallForward>().Entity = this;
			transform.gameObject.layer = 21;
			Utils.SetTagsRecursively(transform, "E_Vehicle");
		}
		this.alertEnabled = false;
	}

	// Token: 0x06002493 RID: 9363 RVA: 0x000E92DD File Offset: 0x000E74DD
	public override void Kill(DamageResponse _dmResponse)
	{
		_dmResponse.Fatal = false;
	}

	// Token: 0x06002494 RID: 9364 RVA: 0x00002914 File Offset: 0x00000B14
	public override void SetDead()
	{
	}

	// Token: 0x06002495 RID: 9365 RVA: 0x000E92DD File Offset: 0x000E74DD
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void ClientKill(DamageResponse _dmResponse)
	{
		_dmResponse.Fatal = false;
	}

	// Token: 0x06002496 RID: 9366 RVA: 0x000E92E8 File Offset: 0x000E74E8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override DamageResponse damageEntityLocal(DamageSource _damageSource, int _strength, bool _criticalHit, float impulseScale)
	{
		DamageResponse result = base.damageEntityLocal(_damageSource, _strength, _criticalHit, impulseScale);
		result.Fatal = false;
		return result;
	}

	// Token: 0x06002497 RID: 9367 RVA: 0x000E930C File Offset: 0x000E750C
	public override void OnEntityUnload()
	{
		base.OnEntityUnload();
		this.IsOn = false;
		if (GameManager.Instance != null && GameManager.Instance.World != null && this.belongsPlayerId != -1)
		{
			EntityAlive entityAlive = (EntityAlive)GameManager.Instance.World.GetEntity(this.belongsPlayerId);
			if (entityAlive != null)
			{
				entityAlive.RemoveOwnedEntity(this.entityId);
			}
		}
		this.FireController.Update();
	}

	// Token: 0x06002498 RID: 9368 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void AddCharacterController()
	{
	}

	// Token: 0x06002499 RID: 9369 RVA: 0x000E9384 File Offset: 0x000E7584
	public override void PostInit()
	{
		Transform transform = base.transform;
		transform.rotation = this.qrotation;
		this.StaticPosition = this.position;
		this.fallPos = this.position;
		this.YawController = transform.GetComponentInChildren<AutoTurretYawLerp>();
		this.PitchController = transform.GetComponentInChildren<AutoTurretPitchLerp>();
		this.FireController = transform.GetComponentInChildren<MiniTurretFireController>();
		this.Laser = transform.FindInChilds("turret_laser", false);
		this.Cone = transform.FindInChilds("turret_cone", false);
		PersistentPlayerData playerData = GameManager.Instance.GetPersistentPlayerList().GetPlayerData(this.OwnerID);
		if (playerData != null)
		{
			this.belongsPlayerId = playerData.EntityId;
		}
		this.HandleNavObject();
		this.InitTurret();
	}

	// Token: 0x0600249A RID: 9370 RVA: 0x000E9435 File Offset: 0x000E7635
	public override void InitInventory()
	{
		this.inventory = new EntityTurret.TurretInventory(GameManager.Instance, this);
	}

	// Token: 0x0600249B RID: 9371 RVA: 0x000E9448 File Offset: 0x000E7648
	public void InitTurret()
	{
		this.FireController.Init(base.EntityClass.Properties, this);
	}

	// Token: 0x0600249C RID: 9372 RVA: 0x000E9464 File Offset: 0x000E7664
	public override void OnUpdateEntity()
	{
		base.OnUpdateEntity();
		if (this.belongsPlayerId == -1)
		{
			PersistentPlayerList persistentPlayerList = GameManager.Instance.GetPersistentPlayerList();
			if (persistentPlayerList != null)
			{
				PersistentPlayerData playerData = persistentPlayerList.GetPlayerData(this.OwnerID);
				if (playerData != null)
				{
					this.belongsPlayerId = playerData.EntityId;
				}
			}
		}
		if (!this.Owner)
		{
			this.Owner = (EntityAlive)GameManager.Instance.World.GetEntity(this.belongsPlayerId);
			if (this.Owner != null)
			{
				this.Owner.AddOwnedEntity(this);
			}
		}
		if (this.uloam == null && this.OriginalItemValue.ItemClass != null)
		{
			this.uloam = base.gameObject.AddMissingComponent<UpdateLightOnAllMaterials>();
			this.uloam.AddRendererNameToIgnore("turret_laser");
			this.uloam.SetTintColorForItem(Vector3.one);
			if (this.OriginalItemValue.ItemClass.Properties.Values.ContainsKey(Block.PropTintColor))
			{
				this.uloam.SetTintColorForItem(Block.StringToVector3(this.OriginalItemValue.GetPropertyOverride(Block.PropTintColor, this.OriginalItemValue.ItemClass.Properties.Values[Block.PropTintColor])));
			}
			else
			{
				this.uloam.SetTintColorForItem(Block.StringToVector3(this.OriginalItemValue.GetPropertyOverride(Block.PropTintColor, "255,255,255")));
			}
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.IsOn = (this.OriginalItemValue.PercentUsesLeft > 0f);
			if ((int)EffectManager.GetValue(PassiveEffects.MagazineSize, this.OriginalItemValue, 0f, null, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) > 0)
			{
				this.IsOn &= (this.OriginalItemValue.Meta > 0);
			}
			if (GameManager.Instance != null && GameManager.Instance.World != null && this.belongsPlayerId != -1)
			{
				this.IsOn &= (this.Owner != null);
				if (this.Owner != null)
				{
					if (EffectManager.GetValue(PassiveEffects.DisableItem, this.OriginalItemValue, 0f, this.Owner, null, this.OriginalItemValue.ItemClass.ItemTags, true, true, true, true, true, 1, true, false) > 0f)
					{
						this.IsOn = false;
					}
					else
					{
						this.maxOwnerDistance = (int)EffectManager.GetValue(PassiveEffects.JunkTurretActiveRange, this.OriginalItemValue, 10f, this.Owner, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
						if (this.IsOn)
						{
							this.DistanceToOwner = base.GetDistanceSq(this.Owner);
							this.IsOn &= (this.DistanceToOwner < (float)(this.maxOwnerDistance * this.maxOwnerDistance));
						}
						if (this.IsOn)
						{
							int num = (int)EffectManager.GetValue(PassiveEffects.JunkTurretActiveCount, this.OriginalItemValue, 1f, this.Owner, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
							int num2 = 0;
							OwnedEntityData[] ownedEntities = this.Owner.GetOwnedEntities();
							for (int i = 0; i < ownedEntities.Length; i++)
							{
								EntityTurret entityTurret = GameManager.Instance.World.GetEntity(ownedEntities[i].Id) as EntityTurret;
								if (!(entityTurret == null) && entityTurret.entityId != this.entityId)
								{
									if (entityTurret.IsOn)
									{
										num2++;
									}
									this.IsOn &= (num2 <= num || this.DistanceToOwner < entityTurret.DistanceToOwner || this.ForceOn);
									if (!this.IsOn)
									{
										break;
									}
								}
							}
						}
					}
				}
			}
			else if (this.IsOn)
			{
				this.IsOn &= (this.belongsPlayerId == -1 && this.OwnerID == null);
			}
			this.ForceOn = false;
			if (this.TargetEntityId != this.lastTargetEntityId || this.IsOn != this.lastIsOn || this.OriginalItemValue.Equals(this.lastOriginalItemValue))
			{
				this.lastOriginalItemValue = this.OriginalItemValue.Clone();
				this.lastTargetEntityId = this.TargetEntityId;
				this.lastIsOn = this.IsOn;
				NetPackageTurretSync package = NetPackageManager.GetPackage<NetPackageTurretSync>().Setup(this.entityId, this.TargetEntityId, this.IsOn, this.OriginalItemValue);
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, true, -1, -1, -1, null, 192, false);
			}
		}
		if (this.Laser != null && this.IsOn != this.Laser.gameObject.activeSelf)
		{
			this.Laser.gameObject.SetActive(this.IsOn);
		}
	}

	// Token: 0x0600249D RID: 9373 RVA: 0x00002914 File Offset: 0x00000B14
	public override void MoveEntityHeaded(Vector3 _direction, bool _isDirAbsolute)
	{
	}

	// Token: 0x0600249E RID: 9374 RVA: 0x000E9924 File Offset: 0x000E7B24
	public void InitDynamicSpawn()
	{
		for (int i = 1; i < ItemClass.list.Length - 1; i++)
		{
			if (ItemClass.list[i] != null)
			{
				string name = ItemClass.list[i].Name;
				if (name == "gunBotT1JunkSledge" || name == "gunBotT2JunkTurret")
				{
					this.OwnerID = PlatformManager.InternalLocalUserIdentifier;
					this.OriginalItemValue = new ItemValue(ItemClass.list[i].Id, false);
					this.AmmoCount = ItemClass.GetForId(ItemClass.list[i].Id).GetInitialMetadata(this.OriginalItemValue);
					this.ForceOn = true;
					PersistentPlayerData playerData = GameManager.Instance.GetPersistentPlayerList().GetPlayerData(this.OwnerID);
					if (playerData != null)
					{
						(GameManager.Instance.World.GetEntity(playerData.EntityId) as EntityAlive).AddOwnedEntity(this);
					}
				}
			}
		}
	}

	// Token: 0x0600249F RID: 9375 RVA: 0x000E9A08 File Offset: 0x000E7C08
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateTransform()
	{
		Vector3 vector = base.transform.position + Origin.position;
		this.position = vector;
		this.StaticPosition = vector;
		Chunk chunk = GameManager.Instance.World.GetChunkFromWorldPos((int)vector.x, (int)vector.y, (int)vector.z) as Chunk;
		if (chunk != null && chunk.IsCollisionMeshGenerated)
		{
			if (this.posCheckTimer <= 0f)
			{
				this.posCheckTimer = 0.5f;
				int modelLayer = base.GetModelLayer();
				this.SetModelLayer(2, false, null);
				float y = this.fallPos.y;
				this.fallPos = vector;
				Ray ray = new Ray(vector + Vector3.up * 0.375f, Vector3.down);
				if (Voxel.Raycast(GameManager.Instance.World, ray, 255f, 1082195968, 128, 0.25f))
				{
					this.groundUpDirection = Voxel.phyxRaycastHit.normal;
					this.fallPos.y = Voxel.voxelRayHitInfo.fmcHit.pos.y;
					if (Vector3.Dot(Vector3.up, this.groundUpDirection) < 0.7f)
					{
						this.fallPos.y = this.fallPos.y - 0.1f;
					}
					if (this.fallPos.y < y)
					{
						this.fallDelay = 5;
					}
				}
				this.SetModelLayer(modelLayer, false, null);
			}
			float deltaTime = Time.deltaTime;
			this.posCheckTimer -= deltaTime;
			this.isFalling = false;
			if (vector != this.fallPos)
			{
				this.posCheckTimer = 0f;
				int num = this.fallDelay - 1;
				this.fallDelay = num;
				if (num < 0)
				{
					this.isFalling = true;
					base.transform.position = Vector3.MoveTowards(base.transform.position, this.fallPos - Origin.position, 5f * deltaTime);
					return;
				}
			}
		}
		else
		{
			this.posCheckTimer = 0.5f;
		}
	}

	// Token: 0x060024A0 RID: 9376 RVA: 0x000E9C08 File Offset: 0x000E7E08
	public void Collect(int _playerId)
	{
		EntityPlayerLocal entityPlayerLocal = this.world.GetEntity(_playerId) as EntityPlayerLocal;
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(entityPlayerLocal);
		ItemStack itemStack = new ItemStack(this.OriginalItemValue, 1);
		if (!uiforPlayer.xui.PlayerInventory.AddItem(itemStack))
		{
			GameManager.Instance.ItemDropServer(itemStack, entityPlayerLocal.GetPosition(), Vector3.zero, _playerId, 60f, false);
		}
		this.OriginalItemValue = ItemValue.None.Clone();
		this.PickedUpWaitingToDelete = true;
		this.bPlayerStatsChanged = true;
		base.transform.gameObject.SetActive(false);
	}

	// Token: 0x060024A1 RID: 9377 RVA: 0x000E9C98 File Offset: 0x000E7E98
	public bool CanInteract(int _interactingEntityId)
	{
		return !this.isFalling && !this.PickedUpWaitingToDelete && this.OriginalItemValue.type != 0 && (this.belongsPlayerId == _interactingEntityId || this.Health <= 1);
	}

	// Token: 0x060024A2 RID: 9378 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsDead()
	{
		return false;
	}

	// Token: 0x060024A3 RID: 9379 RVA: 0x000E9CD0 File Offset: 0x000E7ED0
	public override void Write(BinaryWriter _bw, bool _bNetworkWrite)
	{
		base.Write(_bw, _bNetworkWrite);
		_bw.Write(1);
		this.OwnerID.ToStream(_bw, false);
		this.OriginalItemValue.Write(_bw);
		StreamUtils.Write(_bw, this.StaticPosition);
	}

	// Token: 0x060024A4 RID: 9380 RVA: 0x000E9D08 File Offset: 0x000E7F08
	public override void Read(byte _version, BinaryReader _br)
	{
		base.Read(_version, _br);
		int num = _br.ReadInt32();
		this.OwnerID = PlatformUserIdentifierAbs.FromStream(_br, false, false);
		this.OriginalItemValue = ItemValue.None.Clone();
		this.OriginalItemValue.Read(_br);
		if (num > 0)
		{
			this.StaticPosition = StreamUtils.ReadVector3(_br);
		}
	}

	// Token: 0x04001B6B RID: 7019
	public const int SaveVersion = 1;

	// Token: 0x04001B6C RID: 7020
	public const string JunkTurretSledgeItem = "gunBotT1JunkSledge";

	// Token: 0x04001B6D RID: 7021
	public const string JunkTurretRangedItem = "gunBotT2JunkTurret";

	// Token: 0x04001B6E RID: 7022
	public AutoTurretYawLerp YawController;

	// Token: 0x04001B6F RID: 7023
	public AutoTurretPitchLerp PitchController;

	// Token: 0x04001B70 RID: 7024
	public MiniTurretFireController FireController;

	// Token: 0x04001B71 RID: 7025
	public Transform Laser;

	// Token: 0x04001B72 RID: 7026
	public Transform Cone;

	// Token: 0x04001B73 RID: 7027
	public Material ConeMaterial;

	// Token: 0x04001B74 RID: 7028
	public Color ConeColor;

	// Token: 0x04001B75 RID: 7029
	public float CenteredYaw;

	// Token: 0x04001B76 RID: 7030
	public float CenteredPitch;

	// Token: 0x04001B77 RID: 7031
	public bool TargetOwner;

	// Token: 0x04001B78 RID: 7032
	public bool TargetAllies;

	// Token: 0x04001B79 RID: 7033
	public bool TargetStrangers = true;

	// Token: 0x04001B7A RID: 7034
	public bool TargetEnemies = true;

	// Token: 0x04001B7B RID: 7035
	public int maxOwnerDistance = 10;

	// Token: 0x04001B7C RID: 7036
	public ItemValue OriginalItemValue = ItemValue.None.Clone();

	// Token: 0x04001B7D RID: 7037
	public bool PickedUpWaitingToDelete;

	// Token: 0x04001B7E RID: 7038
	public PlatformUserIdentifierAbs OwnerID;

	// Token: 0x04001B7F RID: 7039
	public bool IsOn;

	// Token: 0x04001B80 RID: 7040
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Rigidbody thisRigidBody;

	// Token: 0x04001B81 RID: 7041
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public UpdateLightOnAllMaterials uloam;

	// Token: 0x04001B82 RID: 7042
	public EntityAlive Owner;

	// Token: 0x04001B83 RID: 7043
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int lastTargetEntityId = -2;

	// Token: 0x04001B84 RID: 7044
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool lastIsOn;

	// Token: 0x04001B85 RID: 7045
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ItemValue lastOriginalItemValue = ItemValue.None.Clone();

	// Token: 0x04001B86 RID: 7046
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cPOSITION_UPDATE_CHECK_TIME = 0.5f;

	// Token: 0x04001B87 RID: 7047
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float posCheckTimer = 0.5f;

	// Token: 0x04001B88 RID: 7048
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int fallDelay;

	// Token: 0x04001B89 RID: 7049
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 fallPos;

	// Token: 0x04001B8A RID: 7050
	public int TargetEntityId = -1;

	// Token: 0x04001B8B RID: 7051
	public bool ForceOn;

	// Token: 0x04001B8C RID: 7052
	public float DistanceToOwner = float.MaxValue;

	// Token: 0x04001B8D RID: 7053
	public Vector3 groundPosition;

	// Token: 0x04001B8E RID: 7054
	public Vector3 groundUpDirection;

	// Token: 0x04001B8F RID: 7055
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isFalling;

	// Token: 0x04001B90 RID: 7056
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 StaticPosition;

	// Token: 0x04001B91 RID: 7057
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cLerpTimeScale = 8f;

	// Token: 0x04001B92 RID: 7058
	public int tmpBelongsPlayerID;

	// Token: 0x02000468 RID: 1128
	public class TurretInventory : Inventory
	{
		// Token: 0x060024A6 RID: 9382 RVA: 0x000E9DCA File Offset: 0x000E7FCA
		public TurretInventory(IGameManager _gameManager, EntityAlive _entity) : base(_gameManager, _entity)
		{
			this.cSlotCount = base.PUBLIC_SLOTS + 1;
			this.SetupSlots();
		}

		// Token: 0x060024A7 RID: 9383 RVA: 0x00002914 File Offset: 0x00000B14
		public override void Execute(int _actionIdx, bool _bReleased, PlayerActionsLocal _playerActions = null)
		{
		}

		// Token: 0x060024A8 RID: 9384 RVA: 0x000E9DE8 File Offset: 0x000E7FE8
		public void SetupSlots()
		{
			this.slots = new ItemInventoryData[this.cSlotCount];
			this.models = new Transform[this.cSlotCount];
			this.m_HoldingItemIdx = 0;
			base.Clear();
		}

		// Token: 0x060024A9 RID: 9385 RVA: 0x00002914 File Offset: 0x00000B14
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void updateHoldingItem()
		{
		}

		// Token: 0x04001B93 RID: 7059
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly int cSlotCount;
	}
}
