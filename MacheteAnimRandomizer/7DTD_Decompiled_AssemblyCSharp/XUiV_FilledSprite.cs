using System;
using UnityEngine;

// Token: 0x02000F04 RID: 3844
public class XUiV_FilledSprite : XUiV_Sprite
{
	// Token: 0x17000C71 RID: 3185
	// (get) Token: 0x060079C3 RID: 31171 RVA: 0x00317808 File Offset: 0x00315A08
	// (set) Token: 0x060079C4 RID: 31172 RVA: 0x00002914 File Offset: 0x00000B14
	public override UIBasicSprite.Type Type
	{
		get
		{
			return this.type;
		}
		set
		{
		}
	}

	// Token: 0x060079C5 RID: 31173 RVA: 0x00317810 File Offset: 0x00315A10
	public XUiV_FilledSprite(string _id) : base(_id)
	{
	}

	// Token: 0x060079C6 RID: 31174 RVA: 0x0031781C File Offset: 0x00315A1C
	public override void UpdateData()
	{
		if (base.applyAtlasAndSprite(false))
		{
			Vector4 border = this.sprite.border;
			this.spriteBorder = new Vector2i(Mathf.RoundToInt(border.x + border.z), Mathf.RoundToInt(border.y + border.w));
		}
		this.sprite.centerType = (this.fillCenter ? UIBasicSprite.AdvancedType.Sliced : UIBasicSprite.AdvancedType.Invisible);
		int num = (this.fillDirection == UIBasicSprite.FillDirection.Horizontal) ? Mathf.RoundToInt(this.fillAmount * (float)this.size.x) : this.size.x;
		int num2 = (this.fillDirection == UIBasicSprite.FillDirection.Vertical) ? Mathf.RoundToInt(this.fillAmount * (float)this.size.y) : this.size.y;
		if (num != this.sprite.width || num2 != this.sprite.height || this.positionDirty)
		{
			this.positionDirty = false;
			this.sprite.SetDimensions(num, num2);
			this.hideFill = ((this.fillDirection == UIBasicSprite.FillDirection.Horizontal && num < this.spriteBorder.x) || (this.fillDirection == UIBasicSprite.FillDirection.Vertical && num2 < this.spriteBorder.y));
			if (this.hideFill)
			{
				this.sprite.color = Color.clear;
			}
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			if (this.fillDirection == UIBasicSprite.FillDirection.Horizontal)
			{
				if (!this.fillInvert)
				{
					num3 = 0;
					num4 = Mathf.FloorToInt((float)(-(float)this.size.x + num) / 2f);
					num5 = -this.size.x + num;
				}
				else
				{
					num3 = this.size.x - num;
					num4 = Mathf.CeilToInt((float)(this.size.x - num) / 2f);
					num5 = 0;
				}
			}
			else
			{
				if (this.fillDirection != UIBasicSprite.FillDirection.Vertical)
				{
					Log.Warning("[XUi] FilledSprite only allows FillDirections Horizontal and Vertical");
					return;
				}
				if (!this.fillInvert)
				{
					num6 = 0;
					num7 = Mathf.FloorToInt((float)(-(float)this.size.y + num2) / 2f);
					num8 = -this.size.y + num2;
				}
				else
				{
					num6 = this.size.y - num2;
					num7 = Mathf.CeilToInt((float)(this.size.y - num2) / 2f);
					num8 = 0;
				}
			}
			int num9;
			switch (this.pivot)
			{
			case UIWidget.Pivot.TopLeft:
			case UIWidget.Pivot.Left:
			case UIWidget.Pivot.BottomLeft:
				num9 = num3;
				break;
			case UIWidget.Pivot.Top:
			case UIWidget.Pivot.Center:
			case UIWidget.Pivot.Bottom:
				num9 = num4;
				break;
			case UIWidget.Pivot.TopRight:
			case UIWidget.Pivot.Right:
			case UIWidget.Pivot.BottomRight:
				num9 = num5;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			int num10;
			switch (this.pivot)
			{
			case UIWidget.Pivot.TopLeft:
			case UIWidget.Pivot.Top:
			case UIWidget.Pivot.TopRight:
				num10 = num8;
				break;
			case UIWidget.Pivot.Left:
			case UIWidget.Pivot.Center:
			case UIWidget.Pivot.Right:
				num10 = num7;
				break;
			case UIWidget.Pivot.BottomLeft:
			case UIWidget.Pivot.Bottom:
			case UIWidget.Pivot.BottomRight:
				num10 = num6;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			this.positionDirty = false;
			this.uiTransform.localPosition = new Vector3((float)(base.PaddedPosition.x + num9), (float)(base.PaddedPosition.y + num10), 0f);
			if (this.EventOnHover || this.EventOnPress)
			{
				this.boxCollider.center = this.sprite.localCenter;
				this.boxCollider.size = new Vector3(this.sprite.localSize.x * this.colliderScale, this.sprite.localSize.y * this.colliderScale, 0f);
			}
		}
		if (!this.hideFill && this.sprite.color != this.color)
		{
			this.sprite.color = this.color;
		}
		if (!this.hideFill && this.globalOpacityModifier != 0f && (this.foregroundLayer ? (base.xui.ForegroundGlobalOpacity < 1f) : (base.xui.BackgroundGlobalOpacity < 1f)))
		{
			float a = Mathf.Clamp01(this.color.a * (this.globalOpacityModifier * (this.foregroundLayer ? base.xui.ForegroundGlobalOpacity : base.xui.BackgroundGlobalOpacity)));
			this.sprite.color = new Color(this.color.r, this.color.g, this.color.b, a);
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
				this.boxCollider = this.collider;
				this.boxCollider.center = this.sprite.localCenter;
				this.boxCollider.size = new Vector3(this.sprite.localSize.x * this.colliderScale, this.sprite.localSize.y * this.colliderScale, 0f);
			}
		}
		this.RefreshBoxCollider();
	}

	// Token: 0x060079C7 RID: 31175 RVA: 0x00317DC4 File Offset: 0x00315FC4
	public override void SetDefaults(XUiController _parent)
	{
		base.SetDefaults(_parent);
		base.FillCenter = true;
		this.type = UIBasicSprite.Type.Sliced;
		base.FillDirection = UIBasicSprite.FillDirection.Horizontal;
	}

	// Token: 0x060079C8 RID: 31176 RVA: 0x00317DE2 File Offset: 0x00315FE2
	public override bool ParseAttribute(string _attribute, string _value, XUiController _parent)
	{
		return _attribute == "type" || base.ParseAttribute(_attribute, _value, _parent);
	}

	// Token: 0x04005C61 RID: 23649
	[PublicizedFrom(EAccessModifier.Protected)]
	public BoxCollider boxCollider;

	// Token: 0x04005C62 RID: 23650
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2i spriteBorder;

	// Token: 0x04005C63 RID: 23651
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hideFill;
}
