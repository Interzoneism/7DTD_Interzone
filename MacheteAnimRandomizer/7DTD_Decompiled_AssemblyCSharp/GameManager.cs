using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;
using Audio;
using GUI_2;
using InControl;
using Platform;
using Twitch;
using Unity.Collections;
using UnityEngine;

// Token: 0x02000F4F RID: 3919
public class GameManager : MonoBehaviour, IGameManager
{
	// Token: 0x06007C98 RID: 31896 RVA: 0x0032595B File Offset: 0x00323B5B
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator waitForGameStart()
	{
		if (!GameManager.IsDedicatedServer && !this.IsEditMode())
		{
			EntityPlayerLocal epl = null;
			while (this.World != null)
			{
				epl = this.World.GetPrimaryPlayer();
				if (epl != null)
				{
					while (!epl.IsSpawned())
					{
						yield return null;
					}
					epl.HasUpdated = false;
					while (!epl.HasUpdated)
					{
						yield return false;
					}
					yield return null;
					this.GameHasStarted = true;
					epl = null;
					goto IL_10F;
				}
				yield return null;
			}
			yield break;
		}
		IL_10F:
		yield break;
	}

	// Token: 0x06007C99 RID: 31897 RVA: 0x0032596C File Offset: 0x00323B6C
	public void ShowBackground(bool show)
	{
		if (this.bShowBackground == show)
		{
			return;
		}
		this.bShowBackground = show;
		Camera main = Camera.main;
		if (main != null)
		{
			if (!this.bShowBackground)
			{
				this.cameraCullMask = main.cullingMask;
				main.cullingMask = LayerMask.GetMask(new string[]
				{
					"LocalPlayer"
				});
				main.backgroundColor = this.backgroundColor;
				return;
			}
			main.cullingMask = this.cameraCullMask;
		}
	}

	// Token: 0x06007C9A RID: 31898 RVA: 0x003259DF File Offset: 0x00323BDF
	public bool ShowBackground()
	{
		return this.bShowBackground;
	}

	// Token: 0x06007C9B RID: 31899 RVA: 0x003259E8 File Offset: 0x00323BE8
	public void IncreaseBackgroundColor()
	{
		switch (this.currentBackgroundColorChannel)
		{
		case 0:
			this.backgroundColor.r = this.backgroundColor.r + 0.003921569f;
			break;
		case 1:
			this.backgroundColor.g = this.backgroundColor.g + 0.003921569f;
			break;
		case 2:
			this.backgroundColor.b = this.backgroundColor.b + 0.003921569f;
			break;
		}
		this.backgroundColor.r = Mathf.Clamp01(this.backgroundColor.r);
		this.backgroundColor.g = Mathf.Clamp01(this.backgroundColor.g);
		this.backgroundColor.b = Mathf.Clamp01(this.backgroundColor.b);
		Camera main = Camera.main;
		if (main != null)
		{
			main.backgroundColor = this.backgroundColor;
		}
	}

	// Token: 0x06007C9C RID: 31900 RVA: 0x00325ABC File Offset: 0x00323CBC
	public void DecreaseBackgroundColor()
	{
		switch (this.currentBackgroundColorChannel)
		{
		case 0:
			this.backgroundColor.r = this.backgroundColor.r - 0.003921569f;
			break;
		case 1:
			this.backgroundColor.g = this.backgroundColor.g - 0.003921569f;
			break;
		case 2:
			this.backgroundColor.b = this.backgroundColor.b - 0.003921569f;
			break;
		}
		this.backgroundColor.r = Mathf.Clamp01(this.backgroundColor.r);
		this.backgroundColor.g = Mathf.Clamp01(this.backgroundColor.g);
		this.backgroundColor.b = Mathf.Clamp01(this.backgroundColor.b);
		Camera main = Camera.main;
		if (main != null)
		{
			main.backgroundColor = this.backgroundColor;
		}
	}

	// Token: 0x06007C9D RID: 31901 RVA: 0x00325B90 File Offset: 0x00323D90
	public void BackgroundColorNext()
	{
		this.currentBackgroundColorChannel++;
		if (this.currentBackgroundColorChannel > 2)
		{
			this.currentBackgroundColorChannel = 0;
		}
	}

	// Token: 0x06007C9E RID: 31902 RVA: 0x00325BB0 File Offset: 0x00323DB0
	public void BackgroundColorPrev()
	{
		this.currentBackgroundColorChannel--;
		if (this.currentBackgroundColorChannel < 0)
		{
			this.currentBackgroundColorChannel = 2;
		}
	}

	// Token: 0x17000D0D RID: 3341
	// (get) Token: 0x06007C9F RID: 31903 RVA: 0x00325BD0 File Offset: 0x00323DD0
	public static bool IsDedicatedServer
	{
		get
		{
			if (!GameManager.isDedicatedChecked)
			{
				string[] commandLineArgs = GameStartupHelper.GetCommandLineArgs();
				for (int i = 0; i < commandLineArgs.Length; i++)
				{
					if (commandLineArgs[i].Equals(Constants.cArgDedicatedServer))
					{
						GameManager.isDedicated = true;
					}
				}
				GameManager.isDedicatedChecked = true;
			}
			return GameManager.isDedicated;
		}
	}

	// Token: 0x06007CA0 RID: 31904 RVA: 0x00325C18 File Offset: 0x00323E18
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnUserDetailsUpdated(IPlatformUserData userData, string name)
	{
		if (this.persistentPlayers != null)
		{
			this.persistentPlayers.HandlePlayerDetailsUpdate(userData, name);
		}
	}

	// Token: 0x17000D0E RID: 3342
	// (get) Token: 0x06007CA1 RID: 31905 RVA: 0x00325C2F File Offset: 0x00323E2F
	// (set) Token: 0x06007CA2 RID: 31906 RVA: 0x00325C37 File Offset: 0x00323E37
	public bool GameIsFocused { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000D0F RID: 3343
	// (get) Token: 0x06007CA3 RID: 31907 RVA: 0x00325C40 File Offset: 0x00323E40
	public bool IsMouseCursorVisible
	{
		get
		{
			return PlatformManager.NativePlatform.Input.CurrentInputStyle == PlayerInputManager.InputStyle.Keyboard && Cursor.visible;
		}
	}

	// Token: 0x140000E4 RID: 228
	// (add) Token: 0x06007CA4 RID: 31908 RVA: 0x00325C5C File Offset: 0x00323E5C
	// (remove) Token: 0x06007CA5 RID: 31909 RVA: 0x00325C94 File Offset: 0x00323E94
	public event GameManager.OnWorldChangedEvent OnWorldChanged;

	// Token: 0x140000E5 RID: 229
	// (add) Token: 0x06007CA6 RID: 31910 RVA: 0x00325CCC File Offset: 0x00323ECC
	// (remove) Token: 0x06007CA7 RID: 31911 RVA: 0x00325D04 File Offset: 0x00323F04
	public event GameManager.OnLocalPlayerChangedEvent OnLocalPlayerChanged;

	// Token: 0x140000E6 RID: 230
	// (add) Token: 0x06007CA8 RID: 31912 RVA: 0x00325D3C File Offset: 0x00323F3C
	// (remove) Token: 0x06007CA9 RID: 31913 RVA: 0x00325D74 File Offset: 0x00323F74
	public event Action<ClientInfo> OnClientSpawned;

	// Token: 0x06007CAA RID: 31914 RVA: 0x00325DA9 File Offset: 0x00323FA9
	public void ApplyAllOptions()
	{
		if (this.windowManager != null)
		{
			GameOptionsManager.ApplyAllOptions(this.windowManager.playerUI);
		}
	}

	// Token: 0x06007CAB RID: 31915 RVA: 0x00325DCC File Offset: 0x00323FCC
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Awake()
	{
		if (!GameEntrypoint.EntrypointSuccess)
		{
			return;
		}
		MicroStopwatch microStopwatch = new MicroStopwatch(true);
		GameManager.Instance = this;
		this.GameIsFocused = (!GameManager.IsDedicatedServer && Application.isFocused);
		Log.Out("Awake IsFocused: " + Application.isFocused.ToString());
		Log.Out("Awake");
		ThreadManager.SetMonoBehaviour(this);
		Utils.InitStatic();
		LoadManager.Init();
		if (Application.isEditor)
		{
			Application.runInBackground = true;
			this.bCursorVisibleOverride = true;
			this.bCursorVisibleOverrideState = true;
		}
		if (!GameManager.IsDedicatedServer)
		{
			GameOptionsManager.ResolutionChanged += this.OnResolutionChanged;
			this.RefreshRefreshRate();
			this.UpdateFPSCap();
		}
		else
		{
			Application.targetFrameRate = 500;
		}
		Application.wantsToQuit += this.OnApplicationQuit;
		if (GameManager.IsDedicatedServer && GamePrefs.GetBool(EnumGamePrefs.TerminalWindowEnabled))
		{
			try
			{
				WinFormInstance server = new WinFormInstance();
				SingletonMonoBehaviour<SdtdConsole>.Instance.RegisterServer(server);
			}
			catch (Exception e)
			{
				Log.Error("Could not start Terminal Window:");
				Log.Exception(e);
			}
		}
		this.windowManager = (GUIWindowManager)UnityEngine.Object.FindObjectOfType(typeof(GUIWindowManager));
		this.nguiWindowManager = UnityEngine.Object.FindObjectOfType<NGUIWindowManager>();
		TaskManager.Init();
		LocalPlayerManager.Init();
		if (!GameManager.IsDedicatedServer)
		{
			GameOptionsManager.LoadControls();
		}
		MeshDataManager.Init();
		OcclusionManager.Load();
		this.waitForTargetFPS = new GameObject("WaitForTargetFPS").AddComponent<WaitForTargetFPS>();
		if (GameManager.IsDedicatedServer)
		{
			GameOptionsManager.ApplyTextureQuality(3);
			QualitySettings.vSyncCount = 0;
			this.waitForTargetFPS.TargetFPS = 0;
		}
		else
		{
			QualitySettings.vSyncCount = GamePrefs.GetInt(PlatformApplicationManager.Application.VSyncCountPref);
			this.waitForTargetFPS.TargetFPS = 0;
		}
		ServerDateTimeRequest.GetNtpTimeAsync(delegate(ServerDateTimeResult result)
		{
			GameManager.ServerClockSync = result;
		}, "pool.ntp.org", 5000);
		GameObjectPool.Instance.Init();
		MemoryPools.InitStatic(!GameManager.IsDedicatedServer);
		this.gameRandomManager = GameRandomManager.Instance;
		this.gameStateManager = new GameStateManager(this);
		this.prefabLODManager = new PrefabLODManager();
		new PrefabEditModeManager();
		UnityEngine.Object.Instantiate<GameObject>(DataLoader.LoadAsset<GameObject>("@:Sound_Mixers/AudioMixerManager.prefab", false));
		this.m_SoundsGameObject = GameObject.Find("Sounds");
		this.PhysicsInit();
		ParticleEffect.Init();
		SelectionBoxManager.Instance.CreateCategory("Selection", SelectionBoxManager.ColSelectionActive, SelectionBoxManager.ColSelectionInactive, SelectionBoxManager.ColSelectionFaceSel, false, null, 0);
		SelectionBoxManager.Instance.CreateCategory("StartPoint", SelectionBoxManager.ColStartPointActive, SelectionBoxManager.ColStartPointInactive, SelectionBoxManager.ColStartPointActive, true, "SB_StartPoint", 31);
		SelectionBoxManager.Instance.CreateCategory("DynamicPrefabs", SelectionBoxManager.ColDynamicPrefabActive, SelectionBoxManager.ColDynamicPrefabInactive, SelectionBoxManager.ColDynamicPrefabFaceSel, true, "SB_Prefabs", 31);
		SelectionBoxManager.Instance.CreateCategory("TraderTeleport", SelectionBoxManager.ColTraderTeleport, SelectionBoxManager.ColTraderTeleportInactive, SelectionBoxManager.ColTraderTeleport, true, "SB_TraderTeleport", 31);
		SelectionBoxManager.Instance.CreateCategory("SleeperVolume", SelectionBoxManager.ColSleeperVolume, SelectionBoxManager.ColSleeperVolumeInactive, SelectionBoxManager.ColSleeperVolume, true, "SB_SleeperVolume", 31);
		SelectionBoxManager.Instance.CreateCategory("InfoVolume", SelectionBoxManager.ColInfoVolume, SelectionBoxManager.ColInfoVolumeInactive, SelectionBoxManager.ColInfoVolume, true, "SB_InfoVolume", 31);
		SelectionBoxManager.Instance.CreateCategory("WallVolume", SelectionBoxManager.ColWallVolume, SelectionBoxManager.ColWallVolumeInactive, SelectionBoxManager.ColWallVolume, true, "SB_WallVolume", 31);
		SelectionBoxManager.Instance.CreateCategory("TriggerVolume", SelectionBoxManager.ColTriggerVolume, SelectionBoxManager.ColTriggerVolumeInactive, SelectionBoxManager.ColTriggerVolume, true, "SB_TriggerVolume", 31);
		SelectionBoxManager.Instance.CreateCategory("POIMarker", SelectionBoxManager.ColDynamicPrefabActive, SelectionBoxManager.ColDynamicPrefabInactive, SelectionBoxManager.ColDynamicPrefabFaceSel, true, "SB_Prefabs", 31);
		SelectionBoxManager.Instance.CreateCategory("PrefabFacing", SelectionBoxManager.ColSleeperVolume, SelectionBoxManager.ColSleeperVolumeInactive, SelectionBoxManager.ColSleeperVolume, true, "SB_SleeperVolume", 31);
		if (!GameManager.IsDedicatedServer)
		{
			if (13 != GamePrefs.GetInt(EnumGamePrefs.LastGameResetRevision))
			{
				if (this.ResetGame())
				{
					GamePrefs.Set(EnumGamePrefs.LastGameResetRevision, 13);
					GamePrefs.Set(EnumGamePrefs.OptionsGfxResetRevision, 4);
					GamePrefs.Instance.Save();
					Log.Out("Game Reset");
				}
				else
				{
					Log.Warning("Failed to Reset Game!");
				}
			}
			else
			{
				if (4 != GamePrefs.GetInt(EnumGamePrefs.OptionsGfxResetRevision) && GameOptionsManager.ResetGameOptions(GameOptionsManager.ResetType.Graphics))
				{
					GamePrefs.Set(EnumGamePrefs.OptionsGfxResetRevision, 4);
					GamePrefs.Instance.Save();
					Log.Out("Graphics Reset");
				}
				if (7 != GamePrefs.GetInt(EnumGamePrefs.OptionsControlsResetRevision) && GameOptionsManager.ResetGameOptions(GameOptionsManager.ResetType.Controls))
				{
					GamePrefs.Set(EnumGamePrefs.OptionsControlsResetRevision, 7);
					GamePrefs.Instance.Save();
					Log.Out("Controls Reset");
				}
			}
		}
		DeviceGamePrefs.Apply();
		if (!GameManager.IsDedicatedServer)
		{
			GameOptionsManager.ApplyAllOptions(this.windowManager.playerUI);
			UIUtils.LoadAtlas();
		}
		Manager.Init();
		UIOptions.Init();
		UIRoot uiroot = UnityEngine.Object.FindObjectOfType<UIRoot>();
		if (!GameManager.IsDedicatedServer)
		{
			this.InitMultiSourceUiAtlases(uiroot.gameObject);
		}
		this.windowManager.gameObject.AddComponent<LocalPlayerUI>();
		this.blockSelectionTool = new BlockToolSelection();
		this.nguiWindowManager.ParseWindows();
		float activeUiScale = GameOptionsManager.GetActiveUiScale();
		this.nguiWindowManager.SetBackgroundScale(activeUiScale);
		this.AddWindows(this.windowManager);
		this.m_GUIConsole = (GUIWindowConsole)this.windowManager.GetWindow(GUIWindowConsole.ID);
		ModManager.LoadMods();
		ThreadManager.RunCoroutineSync(ModManager.LoadPatchStuff(false));
		this.adminTools = new AdminTools();
		SingletonMonoBehaviour<SdtdConsole>.Instance.RegisterCommands();
		IEnumerator enumerator = this.loadStaticData();
		if (GameManager.IsDedicatedServer)
		{
			this.bStaticDataLoadSync = true;
			ThreadManager.RunCoroutineSync(enumerator);
		}
		else
		{
			this.bStaticDataLoadSync = false;
			ThreadManager.StartCoroutine(enumerator);
		}
		if (!GameManager.IsDedicatedServer)
		{
			CursorControllerAbs.LoadStaticData(LoadManager.CreateGroup());
		}
		else
		{
			InputManager.Enabled = false;
		}
		if (GameManager.IsDedicatedServer && GamePrefs.GetBool(EnumGamePrefs.TelnetEnabled))
		{
			try
			{
				TelnetConsole server2 = new TelnetConsole();
				SingletonMonoBehaviour<SdtdConsole>.Instance.RegisterServer(server2);
			}
			catch (Exception e2)
			{
				Log.Error("Could not start network console:");
				Log.Exception(e2);
			}
		}
		AuthorizationManager.Instance.Init();
		ModEvents.SGameAwakeData sgameAwakeData;
		ModEvents.GameAwake.Invoke(ref sgameAwakeData);
		this.nguiWindowManager.Show(EnumNGUIWindow.InGameHUD, false);
		ConsoleCmdShow.Init();
		if (!GameManager.IsDedicatedServer)
		{
			GameSenseManager instance = GameSenseManager.Instance;
			if (instance != null)
			{
				instance.Init();
			}
			if (GamePrefs.GetBool(EnumGamePrefs.OptionsMumblePositionalAudioSupport))
			{
				MumblePositionalAudio.Init();
			}
		}
		DiscordManager instance2 = DiscordManager.Instance;
		if (this.BackgroundMusicClip || this.CreditsSongClip)
		{
			if (!GameManager.IsDedicatedServer)
			{
				base.gameObject.AddComponent<BackgroundMusicMono>();
			}
			else
			{
				Resources.UnloadAsset(this.BackgroundMusicClip);
				Resources.UnloadAsset(this.CreditsSongClip);
			}
		}
		PartyQuests.EnforeInstance();
		Input.simulateMouseWithTouches = false;
		IPlatform nativePlatform = PlatformManager.NativePlatform;
		IApplicationStateController applicationStateController = (nativePlatform != null) ? nativePlatform.ApplicationState : null;
		if (applicationStateController != null)
		{
			ApplicationState lastState = ApplicationState.Foreground;
			applicationStateController.OnApplicationStateChanged += delegate(ApplicationState state)
			{
				if (state != ApplicationState.Suspended && lastState == ApplicationState.Suspended)
				{
					this.OnApplicationResume();
				}
				lastState = state;
			};
			applicationStateController.OnNetworkStateChanged += this.OnNetworkStateChanged;
		}
		PlatformUserManager.DetailsUpdated += this.OnUserDetailsUpdated;
		if (!GameManager.IsDedicatedServer)
		{
			this.triggerEffectManager = new TriggerEffectManager();
			TriggerEffectManager.SetMainMenuLightbarColor();
		}
		Log.Out("Awake done in " + microStopwatch.ElapsedMilliseconds.ToString() + " ms");
	}

	// Token: 0x06007CAC RID: 31916 RVA: 0x003264D0 File Offset: 0x003246D0
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitMultiSourceUiAtlases(GameObject _parent)
	{
		GameObject gameObject = new GameObject("UIAtlases");
		gameObject.transform.parent = _parent.transform;
		Shader shader = Shader.Find("Unlit/Transparent Colored");
		Shader shader2 = Shader.Find("Unlit/Transparent Greyscale");
		MultiSourceAtlasManager multiSourceAtlasManager = MultiSourceAtlasManager.Create(gameObject, "ItemIconAtlas");
		MultiSourceAtlasManager atlasManager = MultiSourceAtlasManager.Create(gameObject, "ItemIconAtlasGreyscale");
		ModManager.ModAtlasesDefaults(gameObject, shader);
		ModManager.RegisterAtlasManager(multiSourceAtlasManager, false, shader, new Action<UIAtlas, bool>(this.AddGreyscaleItemIconAtlas));
		ModManager.RegisterAtlasManager(atlasManager, false, shader2, null);
		Resources.Load<UIAtlas>("GUI/Prefabs/SymbolAtlas");
		Resources.Load<UIAtlas>("GUI/Prefabs/ControllerArtAtlas");
		UIAtlas[] array = Resources.FindObjectsOfTypeAll<UIAtlas>();
		for (int i = 0; i < array.Length; i++)
		{
			string name = array[i].gameObject.name;
			if (!name.ContainsCaseInsensitive("icons_"))
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(array[i].gameObject);
				gameObject2.name = name;
				UIAtlas component = gameObject2.GetComponent<UIAtlas>();
				MultiSourceAtlasManager multiSourceAtlasManager2 = MultiSourceAtlasManager.Create(gameObject, name);
				gameObject2.transform.parent = multiSourceAtlasManager2.transform;
				ModManager.RegisterAtlasManager(multiSourceAtlasManager2, false, component.spriteMaterial.shader, null);
				multiSourceAtlasManager2.AddAtlas(component, false);
			}
		}
		string mipFilter = GameOptionsPlatforms.GetItemIconFilterString();
		LoadManager.AssetsRequestTask<GameObject> assetsRequestTask = LoadManager.LoadAssetsFromAddressables<GameObject>("iconatlas", (string address) => address.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase) && address.Contains(mipFilter), null, false, true);
		List<GameObject> list = new List<GameObject>();
		assetsRequestTask.CollectResults(list);
		foreach (GameObject original in list)
		{
			GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(original);
			gameObject3.transform.parent = multiSourceAtlasManager.transform;
			UIAtlas component2 = gameObject3.GetComponent<UIAtlas>();
			multiSourceAtlasManager.AddAtlas(component2, false);
			this.AddGreyscaleItemIconAtlas(component2, false);
		}
	}

	// Token: 0x06007CAD RID: 31917 RVA: 0x003266A0 File Offset: 0x003248A0
	[PublicizedFrom(EAccessModifier.Private)]
	public void AddGreyscaleItemIconAtlas(UIAtlas _atlas, bool _isLoadingInGame)
	{
		MultiSourceAtlasManager atlasManager = ModManager.GetAtlasManager("ItemIconAtlasGreyscale");
		Shader shader = Shader.Find("Unlit/Transparent Greyscale");
		UIAtlas uiatlas = UnityEngine.Object.Instantiate<UIAtlas>(_atlas, atlasManager.transform);
		uiatlas.spriteMaterial = new Material(shader)
		{
			mainTexture = uiatlas.texture
		};
		atlasManager.AddAtlas(uiatlas, _isLoadingInGame);
	}

	// Token: 0x06007CAE RID: 31918 RVA: 0x003266F0 File Offset: 0x003248F0
	public void AddWindows(GUIWindowManager _guiWindowManager)
	{
		if (_guiWindowManager == this.windowManager)
		{
			_guiWindowManager.Add(GUIWindowConsole.ID, new GUIWindowConsole());
			_guiWindowManager.Add(GUIWindowScreenshotText.ID, new GUIWindowScreenshotText());
		}
		_guiWindowManager.Add(EnumNGUIWindow.InGameHUD.ToStringCached<EnumNGUIWindow>(), new GUIWindowNGUI(EnumNGUIWindow.InGameHUD));
		_guiWindowManager.Add(GUIWindowEditBlockSpawnEntity.ID, new GUIWindowEditBlockSpawnEntity(this));
		new GUIWindowDynamicPrefabMenu(this);
		_guiWindowManager.Add(GUIWindowWOChooseCategory.ID, new GUIWindowWOChooseCategory());
		_guiWindowManager.CloseAllOpenWindows(null, false);
	}

	// Token: 0x06007CAF RID: 31919 RVA: 0x0032676E File Offset: 0x0032496E
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator loadStaticData()
	{
		this.CurrentLoadAction = Localization.Get("loadActionCharacterModels", false);
		yield return null;
		this.CurrentLoadAction = Localization.Get("loadActionTerrainTextures", false);
		yield return null;
		yield return null;
		yield return WorldStaticData.Init(false, GameManager.IsDedicatedServer, delegate(string _progressText, float _percentage)
		{
			this.CurrentLoadAction = _progressText;
		});
		this.CurrentLoadAction = Localization.Get("loadActionDone", false);
		this.bStaticDataLoaded = true;
		yield break;
	}

	// Token: 0x06007CB0 RID: 31920 RVA: 0x0032677D File Offset: 0x0032497D
	[PublicizedFrom(EAccessModifier.Private)]
	public bool ResetGame()
	{
		Log.Out("Resetting Game");
		return GameOptionsManager.ResetGameOptions(GameOptionsManager.ResetType.All) && GameOptionsManager.ResetGameOptions(GameOptionsManager.ResetType.Graphics);
	}

	// Token: 0x17000D10 RID: 3344
	// (get) Token: 0x06007CB1 RID: 31921 RVA: 0x00326799 File Offset: 0x00324999
	// (set) Token: 0x06007CB2 RID: 31922 RVA: 0x003267A1 File Offset: 0x003249A1
	public bool IsStartingGame { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06007CB3 RID: 31923 RVA: 0x003267AA File Offset: 0x003249AA
	public void StartGame(bool _offline)
	{
		Time.timeScale = 1f;
		GamePrefs.Set(EnumGamePrefs.GameGuidClient, "");
		if (GameSparksManager.Instance() != null)
		{
			GameSparksManager.Instance().PrepareNewSession();
		}
		base.StartCoroutine(this.startGameCo(_offline));
	}

	// Token: 0x06007CB4 RID: 31924 RVA: 0x003267EA File Offset: 0x003249EA
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator startGameCo(bool _offline)
	{
		this.IsStartingGame = true;
		PlatformApplicationManager.SetRestartRequired();
		Log.Out("StartGame");
		ModEvents.SGameStartingData sgameStartingData = new ModEvents.SGameStartingData(SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer);
		ModEvents.GameStarting.Invoke(ref sgameStartingData);
		this.allowQuit = false;
		this.backgroundColor = Color.white;
		EntityStats.WeatherSurvivalEnabled = true;
		yield return null;
		yield return ModManager.LoadPatchStuff(true);
		yield return null;
		SaveInfoProvider.Instance.ClearResources();
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && !SingletonMonoBehaviour<ConnectionManager>.Instance.IsConnected)
		{
			yield break;
		}
		if (!GameManager.IsDedicatedServer)
		{
			XUiC_MainMenu.CloseGlobalMenuWindows(this.windowManager.playerUI.xui);
			this.windowManager.CloseAllOpenWindows(null, false);
			XUiFromXml.ClearData();
			LocalPlayerUI.QueueUIForNewPlayerEntity(LocalPlayerUI.CreateUIForNewLocalPlayer());
			this.windowManager.Open(XUiC_LoadingScreen.ID, false, true, true);
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
			{
				if (SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo.EACEnabled)
				{
					this.windowManager.Open("eacWarning", false, false, true);
				}
				if (SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo.AllowsCrossplay)
				{
					this.windowManager.Open("crossplayWarning", false, false, true);
				}
			}
			XUiC_ProgressWindow.Open(LocalPlayerUI.primaryUI, Localization.Get("uiLoadStartingGame", false), null, true, true, true, false);
		}
		yield return null;
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			GamePrefs.Set(EnumGamePrefs.GameWorld, string.Empty);
		}
		this.isEditMode = GameModeEditWorld.TypeName.Equals(GamePrefs.GetString(EnumGamePrefs.GameMode));
		GamePrefs.Set(EnumGamePrefs.DebugStopEnemiesMoving, this.IsEditMode());
		GamePrefs.Set(EnumGamePrefs.DebugMenuEnabled, this.isEditMode || GameUtils.IsPlaytesting());
		GamePrefs.Set(EnumGamePrefs.CreativeMenuEnabled, this.isEditMode || GameUtils.IsPlaytesting());
		GamePrefs.Instance.Save();
		if (!Application.isEditor)
		{
			GameUtils.DebugOutputGamePrefs(delegate(string _text)
			{
				Log.WriteLine("GamePref." + _text);
			});
			GameUtils.DebugOutputGameStats(delegate(string _text)
			{
				Log.WriteLine("GameStat." + _text);
			});
		}
		yield return null;
		CraftingManager.InitForNewGame();
		yield return null;
		GameManager.bSavingActive = true;
		GameManager.bPhysicsActive = !this.IsEditMode();
		GameManager.bTickingActive = !this.IsEditMode();
		GameManager.bShowDecorBlocks = true;
		GameManager.bShowLootBlocks = true;
		GameManager.bShowPaintables = true;
		GameManager.bShowUnpaintables = true;
		GameManager.bShowTerrain = true;
		GameManager.bVolumeBlocksEditing = true;
		Block.nameIdMapping = null;
		ItemClass.nameIdMapping = null;
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			yield return this.StartAsServer(_offline);
		}
		else
		{
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsConnected)
			{
				yield break;
			}
			XUiC_ProgressWindow.Open(LocalPlayerUI.primaryUI, Localization.Get("uiLoadWaitingForServer", false), null, true, true, true, false);
			this.StartAsClient();
		}
		DismembermentManager.Init();
		yield return null;
		if (GameSparksManager.Instance() != null)
		{
			GameSparksManager.Instance().SessionStarted(GamePrefs.GetString(EnumGamePrefs.GameWorld), GamePrefs.GetString(EnumGamePrefs.GameMode), SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer);
		}
		if (!GameManager.IsDedicatedServer && SingletonMonoBehaviour<ConnectionManager>.Instance.CurrentMode != ProtocolManager.NetworkType.OfflineServer)
		{
			PlatformManager.MultiPlatform.User.StartAdvertisePlaying(SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer ? SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo : SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo);
		}
		Log.Out("Loading dymesh settings");
		DynamicMeshManager.CONTENT_ENABLED = GamePrefs.GetBool(EnumGamePrefs.DynamicMeshEnabled);
		DynamicMeshSettings.OnlyPlayerAreas = GamePrefs.GetBool(EnumGamePrefs.DynamicMeshLandClaimOnly);
		DynamicMeshSettings.UseImposterValues = GamePrefs.GetBool(EnumGamePrefs.DynamicMeshUseImposters);
		DynamicMeshSettings.MaxViewDistance = GamePrefs.GetInt(EnumGamePrefs.DynamicMeshDistance);
		DynamicMeshSettings.PlayerAreaChunkBuffer = GamePrefs.GetInt(EnumGamePrefs.DynamicMeshLandClaimBuffer);
		DynamicMeshSettings.MaxRegionMeshData = GamePrefs.GetInt(EnumGamePrefs.DynamicMeshMaxRegionCache);
		DynamicMeshSettings.MaxDyMeshData = GamePrefs.GetInt(EnumGamePrefs.DynamicMeshMaxItemCache);
		DynamicMeshSettings.LogSettings();
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			DynamicMeshManager.Init();
		}
		ModEvents.SGameStartDoneData eventData;
		ModEvents.GameStartDone.Invoke(ref eventData);
		if (GameManager.IsDedicatedServer)
		{
			this.waitForTargetFPS.TargetFPS = 20;
		}
		Log.Out("StartGame done");
		this.IsStartingGame = false;
		yield break;
	}

	// Token: 0x06007CB5 RID: 31925 RVA: 0x00326800 File Offset: 0x00324A00
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateTimeOfDay()
	{
		if (!GameManager.IsDedicatedServer || this.m_World.Players.list.Count > 0)
		{
			int @int = GameStats.GetInt(EnumGameStats.TimeOfDayIncPerSec);
			if (@int == 0)
			{
				this.msPassedSinceLastUpdate += (int)(Time.deltaTime * 1000f);
				if (this.msPassedSinceLastUpdate >= 100)
				{
					this.m_World.SetTime(this.m_World.worldTime);
					this.msPassedSinceLastUpdate = 0;
				}
				return;
			}
			float num = 1000f / (float)@int;
			this.msPassedSinceLastUpdate += (int)(Time.deltaTime * 1000f);
			if ((float)this.msPassedSinceLastUpdate <= Utils.FastMax(num, 50f))
			{
				return;
			}
			int num2 = (int)((float)this.msPassedSinceLastUpdate / num);
			this.msPassedSinceLastUpdate -= (int)num * num2;
			ulong time = this.m_World.worldTime + (ulong)((long)num2);
			this.m_World.SetTime(time);
		}
		ILobbyHost lobbyHost = PlatformManager.NativePlatform.LobbyHost;
		if (lobbyHost != null)
		{
			lobbyHost.UpdateGameTimePlayers(this.m_World.worldTime, this.m_World.Players.list.Count);
		}
		GameSenseManager instance = GameSenseManager.Instance;
		if (instance != null)
		{
			instance.UpdateEventTime(this.m_World.worldTime);
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && Time.time - this.lastTimeWorldTickTimeSentToClients > Constants.cSendWorldTickTimeToClients)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.UpdateGameTimePlayers(this.m_World.worldTime, this.m_World.Players.list.Count);
			this.lastTimeWorldTickTimeSentToClients = Time.time;
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageWorldTime>().Setup(this.m_World.worldTime), true, -1, -1, -1, null, 192, false);
			if (WeatherManager.Instance != null)
			{
				WeatherManager.Instance.SendPackages();
			}
		}
	}

	// Token: 0x06007CB6 RID: 31926 RVA: 0x003269E4 File Offset: 0x00324BE4
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateSendClientPlayerPositionToServer()
	{
		EntityPlayerLocal primaryPlayer = this.m_World.GetPrimaryPlayer();
		EntityAlive entityAlive = primaryPlayer;
		if (entityAlive != null)
		{
			if (entityAlive.AttachedToEntity != null)
			{
				entityAlive = (entityAlive.AttachedToEntity as EntityAlive);
				this.bLastWasAttached = true;
				if (entityAlive.isEntityRemote)
				{
					return;
				}
			}
			else
			{
				if (this.bLastWasAttached)
				{
					this.lastTimeAbsPosSentToServer = int.MaxValue;
				}
				this.bLastWasAttached = false;
			}
		}
		if (entityAlive == null)
		{
			return;
		}
		if (primaryPlayer.bPlayerStatsChanged)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePlayerStats>().Setup(primaryPlayer), false);
			primaryPlayer.bPlayerStatsChanged = false;
		}
		if (primaryPlayer.bPlayerTwitchChanged)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePlayerTwitchStats>().Setup(primaryPlayer), false);
			primaryPlayer.bPlayerTwitchChanged = false;
		}
		if (primaryPlayer.bEntityAliveFlagsChanged)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEntityAliveFlags>().Setup(primaryPlayer), false);
			primaryPlayer.bEntityAliveFlagsChanged = false;
		}
		Vector3i vector3i = NetEntityDistributionEntry.EncodePos(entityAlive.position);
		Vector3i vector3i2 = vector3i - entityAlive.serverPos;
		bool flag = Utils.FastAbs((float)vector3i2.x) >= 2f || Utils.FastAbs((float)vector3i2.y) >= 2f || Utils.FastAbs((float)vector3i2.z) >= 2f || entityAlive.emodel.IsRagdollActive;
		Vector3i vector3i3 = NetEntityDistributionEntry.EncodeRot(entityAlive.rotation);
		Vector3i vector3i4 = vector3i3 - entityAlive.serverRot;
		bool flag2 = Utils.FastAbs((float)vector3i4.x) >= 1f || Utils.FastAbs((float)vector3i4.y) >= 1f || Utils.FastAbs((float)vector3i4.z) >= 1f || entityAlive.emodel.IsRagdollActive;
		if (flag || flag2)
		{
			if (vector3i2.x < -256 || vector3i2.x >= 256 || vector3i2.y < -256 || vector3i2.y >= 256 || vector3i2.z < -256 || vector3i2.z >= 256)
			{
				this.lastTimeAbsPosSentToServer = 0;
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEntityTeleport>().Setup(entityAlive), false);
			}
			else if (vector3i2.x < -128 || vector3i2.x >= 128 || vector3i2.y < -128 || vector3i2.y >= 128 || vector3i2.z < -128 || vector3i2.z >= 128 || this.lastTimeAbsPosSentToServer > 100)
			{
				this.lastTimeAbsPosSentToServer = 0;
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEntityPosAndRot>().Setup(entityAlive), false);
			}
			else
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEntityRelPosAndRot>().Setup(entityAlive.entityId, vector3i2, vector3i3, entityAlive.qrotation, entityAlive.onGround, entityAlive.IsQRotationUsed(), 3), false);
			}
			entityAlive.serverPos = vector3i;
			entityAlive.serverRot = vector3i3;
			this.lastTimeAbsPosSentToServer++;
		}
		if (entityAlive != primaryPlayer)
		{
			if (entityAlive.bPlayerStatsChanged)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePlayerStats>().Setup(entityAlive), false);
				entityAlive.bPlayerStatsChanged = false;
			}
			if (entityAlive.bPlayerTwitchChanged)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePlayerTwitchStats>().Setup(entityAlive), false);
				entityAlive.bPlayerTwitchChanged = false;
			}
			if (entityAlive.bEntityAliveFlagsChanged)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEntityAliveFlags>().Setup(entityAlive), false);
				entityAlive.bEntityAliveFlagsChanged = false;
			}
		}
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(primaryPlayer);
		if (this.countdownSendPlayerDataFileToServer.HasPassed() && uiforPlayer.xui != null && uiforPlayer.xui.isReady)
		{
			this.countdownSendPlayerDataFileToServer.ResetAndRestart();
			this.doSendLocalPlayerData(primaryPlayer);
		}
		if (this.countdownSendPlayerInventoryToServer.HasPassed())
		{
			this.countdownSendPlayerInventoryToServer.Reset();
			this.doSendLocalInventory(primaryPlayer);
		}
		if (primaryPlayer.persistentPlayerData != null && primaryPlayer.persistentPlayerData.questPositionsChanged)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePlayerQuestPositions>().Setup(primaryPlayer.entityId, primaryPlayer.persistentPlayerData), false);
			primaryPlayer.persistentPlayerData.questPositionsChanged = false;
		}
	}

	// Token: 0x06007CB7 RID: 31927 RVA: 0x00326DDE File Offset: 0x00324FDE
	[PublicizedFrom(EAccessModifier.Private)]
	public void FixedUpdate()
	{
		GameManager.fixedUpdateCount++;
	}

	// Token: 0x06007CB8 RID: 31928 RVA: 0x00326DEC File Offset: 0x00324FEC
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Update()
	{
		this.gmUpdate();
	}

	// Token: 0x06007CB9 RID: 31929 RVA: 0x00326DF4 File Offset: 0x00324FF4
	[PublicizedFrom(EAccessModifier.Private)]
	public void gmUpdate()
	{
		GameManager.frameCount = Time.frameCount;
		GameManager.frameTime = Time.time;
		GameManager.fixedUpdateCount = 0;
		GameOptionsManager.CheckResolution();
		ModEvents.SUnityUpdateData sunityUpdateData;
		ModEvents.UnityUpdate.Invoke(ref sunityUpdateData);
		this.handleGlobalActions();
		if (!GameManager.ReportUnusedAssets(false))
		{
			return;
		}
		if ((double)Time.timeScale <= 0.001)
		{
			Physics.SyncTransforms();
		}
		LoadManager.Update();
		PlatformManager.Update();
		InviteManager.Instance.Update();
		this.swUpdateTime.ResetAndRestart();
		this.fps.Update();
		BlockLiquidv2.UpdateTime();
		if (QuestEventManager.Current != null)
		{
			QuestEventManager.Current.Update();
		}
		if (this.m_World != null)
		{
			this.m_World.triggerManager.Update();
		}
		if (TwitchVoteScheduler.Current != null)
		{
			TwitchVoteScheduler.Current.Update(Time.deltaTime);
		}
		if (TwitchManager.Current != null)
		{
			TwitchManager.Current.Update(Time.unscaledDeltaTime);
		}
		if (GameEventManager.Current != null)
		{
			GameEventManager.Current.Update(Time.deltaTime);
		}
		if (PowerManager.HasInstance)
		{
			PowerManager.Instance.Update();
		}
		if (PartyManager.HasInstance)
		{
			PartyManager.Current.Update();
		}
		if (VehicleManager.Instance != null)
		{
			VehicleManager.Instance.Update();
		}
		if (DroneManager.Instance != null)
		{
			DroneManager.Instance.Update();
		}
		if (DismembermentManager.Instance != null)
		{
			DismembermentManager.Instance.Update();
		}
		if (TurretTracker.Instance != null)
		{
			TurretTracker.Instance.Update();
		}
		if (RaycastPathManager.Instance)
		{
			RaycastPathManager.Instance.Update();
		}
		if (EntityCoverManager.Instance != null)
		{
			EntityCoverManager.Instance.Update();
		}
		if (FactionManager.Instance != null)
		{
			FactionManager.Instance.Update();
		}
		if (NavObjectManager.HasInstance)
		{
			NavObjectManager.Instance.Update();
		}
		if (BlockedPlayerList.Instance != null)
		{
			BlockedPlayerList.Instance.Update();
		}
		PrefabEditModeManager instance = PrefabEditModeManager.Instance;
		if (instance != null)
		{
			instance.Update();
		}
		TriggerEffectManager triggerEffectManager = this.triggerEffectManager;
		if (triggerEffectManager != null)
		{
			triggerEffectManager.Update();
		}
		SpeedTreeWindHistoryBufferManager.Instance.Update();
		ThreadManager.UpdateMainThreadTasks();
		if (!GameManager.IsDedicatedServer)
		{
			if (XUiC_MainMenu.openedOnce && !this.isQuitting)
			{
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				bool? flag;
				string text;
				if (crossplatformPlatform == null)
				{
					flag = null;
				}
				else
				{
					IAntiCheatClient antiCheatClient = crossplatformPlatform.AntiCheatClient;
					flag = ((antiCheatClient != null) ? new bool?(antiCheatClient.GetUnhandledViolationMessage(out text)) : null);
				}
				bool? flag2 = flag;
				if (flag2.GetValueOrDefault())
				{
					GUIWindowManager guiwindowManager = LocalPlayerUI.primaryUI.windowManager;
					if (guiwindowManager != null)
					{
						string title = "EAC: " + Localization.Get("eacIntegrityViolation", false);
						if (!string.IsNullOrEmpty(text))
						{
							text += "\n";
						}
						else
						{
							text = "";
						}
						text += Localization.Get("eacUnableToPlayOnProtected", false);
						((XUiC_MessageBoxWindowGroup)((XUiWindowGroup)guiwindowManager.GetWindow(XUiC_MessageBoxWindowGroup.ID)).Controller).ShowMessage(title, text, XUiC_MessageBoxWindowGroup.MessageBoxTypes.Ok, null, null, !this.gameStateManager.IsGameStarted(), true, true);
					}
				}
			}
			if (!this.bCursorVisibleOverride && !this.isQuitting)
			{
				bool flag3 = this.isAnyCursorWindowOpen(null);
				if (this.GameIsFocused && this.bCursorVisible != flag3)
				{
					this.setCursorEnabled(flag3);
				}
				if (!flag3 && Cursor.visible && PlatformManager.NativePlatform.Input.CurrentInputStyle == PlayerInputManager.InputStyle.Keyboard)
				{
					this.setCursorEnabled(false);
				}
			}
			this.UpdateFPSCap();
		}
		object syncRoot = ((ICollection)this.tileEntitiesMusicToRemove).SyncRoot;
		lock (syncRoot)
		{
			for (int i = 0; i < this.tileEntitiesMusicToRemove.Count; i++)
			{
				UnityEngine.Object.Destroy(this.tileEntitiesMusicToRemove[i]);
			}
		}
		if (!this.gameStateManager.IsGameStarted())
		{
			GameTimer.Instance.Reset(GameTimer.Instance.ticks);
			return;
		}
		GameTimer.Instance.updateTimer(GameManager.IsDedicatedServer && this.m_World.Players.Count == 0);
		this.updateBlockParticles();
		this.updateTimeOfDay();
		Manager.FrameUpdate();
		WaterSimulationNative.Instance.Update();
		WaterEvaporationManager.UpdateEvaporation();
		if (GameTimer.Instance.elapsedTicks > 0 || this.m_World.m_ChunkManager.IsForceUpdate() || this.m_World.Players.list.Count == 0)
		{
			this.m_World.m_ChunkManager.DetermineChunksToLoad();
		}
		if (GameManager.IsDedicatedServer && this.m_World.Players.list.Count == 0 && this.lastPlayerCount > 0)
		{
			this.timeToClearAllPools = 8f;
		}
		this.lastPlayerCount = this.m_World.Players.list.Count;
		if (this.m_World.Players.list.Count == 0 && this.timeToClearAllPools > 0f && (this.timeToClearAllPools -= Time.deltaTime) <= 0f)
		{
			Log.Out("Clearing all pools");
			MemoryPools.Cleanup();
			this.m_World.ClearCaches();
		}
		if (!this.UpdateTick())
		{
			return;
		}
		this.m_World.m_ChunkManager.GroundAlignFrameUpdate();
		int num = GameManager.IsDedicatedServer ? 25000 : 2500;
		this.swCopyChunks.ResetAndRestart();
		while (this.m_World.m_ChunkManager.CopyChunksToUnity() && this.swCopyChunks.ElapsedMicroseconds < (long)num)
		{
		}
		if (this.prefabLODManager != null)
		{
			this.prefabLODManager.FrameUpdate();
		}
		this.ExplodeGroupFrameUpdate();
		this.fpsCountdownTimer -= Time.deltaTime;
		if (this.fpsCountdownTimer <= 0f)
		{
			this.fpsCountdownTimer = 30f;
			GameManager.MaxMemoryConsumption = Math.Max(GC.GetTotalMemory(false), GameManager.MaxMemoryConsumption);
			if (!GameManager.IsDedicatedServer || SingletonMonoBehaviour<ConnectionManager>.Instance.ClientCount() > 0 || this.lastStatsPlayerCount > 0)
			{
				this.lastStatsPlayerCount = SingletonMonoBehaviour<ConnectionManager>.Instance.ClientCount();
				Log.Out(ConsoleCmdMem.GetStats(false, this));
			}
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.wsCountdownTimer -= Time.deltaTime;
			if (this.wsCountdownTimer <= 0f)
			{
				this.wsCountdownTimer = 30f;
				if (!this.isEditMode)
				{
					this.m_World.SaveWorldState();
					if (Block.nameIdMapping != null)
					{
						Block.nameIdMapping.SaveIfDirty(true);
					}
					if (ItemClass.nameIdMapping != null)
					{
						ItemClass.nameIdMapping.SaveIfDirty(true);
					}
				}
			}
			this.playerPositionsCountdownTimer -= Time.deltaTime;
			if (this.playerPositionsCountdownTimer <= 0f)
			{
				this.playerPositionsCountdownTimer = 6f;
				if (SingletonMonoBehaviour<ConnectionManager>.Instance.ClientCount() > 0)
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackagePersistentPlayerPositions>().Setup(this.persistentPlayers), false, -1, -1, -1, null, 192, true);
				}
			}
		}
		if (GameManager.IsDedicatedServer)
		{
			this.gcCountdownTimer -= Time.deltaTime;
			if (this.gcCountdownTimer <= 0f)
			{
				this.gcCountdownTimer = 120f;
				GC.Collect();
			}
			if ((float)this.swUpdateTime.ElapsedMilliseconds > 50f)
			{
				this.waitForTargetFPS.SkipSleepThisFrame = true;
			}
		}
		else
		{
			GameSenseManager instance2 = GameSenseManager.Instance;
			if (instance2 != null)
			{
				instance2.Update();
			}
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && this.countdownSaveLocalPlayerDataFile.HasPassed())
			{
				this.countdownSaveLocalPlayerDataFile.ResetAndRestart();
				this.SaveLocalPlayerData();
			}
			this.unloadAssetsDuration += Time.deltaTime;
			if (this.unloadAssetsDuration > 1200f)
			{
				bool flag5 = this.unloadAssetsDuration > 3600f;
				if (!this.isAnyModalWindowOpen())
				{
					this.isUnloadAssetsReady = true;
				}
				else if (this.isUnloadAssetsReady)
				{
					flag5 = true;
				}
				if (flag5)
				{
					this.stopwatchUnloadAssets.ResetAndRestart();
					Resources.UnloadUnusedAssets();
					this.stopwatchUnloadAssets.Stop();
					Log.Out("UnloadUnusedAssets after {0} m, took {1} ms", new object[]
					{
						this.unloadAssetsDuration / 60f,
						this.stopwatchUnloadAssets.ElapsedMilliseconds
					});
					this.unloadAssetsDuration = 0f;
					this.isUnloadAssetsReady = false;
				}
			}
		}
		if (this.stabilityViewer != null)
		{
			this.stabilityViewer.Update();
		}
		ModEvents.SGameUpdateData sgameUpdateData;
		ModEvents.GameUpdate.Invoke(ref sgameUpdateData);
		GameObjectPool.Instance.FrameUpdate();
	}

	// Token: 0x06007CBA RID: 31930 RVA: 0x00327620 File Offset: 0x00325820
	public void LateUpdate()
	{
		ThreadManager.LateUpdate();
		PlatformManager.LateUpdate();
		if (this.m_World != null && this.m_World.aiDirector != null)
		{
			this.m_World.aiDirector.DebugFrameLateUpdate();
		}
		this.UpdateMultiplayerServices();
		MeshDataManager.Instance.LateUpdate();
	}

	// Token: 0x06007CBB RID: 31931 RVA: 0x0032766C File Offset: 0x0032586C
	[PublicizedFrom(EAccessModifier.Private)]
	public bool UpdateTick()
	{
		GameTimer instance = GameTimer.Instance;
		if (instance.elapsedTicks <= 0 && this.m_World.Players.list.Count != 0)
		{
			this.m_World.TickEntitiesSlice();
			return true;
		}
		this.m_World.TickEntitiesFlush();
		float partialTicks = (Time.time - this.lastTime) * 20f;
		this.lastTime = Time.time;
		this.m_World.OnUpdateTick(partialTicks, this.m_World.m_ChunkManager.GetActiveChunkSet());
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && !this.gameStateManager.OnUpdateTick())
		{
			return false;
		}
		this.m_World.TickEntities(partialTicks);
		this.m_World.LetBlocksFall();
		if (!GameManager.IsDedicatedServer)
		{
			this.m_World.SetEntitiesVisibleNearToLocalPlayer();
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.m_World.entityDistributer.OnUpdateEntities();
			this.m_World.m_ChunkManager.SendChunksToClients();
			if (GameManager.bSavingActive)
			{
				ChunkCluster chunkCache = this.m_World.ChunkCache;
				ChunkProviderGenerateWorld chunkProviderGenerateWorld = ((chunkCache != null) ? chunkCache.ChunkProvider : null) as ChunkProviderGenerateWorld;
				if (chunkProviderGenerateWorld != null)
				{
					chunkProviderGenerateWorld.MainThreadCacheProtectedPositions();
				}
				if (instance.ticks % 40UL == 0UL)
				{
					if (chunkCache != null)
					{
						chunkCache.ChunkProvider.SaveRandomChunks(2, instance.ticks, this.m_World.m_ChunkManager.GetActiveChunkSet());
					}
				}
				else if (Time.time - this.lastTimeDecoSaved > 60f)
				{
					this.lastTimeDecoSaved = Time.time;
					this.m_World.SaveDecorations();
				}
			}
		}
		else
		{
			this.updateSendClientPlayerPositionToServer();
		}
		if (this.lastTime - this.activityCheck >= 1f)
		{
			PlatformManager.MultiPlatform.RichPresence.UpdateRichPresence(IRichPresence.PresenceStates.InGame);
			this.activityCheck = this.lastTime;
		}
		return true;
	}

	// Token: 0x06007CBC RID: 31932 RVA: 0x0032782A File Offset: 0x00325A2A
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnNetworkStateChanged(bool connectionState)
	{
		if (!connectionState)
		{
			this.ShutdownMultiplayerServices(GameManager.EMultiShutReason.AppNoNetwork);
		}
	}

	// Token: 0x06007CBD RID: 31933 RVA: 0x00327836 File Offset: 0x00325A36
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnApplicationResume()
	{
		PlatformApplicationManager.SetRestartRequired();
		if (!this.IsSafeToConnect())
		{
			this.ShutdownMultiplayerServices(GameManager.EMultiShutReason.AppSuspended);
			return;
		}
		ThreadManager.StartCoroutine(PlatformApplicationManager.CheckRestartCoroutine(false));
	}

	// Token: 0x06007CBE RID: 31934 RVA: 0x0032785C File Offset: 0x00325A5C
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateMultiplayerServices()
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		if (this.shuttingDownMultiplayerServices)
		{
			return;
		}
		if (this.IsSafeToConnect() || !this.IsSafeToDisconnect())
		{
			return;
		}
		ConnectionManager instance = SingletonMonoBehaviour<ConnectionManager>.Instance;
		if (!(instance == null))
		{
			ProtocolManager.NetworkType currentMode = instance.CurrentMode;
			if (currentMode != ProtocolManager.NetworkType.None && currentMode != ProtocolManager.NetworkType.OfflineServer)
			{
				EUserPerms permissions = PermissionsManager.GetPermissions(PermissionsManager.PermissionSources.All);
				if (!permissions.HasMultiplayer())
				{
					this.ShutdownMultiplayerServices(GameManager.EMultiShutReason.PermMissingMultiplayer);
					return;
				}
				if ((SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient ? SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo : SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo).AllowsCrossplay && !permissions.HasCrossplay())
				{
					this.ShutdownMultiplayerServices(GameManager.EMultiShutReason.PermMissingCrossplay);
					return;
				}
				return;
			}
		}
	}

	// Token: 0x06007CBF RID: 31935 RVA: 0x003278FC File Offset: 0x00325AFC
	[PublicizedFrom(EAccessModifier.Private)]
	public static string GetLocalizationKey(GameManager.EMultiShutReason _reason)
	{
		string result;
		switch (_reason)
		{
		case GameManager.EMultiShutReason.AppNoNetwork:
			result = "app_noNetwork";
			break;
		case GameManager.EMultiShutReason.AppSuspended:
			result = "app_suspended";
			break;
		case GameManager.EMultiShutReason.PermMissingMultiplayer:
			result = "permMissing_multiplayer";
			break;
		case GameManager.EMultiShutReason.PermMissingCrossplay:
			result = "permMissing_crossplay";
			break;
		default:
			throw new ArgumentOutOfRangeException("_reason", _reason, string.Format("Unknown Localization for {0}.{1}", "EMultiShutReason", _reason));
		}
		return result;
	}

	// Token: 0x06007CC0 RID: 31936 RVA: 0x00327968 File Offset: 0x00325B68
	[PublicizedFrom(EAccessModifier.Private)]
	public void ShutdownMultiplayerServices(GameManager.EMultiShutReason _reason)
	{
		ThreadManager.StartCoroutine(this.ShutdownMultiplayerServicesCoroutine(_reason));
	}

	// Token: 0x06007CC1 RID: 31937 RVA: 0x00327977 File Offset: 0x00325B77
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator ShutdownMultiplayerServicesCoroutine(GameManager.EMultiShutReason _reason)
	{
		yield return null;
		if (GameManager.IsDedicatedServer)
		{
			yield break;
		}
		if (this.shuttingDownMultiplayerServices)
		{
			yield break;
		}
		this.shuttingDownMultiplayerServices = true;
		bool isClient = false;
		bool success = false;
		bool failReasonProvided = false;
		try
		{
			Log.Out(string.Format("Waiting to Shut Down Multiplayer Services ({0})...", _reason));
			while (this.shuttingDownMultiplayerServices && !this.IsSafeToConnect() && !this.IsSafeToDisconnect())
			{
				yield return null;
			}
			ConnectionManager connectionManager = SingletonMonoBehaviour<ConnectionManager>.Instance;
			for (;;)
			{
				yield return null;
				if (!this.shuttingDownMultiplayerServices)
				{
					break;
				}
				if (this.IsSafeToConnect())
				{
					goto Block_9;
				}
				if (connectionManager == null)
				{
					goto IL_18D;
				}
				ProtocolManager.NetworkType currentMode = connectionManager.CurrentMode;
				if (currentMode == ProtocolManager.NetworkType.None || currentMode == ProtocolManager.NetworkType.OfflineServer)
				{
					goto IL_18D;
				}
				if (this.IsSafeToDisconnect())
				{
					goto Block_12;
				}
			}
			Log.Warning(string.Format("Cancelled Shutting Down Multiplayer Services ({0}) because already shutting down.", _reason));
			failReasonProvided = true;
			yield break;
			Block_9:
			Log.Warning(string.Format("Cancelled Shutting Down Multiplayer Services ({0}) because safe to connect.", _reason));
			failReasonProvided = true;
			yield break;
			IL_18D:
			Log.Warning(string.Format("Cancelled Shutting Down Multiplayer Services ({0}) because no online connection.", _reason));
			failReasonProvided = true;
			yield break;
			Block_12:
			Log.Out(string.Format("Shutting Down Multiplayer Services ({0})...", _reason));
			if (connectionManager.IsClient)
			{
				this.Disconnect();
				isClient = true;
				success = true;
				yield break;
			}
			ClientInfo[] clientInfos = SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.List.ToArray<ClientInfo>();
			if (clientInfos.Length != 0)
			{
				NetPackagePlayerDenied package = NetPackageManager.GetPackage<NetPackagePlayerDenied>().Setup(new GameUtils.KickPlayerData(GameUtils.EKickReason.SessionClosed, 0, default(DateTime), ""));
				ClientInfo[] array = clientInfos;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SendPackage(package);
				}
				yield return new WaitForSecondsRealtime(1f);
				foreach (ClientInfo clientInfo in clientInfos)
				{
					try
					{
						SingletonMonoBehaviour<ConnectionManager>.Instance.DisconnectClient(clientInfo, false, false);
					}
					catch (Exception arg)
					{
						Log.Warning(string.Format("Failed to disconnect client '{0}' : {1}", clientInfo.playerName, arg));
					}
				}
			}
			this.ShutdownMultiplayerServicesNow();
			connectionManager.MakeServerOffline();
			GamePrefs.Set(EnumGamePrefs.ServerMaxPlayerCount, 1);
			success = true;
			connectionManager = null;
			clientInfos = null;
		}
		finally
		{
			this.shuttingDownMultiplayerServices = false;
			if (success)
			{
				Log.Out(string.Format("Multiplayer Services ({0}) have been shut down.", _reason));
				XUiWindowGroup xuiWindowGroup = (XUiWindowGroup)this.windowManager.GetWindow(XUiC_MessageBoxWindowGroup.ID);
				string title = Localization.Get(isClient ? "multiShut_titleClient" : "multiShut_titleHost", false);
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat(Localization.Get("auth_reason", false), Localization.Get(GameManager.GetLocalizationKey(_reason), false));
				if (!isClient)
				{
					stringBuilder.Append('\n');
					stringBuilder.Append(Localization.Get("multiShut_commonHost", false));
				}
				((XUiC_MessageBoxWindowGroup)xuiWindowGroup.Controller).ShowMessage(title, stringBuilder.ToString(), XUiC_MessageBoxWindowGroup.MessageBoxTypes.Ok, null, null, true, true, true);
			}
			else if (!failReasonProvided)
			{
				Log.Warning(string.Format("Failed Shutting Down Multiplayer Services ({0}).", _reason));
			}
		}
		yield break;
		yield break;
	}

	// Token: 0x06007CC2 RID: 31938 RVA: 0x0032798D File Offset: 0x00325B8D
	public void CreateStabilityViewer()
	{
		if (this.stabilityViewer == null)
		{
			this.stabilityViewer = new StabilityViewer();
		}
	}

	// Token: 0x06007CC3 RID: 31939 RVA: 0x003279A2 File Offset: 0x00325BA2
	public void ClearStabilityViewer()
	{
		if (this.stabilityViewer != null)
		{
			this.stabilityViewer.worldIsReady = false;
			this.stabilityViewer.Clear();
			this.stabilityViewer = null;
		}
	}

	// Token: 0x06007CC4 RID: 31940 RVA: 0x003279CC File Offset: 0x00325BCC
	[PublicizedFrom(EAccessModifier.Private)]
	public void setLocalPlayerEntity(EntityPlayerLocal _playerEntity)
	{
		_playerEntity.IsFlyMode.Value = this.IsEditMode();
		_playerEntity.SetEntityName(GamePrefs.GetString(EnumGamePrefs.PlayerName));
		this.myPlayerId = _playerEntity.entityId;
		this.myEntityPlayerLocal = _playerEntity;
		this.persistentLocalPlayer = this.getPersistentPlayerData(null);
		_playerEntity.persistentPlayerData = this.persistentLocalPlayer;
		_playerEntity.InventoryChangedEvent += this.LocalPlayerInventoryChanged;
		_playerEntity.inventory.OnToolbeltItemsChangedInternal += delegate()
		{
			this.sendPlayerToolbelt = true;
		};
		_playerEntity.bag.OnBackpackItemsChangedInternal += delegate()
		{
			this.sendPlayerBag = true;
		};
		_playerEntity.equipment.OnChanged += delegate()
		{
			this.sendPlayerEquipment = true;
		};
		_playerEntity.DragAndDropItemChanged += delegate()
		{
			this.sendDragAndDropItem = true;
		};
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && this.persistentPlayers != null)
		{
			if (this.persistentLocalPlayer == null)
			{
				this.persistentLocalPlayer = this.persistentPlayers.CreatePlayerData(this.getPersistentPlayerID(null), PlatformManager.NativePlatform.User.PlatformUserId, _playerEntity.EntityName, DeviceFlag.StandaloneWindows.ToPlayGroup());
				this.persistentLocalPlayer.EntityId = this.myPlayerId;
				this.persistentPlayers.MapPlayer(this.persistentLocalPlayer);
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackagePersistentPlayerState>().Setup(this.persistentLocalPlayer, EnumPersistentPlayerDataReason.New), true, -1, -1, -1, null, 192, false);
				this.SavePersistentPlayerData();
			}
			else
			{
				this.persistentLocalPlayer.Update(PlatformManager.NativePlatform.User.PlatformUserId, new AuthoredText(_playerEntity.EntityName, this.persistentLocalPlayer.PrimaryId), DeviceFlag.StandaloneWindows.ToPlayGroup());
				this.persistentLocalPlayer.EntityId = this.myPlayerId;
				this.persistentPlayers.MapPlayer(this.persistentLocalPlayer);
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackagePersistentPlayerState>().Setup(this.persistentLocalPlayer, EnumPersistentPlayerDataReason.Login), true, -1, -1, -1, null, 192, false);
			}
		}
		this.m_World.SetLocalPlayer(_playerEntity);
		LocalPlayerUI.DispatchNewPlayerForUI(_playerEntity);
		this.MarkPlayerEntityFriends();
		if (this.OnLocalPlayerChanged != null)
		{
			this.OnLocalPlayerChanged(_playerEntity);
		}
		GameSenseManager instance = GameSenseManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.SessionStarted(_playerEntity);
	}

	// Token: 0x06007CC5 RID: 31941 RVA: 0x00327C02 File Offset: 0x00325E02
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator StartAsServer(bool _offline)
	{
		while (XUiC_WorldGenerationWindowGroup.IsGenerating())
		{
			yield return null;
		}
		Log.Out("StartAsServer");
		GameServerInfo.PrepareLocalServerInfo();
		this.CalculatePersistentPlayerCount(GamePrefs.GetString(EnumGamePrefs.GameWorld), GamePrefs.GetString(EnumGamePrefs.GameName));
		PlatformManager.MultiPlatform.RichPresence.UpdateRichPresence(IRichPresence.PresenceStates.Loading);
		XUiC_ProgressWindow.Open(LocalPlayerUI.primaryUI, Localization.Get("uiLoadLoadingXml", false), null, true, true, true, false);
		yield return null;
		WorldStaticData.Cleanup(null);
		Block.nameIdMapping = null;
		ItemClass.nameIdMapping = null;
		string @string = GamePrefs.GetString(EnumGamePrefs.GameWorld);
		if (!@string.Equals("Empty") && !@string.Equals("Playtesting"))
		{
			string path = GameIO.GetSaveGameDir() + "/main.ttw";
			string text = GameIO.GetSaveGameDir() + "/" + Constants.cFileBlockMappings;
			string text2 = GameIO.GetSaveGameDir() + "/" + Constants.cFileItemMappings;
			if (!SdFile.Exists(path))
			{
				if (!SdDirectory.Exists(GameIO.GetSaveGameDir()))
				{
					SdDirectory.CreateDirectory(GameIO.GetSaveGameDir());
				}
				Block.nameIdMapping = new NameIdMapping(text, Block.MAX_BLOCKS);
				Block.nameIdMapping.WriteToFile();
				ItemClass.nameIdMapping = new NameIdMapping(text2, ItemClass.MAX_ITEMS);
				ItemClass.nameIdMapping.WriteToFile();
			}
			else
			{
				Block.nameIdMapping = new NameIdMapping(text, Block.MAX_BLOCKS);
				if (!Block.nameIdMapping.LoadFromFile())
				{
					Log.Warning("Could not load block-name-mappings file '" + text + "'!");
					Block.nameIdMapping = null;
				}
				ItemClass.nameIdMapping = new NameIdMapping(text2, ItemClass.MAX_ITEMS);
				if (!ItemClass.nameIdMapping.LoadFromFile())
				{
					Log.Warning("Could not load item-name-mappings file '" + text2 + "'!");
					ItemClass.nameIdMapping = null;
				}
			}
		}
		yield return WorldStaticData.LoadAllXmlsCo(false, null);
		yield return null;
		SingletonMonoBehaviour<ConnectionManager>.Instance.ServerReady();
		Manager.CreateServer();
		LightManager.CreateServer();
		this.gameStateManager.InitGame(true);
		yield return null;
		PowerManager.Instance.LoadPowerManager();
		XUiC_ProgressWindow.SetText(LocalPlayerUI.primaryUI, Localization.Get("uiLoadCreatingWorld", false), true);
		yield return null;
		if (this.isEditMode)
		{
			this.persistentPlayers = new PersistentPlayerList();
		}
		else
		{
			this.persistentPlayers = PersistentPlayerList.ReadXML(GameIO.GetSaveGameDir() + "/players.xml");
			if (this.persistentPlayers != null && this.persistentPlayers.CleanupPlayers())
			{
				this.SavePersistentPlayerData();
			}
		}
		yield return this.createWorld(GamePrefs.GetString(EnumGamePrefs.GameWorld), GamePrefs.GetString(EnumGamePrefs.GameName), null, false);
		GameServerInfo.SetLocalServerWorldInfo();
		NetPackageWorldInfo.PrepareWorldHashes();
		this.FreeAllTileEntityLocks();
		yield return null;
		XUiC_ProgressWindow.SetText(LocalPlayerUI.primaryUI, Localization.Get("uiLoadCreatingPlayer", false), true);
		yield return null;
		if (!GameManager.IsDedicatedServer)
		{
			GameManager.<>c__DisplayClass162_0 CS$<>8__locals1 = new GameManager.<>c__DisplayClass162_0();
			CS$<>8__locals1.persistentPlayerId = this.getPersistentPlayerID(null).CombinedString;
			if (!GamePrefs.GetBool(EnumGamePrefs.SkipSpawnButton) && !this.IsEditMode())
			{
				this.canSpawnPlayer = false;
				bool firstTimeSpawn = !PlayerDataFile.Exists(GameIO.GetPlayerDataDir(), CS$<>8__locals1.persistentPlayerId);
				XUiC_SpawnSelectionWindow.Open(LocalPlayerUI.primaryUI, false, true, firstTimeSpawn);
				while (!this.canSpawnPlayer)
				{
					yield return null;
				}
				yield return new WaitForSeconds(0.1f);
			}
			PlayerDataFile playerDataFile = new PlayerDataFile();
			playerDataFile.Load(GameIO.GetPlayerDataDir(), CS$<>8__locals1.persistentPlayerId);
			EntityCreationData entityCreationData = new EntityCreationData();
			Vector3 pos;
			Vector3 rot;
			int num2;
			if (playerDataFile.bLoaded)
			{
				pos = playerDataFile.ecd.pos;
				rot = new Vector3(playerDataFile.ecd.rot.x, playerDataFile.ecd.rot.y, 0f);
				if (this.isEditMode)
				{
					playerDataFile.id = -1;
				}
				int num;
				if (playerDataFile.id == -1)
				{
					EntityFactory.nextEntityID = (num = EntityFactory.nextEntityID) + 1;
				}
				else
				{
					num = playerDataFile.id;
				}
				num2 = num;
				entityCreationData.entityData = playerDataFile.ecd.entityData;
				entityCreationData.readFileVersion = playerDataFile.ecd.readFileVersion;
			}
			else
			{
				SpawnPosition randomSpawnPosition = this.GetSpawnPointList().GetRandomSpawnPosition(this.m_World, null, 0, 0);
				pos = randomSpawnPosition.position;
				rot = new Vector3(0f, randomSpawnPosition.heading, 0f);
				num2 = EntityFactory.nextEntityID++;
			}
			if (playerDataFile.bLoaded && playerDataFile.ecd.playerProfile != null && GamePrefs.GetBool(EnumGamePrefs.PersistentPlayerProfiles))
			{
				entityCreationData.entityClass = EntityClass.FromString(playerDataFile.ecd.playerProfile.EntityClassName);
				entityCreationData.playerProfile = playerDataFile.ecd.playerProfile;
			}
			else
			{
				entityCreationData.playerProfile = PlayerProfile.LoadLocalProfile();
				entityCreationData.entityClass = EntityClass.FromString(entityCreationData.playerProfile.EntityClassName);
			}
			entityCreationData.skinTexture = GamePrefs.GetString(EnumGamePrefs.OptionsPlayerModelTexture);
			entityCreationData.id = num2;
			entityCreationData.pos = pos;
			entityCreationData.rot = rot;
			entityCreationData.belongsPlayerId = num2;
			EntityPlayerLocal entityPlayerLocal = (EntityPlayerLocal)EntityFactory.CreateEntity(entityCreationData);
			this.setLocalPlayerEntity(entityPlayerLocal);
			if (playerDataFile.bLoaded)
			{
				playerDataFile.ToPlayer(entityPlayerLocal);
				entityPlayerLocal.SetFirstPersonView(true, false);
			}
			this.m_World.SpawnEntityInWorld(entityPlayerLocal);
			this.myEntityPlayerLocal.Respawn(playerDataFile.bLoaded ? RespawnType.LoadedGame : RespawnType.NewGame);
			this.myEntityPlayerLocal.ChunkObserver = this.m_World.m_ChunkManager.AddChunkObserver(this.myEntityPlayerLocal.GetPosition(), true, Utils.FastMin(12, GameUtils.GetViewDistance()), -1);
			IMapChunkDatabase.TryCreateOrLoad(this.myEntityPlayerLocal.entityId, out this.myEntityPlayerLocal.ChunkObserver.mapDatabase, () => new IMapChunkDatabase.DirectoryPlayerId(GameIO.GetPlayerDataDir(), CS$<>8__locals1.persistentPlayerId));
			LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(entityPlayerLocal);
			uiforPlayer.xui.SetDataConnections();
			uiforPlayer.xui.SetCraftingData(playerDataFile.craftingData);
			CS$<>8__locals1 = null;
		}
		Log.Out("Loaded player");
		yield return null;
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			VehicleManager.Init();
			DroneManager.Init();
			TurretTracker.Init();
			RaycastPathManager.Init();
			EntityCoverManager.Init();
			BlockLimitTracker.Init();
		}
		yield return null;
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && this.m_World.ChunkClusters[0].IsFixedSize && !this.IsEditMode() && this.m_World.m_WorldEnvironment != null)
		{
			this.m_World.m_WorldEnvironment.SetColliders((float)((this.m_World.ChunkClusters[0].ChunkMinPos.x + 1) * 16), (float)((this.m_World.ChunkClusters[0].ChunkMinPos.y + 1) * 16), (float)((this.m_World.ChunkClusters[0].ChunkMaxPos.x - this.m_World.ChunkClusters[0].ChunkMinPos.x - 1) * 16), (float)((this.m_World.ChunkClusters[0].ChunkMaxPos.y - this.m_World.ChunkClusters[0].ChunkMinPos.y - 1) * 16), Constants.cSizePlanesAround, 0f);
			this.m_World.m_WorldEnvironment.CreateLevelBorderBox(this.m_World);
		}
		if (this.isEditMode)
		{
			PrefabEditModeManager instance = PrefabEditModeManager.Instance;
			if (instance != null)
			{
				instance.Init();
			}
			yield return null;
		}
		yield return null;
		if (GameManager.IsDedicatedServer || !_offline)
		{
			ServerInformationTcpProvider.Instance.StartServer();
			IMasterServerAnnouncer serverListAnnouncer = PlatformManager.MultiPlatform.ServerListAnnouncer;
			if (serverListAnnouncer != null)
			{
				serverListAnnouncer.AdvertiseServer(delegate
				{
					ILobbyHost lobbyHost = PlatformManager.NativePlatform.LobbyHost;
					if (lobbyHost != null)
					{
						lobbyHost.UpdateLobby(SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo);
					}
					ModEvents.SServerRegisteredData sserverRegisteredData;
					ModEvents.ServerRegistered.Invoke(ref sserverRegisteredData);
				});
			}
			PlayerInteractions.Instance.JoinedMultiplayerServer(this.persistentPlayers);
			AuthorizationManager.Instance.ServerStart();
		}
		else
		{
			GamePrefs.Set(EnumGamePrefs.ServerMaxPlayerCount, 1);
		}
		yield return GCUtils.UnloadAndCollectCo();
		this.gameStateManager.StartGame();
		yield break;
	}

	// Token: 0x06007CC6 RID: 31942 RVA: 0x00327C18 File Offset: 0x00325E18
	[PublicizedFrom(EAccessModifier.Private)]
	public void StartAsClient()
	{
		Log.Out("StartAsClient");
		this.worldCreated = false;
		this.chunkClusterLoaded = false;
		GamePrefs.Set(EnumGamePrefs.GameMode, string.Empty);
		GamePrefs.Set(EnumGamePrefs.GameWorld, string.Empty);
		WorldStaticData.WaitForConfigsFromServer();
		PlatformManager.MultiPlatform.RichPresence.UpdateRichPresence(IRichPresence.PresenceStates.Connecting);
		IAntiCheatClient antiCheatClient = PlatformManager.MultiPlatform.AntiCheatClient;
		if (antiCheatClient == null || !antiCheatClient.ClientAntiCheatEnabled())
		{
			Log.Out("Sending RequestToEnterGame...");
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageRequestToEnterGame>(), false);
		}
		else
		{
			PlatformManager.MultiPlatform.AntiCheatClient.WaitForRemoteAuth(delegate
			{
				Log.Out("Sending RequestToEnterGame...");
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageRequestToEnterGame>(), false);
			});
		}
		XUiC_ProgressWindow.SetText(LocalPlayerUI.primaryUI, Localization.Get("uiLoadWaitingForConfigs", false), true);
		BlockLimitTracker.Init();
	}

	// Token: 0x06007CC7 RID: 31943 RVA: 0x00327CEA File Offset: 0x00325EEA
	public bool IsSafeToConnect()
	{
		return SingletonMonoBehaviour<ConnectionManager>.Instance.CurrentMode == ProtocolManager.NetworkType.None;
	}

	// Token: 0x06007CC8 RID: 31944 RVA: 0x00327CFC File Offset: 0x00325EFC
	public bool IsSafeToDisconnect()
	{
		return SingletonMonoBehaviour<ConnectionManager>.Instance.CurrentMode == ProtocolManager.NetworkType.None || ((!PrefabEditModeManager.Instance.IsActive() || !PrefabEditModeManager.Instance.NeedsSaving) && (this.gameStateManager.IsGameStarted() && !this.IsStartingGame) && !this.isDisconnectingLater);
	}

	// Token: 0x06007CC9 RID: 31945 RVA: 0x00327D54 File Offset: 0x00325F54
	public void Disconnect()
	{
		Log.Out("Disconnect");
		if (!GameManager.IsDedicatedServer)
		{
			this.windowManager.CloseAllOpenWindows(null, false);
			if (this.m_World != null)
			{
				List<EntityPlayerLocal> localPlayers = this.m_World.GetLocalPlayers();
				for (int i = 0; i < localPlayers.Count; i++)
				{
					LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(localPlayers[i]);
					if (null != uiforPlayer && null != uiforPlayer.windowManager)
					{
						uiforPlayer.windowManager.CloseAllOpenWindows(null, false);
						uiforPlayer.xui.gameObject.SetActive(false);
					}
				}
			}
			LocalPlayerUI.primaryUI.windowManager.Close(XUiC_SubtitlesDisplay.ID);
			Manager.StopAllLocal();
		}
		this.Pause(false);
		if (!GameManager.IsDedicatedServer && !this.isEditMode && null != this.myEntityPlayerLocal)
		{
			GameSenseManager instance = GameSenseManager.Instance;
			if (instance != null)
			{
				instance.SessionEnded();
			}
			this.myEntityPlayerLocal.FireEvent(MinEventTypes.onSelfLeaveGame, true);
			this.myEntityPlayerLocal.dropItemOnQuit();
			if (this.myEntityPlayerLocal.AttachedToEntity != null)
			{
				this.triggerEffectManager.StopGamepadVibration();
				this.myEntityPlayerLocal.Detach();
			}
		}
		if (!GameManager.IsDedicatedServer)
		{
			PlatformManager.MultiPlatform.User.StopAdvertisePlaying();
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePlayerDisconnect>().Setup(this.myEntityPlayerLocal), true);
			base.StartCoroutine(this.disconnectLater());
		}
		else if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.StopServers();
		}
		else
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.DisconnectFromServer();
		}
		if (GameSparksManager.Instance() != null)
		{
			GameSparksManager.Instance().SessionEnded();
		}
	}

	// Token: 0x06007CCA RID: 31946 RVA: 0x00327EFF File Offset: 0x003260FF
	[PublicizedFrom(EAccessModifier.Protected)]
	public IEnumerator disconnectLater()
	{
		this.isDisconnectingLater = true;
		yield return new WaitForSeconds(0.2f);
		SingletonMonoBehaviour<ConnectionManager>.Instance.Disconnect();
		GamePrefs.Set(EnumGamePrefs.GameGuidClient, "");
		this.isDisconnectingLater = false;
		yield break;
	}

	// Token: 0x06007CCB RID: 31947 RVA: 0x00327F10 File Offset: 0x00326110
	public void SaveAndCleanupWorld()
	{
		Log.Out("SaveAndCleanupWorld");
		ModEvents.SWorldShuttingDownData sworldShuttingDownData;
		ModEvents.WorldShuttingDown.Invoke(ref sworldShuttingDownData);
		this.shuttingDownMultiplayerServices = false;
		PathAbstractions.CacheEnabled = false;
		this.OnClientSpawned = null;
		PlayerInputRecordingSystem.Instance.AutoSave();
		this.gameStateManager.EndGame();
		PlatformManager.MultiPlatform.RichPresence.UpdateRichPresence(IRichPresence.PresenceStates.Menu);
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && GameManager.bSavingActive && !this.IsEditMode())
		{
			if (VehicleManager.Instance != null)
			{
				VehicleManager.Instance.RemoveAllVehiclesFromMap();
			}
			if (DroneManager.Instance != null)
			{
				DroneManager.Instance.RemoveAllDronesFromMap();
			}
			if (QuestEventManager.HasInstance)
			{
				QuestEventManager.Current.HandleAllPlayersDisconnect();
			}
			this.SaveLocalPlayerData();
			this.SaveWorld();
			World world = this.m_World;
			EntityPlayerLocal entityPlayerLocal = (world != null) ? world.GetPrimaryPlayer() : null;
			if (this.persistentPlayers != null)
			{
				foreach (KeyValuePair<PlatformUserIdentifierAbs, PersistentPlayerData> keyValuePair in this.persistentPlayers.Players)
				{
					if (keyValuePair.Value.EntityId != -1)
					{
						if (entityPlayerLocal && keyValuePair.Value.EntityId == entityPlayerLocal.entityId)
						{
							keyValuePair.Value.Position = new Vector3i(entityPlayerLocal.position);
						}
						keyValuePair.Value.LastLogin = DateTime.Now;
					}
				}
				this.SavePersistentPlayerData();
			}
		}
		if (Block.nameIdMapping != null)
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				Block.nameIdMapping.SaveIfDirty(true);
			}
			Block.nameIdMapping = null;
		}
		if (ItemClass.nameIdMapping != null)
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				ItemClass.nameIdMapping.SaveIfDirty(true);
			}
			ItemClass.nameIdMapping = null;
		}
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && GameManager.bSavingActive && !this.IsEditMode())
		{
			if (this.m_World != null && this.m_World.GetPrimaryPlayer() != null && this.m_World.GetPrimaryPlayer().ChunkObserver.mapDatabase != null)
			{
				ThreadManager.AddSingleTask(new ThreadManager.TaskFunctionDelegate(this.m_World.GetPrimaryPlayer().ChunkObserver.mapDatabase.SaveAsync), new IMapChunkDatabase.DirectoryPlayerId(GameIO.GetPlayerDataLocalDir(), this.persistentLocalPlayer.PrimaryId.CombinedString), null, true);
			}
			if (!GameManager.IsDedicatedServer && this.m_World != null)
			{
				foreach (EntityPlayerLocal entityPlayerLocal2 in this.m_World.GetLocalPlayers())
				{
					entityPlayerLocal2.EnableCamera(false);
					entityPlayerLocal2.SetControllable(false);
				}
			}
		}
		this.ShutdownMultiplayerServicesNow();
		PlayerInteractions.Instance.OnNewPlayerInteraction -= this.HandleFirstSpawnInteractions;
		PlayerInteractions.Instance.Shutdown();
		IGameplayNotifier gameplayNotifier = PlatformManager.NativePlatform.GameplayNotifier;
		if (gameplayNotifier != null)
		{
			gameplayNotifier.GameplayEnd();
		}
		if (!GameManager.IsDedicatedServer)
		{
			if (this.myEntityPlayerLocal != null)
			{
				this.myEntityPlayerLocal.EnableCamera(false);
				this.myEntityPlayerLocal.SetControllable(false);
				if (this.OnLocalPlayerChanged != null)
				{
					this.OnLocalPlayerChanged(null);
				}
				this.m_World.RemoveEntity(this.myPlayerId, EnumRemoveEntityReason.Unloaded);
				this.myPlayerId = -1;
				this.myEntityPlayerLocal = null;
			}
			foreach (LocalPlayerUI localPlayerUI in LocalPlayerUI.PlayerUIs)
			{
				if (!localPlayerUI.isPrimaryUI && !localPlayerUI.IsCleanCopy)
				{
					if (localPlayerUI.entityPlayer)
					{
						localPlayerUI.entityPlayer.EnableCamera(false);
						localPlayerUI.entityPlayer.SetControllable(false);
						World world2 = this.m_World;
						if (world2 != null)
						{
							world2.RemoveEntity(localPlayerUI.entityPlayer.entityId, EnumRemoveEntityReason.Unloaded);
						}
					}
					if (localPlayerUI.gameObject)
					{
						localPlayerUI.xui.Shutdown(false);
						localPlayerUI.windowManager.CloseAllOpenWindows(null, false);
						UnityEngine.Object.Destroy(localPlayerUI.gameObject);
					}
				}
			}
		}
		ModManager.GameEnded();
		if (!GameManager.IsDedicatedServer)
		{
			if (!PlatformApplicationManager.IsRestartRequired)
			{
				GameManager.LoadRemoteResources(null);
			}
			this.windowManager.Close(GUIWindowConsole.ID);
			this.windowManager.Close(XUiC_LoadingScreen.ID);
			if (!GameManager.bHideMainMenuNextTime)
			{
				this.windowManager.Open(XUiC_MainMenu.ID, true, false, true);
			}
			GameManager.bHideMainMenuNextTime = false;
		}
		if (PrefabSleeperVolumeManager.Instance != null)
		{
			PrefabSleeperVolumeManager.Instance.Cleanup();
		}
		if (PrefabVolumeManager.Instance != null)
		{
			PrefabVolumeManager.Instance.Cleanup();
		}
		AstarManager.Cleanup();
		DynamicMeshManager.OnWorldUnload();
		if (GameEventManager.HasInstance)
		{
			GameEventManager.Current.Cleanup();
		}
		if (this.m_World != null)
		{
			if (this.OnWorldChanged != null)
			{
				this.OnWorldChanged(null);
			}
			this.prefabLODManager.Cleanup();
			PrefabEditModeManager instance = PrefabEditModeManager.Instance;
			if (instance != null && instance.IsActive())
			{
				PrefabEditModeManager instance2 = PrefabEditModeManager.Instance;
				if (instance2 != null)
				{
					instance2.Cleanup();
				}
			}
			EnvironmentAudioManager.DestroyInstance();
			LightManager.Clear();
			SkyManager.Cleanup();
			WeatherManager.Cleanup();
			CharacterGazeController.Cleanup();
			WaterSplashCubes.Clear();
			WaterEvaporationManager.ClearAll();
			SleeperVolumeToolManager.CleanUp();
			this.ClearStabilityViewer();
			if (this.m_World.GetPrimaryPlayer() && this.m_World.GetPrimaryPlayer().DynamicMusicManager != null)
			{
				this.m_World.GetPrimaryPlayer().DynamicMusicManager.CleanUpDynamicMembers();
			}
			this.m_World.UnloadWorld(true);
			this.m_World.Cleanup();
			this.m_World = null;
			this.GameHasStarted = false;
		}
		WaterSimulationNative.Instance.Cleanup();
		ProjectileManager.Cleanup();
		VehicleManager.Cleanup();
		DroneManager.Cleanup();
		DismembermentManager.Cleanup();
		TurretTracker.Cleanup();
		BlockLimitTracker.Cleanup();
		MapObjectManager.Reset();
		vp_TargetEventHandler.UnregisterAll();
		this.lootManager = null;
		this.traderManager = null;
		if (QuestEventManager.HasInstance)
		{
			QuestEventManager.Current.Cleanup();
		}
		if (TwitchVoteScheduler.HasInstance)
		{
			TwitchVoteScheduler.Current.Cleanup();
		}
		if (TwitchManager.HasInstance)
		{
			TwitchManager.Current.Cleanup();
		}
		if (PowerManager.HasInstance)
		{
			PowerManager.Instance.Cleanup();
		}
		if (WireManager.HasInstance)
		{
			WireManager.Instance.Cleanup();
		}
		if (PartyManager.HasInstance)
		{
			PartyManager.Current.Cleanup();
		}
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && GameManager.bSavingActive)
		{
			this.IsEditMode();
		}
		if (UIDisplayInfoManager.HasInstance)
		{
			UIDisplayInfoManager.Current.Cleanup();
		}
		if (TextureLoadingManager.Instance != null)
		{
			TextureLoadingManager.Instance.Cleanup();
		}
		if (NavObjectManager.HasInstance)
		{
			NavObjectManager.Instance.Cleanup();
		}
		SelectionBoxManager.Instance.Clear();
		Origin.Cleanup();
		GameObjectPool.Instance.Cleanup();
		MemoryPools.Cleanup();
		VoxelMeshLayer.StaticCleanup();
		GamePrefs.Instance.Save();
		GameManager.bRecordNextSession = false;
		GameManager.bPlayRecordedSession = false;
	}

	// Token: 0x06007CCC RID: 31948 RVA: 0x003285D8 File Offset: 0x003267D8
	[PublicizedFrom(EAccessModifier.Private)]
	public void ShutdownMultiplayerServicesNow()
	{
		if (!GameManager.IsDedicatedServer)
		{
			PlatformManager.MultiPlatform.User.StopAdvertisePlaying();
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			AuthorizationManager.Instance.ServerStop();
		}
		ILobbyHost lobbyHost = PlatformManager.NativePlatform.LobbyHost;
		if (lobbyHost != null)
		{
			lobbyHost.ExitLobby();
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			PlatformManager.MultiPlatform.ServerListAnnouncer.StopServer();
			ServerInformationTcpProvider.Instance.StopServer();
		}
		IGameplayNotifier gameplayNotifier = PlatformManager.NativePlatform.GameplayNotifier;
		if (gameplayNotifier == null)
		{
			return;
		}
		gameplayNotifier.EndOnlineMultiplayer();
	}

	// Token: 0x06007CCD RID: 31949 RVA: 0x0032865F File Offset: 0x0032685F
	public void SaveWorld()
	{
		if (this.m_World != null)
		{
			this.m_World.Save();
		}
	}

	// Token: 0x06007CCE RID: 31950 RVA: 0x00328674 File Offset: 0x00326874
	public void SaveLocalPlayerData()
	{
		if (this.m_World == null)
		{
			return;
		}
		EntityPlayerLocal primaryPlayer = this.m_World.GetPrimaryPlayer();
		if (primaryPlayer == null || !GameManager.bSavingActive)
		{
			return;
		}
		string combinedString = this.getPersistentPlayerID(null).CombinedString;
		PlayerDataFile playerDataFile = new PlayerDataFile();
		playerDataFile.FromPlayer(primaryPlayer);
		playerDataFile.Save(GameIO.GetPlayerDataDir(), combinedString);
		if (primaryPlayer.ChunkObserver.mapDatabase != null)
		{
			ThreadManager.AddSingleTask(new ThreadManager.TaskFunctionDelegate(primaryPlayer.ChunkObserver.mapDatabase.SaveAsync), new IMapChunkDatabase.DirectoryPlayerId(GameIO.GetPlayerDataDir(), combinedString), null, true);
		}
	}

	// Token: 0x06007CCF RID: 31951 RVA: 0x00328704 File Offset: 0x00326904
	public void Cleanup()
	{
		Log.Out("Cleanup");
		WaterSimulationNative.Instance.Cleanup();
		ModEvents.SGameShutdownData sgameShutdownData;
		ModEvents.GameShutdown.Invoke(ref sgameShutdownData);
		AuthorizationManager.Instance.Cleanup();
		VehicleManager.Cleanup();
		Cursor.visible = true;
		Cursor.lockState = SoftCursor.DefaultCursorLockState;
		SingletonMonoBehaviour<SdtdConsole>.Instance.Cleanup();
		WorldStaticData.Cleanup();
		this.adminTools = null;
		GUIWindowConsole guiconsole = this.m_GUIConsole;
		if (guiconsole != null)
		{
			guiconsole.Shutdown();
		}
		GameObjectPool.Instance.Cleanup();
		SaveDataUtils.SaveDataManager.Cleanup();
		LocalPlayerManager.Destroy();
		PlatformManager.Destroy();
		LoadManager.Destroy();
		TaskManager.Destroy();
		MemoryPools.Cleanup();
		GC.Collect();
	}

	// Token: 0x17000D11 RID: 3345
	// (get) Token: 0x06007CD0 RID: 31952 RVA: 0x003287A9 File Offset: 0x003269A9
	public bool IsQuitting
	{
		get
		{
			return this.isQuitting;
		}
	}

	// Token: 0x06007CD1 RID: 31953 RVA: 0x003287B4 File Offset: 0x003269B4
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool OnApplicationQuit()
	{
		AdminTools adminTools = this.adminTools;
		if (adminTools != null)
		{
			adminTools.DestroyFileWatcher();
		}
		if (!this.allowQuit)
		{
			if (!this.isQuitting)
			{
				this.isQuitting = true;
				base.StartCoroutine(this.ApplicationQuitCo(0.3f));
			}
			return false;
		}
		GameSenseManager instance = GameSenseManager.Instance;
		if (instance != null)
		{
			instance.Cleanup();
		}
		ThreadManager.Shutdown();
		WorldStaticData.QuitCleanup();
		if (SingletonMonoBehaviour<SdtdConsole>.Instance != null)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Cleanup();
		}
		Log.Out("OnApplicationQuit");
		return true;
	}

	// Token: 0x06007CD2 RID: 31954 RVA: 0x0032883C File Offset: 0x00326A3C
	public void OnApplicationFocus(bool _focus)
	{
		if (!GameManager.IsDedicatedServer)
		{
			this.GameIsFocused = _focus;
			if (Application.isEditor)
			{
				return;
			}
			if (!_focus)
			{
				this.setCursorEnabled(true);
			}
			else if (this.bCursorVisibleOverride)
			{
				this.setCursorEnabled(this.bCursorVisibleOverrideState);
			}
			else if (!this.isAnyCursorWindowOpen(null))
			{
				this.setCursorEnabled(false);
			}
			if (ActionSetManager.DebugLevel != ActionSetManager.EDebugLevel.Off)
			{
				Log.Out("Focus: " + _focus.ToString());
				Log.Out("Input state:");
				foreach (PlayerActionsBase playerActionsBase in PlatformManager.NativePlatform.Input.ActionSets)
				{
					Log.Out(string.Format("   {0}: {1}", playerActionsBase.GetType().Name, playerActionsBase.Enabled));
				}
				Log.Out("Modal window open: " + LocalPlayerUI.PlayerUIs.Any((LocalPlayerUI ui) => ui.windowManager.IsModalWindowOpen()).ToString());
				Log.Out("Cursor window: " + this.isAnyCursorWindowOpen(null).ToString());
			}
			ModEvents.SGameFocusData sgameFocusData = new ModEvents.SGameFocusData(_focus);
			ModEvents.GameFocus.Invoke(ref sgameFocusData);
		}
	}

	// Token: 0x06007CD3 RID: 31955 RVA: 0x00328998 File Offset: 0x00326B98
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isAnyModalWindowOpen()
	{
		IList<LocalPlayerUI> playerUIs = LocalPlayerUI.PlayerUIs;
		for (int i = playerUIs.Count - 1; i >= 0; i--)
		{
			if (playerUIs[i].windowManager.IsModalWindowOpen())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06007CD4 RID: 31956 RVA: 0x003289D4 File Offset: 0x00326BD4
	public bool isAnyCursorWindowOpen(LocalPlayerUI _ui = null)
	{
		if (_ui == null)
		{
			IList<LocalPlayerUI> playerUIs = LocalPlayerUI.PlayerUIs;
			for (int i = 0; i < playerUIs.Count; i++)
			{
				if (!playerUIs[i].windowManager.IsWindowOpen("timer") && (playerUIs[i].windowManager.IsModalWindowOpen() || playerUIs[i].windowManager.IsCursorWindowOpen()))
				{
					return true;
				}
			}
		}
		else if (_ui.windowManager.IsModalWindowOpen() || _ui.windowManager.IsCursorWindowOpen())
		{
			return true;
		}
		return false;
	}

	// Token: 0x06007CD5 RID: 31957 RVA: 0x00328A60 File Offset: 0x00326C60
	public void SetCursorEnabledOverride(bool _bOverrideOn, bool _bOverrideState)
	{
		if (this.bCursorVisibleOverride != _bOverrideOn)
		{
			this.bCursorVisibleOverride = _bOverrideOn;
			this.setCursorEnabled(_bOverrideState);
		}
	}

	// Token: 0x06007CD6 RID: 31958 RVA: 0x00328A79 File Offset: 0x00326C79
	public bool GetCursorEnabledOverride()
	{
		return this.bCursorVisibleOverride;
	}

	// Token: 0x06007CD7 RID: 31959 RVA: 0x00328A81 File Offset: 0x00326C81
	[PublicizedFrom(EAccessModifier.Private)]
	public void setCursorEnabled(bool _e)
	{
		if (this.IsQuitting)
		{
			return;
		}
		this.bCursorVisible = _e;
		if (ActionSetManager.DebugLevel == ActionSetManager.EDebugLevel.Verbose)
		{
			Log.Out("CursorEnabled: " + _e.ToString());
		}
		SoftCursor.SetCursorVisible(this.bCursorVisible);
	}

	// Token: 0x06007CD8 RID: 31960 RVA: 0x00328ABC File Offset: 0x00326CBC
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator ApplicationQuitCo(float _delay)
	{
		Log.Out("Preparing quit");
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.CurrentMode != ProtocolManager.NetworkType.None)
		{
			try
			{
				this.Disconnect();
			}
			catch (Exception e)
			{
				Log.Error("Disconnecting failed:");
				Log.Exception(e);
			}
			yield return new WaitForSeconds(_delay);
		}
		if (!GameManager.IsDedicatedServer)
		{
			this.windowManager.CloseAllOpenWindows(null, false);
		}
		GamePrefs.Instance.Save();
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.StopServers();
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.Disconnect();
		}
		this.Cleanup();
		yield return new WaitForSeconds(0.05f);
		this.allowQuit = true;
		Application.Quit();
		yield break;
	}

	// Token: 0x06007CD9 RID: 31961 RVA: 0x00328AD4 File Offset: 0x00326CD4
	public void ShowMessagePlayerDenied(GameUtils.KickPlayerData _kickData)
	{
		Log.Out("[NET] Kicked from server: " + _kickData.ToString());
		(((XUiWindowGroup)this.windowManager.GetWindow(XUiC_MessageBoxWindowGroup.ID)).Controller as XUiC_MessageBoxWindowGroup).ShowMessage(Localization.Get("auth_messageTitle", false), _kickData.LocalizedMessage(), XUiC_MessageBoxWindowGroup.MessageBoxTypes.Ok, null, null, true, true, true);
	}

	// Token: 0x06007CDA RID: 31962 RVA: 0x00328B3C File Offset: 0x00326D3C
	public void ShowMessageServerAuthFailed(string _message)
	{
		Log.Out("Client failed to authorize server: " + _message);
		((XUiC_MessageBoxWindowGroup)((XUiWindowGroup)this.windowManager.GetWindow(XUiC_MessageBoxWindowGroup.ID)).Controller).ShowMessage(Localization.Get("auth_serverAuthFailedTitle", false), _message, XUiC_MessageBoxWindowGroup.MessageBoxTypes.Ok, null, null, true, true, true);
	}

	// Token: 0x06007CDB RID: 31963 RVA: 0x00328B90 File Offset: 0x00326D90
	public void PlayerLoginRPC(ClientInfo _cInfo, string _playerName, [TupleElementNames(new string[]
	{
		"userId",
		"token"
	})] ValueTuple<PlatformUserIdentifierAbs, string> _platformUserAndToken, [TupleElementNames(new string[]
	{
		"userId",
		"token"
	})] ValueTuple<PlatformUserIdentifierAbs, string> _crossplatformUserAndToken, string _compatibilityVersion, ulong _discordUserId)
	{
		Log.Out("PlayerLogin: " + _playerName + "/" + _compatibilityVersion);
		Log.Out("Client IP: " + _cInfo.ip);
		AuthorizationManager.Instance.Authorize(_cInfo, _playerName, _platformUserAndToken, _crossplatformUserAndToken, _compatibilityVersion, _discordUserId);
	}

	// Token: 0x06007CDC RID: 31964 RVA: 0x00328BDC File Offset: 0x00326DDC
	public IEnumerator RequestToEnterGame(ClientInfo _cInfo)
	{
		ModEvents.SPlayerJoinedGameData splayerJoinedGameData = new ModEvents.SPlayerJoinedGameData(_cInfo);
		ModEvents.PlayerJoinedGame.Invoke(ref splayerJoinedGameData);
		string playerName = _cInfo.playerName;
		Log.Out("RequestToEnterGame: " + _cInfo.InternalId.CombinedString + "/" + playerName);
		IPlatformUserData userData = PlatformUserManager.GetOrCreate(_cInfo.CrossplatformId);
		if (userData != null)
		{
			userData.MarkBlockedStateChanged();
			yield return PlatformUserManager.ResolveUserBlockedCoroutine(userData);
			if (userData.Blocked[EBlockType.Play].IsBlocked())
			{
				Log.Out(string.Format("Player {0} is blocked", _cInfo.InternalId));
				_cInfo.SendPackage(NetPackageManager.GetPackage<NetPackagePlayerDenied>().Setup(new GameUtils.KickPlayerData(GameUtils.EKickReason.ManualKick, 0, default(DateTime), "")));
				yield break;
			}
		}
		if ((DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent() && !this.persistentPlayerIds.Contains(_cInfo.InternalId.ToString()))
		{
			if (this.persistentPlayerCount + 1 > 100)
			{
				Log.Out("Persistent player data entries limit reached, rejecting new player {0}", new object[]
				{
					_cInfo.InternalId.ToString()
				});
				_cInfo.SendPackage(NetPackageManager.GetPackage<NetPackagePlayerDenied>().Setup(new GameUtils.KickPlayerData(GameUtils.EKickReason.PersistentPlayerDataExceeded, 0, default(DateTime), "")));
				yield break;
			}
			this.persistentPlayerIds.Add(_cInfo.InternalId.ToString());
		}
		PersistentPlayerList playerList = (this.persistentPlayers != null) ? this.persistentPlayers.NetworkCloneRelevantForPlayer() : null;
		_cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageIdMapping>().Setup("blocks", Block.fullMappingDataForClients));
		_cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageIdMapping>().Setup("items", ItemClass.fullMappingDataForClients));
		_cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageLocalization>().Setup(Localization.PatchedData));
		WorldStaticData.SendXmlsToClient(_cInfo);
		PlatformUserIdentifierAbs persistentPlayerID = this.getPersistentPlayerID(_cInfo);
		bool flag = !PlayerDataFile.Exists(GameIO.GetPlayerDataDir(), persistentPlayerID.CombinedString);
		_cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageWorldInfo>().Setup(GamePrefs.GetString(EnumGamePrefs.GameMode), GamePrefs.GetString(EnumGamePrefs.GameWorld), GamePrefs.GetString(EnumGamePrefs.GameName), this.m_World.Guid, playerList, GameTimer.Instance.ticks, this.m_World.ChunkCache.IsFixedSize, flag, this.m_World.GetAllWallVolumes()));
		DecoManager.Instance.SendDecosToClient(_cInfo);
		for (int i = 0; i < this.m_World.ChunkClusters.Count; i++)
		{
			ChunkCluster chunkCluster = this.m_World.ChunkClusters[i];
			if (chunkCluster != null)
			{
				_cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageChunkClusterInfo>().Setup(chunkCluster));
			}
		}
		_cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageWorldSpawnPoints>().Setup(this.GetSpawnPointList()));
		_cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageWorldAreas>().Setup(this.m_World.TraderAreas));
		_cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageGameStats>().Setup(GameStats.Instance));
		yield break;
	}

	// Token: 0x06007CDD RID: 31965 RVA: 0x00328BF4 File Offset: 0x00326DF4
	public void WorldInfo(string _gameMode, string _levelName, string _gameName, string _guid, PersistentPlayerList _playerList, ulong _ticks, bool _fixedSizeCC, bool _firstTimeJoin, Dictionary<string, uint> _worldFileHashes, long _worldDataSize, List<WallVolume> _wallVolumes)
	{
		Log.Out("Received game GUID: " + _guid);
		GamePrefs.Set(EnumGamePrefs.GameMode, _gameMode);
		GamePrefs.Set(EnumGamePrefs.GameGuidClient, _guid);
		GamePrefs.Set(EnumGamePrefs.GameWorld, _levelName);
		this.persistentPlayers = _playerList;
		base.StartCoroutine(this.worldInfoCo(_levelName, _gameName, _fixedSizeCC, _firstTimeJoin, _worldFileHashes, _worldDataSize, _wallVolumes));
	}

	// Token: 0x06007CDE RID: 31966 RVA: 0x00328C50 File Offset: 0x00326E50
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator worldInfoCo(string _levelName, string _gameName, bool _fixedSizeCC, bool _firstTimeJoin, Dictionary<string, uint> _worldFileHashes, long _worldDataSize, List<WallVolume> _wallVolumes)
	{
		while (!WorldStaticData.AllConfigsReceivedAndLoaded())
		{
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsConnected)
			{
				yield break;
			}
			yield return null;
		}
		GeneratedTextManager.PrefilterText(SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo.ServerLoginConfirmationText, GeneratedTextManager.TextFilteringMode.Filter);
		XUiC_ProgressWindow.SetText(LocalPlayerUI.primaryUI, Localization.Get("uiLoadCreatingWorld", false), true);
		yield return null;
		string dataDir = GameIO.GetSaveGameLocalDir();
		string rwiFilename = Path.Combine(dataDir, "RemoteWorldInfo.xml");
		bool downloadWorld = false;
		PathAbstractions.AbstractedLocation worldLocation = PathAbstractions.WorldsSearchPaths.GetLocation(GamePrefs.GetString(EnumGamePrefs.GameWorld), null, null);
		if (worldLocation.Type == PathAbstractions.EAbstractedLocationType.None || (worldLocation.Type == PathAbstractions.EAbstractedLocationType.LocalSave && !SdFile.Exists(worldLocation.FullPath + "/completed")))
		{
			Log.Out("World not found, requesting from server");
			downloadWorld = true;
		}
		else if (worldLocation.Type != PathAbstractions.EAbstractedLocationType.None)
		{
			GameManager.<>c__DisplayClass191_0 CS$<>8__locals1 = new GameManager.<>c__DisplayClass191_0();
			CS$<>8__locals1.worldValid = true;
			yield return NetPackageWorldFolder.TestWorldValid(worldLocation.FullPath, _worldFileHashes, delegate(bool _valid)
			{
				CS$<>8__locals1.worldValid = _valid;
			});
			if (!CS$<>8__locals1.worldValid)
			{
				Log.Out("World not matching server files, request from server");
				downloadWorld = true;
			}
			CS$<>8__locals1 = null;
		}
		int value = SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo.GetValue(GameInfoInt.WorldSize);
		long num = SaveDataLimitUtils.CalculatePlayerMapSize(new Vector2i(value, value));
		long requiredSpace = 2048L + num;
		if (downloadWorld || worldLocation.Type == PathAbstractions.EAbstractedLocationType.LocalSave)
		{
			requiredSpace += _worldDataSize;
		}
		if (SaveInfoProvider.DataLimitEnabled)
		{
			long num2 = 0L;
			string @string = GamePrefs.GetString(EnumGamePrefs.GameGuidClient);
			SaveInfoProvider.SaveEntryInfo saveEntryInfo;
			if (SaveInfoProvider.Instance.TryGetRemoteSaveEntry(@string, out saveEntryInfo))
			{
				num2 = saveEntryInfo.SizeInfo.ReportedSize;
			}
			if (num2 < requiredSpace)
			{
				long pendingBytes = requiredSpace - num2;
				string protectedPath = (saveEntryInfo != null) ? saveEntryInfo.SaveDir : null;
				XUiC_SaveSpaceNeeded confirmationWindow = XUiC_SaveSpaceNeeded.Open(pendingBytes, protectedPath, null, false, true, false, "xuiDmRemoteSaveTitle", "xuiDmRemoteSaveBody", null, null, "xuiStart", null);
				while (confirmationWindow.IsOpen)
				{
					yield return null;
				}
				if (confirmationWindow.Result != XUiC_SaveSpaceNeeded.ConfirmationResult.Confirmed)
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.Disconnect();
				}
				if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsConnected)
				{
					yield break;
				}
				XUiC_ProgressWindow.Open(LocalPlayerUI.primaryUI, null, null, true, true, true, false);
				confirmationWindow = null;
			}
			else if (num2 > requiredSpace)
			{
				requiredSpace = num2;
			}
			SaveInfoProvider.Instance.ClearResources();
		}
		try
		{
			if (!SdDirectory.Exists(dataDir))
			{
				SdDirectory.CreateDirectory(dataDir);
			}
			else
			{
				SdFile.Delete(Path.Combine(dataDir, "archived.flag"));
			}
		}
		catch (Exception e)
		{
			Log.Error("Exception creating local save dir: " + dataDir + " - GUID len: " + GamePrefs.GetString(EnumGamePrefs.GameGuidClient).Length.ToString());
			Log.Exception(e);
			throw;
		}
		string path = Path.Combine(dataDir, "hosts.txt");
		string item = SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo.GetValue(GameInfoString.IP) + ":" + SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo.GetValue(GameInfoInt.Port).ToString();
		List<string> list;
		if (SdFile.Exists(path))
		{
			list = new List<string>(SdFile.ReadAllLines(path));
		}
		else
		{
			list = new List<string>();
		}
		list.Remove(item);
		list.Insert(0, item);
		SdFile.WriteAllLines(path, list.ToArray());
		VersionInformation gameVersion;
		if (VersionInformation.TryParseSerializedString(SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo.GetValue(GameInfoString.ServerVersion), out gameVersion))
		{
			RemoteWorldInfo remoteWorldInfo = new RemoteWorldInfo(_gameName, _levelName, gameVersion, requiredSpace);
			remoteWorldInfo.Write(rwiFilename);
		}
		else
		{
			Log.Error("Failed writing RemoteWorldInfo. Could not parse LastGameServerInfo information.");
		}
		if (downloadWorld)
		{
			XUiC_ProgressWindow.SetText(LocalPlayerUI.primaryUI, string.Format(Localization.Get("uiLoadDownloadingWorldWait", false), 0f, 0, 0), true);
			yield return NetPackageWorldFolder.RequestWorld();
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsConnected)
			{
				yield break;
			}
			Log.Out("World received");
		}
		yield return this.createWorld(_levelName, _gameName, _wallVolumes, _fixedSizeCC);
		XUiC_ProgressWindow.SetText(LocalPlayerUI.primaryUI, Localization.Get("uiLoadCreatingPlayer", false), true);
		yield return null;
		this.worldCreated = true;
		this.firstTimeJoin = _firstTimeJoin;
		string confirmationText = GeneratedTextManager.GetDisplayTextImmediately(SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo.ServerLoginConfirmationText, false, GeneratedTextManager.TextFilteringMode.Filter, GeneratedTextManager.BbCodeSupportMode.Supported);
		if (string.IsNullOrEmpty(confirmationText))
		{
			confirmationText = SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo.ServerLoginConfirmationText.Text;
		}
		if (!string.IsNullOrEmpty(confirmationText))
		{
			LocalPlayerUI playerUI = LocalPlayerUI.GetUIForPrimaryPlayer();
			while (!playerUI.xui.isReady)
			{
				yield return null;
			}
			yield return null;
			if (!string.IsNullOrEmpty(XUiC_ServerJoinRulesDialog.ID) && playerUI.xui.FindWindowGroupByName(XUiC_ServerJoinRulesDialog.ID) != null)
			{
				XUiC_ProgressWindow.Close(LocalPlayerUI.primaryUI);
				this.windowManager.CloseIfOpen("crossplayWarning");
				XUiC_ServerJoinRulesDialog.Show(playerUI, confirmationText);
			}
			else
			{
				this.DoSpawn();
			}
			playerUI = null;
		}
		else
		{
			this.DoSpawn();
		}
		confirmationText = null;
		DynamicMeshManager.Init();
		yield break;
	}

	// Token: 0x06007CDF RID: 31967 RVA: 0x00328C9F File Offset: 0x00326E9F
	public void DoSpawn()
	{
		if (GamePrefs.GetBool(EnumGamePrefs.SkipSpawnButton))
		{
			this.RequestToSpawn(-1);
			return;
		}
		XUiC_SpawnSelectionWindow.Open(LocalPlayerUI.primaryUI, false, true, this.firstTimeJoin);
	}

	// Token: 0x06007CE0 RID: 31968 RVA: 0x00328CC7 File Offset: 0x00326EC7
	public void RequestToSpawn(int _nearEntityId = -1)
	{
		XUiC_ProgressWindow.Open(LocalPlayerUI.primaryUI, null, null, true, true, true, false);
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageRequestToSpawnPlayer>().Setup(Utils.FastMin(12, GamePrefs.GetInt(EnumGamePrefs.OptionsGfxViewDistance)), PlayerProfile.LoadLocalProfile(), _nearEntityId), false);
	}

	// Token: 0x06007CE1 RID: 31969 RVA: 0x00328D01 File Offset: 0x00326F01
	public void ChunkClusterInfo(string _name, int _id, bool _bInifiniteTerrain, Vector2i _cMin, Vector2i _cMax, Vector3 _pos)
	{
		base.StartCoroutine(this.chunkClusterInfoCo(_name, _id, _bInifiniteTerrain, _cMin, _cMax, _pos));
	}

	// Token: 0x06007CE2 RID: 31970 RVA: 0x00328D19 File Offset: 0x00326F19
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator chunkClusterInfoCo(string _name, int _id, bool _bInifiniteTerrain, Vector2i _cMin, Vector2i _cMax, Vector3 _pos)
	{
		while (!this.worldCreated && SingletonMonoBehaviour<ConnectionManager>.Instance.IsConnected)
		{
			yield return null;
		}
		if (!this.worldCreated)
		{
			yield break;
		}
		if (this.m_World == null)
		{
			yield break;
		}
		ChunkCluster chunkCluster = null;
		if (_id == 0)
		{
			chunkCluster = this.m_World.ChunkClusters[0];
		}
		chunkCluster.Position = _pos;
		chunkCluster.ChunkMinPos = _cMin;
		chunkCluster.ChunkMaxPos = _cMax;
		if (!_bInifiniteTerrain && this.m_World.m_WorldEnvironment != null)
		{
			this.m_World.m_WorldEnvironment.SetColliders((float)((_cMin.x + 1) * 16), (float)((_cMin.y + 1) * 16), (float)((_cMax.x - _cMin.x - 1) * 16), (float)((_cMax.y - _cMin.y - 1) * 16), Constants.cSizePlanesAround, 0f);
			this.m_World.m_WorldEnvironment.CreateLevelBorderBox(this.m_World);
			this.m_World.ChunkCache.IsFixedSize = true;
		}
		this.chunkClusterLoaded = true;
		yield break;
	}

	// Token: 0x06007CE3 RID: 31971 RVA: 0x00328D50 File Offset: 0x00326F50
	public void RequestToSpawnPlayer(ClientInfo _cInfo, int _chunkViewDim, PlayerProfile _playerProfile, int _nearEntityId)
	{
		int num = GamePrefs.GetInt(EnumGamePrefs.ServerMaxAllowedViewDistance);
		if (num < 4)
		{
			num = 4;
		}
		else if (num > 12)
		{
			num = 12;
		}
		_chunkViewDim = Mathf.Clamp(_chunkViewDim, 4, num);
		PlatformUserIdentifierAbs persistentPlayerId = this.getPersistentPlayerID(_cInfo);
		PlayerDataFile playerDataFile = new PlayerDataFile();
		playerDataFile.Load(GameIO.GetPlayerDataDir(), persistentPlayerId.CombinedString);
		playerDataFile.lastSpawnPosition = SpawnPosition.Undef;
		int num2 = 0;
		int num3;
		if (!playerDataFile.bLoaded || playerDataFile.id == -1)
		{
			EntityFactory.nextEntityID = (num3 = EntityFactory.nextEntityID) + 1;
		}
		else
		{
			num3 = playerDataFile.id;
		}
		int num4 = num3;
		if (this.m_World.GetEntity(num4) != null)
		{
			num4 = EntityFactory.nextEntityID++;
			playerDataFile.id = num4;
		}
		Log.Out(string.Format("RequestToSpawnPlayer: {0}, {1}, {2}", num4, _cInfo.playerName, _chunkViewDim));
		if (GameStats.GetBool(EnumGameStats.IsSpawnNearOtherPlayer))
		{
			for (int i = 0; i < this.m_World.Players.list.Count; i++)
			{
				int x;
				int y;
				int z;
				if (this.m_World.Players.list[i].TeamNumber == num2 && this.m_World.FindRandomSpawnPointNearPlayer(this.m_World.Players.list[i], 15, out x, out y, out z, 15))
				{
					playerDataFile.lastSpawnPosition = new SpawnPosition(new Vector3i(x, y, z), 0f);
					break;
				}
			}
		}
		if (_nearEntityId != -1)
		{
			AllowSpawnNearFriend spawnNearFriendMode = XUiC_SpawnNearFriendsList.SpawnNearFriendMode;
			Entity entity = this.m_World.GetEntity(_nearEntityId);
			if (entity && spawnNearFriendMode != AllowSpawnNearFriend.Disabled)
			{
				int num5 = 15;
				Vector3 vector;
				bool flag;
				do
				{
					num5--;
					flag = this.m_World.GetRandomSpawnPositionMinMaxToPosition(entity.position, 40, 150, 1, true, out vector, num4, true, 20, true, EnumLandClaimOwner.None, false);
					if (!flag)
					{
						break;
					}
					if (spawnNearFriendMode == AllowSpawnNearFriend.InForest)
					{
						BiomeDefinition biomeInWorld = this.m_World.GetBiomeInWorld((int)vector.x, (int)vector.z);
						BiomeDefinition.BiomeType? biomeType = (biomeInWorld != null) ? new BiomeDefinition.BiomeType?(biomeInWorld.m_BiomeType) : null;
						if (biomeType == null)
						{
							goto IL_234;
						}
						BiomeDefinition.BiomeType valueOrDefault = biomeType.GetValueOrDefault();
						if (valueOrDefault - BiomeDefinition.BiomeType.Forest > 1)
						{
							goto IL_234;
						}
						bool flag2 = true;
						IL_237:
						flag = flag2;
						goto IL_23B;
						IL_234:
						flag2 = false;
						goto IL_237;
					}
					IL_23B:;
				}
				while (num5 > 0 && !flag);
				if (flag)
				{
					playerDataFile.lastSpawnPosition = new SpawnPosition(vector, this.m_World.RandomRange(0f, 360f));
				}
				else
				{
					Log.Warning(string.Format("RequestToSpawnPlayer: Failed getting a valid spawn position near player with entity ID {0}", _nearEntityId));
				}
			}
		}
		if (playerDataFile.lastSpawnPosition.IsUndef())
		{
			playerDataFile.lastSpawnPosition = this.GetSpawnPointList().GetRandomSpawnPosition(this.m_World, null, 0, 0);
		}
		if (!playerDataFile.bLoaded)
		{
			playerDataFile.ecd.pos = playerDataFile.lastSpawnPosition.position;
		}
		EntityCreationData entityCreationData = new EntityCreationData();
		if (!playerDataFile.bLoaded || playerDataFile.ecd.playerProfile == null || !GamePrefs.GetBool(EnumGamePrefs.PersistentPlayerProfiles))
		{
			playerDataFile.ecd.playerProfile = _playerProfile;
		}
		if (playerDataFile.bLoaded)
		{
			entityCreationData.entityData = playerDataFile.ecd.entityData;
			entityCreationData.readFileVersion = playerDataFile.ecd.readFileVersion;
		}
		entityCreationData.entityClass = EntityClass.FromString(playerDataFile.ecd.playerProfile.EntityClassName);
		entityCreationData.playerProfile = playerDataFile.ecd.playerProfile;
		entityCreationData.id = num4;
		entityCreationData.teamNumber = num2;
		entityCreationData.pos = playerDataFile.ecd.pos;
		entityCreationData.rot = playerDataFile.ecd.rot;
		EntityPlayer entityPlayer = (EntityPlayer)EntityFactory.CreateEntity(entityCreationData);
		entityPlayer.isEntityRemote = true;
		entityPlayer.Respawn(playerDataFile.bLoaded ? RespawnType.JoinMultiplayer : RespawnType.EnterMultiplayer);
		playerDataFile.ToPlayer(entityPlayer);
		bool flag3 = false;
		PersistentPlayerList persistentPlayerList = this.persistentPlayers;
		PersistentPlayerData persistentPlayerData = (persistentPlayerList != null) ? persistentPlayerList.GetPlayerData(persistentPlayerId) : null;
		if (persistentPlayerData == null)
		{
			PersistentPlayerList persistentPlayerList2 = this.persistentPlayers;
			persistentPlayerData = ((persistentPlayerList2 != null) ? persistentPlayerList2.CreatePlayerData(persistentPlayerId, _cInfo.PlatformId, _cInfo.playerName, _cInfo.device.ToPlayGroup()) : null);
		}
		else
		{
			persistentPlayerData.Update(_cInfo.PlatformId, new AuthoredText(_cInfo.playerName, persistentPlayerId), _cInfo.device.ToPlayGroup());
			flag3 = true;
		}
		persistentPlayerData.LastLogin = DateTime.Now;
		persistentPlayerData.EntityId = num4;
		if (this.persistentPlayers != null)
		{
			this.persistentPlayers.MapPlayer(persistentPlayerData);
		}
		this.SavePersistentPlayerData();
		SingletonMonoBehaviour<ConnectionManager>.Instance.SetClientEntityId(_cInfo, num4, playerDataFile);
		_cInfo.SendPackage(NetPackageManager.GetPackage<NetPackagePlayerId>().Setup(num4, num2, playerDataFile, _chunkViewDim));
		GameManager.Instance.World.aiDirector.GetComponent<AIDirectorAirDropComponent>().RefreshCrates(num4);
		this.m_World.SpawnEntityInWorld(entityPlayer);
		entityPlayer.ChunkObserver = this.m_World.m_ChunkManager.AddChunkObserver(entityPlayer.GetPosition(), false, _chunkViewDim, entityPlayer.entityId);
		IMapChunkDatabase.TryCreateOrLoad(entityPlayer.entityId, out entityPlayer.ChunkObserver.mapDatabase, () => new IMapChunkDatabase.DirectoryPlayerId(GameIO.GetPlayerDataDir(), persistentPlayerId.CombinedString));
		if (this.persistentPlayers != null)
		{
			this.MarkPlayerEntityFriends();
			this.persistentPlayers.DispatchPlayerEvent(persistentPlayerData, null, EnumPersistentPlayerDataReason.Login);
		}
		if (flag3)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackagePersistentPlayerState>().Setup(persistentPlayerData, EnumPersistentPlayerDataReason.Login), false, -1, -1, -1, null, 192, false);
		}
		else
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackagePersistentPlayerState>().Setup(persistentPlayerData, EnumPersistentPlayerDataReason.New), false, -1, -1, -1, null, 192, false);
		}
		ModEvents.SPlayerSpawningData splayerSpawningData = new ModEvents.SPlayerSpawningData(_cInfo, _chunkViewDim, _playerProfile);
		ModEvents.PlayerSpawning.Invoke(ref splayerSpawningData);
	}

	// Token: 0x06007CE4 RID: 31972 RVA: 0x003292F8 File Offset: 0x003274F8
	[PublicizedFrom(EAccessModifier.Private)]
	public void MarkPlayerEntityFriends()
	{
		if (this.myEntityPlayerLocal == null || this.persistentLocalPlayer == null)
		{
			return;
		}
		for (int i = 0; i < this.m_World.Players.list.Count; i++)
		{
			EntityPlayer entityPlayer = this.m_World.Players.list[i];
			if (entityPlayer.entityId != this.myPlayerId)
			{
				PersistentPlayerList persistentPlayerList = this.persistentPlayers;
				PersistentPlayerData persistentPlayerData = (persistentPlayerList != null) ? persistentPlayerList.GetPlayerDataFromEntityID(entityPlayer.entityId) : null;
				entityPlayer.IsFriendOfLocalPlayer = (persistentPlayerData != null && this.persistentLocalPlayer.ACL != null && this.persistentLocalPlayer.ACL.Contains(persistentPlayerData.PrimaryId));
			}
		}
	}

	// Token: 0x06007CE5 RID: 31973 RVA: 0x003293AC File Offset: 0x003275AC
	public void PersistentPlayerEvent(PlatformUserIdentifierAbs playerID, PlatformUserIdentifierAbs otherPlayerID, EnumPersistentPlayerDataReason reason)
	{
		PersistentPlayerData persistentPlayerData = (this.persistentPlayers != null) ? this.persistentPlayers.GetPlayerData(playerID) : null;
		if (persistentPlayerData == null)
		{
			return;
		}
		PersistentPlayerData persistentPlayerData2 = (otherPlayerID != null) ? this.persistentPlayers.GetPlayerData(otherPlayerID) : null;
		if (persistentPlayerData2 == null && reason != EnumPersistentPlayerDataReason.Login)
		{
			return;
		}
		bool flag = false;
		switch (reason)
		{
		case EnumPersistentPlayerDataReason.ACL_AcceptedInvite:
			persistentPlayerData.AddPlayerToACL(persistentPlayerData2.PrimaryId);
			persistentPlayerData2.AddPlayerToACL(persistentPlayerData.PrimaryId);
			this.MarkPlayerEntityFriends();
			persistentPlayerData2.Dispatch(persistentPlayerData, reason);
			flag = true;
			break;
		case EnumPersistentPlayerDataReason.ACL_DeclinedInvite:
			if (persistentPlayerData2 == this.persistentLocalPlayer)
			{
				persistentPlayerData2.Dispatch(persistentPlayerData, reason);
			}
			else
			{
				flag = true;
			}
			break;
		case EnumPersistentPlayerDataReason.ACL_Invite:
			if (persistentPlayerData2 == this.persistentLocalPlayer)
			{
				EntityPlayerLocal localPlayerFromID = this.m_World.GetLocalPlayerFromID(persistentPlayerData2.EntityId);
				LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(localPlayerFromID);
				if (uiforPlayer != null)
				{
					NGUIWindowManager nguiwindowManager = uiforPlayer.nguiWindowManager;
					if (uiforPlayer.xui.GetChildByType<XUiC_PlayersList>().AddInvite(playerID))
					{
						EntityPlayer entityPlayer = this.m_World.GetEntity(persistentPlayerData.EntityId) as EntityPlayer;
						if (entityPlayer != null)
						{
							GameManager.ShowTooltip(localPlayerFromID, "friendInviteReceived", entityPlayer.PlayerDisplayName, null, null, false, false, 0f);
						}
						persistentPlayerData2.Dispatch(persistentPlayerData, reason);
					}
				}
			}
			else
			{
				flag = true;
			}
			break;
		case EnumPersistentPlayerDataReason.ACL_Removed:
			persistentPlayerData.RemovePlayerFromACL(persistentPlayerData2.PrimaryId);
			persistentPlayerData2.RemovePlayerFromACL(persistentPlayerData.PrimaryId);
			this.MarkPlayerEntityFriends();
			persistentPlayerData.Dispatch(persistentPlayerData2, reason);
			persistentPlayerData2.Dispatch(persistentPlayerData, reason);
			flag = true;
			break;
		}
		this.persistentPlayers.DispatchPlayerEvent(persistentPlayerData, persistentPlayerData2, reason);
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && flag)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackagePlayerAcl>().Setup(persistentPlayerData.PrimaryId, otherPlayerID, reason), true, -1, -1, -1, null, 192, false);
		}
	}

	// Token: 0x06007CE6 RID: 31974 RVA: 0x00329570 File Offset: 0x00327770
	public void PersistentPlayerLogin(PersistentPlayerData ppData)
	{
		if (this.persistentPlayers == null)
		{
			return;
		}
		this.persistentPlayers.SetPlayerData(ppData);
		if (this.myPlayerId != -1 && ppData.EntityId == this.myPlayerId)
		{
			this.persistentLocalPlayer = ppData;
			if (this.myEntityPlayerLocal != null)
			{
				this.myEntityPlayerLocal.persistentPlayerData = this.persistentLocalPlayer;
			}
		}
		this.MarkPlayerEntityFriends();
		this.persistentPlayers.DispatchPlayerEvent(ppData, null, EnumPersistentPlayerDataReason.Login);
	}

	// Token: 0x06007CE7 RID: 31975 RVA: 0x003295E4 File Offset: 0x003277E4
	public void HandlePersistentPlayerDisconnected(int _entityId)
	{
		PersistentPlayerData playerDataFromEntityID = this.persistentPlayers.GetPlayerDataFromEntityID(_entityId);
		if (playerDataFromEntityID != null)
		{
			this.persistentPlayers.DispatchPlayerEvent(playerDataFromEntityID, null, EnumPersistentPlayerDataReason.Disconnected);
			this.persistentPlayers.UnmapPlayer(playerDataFromEntityID.PrimaryId);
		}
	}

	// Token: 0x06007CE8 RID: 31976 RVA: 0x00329620 File Offset: 0x00327820
	public void SendPlayerACLInvite(PersistentPlayerData targetPlayer)
	{
		if (targetPlayer.EntityId == -1)
		{
			this.persistentLocalPlayer.Dispatch(targetPlayer, EnumPersistentPlayerDataReason.ACL_DeclinedInvite);
			return;
		}
		NetPackage package = NetPackageManager.GetPackage<NetPackagePlayerAcl>().Setup(this.persistentLocalPlayer.PrimaryId, targetPlayer.PrimaryId, EnumPersistentPlayerDataReason.ACL_Invite);
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, false, targetPlayer.EntityId, -1, -1, null, 192, false);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(package, false);
	}

	// Token: 0x06007CE9 RID: 31977 RVA: 0x003296A0 File Offset: 0x003278A0
	public void ReplyToPlayerACLInvite(PlatformUserIdentifierAbs requestingPlayerId, bool accepted)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.PersistentPlayerEvent(this.persistentLocalPlayer.PrimaryId, requestingPlayerId, accepted ? EnumPersistentPlayerDataReason.ACL_AcceptedInvite : EnumPersistentPlayerDataReason.ACL_DeclinedInvite);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePlayerAcl>().Setup(this.persistentLocalPlayer.PrimaryId, requestingPlayerId, accepted ? EnumPersistentPlayerDataReason.ACL_AcceptedInvite : EnumPersistentPlayerDataReason.ACL_DeclinedInvite), false);
	}

	// Token: 0x06007CEA RID: 31978 RVA: 0x003296FC File Offset: 0x003278FC
	public void RemovePlayerFromACL(PersistentPlayerData targetPlayer)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.PersistentPlayerEvent(this.persistentLocalPlayer.PrimaryId, targetPlayer.PrimaryId, EnumPersistentPlayerDataReason.ACL_Removed);
			return;
		}
		NetPackage package = NetPackageManager.GetPackage<NetPackagePlayerAcl>().Setup(this.persistentLocalPlayer.PrimaryId, targetPlayer.PrimaryId, EnumPersistentPlayerDataReason.ACL_Removed);
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(package, false);
	}

	// Token: 0x06007CEB RID: 31979 RVA: 0x00329758 File Offset: 0x00327958
	public void PlayerId(int _playerId, int _teamNumber, PlayerDataFile _playerDataFile, int _chunkViewDim)
	{
		Log.Out(string.Format("PlayerId({0}, {1})", _playerId, _teamNumber));
		Log.Out("Allowed ChunkViewDistance: " + _chunkViewDim.ToString());
		GameStats.Set(EnumGameStats.AllowedViewDistance, _chunkViewDim);
		this.myPlayerId = _playerId;
		EntityCreationData entityCreationData = new EntityCreationData();
		entityCreationData.id = _playerId;
		entityCreationData.teamNumber = _teamNumber;
		if (_playerDataFile.bLoaded)
		{
			entityCreationData.entityClass = EntityClass.FromString(_playerDataFile.ecd.playerProfile.EntityClassName);
			entityCreationData.playerProfile = _playerDataFile.ecd.playerProfile;
		}
		else
		{
			entityCreationData.playerProfile = PlayerProfile.LoadLocalProfile();
			entityCreationData.entityClass = EntityClass.FromString(entityCreationData.playerProfile.EntityClassName);
		}
		entityCreationData.skinTexture = GamePrefs.GetString(EnumGamePrefs.OptionsPlayerModelTexture);
		entityCreationData.id = _playerId;
		entityCreationData.pos = _playerDataFile.ecd.pos;
		entityCreationData.rot = _playerDataFile.ecd.rot;
		entityCreationData.belongsPlayerId = _playerId;
		EntityPlayerLocal entityPlayerLocal = EntityFactory.CreateEntity(entityCreationData) as EntityPlayerLocal;
		this.setLocalPlayerEntity(entityPlayerLocal);
		Log.Out(string.Format("Found own player entity with id {0}", entityPlayerLocal.entityId));
		entityPlayerLocal.lastSpawnPosition = _playerDataFile.lastSpawnPosition;
		if (_playerDataFile.bLoaded)
		{
			_playerDataFile.ToPlayer(entityPlayerLocal);
			this.clientRespawnType = RespawnType.JoinMultiplayer;
		}
		else
		{
			this.clientRespawnType = RespawnType.EnterMultiplayer;
		}
		this.m_World.SpawnEntityInWorld(entityPlayerLocal);
		entityPlayerLocal.ChunkObserver = this.m_World.m_ChunkManager.AddChunkObserver(entityPlayerLocal.GetPosition(), true, GameUtils.GetViewDistance(), -1);
		IMapChunkDatabase.TryCreateOrLoad(entityPlayerLocal.entityId, out entityPlayerLocal.ChunkObserver.mapDatabase, delegate
		{
			string combinedString = this.getPersistentPlayerID(null).CombinedString;
			return new IMapChunkDatabase.DirectoryPlayerId(GameIO.GetPlayerDataLocalDir(), combinedString);
		});
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(entityPlayerLocal);
		uiforPlayer.xui.SetDataConnections();
		uiforPlayer.xui.SetCraftingData(_playerDataFile.craftingData);
		this.SetWorldTime(this.m_World.worldTime);
		PlayerInteractions.Instance.JoinedMultiplayerServer(this.persistentPlayers);
		entityPlayerLocal.Respawn(this.clientRespawnType);
		this.gameStateManager.InitGame(false);
		this.gameStateManager.StartGame();
	}

	// Token: 0x06007CEC RID: 31980 RVA: 0x00329960 File Offset: 0x00327B60
	public void PlayerSpawnedInWorld(ClientInfo _cInfo, RespawnType _respawnReason, Vector3i _pos, int _entityId)
	{
		if (_entityId == -1)
		{
			return;
		}
		Entity entity;
		if (!this.m_World.Entities.dict.TryGetValue(_entityId, out entity))
		{
			return;
		}
		EntityPlayer entityPlayer = entity as EntityPlayer;
		if (entityPlayer == null)
		{
			return;
		}
		if (_respawnReason == RespawnType.Died && entityPlayer.isEntityRemote)
		{
			entityPlayer.SetAlive();
		}
		if (_respawnReason == RespawnType.EnterMultiplayer || _respawnReason == RespawnType.JoinMultiplayer)
		{
			this.DisplayGameMessage(EnumGameMessages.JoinedGame, _entityId, -1, true);
		}
		PlayerInteractions.Instance.PlayerSpawnedInMultiplayerServer(this.persistentPlayers, _entityId, _respawnReason);
		bool flag = _respawnReason == RespawnType.NewGame || _respawnReason == RespawnType.EnterMultiplayer || _respawnReason == RespawnType.JoinMultiplayer || _respawnReason == RespawnType.LoadedGame;
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && flag)
		{
			VehicleManager.Instance.UpdateVehicleWaypointsForPlayer(_entityId);
			DroneManager.Instance.UpdateWaypointsForPlayer(_entityId);
		}
		ModEvents.SPlayerSpawnedInWorldData splayerSpawnedInWorldData = new ModEvents.SPlayerSpawnedInWorldData(_cInfo, entityPlayer is EntityPlayerLocal, _entityId, _respawnReason, _pos);
		ModEvents.PlayerSpawnedInWorld.Invoke(ref splayerSpawnedInWorldData);
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			Action<ClientInfo> onClientSpawned = this.OnClientSpawned;
			if (onClientSpawned != null)
			{
				onClientSpawned(_cInfo);
			}
			Log.Out("PlayerSpawnedInWorld (reason: {0}, position: {2}): {1}", new object[]
			{
				_respawnReason.ToStringCached<RespawnType>(),
				(_cInfo != null) ? _cInfo.ToString() : "localplayer",
				_pos.ToString()
			});
		}
	}

	// Token: 0x06007CED RID: 31981 RVA: 0x00329A90 File Offset: 0x00327C90
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleFirstSpawnInteractions(PlayerInteraction _interaction)
	{
		if (_interaction.Type != PlayerInteractionType.FirstSpawn)
		{
			return;
		}
		int num = this.persistentPlayers.PlayerToEntityMap[_interaction.PlayerData.PrimaryId];
		if (!(this.myEntityPlayerLocal == null))
		{
			int num2 = num;
			EntityPlayerLocal entityPlayerLocal = this.myEntityPlayerLocal;
			int? num3 = (entityPlayerLocal != null) ? new int?(entityPlayerLocal.entityId) : null;
			if (!(num2 == num3.GetValueOrDefault() & num3 != null))
			{
				IPlatformUserData orCreate = PlatformUserManager.GetOrCreate(_interaction.PlayerData.PrimaryId);
				if (num != -1 && orCreate != null && orCreate.Blocked[EBlockType.Play].IsBlocked())
				{
					this.DisplayGameMessage(EnumGameMessages.BlockedPlayerAlert, num, -1, true);
					return;
				}
				if (GamePrefs.GetBool(EnumGamePrefs.OptionsAutoPartyWithFriends))
				{
					PersistentPlayerData persistentPlayerData = this.myEntityPlayerLocal.persistentPlayerData;
					if (((persistentPlayerData != null) ? persistentPlayerData.ACL : null) != null && this.myEntityPlayerLocal.persistentPlayerData.ACL.Contains(_interaction.PlayerData.PrimaryId))
					{
						if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
						{
							SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePartyActions>().Setup(NetPackagePartyActions.PartyActions.SendInvite, this.myEntityPlayerLocal.entityId, num, null, null), false);
							return;
						}
						SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackagePartyActions>().Setup(NetPackagePartyActions.PartyActions.SendInvite, this.myEntityPlayerLocal.entityId, num, null, null), false, -1, -1, -1, null, 192, false);
					}
				}
				return;
			}
		}
	}

	// Token: 0x06007CEE RID: 31982 RVA: 0x00329BF4 File Offset: 0x00327DF4
	public void PlayerDisconnected(ClientInfo _cInfo)
	{
		if (_cInfo.entityId != -1)
		{
			EntityPlayer entityPlayer = (EntityPlayer)this.m_World.GetEntity(_cInfo.entityId);
			Log.Out("Player {0} disconnected after {1} minutes", new object[]
			{
				GameUtils.SafeStringFormat(entityPlayer.EntityName),
				((Time.timeSinceLevelLoad - entityPlayer.CreationTimeSinceLevelLoad) / 60f).ToCultureInvariantString("0.0")
			});
		}
		if (GameManager.IsDedicatedServer)
		{
			GC.Collect();
			MemoryPools.Cleanup();
		}
		PersistentPlayerData persistentPlayerData = this.getPersistentPlayerData(_cInfo);
		if (persistentPlayerData != null)
		{
			persistentPlayerData.LastLogin = DateTime.Now;
			persistentPlayerData.EntityId = -1;
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackagePersistentPlayerState>().Setup(persistentPlayerData, EnumPersistentPlayerDataReason.Disconnected), false, -1, -1, -1, null, 192, true);
		}
		this.SavePersistentPlayerData();
		SingletonMonoBehaviour<ConnectionManager>.Instance.DisconnectClient(_cInfo, false, true);
	}

	// Token: 0x06007CEF RID: 31983 RVA: 0x00329CC8 File Offset: 0x00327EC8
	public void SavePlayerData(ClientInfo _cInfo, PlayerDataFile _playerDataFile)
	{
		_cInfo.latestPlayerData = _playerDataFile;
		int entityId = _cInfo.entityId;
		if (entityId != -1)
		{
			EntityPlayer entityPlayer = (EntityPlayer)this.m_World.GetEntity(entityId);
			if (entityPlayer != null)
			{
				_playerDataFile.Save(GameIO.GetPlayerDataDir(), _cInfo.InternalId.CombinedString);
				if (entityPlayer.ChunkObserver.mapDatabase != null)
				{
					ThreadManager.AddSingleTask(new ThreadManager.TaskFunctionDelegate(entityPlayer.ChunkObserver.mapDatabase.SaveAsync), new IMapChunkDatabase.DirectoryPlayerId(GameIO.GetPlayerDataDir(), _cInfo.InternalId.CombinedString), null, true);
				}
				entityPlayer.QuestJournal = _playerDataFile.questJournal;
				if (this.persistentPlayers != null)
				{
					foreach (KeyValuePair<PlatformUserIdentifierAbs, PersistentPlayerData> keyValuePair in this.persistentPlayers.Players)
					{
						if (keyValuePair.Value.EntityId == _playerDataFile.id)
						{
							keyValuePair.Value.Position = new Vector3i(_playerDataFile.ecd.pos);
							break;
						}
					}
				}
			}
		}
		ModEvents.SSavePlayerDataData ssavePlayerDataData = new ModEvents.SSavePlayerDataData(_cInfo, _playerDataFile);
		ModEvents.SavePlayerData.Invoke(ref ssavePlayerDataData);
	}

	// Token: 0x06007CF0 RID: 31984 RVA: 0x00329DFC File Offset: 0x00327FFC
	[PublicizedFrom(EAccessModifier.Private)]
	public PlatformUserIdentifierAbs getPersistentPlayerID(ClientInfo _cInfo)
	{
		return ((_cInfo != null) ? _cInfo.InternalId : null) ?? PlatformManager.InternalLocalUserIdentifier;
	}

	// Token: 0x06007CF1 RID: 31985 RVA: 0x00329E13 File Offset: 0x00328013
	[PublicizedFrom(EAccessModifier.Private)]
	public PersistentPlayerData getPersistentPlayerData(ClientInfo _cInfo)
	{
		PersistentPlayerList persistentPlayerList = this.persistentPlayers;
		if (persistentPlayerList == null)
		{
			return null;
		}
		return persistentPlayerList.GetPlayerData(this.getPersistentPlayerID(_cInfo));
	}

	// Token: 0x06007CF2 RID: 31986 RVA: 0x00329E2D File Offset: 0x0032802D
	public PersistentPlayerList GetPersistentPlayerList()
	{
		return this.persistentPlayers;
	}

	// Token: 0x06007CF3 RID: 31987 RVA: 0x00329E35 File Offset: 0x00328035
	public PersistentPlayerData GetPersistentLocalPlayer()
	{
		return this.persistentLocalPlayer;
	}

	// Token: 0x06007CF4 RID: 31988 RVA: 0x00329E3D File Offset: 0x0032803D
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator createWorld(string _sWorldName, string _sGameName, List<WallVolume> _wallVolumes, bool _fixedSizeCC = false)
	{
		Log.Out(string.Format("createWorld: {0}, {1}, {2}", _sWorldName, _sGameName, GamePrefs.GetString(EnumGamePrefs.GameMode)));
		GamePrefs.Set(EnumGamePrefs.GameNameClient, _sGameName);
		bool flag = GameModeEditWorld.TypeName.Equals(GamePrefs.GetString(EnumGamePrefs.GameMode));
		PathAbstractions.CacheEnabled = !flag;
		if (flag)
		{
			Constants.cDigAndBuildDistance = 50f;
			Constants.cBuildIntervall = 0.2f;
			Constants.cCollectItemDistance = 50f;
		}
		else if (GameModeCreative.TypeName.Equals(GamePrefs.GetString(EnumGamePrefs.GameMode)))
		{
			Constants.cDigAndBuildDistance = 25f;
			Constants.cBuildIntervall = 0.2f;
			Constants.cCollectItemDistance = 25f;
		}
		else
		{
			Constants.cDigAndBuildDistance = 5f;
			Constants.cBuildIntervall = 0.5f;
			Constants.cCollectItemDistance = 3.5f;
		}
		OcclusionManager.Instance.WorldChanging(flag);
		yield return null;
		this.m_World = new World();
		if (GameManager.IsDedicatedServer || this.IsEditMode())
		{
			this.GameHasStarted = true;
		}
		else
		{
			base.StartCoroutine(this.waitForGameStart());
		}
		this.m_World.Init(this, WorldBiomes.Instance);
		if (_wallVolumes != null)
		{
			this.m_World.SetWallVolumesForClient(_wallVolumes);
		}
		yield return null;
		if (this.biomeParticleManager == null)
		{
			this.biomeParticleManager = new BiomeParticleManager();
		}
		if (this.OnWorldChanged != null)
		{
			this.OnWorldChanged(this.m_World);
		}
		PlayerInteractions.Instance.OnNewPlayerInteraction += this.HandleFirstSpawnInteractions;
		yield return null;
		yield return this.m_World.LoadWorld(_sWorldName, _fixedSizeCC);
		yield return null;
		AstarManager.Init(base.gameObject);
		yield return null;
		this.lootManager = new LootManager(this.m_World);
		yield return null;
		this.traderManager = new TraderManager(this.m_World);
		yield return null;
		ResourceRequest weatherLoading = Resources.LoadAsync("Prefabs/WeatherManager");
		while (!weatherLoading.isDone)
		{
			yield return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(weatherLoading.asset as GameObject) as GameObject;
		gameObject.transform.SetParent(base.transform, false);
		WeatherManager.Init(this.m_World, gameObject);
		yield return null;
		yield return EnvironmentAudioManager.CreateNewInstance();
		yield return null;
		new WaterSplashCubes();
		yield return null;
		WireManager.Instance.Init();
		yield return null;
		LoadManager.AssetRequestTask<GameObject> requestTask = LoadManager.LoadAsset<GameObject>("@:Prefabs/SkySystem/SkySystem.prefab", null, null, false, false);
		yield return new WaitUntil(() => requestTask.IsDone);
		SkyManager.Loaded(UnityEngine.Object.Instantiate<GameObject>(requestTask.Asset));
		yield return null;
		if (WeatherManager.Instance)
		{
			WeatherManager.Instance.CloudsFrameUpdateNow();
			WeatherManager.Instance.InitParticles();
		}
		yield return null;
		if (this.IsEditMode())
		{
			DynamicPrefabDecorator dynamicPrefabDecorator = this.GetDynamicPrefabDecorator();
			if (dynamicPrefabDecorator != null && this.IsEditMode())
			{
				dynamicPrefabDecorator.CreateBoundingBoxes();
			}
			SpawnPointList spawnPointList = this.GetSpawnPointList();
			for (int i = 0; i < spawnPointList.Count; i++)
			{
				SpawnPoint spawnPoint = spawnPointList[i];
				SelectionBoxManager.Instance.GetCategory("StartPoint").AddBox(spawnPoint.spawnPosition.ToBlockPos().ToString(), spawnPoint.spawnPosition.ToBlockPos(), Vector3i.one, true, false).facingDirection = spawnPoint.spawnPosition.heading;
			}
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				PrefabSleeperVolumeManager.Instance.StartAsServer();
			}
			else
			{
				PrefabSleeperVolumeManager.Instance.StartAsClient();
			}
		}
		ModEvents.SCreateWorldDoneData screateWorldDoneData = default(ModEvents.SCreateWorldDoneData);
		ModEvents.CreateWorldDone.Invoke(ref screateWorldDoneData);
		Log.Out("createWorld() done");
		yield break;
	}

	// Token: 0x17000D12 RID: 3346
	// (get) Token: 0x06007CF5 RID: 31989 RVA: 0x00329E69 File Offset: 0x00328069
	public World World
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.m_World;
		}
	}

	// Token: 0x06007CF6 RID: 31990 RVA: 0x00329E71 File Offset: 0x00328071
	public SpawnPointList GetSpawnPointList()
	{
		return this.m_World.ChunkCache.ChunkProvider.GetSpawnPointList();
	}

	// Token: 0x06007CF7 RID: 31991 RVA: 0x00329E88 File Offset: 0x00328088
	public ChunkManager.ChunkObserver AddChunkObserver(Vector3 _initialPosition, bool _bBuildVisualMeshAround, int _viewDim, int _entityIdToSendChunksTo)
	{
		return this.m_World.m_ChunkManager.AddChunkObserver(_initialPosition, _bBuildVisualMeshAround, _viewDim, _entityIdToSendChunksTo);
	}

	// Token: 0x06007CF8 RID: 31992 RVA: 0x00329E9F File Offset: 0x0032809F
	public void RemoveChunkObserver(ChunkManager.ChunkObserver _observer)
	{
		this.m_World.m_ChunkManager.RemoveChunkObserver(_observer);
	}

	// Token: 0x06007CF9 RID: 31993 RVA: 0x00329EB4 File Offset: 0x003280B4
	public void ExplosionServer(int _clrIdx, Vector3 _worldPos, Vector3i _blockPos, Quaternion _rotation, ExplosionData _explosionData, int _entityId, float _delay, bool _bRemoveBlockAtExplPosition, ItemValue _itemValueExplosionSource = null)
	{
		if (_bRemoveBlockAtExplPosition)
		{
			this.m_World.SetBlockRPC(_clrIdx, _blockPos, BlockValue.Air);
		}
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageExplosionInitiate>().Setup(_clrIdx, _worldPos, _blockPos, _rotation, _explosionData, _entityId, _delay, _bRemoveBlockAtExplPosition, _itemValueExplosionSource), false);
			return;
		}
		if (_delay <= 0f)
		{
			this.explode(_clrIdx, _worldPos, _blockPos, _rotation, _explosionData, _entityId, _itemValueExplosionSource);
			return;
		}
		base.StartCoroutine(this.explodeLater(_clrIdx, _worldPos, _blockPos, _rotation, _explosionData, _entityId, _itemValueExplosionSource, _delay));
	}

	// Token: 0x06007CFA RID: 31994 RVA: 0x00329F40 File Offset: 0x00328140
	[PublicizedFrom(EAccessModifier.Protected)]
	public IEnumerator explodeLater(int _clrIdx, Vector3 _position, Vector3i _blockPos, Quaternion _rotation, ExplosionData _explosionData, int _entityId, ItemValue _itemValueExplosionSource, float _delayInSec)
	{
		yield return new WaitForSeconds(_delayInSec);
		this.explode(_clrIdx, _position, _blockPos, _rotation, _explosionData, _entityId, _itemValueExplosionSource);
		yield break;
	}

	// Token: 0x06007CFB RID: 31995 RVA: 0x00329F98 File Offset: 0x00328198
	[PublicizedFrom(EAccessModifier.Private)]
	public void explode(int _clrIdx, Vector3 _worldPos, Vector3i _blockPos, Quaternion _rotation, ExplosionData _explosionData, int _entityId, ItemValue _itemValueExplosionSource)
	{
		Explosion explosion = new Explosion(this.m_World, _clrIdx, _worldPos, _blockPos, _explosionData, _entityId);
		explosion.AttackBlocks(_entityId, _itemValueExplosionSource);
		explosion.AttackEntites(_entityId, _itemValueExplosionSource, _explosionData.DamageType);
		this.tempExplPositions.Clear();
		explosion.ChangedBlockPositions.CopyValuesTo(this.tempExplPositions);
		GameManager.ExplodeGroup explodeGroup = new GameManager.ExplodeGroup();
		explodeGroup.pos = _worldPos;
		explodeGroup.radius = _explosionData.BlockRadius;
		explodeGroup.delay = 3;
		foreach (BlockChangeInfo blockChangeInfo in this.tempExplPositions)
		{
			if (blockChangeInfo.blockValue.isair)
			{
				BlockValue block = this.m_World.GetBlock(blockChangeInfo.pos);
				if (!block.isair && block.Block.IsExplosionAffected())
				{
					GameManager.ExplodeGroup.Falling item;
					item.pos = blockChangeInfo.pos;
					item.bv = block;
					explodeGroup.fallings.Add(item);
				}
			}
		}
		if (explodeGroup.fallings.Count > 0)
		{
			this.explodeFallingGroups.Add(explodeGroup);
		}
		GameObject gameObject = this.ExplosionClient(_clrIdx, _worldPos, _rotation, _explosionData.ParticleIndex, _explosionData.BlastPower, (float)_explosionData.EntityRadius, _explosionData.BlockDamage, _entityId, this.tempExplPositions);
		if (gameObject != null)
		{
			if (_explosionData.Duration > 0f)
			{
				TemporaryObject component = gameObject.GetComponent<TemporaryObject>();
				if (component != null)
				{
					component.SetLife(_explosionData.Duration);
				}
			}
			ExplosionDamageArea explosionDamageArea;
			if (gameObject.TryGetComponent<ExplosionDamageArea>(out explosionDamageArea))
			{
				explosionDamageArea.BuffActions = _explosionData.BuffActions;
				explosionDamageArea.InitiatorEntityId = _entityId;
			}
			if (this.m_World.aiDirector != null && !_explosionData.IgnoreHeatMap)
			{
				AudioPlayer component2 = gameObject.GetComponent<AudioPlayer>();
				if (component2)
				{
					this.m_World.aiDirector.OnSoundPlayedAtPosition(_entityId, _worldPos, component2.soundName, 1f);
				}
			}
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.ClientCount() > 0)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageExplosionClient>().Setup(_clrIdx, _worldPos, _rotation, _explosionData.ParticleIndex, _explosionData.BlastPower, _explosionData.BlockDamage, (float)_explosionData.EntityRadius, _entityId, this.tempExplPositions), true, -1, -1, -1, null, 192, false);
		}
		this.tempExplPositions.Clear();
	}

	// Token: 0x06007CFC RID: 31996 RVA: 0x0032A1FC File Offset: 0x003283FC
	public GameObject ExplosionClient(int _clrIdx, Vector3 _center, Quaternion _rotation, int _index, int _blastPower, float _blastRadius, float _blockDamage, int _entityId, List<BlockChangeInfo> _explosionChanges)
	{
		if (this.m_World == null)
		{
			return null;
		}
		GameObject result = null;
		if (_index > 0 && _index < WorldStaticData.prefabExplosions.Length && WorldStaticData.prefabExplosions[_index] != null)
		{
			result = UnityEngine.Object.Instantiate<GameObject>(WorldStaticData.prefabExplosions[_index].gameObject, _center - Origin.position, _rotation);
			ApplyExplosionForce.Explode(_center, (float)_blastPower, _blastRadius);
		}
		if (_explosionChanges.Count > 0)
		{
			this.ChangeBlocks(null, _explosionChanges);
		}
		QuestEventManager.Current.DetectedExplosion(_center, _entityId, _blockDamage);
		return result;
	}

	// Token: 0x06007CFD RID: 31997 RVA: 0x0032A284 File Offset: 0x00328484
	[PublicizedFrom(EAccessModifier.Private)]
	public void ExplodeGroupFrameUpdate()
	{
		int et = EntityClass.FromString("fallingBlock");
		GameRandom gameRandom = this.m_World.GetGameRandom();
		for (int i = this.explodeFallingGroups.Count - 1; i >= 0; i--)
		{
			GameManager.ExplodeGroup explodeGroup = this.explodeFallingGroups[i];
			GameManager.ExplodeGroup explodeGroup2 = explodeGroup;
			int num = explodeGroup2.delay - 1;
			explodeGroup2.delay = num;
			if (num <= 0)
			{
				float num2 = 20f + Mathf.Pow((float)explodeGroup.fallings.Count, 0.73f);
				float num3 = Utils.FastMax(1f, (float)explodeGroup.fallings.Count / num2);
				float num4 = 1f;
				for (int j = 0; j < explodeGroup.fallings.Count; j++)
				{
					if ((num4 -= 1f) <= 0f)
					{
						num4 += num3;
						GameManager.ExplodeGroup.Falling falling = explodeGroup.fallings[j];
						Vector3 vector = falling.pos.ToVector3Center();
						vector.y += 1.4f;
						if (Physics.Raycast(vector - Origin.position, Vector3.down, 3.4028235E+38f, 65536))
						{
							vector.y -= 1.4f;
							Block block = falling.bv.Block;
							block.DropItemsOnEvent(this.m_World, falling.bv, EnumDropEvent.Destroy, 0.5f, vector, Vector3.zero, Constants.cItemExplosionLifetime, -1, true);
							if (block.ShowModelOnFall())
							{
								EntityFallingBlock entityFallingBlock = (EntityFallingBlock)EntityFactory.CreateEntity(et, -1, falling.bv, this.m_World.GetTextureFullArray(falling.pos.x, falling.pos.y, falling.pos.z), 1, vector, Vector3.zero, -1f, -1, null, -1, "");
								Vector3 vector2 = vector - explodeGroup.pos;
								float num5 = 1f - Mathf.Clamp01(vector2.magnitude / explodeGroup.radius) * 0.6f;
								float d = 18f * num5;
								vector2.y += -0.2f + gameRandom.RandomFloat * 6f;
								entityFallingBlock.SetStartVelocity(vector2.normalized * d, (gameRandom.RandomFloat * 15f + 2f) * num5);
								this.m_World.SpawnEntityInWorld(entityFallingBlock);
							}
						}
					}
				}
				this.explodeFallingGroups.RemoveAt(i);
			}
		}
	}

	// Token: 0x06007CFE RID: 31998 RVA: 0x0032A4FB File Offset: 0x003286FB
	[PublicizedFrom(EAccessModifier.Private)]
	public void SavePersistentPlayerData()
	{
		if (!this.isEditMode && this.persistentPlayers != null)
		{
			this.persistentPlayers.Write(GameIO.GetSaveGameDir() + "/players.xml");
		}
	}

	// Token: 0x06007CFF RID: 31999 RVA: 0x0032A528 File Offset: 0x00328728
	public void ChangeBlocks(PlatformUserIdentifierAbs persistentPlayerId, List<BlockChangeInfo> _blocksToChange)
	{
		if (this.m_World == null)
		{
			return;
		}
		List<ChunkCluster> obj = this.ccChanged;
		lock (obj)
		{
			PersistentPlayerData persistentPlayerData = null;
			Entity entity = null;
			if (persistentPlayerId == null)
			{
				persistentPlayerData = this.persistentLocalPlayer;
				entity = this.myEntityPlayerLocal;
			}
			else if (this.persistentPlayers != null)
			{
				persistentPlayerData = this.persistentPlayers.GetPlayerData(persistentPlayerId);
				if (persistentPlayerData != null && persistentPlayerData.EntityId != -1)
				{
					entity = this.m_World.GetEntity(persistentPlayerData.EntityId);
				}
			}
			bool flag2 = false;
			bool flag3 = false;
			ChunkCluster chunkCluster = null;
			int num = 0;
			int i = 0;
			while (i < _blocksToChange.Count)
			{
				BlockChangeInfo blockChangeInfo = _blocksToChange[i];
				if (chunkCluster != null)
				{
					goto IL_C3;
				}
				chunkCluster = this.m_World.ChunkCache;
				if (chunkCluster != null)
				{
					if (!this.ccChanged.Contains(chunkCluster))
					{
						this.ccChanged.Add(chunkCluster);
						num++;
						chunkCluster.ChunkPosNeedsRegeneration_DelayedStart();
						goto IL_C3;
					}
					goto IL_C3;
				}
				IL_63E:
				i++;
				continue;
				IL_C3:
				bool flag4 = blockChangeInfo.bChangeDensity;
				bool bForceDensityChange = blockChangeInfo.bForceDensityChange;
				sbyte density = chunkCluster.GetDensity(blockChangeInfo.pos);
				sbyte b = blockChangeInfo.density;
				if (!flag4)
				{
					if (density < 0 && blockChangeInfo.blockValue.isair)
					{
						b = MarchingCubes.DensityAir;
						flag4 = true;
					}
					else if (density >= 0 && blockChangeInfo.blockValue.Block.shape.IsTerrain())
					{
						b = MarchingCubes.DensityTerrain;
						flag4 = true;
					}
				}
				if (density == b)
				{
					flag4 = false;
				}
				if (blockChangeInfo.bChangeDamage && chunkCluster.GetBlock(blockChangeInfo.pos).type != blockChangeInfo.blockValue.type)
				{
					goto IL_63E;
				}
				Chunk chunk = chunkCluster.GetChunkFromWorldPos(blockChangeInfo.pos) as Chunk;
				int num2 = World.toBlockXZ(blockChangeInfo.pos.x);
				int num3 = World.toBlockXZ(blockChangeInfo.pos.z);
				if (chunk != null)
				{
					if (blockChangeInfo.pos.y >= (int)chunk.GetHeight(World.toBlockXZ(blockChangeInfo.pos.x), World.toBlockXZ(blockChangeInfo.pos.z)) && blockChangeInfo.blockValue.Block.shape.IsTerrain())
					{
						chunk.SetTopSoilBroken(num2, num3);
						Chunk chunk2;
						if (num3 == 15)
						{
							chunk2 = chunkCluster.GetChunkSync(chunk.X, chunk.Z + 1);
						}
						else
						{
							chunk2 = chunk;
						}
						if (chunk2 != null)
						{
							chunk2.SetTopSoilBroken(num2, World.toBlockXZ(num3 + 1));
						}
						if (num2 == 15)
						{
							chunk2 = chunkCluster.GetChunkSync(chunk.X + 1, chunk.Z);
						}
						else
						{
							chunk2 = chunk;
						}
						if (chunk2 != null)
						{
							chunk.SetTopSoilBroken(World.toBlockXZ(num2 + 1), num3);
						}
						if (num3 == 0)
						{
							chunk2 = chunkCluster.GetChunkSync(chunk.X, chunk.Z - 1);
						}
						else
						{
							chunk2 = chunk;
						}
						if (chunk2 != null)
						{
							chunk.SetTopSoilBroken(num2, World.toBlockXZ(num3 - 1));
						}
						if (num2 == 0)
						{
							chunk2 = chunkCluster.GetChunkSync(chunk.X - 1, chunk.Z);
						}
						else
						{
							chunk2 = chunk;
						}
						if (chunk2 != null)
						{
							chunk.SetTopSoilBroken(World.toBlockXZ(num2 - 1), num3);
						}
					}
					this.m_World.UncullChunk(chunk);
				}
				TileEntity tileEntity = null;
				if (!blockChangeInfo.blockValue.ischild)
				{
					tileEntity = this.m_World.GetTileEntity(blockChangeInfo.pos);
				}
				BlockValue bvOld = chunkCluster.SetBlock(blockChangeInfo.pos, blockChangeInfo.bChangeBlockValue, blockChangeInfo.blockValue, flag4, b, true, blockChangeInfo.bUpdateLight, bForceDensityChange, false, blockChangeInfo.changedByEntityId);
				if (tileEntity != null)
				{
					TileEntity tileEntity2 = this.m_World.GetTileEntity(blockChangeInfo.pos);
					if (tileEntity != tileEntity2 && SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
					{
						this.lockedTileEntities.Remove(tileEntity);
						tileEntity.ReplacedBy(bvOld, blockChangeInfo.blockValue, tileEntity2);
					}
					if (blockChangeInfo.blockValue.isair)
					{
						this.lockedTileEntities.Remove(tileEntity);
						if (chunk != null)
						{
							chunk.RemoveTileEntityAt<TileEntity>(this.m_World, World.toBlock(blockChangeInfo.pos));
						}
					}
					else if (tileEntity != tileEntity2)
					{
						this.lockedTileEntities.Remove(tileEntity);
						if (tileEntity2 != null)
						{
							tileEntity2.UpgradeDowngradeFrom(tileEntity);
						}
					}
				}
				if (chunk != null && blockChangeInfo.blockValue.isair)
				{
					chunk.RemoveBlockTrigger(World.toBlock(blockChangeInfo.pos));
				}
				if (bvOld.type != blockChangeInfo.blockValue.type)
				{
					Block block = blockChangeInfo.blockValue.Block;
					Block block2 = bvOld.Block;
					QuestEventManager.Current.BlockChanged(block2, block, blockChangeInfo.pos);
					if (block is BlockLandClaim)
					{
						if (persistentPlayerData != null)
						{
							this.persistentPlayers.PlaceLandProtectionBlock(blockChangeInfo.pos, persistentPlayerData);
							flag2 = true;
							if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
							{
								((BlockLandClaim)block).HandleDeactivatingCurrentLandClaims(persistentPlayerData);
							}
							if (this.m_World != null && BlockLandClaim.IsPrimary(blockChangeInfo.blockValue))
							{
								NavObject navObject = NavObjectManager.Instance.RegisterNavObject("land_claim", blockChangeInfo.pos.ToVector3(), "", false, -1, null);
								if (navObject != null)
								{
									navObject.OwnerEntity = entity;
								}
							}
						}
					}
					else if (block2 is BlockLandClaim)
					{
						this.persistentPlayers.RemoveLandProtectionBlock(blockChangeInfo.pos);
						flag2 = true;
						flag3 = true;
						if (this.m_World != null)
						{
							NavObjectManager.Instance.UnRegisterNavObjectByPosition(blockChangeInfo.pos.ToVector3(), "land_claim");
							if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
							{
								SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityMapMarkerRemove>().Setup(EnumMapObjectType.LandClaim, blockChangeInfo.pos.ToVector3()), false, -1, -1, -1, null, 192, false);
							}
						}
					}
					if (block is BlockSleepingBag || block2 is BlockSleepingBag)
					{
						EntityAlive entityAlive = entity as EntityAlive;
						if (entityAlive)
						{
							if (block is BlockSleepingBag)
							{
								NavObjectManager.Instance.UnRegisterNavObjectByOwnerEntity(entityAlive, "sleeping_bag");
								entityAlive.SpawnPoints.Set(blockChangeInfo.pos);
							}
							else
							{
								this.persistentPlayers.SpawnPointRemoved(blockChangeInfo.pos);
							}
							flag2 = true;
						}
					}
				}
				if (blockChangeInfo.bChangeTexture)
				{
					chunkCluster.SetTextureFullArray(blockChangeInfo.pos, blockChangeInfo.textureFull);
					goto IL_63E;
				}
				if (bvOld.Block.CanBlocksReplace)
				{
					chunkCluster.SetTextureFullArray(blockChangeInfo.pos, new TextureFullArray(0L));
					goto IL_63E;
				}
				goto IL_63E;
			}
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && flag2)
			{
				if (flag3 && entity != null)
				{
					entity.PlayOneShot("keystone_destroyed", false, false, false, null);
				}
				this.SavePersistentPlayerData();
			}
			if (num > 0)
			{
				int num4 = this.ccChanged.Count;
				for (int j = 0; j < num; j++)
				{
					this.ccChanged[--num4].ChunkPosNeedsRegeneration_DelayedStop();
				}
				this.ccChanged.RemoveRange(num4, num);
			}
		}
	}

	// Token: 0x06007D00 RID: 32000 RVA: 0x0032AC28 File Offset: 0x00328E28
	public void SetBlocksRPC(List<BlockChangeInfo> _changes, PlatformUserIdentifierAbs _persistentPlayerId = null)
	{
		this.ChangeBlocks(_persistentPlayerId, _changes);
		NetPackageSetBlock package = NetPackageManager.GetPackage<NetPackageSetBlock>().Setup(this.persistentLocalPlayer, _changes, GameManager.IsDedicatedServer ? -1 : this.myPlayerId);
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.SetBlocksOnClients(-1, package);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(package, false);
	}

	// Token: 0x06007D01 RID: 32001 RVA: 0x0032AC80 File Offset: 0x00328E80
	public void SetBlocksOnClients(int _exceptThisEntityId, NetPackageSetBlock package)
	{
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, false, -1, _exceptThisEntityId, -1, null, 192, false);
	}

	// Token: 0x06007D02 RID: 32002 RVA: 0x0032ACAC File Offset: 0x00328EAC
	public void SetWaterRPC(NetPackageWaterSet package)
	{
		if (this.m_World != null)
		{
			ChunkCluster chunkCache = this.m_World.ChunkCache;
			if (chunkCache != null)
			{
				package.ApplyChanges(chunkCache);
			}
		}
		package.SetSenderId(GameManager.IsDedicatedServer ? -1 : this.myPlayerId);
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, false, -1, -1, -1, null, 192, false);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(package, false);
	}

	// Token: 0x06007D03 RID: 32003 RVA: 0x0032AD24 File Offset: 0x00328F24
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateBlockParticles()
	{
		List<GameManager.BlockParticleCreationData> obj = this.blockParticlesToSpawn;
		lock (obj)
		{
			for (int i = 0; i < this.blockParticlesToSpawn.Count; i++)
			{
				if (this.m_BlockParticles.ContainsKey(this.blockParticlesToSpawn[i].blockPos))
				{
					this.RemoveBlockParticleEffect(this.blockParticlesToSpawn[i].blockPos);
				}
				Transform value = GameManager.Instance.SpawnParticleEffectClientForceCreation(this.blockParticlesToSpawn[i].particleEffect, -1, true);
				this.m_BlockParticles[this.blockParticlesToSpawn[i].blockPos] = value;
			}
			this.blockParticlesToSpawn.Clear();
		}
	}

	// Token: 0x06007D04 RID: 32004 RVA: 0x0032ADF4 File Offset: 0x00328FF4
	public void SpawnBlockParticleEffect(Vector3i _blockPos, ParticleEffect _pe)
	{
		List<GameManager.BlockParticleCreationData> obj = this.blockParticlesToSpawn;
		lock (obj)
		{
			this.blockParticlesToSpawn.Add(new GameManager.BlockParticleCreationData(_blockPos, _pe));
		}
	}

	// Token: 0x06007D05 RID: 32005 RVA: 0x0032AE40 File Offset: 0x00329040
	public bool HasBlockParticleEffect(Vector3i _blockPos)
	{
		return this.m_BlockParticles.ContainsKey(_blockPos);
	}

	// Token: 0x06007D06 RID: 32006 RVA: 0x0032AE4E File Offset: 0x0032904E
	public Transform GetBlockParticleEffect(Vector3i _blockPos)
	{
		return this.m_BlockParticles[_blockPos];
	}

	// Token: 0x06007D07 RID: 32007 RVA: 0x0032AE5C File Offset: 0x0032905C
	public void RemoveBlockParticleEffect(Vector3i _blockPos)
	{
		List<GameManager.BlockParticleCreationData> obj = this.blockParticlesToSpawn;
		lock (obj)
		{
			if (this.m_BlockParticles.ContainsKey(_blockPos))
			{
				Transform transform = this.m_BlockParticles[_blockPos];
				this.m_BlockParticles.Remove(_blockPos);
				if (transform != null)
				{
					UnityEngine.Object.Destroy(transform.gameObject);
				}
			}
			else
			{
				for (int i = this.blockParticlesToSpawn.Count - 1; i >= 0; i--)
				{
					if (this.blockParticlesToSpawn[i].blockPos == _blockPos)
					{
						this.blockParticlesToSpawn.RemoveAt(i);
					}
				}
			}
		}
	}

	// Token: 0x06007D08 RID: 32008 RVA: 0x0032AF10 File Offset: 0x00329110
	public void SpawnParticleEffectServer(ParticleEffect _pe, int _entityId, bool _forceCreation = false, bool _worldSpawn = false)
	{
		if (this.m_World == null)
		{
			return;
		}
		ParticleEffect.SpawnParticleEffect(_pe, _entityId, _forceCreation, _worldSpawn);
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageParticleEffect>().Setup(_pe, _entityId, _forceCreation, _worldSpawn), false);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageParticleEffect>().Setup(_pe, _entityId, _forceCreation, _worldSpawn), false, -1, _entityId, -1, null, 192, false);
	}

	// Token: 0x06007D09 RID: 32009 RVA: 0x0032AF84 File Offset: 0x00329184
	public Transform SpawnParticleEffectClientForceCreation(ParticleEffect _pe, int _entityThatCausedIt, bool _worldSpawn)
	{
		return ParticleEffect.SpawnParticleEffect(_pe, _entityThatCausedIt, true, _worldSpawn);
	}

	// Token: 0x06007D0A RID: 32010 RVA: 0x0032AF8F File Offset: 0x0032918F
	public void SpawnParticleEffectClient(ParticleEffect _pe, int _entityThatCausedIt, bool _forceCreation = false, bool _worldSpawn = false)
	{
		ParticleEffect.SpawnParticleEffect(_pe, _entityThatCausedIt, _forceCreation, _worldSpawn);
	}

	// Token: 0x06007D0B RID: 32011 RVA: 0x0032AF9C File Offset: 0x0032919C
	[PublicizedFrom(EAccessModifier.Private)]
	public void PhysicsInit()
	{
		Physics.ContactEvent += this.PhysicsContactEvent;
	}

	// Token: 0x06007D0C RID: 32012 RVA: 0x0032AFB0 File Offset: 0x003291B0
	[PublicizedFrom(EAccessModifier.Private)]
	public void PhysicsContactEvent(PhysicsScene scene, NativeArray<ContactPairHeader>.ReadOnly pairHeaders)
	{
		int length = pairHeaders.Length;
		for (int i = 0; i < length; i++)
		{
			Rigidbody rigidbody = pairHeaders[i].Body as Rigidbody;
			if (rigidbody)
			{
				EntityFallingBlock component = rigidbody.GetComponent<EntityFallingBlock>();
				if (component)
				{
					component.OnContactEvent();
				}
			}
		}
	}

	// Token: 0x06007D0D RID: 32013 RVA: 0x0032B007 File Offset: 0x00329207
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsEditMode()
	{
		return this.isEditMode;
	}

	// Token: 0x06007D0E RID: 32014 RVA: 0x0032B010 File Offset: 0x00329210
	public void GameMessage(EnumGameMessages _type, EntityAlive _mainEntity, EntityAlive _otherEntity)
	{
		if (_mainEntity == null)
		{
			return;
		}
		int secondaryEntityId = -1;
		if (_mainEntity is EntityPlayer)
		{
			int mainEntityId;
			switch (_type)
			{
			case EnumGameMessages.PlainTextLocal:
			case EnumGameMessages.ChangedTeam:
				return;
			case EnumGameMessages.EntityWasKilled:
				mainEntityId = _mainEntity.entityId;
				if (_otherEntity is EntityPlayer)
				{
					secondaryEntityId = _otherEntity.entityId;
				}
				break;
			case EnumGameMessages.JoinedGame:
			case EnumGameMessages.LeftGame:
			case EnumGameMessages.Chat:
				mainEntityId = _mainEntity.entityId;
				break;
			default:
				return;
			}
			this.GameMessageServer(null, _type, mainEntityId, secondaryEntityId);
			return;
		}
		if (_type == EnumGameMessages.EntityWasKilled || _type == EnumGameMessages.Chat)
		{
			int mainEntityId = (_mainEntity != null) ? _mainEntity.entityId : -1;
			this.GameMessageServer(null, _type, mainEntityId, secondaryEntityId);
			return;
		}
	}

	// Token: 0x06007D0F RID: 32015 RVA: 0x0032B0A8 File Offset: 0x003292A8
	public void GameMessageServer(ClientInfo _cInfo, EnumGameMessages _type, int _mainEntityId, int _secondaryEntityId)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageGameMessage>().Setup(_type, _mainEntityId, _secondaryEntityId), false);
			return;
		}
		Entity entity = this.World.GetEntity(_mainEntityId);
		EntityPlayer entityPlayer = entity as EntityPlayer;
		string mainName;
		if (entityPlayer != null)
		{
			mainName = entityPlayer.PlayerDisplayName;
		}
		else
		{
			EntityAlive entityAlive = entity as EntityAlive;
			if (entityAlive != null)
			{
				mainName = Localization.Get(entityAlive.EntityName, false);
			}
			else
			{
				mainName = Localization.Get("xuiChatServer", false);
			}
		}
		this.FinishGameMessageServer(_cInfo, _type, _mainEntityId, _secondaryEntityId, mainName);
	}

	// Token: 0x06007D10 RID: 32016 RVA: 0x0032B12C File Offset: 0x0032932C
	[PublicizedFrom(EAccessModifier.Private)]
	public void FinishGameMessageServer(ClientInfo _cInfo, EnumGameMessages _type, int _mainEntityId, int _secondaryEntityId, string _mainName)
	{
		PersistentPlayerData playerDataFromEntityID = this.persistentPlayers.GetPlayerDataFromEntityID(_secondaryEntityId);
		string secondaryName = (playerDataFromEntityID != null) ? playerDataFromEntityID.PlayerName.DisplayName : null;
		ModEvents.SGameMessageData sgameMessageData = new ModEvents.SGameMessageData(_cInfo, _type, _mainName, secondaryName);
		ValueTuple<ModEvents.EModEventResult, Mod> valueTuple = ModEvents.GameMessage.Invoke(ref sgameMessageData);
		ModEvents.EModEventResult item = valueTuple.Item1;
		Mod item2 = valueTuple.Item2;
		string text = this.DisplayGameMessage(_type, _mainEntityId, _secondaryEntityId, item2 == null);
		if (item != ModEvents.EModEventResult.StopHandlersAndVanilla)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageGameMessage>().Setup(_type, _mainEntityId, _secondaryEntityId), true, -1, -1, -1, null, 192, false);
			return;
		}
		Log.Out("GameMessage handled by mod '{0}': {1}", new object[]
		{
			item2.Name,
			text
		});
	}

	// Token: 0x06007D11 RID: 32017 RVA: 0x0032B1DC File Offset: 0x003293DC
	public string DisplayGameMessage(EnumGameMessages _type, int _mainEntity, int _secondaryEntity = -1, bool _log = true)
	{
		string text = null;
		PersistentPlayerData playerDataFromEntityID = this.persistentPlayers.GetPlayerDataFromEntityID(_mainEntity);
		string text2;
		if (playerDataFromEntityID == null)
		{
			text2 = null;
		}
		else
		{
			PersistentPlayerName playerName = playerDataFromEntityID.PlayerName;
			text2 = ((playerName != null) ? playerName.DisplayName : null);
		}
		string text3 = text2;
		string text4;
		if (_secondaryEntity != -1)
		{
			PersistentPlayerData playerDataFromEntityID2 = this.persistentPlayers.GetPlayerDataFromEntityID(_secondaryEntity);
			if (playerDataFromEntityID2 == null)
			{
				text4 = null;
			}
			else
			{
				PersistentPlayerName playerName2 = playerDataFromEntityID2.PlayerName;
				text4 = ((playerName2 != null) ? playerName2.DisplayName : null);
			}
		}
		else
		{
			text4 = null;
		}
		string text5 = text4;
		switch (_type)
		{
		case EnumGameMessages.EntityWasKilled:
		{
			string message;
			if (!string.IsNullOrEmpty(text5))
			{
				text = string.Format("GMSG: Player '{0}' killed by '{1}'", text3, text5);
				message = string.Format(Localization.Get("killedGameMessage", false), text5, text3);
				goto IL_133;
			}
			text = string.Format("GMSG: Player '{0}' died", text3);
			message = string.Format(Localization.Get("diedGameMessage", false), text3);
			goto IL_133;
		}
		case EnumGameMessages.JoinedGame:
		{
			text = string.Format("GMSG: Player '{0}' joined the game", text3);
			string message = string.Format(Localization.Get("joinGameMessage", false), text3);
			goto IL_133;
		}
		case EnumGameMessages.LeftGame:
		{
			text = string.Format("GMSG: Player '{0}' left the game", text3);
			string message = string.Format(Localization.Get("leaveGameMessage", false), text3);
			goto IL_133;
		}
		case EnumGameMessages.BlockedPlayerAlert:
		{
			text = string.Format("GMSG: Blocked player '{0}' is present on this server!", text3);
			string message = string.Format("[FF0000A0]" + Localization.Get("blockedPlayerMessage", false), text3);
			goto IL_133;
		}
		}
		return text;
		IL_133:
		if (_log)
		{
			Log.Out(text);
		}
		if (!GameManager.IsDedicatedServer)
		{
			if (_type == EnumGameMessages.BlockedPlayerAlert)
			{
				string message;
				XUiC_ChatOutput.AddMessage(this.myEntityPlayerLocal.PlayerUI.xui, _type, message, EChatType.Global, EChatDirection.Inbound, -1, null, null, EMessageSender.None, GeneratedTextManager.TextFilteringMode.None, GeneratedTextManager.BbCodeSupportMode.Supported);
			}
			else
			{
				foreach (EntityPlayerLocal entityPlayer in this.m_World.GetLocalPlayers())
				{
					string message;
					XUiC_ChatOutput.AddMessage(LocalPlayerUI.GetUIForPlayer(entityPlayer).xui, _type, message, EChatType.Global, EChatDirection.Inbound, -1, null, null, EMessageSender.None, GeneratedTextManager.TextFilteringMode.None, GeneratedTextManager.BbCodeSupportMode.Supported);
				}
			}
		}
		return text;
	}

	// Token: 0x06007D12 RID: 32018 RVA: 0x0032B3B0 File Offset: 0x003295B0
	public void ChatMessageServer(ClientInfo _cInfo, EChatType _chatType, int _senderEntityId, string _msg, List<int> _recipientEntityIds, EMessageSender _msgSender, GeneratedTextManager.BbCodeSupportMode _bbMode = GeneratedTextManager.BbCodeSupportMode.Supported)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			string text = null;
			if (_senderEntityId != -1)
			{
				PersistentPlayerData playerDataFromEntityID = this.persistentPlayers.GetPlayerDataFromEntityID(_senderEntityId);
				string input;
				if (playerDataFromEntityID == null)
				{
					input = null;
				}
				else
				{
					PersistentPlayerName playerName = playerDataFromEntityID.PlayerName;
					if (playerName == null)
					{
						input = null;
					}
					else
					{
						AuthoredText authoredName = playerName.AuthoredName;
						input = ((authoredName != null) ? authoredName.Text : null);
					}
				}
				text = Utils.EscapeBbCodes(input, false, false);
			}
			ModEvents.SChatMessageData schatMessageData = new ModEvents.SChatMessageData(_cInfo, _chatType, _senderEntityId, _msg, text, _recipientEntityIds);
			ValueTuple<ModEvents.EModEventResult, Mod> valueTuple = ModEvents.ChatMessage.Invoke(ref schatMessageData);
			ModEvents.EModEventResult item = valueTuple.Item1;
			Mod item2 = valueTuple.Item2;
			this.ChatMessageClient(_chatType, _senderEntityId, _msg, _recipientEntityIds, _msgSender, GeneratedTextManager.BbCodeSupportMode.Supported);
			string text2 = (((_cInfo != null) ? _cInfo.PlatformId : null) != null) ? _cInfo.PlatformId.CombinedString : "-non-player-";
			string text3 = string.Format("Chat (from '{0}', entity id '{1}', to '{2}'): {3}{4}", new object[]
			{
				text2,
				_senderEntityId,
				_chatType.ToStringCached<EChatType>(),
				(text != null) ? ("'" + text + "': ") : "",
				_msg
			});
			if (item == ModEvents.EModEventResult.StopHandlersAndVanilla)
			{
				Log.Out("Chat handled by mod '{0}': {1}", new object[]
				{
					item2.Name,
					text3
				});
			}
			else
			{
				Log.Out(text3);
			}
			if (item != ModEvents.EModEventResult.StopHandlersAndVanilla)
			{
				if (_recipientEntityIds != null)
				{
					using (List<int>.Enumerator enumerator = _recipientEntityIds.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							int entityId = enumerator.Current;
							ClientInfo clientInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForEntityId(entityId);
							if (clientInfo != null)
							{
								clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(_chatType, _senderEntityId, _msg, null, _msgSender, _bbMode));
							}
						}
						return;
					}
				}
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(_chatType, _senderEntityId, _msg, null, _msgSender, _bbMode), true, -1, -1, -1, null, 192, false);
				return;
			}
		}
		else
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageChat>().Setup(_chatType, _senderEntityId, _msg, _recipientEntityIds, _msgSender, _bbMode), false);
		}
	}

	// Token: 0x06007D13 RID: 32019 RVA: 0x0032B5A8 File Offset: 0x003297A8
	public void ChatMessageClient(EChatType _chatType, int _senderEntityId, string _msg, List<int> _recipientEntityIds, EMessageSender _msgSender, GeneratedTextManager.BbCodeSupportMode _bbMode)
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		foreach (EntityPlayerLocal entityPlayerLocal in this.m_World.GetLocalPlayers())
		{
			if (_recipientEntityIds == null || _recipientEntityIds.Contains(entityPlayerLocal.entityId))
			{
				XUiC_ChatOutput.AddMessage(LocalPlayerUI.GetUIForPlayer(entityPlayerLocal).xui, EnumGameMessages.Chat, _msg, _chatType, EChatDirection.Inbound, _senderEntityId, null, _senderEntityId.ToString(), _msgSender, GeneratedTextManager.TextFilteringMode.Filter, _bbMode);
			}
		}
	}

	// Token: 0x06007D14 RID: 32020 RVA: 0x0032B638 File Offset: 0x00329838
	public void RemoveChunk(long _chunkKey)
	{
		this.m_World.m_ChunkManager.RemoveChunk(_chunkKey);
	}

	// Token: 0x06007D15 RID: 32021 RVA: 0x0032B64B File Offset: 0x0032984B
	public IBlockTool GetActiveBlockTool()
	{
		if (this.activeBlockTool == null)
		{
			return this.blockSelectionTool;
		}
		return this.activeBlockTool;
	}

	// Token: 0x06007D16 RID: 32022 RVA: 0x0032B662 File Offset: 0x00329862
	public void SetActiveBlockTool(IBlockTool _tool)
	{
		this.activeBlockTool = _tool;
	}

	// Token: 0x06007D17 RID: 32023 RVA: 0x0032B66C File Offset: 0x0032986C
	public DynamicPrefabDecorator GetDynamicPrefabDecorator()
	{
		if (this.m_World == null)
		{
			return null;
		}
		ChunkCluster chunkCache = this.m_World.ChunkCache;
		if (chunkCache == null)
		{
			return null;
		}
		return chunkCache.ChunkProvider.GetDynamicPrefabDecorator();
	}

	// Token: 0x06007D18 RID: 32024 RVA: 0x0032B6A0 File Offset: 0x003298A0
	public void SimpleRPC(int _entityId, SimpleRPCType _rpcType, bool _bExeLocal, bool _bOnlyLocal)
	{
		if (_bExeLocal)
		{
			EntityAlive entityAlive = (EntityAlive)this.m_World.GetEntity(_entityId);
			if (entityAlive != null)
			{
				if (_rpcType != SimpleRPCType.OnActivateItem)
				{
					if (_rpcType == SimpleRPCType.OnResetItem)
					{
						entityAlive.inventory.holdingItem.OnHoldingReset(entityAlive.inventory.holdingItemData);
					}
				}
				else
				{
					entityAlive.inventory.holdingItem.OnHoldingItemActivated(entityAlive.inventory.holdingItemData);
				}
			}
		}
		if (_bOnlyLocal)
		{
			return;
		}
		NetPackage package = NetPackageManager.GetPackage<NetPackageSimpleRPC>().Setup(_entityId, _rpcType);
		if (this.m_World.IsRemote())
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(package, false);
			return;
		}
		this.m_World.entityDistributer.SendPacketToTrackedPlayers(_entityId, _entityId, package, false);
	}

	// Token: 0x06007D19 RID: 32025 RVA: 0x0032B74C File Offset: 0x0032994C
	public void ItemDropServer(ItemStack _itemStack, Vector3 _dropPos, Vector3 _randomPosAdd, int _entityId = -1, float _lifetime = 60f, bool _bDropPosIsRelativeToHead = false)
	{
		this.ItemDropServer(_itemStack, _dropPos, _randomPosAdd, Vector3.zero, _entityId, _lifetime, _bDropPosIsRelativeToHead, 0);
	}

	// Token: 0x06007D1A RID: 32026 RVA: 0x0032B770 File Offset: 0x00329970
	public void ItemDropServer(ItemStack _itemStack, Vector3 _dropPos, Vector3 _randomPosAdd, Vector3 _initialMotion, int _entityId = -1, float _lifetime = 60f, bool _bDropPosIsRelativeToHead = false, int _clientEntityId = 0)
	{
		if (this.m_World == null)
		{
			return;
		}
		bool flag = SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer;
		Entity entity = this.m_World.GetEntity(_entityId);
		if (_clientEntityId != 0)
		{
			if (!entity)
			{
				return;
			}
			flag = !entity.isEntityRemote;
		}
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			if (_clientEntityId == -1)
			{
				World world = this.m_World;
				int num = world.clientLastEntityId - 1;
				world.clientLastEntityId = num;
				_clientEntityId = num;
			}
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageItemDrop>().Setup(_itemStack, _dropPos, _initialMotion, _randomPosAdd, _lifetime, _entityId, _bDropPosIsRelativeToHead, _clientEntityId), false);
			if (!flag)
			{
				return;
			}
		}
		if (_bDropPosIsRelativeToHead)
		{
			if (entity == null)
			{
				return;
			}
			_dropPos += entity.getHeadPosition();
		}
		if (!_randomPosAdd.Equals(Vector3.zero))
		{
			_dropPos += new Vector3(this.m_World.RandomRange(-_randomPosAdd.x, _randomPosAdd.x), this.m_World.RandomRange(-_randomPosAdd.y, _randomPosAdd.y), this.m_World.RandomRange(-_randomPosAdd.z, _randomPosAdd.z));
		}
		EntityCreationData entityCreationData = new EntityCreationData();
		entityCreationData.entityClass = EntityClass.FromString("item");
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && _clientEntityId < -1)
		{
			entityCreationData.id = _clientEntityId;
		}
		else
		{
			entityCreationData.id = EntityFactory.nextEntityID++;
		}
		entityCreationData.itemStack = _itemStack.Clone();
		entityCreationData.pos = _dropPos;
		entityCreationData.rot = new Vector3(20f, 0f, 20f);
		entityCreationData.lifetime = _lifetime;
		entityCreationData.belongsPlayerId = _entityId;
		if (_clientEntityId != -1)
		{
			entityCreationData.clientEntityId = _clientEntityId;
		}
		EntityItem entityItem = (EntityItem)EntityFactory.CreateEntity(entityCreationData);
		entityItem.isPhysicsMaster = flag;
		if (_initialMotion.sqrMagnitude > 0.01f)
		{
			entityItem.AddVelocity(_initialMotion);
		}
		this.m_World.SpawnEntityInWorld(entityItem);
		Chunk chunk = (Chunk)this.m_World.GetChunkSync(World.toChunkXZ((int)_dropPos.x), World.toChunkXZ((int)_dropPos.z));
		if (chunk != null)
		{
			List<EntityItem> list = new List<EntityItem>();
			for (int i = 0; i < chunk.entityLists.Length; i++)
			{
				if (chunk.entityLists[i] != null)
				{
					for (int j = 0; j < chunk.entityLists[i].Count; j++)
					{
						if (chunk.entityLists[i][j] is EntityItem)
						{
							list.Add(chunk.entityLists[i][j] as EntityItem);
						}
					}
				}
			}
			int num2 = list.Count - 50;
			if (num2 > 0)
			{
				list.Sort(new GameManager.EntityItemLifetimeComparer());
				int num3 = list.Count - 1;
				while (num3 >= 0 && num2 > 0)
				{
					list[num3].MarkToUnload();
					num2--;
					num3--;
				}
			}
		}
	}

	// Token: 0x06007D1B RID: 32027 RVA: 0x0032BA47 File Offset: 0x00329C47
	public void AddExpServer(int _entityId, string UNUSED_skill, int _experience)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEntityAddExpServer>().Setup(_entityId, _experience), false);
		}
	}

	// Token: 0x06007D1C RID: 32028 RVA: 0x0032BA6C File Offset: 0x00329C6C
	public void AddScoreServer(int _entityId, int _zombieKills, int _playerKills, int _otherTeamnumber, int _conditions)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEntityAddScoreServer>().Setup(_entityId, _zombieKills, _playerKills, _otherTeamnumber, _conditions), false);
			return;
		}
		EntityAlive entityAlive = (EntityAlive)this.m_World.GetEntity(_entityId);
		if (entityAlive == null)
		{
			return;
		}
		if (entityAlive.isEntityRemote)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityAddScoreClient>().Setup(_entityId, _zombieKills, _playerKills, _otherTeamnumber, _conditions), false, entityAlive.entityId, -1, -1, null, 192, false);
			return;
		}
		entityAlive.AddScore(0, _zombieKills, _playerKills, _otherTeamnumber, _conditions);
	}

	// Token: 0x06007D1D RID: 32029 RVA: 0x0032BB08 File Offset: 0x00329D08
	public void AwardKill(EntityAlive killer, EntityAlive killedEntity)
	{
		if (killer.isEntityRemote)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityAwardKillServer>().Setup(killer.entityId, killedEntity.entityId), false, killer.entityId, -1, -1, null, 192, false);
			return;
		}
		QuestEventManager.Current.EntityKilled(killer, killedEntity);
	}

	// Token: 0x06007D1E RID: 32030 RVA: 0x0032BB64 File Offset: 0x00329D64
	public void ItemReloadServer(int _entityId)
	{
		if (this.m_World == null)
		{
			return;
		}
		this.ItemReloadClient(_entityId);
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageItemReload>().Setup(_entityId), false);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageItemReload>().Setup(_entityId), false, -1, _entityId, -1, null, 192, false);
	}

	// Token: 0x06007D1F RID: 32031 RVA: 0x0032BBCC File Offset: 0x00329DCC
	public void ItemReloadClient(int _entityId)
	{
		if (this.m_World == null)
		{
			return;
		}
		EntityAlive entityAlive = (EntityAlive)this.m_World.GetEntity(_entityId);
		if (entityAlive != null && entityAlive.inventory.IsHoldingGun())
		{
			entityAlive.inventory.GetHoldingGun().ReloadGun(entityAlive.inventory.holdingItemData.actionData[0]);
		}
	}

	// Token: 0x06007D20 RID: 32032 RVA: 0x0032BC30 File Offset: 0x00329E30
	public void ItemActionEffectsServer(int _entityId, int _slotIdx, int _itemActionIdx, int _firingState, Vector3 _startPos, Vector3 _direction, int _userData = 0)
	{
		if (this.m_World == null)
		{
			return;
		}
		this.ItemActionEffectsClient(_entityId, _slotIdx, _itemActionIdx, _firingState, _startPos, _direction, _userData);
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageItemActionEffects>().Setup(_entityId, _slotIdx, _itemActionIdx, (ItemActionFiringState)_firingState, _startPos, _direction, _userData), false);
			return;
		}
		int allButAttachedToEntityId = _entityId;
		Entity entity = this.m_World.GetEntity(_entityId);
		if (entity != null && entity.AttachedMainEntity != null)
		{
			allButAttachedToEntityId = entity.AttachedMainEntity.entityId;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageItemActionEffects>().Setup(_entityId, _slotIdx, _itemActionIdx, (ItemActionFiringState)_firingState, _startPos, _direction, _userData), false, -1, allButAttachedToEntityId, _entityId, null, 192, false);
	}

	// Token: 0x06007D21 RID: 32033 RVA: 0x0032BCE8 File Offset: 0x00329EE8
	public void ItemActionEffectsClient(int _entityId, int _slotIdx, int _itemActionIdx, int _firingState, Vector3 _startPos, Vector3 _direction, int _userData = 0)
	{
		if (this.m_World == null)
		{
			return;
		}
		EntityAlive entityAlive = (EntityAlive)this.m_World.GetEntity(_entityId);
		if (entityAlive == null)
		{
			return;
		}
		ItemAction itemActionInSlot = entityAlive.inventory.GetItemActionInSlot(_slotIdx, _itemActionIdx);
		if (itemActionInSlot == null)
		{
			return;
		}
		itemActionInSlot.ItemActionEffects(this, entityAlive.inventory.GetItemActionDataInSlot(_slotIdx, _itemActionIdx), _firingState, _startPos, _direction, _userData);
	}

	// Token: 0x06007D22 RID: 32034 RVA: 0x0032BD48 File Offset: 0x00329F48
	public void SetWorldTime(ulong _worldTime)
	{
		if (this.m_World == null)
		{
			return;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			_worldTime = this.m_World.worldTime;
		}
		this.m_World.SetTime(_worldTime);
	}

	// Token: 0x06007D23 RID: 32035 RVA: 0x0032BD78 File Offset: 0x00329F78
	public void AddVelocityToEntityServer(int _entityId, Vector3 _velToAdd)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEntityAddVelocity>().Setup(_entityId, _velToAdd), false);
			return;
		}
		Entity entity = this.m_World.GetEntity(_entityId);
		if (entity != null)
		{
			entity.AddVelocity(_velToAdd);
		}
	}

	// Token: 0x06007D24 RID: 32036 RVA: 0x0032BDC8 File Offset: 0x00329FC8
	public void CollectEntityServer(int _entityId, int _playerId)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEntityCollect>().Setup(_entityId, _playerId), false);
			return;
		}
		Entity entity = this.m_World.GetEntity(_entityId);
		if (entity == null || (!(entity is EntityItem) && !(entity is EntityVehicle) && !(entity is EntityTurret) && !(entity is EntityDrone)))
		{
			return;
		}
		if (this.m_World.IsLocalPlayer(_playerId))
		{
			this.CollectEntityClient(_entityId, _playerId);
		}
		else
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityCollect>().Setup(_entityId, _playerId), false, _playerId, -1, -1, null, 192, false);
		}
		this.m_World.RemoveEntity(entity.entityId, EnumRemoveEntityReason.Killed);
	}

	// Token: 0x06007D25 RID: 32037 RVA: 0x0032BE84 File Offset: 0x0032A084
	public void CollectEntityClient(int _entityId, int _playerId)
	{
		Entity entity = this.m_World.GetEntity(_entityId);
		if (entity == null)
		{
			return;
		}
		EntityVehicle entityVehicle = entity as EntityVehicle;
		if (entityVehicle)
		{
			entityVehicle.Collect(_playerId);
			return;
		}
		EntityDrone entityDrone = entity as EntityDrone;
		if (entityDrone)
		{
			entityDrone.Collect(_playerId);
			return;
		}
		EntityTurret entityTurret = entity as EntityTurret;
		if (entityTurret)
		{
			entityTurret.Collect(_playerId);
			return;
		}
		EntityItem entityItem = entity as EntityItem;
		if (!entityItem)
		{
			return;
		}
		EntityPlayerLocal entityPlayerLocal = this.m_World.GetEntity(_playerId) as EntityPlayerLocal;
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(entityPlayerLocal);
		bool flag = entityPlayerLocal.inventory.IsHoldingItemActionRunning();
		int num = flag ? uiforPlayer.xui.PlayerInventory.CountAvailableSpaceForItem(entityItem.itemStack.itemValue, false) : -1;
		if ((!flag || num - entityItem.itemStack.itemValue.ItemClass.Stacknumber.Value > entityItem.itemStack.count) && uiforPlayer.xui.PlayerInventory.AddItem(entityItem.itemStack))
		{
			return;
		}
		this.ItemDropServer(entityItem.itemStack, entity.GetPosition(), Vector3.zero, _playerId, 60f, false);
	}

	// Token: 0x06007D26 RID: 32038 RVA: 0x0032BFB0 File Offset: 0x0032A1B0
	public void PickupBlockServer(int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _playerId, PlatformUserIdentifierAbs persistentPlayerId = null)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePickupBlock>().Setup(_clrIdx, _blockPos, _blockValue, _playerId, this.persistentLocalPlayer), false);
			return;
		}
		if (this.m_World.GetBlock(_clrIdx, _blockPos).type != _blockValue.type)
		{
			return;
		}
		if (this.m_World.IsLocalPlayer(_playerId))
		{
			this.PickupBlockClient(_clrIdx, _blockPos, _blockValue, _playerId);
		}
		else
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackagePickupBlock>().Setup(_clrIdx, _blockPos, _blockValue, _playerId, null), false, _playerId, -1, -1, null, 192, false);
		}
		BlockValue blockValue = (_blockValue.Block.PickupSource != null) ? Block.GetBlockValue(_blockValue.Block.PickupSource, false) : BlockValue.Air;
		this.SetBlocksRPC(new List<BlockChangeInfo>
		{
			new BlockChangeInfo(_blockPos, blockValue, true, false)
		}, persistentPlayerId);
	}

	// Token: 0x06007D27 RID: 32039 RVA: 0x0032C098 File Offset: 0x0032A298
	public void PickupBlockClient(int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _playerId)
	{
		if (this.m_World.GetBlock(_clrIdx, _blockPos).type != _blockValue.type)
		{
			return;
		}
		ItemStack itemStack = _blockValue.Block.OnBlockPickedUp(this.m_World, _clrIdx, _blockPos, _blockValue, _playerId);
		foreach (EntityPlayerLocal entityPlayerLocal in this.m_World.GetLocalPlayers())
		{
			if (entityPlayerLocal.entityId == _playerId && entityPlayerLocal.PlayerUI.xui.PlayerInventory.AddItem(itemStack, true))
			{
				return;
			}
		}
		this.ItemDropServer(itemStack, _blockPos.ToVector3() + Vector3.one * 0.5f, Vector3.zero, _playerId, 60f, false);
	}

	// Token: 0x06007D28 RID: 32040 RVA: 0x0032C174 File Offset: 0x0032A374
	public void PlaySoundAtPositionServer(Vector3 _pos, string _audioClipName, AudioRolloffMode _mode, int _distance)
	{
		this.PlaySoundAtPositionServer(_pos, _audioClipName, _mode, _distance, this.m_World.GetPrimaryPlayerId());
	}

	// Token: 0x06007D29 RID: 32041 RVA: 0x0032C18C File Offset: 0x0032A38C
	public void PlaySoundAtPositionServer(Vector3 _pos, string _audioClipName, AudioRolloffMode _mode, int _distance, int _entityId)
	{
		if (this.m_World == null)
		{
			return;
		}
		if (!GameManager.IsDedicatedServer)
		{
			Manager.BroadcastPlay(_pos, _audioClipName, 0f);
			if (this.m_World.aiDirector != null)
			{
				this.m_World.aiDirector.NotifyNoise(this.m_World.GetEntity(_entityId), _pos, _audioClipName, 1f);
			}
		}
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageSoundAtPosition>().Setup(_pos, _audioClipName, _mode, _distance, _entityId), false);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageSoundAtPosition>().Setup(_pos, _audioClipName, _mode, _distance, _entityId), false, -1, _entityId, -1, null, 192, false);
	}

	// Token: 0x06007D2A RID: 32042 RVA: 0x0032C23E File Offset: 0x0032A43E
	public void PlaySoundAtPositionClient(Vector3 _pos, string _audioClipName, AudioRolloffMode _mode, int _distance)
	{
		if (this.m_World == null)
		{
			return;
		}
		Manager.Play(_pos, _audioClipName, -1, false);
		if (this.m_World.aiDirector != null)
		{
			this.m_World.aiDirector.NotifyNoise(null, _pos, _audioClipName, 1f);
		}
	}

	// Token: 0x06007D2B RID: 32043 RVA: 0x0032C278 File Offset: 0x0032A478
	public void WaypointInviteServer(Waypoint _waypoint, EnumWaypointInviteMode _inviteMode, int _inviterEntityId)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageWaypoint>().Setup(_waypoint, _inviteMode, _inviterEntityId), false);
			return;
		}
		_waypoint = _waypoint.Clone();
		_waypoint.bTracked = false;
		if (_inviteMode != EnumWaypointInviteMode.Friends)
		{
			for (int i = 0; i < this.m_World.Players.list.Count; i++)
			{
				EntityPlayer entityPlayer = this.m_World.Players.list[i];
				if (entityPlayer.entityId != _inviterEntityId)
				{
					if (this.m_World.IsLocalPlayer(entityPlayer.entityId))
					{
						this.WaypointInviteClient(_waypoint, _inviteMode, _inviterEntityId, null);
					}
					else
					{
						SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageWaypoint>().Setup(_waypoint, _inviteMode, _inviterEntityId), false, entityPlayer.entityId, -1, -1, null, 192, false);
					}
				}
			}
			return;
		}
		if (this.m_World.GetEntity(_inviterEntityId) as EntityPlayer == null)
		{
			return;
		}
		PersistentPlayerData playerDataFromEntityID = this.persistentPlayers.GetPlayerDataFromEntityID(_inviterEntityId);
		if (playerDataFromEntityID == null)
		{
			return;
		}
		for (int j = 0; j < this.m_World.Players.list.Count; j++)
		{
			EntityPlayer entityPlayer2 = this.m_World.Players.list[j];
			if (entityPlayer2.entityId != _inviterEntityId)
			{
				PersistentPlayerData persistentPlayerData = (this.persistentPlayers != null) ? this.persistentPlayers.GetPlayerDataFromEntityID(entityPlayer2.entityId) : null;
				if (persistentPlayerData != null && playerDataFromEntityID.ACL != null && playerDataFromEntityID.ACL.Contains(persistentPlayerData.PrimaryId))
				{
					if (this.m_World.IsLocalPlayer(entityPlayer2.entityId))
					{
						this.WaypointInviteClient(_waypoint, _inviteMode, _inviterEntityId, null);
					}
					else
					{
						SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageWaypoint>().Setup(_waypoint, _inviteMode, _inviterEntityId), false, entityPlayer2.entityId, -1, -1, null, 192, false);
					}
				}
			}
		}
	}

	// Token: 0x06007D2C RID: 32044 RVA: 0x0032C45C File Offset: 0x0032A65C
	public void RemovePartyInvitesFromAllPlayers(EntityPlayer _player)
	{
		for (int i = 0; i < this.m_World.Players.list.Count; i++)
		{
			EntityPlayer entityPlayer = this.m_World.Players.list[i];
			if (entityPlayer != _player)
			{
				entityPlayer.RemovePartyInvite(_player.entityId);
			}
		}
	}

	// Token: 0x06007D2D RID: 32045 RVA: 0x0032C4B8 File Offset: 0x0032A6B8
	public void WaypointInviteClient(Waypoint _waypoint, EnumWaypointInviteMode _inviteMode, int _inviterEntityId, EntityPlayerLocal _player = null)
	{
		if (_player == null)
		{
			_player = this.myEntityPlayerLocal;
		}
		if (_player == null)
		{
			return;
		}
		PersistentPlayerData playerDataFromEntityID = this.persistentPlayers.GetPlayerDataFromEntityID(_inviterEntityId);
		if (playerDataFromEntityID != null && playerDataFromEntityID.PlatformData.Blocked[EBlockType.TextChat].IsBlocked())
		{
			return;
		}
		if (_player.Waypoints.ContainsWaypoint(_waypoint))
		{
			return;
		}
		for (int i = 0; i < _player.WaypointInvites.Count; i++)
		{
			if (_player.WaypointInvites[i].Equals(_waypoint))
			{
				return;
			}
		}
		_player.WaypointInvites.Insert(0, _waypoint);
		XUiV_Window window = LocalPlayerUI.GetUIForPlayer(_player).xui.GetWindow("mapInvites");
		if (window != null && window.IsVisible)
		{
			((XUiC_MapInvitesList)window.Controller.GetChildById("invitesList")).UpdateInvitesList();
		}
		string strPlayerName = "?";
		EntityPlayer entityPlayer = this.m_World.GetEntity(_inviterEntityId) as EntityPlayer;
		if (entityPlayer != null)
		{
			strPlayerName = entityPlayer.PlayerDisplayName;
		}
		GeneratedTextManager.GetDisplayText(_waypoint.name, delegate(string _filtered)
		{
			GameManager.ShowTooltip(_player, string.Format(Localization.Get("tooltipInviteMarker", false), strPlayerName, _waypoint.bUsingLocalizationId ? Localization.Get(_filtered, false) : _filtered), false, false, 0f);
		}, true, false, GeneratedTextManager.TextFilteringMode.Filter, GeneratedTextManager.BbCodeSupportMode.SupportedAndAddEscapes);
	}

	// Token: 0x06007D2E RID: 32046 RVA: 0x0032C630 File Offset: 0x0032A830
	public void QuestShareServer(int _questCode, string _questID, string _poiName, Vector3 _position, Vector3 _size, Vector3 _returnPos, int _sharedByEntityID, int _sharedWithEntityID, int _questGiverID)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageSharedQuest>().Setup(_questCode, _questID, _poiName, _position, _size, _returnPos, _sharedByEntityID, _sharedWithEntityID, _questGiverID), false);
			return;
		}
		if (this.m_World.IsLocalPlayer(_sharedWithEntityID))
		{
			this.QuestShareClient(_questCode, _questID, _poiName, _position, _size, _returnPos, _sharedByEntityID, _questGiverID, null);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageSharedQuest>().Setup(_questCode, _questID, _poiName, _position, _size, _returnPos, _sharedByEntityID, _sharedWithEntityID, _questGiverID), false, _sharedWithEntityID, -1, -1, null, 192, false);
	}

	// Token: 0x06007D2F RID: 32047 RVA: 0x0032C6CC File Offset: 0x0032A8CC
	public void QuestShareClient(int _questCode, string _questID, string _poiName, Vector3 _position, Vector3 _size, Vector3 _returnPos, int _SharedByEntityID, int _questGiverID, EntityPlayerLocal _player = null)
	{
		if (_player == null)
		{
			_player = this.myEntityPlayerLocal;
		}
		if (_player == null)
		{
			return;
		}
		if (_player.QuestJournal.HasActiveQuestByQuestCode(_questCode))
		{
			if (PartyQuests.AutoAccept)
			{
				Log.Out(string.Format("Ignoring received quest, already have one active with the quest code {0}:", _questCode));
				for (int i = 0; i < _player.QuestJournal.quests.Count; i++)
				{
					Quest quest = _player.QuestJournal.quests[i];
					Log.Out(string.Format("  {0}.: id={1}, code={2}, name={3}, POI={4}, state={5}, owner={6}", new object[]
					{
						i,
						quest.ID,
						quest.QuestCode,
						quest.QuestClass.Name,
						quest.GetParsedText("{poi.name}"),
						quest.CurrentState,
						quest.SharedOwnerID
					}));
				}
			}
			return;
		}
		if (_player.QuestJournal.AddSharedQuestEntry(_questCode, _questID, _poiName, _position, _size, _returnPos, _SharedByEntityID, _questGiverID) && !PartyQuests.AutoAccept)
		{
			string arg = "?";
			EntityPlayer entityPlayer = this.m_World.GetEntity(_SharedByEntityID) as EntityPlayer;
			if (entityPlayer != null)
			{
				arg = entityPlayer.PlayerDisplayName;
			}
			GameManager.ShowTooltip(_player, string.Format(Localization.Get("ttQuestShared", false), arg, QuestClass.GetQuest(_questID).Name), string.Empty, "ui_quest_invite", null, false, false, 0f);
		}
	}

	// Token: 0x06007D30 RID: 32048 RVA: 0x0032C84C File Offset: 0x0032AA4C
	public void SharedKillServer(int _entityID, int _killerID, float _xpModifier = 1f)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageSharedPartyKill>().Setup(_entityID, _killerID), false);
			return;
		}
		EntityPlayer entityPlayer = (EntityPlayer)this.m_World.GetEntity(_killerID);
		EntityAlive entityAlive = this.m_World.GetEntity(_entityID) as EntityAlive;
		if (entityPlayer == null || entityAlive == null)
		{
			return;
		}
		int num = EntityClass.list[entityAlive.entityClass].ExperienceValue;
		num = (int)EffectManager.GetValue(PassiveEffects.ExperienceGain, entityAlive.inventory.holdingItemItemValue, (float)num, entityAlive, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		if (_xpModifier != 1f)
		{
			num = (int)((float)num * _xpModifier + 0.5f);
		}
		if (entityPlayer.IsInParty())
		{
			int num2 = entityPlayer.Party.MemberCountInRange(entityPlayer);
			num = (int)((float)num * (1f - 0.1f * (float)num2));
		}
		if (entityPlayer.Party != null)
		{
			for (int i = 0; i < entityPlayer.Party.MemberList.Count; i++)
			{
				EntityPlayer entityPlayer2 = entityPlayer.Party.MemberList[i];
				if (!(entityPlayer2 == entityPlayer) && Vector3.Distance(entityPlayer.position, entityPlayer2.position) < (float)GameStats.GetInt(EnumGameStats.PartySharedKillRange))
				{
					if (this.m_World.IsLocalPlayer(entityPlayer2.entityId))
					{
						this.SharedKillClient(entityAlive.entityClass, num, null, entityAlive.entityId);
					}
					else
					{
						SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageSharedPartyKill>().Setup(entityAlive.entityClass, num, _killerID, entityAlive.entityId), false, entityPlayer2.entityId, -1, -1, null, 192, false);
					}
				}
			}
		}
	}

	// Token: 0x06007D31 RID: 32049 RVA: 0x0032CA08 File Offset: 0x0032AC08
	public void SharedKillClient(int _entityTypeID, int _xp, EntityPlayerLocal _player = null, int _entityID = -1)
	{
		if (_player == null)
		{
			_player = this.myEntityPlayerLocal;
		}
		if (_player == null)
		{
			return;
		}
		string entityClassName = EntityClass.list[_entityTypeID].entityClassName;
		_xp = _player.Progression.AddLevelExp(_xp, "_xpFromParty", Progression.XPTypes.Kill, true, true);
		_player.bPlayerStatsChanged = true;
		if (_xp > 0)
		{
			GameManager.ShowTooltip(_player, string.Format(Localization.Get("ttPartySharedXPReceived", false), _xp), false, false, 0f);
		}
		QuestEventManager.Current.EntityKilled(_player, (_entityID == -1) ? null : (this.m_World.GetEntity(_entityID) as EntityAlive));
	}

	// Token: 0x06007D32 RID: 32050 RVA: 0x0032CAAA File Offset: 0x0032ACAA
	public IEnumerator ShowExitingGameUICoroutine()
	{
		bool flag = this.windowManager.IsWindowOpen(XUiC_ExitingGame.ID);
		this.windowManager.Open(XUiC_ExitingGame.ID, false, true, true);
		if (flag)
		{
			yield break;
		}
		yield return null;
		yield return null;
		yield break;
	}

	// Token: 0x06007D33 RID: 32051 RVA: 0x0032CABC File Offset: 0x0032ACBC
	public static void ShowTooltipMP(EntityPlayer _player, string _text, string _alertSound = "")
	{
		if (_player is EntityPlayerLocal)
		{
			GameManager.ShowTooltip(_player as EntityPlayerLocal, _text, string.Empty, _alertSound, null, false, false, 0f);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageShowToolbeltMessage>().Setup(_text, _alertSound), false, _player.entityId, -1, -1, null, 192, false);
	}

	// Token: 0x06007D34 RID: 32052 RVA: 0x0032CB1A File Offset: 0x0032AD1A
	public static void ShowTooltip(EntityPlayerLocal _player, string _text, bool _showImmediately = false, bool _pinTooltip = false, float _timeout = 0f)
	{
		GameManager.ShowTooltip(_player, _text, null, null, null, _showImmediately, _pinTooltip, _timeout);
	}

	// Token: 0x06007D35 RID: 32053 RVA: 0x0032CB2A File Offset: 0x0032AD2A
	public static void ShowTooltip(EntityPlayerLocal _player, string _text, string _arg, string _alertSound = null, ToolTipEvent _handler = null, bool _showImmediately = false, bool _pinTooltip = false, float _timeout = 0f)
	{
		GameManager.ShowTooltip(_player, _text, new string[]
		{
			_arg
		}, _alertSound, _handler, _showImmediately, false, _timeout);
	}

	// Token: 0x06007D36 RID: 32054 RVA: 0x0032CB45 File Offset: 0x0032AD45
	public static void ShowTooltip(EntityPlayerLocal _player, string _text, string[] _args, string _alertSound = null, ToolTipEvent _handler = null, bool _showImmediately = false, bool _pinTooltip = false, float _timeout = 0f)
	{
		if (GameManager.IsDedicatedServer || _player == null)
		{
			return;
		}
		XUiC_PopupToolTip.QueueTooltip(LocalPlayerUI.GetUIForPlayer(_player).nguiWindowManager.WindowManager.playerUI.xui, _text, _args, _alertSound, _handler, _showImmediately, _pinTooltip, _timeout);
	}

	// Token: 0x06007D37 RID: 32055 RVA: 0x0032CB82 File Offset: 0x0032AD82
	public static void RemovePinnedTooltip(EntityPlayerLocal _player, string _key)
	{
		if (GameManager.IsDedicatedServer || _player == null)
		{
			return;
		}
		XUiC_PopupToolTip.RemovePinnedTooltip(LocalPlayerUI.GetUIForPlayer(_player).nguiWindowManager.WindowManager.playerUI.xui, _key);
	}

	// Token: 0x06007D38 RID: 32056 RVA: 0x0032CBB5 File Offset: 0x0032ADB5
	public void ClearTooltips(NGUIWindowManager _nguiWindowManager)
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		XUiC_PopupToolTip.ClearTooltips(_nguiWindowManager.WindowManager.playerUI.xui);
	}

	// Token: 0x06007D39 RID: 32057 RVA: 0x0032CBD4 File Offset: 0x0032ADD4
	public void ClearCurrentTooltip(NGUIWindowManager _nguiWindowManager)
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		XUiC_PopupToolTip.ClearCurrentTooltip(_nguiWindowManager.WindowManager.playerUI.xui);
	}

	// Token: 0x06007D3A RID: 32058 RVA: 0x0032CBF3 File Offset: 0x0032ADF3
	public void SetToolTipPause(NGUIWindowManager _nguiWindowManager, bool _isPaused)
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		XUiC_PopupToolTip.SetToolTipPause(_nguiWindowManager.WindowManager.playerUI.xui, _isPaused);
	}

	// Token: 0x06007D3B RID: 32059 RVA: 0x0032CC13 File Offset: 0x0032AE13
	public static void ShowSubtitle(XUi _xui, string speaker, string content, float duration, bool centerAlign = false)
	{
		XUiC_SubtitlesDisplay.DisplaySubtitle(_xui.playerUI, speaker, content, duration, centerAlign);
	}

	// Token: 0x06007D3C RID: 32060 RVA: 0x0032CC25 File Offset: 0x0032AE25
	public static void PlayVideo(string id, bool skippable, XUiC_VideoPlayer.DelegateOnVideoFinished callback = null)
	{
		XUiC_VideoPlayer.PlayVideo(LocalPlayerUI.primaryUI.xui, VideoManager.GetVideoData(id), skippable, callback);
	}

	// Token: 0x06007D3D RID: 32061 RVA: 0x0032CC3E File Offset: 0x0032AE3E
	public static bool IsVideoPlaying()
	{
		return XUiC_VideoPlayer.IsVideoPlaying;
	}

	// Token: 0x06007D3E RID: 32062 RVA: 0x0032CC48 File Offset: 0x0032AE48
	public void ClearTileEntityLockForClient(int _entityId)
	{
		foreach (KeyValuePair<ITileEntity, int> keyValuePair in this.lockedTileEntities)
		{
			if (_entityId == keyValuePair.Value)
			{
				this.lockedTileEntities.Remove(keyValuePair.Key);
				break;
			}
		}
	}

	// Token: 0x06007D3F RID: 32063 RVA: 0x0032CCB4 File Offset: 0x0032AEB4
	public int GetEntityIDForLockedTileEntity(TileEntity te)
	{
		if (this.lockedTileEntities.ContainsKey(te))
		{
			return this.lockedTileEntities[te];
		}
		return -1;
	}

	// Token: 0x06007D40 RID: 32064 RVA: 0x0032CCD2 File Offset: 0x0032AED2
	public IEnumerator ResetWindowsAndLocksByPlayer(int _playerId)
	{
		if (_playerId == -1)
		{
			yield break;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			List<KeyValuePair<ITileEntity, int>> list = new List<KeyValuePair<ITileEntity, int>>();
			foreach (KeyValuePair<ITileEntity, int> item in this.lockedTileEntities)
			{
				if (item.Value == _playerId)
				{
					list.Add(item);
				}
			}
			if (list.Count > 0)
			{
				yield return this.ResetWindowsAndLocks(list);
			}
		}
		yield break;
	}

	// Token: 0x06007D41 RID: 32065 RVA: 0x0032CCE8 File Offset: 0x0032AEE8
	public IEnumerator ResetWindowsAndLocksByChunks(HashSetLong chunks)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			List<KeyValuePair<ITileEntity, int>> list = new List<KeyValuePair<ITileEntity, int>>();
			foreach (long num in chunks)
			{
				foreach (KeyValuePair<ITileEntity, int> item in this.lockedTileEntities)
				{
					Chunk chunk = item.Key.GetChunk();
					if (chunk == null || chunk.Key == num)
					{
						list.Add(item);
					}
				}
			}
			if (list.Count > 0)
			{
				yield return this.ResetWindowsAndLocks(list);
			}
		}
		yield break;
	}

	// Token: 0x06007D42 RID: 32066 RVA: 0x0032CCFE File Offset: 0x0032AEFE
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator ResetWindowsAndLocks(List<KeyValuePair<ITileEntity, int>> _locksToClear)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			HashSet<int> idsToClose = new HashSet<int>();
			foreach (KeyValuePair<ITileEntity, int> keyValuePair in _locksToClear)
			{
				ITileEntity key = keyValuePair.Key;
				if (key.GetChunk() == null)
				{
					Log.Error("[ResetWindowsAndLocks] Failed to find chunk for tile entity. There may be issues unlocking this tile entity");
					this.lockedTileEntities.Remove(key);
				}
				else
				{
					int value = keyValuePair.Value;
					Vector3i blockPos = key.ToWorldPos();
					key.SetUserAccessing(false);
					key.SetModified();
					this.TEUnlockServer(key.GetClrIdx(), blockPos, key.EntityId, true);
					EntityPlayerLocal localPlayerFromID = this.m_World.GetLocalPlayerFromID(value);
					if (localPlayerFromID != null)
					{
						localPlayerFromID.PlayerUI.windowManager.CloseAllOpenWindows(null, false);
					}
					else
					{
						idsToClose.Add(value);
					}
				}
			}
			this.m_World.TickEntitiesFlush();
			yield return null;
			yield return null;
			foreach (int num in idsToClose)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageCloseAllWindows>().Setup(num), true, num, -1, -1, null, 192, false);
			}
			idsToClose = null;
		}
		yield break;
	}

	// Token: 0x06007D43 RID: 32067 RVA: 0x0032CD14 File Offset: 0x0032AF14
	public void TELockServer(int _clrIdx, Vector3i _blockPos, int _lootEntityId, int _entityIdThatOpenedIt, string _customUi = null)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageTELock>().Setup(NetPackageTELock.TELockType.LockServer, _clrIdx, _blockPos, _lootEntityId, _entityIdThatOpenedIt, _customUi, true), false);
			return;
		}
		foreach (KeyValuePair<ITileEntity, int> keyValuePair in this.lockedTileEntities)
		{
			if (_entityIdThatOpenedIt == keyValuePair.Value)
			{
				return;
			}
		}
		TileEntity tileEntity;
		if (_lootEntityId == -1)
		{
			tileEntity = this.m_World.GetTileEntity(_blockPos);
		}
		else
		{
			tileEntity = this.m_World.GetTileEntity(_lootEntityId);
		}
		if (tileEntity == null)
		{
			return;
		}
		if (!this.OpenTileEntityAllowed(_entityIdThatOpenedIt, tileEntity, _customUi))
		{
			return;
		}
		EntityAlive entityAlive;
		if (this.lockedTileEntities.ContainsKey(tileEntity) && (entityAlive = (EntityAlive)this.m_World.GetEntity(this.lockedTileEntities[tileEntity])) != null && !entityAlive.IsDead())
		{
			if (this.m_World.GetEntity(_entityIdThatOpenedIt) as EntityPlayerLocal == null)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageTELock>().Setup(NetPackageTELock.TELockType.DeniedAccess, _clrIdx, _blockPos, _lootEntityId, _entityIdThatOpenedIt, _customUi, true), false, _entityIdThatOpenedIt, -1, -1, null, 192, false);
				return;
			}
			this.TEDeniedAccessClient(_clrIdx, _blockPos, _lootEntityId, _entityIdThatOpenedIt);
			return;
		}
		else
		{
			this.lockedTileEntities[tileEntity] = _entityIdThatOpenedIt;
			if (tileEntity == null)
			{
				return;
			}
			this.OpenTileEntityUi(_entityIdThatOpenedIt, tileEntity, _customUi);
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageTELock>().Setup(NetPackageTELock.TELockType.AccessClient, _clrIdx, _blockPos, _lootEntityId, _entityIdThatOpenedIt, _customUi, true), true, -1, -1, -1, null, 192, false);
		}
	}

	// Token: 0x06007D44 RID: 32068 RVA: 0x0032CEB8 File Offset: 0x0032B0B8
	public void TEUnlockServer(int _clrIdx, Vector3i _blockPos, int _lootEntityId, bool _allowContainerDestroy = true)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageTELock>().Setup(NetPackageTELock.TELockType.UnlockServer, _clrIdx, _blockPos, _lootEntityId, -1, null, _allowContainerDestroy), false);
			return;
		}
		TileEntity tileEntity = null;
		if (_lootEntityId == -1)
		{
			tileEntity = this.m_World.GetTileEntity(_blockPos);
		}
		else
		{
			tileEntity = this.m_World.GetTileEntity(_lootEntityId);
			if (tileEntity == null)
			{
				foreach (KeyValuePair<ITileEntity, int> keyValuePair in this.lockedTileEntities)
				{
					if (keyValuePair.Key.EntityId == _lootEntityId)
					{
						this.lockedTileEntities.Remove(keyValuePair.Key);
						break;
					}
				}
			}
		}
		if (tileEntity == null)
		{
			return;
		}
		this.lockedTileEntities.Remove(tileEntity);
		if (_allowContainerDestroy)
		{
			this.DestroyLootOnClose(tileEntity, _blockPos, _lootEntityId);
		}
	}

	// Token: 0x06007D45 RID: 32069 RVA: 0x0032CF98 File Offset: 0x0032B198
	[PublicizedFrom(EAccessModifier.Private)]
	public void DestroyLootOnClose(TileEntity _te, Vector3i _blockPos, int _lootEntityId)
	{
		ITileEntityLootable tileEntityLootable = _te as ITileEntityLootable;
		if (tileEntityLootable == null)
		{
			return;
		}
		LootContainer.DestroyOnClose destroyOnClose = LootContainer.GetLootContainer(tileEntityLootable.lootListName, true).destroyOnClose;
		if (destroyOnClose != LootContainer.DestroyOnClose.True && (destroyOnClose != LootContainer.DestroyOnClose.Empty || !tileEntityLootable.IsEmpty()))
		{
			return;
		}
		if (tileEntityLootable.bPlayerBackpack)
		{
			if (tileEntityLootable.IsEmpty())
			{
				if (_lootEntityId == -1)
				{
					BlockValue block = this.m_World.GetBlock(_blockPos);
					block.Block.DamageBlock(this.m_World, 0, _blockPos, block, block.Block.MaxDamage, -1, null, false, false);
					return;
				}
				Entity entity = this.m_World.GetEntity(_lootEntityId);
				if (entity != null)
				{
					entity.KillLootContainer();
					return;
				}
			}
		}
		else
		{
			if (_lootEntityId == -1)
			{
				BlockValue block2 = this.m_World.GetBlock(_blockPos);
				this.DropContentOfLootContainerServer(block2, _blockPos, _lootEntityId, null);
				block2.Block.DamageBlock(this.m_World, 0, _blockPos, block2, block2.Block.MaxDamage, -1, null, false, false);
				return;
			}
			this.DropContentOfLootContainerServer(BlockValue.Air, _blockPos, _lootEntityId, null);
			Entity entity2 = this.m_World.GetEntity(_lootEntityId);
			if (entity2 != null)
			{
				entity2.KillLootContainer();
			}
		}
	}

	// Token: 0x06007D46 RID: 32070 RVA: 0x0032D0B0 File Offset: 0x0032B2B0
	public void TEAccessClient(int _clrIdx, Vector3i _blockPos, int _lootEntityId, int _entityIdThatOpenedIt, string _customUi = null)
	{
		if (this.m_World == null)
		{
			return;
		}
		TileEntity tileEntity;
		if (_lootEntityId == -1)
		{
			tileEntity = this.m_World.GetTileEntity(_blockPos);
		}
		else
		{
			tileEntity = this.m_World.GetTileEntity(_lootEntityId);
		}
		if (tileEntity == null)
		{
			return;
		}
		int num = this.myPlayerId;
		this.OpenTileEntityUi(_entityIdThatOpenedIt, tileEntity, _customUi);
	}

	// Token: 0x06007D47 RID: 32071 RVA: 0x0032D0FE File Offset: 0x0032B2FE
	public void FreeAllTileEntityLocks()
	{
		this.lockedTileEntities.Clear();
	}

	// Token: 0x06007D48 RID: 32072 RVA: 0x0032D10C File Offset: 0x0032B30C
	[PublicizedFrom(EAccessModifier.Private)]
	public bool OpenTileEntityAllowed(int _entityIdThatOpenedIt, TileEntity _te, string _customUi)
	{
		ITileEntityLootable te;
		return !_te.TryGetSelfOrFeature(out te) || this.lootContainerCanOpen(te, _entityIdThatOpenedIt);
	}

	// Token: 0x06007D49 RID: 32073 RVA: 0x0032D130 File Offset: 0x0032B330
	[PublicizedFrom(EAccessModifier.Private)]
	public void OpenTileEntityUi(int _entityIdThatOpenedIt, ITileEntity _te, string _customUi)
	{
		EntityPlayerLocal entityPlayerLocal = this.m_World.GetEntity(_entityIdThatOpenedIt) as EntityPlayerLocal;
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(entityPlayerLocal);
		if (!string.IsNullOrEmpty(_customUi))
		{
			if (_customUi == "sign")
			{
				this.signOpened(_te, uiforPlayer);
				return;
			}
			if (_customUi == "lockpick")
			{
				this.lockpickOpened(_te, entityPlayerLocal);
				return;
			}
			if (!(_customUi == "container"))
			{
				return;
			}
			this.lootContainerOpened(_te, uiforPlayer, _entityIdThatOpenedIt);
			return;
		}
		else
		{
			TileEntityLootContainer tileEntityLootContainer = _te as TileEntityLootContainer;
			if (tileEntityLootContainer != null)
			{
				this.lootContainerOpened(tileEntityLootContainer, uiforPlayer, _entityIdThatOpenedIt);
				return;
			}
			TileEntityDewCollector tileEntityDewCollector = _te as TileEntityDewCollector;
			if (tileEntityDewCollector != null)
			{
				this.dewCollectorOpened(tileEntityDewCollector, uiforPlayer, _entityIdThatOpenedIt);
				return;
			}
			TileEntityWorkstation tileEntityWorkstation = _te as TileEntityWorkstation;
			if (tileEntityWorkstation != null)
			{
				this.workstationOpened(tileEntityWorkstation, uiforPlayer);
				return;
			}
			TileEntityTrader tileEntityTrader = _te as TileEntityTrader;
			if (tileEntityTrader != null)
			{
				this.traderOpened(tileEntityTrader, uiforPlayer);
				return;
			}
			if (_te is ITileEntitySignable)
			{
				this.signOpened(_te, uiforPlayer);
				return;
			}
			TileEntityPowerSource tileEntityPowerSource = _te as TileEntityPowerSource;
			if (tileEntityPowerSource != null)
			{
				this.generatorOpened(tileEntityPowerSource, uiforPlayer);
				return;
			}
			TileEntityPoweredTrigger tileEntityPoweredTrigger = _te as TileEntityPoweredTrigger;
			if (tileEntityPoweredTrigger != null)
			{
				this.triggerOpened(tileEntityPoweredTrigger, uiforPlayer);
				return;
			}
			TileEntityPoweredRangedTrap tileEntityPoweredRangedTrap = _te as TileEntityPoweredRangedTrap;
			if (tileEntityPoweredRangedTrap != null)
			{
				this.rangedTrapOpened(tileEntityPoweredRangedTrap, uiforPlayer);
				return;
			}
			TileEntityPowered tileEntityPowered = _te as TileEntityPowered;
			if (tileEntityPowered != null)
			{
				this.poweredGenericOpened(tileEntityPowered, uiforPlayer);
			}
			return;
		}
	}

	// Token: 0x06007D4A RID: 32074 RVA: 0x0032D260 File Offset: 0x0032B460
	public void TEDeniedAccessClient(int _clrIdx, Vector3i _blockPos, int _lootEntityId, int _entityIdThatOpenedIt)
	{
		if (this.m_World == null)
		{
			return;
		}
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(this.m_World.GetEntity(_entityIdThatOpenedIt) as EntityPlayerLocal);
		if (uiforPlayer == null)
		{
			return;
		}
		TileEntity tileEntity;
		if (_lootEntityId == -1)
		{
			tileEntity = this.m_World.GetTileEntity(_blockPos);
		}
		else
		{
			tileEntity = this.m_World.GetTileEntity(_lootEntityId);
		}
		if (tileEntity == null)
		{
			return;
		}
		if (tileEntity is TileEntityTrader)
		{
			if (tileEntity is TileEntityVendingMachine)
			{
				GameManager.ShowTooltip(uiforPlayer.entityPlayer, Localization.Get("ttNoInteractItem", false), string.Empty, "ui_denied", null, false, false, 0f);
			}
			else
			{
				GameManager.ShowTooltip(uiforPlayer.entityPlayer, Localization.Get("ttNoInteractPerson", false), string.Empty, "ui_denied", null, false, false, 0f);
			}
		}
		else
		{
			GameManager.ShowTooltip(uiforPlayer.entityPlayer, Localization.Get("ttNoInteractItem", false), string.Empty, "ui_denied", null, false, false, 0f);
		}
		uiforPlayer.entityPlayer.OverrideFOV = -1f;
		uiforPlayer.xui.Dialog.keepZoomOnClose = false;
	}

	// Token: 0x06007D4B RID: 32075 RVA: 0x0032D36C File Offset: 0x0032B56C
	[PublicizedFrom(EAccessModifier.Private)]
	public void workstationOpened(TileEntityWorkstation _te, LocalPlayerUI _playerUI)
	{
		if (_playerUI != null)
		{
			string blockName = this.m_World.GetBlock(_te.ToWorldPos()).Block.GetBlockName();
			WorkstationData workstationData = CraftingManager.GetWorkstationData(blockName);
			if (workstationData != null)
			{
				string text = (workstationData.WorkstationWindow != "") ? workstationData.WorkstationWindow : string.Format("workstation_{0}", blockName);
				if (_playerUI.windowManager.HasWindow(text))
				{
					((XUiC_WorkstationWindowGroup)((XUiWindowGroup)_playerUI.windowManager.GetWindow(text)).Controller).SetTileEntity(_te);
					_playerUI.windowManager.Open(text, true, false, true);
					return;
				}
				Log.Warning("Window '{0}' not found in XUI!", new object[]
				{
					text
				});
			}
		}
	}

	// Token: 0x06007D4C RID: 32076 RVA: 0x0032D428 File Offset: 0x0032B628
	[PublicizedFrom(EAccessModifier.Private)]
	public void traderOpened(TileEntityTrader _te, LocalPlayerUI _playerUI)
	{
		if (_playerUI != null)
		{
			_playerUI.xui.Trader.TraderTileEntity = _te;
			_playerUI.xui.Trader.Trader = _te.TraderData;
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && GameManager.Instance.traderManager.TraderInventoryRequested(_te.TraderData, XUiM_Player.GetPlayer().entityId))
			{
				_te.SetModified();
			}
			_playerUI.windowManager.CloseAllOpenWindows(null, false);
			_playerUI.windowManager.Open("trader", true, false, true);
		}
	}

	// Token: 0x06007D4D RID: 32077 RVA: 0x0032D4BC File Offset: 0x0032B6BC
	[PublicizedFrom(EAccessModifier.Private)]
	public void signOpened(ITileEntity _te, LocalPlayerUI _playerUI)
	{
		ITileEntitySignable selfOrFeature = _te.GetSelfOrFeature<ITileEntitySignable>();
		if (selfOrFeature == null)
		{
			Log.Error(string.Format("Can not open sign UI for TE {0}", _te));
			return;
		}
		if (_playerUI != null)
		{
			((XUiWindowGroup)_playerUI.windowManager.GetWindow("signMultiline")).Controller.GetChildByType<XUiC_SignWindow>().SetTileEntitySign(selfOrFeature);
			_playerUI.windowManager.Open("signMultiline", true, false, true);
		}
	}

	// Token: 0x06007D4E RID: 32078 RVA: 0x0032D528 File Offset: 0x0032B728
	[PublicizedFrom(EAccessModifier.Private)]
	public void lockpickOpened(ITileEntity _te, EntityPlayerLocal _player)
	{
		ILockPickable selfOrFeature = _te.GetSelfOrFeature<ILockPickable>();
		if (selfOrFeature == null)
		{
			Log.Error(string.Format("Can not open lockpick UI for TE {0}", _te));
			return;
		}
		selfOrFeature.ShowLockpickUi(_player);
	}

	// Token: 0x06007D4F RID: 32079 RVA: 0x0032D558 File Offset: 0x0032B758
	[PublicizedFrom(EAccessModifier.Private)]
	public void generatorOpened(TileEntityPowerSource _te, LocalPlayerUI _playerUI)
	{
		if (_playerUI != null)
		{
			((XUiC_PowerSourceWindowGroup)((XUiWindowGroup)_playerUI.windowManager.GetWindow("powersource")).Controller).TileEntity = _te;
			_playerUI.windowManager.Open("powersource", true, false, true);
		}
	}

	// Token: 0x06007D50 RID: 32080 RVA: 0x0032D5A8 File Offset: 0x0032B7A8
	[PublicizedFrom(EAccessModifier.Private)]
	public void triggerOpened(TileEntityPoweredTrigger _te, LocalPlayerUI _playerUI)
	{
		if (_playerUI != null)
		{
			((XUiC_PowerTriggerWindowGroup)((XUiWindowGroup)_playerUI.windowManager.GetWindow("powertrigger")).Controller).TileEntity = _te;
			_playerUI.windowManager.Open("powertrigger", true, false, true);
		}
	}

	// Token: 0x06007D51 RID: 32081 RVA: 0x0032D5F8 File Offset: 0x0032B7F8
	[PublicizedFrom(EAccessModifier.Private)]
	public void rangedTrapOpened(TileEntityPoweredRangedTrap _te, LocalPlayerUI _playerUI)
	{
		if (_playerUI != null)
		{
			((XUiC_PowerRangedTrapWindowGroup)((XUiWindowGroup)_playerUI.windowManager.GetWindow("powerrangedtrap")).Controller).TileEntity = _te;
			_playerUI.windowManager.Open("powerrangedtrap", true, false, true);
		}
	}

	// Token: 0x06007D52 RID: 32082 RVA: 0x0032D648 File Offset: 0x0032B848
	[PublicizedFrom(EAccessModifier.Private)]
	public void poweredGenericOpened(TileEntityPowered _te, LocalPlayerUI _playerUI)
	{
		if (_playerUI != null && _te.WindowGroupToOpen != string.Empty)
		{
			((XUiC_PoweredGenericWindowGroup)((XUiWindowGroup)_playerUI.windowManager.GetWindow(_te.WindowGroupToOpen)).Controller).TileEntity = _te;
			_playerUI.windowManager.Open(_te.WindowGroupToOpen, true, false, true);
		}
	}

	// Token: 0x06007D53 RID: 32083 RVA: 0x0032D6AC File Offset: 0x0032B8AC
	[PublicizedFrom(EAccessModifier.Private)]
	public bool lootContainerCanOpen(ITileEntityLootable _te, int _entityIdThatOpenedIt)
	{
		if (_te.EntityId != -1)
		{
			Entity entity = this.m_World.GetEntity(_te.EntityId);
			if (entity != null && entity.spawnById > 0 && entity.spawnById != _entityIdThatOpenedIt && !entity.spawnByAllowShare)
			{
				if (TwitchManager.Current.DeniedCrateEvent != "")
				{
					EntityPlayer entityPlayer = this.m_World.GetEntity(_entityIdThatOpenedIt) as EntityPlayer;
					GameEventManager.Current.HandleAction(TwitchManager.Current.DeniedCrateEvent, entityPlayer, entityPlayer, false, "", "", false, true, "", null);
				}
				return false;
			}
		}
		return true;
	}

	// Token: 0x06007D54 RID: 32084 RVA: 0x0032D750 File Offset: 0x0032B950
	[PublicizedFrom(EAccessModifier.Private)]
	public void lootContainerOpened(ITileEntity _te, LocalPlayerUI _playerUI, int _entityIdThatOpenedIt)
	{
		ITileEntityLootable selfOrFeature = _te.GetSelfOrFeature<ITileEntityLootable>();
		if (selfOrFeature == null)
		{
			Log.Error(string.Format("Can not open container UI for TE {0}", _te));
			return;
		}
		FastTags<TagGroup.Global> containerTags = FastTags<TagGroup.Global>.none;
		if (_playerUI != null)
		{
			bool flag = true;
			string lootContainerName = string.Empty;
			if (selfOrFeature.EntityId != -1)
			{
				Entity entity = this.m_World.GetEntity(selfOrFeature.EntityId);
				if (entity != null)
				{
					if (entity.spawnById > 0 && entity.spawnById != _playerUI.entityPlayer.entityId && TwitchManager.Current.StealingCrateEvent != "")
					{
						GameEventManager.Current.HandleAction(TwitchManager.Current.StealingCrateEvent, _playerUI.entityPlayer, _playerUI.entityPlayer, false, "", "", false, true, "", null);
					}
					containerTags = entity.EntityTags;
					lootContainerName = entity.LocalizedEntityName;
					if (entity is EntityVehicle)
					{
						flag = false;
					}
				}
			}
			else
			{
				BlockValue block = this.m_World.GetBlock(selfOrFeature.ToWorldPos());
				containerTags = block.Block.Tags;
				lootContainerName = block.Block.GetLocalizedBlockName();
			}
			if (flag)
			{
				((XUiC_LootWindowGroup)((XUiWindowGroup)_playerUI.windowManager.GetWindow("looting")).Controller).SetTileEntityChest(lootContainerName, selfOrFeature);
				_playerUI.windowManager.Open("looting", true, false, true);
			}
			LootContainer lootContainer = LootContainer.GetLootContainer(selfOrFeature.lootListName, true);
			if (lootContainer != null && _playerUI.entityPlayer != null)
			{
				EntityPlayer entityPlayer = _playerUI.entityPlayer;
				lootContainer.ExecuteBuffActions(selfOrFeature.EntityId, entityPlayer);
				if (lootContainer.OnOpenEvent != "")
				{
					this.m_World.GetEntity(selfOrFeature.EntityId);
					GameEventManager.Current.HandleAction(lootContainer.OnOpenEvent, entityPlayer, entityPlayer, false, selfOrFeature.ToWorldPos(), "", "", false, true, "", null);
				}
			}
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.lootManager.LootContainerOpened(selfOrFeature, _entityIdThatOpenedIt, containerTags);
			selfOrFeature.bTouched = true;
			selfOrFeature.SetModified();
			return;
		}
		this.lootManager.LootContainerOpenedClient(selfOrFeature, _entityIdThatOpenedIt, containerTags);
	}

	// Token: 0x06007D55 RID: 32085 RVA: 0x0032D974 File Offset: 0x0032BB74
	[PublicizedFrom(EAccessModifier.Private)]
	public void dewCollectorOpened(TileEntityDewCollector _te, LocalPlayerUI _playerUI, int _entityIdThatOpenedIt)
	{
		if (_playerUI != null)
		{
			((XUiC_DewCollectorWindowGroup)((XUiWindowGroup)_playerUI.windowManager.GetWindow("dewcollector")).Controller).SetTileEntity(_te);
			_playerUI.windowManager.Open("dewcollector", true, false, true);
		}
	}

	// Token: 0x06007D56 RID: 32086 RVA: 0x0032D9C4 File Offset: 0x0032BBC4
	public void DropContentOfLootContainerServer(BlockValue _bvOld, Vector3i _worldPos, int _lootEntityId, ITileEntityLootable _teOld = null)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			Log.Warning("DropContentOfLootContainerServer can not be called on clients! From:\n" + StackTraceUtility.ExtractStackTrace());
			return;
		}
		string text = "DroppedLootContainer";
		FastTags<TagGroup.Global> none = FastTags<TagGroup.Global>.none;
		ITileEntityLootable tileEntityLootable;
		Vector3 vector;
		if (_lootEntityId == -1)
		{
			tileEntityLootable = (_teOld ?? this.m_World.GetTileEntity(_worldPos).GetSelfOrFeature<ITileEntityLootable>());
			if (tileEntityLootable == null || this.lockedTileEntities.ContainsKey(tileEntityLootable))
			{
				return;
			}
			vector = tileEntityLootable.ToWorldPos().ToVector3() + new Vector3(0.5f, 0.75f, 0.5f);
			if (_bvOld.Block.Properties.Values.ContainsKey("DroppedEntityClass"))
			{
				FastTags<TagGroup.Global> tags = _bvOld.Block.Tags;
				text = _bvOld.Block.Properties.Values["DroppedEntityClass"];
			}
		}
		else
		{
			Entity entity = this.m_World.GetEntity(_lootEntityId);
			if (!entity)
			{
				return;
			}
			FastTags<TagGroup.Global> entityTags = entity.EntityTags;
			vector = entity.GetPosition();
			vector.y += 0.9f;
			if (entity.lootDropProb != 0f)
			{
				EntityLootContainer entityLootContainer = EntityFactory.CreateEntity(EntityClass.list[entity.entityClass].lootDropEntityClass, vector, Vector3.zero) as EntityLootContainer;
				this.m_World.SpawnEntityInWorld(entityLootContainer);
				entityLootContainer.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
				Manager.BroadcastPlay(vector, "zpack_spawn", 0f);
				return;
			}
			tileEntityLootable = this.m_World.GetTileEntity(_lootEntityId).GetSelfOrFeature<ITileEntityLootable>();
			if (tileEntityLootable == null)
			{
				return;
			}
		}
		if (!tileEntityLootable.bTouched)
		{
			this.lootManager.LootContainerOpened(tileEntityLootable, -1, _bvOld.Block.Tags);
		}
		if (!tileEntityLootable.IsEmpty())
		{
			EntityLootContainer entityLootContainer2 = EntityFactory.CreateEntity(text.GetHashCode(), vector, Vector3.zero) as EntityLootContainer;
			if (entityLootContainer2 != null)
			{
				entityLootContainer2.SetContent(ItemStack.Clone(tileEntityLootable.items));
			}
			this.m_World.SpawnEntityInWorld(entityLootContainer2);
		}
		tileEntityLootable.SetEmpty();
	}

	// Token: 0x06007D57 RID: 32087 RVA: 0x0032DBD8 File Offset: 0x0032BDD8
	public EntityLootContainer DropContentInLootContainerServer(int _droppedByID, string _containerEntity, Vector3 _pos, ItemStack[] _items, bool _skipIfEmpty = false)
	{
		if (_skipIfEmpty && ItemStack.IsEmpty(_items))
		{
			return null;
		}
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageDropItemsContainer>().Setup(_droppedByID, _containerEntity, _pos, _items), false);
			return null;
		}
		_pos.y += 0.25f;
		EntityLootContainer entityLootContainer = EntityFactory.CreateEntity(_containerEntity.GetHashCode(), _pos, Vector3.zero) as EntityLootContainer;
		if (entityLootContainer)
		{
			entityLootContainer.SetContent(ItemStack.Clone(_items));
			entityLootContainer.spawnById = _droppedByID;
		}
		this.m_World.SpawnEntityInWorld(entityLootContainer);
		return entityLootContainer;
	}

	// Token: 0x06007D58 RID: 32088 RVA: 0x0032DC6C File Offset: 0x0032BE6C
	public GameStateManager GetGameStateManager()
	{
		return this.gameStateManager;
	}

	// Token: 0x06007D59 RID: 32089 RVA: 0x0032DC74 File Offset: 0x0032BE74
	public void IdMappingReceived(string _name, byte[] _data)
	{
		Log.Out("Received mapping data for: " + _name);
		if (_name == "blocks")
		{
			Block.nameIdMapping = new NameIdMapping(null, Block.MAX_BLOCKS);
			Block.nameIdMapping.LoadFromArray(_data);
			return;
		}
		if (!(_name == "items"))
		{
			Log.Warning("Unknown mapping received for: " + _name);
			return;
		}
		ItemClass.nameIdMapping = new NameIdMapping(null, ItemClass.MAX_ITEMS);
		ItemClass.nameIdMapping.LoadFromArray(_data);
	}

	// Token: 0x06007D5A RID: 32090 RVA: 0x0032DCF7 File Offset: 0x0032BEF7
	public void SetSpawnPointList(SpawnPointList _startPoints)
	{
		base.StartCoroutine(this.setSpawnPointListCo(_startPoints));
	}

	// Token: 0x06007D5B RID: 32091 RVA: 0x0032DD07 File Offset: 0x0032BF07
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator setSpawnPointListCo(SpawnPointList _startPoints)
	{
		while (!this.chunkClusterLoaded && SingletonMonoBehaviour<ConnectionManager>.Instance.IsConnected)
		{
			yield return null;
		}
		if (!this.chunkClusterLoaded)
		{
			yield break;
		}
		this.m_World.ChunkCache.ChunkProvider.SetSpawnPointList(_startPoints);
		yield break;
	}

	// Token: 0x06007D5C RID: 32092 RVA: 0x0032DD20 File Offset: 0x0032BF20
	public void RequestToSpawnEntityServer(EntityCreationData _ecd)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageRequestToSpawnEntity>().Setup(_ecd), false);
			return;
		}
		if (_ecd.entityClass == "fallingTree".GetHashCode())
		{
			for (int i = 0; i < this.m_World.Entities.list.Count; i++)
			{
				if (this.m_World.Entities.list[i] is EntityFallingTree && ((EntityFallingTree)this.m_World.Entities.list[i]).GetBlockPos() == _ecd.blockPos)
				{
					return;
				}
			}
		}
		Entity entity = EntityFactory.CreateEntity(_ecd);
		EntityBackpack entityBackpack = entity as EntityBackpack;
		if (entityBackpack != null)
		{
			foreach (PersistentPlayerData persistentPlayerData in this.persistentPlayers.Players.Values)
			{
				if (persistentPlayerData.EntityId == entityBackpack.RefPlayerId)
				{
					uint timestamp = GameUtils.WorldTimeToTotalMinutes(this.m_World.worldTime);
					persistentPlayerData.AddDroppedBackpack(entity.entityId, new Vector3i(_ecd.pos), timestamp);
					break;
				}
			}
		}
		this.m_World.SpawnEntityInWorld(entity);
	}

	// Token: 0x06007D5D RID: 32093 RVA: 0x0032DE6C File Offset: 0x0032C06C
	[PublicizedFrom(EAccessModifier.Private)]
	public void LocalPlayerInventoryChanged()
	{
		this.countdownSendPlayerInventoryToServer.ResetAndRestart();
	}

	// Token: 0x06007D5E RID: 32094 RVA: 0x0032DE79 File Offset: 0x0032C079
	public void TriggerSendOfLocalPlayerDataFile(float _sendItInSeconds)
	{
		this.countdownSendPlayerDataFileToServer.SetPassedIn(_sendItInSeconds);
	}

	// Token: 0x06007D5F RID: 32095 RVA: 0x0032DE88 File Offset: 0x0032C088
	[PublicizedFrom(EAccessModifier.Private)]
	public void doSendLocalInventory(EntityPlayerLocal _player)
	{
		if (_player == null)
		{
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePlayerInventory>().Setup(_player, this.sendPlayerToolbelt, this.sendPlayerBag, this.sendPlayerEquipment, this.sendDragAndDropItem), false);
		this.sendPlayerToolbelt = false;
		this.sendPlayerBag = false;
		this.sendPlayerEquipment = false;
		this.sendDragAndDropItem = false;
	}

	// Token: 0x06007D60 RID: 32096 RVA: 0x0032DEEC File Offset: 0x0032C0EC
	[PublicizedFrom(EAccessModifier.Private)]
	public void doSendLocalPlayerData(EntityPlayerLocal _player)
	{
		if (_player == null)
		{
			return;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.SaveLocalPlayerData();
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePlayerData>().Setup(_player), false);
		this.sendPlayerToolbelt = false;
		this.sendPlayerBag = false;
		this.sendPlayerEquipment = false;
		this.sendDragAndDropItem = false;
	}

	// Token: 0x06007D61 RID: 32097 RVA: 0x0032DF48 File Offset: 0x0032C148
	public void SetPauseWindowEffects(bool _bOn)
	{
		if (_bOn && !GameModeSurvivalSP.TypeName.Equals(GamePrefs.GetString(EnumGamePrefs.GameMode)))
		{
			foreach (EntityPlayerLocal entityPlayerLocal in this.m_World.GetLocalPlayers())
			{
				if (entityPlayerLocal.AimingGun)
				{
					entityPlayerLocal.AimingGun = false;
				}
			}
		}
	}

	// Token: 0x06007D62 RID: 32098 RVA: 0x0032DFC0 File Offset: 0x0032C1C0
	public static bool ReportUnusedAssets(bool bStart = false)
	{
		if (bStart)
		{
			if (GameManager.materialsBefore == null)
			{
				GameManager.materialsBefore = new List<string>();
			}
			else
			{
				GameManager.materialsBefore.Clear();
			}
			Material[] array = Resources.FindObjectsOfTypeAll<Material>();
			for (int i = 0; i < array.Length; i++)
			{
				GameManager.materialsBefore.Add(array[i].name);
			}
			Resources.UnloadUnusedAssets();
			GC.Collect();
			GameManager.runningAssetsUnused = true;
			GameManager.unusedAssetsTimer = Time.realtimeSinceStartup;
			GameManager.Instance.Pause(true);
		}
		else
		{
			if (GameManager.materialsBefore == null)
			{
				return true;
			}
			if (!GameManager.runningAssetsUnused)
			{
				return true;
			}
			if (Time.realtimeSinceStartup < GameManager.unusedAssetsTimer + 5f)
			{
				return false;
			}
			Material[] array2 = Resources.FindObjectsOfTypeAll<Material>();
			if (GameManager.materialsBefore.Count == array2.Length)
			{
				Log.Out("No unused assets found. ( " + GameManager.materialsBefore.Count.ToString() + " materials found. )");
			}
			else
			{
				Log.Out("Material before: " + GameManager.materialsBefore.Count.ToString());
				Log.Out("Material after: " + array2.Length.ToString());
				string text = "Material Diff: ";
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				for (int j = 0; j < array2.Length; j++)
				{
					int num;
					if (dictionary.TryGetValue(array2[j].name, out num))
					{
						num++;
					}
					else
					{
						dictionary.Add(array2[j].name, 1);
					}
				}
				for (int k = 0; k < GameManager.materialsBefore.Count; k++)
				{
					if (!dictionary.ContainsKey(GameManager.materialsBefore[k]))
					{
						text = text + GameManager.materialsBefore[k] + ", ";
					}
				}
				Log.Out(text);
			}
			GameManager.Instance.Pause(false);
			GameManager.runningAssetsUnused = false;
		}
		return true;
	}

	// Token: 0x06007D63 RID: 32099 RVA: 0x0032E18D File Offset: 0x0032C38D
	public bool IsPaused()
	{
		return this.gamePaused;
	}

	// Token: 0x06007D64 RID: 32100 RVA: 0x0032E198 File Offset: 0x0032C398
	public void Pause(bool _bOn)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsSinglePlayer || GameModeEditWorld.TypeName.Equals(GamePrefs.GetString(EnumGamePrefs.GameMode)))
		{
			_bOn = false;
		}
		this.SetPauseWindowEffects(_bOn);
		if (_bOn)
		{
			GameStats.Set(EnumGameStats.GameState, 2);
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				this.SaveLocalPlayerData();
				this.SaveWorld();
			}
			Time.timeScale = 0f;
			if (this.World.GetPrimaryPlayer() != null)
			{
				this.triggerEffectManager.StopGamepadVibration();
			}
		}
		else
		{
			if (GameStats.GetInt(EnumGameStats.GameState) != 0)
			{
				GameStats.Set(EnumGameStats.GameState, 1);
			}
			Time.timeScale = 1f;
		}
		if (this.gamePaused != _bOn)
		{
			if (_bOn)
			{
				Manager.PauseGameplayAudio();
				EnvironmentAudioManager.Instance.Pause();
				this.m_World.dmsConductor.OnPauseGame();
			}
			else
			{
				Manager.UnPauseGameplayAudio();
				EnvironmentAudioManager.Instance.UnPause();
				this.m_World.dmsConductor.OnUnPauseGame();
			}
		}
		this.gamePaused = _bOn;
	}

	// Token: 0x06007D65 RID: 32101 RVA: 0x00002914 File Offset: 0x00000B14
	public void AddLMPPersistentPlayerData(EntityPlayerLocal _playerEntity)
	{
	}

	// Token: 0x06007D66 RID: 32102 RVA: 0x0032E288 File Offset: 0x0032C488
	public void SetBlockTextureServer(Vector3i _blockPos, BlockFace _blockFace, int _idx, int _playerIdThatChanged, byte _channel = 255)
	{
		this.SetBlockTextureClient(_blockPos, _blockFace, _idx, _channel);
		NetPackageSetBlockTexture package = NetPackageManager.GetPackage<NetPackageSetBlockTexture>().Setup(_blockPos, _blockFace, _idx, GameManager.IsDedicatedServer ? -1 : this.myPlayerId, _channel);
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, false, -1, -1, -1, null, 192, false);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(package, false);
	}

	// Token: 0x06007D67 RID: 32103 RVA: 0x0032E2F8 File Offset: 0x0032C4F8
	public void SetBlockTextureClient(Vector3i _blockPos, BlockFace _blockFace, int _idx, byte _channel)
	{
		DynamicMeshManager.ChunkChanged(_blockPos, -1, 1);
		int num;
		int num2;
		if (_channel == 255)
		{
			num = 0;
			num2 = 0;
		}
		else
		{
			if (_channel >= 1)
			{
				Log.Error(string.Format("Specified texture channel \"{0}\" is out of range of the project channel count of \"{1}\".", _channel, 1));
				return;
			}
			num2 = (int)_channel;
			num = (int)_channel;
		}
		for (int i = num; i <= num2; i++)
		{
			if (_blockFace != BlockFace.None)
			{
				this.m_World.ChunkCache.SetBlockFaceTexture(_blockPos, _blockFace, _idx, i);
			}
			else
			{
				long num3 = (long)_idx;
				long textureFull = num3 | num3 << 8 | num3 << 16 | num3 << 24 | num3 << 32 | num3 << 40;
				this.m_World.ChunkCache.SetTextureFull(_blockPos, textureFull, i);
			}
		}
	}

	// Token: 0x06007D68 RID: 32104 RVA: 0x0032E3A4 File Offset: 0x0032C5A4
	[PublicizedFrom(EAccessModifier.Private)]
	public void handleGlobalActions()
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		if (PlayerActionsGlobal.Instance.Console.WasPressed && !this.m_GUIConsole.isShowing)
		{
			this.windowManager.Open(this.m_GUIConsole, false, false, true);
		}
		if (PlayerActionsGlobal.Instance.Fullscreen.WasPressed)
		{
			Screen.fullScreen = !Screen.fullScreen;
		}
		if (PlayerActionsGlobal.Instance.Screenshot.WasPressed)
		{
			Manager.PlayButtonClick();
			GameUtils.TakeScreenShot(GameUtils.EScreenshotMode.Both, null, 0f, false, 0, 0, InputUtils.ControlKeyPressed);
		}
		if (PlayerActionsGlobal.Instance.DebugScreenshot.WasPressed)
		{
			Manager.PlayButtonClick();
			LocalPlayerUI.primaryUI.windowManager.Open(GUIWindowScreenshotText.ID, false, false, true);
		}
		if (LocalPlayerUI.primaryUI != null)
		{
			IPlatform nativePlatform = PlatformManager.NativePlatform;
			bool? flag;
			if (nativePlatform == null)
			{
				flag = null;
			}
			else
			{
				PlayerActionsLocal primaryPlayer = nativePlatform.Input.PrimaryPlayer;
				if (primaryPlayer == null)
				{
					flag = null;
				}
				else
				{
					PlayerActionsGUI guiactions = primaryPlayer.GUIActions;
					if (guiactions == null)
					{
						flag = null;
					}
					else
					{
						PlayerAction focusSearch = guiactions.FocusSearch;
						flag = ((focusSearch != null) ? new bool?(focusSearch.WasPressed) : null);
					}
				}
			}
			bool? flag2 = flag;
			if (flag2.GetValueOrDefault())
			{
				XUiC_TextInput.SelectCurrentSearchField(LocalPlayerUI.primaryUI);
			}
		}
		LocalPlayerUI uiforPrimaryPlayer = LocalPlayerUI.GetUIForPrimaryPlayer();
		if (uiforPrimaryPlayer != null)
		{
			PlayerActionsLocal playerInput = uiforPrimaryPlayer.playerInput;
			bool? flag3;
			if (playerInput == null)
			{
				flag3 = null;
			}
			else
			{
				PlayerActionsGUI guiactions2 = playerInput.GUIActions;
				if (guiactions2 == null)
				{
					flag3 = null;
				}
				else
				{
					PlayerAction focusSearch2 = guiactions2.FocusSearch;
					flag3 = ((focusSearch2 != null) ? new bool?(focusSearch2.WasPressed) : null);
				}
			}
			bool? flag2 = flag3;
			if (flag2.GetValueOrDefault())
			{
				XUiC_TextInput.SelectCurrentSearchField(uiforPrimaryPlayer);
			}
		}
	}

	// Token: 0x06007D69 RID: 32105 RVA: 0x0032E546 File Offset: 0x0032C746
	public void SetConsoleWindowVisible(bool _b)
	{
		if (_b)
		{
			if (!this.m_GUIConsole.isShowing)
			{
				this.windowManager.Open(this.m_GUIConsole, false, false, true);
				return;
			}
		}
		else
		{
			this.windowManager.Close(this.m_GUIConsole, false);
		}
	}

	// Token: 0x06007D6A RID: 32106 RVA: 0x0032E580 File Offset: 0x0032C780
	public static bool IsSplatMapAvailable()
	{
		string @string = GamePrefs.GetString(EnumGamePrefs.GameWorld);
		return !(@string == "Empty") && !(@string == "Playtesting");
	}

	// Token: 0x17000D13 RID: 3347
	// (get) Token: 0x06007D6B RID: 32107 RVA: 0x0032E5B2 File Offset: 0x0032C7B2
	// (set) Token: 0x06007D6C RID: 32108 RVA: 0x0032E5B9 File Offset: 0x0032C7B9
	public static bool UpdatingRemoteResources { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000D14 RID: 3348
	// (get) Token: 0x06007D6D RID: 32109 RVA: 0x0032E5C1 File Offset: 0x0032C7C1
	// (set) Token: 0x06007D6E RID: 32110 RVA: 0x0032E5C8 File Offset: 0x0032C7C8
	public static bool RemoteResourcesLoaded { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06007D6F RID: 32111 RVA: 0x0032E5D0 File Offset: 0x0032C7D0
	public static void LoadRemoteResources(GameManager.RemoteResourcesCompleteHandler _callback = null)
	{
		if (GameManager.UpdatingRemoteResources)
		{
			return;
		}
		NewsManager.Instance.UpdateNews(false);
		if (BlockedPlayerList.Instance != null)
		{
			GameManager.Instance.StartCoroutine(BlockedPlayerList.Instance.ReadStorageAndResolve());
		}
		DLCTitleStorageManager.Instance.FetchFromSource();
		if (PlatformManager.NativePlatform.User.UserStatus == EUserStatus.LoggedIn)
		{
			GameManager.Instance.StartCoroutine(GameManager.Instance.UpdateRemoteResourcesRoutine(_callback));
			return;
		}
		GameManager.UpdatingRemoteResources = false;
		GameManager.RemoteResourcesLoaded = true;
	}

	// Token: 0x06007D70 RID: 32112 RVA: 0x0032E64A File Offset: 0x0032C84A
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator UpdateRemoteResourcesRoutine(GameManager.RemoteResourcesCompleteHandler _callback)
	{
		IRemoteFileStorage storage = PlatformManager.MultiPlatform.RemoteFileStorage;
		if (storage == null)
		{
			GameManager.RemoteResourcesLoaded = true;
			yield break;
		}
		GameManager.UpdatingRemoteResources = true;
		float readyTime = Time.time;
		while (!storage.IsReady)
		{
			yield return null;
			if (Time.time - readyTime > 3f)
			{
				Log.Warning("Waiting for remote resources timed out");
				GameManager.UpdatingRemoteResources = false;
				GameManager.RemoteResourcesLoaded = true;
				yield break;
			}
		}
		this.retrievingEula = true;
		string filename = string.Format("eula_{0}", Localization.language.ToLower());
		storage.GetFile(filename, new IRemoteFileStorage.FileDownloadCompleteCallback(this.EulaProviderCallback));
		while (this.retrievingEula)
		{
			yield return null;
		}
		if (BacktraceUtils.Initialized)
		{
			this.retrievingBacktraceConfig = true;
			storage.GetFile("backtraceconfig.xml", new IRemoteFileStorage.FileDownloadCompleteCallback(this.BacktraceConfigProviderCallback));
			while (this.retrievingBacktraceConfig)
			{
				yield return null;
			}
		}
		GameManager.UpdatingRemoteResources = false;
		GameManager.RemoteResourcesLoaded = true;
		if (_callback != null)
		{
			_callback();
		}
		yield break;
	}

	// Token: 0x06007D71 RID: 32113 RVA: 0x0032E660 File Offset: 0x0032C860
	[PublicizedFrom(EAccessModifier.Private)]
	public void EulaProviderCallback(IRemoteFileStorage.EFileDownloadResult _result, string _errorDetails, byte[] _data)
	{
		this.retrievingEula = false;
		if (_result != IRemoteFileStorage.EFileDownloadResult.Ok)
		{
			Log.Warning(string.Concat(new string[]
			{
				"Retrieving EULA file failed: ",
				_result.ToStringCached<IRemoteFileStorage.EFileDownloadResult>(),
				" (",
				_errorDetails,
				")"
			}));
			return;
		}
		string retrievedEula;
		if (this.LoadEulaXML(_data, out retrievedEula))
		{
			XUiC_EulaWindow.retrievedEula = retrievedEula;
		}
	}

	// Token: 0x06007D72 RID: 32114 RVA: 0x0032E6C0 File Offset: 0x0032C8C0
	[PublicizedFrom(EAccessModifier.Private)]
	public bool LoadEulaXML(byte[] _data, out string contents)
	{
		contents = "";
		XmlFile xmlFile;
		try
		{
			xmlFile = new XmlFile(_data, true);
		}
		catch (Exception ex)
		{
			Log.Error("Failed loading EULA XML: {0}", new object[]
			{
				ex.Message
			});
			return false;
		}
		XElement root = xmlFile.XmlDoc.Root;
		if (root == null)
		{
			return false;
		}
		int num = int.Parse(root.GetAttribute("version").Trim());
		contents = root.Value;
		if (num > GamePrefs.GetInt(EnumGamePrefs.EulaLatestVersion))
		{
			GamePrefs.Set(EnumGamePrefs.EulaLatestVersion, num);
		}
		return true;
	}

	// Token: 0x06007D73 RID: 32115 RVA: 0x0032E760 File Offset: 0x0032C960
	public static bool HasAcceptedLatestEula()
	{
		return GamePrefs.GetInt(EnumGamePrefs.EulaVersionAccepted) >= GamePrefs.GetInt(EnumGamePrefs.EulaLatestVersion);
	}

	// Token: 0x06007D74 RID: 32116 RVA: 0x0032E77C File Offset: 0x0032C97C
	[PublicizedFrom(EAccessModifier.Private)]
	public void BacktraceConfigProviderCallback(IRemoteFileStorage.EFileDownloadResult _result, string _errorDetails, byte[] _data)
	{
		this.retrievingBacktraceConfig = false;
		if (_result != IRemoteFileStorage.EFileDownloadResult.Ok)
		{
			Log.Warning(string.Concat(new string[]
			{
				"Retrieving Backtrace config file failed: ",
				_result.ToStringCached<IRemoteFileStorage.EFileDownloadResult>(),
				" (",
				_errorDetails,
				")"
			}));
			return;
		}
		try
		{
			BacktraceUtils.UpdateConfig(new XmlFile(_data, true));
		}
		catch (Exception ex)
		{
			Log.Error("Failed loading Backtrace config XML: {0}", new object[]
			{
				ex.Message
			});
		}
	}

	// Token: 0x06007D75 RID: 32117 RVA: 0x0032E804 File Offset: 0x0032CA04
	public bool IsGoreCensored()
	{
		return GameManager.DebugCensorship;
	}

	// Token: 0x17000D15 RID: 3349
	// (get) Token: 0x06007D76 RID: 32118 RVA: 0x0032E810 File Offset: 0x0032CA10
	public int persistentPlayerCount
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.persistentPlayerIds.Count;
		}
	}

	// Token: 0x06007D77 RID: 32119 RVA: 0x0032E820 File Offset: 0x0032CA20
	[PublicizedFrom(EAccessModifier.Private)]
	public void CalculatePersistentPlayerCount(string worldName, string saveName)
	{
		this.persistentPlayerIds = new List<string>();
		string path = GameIO.GetSaveGameDir(worldName, saveName) + "/Player";
		if (!SdDirectory.Exists(path))
		{
			Log.Warning("save folder does not exist");
			return;
		}
		foreach (SdFileSystemInfo sdFileSystemInfo in new SdDirectoryInfo(path).GetFileSystemInfos())
		{
			int length;
			string item;
			if ((length = sdFileSystemInfo.Name.IndexOf('.')) != -1)
			{
				item = sdFileSystemInfo.Name.Substring(0, length);
			}
			else
			{
				item = sdFileSystemInfo.Name;
			}
			if (!this.persistentPlayerIds.Contains(item))
			{
				this.persistentPlayerIds.Add(item);
			}
		}
	}

	// Token: 0x06007D78 RID: 32120 RVA: 0x0032E8C7 File Offset: 0x0032CAC7
	public void OnResolutionChanged(int width, int height)
	{
		this.RefreshRefreshRate();
	}

	// Token: 0x06007D79 RID: 32121 RVA: 0x0032E8D0 File Offset: 0x0032CAD0
	[PublicizedFrom(EAccessModifier.Private)]
	public void RefreshRefreshRate()
	{
		this.currentRefreshRate = (int)PlatformApplicationManager.Application.GetCurrentRefreshRate().value;
	}

	// Token: 0x06007D7A RID: 32122 RVA: 0x0032E8F8 File Offset: 0x0032CAF8
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateFPSCap()
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		int num = this.GameHasStarted ? -1 : this.currentRefreshRate;
		if (Application.targetFrameRate != num)
		{
			Application.targetFrameRate = num;
		}
	}

	// Token: 0x04005F4D RID: 24397
	public static int frameCount;

	// Token: 0x04005F4E RID: 24398
	public static float frameTime;

	// Token: 0x04005F4F RID: 24399
	public static int fixedUpdateCount;

	// Token: 0x04005F50 RID: 24400
	public AudioSource UIAudioSource;

	// Token: 0x04005F51 RID: 24401
	public AudioClip BackgroundMusicClip;

	// Token: 0x04005F52 RID: 24402
	public AudioClip CreditsSongClip;

	// Token: 0x04005F53 RID: 24403
	public bool DebugAILines;

	// Token: 0x04005F54 RID: 24404
	public StabilityViewer stabilityViewer;

	// Token: 0x04005F55 RID: 24405
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public BiomeParticleManager biomeParticleManager;

	// Token: 0x04005F56 RID: 24406
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int cameraCullMask;

	// Token: 0x04005F57 RID: 24407
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bShowBackground = true;

	// Token: 0x04005F58 RID: 24408
	public static bool enableNetworkdPrioritization = true;

	// Token: 0x04005F59 RID: 24409
	public static bool unreliableNetPackets = true;

	// Token: 0x04005F5A RID: 24410
	public static ServerDateTimeResult ServerClockSync;

	// Token: 0x04005F5B RID: 24411
	public NetPackageMetrics netpackageMetrics;

	// Token: 0x04005F5C RID: 24412
	public bool showOpenerMovieOnLoad;

	// Token: 0x04005F5D RID: 24413
	public bool GameHasStarted;

	// Token: 0x04005F5E RID: 24414
	public Color backgroundColor = Color.white;

	// Token: 0x04005F5F RID: 24415
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int currentBackgroundColorChannel;

	// Token: 0x04005F60 RID: 24416
	public static bool bPhysicsActive;

	// Token: 0x04005F61 RID: 24417
	public static bool bTickingActive;

	// Token: 0x04005F62 RID: 24418
	public static bool bSavingActive = true;

	// Token: 0x04005F63 RID: 24419
	public static bool bShowDecorBlocks = true;

	// Token: 0x04005F64 RID: 24420
	public static bool bShowLootBlocks = true;

	// Token: 0x04005F65 RID: 24421
	public static bool bShowPaintables = true;

	// Token: 0x04005F66 RID: 24422
	public static bool bShowUnpaintables = true;

	// Token: 0x04005F67 RID: 24423
	public static bool bShowTerrain = true;

	// Token: 0x04005F68 RID: 24424
	public static bool bVolumeBlocksEditing;

	// Token: 0x04005F69 RID: 24425
	public static bool bHideMainMenuNextTime;

	// Token: 0x04005F6A RID: 24426
	public static bool bRecordNextSession;

	// Token: 0x04005F6B RID: 24427
	public static bool bPlayRecordedSession;

	// Token: 0x04005F6C RID: 24428
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static bool isDedicatedChecked = false;

	// Token: 0x04005F6D RID: 24429
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static bool isDedicated = false;

	// Token: 0x04005F6E RID: 24430
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public World m_World;

	// Token: 0x04005F6F RID: 24431
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool worldCreated;

	// Token: 0x04005F70 RID: 24432
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool chunkClusterLoaded;

	// Token: 0x04005F71 RID: 24433
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int myPlayerId = -1;

	// Token: 0x04005F72 RID: 24434
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityPlayerLocal myEntityPlayerLocal;

	// Token: 0x04005F73 RID: 24435
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public IMapChunkDatabase fowDatabaseForLocalPlayer;

	// Token: 0x04005F74 RID: 24436
	public FPS fps = new FPS(5f);

	// Token: 0x04005F75 RID: 24437
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameObject m_SoundsGameObject;

	// Token: 0x04005F76 RID: 24438
	public GUIWindowConsole m_GUIConsole;

	// Token: 0x04005F77 RID: 24439
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastTimeWorldTickTimeSentToClients;

	// Token: 0x04005F78 RID: 24440
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastTimeGameStateCheckedAndSynced;

	// Token: 0x04005F79 RID: 24441
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastTimeDecoSaved;

	// Token: 0x04005F7A RID: 24442
	public AdminTools adminTools;

	// Token: 0x04005F7B RID: 24443
	public PersistentPlayerList persistentPlayers;

	// Token: 0x04005F7C RID: 24444
	public PersistentPlayerData persistentLocalPlayer;

	// Token: 0x04005F7D RID: 24445
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public GUIWindowManager windowManager;

	// Token: 0x04005F7E RID: 24446
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public NGUIWindowManager nguiWindowManager;

	// Token: 0x04005F7F RID: 24447
	public LootManager lootManager;

	// Token: 0x04005F80 RID: 24448
	public TraderManager traderManager;

	// Token: 0x04005F81 RID: 24449
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int lastDisplayedValueOfTeamTickets;

	// Token: 0x04005F82 RID: 24450
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Dictionary<Vector3i, GameObject> m_PositionSoundMap = new Dictionary<Vector3i, GameObject>();

	// Token: 0x04005F83 RID: 24451
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<GameObject> tileEntitiesMusicToRemove = new List<GameObject>();

	// Token: 0x04005F84 RID: 24452
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int msPassedSinceLastUpdate;

	// Token: 0x04005F85 RID: 24453
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public IBlockTool activeBlockTool;

	// Token: 0x04005F86 RID: 24454
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public IBlockTool blockSelectionTool;

	// Token: 0x04005F87 RID: 24455
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isEditMode;

	// Token: 0x04005F89 RID: 24457
	public bool bCursorVisible = true;

	// Token: 0x04005F8A RID: 24458
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bCursorVisibleOverride;

	// Token: 0x04005F8B RID: 24459
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bCursorVisibleOverrideState;

	// Token: 0x04005F8C RID: 24460
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public DictionarySave<Vector3i, Transform> m_BlockParticles = new DictionarySave<Vector3i, Transform>();

	// Token: 0x04005F8D RID: 24461
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public CountdownTimer countdownCheckBlockParticles = new CountdownTimer(1.1f, true);

	// Token: 0x04005F8E RID: 24462
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public CountdownTimer countdownSendPlayerDataFileToServer = new CountdownTimer(30f, true);

	// Token: 0x04005F8F RID: 24463
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public CountdownTimer countdownSaveLocalPlayerDataFile = new CountdownTimer(30f, true);

	// Token: 0x04005F90 RID: 24464
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float unloadAssetsDuration;

	// Token: 0x04005F91 RID: 24465
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isUnloadAssetsReady;

	// Token: 0x04005F92 RID: 24466
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly MicroStopwatch stopwatchUnloadAssets = new MicroStopwatch(false);

	// Token: 0x04005F93 RID: 24467
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public CountdownTimer countdownSendPlayerInventoryToServer = new CountdownTimer(0.1f, false);

	// Token: 0x04005F94 RID: 24468
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool sendPlayerToolbelt;

	// Token: 0x04005F95 RID: 24469
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool sendPlayerBag;

	// Token: 0x04005F96 RID: 24470
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool sendPlayerEquipment;

	// Token: 0x04005F97 RID: 24471
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool sendDragAndDropItem;

	// Token: 0x04005F98 RID: 24472
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Dictionary<ITileEntity, int> lockedTileEntities = new Dictionary<ITileEntity, int>();

	// Token: 0x04005F99 RID: 24473
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameRandomManager gameRandomManager;

	// Token: 0x04005F9A RID: 24474
	public GameStateManager gameStateManager;

	// Token: 0x04005F9B RID: 24475
	public PrefabLODManager prefabLODManager;

	// Token: 0x04005F9C RID: 24476
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public RespawnType clientRespawnType;

	// Token: 0x04005F9D RID: 24477
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float fpsCountdownTimer = 30f;

	// Token: 0x04005F9E RID: 24478
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float gcCountdownTimer = 120f;

	// Token: 0x04005F9F RID: 24479
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float wsCountdownTimer = 30f;

	// Token: 0x04005FA0 RID: 24480
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float playerPositionsCountdownTimer = 10f;

	// Token: 0x04005FA1 RID: 24481
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public MicroStopwatch swCopyChunks = new MicroStopwatch();

	// Token: 0x04005FA2 RID: 24482
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public MicroStopwatch swUpdateTime = new MicroStopwatch();

	// Token: 0x04005FA3 RID: 24483
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int lastStatsPlayerCount;

	// Token: 0x04005FA4 RID: 24484
	public static long MaxMemoryConsumption;

	// Token: 0x04005FA5 RID: 24485
	public WaitForTargetFPS waitForTargetFPS;

	// Token: 0x04005FA6 RID: 24486
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<GameManager.BlockParticleCreationData> blockParticlesToSpawn = new List<GameManager.BlockParticleCreationData>();

	// Token: 0x04005FA7 RID: 24487
	public static GameManager Instance;

	// Token: 0x04005FAB RID: 24491
	public TriggerEffectManager triggerEffectManager;

	// Token: 0x04005FAC RID: 24492
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int lastPlayerCount;

	// Token: 0x04005FAD RID: 24493
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int currentRefreshRate = 60;

	// Token: 0x04005FAE RID: 24494
	public bool bStaticDataLoadSync;

	// Token: 0x04005FAF RID: 24495
	public bool bStaticDataLoaded;

	// Token: 0x04005FB0 RID: 24496
	public string CurrentLoadAction;

	// Token: 0x04005FB2 RID: 24498
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int lastTimeAbsPosSentToServer;

	// Token: 0x04005FB3 RID: 24499
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bLastWasAttached;

	// Token: 0x04005FB4 RID: 24500
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastTime;

	// Token: 0x04005FB5 RID: 24501
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float timeToClearAllPools = -1f;

	// Token: 0x04005FB6 RID: 24502
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float activityCheck;

	// Token: 0x04005FB7 RID: 24503
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool shuttingDownMultiplayerServices;

	// Token: 0x04005FB8 RID: 24504
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int testing;

	// Token: 0x04005FB9 RID: 24505
	public bool canSpawnPlayer;

	// Token: 0x04005FBA RID: 24506
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isDisconnectingLater;

	// Token: 0x04005FBB RID: 24507
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool allowQuit;

	// Token: 0x04005FBC RID: 24508
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isQuitting;

	// Token: 0x04005FBD RID: 24509
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool firstTimeJoin;

	// Token: 0x04005FBE RID: 24510
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<BlockChangeInfo> tempExplPositions = new List<BlockChangeInfo>();

	// Token: 0x04005FBF RID: 24511
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<GameManager.ExplodeGroup> explodeFallingGroups = new List<GameManager.ExplodeGroup>();

	// Token: 0x04005FC0 RID: 24512
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<ChunkCluster> ccChanged = new List<ChunkCluster>();

	// Token: 0x04005FC1 RID: 24513
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static List<string> materialsBefore = null;

	// Token: 0x04005FC2 RID: 24514
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static float unusedAssetsTimer = 0f;

	// Token: 0x04005FC3 RID: 24515
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static bool runningAssetsUnused = false;

	// Token: 0x04005FC4 RID: 24516
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool gamePaused;

	// Token: 0x04005FC7 RID: 24519
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool retrievingEula;

	// Token: 0x04005FC8 RID: 24520
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool retrievingBacktraceConfig;

	// Token: 0x04005FC9 RID: 24521
	public static bool DebugCensorship;

	// Token: 0x04005FCA RID: 24522
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<string> persistentPlayerIds;

	// Token: 0x02000F50 RID: 3920
	[PublicizedFrom(EAccessModifier.Private)]
	public struct BlockParticleCreationData
	{
		// Token: 0x06007D83 RID: 32131 RVA: 0x0032EB21 File Offset: 0x0032CD21
		public BlockParticleCreationData(Vector3i _blockPos, ParticleEffect _particleEffect)
		{
			this.blockPos = _blockPos;
			this.particleEffect = _particleEffect;
		}

		// Token: 0x04005FCB RID: 24523
		public Vector3i blockPos;

		// Token: 0x04005FCC RID: 24524
		public ParticleEffect particleEffect;
	}

	// Token: 0x02000F51 RID: 3921
	// (Invoke) Token: 0x06007D85 RID: 32133
	public delegate void OnWorldChangedEvent(World _world);

	// Token: 0x02000F52 RID: 3922
	// (Invoke) Token: 0x06007D89 RID: 32137
	public delegate void OnLocalPlayerChangedEvent(EntityPlayerLocal _localPlayer);

	// Token: 0x02000F53 RID: 3923
	[PublicizedFrom(EAccessModifier.Private)]
	public enum EMultiShutReason
	{
		// Token: 0x04005FCE RID: 24526
		AppNoNetwork,
		// Token: 0x04005FCF RID: 24527
		AppSuspended,
		// Token: 0x04005FD0 RID: 24528
		PermMissingMultiplayer,
		// Token: 0x04005FD1 RID: 24529
		PermMissingCrossplay
	}

	// Token: 0x02000F54 RID: 3924
	[PublicizedFrom(EAccessModifier.Private)]
	public class ExplodeGroup
	{
		// Token: 0x04005FD2 RID: 24530
		public Vector3 pos;

		// Token: 0x04005FD3 RID: 24531
		public float radius;

		// Token: 0x04005FD4 RID: 24532
		public int delay;

		// Token: 0x04005FD5 RID: 24533
		public List<GameManager.ExplodeGroup.Falling> fallings = new List<GameManager.ExplodeGroup.Falling>();

		// Token: 0x02000F55 RID: 3925
		public struct Falling
		{
			// Token: 0x04005FD6 RID: 24534
			public Vector3i pos;

			// Token: 0x04005FD7 RID: 24535
			public BlockValue bv;
		}
	}

	// Token: 0x02000F56 RID: 3926
	public class EntityItemLifetimeComparer : IComparer<EntityItem>
	{
		// Token: 0x06007D8D RID: 32141 RVA: 0x0032EB44 File Offset: 0x0032CD44
		public int Compare(EntityItem _obj1, EntityItem _obj2)
		{
			return (int)(_obj2.lifetime - _obj1.lifetime);
		}
	}

	// Token: 0x02000F57 RID: 3927
	// (Invoke) Token: 0x06007D90 RID: 32144
	public delegate void RemoteResourcesCompleteHandler();
}
