using System;
using System.Collections.Generic;
using System.Text;
using InControl;
using Platform;

// Token: 0x020004D6 RID: 1238
public static class InControlExtensions
{
	// Token: 0x06002837 RID: 10295 RVA: 0x001053B0 File Offset: 0x001035B0
	public static string GetBindingString(this PlayerAction _action, bool _forController, PlayerInputManager.InputStyle _inputStyle = PlayerInputManager.InputStyle.Undefined, XUiUtils.EmptyBindingStyle _emptyStyle = XUiUtils.EmptyBindingStyle.LocalizedNone, XUiUtils.DisplayStyle _displayStyle = XUiUtils.DisplayStyle.Plain, bool _isCustomDisplayStyle = false, string _customDisplayStyleString = null)
	{
		if (_action != null)
		{
			string name = _action.Name;
			if (!(name == "GUI Action Up"))
			{
				if (!(name == "GUI Action Down"))
				{
					if (!(name == "GUI Action Left"))
					{
						if (name == "GUI Action Right")
						{
							if (_forController)
							{
								return LocalPlayerUI.primaryUI.playerInput.GUIActions.Cancel.GetBindingOfType(true).GetBindingSourceString(_inputStyle, _emptyStyle, _displayStyle, _isCustomDisplayStyle, _customDisplayStyleString);
							}
							return LocalPlayerUI.primaryUI.playerInput.GUIActions.DPad_Right.GetBindingOfType(false).GetBindingSourceString(_inputStyle, _emptyStyle, _displayStyle, _isCustomDisplayStyle, _customDisplayStyleString);
						}
					}
					else
					{
						if (_forController)
						{
							return LocalPlayerUI.primaryUI.playerInput.GUIActions.HalfStack.GetBindingOfType(true).GetBindingSourceString(_inputStyle, _emptyStyle, _displayStyle, _isCustomDisplayStyle, _customDisplayStyleString);
						}
						return LocalPlayerUI.primaryUI.playerInput.GUIActions.DPad_Left.GetBindingOfType(false).GetBindingSourceString(_inputStyle, _emptyStyle, _displayStyle, _isCustomDisplayStyle, _customDisplayStyleString);
					}
				}
				else
				{
					if (_forController)
					{
						return LocalPlayerUI.primaryUI.playerInput.GUIActions.Submit.GetBindingOfType(true).GetBindingSourceString(_inputStyle, _emptyStyle, _displayStyle, _isCustomDisplayStyle, _customDisplayStyleString);
					}
					return LocalPlayerUI.primaryUI.playerInput.GUIActions.DPad_Down.GetBindingOfType(false).GetBindingSourceString(_inputStyle, _emptyStyle, _displayStyle, _isCustomDisplayStyle, _customDisplayStyleString);
				}
			}
			else
			{
				if (_forController)
				{
					return LocalPlayerUI.primaryUI.playerInput.GUIActions.Inspect.GetBindingOfType(true).GetBindingSourceString(_inputStyle, _emptyStyle, _displayStyle, _isCustomDisplayStyle, _customDisplayStyleString);
				}
				return LocalPlayerUI.primaryUI.playerInput.GUIActions.DPad_Up.GetBindingOfType(false).GetBindingSourceString(_inputStyle, _emptyStyle, _displayStyle, _isCustomDisplayStyle, _customDisplayStyleString);
			}
		}
		return _action.GetBindingOfType(_forController).GetBindingSourceString(_inputStyle, _emptyStyle, _displayStyle, _isCustomDisplayStyle, _customDisplayStyleString);
	}

	// Token: 0x06002838 RID: 10296 RVA: 0x0010556C File Offset: 0x0010376C
	[PublicizedFrom(EAccessModifier.Private)]
	public static string GetBindingSourceString(this BindingSource _bs, PlayerInputManager.InputStyle _inputStyle = PlayerInputManager.InputStyle.Undefined, XUiUtils.EmptyBindingStyle _emptyStyle = XUiUtils.EmptyBindingStyle.LocalizedNone, XUiUtils.DisplayStyle _displayStyle = XUiUtils.DisplayStyle.Plain, bool _isCustomDisplayStyle = false, string _customDisplayStyleString = null)
	{
		string result;
		if (_bs == null)
		{
			switch (_emptyStyle)
			{
			case XUiUtils.EmptyBindingStyle.EmptyString:
				result = string.Empty;
				break;
			case XUiUtils.EmptyBindingStyle.NullString:
				result = null;
				break;
			case XUiUtils.EmptyBindingStyle.LocalizedUnbound:
				result = InControlExtensions.TryLocalizeButtonName("Unbound");
				break;
			case XUiUtils.EmptyBindingStyle.LocalizedNone:
				result = InControlExtensions.TryLocalizeButtonName("None");
				break;
			default:
				throw new ArgumentOutOfRangeException("_emptyStyle", _emptyStyle, null);
			}
			return result;
		}
		switch (_bs.BindingSourceType)
		{
		case BindingSourceType.DeviceBindingSource:
			result = InControlExtensions.GetGamepadSourceString(((DeviceBindingSource)_bs).Control);
			break;
		case BindingSourceType.KeyBindingSource:
			result = InControlExtensions.GetKeyboardSourceString((KeyBindingSource)_bs, _displayStyle, _isCustomDisplayStyle, _customDisplayStyleString);
			break;
		case BindingSourceType.MouseBindingSource:
			result = InControlExtensions.GetMouseSourceString(((MouseBindingSource)_bs).Control);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		return result;
	}

	// Token: 0x06002839 RID: 10297 RVA: 0x00105634 File Offset: 0x00103834
	[PublicizedFrom(EAccessModifier.Private)]
	public static string GetKeyboardSourceString(KeyBindingSource _kbs, XUiUtils.DisplayStyle _displayStyle, bool _isCustomDisplayStyle, string _customDisplayStyleString)
	{
		KeyCombo control = _kbs.Control;
		InControlExtensions.bindingToString.Clear();
		for (int i = 0; i < control.IncludeCount; i++)
		{
			if (i > 0)
			{
				InControlExtensions.bindingToString.Append(" + ");
			}
			string localizedName = control.GetInclude(i).GetLocalizedName();
			if (_isCustomDisplayStyle)
			{
				InControlExtensions.bindingToString.Append(_customDisplayStyleString.Replace("###", localizedName));
			}
			else
			{
				switch (_displayStyle)
				{
				case XUiUtils.DisplayStyle.Plain:
					InControlExtensions.bindingToString.Append(localizedName);
					break;
				case XUiUtils.DisplayStyle.KeyboardWithAngleBrackets:
					InControlExtensions.bindingToString.Append("<");
					InControlExtensions.bindingToString.Append(localizedName);
					InControlExtensions.bindingToString.Append(">");
					break;
				case XUiUtils.DisplayStyle.KeyboardWithParentheses:
					InControlExtensions.bindingToString.Append("( ");
					InControlExtensions.bindingToString.Append(localizedName);
					InControlExtensions.bindingToString.Append(" )");
					break;
				default:
					throw new ArgumentOutOfRangeException("_displayStyle");
				}
			}
		}
		return InControlExtensions.bindingToString.ToString();
	}

	// Token: 0x0600283A RID: 10298 RVA: 0x00105740 File Offset: 0x00103940
	[PublicizedFrom(EAccessModifier.Private)]
	public static string GetMouseSourceString(Mouse _control)
	{
		string result;
		switch (_control)
		{
		case Mouse.LeftButton:
			result = "[sp=Mouse_LeftButton_Large]";
			break;
		case Mouse.RightButton:
			result = "[sp=Mouse_RightButton_Large]";
			break;
		case Mouse.MiddleButton:
			result = "[sp=Mouse_MiddleButton_Large]";
			break;
		default:
			result = _control.GetLocalizedName();
			break;
		}
		return result;
	}

	// Token: 0x0600283B RID: 10299 RVA: 0x00105784 File Offset: 0x00103984
	public static string GetGamepadSourceString(InputControlType _control)
	{
		PlayerInputManager.InputStyle inputStyle = PlayerInputManager.InputStyleFromSelectedIconStyle();
		InputControlType inputControlType;
		switch (_control)
		{
		case InputControlType.Back:
		case InputControlType.Select:
		case InputControlType.View:
		case InputControlType.Minus:
			break;
		case InputControlType.Start:
		case InputControlType.Options:
		case InputControlType.Menu:
		case InputControlType.Plus:
			if (inputStyle != PlayerInputManager.InputStyle.PS4)
			{
				if (inputStyle != PlayerInputManager.InputStyle.XB1)
				{
					inputControlType = _control;
				}
				else
				{
					inputControlType = InputControlType.Menu;
				}
			}
			else
			{
				inputControlType = InputControlType.Options;
			}
			_control = inputControlType;
			goto IL_8C;
		case InputControlType.System:
		case InputControlType.Pause:
		case InputControlType.Share:
		case InputControlType.Home:
		case InputControlType.Power:
		case InputControlType.Capture:
		case InputControlType.Assistant:
			goto IL_8C;
		default:
			if (_control != InputControlType.TouchPadButton)
			{
				goto IL_8C;
			}
			break;
		}
		if (inputStyle != PlayerInputManager.InputStyle.PS4)
		{
			if (inputStyle != PlayerInputManager.InputStyle.XB1)
			{
				inputControlType = _control;
			}
			else
			{
				inputControlType = InputControlType.Back;
			}
		}
		else
		{
			inputControlType = InputControlType.TouchPadButton;
		}
		_control = inputControlType;
		IL_8C:
		string result;
		if (inputStyle != PlayerInputManager.InputStyle.PS4)
		{
			if (inputStyle != PlayerInputManager.InputStyle.XB1)
			{
				result = "[sp={_control.ToStringCached ()}]";
			}
			else
			{
				result = "[sp=XB_Button_" + _control.ToStringCached<InputControlType>() + "]";
			}
		}
		else
		{
			result = "[sp=PS5_Button_" + _control.ToStringCached<InputControlType>() + "]";
		}
		return result;
	}

	// Token: 0x0600283C RID: 10300 RVA: 0x0010585E File Offset: 0x00103A5E
	public static string GetBlankDPadSourceString()
	{
		if (PlayerInputManager.InputStyleFromSelectedIconStyle() == PlayerInputManager.InputStyle.PS4)
		{
			return "[sp=PS5_Button_DPadBlank]";
		}
		return "[sp=XB_Button_DPadBlank]";
	}

	// Token: 0x0600283D RID: 10301 RVA: 0x00105873 File Offset: 0x00103A73
	public static string GetStartButtonSourceString()
	{
		if (PlayerInputManager.InputStyleFromSelectedIconStyle() == PlayerInputManager.InputStyle.PS4)
		{
			return "[sp=PS5_Button_Options]";
		}
		return "[sp=XB_Button_Menu]";
	}

	// Token: 0x0600283E RID: 10302 RVA: 0x00105888 File Offset: 0x00103A88
	public static void SetApplyButtonString(XUiC_SimpleButton _button, string text_key)
	{
		if (PlatformManager.NativePlatform.Input.CurrentInputStyle == PlayerInputManager.InputStyle.Keyboard)
		{
			_button.Text = Localization.Get(text_key, false).ToUpper();
			return;
		}
		_button.Text = InControlExtensions.GetStartButtonSourceString() + " " + Localization.Get(text_key, false).ToUpper();
	}

	// Token: 0x0600283F RID: 10303 RVA: 0x001058DC File Offset: 0x00103ADC
	[PublicizedFrom(EAccessModifier.Private)]
	public static string GetLocalizedName(this Key _key)
	{
		string name = UnityKeyboardProvider.KeyMappings[(int)_key].Name;
		string text = "inpButton" + name.Replace(" ", "");
		string text2 = Localization.Get(text, false);
		if (text2 != text)
		{
			return text2;
		}
		return name;
	}

	// Token: 0x06002840 RID: 10304 RVA: 0x0010592C File Offset: 0x00103B2C
	[PublicizedFrom(EAccessModifier.Private)]
	public static string GetLocalizedName(this Mouse _key)
	{
		string text = "inpButton" + _key.ToStringCached<Mouse>();
		string text2 = Localization.Get(text, false);
		if (text2 != text)
		{
			return text2;
		}
		return _key.ToStringCached<Mouse>();
	}

	// Token: 0x06002841 RID: 10305 RVA: 0x00105964 File Offset: 0x00103B64
	public static string TryLocalizeButtonName(string _buttonName)
	{
		string text = "inpButton" + _buttonName.Replace(" ", "");
		string text2 = Localization.Get(text, false);
		if (text2 != text)
		{
			return text2;
		}
		return _buttonName;
	}

	// Token: 0x06002842 RID: 10306 RVA: 0x001059A0 File Offset: 0x00103BA0
	public static void UnbindBindingsOfType(this PlayerAction _action, bool _controller)
	{
		foreach (BindingSource bindingSource in _action.Bindings)
		{
			if (_controller == (bindingSource.BindingSourceType == BindingSourceType.DeviceBindingSource))
			{
				_action.RemoveBinding(bindingSource);
			}
		}
	}

	// Token: 0x06002843 RID: 10307 RVA: 0x001059FC File Offset: 0x00103BFC
	public static void UnbindBindingsOfType(this PlayerAction _action, BindingSourceType _bindingType)
	{
		foreach (BindingSource bindingSource in _action.Bindings)
		{
			if (bindingSource.BindingSourceType == _bindingType)
			{
				_action.RemoveBinding(bindingSource);
			}
		}
	}

	// Token: 0x06002844 RID: 10308 RVA: 0x00105A54 File Offset: 0x00103C54
	public static PlayerAction BindingUsed(this PlayerActionSet _actionSet, BindingSource _binding)
	{
		if (_binding == null)
		{
			return null;
		}
		int count = _actionSet.Actions.Count;
		for (int i = 0; i < count; i++)
		{
			if (_actionSet.Actions[i].HasBinding(_binding))
			{
				return _actionSet.Actions[i];
			}
		}
		return null;
	}

	// Token: 0x06002845 RID: 10309 RVA: 0x00105AA8 File Offset: 0x00103CA8
	public static BindingSource GetBindingOfType(this PlayerAction _action, bool _forController = false)
	{
		if (_action == null)
		{
			return null;
		}
		foreach (BindingSource bindingSource in _action.Bindings)
		{
			bool flag = BindingSourceType.KeyBindingSource == bindingSource.BindingSourceType || BindingSourceType.MouseBindingSource == bindingSource.BindingSourceType;
			if (_forController != flag)
			{
				return bindingSource;
			}
		}
		return null;
	}

	// Token: 0x06002846 RID: 10310 RVA: 0x00105B14 File Offset: 0x00103D14
	public static string GetNameForControlType(this InputDeviceProfile _profile, InputControlType _controlType)
	{
		foreach (InputControlMapping inputControlMapping in _profile.AnalogMappings)
		{
			if (inputControlMapping.Target == _controlType)
			{
				return inputControlMapping.Target.ToStringCached<InputControlType>();
			}
		}
		foreach (InputControlMapping inputControlMapping2 in _profile.ButtonMappings)
		{
			if (inputControlMapping2.Target == _controlType)
			{
				return inputControlMapping2.Target.ToStringCached<InputControlType>();
			}
		}
		return null;
	}

	// Token: 0x06002847 RID: 10311 RVA: 0x00105B80 File Offset: 0x00103D80
	public static void GetBoundAction(this InputControlType _controlType, PlayerActionSet[] _actionSets, IList<PlayerAction> _result)
	{
		_result.Clear();
		if (_actionSets == null || _actionSets.Length == 0)
		{
			return;
		}
		for (int i = 0; i < _actionSets.Length; i++)
		{
			foreach (PlayerAction playerAction in _actionSets[i].Actions)
			{
				foreach (BindingSource bindingSource in playerAction.UnfilteredBindings)
				{
					if (bindingSource.BindingSourceType == BindingSourceType.DeviceBindingSource && ((DeviceBindingSource)bindingSource).Control == _controlType)
					{
						_result.Add(playerAction);
					}
				}
			}
		}
	}

	// Token: 0x04001EEB RID: 7915
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly StringBuilder bindingToString = new StringBuilder();
}
