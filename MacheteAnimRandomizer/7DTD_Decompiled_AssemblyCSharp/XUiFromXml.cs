using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NCalc;
using NCalc.Domain;
using UnityEngine;

// Token: 0x02000F1F RID: 3871
public static class XUiFromXml
{
	// Token: 0x17000CFB RID: 3323
	// (get) Token: 0x06007BA8 RID: 31656 RVA: 0x0031F9E0 File Offset: 0x0031DBE0
	public static XUiFromXml.DebugLevel DebugXuiLoading
	{
		get
		{
			return XUiFromXml.debugXuiLoading;
		}
	}

	// Token: 0x06007BA9 RID: 31657 RVA: 0x0031F9E8 File Offset: 0x0031DBE8
	[PublicizedFrom(EAccessModifier.Private)]
	static XUiFromXml()
	{
		string launchArgument = GameUtils.GetLaunchArgument("debugxui");
		if (launchArgument == null)
		{
			XUiFromXml.debugXuiLoading = XUiFromXml.DebugLevel.Off;
			return;
		}
		if (launchArgument == "verbose")
		{
			XUiFromXml.debugXuiLoading = XUiFromXml.DebugLevel.Verbose;
			return;
		}
		XUiFromXml.debugXuiLoading = XUiFromXml.DebugLevel.Warning;
	}

	// Token: 0x06007BAA RID: 31658 RVA: 0x0031FA24 File Offset: 0x0031DC24
	public static void ClearLoadingData()
	{
		XUiFromXml.mainXuiXml = null;
		Dictionary<string, XElement> dictionary = XUiFromXml.windowData;
		if (dictionary != null)
		{
			dictionary.Clear();
		}
		XUiFromXml.windowData = null;
		Dictionary<string, XElement> dictionary2 = XUiFromXml.controlData;
		if (dictionary2 != null)
		{
			dictionary2.Clear();
		}
		XUiFromXml.controlData = null;
		IDictionary<string, int> dictionary3 = XUiFromXml.usedWindows;
		if (dictionary3 != null)
		{
			dictionary3.Clear();
		}
		XUiFromXml.usedWindows = null;
		Dictionary<string, Dictionary<string, object>> dictionary4 = XUiFromXml.controlDefaults;
		if (dictionary4 != null)
		{
			dictionary4.Clear();
		}
		XUiFromXml.controlDefaults = null;
		IDictionary<string, int> dictionary5 = XUiFromXml.usedControls;
		if (dictionary5 != null)
		{
			dictionary5.Clear();
		}
		XUiFromXml.usedControls = null;
		if (XUiFromXml.expressionCache != null)
		{
			Dictionary<string, Expression> dictionary6 = XUiFromXml.expressionCache;
			foreach (Expression expression in ((dictionary6 != null) ? dictionary6.Values : null))
			{
				expression.EvaluateFunction -= XUiFromXml.NCalcIdentifierDefinedFunction;
			}
		}
		Dictionary<string, Expression> dictionary7 = XUiFromXml.expressionCache;
		if (dictionary7 != null)
		{
			dictionary7.Clear();
		}
		XUiFromXml.expressionCache = null;
	}

	// Token: 0x06007BAB RID: 31659 RVA: 0x0031FB1C File Offset: 0x0031DD1C
	public static void ClearData()
	{
		XUiFromXml.ClearLoadingData();
		Dictionary<string, XUiFromXml.StyleData> dictionary = XUiFromXml.styles;
		if (dictionary != null)
		{
			dictionary.Clear();
		}
		XUiFromXml.styles = null;
	}

	// Token: 0x06007BAC RID: 31660 RVA: 0x0031FB39 File Offset: 0x0031DD39
	public static bool HasData()
	{
		return XUiFromXml.mainXuiXml != null && XUiFromXml.windowData.Count > 0 && XUiFromXml.controlData.Count > 0 && XUiFromXml.styles.Count > 0;
	}

	// Token: 0x06007BAD RID: 31661 RVA: 0x0031FB6B File Offset: 0x0031DD6B
	public static IEnumerator Load(XmlFile _xmlFile)
	{
		if (GameManager.IsDedicatedServer)
		{
			yield break;
		}
		if (XUi.Stopwatch == null)
		{
			XUi.Stopwatch = new MicroStopwatch();
		}
		if (!XUi.Stopwatch.IsRunning)
		{
			XUi.Stopwatch.Reset();
			XUi.Stopwatch.Start();
		}
		if (XUiFromXml.windowData == null)
		{
			XUiFromXml.windowData = new Dictionary<string, XElement>(StringComparer.Ordinal);
		}
		if (XUiFromXml.usedWindows == null)
		{
			XUiFromXml.usedWindows = new SortedDictionary<string, int>(StringComparer.Ordinal);
		}
		if (XUiFromXml.controlData == null)
		{
			XUiFromXml.controlData = new Dictionary<string, XElement>(StringComparer.Ordinal);
		}
		if (XUiFromXml.controlDefaults == null)
		{
			XUiFromXml.controlDefaults = new Dictionary<string, Dictionary<string, object>>(StringComparer.Ordinal);
		}
		if (XUiFromXml.usedControls == null)
		{
			XUiFromXml.usedControls = new SortedDictionary<string, int>(StringComparer.Ordinal);
		}
		if (XUiFromXml.styles == null)
		{
			XUiFromXml.styles = new CaseInsensitiveStringDictionary<XUiFromXml.StyleData>();
		}
		if (XUiFromXml.expressionCache == null)
		{
			XUiFromXml.expressionCache = new Dictionary<string, Expression>();
		}
		XElement root = _xmlFile.XmlDoc.Root;
		if (!root.HasElements)
		{
			throw new Exception("No elements found!");
		}
		string localName = root.Name.LocalName;
		if (!(localName == "xui"))
		{
			if (!(localName == "windows"))
			{
				if (!(localName == "styles"))
				{
					if (localName == "controls")
					{
						XUiFromXml.loadControls(_xmlFile);
					}
				}
				else
				{
					XUiFromXml.loadStyles(_xmlFile);
				}
			}
			else
			{
				XUiFromXml.loadWindows(_xmlFile);
			}
		}
		else
		{
			XUiFromXml.mainXuiXml = _xmlFile;
		}
		yield break;
	}

	// Token: 0x06007BAE RID: 31662 RVA: 0x0031FB7C File Offset: 0x0031DD7C
	public static void LoadDone(bool _logUnused)
	{
		if (!_logUnused)
		{
			return;
		}
		foreach (KeyValuePair<string, int> keyValuePair in XUiFromXml.usedControls)
		{
			if (XUiFromXml.debugXuiLoading != XUiFromXml.DebugLevel.Off && (XUiFromXml.debugXuiLoading != XUiFromXml.DebugLevel.Warning || keyValuePair.Value <= 0))
			{
				if (keyValuePair.Value > 0)
				{
					Log.Out(string.Format("[XUi] Control '{0}' used {1} times!", keyValuePair.Key, keyValuePair.Value));
				}
				else
				{
					Log.Warning("[XUi] Control '" + keyValuePair.Key + "' not used!");
				}
			}
		}
		foreach (KeyValuePair<string, int> keyValuePair2 in XUiFromXml.usedWindows)
		{
			if (XUiFromXml.debugXuiLoading != XUiFromXml.DebugLevel.Off && (XUiFromXml.debugXuiLoading != XUiFromXml.DebugLevel.Warning || keyValuePair2.Value <= 0))
			{
				if (keyValuePair2.Value > 0)
				{
					Log.Out(string.Format("[XUi] Window '{0}' used {1} times!", keyValuePair2.Key, keyValuePair2.Value));
				}
				else
				{
					Log.Warning("[XUi] Window '" + keyValuePair2.Key + "' not used!");
				}
			}
		}
	}

	// Token: 0x06007BAF RID: 31663 RVA: 0x0031FCC0 File Offset: 0x0031DEC0
	public static void GetWindowGroupNames(out List<string> windowGroupNames)
	{
		windowGroupNames = new List<string>();
		foreach (XElement xelement in XUiFromXml.mainXuiXml.XmlDoc.Root.Elements("ruleset"))
		{
			foreach (XElement element in xelement.Elements("window_group"))
			{
				if (element.HasAttribute("name"))
				{
					string attribute = element.GetAttribute("name");
					if (!windowGroupNames.Contains(attribute))
					{
						windowGroupNames.Add(attribute);
					}
				}
			}
		}
	}

	// Token: 0x06007BB0 RID: 31664 RVA: 0x0031FD98 File Offset: 0x0031DF98
	public static void LoadXui(XUi _xui, string windowGroupToLoad)
	{
		XElement root = XUiFromXml.mainXuiXml.XmlDoc.Root;
		if (root.HasAttribute("ruleset"))
		{
			_xui.Ruleset = root.GetAttribute("ruleset");
		}
		foreach (XElement xelement in root.Elements("ruleset"))
		{
			if (!xelement.HasAttribute("name") || xelement.GetAttribute("name").EqualsCaseInsensitive(_xui.Ruleset))
			{
				if (xelement.HasAttribute("scale"))
				{
					_xui.SetScale(StringParsers.ParseFloat(xelement.GetAttribute("scale"), 0, -1, NumberStyles.Any));
				}
				if (xelement.HasAttribute("stackpanel_scale"))
				{
					_xui.SetStackPanelScale(StringParsers.ParseFloat(xelement.GetAttribute("stackpanel_scale"), 0, -1, NumberStyles.Any));
				}
				if (xelement.HasAttribute("ignore_missing_class"))
				{
					_xui.IgnoreMissingClass = StringParsers.ParseBool(xelement.GetAttribute("ignore_missing_class"), 0, -1, true);
				}
				foreach (XElement xelement2 in xelement.Elements("window_group"))
				{
					string text = "";
					if (xelement2.HasAttribute("name"))
					{
						text = xelement2.GetAttribute("name");
					}
					if (_xui.FindWindowGroupByName(text) == null && windowGroupToLoad.Equals(text))
					{
						XUiWindowGroup.EHasActionSetFor hasActionSetFor = XUiWindowGroup.EHasActionSetFor.Both;
						if (xelement2.HasAttribute("actionSet"))
						{
							string a = xelement2.GetAttribute("actionSet").ToLower().Trim();
							if (!(a == "true"))
							{
								if (!(a == "false"))
								{
									if (!(a == "controller"))
									{
										if (a == "keyboard")
										{
											hasActionSetFor = XUiWindowGroup.EHasActionSetFor.OnlyKeyboard;
										}
									}
									else
									{
										hasActionSetFor = XUiWindowGroup.EHasActionSetFor.OnlyController;
									}
								}
								else
								{
									hasActionSetFor = XUiWindowGroup.EHasActionSetFor.None;
								}
							}
							else
							{
								hasActionSetFor = XUiWindowGroup.EHasActionSetFor.Both;
							}
						}
						string defaultSelectedName = "";
						if (xelement2.HasAttribute("defaultSelected"))
						{
							defaultSelectedName = xelement2.GetAttribute("defaultSelected");
						}
						XUiWindowGroup xuiWindowGroup = new XUiWindowGroup(text, hasActionSetFor, defaultSelectedName)
						{
							xui = _xui
						};
						int stackPanelYOffset;
						if (xelement2.HasAttribute("stack_panel_y_offset") && int.TryParse(xelement2.GetAttribute("stack_panel_y_offset"), out stackPanelYOffset))
						{
							xuiWindowGroup.StackPanelYOffset = stackPanelYOffset;
						}
						int stackPanelPadding = 16;
						if (xelement2.HasAttribute("stack_panel_padding") && int.TryParse(xelement2.GetAttribute("stack_panel_padding"), out stackPanelYOffset))
						{
							xuiWindowGroup.StackPanelPadding = stackPanelPadding;
						}
						if (xelement2.HasAttribute("open_backpack_on_open"))
						{
							StringParsers.TryParseBool(xelement2.GetAttribute("open_backpack_on_open"), out xuiWindowGroup.openBackpackOnOpen, 0, -1, true);
						}
						if (xelement2.HasAttribute("close_compass_on_open"))
						{
							StringParsers.TryParseBool(xelement2.GetAttribute("close_compass_on_open"), out xuiWindowGroup.closeCompassOnOpen, 0, -1, true);
						}
						if (xelement2.HasAttribute("controller"))
						{
							string attribute = xelement2.GetAttribute("controller");
							Type typeWithPrefix = ReflectionHelpers.GetTypeWithPrefix("XUiC_", attribute);
							if (typeWithPrefix != null)
							{
								xuiWindowGroup.Controller = (XUiController)Activator.CreateInstance(typeWithPrefix);
								xuiWindowGroup.Controller.WindowGroup = xuiWindowGroup;
							}
							else
							{
								XUiFromXml.logForNode(_xui.IgnoreMissingClass ? LogType.Warning : LogType.Error, xelement2, "[XUi] Controller '" + attribute + "' not found, using base XUiController");
								xuiWindowGroup.Controller = new XUiController
								{
									WindowGroup = xuiWindowGroup
								};
							}
						}
						else
						{
							xuiWindowGroup.Controller = new XUiController
							{
								WindowGroup = xuiWindowGroup
							};
						}
						xuiWindowGroup.Controller.xui = _xui;
						XUiC_DragAndDropWindow xuiC_DragAndDropWindow = xuiWindowGroup.Controller as XUiC_DragAndDropWindow;
						if (xuiC_DragAndDropWindow != null)
						{
							_xui.dragAndDrop = xuiC_DragAndDropWindow;
						}
						XUiC_OnScreenIcons xuiC_OnScreenIcons = xuiWindowGroup.Controller as XUiC_OnScreenIcons;
						if (xuiC_OnScreenIcons != null)
						{
							_xui.onScreenIcons = xuiC_OnScreenIcons;
						}
						foreach (XElement xelement3 in xelement2.Elements("window"))
						{
							string text2 = "";
							if (xelement3.HasAttribute("name"))
							{
								text2 = xelement3.GetAttribute("name");
							}
							XUiV_Window xuiV_Window = null;
							XElement windowContentElement;
							if (xelement3.HasElements)
							{
								xuiV_Window = XUiFromXml.parseWindow(text2, xelement3, xelement3, xuiWindowGroup);
							}
							else if (XUiFromXml.windowData.TryGetValue(text2, out windowContentElement))
							{
								IDictionary<string, int> dictionary = XUiFromXml.usedWindows;
								string key = text2;
								int num = dictionary[key];
								dictionary[key] = num + 1;
								xuiV_Window = XUiFromXml.parseWindow(text2, xelement3, windowContentElement, xuiWindowGroup);
							}
							else if (XUiFromXml.debugXuiLoading != XUiFromXml.DebugLevel.Off)
							{
								Log.Warning(string.Concat(new string[]
								{
									"[XUi] window name '",
									text2,
									"' not found for window group '",
									xuiWindowGroup.ID,
									"'!"
								}));
							}
							if (xuiV_Window != null)
							{
								_xui.AddWindow(xuiV_Window);
							}
						}
						_xui.WindowGroups.Add(xuiWindowGroup);
					}
				}
			}
		}
	}

	// Token: 0x06007BB1 RID: 31665 RVA: 0x00320358 File Offset: 0x0031E558
	[PublicizedFrom(EAccessModifier.Private)]
	public static XUiV_Window parseWindow(string _name, XElement _windowCallingElement, XElement _windowContentElement, XUiWindowGroup _windowGroup)
	{
		XUiView xuiView = XUiFromXml.parseViewComponents(_windowContentElement, _windowGroup, _windowGroup.Controller, "", null);
		if (xuiView == null)
		{
			return null;
		}
		XUiV_Window xuiV_Window = xuiView as XUiV_Window;
		if (xuiV_Window == null)
		{
			Log.Error(string.Concat(new string[]
			{
				"[XUi] Failed parsing window name '",
				_name,
				"' in window group '",
				_windowGroup.ID,
				"': Named element is not a 'Window' view but a '",
				xuiView.GetType().Name,
				"'!"
			}));
			return null;
		}
		if (_windowCallingElement.HasAttribute("anchor"))
		{
			xuiV_Window.Anchor = _windowCallingElement.GetAttribute("anchor");
		}
		if (_windowCallingElement.HasAttribute("pos"))
		{
			xuiV_Window.Position = StringParsers.ParseVector2i(_windowCallingElement.GetAttribute("pos"), ',');
		}
		return xuiV_Window;
	}

	// Token: 0x06007BB2 RID: 31666 RVA: 0x00320430 File Offset: 0x0031E630
	[PublicizedFrom(EAccessModifier.Private)]
	public static void loadWindows(XmlFile _xmlFile)
	{
		foreach (XElement xelement in _xmlFile.XmlDoc.Root.Elements())
		{
			if (!xelement.HasAttribute("platform") || XUi.IsMatchingPlatform(xelement.GetAttribute("platform")))
			{
				string attribute = xelement.GetAttribute("name");
				if (string.IsNullOrEmpty(attribute))
				{
					Log.Warning("[XUi] windows.xml top level element without non-empty 'name' attribute");
				}
				else if (XUiFromXml.windowData.TryAdd(attribute, xelement))
				{
					XUiFromXml.usedWindows[attribute] = 0;
				}
				else if (XUiFromXml.debugXuiLoading != XUiFromXml.DebugLevel.Off)
				{
					Log.Warning("[XUi] window data already contains '" + attribute + "'");
				}
			}
		}
	}

	// Token: 0x06007BB3 RID: 31667 RVA: 0x0032050C File Offset: 0x0031E70C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void loadControls(XmlFile _xmlFile)
	{
		foreach (XElement xelement in _xmlFile.XmlDoc.Root.Elements())
		{
			string localName = xelement.Name.LocalName;
			Dictionary<string, object> dictionary = new CaseInsensitiveStringDictionary<object>();
			foreach (XAttribute xattribute in xelement.Attributes())
			{
				dictionary[xattribute.Name.LocalName] = xattribute.Value;
			}
			int num = xelement.Elements().Count<XElement>();
			XElement value = xelement.Elements().First<XElement>();
			if (num > 1)
			{
				if (XUiFromXml.debugXuiLoading != XUiFromXml.DebugLevel.Off)
				{
					Log.Out("[XUi] Control '{0}' cannot have more than a single child node!", new object[]
					{
						localName
					});
				}
			}
			else if (num < 1)
			{
				if (XUiFromXml.debugXuiLoading != XUiFromXml.DebugLevel.Off)
				{
					Log.Out("[XUi] Control '{0}' must have a single child node!", new object[]
					{
						localName
					});
					continue;
				}
				continue;
			}
			if (XUiFromXml.controlData.ContainsKey(localName) && XUiFromXml.debugXuiLoading != XUiFromXml.DebugLevel.Off)
			{
				Log.Warning("[XUi] Control '" + localName + "' already defined, overwriting!");
			}
			XUiFromXml.controlData[localName] = value;
			XUiFromXml.controlDefaults[localName] = dictionary;
			XUiFromXml.usedControls[localName] = 0;
		}
	}

	// Token: 0x06007BB4 RID: 31668 RVA: 0x0032068C File Offset: 0x0031E88C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void loadStyles(XmlFile _xmlFile)
	{
		XElement root = _xmlFile.XmlDoc.Root;
		if (root == null || !root.HasElements)
		{
			throw new Exception("No element <styles> found!");
		}
		foreach (XElement xelement in root.Elements())
		{
			XUiFromXml.StyleData styleData;
			if (xelement.Name == "global")
			{
				if (!XUiFromXml.styles.TryGetValue("global", out styleData))
				{
					styleData = new XUiFromXml.StyleData("global", string.Empty);
					XUiFromXml.styles.Add(styleData.KeyName, styleData);
				}
			}
			else
			{
				string text = null;
				string text2 = null;
				if (xelement.HasAttribute("name"))
				{
					text = xelement.GetAttribute("name");
				}
				if (xelement.HasAttribute("type"))
				{
					text2 = xelement.GetAttribute("type");
				}
				if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(text2))
				{
					Log.Warning("[XUi] Style entry with neither 'Type' or 'Name' attribute");
					continue;
				}
				XUiFromXml.StyleData styleData2 = new XUiFromXml.StyleData(text, text2);
				if (XUiFromXml.styles.TryGetValue(styleData2.KeyName, out styleData))
				{
					if (XUiFromXml.debugXuiLoading != XUiFromXml.DebugLevel.Off)
					{
						Log.Warning("[XUi] Style '" + styleData2.KeyName + "' already defined, merging contents");
					}
				}
				else
				{
					XUiFromXml.styles.Add(styleData2.KeyName, styleData2);
					styleData = styleData2;
				}
			}
			foreach (XElement element in xelement.Elements())
			{
				if (!element.HasAttribute("name"))
				{
					Log.Error("[XUi] Style '" + styleData.KeyName + "' contains a entry that has no 'name' attribute!");
				}
				else if (!element.HasAttribute("value"))
				{
					Log.Error("[XUi] Style '" + styleData.KeyName + "' contains a entry that has no 'value' attribute!");
				}
				else
				{
					string attribute = element.GetAttribute("value");
					string attribute2 = element.GetAttribute("name");
					XUiFromXml.StyleEntryData value = new XUiFromXml.StyleEntryData(attribute2, attribute);
					styleData.StyleEntries[attribute2] = value;
				}
			}
		}
	}

	// Token: 0x06007BB5 RID: 31669 RVA: 0x00320910 File Offset: 0x0031EB10
	[PublicizedFrom(EAccessModifier.Private)]
	public static XUiView parseViewComponents(XElement _node, XUiWindowGroup _windowGroup, XUiController _parent = null, string nodeNameOverride = "", Dictionary<string, object> _controlParams = null)
	{
		if (_node.HasAttribute("platform") && !XUi.IsMatchingPlatform(_node.GetAttribute("platform")))
		{
			return null;
		}
		XUi xui = _windowGroup.xui;
		string localName = _node.Name.LocalName;
		string text = localName;
		if (nodeNameOverride == "" && _node.HasAttribute("name"))
		{
			text = _node.GetAttribute("name");
		}
		else if (nodeNameOverride != "")
		{
			text = nodeNameOverride;
		}
		bool flag = true;
		bool flag2 = true;
		bool flag3 = false;
		if (_controlParams != null)
		{
			XUiFromXml.parseControlParams(_node, _controlParams);
		}
		uint num = <PrivateImplementationDetails>.ComputeStringHash(localName);
		XUiView xuiView;
		if (num <= 2240103498U)
		{
			if (num <= 1179827136U)
			{
				if (num != 1013213428U)
				{
					if (num != 1135768689U)
					{
						if (num == 1179827136U)
						{
							if (localName == "gamepad_icon")
							{
								xuiView = new XUiV_GamepadIcon(text);
								goto IL_31A;
							}
						}
					}
					else if (localName == "button")
					{
						xuiView = new XUiV_Button(text);
						goto IL_31A;
					}
				}
				else if (localName == "texture")
				{
					xuiView = new XUiV_Texture(text);
					goto IL_31A;
				}
			}
			else if (num != 1251777503U)
			{
				if (num != 2179094556U)
				{
					if (num == 2240103498U)
					{
						if (localName == "textlist")
						{
							xuiView = new XUiV_TextList(text);
							goto IL_31A;
						}
					}
				}
				else if (localName == "sprite")
				{
					xuiView = new XUiV_Sprite(text);
					goto IL_31A;
				}
			}
			else if (localName == "table")
			{
				xuiView = new XUiV_Table(text);
				goto IL_31A;
			}
		}
		else if (num <= 2843749381U)
		{
			if (num != 2354395792U)
			{
				if (num != 2708649949U)
				{
					if (num == 2843749381U)
					{
						if (localName == "widget")
						{
							xuiView = new XUiV_Widget(text);
							goto IL_31A;
						}
					}
				}
				else if (localName == "window")
				{
					xuiView = new XUiV_Window(text);
					goto IL_31A;
				}
			}
			else if (localName == "filledsprite")
			{
				xuiView = new XUiV_FilledSprite(text);
				goto IL_31A;
			}
		}
		else if (num <= 3439217733U)
		{
			if (num != 2944866961U)
			{
				if (num == 3439217733U)
				{
					if (localName == "panel")
					{
						xuiView = new XUiV_Panel(text);
						goto IL_31A;
					}
				}
			}
			else if (localName == "grid")
			{
				xuiView = new XUiV_Grid(text);
				goto IL_31A;
			}
		}
		else if (num != 3940830471U)
		{
			if (num == 4137097213U)
			{
				if (localName == "label")
				{
					xuiView = new XUiV_Label(text);
					goto IL_31A;
				}
			}
		}
		else if (localName == "rect")
		{
			xuiView = new XUiV_Rect(text);
			goto IL_31A;
		}
		xuiView = XUiFromXml.createFromTemplate(localName, text, _node, _parent, _windowGroup, _controlParams, ref flag, ref flag2, ref flag3);
		IL_31A:
		XUiView xuiView2 = xuiView;
		if (flag2)
		{
			xuiView2.xui = xui;
			XUiFromXml.setController(_node, xuiView2, _parent);
			xuiView2.SetDefaults(_parent);
			XUiFromXml.parseAttributes(_node, xuiView2, _parent, _controlParams);
			xuiView2.SetPostParsingDefaults(_parent);
		}
		xuiView2.Controller.WindowGroup = _windowGroup;
		if (!flag3 && xuiView2.RepeatContent)
		{
			if (_node.Elements().Count<XElement>() != 1)
			{
				if (XUiFromXml.debugXuiLoading != XUiFromXml.DebugLevel.Off)
				{
					XUiFromXml.logForNode(LogType.Warning, _node, "[XUi] XUiFromXml::parseByElementName: Invalid repeater child count. Must have one child element.");
				}
			}
			else
			{
				int repeatCount = xuiView2.RepeatCount;
				if (_controlParams == null)
				{
					_controlParams = new CaseInsensitiveStringDictionary<object>();
				}
				_controlParams["repeat_count"] = repeatCount;
				XElement other = _node.Elements().First<XElement>();
				for (int i = 0; i < repeatCount; i++)
				{
					_controlParams["repeat_i"] = i;
					xuiView2.setRepeatContentTemplateParams(_controlParams, i);
					XElement xelement = new XElement(other);
					_node.Add(xelement);
					XUiFromXml.parseViewComponents(xelement, _windowGroup, xuiView2.Controller, i.ToString(), _controlParams);
					xelement.Remove();
				}
			}
			flag = false;
		}
		if (flag)
		{
			foreach (XElement node in _node.Elements())
			{
				XUiFromXml.parseViewComponents(node, _windowGroup, xuiView2.Controller, "", _controlParams);
			}
		}
		return xuiView2;
	}

	// Token: 0x06007BB6 RID: 31670 RVA: 0x00320DA4 File Offset: 0x0031EFA4
	[PublicizedFrom(EAccessModifier.Private)]
	public static XUiView createFromTemplate(string _templateName, string _viewName, XElement _node, XUiController _parent, XUiWindowGroup _windowGroup, Dictionary<string, object> _outerControlParams, ref bool _parseChildren, ref bool _parseControllerAndAttributes, ref bool _replacedByTemplate)
	{
		XElement other;
		if (!XUiFromXml.controlData.TryGetValue(_templateName, out other))
		{
			if (XUiFromXml.debugXuiLoading != XUiFromXml.DebugLevel.Off)
			{
				XUiFromXml.logForNode(LogType.Warning, _node, "[XUi] View \"" + _templateName + "\" not found!");
			}
			return XUiFromXml.createEmptyView(_viewName, _parent, _windowGroup, ref _parseControllerAndAttributes);
		}
		if (_node.HasElements)
		{
			if (XUiFromXml.debugXuiLoading != XUiFromXml.DebugLevel.Off)
			{
				XUiFromXml.logForNode(LogType.Warning, _node, "[XUi] Instantiation of templates may not have any child nodes!");
			}
			_parseChildren = false;
			return XUiFromXml.createEmptyView(_viewName, _parent, _windowGroup, ref _parseControllerAndAttributes);
		}
		Dictionary<string, object> dictionary = new CaseInsensitiveStringDictionary<object>();
		if (_outerControlParams != null)
		{
			_outerControlParams.CopyTo(dictionary, false);
		}
		if (((_parent != null) ? _parent.ViewComponent : null) != null)
		{
			Dictionary<string, object> dictionary2 = dictionary;
			string key = "width";
			Vector2i innerSize = _parent.ViewComponent.InnerSize;
			dictionary2[key] = innerSize.x.ToString();
			Dictionary<string, object> dictionary3 = dictionary;
			string key2 = "height";
			innerSize = _parent.ViewComponent.InnerSize;
			dictionary3[key2] = innerSize.y.ToString();
		}
		Dictionary<string, object> src;
		if (XUiFromXml.controlDefaults.TryGetValue(_templateName, out src))
		{
			src.CopyTo(dictionary, true);
		}
		XUiFromXml.parseAttributes(_node, null, null, dictionary);
		XElement xelement = new XElement(other);
		IDictionary<string, int> dictionary4 = XUiFromXml.usedControls;
		int num = dictionary4[_templateName];
		dictionary4[_templateName] = num + 1;
		_node.Add(xelement);
		XUiView xuiView = XUiFromXml.parseViewComponents(xelement, _windowGroup, _parent, _viewName, dictionary);
		if (xuiView == null)
		{
			return null;
		}
		xuiView.xui = _windowGroup.xui;
		xelement.Remove();
		_parseChildren = false;
		_parseControllerAndAttributes = false;
		_replacedByTemplate = true;
		return xuiView;
	}

	// Token: 0x06007BB7 RID: 31671 RVA: 0x00320F00 File Offset: 0x0031F100
	[PublicizedFrom(EAccessModifier.Private)]
	public static XUiView createEmptyView(string _viewName, XUiController _parent, XUiWindowGroup _windowGroup, ref bool _parseControllerAndAttributes)
	{
		XUiView xuiView = new XUiView(_viewName)
		{
			xui = _windowGroup.xui
		};
		xuiView.Controller = new XUiController
		{
			xui = _windowGroup.xui
		};
		if (_parent != null)
		{
			xuiView.Controller.Parent = _parent;
			_parent.AddChild(xuiView.Controller);
		}
		xuiView.SetDefaults(_parent);
		_parseControllerAndAttributes = false;
		return xuiView;
	}

	// Token: 0x06007BB8 RID: 31672 RVA: 0x00320F60 File Offset: 0x0031F160
	[PublicizedFrom(EAccessModifier.Private)]
	public static void parseControlParams(XElement _node, Dictionary<string, object> _controlParams)
	{
		foreach (XAttribute xattribute in _node.Attributes())
		{
			string text = xattribute.Value;
			bool flag = false;
			int num;
			while ((num = text.IndexOf("${", StringComparison.Ordinal)) >= 0)
			{
				int num2 = text.IndexOf('}', num);
				int count = num2 - num + 1;
				if (num2 < 0)
				{
					LogType level = LogType.Error;
					string str = "[XUi] Expression has unclosed parameter references: ";
					XName name = xattribute.Name;
					XUiFromXml.logForNode(level, _node, str + ((name != null) ? name.ToString() : null) + "=" + text);
					break;
				}
				string text2 = text.Substring(num + 2, num2 - (num + 2));
				Expression expression;
				if (!XUiFromXml.expressionCache.TryGetValue(text2, out expression))
				{
					expression = new Expression(text2, EvaluateOptions.IgnoreCase | EvaluateOptions.UseDoubleForAbsFunction);
					expression.EvaluateFunction += XUiFromXml.NCalcIdentifierDefinedFunction;
					XUiFromXml.expressionCache.Add(text2, expression);
				}
				expression.Parameters = _controlParams;
				string value2;
				try
				{
					object obj = expression.Evaluate();
					if (obj is decimal)
					{
						decimal value = (decimal)obj;
						value2 = value.ToCultureInvariantString("0.########");
					}
					else if (obj is float)
					{
						float value3 = (float)obj;
						value2 = value3.ToCultureInvariantString();
					}
					else if (obj is double)
					{
						double value4 = (double)obj;
						value2 = value4.ToCultureInvariantString();
					}
					else
					{
						value2 = obj.ToString();
					}
				}
				catch (ArgumentException ex)
				{
					LogType level2 = LogType.Error;
					string[] array = new string[7];
					array[0] = "[XUi] Control parameter '";
					array[1] = ex.ParamName;
					array[2] = "' undefined (in: ";
					int num3 = 3;
					XName name2 = xattribute.Name;
					array[num3] = ((name2 != null) ? name2.ToString() : null);
					array[4] = "=\"";
					array[5] = text;
					array[6] = "\")";
					XUiFromXml.logForNode(level2, _node, string.Concat(array));
					value2 = "";
				}
				catch (Exception e)
				{
					XUiFromXml.logForNode(LogType.Exception, _node, "[XUi] Control expression can not be evaluated: " + text2);
					Log.Exception(e);
					value2 = "";
				}
				text = text.Remove(num, count).Insert(num, value2);
				flag = true;
			}
			if (flag)
			{
				xattribute.Value = text;
			}
		}
	}

	// Token: 0x06007BB9 RID: 31673 RVA: 0x003211C4 File Offset: 0x0031F3C4
	[PublicizedFrom(EAccessModifier.Private)]
	public static void NCalcIdentifierDefinedFunction(string _name, FunctionArgs _args, bool _ignoreCase)
	{
		Expression[] parameters = _args.Parameters;
		if (_name.EqualsCaseInsensitive("defined") && parameters.Length == 1)
		{
			Identifier identifier = parameters[0].ParsedExpression as Identifier;
			if (identifier != null)
			{
				string name = identifier.Name;
				_args.Result = parameters[0].Parameters.ContainsKey(name);
				_args.HasResult = true;
			}
			return;
		}
	}

	// Token: 0x06007BBA RID: 31674 RVA: 0x00321224 File Offset: 0x0031F424
	[PublicizedFrom(EAccessModifier.Private)]
	public static void parseAttributes(XElement _node, XUiView _viewComponent, XUiController _parent, Dictionary<string, object> _controlParams = null)
	{
		string localName = _node.Name.LocalName;
		XUiFromXml.StyleData styleData;
		if (XUiFromXml.styles.TryGetValue(localName, out styleData))
		{
			foreach (KeyValuePair<string, XUiFromXml.StyleEntryData> keyValuePair in styleData.StyleEntries)
			{
				XUiFromXml.StyleEntryData value = keyValuePair.Value;
				XUiFromXml.parseAttribute(_viewComponent, value.Name, value.Value, _parent, _controlParams);
			}
		}
		if (_node.HasAttribute("style"))
		{
			string[] array = _node.GetAttribute("style").Replace(" ", "").Split(',', StringSplitOptions.None);
			int i = 0;
			while (i < array.Length)
			{
				string text = array[i];
				string text2 = localName + "." + text;
				if (XUiFromXml.styles.TryGetValue(text2, out styleData))
				{
					using (Dictionary<string, XUiFromXml.StyleEntryData>.Enumerator enumerator = styleData.StyleEntries.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<string, XUiFromXml.StyleEntryData> keyValuePair2 = enumerator.Current;
							XUiFromXml.StyleEntryData value2 = keyValuePair2.Value;
							XUiFromXml.parseAttribute(_viewComponent, value2.Name, value2.Value, _parent, _controlParams);
						}
						goto IL_19E;
					}
					goto IL_127;
				}
				goto IL_127;
				IL_19E:
				i++;
				continue;
				IL_127:
				if (XUiFromXml.styles.TryGetValue(text, out styleData))
				{
					using (Dictionary<string, XUiFromXml.StyleEntryData>.Enumerator enumerator = styleData.StyleEntries.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<string, XUiFromXml.StyleEntryData> keyValuePair3 = enumerator.Current;
							XUiFromXml.StyleEntryData value3 = keyValuePair3.Value;
							XUiFromXml.parseAttribute(_viewComponent, value3.Name, value3.Value, _parent, _controlParams);
						}
						goto IL_19E;
					}
				}
				XUiFromXml.logForNode(LogType.Error, _node, "[XUi] Style key '" + text2 + "' not found!");
				goto IL_19E;
			}
		}
		foreach (XAttribute xattribute in _node.Attributes())
		{
			string localName2 = xattribute.Name.LocalName;
			if (!(localName2 == "style"))
			{
				string text3 = xattribute.Value;
				if (XUiFromXml.IsStyleRef(text3))
				{
					string text4 = text3.Substring(1, text3.Length - 2);
					XUiFromXml.StyleEntryData styleEntryData;
					if (!XUiFromXml.styles["global"].StyleEntries.TryGetValue(text4, out styleEntryData))
					{
						XUiFromXml.logForNode(LogType.Error, _node, "[XUi] Global style key '" + text4 + "' not found!");
						continue;
					}
					text3 = styleEntryData.Value;
				}
				string attributeNameLower = localName2.ToLower();
				if (text3.IndexOf("\\n", StringComparison.Ordinal) >= 0)
				{
					text3 = text3.Replace("\\n", "\n", StringComparison.Ordinal);
				}
				XUiFromXml.parseAttribute(_viewComponent, attributeNameLower, text3, _parent, _controlParams);
			}
		}
	}

	// Token: 0x06007BBB RID: 31675 RVA: 0x0032150C File Offset: 0x0031F70C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void parseAttribute(XUiView _viewComponent, string _attributeNameLower, string _value, XUiController _parent, Dictionary<string, object> _controlParams = null)
	{
		if (_viewComponent == null)
		{
			_controlParams[_attributeNameLower] = _value;
			return;
		}
		_viewComponent.ParseAttributeViewAndController(_attributeNameLower, _value, _parent, true);
	}

	// Token: 0x06007BBC RID: 31676 RVA: 0x00321528 File Offset: 0x0031F728
	[PublicizedFrom(EAccessModifier.Private)]
	public static void setController(XElement _node, XUiView _viewComponent, XUiController _parent)
	{
		XUi xui = _viewComponent.xui;
		XUiController xuiController;
		if (_node.HasAttribute("controller"))
		{
			string attribute = _node.GetAttribute("controller");
			Type typeWithPrefix = ReflectionHelpers.GetTypeWithPrefix("XUiC_", attribute);
			if (typeWithPrefix == null)
			{
				XUiFromXml.logForNode(xui.IgnoreMissingClass ? LogType.Warning : LogType.Error, _node, "[XUi] Controller '" + attribute + "' not found, using base XUiController");
				xuiController = (xui.IgnoreMissingClass ? new XUiControllerMissing() : new XUiController());
			}
			else if (typeWithPrefix.IsAbstract)
			{
				XUiFromXml.logForNode(LogType.Error, _node, "[XUi] Controller '" + attribute + "' not instantiable, class is abstract");
				xuiController = new XUiController();
			}
			else
			{
				xuiController = (XUiController)Activator.CreateInstance(typeWithPrefix);
			}
		}
		else
		{
			xuiController = new XUiController();
		}
		_viewComponent.Controller = xuiController;
		xuiController.xui = xui;
		XUiC_DragAndDropWindow xuiC_DragAndDropWindow = _viewComponent.Controller as XUiC_DragAndDropWindow;
		if (xuiC_DragAndDropWindow != null)
		{
			xui.dragAndDrop = xuiC_DragAndDropWindow;
		}
		XUiC_OnScreenIcons xuiC_OnScreenIcons = _viewComponent.Controller as XUiC_OnScreenIcons;
		if (xuiC_OnScreenIcons != null)
		{
			xui.onScreenIcons = xuiC_OnScreenIcons;
		}
		if (_parent != null)
		{
			xuiController.Parent = _parent;
			_parent.AddChild(_viewComponent.Controller);
		}
	}

	// Token: 0x06007BBD RID: 31677 RVA: 0x0032164C File Offset: 0x0031F84C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void logForNode(LogType _level, XElement _node, string _message)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(_message);
		stringBuilder.Append(" --- hierarchy: ");
		XUiFromXml.logTree(stringBuilder, _node);
		string txt = stringBuilder.ToString();
		switch (_level)
		{
		case LogType.Error:
		case LogType.Exception:
			Log.Error(txt);
			return;
		case LogType.Warning:
			Log.Warning(txt);
			return;
		case LogType.Log:
			Log.Out(txt);
			return;
		}
		throw new ArgumentOutOfRangeException();
	}

	// Token: 0x06007BBE RID: 31678 RVA: 0x003216B8 File Offset: 0x0031F8B8
	[PublicizedFrom(EAccessModifier.Private)]
	public static void logTree(StringBuilder _sb, XElement _node)
	{
		if (_node.Parent != null)
		{
			XUiFromXml.logTree(_sb, _node.Parent);
			_sb.Append(" -> ");
		}
		if (_node != null)
		{
			if (_node.HasAttribute("name"))
			{
				_sb.Append(_node.Name);
				_sb.Append(" (");
				_sb.Append(_node.GetAttribute("name"));
				_sb.Append(")");
				return;
			}
			_sb.Append(_node.Name);
		}
	}

	// Token: 0x06007BBF RID: 31679 RVA: 0x00321746 File Offset: 0x0031F946
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool IsStyleRef(string _value)
	{
		return _value.StartsWith("[", StringComparison.Ordinal) && _value.IndexOf("]", StringComparison.Ordinal) == _value.Length - 1 && _value.IndexOf("[", 1, StringComparison.Ordinal) < 0;
	}

	// Token: 0x04005D8F RID: 23951
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<string, XElement> windowData;

	// Token: 0x04005D90 RID: 23952
	[PublicizedFrom(EAccessModifier.Private)]
	public static IDictionary<string, int> usedWindows;

	// Token: 0x04005D91 RID: 23953
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<string, XElement> controlData;

	// Token: 0x04005D92 RID: 23954
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<string, Dictionary<string, object>> controlDefaults;

	// Token: 0x04005D93 RID: 23955
	[PublicizedFrom(EAccessModifier.Private)]
	public static IDictionary<string, int> usedControls;

	// Token: 0x04005D94 RID: 23956
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<string, XUiFromXml.StyleData> styles;

	// Token: 0x04005D95 RID: 23957
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<string, Expression> expressionCache;

	// Token: 0x04005D96 RID: 23958
	[PublicizedFrom(EAccessModifier.Private)]
	public static XmlFile mainXuiXml;

	// Token: 0x04005D97 RID: 23959
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly XUiFromXml.DebugLevel debugXuiLoading;

	// Token: 0x02000F20 RID: 3872
	public enum DebugLevel
	{
		// Token: 0x04005D99 RID: 23961
		Off,
		// Token: 0x04005D9A RID: 23962
		Warning,
		// Token: 0x04005D9B RID: 23963
		Verbose
	}

	// Token: 0x02000F21 RID: 3873
	public class StyleData
	{
		// Token: 0x06007BC0 RID: 31680 RVA: 0x00321780 File Offset: 0x0031F980
		public StyleData(string _name, string _type)
		{
			this.Type = _type;
			this.Name = _name;
			if (!string.IsNullOrEmpty(this.Type) && !string.IsNullOrEmpty(this.Name))
			{
				this.KeyName = this.Type + "." + this.Name;
				return;
			}
			if (!string.IsNullOrEmpty(this.Type))
			{
				this.KeyName = this.Type;
				return;
			}
			if (!string.IsNullOrEmpty(this.Name))
			{
				this.KeyName = this.Name;
				return;
			}
			Log.Error("[XUi] Style entry with neither 'Type' or 'Name' attribute");
		}

		// Token: 0x04005D9C RID: 23964
		public readonly string Name;

		// Token: 0x04005D9D RID: 23965
		public readonly string Type;

		// Token: 0x04005D9E RID: 23966
		public readonly string KeyName;

		// Token: 0x04005D9F RID: 23967
		public readonly Dictionary<string, XUiFromXml.StyleEntryData> StyleEntries = new Dictionary<string, XUiFromXml.StyleEntryData>();
	}

	// Token: 0x02000F22 RID: 3874
	public class StyleEntryData
	{
		// Token: 0x17000CFC RID: 3324
		// (get) Token: 0x06007BC1 RID: 31681 RVA: 0x00321824 File Offset: 0x0031FA24
		public string Value
		{
			get
			{
				string text = this.value;
				if (!XUiFromXml.IsStyleRef(text))
				{
					return text;
				}
				string key = text.Substring(1, text.Length - 2);
				XUiFromXml.StyleEntryData styleEntryData;
				if (!XUiFromXml.styles["global"].StyleEntries.TryGetValue(key, out styleEntryData))
				{
					return text;
				}
				text = styleEntryData.Value;
				this.value = text;
				return text;
			}
		}

		// Token: 0x06007BC2 RID: 31682 RVA: 0x00321881 File Offset: 0x0031FA81
		public StyleEntryData(string _name, string _value)
		{
			this.Name = _name;
			this.value = _value;
		}

		// Token: 0x04005DA0 RID: 23968
		public readonly string Name;

		// Token: 0x04005DA1 RID: 23969
		[PublicizedFrom(EAccessModifier.Private)]
		public string value;
	}
}
