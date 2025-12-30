using System;
using System.Collections;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000E55 RID: 3669
[Preserve]
public class XUiC_SpawnSelectionWindow : XUiController
{
	// Token: 0x17000BAC RID: 2988
	// (get) Token: 0x06007327 RID: 29479 RVA: 0x002EF589 File Offset: 0x002ED789
	// (set) Token: 0x06007328 RID: 29480 RVA: 0x002EF591 File Offset: 0x002ED791
	public XUiC_SpawnSelectionWindow.ESpawnWindowMode WindowMode
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.windowMode;
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			if (this.windowMode != value)
			{
				this.windowMode = value;
				this.IsDirty = true;
			}
		}
	}

	// Token: 0x06007329 RID: 29481 RVA: 0x002EF5AA File Offset: 0x002ED7AA
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "spawn_sound")
		{
			base.xui.LoadData<AudioClip>(_value, delegate(AudioClip _clip)
			{
				this.spawnReadyClip = _clip;
			});
			return true;
		}
		return base.ParseAttribute(_name, _value, _parent);
	}

	// Token: 0x0600732A RID: 29482 RVA: 0x002EF5DC File Offset: 0x002ED7DC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(_bindingName);
		if (num <= 3003985336U)
		{
			if (num != 79689725U)
			{
				if (num != 848152551U)
				{
					if (num == 3003985336U)
					{
						if (_bindingName == "progressVisible")
						{
							_value = (this.WindowMode == XUiC_SpawnSelectionWindow.ESpawnWindowMode.Progress).ToString();
							return true;
						}
					}
				}
				else if (_bindingName == "firstTimeSpawn")
				{
					_value = this.bFirstTimeSpawn.ToString();
					return true;
				}
			}
			else if (_bindingName == "hasBedroll")
			{
				_value = this.bHasBedroll.ToString();
				return true;
			}
		}
		else if (num <= 3221349377U)
		{
			if (num != 3073415995U)
			{
				if (num == 3221349377U)
				{
					if (_bindingName == "enteringGame")
					{
						_value = this.bEnteringGame.ToString();
						return true;
					}
				}
			}
			else if (_bindingName == "showNearFriends")
			{
				if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
				{
					_value = false.ToString();
				}
				else if (XUiC_SpawnNearFriendsList.SpawnNearFriendMode == AllowSpawnNearFriend.Disabled)
				{
					_value = false.ToString();
				}
				else
				{
					_value = this.bFirstTimeSpawn.ToString();
				}
				return true;
			}
		}
		else if (num != 4053612733U)
		{
			if (num == 4166775256U)
			{
				if (_bindingName == "buttonsVisible")
				{
					_value = (this.WindowMode == XUiC_SpawnSelectionWindow.ESpawnWindowMode.SpawnSelection).ToString();
					return true;
				}
			}
		}
		else if (_bindingName == "hasBackpack")
		{
			GameServerInfo currentGameServerInfoServerOrClient = SingletonMonoBehaviour<ConnectionManager>.Instance.CurrentGameServerInfoServerOrClient;
			bool flag = currentGameServerInfoServerOrClient == null || currentGameServerInfoServerOrClient.GetValue(GameInfoBool.AllowSpawnNearBackpack);
			_value = (flag && this.bHasBackpack).ToString();
			return true;
		}
		return base.GetBindingValueInternal(ref _value, _bindingName);
	}

	// Token: 0x0600732B RID: 29483 RVA: 0x002EF7A0 File Offset: 0x002ED9A0
	public override void Init()
	{
		base.Init();
		XUiC_SpawnSelectionWindow.ID = base.WindowGroup.ID;
		XUiV_Label label = (XUiV_Label)base.GetChildById("lblProgress").ViewComponent;
		this.ellipsisAnimator = new TextEllipsisAnimator(null, label);
		XUiC_SimpleButton xuiC_SimpleButton = base.GetChildById("btnLeave") as XUiC_SimpleButton;
		if (xuiC_SimpleButton != null)
		{
			xuiC_SimpleButton.OnPressed += delegate(XUiController _, int _)
			{
				base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
				GameManager.Instance.Disconnect();
			};
		}
		XUiC_SimpleButton xuiC_SimpleButton2 = base.GetChildById("btnSpawnFirstTime") as XUiC_SimpleButton;
		if (xuiC_SimpleButton2 != null)
		{
			xuiC_SimpleButton2.OnPressed += delegate(XUiController _, int _)
			{
				this.SpawnButtonPressed(SpawnMethod.Invalid, -1);
			};
		}
		XUiC_SimpleButton xuiC_SimpleButton3 = base.GetChildById("btnSpawn") as XUiC_SimpleButton;
		if (xuiC_SimpleButton3 != null)
		{
			xuiC_SimpleButton3.OnPressed += delegate(XUiController _, int _)
			{
				this.SpawnButtonPressed(SpawnMethod.Invalid, -1);
			};
		}
		XUiC_SimpleButton xuiC_SimpleButton4 = base.GetChildById("btnRespawn") as XUiC_SimpleButton;
		if (xuiC_SimpleButton4 != null)
		{
			xuiC_SimpleButton4.OnPressed += delegate(XUiController _, int _)
			{
				if (GameStats.GetInt(EnumGameStats.DeathPenalty) == 3)
				{
					this.SpawnButtonPressed(SpawnMethod.NewRandomSpawn, -1);
					return;
				}
				this.SpawnButtonPressed(SpawnMethod.NearDeath, -1);
			};
		}
		XUiC_SimpleButton xuiC_SimpleButton5 = base.GetChildById("btnNearBackpack") as XUiC_SimpleButton;
		if (xuiC_SimpleButton5 != null)
		{
			xuiC_SimpleButton5.OnPressed += delegate(XUiController _, int _)
			{
				this.SpawnButtonPressed(SpawnMethod.NearBackpack, -1);
			};
		}
		XUiC_SimpleButton xuiC_SimpleButton6 = base.GetChildById("btnOnBedroll") as XUiC_SimpleButton;
		if (xuiC_SimpleButton6 != null)
		{
			xuiC_SimpleButton6.OnPressed += delegate(XUiController _, int _)
			{
				this.SpawnButtonPressed(SpawnMethod.OnBedRoll, -1);
			};
		}
		XUiC_SimpleButton xuiC_SimpleButton7 = base.GetChildById("btnNearBedroll") as XUiC_SimpleButton;
		if (xuiC_SimpleButton7 != null)
		{
			xuiC_SimpleButton7.OnPressed += delegate(XUiController _, int _)
			{
				this.SpawnButtonPressed(SpawnMethod.NearBedroll, -1);
			};
		}
		XUiC_SpawnNearFriendsList childByType = base.GetChildByType<XUiC_SpawnNearFriendsList>();
		if (childByType != null)
		{
			childByType.SpawnClicked += delegate(PersistentPlayerData _ppd)
			{
				this.SpawnButtonPressed(SpawnMethod.NearFriend, _ppd.EntityId);
			};
		}
	}

	// Token: 0x0600732C RID: 29484 RVA: 0x002EF91C File Offset: 0x002EDB1C
	[PublicizedFrom(EAccessModifier.Private)]
	public void RefreshButtons()
	{
		if (this.WindowMode == XUiC_SpawnSelectionWindow.ESpawnWindowMode.Progress && !this.bEnteringGame)
		{
			return;
		}
		if (this.bEnteringGame)
		{
			return;
		}
		World world = GameManager.Instance.World;
		EntityPlayerLocal entityPlayerLocal = (world != null) ? world.GetPrimaryPlayer() : null;
		if (entityPlayerLocal == null)
		{
			Log.Warning("Refresh buttons cannot process without an EntityPlayerLocal");
			return;
		}
		SpawnPosition spawnPoint;
		Vector3i one;
		if (entityPlayerLocal != null)
		{
			spawnPoint = entityPlayerLocal.GetSpawnPoint();
			one = entityPlayerLocal.GetLastDroppedBackpackPosition();
		}
		else
		{
			spawnPoint = new SpawnPosition(Vector3.forward, 0f);
			one = Vector3i.down;
		}
		this.bHasBedroll = !spawnPoint.IsUndef();
		this.bHasBackpack = (one != Vector3i.zero);
	}

	// Token: 0x0600732D RID: 29485 RVA: 0x002EF9C0 File Offset: 0x002EDBC0
	[PublicizedFrom(EAccessModifier.Private)]
	public void SpawnButtonPressed(SpawnMethod _method, int _nearEntityId = -1)
	{
		base.xui.playerUI.xui.PlayMenuConfirmSound();
		this.spawnMethod = _method;
		if (this.bEnteringGame)
		{
			this.WindowMode = XUiC_SpawnSelectionWindow.ESpawnWindowMode.Progress;
			base.xui.playerUI.CursorController.SetNavigationTarget(null);
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				GameManager.Instance.canSpawnPlayer = true;
				return;
			}
			GameManager.Instance.RequestToSpawn(_nearEntityId);
			if (_method == SpawnMethod.NearFriend)
			{
				ThreadManager.StartCoroutine(this.sendFriendRequest(_nearEntityId));
				return;
			}
		}
		else
		{
			World world = GameManager.Instance.World;
			EntityPlayerLocal entityPlayerLocal = (world != null) ? world.GetPrimaryPlayer() : null;
			if (entityPlayerLocal == null)
			{
				Log.Warning("Spawn button cannot process without an EntityPlayerLocal");
				return;
			}
			SpawnPosition spawnPoint;
			Vector3i blockPos;
			Vector3 position;
			if (entityPlayerLocal != null)
			{
				spawnPoint = entityPlayerLocal.GetSpawnPoint();
				blockPos = entityPlayerLocal.GetLastDroppedBackpackPosition();
				position = entityPlayerLocal.position;
			}
			else
			{
				spawnPoint = new SpawnPosition(Vector3.forward, 0f);
				blockPos = Vector3i.down;
				position = Vector3.down;
			}
			SpawnPosition spawnPosition = new SpawnPosition(blockPos, 0f);
			SpawnPosition spawnPosition2 = new SpawnPosition(position, 0f);
			SpawnPosition randomSpawnPosition = GameManager.Instance.GetSpawnPointList().GetRandomSpawnPosition(entityPlayerLocal.world, null, 0, 0);
			SpawnPosition spawnPosition3;
			switch (this.spawnMethod)
			{
			case SpawnMethod.Invalid:
				spawnPosition3 = SpawnPosition.Undef;
				goto IL_183;
			case SpawnMethod.NewRandomSpawn:
				spawnPosition3 = randomSpawnPosition;
				goto IL_183;
			case SpawnMethod.NearDeath:
				spawnPosition3 = spawnPosition2;
				goto IL_183;
			case SpawnMethod.OnBedRoll:
				spawnPosition3 = spawnPoint;
				goto IL_183;
			case SpawnMethod.NearBedroll:
				spawnPosition3 = spawnPoint;
				goto IL_183;
			case SpawnMethod.NearBackpack:
				spawnPosition3 = spawnPosition;
				goto IL_183;
			case SpawnMethod.Unstuck:
				spawnPosition3 = SpawnPosition.Undef;
				goto IL_183;
			}
			throw new ArgumentOutOfRangeException();
			IL_183:
			this.spawnTarget = spawnPosition3;
			base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
		}
	}

	// Token: 0x0600732E RID: 29486 RVA: 0x002EFB78 File Offset: 0x002EDD78
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator sendFriendRequest(int _entityId)
	{
		if (_entityId == -1)
		{
			yield break;
		}
		while (GameManager.Instance.World != null)
		{
			EntityPlayerLocal epl = GameManager.Instance.World.GetPrimaryPlayer();
			yield return null;
			if (!(epl == null) && epl.Spawned)
			{
				EntityPlayer entityPlayer = GameManager.Instance.World.GetEntity(_entityId) as EntityPlayer;
				if (entityPlayer != null)
				{
					if (!entityPlayer.partyInvites.Contains(epl))
					{
						entityPlayer.AddPartyInvite(epl.entityId);
					}
					NetPackagePartyActions package = NetPackageManager.GetPackage<NetPackagePartyActions>().Setup(NetPackagePartyActions.PartyActions.SendInvite, epl.entityId, _entityId, null, null);
					if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
					{
						SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(package, false);
					}
					else
					{
						SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, false, -1, -1, -1, null, 192, false);
					}
				}
				epl.PlayerUI.xui.GetChildByType<XUiC_PlayersList>().AddInvitePress(_entityId);
				yield break;
			}
		}
		yield break;
	}

	// Token: 0x0600732F RID: 29487 RVA: 0x002EFB88 File Offset: 0x002EDD88
	public override void Update(float _dt)
	{
		base.Update(_dt);
		this.ellipsisAnimator.GetNextAnimatedString(_dt);
		if (this.IsDirty)
		{
			this.RefreshButtons();
			base.RefreshBindings(true);
			if (base.xui.playerUI.CursorController.navigationTarget == null && !base.xui.playerUI.CursorController.CursorModeActive)
			{
				base.GetChildById("content").SelectCursorElement(true, false);
			}
			this.IsDirty = false;
		}
		this.updateLoadState();
	}

	// Token: 0x06007330 RID: 29488 RVA: 0x002EFC0C File Offset: 0x002EDE0C
	public override void OnOpen()
	{
		base.OnOpen();
		this.delayCountdownTime = 1f;
		this.spawnMethod = SpawnMethod.Invalid;
		this.ellipsisAnimator.SetBaseString(Localization.Get("msgBuildingEnvironment", false), TextEllipsisAnimator.AnimationMode.All);
		this.showSpawningComponents(XUiC_SpawnSelectionWindow.ESpawnWindowMode.Progress);
		base.RefreshBindings(false);
		((XUiV_Window)base.ViewComponent).Panel.alpha = 1f;
		this.setCursor = false;
		base.xui.playerUI.CursorController.SetCursorHidden(true);
	}

	// Token: 0x06007331 RID: 29489 RVA: 0x002EFC8D File Offset: 0x002EDE8D
	public override void OnClose()
	{
		base.OnClose();
		this.bChooseSpawnPosition = false;
		base.xui.playerUI.CursorController.SetCursorHidden(false);
		this.setCursor = false;
	}

	// Token: 0x06007332 RID: 29490 RVA: 0x002EFCBC File Offset: 0x002EDEBC
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateLoadState()
	{
		if (!GameManager.Instance.gameStateManager.IsGameStarted())
		{
			if (this.bEnteringGame)
			{
				this.showSpawningComponents(XUiC_SpawnSelectionWindow.ESpawnWindowMode.SpawnSelection);
			}
			return;
		}
		if (GameStats.GetInt(EnumGameStats.GameState) == 2)
		{
			base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
			return;
		}
		if (this.delayCountdownTime > 0f)
		{
			this.delayCountdownTime -= Time.deltaTime;
			return;
		}
		int displayedChunkGameObjectsCount = GameManager.Instance.World.m_ChunkManager.GetDisplayedChunkGameObjectsCount();
		int viewDistance = GameUtils.GetViewDistance();
		int num = (!GameManager.Instance.World.ChunkCache.IsFixedSize) ? (viewDistance * viewDistance - 10) : 0;
		if (displayedChunkGameObjectsCount < num)
		{
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				this.ellipsisAnimator.SetBaseString(Localization.Get("msgStartingGame", false), TextEllipsisAnimator.AnimationMode.All);
				return;
			}
		}
		else if (DistantTerrain.Instance != null && !DistantTerrain.Instance.IsTerrainReady)
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
			{
				this.ellipsisAnimator.SetBaseString(Localization.Get("msgGeneratingDistantTerrain", false), TextEllipsisAnimator.AnimationMode.All);
				return;
			}
		}
		else
		{
			if (!LocalPlayerUI.GetUIForPrimaryPlayer().xui.isReady)
			{
				this.ellipsisAnimator.SetBaseString(Localization.Get("msgLoadingUI", false), TextEllipsisAnimator.AnimationMode.All);
				return;
			}
			if (this.bChooseSpawnPosition && GameManager.Instance.World.GetPrimaryPlayer() != null)
			{
				this.showSpawningComponents(XUiC_SpawnSelectionWindow.ESpawnWindowMode.SpawnSelection);
				return;
			}
			base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
		}
	}

	// Token: 0x06007333 RID: 29491 RVA: 0x002EFE44 File Offset: 0x002EE044
	[PublicizedFrom(EAccessModifier.Private)]
	public void showSpawningComponents(XUiC_SpawnSelectionWindow.ESpawnWindowMode _windowMode)
	{
		GameManager.Instance.SetCursorEnabledOverride(false, false);
		if (!this.setCursor)
		{
			this.WindowMode = _windowMode;
			base.xui.playerUI.CursorController.SetCursorHidden(false);
			this.setCursor = true;
			if (this.WindowMode == XUiC_SpawnSelectionWindow.ESpawnWindowMode.SpawnSelection)
			{
				Manager.PlayXUiSound(this.spawnReadyClip, 1f);
			}
		}
	}

	// Token: 0x06007334 RID: 29492 RVA: 0x002EFEA2 File Offset: 0x002EE0A2
	public static void Open(LocalPlayerUI _playerUi, bool _chooseSpawnPosition, bool _enteringGame, bool _firstTimeSpawn = false)
	{
		XUiC_SpawnSelectionWindow childByType = _playerUi.xui.FindWindowGroupByName(XUiC_SpawnSelectionWindow.ID).GetChildByType<XUiC_SpawnSelectionWindow>();
		childByType.bChooseSpawnPosition = _chooseSpawnPosition;
		childByType.bEnteringGame = _enteringGame;
		childByType.bFirstTimeSpawn = _firstTimeSpawn;
		_playerUi.windowManager.Open(XUiC_SpawnSelectionWindow.ID, true, true, true);
	}

	// Token: 0x06007335 RID: 29493 RVA: 0x002EFEE0 File Offset: 0x002EE0E0
	public static void Close(LocalPlayerUI _playerUi)
	{
		_playerUi.windowManager.CloseIfOpen(XUiC_SpawnSelectionWindow.ID);
	}

	// Token: 0x06007336 RID: 29494 RVA: 0x002EFEF2 File Offset: 0x002EE0F2
	public static bool IsOpenInUI(LocalPlayerUI _playerUi)
	{
		return _playerUi.windowManager.IsWindowOpen(XUiC_SpawnSelectionWindow.ID);
	}

	// Token: 0x06007337 RID: 29495 RVA: 0x002EFF04 File Offset: 0x002EE104
	public static bool IsInSpawnSelection(LocalPlayerUI _playerUi)
	{
		return XUiC_SpawnSelectionWindow.IsOpenInUI(_playerUi) && XUiC_SpawnSelectionWindow.GetWindow(_playerUi).WindowMode == XUiC_SpawnSelectionWindow.ESpawnWindowMode.SpawnSelection;
	}

	// Token: 0x06007338 RID: 29496 RVA: 0x002EFF1E File Offset: 0x002EE11E
	public static XUiC_SpawnSelectionWindow GetWindow(LocalPlayerUI _playerUi)
	{
		return _playerUi.xui.FindWindowGroupByName(XUiC_SpawnSelectionWindow.ID).GetChildByType<XUiC_SpawnSelectionWindow>();
	}

	// Token: 0x040057B1 RID: 22449
	public static string ID = "";

	// Token: 0x040057B2 RID: 22450
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SpawnSelectionWindow.ESpawnWindowMode windowMode;

	// Token: 0x040057B3 RID: 22451
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bChooseSpawnPosition;

	// Token: 0x040057B4 RID: 22452
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bEnteringGame;

	// Token: 0x040057B5 RID: 22453
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bFirstTimeSpawn;

	// Token: 0x040057B6 RID: 22454
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bHasBedroll;

	// Token: 0x040057B7 RID: 22455
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bHasBackpack;

	// Token: 0x040057B8 RID: 22456
	[PublicizedFrom(EAccessModifier.Private)]
	public float delayCountdownTime;

	// Token: 0x040057B9 RID: 22457
	[PublicizedFrom(EAccessModifier.Private)]
	public AudioClip spawnReadyClip;

	// Token: 0x040057BA RID: 22458
	public SpawnMethod spawnMethod;

	// Token: 0x040057BB RID: 22459
	public SpawnPosition spawnTarget;

	// Token: 0x040057BC RID: 22460
	[PublicizedFrom(EAccessModifier.Private)]
	public bool setCursor;

	// Token: 0x040057BD RID: 22461
	[PublicizedFrom(EAccessModifier.Private)]
	public TextEllipsisAnimator ellipsisAnimator;

	// Token: 0x02000E56 RID: 3670
	[PublicizedFrom(EAccessModifier.Private)]
	public enum ESpawnWindowMode
	{
		// Token: 0x040057BF RID: 22463
		Progress,
		// Token: 0x040057C0 RID: 22464
		SpawnSelection
	}
}
