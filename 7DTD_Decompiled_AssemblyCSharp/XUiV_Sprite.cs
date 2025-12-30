using System;
using System.Globalization;
using UnityEngine;

// Token: 0x02000F0A RID: 3850
public class XUiV_Sprite : XUiView
{
	// Token: 0x17000C99 RID: 3225
	// (get) Token: 0x06007A47 RID: 31303 RVA: 0x0031A1EA File Offset: 0x003183EA
	public UISprite Sprite
	{
		get
		{
			return this.sprite;
		}
	}

	// Token: 0x17000C9A RID: 3226
	// (get) Token: 0x06007A48 RID: 31304 RVA: 0x0031A1F2 File Offset: 0x003183F2
	// (set) Token: 0x06007A49 RID: 31305 RVA: 0x0031A1FA File Offset: 0x003183FA
	public string UIAtlas
	{
		get
		{
			return this.uiAtlas;
		}
		set
		{
			if (this.uiAtlas != value)
			{
				this.uiAtlas = value;
				this.uiAtlasChanged = true;
				this.isDirty = true;
			}
		}
	}

	// Token: 0x17000C9B RID: 3227
	// (get) Token: 0x06007A4A RID: 31306 RVA: 0x0031A21F File Offset: 0x0031841F
	// (set) Token: 0x06007A4B RID: 31307 RVA: 0x0031A227 File Offset: 0x00318427
	public string SpriteName
	{
		get
		{
			return this.spriteName;
		}
		set
		{
			if (this.spriteName != value)
			{
				this.spriteName = value;
				this.isDirty = true;
			}
		}
	}

	// Token: 0x17000C9C RID: 3228
	// (get) Token: 0x06007A4C RID: 31308 RVA: 0x0031A245 File Offset: 0x00318445
	// (set) Token: 0x06007A4D RID: 31309 RVA: 0x0031A250 File Offset: 0x00318450
	public Color Color
	{
		get
		{
			return this.color;
		}
		set
		{
			if (this.color.r != value.r || this.color.g != value.g || this.color.b != value.b || this.color.a != value.a)
			{
				this.color = value;
				this.isDirty = true;
			}
		}
	}

	// Token: 0x17000C9D RID: 3229
	// (get) Token: 0x06007A4E RID: 31310 RVA: 0x00317808 File Offset: 0x00315A08
	// (set) Token: 0x06007A4F RID: 31311 RVA: 0x0031A2B7 File Offset: 0x003184B7
	public virtual UIBasicSprite.Type Type
	{
		get
		{
			return this.type;
		}
		set
		{
			if (this.type != value)
			{
				this.type = value;
				this.isDirty = true;
			}
		}
	}

	// Token: 0x17000C9E RID: 3230
	// (get) Token: 0x06007A50 RID: 31312 RVA: 0x0031A2D0 File Offset: 0x003184D0
	// (set) Token: 0x06007A51 RID: 31313 RVA: 0x0031A2D8 File Offset: 0x003184D8
	public UIBasicSprite.FillDirection FillDirection
	{
		get
		{
			return this.fillDirection;
		}
		set
		{
			if (this.fillDirection != value)
			{
				this.fillDirection = value;
				this.isDirty = true;
			}
		}
	}

	// Token: 0x17000C9F RID: 3231
	// (get) Token: 0x06007A52 RID: 31314 RVA: 0x0031A2F1 File Offset: 0x003184F1
	// (set) Token: 0x06007A53 RID: 31315 RVA: 0x0031A2F9 File Offset: 0x003184F9
	public bool FillInvert
	{
		get
		{
			return this.fillInvert;
		}
		set
		{
			if (this.fillInvert != value)
			{
				this.fillInvert = value;
				this.isDirty = true;
			}
		}
	}

	// Token: 0x17000CA0 RID: 3232
	// (get) Token: 0x06007A54 RID: 31316 RVA: 0x0031A312 File Offset: 0x00318512
	// (set) Token: 0x06007A55 RID: 31317 RVA: 0x0031A31A File Offset: 0x0031851A
	public bool FillCenter
	{
		get
		{
			return this.fillCenter;
		}
		set
		{
			if (this.fillCenter != value)
			{
				this.fillCenter = value;
				this.isDirty = true;
			}
		}
	}

	// Token: 0x17000CA1 RID: 3233
	// (get) Token: 0x06007A56 RID: 31318 RVA: 0x0031A333 File Offset: 0x00318533
	// (set) Token: 0x06007A57 RID: 31319 RVA: 0x0031A33B File Offset: 0x0031853B
	public float Fill
	{
		get
		{
			return this.fillAmount;
		}
		set
		{
			if (this.fillAmount != value && (double)Math.Abs((value - this.fillAmount) / value) > 0.005)
			{
				this.fillAmount = Mathf.Clamp01(value);
				this.isDirty = true;
			}
		}
	}

	// Token: 0x17000CA2 RID: 3234
	// (get) Token: 0x06007A58 RID: 31320 RVA: 0x0031A374 File Offset: 0x00318574
	// (set) Token: 0x06007A59 RID: 31321 RVA: 0x0031A381 File Offset: 0x00318581
	public UIBasicSprite.Flip Flip
	{
		get
		{
			return this.sprite.flip;
		}
		set
		{
			if (this.flip != value)
			{
				this.flip = value;
				this.isDirty = true;
			}
		}
	}

	// Token: 0x17000CA3 RID: 3235
	// (get) Token: 0x06007A5A RID: 31322 RVA: 0x0031A39A File Offset: 0x0031859A
	// (set) Token: 0x06007A5B RID: 31323 RVA: 0x0031A3A2 File Offset: 0x003185A2
	public float GlobalOpacityModifier
	{
		get
		{
			return this.globalOpacityModifier;
		}
		set
		{
			if (this.globalOpacityModifier != value)
			{
				this.globalOpacityModifier = value;
				this.isDirty = true;
			}
		}
	}

	// Token: 0x17000CA4 RID: 3236
	// (get) Token: 0x06007A5C RID: 31324 RVA: 0x0031A3BB File Offset: 0x003185BB
	// (set) Token: 0x06007A5D RID: 31325 RVA: 0x0031A3C3 File Offset: 0x003185C3
	public bool ForegroundLayer
	{
		get
		{
			return this.foregroundLayer;
		}
		set
		{
			if (this.foregroundLayer != value)
			{
				this.foregroundLayer = value;
				this.isDirty = true;
			}
		}
	}

	// Token: 0x06007A5E RID: 31326 RVA: 0x0031A3DC File Offset: 0x003185DC
	public XUiV_Sprite(string _id) : base(_id)
	{
	}

	// Token: 0x06007A5F RID: 31327 RVA: 0x0031A439 File Offset: 0x00318639
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CreateComponents(GameObject _go)
	{
		_go.AddComponent<UISprite>();
	}

	// Token: 0x06007A60 RID: 31328 RVA: 0x0031A442 File Offset: 0x00318642
	public override void InitView()
	{
		base.InitView();
		this.sprite = this.uiTransform.GetComponent<UISprite>();
		this.UpdateData();
	}

	// Token: 0x06007A61 RID: 31329 RVA: 0x0031A464 File Offset: 0x00318664
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (base.xui.GlobalOpacityChanged)
		{
			this.isDirty = true;
		}
		UISprite uisprite = this.sprite;
		if (uisprite != null)
		{
			bool isVisible = uisprite.isVisible;
			if (this.lastVisible != isVisible)
			{
				this.isDirty = true;
				this.lastVisible = isVisible;
			}
		}
	}

	// Token: 0x06007A62 RID: 31330 RVA: 0x0031A4B4 File Offset: 0x003186B4
	public override void UpdateData()
	{
		bool initialized = this.initialized;
		this.applyAtlasAndSprite(false);
		this.sprite.keepAspectRatio = this.keepAspectRatio;
		this.sprite.aspectRatio = this.aspectRatio;
		if (this.sprite.color != this.color)
		{
			this.sprite.color = this.color;
		}
		if (this.gradientStart != null)
		{
			this.sprite.gradientTop = this.gradientStart.Value;
			this.sprite.applyGradient = true;
		}
		if (this.gradientEnd != null)
		{
			this.sprite.gradientBottom = this.gradientEnd.Value;
			this.sprite.applyGradient = true;
		}
		if (this.globalOpacityModifier != 0f && (this.foregroundLayer ? (base.xui.ForegroundGlobalOpacity < 1f) : (base.xui.BackgroundGlobalOpacity < 1f)))
		{
			float a = Mathf.Clamp01(this.color.a * (this.globalOpacityModifier * (this.foregroundLayer ? base.xui.ForegroundGlobalOpacity : base.xui.BackgroundGlobalOpacity)));
			this.sprite.color = new Color(this.color.r, this.color.g, this.color.b, a);
		}
		if (this.borderSize > 0 && this.sprite.border.x != (float)this.borderSize)
		{
			this.sprite.border = new Vector4((float)this.borderSize, (float)this.borderSize, (float)this.borderSize, (float)this.borderSize);
		}
		if (this.sprite.centerType != (this.fillCenter ? UIBasicSprite.AdvancedType.Sliced : UIBasicSprite.AdvancedType.Invisible))
		{
			this.sprite.centerType = (this.fillCenter ? UIBasicSprite.AdvancedType.Sliced : UIBasicSprite.AdvancedType.Invisible);
		}
		base.parseAnchors(this.sprite, true);
		if (this.sprite.fillDirection != this.fillDirection)
		{
			this.sprite.fillDirection = this.fillDirection;
		}
		if (this.sprite.invert != this.fillInvert)
		{
			this.sprite.invert = this.fillInvert;
		}
		if (this.sprite.fillAmount != this.fillAmount)
		{
			this.sprite.fillAmount = this.fillAmount;
		}
		if (this.sprite.type != this.type)
		{
			this.sprite.type = this.type;
		}
		if (this.sprite.flip != this.flip)
		{
			this.sprite.flip = this.flip;
		}
		if (!this.initialized)
		{
			this.initialized = true;
			this.sprite.pivot = this.pivot;
			this.sprite.depth = this.depth;
			this.uiTransform.localScale = Vector3.one;
			this.uiTransform.localPosition = new Vector3((float)base.PaddedPosition.x, (float)base.PaddedPosition.y, 0f);
			if (this.EventOnHover || this.EventOnPress)
			{
				BoxCollider collider = this.collider;
				collider.center = this.sprite.localCenter;
				collider.size = new Vector3(this.sprite.localSize.x * this.colliderScale, this.sprite.localSize.y * this.colliderScale, 0f);
			}
		}
		if (this.sprite.isAnchored)
		{
			this.sprite.autoResizeBoxCollider = true;
		}
		else
		{
			this.RefreshBoxCollider();
		}
		base.UpdateData();
	}

	// Token: 0x06007A63 RID: 31331 RVA: 0x0031A85F File Offset: 0x00318A5F
	public override void SetDefaults(XUiController _parent)
	{
		base.SetDefaults(_parent);
		this.FillCenter = true;
		this.Type = UIBasicSprite.Type.Simple;
		this.FillDirection = UIBasicSprite.FillDirection.Horizontal;
	}

	// Token: 0x06007A64 RID: 31332 RVA: 0x0031A880 File Offset: 0x00318A80
	public override bool ParseAttribute(string attribute, string value, XUiController _parent)
	{
		bool flag = base.ParseAttribute(attribute, value, _parent);
		if (!flag)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(attribute);
			if (num <= 3060355671U)
			{
				if (num <= 1645533310U)
				{
					if (num <= 1031692888U)
					{
						if (num != 335748235U)
						{
							if (num == 1031692888U)
							{
								if (attribute == "color")
								{
									this.Color = StringParsers.ParseColor32(value);
									return true;
								}
							}
						}
						else if (attribute == "gradient_end")
						{
							this.gradientEnd = new Color?(StringParsers.ParseColor32(value));
							return true;
						}
					}
					else if (num != 1361572173U)
					{
						if (num == 1645533310U)
						{
							if (attribute == "gradient_start")
							{
								this.gradientStart = new Color?(StringParsers.ParseColor32(value));
								return true;
							}
						}
					}
					else if (attribute == "type")
					{
						this.Type = EnumUtils.Parse<UIBasicSprite.Type>(value, true);
						return true;
					}
				}
				else if (num <= 2179094556U)
				{
					if (num != 1993028337U)
					{
						if (num == 2179094556U)
						{
							if (attribute == "sprite")
							{
								this.SpriteName = value;
								return true;
							}
						}
					}
					else if (attribute == "fillcenter")
					{
						this.FillCenter = StringParsers.ParseBool(value, 0, -1, true);
						return true;
					}
				}
				else if (num != 2984927816U)
				{
					if (num == 3060355671U)
					{
						if (attribute == "globalopacity")
						{
							if (!StringParsers.ParseBool(value, 0, -1, true))
							{
								this.GlobalOpacityModifier = 0f;
								return true;
							}
							return true;
						}
					}
				}
				else if (attribute == "fill")
				{
					this.Fill = StringParsers.ParseFloat(value, 0, -1, NumberStyles.Any);
					return true;
				}
			}
			else if (num <= 3407328204U)
			{
				if (num <= 3360407129U)
				{
					if (num != 3134659686U)
					{
						if (num == 3360407129U)
						{
							if (attribute == "filldirection")
							{
								this.FillDirection = EnumUtils.Parse<UIBasicSprite.FillDirection>(value, true);
								return true;
							}
						}
					}
					else if (attribute == "flip")
					{
						this.Flip = EnumUtils.Parse<UIBasicSprite.Flip>(value, true);
						return true;
					}
				}
				else if (num != 3383065148U)
				{
					if (num == 3407328204U)
					{
						if (attribute == "atlas")
						{
							this.UIAtlas = value;
							return true;
						}
					}
				}
				else if (attribute == "fillinvert")
				{
					this.FillInvert = StringParsers.ParseBool(value, 0, -1, true);
					return true;
				}
			}
			else if (num <= 3458780677U)
			{
				if (num != 3435511540U)
				{
					if (num == 3458780677U)
					{
						if (attribute == "foregroundlayer")
						{
							this.foregroundLayer = StringParsers.ParseBool(value, 0, -1, true);
							return true;
						}
					}
				}
				else if (attribute == "sprite_ps4")
				{
					this.spriteNamePS4 = value;
					return true;
				}
			}
			else if (num != 3465140046U)
			{
				if (num != 3607701014U)
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
				else if (attribute == "sprite_xb1")
				{
					this.spriteNameXB1 = value;
					return true;
				}
			}
			else if (attribute == "bordersize")
			{
				this.borderSize = int.Parse(value);
				return true;
			}
			return false;
		}
		return flag;
	}

	// Token: 0x06007A65 RID: 31333 RVA: 0x0031AC49 File Offset: 0x00318E49
	public void SetSpriteImmediately(string spriteName)
	{
		this.spriteName = spriteName;
		this.applyAtlasAndSprite(true);
	}

	// Token: 0x06007A66 RID: 31334 RVA: 0x0031AC5A File Offset: 0x00318E5A
	public void SetColorImmediately(Color color)
	{
		if (this.sprite != null)
		{
			this.sprite.color = color;
		}
	}

	// Token: 0x06007A67 RID: 31335 RVA: 0x0031AC78 File Offset: 0x00318E78
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool applyAtlasAndSprite(bool _force = false)
	{
		if (this.sprite == null)
		{
			return false;
		}
		if (!_force && this.sprite.spriteName != null && this.sprite.spriteName == this.spriteName && this.sprite.atlas != null && !this.uiAtlasChanged)
		{
			return false;
		}
		this.uiAtlasChanged = false;
		this.sprite.atlas = base.xui.GetAtlasByName(this.UIAtlas, this.spriteName);
		this.sprite.spriteName = this.spriteName;
		return true;
	}

	// Token: 0x04005CA0 RID: 23712
	[PublicizedFrom(EAccessModifier.Private)]
	public string uiAtlas = string.Empty;

	// Token: 0x04005CA1 RID: 23713
	[PublicizedFrom(EAccessModifier.Private)]
	public bool uiAtlasChanged;

	// Token: 0x04005CA2 RID: 23714
	[PublicizedFrom(EAccessModifier.Protected)]
	public string spriteName = string.Empty;

	// Token: 0x04005CA3 RID: 23715
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color color = Color.white;

	// Token: 0x04005CA4 RID: 23716
	[PublicizedFrom(EAccessModifier.Private)]
	public Color? gradientStart;

	// Token: 0x04005CA5 RID: 23717
	[PublicizedFrom(EAccessModifier.Private)]
	public Color? gradientEnd;

	// Token: 0x04005CA6 RID: 23718
	[PublicizedFrom(EAccessModifier.Protected)]
	public UIBasicSprite.Type type;

	// Token: 0x04005CA7 RID: 23719
	[PublicizedFrom(EAccessModifier.Protected)]
	public UIBasicSprite.FillDirection fillDirection;

	// Token: 0x04005CA8 RID: 23720
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool fillInvert;

	// Token: 0x04005CA9 RID: 23721
	[PublicizedFrom(EAccessModifier.Protected)]
	public UIBasicSprite.Flip flip;

	// Token: 0x04005CAA RID: 23722
	[PublicizedFrom(EAccessModifier.Protected)]
	public UISprite sprite;

	// Token: 0x04005CAB RID: 23723
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool fillCenter;

	// Token: 0x04005CAC RID: 23724
	[PublicizedFrom(EAccessModifier.Protected)]
	public float fillAmount;

	// Token: 0x04005CAD RID: 23725
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool foregroundLayer;

	// Token: 0x04005CAE RID: 23726
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool lastVisible;

	// Token: 0x04005CAF RID: 23727
	[PublicizedFrom(EAccessModifier.Protected)]
	public string spriteNameXB1 = string.Empty;

	// Token: 0x04005CB0 RID: 23728
	[PublicizedFrom(EAccessModifier.Protected)]
	public string spriteNamePS4 = string.Empty;

	// Token: 0x04005CB1 RID: 23729
	[PublicizedFrom(EAccessModifier.Protected)]
	public float globalOpacityModifier = 1f;

	// Token: 0x04005CB2 RID: 23730
	[PublicizedFrom(EAccessModifier.Private)]
	public int borderSize = -1;
}
