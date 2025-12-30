using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Audio;
using GamePath;
using Platform;
using RaycastPathing;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000430 RID: 1072
[Preserve]
public class EntityDrone : EntityNPC, ILockable
{
	// Token: 0x170003A1 RID: 929
	// (get) Token: 0x060020F1 RID: 8433 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsValidAimAssistSlowdownTarget
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170003A2 RID: 930
	// (get) Token: 0x060020F2 RID: 8434 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsValidAimAssistSnapTarget
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170003A3 RID: 931
	// (get) Token: 0x060020F3 RID: 8435 RVA: 0x000CFD6C File Offset: 0x000CDF6C
	public DroneLightManager lightManager
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			if (!this._lm)
			{
				this._lm = base.transform.GetComponentInChildren<DroneLightManager>();
			}
			return this._lm;
		}
	}

	// Token: 0x170003A4 RID: 932
	// (get) Token: 0x060020F4 RID: 8436 RVA: 0x000CFD92 File Offset: 0x000CDF92
	public EntityDrone.Orders OrderState
	{
		get
		{
			return this.orderState;
		}
	}

	// Token: 0x060020F5 RID: 8437 RVA: 0x000CFD9A File Offset: 0x000CDF9A
	[PublicizedFrom(EAccessModifier.Private)]
	public void setOrders(EntityDrone.Orders orders)
	{
		this.orderState = orders;
		this.initWorldValues(this.orderState == EntityDrone.Orders.Follow);
		if (GameManager.Instance.World.IsLocalPlayer(this.belongsPlayerId))
		{
			this.HandleNavObject();
		}
	}

	// Token: 0x060020F6 RID: 8438 RVA: 0x000CFDD0 File Offset: 0x000CDFD0
	public static bool IsValidForLocalPlayer()
	{
		PersistentPlayerData playerData = GameManager.Instance.GetPersistentPlayerList().GetPlayerData(PlatformManager.InternalLocalUserIdentifier);
		return playerData != null && EntityDrone.IsValidForPlayer(GameManager.Instance.World.GetEntity(playerData.EntityId) as EntityPlayerLocal);
	}

	// Token: 0x060020F7 RID: 8439 RVA: 0x000CFE18 File Offset: 0x000CE018
	public static bool IsValidForPlayer(EntityPlayerLocal localPlayer)
	{
		foreach (OwnedEntityData ownedEntityData in localPlayer.GetOwnedEntities())
		{
			if (ownedEntityData.ClassId != -1 && EntityClass.list[ownedEntityData.ClassId].entityClassName == "entityJunkDrone")
			{
				GameManager.ShowTooltip(localPlayer, Localization.Get("xuiMaxDeployedDronesReached", false), string.Empty, "ui_denied", null, false, false, 0f);
				return false;
			}
		}
		return true;
	}

	// Token: 0x060020F8 RID: 8440 RVA: 0x000CFE90 File Offset: 0x000CE090
	public static void OnClientSpawnRemote(Entity _entity)
	{
		GameManager instance = GameManager.Instance;
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			EntityDrone entityDrone = _entity as EntityDrone;
			if (entityDrone)
			{
				int i = 1;
				while (i < ItemClass.list.Length - 1)
				{
					ItemClass itemClass = ItemClass.list[i];
					if (itemClass != null && itemClass.Name == "gunBotT3JunkDrone")
					{
						entityDrone.OwnerID = PlatformManager.InternalLocalUserIdentifier;
						PersistentPlayerData playerData = instance.GetPersistentPlayerList().GetPlayerData(entityDrone.OwnerID);
						if (playerData != null)
						{
							entityDrone.belongsPlayerId = playerData.EntityId;
							(instance.World.GetEntity(playerData.EntityId) as EntityAlive).AddOwnedEntity(_entity);
							break;
						}
						break;
					}
					else
					{
						i++;
					}
				}
			}
		}
		instance.World.EntityLoadedDelegates -= EntityDrone.OnClientSpawnRemote;
	}

	// Token: 0x060020F9 RID: 8441 RVA: 0x000CFF58 File Offset: 0x000CE158
	public void InitDynamicSpawn()
	{
		int i = 1;
		while (i < ItemClass.list.Length - 1)
		{
			ItemClass itemClass = ItemClass.list[i];
			if (itemClass != null && itemClass.Name == "gunBotT3JunkDrone")
			{
				this.OriginalItemValue = new ItemValue(itemClass.Id, false);
				this.OwnerID = PlatformManager.InternalLocalUserIdentifier;
				PersistentPlayerData playerData = GameManager.Instance.GetPersistentPlayerList().GetPlayerData(this.OwnerID);
				if (playerData != null)
				{
					this.belongsPlayerId = playerData.EntityId;
					(GameManager.Instance.World.GetEntity(playerData.EntityId) as EntityAlive).AddOwnedEntity(this);
					return;
				}
				break;
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x060020FA RID: 8442 RVA: 0x000CFFFF File Offset: 0x000CE1FF
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		base.Awake();
		this.steering = new EntityDrone.EntitySteering(this);
		this.isLocked = true;
	}

	// Token: 0x060020FB RID: 8443 RVA: 0x000D001C File Offset: 0x000CE21C
	[PublicizedFrom(EAccessModifier.Private)]
	public void LateUpdate()
	{
		if (DroneManager.Debug_LocalControl)
		{
			this.debugInputRotX += Input.GetAxis("Mouse X") * 30f * 0.05f;
			this.debugInputRotY += Input.GetAxis("Mouse Y") * 30f * 0.05f;
			this.debugInputRotY = Mathf.Clamp(this.debugInputRotY, -90f, 90f);
			this.reconCam.transform.localRotation = Quaternion.AngleAxis(this.debugInputRotX, Vector3.up);
			this.reconCam.transform.localRotation *= Quaternion.AngleAxis(this.debugInputRotY, Vector3.left);
			RaycastHit raycastHit;
			if (Input.GetMouseButtonDown(0) && RaycastPathUtils.IsPositionBlocked(this.reconCam.ScreenPointToRay(Input.mousePosition), out raycastHit, 65536, true, 100f))
			{
				RaycastPathUtils.DrawBounds(World.worldToBlockPos(raycastHit.point + Origin.position), Color.yellow, 1f, 1f);
				this.pathMan.CreatePath(this.Owner.position, raycastHit.point + Origin.position, this.currentSpeedFlying, false, this.FollowHoverHeight);
			}
		}
	}

	// Token: 0x060020FC RID: 8444 RVA: 0x000D016C File Offset: 0x000CE36C
	public override void Init(int _entityClass)
	{
		base.Init(_entityClass);
	}

	// Token: 0x060020FD RID: 8445 RVA: 0x000D0175 File Offset: 0x000CE375
	public override void InitInventory()
	{
		this.inventory = new EntityDrone.DroneInventory(GameManager.Instance, this);
	}

	// Token: 0x060020FE RID: 8446 RVA: 0x000D0188 File Offset: 0x000CE388
	[PublicizedFrom(EAccessModifier.Private)]
	public void LogDrone(string format, params object[] args)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer || base.GetAttachedPlayerLocal())
		{
			format = string.Format("{0} Drone {1}", GameManager.frameCount, format);
			Log.Out(format, args);
		}
	}

	// Token: 0x060020FF RID: 8447 RVA: 0x000D01C4 File Offset: 0x000CE3C4
	public override void PostInit()
	{
		this.LogDrone("PostInit {0}, {1} (chunk {2}), rbPos {3}", new object[]
		{
			this,
			this.position,
			World.toChunkXZ(this.position),
			this.PhysicsTransform.position + Origin.position
		});
		float num = 1f / base.transform.localScale.x;
		this.interactionCollider = base.gameObject.GetComponent<BoxCollider>();
		if (this.interactionCollider)
		{
			this.interactionCollider.center = new Vector3(0f, 0.05f * num, 0.05f * num);
			this.interactionCollider.size = new Vector3(2.5f, 2f, 2f);
		}
		this.sensors = new EntityDrone.DroneSensors(this);
		this.sensors.Init();
		this.initWorldValues(this.orderState == EntityDrone.Orders.Follow);
		this.IsFlyMode.Value = true;
		this.bCanClimbLadders = true;
		this.bCanClimbVertical = true;
		this.prefabColor = this.GetPaintColor();
	}

	// Token: 0x06002100 RID: 8448 RVA: 0x000D02E8 File Offset: 0x000CE4E8
	public override void OnAddedToWorld()
	{
		if (this.itemvalueToLoad != null)
		{
			this.OriginalItemValue = this.itemvalueToLoad;
		}
		this.isOwnerSyncPending = true;
		this.InitWeapons();
		this.LoadMods();
		if (this.nativeCollider)
		{
			this.nativeCollider.enabled = true;
		}
		this.Health = Mathf.RoundToInt(base.Stats.Health.Max * (1f - this.OriginalItemValue.UseTimes / (float)this.OriginalItemValue.MaxUseTimes));
		Animator componentInChildren = base.GetComponentInChildren<Animator>();
		if (!componentInChildren.enabled)
		{
			componentInChildren.enabled = true;
		}
		componentInChildren.Play("Base Layer.Idle", 0, 0f);
		componentInChildren.Update(0f);
		componentInChildren.StopPlayback();
		this.pathMan = new FloodFillEntityPathGenerator(this.world, this);
		Origin.OriginChanged = (Action<Vector3>)Delegate.Combine(Origin.OriginChanged, new Action<Vector3>(this.OnOriginChanged));
	}

	// Token: 0x06002101 RID: 8449 RVA: 0x000D03DC File Offset: 0x000CE5DC
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnOriginChanged(Vector3 _origin)
	{
		string str = "EntityDrone - OnOriginChanged: ";
		Vector3 vector = _origin;
		Log.Out(str + vector.ToString());
	}

	// Token: 0x06002102 RID: 8450 RVA: 0x000D0407 File Offset: 0x000CE607
	public override void OnEntityUnload()
	{
		Origin.OriginChanged = (Action<Vector3>)Delegate.Remove(Origin.OriginChanged, new Action<Vector3>(this.OnOriginChanged));
		this.UnRegsiterMovingLights();
		base.OnEntityUnload();
	}

	// Token: 0x06002103 RID: 8451 RVA: 0x000D0435 File Offset: 0x000CE635
	public override bool CanUpdateEntity()
	{
		return this.Owner || base.CanUpdateEntity();
	}

	// Token: 0x06002104 RID: 8452 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool CanNavigatePath()
	{
		return true;
	}

	// Token: 0x06002105 RID: 8453 RVA: 0x000D044C File Offset: 0x000CE64C
	public override float GetEyeHeight()
	{
		if (this.head == null)
		{
			this.head = base.transform.FindInChilds("Head", false);
		}
		return this.head.position.y - base.transform.position.y;
	}

	// Token: 0x06002106 RID: 8454 RVA: 0x000D04A0 File Offset: 0x000CE6A0
	public override Ray GetLookRay()
	{
		return new Ray(this.position + new Vector3(0f, this.GetEyeHeight(), 0f), (this.currentTarget == null) ? this.GetLookVector() : (this.currentTarget.getHeadPosition() - this.position).normalized);
	}

	// Token: 0x06002107 RID: 8455 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool CanBePushed()
	{
		return true;
	}

	// Token: 0x06002108 RID: 8456 RVA: 0x000D0506 File Offset: 0x000CE706
	public override float GetWeight()
	{
		return base.GetWeight();
	}

	// Token: 0x06002109 RID: 8457 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsDead()
	{
		return false;
	}

	// Token: 0x0600210A RID: 8458 RVA: 0x000D050E File Offset: 0x000CE70E
	public override bool IsAttackValid()
	{
		return this.stunWeapon.canFire() || this.machineGunWeapon.canFire();
	}

	// Token: 0x170003A5 RID: 933
	// (get) Token: 0x0600210B RID: 8459 RVA: 0x000C3E01 File Offset: 0x000C2001
	// (set) Token: 0x0600210C RID: 8460 RVA: 0x000D052C File Offset: 0x000CE72C
	public override int Health
	{
		get
		{
			return (int)base.Stats.Health.Value;
		}
		set
		{
			float num = (float)Mathf.Max(value, 1);
			if (num == 1f && this.state != EntityDrone.State.Shutdown)
			{
				this.isShutdownPending = true;
			}
			base.Stats.Health.Value = num;
		}
	}

	// Token: 0x0600210D RID: 8461 RVA: 0x000D056C File Offset: 0x000CE76C
	public override int DamageEntity(DamageSource _damageSource, int _strength, bool _criticalHit, float _impulseScale)
	{
		int strength = Mathf.RoundToInt((float)_strength * this.armorDamageReduction);
		if (_damageSource.damageType == EnumDamageTypes.BloodLoss)
		{
			this.Buffs.RemoveBuff("buffInjuryBleeding", true);
			strength = 0;
		}
		EntityAlive entityAlive = (EntityAlive)this.world.GetEntity(_damageSource.getEntityId());
		if (this.Owner && entityAlive)
		{
			if (!this.debugFriendlyFire && entityAlive && entityAlive.factionId == this.Owner.factionId)
			{
				strength = 0;
			}
		}
		else
		{
			strength = 0;
		}
		return base.DamageEntity(_damageSource, strength, _criticalHit, _impulseScale);
	}

	// Token: 0x0600210E RID: 8462 RVA: 0x00002914 File Offset: 0x00000B14
	public override void PlayStepSound(float _volume)
	{
	}

	// Token: 0x0600210F RID: 8463 RVA: 0x000D0608 File Offset: 0x000CE808
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void HandleNavObject()
	{
		NavObjectManager instance = NavObjectManager.Instance;
		EntityClass eClass = EntityClass.list[this.entityClass];
		if (eClass.NavObject != "")
		{
			NavObject navObject = instance.NavObjectList.Find(delegate(NavObject n)
			{
				NavObjectClass navObjectClass = n.NavObjectClass;
				return ((navObjectClass != null) ? navObjectClass.NavObjectClassName : null) == eClass.NavObject;
			});
			if (navObject != null)
			{
				instance.UnRegisterNavObject(navObject);
			}
			this.NavObject = instance.RegisterNavObject(eClass.NavObject, this, "", false);
		}
		EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		if (primaryPlayer != null)
		{
			primaryPlayer.Waypoints.UpdateEntityDroneWayPoint(this, this.OrderState == EntityDrone.Orders.Follow, false);
		}
	}

	// Token: 0x06002110 RID: 8464 RVA: 0x000D06C0 File Offset: 0x000CE8C0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void AddCharacterController()
	{
		base.AddCharacterController();
		if (this.PhysicsTransform == null)
		{
			return;
		}
		if (this.m_characterController == null)
		{
			return;
		}
		this.RootMotion = false;
		this.m_characterController.SetSize(Vector3.zero, this.physColHeight, this.physColHeight * 0.5f);
		this.setNoClip(true);
	}

	// Token: 0x06002111 RID: 8465 RVA: 0x0003E2E0 File Offset: 0x0003C4E0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override float GetPushBoundsVertical()
	{
		return 1f;
	}

	// Token: 0x06002112 RID: 8466 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateStepSound(float _distX, float _distZ, float _rotYDelta)
	{
	}

	// Token: 0x06002113 RID: 8467 RVA: 0x000D071C File Offset: 0x000CE91C
	public override void Write(BinaryWriter _bw, bool _bNetworkWrite)
	{
		base.Write(_bw, _bNetworkWrite);
		_bw.Write(1);
		this.OwnerID.ToStream(_bw, false);
		this.OriginalItemValue = this.GetUpdatedItemValue();
		this.OriginalItemValue.Write(_bw);
		ushort num = 49251;
		_bw.Write(num);
		this.WriteSyncData(_bw, num);
	}

	// Token: 0x06002114 RID: 8468 RVA: 0x000D0774 File Offset: 0x000CE974
	public override void Read(byte _version, BinaryReader _br)
	{
		base.Read(_version, _br);
		_br.ReadInt32();
		this.OwnerID = PlatformUserIdentifierAbs.FromStream(_br, false, false);
		this.OriginalItemValue = ItemValue.None.Clone();
		this.OriginalItemValue.Read(_br);
		ushort syncFlags = _br.ReadUInt16();
		this.ReadSyncData(_br, syncFlags, 0);
	}

	// Token: 0x06002115 RID: 8469 RVA: 0x000D07CC File Offset: 0x000CE9CC
	public override EntityActivationCommand[] GetActivationCommands(Vector3i _tePos, EntityAlive _entityFocusing)
	{
		bool flag = !this.IsDead();
		if (this.IsDead())
		{
			return new EntityActivationCommand[0];
		}
		bool flag2 = false;
		if (this.belongsToPlayerId(_entityFocusing.entityId))
		{
			flag2 = ((_entityFocusing as EntityPlayerLocal).IsGodMode.Value && Debug.isDebugBuild);
			return new EntityActivationCommand[]
			{
				new EntityActivationCommand("talk", "talk", flag && this.state != EntityDrone.State.Shutdown, null),
				new EntityActivationCommand("service", "service", flag2, null),
				new EntityActivationCommand("repair", "wrench", (float)this.Health < base.Stats.Health.Max, null),
				new EntityActivationCommand("lock", "lock", !this.isLocked, null),
				new EntityActivationCommand("unlock", "unlock", this.isLocked, null),
				new EntityActivationCommand("keypad", "keypad", true, null),
				new EntityActivationCommand("take", "hand", true, null),
				new EntityActivationCommand("stay", "run_and_gun", flag && this.OrderState != EntityDrone.Orders.Stay && this.state != EntityDrone.State.Shutdown, null),
				new EntityActivationCommand("follow", "run", flag && this.OrderState != EntityDrone.Orders.Follow && this.state != EntityDrone.State.Shutdown, null),
				new EntityActivationCommand("heal", "cardio", flag && this.state != EntityDrone.State.Shutdown && this.TargetCanBeHealed(_entityFocusing), null),
				new EntityActivationCommand("storage", "loot_sack", true, null),
				new EntityActivationCommand("drone_silent", this.isQuietMode ? "sight" : "stealth", true, null),
				new EntityActivationCommand("drone_light", this.isFlashlightOn ? "electric_switch" : "lightbulb", this.isFlashlightAttached, null),
				new EntityActivationCommand("force_pickup", "store_all_up", flag2, null)
			};
		}
		bool flag3 = !this.isLocked || this.belongsToPlayerId(_entityFocusing.entityId) || this.IsUserAllowed(PlatformManager.InternalLocalUserIdentifier);
		bool flag4 = this.isLocked && !this.IsUserAllowed(PlatformManager.InternalLocalUserIdentifier);
		bool flag5 = (float)this.Health < base.Stats.Health.Max;
		if (!flag3 && !flag4 && !flag5 && !flag2)
		{
			this.PlaySound("ui_denied", 1f);
			return new EntityActivationCommand[0];
		}
		return new EntityActivationCommand[]
		{
			new EntityActivationCommand("storage", "loot_sack", flag3, null),
			new EntityActivationCommand("keypad", "keypad", flag4, null),
			new EntityActivationCommand("repair", "wrench", flag5, null),
			new EntityActivationCommand("force_pickup", "store_all_up", flag2, null)
		};
	}

	// Token: 0x06002116 RID: 8470 RVA: 0x000D0B04 File Offset: 0x000CED04
	public override bool OnEntityActivated(int _indexInBlockActivationCommands, Vector3i _tePos, EntityAlive _entityFocusing)
	{
		EntityPlayerLocal entityPlayer = _entityFocusing as EntityPlayerLocal;
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(entityPlayer);
		int requestType = -1;
		if (this.belongsToPlayerId(_entityFocusing.entityId))
		{
			switch (_indexInBlockActivationCommands)
			{
			case 0:
				this.startDialog(_entityFocusing);
				break;
			case 1:
				requestType = _indexInBlockActivationCommands;
				break;
			case 2:
				this.doRepairAction(entityPlayer, uiforPlayer);
				break;
			case 3:
				this.PlaySound("misc/locking", 1f);
				this.isLocked = !this.isLocked;
				this.SendSyncData(2);
				break;
			case 4:
				this.PlaySound("misc/unlocking", 1f);
				this.isLocked = !this.isLocked;
				this.SendSyncData(2);
				break;
			case 5:
				this.doKeypadAction(uiforPlayer);
				break;
			case 6:
				this.pickup(_entityFocusing);
				break;
			case 7:
				this.SentryMode();
				break;
			case 8:
				this.FollowMode();
				break;
			case 9:
				if (!this.healWeapon.hasHealingItem())
				{
					GameManager.ShowTooltip(this.Owner as EntityPlayerLocal, Localization.Get("xuiDroneNeedsHealItemsStored", false), string.Empty, "ui_denied", null, false, false, 0f);
					this.PlaySound("drone_empty", 1f);
				}
				else
				{
					this.HealOwner();
				}
				break;
			case 10:
				requestType = _indexInBlockActivationCommands;
				break;
			case 11:
			{
				this.isQuietMode = !this.isQuietMode;
				Handle handle = this.idleLoop;
				if (handle != null)
				{
					handle.Stop(this.entityId);
				}
				this.idleLoop = null;
				this.SendSyncData(32);
				break;
			}
			case 12:
				this.doToggleLightAction();
				break;
			case 13:
				this.pickup(_entityFocusing);
				break;
			}
		}
		else
		{
			switch (_indexInBlockActivationCommands)
			{
			case 0:
				requestType = 10;
				break;
			case 1:
				this.doKeypadAction(uiforPlayer);
				break;
			case 2:
				this.doRepairAction(entityPlayer, uiforPlayer);
				break;
			case 3:
				this.pickup(_entityFocusing);
				break;
			}
		}
		this.processRequest(entityPlayer, requestType);
		return false;
	}

	// Token: 0x06002117 RID: 8471 RVA: 0x000D0D00 File Offset: 0x000CEF00
	public override void OnUpdateEntity()
	{
		base.OnUpdateEntity();
		if (DroneManager.Debug_LocalControl)
		{
			return;
		}
		this.SyncOwnerData();
		this.UpdateTransitionState();
		this.UpdateAnimStates();
		this.UpdateShutdownState();
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && this.Owner && this.isOutOfRange(this.Owner.position, this.MaxDistFromOwner) && this.state != EntityDrone.State.Shutdown && this.state != EntityDrone.State.Sentry)
		{
			this.teleportState();
		}
		if (!this.isQuietMode && this.idleLoop == null && this.state == EntityDrone.State.Idle && !GameManager.IsDedicatedServer)
		{
			this.idleLoop = this.PlaySoundLoop("drone_idle_hover", 0.2f);
		}
		if (!this.isQuietMode && this.idleLoop != null && this.state == EntityDrone.State.Idle && !GameManager.IsDedicatedServer)
		{
			this.notifySoundNoise(0.2f, 5f);
		}
		if ((this.state == EntityDrone.State.Idle || this.state == EntityDrone.State.Sentry || this.state == EntityDrone.State.Follow) && this.areaScanTimer > 0f)
		{
			this.areaScanTimer -= Time.deltaTime;
			if (this.areaScanTimer <= 0f)
			{
				this.isInConfinedSpace = this.pathMan.IsConfinedSpace(this.position, 3f, false);
				this.areaScanTimer = this.areaScanTime;
			}
		}
		this.UpdatePartyBuffs();
		if (this.Owner)
		{
			if (this.currentTarget)
			{
				if (!this.steering.IsInRange(this.currentTarget.position, this.FollowDistance * 2f))
				{
					if (this.decelerationTime > 0f)
					{
						this.decelerationTime = 0f;
					}
					this.accelerationTime += 0.05f;
					this.currentSpeedFlying = Mathf.Lerp(this.currentSpeedFlying, 15f, Mathf.Clamp01(this.accelerationTime / this.SpeedFlying));
				}
				else
				{
					if (this.accelerationTime > 0f)
					{
						this.accelerationTime = 0f;
					}
					this.decelerationTime += 0.05f;
					this.currentSpeedFlying = Mathf.Lerp(this.currentSpeedFlying, this.SpeedFlying, Mathf.Clamp01(this.decelerationTime / (this.SpeedFlying * 0.5f)));
				}
			}
			this.UpdateDroneSystems();
			EntityPlayerLocal entityPlayerLocal = this.Owner as EntityPlayerLocal;
			if (entityPlayerLocal && this.state == EntityDrone.State.Idle)
			{
				if (this.focusBoxNode == null)
				{
					if (entityPlayerLocal.MoveController.FocusBoxPosition == World.worldToBlockPos(this.position))
					{
						RaycastNode raycastNode = RaycastPathWorldUtils.FindNodeType(RaycastPathWorldUtils.ScanVolume(this.world, this.position, false, false, false, 0f), cPathNodeType.Air);
						if (raycastNode != null)
						{
							this.focusBoxNode = raycastNode;
						}
					}
				}
				else
				{
					Vector3 vector = this.focusBoxNode.Center - this.position;
					RaycastPathUtils.DrawLine(this.position, this.focusBoxNode.Center, Color.yellow, 1f);
					if (this.isOutOfRange(this.focusBoxNode.Center, 0.25f))
					{
						this.move(vector.normalized);
					}
					else
					{
						this.focusBoxNode = null;
					}
				}
			}
			else if (this.state != EntityDrone.State.Idle && this.focusBoxNode != null)
			{
				this.focusBoxNode = null;
			}
		}
		this.UpdateDroneServiceMenu();
	}

	// Token: 0x06002118 RID: 8472 RVA: 0x000D104C File Offset: 0x000CF24C
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateTransitionState()
	{
		if (this.transitionState != EntityDrone.State.None)
		{
			if (this.state == this.transitionState)
			{
				this.transitionState = EntityDrone.State.None;
				return;
			}
			EntityDrone.State state = this.transitionState;
			if (state != EntityDrone.State.Idle)
			{
				if (state != EntityDrone.State.Heal)
				{
					if (state == EntityDrone.State.Shutdown)
					{
						this.isShutdownPending = true;
					}
					else
					{
						this.setState(this.transitionState);
					}
				}
				else if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
				{
					this.HealOwnerServer();
				}
				else
				{
					this.healWeapon.RegisterOnFireComplete(new Action(this.onHealDone));
					this.healWeapon.Fire(this.currentTarget);
					Manager.Play(this, "drone_healeffect", 1f, false);
					this.setState(this.transitionState);
				}
			}
			else if (this.state == EntityDrone.State.Shutdown)
			{
				this.setShutdown(false);
			}
			else
			{
				this.setState(this.transitionState);
			}
			this.transitionState = EntityDrone.State.None;
		}
	}

	// Token: 0x06002119 RID: 8473 RVA: 0x000D112C File Offset: 0x000CF32C
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateAnimStates()
	{
		if (this.stateTime < 1f)
		{
			Animator componentInChildren = base.GetComponentInChildren<Animator>();
			if (!componentInChildren.enabled && !componentInChildren.GetAnimatorTransitionInfo(0).IsName("Base Layer.Idle"))
			{
				this.isAnimationStateSet = false;
			}
		}
		if (!this.isAnimationStateSet)
		{
			Animator componentInChildren2 = base.GetComponentInChildren<Animator>();
			if (!componentInChildren2.enabled)
			{
				componentInChildren2.enabled = true;
			}
			if (this.Health > 1 && this.Owner)
			{
				if (this.PlayWakeupAnim)
				{
					this.playWakeupAnim();
					this.PlayWakeupAnim = false;
				}
				else
				{
					this.playIdleAnim();
				}
			}
			else
			{
				componentInChildren2.Play("Base Layer.Idle", 0, 0f);
				componentInChildren2.Update(0f);
				componentInChildren2.StopPlayback();
				componentInChildren2.enabled = false;
			}
			this.isAnimationStateSet = true;
		}
		if (this.wakeupAnimTime > 0f)
		{
			this.wakeupAnimTime -= 0.05f;
			if (this.wakeupAnimTime <= 0f && !GameManager.IsDedicatedServer)
			{
				if (this.Owner)
				{
					Manager.Stop(this.Owner.entityId, "drone_take");
				}
				if (GameManager.Instance.World.IsLocalPlayer(this.belongsPlayerId))
				{
					this.PlayVO("drone_wakeup", false, 1f);
				}
			}
		}
	}

	// Token: 0x0600211A RID: 8474 RVA: 0x000D1274 File Offset: 0x000CF474
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateShutdownState()
	{
		if (this.isShutdownPending)
		{
			this.performShutdown();
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				this.SendSyncData(32768);
			}
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			if (!this.Owner && this.state != EntityDrone.State.Shutdown)
			{
				this.performShutdown();
				this.SendSyncData(32768);
			}
			if (this.Owner && this.Owner.Health <= 0 && this.state != EntityDrone.State.Shutdown && this.state != EntityDrone.State.Sentry)
			{
				this.performShutdown();
				if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
				{
					this.SendSyncData(32768);
				}
			}
			if (this.Health > 1 && this.Owner && this.Owner.Health > 1 && Vector3.Distance(this.position, this.Owner.position) < 10f && this.state == EntityDrone.State.Shutdown)
			{
				this.setShutdown(false);
				if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
				{
					this.SendSyncData(32768);
				}
			}
			if (this.state == EntityDrone.State.Shutdown)
			{
				this.processShutdown();
			}
		}
	}

	// Token: 0x0600211B RID: 8475 RVA: 0x000D13A0 File Offset: 0x000CF5A0
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdatePartyBuffs()
	{
		if (this.Owner && this.isSupportModAttached && !this.isEntityRemote)
		{
			this.BuffAllies();
			EntityPlayer entityPlayer = this.Owner as EntityPlayer;
			if (!this.partyEventsSet && entityPlayer && entityPlayer.Party != null)
			{
				this.registerPartyEvents(entityPlayer, true);
			}
		}
	}

	// Token: 0x0600211C RID: 8476 RVA: 0x000D13FC File Offset: 0x000CF5FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateTasks()
	{
		if (DroneManager.Debug_LocalControl)
		{
			float num = this.debugInputSpeed;
			if (InputUtils.ShiftKeyPressed)
			{
				num *= 10f;
			}
			this.debugInputFwd = this.reconCam.transform.forward;
			this.debugInputFwd.y = 0f;
			if (Input.GetKey(KeyCode.W))
			{
				this.move(this.debugInputFwd, num);
			}
			if (Input.GetKey(KeyCode.S))
			{
				this.move(-this.debugInputFwd, num);
			}
			this.debugInputRgt = this.reconCam.transform.right;
			this.debugInputRgt.y = 0f;
			if (Input.GetKey(KeyCode.A))
			{
				this.move(-this.debugInputRgt, num);
			}
			if (Input.GetKey(KeyCode.D))
			{
				this.move(this.debugInputRgt, num);
			}
			this.debugInputUp = this.reconCam.transform.up;
			this.debugInputUp.x = 0f;
			this.debugInputUp.z = 0f;
			if (Input.GetKey(KeyCode.Space))
			{
				this.move(this.debugInputUp, num * 0.5f);
			}
			if (Input.GetKey(KeyCode.C))
			{
				this.move(-this.debugInputUp, num * 0.5f);
			}
			RaycastPathUtils.DrawBounds(this.Owner.GetBlockPosition().ToVector3CenterXZ() - new Vector3(0.5f, 0f, 0.5f), Color.cyan, 1f, 1f);
			return;
		}
		base.GetEntitySenses().ClearIfExpired();
		if (this.Owner != null)
		{
			this.updateState();
			this.debugUpdate();
		}
	}

	// Token: 0x0600211D RID: 8477 RVA: 0x000D15B0 File Offset: 0x000CF7B0
	public void SetItemValueToLoad(ItemValue itemValue)
	{
		this.itemvalueToLoad = itemValue.Clone();
	}

	// Token: 0x0600211E RID: 8478 RVA: 0x000D15C0 File Offset: 0x000CF7C0
	public void LoadMods()
	{
		LootContainer lootContainer = LootContainer.GetLootContainer("roboticDrone", true);
		Vector2i size = lootContainer.size;
		if (this.lootContainer == null)
		{
			this.lootContainer = new TileEntityLootContainer(null);
			this.lootContainer.entityId = this.entityId;
			this.lootContainer.lootListName = lootContainer.Name;
			this.lootContainer.SetContainerSize(size, true);
			this.bag.SetupSlots(ItemStack.CreateArray(size.x * size.y));
		}
		this.lootContainer.bWasTouched = true;
		this.lightManager.DisableMaterials("junkDroneLamp");
		GameObject gameObject = base.transform.FindInChilds("freightBox", false).gameObject;
		GameObject gameObject2 = base.transform.FindInChilds("armor", false).gameObject;
		GameObject gameObject3 = base.transform.FindInChilds("machineGun", false).gameObject;
		GameObject gameObject4 = base.transform.FindInChilds("teddyBear", false).gameObject;
		if (gameObject != null)
		{
			gameObject.SetActive(false);
		}
		if (gameObject2 != null)
		{
			gameObject2.SetActive(false);
		}
		if (gameObject3 != null)
		{
			gameObject3.SetActive(false);
		}
		if (gameObject4 != null)
		{
			gameObject4.SetActive(false);
		}
		int num = size.x * size.y;
		if (this.OriginalItemValue.HasMods())
		{
			for (int i = 0; i < this.OriginalItemValue.Modifications.Length; i++)
			{
				ItemValue itemValue = this.OriginalItemValue.Modifications[i];
				if (itemValue.ItemClass != null)
				{
					string name = itemValue.ItemClass.Name;
					uint num2 = <PrivateImplementationDetails>.ComputeStringHash(name);
					if (num2 <= 2400030839U)
					{
						if (num2 != 1912183181U)
						{
							if (num2 != 2266484491U)
							{
								if (num2 == 2400030839U)
								{
									if (name == "modRoboticDroneWeaponMod")
									{
										this.isGunModAttached = true;
										if (gameObject3)
										{
											gameObject3.SetActive(true);
										}
									}
								}
							}
							else if (name == "modRoboticDroneMoraleBoosterMod")
							{
								this.isSupportModAttached = true;
								if (gameObject4)
								{
									gameObject4.SetActive(true);
								}
							}
						}
						else if (name == "modRoboticDroneCargoMod")
						{
							num += 8;
							if (gameObject)
							{
								gameObject.SetActive(true);
							}
						}
					}
					else if (num2 <= 3474526689U)
					{
						if (num2 != 2404831999U)
						{
							if (num2 == 3474526689U)
							{
								if (name == "modRoboticDroneArmorPlatingMod")
								{
									this.armorDamageReduction = 0.5f;
									if (gameObject2)
									{
										gameObject2.SetActive(true);
									}
								}
							}
						}
						else if (name == "modRoboticDroneMedicMod")
						{
							this.isHealModAttached = true;
						}
					}
					else if (num2 != 3914512375U)
					{
						if (num2 == 4027736419U)
						{
							if (name == "modRoboticDroneStunWeaponMod")
							{
								this.isStunModAttached = true;
							}
						}
					}
					else if (name == "modRoboticDroneHeadlampMod")
					{
						this.isFlashlightAttached = true;
						DroneLightManager.LightEffect[] lightEffects = this.lightManager.LightEffects;
						if (lightEffects.Length != 0)
						{
							LightManager.RegisterMovingLight(this, lightEffects[0].linkedObjects[0].GetComponent<Light>());
						}
						if (this.isFlashlightOn)
						{
							this.lightManager.InitMaterials("junkDroneLamp");
						}
					}
				}
			}
		}
		ItemStack[] array = ItemStack.CreateArray(num);
		Array.Copy(this.lootContainer.items, 0, array, 0, (this.lootContainer.items.Length < num) ? this.lootContainer.items.Length : num);
		this.bag.SetSlots(array);
		this.lootContainer.SetContainerSize(new Vector2i(8, Mathf.RoundToInt((float)(num / 8))), false);
		this.lootContainer.items = this.bag.GetSlots();
		Color color = this.prefabColor;
		ItemValue itemValue2 = this.OriginalItemValue.CosmeticMods[0];
		if (this.OriginalItemValue.CosmeticMods.Length != 0 && itemValue2 != null && !itemValue2.IsEmpty())
		{
			Vector3 vector = Block.StringToVector3(this.OriginalItemValue.GetPropertyOverride(Block.PropTintColor, "255,255,255"));
			color.r = vector.x;
			color.g = vector.y;
			color.b = vector.z;
		}
		for (int j = 0; j < this.paintableParts.Length; j++)
		{
			this.SetPaint(this.paintableParts[j], color);
		}
	}

	// Token: 0x0600211F RID: 8479 RVA: 0x000D1A4A File Offset: 0x000CFC4A
	[PublicizedFrom(EAccessModifier.Private)]
	public void initWorldValues(bool value)
	{
		this.bWillRespawn = value;
	}

	// Token: 0x06002120 RID: 8480 RVA: 0x000D1A54 File Offset: 0x000CFC54
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitWeapons()
	{
		this.stunWeapon = new DroneWeapons.StunBeamWeapon(this);
		this.stunWeapon.Init();
		this.machineGunWeapon = new DroneWeapons.MachineGunWeapon(this);
		this.machineGunWeapon.Init();
		this.healWeapon = new DroneWeapons.HealBeamWeapon(this);
		this.healWeapon.Init();
	}

	// Token: 0x06002121 RID: 8481 RVA: 0x000D1AA6 File Offset: 0x000CFCA6
	public Color GetPaintColor()
	{
		return base.transform.FindRecursive("BaseMesh").GetComponentInChildren<Renderer>().sharedMaterial.color;
	}

	// Token: 0x06002122 RID: 8482 RVA: 0x000D1AC8 File Offset: 0x000CFCC8
	public void SetPaint(string childName, Color color)
	{
		Transform transform = base.transform.FindRecursive(childName);
		if (transform && transform.gameObject.activeSelf)
		{
			Renderer[] componentsInChildren = transform.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].material.color = color;
			}
		}
	}

	// Token: 0x170003A6 RID: 934
	// (get) Token: 0x06002123 RID: 8483 RVA: 0x000D1B1A File Offset: 0x000CFD1A
	// (set) Token: 0x06002124 RID: 8484 RVA: 0x000D1B22 File Offset: 0x000CFD22
	public bool PlayWakeupAnim { get; set; }

	// Token: 0x06002125 RID: 8485 RVA: 0x000D1B2B File Offset: 0x000CFD2B
	[PublicizedFrom(EAccessModifier.Private)]
	public void playWakeupAnim()
	{
		base.GetComponentInChildren<Animator>().Play("Base Layer.SpawnIn");
		this.wakeupAnimTime = 2.5f;
	}

	// Token: 0x06002126 RID: 8486 RVA: 0x000D1B48 File Offset: 0x000CFD48
	[PublicizedFrom(EAccessModifier.Private)]
	public void playIdleAnim()
	{
		Animator componentInChildren = base.GetComponentInChildren<Animator>();
		componentInChildren.Play("Base Layer.Idle", 0, 0f);
		componentInChildren.Update(0f);
	}

	// Token: 0x06002127 RID: 8487 RVA: 0x000D1B6C File Offset: 0x000CFD6C
	[PublicizedFrom(EAccessModifier.Private)]
	public void UnRegsiterMovingLights()
	{
		DroneLightManager.LightEffect[] lightEffects = this.lightManager.LightEffects;
		if (lightEffects.Length != 0)
		{
			LightManager.UnRegisterMovingLight(this, lightEffects[0].linkedObjects[0].GetComponent<Light>());
		}
	}

	// Token: 0x06002128 RID: 8488 RVA: 0x000D1B9E File Offset: 0x000CFD9E
	[PublicizedFrom(EAccessModifier.Private)]
	public void doToggleLightAction()
	{
		this.isFlashlightOn = !this.isFlashlightOn;
		this.setFlashlightOn(this.isFlashlightOn);
		this.SendSyncData(64);
	}

	// Token: 0x06002129 RID: 8489 RVA: 0x000D1BC3 File Offset: 0x000CFDC3
	[PublicizedFrom(EAccessModifier.Private)]
	public void setFlashlightOn(bool value)
	{
		if (value)
		{
			this.lightManager.InitMaterials("junkDroneLamp");
			return;
		}
		this.lightManager.DisableMaterials("junkDroneLamp");
	}

	// Token: 0x0600212A RID: 8490 RVA: 0x000D1BE9 File Offset: 0x000CFDE9
	[PublicizedFrom(EAccessModifier.Private)]
	public Handle PlaySoundLoop(string sound_path, float _vol = 1f)
	{
		return Manager.Play(this, sound_path, _vol, true);
	}

	// Token: 0x0600212B RID: 8491 RVA: 0x000D1BF4 File Offset: 0x000CFDF4
	public void PlaySound(string sound_path, float _vol = 1f)
	{
		this.PlaySound(this, sound_path, false, false, _vol);
	}

	// Token: 0x0600212C RID: 8492 RVA: 0x000D1C01 File Offset: 0x000CFE01
	public void PlayVO(string sound_path, bool _hasPriority = false, float _vol = 1f)
	{
		this.PlaySound(this, sound_path, true, _hasPriority, _vol);
	}

	// Token: 0x0600212D RID: 8493 RVA: 0x000D1C10 File Offset: 0x000CFE10
	public void PlaySound(Entity entity, string sound_path, bool _isVO = false, bool _hasPriority = false, float _vol = 1f)
	{
		if (!this.isQuietMode)
		{
			if (_isVO)
			{
				if (_hasPriority)
				{
					Handle handle = this.voHandle;
					if (handle != null)
					{
						handle.Stop(this.entityId);
					}
				}
				this.voHandle = Manager.Play(entity, sound_path, _vol, true);
			}
			else
			{
				Manager.Play(entity, sound_path, _vol, false);
			}
			this.notifySoundNoise(_vol * 2.5f, 5f);
		}
	}

	// Token: 0x0600212E RID: 8494 RVA: 0x000D1C74 File Offset: 0x000CFE74
	[PublicizedFrom(EAccessModifier.Private)]
	public void notifySoundNoise(float vol, float duration)
	{
		EntityPlayer entityPlayer = this.Owner as EntityPlayer;
		if (entityPlayer)
		{
			entityPlayer.Stealth.NotifyNoise(vol, duration);
		}
	}

	// Token: 0x0600212F RID: 8495 RVA: 0x000D1CA4 File Offset: 0x000CFEA4
	public void NotifySyncOwner()
	{
		PersistentPlayerData playerData = GameManager.Instance.GetPersistentPlayerList().GetPlayerData(this.OwnerID);
		if (playerData != null)
		{
			this.belongsPlayerId = playerData.EntityId;
			this.Owner = (GameManager.Instance.World.GetEntity(this.belongsPlayerId) as EntityAlive);
		}
		if (this.Owner)
		{
			this.currentTarget = this.Owner;
			this.rotation = Quaternion.LookRotation(this.Owner.position - this.position).eulerAngles;
			if (GameManager.Instance.World.IsLocalPlayer(this.belongsPlayerId))
			{
				this.HandleNavObject();
				this.hasNavObjectsEnabled = true;
				this.SetOwner(this.OwnerID);
				this.SendSyncData(3);
			}
		}
	}

	// Token: 0x06002130 RID: 8496 RVA: 0x000D1D70 File Offset: 0x000CFF70
	[PublicizedFrom(EAccessModifier.Private)]
	public void SyncOwnerData()
	{
		if (this.isOwnerSyncPending)
		{
			this.NotifySyncOwner();
			this.isOwnerSyncPending = false;
		}
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
			if (this.Owner)
			{
				this.Owner.AddOwnedEntity(this);
				this.currentTarget = this.Owner;
				if (!this.hasNavObjectsEnabled && GameManager.Instance.World.IsLocalPlayer(this.belongsPlayerId))
				{
					this.HandleNavObject();
					this.hasNavObjectsEnabled = true;
				}
			}
		}
	}

	// Token: 0x06002131 RID: 8497 RVA: 0x000D1E43 File Offset: 0x000D0043
	[PublicizedFrom(EAccessModifier.Private)]
	public bool belongsToPlayerId(int id)
	{
		return this.belongsPlayerId == id;
	}

	// Token: 0x06002132 RID: 8498 RVA: 0x000D1E50 File Offset: 0x000D0050
	public bool isAlly(EntityAlive _target)
	{
		if (this.debugFriendlyFire)
		{
			return false;
		}
		if (this.Owner && this.Owner == _target)
		{
			return true;
		}
		PersistentPlayerList persistentPlayerList = GameManager.Instance.GetPersistentPlayerList();
		PersistentPlayerData playerData = persistentPlayerList.GetPlayerData(this.OwnerID);
		if (playerData != null && persistentPlayerList.EntityToPlayerMap.ContainsKey(_target.entityId))
		{
			PersistentPlayerData persistentPlayerData = persistentPlayerList.EntityToPlayerMap[_target.entityId];
			if (playerData.ACL != null && persistentPlayerData != null && playerData.ACL.Contains(persistentPlayerData.PrimaryId))
			{
				return true;
			}
			EntityPlayer entityPlayer = this.Owner as EntityPlayer;
			EntityPlayer entityPlayer2 = _target as EntityPlayer;
			if (entityPlayer && entityPlayer2 && entityPlayer.Party != null && entityPlayer.Party.ContainsMember(entityPlayer2))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002133 RID: 8499 RVA: 0x000D1F28 File Offset: 0x000D0128
	public void BuffAllies()
	{
		EntityPlayer entityPlayer = this.Owner as EntityPlayer;
		if (entityPlayer)
		{
			if (entityPlayer.Party != null)
			{
				this.knownPartyMembers = entityPlayer.Party.GetMemberIdArray();
				for (int i = 0; i < this.knownPartyMembers.Length; i++)
				{
					EntityAlive entity = this.world.GetEntity(this.knownPartyMembers[i]) as EntityAlive;
					this.ProcBuffRange(entity);
				}
				return;
			}
			if (this.knownPartyMembers != null && this.knownPartyMembers.Length != 0)
			{
				for (int j = 0; j < this.knownPartyMembers.Length; j++)
				{
					EntityAlive entity2 = this.world.GetEntity(this.knownPartyMembers[j]) as EntityAlive;
					this.removeSupportBuff(entity2);
				}
				this.knownPartyMembers = null;
			}
			this.ProcBuffRange(entityPlayer);
		}
	}

	// Token: 0x06002134 RID: 8500 RVA: 0x000D1FED File Offset: 0x000D01ED
	[PublicizedFrom(EAccessModifier.Private)]
	public void registerPartyEvents(EntityPlayer player, bool value)
	{
		if (value)
		{
			player.Party.PartyMemberRemoved += this.OnPartyMemberRemoved;
		}
		else
		{
			player.Party.PartyMemberRemoved -= this.OnPartyMemberRemoved;
		}
		this.partyEventsSet = value;
	}

	// Token: 0x06002135 RID: 8501 RVA: 0x000D2029 File Offset: 0x000D0229
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnPartyMemberRemoved(EntityPlayer player)
	{
		this.removeSupportBuff(player);
	}

	// Token: 0x06002136 RID: 8502 RVA: 0x000D2034 File Offset: 0x000D0234
	[PublicizedFrom(EAccessModifier.Private)]
	public void RemovePartyBuffs(EntityPlayer owner)
	{
		if (owner.Party != null)
		{
			for (int i = 0; i < owner.Party.MemberList.Count; i++)
			{
				EntityAlive entity = owner.Party.MemberList[i];
				this.removeSupportBuff(entity);
			}
		}
	}

	// Token: 0x06002137 RID: 8503 RVA: 0x000D2080 File Offset: 0x000D0280
	[PublicizedFrom(EAccessModifier.Private)]
	public void ProcBuffRange(EntityAlive entity)
	{
		if (entity)
		{
			if ((this.position - entity.position).magnitude < 32f)
			{
				this.addSupportBuff(entity);
				return;
			}
			this.removeSupportBuff(entity);
		}
	}

	// Token: 0x06002138 RID: 8504 RVA: 0x000D20C4 File Offset: 0x000D02C4
	[PublicizedFrom(EAccessModifier.Private)]
	public void addSupportBuff(EntityAlive entity)
	{
		if (this.state != EntityDrone.State.Shutdown && !entity.Buffs.HasBuff("buffJunkDroneSupportEffect"))
		{
			entity.Buffs.AddBuff("buffJunkDroneSupportEffect", -1, true, false, -1f);
		}
	}

	// Token: 0x06002139 RID: 8505 RVA: 0x000D20FA File Offset: 0x000D02FA
	[PublicizedFrom(EAccessModifier.Private)]
	public void removeSupportBuff(EntityAlive entity)
	{
		if (entity && entity.Buffs.HasBuff("buffJunkDroneSupportEffect") && !this.doesEntityHaveSupport(entity))
		{
			entity.Buffs.RemoveBuff("buffJunkDroneSupportEffect", true);
		}
	}

	// Token: 0x0600213A RID: 8506 RVA: 0x000D2130 File Offset: 0x000D0330
	[PublicizedFrom(EAccessModifier.Private)]
	public bool doesEntityHaveSupport(EntityAlive entity)
	{
		OwnedEntityData[] ownedEntities = entity.GetOwnedEntities();
		for (int i = 0; i < ownedEntities.Length; i++)
		{
			EntityDrone entityDrone = this.world.GetEntity(ownedEntities[i].Id) as EntityDrone;
			if (entityDrone && entityDrone.isSupportModAttached)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600213B RID: 8507 RVA: 0x000D2180 File Offset: 0x000D0380
	[PublicizedFrom(EAccessModifier.Private)]
	public void doKeypadAction(LocalPlayerUI playerUI)
	{
		this.PlaySound("misc/password_type", 1f);
		GUIWindow window = playerUI.windowManager.GetWindow(XUiC_KeypadWindow.ID);
		window.OnWindowClose = (Action)Delegate.Combine(window.OnWindowClose, new Action(this.StopUIInsteractionSecurity));
		XUiC_KeypadWindow.Open(playerUI, this);
	}

	// Token: 0x0600213C RID: 8508 RVA: 0x000D21D8 File Offset: 0x000D03D8
	[PublicizedFrom(EAccessModifier.Private)]
	public void processRequest(EntityPlayer entityPlayer, int requestType)
	{
		if (requestType >= 0)
		{
			this.interactionRequestType = requestType;
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				this.ValidateInteractingPlayer();
				int entityId = this.interactingPlayerId;
				if (entityId == -1)
				{
					entityId = entityPlayer.entityId;
				}
				this.StartInteraction(entityPlayer.entityId, entityId);
				return;
			}
			this.interactingPlayerId = entityPlayer.entityId;
			this.SendSyncData(4096);
			this.interactingPlayerId = -1;
		}
	}

	// Token: 0x0600213D RID: 8509 RVA: 0x000D2240 File Offset: 0x000D0440
	public void startDialog(Entity _entityFocusing)
	{
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(_entityFocusing as EntityPlayerLocal);
		uiforPlayer.xui.Dialog.Respondent = this;
		uiforPlayer.windowManager.CloseAllOpenWindows(null, false);
		uiforPlayer.windowManager.Open("dialog", true, false, true);
		this.PlayVO("drone_greeting", false, 1f);
	}

	// Token: 0x0600213E RID: 8510 RVA: 0x000D229C File Offset: 0x000D049C
	public bool HasStoredItem(EntityAlive entity, string itemGroupOrName, FastTags<TagGroup.Global> fastTags)
	{
		ItemValue item = ItemClass.GetItem(itemGroupOrName, false);
		bool itemClass = item.ItemClass != null;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		if (itemClass)
		{
			num = entity.bag.GetItemCount(item, -1, -1, true);
			num2 = entity.inventory.GetItemCount(item, false, -1, -1, true);
			num3 = ((entity.lootContainer != null && entity.lootContainer.HasItem(item)) ? 1 : 0);
		}
		return num + num2 + num3 > 0;
	}

	// Token: 0x0600213F RID: 8511 RVA: 0x000D2304 File Offset: 0x000D0504
	public ItemStack TakeStoredItem(EntityAlive entity, string itemGroupOrName, FastTags<TagGroup.Global> fastTags)
	{
		ItemValue item = ItemClass.GetItem(itemGroupOrName, false);
		if (item.ItemClass != null)
		{
			entity.bag.GetItemCount(item, -1, -1, true);
			int itemCount = entity.inventory.GetItemCount(item, false, -1, -1, true);
			if (entity.lootContainer != null)
			{
				entity.lootContainer.HasItem(item);
			}
			if (itemCount > 0)
			{
				entity.inventory.DecItem(item, 1, false, null);
			}
			else if (entity.lootContainer != null)
			{
				this.takeFromEntityContainer(entity, itemGroupOrName, fastTags);
			}
			else
			{
				entity.bag.DecItem(item, 1, false, null);
			}
			return new ItemStack(item.Clone(), 1);
		}
		return null;
	}

	// Token: 0x06002140 RID: 8512 RVA: 0x000D239C File Offset: 0x000D059C
	[PublicizedFrom(EAccessModifier.Private)]
	public void takeFromEntityContainer(EntityAlive entity, string itemGroupOrName, FastTags<TagGroup.Global> fastTags)
	{
		ItemStack[] array = entity.bag.GetSlots();
		if (entity.lootContainer != null)
		{
			array = entity.lootContainer.GetItems();
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null && array[i].itemValue != null && array[i].itemValue.ItemClass != null && array[i].itemValue.ItemClass.HasAnyTags(fastTags) && array[i].count > 0 && array[i].itemValue.ItemClass.Name.ContainsCaseInsensitive(itemGroupOrName))
			{
				array[i].count--;
				if (array[i].count == 0)
				{
					array[i] = ItemStack.Empty.Clone();
				}
				entity.bag.SetSlots(array);
				entity.bag.OnUpdate();
				if (entity.lootContainer != null)
				{
					entity.lootContainer.UpdateSlot(i, array[i]);
				}
			}
		}
	}

	// Token: 0x06002141 RID: 8513 RVA: 0x000D2493 File Offset: 0x000D0693
	public void OpenStorage(Entity _entityFocusing)
	{
		this.processRequest(_entityFocusing as EntityPlayerLocal, 10);
	}

	// Token: 0x06002142 RID: 8514 RVA: 0x000D24A3 File Offset: 0x000D06A3
	public ItemValue GetUpdatedItemValue()
	{
		this.OriginalItemValue.UseTimes = (float)this.OriginalItemValue.MaxUseTimes * (1f - (float)this.Health / base.Stats.Health.BaseMax);
		return this.OriginalItemValue;
	}

	// Token: 0x06002143 RID: 8515 RVA: 0x000D24E4 File Offset: 0x000D06E4
	public void Collect(int _playerId)
	{
		EntityPlayerLocal entityPlayerLocal = this.world.GetEntity(_playerId) as EntityPlayerLocal;
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(entityPlayerLocal);
		ItemStack itemStack = new ItemStack(this.GetUpdatedItemValue(), 1);
		if (!uiforPlayer.xui.PlayerInventory.Toolbelt.AddItem(itemStack) && !uiforPlayer.xui.PlayerInventory.AddItem(itemStack))
		{
			GameManager.Instance.ItemDropServer(itemStack, entityPlayerLocal.GetPosition(), Vector3.zero, _playerId, 60f, false);
		}
		this.OriginalItemValue = this.GetUpdatedItemValue();
		base.transform.gameObject.SetActive(false);
		if (this.Owner)
		{
			this.Owner.RemoveOwnedEntity(this.entityId);
			if (DroneManager.Instance != null)
			{
				DroneManager.Instance.RemoveTrackedDrone(this, EnumRemoveEntityReason.Despawned);
			}
		}
	}

	// Token: 0x06002144 RID: 8516 RVA: 0x000D25B0 File Offset: 0x000D07B0
	[PublicizedFrom(EAccessModifier.Private)]
	public void accessInventory(Entity _entityFocusing)
	{
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(_entityFocusing as EntityPlayerLocal);
		uiforPlayer.xui.Dialog.Respondent = this;
		uiforPlayer.windowManager.CloseAllOpenWindows(null, false);
		string lootContainerName = Localization.Get(EntityClass.list[this.entityClass].entityClassName, false);
		GUIWindow window = uiforPlayer.windowManager.GetWindow("looting");
		((XUiC_LootWindowGroup)((XUiWindowGroup)window).Controller).SetTileEntityChest(lootContainerName, this.lootContainer);
		window.OnWindowClose = (Action)Delegate.Combine(window.OnWindowClose, new Action(this.StopUIInteraction));
		uiforPlayer.windowManager.Open("looting", true, false, true);
	}

	// Token: 0x06002145 RID: 8517 RVA: 0x000D2664 File Offset: 0x000D0864
	[PublicizedFrom(EAccessModifier.Private)]
	public void pickup(Entity _entityFocusing)
	{
		if (!this.lootContainer.IsEmpty())
		{
			this.PlayVO("drone_takefail", true, 1f);
			GameManager.ShowTooltip(this.Owner as EntityPlayerLocal, Localization.Get("ttEmptyVehicleBeforePickup", false), string.Empty, "ui_denied", null, false, false, 0f);
			return;
		}
		ItemStack itemStack = new ItemStack(this.GetUpdatedItemValue(), 1);
		EntityPlayer entityPlayer = _entityFocusing as EntityPlayer;
		if (entityPlayer.inventory.CanTakeItem(itemStack) || entityPlayer.bag.CanTakeItem(itemStack))
		{
			this.isBeingPickedUp = true;
			this.PlaySound(entityPlayer, "drone_take", true, true, 1f);
			this.initWorldValues(false);
			this.nativeCollider.enabled = false;
			GameManager.Instance.CollectEntityServer(this.entityId, entityPlayer.entityId);
			if (entityPlayer.Buffs.HasBuff("buffJunkDroneSupportEffect"))
			{
				entityPlayer.Buffs.RemoveBuff("buffJunkDroneSupportEffect", true);
			}
			this.RemovePartyBuffs(entityPlayer);
			if (entityPlayer.Party != null)
			{
				this.registerPartyEvents(entityPlayer, false);
			}
			this.UnRegsiterMovingLights();
			return;
		}
		GameManager.ShowTooltip(entityPlayer as EntityPlayerLocal, Localization.Get("xuiInventoryFullForPickup", false), string.Empty, "ui_denied", null, false, false, 0f);
	}

	// Token: 0x170003A7 RID: 935
	// (get) Token: 0x06002146 RID: 8518 RVA: 0x000D279C File Offset: 0x000D099C
	public int StorageCapacity
	{
		get
		{
			return this.lootContainer.items.Length;
		}
	}

	// Token: 0x06002147 RID: 8519 RVA: 0x000D27AC File Offset: 0x000D09AC
	public int GetStoredItemCount()
	{
		int num = 0;
		for (int i = 0; i < this.lootContainer.items.Length; i++)
		{
			if (!this.lootContainer.items[i].IsEmpty())
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06002148 RID: 8520 RVA: 0x000D27EC File Offset: 0x000D09EC
	public bool CanRemoveExtraStorage()
	{
		return this.GetStoredItemCount() < this.StorageCapacity - 8;
	}

	// Token: 0x06002149 RID: 8521 RVA: 0x000D2804 File Offset: 0x000D0A04
	public void NotifyToManyStoredItems()
	{
		if (this.overItemLimitCooldown > 0f)
		{
			return;
		}
		this.overItemLimitCooldown = 5f;
		if (!this.CanRemoveExtraStorage())
		{
			GameManager.ShowTooltip(this.Owner as EntityPlayerLocal, Localization.Get("ttJunkDroneEmptySomeStorage", false), string.Empty, "ui_denied", null, false, false, 0f);
			return;
		}
	}

	// Token: 0x0600214A RID: 8522 RVA: 0x000D2860 File Offset: 0x000D0A60
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateDroneServiceMenu()
	{
		if (this.overItemLimitCooldown > 0f)
		{
			this.overItemLimitCooldown -= 0.05f;
		}
	}

	// Token: 0x0600214B RID: 8523 RVA: 0x000D2881 File Offset: 0x000D0A81
	[PublicizedFrom(EAccessModifier.Private)]
	public void move(Vector3 dir)
	{
		this.move(dir, this.currentSpeedFlying);
	}

	// Token: 0x0600214C RID: 8524 RVA: 0x000D2890 File Offset: 0x000D0A90
	[PublicizedFrom(EAccessModifier.Private)]
	public void move(Vector3 dir, float speedFlying)
	{
		Vector3 end = this.position + dir.normalized * this.physColHeight;
		if (!RaycastPathUtils.IsPositionBlocked(this.position, end, 1073807360, false))
		{
			this.motion += dir * speedFlying * 0.05f;
		}
	}

	// Token: 0x0600214D RID: 8525 RVA: 0x000D28F4 File Offset: 0x000D0AF4
	[PublicizedFrom(EAccessModifier.Private)]
	public void moveAlongPath(Vector3 dir)
	{
		this.position + dir.normalized * this.physColHeight;
		this.motion += dir * this.SpeedFlying * 0.05f;
	}

	// Token: 0x0600214E RID: 8526 RVA: 0x000D2948 File Offset: 0x000D0B48
	[PublicizedFrom(EAccessModifier.Private)]
	public bool canMove(Vector3 dir)
	{
		Vector3 end = this.position + dir.normalized * this.physColHeight;
		return !RaycastPathUtils.IsPositionBlocked(this.position, end, 1073807360, false);
	}

	// Token: 0x0600214F RID: 8527 RVA: 0x000D2988 File Offset: 0x000D0B88
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 rotateToDir(Vector3 dir)
	{
		return Quaternion.Lerp(base.transform.rotation, Quaternion.LookRotation(dir), (1f - Vector3.Angle(base.transform.forward, dir) / 180f) * this.RotationSpeed * 0.05f).eulerAngles;
	}

	// Token: 0x06002150 RID: 8528 RVA: 0x000D29E0 File Offset: 0x000D0BE0
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 rotateToEuler(Vector3 rot)
	{
		return Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(rot), (1f - Vector3.Angle(base.transform.forward, (rot - base.transform.eulerAngles).normalized) / 180f) * this.RotationSpeed * 0.05f).eulerAngles;
	}

	// Token: 0x06002151 RID: 8529 RVA: 0x000D2A4D File Offset: 0x000D0C4D
	[PublicizedFrom(EAccessModifier.Private)]
	public void rotateTo(Vector3 dir)
	{
		if (dir != Vector3.zero)
		{
			this.rotation = this.rotateToDir(dir);
		}
	}

	// Token: 0x06002152 RID: 8530 RVA: 0x000D2A69 File Offset: 0x000D0C69
	public int GetRepairAmountNeeded()
	{
		return this.GetMaxHealth() - this.Health;
	}

	// Token: 0x06002153 RID: 8531 RVA: 0x000D2A78 File Offset: 0x000D0C78
	public void RepairParts(int _amount)
	{
		this.Health += _amount;
	}

	// Token: 0x06002154 RID: 8532 RVA: 0x000D2A88 File Offset: 0x000D0C88
	[PublicizedFrom(EAccessModifier.Private)]
	public void doRepairAction(EntityPlayer entityPlayer, LocalPlayerUI playerUI)
	{
		string text = "resourceRepairKit";
		if (this.HasStoredItem(entityPlayer, text, EntityDrone.repairKitTags))
		{
			playerUI.xui.CollectedItemList.RemoveItemStack(new ItemStack(ItemClass.GetItem(text, false), 1));
			this.PlaySound("crafting/craft_repair_item", 1f);
			this.TakeStoredItem(entityPlayer, text, EntityDrone.repairKitTags);
			this.performRepair();
			this.SendSyncData(16);
			return;
		}
		Manager.PlayInsidePlayerHead("misc/missingitemtorepair", -1, 0f, false, false);
	}

	// Token: 0x06002155 RID: 8533 RVA: 0x000D2B08 File Offset: 0x000D0D08
	[PublicizedFrom(EAccessModifier.Private)]
	public void performRepair()
	{
		this.Health = (int)base.Stats.Health.Max;
		this.OriginalItemValue.UseTimes = 0f;
		this.setShutdown(false);
		this.PlayWakeupAnim = true;
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.SendSyncData(16);
		}
	}

	// Token: 0x170003A8 RID: 936
	// (get) Token: 0x06002156 RID: 8534 RVA: 0x000D2B5E File Offset: 0x000D0D5E
	public Vector3 HealArmPosition
	{
		get
		{
			return this.healWeapon.WeaponJoint.position + Origin.position;
		}
	}

	// Token: 0x06002157 RID: 8535 RVA: 0x000D2B7A File Offset: 0x000D0D7A
	public bool TargetCanBeHealed(EntityAlive entity)
	{
		return this.isHealModAttached && this.healWeapon.targetCanBeHealed(entity) && this.HasHealingItem();
	}

	// Token: 0x06002158 RID: 8536 RVA: 0x000D2B9A File Offset: 0x000D0D9A
	public bool HasHealingItem()
	{
		return this.healWeapon.hasHealingItem();
	}

	// Token: 0x170003A9 RID: 937
	// (get) Token: 0x06002159 RID: 8537 RVA: 0x000D2BA7 File Offset: 0x000D0DA7
	public EntityDrone.AllyHealMode HealAllyMode
	{
		get
		{
			return this.allyHealMode;
		}
	}

	// Token: 0x170003AA RID: 938
	// (get) Token: 0x0600215A RID: 8538 RVA: 0x000D2BAF File Offset: 0x000D0DAF
	// (set) Token: 0x0600215B RID: 8539 RVA: 0x000D2BB7 File Offset: 0x000D0DB7
	public bool IsHealingAllies { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x0600215C RID: 8540 RVA: 0x000D2BC0 File Offset: 0x000D0DC0
	public void ToggleHealAllies()
	{
		this.IsHealingAllies = !this.IsHealingAllies;
		this.allyHealMode = (this.IsHealingAllies ? EntityDrone.AllyHealMode.HealAllies : EntityDrone.AllyHealMode.DoNotHeal);
	}

	// Token: 0x170003AB RID: 939
	// (get) Token: 0x0600215D RID: 8541 RVA: 0x000D2BE3 File Offset: 0x000D0DE3
	public bool IsHealModAttached
	{
		get
		{
			return this.isHealModAttached;
		}
	}

	// Token: 0x0600215E RID: 8542 RVA: 0x000D2BEC File Offset: 0x000D0DEC
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateNeedsHealItemCheck()
	{
		if (this.needsHealItemTimer > 0f)
		{
			this.needsHealItemTimer -= 0.05f;
		}
		if (this.needsHealItemTimer <= 0f && this.needsHealNotifyCount < 2 && this.checkNotifityNeedsHealItem())
		{
			this.needsHealItemTimer = 30f;
			this.needsHealNotifyCount++;
		}
	}

	// Token: 0x0600215F RID: 8543 RVA: 0x000D2C4F File Offset: 0x000D0E4F
	[PublicizedFrom(EAccessModifier.Private)]
	public void ClearNeedsHealItemCheck()
	{
		this.needsHealItemTimer = 0f;
		this.needsHealNotifyCount = 0;
	}

	// Token: 0x06002160 RID: 8544 RVA: 0x000D2C64 File Offset: 0x000D0E64
	[PublicizedFrom(EAccessModifier.Private)]
	public bool checkNotifityNeedsHealItem()
	{
		if (!this.healWeapon.hasHealingItem())
		{
			GameManager.ShowTooltip(this.Owner as EntityPlayerLocal, Localization.Get("xuiDroneNeedsHealItemsStored", false), string.Empty, "ui_denied", null, false, false, 0f);
			this.PlaySound("drone_empty", 1f);
			return true;
		}
		return false;
	}

	// Token: 0x06002161 RID: 8545 RVA: 0x000D2CBE File Offset: 0x000D0EBE
	public void Heal()
	{
		this.UpdateNeedsHealItemCheck();
		if (!this.IsHealingAllies)
		{
			if (this.state != EntityDrone.State.Heal && this.healWeapon.canFire() && this.healWeapon.targetNeedsHealing(this.Owner))
			{
				this.HealOwnerServer();
			}
			return;
		}
	}

	// Token: 0x06002162 RID: 8546 RVA: 0x000D2D00 File Offset: 0x000D0F00
	public void HealOwner()
	{
		if (this.state != EntityDrone.State.Heal && this.healWeapon.canFire())
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				this.currentTarget = this.Owner;
				this.healTarget(this.Owner);
				return;
			}
			this.setState(EntityDrone.State.Heal);
			this.SendSyncData(32768);
			this.setState(EntityDrone.State.Idle);
		}
	}

	// Token: 0x06002163 RID: 8547 RVA: 0x000D2D61 File Offset: 0x000D0F61
	[PublicizedFrom(EAccessModifier.Private)]
	public void HealOwnerServer()
	{
		if (this.state != EntityDrone.State.Heal && this.healWeapon.canFire() && SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.currentTarget = this.Owner;
			this.healTarget(this.Owner);
		}
	}

	// Token: 0x06002164 RID: 8548 RVA: 0x000D2D9D File Offset: 0x000D0F9D
	[PublicizedFrom(EAccessModifier.Private)]
	public void healTarget(EntityAlive target)
	{
		this.PlayVO("drone_heal", true, 1f);
		base.SetAttackTarget(target, 1200);
		if (this.attackTarget)
		{
			this.setState(EntityDrone.State.Heal);
			this.SendSyncData(32768);
		}
	}

	// Token: 0x06002165 RID: 8549 RVA: 0x000D2DDB File Offset: 0x000D0FDB
	[PublicizedFrom(EAccessModifier.Private)]
	public void onHealDone()
	{
		base.SetRevengeTarget(null);
		base.SetAttackTarget(null, 0);
		if (this.Owner)
		{
			this.Owner.Buffs.RemoveBuff("buffJunkDroneHealCooldownEffect", true);
		}
	}

	// Token: 0x06002166 RID: 8550 RVA: 0x000D2E0F File Offset: 0x000D100F
	public EntityDrone.State GetState()
	{
		return this.state;
	}

	// Token: 0x06002167 RID: 8551 RVA: 0x000D2E17 File Offset: 0x000D1017
	public void FollowMode()
	{
		this.PlayVO("drone_command", true, 1f);
		this.setOrders(EntityDrone.Orders.Follow);
		this.setState(EntityDrone.State.Follow);
		this.SendSyncData(49152);
	}

	// Token: 0x06002168 RID: 8552 RVA: 0x000D2E44 File Offset: 0x000D1044
	public void SentryMode()
	{
		this.PlayVO("drone_command", true, 1f);
		this.sentryPos = this.position;
		this.setOrders(EntityDrone.Orders.Stay);
		this.setState(EntityDrone.State.Sentry);
		this.SendSyncData(49152);
		if (this.Owner && this.Owner.HasOwnedEntity(this.entityId))
		{
			this.Owner.GetOwnedEntity(this.entityId).SetLastKnownPosition(this.position);
		}
	}

	// Token: 0x06002169 RID: 8553 RVA: 0x000D2EC4 File Offset: 0x000D10C4
	[PublicizedFrom(EAccessModifier.Private)]
	public void setState(EntityDrone.State next)
	{
		this.logDrone(string.Format("State: {0} > {1}", this.state, next));
		this.lastState = this.state;
		this.state = next;
		this.stateTime = 0f;
		if (this.lastState == EntityDrone.State.Shutdown)
		{
			Animator componentInChildren = base.GetComponentInChildren<Animator>();
			if (!componentInChildren.enabled)
			{
				componentInChildren.enabled = true;
			}
		}
		switch (this.state)
		{
		case EntityDrone.State.Idle:
		case EntityDrone.State.Sentry:
			break;
		case EntityDrone.State.Follow:
			if (this.lastState == EntityDrone.State.Sentry && this.Owner && this.Owner.HasOwnedEntity(this.entityId))
			{
				this.Owner.GetOwnedEntity(this.entityId).ClearLastKnownPostition();
				return;
			}
			break;
		case EntityDrone.State.Heal:
			this.ClearNeedsHealItemCheck();
			break;
		default:
			return;
		}
	}

	// Token: 0x0600216A RID: 8554 RVA: 0x000D2F94 File Offset: 0x000D1194
	[PublicizedFrom(EAccessModifier.Private)]
	public void idleState()
	{
		if (this.currentTarget && !this.isEntityAboveOrBelow(this.currentTarget))
		{
			Vector3 headPosition = this.currentTarget.getHeadPosition();
			this.rotateTo(this.steering.GetDir2D(this.position, headPosition));
			float num = 0f;
			if (this.position.y - this.currentTarget.getHeadPosition().y > num || this.position.y - this.currentTarget.getHeadPosition().y < num)
			{
				Vector3 position = this.position;
				position.y = this.currentTarget.getHeadPosition().y;
				this.move(this.steering.Seek(this.position, position, this.SpeedFlying));
			}
		}
	}

	// Token: 0x0600216B RID: 8555 RVA: 0x000D3068 File Offset: 0x000D1268
	[PublicizedFrom(EAccessModifier.Private)]
	public void sentryState()
	{
		Vector3 vector = this.sentryPos;
		if (this.world.IsChunkAreaLoaded(vector) && !this.steering.IsInRange(vector, 0.25f))
		{
			Vector3 dir = this.steering.Seek(this.position, vector, 0.25f);
			this.move(dir);
			return;
		}
	}

	// Token: 0x0600216C RID: 8556 RVA: 0x000D30C0 File Offset: 0x000D12C0
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 findOpenFollowPoints(bool debugDraw = false)
	{
		this.currentTarget.GetLookVector().y = 0f;
		Vector3[] array = this.getGroupPositions(this.currentTarget, this.FollowDistance + 1f, false);
		Array.Sort<Vector3>(array, (Vector3 x, Vector3 y) => Vector3.Distance(this.position, x).CompareTo(Vector3.Distance(this.position, y)));
		this.hasOpenGroupPos = false;
		Vector3 vector = this.currentTarget.getHeadPosition();
		for (int i = 0; i < array.Length; i++)
		{
			Vector3i vector3i = World.worldToBlockPos(array[i]);
			if (!RaycastPathUtils.IsPointBlocked(array[i], this.currentTarget.getHeadPosition(), 1073807360, true, 0f) && !RaycastPathUtils.IsPointBlocked(this.currentTarget.getHeadPosition(), array[i], 1073807360, true, 0f))
			{
				RaycastNode raycastNode = RaycastPathWorldUtils.FindNodeType(RaycastPathWorldUtils.ScanVolume(this.world, vector3i.ToVector3Center(), true, false, false, 0f), cPathNodeType.Air);
				if (raycastNode != null)
				{
					if (!this.hasOpenGroupPos)
					{
						vector = raycastNode.Center;
						this.hasOpenGroupPos = true;
					}
					if (debugDraw)
					{
						RaycastPathUtils.DrawNode(new RaycastNode(vector3i.ToVector3Center(), 1f, 0), Color.yellow, 0f);
					}
				}
			}
			else if (debugDraw)
			{
				RaycastPathUtils.DrawNode(new RaycastNode(vector3i.ToVector3Center(), 1f, 0), Color.red, 0f);
			}
		}
		Vector3[] groupFallbackPositions = this.getGroupFallbackPositions(this.currentTarget, this.FollowDistance + 1f, false);
		Array.Sort<Vector3>(groupFallbackPositions, (Vector3 x, Vector3 y) => Vector3.Distance(this.position, x).CompareTo(Vector3.Distance(this.position, y)));
		for (int j = 0; j < groupFallbackPositions.Length; j++)
		{
			Vector3i vector3i2 = World.worldToBlockPos(groupFallbackPositions[j]);
			if (!RaycastPathUtils.IsPointBlocked(groupFallbackPositions[j], this.currentTarget.getHeadPosition(), 1073807360, true, 0f) && !RaycastPathUtils.IsPointBlocked(this.currentTarget.getHeadPosition(), groupFallbackPositions[j], 1073807360, true, 0f))
			{
				RaycastNode raycastNode2 = RaycastPathWorldUtils.FindNodeType(RaycastPathWorldUtils.ScanVolume(this.world, vector3i2.ToVector3Center(), true, false, false, 0f), cPathNodeType.Air);
				if (raycastNode2 != null)
				{
					if (!this.hasOpenGroupPos)
					{
						vector = raycastNode2.Center;
						this.hasOpenGroupPos = true;
					}
					if (debugDraw)
					{
						RaycastPathUtils.DrawNode(new RaycastNode(vector3i2.ToVector3Center(), 1f, 0), Color.yellow, 0f);
					}
				}
			}
			else if (debugDraw)
			{
				RaycastPathUtils.DrawNode(new RaycastNode(vector3i2.ToVector3Center(), 1f, 0), Color.red, 0f);
			}
		}
		if (!this.hasOpenGroupPos)
		{
			Vector3i vector3i3 = World.worldToBlockPos(this.currentTarget.getHeadPosition());
			RaycastNode raycastNode3 = RaycastPathWorldUtils.FindNodeType(RaycastPathWorldUtils.ScanVolume(this.world, vector3i3.ToVector3Center(), false, false, false, 0f), cPathNodeType.Air);
			if (raycastNode3 != null)
			{
				vector = raycastNode3.Center;
			}
		}
		this.followTargetPos = vector;
		if (debugDraw)
		{
			RaycastPathUtils.DrawBounds(World.worldToBlockPos(this.followTargetPos), Color.green, 0f, 1f);
		}
		return this.followTargetPos;
	}

	// Token: 0x0600216D RID: 8557 RVA: 0x000D33BC File Offset: 0x000D15BC
	[PublicizedFrom(EAccessModifier.Private)]
	public void makePath()
	{
		if (this.pathMan.Path == null)
		{
			this.pathMan.CreatePath(this.position + Vector3.up, this.currentTarget.getHeadPosition() - this.currentTarget.GetForwardVector() * 2f + Vector3.up, this.SpeedFlying, false, 0f);
		}
		if (this.pathMan.isBuildingPath)
		{
			return;
		}
		if (this.pathMan.Path != null && this.pathMan.Path.Nodes.Count > 0 && this.nodePath.Count == 0)
		{
			this.nodePath.AddRange(this.pathMan.Path.Nodes);
			this.nodePath.Reverse();
		}
		if (this.nodePath.Count > 0)
		{
			if ((this.pathMan.Path.Target - this.currentTarget.getHeadPosition()).magnitude > this.FollowDistance)
			{
				this.pathMan.Clear();
				this.nodePath.Clear();
				return;
			}
			Vector3 dir = this.steering.Seek(this.position, this.nodePath[0].Center, 0f);
			this.rotateTo(dir);
			this.move(dir, this.SpeedPathing);
			if (this.steering.IsInRange(this.nodePath[0].Center, 0.5f))
			{
				this.nodePath.RemoveAt(0);
				if (this.nodePath.Count == 0)
				{
					this.transitionToIdle = true;
				}
			}
		}
		if (this.transitionToIdle)
		{
			this.transitionToIdleTime -= 0.05f;
			if (this.transitionToIdleTime <= 0f)
			{
				this.transitionToIdleTime = 0.5f;
				this.pathMan.Clear();
				this.setState(EntityDrone.State.Idle);
				this.transitionToIdle = false;
			}
		}
	}

	// Token: 0x0600216E RID: 8558 RVA: 0x000D35B4 File Offset: 0x000D17B4
	[PublicizedFrom(EAccessModifier.Private)]
	public void clearCurrentPath()
	{
		this.currentPath.Clear();
		this.debugPathDelay = 2f;
	}

	// Token: 0x0600216F RID: 8559 RVA: 0x000D35CC File Offset: 0x000D17CC
	[PublicizedFrom(EAccessModifier.Private)]
	public void followState()
	{
		if (!this.currentTarget)
		{
			this.currentTarget = this.Owner;
		}
		if (this.isTargetUnderWater(this.currentTarget.getHeadPosition()))
		{
			if (this.currentPath.Count > 0)
			{
				this.clearCurrentPath();
			}
			Vector3 dir = this.steering.Seek(this.position, this.findOpenBlockAbove(this.currentTarget.getHeadPosition(), 256), 0.2f);
			this.rotateTo(dir);
			this.move(dir);
			return;
		}
		float magnitude = (this.currentTarget.getHeadPosition() - this.position).magnitude;
		this.findOpenFollowPoints(true);
		bool flag = !RaycastPathUtils.IsPointBlocked(this.position, this.followTargetPos, 1073807360, true, 0f);
		if (this.debugPathTiming && this.debugPathDelay > 0f)
		{
			this.debugPathDelay -= Time.deltaTime;
			return;
		}
		if (!flag && this.currentPath.Count == 0)
		{
			this.getPath(this.currentTarget.position);
			return;
		}
		if (this.currentPath.Count <= 0)
		{
			Vector3 a = this.steering.Seek(this.position, this.followTargetPos, this.SpeedFlying);
			if (!this.steering.IsInRange(this.followTargetPos, 0.1f))
			{
				if (magnitude > this.FollowDistance && magnitude < 24f && !RaycastPathUtils.IsPointBlocked(this.position, this.currentTarget.position, 1073807360, false, 0f) && Vector3.Angle(this.currentTarget.GetLookVector(), this.position - this.currentTarget.getHeadPosition()) < 45f)
				{
					float d = 0.5f;
					Vector3 vector = this.steering.Flee(this.position, this.currentTarget.getHeadPosition(), this.SpeedFlying);
					if (!RaycastPathUtils.IsPositionBlocked(this.position, this.position + (a + vector), 1073807360, false))
					{
						a += vector * d;
					}
				}
				if (this.steering.GetAltitude(this.position) < magnitude * 0.33f && !RaycastPathUtils.IsPositionBlocked(this.position, this.position + Vector3.up, 1073807360, false))
				{
					float d2 = 0.75f;
					Vector3 a2 = this.steering.Seek(this.position, this.position + Vector3.up, this.SpeedFlying);
					a += a2 * d2;
				}
				this.rotateTo((this.currentTarget.getHeadPosition() - this.position).normalized);
				this.move(a.normalized);
			}
			if (this.state == EntityDrone.State.Follow && this.steering.IsInRange(this.followTargetPos, 0.5f))
			{
				this.debugPathDelay = 2f;
				this.setState(EntityDrone.State.Idle);
			}
			return;
		}
		this.currentPathTarget = this.currentPath[0];
		Vector3 dir2 = this.steering.Seek(this.position, this.currentPathTarget, 1f);
		this.rotateTo((this.currentPathTarget - this.position).normalized);
		this.move(dir2);
		if (this.steering.IsInRange(this.currentPathTarget, 0.5f))
		{
			this.currentPath.RemoveAt(0);
			return;
		}
		if (this.currentPath.Count > 1)
		{
			RaycastPathUtils.DrawLine(this.currentPath[0], this.currentPath[1], Color.green, 1f);
		}
		if (!this.IsStuckInBlock() && !this.IsNotAbleToReachTarget(this.currentPath[0]))
		{
			if (!RaycastPathUtils.IsPointBlocked(this.position, this.followTargetPos, 1073807360, true, 0f))
			{
				this.clearCurrentPath();
			}
			return;
		}
		if (this.currentPath.Count > 1)
		{
			this.teleportToPosition(this.currentPath[1]);
			this.currentPath.RemoveRange(0, 2);
			return;
		}
		this.teleportToPosition(this.currentPath[0]);
		this.currentPath.RemoveAt(0);
	}

	// Token: 0x06002170 RID: 8560 RVA: 0x000D3A20 File Offset: 0x000D1C20
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void getPath(Vector3 target)
	{
		if (this.findPath(this.position, target, false))
		{
			this.clearCurrentPath();
			this.currentPath.AddRange(this.projectedPath);
			this.currentPath.RemoveAt(0);
			for (int i = 0; i < this.currentPath.Count; i++)
			{
				Vector3 value = this.currentPath[i];
				value.y += 1.5f;
				this.currentPath[i] = value;
			}
			List<RaycastNode> list = RaycastPathWorldUtils.ScanPath(this.world, this.position, this.currentPath, false, false, 0f);
			for (int j = 0; j < this.currentPath.Count; j++)
			{
				Vector3 a = this.currentPath[j];
				RaycastNode raycastNode = list[j];
				if (raycastNode.FlowToWaypoint)
				{
					this.currentPath[j] = (a + raycastNode.Waypoint.Center) * 0.5f;
				}
			}
			for (int k = 0; k < this.currentPath.Count - 1; k++)
			{
				Color endColor = Color.cyan;
				if (RaycastPathUtils.IsPointBlocked(this.currentPath[k], this.currentPath[k + 1], 1073807360, false, 0f))
				{
					endColor = Color.magenta;
				}
				Utils.DrawLine(this.currentPath[k] - Origin.position, this.currentPath[k + 1] - Origin.position, Color.white, endColor, 100, 10f);
			}
			this.projectedPath.Clear();
		}
	}

	// Token: 0x06002171 RID: 8561 RVA: 0x000D3BD0 File Offset: 0x000D1DD0
	[PublicizedFrom(EAccessModifier.Private)]
	public bool findPath(Vector3 start, Vector3 end, bool debugDraw = false)
	{
		PathFinderThread instance = PathFinderThread.Instance;
		if (instance == null)
		{
			return false;
		}
		PathInfo path = instance.GetPath(this.entityId);
		if (path.path == null && !PathFinderThread.Instance.IsCalculatingPath(this.entityId))
		{
			PathFinderThread.Instance.FindPath(this, start, end, this.SpeedFlying, false, null);
			return false;
		}
		if (path.path == null)
		{
			return false;
		}
		for (int i = 0; i < path.path.points.Length; i++)
		{
			Vector3 projectedLocation = path.path.points[i].projectedLocation;
			this.projectedPath.Add(projectedLocation);
		}
		if (debugDraw)
		{
			for (int j = 0; j < this.projectedPath.Count - 1; j++)
			{
				Utils.DrawLine(this.projectedPath[j] - Origin.position, this.projectedPath[j + 1] - Origin.position, Color.white, Color.cyan, 100, 10f);
			}
		}
		return true;
	}

	// Token: 0x06002172 RID: 8562 RVA: 0x000D3CD0 File Offset: 0x000D1ED0
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void healState()
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return;
		}
		if (!this.currentTarget)
		{
			this.onHealDone();
			return;
		}
		this.rotateTo(this.currentTarget.position - this.position);
		float magnitude = (this.position - this.currentTarget.getHeadPosition()).magnitude;
		if (magnitude > this.FollowDistance)
		{
			this.followState();
		}
		if (magnitude <= this.FollowDistance && this.healWeapon.canFire())
		{
			this.healWeapon.RegisterOnFireComplete(new Action(this.onHealDone));
			this.healWeapon.Fire(this.currentTarget);
			return;
		}
		if (this.healWeapon.hasActionCompleted())
		{
			this.setState(EntityDrone.State.Idle);
			this.SendSyncData(32768);
		}
	}

	// Token: 0x06002173 RID: 8563 RVA: 0x000D3DA4 File Offset: 0x000D1FA4
	public void DebugUnstuck()
	{
		this.teleportState();
	}

	// Token: 0x06002174 RID: 8564 RVA: 0x000D3DAC File Offset: 0x000D1FAC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void teleportState()
	{
		if (!this.isTeleporting)
		{
			Log.Out("Drone.teleportState() - {0}", new object[]
			{
				this.entityId
			});
			this.setState(EntityDrone.State.Teleport);
			Vector3 vector = this.Owner.getHeadPosition() - new Vector3(this.Owner.GetLookVector().x, 0f, this.Owner.GetLookVector().z) * this.FollowDistance;
			this.isTeleporting = true;
			this.clearCurrentPath();
			this.motion = Vector3.zero;
			this.SetPosition(vector, true);
			this.orderState = EntityDrone.Orders.Follow;
			base.StartCoroutine(this.validateTeleport(vector));
		}
	}

	// Token: 0x06002175 RID: 8565 RVA: 0x000D3E64 File Offset: 0x000D2064
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator validateTeleport(Vector3 target)
	{
		yield return new WaitForSeconds(1f);
		if (this.Owner)
		{
			if (this.isOutOfRange(this.Owner.position, this.MaxDistFromOwner))
			{
				Log.Out("teleport failed");
			}
			else if (!this.isOutOfRange(target, this.FollowDistance * 1.5f))
			{
				Log.Out("teleport success!");
			}
		}
		this.isTeleporting = false;
		this.setState(EntityDrone.State.Idle);
		yield return null;
		yield break;
	}

	// Token: 0x06002176 RID: 8566 RVA: 0x000D3E7A File Offset: 0x000D207A
	public override void SetPosition(Vector3 _pos, bool _bUpdatePhysics = true)
	{
		base.SetPosition(_pos, _bUpdatePhysics);
	}

	// Token: 0x06002177 RID: 8567 RVA: 0x000D3E84 File Offset: 0x000D2084
	[PublicizedFrom(EAccessModifier.Private)]
	public void teleportToPosition(Vector3 telePos)
	{
		this.motion = Vector3.zero;
		Utils.DrawLine(telePos, this.position, Color.yellow, Color.green, 100, 20f);
		this.SetPosition(telePos, true);
	}

	// Token: 0x06002178 RID: 8568 RVA: 0x000D3EB8 File Offset: 0x000D20B8
	[PublicizedFrom(EAccessModifier.Private)]
	public void performShutdown()
	{
		if (this.Owner)
		{
			Manager.Stop(this.Owner.entityId, "drone_take");
		}
		this.PlayVO("drone_shutdown", true, 1f);
		if (this.Owner && this.Owner.HasOwnedEntity(this.entityId))
		{
			this.Owner.GetOwnedEntity(this.entityId).SetLastKnownPosition(this.position);
		}
		this.setShutdown(true);
		this.isShutdownPending = false;
	}

	// Token: 0x06002179 RID: 8569 RVA: 0x000D3F44 File Offset: 0x000D2144
	[PublicizedFrom(EAccessModifier.Private)]
	public void setShutdown(bool value)
	{
		base.GetComponentInChildren<Animator>().enabled = !value;
		this.PhysicsTransform.gameObject.SetActive(!value);
		this.IsNoCollisionMode.Value = value;
		if (value)
		{
			base.SetRevengeTarget(null);
			base.SetAttackTarget(null, 0);
			this.setState(EntityDrone.State.Shutdown);
			Handle handle = this.idleLoop;
			if (handle != null)
			{
				handle.Stop(this.entityId);
			}
			this.idleLoop = null;
			return;
		}
		this.isShutdown = value;
		this.isGrounded = value;
		if (this.orderState == EntityDrone.Orders.Stay)
		{
			this.setState(EntityDrone.State.Sentry);
		}
		else
		{
			this.setState(EntityDrone.State.Idle);
		}
		if (this.Owner && this.Owner.HasOwnedEntity(this.entityId))
		{
			this.Owner.GetOwnedEntity(this.entityId).ClearLastKnownPostition();
		}
	}

	// Token: 0x0600217A RID: 8570 RVA: 0x000D4015 File Offset: 0x000D2215
	[PublicizedFrom(EAccessModifier.Private)]
	public void setShutdownDestruction(bool value)
	{
		base.transform.FindInChilds("p_smokeLeft", false).gameObject.SetActive(value);
		base.transform.FindInChilds("p_smokeRight", false).gameObject.SetActive(value);
	}

	// Token: 0x0600217B RID: 8571 RVA: 0x000D4050 File Offset: 0x000D2250
	[PublicizedFrom(EAccessModifier.Private)]
	public void processShutdown()
	{
		this.fallBlockPos.RoundToInt(this.position - this.blockHeightOffset);
		RaycastHit raycastHit;
		if ((!this.hasFallPoint || this.world.GetBlock(this.fallBlockPos).isair) && Physics.Raycast(this.position - Origin.position + this.blockHeightOffset, Vector3.down, out raycastHit, 999f, 268500992))
		{
			this.fallPoint = raycastHit.point;
			this.isShutdown = true;
			this.isGrounded = false;
			this.hasFallPoint = true;
		}
		if (this.isGrounded)
		{
			return;
		}
		if (this.isShutdown)
		{
			Vector3 position = this.position;
			float num = Mathf.Min(1f + Vector3.Distance(this.position, this.fallPoint + Origin.position), 5f);
			if (num < 1.2f)
			{
				this.isGrounded = true;
				return;
			}
			position.y -= num * 0.05f;
			position.y = Mathf.Max(position.y, this.fallPoint.y);
			this.SetPosition(position, true);
		}
	}

	// Token: 0x0600217C RID: 8572 RVA: 0x000D417C File Offset: 0x000D237C
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateDroneSystems()
	{
		if (this.sensors != null)
		{
			this.sensors.TargetInRange();
			this.sensors.Update();
		}
		if (this.healWeapon != null && this.isHealModAttached)
		{
			if (this.state != EntityDrone.State.Shutdown && this.state != EntityDrone.State.Sentry && this.state != EntityDrone.State.Heal && this.healWeapon.targetNeedsHealing(this.Owner))
			{
				this.Heal();
			}
			this.healWeapon.Update();
		}
	}

	// Token: 0x0600217D RID: 8573 RVA: 0x000D41F8 File Offset: 0x000D23F8
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateState()
	{
		this.stateTime += 0.05f;
		switch (this.state)
		{
		case EntityDrone.State.Idle:
			if (this.isTargetUnderWater(this.Owner.getHeadPosition()))
			{
				this.currentTarget = this.Owner;
				this.setState(EntityDrone.State.Follow);
				return;
			}
			if (!this.steering.IsInRange(this.Owner.getHeadPosition(), this.FollowDistance + 1f))
			{
				this.currentTarget = this.Owner;
				this.setState(EntityDrone.State.Follow);
				return;
			}
			this.idleState();
			return;
		case EntityDrone.State.Sentry:
			this.sentryState();
			break;
		case EntityDrone.State.Follow:
			this.followState();
			return;
		case EntityDrone.State.Heal:
			this.healState();
			return;
		case EntityDrone.State.Attack:
		case EntityDrone.State.Shutdown:
		case EntityDrone.State.NoClip:
			break;
		default:
			return;
		}
	}

	// Token: 0x0600217E RID: 8574 RVA: 0x000D42BB File Offset: 0x000D24BB
	[PublicizedFrom(EAccessModifier.Private)]
	public void logDrone(string _log)
	{
		if (DroneManager.DebugLogEnabled)
		{
			Type type = base.GetType();
			Log.Out(((type != null) ? type.ToString() : null) + " " + _log);
		}
	}

	// Token: 0x0600217F RID: 8575 RVA: 0x000D42E8 File Offset: 0x000D24E8
	[PublicizedFrom(EAccessModifier.Private)]
	public void debugUpdate()
	{
		this.updateDebugName();
		if (this.debugCamera)
		{
			if (this.currentTarget && this.currentPath.Count == 0)
			{
				this.debugCamera.transform.LookAt(this.currentTarget.emodel.GetHeadTransform());
				return;
			}
			this.debugCamera.transform.forward = base.transform.forward;
		}
	}

	// Token: 0x06002180 RID: 8576 RVA: 0x000D435E File Offset: 0x000D255E
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateDebugName()
	{
		this.aiManager.UpdateDebugName();
	}

	// Token: 0x170003AC RID: 940
	// (get) Token: 0x06002181 RID: 8577 RVA: 0x000D436B File Offset: 0x000D256B
	// (set) Token: 0x06002182 RID: 8578 RVA: 0x000D4374 File Offset: 0x000D2574
	public bool IsVisible
	{
		get
		{
			return this.isVisible;
		}
		set
		{
			if (value != this.isVisible)
			{
				Renderer[] componentsInChildren = this.emodel.gameObject.GetComponentsInChildren<Renderer>(true);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].enabled = value;
				}
				this.isVisible = value;
			}
		}
	}

	// Token: 0x170003AD RID: 941
	// (get) Token: 0x06002183 RID: 8579 RVA: 0x000D43BA File Offset: 0x000D25BA
	// (set) Token: 0x06002184 RID: 8580 RVA: 0x000D43C7 File Offset: 0x000D25C7
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

	// Token: 0x06002185 RID: 8581 RVA: 0x000D43D5 File Offset: 0x000D25D5
	[PublicizedFrom(EAccessModifier.Private)]
	public void setNoClip(bool value)
	{
		this.IsNoCollisionMode.Value = value;
		this.PhysicsTransform.gameObject.layer = (value ? 14 : 15);
	}

	// Token: 0x06002186 RID: 8582 RVA: 0x000D43FC File Offset: 0x000D25FC
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EntityAlive> getAvoidEntities(float distance)
	{
		List<EntityAlive> list = new List<EntityAlive>();
		List<Entity> entitiesInBounds = GameManager.Instance.World.GetEntitiesInBounds(this, new Bounds(this.position, Vector3.one * distance));
		for (int i = 0; i < entitiesInBounds.Count; i++)
		{
			EntityAlive entityAlive = entitiesInBounds[i] as EntityAlive;
			if (entityAlive != null && entitiesInBounds[i].EntityClass != null && !(entityAlive is EntityNPC) && (!entityAlive.IsSleeper || !entityAlive.IsSleeping))
			{
				list.Add(entitiesInBounds[i] as EntityAlive);
			}
		}
		return list;
	}

	// Token: 0x06002187 RID: 8583 RVA: 0x000D4498 File Offset: 0x000D2698
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isOutOfRange(Vector3 _target, float _distance)
	{
		return (this.position - _target).sqrMagnitude > _distance * _distance;
	}

	// Token: 0x06002188 RID: 8584 RVA: 0x000D44C0 File Offset: 0x000D26C0
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsAnyPlayerWithingDist(float dist)
	{
		PersistentPlayerList persistentPlayerList = GameManager.Instance.GetPersistentPlayerList();
		if (((persistentPlayerList != null) ? persistentPlayerList.Players : null) != null)
		{
			foreach (KeyValuePair<PlatformUserIdentifierAbs, PersistentPlayerData> keyValuePair in persistentPlayerList.Players)
			{
				EntityPlayer entityPlayer = this.world.GetEntity(keyValuePair.Value.EntityId) as EntityPlayer;
				if (entityPlayer && (entityPlayer.getChestPosition() - this.position).magnitude <= dist)
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x06002189 RID: 8585 RVA: 0x000D456C File Offset: 0x000D276C
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i getBlockPosition(Vector3 worldPos)
	{
		Vector3i one = new Vector3i(worldPos);
		Vector3 v = worldPos - one.ToVector3Center();
		return one + Vector3i.FromVector3Rounded(v);
	}

	// Token: 0x0600218A RID: 8586 RVA: 0x000D459C File Offset: 0x000D279C
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 findOpenBlockAbove(Vector3 targetPosition, int maxHeight = 256)
	{
		Vector3i vector3i = this.getBlockPosition(targetPosition);
		vector3i += Vector3i.up;
		int num = 1;
		BlockValue block = this.world.GetBlock(vector3i);
		while (!block.isair && num < maxHeight)
		{
			num++;
			vector3i += Vector3i.up;
			block = this.world.GetBlock(vector3i);
		}
		return vector3i.ToVector3Center();
	}

	// Token: 0x0600218B RID: 8587 RVA: 0x000D4600 File Offset: 0x000D2800
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsStuckInBlock()
	{
		this.currentBlockPosition.RoundToInt(this.position);
		this.timeInBlock += Time.deltaTime;
		if (this.currentBlockPosition != this.lastBlockPosition)
		{
			this.lastBlockPosition = this.currentBlockPosition;
			this.timeInBlock = 0f;
		}
		return this.timeInBlock > 0.5f;
	}

	// Token: 0x0600218C RID: 8588 RVA: 0x000D466C File Offset: 0x000D286C
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsNotAbleToReachTarget(Vector3 currentTarget)
	{
		this.timeSpentToNextTarget += Time.deltaTime;
		if (this.targetDestination != currentTarget)
		{
			this.targetDestination = currentTarget;
			this.timeSpentToNextTarget = 0f;
		}
		if (this.timeSpentToNextTarget > 1f)
		{
			this.timeSpentToNextTarget = 0f;
			return true;
		}
		return false;
	}

	// Token: 0x0600218D RID: 8589 RVA: 0x000D46C8 File Offset: 0x000D28C8
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isEntityAboveOrBelow(Entity entity)
	{
		bool result = false;
		Vector3 normalized = (this.position - entity.getHeadPosition()).normalized;
		float num = this.position.x - entity.position.x;
		float num2 = this.position.z - entity.position.z;
		float num3 = this.position.y - entity.getHeadPosition().y;
		if (num > -0.85f && num < 0.85f && num2 > -0.85f && num2 < 0.85f && (num3 < -1.2f || num3 > 1.2f))
		{
			result = true;
		}
		return result;
	}

	// Token: 0x0600218E RID: 8590 RVA: 0x000D4770 File Offset: 0x000D2970
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isTargetUnderWater(Vector3 targetPosition)
	{
		Vector3i blockPosition = this.getBlockPosition(targetPosition);
		return this.world.GetBlock(blockPosition).type == 240;
	}

	// Token: 0x0600218F RID: 8591 RVA: 0x000D47A0 File Offset: 0x000D29A0
	public Vector3 getPositionOnGround()
	{
		return this.getPositionOnGround(this.position);
	}

	// Token: 0x06002190 RID: 8592 RVA: 0x000D47B0 File Offset: 0x000D29B0
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 getPositionOnGround(Vector3 pos)
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(pos - Origin.position, Vector3.down, out raycastHit, 255f, 65536))
		{
			return raycastHit.point + Origin.position;
		}
		return this.position;
	}

	// Token: 0x06002191 RID: 8593 RVA: 0x000D47F8 File Offset: 0x000D29F8
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3[] getGroupPositions(EntityAlive _entity, float followDist, bool debugDraw = false)
	{
		float d = 0.67f;
		Vector3 headPosition = _entity.getHeadPosition();
		Vector3 lookVector = _entity.GetLookVector();
		lookVector.y = 0f;
		this.groupPositions[0] = headPosition - lookVector * followDist;
		Vector3 normalized = (_entity.transform.right - lookVector).normalized;
		normalized.y = 0f;
		this.groupPositions[1] = headPosition + normalized * followDist * d;
		Vector3 normalized2 = (_entity.transform.right + lookVector).normalized;
		normalized2.y = 0f;
		this.groupPositions[2] = headPosition - normalized2 * followDist * d;
		return this.groupPositions;
	}

	// Token: 0x06002192 RID: 8594 RVA: 0x000D48D4 File Offset: 0x000D2AD4
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3[] getGroupFallbackPositions(EntityAlive _entity, float followDist, bool debugDraw = false)
	{
		float d = 0.67f;
		Vector3 headPosition = _entity.getHeadPosition();
		Vector3 vector = -_entity.GetLookVector();
		vector.y = 0f;
		this.fallbackGroupPos[0] = headPosition - vector * followDist;
		Vector3 normalized = (_entity.transform.right - vector).normalized;
		normalized.y = 0f;
		this.fallbackGroupPos[1] = headPosition + normalized * followDist * d;
		Vector3 normalized2 = (_entity.transform.right + vector).normalized;
		normalized2.y = 0f;
		this.fallbackGroupPos[2] = headPosition - normalized2 * followDist * d;
		return this.fallbackGroupPos;
	}

	// Token: 0x06002193 RID: 8595 RVA: 0x000D49B4 File Offset: 0x000D2BB4
	[PublicizedFrom(EAccessModifier.Private)]
	public float getTargetView(EntityAlive target, float degrees, float weight)
	{
		Vector3 lookVector = target.GetLookVector();
		Vector3 to = this.position - target.position;
		float num = Vector3.Angle(lookVector, to);
		if (num < degrees * 0.5f)
		{
			return (1f - num / degrees * 0.5f) * weight;
		}
		return 0f;
	}

	// Token: 0x06002194 RID: 8596 RVA: 0x00002914 File Offset: 0x00000B14
	public void OnWakeUp()
	{
	}

	// Token: 0x06002195 RID: 8597 RVA: 0x00002914 File Offset: 0x00000B14
	public void NotifyOffTheWorld()
	{
	}

	// Token: 0x06002196 RID: 8598 RVA: 0x000D4A01 File Offset: 0x000D2C01
	public override string MakeDebugNameInfo()
	{
		return string.Format("\nState: {0}", this.state.ToStringCached<EntityDrone.State>());
	}

	// Token: 0x170003AE RID: 942
	// (get) Token: 0x06002197 RID: 8599 RVA: 0x000D4A18 File Offset: 0x000D2C18
	public bool IsFrendlyFireEnabled
	{
		get
		{
			return this.debugFriendlyFire;
		}
	}

	// Token: 0x06002198 RID: 8600 RVA: 0x000D4A20 File Offset: 0x000D2C20
	public void DebugToggleFriendlyFire()
	{
		this.debugFriendlyFire = !this.debugFriendlyFire;
	}

	// Token: 0x170003AF RID: 943
	// (get) Token: 0x06002199 RID: 8601 RVA: 0x000D4A31 File Offset: 0x000D2C31
	public bool IsDebugCameraEnabled
	{
		get
		{
			return this.debugShowCamera;
		}
	}

	// Token: 0x0600219A RID: 8602 RVA: 0x000D4A39 File Offset: 0x000D2C39
	public void DebugToggleDebugCamera()
	{
		this._prepareDebugCamera();
		this.debugShowCamera = !this.debugShowCamera;
	}

	// Token: 0x0600219B RID: 8603 RVA: 0x000D4A50 File Offset: 0x000D2C50
	public void SetDebugCameraEnabled(bool value)
	{
		this._prepareDebugCamera();
		this.debugShowCamera = value;
	}

	// Token: 0x0600219C RID: 8604 RVA: 0x000D4A60 File Offset: 0x000D2C60
	[PublicizedFrom(EAccessModifier.Private)]
	public void _prepareDebugCamera()
	{
		if (this.debugShowCamera && this.debugCamera)
		{
			UnityEngine.Object.Destroy(this.debugCamera);
			return;
		}
		this.debugCamera = new GameObject("Camera");
		this.debugCamera.transform.SetParent(base.transform);
		this.debugCamera.transform.localPosition = Vector3.zero;
		this.debugCamera.transform.localRotation = Quaternion.identity;
		Camera camera = this.debugCamera.AddComponent<Camera>();
		Rect rect = camera.rect;
		float num = 0.35f;
		rect.width = num;
		rect.height = num;
		float num2 = 1f - num;
		rect.x = num2;
		rect.y = num2;
		camera.rect = rect;
		camera.farClipPlane = 32f;
	}

	// Token: 0x0600219D RID: 8605 RVA: 0x000D4B30 File Offset: 0x000D2D30
	public void Debug_ToggleReconMode()
	{
		this._prepareReconCam();
		DroneManager.Debug_LocalControl = !DroneManager.Debug_LocalControl;
		EntityPlayerLocal entityPlayerLocal = this.Owner as EntityPlayerLocal;
		entityPlayerLocal.PlayerUI.windowManager.SetHUDEnabled(DroneManager.Debug_LocalControl ? GUIWindowManager.HudEnabledStates.FullHide : GUIWindowManager.HudEnabledStates.Enabled);
		entityPlayerLocal.bEntityAliveFlagsChanged = true;
		entityPlayerLocal.IsGodMode.Value = DroneManager.Debug_LocalControl;
		entityPlayerLocal.IsNoCollisionMode.Value = DroneManager.Debug_LocalControl;
		entityPlayerLocal.IsFlyMode.Value = DroneManager.Debug_LocalControl;
		if (entityPlayerLocal.IsGodMode.Value)
		{
			entityPlayerLocal.Buffs.AddBuff("god", -1, true, false, -1f);
		}
		else if (!GameManager.Instance.World.IsEditor() && !GameModeCreative.TypeName.Equals(GamePrefs.GetString(EnumGamePrefs.GameMode)))
		{
			entityPlayerLocal.Buffs.RemoveBuff("god", true);
		}
		entityPlayerLocal.IsSpectator = DroneManager.Debug_LocalControl;
	}

	// Token: 0x0600219E RID: 8606 RVA: 0x000D4C18 File Offset: 0x000D2E18
	[PublicizedFrom(EAccessModifier.Private)]
	public void _prepareReconCam()
	{
		if (DroneManager.Debug_LocalControl && this.reconCam)
		{
			UnityEngine.Object.Destroy(this.reconCam.gameObject);
			return;
		}
		GameObject gameObject = new GameObject(this.Owner.EntityName + "-Drone|Recon");
		gameObject.transform.SetParent(base.transform);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		this.reconCam = gameObject.AddComponent<Camera>();
	}

	// Token: 0x0600219F RID: 8607 RVA: 0x000D4CA2 File Offset: 0x000D2EA2
	public ushort GetSyncFlagsReplicated(ushort syncFlags)
	{
		return syncFlags & 2;
	}

	// Token: 0x060021A0 RID: 8608 RVA: 0x000D4CA8 File Offset: 0x000D2EA8
	public void SendSyncData(ushort syncFlags)
	{
		int primaryPlayerId = GameManager.Instance.World.GetPrimaryPlayerId();
		this.SendSyncData(syncFlags, primaryPlayerId);
	}

	// Token: 0x060021A1 RID: 8609 RVA: 0x000D4CD0 File Offset: 0x000D2ED0
	[PublicizedFrom(EAccessModifier.Private)]
	public void SendSyncData(ushort syncFlags, int playerId)
	{
		EntityDrone.NetPackageDroneDataSync package = NetPackageManager.GetPackage<EntityDrone.NetPackageDroneDataSync>().Setup(this, playerId, syncFlags);
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(package, false);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, false, -1, -1, -1, null, 192, false);
	}

	// Token: 0x060021A2 RID: 8610 RVA: 0x000D4D24 File Offset: 0x000D2F24
	public void WriteSyncData(BinaryWriter _bw, ushort syncFlags)
	{
		_bw.Write(0);
		if ((syncFlags & 1) > 0)
		{
			this.OwnerID.ToStream(_bw, false);
			_bw.Write(this.Health);
		}
		if ((syncFlags & 16384) > 0)
		{
			_bw.Write((byte)this.OrderState);
			if (this.OrderState == EntityDrone.Orders.Stay)
			{
				float[] array = new float[]
				{
					this.sentryPos.x,
					this.sentryPos.y,
					this.sentryPos.z
				};
				for (int i = 0; i < array.Length; i++)
				{
					_bw.Write(array[i]);
				}
			}
		}
		if ((syncFlags & 32768) > 0)
		{
			_bw.Write((byte)this.state);
		}
		if ((syncFlags & 2) > 0)
		{
			byte b = 0;
			if (this.isInteractionLocked)
			{
				b |= 1;
			}
			if (this.isLocked)
			{
				b |= 2;
			}
			_bw.Write(b);
			this.ownerSteamId.ToStream(_bw, false);
			_bw.Write(this.passwordHash);
			_bw.Write((byte)this.allowedUsers.Count);
			for (int j = 0; j < this.allowedUsers.Count; j++)
			{
				this.allowedUsers[j].ToStream(_bw, false);
			}
		}
		if ((syncFlags & 8) > 0)
		{
			ItemStack[] slots = this.bag.GetSlots();
			_bw.Write((byte)slots.Length);
			for (int k = 0; k < slots.Length; k++)
			{
				slots[k].Write(_bw);
			}
		}
		if ((syncFlags & 4096) > 0)
		{
			_bw.Write(this.interactingPlayerId);
		}
		if ((syncFlags & 32) > 0)
		{
			_bw.Write(this.isQuietMode);
		}
		if ((syncFlags & 64) > 0)
		{
			_bw.Write(this.isFlashlightOn);
		}
		if ((syncFlags & 128) > 0)
		{
			this.OriginalItemValue.Write(_bw);
		}
	}

	// Token: 0x060021A3 RID: 8611 RVA: 0x000D4EE4 File Offset: 0x000D30E4
	public void ReadSyncData(BinaryReader _br, ushort syncFlags, int senderId)
	{
		byte b = _br.ReadByte();
		if ((syncFlags & 4) > 0 && (b & 8) > 0)
		{
			this.OriginalItemValue.Read(_br);
			this.LoadMods();
		}
		if ((syncFlags & 1) > 0)
		{
			this.OwnerID = PlatformUserIdentifierAbs.FromStream(_br, false, false);
			this.Health = _br.ReadInt32();
		}
		if ((syncFlags & 16384) > 0)
		{
			EntityDrone.Orders orders = (EntityDrone.Orders)_br.ReadByte();
			if (orders == EntityDrone.Orders.Stay)
			{
				this.sentryPos.x = _br.ReadSingle();
				this.sentryPos.y = _br.ReadSingle();
				this.sentryPos.z = _br.ReadSingle();
			}
			this.setOrders(orders);
			if (GameManager.IsDedicatedServer)
			{
				this.SendSyncData(16384, senderId);
			}
		}
		if ((syncFlags & 32768) > 0)
		{
			byte b2 = _br.ReadByte();
			this.transitionState = (EntityDrone.State)b2;
			this.logDrone("Read Transition State: " + this.transitionState.ToString());
		}
		if ((syncFlags & 2) > 0)
		{
			byte b3 = _br.ReadByte();
			this.isInteractionLocked = ((b3 & 1) > 0);
			this.isLocked = ((b3 & 2) > 0);
			this.ownerSteamId = PlatformUserIdentifierAbs.FromStream(_br, false, false);
			this.passwordHash = _br.ReadInt32();
			this.allowedUsers.Clear();
			int num = (int)_br.ReadByte();
			for (int i = 0; i < num; i++)
			{
				this.allowedUsers.Add(PlatformUserIdentifierAbs.FromStream(_br, true, false));
			}
		}
		if ((syncFlags & 8) > 0)
		{
			int num2 = (int)_br.ReadByte();
			ItemStack[] array = new ItemStack[num2];
			for (int j = 0; j < num2; j++)
			{
				ItemStack itemStack = new ItemStack();
				array[j] = itemStack.Read(_br);
				this.lootContainer.UpdateSlot(j, array[j]);
			}
			this.bag.SetSlots(array);
			this.bag.OnUpdate();
		}
		if ((syncFlags & 4096) > 0)
		{
			int requestId = _br.ReadInt32();
			this.CheckInteractionRequest(senderId, requestId);
		}
		if ((syncFlags & 16) > 0)
		{
			this.performRepair();
			Log.Warning("Read Repair Action: " + 16.ToString());
		}
		if ((syncFlags & 32) > 0)
		{
			this.isQuietMode = _br.ReadBoolean();
			if (this.isQuietMode)
			{
				Handle handle = this.idleLoop;
				if (handle != null)
				{
					handle.Stop(this.entityId);
				}
				this.idleLoop = null;
			}
		}
		if ((syncFlags & 64) > 0)
		{
			this.isFlashlightOn = _br.ReadBoolean();
			this.setFlashlightOn(this.isFlashlightOn);
		}
		if ((syncFlags & 128) > 0)
		{
			this.OriginalItemValue.Read(_br);
			this.LoadMods();
		}
	}

	// Token: 0x060021A4 RID: 8612 RVA: 0x000D5164 File Offset: 0x000D3364
	[PublicizedFrom(EAccessModifier.Private)]
	public void CheckInteractionRequest(int _playerId, int _requestId)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			if (_requestId != -1)
			{
				this.ValidateInteractingPlayer();
				ushort num = 4096;
				if (this.interactingPlayerId == -1)
				{
					this.interactingPlayerId = _playerId;
					num |= 2;
				}
				this.SendSyncData(num, _playerId);
				return;
			}
			if (this.interactingPlayerId == _playerId)
			{
				this.interactingPlayerId = -1;
				return;
			}
		}
		else
		{
			this.StartInteraction(_playerId, _requestId);
		}
	}

	// Token: 0x060021A5 RID: 8613 RVA: 0x000D51C4 File Offset: 0x000D33C4
	[PublicizedFrom(EAccessModifier.Private)]
	public void StartInteraction(int _playerId, int _requestId)
	{
		EntityPlayerLocal localPlayerFromID = GameManager.Instance.World.GetLocalPlayerFromID(_playerId);
		if (!localPlayerFromID)
		{
			return;
		}
		if (_requestId != _playerId)
		{
			GameManager.ShowTooltip(localPlayerFromID, Localization.Get("ttVehicleInUse", false), string.Empty, "ui_denied", null, false, false, 0f);
			return;
		}
		this.interactingPlayerId = _playerId;
		int num = this.interactionRequestType;
		if (num == 1)
		{
			GUIWindowManager windowManager = LocalPlayerUI.GetUIForPlayer(localPlayerFromID).windowManager;
			((XUiC_DroneWindowGroup)((XUiWindowGroup)windowManager.GetWindow(XUiC_DroneWindowGroup.ID)).Controller).CurrentVehicleEntity = this;
			windowManager.Open(XUiC_DroneWindowGroup.ID, true, false, true);
			Manager.BroadcastPlayByLocalPlayer(this.position, "UseActions/service_vehicle");
			return;
		}
		if (num != 10)
		{
			return;
		}
		this.accessInventory(localPlayerFromID);
	}

	// Token: 0x060021A6 RID: 8614 RVA: 0x000D527B File Offset: 0x000D347B
	public void StopUIInteraction()
	{
		this.StopInteraction(234);
	}

	// Token: 0x060021A7 RID: 8615 RVA: 0x000D5288 File Offset: 0x000D3488
	public void StopUIInsteractionSecurity()
	{
		this.StopInteraction(2);
	}

	// Token: 0x060021A8 RID: 8616 RVA: 0x000D5291 File Offset: 0x000D3491
	[PublicizedFrom(EAccessModifier.Private)]
	public void StopInteraction(ushort syncFlags = 0)
	{
		this.interactingPlayerId = -1;
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			syncFlags |= 4096;
		}
		if (syncFlags != 0)
		{
			this.SendSyncData(syncFlags);
		}
	}

	// Token: 0x060021A9 RID: 8617 RVA: 0x000D52BA File Offset: 0x000D34BA
	[PublicizedFrom(EAccessModifier.Private)]
	public void ValidateInteractingPlayer()
	{
		if (!GameManager.Instance.World.GetEntity(this.interactingPlayerId))
		{
			this.interactingPlayerId = -1;
		}
	}

	// Token: 0x170003B0 RID: 944
	// (get) Token: 0x060021AA RID: 8618 RVA: 0x000D52DF File Offset: 0x000D34DF
	// (set) Token: 0x060021AB RID: 8619 RVA: 0x000D52E7 File Offset: 0x000D34E7
	public int EntityId
	{
		get
		{
			return this.entityId;
		}
		set
		{
			this.entityId = value;
		}
	}

	// Token: 0x060021AC RID: 8620 RVA: 0x000D52F0 File Offset: 0x000D34F0
	public bool IsLocked()
	{
		return this.isLocked;
	}

	// Token: 0x060021AD RID: 8621 RVA: 0x000D52F8 File Offset: 0x000D34F8
	public void SetLocked(bool _isLocked)
	{
		this.isLocked = _isLocked;
	}

	// Token: 0x060021AE RID: 8622 RVA: 0x000D5301 File Offset: 0x000D3501
	public PlatformUserIdentifierAbs GetOwner()
	{
		return this.ownerSteamId;
	}

	// Token: 0x060021AF RID: 8623 RVA: 0x000D5309 File Offset: 0x000D3509
	public void SetOwner(PlatformUserIdentifierAbs _userIdentifier)
	{
		this.ownerSteamId = _userIdentifier;
	}

	// Token: 0x060021B0 RID: 8624 RVA: 0x000D5312 File Offset: 0x000D3512
	public bool IsUserAllowed(PlatformUserIdentifierAbs _userIdentifier)
	{
		return (_userIdentifier != null && _userIdentifier.Equals(this.ownerSteamId)) || this.allowedUsers.Contains(_userIdentifier);
	}

	// Token: 0x060021B1 RID: 8625 RVA: 0x000D5333 File Offset: 0x000D3533
	public List<PlatformUserIdentifierAbs> GetUsers()
	{
		return new List<PlatformUserIdentifierAbs>();
	}

	// Token: 0x060021B2 RID: 8626 RVA: 0x000D533A File Offset: 0x000D353A
	public bool LocalPlayerIsOwner()
	{
		return this.IsOwner(PlatformManager.InternalLocalUserIdentifier);
	}

	// Token: 0x060021B3 RID: 8627 RVA: 0x000D5347 File Offset: 0x000D3547
	public bool IsOwner(PlatformUserIdentifierAbs _userIdentifier)
	{
		if (this.ownerSteamId == null && this.OwnerID != null)
		{
			return this.OwnerID.Equals(_userIdentifier);
		}
		return this.ownerSteamId.Equals(_userIdentifier);
	}

	// Token: 0x060021B4 RID: 8628 RVA: 0x000D5372 File Offset: 0x000D3572
	public bool HasPassword()
	{
		return this.passwordHash != 0;
	}

	// Token: 0x060021B5 RID: 8629 RVA: 0x000D5380 File Offset: 0x000D3580
	public bool CheckPassword(string _password, PlatformUserIdentifierAbs _userIdentifier, out bool changed)
	{
		changed = false;
		bool flag = Utils.HashString(_password) == this.passwordHash.ToString();
		if (this.LocalPlayerIsOwner())
		{
			if (!flag)
			{
				changed = true;
				this.passwordHash = _password.GetHashCode();
				this.allowedUsers.Clear();
				this.isLocked = true;
				if (this.ownerSteamId == null)
				{
					this.SetOwner(_userIdentifier);
				}
				this.SendSyncData(2);
			}
			return true;
		}
		if (flag)
		{
			this.allowedUsers.Add(_userIdentifier);
			this.SendSyncData(2);
			return true;
		}
		return false;
	}

	// Token: 0x060021B6 RID: 8630 RVA: 0x000D5404 File Offset: 0x000D3604
	public string GetPassword()
	{
		return this.passwordHash.ToString();
	}

	// Token: 0x04001891 RID: 6289
	public const string ClassName = "entityJunkDrone";

	// Token: 0x04001892 RID: 6290
	public const string ItemName = "gunBotT3JunkDrone";

	// Token: 0x04001893 RID: 6291
	public const int SaveVersion = 1;

	// Token: 0x04001894 RID: 6292
	public const string cSupportModBuff = "buffJunkDroneSupportEffect";

	// Token: 0x04001895 RID: 6293
	public static FastTags<TagGroup.Global> cStorageModifierTags = FastTags<TagGroup.Global>.Parse("droneStorage");

	// Token: 0x04001896 RID: 6294
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const string cIdleAnimName = "Base Layer.Idle";

	// Token: 0x04001897 RID: 6295
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const string cSpawnAnimName = "Base Layer.SpawnIn";

	// Token: 0x04001898 RID: 6296
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static FastTags<TagGroup.Global> repairKitTags = FastTags<TagGroup.Global>.Parse("junk");

	// Token: 0x04001899 RID: 6297
	public static bool DebugModeEnabled;

	// Token: 0x0400189A RID: 6298
	public ItemValue OriginalItemValue;

	// Token: 0x0400189B RID: 6299
	public PlatformUserIdentifierAbs OwnerID;

	// Token: 0x0400189C RID: 6300
	public float FollowDistance = 3f;

	// Token: 0x0400189D RID: 6301
	public float MaxDistFromOwner = 32f;

	// Token: 0x0400189E RID: 6302
	public float IdleHoverHeight = 2f;

	// Token: 0x0400189F RID: 6303
	public float FollowHoverHeight = 1.5f;

	// Token: 0x040018A0 RID: 6304
	public float StayHoverHeight = 2f;

	// Token: 0x040018A1 RID: 6305
	public float SpeedPathing = 2f;

	// Token: 0x040018A2 RID: 6306
	public float SpeedFlying = 3f;

	// Token: 0x040018A3 RID: 6307
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cMaxSpeedFlying = 15f;

	// Token: 0x040018A4 RID: 6308
	public float RotationSpeed = 12f;

	// Token: 0x040018A5 RID: 6309
	public float AttackActionTime = 3f;

	// Token: 0x040018A6 RID: 6310
	public float HealActionTime = 7f;

	// Token: 0x040018A7 RID: 6311
	public EntityAlive Owner;

	// Token: 0x040018A8 RID: 6312
	public DroneWeapons.HealBeamWeapon healWeapon;

	// Token: 0x040018A9 RID: 6313
	public DroneWeapons.StunBeamWeapon stunWeapon;

	// Token: 0x040018AA RID: 6314
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float accelerationTime;

	// Token: 0x040018AB RID: 6315
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float decelerationTime;

	// Token: 0x040018AC RID: 6316
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float armorDamageReduction = 1f;

	// Token: 0x040018AD RID: 6317
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float currentSpeedFlying;

	// Token: 0x040018AE RID: 6318
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isStunModAttached;

	// Token: 0x040018AF RID: 6319
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isHealModAttached;

	// Token: 0x040018B0 RID: 6320
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isGunModAttached;

	// Token: 0x040018B1 RID: 6321
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityDrone.State state;

	// Token: 0x040018B2 RID: 6322
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityDrone.State lastState;

	// Token: 0x040018B3 RID: 6323
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityDrone.State transitionState;

	// Token: 0x040018B4 RID: 6324
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float stateTime;

	// Token: 0x040018B5 RID: 6325
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float stateMaxTime;

	// Token: 0x040018B6 RID: 6326
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 lastPosition;

	// Token: 0x040018B7 RID: 6327
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float timeSpentAtLocation;

	// Token: 0x040018B8 RID: 6328
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isVisible = true;

	// Token: 0x040018B9 RID: 6329
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityAlive currentTarget;

	// Token: 0x040018BA RID: 6330
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<Vector3> currentPath = new List<Vector3>();

	// Token: 0x040018BB RID: 6331
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityDrone.EntitySteering steering;

	// Token: 0x040018BC RID: 6332
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityDrone.DroneSensors sensors;

	// Token: 0x040018BD RID: 6333
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public DroneWeapons.MachineGunWeapon machineGunWeapon;

	// Token: 0x040018BE RID: 6334
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public DroneWeapons.Weapon activeWeapon;

	// Token: 0x040018BF RID: 6335
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public FloodFillEntityPathGenerator pathMan;

	// Token: 0x040018C0 RID: 6336
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 originalGFXOffset = Vector3.zero;

	// Token: 0x040018C1 RID: 6337
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform head;

	// Token: 0x040018C2 RID: 6338
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float wakeupAnimTime;

	// Token: 0x040018C3 RID: 6339
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Color prefabColor;

	// Token: 0x040018C4 RID: 6340
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public DroneLightManager _lm;

	// Token: 0x040018C5 RID: 6341
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityDrone.Orders orderState;

	// Token: 0x040018C6 RID: 6342
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool hasNavObjectsEnabled;

	// Token: 0x040018C7 RID: 6343
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isOwnerSyncPending;

	// Token: 0x040018C8 RID: 6344
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isAnimationStateSet;

	// Token: 0x040018C9 RID: 6345
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ItemValue itemvalueToLoad;

	// Token: 0x040018CA RID: 6346
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isBeingPickedUp;

	// Token: 0x040018CB RID: 6347
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public BoxCollider interactionCollider;

	// Token: 0x040018CC RID: 6348
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string[] paintableParts = new string[]
	{
		"BaseMesh",
		"junkDroneArmRight",
		"armor"
	};

	// Token: 0x040018CD RID: 6349
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool debugFriendlyFire;

	// Token: 0x040018CE RID: 6350
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool debugShowCamera;

	// Token: 0x040018CF RID: 6351
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameObject debugCamera;

	// Token: 0x040018D0 RID: 6352
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Camera reconCam;

	// Token: 0x040018D1 RID: 6353
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isQuietMode;

	// Token: 0x040018D2 RID: 6354
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isFlashlightAttached;

	// Token: 0x040018D3 RID: 6355
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isFlashlightOn;

	// Token: 0x040018D4 RID: 6356
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isSupportModAttached;

	// Token: 0x040018D5 RID: 6357
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Handle voHandle;

	// Token: 0x040018D6 RID: 6358
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Handle idleLoop;

	// Token: 0x040018D7 RID: 6359
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool partyEventsSet;

	// Token: 0x040018D8 RID: 6360
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int[] knownPartyMembers;

	// Token: 0x040018D9 RID: 6361
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float areaScanTime = 0.5f;

	// Token: 0x040018DA RID: 6362
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float areaScanTimer = 0.5f;

	// Token: 0x040018DB RID: 6363
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isInConfinedSpace;

	// Token: 0x040018DC RID: 6364
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float debugInputRotX;

	// Token: 0x040018DD RID: 6365
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float debugInputRotY;

	// Token: 0x040018DE RID: 6366
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 debugInputFwd;

	// Token: 0x040018DF RID: 6367
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 debugInputRgt;

	// Token: 0x040018E0 RID: 6368
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 debugInputUp;

	// Token: 0x040018E1 RID: 6369
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float debugInputSpeed = 3f;

	// Token: 0x040018E2 RID: 6370
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3i debugOwnerPos;

	// Token: 0x040018E3 RID: 6371
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float overItemLimitCooldown;

	// Token: 0x040018E4 RID: 6372
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isTeleporting;

	// Token: 0x040018E5 RID: 6373
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float retryPathTime = 0.5f;

	// Token: 0x040018E6 RID: 6374
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isTryingToFindPath;

	// Token: 0x040018E7 RID: 6375
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public BlockValue currentBlock;

	// Token: 0x040018E8 RID: 6376
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3i currentBlockPosition;

	// Token: 0x040018E9 RID: 6377
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3i lastBlockPosition;

	// Token: 0x040018EA RID: 6378
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float timeInBlock;

	// Token: 0x040018EB RID: 6379
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float physColHeight = 0.6f;

	// Token: 0x040018EC RID: 6380
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cTalkCommand = 0;

	// Token: 0x040018ED RID: 6381
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cServiceCommand = 1;

	// Token: 0x040018EE RID: 6382
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cRepairCommand = 2;

	// Token: 0x040018EF RID: 6383
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cLockCommand = 3;

	// Token: 0x040018F0 RID: 6384
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cUnlockCommand = 4;

	// Token: 0x040018F1 RID: 6385
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cKeypadCommand = 5;

	// Token: 0x040018F2 RID: 6386
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cTakeCommand = 6;

	// Token: 0x040018F3 RID: 6387
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cStayCommand = 7;

	// Token: 0x040018F4 RID: 6388
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cFollowCommand = 8;

	// Token: 0x040018F5 RID: 6389
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cHealCommand = 9;

	// Token: 0x040018F6 RID: 6390
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cStorageCommand = 10;

	// Token: 0x040018F7 RID: 6391
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cQuiteCommand = 11;

	// Token: 0x040018F8 RID: 6392
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cToggleLightCommand = 12;

	// Token: 0x040018F9 RID: 6393
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cCommandCount = 12;

	// Token: 0x040018FA RID: 6394
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cDebugPickup = 13;

	// Token: 0x040018FB RID: 6395
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cDebugFriendlyFire = 13;

	// Token: 0x040018FC RID: 6396
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cDebugDroneCamera = 14;

	// Token: 0x040018FD RID: 6397
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public RaycastNode focusBoxNode;

	// Token: 0x040018FF RID: 6399
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityDrone.AllyHealMode allyHealMode;

	// Token: 0x04001901 RID: 6401
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cNotifyNeedsHealItemCooldown = 30f;

	// Token: 0x04001902 RID: 6402
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cNotifyNeedsHealMaxNotifyCount = 2;

	// Token: 0x04001903 RID: 6403
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float needsHealItemTimer;

	// Token: 0x04001904 RID: 6404
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int needsHealNotifyCount;

	// Token: 0x04001905 RID: 6405
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 sentryPos;

	// Token: 0x04001906 RID: 6406
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 followTargetPos;

	// Token: 0x04001907 RID: 6407
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool hasOpenGroupPos;

	// Token: 0x04001908 RID: 6408
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<RaycastNode> nodePath = new List<RaycastNode>();

	// Token: 0x04001909 RID: 6409
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool transitionToIdle;

	// Token: 0x0400190A RID: 6410
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float transitionToIdleTime = 0.5f;

	// Token: 0x0400190B RID: 6411
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cPathLayer = 1073807360;

	// Token: 0x0400190C RID: 6412
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 currentPathTarget;

	// Token: 0x0400190D RID: 6413
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool IsTargetPlayer;

	// Token: 0x0400190E RID: 6414
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cDebugExtraPathTime = 2f;

	// Token: 0x0400190F RID: 6415
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float debugPathDelay = 2f;

	// Token: 0x04001910 RID: 6416
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool debugPathTiming;

	// Token: 0x04001911 RID: 6417
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool triedFollowTeleport;

	// Token: 0x04001912 RID: 6418
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<Vector3> projectedPath = new List<Vector3>();

	// Token: 0x04001913 RID: 6419
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isShutdown;

	// Token: 0x04001914 RID: 6420
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isGrounded;

	// Token: 0x04001915 RID: 6421
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 fallPoint;

	// Token: 0x04001916 RID: 6422
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool hasFallPoint;

	// Token: 0x04001917 RID: 6423
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3i fallBlockPos;

	// Token: 0x04001918 RID: 6424
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 blockHeightOffset = new Vector3(0f, 0.5f, 0f);

	// Token: 0x04001919 RID: 6425
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float timeSpentToNextTarget;

	// Token: 0x0400191A RID: 6426
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 targetDestination;

	// Token: 0x0400191B RID: 6427
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3[] groupPositions = new Vector3[3];

	// Token: 0x0400191C RID: 6428
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3[] fallbackGroupPos = new Vector3[3];

	// Token: 0x0400191D RID: 6429
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const ushort cSyncReplicate = 2;

	// Token: 0x0400191E RID: 6430
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const byte cSyncVersion = 0;

	// Token: 0x0400191F RID: 6431
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const ushort cSyncOwnerKey = 1;

	// Token: 0x04001920 RID: 6432
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const ushort cSyncInteractAndSecurity = 2;

	// Token: 0x04001921 RID: 6433
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const ushort cSyncAction = 4;

	// Token: 0x04001922 RID: 6434
	public const ushort cSyncStorage = 8;

	// Token: 0x04001923 RID: 6435
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const ushort cSyncInteractRequest = 4096;

	// Token: 0x04001924 RID: 6436
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const ushort cSyncOrderState = 16384;

	// Token: 0x04001925 RID: 6437
	public const ushort cSyncState = 32768;

	// Token: 0x04001926 RID: 6438
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const byte cSyncInteractAndSecurityFInteracting = 1;

	// Token: 0x04001927 RID: 6439
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const byte cSyncInteractAndSecurityFLocked = 2;

	// Token: 0x04001928 RID: 6440
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const ushort cSyncRepairAction = 16;

	// Token: 0x04001929 RID: 6441
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const ushort cSyncQuiteMode = 32;

	// Token: 0x0400192A RID: 6442
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const ushort cSyncLightMod = 64;

	// Token: 0x0400192B RID: 6443
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const ushort cSyncService = 128;

	// Token: 0x0400192C RID: 6444
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isShutdownPending;

	// Token: 0x0400192D RID: 6445
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isLocked;

	// Token: 0x0400192E RID: 6446
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int passwordHash;

	// Token: 0x0400192F RID: 6447
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<PlatformUserIdentifierAbs> allowedUsers = new List<PlatformUserIdentifierAbs>();

	// Token: 0x04001930 RID: 6448
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isInteractionLocked;

	// Token: 0x04001931 RID: 6449
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int interactingPlayerId = -1;

	// Token: 0x04001932 RID: 6450
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int interactionRequestType;

	// Token: 0x04001933 RID: 6451
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public PlatformUserIdentifierAbs ownerSteamId;

	// Token: 0x02000431 RID: 1073
	public class SoundKeys
	{
		// Token: 0x04001934 RID: 6452
		public const string cIdleHover = "drone_idle_hover";

		// Token: 0x04001935 RID: 6453
		public const string cFly = "drone_fly";

		// Token: 0x04001936 RID: 6454
		public const string cStorageOpen = "vehicle_storage_open";

		// Token: 0x04001937 RID: 6455
		public const string cStorageClose = "vehicle_storage_close";

		// Token: 0x04001938 RID: 6456
		public const string cHealEffect = "drone_healeffect";

		// Token: 0x04001939 RID: 6457
		public const string cAttackEffect = "drone_attackeffect";

		// Token: 0x0400193A RID: 6458
		public const string cCommand = "drone_command";

		// Token: 0x0400193B RID: 6459
		public const string cEmpty = "drone_empty";

		// Token: 0x0400193C RID: 6460
		public const string cEnemySense = "drone_enemy_sense";

		// Token: 0x0400193D RID: 6461
		public const string cEnemyEngauge = "drone_enemy_engauge";

		// Token: 0x0400193E RID: 6462
		public const string cDroneHeal = "drone_heal";

		// Token: 0x0400193F RID: 6463
		public const string cDroneOther = "drone_other";

		// Token: 0x04001940 RID: 6464
		public const string cShutDown = "drone_shutdown";

		// Token: 0x04001941 RID: 6465
		public const string cTake = "drone_take";

		// Token: 0x04001942 RID: 6466
		public const string cTakeFail = "drone_takefail";

		// Token: 0x04001943 RID: 6467
		public const string cWakeUp = "drone_wakeup";

		// Token: 0x04001944 RID: 6468
		public const string cGreeting = "drone_greeting";
	}

	// Token: 0x02000432 RID: 1074
	[PublicizedFrom(EAccessModifier.Private)]
	public class ModKeys
	{
		// Token: 0x04001945 RID: 6469
		public const string cStorageMod = "modRoboticDroneCargoMod";

		// Token: 0x04001946 RID: 6470
		public const string cArmorMod = "modRoboticDroneArmorPlatingMod";

		// Token: 0x04001947 RID: 6471
		public const string cHealMod = "modRoboticDroneMedicMod";

		// Token: 0x04001948 RID: 6472
		public const string cStunMod = "modRoboticDroneStunWeaponMod";

		// Token: 0x04001949 RID: 6473
		public const string cGunMod = "modRoboticDroneWeaponMod";

		// Token: 0x0400194A RID: 6474
		public const string cMoraleMod = "modRoboticDroneMoraleBoosterMod";

		// Token: 0x0400194B RID: 6475
		public const string cHeadlampMod = "modRoboticDroneHeadlampMod";

		// Token: 0x0400194C RID: 6476
		public const string cHeadlampLightName = "junkDroneLamp";
	}

	// Token: 0x02000433 RID: 1075
	public enum State
	{
		// Token: 0x0400194E RID: 6478
		Idle,
		// Token: 0x0400194F RID: 6479
		Sentry,
		// Token: 0x04001950 RID: 6480
		Follow,
		// Token: 0x04001951 RID: 6481
		Heal,
		// Token: 0x04001952 RID: 6482
		Attack,
		// Token: 0x04001953 RID: 6483
		Shutdown,
		// Token: 0x04001954 RID: 6484
		NoClip,
		// Token: 0x04001955 RID: 6485
		Teleport,
		// Token: 0x04001956 RID: 6486
		None
	}

	// Token: 0x02000434 RID: 1076
	public enum Orders
	{
		// Token: 0x04001958 RID: 6488
		Follow,
		// Token: 0x04001959 RID: 6489
		Stay
	}

	// Token: 0x02000435 RID: 1077
	public enum Stance
	{
		// Token: 0x0400195B RID: 6491
		Defensive,
		// Token: 0x0400195C RID: 6492
		Passive,
		// Token: 0x0400195D RID: 6493
		Aggressive
	}

	// Token: 0x02000436 RID: 1078
	public enum AllyHealMode
	{
		// Token: 0x0400195F RID: 6495
		DoNotHeal,
		// Token: 0x04001960 RID: 6496
		HealAllies
	}

	// Token: 0x02000437 RID: 1079
	[Preserve]
	public class NetPackageDroneDataSync : NetPackage
	{
		// Token: 0x060021BD RID: 8637 RVA: 0x000D5608 File Offset: 0x000D3808
		public EntityDrone.NetPackageDroneDataSync Setup(EntityDrone _ev, int _senderId, ushort _syncFlags)
		{
			this.senderId = _senderId;
			this.vehicleId = _ev.entityId;
			this.syncFlags = _syncFlags;
			using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
			{
				pooledBinaryWriter.SetBaseStream(this.entityData);
				_ev.WriteSyncData(pooledBinaryWriter, _syncFlags);
			}
			return this;
		}

		// Token: 0x060021BE RID: 8638 RVA: 0x000D566C File Offset: 0x000D386C
		[PublicizedFrom(EAccessModifier.Protected)]
		public ~NetPackageDroneDataSync()
		{
			MemoryPools.poolMemoryStream.FreeSync(this.entityData);
		}

		// Token: 0x060021BF RID: 8639 RVA: 0x000D56A4 File Offset: 0x000D38A4
		public override void read(PooledBinaryReader _br)
		{
			this.senderId = _br.ReadInt32();
			this.vehicleId = _br.ReadInt32();
			this.syncFlags = _br.ReadUInt16();
			int length = (int)_br.ReadUInt16();
			StreamUtils.StreamCopy(_br.BaseStream, this.entityData, length, null, true);
		}

		// Token: 0x060021C0 RID: 8640 RVA: 0x000D56F0 File Offset: 0x000D38F0
		public override void write(PooledBinaryWriter _bw)
		{
			base.write(_bw);
			_bw.Write(this.senderId);
			_bw.Write(this.vehicleId);
			_bw.Write(this.syncFlags);
			_bw.Write((ushort)this.entityData.Length);
			this.entityData.WriteTo(_bw.BaseStream);
		}

		// Token: 0x060021C1 RID: 8641 RVA: 0x000D574C File Offset: 0x000D394C
		public override void ProcessPackage(World _world, GameManager _callbacks)
		{
			if (_world == null)
			{
				return;
			}
			EntityDrone entityDrone = GameManager.Instance.World.GetEntity(this.vehicleId) as EntityDrone;
			if (entityDrone == null)
			{
				return;
			}
			if (this.entityData.Length > 0L)
			{
				PooledExpandableMemoryStream obj = this.entityData;
				lock (obj)
				{
					this.entityData.Position = 0L;
					try
					{
						using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
						{
							pooledBinaryReader.SetBaseStream(this.entityData);
							entityDrone.ReadSyncData(pooledBinaryReader, this.syncFlags, this.senderId);
						}
					}
					catch (Exception e)
					{
						Log.Exception(e);
						string str = "Error syncing data for entity ";
						EntityDrone entityDrone2 = entityDrone;
						Log.Error(str + ((entityDrone2 != null) ? entityDrone2.ToString() : null) + "; Sender id = " + this.senderId.ToString());
					}
				}
			}
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				ushort syncFlagsReplicated = entityDrone.GetSyncFlagsReplicated(this.syncFlags);
				if (syncFlagsReplicated != 0)
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<EntityDrone.NetPackageDroneDataSync>().Setup(entityDrone, this.senderId, syncFlagsReplicated), false, -1, this.senderId, -1, null, 192, false);
				}
			}
		}

		// Token: 0x060021C2 RID: 8642 RVA: 0x000D58A4 File Offset: 0x000D3AA4
		public override int GetLength()
		{
			return (int)(12L + this.entityData.Length);
		}

		// Token: 0x04001961 RID: 6497
		[PublicizedFrom(EAccessModifier.Private)]
		public int senderId;

		// Token: 0x04001962 RID: 6498
		[PublicizedFrom(EAccessModifier.Private)]
		public int vehicleId;

		// Token: 0x04001963 RID: 6499
		[PublicizedFrom(EAccessModifier.Private)]
		public ushort syncFlags;

		// Token: 0x04001964 RID: 6500
		[PublicizedFrom(EAccessModifier.Private)]
		public PooledExpandableMemoryStream entityData = MemoryPools.poolMemoryStream.AllocSync(true);
	}

	// Token: 0x02000438 RID: 1080
	public class DroneInventory : Inventory
	{
		// Token: 0x060021C4 RID: 8644 RVA: 0x000D58CF File Offset: 0x000D3ACF
		public DroneInventory(IGameManager _gameManager, EntityAlive _entity) : base(_gameManager, _entity)
		{
			this.SetupSlots();
		}

		// Token: 0x060021C5 RID: 8645 RVA: 0x000D58E0 File Offset: 0x000D3AE0
		public void SetupSlots()
		{
			int num = base.PUBLIC_SLOTS + 1;
			this.slots = new ItemInventoryData[num];
			this.models = new Transform[num];
			this.m_HoldingItemIdx = 0;
			base.Clear();
		}
	}

	// Token: 0x02000439 RID: 1081
	[PublicizedFrom(EAccessModifier.Private)]
	public class SteeringMan
	{
		// Token: 0x060021C6 RID: 8646 RVA: 0x000D591B File Offset: 0x000D3B1B
		public Vector3 Seek(Vector3 pos, Vector3 target, float slowingRadius)
		{
			return this.doSeek(pos, target, slowingRadius);
		}

		// Token: 0x060021C7 RID: 8647 RVA: 0x000D5926 File Offset: 0x000D3B26
		public Vector3 Seek2D(Vector3 pos, Vector3 target, float slowingRadius)
		{
			return this.doSeek2D(pos, target, slowingRadius);
		}

		// Token: 0x060021C8 RID: 8648 RVA: 0x000D5931 File Offset: 0x000D3B31
		public Vector3 Flee(Vector3 pos, Vector3 target, float avoidRadius)
		{
			return this.doFlee(pos, target, avoidRadius);
		}

		// Token: 0x060021C9 RID: 8649 RVA: 0x000D593C File Offset: 0x000D3B3C
		public Vector3 Flee2D(Vector3 pos, Vector3 target, float avoidRadius)
		{
			return this.doFlee2D(pos, target, avoidRadius);
		}

		// Token: 0x060021CA RID: 8650 RVA: 0x000D5947 File Offset: 0x000D3B47
		public Vector3 GetDir(Vector3 from, Vector3 to)
		{
			return this.getDirVector(from, to);
		}

		// Token: 0x060021CB RID: 8651 RVA: 0x000D5954 File Offset: 0x000D3B54
		public Vector3 GetDir2D(Vector3 from, Vector3 to)
		{
			Vector3 pos = new Vector3(from.x, 0f, from.z);
			Vector3 target = new Vector3(to.x, 0f, to.z);
			return this.getDirVector(pos, target);
		}

		// Token: 0x060021CC RID: 8652 RVA: 0x000D5999 File Offset: 0x000D3B99
		public bool IsInRange(Vector3 from, Vector3 to, float dist)
		{
			return this.isInRange(from, to, dist);
		}

		// Token: 0x060021CD RID: 8653 RVA: 0x000D59A4 File Offset: 0x000D3BA4
		public bool IsInRange2D(Vector3 from, Vector3 to, float dist)
		{
			return this.isInRange2D(from, to, dist);
		}

		// Token: 0x060021CE RID: 8654 RVA: 0x000D59AF File Offset: 0x000D3BAF
		public Vector3 GetPointAround(Vector3 lhs, Vector3 rhs, float radius)
		{
			return this.getPointAround(lhs, rhs, radius);
		}

		// Token: 0x060021CF RID: 8655 RVA: 0x000D59BA File Offset: 0x000D3BBA
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3 getVec(Vector3 pos, Vector3 target)
		{
			return target - pos;
		}

		// Token: 0x060021D0 RID: 8656 RVA: 0x000D59C4 File Offset: 0x000D3BC4
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3 getDirVector(Vector3 pos, Vector3 target)
		{
			return this.getVec(pos, target).normalized;
		}

		// Token: 0x060021D1 RID: 8657 RVA: 0x000D59E4 File Offset: 0x000D3BE4
		[PublicizedFrom(EAccessModifier.Private)]
		public float getDist(Vector3 pos, Vector3 target)
		{
			return this.getVec(pos, target).magnitude;
		}

		// Token: 0x060021D2 RID: 8658 RVA: 0x000D5A04 File Offset: 0x000D3C04
		[PublicizedFrom(EAccessModifier.Private)]
		public bool isInRange(Vector3 from, Vector3 to, float dist)
		{
			return (from - to).sqrMagnitude < dist * dist;
		}

		// Token: 0x060021D3 RID: 8659 RVA: 0x000D5A28 File Offset: 0x000D3C28
		[PublicizedFrom(EAccessModifier.Private)]
		public bool isInRange2D(Vector3 from, Vector3 to, float dist)
		{
			Vector3 from2 = new Vector3(from.x, 0f, from.z);
			Vector3 to2 = new Vector3(to.x, 0f, to.z);
			return this.isInRange(from2, to2, dist);
		}

		// Token: 0x060021D4 RID: 8660 RVA: 0x000D5A6E File Offset: 0x000D3C6E
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3 getPointAround(Vector3 lhs, Vector3 rhs, float radius)
		{
			return Vector3.Cross(lhs, rhs) * radius * 0.5f;
		}

		// Token: 0x060021D5 RID: 8661 RVA: 0x000D5A88 File Offset: 0x000D3C88
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3 doSeek(Vector3 pos, Vector3 target, float radius)
		{
			float dist = this.getDist(pos, target);
			if (dist < radius)
			{
				return this.getDirVector(pos, target) * (dist / radius);
			}
			return this.getDirVector(pos, target);
		}

		// Token: 0x060021D6 RID: 8662 RVA: 0x000D5ABC File Offset: 0x000D3CBC
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3 doSeek2D(Vector3 pos, Vector3 target, float radius)
		{
			Vector3 result = this.doSeek(pos, target, radius);
			result.y = 0f;
			return result;
		}

		// Token: 0x060021D7 RID: 8663 RVA: 0x000D5AE0 File Offset: 0x000D3CE0
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3 doFlee(Vector3 pos, Vector3 target, float radius)
		{
			return -this.doSeek(pos, target, radius);
		}

		// Token: 0x060021D8 RID: 8664 RVA: 0x000D5AF0 File Offset: 0x000D3CF0
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3 doFlee2D(Vector3 pos, Vector3 target, float radius)
		{
			Vector3 result = this.doFlee(pos, target, radius);
			result.y = 0f;
			return result;
		}

		// Token: 0x04001965 RID: 6501
		[PublicizedFrom(EAccessModifier.Protected)]
		public const int kMaxDistance = 1000;
	}

	// Token: 0x0200043A RID: 1082
	[Preserve]
	[PublicizedFrom(EAccessModifier.Private)]
	public class EntitySteering : EntityDrone.SteeringMan
	{
		// Token: 0x060021DA RID: 8666 RVA: 0x000D5B14 File Offset: 0x000D3D14
		public EntitySteering(EntityAlive _entity)
		{
			this.entity = _entity;
		}

		// Token: 0x060021DB RID: 8667 RVA: 0x000D5B23 File Offset: 0x000D3D23
		public Vector3 Hover(float height, float slowingRadius = 1f)
		{
			return this.doHover(this.entity.position, height, slowingRadius);
		}

		// Token: 0x060021DC RID: 8668 RVA: 0x000D5B38 File Offset: 0x000D3D38
		public Vector3 FollowPlayer(Vector3 playerPos, Vector3 playerLookDir, float followDist, float degrees = 90f, float maxDist = 15f)
		{
			return this.followTarget(this.entity.position, playerPos, playerLookDir, followDist, degrees, maxDist);
		}

		// Token: 0x060021DD RID: 8669 RVA: 0x000D5B52 File Offset: 0x000D3D52
		public Vector3 AvoidArc(Vector3 fromPos, Vector3 toPos, Vector3 dir, Vector3 up, bool subtract, float degrees, float maxDist = 1000f)
		{
			return this.doAvoidArc(fromPos, toPos, dir, up, subtract, degrees, maxDist);
		}

		// Token: 0x060021DE RID: 8670 RVA: 0x000D5B65 File Offset: 0x000D3D65
		public Vector3 AvoidArc2D(Vector3 fromPos, Vector3 toPos, Vector3 dir, bool subtract, float degrees, float maxDist = 1000f)
		{
			return this.doAvoidArc2D(fromPos, toPos, dir, subtract, degrees, maxDist);
		}

		// Token: 0x060021DF RID: 8671 RVA: 0x000D5B76 File Offset: 0x000D3D76
		public Vector3 AvoidTargetView(EntityAlive target, float followDist, bool subtract, float degrees = 90f, float maxDist = 15f)
		{
			return this.avoidTargetView(this.entity.position, target.getHeadPosition(), target.GetLookVector(), followDist, subtract, degrees, maxDist);
		}

		// Token: 0x060021E0 RID: 8672 RVA: 0x000D5B9C File Offset: 0x000D3D9C
		public Vector3 FollowTarget(EntityAlive target, Vector3 viewDir, float followDist, bool subtract, float degrees = 90f, float maxDist = 15f)
		{
			return this.pursueAvoidOwnerView(this.entity.position, target.getHeadPosition(), viewDir, Vector3.zero, followDist, subtract, degrees, maxDist);
		}

		// Token: 0x060021E1 RID: 8673 RVA: 0x000D5BCD File Offset: 0x000D3DCD
		public bool IsInRange(Vector3 target, float dist)
		{
			return base.IsInRange(this.entity.position, target, dist);
		}

		// Token: 0x060021E2 RID: 8674 RVA: 0x000D5BE2 File Offset: 0x000D3DE2
		public bool IsInRange2D(Vector3 target, float dist)
		{
			return base.IsInRange2D(this.entity.position, target, dist);
		}

		// Token: 0x060021E3 RID: 8675 RVA: 0x000D5BF7 File Offset: 0x000D3DF7
		public bool IsInRange2D(EntityAlive target, float dist)
		{
			return base.IsInRange2D(this.entity.position, target.position, dist);
		}

		// Token: 0x060021E4 RID: 8676 RVA: 0x000D5C11 File Offset: 0x000D3E11
		public float GetYPos(float height)
		{
			return this.getYPos(this.entity.position, height);
		}

		// Token: 0x060021E5 RID: 8677 RVA: 0x000D5C25 File Offset: 0x000D3E25
		public float GetAltitude(Vector3 pos)
		{
			return this.getAltitude(pos);
		}

		// Token: 0x060021E6 RID: 8678 RVA: 0x000D5C2E File Offset: 0x000D3E2E
		public bool IsAboveGround(Vector3 pos)
		{
			return this.getAltitude(pos) > -1f;
		}

		// Token: 0x060021E7 RID: 8679 RVA: 0x000D5C3E File Offset: 0x000D3E3E
		public float GetCeiling(Vector3 pos)
		{
			return this.getCeiling(pos);
		}

		// Token: 0x060021E8 RID: 8680 RVA: 0x000D5C47 File Offset: 0x000D3E47
		public bool IsBelowCeiling(Vector3 pos)
		{
			return this.getCeiling(pos) > -1f;
		}

		// Token: 0x060021E9 RID: 8681 RVA: 0x000D5C58 File Offset: 0x000D3E58
		[PublicizedFrom(EAccessModifier.Private)]
		public float getAltitude(Vector3 pos)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(pos - Origin.position, Vector3.down, out raycastHit, 1000f, 65536))
			{
				return raycastHit.distance;
			}
			return -1f;
		}

		// Token: 0x060021EA RID: 8682 RVA: 0x000D5C98 File Offset: 0x000D3E98
		[PublicizedFrom(EAccessModifier.Private)]
		public float getCeiling(Vector3 pos)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(pos - Origin.position, Vector3.up, out raycastHit, 1000f, 65536))
			{
				return raycastHit.distance;
			}
			return -1f;
		}

		// Token: 0x060021EB RID: 8683 RVA: 0x000D5CD8 File Offset: 0x000D3ED8
		[PublicizedFrom(EAccessModifier.Private)]
		public float getYPos(Vector3 pos, float height)
		{
			float altitude = this.getAltitude(pos);
			if (altitude >= 0f)
			{
				return pos.y - altitude + height;
			}
			return -1f;
		}

		// Token: 0x060021EC RID: 8684 RVA: 0x000D5D08 File Offset: 0x000D3F08
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3 doHover(Vector3 pos, float height, float radius)
		{
			float altitude = this.getAltitude(pos);
			if (altitude <= 0f)
			{
				return Vector3.zero;
			}
			Vector3 vector = (altitude < height) ? Vector3.up : Vector3.down;
			float num = Mathf.Abs(height - altitude);
			if (num < radius)
			{
				return vector * (num / radius);
			}
			return vector;
		}

		// Token: 0x060021ED RID: 8685 RVA: 0x000D5D54 File Offset: 0x000D3F54
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3 followTarget(Vector3 pos, Vector3 target, Vector3 lookDir, float followDist, float degrees, float maxDist)
		{
			return base.Seek(pos, target, followDist).normalized;
		}

		// Token: 0x060021EE RID: 8686 RVA: 0x000D5D74 File Offset: 0x000D3F74
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3 avoidTargetView(Vector3 pos, Vector3 target, Vector3 lookDir, float followDist, bool subtract, float degrees, float maxDist)
		{
			return this.AvoidArc2D(pos, target, lookDir, subtract, degrees, maxDist).normalized;
		}

		// Token: 0x060021EF RID: 8687 RVA: 0x000D5D98 File Offset: 0x000D3F98
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3 pursueAvoidOwnerView(Vector3 pos, Vector3 target, Vector3 lookDir, Vector3 offSet, float followDist, bool subtract, float degrees, float maxDist)
		{
			Vector3 a = base.Seek(pos, target, followDist);
			Vector3 b = this.AvoidArc2D(pos, target, lookDir, subtract, degrees, maxDist);
			return (a + b).normalized;
		}

		// Token: 0x060021F0 RID: 8688 RVA: 0x000D5DD0 File Offset: 0x000D3FD0
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3 doAvoidArc(Vector3 from, Vector3 to, Vector3 dir, Vector3 up, bool subtract, float degrees, float maxDist)
		{
			Vector3 to2 = from - to;
			if (Vector3.Angle(dir, to2) < degrees * 0.5f)
			{
				Vector3 vector = base.GetPointAround((to - from).normalized, up, maxDist);
				vector = (subtract ? (to - vector) : (to + vector));
				if (base.IsInRange(from, vector, maxDist))
				{
					return base.Flee(from, vector + dir * to2.magnitude, 0f);
				}
			}
			return Vector3.zero;
		}

		// Token: 0x060021F1 RID: 8689 RVA: 0x000D5E58 File Offset: 0x000D4058
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3 doAvoidArc2D(Vector3 from, Vector3 to, Vector3 dir, bool subtract, float degrees, float maxDist)
		{
			Vector3 result = this.doAvoidArc(from, to, dir, Vector3.up, subtract, degrees, maxDist);
			result.y = 0f;
			return result;
		}

		// Token: 0x04001966 RID: 6502
		[PublicizedFrom(EAccessModifier.Private)]
		public EntityAlive entity;
	}

	// Token: 0x0200043B RID: 1083
	[PublicizedFrom(EAccessModifier.Private)]
	public class DroneSensors
	{
		// Token: 0x060021F2 RID: 8690 RVA: 0x000D5E87 File Offset: 0x000D4087
		public DroneSensors(EntityAlive _entity)
		{
			this.entity = _entity;
		}

		// Token: 0x060021F3 RID: 8691 RVA: 0x000D5EB7 File Offset: 0x000D40B7
		public void Init()
		{
			this.canBarkEnemyDetected = true;
		}

		// Token: 0x060021F4 RID: 8692 RVA: 0x000D5EC0 File Offset: 0x000D40C0
		public void Update()
		{
			if (this.enemyDetectedBarkTimer > 0f)
			{
				this.enemyDetectedBarkTimer -= 0.05f;
				if (this.enemyDetectedBarkTimer <= 0f)
				{
					this.canBarkEnemyDetected = true;
				}
			}
		}

		// Token: 0x060021F5 RID: 8693 RVA: 0x000D5EF5 File Offset: 0x000D40F5
		public EntityAlive TargetInRange()
		{
			EntityAlive entityAlive = this.targetCheck();
			if (entityAlive && this.canBarkEnemyDetected)
			{
				this.barkEnemyDetected();
			}
			return entityAlive;
		}

		// Token: 0x060021F6 RID: 8694 RVA: 0x000D5F14 File Offset: 0x000D4114
		[PublicizedFrom(EAccessModifier.Private)]
		public EntityAlive targetCheck()
		{
			if (this.entity.GetRevengeTarget() && !this.entity.GetRevengeTarget().Buffs.HasBuff("buffShocked"))
			{
				return this.entity.GetRevengeTarget();
			}
			List<Entity> entitiesInBounds = GameManager.Instance.World.GetEntitiesInBounds(this.entity, new Bounds(this.entity.position, Vector3.one * this.EnemyDetectionRadius));
			for (int i = 0; i < entitiesInBounds.Count; i++)
			{
				EntityAlive entityAlive = entitiesInBounds[i] as EntityAlive;
				if (entityAlive != null && entitiesInBounds[i].EntityClass != null && entitiesInBounds[i].EntityClass.bIsEnemyEntity && !(entityAlive is EntityNPC) && (!entityAlive.IsSleeper || !entityAlive.IsSleeping) && !(entitiesInBounds[i] as EntityAlive).Buffs.HasBuff("buffShocked"))
				{
					return entitiesInBounds[i] as EntityAlive;
				}
			}
			return null;
		}

		// Token: 0x060021F7 RID: 8695 RVA: 0x000D6020 File Offset: 0x000D4220
		[PublicizedFrom(EAccessModifier.Private)]
		public void barkEnemyDetected()
		{
			EntityDrone entityDrone = this.entity as EntityDrone;
			if (entityDrone)
			{
				if (entityDrone.Owner)
				{
					Manager.Stop(entityDrone.Owner.entityId, "drone_take");
				}
				if (entityDrone.state == EntityDrone.State.Shutdown)
				{
					return;
				}
				entityDrone.PlayVO("drone_enemy_sense", true, 1f);
				this.enemyDetectedBarkTimer = this.EnemyDetectedBarkCooldown;
				this.canBarkEnemyDetected = false;
			}
		}

		// Token: 0x04001967 RID: 6503
		[PublicizedFrom(EAccessModifier.Private)]
		public EntityAlive entity;

		// Token: 0x04001968 RID: 6504
		public float EnemyDetectionRadius = 20f;

		// Token: 0x04001969 RID: 6505
		public float EnemyDetectedBarkCooldown = 90f;

		// Token: 0x0400196A RID: 6506
		[PublicizedFrom(EAccessModifier.Private)]
		public float enemyDetectedBarkTimer = 10f;

		// Token: 0x0400196B RID: 6507
		[PublicizedFrom(EAccessModifier.Private)]
		public bool canBarkEnemyDetected;
	}
}
