using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Audio;
using DynamicMusic;
using DynamicMusic.Factories;
using InControl;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000457 RID: 1111
[UnityEngine.Scripting.Preserve]
public class EntityPlayerLocal : EntityPlayer, IInventoryChangedListener, IGamePrefsChangedListener
{
	// Token: 0x14000027 RID: 39
	// (add) Token: 0x0600235B RID: 9051 RVA: 0x000DDD04 File Offset: 0x000DBF04
	// (remove) Token: 0x0600235C RID: 9052 RVA: 0x000DDD3C File Offset: 0x000DBF3C
	public event Action InventoryChangedEvent;

	// Token: 0x170003D2 RID: 978
	// (get) Token: 0x0600235D RID: 9053 RVA: 0x000DDD71 File Offset: 0x000DBF71
	public PlayerActionsLocal playerInput
	{
		get
		{
			return PlatformManager.NativePlatform.Input.PrimaryPlayer;
		}
	}

	// Token: 0x0600235E RID: 9054 RVA: 0x000DDD84 File Offset: 0x000DBF84
	[PublicizedFrom(EAccessModifier.Private)]
	public void checkedGetFPController()
	{
		if (!this.PhysicsTransform)
		{
			return;
		}
		if (!this.m_checked_vp_FPController || this.m_vp_FPController == null)
		{
			this.m_checked_vp_FPController = true;
			this.m_vp_FPController = this.PhysicsTransform.GetComponent<vp_FPController>();
			Transform transform = this.PhysicsTransform.Find("Camera");
			if (transform != null)
			{
				this.m_vp_FPCamera = transform.GetComponent<vp_FPCamera>();
			}
		}
		if (this.m_vp_FPWeapon == null)
		{
			this.m_vp_FPWeapon = this.PhysicsTransform.GetComponentInChildren<vp_FPWeapon>();
			if (this.m_vp_FPWeapon != null && this.emodel is EModelSDCS)
			{
				this.m_vp_FPWeapon.DefaultState.Preset.SetFieldValue("PositionOffset", new Vector3(0f, -1.7f, 0.02f));
				this.m_vp_FPWeapon.DefaultState.Preset.SetFieldValue("RenderingFieldOfView", 45);
			}
		}
	}

	// Token: 0x170003D3 RID: 979
	// (get) Token: 0x0600235F RID: 9055 RVA: 0x000DDE84 File Offset: 0x000DC084
	public vp_FPController vp_FPController
	{
		get
		{
			this.checkedGetFPController();
			return this.m_vp_FPController;
		}
	}

	// Token: 0x170003D4 RID: 980
	// (get) Token: 0x06002360 RID: 9056 RVA: 0x000DDE92 File Offset: 0x000DC092
	public vp_FPCamera vp_FPCamera
	{
		get
		{
			this.checkedGetFPController();
			return this.m_vp_FPCamera;
		}
	}

	// Token: 0x170003D5 RID: 981
	// (get) Token: 0x06002361 RID: 9057 RVA: 0x000DDEA0 File Offset: 0x000DC0A0
	public vp_FPWeapon vp_FPWeapon
	{
		get
		{
			this.checkedGetFPController();
			return this.m_vp_FPWeapon;
		}
	}

	// Token: 0x06002362 RID: 9058 RVA: 0x000DDEB0 File Offset: 0x000DC0B0
	public void ShowHoldingItemLayer(bool show)
	{
		if (this.playerCamera)
		{
			if (show)
			{
				this.playerCamera.cullingMask |= 1024;
				return;
			}
			this.playerCamera.cullingMask &= -1025;
		}
	}

	// Token: 0x14000028 RID: 40
	// (add) Token: 0x06002363 RID: 9059 RVA: 0x000DDEFC File Offset: 0x000DC0FC
	// (remove) Token: 0x06002364 RID: 9060 RVA: 0x000DDF34 File Offset: 0x000DC134
	public event Action DragAndDropItemChanged;

	// Token: 0x170003D6 RID: 982
	// (get) Token: 0x06002365 RID: 9061 RVA: 0x000DDF69 File Offset: 0x000DC169
	// (set) Token: 0x06002366 RID: 9062 RVA: 0x000DDF71 File Offset: 0x000DC171
	public ItemStack DragAndDropItem
	{
		get
		{
			return this.dragAndDropItem;
		}
		set
		{
			this.dragAndDropItem = value;
			this.DragAndDropItemChanged();
		}
	}

	// Token: 0x170003D7 RID: 983
	// (get) Token: 0x06002367 RID: 9063 RVA: 0x000DDF85 File Offset: 0x000DC185
	public PlayerMoveController MoveController
	{
		get
		{
			return this.moveController;
		}
	}

	// Token: 0x170003D8 RID: 984
	// (get) Token: 0x06002368 RID: 9064 RVA: 0x000DDF8D File Offset: 0x000DC18D
	public LocalPlayerUI PlayerUI
	{
		get
		{
			return this.playerUI;
		}
	}

	// Token: 0x06002369 RID: 9065 RVA: 0x000DDF95 File Offset: 0x000DC195
	public void MakeAttached(bool bAttached)
	{
		this.isLadderAttached = bAttached;
	}

	// Token: 0x170003D9 RID: 985
	// (get) Token: 0x0600236A RID: 9066 RVA: 0x000DDF9E File Offset: 0x000DC19E
	// (set) Token: 0x0600236B RID: 9067 RVA: 0x000DDFA6 File Offset: 0x000DC1A6
	public bool InAir
	{
		get
		{
			return this.inAir;
		}
		set
		{
			if (this.inAir != value)
			{
				this.inAir = value;
				this.emodel.avatarController.SetInAir(this.inAir);
			}
		}
	}

	// Token: 0x0600236C RID: 9068 RVA: 0x000DDFD0 File Offset: 0x000DC1D0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		this.playerUI = LocalPlayerUI.GetUIForPlayer(this);
		this.windowManager = this.playerUI.windowManager;
		this.nguiWindowManager = this.playerUI.nguiWindowManager;
		this.QuestJournal.OwnerPlayer = this;
		this.challengeJournal = new ChallengeJournal();
		this.challengeJournal.Player = this;
		base.Awake();
		this.dragAndDropItem = ItemStack.Empty;
		this.isEntityRemote = false;
		this.world.AddLocalPlayer(this);
		this.cameraContainerTransform = base.transform;
		this.cameraTransform = this.cameraContainerTransform.Find("Camera");
		this.playerCamera = this.cameraTransform.GetComponent<Camera>();
		this.finalCamera = this.playerCamera;
		this.ScreenEffectManager = this.cameraTransform.gameObject.AddMissingComponent<ScreenEffects>();
		Transform transform = this.cameraTransform.Find("ScreenEffectsWithDepth");
		if (transform != null)
		{
			this.uwEffectHaze = transform.Find("UnderwaterHaze");
		}
		this.uwEffectRefract = this.cameraTransform.Find("effect_refract_plane");
		this.uwEffectDebris = this.cameraTransform.Find("effect_underwater_debris");
		this.uwEffectDroplets = this.cameraTransform.Find("effect_dropletsParticle");
		this.uwEffectWaterFade = this.cameraTransform.Find("effect_water_fade");
		this.audioSourceBiomeActive = this.cameraTransform.gameObject.AddComponent<AudioSource>();
		this.audioSourceBiomeFadeOut = this.cameraTransform.gameObject.AddComponent<AudioSource>();
		this.overlayMaterial = new Material(Shader.Find("Game/UI/Screen Overlay"));
		this.CameraDOFInit();
		Shader.SetGlobalFloat("_UnderWater", 0f);
		this.renderManager = GameRenderManager.Create(this);
		GameOptionsManager.ApplyCameraOptions(this);
		this.bPlayingSpawnIn = true;
		this.spawnInTime = Time.time;
		SkyManager.SetFogDebug(-1f, float.MinValue, float.MinValue);
		WeatherManager.Instance.PushTransitions();
		this.ThreatLevel = Factory.CreateThreatLevel();
		this.ScreenEffectManager.SetScreenEffect("VibrantDeSat", 1f, 4f);
		GameManager.Instance.triggerEffectManager.EnableVibration();
		this.moveController = base.GetComponent<PlayerMoveController>();
		this.MoveController.Init();
		MumblePositionalAudio instance = SingletonMonoBehaviour<MumblePositionalAudio>.Instance;
		if (instance != null)
		{
			instance.SetPlayer(this);
		}
		this.lastWaypointUpdateTime = Time.time - 30f;
	}

	// Token: 0x170003DA RID: 986
	// (get) Token: 0x0600236D RID: 9069 RVA: 0x000DE22F File Offset: 0x000DC42F
	// (set) Token: 0x0600236E RID: 9070 RVA: 0x000DE237 File Offset: 0x000DC437
	public virtual Vector2 OnValue_InputMoveVector
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return this.m_MoveVector;
		}
		[PublicizedFrom(EAccessModifier.Protected)]
		set
		{
			this.m_MoveVector = (this.MovementRunning ? value.normalized : value);
		}
	}

	// Token: 0x170003DB RID: 987
	// (get) Token: 0x0600236F RID: 9071 RVA: 0x000DE251 File Offset: 0x000DC451
	// (set) Token: 0x06002370 RID: 9072 RVA: 0x000DE259 File Offset: 0x000DC459
	public virtual Vector2 OnValue_InputSmoothLook
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return this.m_SmoothLook;
		}
		[PublicizedFrom(EAccessModifier.Protected)]
		set
		{
			this.m_SmoothLook = value;
		}
	}

	// Token: 0x170003DC RID: 988
	// (get) Token: 0x06002371 RID: 9073 RVA: 0x000DE251 File Offset: 0x000DC451
	public virtual Vector2 OnValue_InputRawLook
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return this.m_SmoothLook;
		}
	}

	// Token: 0x170003DD RID: 989
	// (get) Token: 0x06002372 RID: 9074 RVA: 0x000C800A File Offset: 0x000C620A
	public virtual Vector3 OnValue_CameraLookDirection
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return this.GetLookVector();
		}
	}

	// Token: 0x06002373 RID: 9075 RVA: 0x000DE264 File Offset: 0x000DC464
	public override void Init(int _entityClass)
	{
		base.Init(_entityClass);
		Transform modelTransform = this.emodel.GetModelTransform();
		for (int i = 0; i < modelTransform.childCount; i++)
		{
			Transform child = modelTransform.GetChild(i);
			for (int j = 0; j < child.childCount; j++)
			{
				if (child.GetChild(j).GetComponent<Renderer>() is SkinnedMeshRenderer)
				{
					((SkinnedMeshRenderer)child.GetChild(j).GetComponent<Renderer>()).updateWhenOffscreen = true;
				}
			}
		}
		this.SetFirstPersonView(true, false);
		this.SetFirstPersonView(this.bPreferFirstPerson, false);
		this.IsGodMode.Value = !GameStats.GetBool(EnumGameStats.IsPlayerDamageEnabled);
		this.IsGodMode.OnChangeDelegates += this.OnWeatherGodModeChanged;
		this.IsNoCollisionMode.Value = this.IsGodMode.Value;
		if (this.m_characterController != null)
		{
			this.PhysicsTransform.gameObject.layer = 20;
		}
		if (this.vp_FPController != null)
		{
			this.vp_FPController.localPlayer = this;
			this.vp_FPController.Player.Register(this);
			this.vp_FPController.Player.FallImpact2.Register(this, "FallImpact", 0);
			this.vp_FPController.SyncCharacterController();
			this.vp_FPController.enabled = false;
		}
		GamePrefs.AddChangeListener(this);
	}

	// Token: 0x06002374 RID: 9076 RVA: 0x00002914 File Offset: 0x00000B14
	public void OnGamePrefChanged(EnumGamePrefs _enum)
	{
	}

	// Token: 0x06002375 RID: 9077 RVA: 0x000DE3B0 File Offset: 0x000DC5B0
	public bool NACommand(List<string> args)
	{
		if (args == null)
		{
			return false;
		}
		int count = args.Count;
		if (count > 0)
		{
			if (args[0] == "create")
			{
				if (count == 1)
				{
					return this.NAInit();
				}
			}
			else if (args[0] == "equip")
			{
				if (count != 1)
				{
					return this.NAEquip(args[1]);
				}
				return this.NAListEquipment();
			}
			else if (args[0] == "unequip")
			{
				if (count != 1)
				{
					return this.NAUnEquip(args[1]);
				}
			}
			else if (args[0] == "rot_x")
			{
				if (count != 1)
				{
					return this.NARotateX(args[1]);
				}
			}
			else
			{
				if (args[0] == "help")
				{
					this.NAHelp();
					return true;
				}
				if (args[0] == "parts")
				{
					return this.NAListParts();
				}
			}
		}
		this.NAHelp();
		return false;
	}

	// Token: 0x06002376 RID: 9078 RVA: 0x000DE4A3 File Offset: 0x000DC6A3
	[PublicizedFrom(EAccessModifier.Private)]
	public PlayerEquippedSlots _GetNASlots()
	{
		return base.transform.GetComponent<PlayerEquippedSlots>();
	}

	// Token: 0x06002377 RID: 9079 RVA: 0x000DE4B0 File Offset: 0x000DC6B0
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform _GetNAOutfit()
	{
		return base.transform.Find("Graphics/Model/base");
	}

	// Token: 0x06002378 RID: 9080 RVA: 0x000DE4C4 File Offset: 0x000DC6C4
	[PublicizedFrom(EAccessModifier.Private)]
	public void NAHelp()
	{
		Log.Warning("New avatar command help.");
		Log.Warning("------------------------");
		Log.Warning("na create                Create new avatar.");
		Log.Warning("na equip                 List current equipment.");
		Log.Warning("na equip {partname}      Equip a part.");
		Log.Warning("na help                  This help.");
		Log.Warning("na parts                 List available parts.");
		Log.Warning("na rot_x {degrees}       Turn player (0 faces away, 180 towards).");
		Log.Warning("na unequip {partname}    Unequip a part.");
	}

	// Token: 0x06002379 RID: 9081 RVA: 0x000DE52C File Offset: 0x000DC72C
	[PublicizedFrom(EAccessModifier.Private)]
	public bool NAInit()
	{
		Log.Warning("New avatar init.");
		Transform transform = base.transform.Find("Graphics/Model");
		if (transform == null)
		{
			Log.Error("Entity does not have 'Graphics/Model' node!");
			return false;
		}
		if (transform.Find("base") != null)
		{
			return false;
		}
		Transform transform2 = DataLoader.LoadAsset<Transform>("@:Entities/Player/Male/maleTestPrefab.prefab", false);
		if (transform2 == null)
		{
			return false;
		}
		transform2 = UnityEngine.Object.Instantiate<Transform>(transform2, transform);
		transform2.name = "base";
		transform2.localPosition = new Vector3(0f, 0f, 1f);
		transform2.localRotation = Quaternion.Euler(0f, 135f, 0f);
		PlayerEquippedSlots playerEquippedSlots = this._GetNASlots();
		if (playerEquippedSlots == null)
		{
			return false;
		}
		playerEquippedSlots.Init(this._GetNAOutfit());
		this.NAEquip("baseHead");
		this.NAEquip("baseBody");
		this.NAEquip("baseHands");
		this.NAEquip("baseFeet");
		return true;
	}

	// Token: 0x0600237A RID: 9082 RVA: 0x000DE62C File Offset: 0x000DC82C
	[PublicizedFrom(EAccessModifier.Private)]
	public bool NAListParts()
	{
		PlayerEquippedSlots playerEquippedSlots = this._GetNASlots();
		if (playerEquippedSlots == null)
		{
			return false;
		}
		playerEquippedSlots.ListParts();
		return true;
	}

	// Token: 0x0600237B RID: 9083 RVA: 0x000DE654 File Offset: 0x000DC854
	[PublicizedFrom(EAccessModifier.Private)]
	public bool NAListEquipment()
	{
		PlayerEquippedSlots playerEquippedSlots = this._GetNASlots();
		if (playerEquippedSlots == null)
		{
			return false;
		}
		playerEquippedSlots.ListEquipment();
		return true;
	}

	// Token: 0x0600237C RID: 9084 RVA: 0x000DE67C File Offset: 0x000DC87C
	[PublicizedFrom(EAccessModifier.Private)]
	public bool NAEquip(string partName)
	{
		Log.Warning("New avatar equip {0}", new object[]
		{
			partName
		});
		PlayerEquippedSlots playerEquippedSlots = this._GetNASlots();
		return !(playerEquippedSlots == null) && playerEquippedSlots.Equip(partName);
	}

	// Token: 0x0600237D RID: 9085 RVA: 0x000DE6B8 File Offset: 0x000DC8B8
	[PublicizedFrom(EAccessModifier.Private)]
	public bool NAUnEquip(string partName)
	{
		Log.Warning("New avatar unequip {0}", new object[]
		{
			partName
		});
		PlayerEquippedSlots playerEquippedSlots = this._GetNASlots();
		return !(playerEquippedSlots == null) && playerEquippedSlots.UnEquip(partName);
	}

	// Token: 0x0600237E RID: 9086 RVA: 0x000DE6F4 File Offset: 0x000DC8F4
	[PublicizedFrom(EAccessModifier.Private)]
	public bool NARotateX(string value)
	{
		Transform transform = this._GetNAOutfit();
		if (transform == null)
		{
			return false;
		}
		transform.localRotation = Quaternion.Euler(0f, Convert.ToSingle(value), 0f);
		return true;
	}

	// Token: 0x0600237F RID: 9087 RVA: 0x000DE730 File Offset: 0x000DC930
	[PublicizedFrom(EAccessModifier.Private)]
	public bool _IsEquipped(string partName)
	{
		PlayerEquippedSlots playerEquippedSlots = this._GetNASlots();
		return !(playerEquippedSlots == null) && playerEquippedSlots.IsEquipped(partName);
	}

	// Token: 0x06002380 RID: 9088 RVA: 0x000DE758 File Offset: 0x000DC958
	public override void PostInit()
	{
		base.PostInit();
		this.inventory.AddChangeListener(this);
		this.inventory.OnToolbeltItemsChangedInternal += this.callInventoryChanged;
		this.bag.OnBackpackItemsChangedInternal += this.callInventoryChanged;
		this.equipment.OnChanged += this.callInventoryChanged;
		this.DragAndDropItemChanged += this.callInventoryChanged;
	}

	// Token: 0x06002381 RID: 9089 RVA: 0x000DE7CE File Offset: 0x000DC9CE
	[PublicizedFrom(EAccessModifier.Private)]
	public void callInventoryChanged()
	{
		if (this.InventoryChangedEvent != null)
		{
			this.InventoryChangedEvent();
		}
	}

	// Token: 0x06002382 RID: 9090 RVA: 0x000DE7E3 File Offset: 0x000DC9E3
	public override void OnAddedToWorld()
	{
		base.GetOwnedEntities();
		base.OnAddedToWorld();
	}

	// Token: 0x06002383 RID: 9091 RVA: 0x000DE7F4 File Offset: 0x000DC9F4
	public override void OnEntityUnload()
	{
		base.OnEntityUnload();
		this.InventoryChangedEvent = null;
		GamePrefs.RemoveChangeListener(this);
		if (this.QuestJournal != null)
		{
			this.QuestJournal.UnHookQuests();
		}
		if (this.challengeJournal != null)
		{
			this.challengeJournal.EndChallenges();
		}
		this.inventory.CleanupHoldingActions();
		this.inventory.ResetActiveIndex();
		this.inventory.RemoveChangeListener(this);
		this.bag.Clear();
		this.renderManager.Destroy();
		this.renderManager = null;
		if (this.cameraTransform.parent == null)
		{
			UnityEngine.Object.Destroy(this.cameraTransform.gameObject);
		}
		GameManager.Instance.World.RemoveLocalPlayer(this);
		if (this.ScreenEffectManager)
		{
			this.ClearScreenEffects();
		}
		this.playerUI = null;
		this.windowManager = null;
		this.nguiWindowManager = null;
		this.moveController = null;
	}

	// Token: 0x06002384 RID: 9092 RVA: 0x00002914 File Offset: 0x00000B14
	public void OnInventoryChanged(Inventory _inventory)
	{
	}

	// Token: 0x06002385 RID: 9093 RVA: 0x000DE8DB File Offset: 0x000DCADB
	public bool IsMoveStateStill()
	{
		return this.moveState == EntityPlayerLocal.MoveState.Idle || this.moveState == EntityPlayerLocal.MoveState.Crouch || (this.moveState == EntityPlayerLocal.MoveState.Swim && !this.IsSwimmingMoving());
	}

	// Token: 0x06002386 RID: 9094 RVA: 0x000DE908 File Offset: 0x000DCB08
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetMoveStateToDefault()
	{
		this.SwimModeStop();
		if (!this.m_vp_FPController.enabled)
		{
			return;
		}
		vp_FPPlayerEventHandler player = this.m_vp_FPController.Player;
		if (base.IsCrouching)
		{
			if (this.MovementRunning && !player.Zoom.Active)
			{
				this.SetMoveState(EntityPlayerLocal.MoveState.CrouchRun, false);
				return;
			}
			if (player.InputMoveVector.Get() != Vector2.zero && this.m_vp_FPController.Velocity.sqrMagnitude > 0.01f)
			{
				this.SetMoveState(EntityPlayerLocal.MoveState.CrouchWalk, false);
				return;
			}
			this.SetMoveState(EntityPlayerLocal.MoveState.Crouch, false);
			return;
		}
		else
		{
			if (this.MovementRunning && !player.Zoom.Active)
			{
				this.SetMoveState(EntityPlayerLocal.MoveState.Run, false);
				return;
			}
			if (player.InputMoveVector.Get() != Vector2.zero && this.m_vp_FPController.Velocity.sqrMagnitude > 0.01f)
			{
				this.SetMoveState(EntityPlayerLocal.MoveState.Walk, false);
				return;
			}
			this.SetMoveState(EntityPlayerLocal.MoveState.Idle, false);
			return;
		}
	}

	// Token: 0x06002387 RID: 9095 RVA: 0x000DEA0C File Offset: 0x000DCC0C
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetMoveState(EntityPlayerLocal.MoveState _state, bool _isOverride = false)
	{
		vp_FPPlayerEventHandler player = this.m_vp_FPController.Player;
		bool aimingGun = this.AimingGun;
		int value = this.inventory.holdingItem.HoldType.Value;
		bool flag = (value == 27 || value == 53 || value == 68) && this.SpecialAttack;
		if (_state != this.moveState)
		{
			EntityPlayerLocal.MoveState moveState = this.moveState;
			if (moveState - EntityPlayerLocal.MoveState.Crouch <= 2 && _state != EntityPlayerLocal.MoveState.Crouch && _state != EntityPlayerLocal.MoveState.CrouchWalk && _state != EntityPlayerLocal.MoveState.CrouchRun && !_isOverride)
			{
				if (!this.vp_FPCamera.HasOverheadSpace && !this.IsGodMode.Value)
				{
					_state = this.moveState;
				}
				else if (player.Crouch.Active && !player.Crouch.TryStop(true))
				{
					_state = this.moveState;
				}
				if (_state != this.moveState)
				{
					this.FireEvent(MinEventTypes.onSelfStand, true);
				}
			}
		}
		bool flag2 = _state != this.moveState;
		if (!flag2 && aimingGun == this.moveStateAiming && flag == this.moveStateHoldBow)
		{
			return;
		}
		this.m_vp_FPController.MotorDamping = 0.346f;
		this.m_vp_FPController.PhysicsSlopeSlideLimit = 60f;
		this.m_vp_FPController.PhysicsCrouchHeightModifier = 0.7f;
		this.SetMoveStateWeaponDamping(0.08f, 0.75f);
		if (this.m_vp_FPWeapon != null)
		{
			this.m_vp_FPWeapon.RotationLookSway = new Vector3(0.25f, 0.17f, 0f);
			this.m_vp_FPWeapon.RetractionDistance = 0.1f;
			this.m_vp_FPWeapon.BobRate = new Vector4(0.9f, 0.45f, 0f, 0f);
			this.m_vp_FPWeapon.BobAmplitude = new Vector4(0.35f, 0.5f, 0f, 0f);
			this.m_vp_FPWeapon.BobInputVelocityScale = 1f;
			this.m_vp_FPWeapon.ShakeSpeed = 0f;
			this.m_vp_FPWeapon.ShakeAmplitude = new Vector3(0.25f, 0f, 2f);
		}
		switch (_state)
		{
		case EntityPlayerLocal.MoveState.Off:
			if (this.m_vp_FPController.enabled)
			{
				player.Crouch.Stop(0f);
				player.Jump.Stop(0f);
				this.m_vp_FPController.Stop();
				this.m_vp_FPController.enabled = false;
			}
			this.SwimModeStop();
			break;
		case EntityPlayerLocal.MoveState.Attached:
			player.Crouch.Stop(0f);
			player.Jump.Stop(0f);
			this.SwimModeStop();
			break;
		case EntityPlayerLocal.MoveState.Idle:
			this.m_vp_FPController.MotorAcceleration = 0.12f;
			this.m_vp_FPController.MotorBackwardsSpeed = 0.8f;
			this.m_vp_FPController.MotorSidewaysSpeed = 0.8f;
			this.m_vp_FPController.MotorSlopeSpeedDown = 1.2f;
			this.m_vp_FPController.MotorSlopeSpeedUp = 0.8f;
			if (flag2 && this.moveState != EntityPlayerLocal.MoveState.Walk)
			{
				this.FireEvent(MinEventTypes.onSelfWalk, true);
			}
			break;
		case EntityPlayerLocal.MoveState.Walk:
			this.m_vp_FPController.MotorAcceleration = 0.12f;
			this.m_vp_FPController.MotorBackwardsSpeed = 0.8f;
			this.m_vp_FPController.MotorSidewaysSpeed = 0.8f;
			this.m_vp_FPController.MotorSlopeSpeedDown = 1.2f;
			this.m_vp_FPController.MotorSlopeSpeedUp = 0.8f;
			this.SetMoveStateWeapon();
			if (flag2 && this.moveState != EntityPlayerLocal.MoveState.Idle)
			{
				this.FireEvent(MinEventTypes.onSelfWalk, true);
			}
			break;
		case EntityPlayerLocal.MoveState.Run:
			this.m_vp_FPController.MotorAcceleration = 0.35f;
			this.m_vp_FPController.MotorBackwardsSpeed = 0.8f;
			this.m_vp_FPController.MotorSidewaysSpeed = 0.5f;
			this.m_vp_FPController.MotorSlopeSpeedDown = 1.2f;
			this.m_vp_FPController.MotorSlopeSpeedUp = 0.8f;
			this.SetMoveStateWeapon();
			if (this.m_vp_FPWeapon != null)
			{
				this.m_vp_FPWeapon.BobRate = new Vector4(2f, 1f, 0f, 0f);
				this.m_vp_FPWeapon.BobAmplitude = new Vector4(1.5f, 1.2f, 0f, 0f);
			}
			if (flag2)
			{
				this.FireEvent(MinEventTypes.onSelfRun, true);
			}
			break;
		case EntityPlayerLocal.MoveState.Crouch:
			player.Crouch.Start(0f);
			this.m_vp_FPController.MotorAcceleration = 0.08f;
			this.m_vp_FPController.MotorBackwardsSpeed = 1f;
			this.m_vp_FPController.MotorSidewaysSpeed = 1f;
			this.m_vp_FPController.MotorSlopeSpeedDown = 1f;
			this.m_vp_FPController.MotorSlopeSpeedUp = 1f;
			if (flag2 && this.moveState != EntityPlayerLocal.MoveState.CrouchWalk && this.moveState != EntityPlayerLocal.MoveState.CrouchRun)
			{
				this.FireEvent(MinEventTypes.onSelfCrouch, true);
			}
			break;
		case EntityPlayerLocal.MoveState.CrouchWalk:
			player.Crouch.Start(0f);
			this.m_vp_FPController.MotorAcceleration = 0.08f;
			this.m_vp_FPController.MotorBackwardsSpeed = 1f;
			this.m_vp_FPController.MotorSidewaysSpeed = 1f;
			this.m_vp_FPController.MotorSlopeSpeedDown = 1f;
			this.m_vp_FPController.MotorSlopeSpeedUp = 1f;
			if (flag2)
			{
				this.FireEvent(MinEventTypes.onSelfCrouchWalk, true);
			}
			break;
		case EntityPlayerLocal.MoveState.CrouchRun:
			player.Crouch.Start(0f);
			this.m_vp_FPController.MotorAcceleration = 0.11f;
			this.m_vp_FPController.MotorBackwardsSpeed = 1f;
			this.m_vp_FPController.MotorSidewaysSpeed = 1f;
			this.m_vp_FPController.MotorSlopeSpeedDown = 1f;
			this.m_vp_FPController.MotorSlopeSpeedUp = 1f;
			if (flag2)
			{
				this.FireEvent(MinEventTypes.onSelfCrouchRun, true);
			}
			break;
		}
		if (flag)
		{
			this.SetMoveStateWeaponDamping(0.04f, 0.5f);
			if (this.m_vp_FPWeapon != null)
			{
				this.m_vp_FPWeapon.RotationLookSway = new Vector3(0.02f, 0.02f, 0f);
				this.m_vp_FPWeapon.RetractionDistance = 0f;
				this.m_vp_FPWeapon.BobAmplitude = new Vector4(0.1f, 0.05f, 0f, 0f);
				this.m_vp_FPWeapon.BobInputVelocityScale = 1f;
			}
		}
		if (aimingGun != this.moveStateAiming)
		{
			if (aimingGun)
			{
				this.FireEvent(MinEventTypes.onSelfAimingGunStart, true);
			}
			else
			{
				this.FireEvent(MinEventTypes.onSelfAimingGunStop, true);
			}
		}
		if (aimingGun)
		{
			this.SetMoveStateWeaponDamping(0.5f, 0.9f);
			if (this.m_vp_FPWeapon != null)
			{
				this.m_vp_FPWeapon.RotationLookSway = new Vector3(0.3f, 0.21f, 0f);
				this.m_vp_FPWeapon.RetractionDistance = 0f;
				this.m_vp_FPWeapon.ShakeSpeed = 0f;
				this.m_vp_FPWeapon.ShakeAmplitude = Vector3.zero;
				this.m_vp_FPWeapon.BobAmplitude = new Vector4(0.035f, 0.05f, 0f, 0f);
			}
		}
		this.m_vp_FPCamera.Refresh();
		if (this.m_vp_FPWeapon != null)
		{
			this.m_vp_FPWeapon.Refresh();
		}
		this.moveState = _state;
		this.moveStateAiming = aimingGun;
		this.moveStateHoldBow = flag;
	}

	// Token: 0x06002388 RID: 9096 RVA: 0x000DF148 File Offset: 0x000DD348
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetMoveStateWeapon()
	{
		this.SetMoveStateWeaponDamping(0.01f, 0.25f);
		if (this.m_vp_FPWeapon != null)
		{
			this.m_vp_FPWeapon.RotationLookSway = new Vector3(0.3f, 0.21f, 0f);
			this.m_vp_FPWeapon.RetractionDistance = 0f;
			this.m_vp_FPWeapon.BobAmplitude = new Vector4(0.25f, 0.15f, 0f, 0f);
			this.m_vp_FPWeapon.BobInputVelocityScale = 100f;
		}
	}

	// Token: 0x06002389 RID: 9097 RVA: 0x000DF1D8 File Offset: 0x000DD3D8
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetMoveStateWeaponDamping(float _stiffness, float _damping)
	{
		if (this.m_vp_FPWeapon != null)
		{
			this.m_vp_FPWeapon.PositionSpringStiffness = _stiffness;
			this.m_vp_FPWeapon.PositionSpringDamping = _damping;
			this.m_vp_FPWeapon.PositionPivotSpringStiffness = _stiffness;
			this.m_vp_FPWeapon.PositionPivotSpringDamping = _damping;
			this.m_vp_FPWeapon.RotationSpringStiffness = _stiffness;
			this.m_vp_FPWeapon.RotationSpringDamping = _damping;
			this.m_vp_FPWeapon.RotationPivotSpringStiffness = _stiffness;
			this.m_vp_FPWeapon.RotationPivotSpringDamping = _damping;
		}
	}

	// Token: 0x0600238A RID: 9098 RVA: 0x000DF253 File Offset: 0x000DD453
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsMoveStateCrouch()
	{
		return this.moveState == EntityPlayerLocal.MoveState.Crouch || this.moveState == EntityPlayerLocal.MoveState.CrouchWalk || this.moveState == EntityPlayerLocal.MoveState.CrouchRun;
	}

	// Token: 0x0600238B RID: 9099 RVA: 0x000DF274 File Offset: 0x000DD474
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool CalcIfSwimming()
	{
		float inWaterPercent = this.inWaterPercent;
		if (this.swimClimbing)
		{
			return inWaterPercent >= 0.04f && !this.onGround;
		}
		if (this.IsMoveStateCrouch())
		{
			return inWaterPercent >= 0.7f;
		}
		return inWaterPercent >= 0.6f;
	}

	// Token: 0x0600238C RID: 9100 RVA: 0x000DF2C4 File Offset: 0x000DD4C4
	[PublicizedFrom(EAccessModifier.Private)]
	public void SwimModeTick()
	{
		vp_FPPlayerEventHandler player = this.m_vp_FPController.Player;
		this.SetMoveState(EntityPlayerLocal.MoveState.Swim, false);
		if (this.swimMode < 0)
		{
			this.swimMode = 1;
			this.swimExhaustedTicks = 0;
			this.swimClimbing = false;
			this.FireEvent(MinEventTypes.onSelfSwimStart, true);
			this.emodel.avatarController.SetSwim(true);
			this.m_vp_FPController.ScaleFallSpeed(0.2f);
		}
		this.m_vp_FPController.MotorFreeFly = true;
		this.m_vp_FPController.MotorJumpForce = 0f;
		this.m_vp_FPController.MotorJumpForceDamping = 0f;
		this.m_vp_FPController.MotorJumpForceHold = 0f;
		this.m_vp_FPController.MotorJumpForceHoldDamping = 1f;
		this.m_vp_FPController.MotorAcceleration = 0.00032f;
		if (!this.Jumping && !this.inputWasDown && player.InputMoveVector.Get().SqrMagnitude() < 0.001f)
		{
			this.m_vp_FPController.PhysicsGravityModifier = 0.003f;
			if (this.swimMode != 0)
			{
				this.swimMode = 0;
				this.FireEvent(MinEventTypes.onSelfSwimIdle, true);
			}
		}
		else
		{
			this.m_vp_FPController.PhysicsGravityModifier = 0f;
			if (this.MovementRunning)
			{
				this.m_vp_FPController.MotorAcceleration = 0.0024f;
				if (this.swimMode != 2)
				{
					this.swimMode = 2;
					this.FireEvent(MinEventTypes.onSelfSwimRun, true);
				}
			}
			else if (this.swimMode != 1)
			{
				this.swimMode = 1;
			}
		}
		if (this.Stamina <= 0f)
		{
			this.swimExhaustedTicks = 60;
		}
		if (this.swimExhaustedTicks > 0)
		{
			this.swimExhaustedTicks--;
			this.m_vp_FPController.PhysicsGravityModifier = 0.004f;
			if (!this.isHeadUnderwater)
			{
				this.m_vp_FPController.PhysicsGravityModifier = 0.08f;
			}
			this.m_vp_FPController.MotorAcceleration = 0.00025f;
		}
	}

	// Token: 0x0600238D RID: 9101 RVA: 0x000DF499 File Offset: 0x000DD699
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsSwimmingMoving()
	{
		return this.swimMode > 0;
	}

	// Token: 0x0600238E RID: 9102 RVA: 0x000DF4A4 File Offset: 0x000DD6A4
	public void SwimModeUpdateThrottle()
	{
		float timeScale = Time.timeScale;
		float num = timeScale;
		if (this.swimExhaustedTicks > 0)
		{
			num *= 0.45f;
		}
		float num2 = 0.79f;
		float y = 0f;
		this.swimClimbing = false;
		if (this.inputWasJump && (this.vp_FPCamera.HasOverheadSpace || this.IsGodMode.Value))
		{
			if (this.onGround)
			{
				vp_FPController vp_FPController = this.m_vp_FPController;
				vp_FPController.m_MotorThrottle.y = vp_FPController.m_MotorThrottle.y + 0.05f * num;
			}
			else
			{
				bool flag = false;
				if (this.swimExhaustedTicks == 0 && this.moveDirection.z > 0f)
				{
					Vector3 hipPosition = this.getHipPosition();
					Vector3 forwardVector = base.GetForwardVector();
					forwardVector.y = -0.25f;
					Ray ray = new Ray(hipPosition, forwardVector);
					float num3 = this.position.y + 0.12f;
					for (float num4 = hipPosition.y + 0.3f; num4 > num3; num4 -= 0.16f)
					{
						hipPosition.y = num4;
						ray.origin = hipPosition;
						if (Voxel.Raycast(this.world, ray, 0.45f, 1073807360, 65, 0.165f) && Voxel.phyxRaycastHit.normal.y > 0.3f)
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					this.swimClimbing = true;
					num2 = 0.1f;
					y = 0.02f;
				}
				else
				{
					vp_FPController vp_FPController2 = this.m_vp_FPController;
					vp_FPController2.m_MotorThrottle.y = vp_FPController2.m_MotorThrottle.y + 0.00052f * num;
				}
			}
		}
		else if (this.inputWasDown)
		{
			vp_FPController vp_FPController3 = this.m_vp_FPController;
			vp_FPController3.m_MotorThrottle.y = vp_FPController3.m_MotorThrottle.y + -0.00038f * num;
		}
		Vector3 lookVector = this.GetLookVector();
		float num5 = this.m_vp_FPController.MotorAcceleration * num;
		this.m_vp_FPController.m_MotorThrottle += lookVector * (this.moveDirection.z * num5);
		this.m_vp_FPController.m_MotorThrottle += base.transform.TransformDirection(Vector3.right) * (this.moveDirection.x * num5 * 0.7f);
		float num6 = 0.01f + Mathf.Pow(this.m_vp_FPController.m_MotorThrottle.magnitude * 5.4f, 2f);
		this.m_vp_FPController.m_MotorThrottle /= 1f + num6 * timeScale;
		if (this.swimClimbing)
		{
			this.m_vp_FPController.m_MotorThrottle.y = y;
		}
		if (this.inWaterPercent < num2 && !base.IsInElevator() && this.m_vp_FPController.m_MotorThrottle.y > 0f)
		{
			vp_FPController vp_FPController4 = this.m_vp_FPController;
			vp_FPController4.m_MotorThrottle.y = vp_FPController4.m_MotorThrottle.y * 0.5f;
			if (this.inWaterPercent < num2 - 0.04f)
			{
				this.m_vp_FPController.m_MotorThrottle.y = 0f;
			}
		}
		this.m_vp_FPController.m_MotorThrottle = vp_MathUtility.SnapToZero(this.m_vp_FPController.m_MotorThrottle, 2E-05f);
	}

	// Token: 0x0600238F RID: 9103 RVA: 0x000DF7B8 File Offset: 0x000DD9B8
	[PublicizedFrom(EAccessModifier.Private)]
	public void SwimModeStop()
	{
		if (this.swimMode >= 0)
		{
			this.swimMode = -1;
			this.FireEvent(MinEventTypes.onSelfSwimStop, true);
			this.emodel.avatarController.SetSwim(false);
			this.m_vp_FPController.MotorFreeFly = false;
			this.m_vp_FPController.PhysicsGravityModifier = 0.2f;
			this.m_vp_FPController.MotorJumpForce = 0.13f;
			this.m_vp_FPController.MotorJumpForceDamping = 0.08f;
			this.m_vp_FPController.MotorJumpForceHold = 0.003f;
			this.m_vp_FPController.MotorJumpForceHoldDamping = 0.5f;
		}
	}

	// Token: 0x06002390 RID: 9104 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void StartJumpSwimMotion()
	{
	}

	// Token: 0x06002391 RID: 9105 RVA: 0x000DF84B File Offset: 0x000DDA4B
	public override void PhysicsPush(Vector3 forceVec, Vector3 forceWorldPos, bool affectLocalPlayerController = false)
	{
		if (this.IsGodMode.Value)
		{
			return;
		}
		if (affectLocalPlayerController && this.vp_FPController != null)
		{
			this.vp_FPController.AddForce(forceVec);
			return;
		}
		base.PhysicsPush(forceVec, forceWorldPos, affectLocalPlayerController);
	}

	// Token: 0x06002392 RID: 9106 RVA: 0x000DF884 File Offset: 0x000DDA84
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void onSpawnStateChanged()
	{
		base.onSpawnStateChanged();
		if (this.vp_FPController && (!this.Spawned || this.moveController.respawnReason != RespawnType.Teleport || !this.IsFlyMode.Value))
		{
			this.vp_FPController.enabled = this.Spawned;
		}
		if (this.Spawned && SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && PrefabEditModeManager.Instance.IsActive())
		{
			PrefabEditModeManager.Instance.LoadRecentlyUsedOrCreateNew();
		}
	}

	// Token: 0x06002393 RID: 9107 RVA: 0x000DF904 File Offset: 0x000DDB04
	public override void MoveEntityHeaded(Vector3 _direction, bool _isDirAbsolute)
	{
		vp_FPController vp_FPController = this.vp_FPController;
		bool flag = vp_FPController != null && !this.IsStuck && !this.IsFlyMode.Value;
		bool flag2 = true;
		bool flag3 = false;
		if (this.onGround)
		{
			this.isLadderAttached = false;
			this.canLadderAirAttach = true;
		}
		if (this.isLadderAttached)
		{
			this.speedStrafe = 0f;
		}
		bool flag4 = this.jumpTrigger;
		bool flag5 = flag4 != this.wasJumpTrigger;
		this.wasJumpTrigger = this.jumpTrigger;
		if (base.IsInElevator() && !this.IsFlyMode.Value)
		{
			flag3 = true;
			bool flag6 = false;
			if (flag4)
			{
				if (flag5)
				{
					if (this.isLadderAttached)
					{
						this.isLadderAttached = false;
						this.canLadderAirAttach = false;
						this.wasLadderAttachedJump = true;
					}
					else
					{
						this.canLadderAirAttach = true;
						this.wasLadderAttachedJump = false;
					}
				}
			}
			else
			{
				this.wasLadderAttachedJump = false;
			}
			if (!this.isLadderAttached)
			{
				if (vp_FPController != null)
				{
					bool flag7 = vp_FPController.enabled ? vp_FPController.Grounded : this.onGround;
					if ((flag7 || this.isSwimming) && vp_FPController.IsCollidingWall && vp_FPController.ProjectedWallMove < 0.3f && _direction.z > 0f)
					{
						flag6 = true;
					}
					else if (!flag7 && this.canLadderAirAttach)
					{
						float y = vp_FPController.Velocity.y;
						if (y <= 0f)
						{
							if (y >= -3f)
							{
								this.isLadderAttached = true;
								this.wasLadderAttachedJump = false;
							}
							else
							{
								vp_FPController.ScaleFallSpeed(0.75f);
							}
						}
					}
				}
				else if (this.onGround && this.isCollidedHorizontally && this.projectedMove < 0.3f && _direction.z > 0f)
				{
					flag6 = true;
				}
				else if (!this.onGround)
				{
					this.isLadderAttached = true;
				}
			}
			Vector3 cameraLook = this.GetCameraLook(1f);
			if (flag6 && cameraLook.y > 0.1f)
			{
				this.isLadderAttached = true;
			}
			if (this.isLadderAttached)
			{
				this.SetMoveState(EntityPlayerLocal.MoveState.Off, true);
				float num = this.MovementRunning ? 0.17f : 0.06f;
				num *= this.GetSpeedModifier();
				if (_direction.x != 0f || _direction.z > 0f)
				{
					Vector3 vector = _direction;
					if (vector.z < 0f)
					{
						vector.z = 0f;
					}
					float num2 = num * 0.65f;
					this.Move(vector, _isDirAbsolute, num2, num2);
				}
				if (this.motion.x < -0.11f)
				{
					this.motion.x = -0.11f;
				}
				if (this.motion.x > 0.11f)
				{
					this.motion.x = 0.11f;
				}
				if (this.motion.z < -0.11f)
				{
					this.motion.z = -0.11f;
				}
				if (this.motion.z > 0.11f)
				{
					this.motion.z = 0.11f;
				}
				cameraLook.y += 0.15f;
				cameraLook.y *= 2f;
				cameraLook.y = Mathf.Clamp(cameraLook.y, -1f, 1f);
				this.motion.y = _direction.z * cameraLook.y * num;
				this.fallDistance = 0f;
				this.entityCollision(this.motion);
				this.motion *= base.ScalePhysicsMulConstant(0.545f);
				this.distanceClimbed += this.motion.magnitude;
				if (this.distanceClimbed > 0.5f)
				{
					base.internalPlayStepSound(1f);
					this.distanceClimbed = 0f;
				}
			}
			flag2 = !this.isLadderAttached;
		}
		else
		{
			this.isLadderAttached = false;
		}
		if (flag && (!flag3 || flag2))
		{
			vp_FPController.enabled = true;
			this.motion = Vector3.zero;
			this.world.CheckEntityCollisionWithBlocks(this);
			if (vp_FPController.Grounded)
			{
				Transform groundTransform = vp_FPController.GroundTransform;
				if (groundTransform && groundTransform.CompareTag("LargeEntityBlocker"))
				{
					Vector2 randomOnUnitCircle = this.rand.RandomOnUnitCircle;
					vp_FPController.AddForce(randomOnUnitCircle.x * 0.008f, 0f, randomOnUnitCircle.y * 0.008f);
				}
			}
			flag2 = false;
		}
		if (flag2)
		{
			bool inElevator = base.IsInElevator();
			base.SetInElevator(false);
			base.MoveEntityHeaded(_direction, false);
			base.SetInElevator(inElevator);
		}
	}

	// Token: 0x06002394 RID: 9108 RVA: 0x000DFD88 File Offset: 0x000DDF88
	public void SetCameraAttachedToPlayer(bool _b, bool _lockCamera)
	{
		if (_b)
		{
			this.cameraTransform.SetParent(this.cameraContainerTransform, false);
			this.cameraTransform.SetAsFirstSibling();
			this.cameraTransform.SetLocalPositionAndRotation(Constants.cDefaultCameraPlayerOffset, Quaternion.identity);
			this.vp_FPCamera.Locked3rdPerson = false;
			this.movementInput.bDetachedCameraMove = false;
			return;
		}
		this.cameraTransform.parent = null;
		this.vp_FPCamera.Locked3rdPerson = _lockCamera;
	}

	// Token: 0x06002395 RID: 9109 RVA: 0x000DFDFB File Offset: 0x000DDFFB
	public bool IsCameraAttachedToPlayerOrScope()
	{
		return this.cameraTransform.parent != null;
	}

	// Token: 0x06002396 RID: 9110 RVA: 0x000DFE10 File Offset: 0x000DE010
	[PublicizedFrom(EAccessModifier.Private)]
	public bool CheckNonSolidVertical(Vector3i blockPos, int maxY, int verticalSpace)
	{
		for (int i = 0; i < maxY; i++)
		{
			if (!this.world.GetBlock(blockPos.x, blockPos.y + i + 1, blockPos.z).Block.shape.IsSolidSpace)
			{
				bool flag = true;
				for (int j = 1; j < verticalSpace; j++)
				{
					if (this.world.GetBlock(blockPos.x, blockPos.y + i + 1 + j, blockPos.z).Block.shape.IsSolidSpace)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06002397 RID: 9111 RVA: 0x000DFEB4 File Offset: 0x000DE0B4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateTransform()
	{
		if (this.AttachedToEntity != null)
		{
			return;
		}
		if (this.m_vp_FPController != null)
		{
			if (this.m_vp_FPController.enabled)
			{
				this.position = this.vp_FPController.SmoothPosition + Origin.position;
			}
			else
			{
				float elapsedPartialTicks = GameTimer.Instance.elapsedPartialTicks;
				if (elapsedPartialTicks < 1f)
				{
					base.transform.position = this.lastTickPos[0] + (this.position - this.lastTickPos[0]) * elapsedPartialTicks - Origin.position;
				}
				else
				{
					base.transform.position = this.position - Origin.position;
				}
				this.vp_FPController.SetPosition(base.transform.position);
			}
			this.rotation = this.PhysicsTransform.eulerAngles;
			if (this.vp_FPCamera != null)
			{
				this.rotation.x = -this.vp_FPCamera.Angle.x;
			}
			float num = base.width / 2f;
			float num2 = base.depth / 2f;
			this.boundingBox = BoundsUtils.BoundsForMinMax(this.position.x - num, this.position.y - this.yOffset + this.ySize, this.position.z - num2, this.position.x + num, this.position.y - this.yOffset + this.ySize + base.height, this.position.z + num2);
			return;
		}
		base.updateTransform();
	}

	// Token: 0x06002398 RID: 9112 RVA: 0x000E0074 File Offset: 0x000DE274
	public override void SetRotation(Vector3 _rot)
	{
		base.SetRotation(_rot);
		if (this.PhysicsTransform && !this.emodel.IsRagdollActive)
		{
			this.PhysicsTransform.eulerAngles = _rot;
		}
		if (this.m_vp_FPCamera)
		{
			this.m_vp_FPCamera.Angle = new Vector2(-_rot.x, _rot.y);
		}
	}

	// Token: 0x06002399 RID: 9113 RVA: 0x000E00D8 File Offset: 0x000DE2D8
	public override void OnUpdatePosition(float _partialTicks)
	{
		if (GameManager.bPlayRecordedSession && this.Spawned)
		{
			PlayerInputRecordingSystem.Instance.Play(this, true);
		}
		if (this.m_vp_FPController != null)
		{
			this.ticksExisted++;
			this.prevPos = this.position;
			this.prevRotation = this.rotation;
			if (this.Spawned)
			{
				Vector3 vector = Vector3.zero;
				for (int i = 0; i < this.lastTickPos.Length - 1; i++)
				{
					vector += this.lastTickPos[i] - this.lastTickPos[i + 1];
				}
				vector /= (float)(this.lastTickPos.Length - 1);
				float num = Mathf.Sqrt(vector.x * vector.x + vector.z * vector.z);
				this.UpdateDistanceTravelledAchievement(num);
				if (this.AttachedToEntity == null)
				{
					this.updateStepSound(vector.x, vector.z, 0f);
					base.updatePlayerLandSound(num, vector.y);
				}
				else
				{
					this.distanceWalked += num;
				}
			}
			this.updateSpeedForwardAndStrafe(this.m_vp_FPController.Velocity, _partialTicks);
			base.ReplicateSpeeds();
		}
		else
		{
			base.OnUpdatePosition(_partialTicks);
		}
		if (this.Spawned)
		{
			if (this.position.y >= 2f && this.position.y < 4f)
			{
				IAchievementManager achievementManager = PlatformManager.NativePlatform.AchievementManager;
				if (achievementManager != null)
				{
					achievementManager.SetAchievementStat(EnumAchievementDataStat.DepthAchieved, 1);
				}
			}
			else if (this.position.y >= 255f)
			{
				IAchievementManager achievementManager2 = PlatformManager.NativePlatform.AchievementManager;
				if (achievementManager2 != null)
				{
					achievementManager2.SetAchievementStat(EnumAchievementDataStat.HeightAchieved, 1);
				}
			}
		}
		GameSenseManager instance = GameSenseManager.Instance;
		if (instance != null)
		{
			instance.UpdateEventCompass(this.rotation.y);
		}
		if (GameManager.bRecordNextSession && this.Spawned)
		{
			PlayerInputRecordingSystem.Instance.Record(this, GameTimer.Instance.ticks);
		}
	}

	// Token: 0x0600239A RID: 9114 RVA: 0x000E02D0 File Offset: 0x000DE4D0
	[PublicizedFrom(EAccessModifier.Protected)]
	public void UpdateDistanceTravelledAchievement(float distanceTravelled)
	{
		float num = distanceTravelled / 1000f;
		this.achievementDistanceAccu += num;
		if ((double)this.achievementDistanceAccu > 0.05)
		{
			IAchievementManager achievementManager = PlatformManager.NativePlatform.AchievementManager;
			if (achievementManager != null)
			{
				achievementManager.SetAchievementStat(EnumAchievementDataStat.KMTravelled, this.achievementDistanceAccu);
			}
			this.achievementDistanceAccu -= 0.05f;
		}
	}

	// Token: 0x0600239B RID: 9115 RVA: 0x000E0334 File Offset: 0x000DE534
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateSpeedForwardAndStrafe(Vector3 _dist, float _partialTicks)
	{
		this.speedForward = 0f;
		this.speedStrafe = 0f;
		this.speedVertical = 0f;
		if (this.isLadderAttached)
		{
			this.speedForward += _dist.y;
			this.speedStrafe = 1234f;
			this.speedVertical = 0f;
		}
		else
		{
			if (Mathf.Abs(_dist.x) > 0.001f || Mathf.Abs(_dist.z) > 0.001f)
			{
				Vector3 vector = base.transform.InverseTransformDirection(_dist).normalized * _dist.magnitude;
				this.speedForward = (float)((int)(vector.z * 100f)) / 100f;
				this.speedStrafe = (float)((int)(vector.x * 100f)) / 100f;
			}
			if (Mathf.Abs(_dist.y) > 0.001f)
			{
				this.speedVertical += _dist.y;
			}
		}
		this.SetMovementState();
	}

	// Token: 0x0600239C RID: 9116 RVA: 0x000E043C File Offset: 0x000DE63C
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateBlockRadiusEffects()
	{
		Vector3i blockPosition = base.GetBlockPosition();
		int num = World.toChunkXZ(blockPosition.x);
		int num2 = World.toChunkXZ(blockPosition.z);
		for (int i = -1; i < 2; i++)
		{
			for (int j = -1; j < 2; j++)
			{
				Chunk chunk = (Chunk)this.world.GetChunkSync(num + j, num2 + i);
				if (chunk != null)
				{
					DictionaryList<Vector3i, TileEntity> tileEntities = chunk.GetTileEntities();
					for (int k = 0; k < tileEntities.list.Count; k++)
					{
						TileEntity tileEntity = tileEntities.list[k];
						if (tileEntity.IsActive(this.world))
						{
							Block block = this.world.GetBlock(tileEntity.ToWorldPos()).Block;
							if (block.RadiusEffects != null)
							{
								float distanceSq = base.GetDistanceSq(tileEntity.ToWorldPos().ToVector3());
								for (int l = 0; l < block.RadiusEffects.Length; l++)
								{
									BlockRadiusEffect blockRadiusEffect = block.RadiusEffects[l];
									if (distanceSq <= blockRadiusEffect.radius * blockRadiusEffect.radius && !this.Buffs.HasBuff(blockRadiusEffect.variable))
									{
										this.Buffs.AddBuff(blockRadiusEffect.variable, -1, true, false, -1f);
									}
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x0600239D RID: 9117 RVA: 0x000E05A0 File Offset: 0x000DE7A0
	public override void OnUpdateEntity()
	{
		if (this.DropTimeDelay > 0f)
		{
			this.DropTimeDelay -= 0.05f;
		}
		float value = base.Stats.Health.Value;
		float num = (this.oldHealth - value) / base.Stats.Health.Max;
		float time = Time.time;
		if (value != this.oldHealth || time > this.vibrationTimeout)
		{
			if (GamePrefs.GetInt(EnumGamePrefs.OptionsControllerVibrationStrength) > 0 && this.playerInput != null && this.moveController.GetControllerVibration())
			{
				InputDevice inputDevice = this.playerInput.Device;
				if (inputDevice == null && this.playerInput.LastInputType == BindingSourceType.DeviceBindingSource)
				{
					inputDevice = InputManager.ActiveDevice;
				}
				if (inputDevice != null)
				{
					if (this.oldHealth > value)
					{
						if (value <= 0f)
						{
							inputDevice.Vibrate(0.5f);
							GameManager.Instance.triggerEffectManager.SetGamepadVibration(0.5f);
						}
						else if (num > 0.25f)
						{
							inputDevice.Vibrate(0.5f);
							GameManager.Instance.triggerEffectManager.SetGamepadVibration(0.5f);
						}
						else if (num > 0.1f)
						{
							inputDevice.Vibrate(0.35f);
							GameManager.Instance.triggerEffectManager.SetGamepadVibration(0.35f);
						}
						else
						{
							inputDevice.Vibrate(0.25f);
							GameManager.Instance.triggerEffectManager.SetGamepadVibration(0.25f);
						}
						this.vibrationTimeout = Time.time + 0.25f;
					}
					else
					{
						inputDevice.StopVibration();
						GameManager.Instance.triggerEffectManager.StopGamepadVibration();
						this.vibrationTimeout = float.MaxValue;
					}
				}
			}
			if (num > 0.02f)
			{
				GameSenseManager instance = GameSenseManager.Instance;
				if (instance != null)
				{
					instance.UpdateEventHit();
				}
			}
			GameSenseManager instance2 = GameSenseManager.Instance;
			if (instance2 != null)
			{
				instance2.UpdateEventHealth((int)(base.Stats.Health.ValuePercentUI * 100f));
			}
			this.oldHealth = value;
		}
		this.equipment.Update();
		base.OnUpdateEntity();
		this.WeatherStatusTick();
	}

	// Token: 0x0600239E RID: 9118 RVA: 0x000E079C File Offset: 0x000DE99C
	public override void OnUpdateLive()
	{
		if (this.IsSpawned())
		{
			if (Time.time - this.updateBedrollPositionChecks > 5f)
			{
				this.updateBedrollPositionChecks = Time.time;
				if (!this.CheckSpawnPointStillThere())
				{
					this.RemoveSpawnPoints(true);
				}
			}
			if (Time.time - this.updateRadiationChecks > 5f)
			{
				this.updateRadiationChecks = Time.time;
				IChunkProvider chunkProvider = this.world.ChunkCache.ChunkProvider;
				IBiomeProvider biomeProvider;
				if (chunkProvider != null && (biomeProvider = chunkProvider.GetBiomeProvider()) != null)
				{
					float radiationAt = biomeProvider.GetRadiationAt((int)this.position.x, (int)this.position.z);
					this.Buffs.SetCustomVar("_biomeradiation", radiationAt, true, CVarOperation.set);
				}
			}
			GameEventManager.Current.UpdateCurrentBossGroup(this);
		}
		if (this.AttachedToEntity != null)
		{
			this.SetMoveState(EntityPlayerLocal.MoveState.Attached, true);
			base.OnUpdateLive();
			if (!this.isEntityRemote)
			{
				this.updateBlockRadiusEffects();
			}
			this.IsStuck = false;
			return;
		}
		bool isStuck = this.IsStuck;
		this.IsStuck = false;
		if (!this.IsFlyMode.Value)
		{
			float num = this.boundingBox.min.y + 0.5f;
			this.IsStuck = this.pushOutOfBlocks(this.position.x - base.width * 0.3f, num, this.position.z + base.depth * 0.3f);
			this.IsStuck = (this.pushOutOfBlocks(this.position.x - base.width * 0.3f, num, this.position.z - base.depth * 0.3f) || this.IsStuck);
			this.IsStuck = (this.pushOutOfBlocks(this.position.x + base.width * 0.3f, num, this.position.z - base.depth * 0.3f) || this.IsStuck);
			this.IsStuck = (this.pushOutOfBlocks(this.position.x + base.width * 0.3f, num, this.position.z + base.depth * 0.3f) || this.IsStuck);
			if (!this.IsStuck)
			{
				int num2 = Utils.Fastfloor(this.position.x);
				int num3 = Utils.Fastfloor(num);
				int num4 = Utils.Fastfloor(this.position.z);
				if (this.shouldPushOutOfBlock(num2, num3, num4, true))
				{
					if (!this.shouldPushOutOfBlock(num2 - 1, num3, num4, true))
					{
						this.IsStuck = true;
						this.motion = new Vector3(-0.25f, 0f, 0f);
					}
					else if (!this.shouldPushOutOfBlock(num2 + 1, num3, num4, true))
					{
						this.IsStuck = true;
						this.motion = new Vector3(0.25f, 0f, 0f);
					}
					if (!this.shouldPushOutOfBlock(num2, num3, num4 - 1, true))
					{
						this.IsStuck = true;
						this.motion = new Vector3(0f, 0f, -0.25f);
					}
					else if (!this.shouldPushOutOfBlock(num2, num3, num4 + 1, true))
					{
						this.IsStuck = true;
						this.motion = new Vector3(0f, 0f, 0.25f);
					}
					else if (this.CheckNonSolidVertical(new Vector3i(num2, num3 + 1, num4), 4, 2))
					{
						this.IsStuck = true;
						this.motion = new Vector3(0f, 1.6f, 0f);
						Log.Warning("{0} Player is stuck, trying to unstick", new object[]
						{
							Time.frameCount
						});
					}
				}
			}
		}
		bool flag = true;
		bool flag2 = false;
		bool flag3 = this.InAir;
		this.InAir = (!this.isLadderAttached && !this.onGround);
		if (!this.wasJumping && !this.jumpTrigger && flag3 && !this.InAir && !this.isLadderAttached)
		{
			this.EndJump();
		}
		if (this.m_vp_FPController != null)
		{
			if (this.IsStuck || this.IsFlyMode.Value)
			{
				this.SetMoveState(EntityPlayerLocal.MoveState.Off, true);
			}
			else
			{
				flag = false;
				base.Stats.Health.RegenerationAmount = 0f;
				base.Stats.Stamina.RegenerationAmount = 0f;
				if (isStuck != this.IsStuck)
				{
					this.m_vp_FPController.Stop();
				}
				if (this.m_vp_FPController.enabled)
				{
					this.onGround = this.m_vp_FPController.Grounded;
				}
				bool flag4 = this.jumpTrigger;
				if (this.isSwimming)
				{
					this.SwimModeTick();
					flag2 = true;
				}
				else
				{
					if (this.vp_FPCamera != null && this.m_vp_FPController != null && this.m_vp_FPWeapon != null)
					{
						this.SetMoveStateToDefault();
					}
					if (flag4 && (this.vp_FPCamera.HasOverheadSpace || this.IsGodMode.Value))
					{
						vp_Activity jump = this.m_vp_FPController.Player.Jump;
						bool active = jump.Active;
						this.m_vp_FPController.MotorJumpForce = Mathf.Max(EffectManager.GetValue(PassiveEffects.JumpStrength, null, this.m_vp_FPController.originalMotorJumpForce, this, null, this.CurrentStanceTag | this.CurrentMovementTag, true, true, true, true, true, 1, true, false), 0f);
						this.m_vp_FPController.MotorJumpForceHold = this.m_vp_FPController.MotorJumpForce / Mathf.Lerp(90f, 180f, Mathf.Clamp01(1f - this.m_vp_FPController.originalMotorJumpForce / this.m_vp_FPController.MotorJumpForce)) * Time.timeScale;
						if (base.IsInElevator())
						{
							if (!active && !this.wasJumping && (this.onGround || this.isLadderAttached))
							{
								jump.Start(0f);
							}
						}
						else if (!this.wasJumping)
						{
							jump.TryStart(true);
						}
						if (!active && jump.Active)
						{
							this.Jumping = true;
							this.Stamina -= Mathf.Max(EffectManager.GetValue(PassiveEffects.StaminaLoss, null, 4f, this, null, FastTags<TagGroup.Global>.Parse("jumping") | this.CurrentStanceTag | this.CurrentMovementTag, true, true, true, true, true, 1, true, false), 0f);
							this.FireEvent(MinEventTypes.onSelfJump, true);
							this.PlayOneShot(this.GetSoundJump(), false, false, false, null);
						}
						if (this.onGround && this.wasJumping)
						{
							this.Jumping = false;
							this.jumpTrigger = false;
						}
						if (this.isLadderAttached && this.wasJumping)
						{
							this.bJumping = false;
							this.jumpTrigger = false;
						}
					}
					else
					{
						this.m_vp_FPController.Player.Jump.Stop(0f);
					}
				}
				this.wasJumping = flag4;
			}
		}
		if (flag)
		{
			base.OnUpdateLive();
		}
		else
		{
			base.CheckSleeperTriggers();
			base.Stats.Tick(this.world.worldTime);
			this.m_vp_FPController.SpeedModifier = this.GetSpeedModifier() * Mathf.Clamp01(EffectManager.GetValue(PassiveEffects.Mobility, null, 1f, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false)) * (this.isMotionSlowedDown ? this.motionMultiplier : 1f);
			this.isMotionSlowedDown = false;
			base.updateCurrentBlockPosAndValue();
			if (this.canEntityMove())
			{
				this.MoveEntityHeaded(this.moveDirection, false);
			}
			base.checkForTeleportOutOfTraderArea();
		}
		if (this.challengeJournal != null)
		{
			this.challengeJournal.Update(this.world);
		}
		if (this.QuestJournal != null)
		{
			this.QuestJournal.Update(this.world.WorldDay);
		}
		if (!this.isEntityRemote)
		{
			this.updateBlockRadiusEffects();
		}
		int num5 = this.inventorySendCounter - 1;
		this.inventorySendCounter = num5;
		if (num5 <= 0)
		{
			this.inventorySendCounter = 40;
			this.ResendPlayerInventory();
		}
		if (this.Stamina <= 0f)
		{
			this.bExhausted = true;
			this.Stamina = 0f;
		}
		float stamina = this.Stamina;
		if (this.bExhausted && stamina > base.Stats.Stamina.Max * 0.2f)
		{
			this.isExhaustedSoundAllowed = true;
			this.bExhausted = false;
		}
		if (this.bExhausted && this.isExhaustedSoundAllowed)
		{
			this.PlayOneShot(this.GetSoundStamina(), false, false, false, null);
			GameManager.ShowTooltip(this, "ttOutOfStamina", false, false, 0f);
			this.isExhaustedSoundAllowed = false;
		}
		if (this.prevStaminaValue >= stamina - 0.1f && this.moveState == EntityPlayerLocal.MoveState.Run && !flag2)
		{
			this.runTicks++;
			if (this.runTicks > 100 && stamina / base.Stats.Stamina.Max < 0.5f && this.sprintLoopSoundPlayId == -1 && !this.IsDead())
			{
				Manager.BroadcastPlay(this, "Player" + (this.IsMale ? "Male" : "Female") + "RunLoop", false);
				this.sprintLoopSoundPlayId = 0;
			}
			this.lerpCameraFastFOV += Time.deltaTime * Constants.cRunningFOVSpeedUp;
		}
		else
		{
			if (this.sprintLoopSoundPlayId != -1)
			{
				Manager.BroadcastStop(this.entityId, "Player" + (this.IsMale ? "Male" : "Female") + "RunLoop");
				this.sprintLoopSoundPlayId = -1;
				if (this.isExhaustedSoundAllowed)
				{
					this.PlayOneShot(this.GetSoundStamina(), false, false, false, null);
				}
			}
			this.lerpCameraFastFOV -= Time.deltaTime * Constants.cRunningFOVSpeedDown;
			this.runTicks = 0;
		}
		this.lerpCameraFastFOV = Mathf.Clamp01(this.lerpCameraFastFOV);
		this.prevStaminaValue = stamina;
		if (this.playerUI != null && this.playerUI.windowManager.IsModalWindowOpen())
		{
			this.TryCancelChargedAction();
		}
	}

	// Token: 0x0600239F RID: 9119 RVA: 0x000E1178 File Offset: 0x000DF378
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void HandleLootStageMaxCheck()
	{
		base.HandleLootStageMaxCheck();
		if (this.biomeStandingOn == null)
		{
			return;
		}
		if (GameStats.GetBool(EnumGameStats.BiomeProgression))
		{
			int lootStage = base.GetLootStage(0f, 0f);
			int num = Mathf.FloorToInt(EffectManager.GetValue(PassiveEffects.LootStageMax, null, (float)this.biomeStandingOn.LootStageMax, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false));
			if (lootStage >= num)
			{
				if (!this.LootAtMax || this.lastLootStage == -1)
				{
					this.FireEvent(MinEventTypes.onSelfBiomeLootStageMaxEntered, true);
					this.LootAtMax = true;
				}
			}
			else if (lootStage < num && (this.LootAtMax || this.lastLootStage == -1))
			{
				this.FireEvent(MinEventTypes.onSelfBiomeLootStageMaxExited, true);
				this.LootAtMax = false;
			}
			this.lastLootStage = lootStage;
		}
	}

	// Token: 0x060023A0 RID: 9120 RVA: 0x000E1234 File Offset: 0x000DF434
	[PublicizedFrom(EAccessModifier.Private)]
	public bool canEntityMove()
	{
		bool result = true;
		if (!this.IsFlyMode.Value)
		{
			Chunk chunk = (Chunk)GameManager.Instance.World.GetChunkFromWorldPos((int)(this.position.x + this.moveDirection.x * 2f), (int)this.position.y, (int)(this.position.z + this.moveDirection.z * 2f));
			if (chunk == null || !chunk.IsCollisionMeshGenerated)
			{
				result = false;
			}
		}
		return result;
	}

	// Token: 0x060023A1 RID: 9121 RVA: 0x000E12C0 File Offset: 0x000DF4C0
	[PublicizedFrom(EAccessModifier.Private)]
	public bool shouldPushOutOfBlock(int _x, int _y, int _z, bool pushOutOfTerrain)
	{
		BlockShape shape = this.world.GetBlock(_x, _y, _z).Block.shape;
		if (shape.IsSolidSpace && !shape.IsTerrain())
		{
			return true;
		}
		if (pushOutOfTerrain && shape.IsSolidSpace && shape.IsTerrain())
		{
			BlockShape shape2 = this.world.GetBlock(_x, _y + 1, _z).Block.shape;
			if (shape2.IsSolidSpace && shape2.IsTerrain())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060023A2 RID: 9122 RVA: 0x000E1340 File Offset: 0x000DF540
	[PublicizedFrom(EAccessModifier.Private)]
	public bool pushOutOfBlocks(float _x, float _y, float _z)
	{
		int num = Utils.Fastfloor(_x);
		int num2 = Utils.Fastfloor(_y);
		int num3 = Utils.Fastfloor(_z);
		float num4 = _x - (float)num;
		float num5 = _z - (float)num3;
		bool result = false;
		bool flag = base.IsCrouching || this.IsMoveStateCrouch();
		if (this.shouldPushOutOfBlock(num, num2, num3, false) || (!flag && this.shouldPushOutOfBlock(num, num2 + 1, num3, false)))
		{
			bool flag2 = !this.shouldPushOutOfBlock(num - 1, num2, num3, true) && !this.shouldPushOutOfBlock(num - 1, num2 + 1, num3, true);
			bool flag3 = !this.shouldPushOutOfBlock(num + 1, num2, num3, true) && !this.shouldPushOutOfBlock(num + 1, num2 + 1, num3, true);
			bool flag4 = !this.shouldPushOutOfBlock(num, num2, num3 - 1, true) && !this.shouldPushOutOfBlock(num, num2 + 1, num3 - 1, true);
			bool flag5 = !this.shouldPushOutOfBlock(num, num2, num3 + 1, true) && !this.shouldPushOutOfBlock(num, num2 + 1, num3 + 1, true);
			byte b = byte.MaxValue;
			float num6 = 9999f;
			if (flag2 && num4 < num6)
			{
				num6 = num4;
				b = 0;
			}
			if (flag3 && 1.0 - (double)num4 < (double)num6)
			{
				num6 = 1f - num4;
				b = 1;
			}
			if (flag4 && num5 < num6)
			{
				num6 = num5;
				b = 4;
			}
			if (flag5 && 1f - num5 < num6)
			{
				b = 5;
			}
			float num7 = 0.1f;
			if (b == 0)
			{
				this.motion.x = -num7;
			}
			if (b == 1)
			{
				this.motion.x = num7;
			}
			if (b == 4)
			{
				this.motion.z = -num7;
			}
			if (b == 5)
			{
				this.motion.z = num7;
			}
			if (b != 255)
			{
				result = true;
			}
		}
		return result;
	}

	// Token: 0x060023A3 RID: 9123 RVA: 0x000E14F3 File Offset: 0x000DF6F3
	public override void Move(Vector3 _direction, bool _isDirAbsolute, float _velocity, float _maxVelocity)
	{
		base.Move(_direction, _isDirAbsolute, _velocity, _maxVelocity);
	}

	// Token: 0x060023A4 RID: 9124 RVA: 0x000E1500 File Offset: 0x000DF700
	public override void SetAlive()
	{
		base.SetAlive();
		if (this.PhysicsTransform != null)
		{
			this.PhysicsTransform.gameObject.layer = 20;
		}
		if (this.m_vp_FPController != null)
		{
			this.m_vp_FPController.Player.Dead.Stop(0f);
		}
		this.SetModelLayer(24, false, null);
		this.ShowHoldingItemLayer(true);
		this.bPlayerStatsChanged = true;
	}

	// Token: 0x060023A5 RID: 9125 RVA: 0x000E1574 File Offset: 0x000DF774
	[PublicizedFrom(EAccessModifier.Protected)]
	public void LateUpdate()
	{
		if (this.bLerpCameraFlag)
		{
			this.lerpCameraLerpValue += Time.deltaTime * 4f;
			if (this.lerpCameraLerpValue >= 1f)
			{
				this.bLerpCameraFlag = false;
				this.playerCamera.fieldOfView = this.lerpCameraEndFOV;
			}
			else
			{
				this.playerCamera.fieldOfView = Mathf.Lerp(this.lerpCameraStartFOV, this.lerpCameraEndFOV, this.lerpCameraLerpValue);
			}
		}
		if (!this.AimingGun)
		{
			float num = (float)this.GetCameraFOV();
			float a = this.playerCamera.fieldOfView;
			if (this.lerpCameraLerpValue <= 0f || !this.bLerpCameraFlag)
			{
				a = num;
			}
			this.playerCamera.fieldOfView = Mathf.Lerp(a, num * Constants.cRunningFOVMultiplier, this.lerpCameraFastFOV);
			if (this.OverrideFOV != -1f)
			{
				this.playerCamera.fieldOfView = this.OverrideFOV;
				this.playerCamera.transform.LookAt(this.OverrideLookAt - Origin.position);
			}
			else if (this.lastOverrideFOV != -1f)
			{
				this.playerCamera.fieldOfView = num;
			}
			this.lastOverrideFOV = this.OverrideFOV;
		}
		this.WorldBoundsUpdate();
	}

	// Token: 0x060023A6 RID: 9126 RVA: 0x000E16AC File Offset: 0x000DF8AC
	[PublicizedFrom(EAccessModifier.Private)]
	public void WorldBoundsUpdate()
	{
		this.InWorldPercent = this.world.InBoundsForPlayersPercent(this.position);
		this.InWorldLookPercent = this.InWorldPercent;
		if (this.InWorldPercent < 1f && !this.AttachedToEntity && !this.IsFlyMode.Value)
		{
			Vector3 forward = this.playerCamera.transform.forward;
			this.InWorldLookPercent = this.world.InBoundsForPlayersPercent(this.position + forward * 80f * 0.3f);
			if (this.InWorldPercent <= 0.05f)
			{
				Vector3i vector3i;
				Vector3i vector3i2;
				this.world.GetWorldExtent(out vector3i, out vector3i2);
				Vector2 vector;
				vector.x = (float)(vector3i.x + vector3i2.x) * 0.5f;
				vector.y = (float)(vector3i.z + vector3i2.z) * 0.5f;
				Vector3 normalized = new Vector3(vector.x - this.position.x, 0f, vector.y - this.position.z).normalized;
				float num = (1f - this.InWorldPercent / 0.05f) * 0.8f * Time.deltaTime;
				this.m_vp_FPController.AddForce(normalized.x * num, 0f, normalized.z * num);
				GameManager.ShowTooltip(this, Localization.Get("ttWorldEnd", false), false, false, 0f);
			}
		}
	}

	// Token: 0x060023A7 RID: 9127 RVA: 0x000E183C File Offset: 0x000DFA3C
	public void UnderwaterCameraFrameUpdate()
	{
		bool flag = this.UnderwaterCameraCheck();
		if (this.IsUnderwaterCamera != flag)
		{
			this.IsUnderwaterCamera = flag;
			Shader.SetGlobalFloat("_UnderWater", (float)(flag ? 1 : 0));
			if (this.uwEffectHaze)
			{
				this.uwEffectHaze.gameObject.SetActive(flag);
			}
			if (this.uwEffectRefract)
			{
				this.uwEffectRefract.gameObject.SetActive(flag);
			}
			if (this.uwEffectDebris)
			{
				this.uwEffectDebris.gameObject.SetActive(flag);
			}
			if (!flag)
			{
				if (this.uwEffectDroplets)
				{
					this.uwEffectDroplets.gameObject.SetActive(true);
					this.uwEffectDroplets.GetComponent<ParticleSystem>().GetComponent<Renderer>().enabled = true;
					this.uwEffectDroplets.GetComponent<ParticleSystem>().Play();
					this.uwEffectDroplets.GetComponent<ParticleSystem>().Emit(this.rand.RandomRange(60, 120));
				}
				if (this.uwEffectWaterFade)
				{
					this.uwEffectWaterFade.gameObject.SetActive(true);
					this.uwEffectWaterFade.GetComponent<ParticleSystem>().GetComponent<Renderer>().enabled = true;
					this.uwEffectWaterFade.GetComponent<ParticleSystem>().Play();
					this.uwEffectWaterFade.GetComponent<ParticleSystem>().Emit(1);
				}
			}
		}
	}

	// Token: 0x060023A8 RID: 9128 RVA: 0x000E1990 File Offset: 0x000DFB90
	public bool UnderwaterCameraCheck()
	{
		Vector3 pos = this.cameraTransform.position + Origin.position;
		pos.y += 0.28f;
		if (this.UnderwaterCameraCheckPos(pos))
		{
			return true;
		}
		Vector2 forwardVector = base.GetForwardVector2();
		pos.x -= forwardVector.x * 0.3f;
		pos.z -= forwardVector.y * 0.3f;
		return this.UnderwaterCameraCheckPos(pos);
	}

	// Token: 0x060023A9 RID: 9129 RVA: 0x000E1A10 File Offset: 0x000DFC10
	[PublicizedFrom(EAccessModifier.Private)]
	public bool UnderwaterCameraCheckPos(Vector3 pos)
	{
		Vector3i vector3i = World.worldToBlockPos(pos);
		float waterPercent = this.world.GetWaterPercent(vector3i);
		return waterPercent > 0f && (float)vector3i.y + waterPercent - pos.y > 0f;
	}

	// Token: 0x060023AA RID: 9130 RVA: 0x000E1A52 File Offset: 0x000DFC52
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool IsHeadUnderwater()
	{
		return this.inWaterPercent >= 0.791f;
	}

	// Token: 0x060023AB RID: 9131 RVA: 0x000E1A64 File Offset: 0x000DFC64
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnHeadUnderwaterStateChanged(bool _bUnderwater)
	{
		base.OnHeadUnderwaterStateChanged(_bUnderwater);
		if (_bUnderwater)
		{
			this.Buffs.SetCustomVar("_underwater", 1f, true, CVarOperation.set);
		}
		else if (!this.IsDead())
		{
			this.Buffs.SetCustomVar("_underwater", 0f, true, CVarOperation.set);
			if (this.soundWaterSurface != null && Time.time - this.lastTimeUnderwater > 3f)
			{
				Manager.BroadcastPlay(this, this.soundWaterSurface, false);
			}
		}
		else
		{
			this.Buffs.SetCustomVar("_underwater", 0f, true, CVarOperation.set);
		}
		this.lastTimeUnderwater = Time.time;
	}

	// Token: 0x060023AC RID: 9132 RVA: 0x000E1AFF File Offset: 0x000DFCFF
	public override void SetDead()
	{
		base.SetDead();
		this.lastHitDirection = Utils.EnumHitDirection.None;
	}

	// Token: 0x060023AD RID: 9133 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Private)]
	public void CameraDOFInit()
	{
	}

	// Token: 0x060023AE RID: 9134 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Private)]
	public void CameraDOFFrameUpdate()
	{
	}

	// Token: 0x060023AF RID: 9135 RVA: 0x000E1B10 File Offset: 0x000DFD10
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayerLocal.HolsterState getCurrentHolsterState()
	{
		EntityPlayerLocal.HolsterState result = EntityPlayerLocal.HolsterState.Unholstered;
		AvatarMultiBodyController avatarMultiBodyController = this.emodel.avatarController as AvatarMultiBodyController;
		Animator animator = null;
		if (avatarMultiBodyController != null)
		{
			animator = avatarMultiBodyController.BodyAnimators[1].Animator;
		}
		if (avatarMultiBodyController != null && animator != null)
		{
			AnimatorClipInfo[] currentAnimatorClipInfo = animator.GetCurrentAnimatorClipInfo(1);
			if (currentAnimatorClipInfo.Length != 0)
			{
				result = EntityPlayerLocal.HolsterState.Undefined;
				foreach (AnimatorClipInfo animatorClipInfo in currentAnimatorClipInfo)
				{
					if (animatorClipInfo.clip.name == "FP_Unarmed_Additive_HolsteHold" && animatorClipInfo.weight == 1f)
					{
						result = EntityPlayerLocal.HolsterState.Holstered;
						break;
					}
				}
			}
		}
		else
		{
			result = this.lastHolsterState;
		}
		return result;
	}

	// Token: 0x060023B0 RID: 9136 RVA: 0x000E1BC4 File Offset: 0x000DFDC4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Update()
	{
		base.Update();
		if (this.desiredHolsterState != EntityPlayerLocal.HolsterState.Undefined)
		{
			EntityPlayerLocal.HolsterState currentHolsterState = this.getCurrentHolsterState();
			if (currentHolsterState != EntityPlayerLocal.HolsterState.Undefined && currentHolsterState != this.desiredHolsterState)
			{
				this.lastHolsterState = currentHolsterState;
				this.emodel.avatarController.UpdateBool("Holstered", this.desiredHolsterState == EntityPlayerLocal.HolsterState.Holstered, true);
				this.desiredHolsterState = EntityPlayerLocal.HolsterState.Undefined;
			}
		}
		this.renderManager.FrameUpdate();
		if (this.bPlayingSpawnIn)
		{
			WeatherManager.Instance.PushTransitions();
		}
		this.CameraDOFFrameUpdate();
		float time = Time.time;
		bool flag = this.IsDead();
		if (this.Spawned != this.wasSpawned)
		{
			if (!flag && this.lastRespawnReason != RespawnType.Teleport)
			{
				this.bPlayingSpawnIn = true;
				this.spawnInTime = time;
				Manager.PlayInsidePlayerHead("spawnInStinger", -1, 0f, false, false);
				this.HolsterWeapon(false);
			}
			this.wasSpawned = this.Spawned;
		}
		if (flag)
		{
			if (this.deathTime == 0f)
			{
				this.deathTime = time;
				Manager.PlayInsidePlayerHead("player_death_stinger", this.entityId);
			}
		}
		else if (this.deathTime > 0f)
		{
			this.ClearScreenEffects();
			this.deathTime = 0f;
		}
		float num = (float)this.Health;
		if (num < this.dyingEffectHealthLast - 3f)
		{
			this.dyingEffectCur = Mathf.Clamp01(1f - Mathf.Clamp(num, 0f, 70f) / 70f);
			this.dyingEffectHitTime = time;
		}
		this.dyingEffectHealthLast = num;
		this.dyingEffectCur *= Mathf.Clamp01(1f - (time - this.dyingEffectHitTime) / 600f);
		if (this.dyingEffectCur < 0.01f)
		{
			this.dyingEffectCur = 0f;
		}
		if (this.dyingEffectLast != this.dyingEffectCur)
		{
			this.dyingEffectLast = this.dyingEffectCur;
			if (!flag)
			{
				this.ScreenEffectManager.SetScreenEffect("Dying", this.dyingEffectCur, 0f);
			}
		}
		if (this.bPlayingSpawnIn && time < this.spawnInTime + EntityPlayerLocal.spawnInEffectSpeed)
		{
			this.spawnInIntensity = Mathf.Clamp01(1f - (time - this.spawnInTime) / EntityPlayerLocal.spawnInEffectSpeed);
			this.ScreenEffectManager.SetScreenEffect("VibrantDeSat", this.spawnInIntensity, 4f);
			this.bPlayingSpawnIn = (this.spawnInIntensity > 0f);
		}
		else if (this.spawnInIntensity > 0f)
		{
			this.spawnInIntensity = 0f;
			this.ScreenEffectManager.SetScreenEffect("VibrantDeSat", 0f, 4f);
			this.bPlayingSpawnIn = false;
		}
		if ((double)base.GetCVar("_underwater") > 0.1 && this.equipment.IsNaked() && this.biomeStandingOn != null && this.biomeStandingOn.m_sBiomeName == "snow")
		{
			IAchievementManager achievementManager = PlatformManager.NativePlatform.AchievementManager;
			if (achievementManager != null)
			{
				achievementManager.SetAchievementStat(EnumAchievementDataStat.SubZeroNakedSwim, 1);
			}
		}
		ProgressionValue progressionValue = this.Progression.GetProgressionValue("attFortitude");
		if (progressionValue != null)
		{
			IAchievementManager achievementManager2 = PlatformManager.NativePlatform.AchievementManager;
			if (achievementManager2 != null)
			{
				achievementManager2.SetAchievementStat(EnumAchievementDataStat.HighestFortitude, progressionValue.Level);
			}
		}
		IAchievementManager achievementManager3 = PlatformManager.NativePlatform.AchievementManager;
		if (achievementManager3 != null)
		{
			achievementManager3.SetAchievementStat(EnumAchievementDataStat.HighestGamestage, base.gameStage);
		}
		IAchievementManager achievementManager4 = PlatformManager.NativePlatform.AchievementManager;
		if (achievementManager4 != null)
		{
			achievementManager4.SetAchievementStat(EnumAchievementDataStat.HighestPlayerLevel, this.Progression.Level);
		}
		if (base.RentedVMPosition != Vector3i.zero && this.RentalEndDay <= GameUtils.WorldTimeToDays(GameManager.Instance.World.worldTime))
		{
			base.RentedVMPosition = Vector3i.zero;
			this.RentalEndTime = 0UL;
			this.RentalEndDay = 0;
		}
		this.sneakDamageBlendTimer.Tick(Time.deltaTime);
		this.ThreatLevel.Numeric = ThreatLevelUtility.GetThreatLevelOn(this);
		if (this.Spawned)
		{
			AudioListener.volume = Mathf.Lerp(AudioListener.volume, GamePrefs.GetFloat(EnumGamePrefs.OptionsOverallAudioVolumeLevel), Time.deltaTime);
		}
		float num2 = GamePrefs.GetFloat(EnumGamePrefs.OptionsAmbientVolumeLevel) * this.biomeVolume;
		if (this.audioSourceBiomeActive.isPlaying && Utils.FastAbs(this.audioSourceBiomeActive.volume - num2 * 0.95f) > 0.01f)
		{
			this.audioSourceBiomeActive.volume = Mathf.Lerp(this.audioSourceBiomeActive.volume, num2, Time.deltaTime);
			if (this.audioSourceBiomeActive.volume > num2 * 0.95f)
			{
				this.audioSourceBiomeActive.volume = num2;
			}
		}
		if (this.audioSourceBiomeFadeOut.isPlaying && this.audioSourceBiomeFadeOut.volume > 0.001f)
		{
			this.audioSourceBiomeFadeOut.volume = Mathf.Lerp(this.audioSourceBiomeFadeOut.volume, 0f, Time.deltaTime);
			if ((double)this.audioSourceBiomeFadeOut.volume < 0.05)
			{
				this.audioSourceBiomeFadeOut.clip = null;
				this.audioSourceBiomeFadeOut.Stop();
			}
		}
		this.FrameUpdateCamera();
		if (!GameManager.Instance.gameStateManager.IsGameStarted())
		{
			return;
		}
		this.ShelterFrameUpdate();
		if (this.emodel.IsRagdollActive || (this.IsDead() && this.bSwitchCameraBackAfterRespawn))
		{
			this.SelfCameraFrameUpdate();
		}
		if (this.inventory.IsHoldingGun() && this.inventory.GetHoldingGun() is ItemActionRanged)
		{
			float num3 = (float)Screen.width / this.cameraTransform.GetComponent<Camera>().fieldOfView;
			float num4 = EffectManager.GetValue(PassiveEffects.SpreadDegreesHorizontal, this.inventory.holdingItemData.itemValue, 90f, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) * num3;
			float num5 = EffectManager.GetValue(PassiveEffects.SpreadDegreesVertical, this.inventory.holdingItemData.itemValue, 90f, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) * num3;
			if (num4 > num5)
			{
				this.crossHairOpenArea = (float)((int)num4) * (this.inventory.holdingItemData.actionData[0] as ItemActionRanged.ItemActionDataRanged).lastAccuracy;
			}
			else
			{
				this.crossHairOpenArea = (float)((int)num5) * (this.inventory.holdingItemData.actionData[0] as ItemActionRanged.ItemActionDataRanged).lastAccuracy;
			}
		}
		else
		{
			Vector3 vector = this.prevPos - this.position;
			float b = Mathf.Max(20f, Mathf.Clamp01(Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z) * 3f) * 100f);
			this.crossHairOpenArea = Mathf.Lerp(this.crossHairOpenArea, b, Time.deltaTime * 4f);
		}
		if (this.autoMove != null)
		{
			this.autoMove.Update();
		}
		List<Vector3i> obj = this.backpackPositionsFromThread;
		lock (obj)
		{
			if (this.backpackPositionsFromThread.Count > 0)
			{
				this.SetDroppedBackpackPositions(this.backpackPositionsFromThread);
				this.backpackPositionsFromThread.Clear();
			}
		}
		if (base.IsAlive())
		{
			this.recoveryPointTimer += Time.deltaTime;
			if (this.recoveryPointTimer > 30f && this.onGround)
			{
				this.TryAddRecoveryPosition(Vector3i.FromVector3Rounded(this.position));
				this.recoveryPointTimer = 0f;
			}
		}
	}

	// Token: 0x060023B1 RID: 9137 RVA: 0x000E231C File Offset: 0x000E051C
	public void HandleHordeEvent(AIDirector.HordeEvent msg)
	{
		string text = null;
		if (msg != AIDirector.HordeEvent.Warn2)
		{
			if (msg == AIDirector.HordeEvent.Spawn)
			{
				GameManager.Instance.StartCoroutine(this.shakeCamera(Vector3.one, 1f, 50f, 5f));
				text = "Enemies/Horde/horde_spawn";
			}
		}
		else
		{
			text = "Enemies/Horde/horde_spawn_warning";
		}
		if (text != null)
		{
			Manager.PlayInsidePlayerHead(text, -1, 0f, false, false);
		}
	}

	// Token: 0x060023B2 RID: 9138 RVA: 0x000E237C File Offset: 0x000E057C
	public override void OnFired()
	{
		base.OnFired();
		Vector2 vector = new Vector2(EffectManager.GetValue(PassiveEffects.KickDegreesHorizontalMin, this.inventory.holdingItemItemValue, 0f, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false), EffectManager.GetValue(PassiveEffects.KickDegreesHorizontalMax, this.inventory.holdingItemItemValue, 0f, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false));
		Vector2 vector2 = new Vector2(EffectManager.GetValue(PassiveEffects.KickDegreesVerticalMin, this.inventory.holdingItemItemValue, 0f, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false), EffectManager.GetValue(PassiveEffects.KickDegreesVerticalMax, this.inventory.holdingItemItemValue, 0f, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false));
		if (vector.x != 0f || vector.y != 0f || vector2.x != 0f || vector2.y != 0f)
		{
			switch (this.inventory.holdingItem.GetCameraShakeType(this.inventory.holdingItemData))
			{
			case EnumCameraShake.Tiny:
				GameManager.Instance.StartCoroutine(this.shakeCamera(Vector3.one, 0.5f, 5f, 1f));
				break;
			case EnumCameraShake.Small:
				GameManager.Instance.StartCoroutine(this.shakeCamera(Vector3.one, 0.5f, 10f, 1f));
				break;
			case EnumCameraShake.Big:
				GameManager.Instance.StartCoroutine(this.shakeCamera(Vector3.one, 0.5f, 20f, 1f));
				break;
			}
		}
		TriggerEffectManager.ControllerTriggerEffect controllerTriggerEffectShoot = this.inventory.holdingItem.GetControllerTriggerEffectShoot();
		if (controllerTriggerEffectShoot.XboxTriggerEffect.Effect != TriggerEffectManager.EffectXbox.Off || controllerTriggerEffectShoot.DualsenseEffect.Effect != TriggerEffectManager.EffectDualsense.Off)
		{
			GameManager.Instance.triggerEffectManager.SetTriggerEffect(TriggerEffectManager.GamepadTrigger.RightTrigger, controllerTriggerEffectShoot, false);
		}
		if (this.bFirstPersonView)
		{
			if (!this.AimingGun)
			{
				MovementInput movementInput = this.movementInput;
				movementInput.rotation.x = movementInput.rotation.x + this.rand.RandomRange(vector2.x, vector2.y) * 2f;
				MovementInput movementInput2 = this.movementInput;
				movementInput2.rotation.y = movementInput2.rotation.y + this.rand.RandomRange(vector.x, vector.y) * 2f;
				return;
			}
			MovementInput movementInput3 = this.movementInput;
			movementInput3.rotation.x = movementInput3.rotation.x + this.rand.RandomRange(vector2.x, vector2.y);
			MovementInput movementInput4 = this.movementInput;
			movementInput4.rotation.y = movementInput4.rotation.y + this.rand.RandomRange(vector.x, vector.y);
		}
	}

	// Token: 0x060023B3 RID: 9139 RVA: 0x000E262C File Offset: 0x000E082C
	public int GetCrosshairOpenArea()
	{
		return (int)this.crossHairOpenArea;
	}

	// Token: 0x060023B4 RID: 9140 RVA: 0x000E2638 File Offset: 0x000E0838
	public Vector2 GetCrosshairPosition2D()
	{
		Vector3 vector = this.finalCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0f));
		return new Vector2(vector.x, vector.y);
	}

	// Token: 0x060023B5 RID: 9141 RVA: 0x000E2678 File Offset: 0x000E0878
	public Vector3 GetCrosshairPosition3D(float _z, float _attributeOffset2D, Vector3 _altStartPosition)
	{
		if (!this.playerCamera.enabled || this.movementInput.bCameraChange)
		{
			return _altStartPosition;
		}
		Vector2 crosshairPosition2D = this.GetCrosshairPosition2D();
		crosshairPosition2D.x += this.rand.RandomRange(-_attributeOffset2D, _attributeOffset2D) * 700f;
		crosshairPosition2D.y += this.rand.RandomRange(-_attributeOffset2D, _attributeOffset2D) * 700f;
		Vector3 vector = this.playerCamera.ScreenToWorldPoint(new Vector3(crosshairPosition2D.x, crosshairPosition2D.y, _z)) + Origin.position;
		if (!this.bFirstPersonView)
		{
			vector += this.GetLookVector() * (0.94f + this.movementInput.cameraDistance);
		}
		return vector;
	}

	// Token: 0x060023B6 RID: 9142 RVA: 0x000E273A File Offset: 0x000E093A
	public override void OnHoldingItemChanged()
	{
		if (!this.IsDead())
		{
			this.SetModelLayer(24, false, null);
		}
	}

	// Token: 0x060023B7 RID: 9143 RVA: 0x000E2750 File Offset: 0x000E0950
	public override void SetModelLayer(int _layerId, bool force = false, string[] excludeTags = null)
	{
		if (this.emodel == null)
		{
			return;
		}
		Transform modelTransform = this.emodel.GetModelTransform();
		if (modelTransform == null)
		{
			return;
		}
		if (this.oldLayer != _layerId || force)
		{
			this.oldLayer = _layerId;
			Utils.SetLayerWithExclusionList(modelTransform.gameObject, _layerId, excludeTags);
			if (_layerId == 24 && modelTransform.childCount > 0)
			{
				Utils.SetLayerWithExclusionList(modelTransform.GetChild(0).gameObject, _layerId, excludeTags);
			}
			modelTransform.gameObject.GetComponentsInChildren<Collider>(true, EntityPlayerLocal.setLayerRecursivelyList);
			for (int i = EntityPlayerLocal.setLayerRecursivelyList.Count - 1; i >= 0; i--)
			{
				Utils.SetLayerWithExclusionList(EntityPlayerLocal.setLayerRecursivelyList[i].gameObject, _layerId, excludeTags);
			}
			EntityPlayerLocal.setLayerRecursivelyList.Clear();
		}
	}

	// Token: 0x060023B8 RID: 9144 RVA: 0x000E2810 File Offset: 0x000E0A10
	public virtual void MoveByInput()
	{
		bool isCrouching = base.IsCrouching;
		if (this.IsStuck || EffectManager.GetValue(PassiveEffects.DisableMovement, null, 0f, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) > 0f)
		{
			this.movementInput.Clear();
		}
		if (EffectManager.GetValue(PassiveEffects.FlipControls, null, 0f, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) > 0f)
		{
			this.movementInput.moveForward *= -1f;
			this.movementInput.moveStrafe *= -1f;
		}
		if (this.AttachedToEntity != null)
		{
			this.Crouching = false;
			this.CrouchingLocked = false;
			base.Climbing = false;
			this.MovementRunning = false;
			this.AimingGun = false;
			this.AttachedToEntity.MoveByAttachedEntity(this);
		}
		else
		{
			bool flag = false;
			this.moveDirection.x = this.movementInput.moveStrafe;
			this.moveDirection.z = this.movementInput.moveForward;
			if (this.moveDirection.x != 0f || this.moveDirection.z != 0f)
			{
				flag = true;
			}
			bool flag2 = (!base.IsSwimming()) ? (!this.bExhausted && this.moveDirection.z > 0f) : (this.swimExhaustedTicks == 0);
			bool flag3 = this.movementInput.running && flag;
			if (!this.IsFlyMode.Value)
			{
				flag3 = (flag3 && flag2);
			}
			this.MovementRunning = flag3;
			if (base.IsSwimming())
			{
				if (!this.IsSwimmingMoving() || this.swimExhaustedTicks > 0)
				{
					this.CurrentMovementTag = EntityAlive.MovementTagFloating;
				}
				else if (!this.MovementRunning)
				{
					this.CurrentMovementTag = EntityAlive.MovementTagSwimming;
				}
				else
				{
					this.CurrentMovementTag = EntityAlive.MovementTagSwimmingRun;
				}
			}
			else if (flag)
			{
				if (!this.MovementRunning)
				{
					this.CurrentMovementTag = EntityAlive.MovementTagWalking;
				}
				else
				{
					this.CurrentMovementTag = EntityAlive.MovementTagRunning;
				}
			}
			else
			{
				this.CurrentMovementTag = EntityAlive.MovementTagIdle;
			}
			if (this.movementInput.downToggle)
			{
				this.CrouchingLocked = !this.CrouchingLocked;
			}
			this.CrouchingLocked = (this.CrouchingLocked && !this.isLadderAttached && !this.movementInput.down);
			this.Crouching = (!this.IsFlyMode.Value && !this.isLadderAttached && (this.movementInput.down || this.CrouchingLocked));
			if (!this.AimingGun)
			{
				if (!this.IsFlyMode.Value)
				{
					if (!this.JetpackWearing)
					{
						if (this.movementInput.jump && this.vp_FPController && !this.inputWasJump)
						{
							this.vp_FPController.enabled = true;
						}
						if (!this.Jumping && !this.wasJumping && this.movementInput.jump && (this.onGround || this.isLadderAttached) && this.AttachedToEntity == null)
						{
							this.jumpTrigger = true;
						}
						else if (this.wasLadderAttachedJump && !this.isLadderAttached && this.movementInput.jump && !this.inputWasJump)
						{
							this.canLadderAirAttach = true;
						}
					}
					else
					{
						if (this.movementInput.jump)
						{
							this.motion.y = this.motion.y + 0.15f;
							flag = true;
						}
						if (this.movementInput.down)
						{
							this.motion.y = this.motion.y - 0.15f;
						}
					}
				}
				else
				{
					if (this.movementInput.jump)
					{
						if (this.movementInput.running)
						{
							this.motion.y = 0.9f;
						}
						else
						{
							this.motion.y = 0.3f * this.GodModeSpeedModifier;
						}
					}
					if (this.movementInput.down)
					{
						if (this.movementInput.running)
						{
							this.motion.y = -0.9f;
						}
						else
						{
							this.motion.y = -0.3f * this.GodModeSpeedModifier;
						}
					}
				}
			}
			this.JetpackActive = (this.JetpackWearing && flag);
		}
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			bool isCrouching2 = base.IsCrouching;
			if (isCrouching2 != isCrouching)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEntityStealth>().Setup(this, isCrouching2), false);
			}
		}
		if (this.vp_FPController != null)
		{
			if (this.AttachedToEntity == null)
			{
				this.vp_FPController.Player.InputMoveVector.Set(new Vector2(this.moveDirection.x, this.moveDirection.z));
			}
			this.vp_FPController.Player.InputSmoothLook.Set(new Vector2(this.movementInput.rotation.y, -this.movementInput.rotation.x));
		}
		this.inputWasJump = this.movementInput.jump;
		this.inputWasDown = this.movementInput.down;
		this.movementInput.Clear();
	}

	// Token: 0x060023B9 RID: 9145 RVA: 0x000E2D44 File Offset: 0x000E0F44
	public void SwitchFirstPersonViewFromInput()
	{
		bool flag = !this.bFirstPersonView;
		if (!GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled))
		{
			if (this.bFirstPersonView)
			{
				return;
			}
			flag = true;
		}
		this.SetFirstPersonView(flag, true);
		if (!this.bFirstPersonView)
		{
			vp_FPCamera vp_FPCamera = this.vp_FPCamera;
			if (vp_FPCamera)
			{
				vp_FPCamera.m_Current3rdPersonBlend = 1f;
			}
		}
	}

	// Token: 0x060023BA RID: 9146 RVA: 0x000E2D99 File Offset: 0x000E0F99
	public void SwitchFirstPersonView(bool _bLerpPosition)
	{
		this.SetFirstPersonView(!this.bFirstPersonView, _bLerpPosition);
	}

	// Token: 0x060023BB RID: 9147 RVA: 0x000E2DAC File Offset: 0x000E0FAC
	public void SetFirstPersonView(bool _bFirstPersonView, bool _bLerpPosition)
	{
		this.bFirstPersonView = _bFirstPersonView;
		base.SetCVar(".IsFPV", (float)(_bFirstPersonView ? 1 : 0));
		this.FireEvent(MinEventTypes.onSelfChangedView, true);
		if (this.bFirstPersonView)
		{
			this.switchModelView(EnumEntityModelView.FirstPerson);
		}
		else
		{
			this.switchModelView(EnumEntityModelView.ThirdPerson);
		}
		this.UpdateCameraFOV(_bLerpPosition);
		this.refreshHolsterState();
	}

	// Token: 0x060023BC RID: 9148 RVA: 0x000E2E01 File Offset: 0x000E1001
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void switchModelView(EnumEntityModelView modelView)
	{
		base.switchModelView(modelView);
		this.SetModelLayer(24, true, null);
		if (this.vp_FPController != null)
		{
			this.vp_FPController.Player.IsFirstPerson.Set(modelView == EnumEntityModelView.FirstPerson);
		}
	}

	// Token: 0x060023BD RID: 9149 RVA: 0x000E2E40 File Offset: 0x000E1040
	public override void BeforePlayerRespawn(RespawnType _type)
	{
		base.BeforePlayerRespawn(_type);
		switch (_type)
		{
		case RespawnType.NewGame:
		case RespawnType.LoadedGame:
		case RespawnType.Teleport:
			break;
		case RespawnType.Died:
			for (int i = 0; i < this.overlayDirectionTime.Length; i++)
			{
				this.overlayDirectionTime[i] = 0f;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060023BE RID: 9150 RVA: 0x000E2E8C File Offset: 0x000E108C
	public override void AfterPlayerRespawn(RespawnType _type)
	{
		if (_type == RespawnType.Teleport)
		{
			this.RespawnTeleportTimeoutLogged = false;
		}
		base.AfterPlayerRespawn(_type);
		if (this.bSwitchCameraBackAfterRespawn)
		{
			this.bSwitchCameraBackAfterRespawn = false;
			this.m_vp_FPCamera.enabled = true;
			this.SetFirstPersonView(this.bPreferFirstPerson, true);
		}
		if (!GameManager.Instance.IsEditMode() && (_type == RespawnType.NewGame || _type == RespawnType.EnterMultiplayer))
		{
			this.emodel.avatarController.PlayPlayerFPRevive();
		}
		if (this.world.IsEditor() || GameModeCreative.TypeName.Equals(GamePrefs.GetString(EnumGamePrefs.GameMode)))
		{
			if (GameManager.Instance.IsEditMode() && !PrefabEditModeManager.Instance.IsActive())
			{
				SkyManager.SetFogDebug(0f, float.MinValue, float.MinValue);
				World world = GameManager.Instance.World;
				if (((world != null) ? world.BiomeAtmosphereEffects : null) != null)
				{
					GameManager.Instance.World.BiomeAtmosphereEffects.ForceDefault = true;
				}
			}
			if (this.Buffs != null && !this.Buffs.HasBuff("god"))
			{
				this.Buffs.AddBuff("god", -1, true, false, -1f);
			}
		}
		switch (_type)
		{
		case RespawnType.NewGame:
		case RespawnType.EnterMultiplayer:
			this.SetAlive();
			this.Score = 0;
			if (this.world.IsEditor())
			{
				this.inventory.SetItem(1, new ItemValue(1, false), 64, true);
				this.inventory.SetHoldingItemIdx(0);
			}
			else
			{
				this.SetupStartingItems();
			}
			this.Buffs.UnPauseAll();
			this.FireEvent(MinEventTypes.onSelfFirstSpawn, true);
			this.FireEvent(MinEventTypes.onSelfEnteredGame, true);
			this.SetFirstPersonView(this.bPreferFirstPerson, false);
			return;
		case RespawnType.LoadedGame:
		case RespawnType.JoinMultiplayer:
			this.FireEvent(MinEventTypes.onSelfEnteredGame, true);
			this.HandleMapObjects(true);
			this.SetFirstPersonView(this.bPreferFirstPerson, false);
			return;
		case RespawnType.Died:
			this.SetAlive();
			this.Health = this.GetMaxHealth();
			this.Stamina = (float)this.GetMaxStamina();
			this.Water = (float)this.GetMaxWater();
			base.Stats.Stamina.MaxModifier = 0f;
			this.CrouchingLocked = (this.Crouching = false);
			this.FireEvent(MinEventTypes.onSelfRespawn, true);
			Manager.StopLoopInsidePlayerHead("player_death_stinger_lp", this.entityId);
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				Manager.Instance.StopDistantLoopingPositionalSounds(base.transform.position);
				return;
			}
			break;
		case RespawnType.Teleport:
			this.FireEvent(MinEventTypes.onSelfTeleported, true);
			break;
		default:
			return;
		}
	}

	// Token: 0x060023BF RID: 9151 RVA: 0x000E30E6 File Offset: 0x000E12E6
	public void RequestUnstuck()
	{
		if (!this.unstuckRequested)
		{
			ThreadManager.StartCoroutine(this.<RequestUnstuck>g__RequestUnstuckCo|265_0());
		}
	}

	// Token: 0x060023C0 RID: 9152 RVA: 0x000E30FC File Offset: 0x000E12FC
	public void SetupStartingItems()
	{
		for (int i = 0; i < this.itemsOnEnterGame.Count; i++)
		{
			ItemStack itemStack = this.itemsOnEnterGame[i];
			itemStack.itemValue.Meta = ItemClass.GetForId(itemStack.itemValue.type).GetInitialMetadata(itemStack.itemValue);
			this.inventory.SetItem(i + 1, itemStack);
		}
		this.inventory.SetHoldingItemIdx(0);
	}

	// Token: 0x060023C1 RID: 9153 RVA: 0x000E3170 File Offset: 0x000E1370
	public override Vector3 GetCameraLook(float _t)
	{
		if (!this.bFirstPersonView)
		{
			return this.cameraTransform.forward.normalized;
		}
		return base.GetCameraLook(_t);
	}

	// Token: 0x060023C2 RID: 9154 RVA: 0x000E31A0 File Offset: 0x000E13A0
	public override Ray GetLookRay()
	{
		Ray result = this.playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
		result.origin += Origin.position;
		if (this.bFirstPersonView)
		{
			result.direction += this.playerCamera.transform.up * 0.0001f;
			return result;
		}
		result.origin += result.direction * (0.94f + this.movementInput.cameraDistance);
		result.direction += this.playerCamera.transform.up * 0.0001f;
		return result;
	}

	// Token: 0x060023C3 RID: 9155 RVA: 0x000E3276 File Offset: 0x000E1476
	public override Vector3 GetLookVector()
	{
		if (this.playerCamera.enabled)
		{
			return this.cameraTransform.forward;
		}
		return base.GetLookVector();
	}

	// Token: 0x060023C4 RID: 9156 RVA: 0x000E3297 File Offset: 0x000E1497
	public override Vector3 GetLookVector(Vector3 _altLookVector)
	{
		if (!this.playerCamera.enabled)
		{
			return _altLookVector;
		}
		return this.GetLookVector();
	}

	// Token: 0x060023C5 RID: 9157 RVA: 0x00002914 File Offset: 0x00000B14
	public static void CheckPos()
	{
	}

	// Token: 0x060023C6 RID: 9158 RVA: 0x000E32B0 File Offset: 0x000E14B0
	[PublicizedFrom(EAccessModifier.Private)]
	public void startDeathCamera()
	{
		if (this.bFirstPersonView)
		{
			this.SetFirstPersonView(false, true);
		}
		this.bSwitchCameraBackAfterRespawn = true;
		this.StartSelfCamera();
		this.selfCameraSeekPos = this.selfCameraPos - this.cameraTransform.forward * 2.8f;
		this.selfCameraSeekPos.y = this.selfCameraSeekPos.y + 2.2f;
		this.ScreenEffectManager.SetScreenEffect("Dying", 0.5f, 0.5f);
		this.ScreenEffectManager.SetScreenEffect("Dead", 1f, 0.5f);
		this.ScreenEffectManager.SetScreenEffect("FadeToBlack", 1f, (float)base.GetTimeStayAfterDeath() * 0.05f);
	}

	// Token: 0x060023C7 RID: 9159 RVA: 0x000E336A File Offset: 0x000E156A
	[PublicizedFrom(EAccessModifier.Private)]
	public void StartSelfCamera()
	{
		this.selfCameraPos = this.cameraTransform.position + Origin.position;
		this.m_vp_FPCamera.enabled = false;
	}

	// Token: 0x060023C8 RID: 9160 RVA: 0x000E3394 File Offset: 0x000E1594
	[PublicizedFrom(EAccessModifier.Private)]
	public void SelfCameraFrameUpdate()
	{
		Vector3 vector = this.emodel.GetChestPosition();
		vector.y += 0.2f;
		if (this.selfCameraSeekPos.y < vector.y + 0.1f)
		{
			this.selfCameraSeekPos.y = this.selfCameraSeekPos.y + 0.05f;
		}
		this.selfCameraPos = Vector3.MoveTowards(this.selfCameraPos, this.selfCameraSeekPos, Time.deltaTime * 2.5f);
		Vector3 direction = this.selfCameraPos - vector;
		float magnitude = direction.magnitude;
		vector -= Origin.position;
		bool flag = false;
		float v = magnitude;
		float num = magnitude - 0.28f;
		RaycastHit raycastHit;
		if (num > 0f && Physics.SphereCast(vector, 0.28f, direction, out raycastHit, num, 65536))
		{
			v = Utils.FastMin(raycastHit.distance, v);
			flag = true;
		}
		if (!flag && Physics.Raycast(vector, direction, out raycastHit, magnitude + 0.28f, 65536))
		{
			v = raycastHit.distance - 0.28f;
			flag = true;
		}
		if (flag)
		{
			this.selfCameraPos = vector + direction.normalized * Utils.FastMax(0.01f, v) + Origin.position;
			this.selfCameraSeekPos = this.selfCameraPos;
		}
		Vector3 vector2 = this.selfCameraPos - Origin.position;
		this.cameraTransform.position = vector2;
		Quaternion b = Quaternion.LookRotation(vector - vector2);
		this.cameraTransform.rotation = Quaternion.Slerp(this.cameraTransform.rotation, b, 0.1f);
	}

	// Token: 0x060023C9 RID: 9161 RVA: 0x000E3525 File Offset: 0x000E1725
	[PublicizedFrom(EAccessModifier.Private)]
	public void ClearScreenEffects()
	{
		this.ScreenEffectManager.DisableScreenEffect("Dying");
		this.ScreenEffectManager.DisableScreenEffect("Dead");
		this.ScreenEffectManager.DisableScreenEffect("FadeToBlack");
	}

	// Token: 0x170003DE RID: 990
	// (get) Token: 0x060023CA RID: 9162 RVA: 0x000E3557 File Offset: 0x000E1757
	public bool CancellingInventoryActions
	{
		get
		{
			return this.cancellingInventoryActions;
		}
	}

	// Token: 0x060023CB RID: 9163 RVA: 0x000E355F File Offset: 0x000E175F
	public IEnumerator CancelInventoryActions(Action cancelCallback, bool holsterWeapon)
	{
		this.cancellingInventoryActions = true;
		if (this.inventory.holdingItem.Actions != null && this.inventory.holdingItem.Actions.Length != 0 && this.inventory.holdingItemData.actionData != null && this.inventory.holdingItemData.actionData.Count > 0)
		{
			for (int i = 0; i < this.inventory.holdingItem.Actions.Length; i++)
			{
				if (this.inventory.holdingItem.Actions[i] != null && this.inventory.holdingItemData.actionData[i] != null)
				{
					if (this.inventory.holdingItem.Actions[i].IsActionRunning(this.inventory.holdingItemData.actionData[i]))
					{
						this.inventory.holdingItem.Actions[i].CancelAction(this.inventory.holdingItemData.actionData[i]);
					}
					this.inventory.holdingItem.Actions[i].CancelReload(this.inventory.holdingItemData.actionData[i], holsterWeapon);
				}
			}
		}
		while (this.IsReloading())
		{
			yield return null;
		}
		for (;;)
		{
			EntityPlayerLocal.HolsterState currentHolsterState = this.getCurrentHolsterState();
			if (this.desiredHolsterState == EntityPlayerLocal.HolsterState.Undefined || this.desiredHolsterState == currentHolsterState)
			{
				break;
			}
			yield return null;
		}
		cancelCallback();
		this.cancellingInventoryActions = false;
		yield break;
	}

	// Token: 0x060023CC RID: 9164 RVA: 0x000E357C File Offset: 0x000E177C
	public bool IsReloading()
	{
		if (this.inventory.holdingItemData.actionData != null)
		{
			foreach (ItemActionData itemActionData in this.inventory.holdingItemData.actionData)
			{
				ItemActionRanged.ItemActionDataRanged itemActionDataRanged = itemActionData as ItemActionRanged.ItemActionDataRanged;
				if (itemActionDataRanged != null)
				{
					return ItemActionRanged.Reloading(itemActionDataRanged);
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x060023CD RID: 9165 RVA: 0x000E35F8 File Offset: 0x000E17F8
	public override void OnEntityDeath()
	{
		GameManager.Instance.TriggerSendOfLocalPlayerDataFile(0f);
		base.StartCoroutine(this.CancelInventoryActions(delegate
		{
		}, true));
		this.inventory.ReleaseAll(this.playerInput);
		this.inventory.SetActiveItemIndexOff();
		Manager.BroadcastStop(this.entityId, "Player" + (this.IsMale ? "Male" : "Female") + "RunLoop");
		this.sprintLoopSoundPlayId = -1;
		this.windowManager.CloseAllOpenWindows(null, false);
		this.windowManager.Close("windowpaging");
		GameManager.Instance.ClearTooltips(this.nguiWindowManager);
		this.windowManager.Open("death", false, false, false);
		this.AimingGun = false;
		this.BloodMoonParticipation = false;
		base.OnEntityDeath();
		this.startDeathCamera();
	}

	// Token: 0x060023CE RID: 9166 RVA: 0x000E36F0 File Offset: 0x000E18F0
	public override void OnDeathUpdate()
	{
		base.OnDeathUpdate();
		if (this.Spawned && base.GetDeathTime() >= base.GetTimeStayAfterDeath())
		{
			this.windowManager.Close("death");
			if (this.ShouldRemoveEquipmentOnDeath((EntityPlayerLocal.DropOption)GameStats.GetInt(EnumGameStats.DropOnDeath)))
			{
				for (int i = 0; i < 8; i++)
				{
					ItemValue slotItem = this.equipment.GetSlotItem(i);
					if (this.dropValueCondition(slotItem))
					{
						this.equipment.SetSlotItem(i, null, true);
					}
				}
			}
			this.Respawn(RespawnType.Died);
		}
	}

	// Token: 0x060023CF RID: 9167 RVA: 0x000E3774 File Offset: 0x000E1974
	[PublicizedFrom(EAccessModifier.Private)]
	public void FrameUpdateCamera()
	{
		this.UnderwaterCameraFrameUpdate();
		vp_FPCamera vp_FPCamera = this.m_vp_FPCamera;
		float z = -0.94f - this.movementInput.cameraDistance;
		vp_FPCamera.Position3rdPersonOffset = new Vector3(0.51f, 0.03f, z);
	}

	// Token: 0x060023D0 RID: 9168 RVA: 0x000E37B4 File Offset: 0x000E19B4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateCameraFOV(bool _bLerpPosition)
	{
		if (!this.playerCamera.enabled)
		{
			return;
		}
		if (!this.IsCameraAttachedToPlayerOrScope() && this.bFirstPersonView)
		{
			return;
		}
		if (this.AimingGun)
		{
			this.inventory.holdingItem.GetIronSights(this.inventory.holdingItemData, out this.lerpCameraEndFOV);
			if (this.lerpCameraEndFOV != 0f)
			{
				this.bLerpCameraFlag = _bLerpPosition;
				this.lerpCameraLerpValue = 0f;
				this.lerpCameraStartFOV = this.playerCamera.fieldOfView;
				return;
			}
		}
		else
		{
			if (this.bFirstPersonView && this.bSwitchTo3rdPersonAfterAiming)
			{
				this.bSwitchTo3rdPersonAfterAiming = false;
				this.SwitchFirstPersonView(true);
				return;
			}
			float fieldOfView = (float)GamePrefs.GetInt(EnumGamePrefs.OptionsGfxFOV);
			if (this.bFirstPersonView)
			{
				this.bLerpCameraFlag = _bLerpPosition;
				if (this.bLerpCameraFlag)
				{
					this.lerpCameraLerpValue = 0f;
					this.lerpCameraStartFOV = this.playerCamera.fieldOfView;
					this.lerpCameraEndFOV = fieldOfView;
					return;
				}
				this.playerCamera.fieldOfView = fieldOfView;
				return;
			}
			else
			{
				this.playerCamera.fieldOfView = fieldOfView;
			}
		}
	}

	// Token: 0x060023D1 RID: 9169 RVA: 0x000E38BC File Offset: 0x000E1ABC
	public override void PhysicsResume(Vector3 pos, float rotY)
	{
		Transform transform = this.cameraTransform;
		Vector3 position = transform.position;
		Quaternion rotation = transform.rotation;
		base.PhysicsResume(pos, rotY);
		transform.SetPositionAndRotation(position, rotation);
	}

	// Token: 0x060023D2 RID: 9170 RVA: 0x000E38EC File Offset: 0x000E1AEC
	public override void OnRagdoll(bool isActive)
	{
		base.OnRagdoll(isActive);
		if (isActive)
		{
			this.SetFirstPersonView(false, true);
			this.emodel.InitRigidBodies();
			this.StartSelfCamera();
			Vector3 forwardVector = base.GetForwardVector();
			this.selfCameraSeekPos = this.emodel.GetChestPosition();
			this.selfCameraSeekPos.x = this.selfCameraSeekPos.x - forwardVector.x * 2.2f;
			this.selfCameraSeekPos.y = this.selfCameraSeekPos.y + 1.2f;
			this.selfCameraSeekPos.z = this.selfCameraSeekPos.z - forwardVector.z * 2.2f;
			Transform transform = this.cameraTransform;
			transform.position = Vector3.MoveTowards(transform.position, this.selfCameraSeekPos, 1.2f);
			return;
		}
		this.SetRotation(this.rotation);
		this.m_vp_FPCamera.enabled = true;
		this.SetFirstPersonView(true, false);
	}

	// Token: 0x060023D3 RID: 9171 RVA: 0x000E39C3 File Offset: 0x000E1BC3
	public override int DamageEntity(DamageSource _damageSource, int _strength, bool _criticalHit, float impulseScale = 1f)
	{
		_strength = base.DamageEntity(_damageSource, _strength, _criticalHit, impulseScale);
		if (_strength > 0)
		{
			GameManager.Instance.StartCoroutine(this.shakeCamera(_damageSource.getDirection(), 0.5f, (float)(_strength * 4), 1f));
		}
		return _strength;
	}

	// Token: 0x060023D4 RID: 9172 RVA: 0x000E39FC File Offset: 0x000E1BFC
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator shakeCamera(Vector3 _direction, float time, float strength, float speed = 1f)
	{
		this.m_vp_FPCamera.ShakeSpeed2 = speed;
		this.m_vp_FPCamera.ShakeAmplitude2 = new Vector3(-_direction.x, _direction.y, 0f) * strength;
		yield return new WaitForSeconds(time);
		this.m_vp_FPCamera.ShakeSpeed2 = 0f;
		this.m_vp_FPCamera.ShakeAmplitude2 = Vector3.zero;
		yield break;
	}

	// Token: 0x060023D5 RID: 9173 RVA: 0x000E3A28 File Offset: 0x000E1C28
	public override void Kill(DamageResponse _dmResponse)
	{
		if (!this.IsDead())
		{
			base.Kill(_dmResponse);
			GameManager.Instance.StartCoroutine(this.shakeCamera(Vector3.one, 0.5f, 20f, 1f));
			if (this.m_vp_FPController != null)
			{
				this.m_vp_FPController.Player.Dead.Start(0f);
			}
		}
	}

	// Token: 0x060023D6 RID: 9174 RVA: 0x000E3A94 File Offset: 0x000E1C94
	public override void SetPosition(Vector3 _pos, bool _bUpdatePhysics = true)
	{
		base.SetPosition(_pos, _bUpdatePhysics);
		if (this.vp_FPController != null)
		{
			if (!this.emodel.IsRagdollActive)
			{
				this.vp_FPController.SetPosition(_pos - Origin.position);
			}
			this.vp_FPController.Stop();
			if (this.AttachedToEntity)
			{
				this.vp_FPController.Transform.localPosition = Vector3.zero;
			}
		}
		Manager.CameraChanged();
		Origin.Instance.UpdateLocalPlayer(this);
	}

	// Token: 0x060023D7 RID: 9175 RVA: 0x000E3B18 File Offset: 0x000E1D18
	public override void Respawn(RespawnType _reason)
	{
		Log.Out("Respawning: " + _reason.ToStringCached<RespawnType>());
		base.Respawn(_reason);
		if (_reason == RespawnType.Teleport)
		{
			this.RespawnTeleportDuration = Time.realtimeSinceStartup;
			this.RespawnTeleportTimeoutLogged = false;
		}
		this.moveController.Respawn(_reason);
		Shader.SetGlobalFloat("_UnderWater", 0f);
		this.SetControllable(false);
		if (this.vp_FPController != null && !this.AttachedToEntity)
		{
			this.vp_FPController.ResetState();
			this.vp_FPController.SetPosition(base.GetPosition() - Origin.position);
			this.vp_FPController.Stop();
			this.vp_FPCamera.Locked3rdPerson = false;
		}
		if (_reason == RespawnType.Teleport)
		{
			this.windowManager.CloseAllOpenWindows("map");
		}
		this.isFallDeath = false;
	}

	// Token: 0x060023D8 RID: 9176 RVA: 0x000E3BF0 File Offset: 0x000E1DF0
	public void TeleportToPosition(Vector3 _pos, bool _onlyIfNotFlying = false, Vector3? _viewDirection = null)
	{
		if (!_onlyIfNotFlying || !this.IsFlyMode.Value)
		{
			this.Teleport(_pos, (_viewDirection != null) ? _viewDirection.Value.y : float.MinValue);
			if (_pos.y >= 0f)
			{
				ThreadManager.StartCoroutine(this.setVerticalPosition(_pos, _viewDirection));
				return;
			}
			Log.Out("Teleported to {0}", new object[]
			{
				_pos.ToCultureInvariantString()
			});
		}
	}

	// Token: 0x060023D9 RID: 9177 RVA: 0x000E3C65 File Offset: 0x000E1E65
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator setVerticalPosition(Vector3 _pos, Vector3? _viewDirection)
	{
		while (!this.Spawned)
		{
			yield return null;
		}
		if (this.AttachedToEntity != null)
		{
			this.AttachedToEntity.SetPosition(_pos, true);
		}
		else
		{
			this.SetPosition(_pos, true);
		}
		if (_viewDirection != null)
		{
			this.SetRotation(_viewDirection.Value);
		}
		Log.Out("Teleported to {0}", new object[]
		{
			_pos.ToCultureInvariantString()
		});
		yield break;
	}

	// Token: 0x060023DA RID: 9178 RVA: 0x000E3C82 File Offset: 0x000E1E82
	public void SetControllable(bool _b)
	{
		if (_b && this.bIntroAnimActive)
		{
			return;
		}
		base.transform.GetComponent<PlayerMoveController>().SetControllableOverride(_b);
	}

	// Token: 0x060023DB RID: 9179 RVA: 0x000E3CA1 File Offset: 0x000E1EA1
	public void NotifySneakDamage(float multiplier)
	{
		this.sneakDamageText = string.Format(Localization.Get("sneakDamageBonus", false), multiplier.ToCultureInvariantString("f1")).ToUpper();
		this.sneakDamageBlendTimer.FadeIn();
	}

	// Token: 0x060023DC RID: 9180 RVA: 0x000E3CD4 File Offset: 0x000E1ED4
	public void NotifyDamageMultiplier(float multiplier)
	{
		this.sneakDamageText = string.Format(Localization.Get("stunnedDamageBonus", false), multiplier.ToCultureInvariantString("f1")).ToUpper();
		this.sneakDamageBlendTimer.FadeIn();
	}

	// Token: 0x060023DD RID: 9181 RVA: 0x000E3D07 File Offset: 0x000E1F07
	public override void EnableCamera(bool _b)
	{
		this.playerCamera.enabled = _b;
	}

	// Token: 0x060023DE RID: 9182 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsAttackValid()
	{
		return true;
	}

	// Token: 0x060023DF RID: 9183 RVA: 0x000E3D18 File Offset: 0x000E1F18
	public override bool IsAimingGunPossible()
	{
		return (this.inventory.holdingItem.Actions[0] == null || this.inventory.holdingItem.Actions[0].IsAimingGunPossible(this.inventory.holdingItemData.actionData[0])) && (this.inventory.holdingItem.Actions[1] == null || this.inventory.holdingItem.Actions[1].IsAimingGunPossible(this.inventory.holdingItemData.actionData[1]));
	}

	// Token: 0x060023E0 RID: 9184 RVA: 0x000B1953 File Offset: 0x000AFB53
	public override bool IsDrawMapIcon()
	{
		return this.IsSpawned();
	}

	// Token: 0x060023E1 RID: 9185 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsMapIconBlinking()
	{
		return true;
	}

	// Token: 0x060023E2 RID: 9186 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool CanMapIconBeSelected()
	{
		return false;
	}

	// Token: 0x060023E3 RID: 9187 RVA: 0x000B195B File Offset: 0x000AFB5B
	public override Color GetMapIconColor()
	{
		return Color.white;
	}

	// Token: 0x060023E4 RID: 9188 RVA: 0x000ADB75 File Offset: 0x000ABD75
	public override int GetLayerForMapIcon()
	{
		return 20;
	}

	// Token: 0x060023E5 RID: 9189 RVA: 0x000E3DB0 File Offset: 0x000E1FB0
	[PublicizedFrom(EAccessModifier.Private)]
	public bool BreakLeg(float chance)
	{
		if (this.rand.RandomFloat <= chance && this.Buffs.AddBuff("injuryBrokenLeg", -1, true, false, -1f) == EntityBuffs.BuffStatus.Added)
		{
			this.PlayOneShot("breakleg", false, false, false, null);
			IAchievementManager achievementManager = PlatformManager.NativePlatform.AchievementManager;
			if (achievementManager != null)
			{
				achievementManager.SetAchievementStat(EnumAchievementDataStat.LegBroken, 1);
			}
		}
		return false;
	}

	// Token: 0x060023E6 RID: 9190 RVA: 0x0000FB42 File Offset: 0x0000DD42
	[PublicizedFrom(EAccessModifier.Private)]
	public bool FractureLeg(float chance)
	{
		return false;
	}

	// Token: 0x060023E7 RID: 9191 RVA: 0x000E3E0D File Offset: 0x000E200D
	[PublicizedFrom(EAccessModifier.Private)]
	public bool SprainLeg(float chance)
	{
		return this.rand.RandomFloat <= chance && this.Buffs.AddBuff("injurySprainedLeg", -1, true, false, -1f) == EntityBuffs.BuffStatus.Added;
	}

	// Token: 0x060023E8 RID: 9192 RVA: 0x000E3E3A File Offset: 0x000E203A
	[PublicizedFrom(EAccessModifier.Private)]
	public bool HasBrokenLeg()
	{
		return this.Buffs.HasBuff("injuryBrokenLeg");
	}

	// Token: 0x060023E9 RID: 9193 RVA: 0x000E3E3A File Offset: 0x000E203A
	[PublicizedFrom(EAccessModifier.Private)]
	public bool HasFracturedLeg()
	{
		return this.Buffs.HasBuff("injuryBrokenLeg");
	}

	// Token: 0x060023EA RID: 9194 RVA: 0x000E3E4C File Offset: 0x000E204C
	[PublicizedFrom(EAccessModifier.Private)]
	public bool HasSprainedLeg()
	{
		return this.Buffs.HasBuff("injurySprainedLeg");
	}

	// Token: 0x060023EB RID: 9195 RVA: 0x000E3E5E File Offset: 0x000E205E
	public override float GetSpeedModifier()
	{
		if (!this.IsGodMode.Value && this.cameraTransform.parent != null)
		{
			return base.GetSpeedModifier();
		}
		return base.GetSpeedModifier() * this.GodModeSpeedModifier;
	}

	// Token: 0x060023EC RID: 9196 RVA: 0x000E3E94 File Offset: 0x000E2094
	[PublicizedFrom(EAccessModifier.Protected)]
	public void FallImpact(float speed)
	{
		if (this.IsGodMode.Value || this.AttachedToEntity != null)
		{
			return;
		}
		if (speed <= 0f)
		{
			return;
		}
		Vector3i pos = World.worldToBlockPos(this.vp_FPController.Transform.position + Origin.position);
		BlockValue block = this.world.GetBlock(pos);
		if (block.isair || block.Block.IsElevator((int)block.rotation))
		{
			pos.y--;
			block = this.world.GetBlock(pos);
		}
		float num = 1f;
		if (!block.isair)
		{
			num = block.Block.FallDamage;
			if (num <= 0f)
			{
				return;
			}
		}
		if (speed > 1f)
		{
			speed = 1f;
		}
		speed *= 1f;
		speed *= num;
		speed = EffectManager.GetValue(PassiveEffects.FallDamageReduction, this.inventory.holdingItemItemValue, speed, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		this.fallHealth = this.Health;
		base.SetCVar("_fallSpeed", speed);
		this.FireEvent(MinEventTypes.onSelfFallImpact, true);
	}

	// Token: 0x060023ED RID: 9197 RVA: 0x000E3FB5 File Offset: 0x000E21B5
	public override void BuffAdded(BuffValue _buff)
	{
		if (_buff.BuffClass.NameTag.Test_Bit(EntityAlive.FallingBuffTagBit) && this.fallHealth > 0 && this.Health <= 0)
		{
			this.isFallDeath = true;
		}
	}

	// Token: 0x060023EE RID: 9198 RVA: 0x000E3FE8 File Offset: 0x000E21E8
	[PublicizedFrom(EAccessModifier.Private)]
	public void ResendPlayerInventory()
	{
		if (this.world.aiDirector != null)
		{
			this.world.aiDirector.UpdatePlayerInventory(this);
			return;
		}
		AIDirectorPlayerInventory inventory = AIDirectorPlayerInventory.FromEntity(this);
		if (!inventory.Equals(this.xmitInventory))
		{
			this.xmitInventory = inventory;
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePlayerInventoryForAI>().Setup(this, inventory), false);
		}
	}

	// Token: 0x060023EF RID: 9199 RVA: 0x000E4048 File Offset: 0x000E2248
	public override void CopyPropertiesFromEntityClass()
	{
		base.CopyPropertiesFromEntityClass();
		EntityClass entityClass = EntityClass.list[this.entityClass];
		if (entityClass.Properties.Values.ContainsKey(EntityClass.PropDropInventoryBlock))
		{
			this.dropInventoryBlock = entityClass.Properties.Values[EntityClass.PropDropInventoryBlock];
		}
	}

	// Token: 0x060023F0 RID: 9200 RVA: 0x000E409E File Offset: 0x000E229E
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void dropItemOnDeath()
	{
		this.dropBackpack(true);
		this.inventory.SetFlashlight(false);
	}

	// Token: 0x060023F1 RID: 9201 RVA: 0x000E40B4 File Offset: 0x000E22B4
	public void dropItemOnQuit()
	{
		this.dropBackpack(false);
	}

	// Token: 0x060023F2 RID: 9202 RVA: 0x000E40C0 File Offset: 0x000E22C0
	public override void SetDroppedBackpackPositions(List<Vector3i> _positions)
	{
		if (!ThreadManager.IsMainThread())
		{
			List<Vector3i> obj = this.backpackPositionsFromThread;
			lock (obj)
			{
				this.backpackPositionsFromThread.Clear();
				if (_positions != null)
				{
					this.backpackPositionsFromThread.AddRange(_positions);
				}
			}
			return;
		}
		base.SetDroppedBackpackPositions(_positions);
		if (this.backpackNavObjects != null)
		{
			foreach (NavObject navObject in this.backpackNavObjects)
			{
				NavObjectManager.Instance.UnRegisterNavObject(navObject);
			}
			this.backpackNavObjects.Clear();
		}
		else
		{
			this.backpackNavObjects = new List<NavObject>();
		}
		if (_positions != null)
		{
			for (int i = 0; i < _positions.Count; i++)
			{
				Vector3i vector3i = _positions[i];
				if (!vector3i.Equals(Vector3i.zero))
				{
					this.backpackNavObjects.Add(NavObjectManager.Instance.RegisterNavObject("backpack_distant", vector3i.ToVector3() + new Vector3(0.5f, 0f, 0.5f), "", false, -1, null));
				}
			}
		}
	}

	// Token: 0x060023F3 RID: 9203 RVA: 0x000E4200 File Offset: 0x000E2400
	[PublicizedFrom(EAccessModifier.Protected)]
	public void dropBackpack(bool _isDying)
	{
		EntityPlayerLocal.DropOption dropOption = (EntityPlayerLocal.DropOption)(_isDying ? GameStats.GetInt(EnumGameStats.DropOnDeath) : GameStats.GetInt(EnumGameStats.DropOnQuit));
		if (dropOption == EntityPlayerLocal.DropOption.None)
		{
			return;
		}
		if (string.IsNullOrEmpty(this.dropInventoryBlock))
		{
			return;
		}
		if (this.playerUI.xui != null)
		{
			this.playerUI.xui.CancelAllCrafting();
		}
		bool flag = false;
		bool flag2 = false;
		ItemStack[] slots = this.bag.GetSlots();
		for (int i = 0; i < slots.Length; i++)
		{
			if (!slots[i].itemValue.type.Equals(ItemValue.None.type))
			{
				flag = true;
				break;
			}
		}
		for (int j = 0; j < this.inventory.GetItemCount(); j++)
		{
			if (!this.inventory.GetItem(j).itemValue.type.Equals(ItemValue.None.type))
			{
				flag2 = true;
				break;
			}
		}
		if (!flag && !flag2 && !this.equipment.HasAnyItems(-1))
		{
			return;
		}
		if (_isDying && dropOption == EntityPlayerLocal.DropOption.DeleteAll)
		{
			ItemStack[] slots2 = this.bag.GetSlots();
			for (int k = 0; k < slots2.Length; k++)
			{
				slots2[k] = ItemStack.Empty.Clone();
			}
			this.bag.SetSlots(slots2);
			for (int l = 0; l < this.inventory.GetItemCount(); l++)
			{
				this.inventory.SetItem(l, ItemStack.Empty.Clone());
			}
			for (int m = 0; m < 8; m++)
			{
				ItemValue slotItem = this.equipment.GetSlotItem(m);
				if (slotItem != null && !slotItem.ItemClassOrMissing.KeepOnDeath())
				{
					this.equipment.SetSlotItem(m, null, true);
				}
			}
			return;
		}
		EntityBackpack entityBackpack = EntityFactory.CreateEntity("Backpack".GetHashCode(), this.position + base.transform.up * 2f) as EntityBackpack;
		TileEntityLootContainer tileEntityLootContainer = new TileEntityLootContainer(null);
		string lootList = entityBackpack.GetLootList();
		tileEntityLootContainer.lootListName = lootList;
		tileEntityLootContainer.SetUserAccessing(true);
		tileEntityLootContainer.SetEmpty();
		tileEntityLootContainer.SetContainerSize(LootContainer.GetLootContainer(lootList, true).size, true);
		PreferenceTracker preferenceTracker = new PreferenceTracker(this.entityId);
		if (dropOption == EntityPlayerLocal.DropOption.All || dropOption == EntityPlayerLocal.DropOption.Backpack)
		{
			ItemStack[] slots3 = this.bag.GetSlots();
			preferenceTracker.SetBag(slots3, this.dropStackCondition);
			for (int n = 0; n < slots3.Length; n++)
			{
				if (this.dropStackCondition(slots3[n]))
				{
					tileEntityLootContainer.AddItem(slots3[n]);
					slots3[n] = ItemStack.Empty.Clone();
				}
			}
			this.bag.SetSlots(slots3);
		}
		if (dropOption == EntityPlayerLocal.DropOption.All)
		{
			ItemValue[] array = new ItemValue[8];
			for (int num = 0; num < array.Length; num++)
			{
				ItemValue slotItem2 = this.equipment.GetSlotItem(num);
				if (this.dropValueCondition(slotItem2))
				{
					array[num] = slotItem2;
					ItemStack item = new ItemStack(slotItem2, 1);
					tileEntityLootContainer.AddItem(item);
					this.equipment.SetSlotItem(num, null, true);
				}
			}
			preferenceTracker.SetEquipment(array, this.dropValueCondition);
		}
		if (dropOption == EntityPlayerLocal.DropOption.All || dropOption == EntityPlayerLocal.DropOption.Toolbelt)
		{
			ItemStack[] array2 = new ItemStack[this.inventory.GetItemCount()];
			for (int num2 = 0; num2 < array2.Length; num2++)
			{
				if (num2 != this.inventory.DUMMY_SLOT_IDX)
				{
					ItemStack item2 = this.inventory.GetItem(num2);
					array2[num2] = item2;
					if (this.dropStackCondition(item2))
					{
						tileEntityLootContainer.AddItem(item2);
						this.inventory.SetItem(num2, new ItemStack(ItemValue.None.Clone(), 0));
					}
				}
			}
			preferenceTracker.SetToolbelt(array2, this.dropStackCondition);
		}
		if (preferenceTracker.AnyPreferences)
		{
			tileEntityLootContainer.preferences = preferenceTracker;
		}
		tileEntityLootContainer.bPlayerBackpack = true;
		tileEntityLootContainer.SetUserAccessing(false);
		tileEntityLootContainer.SetModified();
		entityBackpack.RefPlayerId = this.entityId;
		EntityCreationData entityCreationData = new EntityCreationData(entityBackpack, true);
		entityCreationData.entityName = string.Format(Localization.Get("playersBackpack", false), this.EntityName);
		entityCreationData.id = -1;
		entityCreationData.lootContainer = tileEntityLootContainer;
		GameManager.Instance.RequestToSpawnEntityServer(entityCreationData);
		entityBackpack.OnEntityUnload();
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.SetDroppedBackpackPositions(GameManager.Instance.persistentLocalPlayer.GetDroppedBackpackPositions());
		}
	}

	// Token: 0x060023F4 RID: 9204 RVA: 0x000E4656 File Offset: 0x000E2856
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool ShouldRemoveEquipmentOnDeath(EntityPlayerLocal.DropOption dropOption)
	{
		return dropOption == EntityPlayerLocal.DropOption.All || dropOption == EntityPlayerLocal.DropOption.DeleteAll;
	}

	// Token: 0x060023F5 RID: 9205 RVA: 0x000E4664 File Offset: 0x000E2864
	public override int AttachToEntity(Entity _other, int slot = -1)
	{
		if (_other.IsAttached(this))
		{
			return -1;
		}
		vp_FPController vp_FPController = this.vp_FPController;
		vp_FPController.enabled = true;
		Transform transform = this.m_vp_FPCamera.Transform;
		Vector3 position = transform.position;
		Quaternion rotation = transform.rotation;
		this.SetFirstPersonView(false, false);
		slot = base.AttachToEntity(_other, slot);
		if (slot >= 0)
		{
			transform.position = position;
			transform.rotation = rotation;
			vp_FPController.Stop();
			vp_FPController.Player.Driving.Start(0f);
		}
		else
		{
			this.SetFirstPersonView(true, false);
		}
		EntityVehicle entityVehicle = _other as EntityVehicle;
		if (entityVehicle && entityVehicle.LocalPlayerIsOwner())
		{
			this.Waypoints.UpdateEntityVehicleWayPoint(entityVehicle, false);
			this.Waypoints.SetWaypointHiddenOnMap(this.entityId, true);
		}
		Manager.BroadcastStop(this.entityId, "Player" + (this.IsMale ? "Male" : "Female") + "RunLoop");
		this.sprintLoopSoundPlayId = -1;
		return slot;
	}

	// Token: 0x060023F6 RID: 9206 RVA: 0x000E4760 File Offset: 0x000E2960
	public override void Detach()
	{
		this.SetFirstPersonView(this.bPreferFirstPerson, false);
		this.moveController.isAutorun = false;
		Vector3 vector = Vector3.zero;
		EntityVehicle entityVehicle = this.AttachedToEntity as EntityVehicle;
		if (entityVehicle)
		{
			vector = entityVehicle.GetExitVelocity() * Time.fixedDeltaTime;
			if (entityVehicle.LocalPlayerIsOwner())
			{
				this.Waypoints.UpdateEntityVehicleWayPoint(entityVehicle, false);
			}
		}
		base.Detach();
		vp_FPController vp_FPController = this.vp_FPController;
		vp_FPController.Player.Driving.Stop(0f);
		vp_FPController.Stop();
		vp_FPController.m_MaxHeightInitialFallSpeed = Utils.FastMin(vector.y, 0f);
		vp_FPController.AddForce(vector);
	}

	// Token: 0x060023F7 RID: 9207 RVA: 0x000E4809 File Offset: 0x000E2A09
	public override void ProcessDamageResponseLocal(DamageResponse _dmResponse)
	{
		base.ProcessDamageResponseLocal(_dmResponse);
		if (_dmResponse.Source.damageType != EnumDamageTypes.Weather)
		{
			this.healthLostThisRound = (_dmResponse.Strength > 0);
		}
	}

	// Token: 0x060023F8 RID: 9208 RVA: 0x000E4830 File Offset: 0x000E2A30
	public override bool CanUpdateEntity()
	{
		return this.IsFlyMode.Value || base.CanUpdateEntity();
	}

	// Token: 0x060023F9 RID: 9209 RVA: 0x000E4848 File Offset: 0x000E2A48
	public override void OnHUD()
	{
		if (Event.current.type != EventType.Repaint)
		{
			return;
		}
		NGuiWdwInGameHUD inGameHUD = this.nguiWindowManager.InGameHUD;
		if (!GameManager.Instance.gameStateManager.IsGameStarted() || GameStats.GetInt(EnumGameStats.GameState) != 1 || !this.Spawned)
		{
			return;
		}
		bool flag = this.windowManager.IsModalWindowOpen() || LocalPlayerUI.primaryUI.windowManager.IsModalWindowOpen();
		this.guiDrawOverlayTextures(inGameHUD, flag);
		if (this.windowManager.IsFullHUDDisabled())
		{
			return;
		}
		if (this.inventory != null && !flag)
		{
			this.inventory.holdingItem.OnHUD(this.inventory.holdingItemData, Screen.width - 10, Screen.height - 10);
		}
		if (!LocalPlayerUI.primaryUI.windowManager.IsModalWindowOpen() && !this.windowManager.IsWindowOpen("toolbelt") && !this.windowManager.IsWindowOpen(XUiC_InGameMenuWindow.ID) && !this.windowManager.IsWindowOpen("dialog") && !this.windowManager.IsWindowOpen("tipWindow") && !this.windowManager.IsWindowOpen("questOffer"))
		{
			this.windowManager.Open("toolbelt", false, false, true);
		}
		this.windowManager.OpenIfNotOpen("CalloutGroup", false, false, true);
		if (!this.windowManager.IsModalWindowOpen() && !this.windowManager.IsWindowOpen(XUiC_CompassWindow.ID) && !LocalPlayerUI.primaryUI.windowManager.IsModalWindowOpen())
		{
			this.windowManager.Open(XUiC_CompassWindow.ID, false, false, true);
		}
		this.windowManager.OpenIfNotOpen("toolTip", false, false, true);
		this.windowManager.OpenIfNotOpen(XUiC_ChatOutput.ID, false, false, true);
		if (Event.current.type == EventType.Repaint)
		{
			if (this.sneakDamageBlendTimer.Value > 0f)
			{
				this.nguiWindowManager.SetLabel(EnumNGUIWindow.CriticalHitText, this.sneakDamageText, new Color?(new Color(1f, 1f, 1f, this.sneakDamageBlendTimer.Value)), true);
			}
			else if (this.nguiWindowManager.IsShowing(EnumNGUIWindow.CriticalHitText))
			{
				this.nguiWindowManager.Show(EnumNGUIWindow.CriticalHitText, false);
			}
		}
		this.guiDrawCrosshair(inGameHUD, flag);
	}

	// Token: 0x060023FA RID: 9210 RVA: 0x000E4A73 File Offset: 0x000E2C73
	public void ForceBloodSplatter()
	{
		this.healthLostThisRound = true;
		this.lastHitDirection = Utils.EnumHitDirection.Front;
	}

	// Token: 0x060023FB RID: 9211 RVA: 0x000E4A84 File Offset: 0x000E2C84
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void guiDrawOverlayTextures(NGuiWdwInGameHUD _guiInGame, bool bModalWindowOpen)
	{
		if (Event.current.type != EventType.Repaint)
		{
			return;
		}
		this.inventory.holdingItem.OnScreenOverlay(this.inventory.holdingItemData);
		Vector3 vector = this.finalCamera.ViewportToScreenPoint(new Vector3(0f, 0f, 0f));
		Vector3 vector2 = this.finalCamera.ViewportToScreenPoint(new Vector3(1f, 1f, 0f));
		if (this.healthLostThisRound && !this.IsDead() && this.lastHitDirection != Utils.EnumHitDirection.None)
		{
			this.healthLostThisRound = false;
			int lastHitDirection = (int)this.lastHitDirection;
			this.overlayDirectionTime[lastHitDirection] = 6f;
			byte[] array = this.overlayAlternating;
			int num = lastHitDirection;
			array[num] += 1;
			int num2 = lastHitDirection * 2 + (int)(this.overlayAlternating[lastHitDirection] & 1);
			this.overlayDirectionTime[num2] = 6f;
			this.overlayBloodDropsPositions[num2 * 3] = new Vector2(this.rand.RandomRange(vector.x, vector2.x), this.rand.RandomRange(vector.y, vector2.y));
			this.overlayBloodDropsPositions[num2 * 3 + 1] = new Vector2(this.rand.RandomRange(vector.x, vector2.x), this.rand.RandomRange(vector.y, vector2.y));
			this.overlayBloodDropsPositions[num2 * 3 + 2] = new Vector2(this.rand.RandomRange(vector.x, vector2.x), this.rand.RandomRange(vector.y, vector2.y));
			this.lastHitDirection = Utils.EnumHitDirection.None;
		}
		for (int i = 0; i < 8; i++)
		{
			if (this.overlayDirectionTime[i] > 0f)
			{
				float num3 = Mathf.Pow(1f - this.overlayDirectionTime[i] / 6f, 0.28f);
				this.overlayMaterial.SetColor("_Color", new Color(num3, num3, num3));
				if (this.windowManager.IsHUDEnabled())
				{
					Vector3 vector3 = (vector2 + vector) * 0.5f;
					int pixelWidth = this.finalCamera.pixelWidth;
					int pixelHeight = this.finalCamera.pixelHeight;
					Texture2D texture2D = _guiInGame.overlayDamageTextures[i];
					float num4 = (float)pixelHeight / 512f;
					float num5 = (float)texture2D.width * num4;
					float num6 = (float)texture2D.height * num4;
					Rect screenRect = new Rect(vector3.x - num5 * 0.5f, 0f, num5, num6);
					int num7 = i >> 1;
					if (num7 == 1)
					{
						screenRect.y = (float)pixelHeight - num6;
					}
					else if (num7 >= 2)
					{
						screenRect.x = (float)pixelWidth - num5;
						screenRect.y = vector3.y - num6 * 0.5f;
						if (num7 == 3)
						{
							screenRect.x = 0f;
						}
					}
					Graphics.DrawTexture(screenRect, texture2D, this.overlayMaterial);
					int num8 = i * 3;
					Graphics.DrawTexture(new Rect(this.overlayBloodDropsPositions[num8].x, (float)pixelHeight - this.overlayBloodDropsPositions[num8].y, (float)_guiInGame.overlayDamageBloodDrops[0].width, (float)_guiInGame.overlayDamageBloodDrops[0].height), _guiInGame.overlayDamageBloodDrops[0], this.overlayMaterial);
					Graphics.DrawTexture(new Rect(this.overlayBloodDropsPositions[num8 + 1].x, (float)pixelHeight - this.overlayBloodDropsPositions[num8 + 1].y, (float)_guiInGame.overlayDamageBloodDrops[1].width, (float)_guiInGame.overlayDamageBloodDrops[1].height), _guiInGame.overlayDamageBloodDrops[1], this.overlayMaterial);
					Graphics.DrawTexture(new Rect(this.overlayBloodDropsPositions[num8 + 2].x, (float)pixelHeight - this.overlayBloodDropsPositions[num8 + 2].y, (float)_guiInGame.overlayDamageBloodDrops[2].width, (float)_guiInGame.overlayDamageBloodDrops[2].height), _guiInGame.overlayDamageBloodDrops[2], this.overlayMaterial);
				}
				if (this.Health > 0)
				{
					this.overlayDirectionTime[i] -= Time.deltaTime;
				}
				else
				{
					this.overlayDirectionTime[i] = 0f;
				}
			}
		}
	}

	// Token: 0x060023FC RID: 9212 RVA: 0x0003E2E0 File Offset: 0x0003C4E0
	[PublicizedFrom(EAccessModifier.Private)]
	public float CrosshairAlpha(NGuiWdwInGameHUD _guiInGame)
	{
		return 1f;
	}

	// Token: 0x060023FD RID: 9213 RVA: 0x000E4EDC File Offset: 0x000E30DC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void guiDrawCrosshair(NGuiWdwInGameHUD _guiInGame, bool bModalWindowOpen)
	{
		if (!_guiInGame.showCrosshair)
		{
			return;
		}
		if (Event.current.type != EventType.Repaint)
		{
			return;
		}
		if (this.IsDead() || this.emodel.IsRagdollActive)
		{
			return;
		}
		if (this.AttachedToEntity != null)
		{
			return;
		}
		ItemClass.EnumCrosshairType crosshairType = this.inventory.holdingItem.GetCrosshairType(this.inventory.holdingItemData);
		if (!bModalWindowOpen && this.inventory != null)
		{
			Vector2 crosshairPosition2D = this.GetCrosshairPosition2D();
			crosshairPosition2D.y = (float)Screen.height - crosshairPosition2D.y;
			float num = (float)Screen.height * 0.059f;
			switch (crosshairType)
			{
			case ItemClass.EnumCrosshairType.Plus:
				if (Event.current.type == EventType.Repaint)
				{
					Color color = GUI.color;
					GUI.color = new Color(color.r, color.g, color.b, _guiInGame.crosshairAlpha);
					GUI.DrawTexture(new Rect(crosshairPosition2D.x - num / 2f, crosshairPosition2D.y - num / 2f, num, num), _guiInGame.CrosshairTexture, ScaleMode.StretchToFill);
					GUI.color = color;
					return;
				}
				break;
			case ItemClass.EnumCrosshairType.Crosshair:
			case ItemClass.EnumCrosshairType.CrosshairOnAiming:
				if (crosshairType != ItemClass.EnumCrosshairType.Crosshair || !this.AimingGun || ItemAction.ShowDistanceDebugInfo)
				{
					this.GetCrosshairOpenArea();
					float num2 = EffectManager.GetValue(PassiveEffects.SpreadDegreesHorizontal, this.inventory.holdingItemData.itemValue, 90f, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
					num2 *= 0.5f;
					num2 *= (this.inventory.holdingItemData.actionData[0] as ItemActionRanged.ItemActionDataRanged).lastAccuracy;
					num2 *= (float)Mathf.RoundToInt((float)Screen.width / this.cameraTransform.GetComponent<Camera>().fieldOfView);
					float num3 = EffectManager.GetValue(PassiveEffects.SpreadDegreesVertical, this.inventory.holdingItemData.itemValue, 90f, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
					num3 *= 0.5f;
					num3 *= (this.inventory.holdingItemData.actionData[0] as ItemActionRanged.ItemActionDataRanged).lastAccuracy;
					num3 *= (float)Mathf.RoundToInt((float)Screen.width / this.cameraTransform.GetComponent<Camera>().fieldOfView);
					int num4 = (int)crosshairPosition2D.x;
					int num5 = (int)crosshairPosition2D.y;
					int num6 = 18;
					Color black = Color.black;
					Color white = Color.white;
					black.a = this.CrosshairAlpha(_guiInGame) * this.weaponCrossHairAlpha;
					white.a = this.CrosshairAlpha(_guiInGame) * this.weaponCrossHairAlpha;
					GUIUtils.DrawLine(new Vector2((float)num4 - num2, (float)(num5 + 1)), new Vector2((float)num4 - (num2 + (float)num6), (float)(num5 + 1)), black);
					GUIUtils.DrawLine(new Vector2((float)num4 + num2, (float)(num5 + 1)), new Vector2((float)num4 + num2 + (float)num6, (float)(num5 + 1)), black);
					GUIUtils.DrawLine(new Vector2((float)(num4 + 1), (float)num5 - num3), new Vector2((float)(num4 + 1), (float)num5 - (num3 + (float)num6)), black);
					GUIUtils.DrawLine(new Vector2((float)(num4 + 1), (float)num5 + num3), new Vector2((float)(num4 + 1), (float)num5 + num3 + (float)num6), black);
					GUIUtils.DrawLine(new Vector2((float)num4 + num2, (float)num5), new Vector2((float)num4 + num2 + (float)num6, (float)num5), white);
					GUIUtils.DrawLine(new Vector2((float)num4, (float)num5 - num3), new Vector2((float)num4, (float)num5 - (num3 + (float)num6)), white);
					GUIUtils.DrawLine(new Vector2((float)num4 - num2, (float)num5), new Vector2((float)num4 - (num2 + (float)num6), (float)num5), white);
					GUIUtils.DrawLine(new Vector2((float)num4, (float)num5 + num3), new Vector2((float)num4, (float)num5 + num3 + (float)num6), white);
					GUIUtils.DrawLine(new Vector2((float)num4 - num2, (float)(num5 - 1)), new Vector2((float)num4 - (num2 + (float)num6), (float)(num5 - 1)), black);
					GUIUtils.DrawLine(new Vector2((float)num4 + num2, (float)(num5 - 1)), new Vector2((float)num4 + num2 + (float)num6, (float)(num5 - 1)), black);
					GUIUtils.DrawLine(new Vector2((float)(num4 - 1), (float)num5 - num3), new Vector2((float)(num4 - 1), (float)num5 - (num3 + (float)num6)), black);
					GUIUtils.DrawLine(new Vector2((float)(num4 - 1), (float)num5 + num3), new Vector2((float)(num4 - 1), (float)num5 + num3 + (float)num6), black);
					return;
				}
				break;
			case ItemClass.EnumCrosshairType.Damage:
				if (Event.current.type == EventType.Repaint)
				{
					Color color2 = GUI.color;
					if (this.playerUI.xui.BackgroundGlobalOpacity < 1f)
					{
						float a = color2.a * this.playerUI.xui.BackgroundGlobalOpacity;
						GUI.color = new Color(color2.r, color2.g, color2.b, a);
					}
					else
					{
						GUI.color = new Color(color2.r, color2.g, color2.b, this.CrosshairAlpha(_guiInGame));
					}
					GUI.DrawTexture(new Rect(crosshairPosition2D.x - num / 2f, crosshairPosition2D.y - num / 2f, num, num), _guiInGame.CrosshairDamage, ScaleMode.StretchToFill);
					GUI.color = color2;
					return;
				}
				break;
			case ItemClass.EnumCrosshairType.Upgrade:
				if (Event.current.type == EventType.Repaint)
				{
					Color color3 = GUI.color;
					if (this.playerUI.xui.BackgroundGlobalOpacity < 1f)
					{
						float a2 = color3.a * this.playerUI.xui.BackgroundGlobalOpacity;
						GUI.color = new Color(color3.r, color3.g, color3.b, a2);
					}
					else
					{
						GUI.color = new Color(color3.r, color3.g, color3.b, this.CrosshairAlpha(_guiInGame));
					}
					GUI.DrawTexture(new Rect(crosshairPosition2D.x - num / 2f, crosshairPosition2D.y - num / 2f, num, num), _guiInGame.CrosshairUpgrade, ScaleMode.StretchToFill);
					GUI.color = color3;
					return;
				}
				break;
			case ItemClass.EnumCrosshairType.Repair:
				if (Event.current.type == EventType.Repaint)
				{
					Color color4 = GUI.color;
					if (this.playerUI.xui.BackgroundGlobalOpacity < 1f)
					{
						float a3 = color4.a * this.playerUI.xui.BackgroundGlobalOpacity;
						GUI.color = new Color(color4.r, color4.g, color4.b, a3);
					}
					else
					{
						GUI.color = new Color(color4.r, color4.g, color4.b, this.CrosshairAlpha(_guiInGame));
					}
					GUI.DrawTexture(new Rect(crosshairPosition2D.x - num / 2f, crosshairPosition2D.y - num / 2f, num, num), _guiInGame.CrosshairRepair, ScaleMode.StretchToFill);
					GUI.color = color4;
					return;
				}
				break;
			case ItemClass.EnumCrosshairType.PowerSource:
				if (Event.current.type == EventType.Repaint)
				{
					Color color5 = GUI.color;
					if (this.playerUI.xui.BackgroundGlobalOpacity < 1f)
					{
						float a4 = color5.a * this.playerUI.xui.BackgroundGlobalOpacity;
						GUI.color = new Color(color5.r, color5.g, color5.b, a4);
					}
					else
					{
						GUI.color = new Color(color5.r, color5.g, color5.b, this.CrosshairAlpha(_guiInGame));
					}
					GUI.DrawTexture(new Rect(crosshairPosition2D.x - num / 2f, crosshairPosition2D.y - num / 2f, num, num), _guiInGame.CrosshairPowerSource, ScaleMode.StretchToFill);
					GUI.color = color5;
				}
				break;
			case ItemClass.EnumCrosshairType.Heal:
				if (Event.current.type == EventType.Repaint)
				{
					Color color6 = GUI.color;
					if (this.playerUI.xui.BackgroundGlobalOpacity < 1f)
					{
						float a5 = color6.a * this.playerUI.xui.BackgroundGlobalOpacity;
						GUI.color = new Color(color6.r, color6.g, color6.b, a5);
					}
					else
					{
						GUI.color = new Color(color6.r, color6.g, color6.b, this.CrosshairAlpha(_guiInGame));
					}
					GUI.DrawTexture(new Rect(crosshairPosition2D.x - num / 2f, crosshairPosition2D.y - num / 2f, num, num), _guiInGame.CrosshairRepair, ScaleMode.StretchToFill);
					GUI.color = color6;
					return;
				}
				break;
			case ItemClass.EnumCrosshairType.PowerItem:
				if (Event.current.type == EventType.Repaint)
				{
					Color color7 = GUI.color;
					if (this.playerUI.xui.BackgroundGlobalOpacity < 1f)
					{
						float a6 = color7.a * this.playerUI.xui.BackgroundGlobalOpacity;
						GUI.color = new Color(color7.r, color7.g, color7.b, a6);
					}
					else
					{
						GUI.color = new Color(color7.r, color7.g, color7.b, this.CrosshairAlpha(_guiInGame));
					}
					GUI.DrawTexture(new Rect(crosshairPosition2D.x - num / 2f, crosshairPosition2D.y - num / 2f, num, num), _guiInGame.CrosshairPowerItem, ScaleMode.StretchToFill);
					GUI.color = color7;
					return;
				}
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x060023FE RID: 9214 RVA: 0x000E581C File Offset: 0x000E3A1C
	[PublicizedFrom(EAccessModifier.Private)]
	public void ShelterFrameUpdate()
	{
		if (!this.shelterIsUpdating)
		{
			this.shelterIsUpdating = true;
			this.shelterStartPos = this.position - Origin.position;
			this.shelterPos = this.shelterStartPos;
			this.shelterPos.y = this.shelterPos.y + 0.62f;
			this.shelterSideCount = -10;
		}
		if (this.shelterSideCount == -10)
		{
			this.shelterSideCount = -2;
			this.ShelterCheckSkyUp();
			this.shelterAbovePercent = 1f;
			if (!this.shelterIsUpdating)
			{
				this.shelterAbovePercent = 0f;
				this.shelterPercent = 0f;
				return;
			}
		}
		else if (this.shelterSideCount < 0)
		{
			this.shelterSideCount++;
			this.ShelterCheckSkyDiagonal();
			this.shelterPos.y = this.shelterPos.y + 0.1f;
			if (!this.shelterIsUpdating)
			{
				this.shelterPercent = 0f;
				return;
			}
			if (this.shelterSideCount == 0)
			{
				this.shelterStartPos.y = this.shelterStartPos.y + 0.48f;
				this.shelterStartPos.x = this.shelterStartPos.x - 0.13f;
				this.shelterStartPos.z = this.shelterStartPos.z - 0.13f;
				this.shelterPosOffset = Vector3.zero;
				return;
			}
		}
		else
		{
			if (this.shelterSideCount == 0)
			{
				this.shelterSideCount = 1;
				this.shelterPos = this.shelterStartPos + this.shelterPosOffset;
				this.shelterDir = 0f;
				this.shelterRadius = 1f;
			}
			this.ShelterCheckSides();
			if (!this.shelterIsUpdating)
			{
				this.shelterPercent = 0f;
				return;
			}
			if (this.shelterSideCount >= 4)
			{
				if (this.shelterPosOffset.y < 0.5f)
				{
					this.shelterPosOffset.y = this.shelterPosOffset.y + (base.IsCrouching ? 0.54f : 0.65f);
					this.shelterPosOffset.x = this.shelterPosOffset.x + 0.011f;
					this.shelterPosOffset.z = this.shelterPosOffset.z + 0.013f;
					this.shelterSideCount = 0;
					return;
				}
				this.shelterIsUpdating = false;
				this.shelterPercent = 1f;
			}
		}
	}

	// Token: 0x060023FF RID: 9215 RVA: 0x000E5A2C File Offset: 0x000E3C2C
	[PublicizedFrom(EAccessModifier.Private)]
	public void ShelterCheckSkyUp()
	{
		PhysicsScene defaultPhysicsScene = Physics.defaultPhysicsScene;
		for (int i = 0; i < 4; i++)
		{
			Vector3 origin = this.shelterPos;
			float num = this.shelterDir * 0.017453292f;
			origin.x += (float)Math.Cos((double)num) * 0.18f;
			origin.z += (float)Math.Sin((double)num) * 0.18f;
			RaycastHit raycastHit;
			if (!defaultPhysicsScene.Raycast(origin, Vector3.up, out raycastHit, 253f, 1073807360, QueryTriggerInteraction.UseGlobal))
			{
				this.shelterIsUpdating = false;
				return;
			}
			this.shelterDir += 90f;
		}
	}

	// Token: 0x06002400 RID: 9216 RVA: 0x000E5AD0 File Offset: 0x000E3CD0
	[PublicizedFrom(EAccessModifier.Private)]
	public void ShelterCheckSkyDiagonal()
	{
		PhysicsScene defaultPhysicsScene = Physics.defaultPhysicsScene;
		Vector3 direction;
		direction.y = 0.75f + (float)(this.shelterSideCount + 1) * 0.13f;
		for (int i = 0; i < 8; i++)
		{
			float num = this.shelterDir * 0.017453292f;
			direction.x = (float)Math.Cos((double)num);
			direction.z = (float)Math.Sin((double)num);
			RaycastHit raycastHit;
			if (!defaultPhysicsScene.Raycast(this.shelterPos, direction, out raycastHit, 253f, 1073807360, QueryTriggerInteraction.UseGlobal))
			{
				this.shelterIsUpdating = false;
				return;
			}
			this.shelterDir += 45f;
		}
	}

	// Token: 0x06002401 RID: 9217 RVA: 0x000E5B70 File Offset: 0x000E3D70
	[PublicizedFrom(EAccessModifier.Private)]
	public void ShelterCheckSides()
	{
		PhysicsScene defaultPhysicsScene = Physics.defaultPhysicsScene;
		Vector3 vector;
		vector.y = 0f;
		for (int i = 0; i < 8; i++)
		{
			float num = this.shelterDir * 0.017453292f;
			vector.x = (float)Math.Cos((double)num) * this.shelterRadius;
			vector.z = (float)Math.Sin((double)num) * this.shelterRadius;
			Vector3 origin = this.shelterPos + vector;
			RaycastHit raycastHit;
			if (defaultPhysicsScene.Raycast(this.shelterPos, vector, out raycastHit, this.shelterRadius, 1073807360, QueryTriggerInteraction.UseGlobal))
			{
				origin = raycastHit.point;
				origin.y -= 0.001f;
			}
			if (!defaultPhysicsScene.Raycast(origin, Vector3.up, out raycastHit, 253f, 1073807360, QueryTriggerInteraction.UseGlobal))
			{
				this.shelterIsUpdating = false;
				break;
			}
			this.shelterDir += 45f;
		}
		this.shelterDir += -359f;
		this.shelterRadius += 1f;
		if (this.shelterRadius > 3.1f)
		{
			this.shelterRadius = 0f;
			this.shelterSideCount++;
			this.shelterPos.x = this.shelterPos.x + 0.13f;
			this.shelterPos.z = this.shelterPos.z + 0.13f;
		}
	}

	// Token: 0x06002402 RID: 9218 RVA: 0x000E5CCC File Offset: 0x000E3ECC
	public void WeatherStatusFrameUpdate()
	{
		bool flag = this.AreaMessage != null && this.stormAlertBiomeStandingOn == this.biomeStandingOn;
		this.AreaMessage = null;
		this.isPlayerInStorm = false;
		if (this.biomeStandingOn != null)
		{
			int stormLevel = this.biomeStandingOn.currentWeatherGroup.stormLevel;
			if (stormLevel > 0 && this.biomeStandingOn.Difficulty >= 2)
			{
				if (stormLevel >= 2)
				{
					this.isPlayerInStorm = !this.isIndoorsCurrent;
				}
				this.AreaMessageAlpha = 1f;
				string key = "weatherStormBuild";
				if (stormLevel == 1)
				{
					if (this.isIndoorsCurrent)
					{
						key = "weatherStormBuildSafe";
						this.AreaMessageAlpha = 0.85f;
					}
				}
				else if (stormLevel >= 2)
				{
					key = "weatherStorm";
					if (this.isIndoorsCurrent)
					{
						key = "weatherStormSafe";
						this.AreaMessageAlpha = 0.75f;
					}
				}
				if (stormLevel == 1)
				{
					int remainingSeconds = (int)WeatherManager.currentWeather.remainingSeconds;
					string arg = "";
					if (remainingSeconds <= 60)
					{
						if (!flag || (float)remainingSeconds > this.stormRemainingSeconds)
						{
							this.stormRemainingSeconds = (float)remainingSeconds;
						}
						this.stormRemainingSeconds = Mathf.MoveTowards(this.stormRemainingSeconds, (float)remainingSeconds, Time.deltaTime);
						arg = string.Format("{0:d2}", (int)(this.stormRemainingSeconds + 0.99f));
					}
					this.AreaMessage = string.Format(Localization.Get(key, false), arg);
				}
				else
				{
					this.AreaMessage = Localization.Get(key, false);
				}
				if (!flag)
				{
					Manager.PlayInsidePlayerHead("ui_weather_alert", -1, 0f, false, true);
					this.stormAlertBiomeStandingOn = this.biomeStandingOn;
				}
			}
		}
	}

	// Token: 0x06002403 RID: 9219 RVA: 0x000E5E48 File Offset: 0x000E4048
	[PublicizedFrom(EAccessModifier.Private)]
	public void WeatherStatusTick()
	{
		if (this.SpawnedTicks > 100)
		{
			bool flag = false;
			BiomeDefinition biomeStandingOn = this.biomeStandingOn;
			BiomeDefinition.WeatherGroup weatherGroup = (biomeStandingOn != null) ? biomeStandingOn.currentWeatherGroup : null;
			if (weatherGroup != null && weatherGroup != this.weatherGroup)
			{
				string format = "{0} WeatherStatusTick {1} to {2}";
				object[] array = new object[3];
				array[0] = base.GetDebugName();
				int num = 1;
				BiomeDefinition.WeatherGroup weatherGroup2 = this.weatherGroup;
				array[num] = ((weatherGroup2 != null) ? weatherGroup2.name : null);
				array[2] = weatherGroup.name;
				Log.Out(format, array);
				if (this.weatherBuff != null)
				{
					this.Buffs.RemoveBuff(this.weatherBuff, true);
				}
				this.weatherGroup = weatherGroup;
				this.weatherBuff = weatherGroup.buffName;
				flag = true;
			}
			bool flag2 = this.shelterPercent > 0f;
			if (this.isIndoorsCurrent != flag2)
			{
				this.isIndoorsCurrent = flag2;
				flag = true;
			}
			if (flag)
			{
				this.WeatherBuffUpdate();
			}
		}
	}

	// Token: 0x06002404 RID: 9220 RVA: 0x000E5F14 File Offset: 0x000E4114
	[PublicizedFrom(EAccessModifier.Private)]
	public void WeatherBuffUpdate()
	{
		Log.Out("{0} WeatherBuffUpdate {1}, indoors {2}", new object[]
		{
			base.GetDebugName(),
			this.weatherBuff,
			this.isIndoorsCurrent
		});
		if (this.weatherBuff != null)
		{
			if (this.isIndoorsCurrent)
			{
				this.Buffs.RemoveBuff(this.weatherBuff, true);
				return;
			}
			this.Buffs.AddBuff(this.weatherBuff, -1, true, false, -1f);
		}
	}

	// Token: 0x06002405 RID: 9221 RVA: 0x000E5F8E File Offset: 0x000E418E
	public void OnWeatherGodModeChanged(bool _oldValue, bool _newValue)
	{
		if (!_newValue)
		{
			this.WeatherBuffUpdate();
		}
	}

	// Token: 0x06002406 RID: 9222 RVA: 0x000E5F9C File Offset: 0x000E419C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void onNewBiomeEntered(BiomeDefinition _biome)
	{
		if (_biome != null)
		{
			EnvironmentAudioManager.Instance.EnterBiome(_biome);
			this.MinEventContext.Biome = _biome;
			this.FireEvent(MinEventTypes.onSelfEnteredBiome, true);
			QuestEventManager.Current.BiomeEntered(_biome);
			this.lastLootStage = -1;
		}
		if (this.biomeStandingOn != null && this.biomeStandingOn.Buff != null)
		{
			Log.Out("{0} onNewBiomeEntered -{1}", new object[]
			{
				base.GetDebugName(),
				this.biomeStandingOn.Buff
			});
			this.Buffs.RemoveBuff(this.biomeStandingOn.Buff, true);
		}
		if (_biome != null && EntityStats.NewWeatherSurvivalEnabled && _biome.Buff != null)
		{
			Log.Out("{0} onNewBiomeEntered +{1}", new object[]
			{
				base.GetDebugName(),
				_biome.Buff
			});
			this.Buffs.AddBuff(_biome.Buff, -1, true, false, -1f);
		}
		this.biomeStandingOn = _biome;
	}

	// Token: 0x06002407 RID: 9223 RVA: 0x000E6086 File Offset: 0x000E4286
	public void ResetBiomeWeatherOnDeath()
	{
		this.onNewBiomeEntered(null);
		this.isIndoorsCurrent = true;
		this.WeatherBuffUpdate();
		this.weatherBuff = null;
		this.weatherGroup = null;
	}

	// Token: 0x06002408 RID: 9224 RVA: 0x000E60AC File Offset: 0x000E42AC
	public override float GetWetnessRate()
	{
		float num = Mathf.Pow(this.inWaterPercent, 2f);
		if (this.shelterAbovePercent == 0f)
		{
			float v = WeatherManager.Instance.GetCurrentWetPercent(this) * 0.05f;
			num = Utils.FastMax(num, v);
		}
		return num;
	}

	// Token: 0x06002409 RID: 9225 RVA: 0x000E60F4 File Offset: 0x000E42F4
	public SpawnPosition GetSpawnPoint()
	{
		if (this.SpawnPoints == null || this.SpawnPoints.Count == 0)
		{
			return SpawnPosition.Undef;
		}
		return new SpawnPosition(this.SpawnPoints[0].ToVector3() + new Vector3(0.5f, 0f, 0.5f), 0f);
	}

	// Token: 0x0600240A RID: 9226 RVA: 0x000E6153 File Offset: 0x000E4353
	public override void AddUIHarvestingItem(ItemStack itemStack, bool _bAddOnlyIfNotExisting = false)
	{
		this.playerUI.xui.CollectedItemList.AddItemStack(itemStack, _bAddOnlyIfNotExisting);
	}

	// Token: 0x0600240B RID: 9227 RVA: 0x000E616C File Offset: 0x000E436C
	public override Vector3 GetDropPosition()
	{
		Vector3 vector = base.GetDropPosition();
		Vector3 direction = vector - this.getHeadPosition();
		RaycastHit raycastHit;
		if (Physics.Raycast(new Ray(this.getHeadPosition() - Origin.position, direction), out raycastHit, direction.magnitude, 1073807360))
		{
			vector = raycastHit.point - direction.normalized * 0.5f + Origin.position;
		}
		return vector;
	}

	// Token: 0x0600240C RID: 9228 RVA: 0x000E61E4 File Offset: 0x000E43E4
	public bool CheckSpawnPointStillThere()
	{
		SpawnPosition spawnPoint = this.GetSpawnPoint();
		return spawnPoint.IsUndef() || this.world.GetChunkFromWorldPos(spawnPoint.ToBlockPos()) == null || this.world.GetBlock(spawnPoint.ToBlockPos()).Block is BlockSleepingBag;
	}

	// Token: 0x0600240D RID: 9229 RVA: 0x000E623D File Offset: 0x000E443D
	public void RemoveSpawnPoints(bool showTooltip = true)
	{
		this.SpawnPoints.Clear();
		if (showTooltip)
		{
			GameManager.ShowTooltip(this, Localization.Get("ttBedrollGone", false), false, false, 0f);
		}
		this.selectedSpawnPointKey = -1L;
	}

	// Token: 0x0600240E RID: 9230 RVA: 0x000E6270 File Offset: 0x000E4470
	public void EmptyBackpackAndToolbelt()
	{
		ItemStack[] slots = this.bag.GetSlots();
		for (int i = 0; i < slots.Length; i++)
		{
			slots[i] = ItemStack.Empty.Clone();
		}
		this.bag.SetSlots(slots);
		for (int j = 0; j < this.inventory.GetItemCount(); j++)
		{
			if (j != this.inventory.DUMMY_SLOT_IDX)
			{
				this.inventory.SetItem(j, new ItemStack(ItemValue.None.Clone(), 0));
			}
		}
	}

	// Token: 0x0600240F RID: 9231 RVA: 0x000E62F0 File Offset: 0x000E44F0
	public void EmptyBackpack()
	{
		ItemStack[] slots = this.bag.GetSlots();
		for (int i = 0; i < slots.Length; i++)
		{
			slots[i] = ItemStack.Empty.Clone();
		}
		this.bag.SetSlots(slots);
	}

	// Token: 0x06002410 RID: 9232 RVA: 0x000E6330 File Offset: 0x000E4530
	public void EmptyToolbelt(int start, int end)
	{
		for (int i = start; i < end; i++)
		{
			if (i != this.inventory.DUMMY_SLOT_IDX)
			{
				this.inventory.SetItem(i, new ItemStack(ItemValue.None.Clone(), 0));
			}
		}
	}

	// Token: 0x06002411 RID: 9233 RVA: 0x000E6374 File Offset: 0x000E4574
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleMapObjects(bool usePersistantBackpackPositions)
	{
		if (this.persistentPlayerData != null)
		{
			List<Vector3i> landProtectionBlocks = this.persistentPlayerData.GetLandProtectionBlocks();
			for (int i = 0; i < landProtectionBlocks.Count; i++)
			{
				NavObject navObject = NavObjectManager.Instance.RegisterNavObject("land_claim", landProtectionBlocks[i].ToVector3(), "", false, -1, null);
				if (navObject != null)
				{
					navObject.OwnerEntity = this;
				}
			}
			this.persistentPlayerData.ShowBedrollOnMap();
			this.SetDroppedBackpackPositions(usePersistantBackpackPositions ? this.persistentPlayerData.GetDroppedBackpackPositions() : this.droppedBackpackPositions);
		}
	}

	// Token: 0x06002412 RID: 9234 RVA: 0x000E6400 File Offset: 0x000E4600
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void AnalyticsSendDeath(DamageResponse _dmResponse)
	{
		DamageSource source = _dmResponse.Source;
		string text;
		if (this.isFallDeath)
		{
			text = "fall";
		}
		else if (source.BuffClass != null)
		{
			text = source.BuffClass.Name;
		}
		else if (source.ItemClass != null)
		{
			text = source.ItemClass.Name;
		}
		else
		{
			text = source.damageType.ToStringCached<EnumDamageTypes>();
		}
		if (this.entityThatKilledMe)
		{
			text += "_";
			if (this.entityThatKilledMe == this)
			{
				text += "self";
			}
			else if (this.entityThatKilledMe is EntityPlayer)
			{
				text += "player";
			}
			else
			{
				text += this.entityThatKilledMe.EntityName;
			}
		}
		GameSparksCollector.IncrementCounter(GameSparksCollector.GSDataKey.PlayerDeathCauses, text, 1, true, GameSparksCollector.GSDataCollection.SessionUpdates);
	}

	// Token: 0x06002413 RID: 9235 RVA: 0x000E64C8 File Offset: 0x000E46C8
	public EntityPlayerLocal.AutoMove EnableAutoMove(bool _enable)
	{
		if (!_enable)
		{
			this.autoMove = null;
		}
		else
		{
			this.autoMove = new EntityPlayerLocal.AutoMove(this);
		}
		return this.autoMove;
	}

	// Token: 0x06002414 RID: 9236 RVA: 0x000E64E8 File Offset: 0x000E46E8
	public void TryCancelChargedAction()
	{
		ItemAction itemAction = this.inventory.holdingItem.Actions[0];
		if (itemAction is ItemActionCatapult || itemAction is ItemActionThrowAway)
		{
			itemAction.CancelAction(this.inventory.holdingItemData.actionData[0]);
			this.inventory.holdingItemData.actionData[0].HasExecuted = false;
		}
	}

	// Token: 0x06002415 RID: 9237 RVA: 0x000E6550 File Offset: 0x000E4750
	public bool TryAddRecoveryPosition(Vector3i _position)
	{
		if (this.recoveryPositions.Contains(_position))
		{
			return false;
		}
		if (!GameManager.Instance.World.CanPlayersSpawnAtPos(_position, false) || GameManager.Instance.World.GetPOIAtPosition(_position.ToVector3(), true) != null)
		{
			return false;
		}
		if (this.recoveryPositions.Count == 0)
		{
			this.recoveryPositions.Add(_position);
			return true;
		}
		if ((this.recoveryPositions[this.recoveryPositions.Count - 1] - _position).ToVector3().sqrMagnitude < 10000f)
		{
			return false;
		}
		if (this.recoveryPositions.Count >= 5)
		{
			this.recoveryPositions.RemoveAt(0);
		}
		this.recoveryPositions.Add(_position);
		return true;
	}

	// Token: 0x06002416 RID: 9238 RVA: 0x000E661C File Offset: 0x000E481C
	public void GiveExp(CraftCompleteData data)
	{
		int num = (int)this.Buffs.GetCustomVar("_craftCount_" + data.RecipeName);
		int recipeUsedCount = (int)data.RecipeUsedCount;
		this.Buffs.SetCustomVar("_craftCount_" + data.RecipeName, (float)(num + recipeUsedCount), true, CVarOperation.set);
		this.Progression.AddLevelExp(data.CraftExpGain / (num + recipeUsedCount), "_xpFromCrafting", Progression.XPTypes.Crafting, true, true);
		this.totalItemsCrafted += (uint)recipeUsedCount;
		QuestEventManager.Current.CraftedItem(data.CraftedItemStack);
		XUiC_RecipeStack.HandleCraftXPGained();
	}

	// Token: 0x06002417 RID: 9239 RVA: 0x000E66AF File Offset: 0x000E48AF
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnGUI()
	{
		this.renderManager.OnGUI();
	}

	// Token: 0x06002418 RID: 9240 RVA: 0x000E66BC File Offset: 0x000E48BC
	public override void PlayStepSound(float _volume)
	{
		if (!this.bFirstPersonView)
		{
			base.PlayStepSound(_volume);
		}
	}

	// Token: 0x06002419 RID: 9241 RVA: 0x000E66D0 File Offset: 0x000E48D0
	public void HolsterWeapon(bool holster)
	{
		EntityPlayerLocal.HolsterState holsterState = holster ? EntityPlayerLocal.HolsterState.Holstered : EntityPlayerLocal.HolsterState.Unholstered;
		if (this.desiredHolsterState != holsterState)
		{
			this.desiredHolsterState = holsterState;
		}
	}

	// Token: 0x0600241A RID: 9242 RVA: 0x000E66F5 File Offset: 0x000E48F5
	[PublicizedFrom(EAccessModifier.Private)]
	public void refreshHolsterState()
	{
		this.desiredHolsterState = this.lastHolsterState;
	}

	// Token: 0x0600241B RID: 9243 RVA: 0x000E6704 File Offset: 0x000E4904
	public void UpdateRespawn()
	{
		if (!this.RespawnTeleportTimeoutLogged && Time.realtimeSinceStartup - this.RespawnTeleportDuration > 5f)
		{
			Vector3i vector3i = World.worldToBlockPos(this.position);
			Chunk chunk = (Chunk)this.world.GetChunkFromWorldPos(vector3i.x, vector3i.y, vector3i.z);
			if (chunk == null)
			{
				Log.Error(string.Format("[FELLTHROUGHWORLD] Teleport respawn failed. Local player blockPos: {0}, Chunk is null.", vector3i));
			}
			else
			{
				long key = chunk.Key;
				Log.Out(string.Format("[FELLTHROUGHWORLD] Teleport respawn failed. Local player blockPos: {0}, Chunk Key: {1}, Chunk Pos X:{2}, Z:{3}", new object[]
				{
					vector3i,
					(chunk != null) ? chunk.Key : "null",
					WorldChunkCache.extractX(chunk.Key) << 4,
					WorldChunkCache.extractZ(chunk.Key) << 4
				}));
				Log.Out(string.Format("[FELLTHROUGHWORLD] Time since chunk {0} selected for generation: {1}", key, ChunkManager.SecondsSinceChunkSelectedForGeneration(chunk.Key)));
				ValueTuple<int, double> valueTuple = ChunkCluster.SecondsSinceChunkStartedRegeneration(chunk.Key);
				if (valueTuple.Item1 == -1)
				{
					Log.Out(string.Format("[FELLTHROUGHWORLD] Chunk {0} has never started regeneration.", key));
				}
				else
				{
					Log.Out(string.Format("[FELLTHROUGHWORLD] Time since a layer on chunk {0} started regeneration: Layer {1}, {2}", key, valueTuple.Item1, valueTuple.Item2));
				}
				valueTuple = ChunkCluster.SecondsSinceChunkEndedRegeneration(chunk.Key);
				if (valueTuple.Item1 == -1)
				{
					Log.Out(string.Format("[FELLTHROUGHWORLD] Chunk {0} has never finished regeneration.", key));
				}
				else
				{
					Log.Out(string.Format("[FELLTHROUGHWORLD] Time since a layer on chunk {0} finished regeneration: Layer {1}, {2}", key, valueTuple.Item1, valueTuple.Item2));
				}
			}
			ChunkCluster chunkCache = this.world.ChunkCache;
			chunk.LogChunkState();
			ChunkProviderGenerateWorld chunkProviderGenerateWorld = chunkCache.ChunkProvider as ChunkProviderGenerateWorld;
			if (chunkProviderGenerateWorld != null)
			{
				chunkProviderGenerateWorld.LogCurrentChunkGeneration();
			}
			ChunkManager.LogCurrentGenerationState();
			ChunkCluster.LogCurrentChunkRegenerationState();
			BacktraceUtils.SendErrorReport("fellthroughworld", "Player fell through world", null);
			this.RespawnTeleportTimeoutLogged = true;
		}
	}

	// Token: 0x0600241E RID: 9246 RVA: 0x000E6A8B File Offset: 0x000E4C8B
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator <RequestUnstuck>g__RequestUnstuckCo|265_0()
	{
		this.unstuckRequested = true;
		yield return this.moveController.UnstuckPlayerCo();
		this.unstuckRequested = false;
		yield break;
	}

	// Token: 0x04001A4B RID: 6731
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cHolsterLayerIndex = 1;

	// Token: 0x04001A4D RID: 6733
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float prevStaminaValue;

	// Token: 0x04001A4E RID: 6734
	public float weaponCrossHairAlpha = 0.8f;

	// Token: 0x04001A4F RID: 6735
	public const float cCameraTPVBaseDistance = 0.94f;

	// Token: 0x04001A50 RID: 6736
	public const float cCameraTPVOffsetMin = -0.2f;

	// Token: 0x04001A51 RID: 6737
	public const float cCameraTPVOffsetMax = 3f;

	// Token: 0x04001A52 RID: 6738
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform cameraContainerTransform;

	// Token: 0x04001A53 RID: 6739
	public Transform cameraTransform;

	// Token: 0x04001A54 RID: 6740
	public Camera playerCamera;

	// Token: 0x04001A55 RID: 6741
	public Camera finalCamera;

	// Token: 0x04001A56 RID: 6742
	public bool IsUnderwaterCamera;

	// Token: 0x04001A57 RID: 6743
	public GameRenderManager renderManager;

	// Token: 0x04001A58 RID: 6744
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public vp_FPController m_vp_FPController;

	// Token: 0x04001A59 RID: 6745
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public vp_FPCamera m_vp_FPCamera;

	// Token: 0x04001A5A RID: 6746
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public vp_FPWeapon m_vp_FPWeapon;

	// Token: 0x04001A5B RID: 6747
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool m_checked_vp_FPController;

	// Token: 0x04001A5C RID: 6748
	public WorldRayHitInfo HitInfo = new WorldRayHitInfo();

	// Token: 0x04001A5D RID: 6749
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float SPRINT_GRACE_PERIOD = 0.2f;

	// Token: 0x04001A5E RID: 6750
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float timeInSprintGrace;

	// Token: 0x04001A5F RID: 6751
	public float RespawnTeleportDuration;

	// Token: 0x04001A60 RID: 6752
	public const float RespawnTeleportTimeout = 5f;

	// Token: 0x04001A61 RID: 6753
	public bool RespawnTeleportTimeoutLogged;

	// Token: 0x04001A62 RID: 6754
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float VIBRATION_LOW = 0.25f;

	// Token: 0x04001A63 RID: 6755
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float VIBRATION_MEDIUM = 0.35f;

	// Token: 0x04001A64 RID: 6756
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float VIBRATION_HIGH = 0.5f;

	// Token: 0x04001A65 RID: 6757
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float VIBRATION_DURATION = 0.25f;

	// Token: 0x04001A66 RID: 6758
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float oldHealth;

	// Token: 0x04001A67 RID: 6759
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float vibrationTimeout = float.MaxValue;

	// Token: 0x04001A68 RID: 6760
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityPlayerLocal.HolsterState desiredHolsterState = EntityPlayerLocal.HolsterState.Undefined;

	// Token: 0x04001A69 RID: 6761
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityPlayerLocal.HolsterState lastHolsterState = EntityPlayerLocal.HolsterState.Unholstered;

	// Token: 0x04001A6A RID: 6762
	public PersistentPlayerData persistentPlayerData;

	// Token: 0x04001A6B RID: 6763
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float lastTimeJetpackDecreased;

	// Token: 0x04001A6C RID: 6764
	public bool bFirstPersonView = true;

	// Token: 0x04001A6D RID: 6765
	public bool bPreferFirstPerson = true;

	// Token: 0x04001A6E RID: 6766
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float crossHairOpenArea;

	// Token: 0x04001A6F RID: 6767
	public MovementInput movementInput = new MovementInput();

	// Token: 0x04001A70 RID: 6768
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool inputWasJump;

	// Token: 0x04001A71 RID: 6769
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool inputWasDown;

	// Token: 0x04001A72 RID: 6770
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool jumpTrigger;

	// Token: 0x04001A73 RID: 6771
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bSwitchCameraBackAfterRespawn;

	// Token: 0x04001A74 RID: 6772
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bSwitchTo3rdPersonAfterAiming;

	// Token: 0x04001A75 RID: 6773
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 selfCameraPos;

	// Token: 0x04001A76 RID: 6774
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 selfCameraSeekPos;

	// Token: 0x04001A77 RID: 6775
	public bool bExhausted;

	// Token: 0x04001A78 RID: 6776
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isExhaustedSoundAllowed = true;

	// Token: 0x04001A79 RID: 6777
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int inventorySendCounter;

	// Token: 0x04001A7A RID: 6778
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public AIDirectorPlayerInventory xmitInventory;

	// Token: 0x04001A7B RID: 6779
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ItemStack dragAndDropItem;

	// Token: 0x04001A7C RID: 6780
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isReloadCancelling;

	// Token: 0x04001A7E RID: 6782
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isLadderAttached;

	// Token: 0x04001A7F RID: 6783
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool canLadderAirAttach = true;

	// Token: 0x04001A80 RID: 6784
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool wasJumping;

	// Token: 0x04001A81 RID: 6785
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool wasJumpTrigger;

	// Token: 0x04001A82 RID: 6786
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool wasLadderAttachedJump;

	// Token: 0x04001A83 RID: 6787
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int swimMode = -1;

	// Token: 0x04001A84 RID: 6788
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int swimExhaustedTicks;

	// Token: 0x04001A85 RID: 6789
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool swimClimbing;

	// Token: 0x04001A86 RID: 6790
	public bool bLerpCameraFlag;

	// Token: 0x04001A87 RID: 6791
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lerpCameraLerpValue;

	// Token: 0x04001A88 RID: 6792
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lerpCameraStartFOV;

	// Token: 0x04001A89 RID: 6793
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lerpCameraEndFOV;

	// Token: 0x04001A8A RID: 6794
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lerpCameraFastFOV;

	// Token: 0x04001A8B RID: 6795
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastOverrideFOV = -1f;

	// Token: 0x04001A8C RID: 6796
	public float OverrideFOV = -1f;

	// Token: 0x04001A8D RID: 6797
	public Vector3 OverrideLookAt = Vector3.zero;

	// Token: 0x04001A8E RID: 6798
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float biomeVolume;

	// Token: 0x04001A8F RID: 6799
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public AudioSource audioSourceBiomeActive;

	// Token: 0x04001A90 RID: 6800
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public AudioSource audioSourceBiomeFadeOut;

	// Token: 0x04001A91 RID: 6801
	public BlendCycleTimer sneakDamageBlendTimer = new BlendCycleTimer(0.5f, 2f, 0.5f);

	// Token: 0x04001A92 RID: 6802
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string sneakDamageText = "";

	// Token: 0x04001A93 RID: 6803
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string dropInventoryBlock;

	// Token: 0x04001A94 RID: 6804
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int runTicks;

	// Token: 0x04001A95 RID: 6805
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int sprintLoopSoundPlayId = -1;

	// Token: 0x04001A96 RID: 6806
	public DynamicMusicManager DynamicMusicManager;

	// Token: 0x04001A97 RID: 6807
	public IThreatLevel ThreatLevel;

	// Token: 0x04001A98 RID: 6808
	public float LastTargetEventTime;

	// Token: 0x04001A99 RID: 6809
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Material overlayMaterial;

	// Token: 0x04001A9A RID: 6810
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public byte[] overlayAlternating = new byte[4];

	// Token: 0x04001A9B RID: 6811
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float[] overlayDirectionTime = new float[8];

	// Token: 0x04001A9C RID: 6812
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2[] overlayBloodDropsPositions = new Vector2[24];

	// Token: 0x04001A9D RID: 6813
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform uwEffectRefract;

	// Token: 0x04001A9E RID: 6814
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform uwEffectDebris;

	// Token: 0x04001A9F RID: 6815
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform uwEffectDroplets;

	// Token: 0x04001AA0 RID: 6816
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform uwEffectWaterFade;

	// Token: 0x04001AA1 RID: 6817
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform uwEffectHaze;

	// Token: 0x04001AA2 RID: 6818
	public bool bIntroAnimActive;

	// Token: 0x04001AA3 RID: 6819
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public LocalPlayerUI playerUI;

	// Token: 0x04001AA4 RID: 6820
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GUIWindowManager windowManager;

	// Token: 0x04001AA5 RID: 6821
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public NGUIWindowManager nguiWindowManager;

	// Token: 0x04001AA6 RID: 6822
	public string AreaMessage;

	// Token: 0x04001AA7 RID: 6823
	public float AreaMessageAlpha;

	// Token: 0x04001AA8 RID: 6824
	public ScreenEffects ScreenEffectManager;

	// Token: 0x04001AA9 RID: 6825
	public float GodModeSpeedModifier = 1f;

	// Token: 0x04001AAA RID: 6826
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public PlayerMoveController moveController;

	// Token: 0x04001AAB RID: 6827
	public bool isStunned;

	// Token: 0x04001AAC RID: 6828
	public bool isDeafened;

	// Token: 0x04001AAD RID: 6829
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int oldLayer = -1;

	// Token: 0x04001AAE RID: 6830
	public Rect ZombieCompassBounds;

	// Token: 0x04001AAF RID: 6831
	public float DropTimeDelay;

	// Token: 0x04001AB0 RID: 6832
	public float InteractTimeDelay;

	// Token: 0x04001AB1 RID: 6833
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float recoveryPointTimer;

	// Token: 0x04001AB2 RID: 6834
	public List<Vector3i> recoveryPositions = new List<Vector3i>();

	// Token: 0x04001AB3 RID: 6835
	public bool DebugDismembermentChance;

	// Token: 0x04001AB4 RID: 6836
	public bool BloodMoonParticipation;

	// Token: 0x04001AB5 RID: 6837
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool inAir;

	// Token: 0x04001AB6 RID: 6838
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 m_MoveVector;

	// Token: 0x04001AB7 RID: 6839
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 m_SmoothLook;

	// Token: 0x04001AB8 RID: 6840
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityPlayerLocal.MoveState moveState;

	// Token: 0x04001AB9 RID: 6841
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool moveStateAiming;

	// Token: 0x04001ABA RID: 6842
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool moveStateHoldBow;

	// Token: 0x04001ABB RID: 6843
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSwimDragBase = 0.01f;

	// Token: 0x04001ABC RID: 6844
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSwimDragScale = 5.4f;

	// Token: 0x04001ABD RID: 6845
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSwimDragPow = 2f;

	// Token: 0x04001ABE RID: 6846
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSwimAccelExhausted = 0.00025f;

	// Token: 0x04001ABF RID: 6847
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSwimAccel = 0.00032f;

	// Token: 0x04001AC0 RID: 6848
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSwimAccelRun = 0.0024f;

	// Token: 0x04001AC1 RID: 6849
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSwimAccelUp = 0.00052f;

	// Token: 0x04001AC2 RID: 6850
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSwimAccelUpGrounded = 0.05f;

	// Token: 0x04001AC3 RID: 6851
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSwimAccelDown = -0.00038f;

	// Token: 0x04001AC4 RID: 6852
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float achievementDistanceAccu;

	// Token: 0x04001AC5 RID: 6853
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastWaypointUpdateTime;

	// Token: 0x04001AC6 RID: 6854
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cWaypointUpdateTime = 30f;

	// Token: 0x04001AC7 RID: 6855
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float updateBedrollPositionChecks;

	// Token: 0x04001AC8 RID: 6856
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float updateRadiationChecks;

	// Token: 0x04001AC9 RID: 6857
	public bool LootAtMax;

	// Token: 0x04001ACA RID: 6858
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int lastLootStage = -1;

	// Token: 0x04001ACB RID: 6859
	public float InWorldPercent;

	// Token: 0x04001ACC RID: 6860
	public float InWorldLookPercent;

	// Token: 0x04001ACD RID: 6861
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastTimeUnderwater;

	// Token: 0x04001ACE RID: 6862
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool wasSpawned;

	// Token: 0x04001ACF RID: 6863
	public float spawnEffectPow = 4.19f;

	// Token: 0x04001AD0 RID: 6864
	public static float spawnInEffectSpeed = 3f;

	// Token: 0x04001AD1 RID: 6865
	public bool bPlayingSpawnIn;

	// Token: 0x04001AD2 RID: 6866
	public float spawnInTime;

	// Token: 0x04001AD3 RID: 6867
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float spawnInIntensity;

	// Token: 0x04001AD4 RID: 6868
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float deathTime;

	// Token: 0x04001AD5 RID: 6869
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool unstuckRequested;

	// Token: 0x04001AD6 RID: 6870
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Predicate<ItemStack> dropStackCondition = (ItemStack s) => s != null && !s.IsEmpty() && !s.itemValue.ItemClassOrMissing.KeepOnDeath();

	// Token: 0x04001AD7 RID: 6871
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Predicate<ItemValue> dropValueCondition = (ItemValue v) => v != null && !v.IsEmpty() && !v.ItemClassOrMissing.KeepOnDeath();

	// Token: 0x04001AD8 RID: 6872
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cDyingEffectSpeed = 600f;

	// Token: 0x04001AD9 RID: 6873
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cDyingEffectStartHealth = 70f;

	// Token: 0x04001ADA RID: 6874
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cDyingEffectHealthThreshold = 3f;

	// Token: 0x04001ADB RID: 6875
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float dyingEffectHitTime;

	// Token: 0x04001ADC RID: 6876
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float dyingEffectCur;

	// Token: 0x04001ADD RID: 6877
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float dyingEffectLast;

	// Token: 0x04001ADE RID: 6878
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float dyingEffectHealthLast;

	// Token: 0x04001ADF RID: 6879
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int fallHealth;

	// Token: 0x04001AE0 RID: 6880
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isFallDeath;

	// Token: 0x04001AE1 RID: 6881
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static List<Collider> setLayerRecursivelyList = new List<Collider>();

	// Token: 0x04001AE2 RID: 6882
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool cancellingInventoryActions;

	// Token: 0x04001AE3 RID: 6883
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public NavObject backpackNavObject;

	// Token: 0x04001AE4 RID: 6884
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<NavObject> backpackNavObjects;

	// Token: 0x04001AE5 RID: 6885
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<Vector3i> backpackPositionsFromThread = new List<Vector3i>();

	// Token: 0x04001AE6 RID: 6886
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameObject[] screenBloodEffect;

	// Token: 0x04001AE7 RID: 6887
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Material[] screenBloodMtrl;

	// Token: 0x04001AE8 RID: 6888
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool healthLostThisRound;

	// Token: 0x04001AE9 RID: 6889
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cCrosshairScreenHeightFactor = 0.059f;

	// Token: 0x04001AEA RID: 6890
	public float shelterPercent;

	// Token: 0x04001AEB RID: 6891
	public float shelterAbovePercent;

	// Token: 0x04001AEC RID: 6892
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool shelterIsUpdating;

	// Token: 0x04001AED RID: 6893
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int shelterSideCount;

	// Token: 0x04001AEE RID: 6894
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 shelterStartPos;

	// Token: 0x04001AEF RID: 6895
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 shelterPos;

	// Token: 0x04001AF0 RID: 6896
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 shelterPosOffset;

	// Token: 0x04001AF1 RID: 6897
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float shelterDir;

	// Token: 0x04001AF2 RID: 6898
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float shelterRadius;

	// Token: 0x04001AF3 RID: 6899
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cShelterSideStep = 0.13f;

	// Token: 0x04001AF4 RID: 6900
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cShelterUpDistance = 253f;

	// Token: 0x04001AF5 RID: 6901
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cShelterAngles = 8;

	// Token: 0x04001AF6 RID: 6902
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cShelterAngleStep = 45f;

	// Token: 0x04001AF7 RID: 6903
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cShelterRadiusStep = 1f;

	// Token: 0x04001AF8 RID: 6904
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cShelterRadiusMax = 3f;

	// Token: 0x04001AF9 RID: 6905
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cShelterMask = 1073807360;

	// Token: 0x04001AFA RID: 6906
	public bool isPlayerInStorm;

	// Token: 0x04001AFB RID: 6907
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public BiomeDefinition stormAlertBiomeStandingOn;

	// Token: 0x04001AFC RID: 6908
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float stormRemainingSeconds;

	// Token: 0x04001AFD RID: 6909
	public bool isIndoorsCurrent = true;

	// Token: 0x04001AFE RID: 6910
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public BiomeDefinition.WeatherGroup weatherGroup;

	// Token: 0x04001AFF RID: 6911
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string weatherBuff;

	// Token: 0x04001B00 RID: 6912
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityPlayerLocal.AutoMove autoMove;

	// Token: 0x02000458 RID: 1112
	[PublicizedFrom(EAccessModifier.Private)]
	public enum HolsterState
	{
		// Token: 0x04001B02 RID: 6914
		Holstered,
		// Token: 0x04001B03 RID: 6915
		Unholstered,
		// Token: 0x04001B04 RID: 6916
		Undefined
	}

	// Token: 0x02000459 RID: 1113
	[PublicizedFrom(EAccessModifier.Private)]
	public enum MoveState
	{
		// Token: 0x04001B06 RID: 6918
		None,
		// Token: 0x04001B07 RID: 6919
		Off,
		// Token: 0x04001B08 RID: 6920
		Attached,
		// Token: 0x04001B09 RID: 6921
		Idle,
		// Token: 0x04001B0A RID: 6922
		Walk,
		// Token: 0x04001B0B RID: 6923
		Run,
		// Token: 0x04001B0C RID: 6924
		Swim,
		// Token: 0x04001B0D RID: 6925
		Crouch,
		// Token: 0x04001B0E RID: 6926
		CrouchWalk,
		// Token: 0x04001B0F RID: 6927
		CrouchRun,
		// Token: 0x04001B10 RID: 6928
		Jump
	}

	// Token: 0x0200045A RID: 1114
	[PublicizedFrom(EAccessModifier.Protected)]
	public enum DropOption
	{
		// Token: 0x04001B12 RID: 6930
		None,
		// Token: 0x04001B13 RID: 6931
		All,
		// Token: 0x04001B14 RID: 6932
		Toolbelt,
		// Token: 0x04001B15 RID: 6933
		Backpack,
		// Token: 0x04001B16 RID: 6934
		DeleteAll
	}

	// Token: 0x0200045B RID: 1115
	public class AutoMove
	{
		// Token: 0x0600241F RID: 9247 RVA: 0x000E6A9A File Offset: 0x000E4C9A
		public AutoMove(Entity _entity)
		{
			this.entity = _entity;
		}

		// Token: 0x06002420 RID: 9248 RVA: 0x000E6AAC File Offset: 0x000E4CAC
		public void StartLine(float _duration, int _loopCount, Vector3 _endPos)
		{
			this.mode = EntityPlayerLocal.AutoMove.Mode.Line;
			this.endTime = _duration;
			this.loopCount = _loopCount;
			this.startPos = this.entity.position;
			this.targetPos = _endPos;
			this.isPingPong = false;
			if (this.loopCount < 0)
			{
				this.loopCount *= -2;
				this.isPingPong = true;
			}
		}

		// Token: 0x06002421 RID: 9249 RVA: 0x000E6B0C File Offset: 0x000E4D0C
		public void StartOrbit(float _duration, int _loopCount, Vector3 _orbitPos)
		{
			this.mode = EntityPlayerLocal.AutoMove.Mode.Orbit;
			this.endTime = _duration;
			this.loopCount = _loopCount;
			this.startPos = this.entity.position;
			this.targetPos = _orbitPos;
			this.isPingPong = false;
			if (this.loopCount < 0)
			{
				this.loopCount *= -2;
				this.isPingPong = true;
			}
		}

		// Token: 0x06002422 RID: 9250 RVA: 0x000E6B6C File Offset: 0x000E4D6C
		public void StartRelative(float velX, float velZ, float rotVel)
		{
			this.mode = EntityPlayerLocal.AutoMove.Mode.Relative;
			this.vel.x = velX;
			this.vel.z = velZ;
			this.rotY = this.entity.rotation.y;
			this.rotYVel = rotVel;
		}

		// Token: 0x06002423 RID: 9251 RVA: 0x000E6BAA File Offset: 0x000E4DAA
		public void SetLookAt(Vector3 _pos)
		{
			this.lookAtPos = _pos;
		}

		// Token: 0x06002424 RID: 9252 RVA: 0x000E6BB4 File Offset: 0x000E4DB4
		public void Update()
		{
			if (this.mode == EntityPlayerLocal.AutoMove.Mode.Off)
			{
				return;
			}
			if (this.mode == EntityPlayerLocal.AutoMove.Mode.Line)
			{
				this.curTime += Time.deltaTime;
				float num = this.curTime / this.endTime;
				if (num > 1f)
				{
					this.curTime = 0f;
					num = 0f;
					if (this.isPingPong)
					{
						Vector3 vector = this.startPos;
						Vector3 vector2 = this.targetPos;
						this.targetPos = vector;
						this.startPos = vector2;
					}
					int num2 = this.loopCount - 1;
					this.loopCount = num2;
					if (num2 <= 0)
					{
						num = (float)(this.isPingPong ? 0 : 1);
						this.mode = EntityPlayerLocal.AutoMove.Mode.Off;
					}
				}
				Vector3 pos = Vector3.Lerp(this.startPos, this.targetPos, num);
				this.entity.SetPosition(pos, true);
			}
			if (this.mode == EntityPlayerLocal.AutoMove.Mode.Orbit)
			{
				this.curTime += Time.deltaTime;
				float num3 = this.curTime / this.endTime;
				if (num3 > 1f)
				{
					this.curTime -= this.endTime;
					num3 = this.curTime / this.endTime;
					if (this.isPingPong)
					{
						this.isFlipped = !this.isFlipped;
					}
					int num2 = this.loopCount - 1;
					this.loopCount = num2;
					if (num2 <= 0)
					{
						num3 = 1f;
						this.mode = EntityPlayerLocal.AutoMove.Mode.Off;
					}
				}
				Vector3 point = this.startPos - this.targetPos;
				float num4 = 360f * num3;
				num4 *= (float)(this.isFlipped ? -1 : 1);
				this.entity.SetPosition(this.targetPos + Quaternion.Euler(0f, num4, 0f) * point, true);
				Vector3 eulerAngles = Quaternion.LookRotation(this.targetPos - this.entity.position, Vector3.up).eulerAngles;
				eulerAngles.x *= -1f;
				this.entity.SetRotation(eulerAngles);
			}
			if (this.mode == EntityPlayerLocal.AutoMove.Mode.Relative)
			{
				this.rotY += this.rotYVel * Time.deltaTime;
				this.entity.SetRotation(new Vector3(0f, this.rotY, 0f));
				this.entity.SetPosition(this.entity.position + Quaternion.Euler(0f, this.rotY, 0f) * this.vel * Time.deltaTime, true);
			}
			if (this.lookAtPos.sqrMagnitude > 0f)
			{
				Vector3 eulerAngles2 = Quaternion.LookRotation(this.lookAtPos - this.entity.position, Vector3.up).eulerAngles;
				eulerAngles2.x *= -1f;
				this.entity.SetRotation(eulerAngles2);
			}
		}

		// Token: 0x04001B17 RID: 6935
		[PublicizedFrom(EAccessModifier.Private)]
		public Entity entity;

		// Token: 0x04001B18 RID: 6936
		[PublicizedFrom(EAccessModifier.Private)]
		public EntityPlayerLocal.AutoMove.Mode mode;

		// Token: 0x04001B19 RID: 6937
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3 startPos;

		// Token: 0x04001B1A RID: 6938
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3 targetPos;

		// Token: 0x04001B1B RID: 6939
		[PublicizedFrom(EAccessModifier.Private)]
		public float curTime;

		// Token: 0x04001B1C RID: 6940
		[PublicizedFrom(EAccessModifier.Private)]
		public float endTime;

		// Token: 0x04001B1D RID: 6941
		[PublicizedFrom(EAccessModifier.Private)]
		public int loopCount;

		// Token: 0x04001B1E RID: 6942
		[PublicizedFrom(EAccessModifier.Private)]
		public bool isPingPong;

		// Token: 0x04001B1F RID: 6943
		[PublicizedFrom(EAccessModifier.Private)]
		public bool isFlipped;

		// Token: 0x04001B20 RID: 6944
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3 vel;

		// Token: 0x04001B21 RID: 6945
		[PublicizedFrom(EAccessModifier.Private)]
		public float rotY;

		// Token: 0x04001B22 RID: 6946
		[PublicizedFrom(EAccessModifier.Private)]
		public float rotYVel;

		// Token: 0x04001B23 RID: 6947
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3 lookAtPos;

		// Token: 0x0200045C RID: 1116
		[PublicizedFrom(EAccessModifier.Private)]
		public enum Mode
		{
			// Token: 0x04001B25 RID: 6949
			Off,
			// Token: 0x04001B26 RID: 6950
			Line,
			// Token: 0x04001B27 RID: 6951
			Orbit,
			// Token: 0x04001B28 RID: 6952
			Relative
		}
	}
}
