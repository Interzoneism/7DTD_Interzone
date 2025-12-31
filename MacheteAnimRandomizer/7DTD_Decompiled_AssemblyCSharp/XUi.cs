using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Audio;
using Platform;
using UnityEngine;

// Token: 0x02000F11 RID: 3857
public class XUi : MonoBehaviour
{
	// Token: 0x17000CCD RID: 3277
	// (get) Token: 0x06007AE1 RID: 31457 RVA: 0x0031C903 File Offset: 0x0031AB03
	// (set) Token: 0x06007AE2 RID: 31458 RVA: 0x0031C90B File Offset: 0x0031AB0B
	public AudioClip uiScrollSound { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000CCE RID: 3278
	// (get) Token: 0x06007AE3 RID: 31459 RVA: 0x0031C914 File Offset: 0x0031AB14
	// (set) Token: 0x06007AE4 RID: 31460 RVA: 0x0031C91C File Offset: 0x0031AB1C
	public AudioClip uiClickSound { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000CCF RID: 3279
	// (get) Token: 0x06007AE5 RID: 31461 RVA: 0x0031C925 File Offset: 0x0031AB25
	// (set) Token: 0x06007AE6 RID: 31462 RVA: 0x0031C92D File Offset: 0x0031AB2D
	public AudioClip uiConfirmSound { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000CD0 RID: 3280
	// (get) Token: 0x06007AE7 RID: 31463 RVA: 0x0031C936 File Offset: 0x0031AB36
	// (set) Token: 0x06007AE8 RID: 31464 RVA: 0x0031C93E File Offset: 0x0031AB3E
	public AudioClip uiBackSound { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000CD1 RID: 3281
	// (get) Token: 0x06007AE9 RID: 31465 RVA: 0x0031C947 File Offset: 0x0031AB47
	// (set) Token: 0x06007AEA RID: 31466 RVA: 0x0031C94F File Offset: 0x0031AB4F
	public AudioClip uiSliderSound { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000CD2 RID: 3282
	// (get) Token: 0x06007AEB RID: 31467 RVA: 0x0031C958 File Offset: 0x0031AB58
	// (set) Token: 0x06007AEC RID: 31468 RVA: 0x0031C960 File Offset: 0x0031AB60
	public bool isReady
	{
		get
		{
			return this.mIsReady;
		}
		set
		{
			this.mIsReady = value;
			if (this.mIsReady && this.OnBuilt != null)
			{
				this.OnBuilt();
			}
		}
	}

	// Token: 0x17000CD3 RID: 3283
	// (get) Token: 0x06007AED RID: 31469 RVA: 0x0031C984 File Offset: 0x0031AB84
	// (set) Token: 0x06007AEE RID: 31470 RVA: 0x0031C98C File Offset: 0x0031AB8C
	public bool isMinimal { get; set; }

	// Token: 0x140000E1 RID: 225
	// (add) Token: 0x06007AEF RID: 31471 RVA: 0x0031C998 File Offset: 0x0031AB98
	// (remove) Token: 0x06007AF0 RID: 31472 RVA: 0x0031C9D0 File Offset: 0x0031ABD0
	public event Action OnShutdown;

	// Token: 0x140000E2 RID: 226
	// (add) Token: 0x06007AF1 RID: 31473 RVA: 0x0031CA08 File Offset: 0x0031AC08
	// (remove) Token: 0x06007AF2 RID: 31474 RVA: 0x0031CA40 File Offset: 0x0031AC40
	public event Action OnBuilt;

	// Token: 0x17000CD4 RID: 3284
	// (get) Token: 0x06007AF3 RID: 31475 RVA: 0x0031CA75 File Offset: 0x0031AC75
	public bool GlobalOpacityChanged
	{
		get
		{
			return this.oldBackgroundGlobalOpacity != this.BackgroundGlobalOpacity || this.oldForegroundGlobalOpacity != this.ForegroundGlobalOpacity;
		}
	}

	// Token: 0x17000CD5 RID: 3285
	// (get) Token: 0x06007AF4 RID: 31476 RVA: 0x0031CA98 File Offset: 0x0031AC98
	public Transform StackPanelTransform
	{
		get
		{
			return this.stackPanelRoot;
		}
	}

	// Token: 0x06007AF5 RID: 31477 RVA: 0x0031CAA0 File Offset: 0x0031ACA0
	public static XUi Instantiate(LocalPlayerUI playerUI, GameObject xuiPrefab = null)
	{
		if (GameManager.IsDedicatedServer)
		{
			return null;
		}
		Log.Out("[XUi] Instantiating XUi from {0} prefab.", new object[]
		{
			(xuiPrefab != null) ? xuiPrefab.name : ((XUi.defaultPrefab != null) ? XUi.defaultPrefab.name : "default")
		});
		MicroStopwatch microStopwatch = new MicroStopwatch(true);
		Transform transform = UnityEngine.Object.Instantiate<GameObject>(xuiPrefab ? xuiPrefab : ((XUi.defaultPrefab != null) ? XUi.defaultPrefab : Resources.Load<GameObject>("Prefabs/XUi"))).transform;
		transform.name = transform.name.Replace("(Clone)", "").Replace("_Full", "");
		transform.parent = playerUI.transform.Find("NGUI Camera");
		XUi.UIRoot = UnityEngine.Object.FindObjectOfType<UIRoot>();
		XUi component = transform.GetComponent<XUi>();
		component.SetScale(-1f);
		component.gameObject.SetActive(true);
		component.Init(XUi.ID++);
		Log.Out("[XUi] XUi instantiation completed in {0} ms", new object[]
		{
			microStopwatch.ElapsedMilliseconds
		});
		return component;
	}

	// Token: 0x06007AF6 RID: 31478 RVA: 0x0031CBCB File Offset: 0x0031ADCB
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnGamePrefChanged(EnumGamePrefs _obj)
	{
		if (_obj == EnumGamePrefs.OptionsScreenBoundsValue)
		{
			this.SetScale(-1f);
			this.UpdateAnchors();
			this.RecenterWindowGroup(null, true);
			return;
		}
		if (_obj == EnumGamePrefs.OptionsHudSize)
		{
			this.SetScale(-1f);
		}
	}

	// Token: 0x06007AF7 RID: 31479 RVA: 0x0031CC02 File Offset: 0x0031AE02
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnResolutionChanged(int _arg1, int _arg2)
	{
		ThreadManager.StartCoroutine(this.delayedScaleUpdate());
	}

	// Token: 0x06007AF8 RID: 31480 RVA: 0x0031CC10 File Offset: 0x0031AE10
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator delayedScaleUpdate()
	{
		yield return null;
		this.SetScale(-1f);
		yield break;
	}

	// Token: 0x06007AF9 RID: 31481 RVA: 0x0031CC1F File Offset: 0x0031AE1F
	public void LateInitialize()
	{
		this.LateInit();
	}

	// Token: 0x06007AFA RID: 31482 RVA: 0x0031CC27 File Offset: 0x0031AE27
	public void Shutdown(bool _destroyImmediate = false)
	{
		if (this.OnShutdown != null)
		{
			this.OnShutdown();
		}
		this.Cleanup(_destroyImmediate);
	}

	// Token: 0x06007AFB RID: 31483 RVA: 0x0031CC43 File Offset: 0x0031AE43
	public static void Reload(LocalPlayerUI _playerUI)
	{
		if (_playerUI.xui != null)
		{
			_playerUI.xui.Shutdown(true);
		}
		XUi.SetXmlsForUi(_playerUI);
		XUi xui = XUi.Instantiate(_playerUI, null);
		xui.Load(null, false);
		xui.SetDataConnections();
	}

	// Token: 0x06007AFC RID: 31484 RVA: 0x0031CC7C File Offset: 0x0031AE7C
	public static void ReloadWindow(LocalPlayerUI _playerUI, string _windowGroupName)
	{
		if (_playerUI.xui == null)
		{
			Log.Error("Can not reload single window, XUi not instantiated");
		}
		for (int i = 0; i < _playerUI.xui.WindowGroups.Count; i++)
		{
			XUiWindowGroup xuiWindowGroup = _playerUI.xui.WindowGroups[i];
			if (xuiWindowGroup.ID == _windowGroupName)
			{
				xuiWindowGroup.Controller.Cleanup();
				_playerUI.windowManager.Remove(xuiWindowGroup.ID);
				for (int j = 0; j < xuiWindowGroup.Controller.Children.Count; j++)
				{
					UnityEngine.Object.DestroyImmediate(xuiWindowGroup.Controller.Children[j].ViewComponent.UiTransform.gameObject);
				}
				_playerUI.xui.WindowGroups.RemoveAt(i);
				break;
			}
		}
		XUi.SetXmlsForUi(_playerUI);
		_playerUI.xui.Load(new List<string>
		{
			_windowGroupName
		}, false);
		_playerUI.xui.isReady = true;
	}

	// Token: 0x06007AFD RID: 31485 RVA: 0x0031CD7C File Offset: 0x0031AF7C
	public static void SetXmlsForUi(LocalPlayerUI _playerUI)
	{
		XUiFromXml.ClearData();
		if (!_playerUI.isPrimaryUI)
		{
			ThreadManager.RunCoroutineSync(XUi.PatchAndLoadXuiXml("XUi_Common/styles"));
			ThreadManager.RunCoroutineSync(XUi.PatchAndLoadXuiXml("XUi_Common/controls"));
			ThreadManager.RunCoroutineSync(XUi.PatchAndLoadXuiXml("XUi/styles"));
			ThreadManager.RunCoroutineSync(XUi.PatchAndLoadXuiXml("XUi/controls"));
			ThreadManager.RunCoroutineSync(XUi.PatchAndLoadXuiXml("XUi/windows"));
			ThreadManager.RunCoroutineSync(XUi.PatchAndLoadXuiXml("XUi/xui"));
			return;
		}
		ThreadManager.RunCoroutineSync(XUi.PatchAndLoadXuiXml("XUi_Common/styles"));
		ThreadManager.RunCoroutineSync(XUi.PatchAndLoadXuiXml("XUi_Common/controls"));
		ThreadManager.RunCoroutineSync(XUi.PatchAndLoadXuiXml("XUi_Menu/styles"));
		ThreadManager.RunCoroutineSync(XUi.PatchAndLoadXuiXml("XUi_Menu/controls"));
		ThreadManager.RunCoroutineSync(XUi.PatchAndLoadXuiXml("XUi_Menu/windows"));
		ThreadManager.RunCoroutineSync(XUi.PatchAndLoadXuiXml("XUi_Menu/xui"));
	}

	// Token: 0x06007AFE RID: 31486 RVA: 0x0031CE4B File Offset: 0x0031B04B
	public static IEnumerator PatchAndLoadXuiXml(string _relPathXuiFile)
	{
		MicroStopwatch timer = null;
		bool coroutineHadException = false;
		XmlFile xmlFile = null;
		yield return XmlPatcher.LoadAndPatchConfig(_relPathXuiFile, delegate(XmlFile _file)
		{
			xmlFile = _file;
		});
		yield return XmlPatcher.ApplyConditionalXmlBlocks(_relPathXuiFile, xmlFile, timer, XmlPatcher.EEvaluator.Host, delegate
		{
			coroutineHadException = true;
		});
		if (coroutineHadException)
		{
			yield break;
		}
		yield return XmlPatcher.ApplyConditionalXmlBlocks(_relPathXuiFile, xmlFile, timer, XmlPatcher.EEvaluator.Client, delegate
		{
			coroutineHadException = true;
		});
		if (coroutineHadException)
		{
			yield break;
		}
		yield return XUiFromXml.Load(xmlFile);
		yield break;
	}

	// Token: 0x06007AFF RID: 31487 RVA: 0x0031CE5C File Offset: 0x0031B05C
	public void SetDataConnections()
	{
		if (this.playerUI.entityPlayer != null)
		{
			this.PlayerInventory = new XUiM_PlayerInventory(this, this.playerUI.entityPlayer);
			this.PlayerEquipment = new XUiM_PlayerEquipment(this, this.playerUI.entityPlayer);
		}
	}

	// Token: 0x06007B00 RID: 31488 RVA: 0x0031CEAC File Offset: 0x0031B0AC
	public float GetPixelRatioFactor()
	{
		if (XUi.pixelRatioFactor == 0f || Screen.height != XUi.lastScreenHeight)
		{
			float @float = GamePrefs.GetFloat(EnumGamePrefs.OptionsScreenBoundsValue);
			float activeUiScale = GameOptionsManager.GetActiveUiScale();
			XUi.pixelRatioFactor = XUi.UIRoot.pixelSizeAdjustment / this.xuiGlobalScaling / @float / activeUiScale;
			XUi.lastScreenHeight = Screen.height;
		}
		return XUi.pixelRatioFactor;
	}

	// Token: 0x06007B01 RID: 31489 RVA: 0x0031CF0B File Offset: 0x0031B10B
	public Vector2i GetXUiScreenSize()
	{
		return new Vector2i(new Vector2((float)this.playerUI.camera.pixelWidth, (float)this.playerUI.camera.pixelHeight) * this.GetPixelRatioFactor());
	}

	// Token: 0x06007B02 RID: 31490 RVA: 0x0031CF44 File Offset: 0x0031B144
	public Vector2i GetMouseXUIPosition()
	{
		Vector3 v = this.playerUI.CursorController.GetLocalScreenPosition();
		return this.TranslateScreenVectorToXuiVector(v);
	}

	// Token: 0x06007B03 RID: 31491 RVA: 0x0031CF74 File Offset: 0x0031B174
	public Vector2i TranslateScreenVectorToXuiVector(Vector2 _screenSpaceVector)
	{
		_screenSpaceVector.x -= (float)this.playerUI.camera.pixelWidth / 2f;
		_screenSpaceVector.y -= (float)this.playerUI.camera.pixelHeight / 2f;
		return new Vector2i(_screenSpaceVector * this.GetPixelRatioFactor());
	}

	// Token: 0x06007B04 RID: 31492 RVA: 0x0031CFD8 File Offset: 0x0031B1D8
	public Vector3 TranslateScreenVectorToXuiVector(Vector3 _screenSpaceVector)
	{
		_screenSpaceVector.x -= (float)this.playerUI.camera.pixelWidth / 2f;
		_screenSpaceVector.y -= (float)this.playerUI.camera.pixelHeight / 2f;
		return _screenSpaceVector * this.GetPixelRatioFactor();
	}

	// Token: 0x06007B05 RID: 31493 RVA: 0x0031D035 File Offset: 0x0031B235
	public static bool IsGameRunning()
	{
		return GameManager.Instance != null && GameManager.Instance.World != null;
	}

	// Token: 0x17000CD6 RID: 3286
	// (get) Token: 0x06007B06 RID: 31494 RVA: 0x0031D053 File Offset: 0x0031B253
	public LocalPlayerUI playerUI
	{
		get
		{
			if (this.mPlayerUI == null)
			{
				this.mPlayerUI = base.GetComponentInParent<LocalPlayerUI>();
			}
			return this.mPlayerUI;
		}
	}

	// Token: 0x17000CD7 RID: 3287
	// (get) Token: 0x06007B07 RID: 31495 RVA: 0x0031D078 File Offset: 0x0031B278
	public XUiC_GamepadCalloutWindow calloutWindow
	{
		get
		{
			XUiController xuiController = this.FindWindowGroupByName("CalloutGroup");
			if (xuiController != null)
			{
				this.mCalloutWindow = xuiController.GetChildByType<XUiC_GamepadCalloutWindow>();
			}
			return this.mCalloutWindow;
		}
	}

	// Token: 0x17000CD8 RID: 3288
	// (get) Token: 0x06007B08 RID: 31496 RVA: 0x0031D0A6 File Offset: 0x0031B2A6
	// (set) Token: 0x06007B09 RID: 31497 RVA: 0x0031D0AE File Offset: 0x0031B2AE
	public XUiC_DragAndDropWindow dragAndDrop { get; set; }

	// Token: 0x17000CD9 RID: 3289
	// (get) Token: 0x06007B0A RID: 31498 RVA: 0x0031D0B7 File Offset: 0x0031B2B7
	// (set) Token: 0x06007B0B RID: 31499 RVA: 0x0031D0BF File Offset: 0x0031B2BF
	public XUiC_OnScreenIcons onScreenIcons { get; set; }

	// Token: 0x17000CDA RID: 3290
	// (get) Token: 0x06007B0C RID: 31500 RVA: 0x0031D0C8 File Offset: 0x0031B2C8
	// (set) Token: 0x06007B0D RID: 31501 RVA: 0x0031D0D0 File Offset: 0x0031B2D0
	public XUiC_ToolTip currentToolTip { get; set; }

	// Token: 0x17000CDB RID: 3291
	// (get) Token: 0x06007B0E RID: 31502 RVA: 0x0031D0D9 File Offset: 0x0031B2D9
	// (set) Token: 0x06007B0F RID: 31503 RVA: 0x0031D0E1 File Offset: 0x0031B2E1
	public XUiC_PopupMenu currentPopupMenu { get; set; }

	// Token: 0x17000CDC RID: 3292
	// (get) Token: 0x06007B10 RID: 31504 RVA: 0x0031D0EA File Offset: 0x0031B2EA
	// (set) Token: 0x06007B11 RID: 31505 RVA: 0x0031D0F2 File Offset: 0x0031B2F2
	public XUiC_SaveIndicator saveIndicator { get; set; }

	// Token: 0x17000CDD RID: 3293
	// (get) Token: 0x06007B12 RID: 31506 RVA: 0x0031D0FB File Offset: 0x0031B2FB
	// (set) Token: 0x06007B13 RID: 31507 RVA: 0x0031D103 File Offset: 0x0031B303
	public XUiC_BasePartStack basePartStack { get; set; }

	// Token: 0x17000CDE RID: 3294
	// (get) Token: 0x06007B14 RID: 31508 RVA: 0x0031D10C File Offset: 0x0031B30C
	// (set) Token: 0x06007B15 RID: 31509 RVA: 0x0031D114 File Offset: 0x0031B314
	public XUiC_EquipmentStack equipmentStack { get; set; }

	// Token: 0x17000CDF RID: 3295
	// (get) Token: 0x06007B16 RID: 31510 RVA: 0x0031D11D File Offset: 0x0031B31D
	// (set) Token: 0x06007B17 RID: 31511 RVA: 0x0031D125 File Offset: 0x0031B325
	public XUiC_ItemStack itemStack { get; set; }

	// Token: 0x17000CE0 RID: 3296
	// (get) Token: 0x06007B18 RID: 31512 RVA: 0x0031D12E File Offset: 0x0031B32E
	// (set) Token: 0x06007B19 RID: 31513 RVA: 0x0031D136 File Offset: 0x0031B336
	public XUiC_RecipeEntry recipeEntry { get; set; }

	// Token: 0x17000CE1 RID: 3297
	// (get) Token: 0x06007B1A RID: 31514 RVA: 0x0031D13F File Offset: 0x0031B33F
	// (set) Token: 0x06007B1B RID: 31515 RVA: 0x0031D147 File Offset: 0x0031B347
	public ProgressionValue selectedSkill { get; set; }

	// Token: 0x17000CE2 RID: 3298
	// (get) Token: 0x06007B1C RID: 31516 RVA: 0x0031D150 File Offset: 0x0031B350
	// (set) Token: 0x06007B1D RID: 31517 RVA: 0x0031D158 File Offset: 0x0031B358
	public ITileEntityLootable lootContainer { get; set; }

	// Token: 0x17000CE3 RID: 3299
	// (get) Token: 0x06007B1E RID: 31518 RVA: 0x0031D161 File Offset: 0x0031B361
	// (set) Token: 0x06007B1F RID: 31519 RVA: 0x0031D169 File Offset: 0x0031B369
	public string currentWorkstation { get; set; }

	// Token: 0x17000CE4 RID: 3300
	// (get) Token: 0x06007B20 RID: 31520 RVA: 0x0031D172 File Offset: 0x0031B372
	// (set) Token: 0x06007B21 RID: 31521 RVA: 0x0031D17A File Offset: 0x0031B37A
	public EntityVehicle vehicle { get; set; }

	// Token: 0x17000CE5 RID: 3301
	// (get) Token: 0x06007B22 RID: 31522 RVA: 0x0031D183 File Offset: 0x0031B383
	// (set) Token: 0x06007B23 RID: 31523 RVA: 0x0031D18B File Offset: 0x0031B38B
	public XUiC_WorkstationToolGrid currentWorkstationToolGrid { get; set; }

	// Token: 0x17000CE6 RID: 3302
	// (get) Token: 0x06007B24 RID: 31524 RVA: 0x0031D194 File Offset: 0x0031B394
	// (set) Token: 0x06007B25 RID: 31525 RVA: 0x0031D19C File Offset: 0x0031B39C
	public XUiC_WorkstationFuelGrid currentWorkstationFuelGrid { get; set; }

	// Token: 0x17000CE7 RID: 3303
	// (get) Token: 0x06007B26 RID: 31526 RVA: 0x0031D1A5 File Offset: 0x0031B3A5
	// (set) Token: 0x06007B27 RID: 31527 RVA: 0x0031D1AD File Offset: 0x0031B3AD
	public XUiC_WorkstationInputGrid currentWorkstationInputGrid { get; set; }

	// Token: 0x17000CE8 RID: 3304
	// (get) Token: 0x06007B28 RID: 31528 RVA: 0x0031D1B6 File Offset: 0x0031B3B6
	// (set) Token: 0x06007B29 RID: 31529 RVA: 0x0031D1BE File Offset: 0x0031B3BE
	public XUiC_DewCollectorModGrid currentDewCollectorModGrid { get; set; }

	// Token: 0x17000CE9 RID: 3305
	// (get) Token: 0x06007B2A RID: 31530 RVA: 0x0031D1C7 File Offset: 0x0031B3C7
	// (set) Token: 0x06007B2B RID: 31531 RVA: 0x0031D1CF File Offset: 0x0031B3CF
	public XUiC_CombineGrid currentCombineGrid { get; set; }

	// Token: 0x17000CEA RID: 3306
	// (get) Token: 0x06007B2C RID: 31532 RVA: 0x0031D1D8 File Offset: 0x0031B3D8
	// (set) Token: 0x06007B2D RID: 31533 RVA: 0x0031D1E0 File Offset: 0x0031B3E0
	public XUiC_PowerSourceSlots powerSourceSlots { get; set; }

	// Token: 0x17000CEB RID: 3307
	// (get) Token: 0x06007B2E RID: 31534 RVA: 0x0031D1E9 File Offset: 0x0031B3E9
	// (set) Token: 0x06007B2F RID: 31535 RVA: 0x0031D1F1 File Offset: 0x0031B3F1
	public XUiC_PowerRangedAmmoSlots powerAmmoSlots { get; set; }

	// Token: 0x17000CEC RID: 3308
	// (get) Token: 0x06007B30 RID: 31536 RVA: 0x0031D1FA File Offset: 0x0031B3FA
	// (set) Token: 0x06007B31 RID: 31537 RVA: 0x0031D202 File Offset: 0x0031B402
	public XUiC_SelectableEntry currentSelectedEntry { get; set; }

	// Token: 0x17000CED RID: 3309
	// (get) Token: 0x06007B32 RID: 31538 RVA: 0x0031D20B File Offset: 0x0031B40B
	// (set) Token: 0x06007B33 RID: 31539 RVA: 0x0031D213 File Offset: 0x0031B413
	public bool isUsingItemActionEntryUse { get; set; }

	// Token: 0x17000CEE RID: 3310
	// (get) Token: 0x06007B34 RID: 31540 RVA: 0x0031D21C File Offset: 0x0031B41C
	// (set) Token: 0x06007B35 RID: 31541 RVA: 0x0031D224 File Offset: 0x0031B424
	public bool isUsingItemActionEntryPromptComplete { get; set; }

	// Token: 0x17000CEF RID: 3311
	// (get) Token: 0x06007B36 RID: 31542 RVA: 0x0031D22D File Offset: 0x0031B42D
	// (set) Token: 0x06007B37 RID: 31543 RVA: 0x0031D234 File Offset: 0x0031B434
	public static bool InGameMenuOpen
	{
		get
		{
			return XUi._inGameMenuOpen;
		}
		set
		{
			XUi._inGameMenuOpen = value;
			World world = GameManager.Instance.World;
			if (world == null)
			{
				return;
			}
			EntityPlayerLocal primaryPlayer = world.GetPrimaryPlayer();
			if (primaryPlayer == null)
			{
				return;
			}
			LocalPlayerUI playerUI = primaryPlayer.PlayerUI;
			if (playerUI == null)
			{
				return;
			}
			NGUIWindowManager nguiWindowManager = playerUI.nguiWindowManager;
			if (nguiWindowManager == null)
			{
				return;
			}
			GameManager.Instance.SetToolTipPause(nguiWindowManager, value);
		}
	}

	// Token: 0x06007B38 RID: 31544 RVA: 0x0031D295 File Offset: 0x0031B495
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDestroy()
	{
		GameOptionsManager.ResolutionChanged -= this.OnResolutionChanged;
		GamePrefs.OnGamePrefChanged -= this.OnGamePrefChanged;
		LocalPlayerManager.OnLocalPlayersChanged -= this.HandleLocalPlayersChanged;
		this.Shutdown(false);
	}

	// Token: 0x06007B39 RID: 31545 RVA: 0x0031D2D4 File Offset: 0x0031B4D4
	public void SetScale(float scale = -1f)
	{
		if (scale > 0f)
		{
			this.xuiGlobalScaling = scale;
		}
		float @float = GamePrefs.GetFloat(EnumGamePrefs.OptionsScreenBoundsValue);
		float activeUiScale = GameOptionsManager.GetActiveUiScale();
		base.transform.localScale = Vector3.one * this.xuiGlobalScaling * @float * activeUiScale;
		base.transform.localPosition = Vector3.zero;
		XUi.pixelRatioFactor = 0f;
	}

	// Token: 0x06007B3A RID: 31546 RVA: 0x0031D342 File Offset: 0x0031B542
	public float GetScale()
	{
		return this.xuiGlobalScaling;
	}

	// Token: 0x06007B3B RID: 31547 RVA: 0x0031D34A File Offset: 0x0031B54A
	public void SetStackPanelScale(float scale)
	{
		this.defaultStackPanelScale = scale;
		this.stackPanelRoot.localScale = Vector3.one * scale;
		this.stackPanelRoot.localPosition = Vector3.zero;
	}

	// Token: 0x06007B3C RID: 31548 RVA: 0x0031D37C File Offset: 0x0031B57C
	public void Awake()
	{
		this.WindowGroups = new List<XUiWindowGroup>();
		this.ControllersByType = new Dictionary<Type, List<XUiController>>();
		this.FontsByName = new CaseInsensitiveStringDictionary<NGUIFont>();
		foreach (NGUIFont nguifont in this.NGUIFonts)
		{
			this.FontsByName.Add(nguifont.name, nguifont);
		}
	}

	// Token: 0x06007B3D RID: 31549 RVA: 0x0031D3D8 File Offset: 0x0031B5D8
	public void Load(List<string> windowGroupSubset = null, bool async = false)
	{
		if (async)
		{
			this.asyncLoad = true;
			this.loadAsyncCoroutine = ThreadManager.StartCoroutine(this.LoadAsync(windowGroupSubset));
			return;
		}
		this.asyncLoad = false;
		if (!XUiFromXml.HasData())
		{
			Log.Error("Loading XUi synchronously failed: XMLs not set.");
			return;
		}
		ThreadManager.RunCoroutineSync(this.LoadAsync(windowGroupSubset));
	}

	// Token: 0x06007B3E RID: 31550 RVA: 0x0031D427 File Offset: 0x0031B627
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator LoadAsync(List<string> windowGroupSubset = null)
	{
		yield return null;
		while (!XUiFromXml.HasData())
		{
			yield return null;
		}
		MicroStopwatch msw = new MicroStopwatch();
		Log.Out("[XUi] Loading XUi " + (this.asyncLoad ? "asynchronously" : "synchronously"));
		List<string> asyncWindowGroupList = (windowGroupSubset != null) ? new List<string>(windowGroupSubset) : new List<string>();
		if (windowGroupSubset == null)
		{
			XUiFromXml.GetWindowGroupNames(out asyncWindowGroupList);
		}
		if (msw.ElapsedMilliseconds > (long)Constants.cMaxLoadTimePerFrameMillis)
		{
			yield return null;
			msw.ResetAndRestart();
		}
		this.accumElapsedMilliseconds = 0L;
		MicroStopwatch ms = new MicroStopwatch(true);
		foreach (string text in asyncWindowGroupList)
		{
			ms.Reset();
			ms.Start();
			XUiFromXml.LoadXui(this, text);
			this.accumElapsedMilliseconds += ms.ElapsedMilliseconds;
			if (XUiFromXml.DebugXuiLoading == XUiFromXml.DebugLevel.Verbose)
			{
				Log.Out("[XUi] Parsing window group, {0}, completed in {1} ms.", new object[]
				{
					text,
					ms.ElapsedMilliseconds
				});
			}
			if (msw.ElapsedMilliseconds > 20L)
			{
				yield return null;
				msw.ResetAndRestart();
			}
		}
		List<string>.Enumerator enumerator = default(List<string>.Enumerator);
		XUiFromXml.LoadDone(windowGroupSubset == null);
		Log.Out("[XUi] Parsing all window groups completed in {0} ms total.", new object[]
		{
			this.accumElapsedMilliseconds
		});
		if (msw.ElapsedMilliseconds > (long)Constants.cMaxLoadTimePerFrameMillis)
		{
			yield return null;
			msw.ResetAndRestart();
		}
		this.accumElapsedMilliseconds = 0L;
		int num;
		for (int i = 0; i < this.WindowGroups.Count; i = num + 1)
		{
			XUiWindowGroup xuiWindowGroup = this.WindowGroups[i];
			ms.Reset();
			ms.Start();
			try
			{
				xuiWindowGroup.Init();
			}
			catch (Exception e)
			{
				Log.Error("[XUi] Failed initializing window group " + xuiWindowGroup.ID);
				Log.Exception(e);
			}
			this.accumElapsedMilliseconds += ms.ElapsedMilliseconds;
			if (XUiFromXml.DebugXuiLoading == XUiFromXml.DebugLevel.Verbose)
			{
				Log.Out("[XUi] Initialize window group, {0}, completed in {1} ms.", new object[]
				{
					xuiWindowGroup.ID,
					ms.ElapsedMilliseconds
				});
			}
			if (msw.ElapsedMilliseconds > 20L)
			{
				yield return null;
				msw.ResetAndRestart();
			}
			num = i;
		}
		Log.Out("[XUi] Initialized all window groups completed in {0} ms total.", new object[]
		{
			this.accumElapsedMilliseconds
		});
		while (this.loadGroup.Pending)
		{
			yield return null;
		}
		this.PostLoadInit();
		this.isReady = (windowGroupSubset == null);
		this.loadAsyncCoroutine = null;
		XUiUpdater.Add(this);
		yield break;
		yield break;
	}

	// Token: 0x06007B3F RID: 31551 RVA: 0x0031D440 File Offset: 0x0031B640
	[PublicizedFrom(EAccessModifier.Private)]
	public void PostLoadInit()
	{
		this.RadialWindow = (XUiC_Radial)this.FindWindowGroupByName("radial");
		this.ControllersByType.Clear();
		foreach (XUiWindowGroup xuiWindowGroup in this.WindowGroups)
		{
			if (xuiWindowGroup.Initialized)
			{
				this.AddControllerTypeEntry(xuiWindowGroup.Controller);
				foreach (XUiController controller in xuiWindowGroup.Controller.Children)
				{
					this.AddControllerTypeEntry(controller);
				}
			}
		}
		if (WorldStaticData.LoadAllXmlsCoComplete)
		{
			XUiFromXml.ClearLoadingData();
		}
	}

	// Token: 0x06007B40 RID: 31552 RVA: 0x0031D514 File Offset: 0x0031B714
	[PublicizedFrom(EAccessModifier.Private)]
	public void AddControllerTypeEntry(XUiController _controller)
	{
		Type type = _controller.GetType();
		List<XUiController> list;
		if (!this.ControllersByType.TryGetValue(type, out list))
		{
			list = new List<XUiController>();
			this.ControllersByType.Add(type, list);
		}
		list.Add(_controller);
	}

	// Token: 0x06007B41 RID: 31553 RVA: 0x0031D552 File Offset: 0x0031B752
	public int RegisterXUiView(XUiView _view)
	{
		this.xuiViewList.Add(_view);
		return this.xuiViewList.Count - 1;
	}

	// Token: 0x06007B42 RID: 31554 RVA: 0x0031D56D File Offset: 0x0031B76D
	public void LoadData<T>(string _path, Action<T> _callback) where T : UnityEngine.Object
	{
		LoadManager.LoadAsset<T>(_path, _callback, this.loadGroup, false, !this.asyncLoad);
	}

	// Token: 0x06007B43 RID: 31555 RVA: 0x0031D588 File Offset: 0x0031B788
	public void Init(int _id)
	{
		GamePrefs.OnGamePrefChanged += this.OnGamePrefChanged;
		GameOptionsManager.ResolutionChanged += this.OnResolutionChanged;
		this.loadGroup = LoadManager.CreateGroup();
		this.id = _id;
		this.windows = new List<XUiV_Window>();
		this.lastScreenSize = new Vector2((float)this.playerUI.camera.pixelWidth, (float)this.playerUI.camera.pixelHeight);
		base.gameObject.GetOrAddComponent<XUi_FallThrough>().SetXUi(this);
		this.stackPanelRoot = base.transform.Find("StackPanels").transform;
		this.stackPanels.Add("Left", new XUi.StackPanel("Left", base.transform.Find("StackPanels/Left").transform));
		this.stackPanels.Add("Center", new XUi.StackPanel("Center", base.transform.Find("StackPanels/Center").transform));
		this.stackPanels.Add("Right", new XUi.StackPanel("Right", base.transform.Find("StackPanels/Right").transform));
		MultiSourceAtlasManager[] array = Resources.FindObjectsOfTypeAll<MultiSourceAtlasManager>();
		for (int i = 0; i < array.Length; i++)
		{
			this.allMultiSourceAtlases.Add(array[i].name, array[i]);
		}
		if (Application.isPlaying)
		{
			this.LoadData<AudioClip>("@:Sounds/UI/ui_menu_cycle.wav", delegate(AudioClip o)
			{
				this.uiScrollSound = o;
			});
			this.LoadData<AudioClip>("@:Sounds/UI/ui_menu_click.wav", delegate(AudioClip o)
			{
				this.uiClickSound = o;
			});
			this.LoadData<AudioClip>("@:Sounds/UI/ui_menu_start.wav", delegate(AudioClip o)
			{
				this.uiConfirmSound = o;
			});
			this.LoadData<AudioClip>("@:Sounds/UI/ui_menu_back.wav", delegate(AudioClip o)
			{
				this.uiBackSound = o;
			});
			this.LoadData<AudioClip>("@:Sounds/UI/ui_hover.wav", delegate(AudioClip o)
			{
				this.uiSliderSound = o;
			});
		}
		this.anchors = base.transform.parent.GetComponentsInChildren<UIAnchor>();
		this.xuiAnchors = new UIAnchor[9];
		foreach (UIAnchor uianchor in this.anchors)
		{
			uianchor.runOnlyOnce = false;
			uianchor.uiCamera = this.playerUI.camera;
			if (uianchor.transform.parent.GetComponent<XUi>() == this)
			{
				this.xuiAnchors[(int)uianchor.side] = uianchor;
			}
		}
		this.UpdateAnchors();
		this.BackgroundGlobalOpacity = GamePrefs.GetFloat(EnumGamePrefs.OptionsBackgroundGlobalOpacity);
		this.ForegroundGlobalOpacity = GamePrefs.GetFloat(EnumGamePrefs.OptionsForegroundGlobalOpacity);
		LocalPlayerManager.OnLocalPlayersChanged += this.HandleLocalPlayersChanged;
		this.Vehicle = new XUiM_Vehicle();
		this.AssembleItem = new XUiM_AssembleItem();
		this.QuestTracker = new XUiM_Quest();
		this.Recipes = new XUiM_Recipes();
		this.Trader = new XUiM_Trader();
		this.Dialog = new XUiM_Dialog();
	}

	// Token: 0x06007B44 RID: 31556 RVA: 0x0031D858 File Offset: 0x0031BA58
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleLocalPlayersChanged()
	{
		this.lastScreenSize = new Vector2((float)this.playerUI.camera.pixelWidth, (float)this.playerUI.camera.pixelHeight);
		this.UpdateAnchors();
		this.RecenterWindowGroup(null, true);
		for (int i = 0; i < this.windows.Count; i++)
		{
			XUiV_Window xuiV_Window = this.windows[i];
			if (xuiV_Window.IsCursorArea && xuiV_Window.IsOpen)
			{
				this.UpdateWindowSoftCursorBounds(xuiV_Window);
			}
		}
	}

	// Token: 0x06007B45 RID: 31557 RVA: 0x0031D8DA File Offset: 0x0031BADA
	public void LateInit()
	{
		this.RadialWindow = (XUiC_Radial)this.FindWindowGroupByName("radial");
		XUiM_PlayerBuffs.HasLocalizationBeenCached = false;
		XUiM_Vehicle.HasLocalizationBeenCached = false;
	}

	// Token: 0x06007B46 RID: 31558 RVA: 0x0031D900 File Offset: 0x0031BB00
	public void Cleanup(bool _destroyImmediate = false)
	{
		this.CancelLoading();
		for (int i = 0; i < this.WindowGroups.Count; i++)
		{
			XUiWindowGroup xuiWindowGroup = this.WindowGroups[i];
			xuiWindowGroup.Controller.Cleanup();
			this.playerUI.windowManager.Remove(xuiWindowGroup.ID);
		}
		this.WindowGroups.Clear();
		XUiUpdater.Remove(this);
		if (_destroyImmediate)
		{
			UnityEngine.Object.DestroyImmediate(base.gameObject);
		}
	}

	// Token: 0x06007B47 RID: 31559 RVA: 0x0031D976 File Offset: 0x0031BB76
	public void CancelLoading()
	{
		if (this.loadAsyncCoroutine != null)
		{
			ThreadManager.StopCoroutine(this.loadAsyncCoroutine);
			this.loadAsyncCoroutine = null;
		}
	}

	// Token: 0x06007B48 RID: 31560 RVA: 0x0031D994 File Offset: 0x0031BB94
	public void OnUpdateInput()
	{
		if (this.WindowGroups == null)
		{
			return;
		}
		for (int i = 0; i < this.WindowGroups.Count; i++)
		{
			if (!this.WindowGroups[i].Controller.IsDormant && (this.WindowGroups[i].isShowing || this.WindowGroups[i].Controller.AlwaysUpdate()))
			{
				try
				{
					this.WindowGroups[i].Controller.UpdateInput();
				}
				catch (Exception e)
				{
					Log.Error("[XUi] Error while handling input for window group '" + this.WindowGroups[i].ID + "':");
					Log.Exception(e);
				}
			}
		}
	}

	// Token: 0x06007B49 RID: 31561 RVA: 0x0031DA60 File Offset: 0x0031BC60
	public void OnUpdateDeltaTime(float updateDeltaTime)
	{
		if (this.playerUI.entityPlayer != null)
		{
			if (this.PlayerInventory == null)
			{
				this.PlayerInventory = new XUiM_PlayerInventory(this, this.playerUI.entityPlayer);
			}
			if (this.PlayerEquipment == null)
			{
				this.PlayerEquipment = new XUiM_PlayerEquipment(this, this.playerUI.entityPlayer);
			}
		}
		if (this.WindowGroups == null)
		{
			return;
		}
		if (this.currentToolTip != null)
		{
			this.playerUI.windowManager.OpenIfNotOpen(this.currentToolTip.ID, false, false, true);
		}
		if (this.saveIndicator != null)
		{
			this.playerUI.windowManager.OpenIfNotOpen(this.saveIndicator.ID, false, false, true);
		}
		if (this.lastScreenSize.x != (float)this.playerUI.camera.pixelWidth || this.lastScreenSize.y != (float)this.playerUI.camera.pixelHeight)
		{
			this.lastScreenSize = new Vector2((float)this.playerUI.camera.pixelWidth, (float)this.playerUI.camera.pixelHeight);
			this.UpdateAnchors();
			this.RecenterWindowGroup(null, true);
		}
		for (int i = 0; i < this.WindowGroups.Count; i++)
		{
			XUiWindowGroup xuiWindowGroup = this.WindowGroups[i];
			if (xuiWindowGroup.Initialized && !xuiWindowGroup.Controller.IsDormant && (xuiWindowGroup.isShowing || xuiWindowGroup.Controller.AlwaysUpdate()))
			{
				try
				{
					xuiWindowGroup.Controller.Update(updateDeltaTime);
				}
				catch (Exception e)
				{
					Log.Error("[XUi] Error while updating window group '" + xuiWindowGroup.ID + "':");
					Log.Exception(e);
				}
			}
		}
		this.oldBackgroundGlobalOpacity = this.BackgroundGlobalOpacity;
		this.oldForegroundGlobalOpacity = this.ForegroundGlobalOpacity;
	}

	// Token: 0x06007B4A RID: 31562 RVA: 0x0031DC30 File Offset: 0x0031BE30
	public void RecenterWindowGroup(XUiWindowGroup _wg, bool _forceImmediate = false)
	{
		if (!_forceImmediate && GameStats.GetInt(EnumGameStats.GameState) != 2)
		{
			if (base.gameObject.activeInHierarchy)
			{
				base.StartCoroutine(this.recenterLater(_wg));
			}
			return;
		}
		if (_wg == null)
		{
			this.CalculateWindowGroupLayouts();
			return;
		}
		this.CalculateWindowGroupLayout(_wg);
	}

	// Token: 0x06007B4B RID: 31563 RVA: 0x0031DC6B File Offset: 0x0031BE6B
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator recenterLater(XUiWindowGroup _wg)
	{
		yield return null;
		if (_wg != null)
		{
			this.CalculateWindowGroupLayout(_wg);
		}
		else
		{
			this.CalculateWindowGroupLayouts();
		}
		yield return null;
		this.playerUI.CursorController.ResetNavigationTarget();
		yield break;
	}

	// Token: 0x06007B4C RID: 31564 RVA: 0x0031DC84 File Offset: 0x0031BE84
	[PublicizedFrom(EAccessModifier.Private)]
	public void CalculateWindowGroupLayouts()
	{
		foreach (XUiWindowGroup xuiWindowGroup in this.WindowGroups)
		{
			if (xuiWindowGroup.isShowing)
			{
				this.CalculateWindowGroupLayout(xuiWindowGroup);
			}
		}
	}

	// Token: 0x06007B4D RID: 31565 RVA: 0x0031DCE0 File Offset: 0x0031BEE0
	[PublicizedFrom(EAccessModifier.Private)]
	public void CalculateWindowGroupLayout(XUiWindowGroup _wg)
	{
		if (_wg == null || _wg.ID == "backpack")
		{
			return;
		}
		XUiController controller = _wg.Controller;
		if (((controller != null) ? controller.Children : null) == null)
		{
			return;
		}
		if (!_wg.HasStackPanelWindows())
		{
			return;
		}
		bool flag = false;
		int num = 0;
		int num2 = 0;
		foreach (XUi.StackPanel stackPanel in this.stackPanels.list)
		{
			bool flag2 = this.LayoutWindowsInPanel(_wg, stackPanel);
			if (flag2 && num > 0)
			{
				num += _wg.StackPanelPadding;
			}
			stackPanel.Transform.localPosition = new Vector3((float)num, 0f, 0f);
			num += stackPanel.Size.x;
			num2 = Math.Max(num2, stackPanel.Size.y);
			flag = (flag || flag2);
		}
		if (flag)
		{
			this.stackPanelRoot.localPosition = new Vector3(-(this.defaultStackPanelScale * (float)num / 2f), (float)_wg.StackPanelYOffset, 0f);
			this.stackPanelRoot.localScale = Vector3.one * this.defaultStackPanelScale;
		}
		if (flag && (!_wg.LeftPanelVAlignTop || !_wg.RightPanelVAlignTop))
		{
			if (!_wg.LeftPanelVAlignTop)
			{
				this.stackPanels.list[0].Transform.localPosition = new Vector3((float)((int)this.stackPanels.list[0].Transform.position.x), (float)(-(float)(num2 - this.stackPanels.list[0].Size.y)), 0f);
			}
			if (!_wg.RightPanelVAlignTop)
			{
				this.stackPanels.list[2].Transform.localPosition = new Vector3((float)((int)this.stackPanels.list[2].Transform.position.x), (float)(-(float)(num2 - this.stackPanels.list[2].Size.y)), 0f);
			}
		}
	}

	// Token: 0x06007B4E RID: 31566 RVA: 0x0031DF0C File Offset: 0x0031C10C
	[PublicizedFrom(EAccessModifier.Private)]
	public bool LayoutWindowsInPanel(XUiWindowGroup _wg, XUi.StackPanel _panel)
	{
		XUi.<>c__DisplayClass235_0 CS$<>8__locals1;
		CS$<>8__locals1._panel = _panel;
		CS$<>8__locals1.windowCount = 0;
		CS$<>8__locals1.yPos = 0;
		CS$<>8__locals1.maxWidth = 0;
		XUi.<LayoutWindowsInPanel>g__LayoutWindowGroupWindows|235_0(_wg, ref CS$<>8__locals1);
		if (this.playerUI.windowManager.IsWindowOpen("backpack"))
		{
			XUi.<LayoutWindowsInPanel>g__LayoutWindowGroupWindows|235_0(this.GetWindowGroupById("backpack"), ref CS$<>8__locals1);
		}
		CS$<>8__locals1._panel.Size.x = CS$<>8__locals1.maxWidth;
		CS$<>8__locals1._panel.Size.y = -CS$<>8__locals1.yPos;
		CS$<>8__locals1._panel.WindowCount = CS$<>8__locals1.windowCount;
		return CS$<>8__locals1.windowCount != 0;
	}

	// Token: 0x06007B4F RID: 31567 RVA: 0x0031DFB4 File Offset: 0x0031C1B4
	public Bounds GetXUIWindowWorldBounds(Transform _xuiElement, bool _includeInactive = false)
	{
		Bounds result = new Bounds(Vector3.zero, Vector3.zero);
		bool flag = false;
		_xuiElement.GetComponentsInChildren<UIBasicSprite>(_includeInactive, XUi.getXUIWindowWorldBoundsList);
		List<UIBasicSprite> list = XUi.getXUIWindowWorldBoundsList;
		for (int i = 0; i < list.Count; i++)
		{
			UIBasicSprite uibasicSprite = list[i];
			Transform parent = uibasicSprite.transform.parent;
			if (!(parent != null) || !parent.name.Equals("MapSpriteEntity(Clone)"))
			{
				Vector3[] worldCorners = uibasicSprite.worldCorners;
				for (int j = 0; j < worldCorners.Length; j++)
				{
					if (!flag)
					{
						result = new Bounds(worldCorners[j], Vector3.zero);
						flag = true;
					}
					else
					{
						result.Encapsulate(worldCorners[j]);
					}
				}
			}
		}
		XUi.getXUIWindowWorldBoundsList.Clear();
		return result;
	}

	// Token: 0x06007B50 RID: 31568 RVA: 0x0031E084 File Offset: 0x0031C284
	public Bounds GetXUIWindowScreenBounds(Transform _xuiElement, bool _includeInactive = false)
	{
		Bounds xuiwindowWorldBounds = this.GetXUIWindowWorldBounds(_xuiElement, _includeInactive);
		Bounds result = new Bounds(this.playerUI.camera.WorldToScreenPoint(xuiwindowWorldBounds.min), Vector3.zero);
		result.Encapsulate(this.playerUI.camera.WorldToScreenPoint(xuiwindowWorldBounds.max));
		return result;
	}

	// Token: 0x06007B51 RID: 31569 RVA: 0x0031E0DC File Offset: 0x0031C2DC
	public Bounds GetXUIWindowViewportBounds(Transform _xuiElement, bool _includeInactive = false)
	{
		Bounds xuiwindowWorldBounds = this.GetXUIWindowWorldBounds(_xuiElement, _includeInactive);
		Bounds result = new Bounds(this.playerUI.camera.WorldToViewportPoint(xuiwindowWorldBounds.min), Vector3.zero);
		result.Encapsulate(this.playerUI.camera.WorldToViewportPoint(xuiwindowWorldBounds.max));
		return result;
	}

	// Token: 0x06007B52 RID: 31570 RVA: 0x0031E134 File Offset: 0x0031C334
	public Bounds GetXUIWindowPixelBounds(Transform _xuiElement, bool _includeInactive = false)
	{
		Bounds xuiwindowViewportBounds = this.GetXUIWindowViewportBounds(_xuiElement, _includeInactive);
		Vector3 center = Vector3.Scale(xuiwindowViewportBounds.min, new Vector3((float)this.playerUI.camera.pixelWidth, (float)this.playerUI.camera.pixelHeight, 1f));
		Vector3 point = Vector3.Scale(xuiwindowViewportBounds.max, new Vector3((float)this.playerUI.camera.pixelWidth, (float)this.playerUI.camera.pixelHeight, 1f));
		Bounds result = new Bounds(center, Vector3.zero);
		result.Encapsulate(point);
		return result;
	}

	// Token: 0x06007B53 RID: 31571 RVA: 0x0031E1D4 File Offset: 0x0031C3D4
	public void RefreshAllWindows(bool _includeViewComponents = false)
	{
		if (this.WindowGroups == null)
		{
			return;
		}
		for (int i = 0; i < this.WindowGroups.Count; i++)
		{
			this.WindowGroups[i].Controller.SetAllChildrenDirty(_includeViewComponents);
		}
	}

	// Token: 0x06007B54 RID: 31572 RVA: 0x0031E218 File Offset: 0x0031C418
	public void PlayMenuSound(XUi.UISoundType _soundType)
	{
		switch (_soundType)
		{
		case XUi.UISoundType.ClickSound:
			this.PlayMenuClickSound();
			return;
		case XUi.UISoundType.ScrollSound:
			this.PlayMenuScrollSound();
			return;
		case XUi.UISoundType.ConfirmSound:
			this.PlayMenuConfirmSound();
			return;
		case XUi.UISoundType.BackSound:
			this.PlayMenuBackSound();
			return;
		case XUi.UISoundType.SliderSound:
			this.PlayMenuSliderSound();
			return;
		case XUi.UISoundType.None:
			return;
		default:
			return;
		}
	}

	// Token: 0x06007B55 RID: 31573 RVA: 0x0031E267 File Offset: 0x0031C467
	public void PlayMenuScrollSound()
	{
		Manager.PlayXUiSound(this.uiScrollSound, this.uiScrollVolume);
	}

	// Token: 0x06007B56 RID: 31574 RVA: 0x0031E27A File Offset: 0x0031C47A
	public void PlayMenuClickSound()
	{
		Manager.PlayXUiSound(this.uiClickSound, this.uiClickVolume);
	}

	// Token: 0x06007B57 RID: 31575 RVA: 0x0031E28D File Offset: 0x0031C48D
	public void PlayMenuConfirmSound()
	{
		Manager.PlayXUiSound(this.uiConfirmSound, this.uiConfirmVolume);
	}

	// Token: 0x06007B58 RID: 31576 RVA: 0x0031E2A0 File Offset: 0x0031C4A0
	public void PlayMenuBackSound()
	{
		Manager.PlayXUiSound(this.uiBackSound, this.uiBackVolume);
	}

	// Token: 0x06007B59 RID: 31577 RVA: 0x0031E2B3 File Offset: 0x0031C4B3
	public void PlayMenuSliderSound()
	{
		Manager.PlayXUiSound(this.uiSliderSound, this.uiSliderVolume);
	}

	// Token: 0x06007B5A RID: 31578 RVA: 0x0031E2C8 File Offset: 0x0031C4C8
	public UIAtlas GetAtlasByName(string _atlasName, string _spriteName)
	{
		if (string.IsNullOrEmpty(_atlasName))
		{
			return null;
		}
		MultiSourceAtlasManager multiSourceAtlasManager;
		if (!string.IsNullOrEmpty(_spriteName) && this.allMultiSourceAtlases.TryGetValue(_atlasName, out multiSourceAtlasManager))
		{
			return multiSourceAtlasManager.GetAtlasForSprite(_spriteName);
		}
		UIAtlas result;
		if (this.allAtlases.TryGetValue(_atlasName, out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x06007B5B RID: 31579 RVA: 0x0031E314 File Offset: 0x0031C514
	public NGUIFont GetUIFontByName(string _name, bool _showWarning = true)
	{
		NGUIFont result;
		if (this.FontsByName.TryGetValue(_name, out result))
		{
			return result;
		}
		if (_showWarning)
		{
			Log.Warning("XUi font not found: " + _name + ", from: " + StackTraceUtility.ExtractStackTrace());
		}
		return null;
	}

	// Token: 0x06007B5C RID: 31580 RVA: 0x0031E351 File Offset: 0x0031C551
	public void AddWindow(XUiV_Window _window)
	{
		this.windows.Add(_window);
	}

	// Token: 0x06007B5D RID: 31581 RVA: 0x0031E360 File Offset: 0x0031C560
	public XUiV_Window GetWindow(string _name)
	{
		for (int i = 0; i < this.windows.Count; i++)
		{
			if (this.windows[i].ID == _name)
			{
				return this.windows[i];
			}
		}
		return null;
	}

	// Token: 0x06007B5E RID: 31582 RVA: 0x0031E3AC File Offset: 0x0031C5AC
	public XUiController FindWindowGroupByName(string _name)
	{
		for (int i = 0; i < this.WindowGroups.Count; i++)
		{
			if (this.WindowGroups[i].ID.EqualsCaseInsensitive(_name))
			{
				return this.WindowGroups[i].Controller;
			}
		}
		return null;
	}

	// Token: 0x06007B5F RID: 31583 RVA: 0x0031E3FC File Offset: 0x0031C5FC
	public XUiController GetChildById(string _id)
	{
		XUiController xuiController = null;
		for (int i = 0; i < this.WindowGroups.Count; i++)
		{
			xuiController = this.WindowGroups[i].Controller.GetChildById(_id);
			if (xuiController != null)
			{
				return xuiController;
			}
		}
		return xuiController;
	}

	// Token: 0x06007B60 RID: 31584 RVA: 0x0031E440 File Offset: 0x0031C640
	public List<XUiController> GetChildrenById(string _id)
	{
		List<XUiController> list = new List<XUiController>();
		for (int i = 0; i < this.WindowGroups.Count; i++)
		{
			this.WindowGroups[i].Controller.GetChildrenById(_id, list);
		}
		return list;
	}

	// Token: 0x06007B61 RID: 31585 RVA: 0x0031E484 File Offset: 0x0031C684
	public T GetChildByType<T>() where T : XUiController
	{
		for (int i = 0; i < this.WindowGroups.Count; i++)
		{
			T childByType = this.WindowGroups[i].Controller.GetChildByType<T>();
			if (childByType != null)
			{
				return childByType;
			}
		}
		return default(T);
	}

	// Token: 0x06007B62 RID: 31586 RVA: 0x0031E4D4 File Offset: 0x0031C6D4
	public List<T> GetChildrenByType<T>() where T : XUiController
	{
		List<T> list = new List<T>();
		for (int i = 0; i < this.WindowGroups.Count; i++)
		{
			this.WindowGroups[i].Controller.GetChildrenByType<T>(list);
		}
		return list;
	}

	// Token: 0x06007B63 RID: 31587 RVA: 0x0031E518 File Offset: 0x0031C718
	public T GetWindowByType<T>() where T : XUiController
	{
		Type typeFromHandle = typeof(T);
		List<XUiController> list;
		this.ControllersByType.TryGetValue(typeFromHandle, out list);
		if (list == null || list.Count == 0)
		{
			return default(T);
		}
		if (list.Count > 1)
		{
			Log.Warning("Multiple controllers of type " + typeof(T).FullName);
		}
		return (T)((object)list[0]);
	}

	// Token: 0x06007B64 RID: 31588 RVA: 0x0031E588 File Offset: 0x0031C788
	public List<T> GetWindowsByType<T>() where T : XUiController
	{
		Type typeFromHandle = typeof(T);
		List<XUiController> list;
		this.ControllersByType.TryGetValue(typeFromHandle, out list);
		List<T> list2 = new List<T>();
		if (list != null)
		{
			foreach (XUiController xuiController in list)
			{
				list2.Add((T)((object)xuiController));
			}
		}
		return list2;
	}

	// Token: 0x06007B65 RID: 31589 RVA: 0x0031E604 File Offset: 0x0031C804
	public XUiWindowGroup GetWindowGroupById(string _id)
	{
		foreach (XUiWindowGroup xuiWindowGroup in this.WindowGroups)
		{
			if (xuiWindowGroup.ID == _id)
			{
				return xuiWindowGroup;
			}
		}
		return null;
	}

	// Token: 0x06007B66 RID: 31590 RVA: 0x0031E668 File Offset: 0x0031C868
	public static string UppercaseFirst(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return string.Empty;
		}
		return char.ToUpper(s[0]).ToString() + s.Substring(1);
	}

	// Token: 0x06007B67 RID: 31591 RVA: 0x0031E6A4 File Offset: 0x0031C8A4
	public void CancelAllCrafting()
	{
		XUiC_RecipeStack[] recipesToCraft = this.FindWindowGroupByName("crafting").GetChildByType<XUiC_CraftingQueue>().GetRecipesToCraft();
		for (int i = 0; i < recipesToCraft.Length; i++)
		{
			recipesToCraft[i].ForceCancel();
		}
	}

	// Token: 0x06007B68 RID: 31592 RVA: 0x0031E6E0 File Offset: 0x0031C8E0
	public CraftingData GetCraftingData()
	{
		CraftingData craftingData = new CraftingData();
		XUiController xuiController = this.FindWindowGroupByName("crafting");
		if (xuiController == null)
		{
			return craftingData;
		}
		XUiC_CraftingQueue childByType = xuiController.GetChildByType<XUiC_CraftingQueue>();
		if (childByType == null)
		{
			return craftingData;
		}
		XUiC_RecipeStack[] recipesToCraft = childByType.GetRecipesToCraft();
		if (recipesToCraft == null)
		{
			return craftingData;
		}
		new RecipeQueueItem[recipesToCraft.Length];
		craftingData.RecipeQueueItems = new RecipeQueueItem[recipesToCraft.Length];
		for (int i = 0; i < recipesToCraft.Length; i++)
		{
			RecipeQueueItem recipeQueueItem = new RecipeQueueItem();
			recipeQueueItem.Recipe = recipesToCraft[i].GetRecipe();
			recipeQueueItem.Multiplier = (short)recipesToCraft[i].GetRecipeCount();
			recipeQueueItem.CraftingTimeLeft = recipesToCraft[i].GetRecipeCraftingTimeLeft();
			recipeQueueItem.IsCrafting = recipesToCraft[i].IsCrafting;
			recipeQueueItem.Quality = (byte)recipesToCraft[i].OutputQuality;
			recipeQueueItem.StartingEntityId = recipesToCraft[i].StartingEntityId;
			recipeQueueItem.RepairItem = recipesToCraft[i].OriginalItem;
			recipeQueueItem.AmountToRepair = (ushort)recipesToCraft[i].AmountToRepair;
			recipeQueueItem.OneItemCraftTime = recipesToCraft[i].GetOneItemCraftTime();
			craftingData.RecipeQueueItems[i] = recipeQueueItem;
		}
		return craftingData;
	}

	// Token: 0x06007B69 RID: 31593 RVA: 0x0031E7F1 File Offset: 0x0031C9F1
	public void SetCraftingData(CraftingData _cd)
	{
		base.StartCoroutine(this.SetCraftingDataAsync(_cd));
	}

	// Token: 0x06007B6A RID: 31594 RVA: 0x0031E801 File Offset: 0x0031CA01
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator SetCraftingDataAsync(CraftingData _cd)
	{
		while (!this.isReady)
		{
			yield return null;
		}
		XUiC_CraftingQueue childByType = this.GetChildByType<XUiC_CraftingQueue>();
		if (childByType != null)
		{
			childByType.ClearQueue();
			for (int i = 0; i < _cd.RecipeQueueItems.Length; i++)
			{
				RecipeQueueItem recipeQueueItem = _cd.RecipeQueueItems[i];
				if (recipeQueueItem != null)
				{
					if (recipeQueueItem.RepairItem != null && recipeQueueItem.RepairItem.type != 0)
					{
						childByType.AddItemToRepairAtIndex(i, recipeQueueItem.CraftingTimeLeft, recipeQueueItem.RepairItem, (int)recipeQueueItem.AmountToRepair, recipeQueueItem.IsCrafting, recipeQueueItem.StartingEntityId);
					}
					else
					{
						childByType.AddRecipeToCraftAtIndex(i, recipeQueueItem.Recipe, (int)recipeQueueItem.Multiplier, recipeQueueItem.CraftingTimeLeft, recipeQueueItem.IsCrafting, false, (int)recipeQueueItem.Quality, recipeQueueItem.StartingEntityId, recipeQueueItem.OneItemCraftTime);
					}
				}
				else
				{
					childByType.AddRecipeToCraftAtIndex(i, null, 0, -1f, false, false, -1, -1, -1f);
				}
			}
			childByType.IsDirty = true;
		}
		yield break;
	}

	// Token: 0x06007B6B RID: 31595 RVA: 0x0031E817 File Offset: 0x0031CA17
	public UIAnchor GetAnchor(UIAnchor.Side _anchorSide)
	{
		return this.xuiAnchors[(int)_anchorSide];
	}

	// Token: 0x06007B6C RID: 31596 RVA: 0x0031E824 File Offset: 0x0031CA24
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateAnchors()
	{
		float @float = GamePrefs.GetFloat(EnumGamePrefs.OptionsScreenBoundsValue);
		float num = (1f - @float) / 2f;
		foreach (UIAnchor uianchor in this.anchors)
		{
			if (uianchor != null && !(uianchor.name == "AnchorCenterCenter"))
			{
				if (uianchor.side == UIAnchor.Side.Right || uianchor.side == UIAnchor.Side.TopRight || uianchor.side == UIAnchor.Side.BottomRight)
				{
					uianchor.relativeOffset.x = -num;
				}
				if (uianchor.side == UIAnchor.Side.Left || uianchor.side == UIAnchor.Side.TopLeft || uianchor.side == UIAnchor.Side.BottomLeft)
				{
					uianchor.relativeOffset.x = num;
				}
				if (uianchor.side == UIAnchor.Side.Top || uianchor.side == UIAnchor.Side.TopRight || uianchor.side == UIAnchor.Side.TopLeft)
				{
					uianchor.relativeOffset.y = -num;
				}
				if (uianchor.side == UIAnchor.Side.Bottom || uianchor.side == UIAnchor.Side.BottomRight || uianchor.side == UIAnchor.Side.BottomLeft)
				{
					uianchor.relativeOffset.y = num;
				}
				if (uianchor.side == UIAnchor.Side.Bottom || uianchor.side == UIAnchor.Side.Top || uianchor.side == UIAnchor.Side.Center)
				{
					uianchor.relativeOffset.x = 0f;
				}
			}
		}
	}

	// Token: 0x06007B6D RID: 31597 RVA: 0x0031E968 File Offset: 0x0031CB68
	public static void HandlePaging(XUi _xui, Func<bool> _onPageUp, Func<bool> _onPageDown, bool useVerticalAxis = false)
	{
		if (!(null != _xui.playerUI) || _xui.playerUI.playerInput == null || _xui.playerUI.playerInput.GUIActions == null || !_xui.playerUI.windowManager.IsKeyShortcutsAllowed())
		{
			XUi.previousPagingVector = Vector2.zero;
			return;
		}
		Vector2 vector = _xui.playerUI.playerInput.GUIActions.Camera.Vector;
		if (vector == Vector2.zero)
		{
			XUi.pagingRepeatTimer = 0f;
			XUi.previousPagingVector = Vector2.zero;
			return;
		}
		if (XUi.previousPagingVector != Vector2.zero)
		{
			XUi.pagingRepeatTimer -= Time.unscaledDeltaTime;
			if (XUi.pagingRepeatTimer > 0f)
			{
				return;
			}
		}
		else
		{
			XUi.initialPagingInput = true;
		}
		XUi.previousPagingVector = vector;
		bool flag = false;
		if (useVerticalAxis)
		{
			if ((vector.y > 0f && _onPageUp()) || (vector.y < 0f && _onPageDown()))
			{
				flag = true;
			}
		}
		else if ((vector.x > 0f && _onPageUp()) || (vector.x < 0f && _onPageDown()))
		{
			flag = true;
		}
		if (flag)
		{
			_xui.playerUI.CursorController.PlayPagingSound();
		}
		XUi.pagingRepeatTimer = (XUi.initialPagingInput ? 0.35f : 0.1f);
		XUi.initialPagingInput = false;
	}

	// Token: 0x06007B6E RID: 31598 RVA: 0x0031EAD2 File Offset: 0x0031CCD2
	public void UpdateWindowSoftCursorBounds(XUiV_Window _window)
	{
		this.playerUI.CursorController != null;
	}

	// Token: 0x06007B6F RID: 31599 RVA: 0x0031EAE8 File Offset: 0x0031CCE8
	public void RemoveWindowFromSoftCursorBounds(XUiV_Window _window)
	{
		CursorControllerAbs cursorController = this.playerUI.CursorController;
		if (cursorController != null)
		{
			cursorController.RemoveBounds(_window.ID);
		}
	}

	// Token: 0x06007B70 RID: 31600 RVA: 0x0031EB18 File Offset: 0x0031CD18
	public void GetOpenWindows(List<XUiV_Window> list)
	{
		list.Clear();
		foreach (XUiV_Window xuiV_Window in this.windows)
		{
			if (xuiV_Window.IsOpen && xuiV_Window.IsVisible)
			{
				list.Add(xuiV_Window);
			}
		}
	}

	// Token: 0x06007B71 RID: 31601 RVA: 0x0031EB84 File Offset: 0x0031CD84
	public void ForceInputStyleChange()
	{
		List<XUiV_Window> list = new List<XUiV_Window>();
		this.GetOpenWindows(list);
		foreach (XUiV_Window xuiV_Window in list)
		{
			xuiV_Window.Controller.ForceInputStyleChange(PlatformManager.NativePlatform.Input.CurrentInputStyle, PlatformManager.NativePlatform.Input.CurrentInputStyle);
		}
	}

	// Token: 0x06007B72 RID: 31602 RVA: 0x0031EC00 File Offset: 0x0031CE00
	public static bool IsMatchingPlatform(string platformStr)
	{
		bool result = true;
		string[] array = platformStr.Split(",", StringSplitOptions.None);
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = array[i].Trim().ToUpper();
			if (!array[i].StartsWith("!"))
			{
				result = false;
			}
		}
		for (int j = 0; j < array.Length; j++)
		{
			if (Submission.Enabled)
			{
				if (array[j] == "SUBMISSION")
				{
					return true;
				}
				if (array[j] == "!SUBMISSION")
				{
					return false;
				}
			}
			if (DeviceFlag.StandaloneWindows.IsCurrent())
			{
				if (array[j] == "WINDOWS")
				{
					return true;
				}
				if (array[j] == "!WINDOWS")
				{
					return false;
				}
			}
			if (DeviceFlag.StandaloneLinux.IsCurrent())
			{
				if (array[j] == "LINUX")
				{
					return true;
				}
				if (array[j] == "!LINUX")
				{
					return false;
				}
			}
			if (DeviceFlag.StandaloneOSX.IsCurrent())
			{
				if (array[j] == "OSX")
				{
					return true;
				}
				if (array[j] == "!OSX")
				{
					return false;
				}
			}
			if ((DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX).IsCurrent())
			{
				if (array[j] == "STANDALONE")
				{
					return true;
				}
				if (array[j] == "!STANDALONE")
				{
					return false;
				}
			}
			if (DeviceFlag.PS5.IsCurrent())
			{
				if (array[j] == "PS5")
				{
					return true;
				}
				if (array[j] == "!PS5")
				{
					return false;
				}
			}
			if (DeviceFlag.XBoxSeriesS.IsCurrent())
			{
				if (array[j] == "XBOX_S")
				{
					return true;
				}
				if (array[j] == "!XBOX_S")
				{
					return false;
				}
			}
			if (DeviceFlag.XBoxSeriesX.IsCurrent())
			{
				if (array[j] == "XBOX_X")
				{
					return true;
				}
				if (array[j] == "!XBOX_X")
				{
					return false;
				}
			}
			if ((DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX).IsCurrent())
			{
				if (array[j] == "XBOX")
				{
					return true;
				}
				if (array[j] == "!XBOX")
				{
					return false;
				}
			}
			if ((DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent())
			{
				if (array[j] == "CONSOLE")
				{
					return true;
				}
				if (array[j] == "!CONSOLE")
				{
					return false;
				}
			}
		}
		return result;
	}

	// Token: 0x06007B7A RID: 31610 RVA: 0x0031EF40 File Offset: 0x0031D140
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static void <LayoutWindowsInPanel>g__LayoutWindowGroupWindows|235_0(XUiWindowGroup _wg, ref XUi.<>c__DisplayClass235_0 A_1)
	{
		foreach (XUiController xuiController in _wg.Controller.Children)
		{
			XUiV_Window xuiV_Window = (XUiV_Window)xuiController.ViewComponent;
			if (xuiV_Window != null && xuiV_Window.UiTransform.gameObject.activeInHierarchy && !(xuiController.ViewComponent.UiTransform.parent.name != A_1._panel.Name) && xuiV_Window.Size.y > 0)
			{
				if (A_1.yPos < 0)
				{
					A_1.yPos -= _wg.StackPanelPadding;
				}
				xuiV_Window.Position = new Vector2i(0, A_1.yPos);
				xuiV_Window.UiTransform.localPosition = new Vector3(0f, (float)A_1.yPos);
				A_1.yPos -= xuiV_Window.Size.y;
				A_1.maxWidth = Math.Max(A_1.maxWidth, xuiV_Window.Size.x);
				int windowCount = A_1.windowCount;
				A_1.windowCount = windowCount + 1;
			}
		}
	}

	// Token: 0x04005CF0 RID: 23792
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float xuiGlobalScaling = 1.255f;

	// Token: 0x04005CF1 RID: 23793
	public static string RootNode = "NGUI Camera";

	// Token: 0x04005CF2 RID: 23794
	public static int ID = -1;

	// Token: 0x04005CF3 RID: 23795
	public static string BlankTexture = "menu_empty";

	// Token: 0x04005CF4 RID: 23796
	public static Transform XUiRootTransform;

	// Token: 0x04005CF5 RID: 23797
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static int lastScreenHeight;

	// Token: 0x04005CF6 RID: 23798
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static float pixelRatioFactor;

	// Token: 0x04005CF7 RID: 23799
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static UIRoot UIRoot;

	// Token: 0x04005CF9 RID: 23801
	[NonSerialized]
	public float uiScrollVolume = 0.5f;

	// Token: 0x04005CFB RID: 23803
	[NonSerialized]
	public float uiClickVolume = 0.5f;

	// Token: 0x04005CFD RID: 23805
	[NonSerialized]
	public float uiConfirmVolume = 0.5f;

	// Token: 0x04005CFF RID: 23807
	[NonSerialized]
	public float uiBackVolume = 0.5f;

	// Token: 0x04005D01 RID: 23809
	[NonSerialized]
	public float uiSliderVolume = 0.25f;

	// Token: 0x04005D02 RID: 23810
	public int id;

	// Token: 0x04005D03 RID: 23811
	public NGUIFont[] NGUIFonts;

	// Token: 0x04005D04 RID: 23812
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public CaseInsensitiveStringDictionary<NGUIFont> FontsByName;

	// Token: 0x04005D05 RID: 23813
	public List<XUiWindowGroup> WindowGroups;

	// Token: 0x04005D06 RID: 23814
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Dictionary<Type, List<XUiController>> ControllersByType;

	// Token: 0x04005D07 RID: 23815
	public Vector2 lastScreenSize = Vector2.zero;

	// Token: 0x04005D08 RID: 23816
	public float BackgroundGlobalOpacity = 1f;

	// Token: 0x04005D09 RID: 23817
	public float ForegroundGlobalOpacity = 1f;

	// Token: 0x04005D0A RID: 23818
	public string Ruleset = "default";

	// Token: 0x04005D0B RID: 23819
	public XUiM_PlayerInventory PlayerInventory;

	// Token: 0x04005D0C RID: 23820
	public XUiM_PlayerEquipment PlayerEquipment;

	// Token: 0x04005D0D RID: 23821
	public XUiM_Vehicle Vehicle;

	// Token: 0x04005D0E RID: 23822
	public XUiM_AssembleItem AssembleItem;

	// Token: 0x04005D0F RID: 23823
	public XUiM_Quest QuestTracker;

	// Token: 0x04005D10 RID: 23824
	public XUiM_Recipes Recipes;

	// Token: 0x04005D11 RID: 23825
	public XUiM_Trader Trader;

	// Token: 0x04005D12 RID: 23826
	public XUiM_Dialog Dialog;

	// Token: 0x04005D13 RID: 23827
	public XUiC_BuffPopoutList BuffPopoutList;

	// Token: 0x04005D14 RID: 23828
	public XUiC_CollectedItemList CollectedItemList;

	// Token: 0x04005D15 RID: 23829
	public XUiC_Radial RadialWindow;

	// Token: 0x04005D16 RID: 23830
	public bool IgnoreMissingClass;

	// Token: 0x04005D17 RID: 23831
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool mIsReady;

	// Token: 0x04005D19 RID: 23833
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool asyncLoad;

	// Token: 0x04005D1A RID: 23834
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public LoadManager.LoadGroup loadGroup;

	// Token: 0x04005D1D RID: 23837
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<XUiV_Window> windows;

	// Token: 0x04005D1E RID: 23838
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public UIAnchor[] anchors;

	// Token: 0x04005D1F RID: 23839
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public UIAnchor[] xuiAnchors;

	// Token: 0x04005D20 RID: 23840
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float oldBackgroundGlobalOpacity;

	// Token: 0x04005D21 RID: 23841
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float oldForegroundGlobalOpacity;

	// Token: 0x04005D22 RID: 23842
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int repositionFrames;

	// Token: 0x04005D23 RID: 23843
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public XUiWindowGroup currentlyOpeningWindowGroup;

	// Token: 0x04005D24 RID: 23844
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float defaultStackPanelScale = 1f;

	// Token: 0x04005D25 RID: 23845
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform stackPanelRoot;

	// Token: 0x04005D26 RID: 23846
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly DictionaryList<string, XUi.StackPanel> stackPanels = new DictionaryList<string, XUi.StackPanel>();

	// Token: 0x04005D27 RID: 23847
	public static MicroStopwatch Stopwatch;

	// Token: 0x04005D28 RID: 23848
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Dictionary<string, UIAtlas> allAtlases = new CaseInsensitiveStringDictionary<UIAtlas>();

	// Token: 0x04005D29 RID: 23849
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Dictionary<string, MultiSourceAtlasManager> allMultiSourceAtlases = new CaseInsensitiveStringDictionary<MultiSourceAtlasManager>();

	// Token: 0x04005D2A RID: 23850
	public static GameObject defaultPrefab = null;

	// Token: 0x04005D2B RID: 23851
	public static GameObject fullPrefab = null;

	// Token: 0x04005D2C RID: 23852
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public LocalPlayerUI mPlayerUI;

	// Token: 0x04005D2D RID: 23853
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public XUiC_GamepadCalloutWindow mCalloutWindow;

	// Token: 0x04005D45 RID: 23877
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static bool _inGameMenuOpen = false;

	// Token: 0x04005D46 RID: 23878
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public long accumElapsedMilliseconds;

	// Token: 0x04005D47 RID: 23879
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Coroutine loadAsyncCoroutine;

	// Token: 0x04005D48 RID: 23880
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<XUiView> xuiViewList = new List<XUiView>();

	// Token: 0x04005D49 RID: 23881
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static List<UIBasicSprite> getXUIWindowWorldBoundsList = new List<UIBasicSprite>();

	// Token: 0x04005D4A RID: 23882
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Vector2 previousPagingVector = Vector2.zero;

	// Token: 0x04005D4B RID: 23883
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static float pagingRepeatTimer;

	// Token: 0x04005D4C RID: 23884
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static bool initialPagingInput = false;

	// Token: 0x02000F12 RID: 3858
	public enum UISoundType
	{
		// Token: 0x04005D4E RID: 23886
		ClickSound,
		// Token: 0x04005D4F RID: 23887
		ScrollSound,
		// Token: 0x04005D50 RID: 23888
		ConfirmSound,
		// Token: 0x04005D51 RID: 23889
		BackSound,
		// Token: 0x04005D52 RID: 23890
		SliderSound,
		// Token: 0x04005D53 RID: 23891
		None
	}

	// Token: 0x02000F13 RID: 3859
	[PublicizedFrom(EAccessModifier.Private)]
	public class StackPanel
	{
		// Token: 0x06007B7B RID: 31611 RVA: 0x0031F094 File Offset: 0x0031D294
		public StackPanel(string _name, Transform _transform)
		{
			this.Name = _name;
			this.Transform = _transform;
		}

		// Token: 0x04005D54 RID: 23892
		public readonly string Name;

		// Token: 0x04005D55 RID: 23893
		public readonly Transform Transform;

		// Token: 0x04005D56 RID: 23894
		public int WindowCount;

		// Token: 0x04005D57 RID: 23895
		public Vector2Int Size;
	}

	// Token: 0x02000F14 RID: 3860
	[PublicizedFrom(EAccessModifier.Private)]
	public enum Anchor
	{
		// Token: 0x04005D59 RID: 23897
		LeftTop,
		// Token: 0x04005D5A RID: 23898
		LeftCenter,
		// Token: 0x04005D5B RID: 23899
		LeftBottom,
		// Token: 0x04005D5C RID: 23900
		CenterTop,
		// Token: 0x04005D5D RID: 23901
		CenterCenter,
		// Token: 0x04005D5E RID: 23902
		CenterBottom,
		// Token: 0x04005D5F RID: 23903
		RightTop,
		// Token: 0x04005D60 RID: 23904
		RightCenter,
		// Token: 0x04005D61 RID: 23905
		RightBottom,
		// Token: 0x04005D62 RID: 23906
		Count
	}

	// Token: 0x02000F15 RID: 3861
	public enum Alignment
	{
		// Token: 0x04005D64 RID: 23908
		TopLeft,
		// Token: 0x04005D65 RID: 23909
		CenterLeft,
		// Token: 0x04005D66 RID: 23910
		BottomLeft,
		// Token: 0x04005D67 RID: 23911
		TopCenter,
		// Token: 0x04005D68 RID: 23912
		CenterCenter,
		// Token: 0x04005D69 RID: 23913
		BottomCenter,
		// Token: 0x04005D6A RID: 23914
		TopRight,
		// Token: 0x04005D6B RID: 23915
		CenterRight,
		// Token: 0x04005D6C RID: 23916
		BottomRight,
		// Token: 0x04005D6D RID: 23917
		Count
	}
}
