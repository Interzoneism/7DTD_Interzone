using System;
using System.Globalization;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000E07 RID: 3591
[Preserve]
public class XUiC_ServerBrowserGamePrefSelector : XUiController
{
	// Token: 0x17000B51 RID: 2897
	// (get) Token: 0x06007076 RID: 28790 RVA: 0x002DDF40 File Offset: 0x002DC140
	public GamePrefs.EnumType ValueType
	{
		get
		{
			return this.valueType;
		}
	}

	// Token: 0x17000B52 RID: 2898
	// (get) Token: 0x06007077 RID: 28791 RVA: 0x002DDF48 File Offset: 0x002DC148
	public GameInfoBool GameInfoBool
	{
		get
		{
			return this.gameInfoBool;
		}
	}

	// Token: 0x17000B53 RID: 2899
	// (get) Token: 0x06007078 RID: 28792 RVA: 0x002DDF50 File Offset: 0x002DC150
	public GameInfoInt GameInfoInt
	{
		get
		{
			return this.gameInfoInt;
		}
	}

	// Token: 0x17000B54 RID: 2900
	// (get) Token: 0x06007079 RID: 28793 RVA: 0x002DDF58 File Offset: 0x002DC158
	// (set) Token: 0x0600707A RID: 28794 RVA: 0x002DDF60 File Offset: 0x002DC160
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

	// Token: 0x0600707B RID: 28795 RVA: 0x002DDF78 File Offset: 0x002DC178
	public override void Init()
	{
		base.Init();
		if (EnumUtils.TryParse<GameInfoBool>(this.viewComponent.ID, out this.gameInfoBool, false))
		{
			this.valueType = GamePrefs.EnumType.Bool;
		}
		else
		{
			this.gameInfoBool = (GameInfoBool)(-1);
		}
		if (EnumUtils.TryParse<GameInfoInt>(this.viewComponent.ID, out this.gameInfoInt, false))
		{
			this.valueType = GamePrefs.EnumType.Int;
		}
		else
		{
			this.gameInfoInt = (GameInfoInt)(-1);
		}
		this.controlCombo = base.GetChildById("ControlCombo").GetChildByType<XUiC_ComboBoxList<XUiC_ServerBrowserGamePrefSelector.GameOptionValue>>();
		this.controlCombo.OnValueChanged += this.ControlCombo_OnValueChanged;
		this.SetupOptions();
	}

	// Token: 0x0600707C RID: 28796 RVA: 0x002DE010 File Offset: 0x002DC210
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "values")
		{
			if (_value.Length > 0)
			{
				string[] array = _value.Split(',', StringSplitOptions.None);
				this.valuesFromXml = new int[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					this.valuesFromXml[i] = StringParsers.ParseSInt32(array[i], 0, -1, NumberStyles.Integer);
				}
			}
			return true;
		}
		if (_name == "display_names")
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
		if (!(_name == "value_localization_prefix"))
		{
			return base.ParseAttribute(_name, _value, _parent);
		}
		if (_value.Length > 0)
		{
			this.valueLocalizationPrefixFromXml = _value.Trim();
		}
		return true;
	}

	// Token: 0x0600707D RID: 28797 RVA: 0x002DE0F4 File Offset: 0x002DC2F4
	[PublicizedFrom(EAccessModifier.Private)]
	public string GetGameInfoName()
	{
		GamePrefs.EnumType enumType = this.valueType;
		if (enumType == GamePrefs.EnumType.Int)
		{
			return this.gameInfoInt.ToStringCached<GameInfoInt>();
		}
		if (enumType == GamePrefs.EnumType.Bool)
		{
			return this.gameInfoBool.ToStringCached<GameInfoBool>();
		}
		return null;
	}

	// Token: 0x0600707E RID: 28798 RVA: 0x002DE128 File Offset: 0x002DC328
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupOptions()
	{
		string[] array = null;
		if (this.valueType == GamePrefs.EnumType.Int)
		{
			if (this.valuesFromXml == null)
			{
				if (this.namesFromXml == null)
				{
					throw new Exception("Illegal option setup for " + this.GetGameInfoName() + " (no values and no names specified)");
				}
				this.valuesFromXml = new int[this.namesFromXml.Length];
				for (int i = 0; i < this.valuesFromXml.Length; i++)
				{
					this.valuesFromXml[i] = i;
				}
			}
			array = new string[this.valuesFromXml.Length];
			if (this.namesFromXml == null || this.namesFromXml.Length != this.valuesFromXml.Length)
			{
				for (int j = 0; j < this.valuesFromXml.Length; j++)
				{
					if (this.namesFromXml != null && j < this.namesFromXml.Length)
					{
						array[j] = Localization.Get(this.namesFromXml[j], false);
					}
					else
					{
						int num = this.valuesFromXml[j];
						if (this.valuePreDisplayModifierFunc != null)
						{
							num = this.valuePreDisplayModifierFunc(num);
						}
						array[j] = string.Format(Localization.Get(this.valueLocalizationPrefixFromXml + ((num == 1) ? "" : "s"), false), num);
					}
				}
			}
			else
			{
				for (int k = 0; k < this.namesFromXml.Length; k++)
				{
					array[k] = Localization.Get(this.namesFromXml[k], false);
				}
			}
		}
		else if (this.valueType == GamePrefs.EnumType.Bool)
		{
			this.valuesFromXml = new int[]
			{
				0,
				1
			};
			array = new string[]
			{
				Localization.Get("goOff", false),
				Localization.Get("goOn", false)
			};
		}
		this.controlCombo.Elements.Clear();
		XUiC_ServerBrowserGamePrefSelector.GameOptionValue item = new XUiC_ServerBrowserGamePrefSelector.GameOptionValue(int.MinValue, Localization.Get("goAnyValue", false));
		this.controlCombo.Elements.Add(item);
		if (this.valuesFromXml == null)
		{
			throw new Exception("Illegal option setup for " + this.GetGameInfoName() + " (values still null)");
		}
		if (array == null)
		{
			throw new Exception("Illegal option setup for " + this.GetGameInfoName() + " (names null)");
		}
		for (int l = 0; l < this.valuesFromXml.Length; l++)
		{
			item = new XUiC_ServerBrowserGamePrefSelector.GameOptionValue(this.valuesFromXml[l], array[l]);
			this.controlCombo.Elements.Add(item);
		}
	}

	// Token: 0x0600707F RID: 28799 RVA: 0x002DE376 File Offset: 0x002DC576
	[PublicizedFrom(EAccessModifier.Private)]
	public void ControlCombo_OnValueChanged(XUiController _sender, XUiC_ServerBrowserGamePrefSelector.GameOptionValue _oldValue, XUiC_ServerBrowserGamePrefSelector.GameOptionValue _newValue)
	{
		if (this.OnValueChanged != null)
		{
			this.OnValueChanged(this);
		}
	}

	// Token: 0x06007080 RID: 28800 RVA: 0x002DE38C File Offset: 0x002DC58C
	public void SetCurrentValue(object _value)
	{
		try
		{
			if (this.valueType == GamePrefs.EnumType.Int)
			{
				int num = (int)_value;
				bool flag = false;
				for (int i = 1; i < this.controlCombo.Elements.Count; i++)
				{
					if (this.controlCombo.Elements[i].Value == num)
					{
						this.controlCombo.SelectedIndex = i;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					this.controlCombo.SelectedIndex = this.controlCombo.MinIndex;
				}
			}
			else if (this.valueType == GamePrefs.EnumType.Bool)
			{
				this.controlCombo.SelectedIndex = (((bool)_value) ? 2 : 1);
			}
		}
		catch (Exception e)
		{
			Log.Exception(e);
		}
	}

	// Token: 0x06007081 RID: 28801 RVA: 0x002DE444 File Offset: 0x002DC644
	public XUiC_ServersList.UiServerFilter GetFilter()
	{
		int value = this.controlCombo.Value.Value;
		bool flag = value == int.MinValue;
		GamePrefs.EnumType enumType = this.valueType;
		string name;
		if (enumType != GamePrefs.EnumType.Int)
		{
			if (enumType != GamePrefs.EnumType.Bool)
			{
				throw new ArgumentOutOfRangeException();
			}
			name = this.gameInfoBool.ToStringCached<GameInfoBool>();
		}
		else
		{
			name = this.gameInfoInt.ToStringCached<GameInfoInt>();
		}
		if (flag)
		{
			return new XUiC_ServersList.UiServerFilter(name, XUiC_ServersList.EnumServerLists.Regular, null, IServerListInterface.ServerFilter.EServerFilterType.Any, 0, 0, false, null);
		}
		int intMinValue = 0;
		bool boolValue = false;
		enumType = this.valueType;
		Func<XUiC_ServersList.ListEntry, bool> func;
		IServerListInterface.ServerFilter.EServerFilterType type;
		if (enumType != GamePrefs.EnumType.Int)
		{
			if (enumType != GamePrefs.EnumType.Bool)
			{
				throw new ArgumentOutOfRangeException();
			}
			bool bValue = value == 1;
			func = ((XUiC_ServersList.ListEntry _entry) => _entry.gameServerInfo.GetValue(this.gameInfoBool) == bValue);
			type = IServerListInterface.ServerFilter.EServerFilterType.BoolValue;
			boolValue = bValue;
		}
		else
		{
			int iValue = value;
			func = ((XUiC_ServersList.ListEntry _entry) => _entry.gameServerInfo.GetValue(this.gameInfoInt) == iValue);
			type = IServerListInterface.ServerFilter.EServerFilterType.IntValue;
			intMinValue = iValue;
		}
		return new XUiC_ServersList.UiServerFilter(name, XUiC_ServersList.EnumServerLists.Regular, func, type, intMinValue, 0, boolValue, null);
	}

	// Token: 0x04005574 RID: 21876
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<XUiC_ServerBrowserGamePrefSelector.GameOptionValue> controlCombo;

	// Token: 0x04005575 RID: 21877
	[PublicizedFrom(EAccessModifier.Private)]
	public int[] valuesFromXml;

	// Token: 0x04005576 RID: 21878
	[PublicizedFrom(EAccessModifier.Private)]
	public string[] namesFromXml;

	// Token: 0x04005577 RID: 21879
	[PublicizedFrom(EAccessModifier.Private)]
	public string valueLocalizationPrefixFromXml;

	// Token: 0x04005578 RID: 21880
	[PublicizedFrom(EAccessModifier.Private)]
	public GamePrefs.EnumType valueType;

	// Token: 0x04005579 RID: 21881
	[PublicizedFrom(EAccessModifier.Private)]
	public GameInfoBool gameInfoBool;

	// Token: 0x0400557A RID: 21882
	[PublicizedFrom(EAccessModifier.Private)]
	public GameInfoInt gameInfoInt;

	// Token: 0x0400557B RID: 21883
	[PublicizedFrom(EAccessModifier.Private)]
	public Func<int, int> valuePreDisplayModifierFunc;

	// Token: 0x0400557C RID: 21884
	public Action<XUiC_ServerBrowserGamePrefSelector> OnValueChanged;

	// Token: 0x02000E08 RID: 3592
	public struct GameOptionValue
	{
		// Token: 0x06007083 RID: 28803 RVA: 0x002DE531 File Offset: 0x002DC731
		public GameOptionValue(int _value, string _displayName)
		{
			this.Value = _value;
			this.DisplayName = _displayName;
		}

		// Token: 0x06007084 RID: 28804 RVA: 0x002DE541 File Offset: 0x002DC741
		public override string ToString()
		{
			return this.DisplayName;
		}

		// Token: 0x0400557D RID: 21885
		public readonly int Value;

		// Token: 0x0400557E RID: 21886
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string DisplayName;
	}
}
