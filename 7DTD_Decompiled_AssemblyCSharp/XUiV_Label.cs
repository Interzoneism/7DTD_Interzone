using System;
using System.Collections.Generic;
using System.Globalization;
using Platform;
using UnityEngine;

// Token: 0x02000F07 RID: 3847
public class XUiV_Label : XUiView
{
	// Token: 0x060079EB RID: 31211 RVA: 0x003184BD File Offset: 0x003166BD
	[PublicizedFrom(EAccessModifier.Private)]
	static XUiV_Label()
	{
		Localization.LanguageSelected += XUiV_Label.OnLanguageSelected;
		XUiV_Label.OnLanguageSelected(Localization.RequestedLanguage);
	}

	// Token: 0x060079EC RID: 31212 RVA: 0x003184EC File Offset: 0x003166EC
	[PublicizedFrom(EAccessModifier.Private)]
	public static void OnLanguageSelected(string _lang)
	{
		string text = Localization.Get("cultureInfoName", false);
		if (string.IsNullOrEmpty(text))
		{
			Log.Warning("No culture info name given for selected language: " + _lang);
			return;
		}
		TextInfo textInfo;
		try
		{
			textInfo = CultureInfo.GetCultureInfo(text).TextInfo;
		}
		catch (Exception)
		{
			Log.Warning(string.Concat(new string[]
			{
				"No culture info found for given name: ",
				text,
				" (language: ",
				_lang,
				")"
			}));
			return;
		}
		if (textInfo.CultureName != XUiV_Label.textInfo.CultureName)
		{
			XUiV_Label.textInfo = textInfo;
			Log.Out("Updated culture for display texts");
		}
	}

	// Token: 0x17000C7A RID: 3194
	// (get) Token: 0x060079ED RID: 31213 RVA: 0x00318598 File Offset: 0x00316798
	// (set) Token: 0x060079EE RID: 31214 RVA: 0x003185A0 File Offset: 0x003167A0
	public UILabel Label
	{
		get
		{
			return this.label;
		}
		set
		{
			this.label = value;
			this.isDirty = true;
		}
	}

	// Token: 0x17000C7B RID: 3195
	// (get) Token: 0x060079EF RID: 31215 RVA: 0x003185B0 File Offset: 0x003167B0
	// (set) Token: 0x060079F0 RID: 31216 RVA: 0x003185B8 File Offset: 0x003167B8
	public NGUIFont UIFont
	{
		get
		{
			return this.uiFont;
		}
		set
		{
			this.uiFont = value;
			this.isDirty = true;
		}
	}

	// Token: 0x17000C7C RID: 3196
	// (get) Token: 0x060079F1 RID: 31217 RVA: 0x003185C8 File Offset: 0x003167C8
	// (set) Token: 0x060079F2 RID: 31218 RVA: 0x003185D0 File Offset: 0x003167D0
	public int FontSize
	{
		get
		{
			return this.fontSize;
		}
		set
		{
			this.fontSize = value;
			this.isDirty = true;
		}
	}

	// Token: 0x17000C7D RID: 3197
	// (get) Token: 0x060079F3 RID: 31219 RVA: 0x003185E0 File Offset: 0x003167E0
	// (set) Token: 0x060079F4 RID: 31220 RVA: 0x003185E8 File Offset: 0x003167E8
	public UILabel.Overflow Overflow
	{
		get
		{
			return this.overflow;
		}
		set
		{
			this.overflow = value;
			this.isDirty = true;
		}
	}

	// Token: 0x17000C7E RID: 3198
	// (get) Token: 0x060079F5 RID: 31221 RVA: 0x003185F8 File Offset: 0x003167F8
	// (set) Token: 0x060079F6 RID: 31222 RVA: 0x00318600 File Offset: 0x00316800
	public int OverflowHeight
	{
		get
		{
			return this.overflowHeight;
		}
		set
		{
			this.overflowHeight = value;
			this.isDirty = true;
		}
	}

	// Token: 0x17000C7F RID: 3199
	// (get) Token: 0x060079F7 RID: 31223 RVA: 0x00318610 File Offset: 0x00316810
	// (set) Token: 0x060079F8 RID: 31224 RVA: 0x00318618 File Offset: 0x00316818
	public int OverflowWidth
	{
		get
		{
			return this.overflowWidth;
		}
		set
		{
			this.overflowWidth = value;
			this.isDirty = true;
		}
	}

	// Token: 0x17000C80 RID: 3200
	// (get) Token: 0x060079F9 RID: 31225 RVA: 0x00318628 File Offset: 0x00316828
	// (set) Token: 0x060079FA RID: 31226 RVA: 0x00318630 File Offset: 0x00316830
	public UILabel.Effect Effect
	{
		get
		{
			return this.effect;
		}
		set
		{
			this.effect = value;
			this.isDirty = true;
		}
	}

	// Token: 0x17000C81 RID: 3201
	// (get) Token: 0x060079FB RID: 31227 RVA: 0x00318640 File Offset: 0x00316840
	// (set) Token: 0x060079FC RID: 31228 RVA: 0x00318648 File Offset: 0x00316848
	public Color EffectColor
	{
		get
		{
			return this.effectColor;
		}
		set
		{
			this.effectColor = value;
			this.isDirty = true;
		}
	}

	// Token: 0x17000C82 RID: 3202
	// (get) Token: 0x060079FD RID: 31229 RVA: 0x00318658 File Offset: 0x00316858
	// (set) Token: 0x060079FE RID: 31230 RVA: 0x00318660 File Offset: 0x00316860
	public Vector2 EffectDistance
	{
		get
		{
			return this.effectDistance;
		}
		set
		{
			this.effectDistance = value;
			this.isDirty = true;
		}
	}

	// Token: 0x17000C83 RID: 3203
	// (get) Token: 0x060079FF RID: 31231 RVA: 0x00318670 File Offset: 0x00316870
	// (set) Token: 0x06007A00 RID: 31232 RVA: 0x00318678 File Offset: 0x00316878
	public UILabel.Crispness Crispness
	{
		get
		{
			return this.crispness;
		}
		set
		{
			this.crispness = value;
			this.isDirty = true;
		}
	}

	// Token: 0x17000C84 RID: 3204
	// (get) Token: 0x06007A01 RID: 31233 RVA: 0x00318688 File Offset: 0x00316888
	// (set) Token: 0x06007A02 RID: 31234 RVA: 0x00318690 File Offset: 0x00316890
	public string Text
	{
		get
		{
			return this.text;
		}
		set
		{
			if (this.text != value)
			{
				this.text = value;
				this.isDirty = true;
				this.bUpdateText = true;
			}
		}
	}

	// Token: 0x17000C85 RID: 3205
	// (get) Token: 0x06007A03 RID: 31235 RVA: 0x003186B5 File Offset: 0x003168B5
	// (set) Token: 0x06007A04 RID: 31236 RVA: 0x003186BD File Offset: 0x003168BD
	public Color Color
	{
		get
		{
			return this.color;
		}
		set
		{
			if (this.color != value)
			{
				this.color = value;
				this.isDirty = true;
			}
		}
	}

	// Token: 0x17000C86 RID: 3206
	// (get) Token: 0x06007A05 RID: 31237 RVA: 0x003186DB File Offset: 0x003168DB
	// (set) Token: 0x06007A06 RID: 31238 RVA: 0x003186E3 File Offset: 0x003168E3
	public NGUIText.Alignment Alignment
	{
		get
		{
			return this.alignment;
		}
		set
		{
			if (this.alignment != value)
			{
				this.alignment = value;
				this.isDirty = true;
			}
		}
	}

	// Token: 0x17000C87 RID: 3207
	// (get) Token: 0x06007A07 RID: 31239 RVA: 0x003186FC File Offset: 0x003168FC
	// (set) Token: 0x06007A08 RID: 31240 RVA: 0x00318704 File Offset: 0x00316904
	public bool SupportBbCode
	{
		get
		{
			return this.supportBbCode;
		}
		set
		{
			if (this.supportBbCode != value)
			{
				this.supportBbCode = value;
				this.isDirty = true;
			}
		}
	}

	// Token: 0x17000C88 RID: 3208
	// (get) Token: 0x06007A09 RID: 31241 RVA: 0x0031871D File Offset: 0x0031691D
	// (set) Token: 0x06007A0A RID: 31242 RVA: 0x00318725 File Offset: 0x00316925
	public int MaxLineCount
	{
		get
		{
			return this.maxLineCount;
		}
		set
		{
			if (value != this.maxLineCount)
			{
				this.maxLineCount = value;
				this.isDirty = true;
			}
		}
	}

	// Token: 0x17000C89 RID: 3209
	// (get) Token: 0x06007A0B RID: 31243 RVA: 0x0031873E File Offset: 0x0031693E
	// (set) Token: 0x06007A0C RID: 31244 RVA: 0x00318746 File Offset: 0x00316946
	public int SpacingX
	{
		get
		{
			return this.spacingX;
		}
		set
		{
			if (value != this.spacingX)
			{
				this.spacingX = value;
				this.isDirty = true;
			}
		}
	}

	// Token: 0x17000C8A RID: 3210
	// (get) Token: 0x06007A0D RID: 31245 RVA: 0x0031875F File Offset: 0x0031695F
	// (set) Token: 0x06007A0E RID: 31246 RVA: 0x00318767 File Offset: 0x00316967
	public int SpacingY
	{
		get
		{
			return this.spacingY;
		}
		set
		{
			if (value != this.spacingY)
			{
				this.spacingY = value;
				this.isDirty = true;
			}
		}
	}

	// Token: 0x17000C8B RID: 3211
	// (get) Token: 0x06007A0F RID: 31247 RVA: 0x00318780 File Offset: 0x00316980
	// (set) Token: 0x06007A10 RID: 31248 RVA: 0x00318788 File Offset: 0x00316988
	public float GlobalOpacityModifier
	{
		get
		{
			return this.globalOpacityModifier;
		}
		set
		{
			this.globalOpacityModifier = value;
			this.isDirty = true;
		}
	}

	// Token: 0x17000C8C RID: 3212
	// (get) Token: 0x06007A11 RID: 31249 RVA: 0x00318798 File Offset: 0x00316998
	public Bounds LabelBounds
	{
		get
		{
			return this.label.CalculateBounds();
		}
	}

	// Token: 0x06007A12 RID: 31250 RVA: 0x003187A5 File Offset: 0x003169A5
	public XUiV_Label(string _id) : base(_id)
	{
	}

	// Token: 0x06007A13 RID: 31251 RVA: 0x003187DC File Offset: 0x003169DC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CreateComponents(GameObject _go)
	{
		_go.AddComponent<UILabel>();
	}

	// Token: 0x06007A14 RID: 31252 RVA: 0x003187E8 File Offset: 0x003169E8
	public override void InitView()
	{
		if (this.supportUrls)
		{
			this.EventOnPress = true;
			this.controller.OnPress += delegate(XUiController _, int _)
			{
				XUiUtils.HandleLabelUrlClick(this, this.label, this.supportedUrlTypes);
			};
		}
		base.InitView();
		this.label = this.uiTransform.GetComponent<UILabel>();
		if (this.UIFont != null)
		{
			this.UpdateData();
		}
		PlatformManager.NativePlatform.Input.OnLastInputStyleChanged += this.OnLastInputStyleChanged;
		this.initialized = true;
	}

	// Token: 0x06007A15 RID: 31253 RVA: 0x00318868 File Offset: 0x00316A68
	public override void Cleanup()
	{
		base.Cleanup();
		IPlatform nativePlatform = PlatformManager.NativePlatform;
		if (((nativePlatform != null) ? nativePlatform.Input : null) != null)
		{
			PlatformManager.NativePlatform.Input.OnLastInputStyleChanged -= this.OnLastInputStyleChanged;
		}
	}

	// Token: 0x06007A16 RID: 31254 RVA: 0x0031889E File Offset: 0x00316A9E
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (base.xui.GlobalOpacityChanged)
		{
			this.isDirty = true;
		}
	}

	// Token: 0x06007A17 RID: 31255 RVA: 0x003188BC File Offset: 0x00316ABC
	public override void UpdateData()
	{
		base.UpdateData();
		if (this.uiFont != null)
		{
			this.label.font = this.uiFont;
		}
		this.label.depth = this.depth;
		this.label.symbolDepth = this.depth + 1;
		this.label.fontSize = this.fontSize;
		base.parseAnchors(this.label, this.label.overflowMethod != UILabel.Overflow.ResizeFreely);
		this.label.supportEncoding = this.supportBbCode;
		if (this.text != null && this.bUpdateText)
		{
			this.label.text = this.GetFormattedText(this.text);
			this.bUpdateText = false;
		}
		this.label.supportEncoding = this.supportBbCode;
		this.label.color = this.color;
		this.label.alignment = this.alignment;
		this.label.keepCrispWhenShrunk = this.crispness;
		this.label.effectStyle = this.effect;
		this.label.effectColor = this.effectColor;
		this.label.effectDistance = this.effectDistance;
		this.label.overflowMethod = this.overflow;
		this.label.overflowWidth = this.overflowWidth;
		this.label.overflowHeight = this.overflowHeight;
		this.label.spacingX = this.spacingX;
		this.label.spacingY = this.spacingY;
		this.label.maxLineCount = this.maxLineCount;
		if (!this.initialized)
		{
			this.label.pivot = this.pivot;
			this.uiTransform.localScale = Vector3.one;
			this.uiTransform.localPosition = new Vector3((float)base.PaddedPosition.x, (float)base.PaddedPosition.y, 0f);
			if (this.EventOnHover || this.EventOnPress)
			{
				BoxCollider collider = this.collider;
				collider.center = this.Label.localCenter;
				collider.size = new Vector3(this.label.localSize.x * this.colliderScale, this.label.localSize.y * this.colliderScale, 0f);
			}
		}
	}

	// Token: 0x06007A18 RID: 31256 RVA: 0x00318B19 File Offset: 0x00316D19
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnLastInputStyleChanged(PlayerInputManager.InputStyle _obj)
	{
		if (this.parseActions && this.currentTextHasActions)
		{
			this.ForceTextUpdate();
		}
	}

	// Token: 0x06007A19 RID: 31257 RVA: 0x00318B34 File Offset: 0x00316D34
	[PublicizedFrom(EAccessModifier.Protected)]
	public string GetFormattedText(string _text)
	{
		if (this.upperCase)
		{
			_text = XUiV_Label.textInfo.ToUpper(_text);
		}
		else if (this.lowerCase)
		{
			_text = XUiV_Label.textInfo.ToLower(_text);
		}
		if (this.parseActions)
		{
			this.currentTextHasActions = XUiUtils.ParseActionsMarkup(base.xui, _text, out _text, this.actionsDefaultFormat, this.forceInputStyle);
		}
		return _text;
	}

	// Token: 0x06007A1A RID: 31258 RVA: 0x00318B96 File Offset: 0x00316D96
	public override void SetDefaults(XUiController _parent)
	{
		base.SetDefaults(_parent);
		this.Alignment = NGUIText.Alignment.Left;
		this.FontSize = 16;
		this.overflow = UILabel.Overflow.ShrinkContent;
	}

	// Token: 0x06007A1B RID: 31259 RVA: 0x00318BB5 File Offset: 0x00316DB5
	public void SetText(string _text)
	{
		this.Text = _text;
	}

	// Token: 0x06007A1C RID: 31260 RVA: 0x00318BC0 File Offset: 0x00316DC0
	public override bool ParseAttribute(string attribute, string value, XUiController _parent)
	{
		bool flag = base.ParseAttribute(attribute, value, _parent);
		if (!flag)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(attribute);
			if (num <= 2396994324U)
			{
				if (num <= 1519750509U)
				{
					if (num <= 815142563U)
					{
						if (num != 233351102U)
						{
							if (num != 250128721U)
							{
								if (num == 815142563U)
								{
									if (attribute == "lower_case")
									{
										this.lowerCase = StringParsers.ParseBool(value, 0, -1, true);
										return true;
									}
								}
							}
							else if (attribute == "spacing_x")
							{
								this.spacingX = StringParsers.ParseSInt32(value, 0, -1, NumberStyles.Integer);
								return true;
							}
						}
						else if (attribute == "spacing_y")
						{
							this.spacingY = StringParsers.ParseSInt32(value, 0, -1, NumberStyles.Integer);
							return true;
						}
					}
					else if (num != 1021845972U)
					{
						if (num != 1031692888U)
						{
							if (num == 1519750509U)
							{
								if (attribute == "force_input_style")
								{
									this.forceInputStyle = EnumUtils.Parse<XUiUtils.ForceLabelInputStyle>(value, true);
									return true;
								}
							}
						}
						else if (attribute == "color")
						{
							this.Color = StringParsers.ParseColor32(value);
							return true;
						}
					}
					else if (attribute == "upper_case")
					{
						this.upperCase = StringParsers.ParseBool(value, 0, -1, true);
						return true;
					}
				}
				else if (num <= 2023240872U)
				{
					if (num != 1788384520U)
					{
						if (num != 1852738900U)
						{
							if (num == 2023240872U)
							{
								if (attribute == "overflow_width")
								{
									this.OverflowWidth = StringParsers.ParseSInt32(value, 0, -1, NumberStyles.Integer);
									return true;
								}
							}
						}
						else if (attribute == "effect")
						{
							this.Effect = EnumUtils.Parse<UILabel.Effect>(value, true);
							return true;
						}
					}
					else if (attribute == "parse_actions")
					{
						this.parseActions = StringParsers.ParseBool(value, 0, -1, true);
						return true;
					}
				}
				else if (num != 2140393898U)
				{
					if (num != 2274577235U)
					{
						if (num == 2396994324U)
						{
							if (attribute == "font_size")
							{
								this.FontSize = int.Parse(value);
								return true;
							}
						}
					}
					else if (attribute == "crispness")
					{
						this.Crispness = EnumUtils.Parse<UILabel.Crispness>(value, true);
						return true;
					}
				}
				else if (attribute == "text_key")
				{
					if (!string.IsNullOrEmpty(value))
					{
						this.Text = Localization.Get(value, false);
						return true;
					}
					return true;
				}
			}
			else if (num <= 2887479055U)
			{
				if (num <= 2572986219U)
				{
					if (num != 2424738503U)
					{
						if (num != 2439222772U)
						{
							if (num == 2572986219U)
							{
								if (attribute == "overflow")
								{
									this.Overflow = EnumUtils.Parse<UILabel.Overflow>(value, true);
									return true;
								}
							}
						}
						else if (attribute == "font_face")
						{
							this.UIFont = base.xui.GetUIFontByName(value, false);
							if (this.UIFont == null)
							{
								Log.Warning(string.Concat(new string[]
								{
									"XUi Label: Font not found: ",
									value,
									", from: ",
									base.Controller.GetParentWindow().ID,
									".",
									base.ID
								}));
								return true;
							}
							return true;
						}
					}
					else if (attribute == "support_urls")
					{
						if (value.EqualsCaseInsensitive("false"))
						{
							this.supportUrls = false;
							return true;
						}
						this.supportUrls = true;
						if (value.EqualsCaseInsensitive("true"))
						{
							this.supportedUrlTypes = new HashSet<string>
							{
								"HTTP"
							};
							return true;
						}
						this.supportedUrlTypes = new HashSet<string>(value.Split(",", StringSplitOptions.None));
						return true;
					}
				}
				else if (num != 2689418572U)
				{
					if (num != 2863932660U)
					{
						if (num == 2887479055U)
						{
							if (attribute == "justify")
							{
								this.Alignment = EnumUtils.Parse<NGUIText.Alignment>(value, true);
								return true;
							}
						}
					}
					else if (attribute == "effect_color")
					{
						this.EffectColor = StringParsers.ParseColor32(value);
						return true;
					}
				}
				else if (attribute == "max_line_count")
				{
					this.maxLineCount = StringParsers.ParseSInt32(value, 0, -1, NumberStyles.Integer);
					return true;
				}
			}
			else if (num <= 3446912195U)
			{
				if (num != 3060355671U)
				{
					if (num != 3185987134U)
					{
						if (num == 3446912195U)
						{
							if (attribute == "support_bb_code")
							{
								this.supportBbCode = StringParsers.ParseBool(value, 0, -1, true);
								return true;
							}
						}
					}
					else if (attribute == "text")
					{
						this.Text = value;
						return true;
					}
				}
				else if (attribute == "globalopacity")
				{
					if (!StringParsers.ParseBool(value, 0, -1, true))
					{
						this.GlobalOpacityModifier = 0f;
						return true;
					}
					return true;
				}
			}
			else if (num <= 4144336821U)
			{
				if (num != 3458663872U)
				{
					if (num == 4144336821U)
					{
						if (attribute == "globalopacitymod")
						{
							this.GlobalOpacityModifier = StringParsers.ParseFloat(value, 0, -1, NumberStyles.Any);
							return true;
						}
					}
				}
				else if (attribute == "actions_default_format")
				{
					this.actionsDefaultFormat = value;
					return true;
				}
			}
			else if (num != 4211482175U)
			{
				if (num == 4255248174U)
				{
					if (attribute == "effect_distance")
					{
						this.EffectDistance = StringParsers.ParseVector2(value);
						return true;
					}
				}
			}
			else if (attribute == "overflow_height")
			{
				this.OverflowHeight = StringParsers.ParseSInt32(value, 0, -1, NumberStyles.Integer);
				return true;
			}
			return false;
		}
		return flag;
	}

	// Token: 0x06007A1D RID: 31261 RVA: 0x003191FA File Offset: 0x003173FA
	public void SetTextImmediately(string _text)
	{
		if (this.label != null)
		{
			this.text = _text;
			this.label.text = this.GetFormattedText(this.text);
		}
	}

	// Token: 0x06007A1E RID: 31262 RVA: 0x00319228 File Offset: 0x00317428
	public void ForceTextUpdate()
	{
		this.bUpdateText = true;
		this.isDirty = true;
	}

	// Token: 0x04005C70 RID: 23664
	[PublicizedFrom(EAccessModifier.Protected)]
	public static TextInfo textInfo = Utils.StandardCulture.TextInfo;

	// Token: 0x04005C71 RID: 23665
	[PublicizedFrom(EAccessModifier.Protected)]
	public UILabel label;

	// Token: 0x04005C72 RID: 23666
	[PublicizedFrom(EAccessModifier.Protected)]
	public NGUIFont uiFont;

	// Token: 0x04005C73 RID: 23667
	[PublicizedFrom(EAccessModifier.Protected)]
	public int fontSize;

	// Token: 0x04005C74 RID: 23668
	[PublicizedFrom(EAccessModifier.Protected)]
	public UILabel.Overflow overflow;

	// Token: 0x04005C75 RID: 23669
	[PublicizedFrom(EAccessModifier.Protected)]
	public int overflowHeight;

	// Token: 0x04005C76 RID: 23670
	[PublicizedFrom(EAccessModifier.Protected)]
	public int overflowWidth;

	// Token: 0x04005C77 RID: 23671
	[PublicizedFrom(EAccessModifier.Protected)]
	public UILabel.Effect effect;

	// Token: 0x04005C78 RID: 23672
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color effectColor = new Color32(0, 0, 0, 80);

	// Token: 0x04005C79 RID: 23673
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vector2 effectDistance;

	// Token: 0x04005C7A RID: 23674
	[PublicizedFrom(EAccessModifier.Protected)]
	public UILabel.Crispness crispness;

	// Token: 0x04005C7B RID: 23675
	[PublicizedFrom(EAccessModifier.Protected)]
	public string text;

	// Token: 0x04005C7C RID: 23676
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color color;

	// Token: 0x04005C7D RID: 23677
	[PublicizedFrom(EAccessModifier.Protected)]
	public int maxLineCount;

	// Token: 0x04005C7E RID: 23678
	[PublicizedFrom(EAccessModifier.Protected)]
	public int spacingX = 1;

	// Token: 0x04005C7F RID: 23679
	[PublicizedFrom(EAccessModifier.Protected)]
	public int spacingY;

	// Token: 0x04005C80 RID: 23680
	[PublicizedFrom(EAccessModifier.Protected)]
	public new NGUIText.Alignment alignment;

	// Token: 0x04005C81 RID: 23681
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool supportBbCode = true;

	// Token: 0x04005C82 RID: 23682
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool upperCase;

	// Token: 0x04005C83 RID: 23683
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool lowerCase;

	// Token: 0x04005C84 RID: 23684
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool parseActions;

	// Token: 0x04005C85 RID: 23685
	[PublicizedFrom(EAccessModifier.Protected)]
	public string actionsDefaultFormat;

	// Token: 0x04005C86 RID: 23686
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool currentTextHasActions;

	// Token: 0x04005C87 RID: 23687
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool supportUrls;

	// Token: 0x04005C88 RID: 23688
	[PublicizedFrom(EAccessModifier.Protected)]
	public HashSet<string> supportedUrlTypes;

	// Token: 0x04005C89 RID: 23689
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bUpdateText;

	// Token: 0x04005C8A RID: 23690
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiUtils.ForceLabelInputStyle forceInputStyle;

	// Token: 0x04005C8B RID: 23691
	[PublicizedFrom(EAccessModifier.Private)]
	public float globalOpacityModifier = 1f;
}
