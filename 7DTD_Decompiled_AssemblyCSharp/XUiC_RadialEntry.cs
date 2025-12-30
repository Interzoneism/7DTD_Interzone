using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000DD6 RID: 3542
[Preserve]
public class XUiC_RadialEntry : XUiController
{
	// Token: 0x06006ED5 RID: 28373 RVA: 0x002D3E34 File Offset: 0x002D2034
	public override void Init()
	{
		base.Init();
		XUiController childById = base.GetChildById("icon");
		XUiController childById2 = base.GetChildById("background");
		XUiController childById3 = base.GetChildById("text");
		this.icon = (childById.ViewComponent as XUiV_Sprite);
		this.background = (childById2.ViewComponent as XUiV_Sprite);
		if (childById3 != null)
		{
			this.text = (childById3.ViewComponent as XUiV_Label);
		}
		if (this.background != null)
		{
			this.backgroundColor = this.background.Color;
		}
	}

	// Token: 0x06006ED6 RID: 28374 RVA: 0x002D3EBA File Offset: 0x002D20BA
	public void SetHighlighted(bool _highlighted)
	{
		this.background.Color = (_highlighted ? this.highlightColor : this.backgroundColor);
	}

	// Token: 0x06006ED7 RID: 28375 RVA: 0x002D3ED8 File Offset: 0x002D20D8
	public void SetSprite(string _sprite, Color _color)
	{
		this.icon.SpriteName = _sprite;
		this.icon.Color = _color;
	}

	// Token: 0x06006ED8 RID: 28376 RVA: 0x002D3EF2 File Offset: 0x002D20F2
	public void SetText(string _text)
	{
		if (this.text != null)
		{
			this.text.Text = _text;
			this.text.IsVisible = (_text != "");
		}
	}

	// Token: 0x06006ED9 RID: 28377 RVA: 0x002D3F1E File Offset: 0x002D211E
	public void SetAtlas(string _atlas)
	{
		this.icon.UIAtlas = _atlas;
	}

	// Token: 0x06006EDA RID: 28378 RVA: 0x002D3F2C File Offset: 0x002D212C
	public void ResetScale()
	{
		this.SetScale(1f, true);
	}

	// Token: 0x06006EDB RID: 28379 RVA: 0x002D3F3C File Offset: 0x002D213C
	public void SetScale(float _scale, bool _instant = false)
	{
		float duration = _instant ? 0f : 0.15f;
		TweenScale.Begin(this.viewComponent.UiTransform.gameObject, duration, Vector3.one * _scale);
	}

	// Token: 0x06006EDC RID: 28380 RVA: 0x002D3F7C File Offset: 0x002D217C
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		bool flag = base.ParseAttribute(_name, _value, _parent);
		if (!flag && _name.EqualsCaseInsensitive("highlight_color"))
		{
			this.highlightColor = StringParsers.ParseColor32(_value);
			return true;
		}
		return flag;
	}

	// Token: 0x04005431 RID: 21553
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite icon;

	// Token: 0x04005432 RID: 21554
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite background;

	// Token: 0x04005433 RID: 21555
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label text;

	// Token: 0x04005434 RID: 21556
	[PublicizedFrom(EAccessModifier.Private)]
	public Color backgroundColor;

	// Token: 0x04005435 RID: 21557
	[PublicizedFrom(EAccessModifier.Private)]
	public Color highlightColor;

	// Token: 0x04005436 RID: 21558
	public string SelectionText;

	// Token: 0x04005437 RID: 21559
	public int MenuItemIndex;

	// Token: 0x04005438 RID: 21560
	public int CommandIndex;
}
