using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using InControl;
using Platform;
using UnityEngine;

// Token: 0x02000F27 RID: 3879
public static class XUiUtils
{
	// Token: 0x06007BD7 RID: 31703 RVA: 0x00321D6C File Offset: 0x0031FF6C
	public static string GetBindingXuiMarkupString(this PlayerAction _action, XUiUtils.EmptyBindingStyle _emptyStyle = XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle _displayStyle = XUiUtils.DisplayStyle.Plain, string _customDisplayStyle = null)
	{
		if (_action == null)
		{
			return "";
		}
		string name = ((PlayerActionsBase)_action.Owner).Name;
		string name2 = _action.Name;
		bool flag = _emptyStyle > XUiUtils.EmptyBindingStyle.EmptyString;
		bool flag2 = !string.IsNullOrEmpty(_customDisplayStyle);
		bool flag3 = _displayStyle > XUiUtils.DisplayStyle.Plain || flag2;
		string text = (flag || flag3) ? (":" + _emptyStyle.ToStringCached<XUiUtils.EmptyBindingStyle>()) : "";
		string text2 = flag3 ? (":" + (flag2 ? _customDisplayStyle : _displayStyle.ToStringCached<XUiUtils.DisplayStyle>())) : "";
		return string.Concat(new string[]
		{
			"[action:",
			name,
			":",
			name2,
			text,
			text2,
			"]"
		});
	}

	// Token: 0x06007BD8 RID: 31704 RVA: 0x00321E24 File Offset: 0x00320024
	public static bool ParseActionsMarkup(XUi _xui, string _input, out string _parsed, string _defaultCustomFormat = null, XUiUtils.ForceLabelInputStyle _forceInputStyle = XUiUtils.ForceLabelInputStyle.Off)
	{
		bool result = false;
		int num;
		while ((num = _input.IndexOf("[action:", StringComparison.OrdinalIgnoreCase)) >= 0)
		{
			int num2 = num + "[action:".Length;
			int num3 = _input.IndexOf(':', num2);
			int num4 = _input.IndexOf(':', num3 + 1);
			int num5 = _input.IndexOf(':', num4 + 1);
			int i = 0;
			int num6 = num2;
			while (i >= 0)
			{
				int num7 = _input.IndexOf('[', num6);
				int num8 = _input.IndexOf(']', num6);
				bool flag = num7 >= 0;
				bool flag2 = num8 >= 0;
				if (flag && num7 < num8)
				{
					i++;
					num6 = num7 + 1;
				}
				else
				{
					if (!flag2)
					{
						break;
					}
					i--;
					num6 = num8 + 1;
				}
			}
			if (i >= 0)
			{
				Log.Warning("[XUi] Could not parse action descriptor in label text, no closing bracket found");
			}
			else
			{
				int num9 = num6 - 1;
				bool flag3 = num4 >= 0 && num4 < num9;
				bool flag4 = flag3 && num5 >= 0 && num5 < num9;
				if (num9 < 0)
				{
					Log.Warning("[XUi] Could not parse action descriptor in label text, no closing bracket found");
				}
				else if (num3 < 0 || num3 > num9)
				{
					Log.Warning("[XUi] Could not parse action descriptor in label text, no separator between action set name and action found");
				}
				else
				{
					int num10 = flag3 ? (num4 - 1) : (num9 - 1);
					int num11 = flag4 ? (num5 - 1) : (num9 - 1);
					string text = _input.Substring(num2, num3 - num2);
					string text2 = _input.Substring(num3 + 1, num10 - num3);
					string text3 = flag3 ? _input.Substring(num4 + 1, num11 - num4) : null;
					string text4 = flag4 ? _input.Substring(num5 + 1, num9 - num5 - 1) : null;
					PlayerActionsBase actionSetForName = PlatformManager.NativePlatform.Input.GetActionSetForName(text);
					if (actionSetForName == null)
					{
						Log.Warning("[XUi] Could not parse action descriptor in label text, action set \"" + text + "\" not found");
					}
					else
					{
						PlayerAction playerActionByName = actionSetForName.GetPlayerActionByName(text2);
						if (playerActionByName != null)
						{
							XUiUtils.EmptyBindingStyle emptyStyle = XUiUtils.EmptyBindingStyle.EmptyString;
							if (flag3 && text3.Length > 0 && !EnumUtils.TryParse<XUiUtils.EmptyBindingStyle>(text3, out emptyStyle, true))
							{
								Log.Warning("[XUi] Could not parse action descriptor empty style, \"" + text3 + "\" unknown");
							}
							bool isCustomDisplayStyle = false;
							XUiUtils.DisplayStyle displayStyle = XUiUtils.DisplayStyle.Plain;
							if (flag4 && !EnumUtils.TryParse<XUiUtils.DisplayStyle>(text4, out displayStyle, true))
							{
								if (text4.Length < 1)
								{
									Log.Warning("[XUi] Could not parse action descriptor display type, \"" + text4 + "\" unknown");
								}
								else if (text4.IndexOf("###", StringComparison.Ordinal) < 0)
								{
									Log.Warning("[XUi] Could not parse action descriptor display type, \"" + text4 + "\" assumed to be a custom format, missing the '#' placeholder");
								}
								else
								{
									isCustomDisplayStyle = true;
								}
							}
							if (!flag4 && !string.IsNullOrEmpty(_defaultCustomFormat))
							{
								isCustomDisplayStyle = true;
								text4 = _defaultCustomFormat;
							}
							string bindingString = playerActionByName.GetBindingString(_forceInputStyle != XUiUtils.ForceLabelInputStyle.Keyboard && (PlatformManager.NativePlatform.Input.CurrentInputStyle != PlayerInputManager.InputStyle.Keyboard || _forceInputStyle == XUiUtils.ForceLabelInputStyle.Controller), XUiUtils.GetInputStyleFromForcedStyle(_forceInputStyle), emptyStyle, displayStyle, isCustomDisplayStyle, text4);
							_input = _input.Remove(num, num9 - num + 1);
							_input = _input.Insert(num, bindingString);
							result = true;
							continue;
						}
						Log.Warning("[XUi] Could not parse action descriptor in label text, action \"" + text2 + "\" not found");
					}
				}
			}
			IL_337:
			while ((num = _input.IndexOf("[button:", StringComparison.OrdinalIgnoreCase)) >= 0)
			{
				int num12 = num + "[button:".Length;
				int num13 = _input.IndexOf(']', num);
				if (num13 < 0)
				{
					Log.Warning("[XUi] Could not parse button descriptor in label text, no closing bracket found");
					break;
				}
				string value = InControlExtensions.TryLocalizeButtonName(_input.Substring(num12, num13 - num12));
				_input = _input.Remove(num, num13 - num + 1);
				_input = _input.Insert(num, value);
				result = true;
			}
			_parsed = _input;
			return result;
		}
		goto IL_337;
	}

	// Token: 0x06007BD9 RID: 31705 RVA: 0x0032217D File Offset: 0x0032037D
	[PublicizedFrom(EAccessModifier.Private)]
	public static PlayerInputManager.InputStyle GetInputStyleFromForcedStyle(XUiUtils.ForceLabelInputStyle _forceStyle)
	{
		if (_forceStyle == XUiUtils.ForceLabelInputStyle.Keyboard)
		{
			return PlayerInputManager.InputStyle.Keyboard;
		}
		if (_forceStyle != XUiUtils.ForceLabelInputStyle.Controller)
		{
			return PlatformManager.NativePlatform.Input.CurrentInputStyle;
		}
		return PlatformManager.NativePlatform.Input.CurrentControllerInputStyle;
	}

	// Token: 0x06007BDA RID: 31706 RVA: 0x003221AC File Offset: 0x003203AC
	public static string GetXuiHierarchy(this XUiController _current)
	{
		StringBuilder stringBuilder = new StringBuilder();
		XUiUtils.getXuiHierarchyRec(_current, stringBuilder);
		return stringBuilder.ToString();
	}

	// Token: 0x06007BDB RID: 31707 RVA: 0x003221CC File Offset: 0x003203CC
	[PublicizedFrom(EAccessModifier.Private)]
	public static void getXuiHierarchyRec(XUiController _current, StringBuilder _sb)
	{
		if (_current.Parent != null)
		{
			XUiUtils.getXuiHierarchyRec(_current.Parent, _sb);
			_sb.Append(" -> ");
		}
		string text;
		string id;
		if (_current.ViewComponent != null)
		{
			text = _current.ViewComponent.GetType().Name.Replace("XUiV_", "");
			id = _current.ViewComponent.ID;
		}
		else
		{
			text = "windowgroup";
			id = _current.WindowGroup.ID;
		}
		if (text.EqualsCaseInsensitive(id))
		{
			_sb.Append(id);
			return;
		}
		_sb.Append(text);
		_sb.Append(" (");
		_sb.Append(id);
		_sb.Append(")");
	}

	// Token: 0x06007BDC RID: 31708 RVA: 0x00322280 File Offset: 0x00320480
	public static string ToXuiColorString(this Color32 _color)
	{
		return string.Format("{0},{1},{2},{3}", new object[]
		{
			_color.r,
			_color.g,
			_color.b,
			_color.a
		});
	}

	// Token: 0x06007BDD RID: 31709 RVA: 0x003222D8 File Offset: 0x003204D8
	[PublicizedFrom(EAccessModifier.Private)]
	static XUiUtils()
	{
		XUiUtils.RegisterLabelUrlHandler("Chat", new XUiUtils.HandleTextUrlDelegate(XUiUtils.handleChatTargetUrlClick));
		XUiUtils.RegisterLabelUrlHandler("HTTP", new XUiUtils.HandleTextUrlDelegate(XUiUtils.handleHttpUrl));
	}

	// Token: 0x06007BDE RID: 31710 RVA: 0x0032232C File Offset: 0x0032052C
	public static void RegisterLabelUrlHandler(string _type, XUiUtils.HandleTextUrlDelegate _handler)
	{
		if (XUiUtils.urlHandlers.ContainsKey(_type))
		{
			Log.Warning("Register a new label URL handler for the type '" + _type + "'");
		}
		XUiUtils.urlHandlers[_type] = _handler;
	}

	// Token: 0x06007BDF RID: 31711 RVA: 0x0032235C File Offset: 0x0032055C
	public static void HandleLabelUrlClick(XUiView _view, UILabel _label, HashSet<string> _allowedTypes = null)
	{
		string urlAtPosition = _label.GetUrlAtPosition(UICamera.lastWorldPosition);
		if (string.IsNullOrEmpty(urlAtPosition))
		{
			return;
		}
		Dictionary<string, string> dictionary2;
		if (urlAtPosition.StartsWith("http://") || urlAtPosition.StartsWith("https://"))
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["Type"] = "HTTP";
			dictionary["Target"] = urlAtPosition;
			dictionary2 = dictionary;
		}
		else
		{
			MatchCollection matchCollection = XUiUtils.urlMatcher.Matches(urlAtPosition);
			if (matchCollection.Count == 0)
			{
				Log.Warning("Text URL ('" + urlAtPosition + "'): Invalid URL");
				return;
			}
			dictionary2 = new Dictionary<string, string>();
			foreach (object obj in matchCollection)
			{
				Match match = (Match)obj;
				dictionary2[match.Groups[1].Value] = match.Groups[2].Value;
			}
		}
		string text;
		if (!dictionary2.TryGetValue("Type", out text))
		{
			Log.Warning(string.Concat(new string[]
			{
				"Text URL ('",
				urlAtPosition,
				"'): No explicit or implicit type defined in '",
				urlAtPosition,
				"'"
			}));
			return;
		}
		XUiUtils.HandleTextUrlDelegate handleTextUrlDelegate;
		if (!XUiUtils.urlHandlers.TryGetValue(text, out handleTextUrlDelegate))
		{
			Log.Warning(string.Concat(new string[]
			{
				"Text URL ('",
				urlAtPosition,
				"'): No handler for type '",
				text,
				"'"
			}));
			return;
		}
		if (_allowedTypes != null && !_allowedTypes.Contains(text))
		{
			Log.Warning(string.Concat(new string[]
			{
				"Text URL ('",
				urlAtPosition,
				"'): URL type '",
				text,
				"' not allowed on this label"
			}));
			return;
		}
		handleTextUrlDelegate(_view, urlAtPosition, dictionary2);
	}

	// Token: 0x06007BE0 RID: 31712 RVA: 0x0032252C File Offset: 0x0032072C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void handleChatTargetUrlClick(XUiView _sender, string _sourceUrl, Dictionary<string, string> _urlElements)
	{
		string text;
		if (!_urlElements.TryGetValue("ChatType", out text))
		{
			Log.Warning("Chat URL ('" + _sourceUrl + "'): No ChatType defined");
			return;
		}
		string targetId;
		_urlElements.TryGetValue("Sender", out targetId);
		EChatType chatType;
		if (!EnumUtils.TryParse<EChatType>(text, out chatType, false))
		{
			Log.Warning(string.Concat(new string[]
			{
				"Chat URL ('",
				_sourceUrl,
				"'): Invalid chat type value '",
				text,
				"'"
			}));
			return;
		}
		XUiC_Chat.SetChatTarget(_sender.xui, chatType, targetId);
	}

	// Token: 0x06007BE1 RID: 31713 RVA: 0x003225B8 File Offset: 0x003207B8
	[PublicizedFrom(EAccessModifier.Private)]
	public static void handleHttpUrl(XUiView _sender, string _sourceUrl, Dictionary<string, string> _urlElements)
	{
		string url;
		if (!_urlElements.TryGetValue("Target", out url))
		{
			Log.Warning("Web URL (" + _sourceUrl + "): No Target defined");
			return;
		}
		XUiC_MessageBoxWindowGroup.ShowUrlConfirmationDialog(_sender.xui, url, false, null, null, null, null);
	}

	// Token: 0x06007BE2 RID: 31714 RVA: 0x003225FB File Offset: 0x003207FB
	public static string BuildUrlFunctionString(string _type)
	{
		return "[url=Type>" + _type + "]";
	}

	// Token: 0x06007BE3 RID: 31715 RVA: 0x00322610 File Offset: 0x00320810
	public static string BuildUrlFunctionString(string _type, [TupleElementNames(new string[]
	{
		"key",
		"value"
	})] ValueTuple<string, string> _param1)
	{
		return string.Concat(new string[]
		{
			"[url=Type>",
			_type,
			"|",
			_param1.Item1,
			">",
			_param1.Item2,
			"]"
		});
	}

	// Token: 0x06007BE4 RID: 31716 RVA: 0x00322660 File Offset: 0x00320860
	public static string BuildUrlFunctionString(string _type, [TupleElementNames(new string[]
	{
		"key",
		"value"
	})] ValueTuple<string, string> _param1, [TupleElementNames(new string[]
	{
		"key",
		"value"
	})] ValueTuple<string, string> _param2)
	{
		return string.Concat(new string[]
		{
			"[url=Type>",
			_type,
			"|",
			_param1.Item1,
			">",
			_param1.Item2,
			"|",
			_param2.Item1,
			">",
			_param2.Item2,
			"]"
		});
	}

	// Token: 0x06007BE5 RID: 31717 RVA: 0x003226D4 File Offset: 0x003208D4
	public static string BuildUrlFunctionString(string _type, [TupleElementNames(new string[]
	{
		"key",
		"value"
	})] ValueTuple<string, string> _param1, [TupleElementNames(new string[]
	{
		"key",
		"value"
	})] ValueTuple<string, string> _param2, [TupleElementNames(new string[]
	{
		"key",
		"value"
	})] ValueTuple<string, string> _param3)
	{
		return string.Concat(new string[]
		{
			"[url=Type>",
			_type,
			"|",
			_param1.Item1,
			">",
			_param1.Item2,
			"|",
			_param2.Item1,
			">",
			_param2.Item2,
			"|",
			_param3.Item1,
			">",
			_param3.Item2,
			"]"
		});
	}

	// Token: 0x04005DAA RID: 23978
	public const string LabelUrlTypeFieldName = "Type";

	// Token: 0x04005DAB RID: 23979
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Dictionary<string, XUiUtils.HandleTextUrlDelegate> urlHandlers = new Dictionary<string, XUiUtils.HandleTextUrlDelegate>();

	// Token: 0x04005DAC RID: 23980
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Regex urlMatcher = new Regex("([^|]+)>([^|]+)", RegexOptions.Compiled | RegexOptions.Singleline);

	// Token: 0x02000F28 RID: 3880
	public enum EmptyBindingStyle
	{
		// Token: 0x04005DAE RID: 23982
		EmptyString,
		// Token: 0x04005DAF RID: 23983
		NullString,
		// Token: 0x04005DB0 RID: 23984
		LocalizedUnbound,
		// Token: 0x04005DB1 RID: 23985
		LocalizedNone
	}

	// Token: 0x02000F29 RID: 3881
	public enum DisplayStyle
	{
		// Token: 0x04005DB3 RID: 23987
		Plain,
		// Token: 0x04005DB4 RID: 23988
		KeyboardWithAngleBrackets,
		// Token: 0x04005DB5 RID: 23989
		KeyboardWithParentheses
	}

	// Token: 0x02000F2A RID: 3882
	public enum ForceLabelInputStyle
	{
		// Token: 0x04005DB7 RID: 23991
		Off,
		// Token: 0x04005DB8 RID: 23992
		Keyboard,
		// Token: 0x04005DB9 RID: 23993
		Controller
	}

	// Token: 0x02000F2B RID: 3883
	// (Invoke) Token: 0x06007BE7 RID: 31719
	public delegate void HandleTextUrlDelegate(XUiView _sender, string _sourceUrl, Dictionary<string, string> _urlElements);
}
