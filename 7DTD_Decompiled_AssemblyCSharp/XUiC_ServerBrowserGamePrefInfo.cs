using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.Scripting;

// Token: 0x02000E05 RID: 3589
[Preserve]
public class XUiC_ServerBrowserGamePrefInfo : XUiController
{
	// Token: 0x17000B4A RID: 2890
	// (get) Token: 0x06007068 RID: 28776 RVA: 0x002DDA6D File Offset: 0x002DBC6D
	public GamePrefs.EnumType ValueType
	{
		get
		{
			return this.valueType;
		}
	}

	// Token: 0x17000B4B RID: 2891
	// (get) Token: 0x06007069 RID: 28777 RVA: 0x002DDA75 File Offset: 0x002DBC75
	public GameInfoBool GameInfoBool
	{
		get
		{
			return this.gameInfoBool;
		}
	}

	// Token: 0x17000B4C RID: 2892
	// (get) Token: 0x0600706A RID: 28778 RVA: 0x002DDA7D File Offset: 0x002DBC7D
	public GameInfoInt GameInfoInt
	{
		get
		{
			return this.gameInfoInt;
		}
	}

	// Token: 0x17000B4D RID: 2893
	// (get) Token: 0x0600706B RID: 28779 RVA: 0x002DDA85 File Offset: 0x002DBC85
	public GameInfoString GameInfoString
	{
		get
		{
			return this.gameInfoString;
		}
	}

	// Token: 0x17000B4E RID: 2894
	// (set) Token: 0x0600706C RID: 28780 RVA: 0x002DDA8D File Offset: 0x002DBC8D
	public Func<int, int> ValuePreDisplayModifierFunc
	{
		set
		{
			if (this.valuePreDisplayModifierFunc != value)
			{
				this.valuePreDisplayModifierFunc = value;
				this.SetupOptions();
			}
		}
	}

	// Token: 0x17000B4F RID: 2895
	// (set) Token: 0x0600706D RID: 28781 RVA: 0x002DDAAA File Offset: 0x002DBCAA
	public Func<GameServerInfo, int, string> CustomIntValueFormatter
	{
		set
		{
			if (this.customIntValueFormatter != value)
			{
				this.customIntValueFormatter = value;
				this.IsDirty = true;
			}
		}
	}

	// Token: 0x17000B50 RID: 2896
	// (set) Token: 0x0600706E RID: 28782 RVA: 0x002DDAC8 File Offset: 0x002DBCC8
	public Func<GameServerInfo, string, string> CustomStringValueFormatter
	{
		set
		{
			if (this.customStringValueFormatter != value)
			{
				this.customStringValueFormatter = value;
				this.IsDirty = true;
			}
		}
	}

	// Token: 0x0600706F RID: 28783 RVA: 0x002DDAE8 File Offset: 0x002DBCE8
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
		if (EnumUtils.TryParse<GameInfoString>(this.viewComponent.ID, out this.gameInfoString, false))
		{
			this.valueType = GamePrefs.EnumType.String;
		}
		else
		{
			this.gameInfoString = (GameInfoString)(-1);
		}
		this.label = (XUiV_Label)base.GetChildById("value").ViewComponent;
		this.SetupOptions();
	}

	// Token: 0x06007070 RID: 28784 RVA: 0x002DDB98 File Offset: 0x002DBD98
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "display_names")
		{
			if (_value.Length > 0)
			{
				this.namesFromXml = _value.Split(',', StringSplitOptions.None);
				for (int i = 0; i < this.namesFromXml.Length; i++)
				{
					this.namesFromXml[i] = this.namesFromXml[i].Trim();
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

	// Token: 0x06007071 RID: 28785 RVA: 0x002DDC24 File Offset: 0x002DBE24
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupOptions()
	{
		if (this.valueType == GamePrefs.EnumType.Int)
		{
			if (this.namesFromXml != null)
			{
				this.values = new List<XUiC_ServerBrowserGamePrefInfo.GameOptionValue>();
				for (int i = 0; i < this.namesFromXml.Length; i++)
				{
					string text = this.namesFromXml[i];
					int value = i;
					if (text.IndexOf('=') > 0)
					{
						string[] array = text.Split('=', StringSplitOptions.None);
						text = array[1];
						value = StringParsers.ParseSInt32(array[0], 0, -1, NumberStyles.Integer);
					}
					this.values.Add(new XUiC_ServerBrowserGamePrefInfo.GameOptionValue(value, Localization.Get(text, false)));
				}
				return;
			}
		}
		else if (this.valueType == GamePrefs.EnumType.Bool)
		{
			this.values = new List<XUiC_ServerBrowserGamePrefInfo.GameOptionValue>
			{
				new XUiC_ServerBrowserGamePrefInfo.GameOptionValue(0, Localization.Get("goOff", false)),
				new XUiC_ServerBrowserGamePrefInfo.GameOptionValue(1, Localization.Get("goOn", false))
			};
		}
	}

	// Token: 0x06007072 RID: 28786 RVA: 0x002DDCEC File Offset: 0x002DBEEC
	public void SetCurrentValue(GameServerInfo _gameInfo)
	{
		try
		{
			if (_gameInfo != null)
			{
				if (this.valueType == GamePrefs.EnumType.Int)
				{
					int value = _gameInfo.GetValue(this.gameInfoInt);
					bool flag = false;
					if (this.values != null)
					{
						foreach (XUiC_ServerBrowserGamePrefInfo.GameOptionValue gameOptionValue in this.values)
						{
							if (gameOptionValue.Value == value)
							{
								this.label.Text = gameOptionValue.ToString();
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						if (this.customIntValueFormatter != null)
						{
							this.label.Text = this.customIntValueFormatter(_gameInfo, value);
						}
						else if (this.valueLocalizationPrefixFromXml != null)
						{
							this.label.Text = string.Format(Localization.Get(this.valueLocalizationPrefixFromXml + ((value == 1) ? "" : "s"), false), value);
						}
						else
						{
							this.label.Text = value.ToString();
						}
					}
				}
				else if (this.valueType == GamePrefs.EnumType.Bool)
				{
					bool value2 = _gameInfo.GetValue(this.gameInfoBool);
					this.label.Text = (value2 ? this.values[1].ToString() : this.values[0].ToString());
				}
				else if (this.valueType == GamePrefs.EnumType.String)
				{
					string value3 = _gameInfo.GetValue(this.gameInfoString);
					if (this.customStringValueFormatter != null)
					{
						this.label.Text = this.customStringValueFormatter(_gameInfo, value3);
					}
					else if (this.valueLocalizationPrefixFromXml != null && !string.IsNullOrEmpty(value3))
					{
						this.label.Text = Localization.Get(this.valueLocalizationPrefixFromXml + value3, false);
					}
					else
					{
						this.label.Text = value3;
					}
				}
			}
			else
			{
				this.label.Text = "";
			}
		}
		catch (Exception e)
		{
			Log.Exception(e);
		}
	}

	// Token: 0x04005567 RID: 21863
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label label;

	// Token: 0x04005568 RID: 21864
	[PublicizedFrom(EAccessModifier.Private)]
	public string[] namesFromXml;

	// Token: 0x04005569 RID: 21865
	[PublicizedFrom(EAccessModifier.Private)]
	public string valueLocalizationPrefixFromXml;

	// Token: 0x0400556A RID: 21866
	[PublicizedFrom(EAccessModifier.Private)]
	public GamePrefs.EnumType valueType;

	// Token: 0x0400556B RID: 21867
	[PublicizedFrom(EAccessModifier.Private)]
	public GameInfoBool gameInfoBool;

	// Token: 0x0400556C RID: 21868
	[PublicizedFrom(EAccessModifier.Private)]
	public GameInfoInt gameInfoInt;

	// Token: 0x0400556D RID: 21869
	[PublicizedFrom(EAccessModifier.Private)]
	public GameInfoString gameInfoString;

	// Token: 0x0400556E RID: 21870
	[PublicizedFrom(EAccessModifier.Private)]
	public List<XUiC_ServerBrowserGamePrefInfo.GameOptionValue> values;

	// Token: 0x0400556F RID: 21871
	[PublicizedFrom(EAccessModifier.Private)]
	public Func<int, int> valuePreDisplayModifierFunc;

	// Token: 0x04005570 RID: 21872
	[PublicizedFrom(EAccessModifier.Private)]
	public Func<GameServerInfo, int, string> customIntValueFormatter;

	// Token: 0x04005571 RID: 21873
	[PublicizedFrom(EAccessModifier.Private)]
	public Func<GameServerInfo, string, string> customStringValueFormatter;

	// Token: 0x02000E06 RID: 3590
	public struct GameOptionValue
	{
		// Token: 0x06007074 RID: 28788 RVA: 0x002DDF28 File Offset: 0x002DC128
		public GameOptionValue(int _value, string _displayName)
		{
			this.Value = _value;
			this.DisplayName = _displayName;
		}

		// Token: 0x06007075 RID: 28789 RVA: 0x002DDF38 File Offset: 0x002DC138
		public override string ToString()
		{
			return this.DisplayName;
		}

		// Token: 0x04005572 RID: 21874
		public readonly int Value;

		// Token: 0x04005573 RID: 21875
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string DisplayName;
	}
}
