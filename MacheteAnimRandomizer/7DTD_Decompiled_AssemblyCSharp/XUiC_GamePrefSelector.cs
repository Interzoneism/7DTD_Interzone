using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000CBD RID: 3261
[Preserve]
public class XUiC_GamePrefSelector : XUiController
{
	// Token: 0x17000A48 RID: 2632
	// (get) Token: 0x060064D5 RID: 25813 RVA: 0x0028D194 File Offset: 0x0028B394
	public EnumGamePrefs GamePref
	{
		get
		{
			return this.gamePref;
		}
	}

	// Token: 0x17000A49 RID: 2633
	// (get) Token: 0x060064D6 RID: 25814 RVA: 0x0028D19C File Offset: 0x0028B39C
	public XUiC_TextInput ControlText
	{
		get
		{
			return this.controlText;
		}
	}

	// Token: 0x17000A4A RID: 2634
	// (get) Token: 0x060064D7 RID: 25815 RVA: 0x0028D1A4 File Offset: 0x0028B3A4
	// (set) Token: 0x060064D8 RID: 25816 RVA: 0x0028D1AC File Offset: 0x0028B3AC
	public Func<int, int> ValuePreDisplayModifierFunc
	{
		get
		{
			return this.valuePreDisplayModifierFunc;
		}
		set
		{
			this.valuePreDisplayModifierFunc = value;
			if (this.valuePreDisplayModifierFunc != null)
			{
				this.SetupOptions();
			}
		}
	}

	// Token: 0x17000A4B RID: 2635
	// (get) Token: 0x060064D9 RID: 25817 RVA: 0x0028D1C3 File Offset: 0x0028B3C3
	// (set) Token: 0x060064DA RID: 25818 RVA: 0x0028D1CC File Offset: 0x0028B3CC
	public bool Enabled
	{
		get
		{
			return this.enabled;
		}
		set
		{
			if (this.enabled != value)
			{
				this.enabled = value;
				this.controlCombo.Enabled = value;
				this.controlText.Enabled = value;
				this.controlText.ActiveTextColor = (value ? this.enabledColor : this.disabledColor);
				this.controlLabel.Color = (value ? this.enabledColor : this.disabledColor);
			}
		}
	}

	// Token: 0x060064DB RID: 25819 RVA: 0x0028D23C File Offset: 0x0028B43C
	public override void Init()
	{
		base.Init();
		this.gamePref = EnumUtils.Parse<EnumGamePrefs>(this.viewComponent.ID, false);
		this.controlLabel = (XUiV_Label)base.GetChildById("ControlLabel").ViewComponent;
		this.enabledColor = this.controlLabel.Color;
		this.controlCombo = base.GetChildById("ControlCombo").GetChildByType<XUiC_ComboBoxList<XUiC_GamePrefSelector.GameOptionValue>>();
		this.controlCombo.OnValueChanged += this.ControlCombo_OnValueChanged;
		this.controlText = base.GetChildById("ControlText").GetChildByType<XUiC_TextInput>();
		this.controlText.OnChangeHandler += this.ControlText_OnChangeHandler;
		if (!this.isTextInput)
		{
			this.SetupOptions();
		}
	}

	// Token: 0x060064DC RID: 25820 RVA: 0x0028D2FA File Offset: 0x0028B4FA
	public override void OnOpen()
	{
		base.OnOpen();
		this.controlCombo.ViewComponent.IsVisible = !this.isTextInput;
		this.controlText.ViewComponent.IsVisible = this.isTextInput;
	}

	// Token: 0x060064DD RID: 25821 RVA: 0x00272183 File Offset: 0x00270383
	public override void Update(float _dt)
	{
		base.Update(_dt);
	}

	// Token: 0x060064DE RID: 25822 RVA: 0x0028D334 File Offset: 0x0028B534
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(_name);
		if (num <= 1102564575U)
		{
			if (num <= 877087803U)
			{
				if (num != 811104650U)
				{
					if (num == 877087803U)
					{
						if (_name == "values")
						{
							if (_value.Length > 0)
							{
								this.valuesFromXml = _value.Split(',', StringSplitOptions.None);
								for (int i = 0; i < this.valuesFromXml.Length; i++)
								{
									this.valuesFromXml[i] = this.valuesFromXml[i].Trim();
								}
							}
							return true;
						}
					}
				}
				else if (_name == "values_enforced")
				{
					this.valuesEnforced = StringParsers.ParseBool(_value, 0, -1, true);
					return true;
				}
			}
			else if (num != 996973687U)
			{
				if (num == 1102564575U)
				{
					if (_name == "value_localization_prefix")
					{
						if (_value.Length > 0)
						{
							this.valueLocalizationPrefixFromXml = _value.Trim();
						}
						return true;
					}
				}
			}
			else if (_name == "value_type")
			{
				this.valueType = EnumUtils.Parse<GamePrefs.EnumType>(_value, true);
				return true;
			}
		}
		else if (num <= 2533046024U)
		{
			if (num != 1222086634U)
			{
				if (num == 2533046024U)
				{
					if (_name == "always_show")
					{
						this.alwaysShow = StringParsers.ParseBool(_value, 0, -1, true);
						return true;
					}
				}
			}
			else if (_name == "display_names")
			{
				if (_value.Length > 0)
				{
					this.namesFromXml = _value.Split(',', StringSplitOptions.None);
					for (int j = 0; j < this.namesFromXml.Length; j++)
					{
						this.namesFromXml[j] = this.namesFromXml[j].Trim();
					}
				}
				return true;
			}
		}
		else if (num != 3319127657U)
		{
			if (num != 3589293078U)
			{
				if (num == 3931288483U)
				{
					if (_name == "is_textinput")
					{
						this.isTextInput = StringParsers.ParseBool(_value, 0, -1, true);
						return true;
					}
				}
			}
			else if (_name == "values_from_gameserverconfig")
			{
				this.valuesFromGameServerConfig = StringParsers.ParseBool(_value, 0, -1, true);
				return true;
			}
		}
		else if (_name == "has_default")
		{
			this.hasDefault = StringParsers.ParseBool(_value, 0, -1, true);
			return true;
		}
		return base.ParseAttribute(_name, _value, _parent);
	}

	// Token: 0x060064DF RID: 25823 RVA: 0x0028D594 File Offset: 0x0028B794
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupOptions()
	{
		int[] array = null;
		string[] array2 = null;
		string[] array3;
		switch (this.valueType)
		{
		case GamePrefs.EnumType.Int:
			if (this.valuesFromXml != null)
			{
				array = new int[this.valuesFromXml.Length];
				for (int i = 0; i < this.valuesFromXml.Length; i++)
				{
					array[i] = StringParsers.ParseSInt32(this.valuesFromXml[i], 0, -1, NumberStyles.Integer);
				}
			}
			if (array == null)
			{
				if (this.namesFromXml == null)
				{
					throw new Exception("Illegal option setup for " + this.gamePref.ToStringCached<EnumGamePrefs>() + " (no values and no names specified)");
				}
				array = new int[this.namesFromXml.Length];
				for (int j = 0; j < array.Length; j++)
				{
					array[j] = j;
				}
			}
			array3 = new string[array.Length];
			if (this.namesFromXml == null || this.namesFromXml.Length != array.Length)
			{
				for (int k = 0; k < array.Length; k++)
				{
					if (this.namesFromXml != null && k < this.namesFromXml.Length)
					{
						array3[k] = Localization.Get(this.namesFromXml[k], false);
					}
					else
					{
						int num = array[k];
						if (this.valuePreDisplayModifierFunc != null)
						{
							num = this.valuePreDisplayModifierFunc(num);
						}
						if (string.IsNullOrEmpty(this.valueLocalizationPrefixFromXml))
						{
							array3[k] = num.ToString();
						}
						else
						{
							array3[k] = string.Format(Localization.Get(this.valueLocalizationPrefixFromXml + ((num == 1) ? "" : "s"), false), num);
						}
					}
				}
				goto IL_302;
			}
			for (int l = 0; l < this.namesFromXml.Length; l++)
			{
				array3[l] = Localization.Get(this.namesFromXml[l], false);
			}
			goto IL_302;
		case GamePrefs.EnumType.String:
			array2 = this.valuesFromXml;
			array3 = new string[array2.Length];
			if (this.namesFromXml == null || this.namesFromXml.Length != array2.Length)
			{
				for (int m = 0; m < array2.Length; m++)
				{
					if (this.namesFromXml != null && m < this.namesFromXml.Length)
					{
						array3[m] = Localization.Get(this.namesFromXml[m], false);
					}
					else
					{
						string text = array2[m];
						if (string.IsNullOrEmpty(this.valueLocalizationPrefixFromXml))
						{
							array3[m] = text;
						}
						else
						{
							array3[m] = Localization.Get(this.valueLocalizationPrefixFromXml + text, false);
						}
					}
				}
				goto IL_302;
			}
			for (int n = 0; n < this.namesFromXml.Length; n++)
			{
				array3[n] = Localization.Get(this.namesFromXml[n], false);
			}
			goto IL_302;
		case GamePrefs.EnumType.Bool:
			array = new int[]
			{
				0,
				1
			};
			array3 = new string[2];
			if (this.namesFromXml != null && this.namesFromXml.Length == 2)
			{
				array3[0] = Localization.Get(this.namesFromXml[0], false);
				array3[1] = Localization.Get(this.namesFromXml[1], false);
				goto IL_302;
			}
			array3[0] = Localization.Get("goOff", false);
			array3[1] = Localization.Get("goOn", false);
			goto IL_302;
		}
		throw new NotSupportedException("Not a valid GamePref: " + this.viewComponent.ID);
		IL_302:
		this.controlCombo.Elements.Clear();
		if (array3 == null)
		{
			throw new Exception("Illegal option setup for " + this.gamePref.ToStringCached<EnumGamePrefs>() + " (names null)");
		}
		if (this.valueType != GamePrefs.EnumType.String)
		{
			if (array == null)
			{
				throw new Exception("Illegal option setup for " + this.gamePref.ToStringCached<EnumGamePrefs>() + " (values still null)");
			}
			for (int num2 = 0; num2 < array.Length; num2++)
			{
				this.controlCombo.Elements.Add(new XUiC_GamePrefSelector.GameOptionValue(array[num2], array3[num2]));
			}
		}
		if (this.valueType == GamePrefs.EnumType.String)
		{
			if (array2 == null)
			{
				throw new Exception("Illegal option setup for " + this.gamePref.ToStringCached<EnumGamePrefs>() + " (values still null)");
			}
			for (int num3 = 0; num3 < array2.Length; num3++)
			{
				this.controlCombo.Elements.Add(new XUiC_GamePrefSelector.GameOptionValue(array2[num3], array3[num3]));
			}
		}
	}

	// Token: 0x060064E0 RID: 25824 RVA: 0x0028D98C File Offset: 0x0028BB8C
	[PublicizedFrom(EAccessModifier.Private)]
	public void ControlText_OnChangeHandler(XUiController _sender, string _text, bool _changeFromCode)
	{
		GamePrefs.EnumType enumType = this.valueType;
		int value;
		if (enumType != GamePrefs.EnumType.Int)
		{
			if (enumType != GamePrefs.EnumType.String)
			{
				throw new Exception("Illegal option setup for " + this.gamePref.ToStringCached<EnumGamePrefs>());
			}
			GamePrefs.Set(this.gamePref, _text);
		}
		else if (int.TryParse(_text, out value))
		{
			GamePrefs.Set(this.gamePref, value);
		}
		Action<XUiC_GamePrefSelector, EnumGamePrefs> onValueChanged = this.OnValueChanged;
		if (onValueChanged != null)
		{
			onValueChanged(this, this.gamePref);
		}
		this.CheckDefaultValue();
	}

	// Token: 0x060064E1 RID: 25825 RVA: 0x0028DA08 File Offset: 0x0028BC08
	[PublicizedFrom(EAccessModifier.Private)]
	public void ControlCombo_OnValueChanged(XUiController _sender, XUiC_GamePrefSelector.GameOptionValue _oldValue, XUiC_GamePrefSelector.GameOptionValue _newValue)
	{
		switch (this.valueType)
		{
		case GamePrefs.EnumType.Int:
			GamePrefs.Set(this.gamePref, _newValue.IntValue);
			goto IL_76;
		case GamePrefs.EnumType.String:
			GamePrefs.Set(this.gamePref, _newValue.StringValue);
			goto IL_76;
		case GamePrefs.EnumType.Bool:
			GamePrefs.Set(this.gamePref, _newValue.IntValue == 1);
			goto IL_76;
		}
		throw new Exception("Illegal option setup for " + this.gamePref.ToStringCached<EnumGamePrefs>());
		IL_76:
		Action<XUiC_GamePrefSelector, EnumGamePrefs> onValueChanged = this.OnValueChanged;
		if (onValueChanged != null)
		{
			onValueChanged(this, this.gamePref);
		}
		this.CheckDefaultValue();
	}

	// Token: 0x060064E2 RID: 25826 RVA: 0x0028DAAC File Offset: 0x0028BCAC
	public void SetCurrentValue()
	{
		try
		{
			switch (this.valueType)
			{
			case GamePrefs.EnumType.Int:
			{
				int @int = GamePrefs.GetInt(this.gamePref);
				if (this.isTextInput)
				{
					this.controlText.Text = @int.ToString();
					goto IL_3A2;
				}
				bool flag = false;
				for (int i = 1; i < this.controlCombo.Elements.Count; i++)
				{
					if (this.controlCombo.Elements[i].IntValue == @int)
					{
						this.controlCombo.SelectedIndex = i;
						flag = true;
						break;
					}
				}
				if (this.valuesEnforced && !flag)
				{
					int num = -1;
					int num2 = int.MaxValue;
					for (int j = 0; j < this.controlCombo.Elements.Count; j++)
					{
						int num3 = Math.Abs(this.controlCombo.Elements[j].IntValue - @int);
						if (num < 0 || num3 < num2)
						{
							num = j;
							num2 = num3;
							if (num2 <= 0)
							{
								break;
							}
						}
					}
					if (num >= 0)
					{
						this.controlCombo.SelectedIndex = num;
						GamePrefs.Set(this.gamePref, this.controlCombo.Value.IntValue);
						flag = true;
					}
				}
				if (flag)
				{
					goto IL_3A2;
				}
				if (string.IsNullOrEmpty(this.valueLocalizationPrefixFromXml))
				{
					XUiC_GamePrefSelector.GameOptionValue value = new XUiC_GamePrefSelector.GameOptionValue(@int, string.Format("{0} {1}", @int.ToString(), Localization.Get("goCustomValueSuffix", false)));
					this.controlCombo.Value = value;
					goto IL_3A2;
				}
				XUiC_GamePrefSelector.GameOptionValue value2 = new XUiC_GamePrefSelector.GameOptionValue(@int, string.Format(Localization.Get(this.valueLocalizationPrefixFromXml + ((@int == 1) ? "" : "s"), false), @int.ToString()) + " " + Localization.Get("goCustomValueSuffix", false));
				this.controlCombo.Value = value2;
				goto IL_3A2;
			}
			case GamePrefs.EnumType.String:
			{
				string @string = GamePrefs.GetString(this.gamePref);
				if (this.isTextInput)
				{
					this.controlText.Text = GamePrefs.GetString(this.gamePref);
					goto IL_3A2;
				}
				bool flag2 = false;
				for (int k = 1; k < this.controlCombo.Elements.Count; k++)
				{
					if (this.controlCombo.Elements[k].StringValue == @string)
					{
						this.controlCombo.SelectedIndex = k;
						flag2 = true;
						break;
					}
				}
				if (this.valuesEnforced && !flag2 && this.controlCombo.Elements.Count > 0)
				{
					this.controlCombo.SelectedIndex = 0;
					GamePrefs.Set(this.gamePref, this.controlCombo.Value.StringValue);
					flag2 = true;
				}
				if (flag2)
				{
					goto IL_3A2;
				}
				if (string.IsNullOrEmpty(@string))
				{
					this.controlCombo.SelectedIndex = 0;
					GamePrefs.Set(this.gamePref, this.controlCombo.Value.StringValue);
					goto IL_3A2;
				}
				if (string.IsNullOrEmpty(this.valueLocalizationPrefixFromXml))
				{
					XUiC_GamePrefSelector.GameOptionValue value3 = new XUiC_GamePrefSelector.GameOptionValue(@string, string.Format("{0} {1}", @string, Localization.Get("goCustomValueSuffix", false)));
					this.controlCombo.Value = value3;
					goto IL_3A2;
				}
				XUiC_GamePrefSelector.GameOptionValue value4 = new XUiC_GamePrefSelector.GameOptionValue(@string, string.Format(Localization.Get(this.valueLocalizationPrefixFromXml ?? "", false), @string) + " " + Localization.Get("goCustomValueSuffix", false));
				this.controlCombo.Value = value4;
				goto IL_3A2;
			}
			case GamePrefs.EnumType.Bool:
				this.controlCombo.SelectedIndex = (GamePrefs.GetBool(this.gamePref) ? 1 : 0);
				goto IL_3A2;
			}
			throw new Exception("Illegal option setup for " + this.gamePref.ToStringCached<EnumGamePrefs>());
			IL_3A2:;
		}
		catch (Exception e)
		{
			Log.Exception(e);
		}
		Action<XUiC_GamePrefSelector, EnumGamePrefs> onValueChanged = this.OnValueChanged;
		if (onValueChanged != null)
		{
			onValueChanged(this, this.gamePref);
		}
		this.CheckDefaultValue();
	}

	// Token: 0x060064E3 RID: 25827 RVA: 0x0028DEA0 File Offset: 0x0028C0A0
	public void CheckDefaultValue()
	{
		Color color;
		if (this.enabled)
		{
			color = (this.IsDefaultValueForGameMode() ? Color.white : Color.yellow);
		}
		else
		{
			color = this.disabledColor;
		}
		this.controlText.ActiveTextColor = color;
		this.controlCombo.TextColor = color;
	}

	// Token: 0x060064E4 RID: 25828 RVA: 0x0028DEEC File Offset: 0x0028C0EC
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsDefaultValueForGameMode()
	{
		if (!this.hasDefault)
		{
			return true;
		}
		if (this.currentGameMode == null)
		{
			return true;
		}
		Dictionary<EnumGamePrefs, GameMode.ModeGamePref> gamePrefs = this.currentGameMode.GetGamePrefs();
		if (!gamePrefs.ContainsKey(this.gamePref))
		{
			return false;
		}
		switch (this.valueType)
		{
		case GamePrefs.EnumType.Int:
		{
			int intValue;
			if (this.isTextInput)
			{
				StringParsers.TryParseSInt32(this.controlText.Text, out intValue, 0, -1, NumberStyles.Integer);
			}
			else
			{
				intValue = this.controlCombo.Value.IntValue;
			}
			return intValue == (int)gamePrefs[this.gamePref].DefaultValue;
		}
		case GamePrefs.EnumType.String:
			if (this.isTextInput)
			{
				return this.controlText.Text == (string)gamePrefs[this.gamePref].DefaultValue;
			}
			return this.controlCombo.Value.StringValue == (string)gamePrefs[this.gamePref].DefaultValue;
		case GamePrefs.EnumType.Bool:
			return this.controlCombo.Value.IntValue == 1 == (bool)gamePrefs[this.gamePref].DefaultValue;
		}
		return false;
	}

	// Token: 0x060064E5 RID: 25829 RVA: 0x0028E01E File Offset: 0x0028C21E
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetVisible(bool _visible)
	{
		this.viewComponent.IsVisible = _visible;
	}

	// Token: 0x060064E6 RID: 25830 RVA: 0x0028E02C File Offset: 0x0028C22C
	public void SetCurrentGameMode(GameMode _gameMode)
	{
		this.currentGameMode = _gameMode;
		this.SetVisible(this.alwaysShow || (_gameMode != null && _gameMode.GetGamePrefs().ContainsKey(this.gamePref)));
		this.CheckDefaultValue();
	}

	// Token: 0x060064E7 RID: 25831 RVA: 0x0028E063 File Offset: 0x0028C263
	[PublicizedFrom(EAccessModifier.Internal)]
	public void OverrideValues(List<string> overrideValues)
	{
		this.valuesFromXml = overrideValues.ToArray();
		this.SetupOptions();
	}

	// Token: 0x04004C11 RID: 19473
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label controlLabel;

	// Token: 0x04004C12 RID: 19474
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<XUiC_GamePrefSelector.GameOptionValue> controlCombo;

	// Token: 0x04004C13 RID: 19475
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput controlText;

	// Token: 0x04004C14 RID: 19476
	[PublicizedFrom(EAccessModifier.Private)]
	public Color enabledColor;

	// Token: 0x04004C15 RID: 19477
	[PublicizedFrom(EAccessModifier.Private)]
	public Color disabledColor = new Color(0.625f, 0.625f, 0.625f);

	// Token: 0x04004C16 RID: 19478
	[PublicizedFrom(EAccessModifier.Private)]
	public bool valuesFromGameServerConfig;

	// Token: 0x04004C17 RID: 19479
	[PublicizedFrom(EAccessModifier.Private)]
	public string[] valuesFromXml;

	// Token: 0x04004C18 RID: 19480
	[PublicizedFrom(EAccessModifier.Private)]
	public string[] namesFromXml;

	// Token: 0x04004C19 RID: 19481
	[PublicizedFrom(EAccessModifier.Private)]
	public string valueLocalizationPrefixFromXml;

	// Token: 0x04004C1A RID: 19482
	[PublicizedFrom(EAccessModifier.Private)]
	public bool valuesEnforced;

	// Token: 0x04004C1B RID: 19483
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isTextInput;

	// Token: 0x04004C1C RID: 19484
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasDefault = true;

	// Token: 0x04004C1D RID: 19485
	[PublicizedFrom(EAccessModifier.Private)]
	public bool alwaysShow;

	// Token: 0x04004C1E RID: 19486
	[PublicizedFrom(EAccessModifier.Private)]
	public EnumGamePrefs gamePref = EnumGamePrefs.Last;

	// Token: 0x04004C1F RID: 19487
	[PublicizedFrom(EAccessModifier.Private)]
	public GamePrefs.EnumType valueType;

	// Token: 0x04004C20 RID: 19488
	[PublicizedFrom(EAccessModifier.Private)]
	public GameMode currentGameMode;

	// Token: 0x04004C21 RID: 19489
	[PublicizedFrom(EAccessModifier.Private)]
	public Func<int, int> valuePreDisplayModifierFunc;

	// Token: 0x04004C22 RID: 19490
	public Action<XUiC_GamePrefSelector, EnumGamePrefs> OnValueChanged;

	// Token: 0x04004C23 RID: 19491
	[PublicizedFrom(EAccessModifier.Private)]
	public bool enabled = true;

	// Token: 0x02000CBE RID: 3262
	public enum EOptionValueType
	{
		// Token: 0x04004C25 RID: 19493
		Null,
		// Token: 0x04004C26 RID: 19494
		Custom,
		// Token: 0x04004C27 RID: 19495
		Any,
		// Token: 0x04004C28 RID: 19496
		Int,
		// Token: 0x04004C29 RID: 19497
		String
	}

	// Token: 0x02000CBF RID: 3263
	public readonly struct GameOptionValue : IEquatable<XUiC_GamePrefSelector.GameOptionValue>
	{
		// Token: 0x060064E9 RID: 25833 RVA: 0x0028E0B2 File Offset: 0x0028C2B2
		public GameOptionValue(XUiC_GamePrefSelector.EOptionValueType _type, string _displayName)
		{
			this.Type = _type;
			this.IntValue = -1;
			this.StringValue = null;
			this.DisplayName = _displayName;
		}

		// Token: 0x060064EA RID: 25834 RVA: 0x0028E0D0 File Offset: 0x0028C2D0
		public GameOptionValue(int _intValue, string _displayName)
		{
			this.Type = XUiC_GamePrefSelector.EOptionValueType.Int;
			this.IntValue = _intValue;
			this.StringValue = null;
			this.DisplayName = _displayName;
		}

		// Token: 0x060064EB RID: 25835 RVA: 0x0028E0EE File Offset: 0x0028C2EE
		public GameOptionValue(string _stringValue, string _displayName)
		{
			this.Type = XUiC_GamePrefSelector.EOptionValueType.String;
			this.IntValue = -1;
			this.StringValue = _stringValue;
			this.DisplayName = _displayName;
		}

		// Token: 0x060064EC RID: 25836 RVA: 0x0028E10C File Offset: 0x0028C30C
		public override string ToString()
		{
			return this.DisplayName;
		}

		// Token: 0x060064ED RID: 25837 RVA: 0x0028E114 File Offset: 0x0028C314
		public bool Equals(XUiC_GamePrefSelector.GameOptionValue _other)
		{
			if (this.Type != _other.Type)
			{
				return false;
			}
			switch (this.Type)
			{
			case XUiC_GamePrefSelector.EOptionValueType.Custom:
			case XUiC_GamePrefSelector.EOptionValueType.Any:
				return true;
			case XUiC_GamePrefSelector.EOptionValueType.Int:
				return this.IntValue == _other.IntValue;
			case XUiC_GamePrefSelector.EOptionValueType.String:
				return this.StringValue == _other.StringValue;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x060064EE RID: 25838 RVA: 0x0028E17C File Offset: 0x0028C37C
		public override bool Equals(object _obj)
		{
			if (_obj is XUiC_GamePrefSelector.GameOptionValue)
			{
				XUiC_GamePrefSelector.GameOptionValue other = (XUiC_GamePrefSelector.GameOptionValue)_obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x060064EF RID: 25839 RVA: 0x0028E1A4 File Offset: 0x0028C3A4
		public override int GetHashCode()
		{
			return (int)(((this.Type * (XUiC_GamePrefSelector.EOptionValueType)397 ^ (XUiC_GamePrefSelector.EOptionValueType)this.IntValue) * (XUiC_GamePrefSelector.EOptionValueType)397 ^ (XUiC_GamePrefSelector.EOptionValueType)((this.StringValue != null) ? this.StringValue.GetHashCode() : 0)) * (XUiC_GamePrefSelector.EOptionValueType)397 ^ (XUiC_GamePrefSelector.EOptionValueType)((this.DisplayName != null) ? this.DisplayName.GetHashCode() : 0));
		}

		// Token: 0x04004C2A RID: 19498
		public readonly XUiC_GamePrefSelector.EOptionValueType Type;

		// Token: 0x04004C2B RID: 19499
		public readonly int IntValue;

		// Token: 0x04004C2C RID: 19500
		public readonly string StringValue;

		// Token: 0x04004C2D RID: 19501
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string DisplayName;
	}
}
