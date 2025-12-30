using System;
using System.Globalization;
using UnityEngine;

// Token: 0x02000F08 RID: 3848
public class XUiV_Panel : XUiView
{
	// Token: 0x17000C8D RID: 3213
	// (get) Token: 0x06007A20 RID: 31264 RVA: 0x0031924C File Offset: 0x0031744C
	// (set) Token: 0x06007A21 RID: 31265 RVA: 0x00319254 File Offset: 0x00317454
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

	// Token: 0x17000C8E RID: 3214
	// (get) Token: 0x06007A22 RID: 31266 RVA: 0x0031926D File Offset: 0x0031746D
	// (set) Token: 0x06007A23 RID: 31267 RVA: 0x00319275 File Offset: 0x00317475
	public Color BackgroundColor
	{
		get
		{
			return this.backgroundColor;
		}
		set
		{
			this.backgroundColor = value;
			this.isDirty = true;
			this.useBackground = true;
		}
	}

	// Token: 0x17000C8F RID: 3215
	// (get) Token: 0x06007A24 RID: 31268 RVA: 0x0031928C File Offset: 0x0031748C
	// (set) Token: 0x06007A25 RID: 31269 RVA: 0x00319294 File Offset: 0x00317494
	public string BackgroundSpriteName
	{
		get
		{
			return this.backgroundSpriteName;
		}
		set
		{
			this.backgroundSpriteName = value;
			if (value == "")
			{
				this.backgroundSpriteName = XUi.BlankTexture;
			}
		}
	}

	// Token: 0x17000C90 RID: 3216
	// (get) Token: 0x06007A26 RID: 31270 RVA: 0x003192B5 File Offset: 0x003174B5
	// (set) Token: 0x06007A27 RID: 31271 RVA: 0x003192BD File Offset: 0x003174BD
	public string BorderSpriteName
	{
		get
		{
			return this.borderSpriteName;
		}
		set
		{
			this.borderSpriteName = value;
			if (value == "")
			{
				this.borderSpriteName = XUi.BlankTexture;
			}
		}
	}

	// Token: 0x17000C91 RID: 3217
	// (get) Token: 0x06007A28 RID: 31272 RVA: 0x003192DE File Offset: 0x003174DE
	// (set) Token: 0x06007A29 RID: 31273 RVA: 0x003192E6 File Offset: 0x003174E6
	public Color BorderColor
	{
		get
		{
			return this.borderColor;
		}
		set
		{
			this.borderColor = value;
			this.isDirty = true;
			this.useBackground = true;
		}
	}

	// Token: 0x17000C92 RID: 3218
	// (get) Token: 0x06007A2A RID: 31274 RVA: 0x003192FD File Offset: 0x003174FD
	// (set) Token: 0x06007A2B RID: 31275 RVA: 0x00319305 File Offset: 0x00317505
	public XUi_Thickness BorderThickness
	{
		get
		{
			return this.borderThickness;
		}
		set
		{
			this.borderThickness = value;
			this.isDirty = true;
			this.useBackground = true;
		}
	}

	// Token: 0x17000C93 RID: 3219
	// (get) Token: 0x06007A2C RID: 31276 RVA: 0x0031931C File Offset: 0x0031751C
	// (set) Token: 0x06007A2D RID: 31277 RVA: 0x00319324 File Offset: 0x00317524
	public UIDrawCall.Clipping Clipping
	{
		get
		{
			return this.clipping;
		}
		set
		{
			if (value != this.clipping)
			{
				this.clipping = value;
				this.isDirty = true;
			}
		}
	}

	// Token: 0x17000C94 RID: 3220
	// (get) Token: 0x06007A2E RID: 31278 RVA: 0x0031933D File Offset: 0x0031753D
	// (set) Token: 0x06007A2F RID: 31279 RVA: 0x00319345 File Offset: 0x00317545
	public Vector2 ClippingSize
	{
		get
		{
			return this.clippingSize;
		}
		set
		{
			if (value != this.clippingSize)
			{
				this.clippingSize = value;
				this.isDirty = true;
			}
		}
	}

	// Token: 0x17000C95 RID: 3221
	// (get) Token: 0x06007A30 RID: 31280 RVA: 0x00319363 File Offset: 0x00317563
	// (set) Token: 0x06007A31 RID: 31281 RVA: 0x0031936B File Offset: 0x0031756B
	public Vector2 ClippingCenter
	{
		get
		{
			return this.clippingCenter;
		}
		set
		{
			if (value != this.clippingCenter)
			{
				this.clippingCenter = value;
				this.isDirty = true;
			}
		}
	}

	// Token: 0x17000C96 RID: 3222
	// (get) Token: 0x06007A32 RID: 31282 RVA: 0x00319389 File Offset: 0x00317589
	// (set) Token: 0x06007A33 RID: 31283 RVA: 0x00319391 File Offset: 0x00317591
	public Vector2 ClippingSoftness
	{
		get
		{
			return this.clippingSoftness;
		}
		set
		{
			if (value != this.clippingSoftness)
			{
				this.clippingSoftness = value;
				this.isDirty = true;
			}
		}
	}

	// Token: 0x17000C97 RID: 3223
	// (get) Token: 0x06007A34 RID: 31284 RVA: 0x003144B8 File Offset: 0x003126B8
	// (set) Token: 0x06007A35 RID: 31285 RVA: 0x003193AF File Offset: 0x003175AF
	public override bool Enabled
	{
		get
		{
			return this.enabled;
		}
		set
		{
			this.enabled = value;
			this.RefreshEnabled();
		}
	}

	// Token: 0x06007A36 RID: 31286 RVA: 0x003193C0 File Offset: 0x003175C0
	[PublicizedFrom(EAccessModifier.Private)]
	public void RefreshEnabled()
	{
		if (this.panel != null)
		{
			this.panel.gameObject.SetActive(this.enabled);
			this.panel.enabled = this.enabled;
			this.borderSprite.Enabled = this.enabled;
			this.backgroundSprite.Enabled = this.enabled;
		}
	}

	// Token: 0x06007A37 RID: 31287 RVA: 0x00319424 File Offset: 0x00317624
	public XUiV_Panel(string _id) : base(_id)
	{
	}

	// Token: 0x06007A38 RID: 31288 RVA: 0x003194A0 File Offset: 0x003176A0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CreateComponents(GameObject _go)
	{
		_go.AddComponent<UIPanel>();
	}

	// Token: 0x06007A39 RID: 31289 RVA: 0x003194AC File Offset: 0x003176AC
	public override void InitView()
	{
		this.borderSprite = new XUiV_Sprite("_border");
		this.borderSprite.xui = base.xui;
		this.borderSprite.Controller = new XUiController(base.Controller);
		this.borderSprite.Controller.xui = base.xui;
		this.borderSprite.IgnoreParentPadding = true;
		this.backgroundSprite = new XUiV_Sprite("_background");
		this.backgroundSprite.xui = base.xui;
		this.backgroundSprite.Controller = new XUiController(base.Controller);
		this.backgroundSprite.Controller.xui = base.xui;
		this.backgroundSprite.IgnoreParentPadding = true;
		if (!this.disableAutoBackground && this.useBackground)
		{
			this.borderSprite.SetDefaults(base.Controller);
			this.borderSprite.GlobalOpacityModifier = 0f;
			this.borderSprite.Position = new Vector2i(this.borderInset.Left, -this.borderInset.Top);
			this.borderSprite.Size = new Vector2i(base.Size.x - this.borderInset.SumLeftRight, base.Size.y - this.borderInset.SumTopBottom);
			this.borderSprite.UIAtlas = "UIAtlas";
			this.borderSprite.SpriteName = this.borderSpriteName;
			this.borderSprite.Color = this.borderColor;
			this.borderSprite.Depth = this.depth + 1;
			this.borderSprite.Pivot = UIWidget.Pivot.TopLeft;
			this.borderSprite.Type = UIBasicSprite.Type.Sliced;
			this.borderSprite.Controller.WindowGroup = base.Controller.WindowGroup;
			this.backgroundSprite.SetDefaults(base.Controller);
			this.backgroundSprite.GlobalOpacityModifier = 2f;
			this.backgroundSprite.Position = new Vector2i(this.borderThickness.left, -this.borderThickness.top);
			this.backgroundSprite.Size = new Vector2i(base.Size.x - (this.borderThickness.left + this.borderThickness.right), base.Size.y - (this.borderThickness.top + this.borderThickness.bottom));
			this.backgroundSprite.UIAtlas = "UIAtlas";
			this.backgroundSprite.SpriteName = this.backgroundSpriteName;
			this.backgroundSprite.Color = this.backgroundColor;
			this.backgroundSprite.Depth = this.depth;
			this.backgroundSprite.Pivot = UIWidget.Pivot.TopLeft;
			this.backgroundSprite.Type = UIBasicSprite.Type.Sliced;
			this.backgroundSprite.Controller.WindowGroup = base.Controller.WindowGroup;
		}
		base.InitView();
		this.panel = this.uiTransform.gameObject.GetComponent<UIPanel>();
		if (!this.createUiPanel && this.clipping == UIDrawCall.Clipping.None)
		{
			UnityEngine.Object.Destroy(this.panel);
			this.panel = null;
		}
		else
		{
			this.panel.enabled = true;
			this.panel.depth = this.depth;
			if (this.clipping != UIDrawCall.Clipping.None)
			{
				if (this.clippingCenter == new Vector2(-10000f, -10000f))
				{
					this.clippingCenter = new Vector2((float)(this.size.x / 2), (float)(-(float)this.size.y / 2));
				}
				if (this.clippingSize == new Vector2(-10000f, -10000f))
				{
					this.clippingSize = new Vector2((float)this.size.x, (float)this.size.y);
				}
				this.updateClipping();
			}
		}
		BoxCollider collider = this.collider;
		if (collider != null)
		{
			float x = (float)this.size.x * 0.5f;
			float num = (float)this.size.y * 0.5f;
			collider.center = new Vector3(x, -num, 0f);
			collider.size = new Vector3((float)this.size.x * this.colliderScale, (float)this.size.y * this.colliderScale, 0f);
		}
		if (!this.disableAutoBackground && this.useBackground && this.backgroundSprite != null && this.borderSprite != null)
		{
			this.backgroundSprite.Color = this.backgroundColor;
			this.borderSprite.Color = this.borderColor;
		}
		this.RefreshEnabled();
		this.isDirty = true;
	}

	// Token: 0x06007A3A RID: 31290 RVA: 0x00319958 File Offset: 0x00317B58
	public override void UpdateData()
	{
		if (this.isDirty)
		{
			if (!this.disableAutoBackground && this.useBackground && this.backgroundSprite != null)
			{
				this.borderSprite.FillCenter = false;
				this.borderSprite.Position = new Vector2i(this.borderInset.Left, -this.borderInset.Top);
				this.borderSprite.Size = new Vector2i(base.Size.x - this.borderInset.SumLeftRight, base.Size.y - this.borderInset.SumTopBottom);
				this.borderSprite.Color = this.borderColor;
				this.borderSprite.SpriteName = this.borderSpriteName;
				this.borderSprite.GlobalOpacityModifier = this.globalOpacityModifier;
				this.backgroundSprite.Size = new Vector2i(base.Size.x - (this.borderThickness.left + this.borderThickness.right), base.Size.y - (this.borderThickness.top + this.borderThickness.bottom));
				this.backgroundSprite.Position = new Vector2i(this.borderThickness.left, -this.borderThickness.top);
				this.backgroundSprite.Color = this.backgroundColor;
				this.backgroundSprite.SpriteName = this.backgroundSpriteName;
				this.backgroundSprite.GlobalOpacityModifier = this.globalOpacityModifier;
			}
			if (this.panel != null)
			{
				this.panel.depth = this.depth;
			}
			this.updateClipping();
		}
		base.UpdateData();
	}

	// Token: 0x06007A3B RID: 31291 RVA: 0x00319B10 File Offset: 0x00317D10
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateClipping()
	{
		if (this.clipping != UIDrawCall.Clipping.None)
		{
			if (this.panel.clipping != this.clipping)
			{
				this.panel.clipping = this.clipping;
			}
			if (this.panel.clipSoftness != this.clippingSoftness)
			{
				this.panel.clipSoftness = this.clippingSoftness;
			}
			if (this.clippingSize.x < 0f)
			{
				this.clippingSize.x = 0f;
			}
			if (this.clippingSize.y < 0f)
			{
				this.clippingSize.y = 0f;
			}
			Vector4 vector = new Vector4(this.clippingCenter.x, this.clippingCenter.y, this.clippingSize.x, this.clippingSize.y);
			if (this.panel.baseClipRegion != vector)
			{
				this.panel.baseClipRegion = vector;
			}
		}
	}

	// Token: 0x06007A3C RID: 31292 RVA: 0x00319C0C File Offset: 0x00317E0C
	public override void SetDefaults(XUiController _parent)
	{
		base.SetDefaults(_parent);
		this.backgroundColor = new Color32(96, 96, 96, byte.MaxValue);
		this.borderColor = new Color32(0, 0, 0, 0);
		this.borderThickness = new XUi_Thickness(3);
	}

	// Token: 0x06007A3D RID: 31293 RVA: 0x00319C5C File Offset: 0x00317E5C
	public override bool ParseAttribute(string attribute, string value, XUiController _parent)
	{
		bool flag = base.ParseAttribute(attribute, value, _parent);
		if (!flag)
		{
			flag = true;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(attribute);
			if (num <= 1650154374U)
			{
				if (num <= 727013168U)
				{
					if (num != 256789003U)
					{
						if (num != 625300902U)
						{
							if (num == 727013168U)
							{
								if (attribute == "backgroundcolor")
								{
									this.BackgroundColor = StringParsers.ParseColor32(value);
									return flag;
								}
							}
						}
						else if (attribute == "clippingsize")
						{
							this.clippingSize = StringParsers.ParseVector2(value);
							return flag;
						}
					}
					else if (attribute == "createuipanel")
					{
						this.createUiPanel = StringParsers.ParseBool(value, 0, -1, true);
						return flag;
					}
				}
				else if (num <= 1092012428U)
				{
					if (num != 954579279U)
					{
						if (num == 1092012428U)
						{
							if (attribute == "disableautobackground")
							{
								this.disableAutoBackground = StringParsers.ParseBool(value, 0, -1, true);
								return flag;
							}
						}
					}
					else if (attribute == "clipping")
					{
						this.clipping = EnumUtils.Parse<UIDrawCall.Clipping>(value, true);
						return flag;
					}
				}
				else if (num != 1187297821U)
				{
					if (num == 1650154374U)
					{
						if (attribute == "bordercolor")
						{
							this.BorderColor = StringParsers.ParseColor32(value);
							return flag;
						}
					}
				}
				else if (attribute == "borderspritename")
				{
					this.BorderSpriteName = value;
					return flag;
				}
			}
			else if (num <= 2728752196U)
			{
				if (num <= 1694002274U)
				{
					if (num != 1657231807U)
					{
						if (num == 1694002274U)
						{
							if (attribute == "clippingcenter")
							{
								this.clippingCenter = StringParsers.ParseVector2(value);
								return flag;
							}
						}
					}
					else if (attribute == "backgroundspritename")
					{
						this.BackgroundSpriteName = value;
						return flag;
					}
				}
				else if (num != 2511885439U)
				{
					if (num == 2728752196U)
					{
						if (attribute == "clippingsoftness")
						{
							this.clippingSoftness = StringParsers.ParseVector2(value);
							return flag;
						}
					}
				}
				else if (attribute == "borderthickness")
				{
					this.BorderThickness = XUi_Thickness.Parse(value);
					return flag;
				}
			}
			else if (num <= 3060355671U)
			{
				if (num != 3018589076U)
				{
					if (num == 3060355671U)
					{
						if (attribute == "globalopacity")
						{
							if (!StringParsers.ParseBool(value, 0, -1, true))
							{
								this.GlobalOpacityModifier = 0f;
								return flag;
							}
							return flag;
						}
					}
				}
				else if (attribute == "borderinset")
				{
					XUiSideSizes.TryParse(value, out this.borderInset, attribute);
					return flag;
				}
			}
			else if (num != 3619465623U)
			{
				if (num == 4144336821U)
				{
					if (attribute == "globalopacitymod")
					{
						this.GlobalOpacityModifier = StringParsers.ParseFloat(value, 0, -1, NumberStyles.Any);
						return flag;
					}
				}
			}
			else if (attribute == "snapcursor")
			{
				this.EventOnHover = true;
				return flag;
			}
			flag = false;
		}
		return flag;
	}

	// Token: 0x04005C8C RID: 23692
	[PublicizedFrom(EAccessModifier.Protected)]
	public string backgroundSpriteName = XUi.BlankTexture;

	// Token: 0x04005C8D RID: 23693
	[PublicizedFrom(EAccessModifier.Protected)]
	public string borderSpriteName = XUi.BlankTexture;

	// Token: 0x04005C8E RID: 23694
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color borderColor = Color.white;

	// Token: 0x04005C8F RID: 23695
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUi_Thickness borderThickness;

	// Token: 0x04005C90 RID: 23696
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiSideSizes borderInset;

	// Token: 0x04005C91 RID: 23697
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color backgroundColor = Color.white;

	// Token: 0x04005C92 RID: 23698
	[PublicizedFrom(EAccessModifier.Private)]
	public bool useBackground = true;

	// Token: 0x04005C93 RID: 23699
	public bool createUiPanel;

	// Token: 0x04005C94 RID: 23700
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite borderSprite;

	// Token: 0x04005C95 RID: 23701
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite backgroundSprite;

	// Token: 0x04005C96 RID: 23702
	[PublicizedFrom(EAccessModifier.Protected)]
	public UIPanel panel;

	// Token: 0x04005C97 RID: 23703
	[PublicizedFrom(EAccessModifier.Protected)]
	public UIDrawCall.Clipping clipping;

	// Token: 0x04005C98 RID: 23704
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vector2 clippingSize = new Vector2(-10000f, -10000f);

	// Token: 0x04005C99 RID: 23705
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vector2 clippingCenter = new Vector2(-10000f, -10000f);

	// Token: 0x04005C9A RID: 23706
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vector2 clippingSoftness;

	// Token: 0x04005C9B RID: 23707
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool disableAutoBackground;

	// Token: 0x04005C9C RID: 23708
	[PublicizedFrom(EAccessModifier.Protected)]
	public float globalOpacityModifier = 1f;
}
