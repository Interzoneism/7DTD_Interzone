using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000CC5 RID: 3269
[Preserve]
public class XUiC_InGameDebugMenu : XUiController
{
	// Token: 0x17000A50 RID: 2640
	// (get) Token: 0x06006512 RID: 25874 RVA: 0x0028FB89 File Offset: 0x0028DD89
	public ChunkCluster ChunkCluster0
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			if (GameManager.Instance == null)
			{
				return null;
			}
			if (GameManager.Instance.World != null)
			{
				return GameManager.Instance.World.ChunkClusters[0];
			}
			return null;
		}
	}

	// Token: 0x17000A51 RID: 2641
	// (get) Token: 0x06006513 RID: 25875 RVA: 0x0028FBBD File Offset: 0x0028DDBD
	public bool HasWorld
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return GameManager.Instance.World != null;
		}
	}

	// Token: 0x06006514 RID: 25876 RVA: 0x0028FBCC File Offset: 0x0028DDCC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "decorationsState")
		{
			if (this.HasWorld)
			{
				_value = ((this.ChunkCluster0 != null && this.ChunkCluster0.ChunkProvider.IsDecorationsEnabled()) ? "On" : "Off");
			}
			else
			{
				_value = "N/A";
			}
			return true;
		}
		if (!(_bindingName == "cam_positions_open"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		_value = this.showCamPositions.ToString();
		return true;
	}

	// Token: 0x06006515 RID: 25877 RVA: 0x0028FC4C File Offset: 0x0028DE4C
	public override void Init()
	{
		base.Init();
		XUiC_InGameDebugMenu.ID = base.WindowGroup.ID;
		XUiC_SimpleButton xuiC_SimpleButton = base.GetChildById("btnSuicide") as XUiC_SimpleButton;
		if (xuiC_SimpleButton != null)
		{
			xuiC_SimpleButton.OnPressed += this.BtnSuicide_Controller_OnPress;
		}
		XUiC_TextInput xuiC_TextInput = base.GetChildById("teleportX") as XUiC_TextInput;
		XUiC_TextInput xuiC_TextInput2 = base.GetChildById("teleportZ") as XUiC_TextInput;
		if (xuiC_TextInput != null)
		{
			xuiC_TextInput.OnChangeHandler += XUiC_InGameDebugMenu.TeleportX_OnChangeHandler;
			xuiC_TextInput.OnSubmitHandler += this.Teleport_OnSubmitHandler;
			xuiC_TextInput.SelectOnTab = xuiC_TextInput2;
			xuiC_TextInput.Text = XUiC_InGameDebugMenu.lastTeleportX.ToString();
		}
		if (xuiC_TextInput2 != null)
		{
			xuiC_TextInput2.OnChangeHandler += XUiC_InGameDebugMenu.TeleportZ_OnChangeHandler;
			xuiC_TextInput2.OnSubmitHandler += this.Teleport_OnSubmitHandler;
			xuiC_TextInput2.SelectOnTab = xuiC_TextInput;
			xuiC_TextInput2.Text = XUiC_InGameDebugMenu.lastTeleportZ.ToString();
		}
		XUiC_SimpleButton xuiC_SimpleButton2 = base.GetChildById("btnTeleport") as XUiC_SimpleButton;
		if (xuiC_SimpleButton2 != null)
		{
			xuiC_SimpleButton2.OnPressed += this.BtnTeleport_Controller_OnPress;
		}
		XUiC_SimpleButton xuiC_SimpleButton3 = base.GetChildById("btnRecalcLight") as XUiC_SimpleButton;
		if (xuiC_SimpleButton3 != null)
		{
			xuiC_SimpleButton3.OnPressed += this.BtnRecalcLight_Controller_OnPress;
		}
		XUiC_SimpleButton xuiC_SimpleButton4 = base.GetChildById("btnRecalcStability") as XUiC_SimpleButton;
		if (xuiC_SimpleButton4 != null)
		{
			xuiC_SimpleButton4.OnPressed += this.BtnRecalcStability_Controller_OnPress;
		}
		XUiC_SimpleButton xuiC_SimpleButton5 = base.GetChildById("btnReloadChunks") as XUiC_SimpleButton;
		if (xuiC_SimpleButton5 != null)
		{
			xuiC_SimpleButton5.OnPressed += this.BtnReloadChunks_Controller_OnPress;
		}
		XUiController childById = base.GetChildById("toggleFlyMode");
		this.toggleFlyMode = ((childById != null) ? childById.GetChildByType<XUiC_ToggleButton>() : null);
		if (this.toggleFlyMode != null)
		{
			this.toggleFlyMode.OnValueChanged += this.ToggleFlyMode_OnValueChanged;
		}
		XUiController childById2 = base.GetChildById("toggleGodMode");
		this.toggleGodMode = ((childById2 != null) ? childById2.GetChildByType<XUiC_ToggleButton>() : null);
		if (this.toggleGodMode != null)
		{
			this.toggleGodMode.OnValueChanged += this.ToggleGodMode_OnValueChanged;
		}
		XUiController childById3 = base.GetChildById("toggleNoCollisionMode");
		this.toggleNoCollisionMode = ((childById3 != null) ? childById3.GetChildByType<XUiC_ToggleButton>() : null);
		if (this.toggleNoCollisionMode != null)
		{
			this.toggleNoCollisionMode.OnValueChanged += this.ToggleNoCollisionMode_OnValueChanged;
		}
		XUiController childById4 = base.GetChildById("toggleInvisibleMode");
		this.toggleInvisibleMode = ((childById4 != null) ? childById4.GetChildByType<XUiC_ToggleButton>() : null);
		if (this.toggleInvisibleMode != null)
		{
			this.toggleInvisibleMode.OnValueChanged += this.toggleInvisibleModeOnValueChanged;
		}
		XUiController childById5 = base.GetChildById("togglePhysics");
		this.togglePhysics = ((childById5 != null) ? childById5.GetChildByType<XUiC_ToggleButton>() : null);
		if (this.togglePhysics != null)
		{
			this.togglePhysics.OnValueChanged += this.TogglePhysics_OnValueChanged;
		}
		XUiController childById6 = base.GetChildById("toggleWaterSim");
		this.toggleWaterSim = ((childById6 != null) ? childById6.GetChildByType<XUiC_ToggleButton>() : null);
		if (this.toggleWaterSim != null)
		{
			this.toggleWaterSim.OnValueChanged += this.ToggleWaterSim_OnValueChanged;
		}
		XUiController childById7 = base.GetChildById("toggleDebugShaders");
		this.toggleDebugShaders = ((childById7 != null) ? childById7.GetChildByType<XUiC_ToggleButton>() : null);
		if (this.toggleDebugShaders != null)
		{
			this.toggleDebugShaders.OnValueChanged += this.ToggleDebugShaders_OnValueChanged;
		}
		XUiController childById8 = base.GetChildById("toggleLightPerformance");
		this.toggleLightPerformance = ((childById8 != null) ? childById8.GetChildByType<XUiC_ToggleButton>() : null);
		if (this.toggleLightPerformance != null)
		{
			this.toggleLightPerformance.OnValueChanged += this.ToggleLightPerformance_OnValueChanged;
		}
		XUiController childById9 = base.GetChildById("toggleStabilityGlue");
		this.toggleStabilityGlue = ((childById9 != null) ? childById9.GetChildByType<XUiC_ToggleButton>() : null);
		if (this.toggleStabilityGlue != null)
		{
			this.toggleStabilityGlue.OnValueChanged += this.ToggleStabilityGlue_OnValueChanged;
		}
		XUiC_SimpleButton xuiC_SimpleButton6 = base.GetChildById("btnPlaytest") as XUiC_SimpleButton;
		if (xuiC_SimpleButton6 != null)
		{
			xuiC_SimpleButton6.OnPressed += this.BtnPlaytestOnPressed;
		}
		XUiC_SimpleButton xuiC_SimpleButton7 = base.GetChildById("btnBackToEditor") as XUiC_SimpleButton;
		if (xuiC_SimpleButton7 != null)
		{
			xuiC_SimpleButton7.OnPressed += this.BtnBackToEditorOnPressed;
		}
		XUiController childById10 = base.GetChildById("toggleCamPositions");
		XUiC_ToggleButton xuiC_ToggleButton = (childById10 != null) ? childById10.GetChildByType<XUiC_ToggleButton>() : null;
		if (xuiC_ToggleButton != null)
		{
			xuiC_ToggleButton.OnValueChanged += delegate(XUiC_ToggleButton _, bool _value)
			{
				this.showCamPositions = _value;
				base.RefreshBindings(false);
			};
		}
		XUiC_ComboBoxList<BiomeDefinition.BiomeType> xuiC_ComboBoxList = base.GetChildById("cbxPlaytestBiome") as XUiC_ComboBoxList<BiomeDefinition.BiomeType>;
		if (xuiC_ComboBoxList != null)
		{
			xuiC_ComboBoxList.Elements.AddRange(new BiomeDefinition.BiomeType[]
			{
				BiomeDefinition.BiomeType.Snow,
				BiomeDefinition.BiomeType.PineForest,
				BiomeDefinition.BiomeType.Desert,
				BiomeDefinition.BiomeType.Wasteland,
				BiomeDefinition.BiomeType.burnt_forest
			});
			xuiC_ComboBoxList.Value = (BiomeDefinition.BiomeType)GamePrefs.GetInt(EnumGamePrefs.PlaytestBiome);
			xuiC_ComboBoxList.OnValueChanged += delegate(XUiController _, BiomeDefinition.BiomeType _, BiomeDefinition.BiomeType _newValue)
			{
				GamePrefs.Set(EnumGamePrefs.PlaytestBiome, (int)_newValue);
			};
		}
	}

	// Token: 0x06006516 RID: 25878 RVA: 0x002900DC File Offset: 0x0028E2DC
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnSuicide_Controller_OnPress(XUiController _sender, int _mouseButton)
	{
		if (base.xui.playerUI.entityPlayer == null)
		{
			return;
		}
		base.xui.playerUI.windowManager.Close(base.WindowGroup.ID);
		base.xui.playerUI.entityPlayer.Kill(DamageResponse.New(new DamageSource(EnumDamageSource.Internal, EnumDamageTypes.Suicide), true));
		GameManager.Instance.Pause(false);
	}

	// Token: 0x06006517 RID: 25879 RVA: 0x00290150 File Offset: 0x0028E350
	[PublicizedFrom(EAccessModifier.Private)]
	public static void TeleportX_OnChangeHandler(XUiController _sender, string _text, bool _changeFromCode)
	{
		if (!int.TryParse(_text, out XUiC_InGameDebugMenu.lastTeleportX))
		{
			XUiC_InGameDebugMenu.lastTeleportX = int.MinValue;
		}
	}

	// Token: 0x06006518 RID: 25880 RVA: 0x00290169 File Offset: 0x0028E369
	[PublicizedFrom(EAccessModifier.Private)]
	public static void TeleportZ_OnChangeHandler(XUiController _sender, string _text, bool _changeFromCode)
	{
		if (!int.TryParse(_text, out XUiC_InGameDebugMenu.lastTeleportZ))
		{
			XUiC_InGameDebugMenu.lastTeleportZ = int.MinValue;
		}
	}

	// Token: 0x06006519 RID: 25881 RVA: 0x00290182 File Offset: 0x0028E382
	[PublicizedFrom(EAccessModifier.Private)]
	public void Teleport_OnSubmitHandler(XUiController _sender, string _text)
	{
		this.BtnTeleport_Controller_OnPress(_sender, -1);
	}

	// Token: 0x0600651A RID: 25882 RVA: 0x0029018C File Offset: 0x0028E38C
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnTeleport_Controller_OnPress(XUiController _sender, int _mouseButton)
	{
		if (base.xui.playerUI.entityPlayer == null)
		{
			return;
		}
		if (XUiC_InGameDebugMenu.lastTeleportX == -2147483648 || XUiC_InGameDebugMenu.lastTeleportZ == -2147483648)
		{
			return;
		}
		base.xui.playerUI.windowManager.Close(base.WindowGroup.ID);
		base.xui.playerUI.entityPlayer.Teleport(new Vector3((float)XUiC_InGameDebugMenu.lastTeleportX, 240f, (float)XUiC_InGameDebugMenu.lastTeleportZ), float.MinValue);
	}

	// Token: 0x0600651B RID: 25883 RVA: 0x0029021C File Offset: 0x0028E41C
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnRecalcLight_Controller_OnPress(XUiController _sender, int _mouseButton)
	{
		if (this.ChunkCluster0 == null)
		{
			return;
		}
		LightProcessor lightProcessor = new LightProcessor(GameManager.Instance.World);
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		List<Chunk> chunkArrayCopySync = this.ChunkCluster0.GetChunkArrayCopySync();
		foreach (Chunk chunk in chunkArrayCopySync)
		{
			chunk.ResetLights(0);
			chunk.RefreshSunlight();
		}
		foreach (Chunk chunk2 in chunkArrayCopySync)
		{
			lightProcessor.GenerateSunlight(chunk2, false);
		}
		foreach (Chunk chunk3 in chunkArrayCopySync)
		{
			lightProcessor.GenerateSunlight(chunk3, true);
		}
		foreach (Chunk c in chunkArrayCopySync)
		{
			lightProcessor.LightChunk(c);
		}
		stopwatch.Stop();
		foreach (Chunk chunk4 in chunkArrayCopySync)
		{
			chunk4.NeedsRegeneration = true;
		}
		Log.Out(string.Concat(new string[]
		{
			"#",
			chunkArrayCopySync.Count.ToString(),
			" chunks needed ",
			stopwatch.ElapsedMilliseconds.ToString(),
			"ms"
		}));
	}

	// Token: 0x0600651C RID: 25884 RVA: 0x002903EC File Offset: 0x0028E5EC
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnRecalcStability_Controller_OnPress(XUiController _sender, int _mouseButton)
	{
		ChunkCluster chunkCluster = this.ChunkCluster0;
		if (chunkCluster == null)
		{
			return;
		}
		StabilityInitializer stabilityInitializer = new StabilityInitializer(GameManager.Instance.World);
		MicroStopwatch microStopwatch = new MicroStopwatch();
		foreach (Chunk chunk in chunkCluster.GetChunkArray())
		{
			chunk.ResetStabilityToBottomMost();
		}
		Log.Out("RecalcStability reset in {0}ms", new object[]
		{
			microStopwatch.ElapsedMilliseconds
		});
		foreach (Chunk chunk2 in chunkCluster.GetChunkArray())
		{
			stabilityInitializer.DistributeStability(chunk2);
			chunk2.NeedsRegeneration = true;
		}
		Log.Out("RecalcStability #{0} in {1}ms", new object[]
		{
			chunkCluster.GetChunkArray().Count,
			microStopwatch.ElapsedMilliseconds
		});
	}

	// Token: 0x0600651D RID: 25885 RVA: 0x002904F8 File Offset: 0x0028E6F8
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnReloadChunks_Controller_OnPress(XUiController _sender, int _mouseButton)
	{
		if (this.ChunkCluster0 == null)
		{
			return;
		}
		GameManager.Instance.World.m_ChunkManager.ReloadAllChunks();
		this.ChunkCluster0.ChunkProvider.ReloadAllChunks();
		GameManager.Instance.World.UnloadEntities(GameManager.Instance.World.Entities.list, false);
	}

	// Token: 0x0600651E RID: 25886 RVA: 0x00290556 File Offset: 0x0028E756
	[PublicizedFrom(EAccessModifier.Private)]
	public void ToggleFlyMode_OnValueChanged(XUiC_ToggleButton _sender, bool _newValue)
	{
		if (base.xui.playerUI.entityPlayer != null)
		{
			base.xui.playerUI.entityPlayer.IsFlyMode.Value = _newValue;
		}
	}

	// Token: 0x0600651F RID: 25887 RVA: 0x0029058C File Offset: 0x0028E78C
	[PublicizedFrom(EAccessModifier.Private)]
	public void ToggleGodMode_OnValueChanged(XUiC_ToggleButton _sender, bool _newValue)
	{
		EntityPlayerLocal entityPlayer = base.xui.playerUI.entityPlayer;
		if (entityPlayer == null)
		{
			return;
		}
		PlayerMoveController moveController = entityPlayer.MoveController;
		if (moveController == null)
		{
			return;
		}
		Action action = moveController.toggleGodMode;
		if (action == null)
		{
			return;
		}
		action();
	}

	// Token: 0x06006520 RID: 25888 RVA: 0x002905CE File Offset: 0x0028E7CE
	[PublicizedFrom(EAccessModifier.Private)]
	public void ToggleNoCollisionMode_OnValueChanged(XUiC_ToggleButton _sender, bool _newValue)
	{
		if (base.xui.playerUI.entityPlayer != null)
		{
			base.xui.playerUI.entityPlayer.IsNoCollisionMode.Value = _newValue;
		}
	}

	// Token: 0x06006521 RID: 25889 RVA: 0x00290603 File Offset: 0x0028E803
	[PublicizedFrom(EAccessModifier.Private)]
	public void toggleInvisibleModeOnValueChanged(XUiC_ToggleButton _sender, bool _newValue)
	{
		if (base.xui.playerUI.entityPlayer != null)
		{
			base.xui.playerUI.entityPlayer.IsSpectator = _newValue;
		}
	}

	// Token: 0x06006522 RID: 25890 RVA: 0x00290633 File Offset: 0x0028E833
	[PublicizedFrom(EAccessModifier.Private)]
	public void TogglePhysics_OnValueChanged(XUiC_ToggleButton _sender, bool _newValue)
	{
		GameManager.bPhysicsActive = _newValue;
	}

	// Token: 0x06006523 RID: 25891 RVA: 0x0029063B File Offset: 0x0028E83B
	[PublicizedFrom(EAccessModifier.Private)]
	public void ToggleWaterSim_OnValueChanged(XUiC_ToggleButton _sender, bool _newValue)
	{
		WaterSimulationNative.Instance.SetPaused(!_newValue);
	}

	// Token: 0x06006524 RID: 25892 RVA: 0x0029064B File Offset: 0x0028E84B
	[PublicizedFrom(EAccessModifier.Private)]
	public void ToggleDebugShaders_OnValueChanged(XUiC_ToggleButton _sender, bool _newValue)
	{
		MeshDescription.SetDebugStabilityShader(_newValue);
	}

	// Token: 0x06006525 RID: 25893 RVA: 0x00290653 File Offset: 0x0028E853
	[PublicizedFrom(EAccessModifier.Private)]
	public void ToggleLightPerformance_OnValueChanged(XUiC_ToggleButton _sender, bool _newValue)
	{
		LightViewer.SetEnabled(_newValue);
	}

	// Token: 0x06006526 RID: 25894 RVA: 0x0029065C File Offset: 0x0028E85C
	[PublicizedFrom(EAccessModifier.Private)]
	public void ToggleStabilityGlue_OnValueChanged(XUiC_ToggleButton _sender, bool _newValue)
	{
		if (_newValue)
		{
			if (GameManager.Instance.stabilityViewer == null)
			{
				GameManager.Instance.CreateStabilityViewer();
				GameManager.Instance.stabilityViewer.StartSearch(100);
				return;
			}
		}
		else if (GameManager.Instance.stabilityViewer != null)
		{
			GameManager.Instance.ClearStabilityViewer();
		}
	}

	// Token: 0x06006527 RID: 25895 RVA: 0x002906AA File Offset: 0x0028E8AA
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnPlaytestOnPressed(XUiController _sender, int _mouseButton)
	{
		if (PrefabEditModeManager.Instance.IsActive() && SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			XUiC_SaveDirtyPrefab.Show(base.xui, new Action<XUiC_SaveDirtyPrefab.ESelectedAction>(this.startPlaytest), XUiC_SaveDirtyPrefab.EMode.AskSaveIfDirty);
			return;
		}
		this.startPlaytest(XUiC_SaveDirtyPrefab.ESelectedAction.DontSave);
	}

	// Token: 0x06006528 RID: 25896 RVA: 0x002906E4 File Offset: 0x0028E8E4
	[PublicizedFrom(EAccessModifier.Private)]
	public void startPlaytest(XUiC_SaveDirtyPrefab.ESelectedAction _action)
	{
		if (_action == XUiC_SaveDirtyPrefab.ESelectedAction.Cancel)
		{
			return;
		}
		GameUtils.StartPlaytesting();
	}

	// Token: 0x06006529 RID: 25897 RVA: 0x002906F0 File Offset: 0x0028E8F0
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnBackToEditorOnPressed(XUiController _sender, int _mouseButton)
	{
		GameUtils.StartSinglePrefabEditing();
	}

	// Token: 0x0600652A RID: 25898 RVA: 0x002906F8 File Offset: 0x0028E8F8
	public override void Update(float _dt)
	{
		base.Update(_dt);
		EntityPlayerLocal entityPlayer = base.xui.playerUI.entityPlayer;
		if (this.toggleFlyMode != null)
		{
			this.toggleFlyMode.Value = (entityPlayer != null && entityPlayer.IsFlyMode.Value);
		}
		if (this.toggleGodMode != null)
		{
			this.toggleGodMode.Value = (entityPlayer != null && entityPlayer.IsGodMode.Value);
		}
		if (this.toggleNoCollisionMode != null)
		{
			this.toggleNoCollisionMode.Value = (entityPlayer != null && entityPlayer.IsNoCollisionMode.Value);
		}
		if (this.toggleInvisibleMode != null)
		{
			this.toggleInvisibleMode.Value = (entityPlayer != null && entityPlayer.IsSpectator);
		}
		if (this.togglePhysics != null)
		{
			this.togglePhysics.Value = GameManager.bPhysicsActive;
		}
		if (this.toggleWaterSim != null)
		{
			this.toggleWaterSim.Value = !WaterSimulationNative.Instance.IsPaused;
		}
		if (this.toggleDebugShaders != null)
		{
			this.toggleDebugShaders.Value = MeshDescription.bDebugStability;
		}
		if (this.toggleLightPerformance != null)
		{
			this.toggleLightPerformance.Value = LightViewer.IsEnabled;
		}
		base.RefreshBindings(true);
	}

	// Token: 0x04004C62 RID: 19554
	[PublicizedFrom(EAccessModifier.Private)]
	public static int lastTeleportX;

	// Token: 0x04004C63 RID: 19555
	[PublicizedFrom(EAccessModifier.Private)]
	public static int lastTeleportZ;

	// Token: 0x04004C64 RID: 19556
	public static string ID = "";

	// Token: 0x04004C65 RID: 19557
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ToggleButton togglePhysics;

	// Token: 0x04004C66 RID: 19558
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ToggleButton toggleWaterSim;

	// Token: 0x04004C67 RID: 19559
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ToggleButton toggleDebugShaders;

	// Token: 0x04004C68 RID: 19560
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ToggleButton toggleLightPerformance;

	// Token: 0x04004C69 RID: 19561
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ToggleButton toggleStabilityGlue;

	// Token: 0x04004C6A RID: 19562
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ToggleButton toggleFlyMode;

	// Token: 0x04004C6B RID: 19563
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ToggleButton toggleGodMode;

	// Token: 0x04004C6C RID: 19564
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ToggleButton toggleNoCollisionMode;

	// Token: 0x04004C6D RID: 19565
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ToggleButton toggleInvisibleMode;

	// Token: 0x04004C6E RID: 19566
	[PublicizedFrom(EAccessModifier.Private)]
	public bool showCamPositions;
}
