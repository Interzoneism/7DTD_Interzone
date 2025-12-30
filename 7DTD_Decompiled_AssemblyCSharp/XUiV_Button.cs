using System;
using System.Globalization;
using UnityEngine;

// Token: 0x02000F03 RID: 3843
public class XUiV_Button : XUiView
{
	// Token: 0x06007994 RID: 31124 RVA: 0x00316BB0 File Offset: 0x00314DB0
	public XUiV_Button(string _id) : base(_id)
	{
		this.UseSelectionBox = false;
	}

	// Token: 0x17000C5D RID: 3165
	// (get) Token: 0x06007995 RID: 31125 RVA: 0x00316C68 File Offset: 0x00314E68
	public UISprite Sprite
	{
		get
		{
			return this.sprite;
		}
	}

	// Token: 0x17000C5E RID: 3166
	// (get) Token: 0x06007996 RID: 31126 RVA: 0x00316C70 File Offset: 0x00314E70
	// (set) Token: 0x06007997 RID: 31127 RVA: 0x00316C78 File Offset: 0x00314E78
	public string UIAtlas
	{
		get
		{
			return this.uiAtlas;
		}
		set
		{
			this.uiAtlas = value;
			this.isDirty = true;
		}
	}

	// Token: 0x17000C5F RID: 3167
	// (get) Token: 0x06007998 RID: 31128 RVA: 0x00316C88 File Offset: 0x00314E88
	// (set) Token: 0x06007999 RID: 31129 RVA: 0x00316C90 File Offset: 0x00314E90
	public UIBasicSprite.Type Type
	{
		get
		{
			return this.type;
		}
		set
		{
			this.type = value;
			this.isDirty = true;
		}
	}

	// Token: 0x17000C60 RID: 3168
	// (get) Token: 0x0600799A RID: 31130 RVA: 0x00316CA0 File Offset: 0x00314EA0
	// (set) Token: 0x0600799B RID: 31131 RVA: 0x00316CA8 File Offset: 0x00314EA8
	public string DefaultSpriteName
	{
		get
		{
			return this.defaultSpriteName;
		}
		set
		{
			this.defaultSpriteName = value;
			this.isDirty = true;
			this.updateCurrentSprite();
		}
	}

	// Token: 0x17000C61 RID: 3169
	// (get) Token: 0x0600799C RID: 31132 RVA: 0x00316CBE File Offset: 0x00314EBE
	// (set) Token: 0x0600799D RID: 31133 RVA: 0x00316CC6 File Offset: 0x00314EC6
	public Color DefaultSpriteColor
	{
		get
		{
			return this.defaultSpriteColor;
		}
		set
		{
			this.defaultSpriteColor = value;
			this.isDirty = true;
			this.updateCurrentSprite();
		}
	}

	// Token: 0x17000C62 RID: 3170
	// (get) Token: 0x0600799E RID: 31134 RVA: 0x00316CDC File Offset: 0x00314EDC
	// (set) Token: 0x0600799F RID: 31135 RVA: 0x00316CFD File Offset: 0x00314EFD
	public string HoverSpriteName
	{
		get
		{
			if (this.hoverSpriteName == "")
			{
				return this.defaultSpriteName;
			}
			return this.hoverSpriteName;
		}
		set
		{
			this.hoverSpriteName = value;
			this.isDirty = true;
			this.updateCurrentSprite();
		}
	}

	// Token: 0x17000C63 RID: 3171
	// (get) Token: 0x060079A0 RID: 31136 RVA: 0x00316D13 File Offset: 0x00314F13
	// (set) Token: 0x060079A1 RID: 31137 RVA: 0x00316D1B File Offset: 0x00314F1B
	public Color HoverSpriteColor
	{
		get
		{
			return this.hoverSpriteColor;
		}
		set
		{
			this.hoverSpriteColor = value;
			this.isDirty = true;
			this.updateCurrentSprite();
		}
	}

	// Token: 0x17000C64 RID: 3172
	// (get) Token: 0x060079A2 RID: 31138 RVA: 0x00316D31 File Offset: 0x00314F31
	// (set) Token: 0x060079A3 RID: 31139 RVA: 0x00316D52 File Offset: 0x00314F52
	public string SelectedSpriteName
	{
		get
		{
			if (this.selectedSpriteName == "")
			{
				return this.defaultSpriteName;
			}
			return this.selectedSpriteName;
		}
		set
		{
			this.selectedSpriteName = value;
			this.isDirty = true;
			this.updateCurrentSprite();
		}
	}

	// Token: 0x17000C65 RID: 3173
	// (get) Token: 0x060079A4 RID: 31140 RVA: 0x00316D68 File Offset: 0x00314F68
	// (set) Token: 0x060079A5 RID: 31141 RVA: 0x00316D70 File Offset: 0x00314F70
	public Color SelectedSpriteColor
	{
		get
		{
			return this.selectedSpriteColor;
		}
		set
		{
			this.selectedSpriteColor = value;
			this.isDirty = true;
			this.updateCurrentSprite();
		}
	}

	// Token: 0x17000C66 RID: 3174
	// (get) Token: 0x060079A6 RID: 31142 RVA: 0x00316D86 File Offset: 0x00314F86
	// (set) Token: 0x060079A7 RID: 31143 RVA: 0x00316DA7 File Offset: 0x00314FA7
	public string DisabledSpriteName
	{
		get
		{
			if (this.disabledSpriteName == "")
			{
				return this.defaultSpriteName;
			}
			return this.disabledSpriteName;
		}
		set
		{
			this.disabledSpriteName = value;
			this.isDirty = true;
			this.updateCurrentSprite();
		}
	}

	// Token: 0x17000C67 RID: 3175
	// (get) Token: 0x060079A8 RID: 31144 RVA: 0x00316DBD File Offset: 0x00314FBD
	// (set) Token: 0x060079A9 RID: 31145 RVA: 0x00316DC5 File Offset: 0x00314FC5
	public Color DisabledSpriteColor
	{
		get
		{
			return this.disabledSpriteColor;
		}
		set
		{
			this.disabledSpriteColor = value;
			this.isDirty = true;
			this.updateCurrentSprite();
		}
	}

	// Token: 0x17000C68 RID: 3176
	// (get) Token: 0x060079AA RID: 31146 RVA: 0x00316DDB File Offset: 0x00314FDB
	// (set) Token: 0x060079AB RID: 31147 RVA: 0x00316DE3 File Offset: 0x00314FE3
	public bool ManualColors
	{
		get
		{
			return this.manualColors;
		}
		set
		{
			if (value != this.manualColors)
			{
				this.manualColors = value;
				this.isDirty = true;
				this.updateCurrentSprite();
			}
		}
	}

	// Token: 0x17000C69 RID: 3177
	// (get) Token: 0x060079AC RID: 31148 RVA: 0x00316E02 File Offset: 0x00315002
	// (set) Token: 0x060079AD RID: 31149 RVA: 0x00316E0F File Offset: 0x0031500F
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

	// Token: 0x17000C6A RID: 3178
	// (get) Token: 0x060079AE RID: 31150 RVA: 0x00316E28 File Offset: 0x00315028
	// (set) Token: 0x060079AF RID: 31151 RVA: 0x00316E30 File Offset: 0x00315030
	public Color CurrentColor
	{
		get
		{
			return this.currentColor;
		}
		set
		{
			this.currentColor = value;
			this.isDirty = true;
			this.colorDirty = true;
		}
	}

	// Token: 0x17000C6B RID: 3179
	// (get) Token: 0x060079B0 RID: 31152 RVA: 0x00316E47 File Offset: 0x00315047
	// (set) Token: 0x060079B1 RID: 31153 RVA: 0x00316E4F File Offset: 0x0031504F
	public string CurrentSpriteName
	{
		get
		{
			return this.currentSpriteName;
		}
		set
		{
			if (value != this.currentSpriteName)
			{
				this.currentSpriteName = value;
				this.isDirty = true;
				this.colorDirty = true;
			}
		}
	}

	// Token: 0x17000C6C RID: 3180
	// (get) Token: 0x060079B2 RID: 31154 RVA: 0x00316E74 File Offset: 0x00315074
	// (set) Token: 0x060079B3 RID: 31155 RVA: 0x00316E7C File Offset: 0x0031507C
	public bool Selected
	{
		get
		{
			return this.selected;
		}
		set
		{
			if (this.selected != value)
			{
				this.selected = value;
				this.isDirty = true;
				this.updateCurrentSprite();
			}
		}
	}

	// Token: 0x17000C6D RID: 3181
	// (get) Token: 0x060079B4 RID: 31156 RVA: 0x00316E9B File Offset: 0x0031509B
	// (set) Token: 0x060079B5 RID: 31157 RVA: 0x00316EA3 File Offset: 0x003150A3
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

	// Token: 0x17000C6E RID: 3182
	// (get) Token: 0x060079B6 RID: 31158 RVA: 0x00316EB3 File Offset: 0x003150B3
	// (set) Token: 0x060079B7 RID: 31159 RVA: 0x00316EBB File Offset: 0x003150BB
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

	// Token: 0x17000C6F RID: 3183
	// (get) Token: 0x060079B8 RID: 31160 RVA: 0x00316ED4 File Offset: 0x003150D4
	// (set) Token: 0x060079B9 RID: 31161 RVA: 0x00316EDC File Offset: 0x003150DC
	public float HoverScale
	{
		get
		{
			return this.hoverScale;
		}
		set
		{
			this.hoverScale = value;
			this.isDirty = true;
		}
	}

	// Token: 0x17000C70 RID: 3184
	// (set) Token: 0x060079BA RID: 31162 RVA: 0x00316EEC File Offset: 0x003150EC
	public override bool Enabled
	{
		set
		{
			bool enabled = this.enabled;
			base.Enabled = value;
			if (value != enabled)
			{
				this.updateCurrentSprite();
				if (!value && this.hoverScale != 1f && this.tweenScale.value != Vector3.one)
				{
					this.tweenScale.SetStartToCurrentValue();
					this.tweenScale.to = Vector3.one;
					this.tweenScale.enabled = true;
					this.tweenScale.duration = 0.25f;
					this.tweenScale.ResetToBeginning();
				}
				if (!this.gamepadSelectableSetFromAttributes)
				{
					base.IsNavigatable = value;
				}
			}
		}
	}

	// Token: 0x060079BB RID: 31163 RVA: 0x00316F8C File Offset: 0x0031518C
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateCurrentSprite()
	{
		if (!this.Enabled)
		{
			if (!this.manualColors)
			{
				this.CurrentColor = this.disabledSpriteColor;
			}
			this.CurrentSpriteName = this.DisabledSpriteName;
			return;
		}
		if (this.Selected)
		{
			if (!this.manualColors)
			{
				this.CurrentColor = this.selectedSpriteColor;
			}
			this.CurrentSpriteName = this.SelectedSpriteName;
			return;
		}
		if (!this.manualColors)
		{
			this.CurrentColor = (this.isOver ? this.hoverSpriteColor : this.defaultSpriteColor);
		}
		this.CurrentSpriteName = (this.isOver ? this.HoverSpriteName : this.DefaultSpriteName);
	}

	// Token: 0x060079BC RID: 31164 RVA: 0x0031702B File Offset: 0x0031522B
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CreateComponents(GameObject _go)
	{
		_go.AddComponent<UISprite>();
		_go.AddComponent<TweenScale>().enabled = false;
	}

	// Token: 0x060079BD RID: 31165 RVA: 0x00317040 File Offset: 0x00315240
	public override void InitView()
	{
		this.EventOnPress = true;
		this.EventOnHover = true;
		base.InitView();
		this.sprite = this.uiTransform.GetComponent<UISprite>();
		this.UpdateData();
		this.initialized = true;
		this.Enabled = true;
	}

	// Token: 0x060079BE RID: 31166 RVA: 0x0031707C File Offset: 0x0031527C
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (null != this.sprite)
		{
			bool isVisible = this.sprite.isVisible;
			if (this.lastVisible != isVisible)
			{
				this.isDirty = true;
			}
			this.lastVisible = isVisible;
			if (this.isOver && UICamera.hoveredObject != this.uiTransform.gameObject)
			{
				this.OnHover(this.uiTransform.gameObject, false);
			}
		}
	}

	// Token: 0x060079BF RID: 31167 RVA: 0x003170F4 File Offset: 0x003152F4
	public override void UpdateData()
	{
		this.sprite.spriteName = this.currentSpriteName;
		this.sprite.atlas = base.xui.GetAtlasByName(this.uiAtlas, this.currentSpriteName);
		this.sprite.color = this.currentColor;
		if (this.globalOpacityModifier != 0f && (this.foregroundLayer ? (base.xui.ForegroundGlobalOpacity < 1f) : (base.xui.BackgroundGlobalOpacity < 1f)))
		{
			float a = Mathf.Clamp01(this.currentColor.a * (this.globalOpacityModifier * (this.foregroundLayer ? base.xui.ForegroundGlobalOpacity : base.xui.BackgroundGlobalOpacity)));
			this.sprite.color = new Color(this.currentColor.r, this.currentColor.g, this.currentColor.b, a);
		}
		if (this.borderSize > 0)
		{
			this.sprite.border = new Vector4((float)this.borderSize, (float)this.borderSize, (float)this.borderSize, (float)this.borderSize);
		}
		this.sprite.centerType = UIBasicSprite.AdvancedType.Sliced;
		this.sprite.type = this.type;
		base.parseAnchors(this.sprite, true);
		if (this.sprite.flip != this.flip)
		{
			this.sprite.flip = this.flip;
		}
		if (this.hoverScale != 1f && this.tweenScale == null)
		{
			this.tweenScale = this.uiTransform.gameObject.GetComponent<TweenScale>();
		}
		if (!this.initialized)
		{
			this.sprite.pivot = this.pivot;
			this.sprite.depth = this.depth;
			this.uiTransform.localScale = Vector3.one;
			this.uiTransform.localPosition = new Vector3((float)base.PaddedPosition.x, (float)base.PaddedPosition.y, 0f);
			BoxCollider collider = this.collider;
			collider.center = this.sprite.localCenter;
			collider.size = new Vector3(this.sprite.localSize.x * this.colliderScale, this.sprite.localSize.y * this.colliderScale, 0f);
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

	// Token: 0x060079C0 RID: 31168 RVA: 0x0031738C File Offset: 0x0031558C
	public override void OnHover(GameObject _go, bool _isOver)
	{
		base.OnHover(_go, _isOver);
		this.updateCurrentSprite();
		if (this.Enabled && this.hoverScale != 1f)
		{
			this.tweenScale.to = (this.isOver ? (Vector3.one * this.hoverScale) : Vector3.one);
			this.tweenScale.SetStartToCurrentValue();
			this.tweenScale.duration = 0.25f;
			this.tweenScale.ResetToBeginning();
			this.tweenScale.enabled = true;
		}
	}

	// Token: 0x060079C1 RID: 31169 RVA: 0x00317418 File Offset: 0x00315618
	public override bool ParseAttribute(string attribute, string value, XUiController _parent)
	{
		bool flag = base.ParseAttribute(attribute, value, _parent);
		if (!flag)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(attribute);
			if (num <= 2179094556U)
			{
				if (num <= 358097113U)
				{
					if (num <= 45741760U)
					{
						if (num != 19667905U)
						{
							if (num == 45741760U)
							{
								if (attribute == "hovercolor")
								{
									this.HoverSpriteColor = StringParsers.ParseColor32(value);
									return true;
								}
							}
						}
						else if (attribute == "defaultcolor")
						{
							this.DefaultSpriteColor = StringParsers.ParseColor32(value);
							this.CurrentColor = this.defaultSpriteColor;
							return true;
						}
					}
					else if (num != 311159388U)
					{
						if (num == 358097113U)
						{
							if (attribute == "hoverscale")
							{
								this.HoverScale = StringParsers.ParseFloat(value, 0, -1, NumberStyles.Any);
								return true;
							}
						}
					}
					else if (attribute == "disabledsprite")
					{
						this.DisabledSpriteName = value;
						return true;
					}
				}
				else if (num <= 1361572173U)
				{
					if (num != 1309284212U)
					{
						if (num == 1361572173U)
						{
							if (attribute == "type")
							{
								this.Type = EnumUtils.Parse<UIBasicSprite.Type>(value, true);
								return true;
							}
						}
					}
					else if (attribute == "selected")
					{
						this.Selected = StringParsers.ParseBool(value, 0, -1, true);
						return true;
					}
				}
				else if (num != 2006771483U)
				{
					if (num == 2179094556U)
					{
						if (attribute == "sprite")
						{
							this.DefaultSpriteName = value;
							this.CurrentSpriteName = value;
							return true;
						}
					}
				}
				else if (attribute == "selectedcolor")
				{
					this.SelectedSpriteColor = StringParsers.ParseColor32(value);
					return true;
				}
			}
			else if (num <= 3060355671U)
			{
				if (num <= 2245939092U)
				{
					if (num != 2207167896U)
					{
						if (num == 2245939092U)
						{
							if (attribute == "hoversprite")
							{
								this.HoverSpriteName = value;
								return true;
							}
						}
					}
					else if (attribute == "disabledcolor")
					{
						this.DisabledSpriteColor = StringParsers.ParseColor32(value);
						return true;
					}
				}
				else if (num != 2629038627U)
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
				else if (attribute == "manualcolors")
				{
					this.ManualColors = StringParsers.ParseBool(value, 0, -1, true);
					return true;
				}
			}
			else if (num <= 3194542381U)
			{
				if (num != 3134659686U)
				{
					if (num == 3194542381U)
					{
						if (attribute == "selectedsprite")
						{
							this.SelectedSpriteName = value;
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
			else if (num != 3407328204U)
			{
				if (num != 3458780677U)
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
				else if (attribute == "foregroundlayer")
				{
					this.foregroundLayer = StringParsers.ParseBool(value, 0, -1, true);
					return true;
				}
			}
			else if (attribute == "atlas")
			{
				this.UIAtlas = value;
				return true;
			}
			return false;
		}
		return flag;
	}

	// Token: 0x060079C2 RID: 31170 RVA: 0x003177EA File Offset: 0x003159EA
	public override void OnOpen()
	{
		base.OnOpen();
		this.updateCurrentSprite();
		this.uiTransform.localScale = Vector3.one;
	}

	// Token: 0x04005C4A RID: 23626
	[PublicizedFrom(EAccessModifier.Protected)]
	public string uiAtlas = string.Empty;

	// Token: 0x04005C4B RID: 23627
	[PublicizedFrom(EAccessModifier.Protected)]
	public UISprite sprite;

	// Token: 0x04005C4C RID: 23628
	[PublicizedFrom(EAccessModifier.Protected)]
	public UIBasicSprite.Type type;

	// Token: 0x04005C4D RID: 23629
	[PublicizedFrom(EAccessModifier.Protected)]
	public UIBasicSprite.Flip flip;

	// Token: 0x04005C4E RID: 23630
	[PublicizedFrom(EAccessModifier.Protected)]
	public string defaultSpriteName = string.Empty;

	// Token: 0x04005C4F RID: 23631
	[PublicizedFrom(EAccessModifier.Protected)]
	public string hoverSpriteName = string.Empty;

	// Token: 0x04005C50 RID: 23632
	[PublicizedFrom(EAccessModifier.Protected)]
	public string selectedSpriteName = string.Empty;

	// Token: 0x04005C51 RID: 23633
	[PublicizedFrom(EAccessModifier.Protected)]
	public string disabledSpriteName = string.Empty;

	// Token: 0x04005C52 RID: 23634
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color defaultSpriteColor = Color.white;

	// Token: 0x04005C53 RID: 23635
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color hoverSpriteColor = Color.white;

	// Token: 0x04005C54 RID: 23636
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color selectedSpriteColor = Color.white;

	// Token: 0x04005C55 RID: 23637
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color disabledSpriteColor = Color.white;

	// Token: 0x04005C56 RID: 23638
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool manualColors;

	// Token: 0x04005C57 RID: 23639
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color currentColor = Color.white;

	// Token: 0x04005C58 RID: 23640
	[PublicizedFrom(EAccessModifier.Protected)]
	public string currentSpriteName = string.Empty;

	// Token: 0x04005C59 RID: 23641
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool selected;

	// Token: 0x04005C5A RID: 23642
	[PublicizedFrom(EAccessModifier.Private)]
	public bool lastVisible;

	// Token: 0x04005C5B RID: 23643
	[PublicizedFrom(EAccessModifier.Private)]
	public bool colorDirty;

	// Token: 0x04005C5C RID: 23644
	[PublicizedFrom(EAccessModifier.Private)]
	public float hoverScale = 1f;

	// Token: 0x04005C5D RID: 23645
	[PublicizedFrom(EAccessModifier.Private)]
	public bool foregroundLayer = true;

	// Token: 0x04005C5E RID: 23646
	[PublicizedFrom(EAccessModifier.Private)]
	public TweenScale tweenScale;

	// Token: 0x04005C5F RID: 23647
	[PublicizedFrom(EAccessModifier.Private)]
	public float globalOpacityModifier = 1f;

	// Token: 0x04005C60 RID: 23648
	[PublicizedFrom(EAccessModifier.Private)]
	public int borderSize = -1;
}
