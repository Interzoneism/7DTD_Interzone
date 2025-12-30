using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

// Token: 0x02000F0C RID: 3852
public class XUiV_TextList : XUiView
{
	// Token: 0x17000CAC RID: 3244
	// (get) Token: 0x06007A7D RID: 31357 RVA: 0x0031AFCA File Offset: 0x003191CA
	public UILabel Label
	{
		get
		{
			return this.label;
		}
	}

	// Token: 0x17000CAD RID: 3245
	// (get) Token: 0x06007A7E RID: 31358 RVA: 0x0031AFD2 File Offset: 0x003191D2
	public UITextList TextList
	{
		get
		{
			return this.textList;
		}
	}

	// Token: 0x17000CAE RID: 3246
	// (get) Token: 0x06007A7F RID: 31359 RVA: 0x0031AFDA File Offset: 0x003191DA
	// (set) Token: 0x06007A80 RID: 31360 RVA: 0x0031AFE2 File Offset: 0x003191E2
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

	// Token: 0x17000CAF RID: 3247
	// (get) Token: 0x06007A81 RID: 31361 RVA: 0x0031AFF2 File Offset: 0x003191F2
	// (set) Token: 0x06007A82 RID: 31362 RVA: 0x0031AFFA File Offset: 0x003191FA
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

	// Token: 0x17000CB0 RID: 3248
	// (get) Token: 0x06007A83 RID: 31363 RVA: 0x0031B00A File Offset: 0x0031920A
	// (set) Token: 0x06007A84 RID: 31364 RVA: 0x0031B012 File Offset: 0x00319212
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

	// Token: 0x17000CB1 RID: 3249
	// (get) Token: 0x06007A85 RID: 31365 RVA: 0x0031B022 File Offset: 0x00319222
	// (set) Token: 0x06007A86 RID: 31366 RVA: 0x0031B02A File Offset: 0x0031922A
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

	// Token: 0x17000CB2 RID: 3250
	// (get) Token: 0x06007A87 RID: 31367 RVA: 0x0031B03A File Offset: 0x0031923A
	// (set) Token: 0x06007A88 RID: 31368 RVA: 0x0031B042 File Offset: 0x00319242
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

	// Token: 0x17000CB3 RID: 3251
	// (get) Token: 0x06007A89 RID: 31369 RVA: 0x0031B052 File Offset: 0x00319252
	// (set) Token: 0x06007A8A RID: 31370 RVA: 0x0031B05A File Offset: 0x0031925A
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

	// Token: 0x17000CB4 RID: 3252
	// (get) Token: 0x06007A8B RID: 31371 RVA: 0x0031B06A File Offset: 0x0031926A
	// (set) Token: 0x06007A8C RID: 31372 RVA: 0x0031B072 File Offset: 0x00319272
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

	// Token: 0x17000CB5 RID: 3253
	// (get) Token: 0x06007A8D RID: 31373 RVA: 0x0031B090 File Offset: 0x00319290
	// (set) Token: 0x06007A8E RID: 31374 RVA: 0x0031B098 File Offset: 0x00319298
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

	// Token: 0x17000CB6 RID: 3254
	// (get) Token: 0x06007A8F RID: 31375 RVA: 0x0031B0B1 File Offset: 0x003192B1
	// (set) Token: 0x06007A90 RID: 31376 RVA: 0x0031B0B9 File Offset: 0x003192B9
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

	// Token: 0x17000CB7 RID: 3255
	// (get) Token: 0x06007A91 RID: 31377 RVA: 0x0031B0D2 File Offset: 0x003192D2
	// (set) Token: 0x06007A92 RID: 31378 RVA: 0x0031B0DA File Offset: 0x003192DA
	public int ParagraphHistory
	{
		get
		{
			return this.paragraphHistory;
		}
		set
		{
			if (value != this.paragraphHistory)
			{
				this.paragraphHistory = value;
				this.isDirty = true;
			}
		}
	}

	// Token: 0x17000CB8 RID: 3256
	// (get) Token: 0x06007A93 RID: 31379 RVA: 0x0031B0F3 File Offset: 0x003192F3
	// (set) Token: 0x06007A94 RID: 31380 RVA: 0x0031B0FB File Offset: 0x003192FB
	public UITextList.Style ListStyle
	{
		get
		{
			return this.listStyle;
		}
		set
		{
			if (value != this.listStyle)
			{
				this.listStyle = value;
				this.isDirty = true;
			}
		}
	}

	// Token: 0x17000CB9 RID: 3257
	// (get) Token: 0x06007A95 RID: 31381 RVA: 0x0031B114 File Offset: 0x00319314
	// (set) Token: 0x06007A96 RID: 31382 RVA: 0x0031B11C File Offset: 0x0031931C
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

	// Token: 0x06007A97 RID: 31383 RVA: 0x0031B12C File Offset: 0x0031932C
	public XUiV_TextList(string _id) : base(_id)
	{
	}

	// Token: 0x06007A98 RID: 31384 RVA: 0x0031B164 File Offset: 0x00319364
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CreateComponents(GameObject _go)
	{
		_go.AddComponent<UILabel>();
		_go.AddComponent<UITextList>();
	}

	// Token: 0x06007A99 RID: 31385 RVA: 0x0031B174 File Offset: 0x00319374
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
		this.EventOnDrag = true;
		this.EventOnScroll = true;
		base.InitView();
		this.label = this.uiTransform.GetComponent<UILabel>();
		this.textList = this.uiTransform.GetComponent<UITextList>();
		if (this.UIFont != null)
		{
			this.UpdateData();
		}
		this.initialized = true;
	}

	// Token: 0x06007A9A RID: 31386 RVA: 0x0031889E File Offset: 0x00316A9E
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (base.xui.GlobalOpacityChanged)
		{
			this.isDirty = true;
		}
	}

	// Token: 0x06007A9B RID: 31387 RVA: 0x0031B1F8 File Offset: 0x003193F8
	public override void UpdateData()
	{
		base.UpdateData();
		if (this.uiFont != null)
		{
			this.label.font = this.uiFont;
		}
		this.label.depth = this.depth;
		this.label.fontSize = this.fontSize;
		this.label.width = this.size.x;
		this.label.height = this.size.y;
		this.label.color = this.color;
		this.label.alignment = this.alignment;
		this.label.supportEncoding = this.supportBbCode;
		this.label.keepCrispWhenShrunk = this.crispness;
		this.label.effectStyle = this.effect;
		this.label.effectColor = this.effectColor;
		this.label.effectDistance = this.effectDistance;
		this.label.spacingX = 1;
		this.textList.paragraphHistory = this.paragraphHistory;
		this.textList.style = this.listStyle;
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

	// Token: 0x06007A9C RID: 31388 RVA: 0x0031B3DC File Offset: 0x003195DC
	public void AddLine(string _line)
	{
		if (!string.IsNullOrEmpty(this.firstLinePrefix))
		{
			_line = this.firstLinePrefix + _line;
		}
		if (this.upperCase)
		{
			_line = _line.ToUpper();
		}
		else if (this.lowerCase)
		{
			_line = _line.ToLower();
		}
		this.textList.Add(_line);
	}

	// Token: 0x06007A9D RID: 31389 RVA: 0x0031B432 File Offset: 0x00319632
	public override void SetDefaults(XUiController _parent)
	{
		base.SetDefaults(_parent);
		this.Alignment = NGUIText.Alignment.Left;
		this.FontSize = 16;
	}

	// Token: 0x06007A9E RID: 31390 RVA: 0x0031B44C File Offset: 0x0031964C
	public override bool ParseAttribute(string attribute, string value, XUiController _parent)
	{
		if (base.ParseAttribute(attribute, value, _parent))
		{
			return true;
		}
		uint num = <PrivateImplementationDetails>.ComputeStringHash(attribute);
		if (num <= 2439222772U)
		{
			if (num <= 1852738900U)
			{
				if (num <= 1021845972U)
				{
					if (num != 815142563U)
					{
						if (num == 1021845972U)
						{
							if (attribute == "upper_case")
							{
								this.upperCase = StringParsers.ParseBool(value, 0, -1, true);
								return true;
							}
						}
					}
					else if (attribute == "lower_case")
					{
						this.lowerCase = StringParsers.ParseBool(value, 0, -1, true);
						return true;
					}
				}
				else if (num != 1031692888U)
				{
					if (num == 1852738900U)
					{
						if (attribute == "effect")
						{
							this.Effect = EnumUtils.Parse<UILabel.Effect>(value, true);
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
			else if (num <= 2396994324U)
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
			else if (num != 2424738503U)
			{
				if (num == 2439222772U)
				{
					if (attribute == "font_face")
					{
						this.UIFont = base.xui.GetUIFontByName(value, false);
						if (this.UIFont == null)
						{
							Log.Warning(string.Concat(new string[]
							{
								"XUi TextList: Font not found: ",
								value,
								", from: ",
								base.Controller.GetParentWindow().ID,
								".",
								base.ID
							}));
						}
						return true;
					}
				}
			}
			else if (attribute == "support_urls")
			{
				if (value.EqualsCaseInsensitive("false"))
				{
					this.supportUrls = false;
				}
				else
				{
					this.supportUrls = true;
					if (value.EqualsCaseInsensitive("true"))
					{
						this.supportedUrlTypes = new HashSet<string>
						{
							"HTTP"
						};
					}
					else
					{
						this.supportedUrlTypes = new HashSet<string>(value.Split(",", StringSplitOptions.None));
					}
				}
				return true;
			}
		}
		else if (num <= 3446912195U)
		{
			if (num <= 2887479055U)
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
			else if (num != 3060355671U)
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
			else if (attribute == "globalopacity")
			{
				if (!StringParsers.ParseBool(value, 0, -1, true))
				{
					this.GlobalOpacityModifier = 0f;
				}
				return true;
			}
		}
		else if (num <= 4072007535U)
		{
			if (num != 3879501881U)
			{
				if (num == 4072007535U)
				{
					if (attribute == "list_style")
					{
						this.listStyle = EnumUtils.Parse<UITextList.Style>(value, true);
						return true;
					}
				}
			}
			else if (attribute == "max_paragraphs")
			{
				this.paragraphHistory = StringParsers.ParseSInt32(value, 0, -1, NumberStyles.Integer);
				return true;
			}
		}
		else if (num != 4144336821U)
		{
			if (num != 4165025527U)
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
			else if (attribute == "prefix_first_line")
			{
				this.firstLinePrefix = value;
				return true;
			}
		}
		else if (attribute == "globalopacitymod")
		{
			this.GlobalOpacityModifier = StringParsers.ParseFloat(value, 0, -1, NumberStyles.Any);
			return true;
		}
		return false;
	}

	// Token: 0x04005CBC RID: 23740
	[PublicizedFrom(EAccessModifier.Protected)]
	public UITextList textList;

	// Token: 0x04005CBD RID: 23741
	[PublicizedFrom(EAccessModifier.Protected)]
	public UILabel label;

	// Token: 0x04005CBE RID: 23742
	[PublicizedFrom(EAccessModifier.Protected)]
	public NGUIFont uiFont;

	// Token: 0x04005CBF RID: 23743
	[PublicizedFrom(EAccessModifier.Protected)]
	public int fontSize;

	// Token: 0x04005CC0 RID: 23744
	[PublicizedFrom(EAccessModifier.Protected)]
	public UILabel.Effect effect;

	// Token: 0x04005CC1 RID: 23745
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color effectColor = new Color32(0, 0, 0, 80);

	// Token: 0x04005CC2 RID: 23746
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vector2 effectDistance;

	// Token: 0x04005CC3 RID: 23747
	[PublicizedFrom(EAccessModifier.Protected)]
	public UILabel.Crispness crispness;

	// Token: 0x04005CC4 RID: 23748
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color color;

	// Token: 0x04005CC5 RID: 23749
	[PublicizedFrom(EAccessModifier.Protected)]
	public UITextList.Style listStyle;

	// Token: 0x04005CC6 RID: 23750
	[PublicizedFrom(EAccessModifier.Protected)]
	public string firstLinePrefix;

	// Token: 0x04005CC7 RID: 23751
	[PublicizedFrom(EAccessModifier.Protected)]
	public int paragraphHistory = 50;

	// Token: 0x04005CC8 RID: 23752
	[PublicizedFrom(EAccessModifier.Protected)]
	public new NGUIText.Alignment alignment;

	// Token: 0x04005CC9 RID: 23753
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool supportBbCode = true;

	// Token: 0x04005CCA RID: 23754
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool supportUrls;

	// Token: 0x04005CCB RID: 23755
	[PublicizedFrom(EAccessModifier.Protected)]
	public HashSet<string> supportedUrlTypes;

	// Token: 0x04005CCC RID: 23756
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool upperCase;

	// Token: 0x04005CCD RID: 23757
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool lowerCase;

	// Token: 0x04005CCE RID: 23758
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bUpdateText;

	// Token: 0x04005CCF RID: 23759
	[PublicizedFrom(EAccessModifier.Private)]
	public float globalOpacityModifier = 1f;
}
