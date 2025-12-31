using System;
using System.Collections;
using System.Collections.Generic;
using InControl;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000D4E RID: 3406
[UnityEngine.Scripting.Preserve]
public class XUiC_OptionsControlsNewBinding : XUiController
{
	// Token: 0x06006A4D RID: 27213 RVA: 0x002B467C File Offset: 0x002B287C
	public override void Init()
	{
		base.Init();
		XUiC_OptionsControlsNewBinding.ID = base.WindowGroup.ID;
		this.lblAction = (base.GetChildById("forAction").ViewComponent as XUiV_Label);
		this.rectInUse = (base.GetChildById("inUse").ViewComponent as XUiV_Rect);
		this.lblInUseBy = (base.GetChildById("inUseBy").ViewComponent as XUiV_Label);
		this.lblAbort = (base.GetChildById("newBindingAbort").ViewComponent as XUiV_Label);
		((XUiC_SimpleButton)base.GetChildById("btnCancel")).OnPressed += this.BtnCancel_OnPressed;
		((XUiC_SimpleButton)base.GetChildById("btnNewBinding")).OnPressed += this.BtnNewBinding_OnPressed;
		((XUiC_SimpleButton)base.GetChildById("btnGrabBinding")).OnPressed += this.BtnGrabBinding_OnPressed;
	}

	// Token: 0x06006A4E RID: 27214 RVA: 0x0028BEAC File Offset: 0x0028A0AC
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnCancel_OnPressed(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
	}

	// Token: 0x06006A4F RID: 27215 RVA: 0x002B476E File Offset: 0x002B296E
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnNewBinding_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.startGetBinding();
	}

	// Token: 0x06006A50 RID: 27216 RVA: 0x002B4778 File Offset: 0x002B2978
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnGrabBinding_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.conflictingAction.ClearInputState();
		this.conflictingAction.RemoveBinding(this.binding);
		this.action.UnbindBindingsOfType(this.forController);
		this.action.AddBinding(this.binding);
		ThreadManager.StartCoroutine(this.closeNextFrame());
	}

	// Token: 0x06006A51 RID: 27217 RVA: 0x002B47D0 File Offset: 0x002B29D0
	[PublicizedFrom(EAccessModifier.Private)]
	public void startGetBinding()
	{
		base.xui.playerUI.windowManager.IsInputLocked = true;
		InputUtils.EnableAllPlayerActions(false);
		GameManager.Instance.SetCursorEnabledOverride(true, false);
		this.rectInUse.IsVisible = false;
		this.conflictingAction = null;
		this.binding = null;
		this.action.Owner.ListenOptions.IncludeUnknownControllers = false;
		this.action.Owner.ListenOptions.IncludeMouseButtons = !this.forController;
		this.action.Owner.ListenOptions.IncludeMouseScrollWheel = !this.forController;
		this.action.Owner.ListenOptions.IncludeKeys = true;
		this.action.Owner.ListenOptions.OnBindingFound = new Func<PlayerAction, BindingSource, bool>(this.onBindingReceived);
		this.action.ListenForBinding();
	}

	// Token: 0x06006A52 RID: 27218 RVA: 0x002B48B3 File Offset: 0x002B2AB3
	[PublicizedFrom(EAccessModifier.Private)]
	public void stoppedGetBinding(bool _alsoStopListening = true)
	{
		if (_alsoStopListening)
		{
			this.action.StopListeningForBinding();
		}
		InputUtils.EnableAllPlayerActions(true);
		base.xui.playerUI.windowManager.IsInputLocked = false;
		GameManager.Instance.SetCursorEnabledOverride(false, false);
	}

	// Token: 0x06006A53 RID: 27219 RVA: 0x002B48EC File Offset: 0x002B2AEC
	[PublicizedFrom(EAccessModifier.Private)]
	public bool onBindingReceived(PlayerAction _action, BindingSource _binding)
	{
		if (this.bindingAbortActions.Contains(_binding))
		{
			Log.Out("Abort action pressed, aborting listening for new binding for {0}", new object[]
			{
				_action.Name
			});
			this.stoppedGetBinding(true);
			ThreadManager.StartCoroutine(this.closeNextFrame());
			return false;
		}
		if (this.forController && (_binding is KeyBindingSource || _binding is MouseBindingSource))
		{
			Log.Out("Cannot accept key or mouse for controller binding");
			return false;
		}
		if (!this.forController && _binding is DeviceBindingSource)
		{
			Log.Out("Cannot accept device binding source for keyboard/mouse binding");
			return false;
		}
		foreach (BindingSource b in this.bindingForbidden)
		{
			if (_binding == b)
			{
				Log.Out("Binding {0} not allowed", new object[]
				{
					_binding.Name
				});
				return false;
			}
		}
		if (this.forController != (_binding.BindingSourceType == BindingSourceType.DeviceBindingSource))
		{
			Log.Out("New binding ({0}) doesn't match expected input device type ({1})", new object[]
			{
				_binding.BindingSourceType.ToStringCached<BindingSourceType>(),
				this.forController ? BindingSourceType.DeviceBindingSource.ToStringCached<BindingSourceType>() : BindingSourceType.KeyBindingSource.ToStringCached<BindingSourceType>()
			});
			return false;
		}
		if (_action.HasBinding(_binding))
		{
			Log.Out("Binding {0} already bound to the current action {1}", new object[]
			{
				_binding.Name,
				_action.Name
			});
			this.stoppedGetBinding(true);
			ThreadManager.StartCoroutine(this.closeNextFrame());
			return false;
		}
		if (this.alreadyBound(_binding, _action))
		{
			Log.Out("Binding {0} already bound to the action {1}", new object[]
			{
				_binding.Name,
				this.conflictingAction.Name
			});
			this.stoppedGetBinding(true);
			return false;
		}
		_action.UnbindBindingsOfType(this.forController);
		this.stoppedGetBinding(false);
		ThreadManager.StartCoroutine(this.closeNextFrame());
		return true;
	}

	// Token: 0x06006A54 RID: 27220 RVA: 0x002B4A98 File Offset: 0x002B2C98
	[PublicizedFrom(EAccessModifier.Private)]
	public bool alreadyBound(BindingSource _binding, PlayerAction _selfAction)
	{
		PlayerAction playerAction = _selfAction.Owner.BindingUsed(_binding);
		if (playerAction == null && _selfAction.Owner.UserData != null)
		{
			PlayerActionsBase[] bindingsConflictWithSet = ((PlayerActionData.ActionSetUserData)_selfAction.Owner.UserData).bindingsConflictWithSet;
			for (int i = 0; i < bindingsConflictWithSet.Length; i++)
			{
				playerAction = bindingsConflictWithSet[i].BindingUsed(_binding);
				if (playerAction != null)
				{
					break;
				}
			}
		}
		if (playerAction != null)
		{
			PlayerActionData.ActionUserData actionUserData = _selfAction.UserData as PlayerActionData.ActionUserData;
			PlayerActionData.ActionUserData actionUserData2 = playerAction.UserData as PlayerActionData.ActionUserData;
			if (actionUserData.allowMultipleBindings || actionUserData2.allowMultipleBindings)
			{
				PlayerActionSet playerActionSet = base.xui.playerUI.playerInput;
				if (!base.xui.playerUI.playerInput.Actions.Contains(_selfAction) || !base.xui.playerUI.playerInput.Actions.Contains(playerAction))
				{
					if (base.xui.playerUI.playerInput.GUIActions.Actions.Contains(_selfAction) && base.xui.playerUI.playerInput.GUIActions.Actions.Contains(playerAction))
					{
						playerActionSet = base.xui.playerUI.playerInput.GUIActions;
					}
					else if (base.xui.playerUI.playerInput.VehicleActions.Actions.Contains(_selfAction) && base.xui.playerUI.playerInput.VehicleActions.Actions.Contains(playerAction))
					{
						playerActionSet = base.xui.playerUI.playerInput.VehicleActions;
					}
					else
					{
						if (!base.xui.playerUI.playerInput.PermanentActions.Actions.Contains(_selfAction) || !base.xui.playerUI.playerInput.PermanentActions.Actions.Contains(playerAction))
						{
							return false;
						}
						playerActionSet = base.xui.playerUI.playerInput.PermanentActions;
					}
				}
				bool flag = false;
				foreach (PlayerAction playerAction2 in playerActionSet.Actions)
				{
					if (playerAction2 != playerAction && playerAction2 != _selfAction && playerAction2.Bindings.Contains(_binding))
					{
						flag = true;
						playerAction = playerAction2;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			this.rectInUse.IsVisible = true;
			this.binding = playerAction.GetBindingOfType(this.forController);
			this.conflictingAction = playerAction;
			if (this.forController)
			{
				this.lblInUseBy.Text = string.Format(Localization.Get("xuiNewBindingConflictingAction_Controller", false), playerAction.GetBindingString(true, PlatformManager.NativePlatform.Input.CurrentControllerInputStyle, XUiUtils.EmptyBindingStyle.LocalizedNone, XUiUtils.DisplayStyle.Plain, false, null), ((PlayerActionData.ActionUserData)playerAction.UserData).LocalizedName);
			}
			else
			{
				this.lblInUseBy.Text = string.Format(Localization.Get("xuiNewBindingConflictingAction", false), playerAction.GetBindingString(this.forController, PlayerInputManager.InputStyle.Undefined, XUiUtils.EmptyBindingStyle.LocalizedNone, XUiUtils.DisplayStyle.Plain, false, null), ((PlayerActionData.ActionUserData)playerAction.UserData).LocalizedName);
			}
			base.GetChildById("btnNewBinding").SelectCursorElement(true, false);
			this.lblInUseBy.ToolTip = ((PlayerActionData.ActionUserData)playerAction.UserData).LocalizedDescription;
			return true;
		}
		return false;
	}

	// Token: 0x06006A55 RID: 27221 RVA: 0x002B4DD8 File Offset: 0x002B2FD8
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator closeNextFrame()
	{
		yield return null;
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
		yield break;
	}

	// Token: 0x06006A56 RID: 27222 RVA: 0x002B4DE7 File Offset: 0x002B2FE7
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator startNextFrame()
	{
		yield return null;
		this.startGetBinding();
		yield break;
	}

	// Token: 0x06006A57 RID: 27223 RVA: 0x002B4DF8 File Offset: 0x002B2FF8
	public override void OnOpen()
	{
		base.OnOpen();
		if (this.forController)
		{
			this.lblAction.Text = string.Format(Localization.Get("xuiNewBindingCurrent_Controller", false), ((PlayerActionData.ActionUserData)this.action.UserData).LocalizedName, this.action.GetBindingString(true, PlayerInputManager.InputStyleFromSelectedIconStyle(), XUiUtils.EmptyBindingStyle.LocalizedNone, XUiUtils.DisplayStyle.Plain, false, null));
		}
		else
		{
			this.lblAction.Text = string.Format(Localization.Get("xuiNewBindingCurrent", false), ((PlayerActionData.ActionUserData)this.action.UserData).LocalizedName, this.action.GetBindingString(this.forController, PlayerInputManager.InputStyle.Undefined, XUiUtils.EmptyBindingStyle.LocalizedNone, XUiUtils.DisplayStyle.Plain, false, null));
		}
		PlayerInputManager.InputStyle inputStyle = PlayerInputManager.InputStyleFromSelectedIconStyle();
		string text;
		if (this.forController)
		{
			string arg;
			if (inputStyle == PlayerInputManager.InputStyle.PS4)
			{
				arg = "[sp=PS5_Button_Options] / ESC";
			}
			else
			{
				arg = "[sp=XB_Button_Back] / ESC";
			}
			text = string.Format(Localization.Get("xuiNewBindingAbort_Controller", false), arg);
		}
		else
		{
			string arg;
			if (inputStyle == PlayerInputManager.InputStyle.PS4)
			{
				arg = "ESC / [sp=PS5_Button_Options]";
			}
			else
			{
				arg = "ESC / [sp=XB_Button_Back]";
			}
			text = string.Format(Localization.Get("xuiNewBindingAbort_Controller", false), arg);
		}
		this.lblAbort.Text = text;
		ThreadManager.StartCoroutine(this.startNextFrame());
	}

	// Token: 0x06006A58 RID: 27224 RVA: 0x002B4F10 File Offset: 0x002B3110
	public override void OnClose()
	{
		base.OnClose();
		base.xui.playerUI.windowManager.Open(this.windowToOpen, true, false, true);
	}

	// Token: 0x06006A59 RID: 27225 RVA: 0x002B4F38 File Offset: 0x002B3138
	public static void GetNewBinding(XUi _xuiInstance, PlayerAction _action, string _windowToOpen, bool _forController = false)
	{
		XUiC_OptionsControlsNewBinding childByType = _xuiInstance.FindWindowGroupByName(XUiC_OptionsControlsNewBinding.ID).GetChildByType<XUiC_OptionsControlsNewBinding>();
		childByType.action = _action;
		childByType.windowToOpen = _windowToOpen;
		childByType.forController = _forController;
		_xuiInstance.playerUI.windowManager.Open(childByType.WindowGroup.ID, true, false, true);
	}

	// Token: 0x04005032 RID: 20530
	public static string ID = "";

	// Token: 0x04005033 RID: 20531
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblAction;

	// Token: 0x04005034 RID: 20532
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblAbort;

	// Token: 0x04005035 RID: 20533
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Rect rectInUse;

	// Token: 0x04005036 RID: 20534
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblInUseBy;

	// Token: 0x04005037 RID: 20535
	[PublicizedFrom(EAccessModifier.Private)]
	public PlayerAction action;

	// Token: 0x04005038 RID: 20536
	[PublicizedFrom(EAccessModifier.Private)]
	public PlayerAction conflictingAction;

	// Token: 0x04005039 RID: 20537
	[PublicizedFrom(EAccessModifier.Private)]
	public BindingSource binding;

	// Token: 0x0400503A RID: 20538
	[PublicizedFrom(EAccessModifier.Private)]
	public string windowToOpen;

	// Token: 0x0400503B RID: 20539
	[PublicizedFrom(EAccessModifier.Private)]
	public bool forController;

	// Token: 0x0400503C RID: 20540
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<BindingSource> bindingAbortActions = new List<BindingSource>
	{
		new DeviceBindingSource(InputControlType.Back),
		new DeviceBindingSource(InputControlType.Options),
		new DeviceBindingSource(InputControlType.View),
		new DeviceBindingSource(InputControlType.Minus),
		new KeyBindingSource(new Key[]
		{
			Key.Escape
		})
	};

	// Token: 0x0400503D RID: 20541
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly BindingSource[] bindingForbidden = new BindingSource[]
	{
		new KeyBindingSource(new Key[]
		{
			Key.F1
		}),
		new KeyBindingSource(new Key[]
		{
			Key.F2
		}),
		new KeyBindingSource(new Key[]
		{
			Key.F3
		}),
		new KeyBindingSource(new Key[]
		{
			Key.F4
		}),
		new KeyBindingSource(new Key[]
		{
			Key.F5
		}),
		new KeyBindingSource(new Key[]
		{
			Key.F6
		}),
		new KeyBindingSource(new Key[]
		{
			Key.F7
		}),
		new KeyBindingSource(new Key[]
		{
			Key.F8
		}),
		new KeyBindingSource(new Key[]
		{
			Key.F9
		}),
		new KeyBindingSource(new Key[]
		{
			Key.F10
		}),
		new KeyBindingSource(new Key[]
		{
			Key.F11
		}),
		new KeyBindingSource(new Key[]
		{
			Key.F12
		}),
		new DeviceBindingSource(InputControlType.Start),
		new DeviceBindingSource(InputControlType.Back),
		new DeviceBindingSource(InputControlType.LeftStickUp),
		new DeviceBindingSource(InputControlType.LeftStickDown),
		new DeviceBindingSource(InputControlType.LeftStickLeft),
		new DeviceBindingSource(InputControlType.LeftStickRight),
		new DeviceBindingSource(InputControlType.RightStickUp),
		new DeviceBindingSource(InputControlType.RightStickDown),
		new DeviceBindingSource(InputControlType.RightStickLeft),
		new DeviceBindingSource(InputControlType.RightStickRight),
		new DeviceBindingSource(InputControlType.Share),
		new DeviceBindingSource(InputControlType.Menu),
		new DeviceBindingSource(InputControlType.View),
		new DeviceBindingSource(InputControlType.Options),
		new DeviceBindingSource(InputControlType.Plus),
		new DeviceBindingSource(InputControlType.Minus),
		new DeviceBindingSource(InputControlType.TouchPadButton),
		new DeviceBindingSource(InputControlType.Select),
		new DeviceBindingSource(InputControlType.LeftStickY),
		new DeviceBindingSource(InputControlType.LeftStickX),
		new DeviceBindingSource(InputControlType.RightStickY),
		new DeviceBindingSource(InputControlType.RightStickX),
		new DeviceBindingSource(InputControlType.Create),
		new DeviceBindingSource(InputControlType.Guide),
		new DeviceBindingSource(InputControlType.Home),
		new DeviceBindingSource(InputControlType.Mute),
		new DeviceBindingSource(InputControlType.Capture)
	};
}
