using System;
using System.Collections;
using System.Collections.Generic;
using InControl;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000D47 RID: 3399
[UnityEngine.Scripting.Preserve]
public class XUiC_OptionsController : XUiController
{
	// Token: 0x140000AD RID: 173
	// (add) Token: 0x06006A05 RID: 27141 RVA: 0x002B1304 File Offset: 0x002AF504
	// (remove) Token: 0x06006A06 RID: 27142 RVA: 0x002B1338 File Offset: 0x002AF538
	public static event Action OnSettingsChanged;

	// Token: 0x06006A07 RID: 27143 RVA: 0x002B136C File Offset: 0x002AF56C
	public override void Init()
	{
		base.Init();
		base.RegisterForInputStyleChanges();
		XUiC_OptionsController.ID = base.WindowGroup.ID;
		XUiController childById = base.GetChildById("AllowController");
		if (childById != null)
		{
			this.comboAllowController = childById.GetChildByType<XUiC_ComboBoxBool>();
			this.comboAllowController.OnValueChangedGeneric += this.Combo_OnValueChangedGeneric;
		}
		this.comboVibrationStrength = base.GetChildById("ControllerVibration").GetChildByType<XUiC_ComboBoxList<string>>();
		this.comboInterfaceSensitivity = base.GetChildById("ControllerInterfaceSensitivity").GetChildByType<XUiC_ComboBoxFloat>();
		XUiController childById2 = base.GetChildById("ShowDS4");
		this.comboShowDS4 = ((childById2 != null) ? childById2.GetChildByType<XUiC_ComboBoxBool>() : null);
		this.comboShowBindingsFor = base.GetChildById("ShowBindingsFor").GetChildByType<XUiC_ComboBoxList<string>>();
		this.comboLookSensitivityX = base.GetChildById("ControllerLookSensitivityX").GetChildByType<XUiC_ComboBoxFloat>();
		this.comboLookSensitivityY = base.GetChildById("ControllerLookSensitivityY").GetChildByType<XUiC_ComboBoxFloat>();
		this.comboLookInvert = base.GetChildById("ControllerLookInvert").GetChildByType<XUiC_ComboBoxBool>();
		this.comboLookAcceleration = base.GetChildById("ControllerLookAcceleration").GetChildByType<XUiC_ComboBoxFloat>();
		this.comboJoystickLayout = base.GetChildById("ControllerJoystickLayout").GetChildByType<XUiC_ComboBoxList<string>>();
		this.comboZoomSensitivity = base.GetChildById("ControllerZoomSensitivity").GetChildByType<XUiC_ComboBoxFloat>();
		this.comboLookAxisDeadzone = base.GetChildById("ControllerLookAxisDeadzone").GetChildByType<XUiC_ComboBoxFloat>();
		this.comboMoveAxisDeadzone = base.GetChildById("ControllerMoveAxisDeadzone").GetChildByType<XUiC_ComboBoxFloat>();
		this.comboCursorSnap = base.GetChildById("ControllerCursorSnap").GetChildByType<XUiC_ComboBoxBool>();
		this.comboCursorHoverSensitivity = base.GetChildById("ControllerCursorHoverSensitivity").GetChildByType<XUiC_ComboBoxFloat>();
		this.comboAimAssists = base.GetChildById("ControllerAimAssists").GetChildByType<XUiC_ComboBoxBool>();
		this.comboWeaponAiming = base.GetChildById("WeaponAiming").GetChildByType<XUiC_ComboBoxBool>();
		this.comboVehicleSensitivity = base.GetChildById("ControllerVehicleSensitivity").GetChildByType<XUiC_ComboBoxFloat>();
		this.comboSprintLock = base.GetChildById("SprintLock").GetChildByType<XUiC_ComboBoxList<string>>();
		XUiController childById3 = base.GetChildById("ControllerTriggerEffects");
		this.comboTriggerEffects = ((childById3 != null) ? childById3.GetChildByType<XUiC_ComboBoxBool>() : null);
		XUiController childById4 = base.GetChildById("ControllerIconStyle");
		if (childById4 != null)
		{
			this.comboIconStyle = childById4.GetChildByType<XUiC_ComboBoxList<string>>();
			this.comboIconStyle.OnValueChangedGeneric += this.Combo_OnValueChangedGeneric;
		}
		this.comboVibrationStrength.OnValueChangedGeneric += this.Combo_OnValueChangedGeneric;
		this.comboInterfaceSensitivity.OnValueChangedGeneric += this.Combo_OnValueChangedGeneric;
		this.comboLookSensitivityX.OnValueChangedGeneric += this.Combo_OnValueChangedGeneric;
		this.comboLookSensitivityY.OnValueChangedGeneric += this.Combo_OnValueChangedGeneric;
		this.comboJoystickLayout.OnValueChangedGeneric += this.Combo_OnValueChangedGeneric;
		this.comboLookAcceleration.OnValueChangedGeneric += this.Combo_OnValueChangedGeneric;
		this.comboLookInvert.OnValueChangedGeneric += this.Combo_OnValueChangedGeneric;
		this.comboZoomSensitivity.OnValueChangedGeneric += this.Combo_OnValueChangedGeneric;
		this.comboLookAxisDeadzone.OnValueChangedGeneric += this.Combo_OnValueChangedGeneric;
		this.comboMoveAxisDeadzone.OnValueChangedGeneric += this.Combo_OnValueChangedGeneric;
		this.comboCursorSnap.OnValueChangedGeneric += this.Combo_OnValueChangedGeneric;
		this.comboCursorHoverSensitivity.OnValueChangedGeneric += this.Combo_OnValueChangedGeneric;
		this.comboAimAssists.OnValueChangedGeneric += this.Combo_OnValueChangedGeneric;
		this.comboWeaponAiming.OnValueChangedGeneric += this.Combo_OnValueChangedGeneric;
		this.comboVehicleSensitivity.OnValueChangedGeneric += this.Combo_OnValueChangedGeneric;
		this.comboSprintLock.OnValueChangedGeneric += this.Combo_OnValueChangedGeneric;
		if (this.comboTriggerEffects != null)
		{
			this.comboTriggerEffects.OnValueChangedGeneric += this.Combo_OnValueChangedGeneric;
		}
		this.comboInterfaceSensitivity.Min = 0.10000000149011612;
		this.comboInterfaceSensitivity.Max = 1.0;
		if (this.comboShowDS4 != null)
		{
			this.comboShowDS4.OnValueChanged += this.ComboShowDs4_OnOnValueChanged;
		}
		this.comboShowBindingsFor.OnValueChanged += this.ComboShowMenuBindings_OnOnValueChanged;
		this.comboLookSensitivityX.Min = (this.comboLookSensitivityY.Min = 0.05000000074505806);
		this.comboLookSensitivityX.Max = (this.comboLookSensitivityY.Max = 1.0);
		this.comboZoomSensitivity.Min = (this.comboVehicleSensitivity.Min = 0.05000000074505806);
		this.comboZoomSensitivity.Max = (this.comboVehicleSensitivity.Max = 2.0);
		this.comboLookAcceleration.Min = 0.0;
		this.comboLookAcceleration.Max = 10.0;
		this.comboLookAxisDeadzone.Min = (this.comboMoveAxisDeadzone.Min = 0.0);
		this.comboLookAxisDeadzone.Max = (this.comboMoveAxisDeadzone.Max = 0.20000000298023224);
		this.comboCursorHoverSensitivity.Min = 0.10000000149011612;
		this.comboCursorHoverSensitivity.Max = 1.0;
		this.tabs = (base.GetChildById("tabs") as XUiC_TabSelector);
		this.btnBack = (base.GetChildById("btnBack") as XUiC_SimpleButton);
		this.btnDefaults = (base.GetChildById("btnDefaults") as XUiC_SimpleButton);
		this.btnApply = (base.GetChildById("btnApply") as XUiC_SimpleButton);
		this.btnBack.OnPressed += this.BtnBack_OnPressed;
		this.btnDefaults.OnPressed += this.BtnDefaults_OnOnPressed;
		this.btnApply.OnPressed += this.BtnApply_OnPressed;
		this.actionSetIngame = base.xui.playerUI.playerInput;
		this.actionSetVehicles = base.xui.playerUI.playerInput.VehicleActions;
		this.actionSetMenu = base.xui.playerUI.playerInput.GUIActions;
		this.actionSetPermanent = base.xui.playerUI.playerInput.PermanentActions;
		this.AddControllerLabelMappingsForButton("Menu", null, null);
		this.AddControllerLabelMappingsForButton("RightTrigger", null, null);
		this.AddControllerLabelMappingsForButton("RightBumper", null, null);
		this.AddControllerLabelMappingsForButton("Action4", null, null);
		this.AddControllerLabelMappingsForButton("Action3", null, null);
		this.AddControllerLabelMappingsForButton("Action2", null, null);
		this.AddControllerLabelMappingsForButton("Action1", null, null);
		this.AddControllerLabelMappingsForButton("RightStickButton", null, null);
		this.AddControllerLabelMappingsForButton("View", null, null);
		this.AddControllerLabelMappingsForButton("LeftTrigger", null, null);
		this.AddControllerLabelMappingsForButton("LeftBumper", null, null);
		this.AddControllerLabelMappingsForButton("LeftStickButton", null, null);
		this.AddControllerLabelMappingsForButton("DPadUp", null, null);
		this.AddControllerLabelMappingsForButton("DPadLeft", null, null);
		this.AddControllerLabelMappingsForButton("DPadDown", null, null);
		this.AddControllerLabelMappingsForButton("DPadRight", null, null);
		this.AssignControllerLabelMappingsForButton("LeftStick", ref this.leftStickLabel, new string[]
		{
			"LeftStickLeft",
			"LeftStickRight",
			"LeftStickUp",
			"LeftStickDown"
		}, null);
		this.AssignControllerLabelMappingsForButton("RightStick", ref this.rightStickLabel, new string[]
		{
			"RightStickLeft",
			"RightStickRight",
			"RightStickUp",
			"RightStickDown"
		}, null);
		(base.GetChildById("controllerArt").ViewComponent as XUiV_Sprite).Sprite.fixedAspect = true;
		(base.GetChildById("controllerLines").ViewComponent as XUiV_Sprite).Sprite.fixedAspect = true;
		this.updateControllerMappingLabels();
		this.RefreshApplyLabel();
	}

	// Token: 0x06006A08 RID: 27144 RVA: 0x002B1B42 File Offset: 0x002AFD42
	[PublicizedFrom(EAccessModifier.Private)]
	public void RefreshApplyLabel()
	{
		InControlExtensions.SetApplyButtonString(this.btnApply, "xuiApply");
	}

	// Token: 0x06006A09 RID: 27145 RVA: 0x002B1B54 File Offset: 0x002AFD54
	[PublicizedFrom(EAccessModifier.Private)]
	public void Combo_OnValueChangedGeneric(XUiController _sender)
	{
		this.btnApply.Enabled = true;
	}

	// Token: 0x06006A0A RID: 27146 RVA: 0x002B1B64 File Offset: 0x002AFD64
	[PublicizedFrom(EAccessModifier.Private)]
	public void AddControllerLabelMappingsForButton(string _uiName, string[] _controlTypeNames = null, InputControlType[] _controlTypes = null)
	{
		XUiController[] childrenById = base.GetChildById("controllerlayout").GetChildrenById(_uiName, null);
		XUiV_Label[] array = new XUiV_Label[childrenById.Length];
		for (int i = 0; i < childrenById.Length; i++)
		{
			array[i] = (XUiV_Label)childrenById[i].ViewComponent;
		}
		if (_controlTypeNames == null)
		{
			this.labelsForControllers.Add(new XUiC_OptionsController.ControllerLabelMapping(_uiName, array));
			return;
		}
		this.labelsForControllers.Add(new XUiC_OptionsController.ControllerLabelMapping(_controlTypeNames, array, _controlTypes));
	}

	// Token: 0x06006A0B RID: 27147 RVA: 0x002B1BD4 File Offset: 0x002AFDD4
	[PublicizedFrom(EAccessModifier.Private)]
	public void AssignControllerLabelMappingsForButton(string _uiName, ref XUiC_OptionsController.ControllerLabelMapping assignTo, string[] _controlTypeNames = null, InputControlType[] _controlTypes = null)
	{
		XUiController[] childrenById = base.GetChildById("controllerlayout").GetChildrenById(_uiName, null);
		XUiV_Label[] array = new XUiV_Label[childrenById.Length];
		for (int i = 0; i < childrenById.Length; i++)
		{
			array[i] = (XUiV_Label)childrenById[i].ViewComponent;
		}
		if (_controlTypeNames == null)
		{
			assignTo = new XUiC_OptionsController.ControllerLabelMapping(_uiName, array);
			return;
		}
		assignTo = new XUiC_OptionsController.ControllerLabelMapping(_controlTypeNames, array, _controlTypes);
	}

	// Token: 0x06006A0C RID: 27148 RVA: 0x002B1C3B File Offset: 0x002AFE3B
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnApply_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.applyChanges();
		this.btnApply.Enabled = false;
	}

	// Token: 0x06006A0D RID: 27149 RVA: 0x002B1C50 File Offset: 0x002AFE50
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnDefaults_OnOnPressed(XUiController _sender, int _mouseButton)
	{
		switch (this.tabs.SelectedTabIndex)
		{
		case 0:
			if (this.comboAllowController != null)
			{
				this.comboAllowController.Value = (bool)GamePrefs.GetDefault(EnumGamePrefs.OptionsAllowController);
			}
			this.comboVibrationStrength.Value = ((eControllerVibrationStrength)GamePrefs.GetDefault(EnumGamePrefs.OptionsControllerVibrationStrength)).ToString();
			this.comboInterfaceSensitivity.Value = (double)((float)GamePrefs.GetDefault(EnumGamePrefs.OptionsInterfaceSensitivity));
			this.comboLookSensitivityX.Value = (double)((float)GamePrefs.GetDefault(EnumGamePrefs.OptionsControllerSensitivityX));
			this.comboLookSensitivityY.Value = (double)((float)GamePrefs.GetDefault(EnumGamePrefs.OptionsControllerSensitivityY));
			this.comboJoystickLayout.Value = ((eControllerJoystickLayout)GamePrefs.GetDefault(EnumGamePrefs.OptionsControllerJoystickLayout)).ToString();
			this.comboLookAcceleration.Value = (double)((float)GamePrefs.GetDefault(EnumGamePrefs.OptionsControllerLookAcceleration));
			this.comboLookInvert.Value = (bool)GamePrefs.GetDefault(EnumGamePrefs.OptionsControllerLookInvert);
			this.comboZoomSensitivity.Value = (double)((float)GamePrefs.GetDefault(EnumGamePrefs.OptionsControllerZoomSensitivity));
			this.comboLookAxisDeadzone.Value = (double)((float)GamePrefs.GetDefault(EnumGamePrefs.OptionsControllerLookAxisDeadzone));
			this.comboMoveAxisDeadzone.Value = (double)((float)GamePrefs.GetDefault(EnumGamePrefs.OptionsControllerMoveAxisDeadzone));
			this.comboCursorSnap.Value = (bool)GamePrefs.GetDefault(EnumGamePrefs.OptionsControllerCursorSnap);
			this.comboCursorHoverSensitivity.Value = (double)((float)GamePrefs.GetDefault(EnumGamePrefs.OptionsControllerCursorHoverSensitivity));
			this.comboVehicleSensitivity.Value = (double)((float)GamePrefs.GetDefault(EnumGamePrefs.OptionsControllerVehicleSensitivity));
			this.comboWeaponAiming.Value = (bool)GamePrefs.GetDefault(EnumGamePrefs.OptionsControllerWeaponAiming);
			this.comboAimAssists.Value = (bool)GamePrefs.GetDefault(EnumGamePrefs.OptionsControllerAimAssists);
			this.comboSprintLock.SelectedIndex = (int)GamePrefs.GetDefault(EnumGamePrefs.OptionsControlsSprintLock);
			if (this.comboTriggerEffects != null)
			{
				this.comboTriggerEffects.Value = (bool)GamePrefs.GetDefault(EnumGamePrefs.OptionsControllerTriggerEffects);
			}
			if (this.comboIconStyle != null)
			{
				this.comboIconStyle.Value = ((PlayerInputManager.ControllerIconStyle)GamePrefs.GetDefault(EnumGamePrefs.OptionsControllerIconStyle)).ToString();
				goto IL_2D9;
			}
			goto IL_2D9;
		case 1:
			using (List<PlayerAction>.Enumerator enumerator = this.actionTabGroups["inpTabPlayerOnFoot"].GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PlayerAction playerAction = enumerator.Current;
					playerAction.ResetBindings();
				}
				goto IL_2D9;
			}
			break;
		case 2:
			break;
		default:
			return;
		}
		foreach (PlayerAction playerAction2 in this.actionTabGroups["inpTabVehicle"])
		{
			playerAction2.ResetBindings();
		}
		IL_2D9:
		this.updateControllerMappingLabels();
		this.updateActionBindingLabels();
		this.btnApply.Enabled = true;
	}

	// Token: 0x06006A0E RID: 27150 RVA: 0x002A30F5 File Offset: 0x002A12F5
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnBack_OnPressed(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
		base.xui.playerUI.windowManager.Open(XUiC_OptionsMenu.ID, true, false, true);
	}

	// Token: 0x06006A0F RID: 27151 RVA: 0x002B1F6C File Offset: 0x002B016C
	[PublicizedFrom(EAccessModifier.Private)]
	public void ComboShowDs4_OnOnValueChanged(XUiController _sender, bool _oldValue, bool _newValue)
	{
		this.IsDirty = true;
		this.updateControllerMappingLabels();
	}

	// Token: 0x06006A10 RID: 27152 RVA: 0x002B1F6C File Offset: 0x002B016C
	[PublicizedFrom(EAccessModifier.Private)]
	public void ComboShowMenuBindings_OnOnValueChanged(XUiController _sender, string _s, string _newValue1)
	{
		this.IsDirty = true;
		this.updateControllerMappingLabels();
	}

	// Token: 0x06006A11 RID: 27153 RVA: 0x002B1F7C File Offset: 0x002B017C
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateControllerMappingLabels()
	{
		List<PlayerAction> list = new List<PlayerAction>();
		PlayerActionsBase playerActionsBase = (this.comboShowBindingsFor.SelectedIndex <= 0) ? this.actionSetIngame : ((this.comboShowBindingsFor.SelectedIndex == 1) ? this.actionSetVehicles : this.actionSetMenu);
		int num = (this.comboShowDS4 == null || !this.comboShowDS4.Value) ? 0 : 1;
		foreach (XUiC_OptionsController.ControllerLabelMapping controllerLabelMapping in this.labelsForControllers)
		{
			string text = "";
			string text2 = "";
			foreach (InputControlType controlType in controllerLabelMapping.ControlTypes)
			{
				PlayerActionSet[] actionSets = new PlayerActionsBase[]
				{
					playerActionsBase,
					this.actionSetPermanent
				};
				controlType.GetBoundAction(actionSets, list);
				if (list.Count > 0)
				{
					foreach (PlayerAction playerAction in list)
					{
						PlayerActionData.ActionUserData actionUserData = (PlayerActionData.ActionUserData)playerAction.UserData;
						if (actionUserData != null)
						{
							if ((actionUserData.appliesToInputType == PlayerActionData.EAppliesToInputType.Both || actionUserData.appliesToInputType == PlayerActionData.EAppliesToInputType.ControllerOnly) && !actionUserData.doNotDisplay)
							{
								if (text.Length > 0)
								{
									text += ", ";
									text2 += ", ";
								}
								text += actionUserData.LocalizedName;
								text2 += actionUserData.LocalizedDescription;
							}
						}
						else if (ActionSetManager.DebugLevel > ActionSetManager.EDebugLevel.Off)
						{
							text += " !NULL! ";
							text2 += " !NULL! ";
						}
					}
				}
			}
			if (text.Length == 0)
			{
				text = Localization.Get("inpUnboundControllerKey", false);
				text2 = Localization.Get("inpUnboundControllerKeyTooltip", false);
			}
			int num2 = 0;
			if (controllerLabelMapping.Labels.Length > 1)
			{
				num2 = num;
			}
			controllerLabelMapping.Labels[num2].Text = text;
			controllerLabelMapping.Labels[num2].ToolTip = text2;
		}
		string text3 = "";
		string text4 = "";
		switch (GamePrefs.GetInt(EnumGamePrefs.OptionsControllerJoystickLayout))
		{
		case 0:
			text3 = Localization.Get("inpStandardLeftStick", false);
			text4 = Localization.Get("inpStandardRightStick", false);
			break;
		case 1:
			text3 = Localization.Get("inpStandardRightStick", false);
			text4 = Localization.Get("inpStandardLeftStick", false);
			break;
		case 2:
			text3 = Localization.Get("inpLegacyLeftStick", false);
			text4 = Localization.Get("inpLegacyRightStick", false);
			break;
		case 3:
			text3 = Localization.Get("inpLegacyRightStick", false);
			text4 = Localization.Get("inpLegacyLeftStick", false);
			break;
		}
		XUiV_Label[] labels = this.leftStickLabel.Labels;
		for (int i = 0; i < labels.Length; i++)
		{
			labels[i].Text = text3;
		}
		labels = this.rightStickLabel.Labels;
		for (int i = 0; i < labels.Length; i++)
		{
			labels[i].Text = text4;
		}
	}

	// Token: 0x06006A12 RID: 27154 RVA: 0x002B22B8 File Offset: 0x002B04B8
	[PublicizedFrom(EAccessModifier.Private)]
	public void createControlsEntries()
	{
		PlayerActionsBase[] array = new PlayerActionsBase[]
		{
			base.xui.playerUI.playerInput,
			base.xui.playerUI.playerInput.VehicleActions
		};
		this.actionTabGroups = new Dictionary<string, List<PlayerAction>>();
		this.actionTabGroups.Add("inpTabPlayerOnFoot", new List<PlayerAction>());
		PlayerActionsBase[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			foreach (PlayerAction playerAction in array2[i].ControllerRebindableActions)
			{
				PlayerActionData.ActionUserData actionUserData = playerAction.UserData as PlayerActionData.ActionUserData;
				if (actionUserData != null)
				{
					if (actionUserData.actionGroup.actionTab.tabNameKey == "inpTabPlayerControl" || actionUserData.actionGroup.actionTab.tabNameKey == "inpTabToolbelt")
					{
						this.actionTabGroups["inpTabPlayerOnFoot"].Add(playerAction);
					}
					else if (this.actionTabGroups.ContainsKey(actionUserData.actionGroup.actionTab.tabNameKey))
					{
						this.actionTabGroups[actionUserData.actionGroup.actionTab.tabNameKey].Add(playerAction);
					}
					else
					{
						this.actionTabGroups.Add(actionUserData.actionGroup.actionTab.tabNameKey, new List<PlayerAction>());
						this.actionTabGroups[actionUserData.actionGroup.actionTab.tabNameKey].Add(playerAction);
					}
				}
			}
		}
		this.actionTabGroups["inpTabPlayerOnFoot"].Add(base.xui.playerUI.playerInput.PermanentActions.PushToTalk);
		int num = 1;
		foreach (KeyValuePair<string, List<PlayerAction>> keyValuePair in this.actionTabGroups)
		{
			XUiV_Grid xuiV_Grid = (XUiV_Grid)this.tabs.GetTab(num).GetChildById("controlsGrid").ViewComponent;
			this.tabs.SetTabCaption(num, Localization.Get(keyValuePair.Key, false));
			for (int j = 0; j < xuiV_Grid.Controller.Children.Count; j++)
			{
				if (j < keyValuePair.Value.Count)
				{
					this.AssignActionToBindingEntry(xuiV_Grid.Controller.Children[j], keyValuePair.Value[j]);
				}
				else
				{
					this.AssignActionToBindingEntry(xuiV_Grid.Controller.Children[j], null);
				}
			}
			xuiV_Grid.Grid.Reposition();
			num++;
		}
	}

	// Token: 0x06006A13 RID: 27155 RVA: 0x002B25B0 File Offset: 0x002B07B0
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateOptions()
	{
		if (this.comboAllowController != null)
		{
			this.comboAllowController.Value = GamePrefs.GetBool(EnumGamePrefs.OptionsAllowController);
		}
		this.comboVibrationStrength.Value = ((eControllerVibrationStrength)GamePrefs.GetInt(EnumGamePrefs.OptionsControllerVibrationStrength)).ToString();
		this.comboInterfaceSensitivity.Value = (double)GamePrefs.GetFloat(EnumGamePrefs.OptionsInterfaceSensitivity);
		this.comboLookSensitivityX.Value = (double)GamePrefs.GetFloat(EnumGamePrefs.OptionsControllerSensitivityX);
		this.comboLookSensitivityY.Value = (double)GamePrefs.GetFloat(EnumGamePrefs.OptionsControllerSensitivityY);
		this.comboJoystickLayout.Value = ((eControllerJoystickLayout)GamePrefs.GetInt(EnumGamePrefs.OptionsControllerJoystickLayout)).ToString();
		this.comboLookAcceleration.Value = (double)GamePrefs.GetFloat(EnumGamePrefs.OptionsControllerLookAcceleration);
		this.comboLookInvert.Value = GamePrefs.GetBool(EnumGamePrefs.OptionsControllerLookInvert);
		this.comboZoomSensitivity.Value = (double)GamePrefs.GetFloat(EnumGamePrefs.OptionsControllerZoomSensitivity);
		this.comboLookAxisDeadzone.Value = (double)GamePrefs.GetFloat(EnumGamePrefs.OptionsControllerLookAxisDeadzone);
		this.comboMoveAxisDeadzone.Value = (double)GamePrefs.GetFloat(EnumGamePrefs.OptionsControllerMoveAxisDeadzone);
		this.comboCursorSnap.Value = GamePrefs.GetBool(EnumGamePrefs.OptionsControllerCursorSnap);
		this.comboCursorHoverSensitivity.Value = (double)GamePrefs.GetFloat(EnumGamePrefs.OptionsControllerCursorHoverSensitivity);
		this.comboVehicleSensitivity.Value = (double)GamePrefs.GetFloat(EnumGamePrefs.OptionsControllerVehicleSensitivity);
		this.comboWeaponAiming.Value = GamePrefs.GetBool(EnumGamePrefs.OptionsControllerWeaponAiming);
		this.comboAimAssists.Value = GamePrefs.GetBool(EnumGamePrefs.OptionsControllerAimAssists);
		this.comboSprintLock.SelectedIndex = GamePrefs.GetInt(EnumGamePrefs.OptionsControlsSprintLock);
		if (this.comboTriggerEffects != null)
		{
			this.comboTriggerEffects.Value = GamePrefs.GetBool(EnumGamePrefs.OptionsControllerTriggerEffects);
		}
		if (this.comboIconStyle != null)
		{
			this.comboIconStyle.Value = ((PlayerInputManager.ControllerIconStyle)GamePrefs.GetInt(EnumGamePrefs.OptionsControllerIconStyle)).ToString();
		}
	}

	// Token: 0x06006A14 RID: 27156 RVA: 0x002B2798 File Offset: 0x002B0998
	[PublicizedFrom(EAccessModifier.Private)]
	public void AssignActionToBindingEntry(XUiController _controller, PlayerAction _action)
	{
		XUiController childById = _controller.GetChildById("label");
		_controller.GetChildById("value");
		XUiController childById2 = _controller.GetChildById("unbind");
		XUiController childById3 = _controller.GetChildById("background");
		if (_action != null)
		{
			_controller.ViewComponent.UiTransform.name = _action.Name;
			PlayerActionData.ActionUserData actionUserData = (PlayerActionData.ActionUserData)_action.UserData;
			this.buttonActionDictionary.Add(_controller, _action);
			((XUiV_Label)childById.ViewComponent).Text = actionUserData.LocalizedName;
			childById.ViewComponent.ToolTip = actionUserData.LocalizedDescription;
			if (actionUserData.allowRebind)
			{
				childById3.OnPress += this.NewBindingClicked;
				childById2.OnPress += this.UnbindButtonClicked;
				childById2.ViewComponent.ToolTip = Localization.Get("xuiRemoveBinding", false);
				return;
			}
		}
		else
		{
			childById2.ViewComponent.ForceHide = true;
			childById2.ViewComponent.IsNavigatable = (childById2.ViewComponent.IsSnappable = (childById2.ViewComponent.IsVisible = false));
			childById3.ViewComponent.ForceHide = true;
			childById3.ViewComponent.IsNavigatable = (childById3.ViewComponent.IsSnappable = (childById3.ViewComponent.IsVisible = false));
			_controller.ViewComponent.UiTransform.gameObject.SetActive(false);
		}
	}

	// Token: 0x06006A15 RID: 27157 RVA: 0x002B28FD File Offset: 0x002B0AFD
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator updateActionBindingLabelsLater()
	{
		yield return null;
		this.updateActionBindingLabels();
		yield break;
	}

	// Token: 0x06006A16 RID: 27158 RVA: 0x0026BD11 File Offset: 0x00269F11
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void InputStyleChanged(PlayerInputManager.InputStyle _oldStyle, PlayerInputManager.InputStyle _newStyle)
	{
		base.InputStyleChanged(_oldStyle, _newStyle);
		this.IsDirty = true;
	}

	// Token: 0x06006A17 RID: 27159 RVA: 0x002B290C File Offset: 0x002B0B0C
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateActionBindingLabels()
	{
		foreach (KeyValuePair<XUiController, PlayerAction> keyValuePair in this.buttonActionDictionary)
		{
			((XUiV_Label)keyValuePair.Key.GetChildById("value").ViewComponent).Text = keyValuePair.Value.GetBindingString(true, PlatformManager.NativePlatform.Input.CurrentControllerInputStyle, XUiUtils.EmptyBindingStyle.LocalizedNone, XUiUtils.DisplayStyle.Plain, false, null);
		}
	}

	// Token: 0x06006A18 RID: 27160 RVA: 0x002B2998 File Offset: 0x002B0B98
	[PublicizedFrom(EAccessModifier.Private)]
	public void NewBindingClicked(XUiController _sender, int _mouseButton)
	{
		PlayerAction action;
		if (this.buttonActionDictionary.TryGetValue(_sender.Parent, out action))
		{
			this.closedForNewBinding = true;
			this.btnApply.Enabled = true;
			base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
			XUiC_OptionsControlsNewBinding.GetNewBinding(base.xui, action, this.windowGroup.ID, true);
		}
	}

	// Token: 0x06006A19 RID: 27161 RVA: 0x002B2A08 File Offset: 0x002B0C08
	[PublicizedFrom(EAccessModifier.Private)]
	public void UnbindButtonClicked(XUiController _sender, int _mouseButton)
	{
		PlayerAction action;
		if (this.buttonActionDictionary.TryGetValue(_sender.Parent, out action))
		{
			action.UnbindBindingsOfType(true);
			ThreadManager.StartCoroutine(this.updateActionBindingLabelsLater());
			this.btnApply.Enabled = true;
		}
	}

	// Token: 0x06006A1A RID: 27162 RVA: 0x002B2A4C File Offset: 0x002B0C4C
	[PublicizedFrom(EAccessModifier.Private)]
	public void applyChanges()
	{
		if (this.comboAllowController != null)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsAllowController, this.comboAllowController.Value);
		}
		GamePrefs.Set(EnumGamePrefs.OptionsControllerVibrationStrength, this.comboVibrationStrength.SelectedIndex);
		GamePrefs.Set(EnumGamePrefs.OptionsInterfaceSensitivity, (float)this.comboInterfaceSensitivity.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsControllerSensitivityX, (float)this.comboLookSensitivityX.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsControllerSensitivityY, (float)this.comboLookSensitivityY.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsControllerJoystickLayout, this.comboJoystickLayout.SelectedIndex);
		GamePrefs.Set(EnumGamePrefs.OptionsControllerLookInvert, this.comboLookInvert.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsControllerLookAcceleration, (float)this.comboLookAcceleration.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsControllerZoomSensitivity, (float)this.comboZoomSensitivity.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsControllerLookAxisDeadzone, (float)this.comboLookAxisDeadzone.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsControllerMoveAxisDeadzone, (float)this.comboMoveAxisDeadzone.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsControllerCursorHoverSensitivity, (float)this.comboCursorHoverSensitivity.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsControllerCursorSnap, this.comboCursorSnap.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsControllerAimAssists, this.comboAimAssists.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsControllerVehicleSensitivity, (float)this.comboVehicleSensitivity.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsControllerWeaponAiming, this.comboWeaponAiming.Value);
		GamePrefs.Set(EnumGamePrefs.OptionsControlsSprintLock, this.comboSprintLock.SelectedIndex);
		if (this.comboIconStyle != null)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsControllerIconStyle, this.comboIconStyle.SelectedIndex);
		}
		if (this.comboTriggerEffects != null)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsControllerTriggerEffects, this.comboTriggerEffects.Value);
		}
		GameOptionsManager.SaveControls();
		GamePrefs.Instance.Save();
		PlayerMoveController.UpdateControlsOptions();
		CursorControllerAbs.UpdateGamePrefs();
		TriggerEffectManager.UpdateControllerVibrationStrength();
		TriggerEffectManager.SetEnabled(GamePrefs.GetBool(EnumGamePrefs.OptionsControllerTriggerEffects));
		this.storeCurrentBindings();
		this.updateControllerMappingLabels();
		Action onSettingsChanged = XUiC_OptionsController.OnSettingsChanged;
		if (onSettingsChanged != null)
		{
			onSettingsChanged();
		}
		PlatformManager.NativePlatform.Input.ForceInputStyleChange();
		base.xui.calloutWindow.ForceInputStyleChange(base.CurrentInputStyle, base.CurrentInputStyle);
		this.IsDirty = true;
	}

	// Token: 0x06006A1B RID: 27163 RVA: 0x002B2C84 File Offset: 0x002B0E84
	[PublicizedFrom(EAccessModifier.Private)]
	public void storeCurrentBindings()
	{
		this.actionBindingsOnOpen.Clear();
		foreach (PlayerActionSet playerActionSet in PlatformManager.NativePlatform.Input.ActionSets)
		{
			this.actionBindingsOnOpen.Add(playerActionSet.Save());
		}
	}

	// Token: 0x06006A1C RID: 27164 RVA: 0x002B2CF0 File Offset: 0x002B0EF0
	public override void OnOpen()
	{
		base.WindowGroup.openWindowOnEsc = XUiC_OptionsMenu.ID;
		if (!this.initialized)
		{
			this.createControlsEntries();
			this.initialized = true;
		}
		if (!this.closedForNewBinding)
		{
			this.updateOptions();
			this.storeCurrentBindings();
			this.btnApply.Enabled = false;
		}
		this.closedForNewBinding = false;
		this.updateActionBindingLabels();
		this.RefreshApplyLabel();
		base.OnOpen();
		if (this.initialized)
		{
			List<XUiController> list = new List<XUiController>();
			base.GetChildrenById("bindingEntry", list);
			foreach (XUiController xuiController in list)
			{
				if (!this.buttonActionDictionary.ContainsKey(xuiController))
				{
					xuiController.ViewComponent.UiTransform.gameObject.SetActive(false);
				}
			}
		}
		PlayerInputManager.InputStyle currentControllerInputStyle = PlatformManager.NativePlatform.Input.CurrentControllerInputStyle;
		if (currentControllerInputStyle != PlayerInputManager.InputStyle.Keyboard)
		{
			bool flag = currentControllerInputStyle == PlayerInputManager.InputStyle.PS4;
			if (this.comboShowDS4 != null && flag != this.comboShowDS4.Value)
			{
				this.comboShowDS4.Value = flag;
				this.updateControllerMappingLabels();
			}
		}
	}

	// Token: 0x06006A1D RID: 27165 RVA: 0x002B2E18 File Offset: 0x002B1018
	public override void OnClose()
	{
		base.OnClose();
		if (!this.closedForNewBinding)
		{
			PlatformManager.NativePlatform.Input.LoadActionSetsFromStrings(this.actionBindingsOnOpen);
			this.btnApply.Enabled = false;
		}
	}

	// Token: 0x06006A1E RID: 27166 RVA: 0x002B2E4C File Offset: 0x002B104C
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty)
		{
			base.RefreshBindings(false);
			this.updateActionBindingLabels();
			this.RefreshApplyLabel();
			this.IsDirty = false;
		}
		if (this.btnApply.Enabled && base.xui.playerUI.playerInput.GUIActions.Apply.WasPressed && !base.xui.playerUI.windowManager.IsWindowOpen("optionsControlsNewBinding"))
		{
			this.BtnApply_OnPressed(null, 0);
		}
	}

	// Token: 0x06006A1F RID: 27167 RVA: 0x002B2ED4 File Offset: 0x002B10D4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "isds4")
		{
			_value = (DeviceFlag.PS5.IsCurrent() || (this.comboShowDS4 != null && this.comboShowDS4.Value)).ToString();
			return true;
		}
		if (_bindingName == "isxb1")
		{
			_value = ((DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX).IsCurrent() || (this.comboShowDS4 != null && !this.comboShowDS4.Value)).ToString();
			return true;
		}
		if (_bindingName == "controller_art")
		{
			_value = ((DeviceFlag.PS5.IsCurrent() || (this.comboShowDS4 != null && this.comboShowDS4.Value)) ? "Controller_Art_PS5" : "Controller_Art_XB");
			return true;
		}
		if (!(_bindingName == "controller_lines"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		_value = ((DeviceFlag.PS5.IsCurrent() || (this.comboShowDS4 != null && this.comboShowDS4.Value)) ? "Controller_Lines_PS5" : "Controller_Lines_XB");
		return true;
	}

	// Token: 0x04004FE5 RID: 20453
	public static string ID = "";

	// Token: 0x04004FE7 RID: 20455
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TabSelector tabs;

	// Token: 0x04004FE8 RID: 20456
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboAllowController;

	// Token: 0x04004FE9 RID: 20457
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<string> comboVibrationStrength;

	// Token: 0x04004FEA RID: 20458
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat comboInterfaceSensitivity;

	// Token: 0x04004FEB RID: 20459
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboShowDS4;

	// Token: 0x04004FEC RID: 20460
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat comboLookSensitivityX;

	// Token: 0x04004FED RID: 20461
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat comboLookSensitivityY;

	// Token: 0x04004FEE RID: 20462
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<string> comboJoystickLayout;

	// Token: 0x04004FEF RID: 20463
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<string> comboShowBindingsFor;

	// Token: 0x04004FF0 RID: 20464
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboLookInvert;

	// Token: 0x04004FF1 RID: 20465
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat comboLookAcceleration;

	// Token: 0x04004FF2 RID: 20466
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat comboZoomSensitivity;

	// Token: 0x04004FF3 RID: 20467
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat comboLookAxisDeadzone;

	// Token: 0x04004FF4 RID: 20468
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat comboMoveAxisDeadzone;

	// Token: 0x04004FF5 RID: 20469
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboCursorSnap;

	// Token: 0x04004FF6 RID: 20470
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat comboCursorHoverSensitivity;

	// Token: 0x04004FF7 RID: 20471
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat comboVehicleSensitivity;

	// Token: 0x04004FF8 RID: 20472
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboAimAssists;

	// Token: 0x04004FF9 RID: 20473
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboWeaponAiming;

	// Token: 0x04004FFA RID: 20474
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<string> comboSprintLock;

	// Token: 0x04004FFB RID: 20475
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxBool comboTriggerEffects;

	// Token: 0x04004FFC RID: 20476
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<string> comboIconStyle;

	// Token: 0x04004FFD RID: 20477
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnBack;

	// Token: 0x04004FFE RID: 20478
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnDefaults;

	// Token: 0x04004FFF RID: 20479
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnApply;

	// Token: 0x04005000 RID: 20480
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<XUiController, PlayerAction> buttonActionDictionary = new Dictionary<XUiController, PlayerAction>();

	// Token: 0x04005001 RID: 20481
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, List<PlayerAction>> actionTabGroups;

	// Token: 0x04005002 RID: 20482
	[PublicizedFrom(EAccessModifier.Private)]
	public bool initialized;

	// Token: 0x04005003 RID: 20483
	[PublicizedFrom(EAccessModifier.Private)]
	public bool closedForNewBinding;

	// Token: 0x04005004 RID: 20484
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<string> actionBindingsOnOpen = new List<string>();

	// Token: 0x04005005 RID: 20485
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<XUiC_OptionsController.ControllerLabelMapping> labelsForControllers = new List<XUiC_OptionsController.ControllerLabelMapping>();

	// Token: 0x04005006 RID: 20486
	[PublicizedFrom(EAccessModifier.Private)]
	public PlayerActionsBase actionSetIngame;

	// Token: 0x04005007 RID: 20487
	[PublicizedFrom(EAccessModifier.Private)]
	public PlayerActionsBase actionSetVehicles;

	// Token: 0x04005008 RID: 20488
	[PublicizedFrom(EAccessModifier.Private)]
	public PlayerActionsBase actionSetMenu;

	// Token: 0x04005009 RID: 20489
	[PublicizedFrom(EAccessModifier.Private)]
	public PlayerActionsBase actionSetPermanent;

	// Token: 0x0400500A RID: 20490
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_OptionsController.ControllerLabelMapping leftStickLabel;

	// Token: 0x0400500B RID: 20491
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_OptionsController.ControllerLabelMapping rightStickLabel;

	// Token: 0x02000D48 RID: 3400
	[PublicizedFrom(EAccessModifier.Private)]
	public struct ControllerLabelMapping
	{
		// Token: 0x06006A22 RID: 27170 RVA: 0x002B3018 File Offset: 0x002B1218
		public ControllerLabelMapping(string[] _controlTypeNames, XUiV_Label[] _labels, InputControlType[] _controlTypes = null)
		{
			this.ControlTypeNames = _controlTypeNames;
			if (_controlTypes == null)
			{
				this.ControlTypes = new InputControlType[_controlTypeNames.Length];
				for (int i = 0; i < _controlTypeNames.Length; i++)
				{
					this.ControlTypes[i] = EnumUtils.Parse<InputControlType>(_controlTypeNames[i], true);
				}
			}
			else
			{
				this.ControlTypes = _controlTypes;
			}
			this.Labels = _labels;
		}

		// Token: 0x06006A23 RID: 27171 RVA: 0x002B306C File Offset: 0x002B126C
		public ControllerLabelMapping(string _controlTypeName, XUiV_Label[] _labels)
		{
			this.ControlTypeNames = new string[]
			{
				_controlTypeName
			};
			this.ControlTypes = new InputControlType[]
			{
				EnumUtils.Parse<InputControlType>(_controlTypeName, true)
			};
			this.Labels = _labels;
		}

		// Token: 0x0400500C RID: 20492
		public readonly string[] ControlTypeNames;

		// Token: 0x0400500D RID: 20493
		public readonly InputControlType[] ControlTypes;

		// Token: 0x0400500E RID: 20494
		public readonly XUiV_Label[] Labels;
	}
}
