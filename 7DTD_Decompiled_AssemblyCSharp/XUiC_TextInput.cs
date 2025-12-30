using System;
using System.Collections;
using InControl;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000E66 RID: 3686
[UnityEngine.Scripting.Preserve]
public class XUiC_TextInput : XUiController
{
	// Token: 0x140000C6 RID: 198
	// (add) Token: 0x060073AB RID: 29611 RVA: 0x002F1AEC File Offset: 0x002EFCEC
	// (remove) Token: 0x060073AC RID: 29612 RVA: 0x002F1B24 File Offset: 0x002EFD24
	public event XUiEvent_InputOnSubmitEventHandler OnSubmitHandler;

	// Token: 0x140000C7 RID: 199
	// (add) Token: 0x060073AD RID: 29613 RVA: 0x002F1B5C File Offset: 0x002EFD5C
	// (remove) Token: 0x060073AE RID: 29614 RVA: 0x002F1B94 File Offset: 0x002EFD94
	public event XUiEvent_InputOnChangedEventHandler OnChangeHandler;

	// Token: 0x140000C8 RID: 200
	// (add) Token: 0x060073AF RID: 29615 RVA: 0x002F1BCC File Offset: 0x002EFDCC
	// (remove) Token: 0x060073B0 RID: 29616 RVA: 0x002F1C04 File Offset: 0x002EFE04
	public event XUiEvent_InputOnAbortedEventHandler OnInputAbortedHandler;

	// Token: 0x140000C9 RID: 201
	// (add) Token: 0x060073B1 RID: 29617 RVA: 0x002F1C3C File Offset: 0x002EFE3C
	// (remove) Token: 0x060073B2 RID: 29618 RVA: 0x002F1C74 File Offset: 0x002EFE74
	public event XUiEvent_InputOnSelectedEventHandler OnInputSelectedHandler;

	// Token: 0x140000CA RID: 202
	// (add) Token: 0x060073B3 RID: 29619 RVA: 0x002F1CAC File Offset: 0x002EFEAC
	// (remove) Token: 0x060073B4 RID: 29620 RVA: 0x002F1CE4 File Offset: 0x002EFEE4
	public event XUiEvent_InputOnErrorEventHandler OnInputErrorHandler;

	// Token: 0x140000CB RID: 203
	// (add) Token: 0x060073B5 RID: 29621 RVA: 0x002F1D1C File Offset: 0x002EFF1C
	// (remove) Token: 0x060073B6 RID: 29622 RVA: 0x002F1D54 File Offset: 0x002EFF54
	public event UIInput.OnClipboard OnClipboardHandler;

	// Token: 0x060073B7 RID: 29623 RVA: 0x002F1D89 File Offset: 0x002EFF89
	public static void SelectCurrentSearchField(LocalPlayerUI _playerUi)
	{
		if (XUiC_TextInput.currentSearchField != null && !_playerUi.windowManager.IsInputActive() && XUiC_TextInput.currentSearchField.viewComponent.UiTransform.gameObject.activeInHierarchy)
		{
			XUiC_TextInput.currentSearchField.SetSelected(true, false);
		}
	}

	// Token: 0x17000BB8 RID: 3000
	// (get) Token: 0x060073B8 RID: 29624 RVA: 0x002F1DC6 File Offset: 0x002EFFC6
	public UIInput UIInput
	{
		get
		{
			return this.uiInput;
		}
	}

	// Token: 0x17000BB9 RID: 3001
	// (get) Token: 0x060073B9 RID: 29625 RVA: 0x002F1DCE File Offset: 0x002EFFCE
	public XUiController UIInputController
	{
		get
		{
			return this.uiInputController;
		}
	}

	// Token: 0x17000BBA RID: 3002
	// (get) Token: 0x060073BA RID: 29626 RVA: 0x002F1DD6 File Offset: 0x002EFFD6
	// (set) Token: 0x060073BB RID: 29627 RVA: 0x002F1DE3 File Offset: 0x002EFFE3
	public string Text
	{
		get
		{
			return this.uiInput.value;
		}
		set
		{
			this.textChangeFromCode = true;
			this.uiInput.value = value;
			this.textChangeFromCode = false;
			this.uiInput.UpdateLabel();
		}
	}

	// Token: 0x17000BBB RID: 3003
	// (get) Token: 0x060073BC RID: 29628 RVA: 0x002F1E0A File Offset: 0x002F000A
	// (set) Token: 0x060073BD RID: 29629 RVA: 0x002F1E14 File Offset: 0x002F0014
	public XUiC_TextInput SelectOnTab
	{
		get
		{
			return this.selectOnTab;
		}
		set
		{
			if (this.selectOnTab != value)
			{
				this.selectOnTab = value;
				if (value != null)
				{
					UIKeyNavigation uikeyNavigation = this.uiInputController.ViewComponent.UiTransform.gameObject.AddMissingComponent<UIKeyNavigation>();
					uikeyNavigation.constraint = UIKeyNavigation.Constraint.Explicit;
					uikeyNavigation.onTab = value.uiInput.gameObject;
					return;
				}
				UIKeyNavigation uikeyNavigation2 = this.uiInputController.ViewComponent.UiTransform.gameObject.AddMissingComponent<UIKeyNavigation>();
				if (uikeyNavigation2 != null)
				{
					UnityEngine.Object.Destroy(uikeyNavigation2);
				}
			}
		}
	}

	// Token: 0x17000BBC RID: 3004
	// (get) Token: 0x060073BE RID: 29630 RVA: 0x002F1E90 File Offset: 0x002F0090
	// (set) Token: 0x060073BF RID: 29631 RVA: 0x002F1E98 File Offset: 0x002F0098
	public Color ActiveTextColor
	{
		get
		{
			return this.activeTextColor;
		}
		set
		{
			if (value != this.activeTextColor)
			{
				this.activeTextColor = value;
				this.IsDirty = true;
			}
		}
	}

	// Token: 0x17000BBD RID: 3005
	// (get) Token: 0x060073C0 RID: 29632 RVA: 0x002F1EB6 File Offset: 0x002F00B6
	// (set) Token: 0x060073C1 RID: 29633 RVA: 0x002F1EC3 File Offset: 0x002F00C3
	public bool Enabled
	{
		get
		{
			return this.uiInput.enabled;
		}
		set
		{
			this.uiInput.enabled = value;
		}
	}

	// Token: 0x17000BBE RID: 3006
	// (get) Token: 0x060073C2 RID: 29634 RVA: 0x002F1ED1 File Offset: 0x002F00D1
	// (set) Token: 0x060073C3 RID: 29635 RVA: 0x002F1EE8 File Offset: 0x002F00E8
	public bool SupportBbCode
	{
		get
		{
			return ((XUiV_Label)this.text.ViewComponent).SupportBbCode;
		}
		set
		{
			((XUiV_Label)this.text.ViewComponent).SupportBbCode = value;
		}
	}

	// Token: 0x17000BBF RID: 3007
	// (get) Token: 0x060073C4 RID: 29636 RVA: 0x002F1F00 File Offset: 0x002F0100
	public bool IsSelected
	{
		get
		{
			return this.uiInput.isSelected;
		}
	}

	// Token: 0x060073C5 RID: 29637 RVA: 0x002F1F10 File Offset: 0x002F0110
	public override void Init()
	{
		XUiV_Panel xuiV_Panel = this.viewComponent as XUiV_Panel;
		if (xuiV_Panel != null)
		{
			xuiV_Panel.createUiPanel = true;
		}
		base.Init();
		this.text = base.GetChildById("text");
		this.uiInputController = this.text;
		this.uiInput = this.uiInputController.ViewComponent.UiTransform.gameObject.AddComponent<UIInput>();
		if (this.ignoreUpDownKeys)
		{
			this.uiInput.onDownArrow = delegate()
			{
			};
			this.uiInput.onUpArrow = delegate()
			{
			};
		}
		XUiController childById = base.GetChildById("btnShowPassword");
		if (childById != null)
		{
			childById.OnPress += this.BtnShowPassword_OnPress;
		}
		XUiController childById2 = base.GetChildById("btnClearInput");
		if (childById2 != null)
		{
			childById2.OnPress += this.BtnClearInput_OnPress;
		}
		EventDelegate.Add(this.uiInput.onSubmit, new EventDelegate.Callback(this.OnSubmit));
		EventDelegate.Add(this.uiInput.onChange, new EventDelegate.Callback(this.OnChange));
		this.uiInput.onClipboard += this.OnClipboard;
		this.uiInput.label = ((XUiV_Label)this.text.ViewComponent).Label;
		this.uiInput.label.autoResizeBoxCollider = true;
		this.uiInput.value = (this.value ?? "");
		this.uiInput.activeTextColor = this.activeTextColor;
		this.uiInput.caretColor = this.caretColor;
		this.uiInput.selectionColor = this.selectionColor;
		this.uiInput.inputType = this.displayType;
		this.uiInput.validation = this.validation;
		this.uiInput.hideInput = this.hideInput;
		this.uiInput.onReturnKey = this.onReturnKey;
		this.uiInput.characterLimit = this.characterLimit;
		this.uiInput.keyboardType = this.inputType;
		this.uiInputController.OnSelect += this.InputFieldSelected;
	}

	// Token: 0x060073C6 RID: 29638 RVA: 0x002F2162 File Offset: 0x002F0362
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnShowPassword_OnPress(XUiController _sender, int _mouseButton)
	{
		this.displayType = ((this.displayType == UIInput.InputType.Password) ? UIInput.InputType.Standard : UIInput.InputType.Password);
		this.IsDirty = true;
	}

	// Token: 0x060073C7 RID: 29639 RVA: 0x002F217E File Offset: 0x002F037E
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnClearInput_OnPress(XUiController _sender, int _mouseButton)
	{
		this.Text = "";
		this.IsDirty = true;
	}

	// Token: 0x060073C8 RID: 29640 RVA: 0x002F2192 File Offset: 0x002F0392
	public void ShowVirtualKeyboard()
	{
		this.XUiC_TextInput_OnPress(this, -1);
	}

	// Token: 0x060073C9 RID: 29641 RVA: 0x002F219C File Offset: 0x002F039C
	[PublicizedFrom(EAccessModifier.Private)]
	public void InputFieldSelected(XUiController _sender, bool _selected)
	{
		if (_selected)
		{
			ThreadManager.StartCoroutine(this.removeSelection());
			return;
		}
		if (this.OnInputSelectedHandler != null)
		{
			this.OnInputSelectedHandler(this, false);
		}
	}

	// Token: 0x060073CA RID: 29642 RVA: 0x002F21C3 File Offset: 0x002F03C3
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator removeSelection()
	{
		yield return null;
		if (PlatformManager.NativePlatform.Input.CurrentInputStyle != PlayerInputManager.InputStyle.Keyboard)
		{
			this.SetSelected(false, false);
			this.XUiC_TextInput_OnPress(this, -1);
		}
		if (this.OnInputSelectedHandler != null)
		{
			this.OnInputSelectedHandler(this, true);
		}
		yield break;
	}

	// Token: 0x060073CB RID: 29643 RVA: 0x002F21D4 File Offset: 0x002F03D4
	[PublicizedFrom(EAccessModifier.Private)]
	public void XUiC_TextInput_OnPress(XUiController _sender, int _mouseButton)
	{
		if (!this.Enabled)
		{
			return;
		}
		IVirtualKeyboard virtualKeyboard = PlatformManager.NativePlatform.VirtualKeyboard;
		string text;
		if (virtualKeyboard == null)
		{
			text = Localization.Get("ttPlatformHasNoVirtualKeyboard", false);
		}
		else
		{
			uint singleLineLength = (uint)((this.characterLimit <= 0) ? 200 : this.characterLimit);
			text = virtualKeyboard.Open(this.virtKeyboardPrompt, this.uiInput.value, new Action<bool, string>(this.OnTextReceived), this.displayType, this.onReturnKey == UIInput.OnReturnKey.NewLine, singleLineLength);
		}
		if (text != null)
		{
			GameManager instance = GameManager.Instance;
			EntityPlayerLocal player;
			if (instance == null)
			{
				player = null;
			}
			else
			{
				World world = instance.World;
				player = ((world != null) ? world.GetPrimaryPlayer() : null);
			}
			GameManager.ShowTooltip(player, "[BB0000]" + text, false, false, 0f);
			if (this.OnInputErrorHandler != null)
			{
				this.OnInputErrorHandler(this, text);
			}
		}
	}

	// Token: 0x060073CC RID: 29644 RVA: 0x002F22A0 File Offset: 0x002F04A0
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnTextReceived(bool _success, string _text)
	{
		if (this.isOpen)
		{
			this.Text = _text;
			if (_success)
			{
				if (this.OnSubmitHandler != null)
				{
					this.OnSubmitHandler(this, _text);
				}
				else if (this.OnChangeHandler != null)
				{
					this.OnChangeHandler(this, _text, false);
				}
			}
			else if (this.OnInputAbortedHandler != null)
			{
				this.OnInputAbortedHandler(this);
			}
			this.uiInput.RemoveFocus();
		}
	}

	// Token: 0x060073CD RID: 29645 RVA: 0x002F2310 File Offset: 0x002F0510
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnSubmit()
	{
		string input = UIInput.current.value;
		this.uiInput.RemoveFocus();
		if (this.OnSubmitHandler != null)
		{
			ThreadManager.StartCoroutine(this.delaySubmitHandler(input));
		}
	}

	// Token: 0x060073CE RID: 29646 RVA: 0x002F2348 File Offset: 0x002F0548
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator delaySubmitHandler(string _input)
	{
		yield return null;
		XUiEvent_InputOnSubmitEventHandler onSubmitHandler = this.OnSubmitHandler;
		if (onSubmitHandler != null)
		{
			onSubmitHandler(this, _input);
		}
		yield break;
	}

	// Token: 0x060073CF RID: 29647 RVA: 0x002F235E File Offset: 0x002F055E
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnChange()
	{
		XUiEvent_InputOnChangedEventHandler onChangeHandler = this.OnChangeHandler;
		if (onChangeHandler == null)
		{
			return;
		}
		onChangeHandler(this, UIInput.current.value, this.textChangeFromCode);
	}

	// Token: 0x060073D0 RID: 29648 RVA: 0x002F2381 File Offset: 0x002F0581
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnClipboard(UIInput.ClipboardAction _actiontype, string _oldtext, int _selstart, int _selend, string _actionresulttext)
	{
		UIInput.OnClipboard onClipboardHandler = this.OnClipboardHandler;
		if (onClipboardHandler == null)
		{
			return;
		}
		onClipboardHandler(_actiontype, _oldtext, _selstart, _selend, _actionresulttext);
	}

	// Token: 0x060073D1 RID: 29649 RVA: 0x002F239C File Offset: 0x002F059C
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(name);
		if (num <= 2346532384U)
		{
			if (num <= 679658536U)
			{
				if (num <= 94319076U)
				{
					if (num != 38871051U)
					{
						if (num == 94319076U)
						{
							if (name == "input_type")
							{
								this.inputType = EnumUtils.Parse<UIInput.KeyboardType>(value, true);
							}
						}
					}
					else if (name == "clear_on_open")
					{
						this.clearOnOpen = StringParsers.ParseBool(value, 0, -1, true);
					}
				}
				else if (num != 226467462U)
				{
					if (num == 679658536U)
					{
						if (name == "caret_color")
						{
							this.caretColor = StringParsers.ParseColor32(value);
						}
					}
				}
				else if (name == "search_field")
				{
					this.isSearchField = StringParsers.ParseBool(value, 0, -1, true);
				}
			}
			else if (num <= 1114941561U)
			{
				if (num != 1113510858U)
				{
					if (num == 1114941561U)
					{
						if (name == "close_group_on_tab")
						{
							this.closeGroupOnTab = StringParsers.ParseBool(value, 0, -1, true);
						}
					}
				}
				else if (name == "value")
				{
					this.value = value;
				}
			}
			else if (num != 1682030439U)
			{
				if (num != 2196932415U)
				{
					if (num == 2346532384U)
					{
						if (name == "open_vk_on_open")
						{
							this.openVKOnOpen = StringParsers.ParseBool(value, 0, -1, true);
						}
					}
				}
				else if (name == "active_text_color")
				{
					this.ActiveTextColor = StringParsers.ParseColor32(value);
				}
			}
			else if (name == "password_field")
			{
				this.isPasswordField = StringParsers.ParseBool(value, 0, -1, true);
			}
		}
		else if (num <= 3198318611U)
		{
			if (num <= 2431563478U)
			{
				if (num != 2408036522U)
				{
					if (num == 2431563478U)
					{
						if (name == "focus_on_open")
						{
							this.focusOnOpen = StringParsers.ParseBool(value, 0, -1, true);
						}
					}
				}
				else if (name == "hide_input")
				{
					this.hideInput = StringParsers.ParseBool(value, 0, -1, true);
				}
			}
			else if (num != 2509030743U)
			{
				if (num != 2661558015U)
				{
					if (num == 3198318611U)
					{
						if (name == "virtual_keyboard_prompt")
						{
							this.virtKeyboardPrompt = Localization.Get(value, false);
						}
					}
				}
				else if (name == "ignore_up_down_keys")
				{
					this.ignoreUpDownKeys = StringParsers.ParseBool(value, 0, -1, true);
				}
			}
			else if (name == "clear_button")
			{
				this.hasClearButton = StringParsers.ParseBool(value, 0, -1, true);
			}
		}
		else if (num <= 3827651548U)
		{
			if (num != 3378670817U)
			{
				if (num == 3827651548U)
				{
					if (name == "use_virtual_keyboard")
					{
						this.useVirtualKeyboard = StringParsers.ParseBool(value, 0, -1, true);
					}
				}
			}
			else if (name == "selection_color")
			{
				this.selectionColor = StringParsers.ParseColor32(value);
			}
		}
		else if (num != 4029825831U)
		{
			if (num != 4061606266U)
			{
				if (num == 4181838876U)
				{
					if (name == "character_limit")
					{
						this.characterLimit = int.Parse(value);
					}
				}
			}
			else if (name == "validation")
			{
				this.validation = EnumUtils.Parse<UIInput.Validation>(value, true);
			}
		}
		else if (name == "on_return")
		{
			this.onReturnKey = EnumUtils.Parse<UIInput.OnReturnKey>(value, true);
		}
		return base.ParseAttribute(name, value, _parent);
	}

	// Token: 0x060073D2 RID: 29650 RVA: 0x002F27BC File Offset: 0x002F09BC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "clearbutton")
		{
			_value = this.hasClearButton.ToString();
			return true;
		}
		if (_bindingName == "passwordfield")
		{
			_value = this.isPasswordField.ToString();
			return true;
		}
		if (!(_bindingName == "showpassword"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		_value = (this.displayType == UIInput.InputType.Standard).ToString();
		return true;
	}

	// Token: 0x060073D3 RID: 29651 RVA: 0x002F2830 File Offset: 0x002F0A30
	public override void OnOpen()
	{
		base.OnOpen();
		((XUiV_Label)this.text.ViewComponent).Text = this.uiInput.value;
		this.IsDirty = true;
		this.isOpen = true;
		this.openCompleted = false;
		this.removeFocusOnOpen = true;
		if (this.isPasswordField)
		{
			this.displayType = UIInput.InputType.Password;
			this.uiInput.UpdateLabel();
		}
		if (this.isSearchField)
		{
			XUiC_TextInput.currentSearchField = this;
		}
		if (this.clearOnOpen)
		{
			this.Text = "";
		}
		if (this.focusOnOpen)
		{
			this.SetSelected(true, true);
		}
		if (this.openVKOnOpen)
		{
			this.SelectOrVirtualKeyboard(true);
		}
	}

	// Token: 0x060073D4 RID: 29652 RVA: 0x002F28D9 File Offset: 0x002F0AD9
	public override void OnClose()
	{
		this.SetSelected(false, false);
		base.OnClose();
		this.isOpen = false;
		if (XUiC_TextInput.currentSearchField == this)
		{
			XUiC_TextInput.currentSearchField = null;
		}
	}

	// Token: 0x060073D5 RID: 29653 RVA: 0x002F2900 File Offset: 0x002F0B00
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (base.xui.playerUI.CursorController.GetMouseButton(UICamera.MouseButton.LeftButton) && this.uiInput.isSelected && UICamera.hoveredObject != this.uiInputController.ViewComponent.UiTransform.gameObject)
		{
			this.uiInput.isSelected = false;
		}
		if (this.closeGroupOnTab && this.uiInput.isSelected && this.selectOnTab == null)
		{
			PlayerAction inventory = base.xui.playerUI.playerInput.PermanentActions.Inventory;
			KeyBindingSource keyBindingSource = inventory.GetBindingOfType(false) as KeyBindingSource;
			if (inventory.WasPressed && keyBindingSource != null && keyBindingSource.Control == XUiC_TextInput.tabCombo)
			{
				ThreadManager.StartCoroutine(this.closeOnTabLater());
			}
		}
		if (this.IsDirty)
		{
			this.uiInput.inputType = this.displayType;
			this.uiInput.activeTextColor = this.activeTextColor;
			this.uiInput.UpdateLabel();
			if (!this.openCompleted && this.removeFocusOnOpen)
			{
				this.uiInput.RemoveFocus();
			}
			this.IsDirty = false;
			base.RefreshBindings(false);
			if (!this.openCompleted)
			{
				this.openCompleted = true;
			}
		}
	}

	// Token: 0x060073D6 RID: 29654 RVA: 0x002F2A45 File Offset: 0x002F0C45
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator closeOnTabLater()
	{
		PlayerAction inventoryAction = base.xui.playerUI.playerInput.PermanentActions.Inventory;
		KeyBindingSource keyBindingSource = inventoryAction.GetBindingOfType(false) as KeyBindingSource;
		if (keyBindingSource == null || keyBindingSource.Control != XUiC_TextInput.tabCombo)
		{
			yield break;
		}
		while (inventoryAction.IsPressed)
		{
			yield return null;
		}
		if (this.closeGroupOnTab && this.uiInput.isSelected && this.selectOnTab == null)
		{
			base.xui.playerUI.windowManager.Close(this.windowGroup, false);
		}
		yield break;
	}

	// Token: 0x060073D7 RID: 29655 RVA: 0x002F2A54 File Offset: 0x002F0C54
	public void SetSelected(bool _selected = true, bool _delayed = false)
	{
		ThreadManager.StartCoroutine(this.setSelectedDelayed(_selected, _delayed));
	}

	// Token: 0x060073D8 RID: 29656 RVA: 0x002F2A64 File Offset: 0x002F0C64
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator setSelectedDelayed(bool _selected, bool _delayed)
	{
		if (_delayed)
		{
			yield return null;
		}
		if (!_selected || PlatformManager.NativePlatform.Input.CurrentInputStyle == PlayerInputManager.InputStyle.Keyboard)
		{
			this.uiInput.isSelected = _selected;
			if (!this.openCompleted)
			{
				this.removeFocusOnOpen = false;
			}
		}
		yield break;
	}

	// Token: 0x060073D9 RID: 29657 RVA: 0x002F2A81 File Offset: 0x002F0C81
	public void SelectOrVirtualKeyboard(bool _delayed = false)
	{
		if (PlatformManager.NativePlatform.Input.CurrentInputStyle == PlayerInputManager.InputStyle.Keyboard)
		{
			this.SetSelected(true, _delayed);
			return;
		}
		this.ShowVirtualKeyboard();
	}

	// Token: 0x0400580A RID: 22538
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController text;

	// Token: 0x0400580B RID: 22539
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController uiInputController;

	// Token: 0x0400580C RID: 22540
	[PublicizedFrom(EAccessModifier.Private)]
	public UIInput uiInput;

	// Token: 0x0400580D RID: 22541
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput selectOnTab;

	// Token: 0x0400580E RID: 22542
	[PublicizedFrom(EAccessModifier.Private)]
	public string value;

	// Token: 0x0400580F RID: 22543
	[PublicizedFrom(EAccessModifier.Private)]
	public Color activeTextColor;

	// Token: 0x04005810 RID: 22544
	[PublicizedFrom(EAccessModifier.Private)]
	public Color caretColor;

	// Token: 0x04005811 RID: 22545
	[PublicizedFrom(EAccessModifier.Private)]
	public Color selectionColor;

	// Token: 0x04005812 RID: 22546
	[PublicizedFrom(EAccessModifier.Private)]
	public UIInput.InputType displayType;

	// Token: 0x04005813 RID: 22547
	[PublicizedFrom(EAccessModifier.Private)]
	public UIInput.Validation validation;

	// Token: 0x04005814 RID: 22548
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hideInput;

	// Token: 0x04005815 RID: 22549
	[PublicizedFrom(EAccessModifier.Private)]
	public UIInput.OnReturnKey onReturnKey = UIInput.OnReturnKey.Submit;

	// Token: 0x04005816 RID: 22550
	[PublicizedFrom(EAccessModifier.Private)]
	public int characterLimit;

	// Token: 0x04005817 RID: 22551
	[PublicizedFrom(EAccessModifier.Private)]
	public UIInput.KeyboardType inputType;

	// Token: 0x04005818 RID: 22552
	[PublicizedFrom(EAccessModifier.Private)]
	public bool ignoreUpDownKeys;

	// Token: 0x04005819 RID: 22553
	public bool useVirtualKeyboard;

	// Token: 0x0400581A RID: 22554
	[PublicizedFrom(EAccessModifier.Private)]
	public string virtKeyboardPrompt = "";

	// Token: 0x0400581B RID: 22555
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isOpen;

	// Token: 0x0400581C RID: 22556
	[PublicizedFrom(EAccessModifier.Private)]
	public bool openCompleted;

	// Token: 0x0400581D RID: 22557
	[PublicizedFrom(EAccessModifier.Private)]
	public bool removeFocusOnOpen = true;

	// Token: 0x0400581E RID: 22558
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isSearchField;

	// Token: 0x0400581F RID: 22559
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasClearButton;

	// Token: 0x04005820 RID: 22560
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isPasswordField;

	// Token: 0x04005821 RID: 22561
	[PublicizedFrom(EAccessModifier.Private)]
	public bool focusOnOpen;

	// Token: 0x04005822 RID: 22562
	[PublicizedFrom(EAccessModifier.Private)]
	public bool openVKOnOpen;

	// Token: 0x04005823 RID: 22563
	[PublicizedFrom(EAccessModifier.Private)]
	public bool clearOnOpen;

	// Token: 0x04005824 RID: 22564
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly KeyCombo tabCombo = new KeyCombo(new Key[]
	{
		Key.Tab
	});

	// Token: 0x04005825 RID: 22565
	[PublicizedFrom(EAccessModifier.Private)]
	public bool closeGroupOnTab;

	// Token: 0x04005826 RID: 22566
	[PublicizedFrom(EAccessModifier.Private)]
	public bool textChangeFromCode;

	// Token: 0x04005827 RID: 22567
	[PublicizedFrom(EAccessModifier.Private)]
	public static XUiC_TextInput currentSearchField;
}
