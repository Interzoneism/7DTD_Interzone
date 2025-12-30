using System;
using System.Collections.Generic;
using System.Globalization;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000D7A RID: 3450
[Preserve]
public class XUiC_PopupMenu : XUiController
{
	// Token: 0x06006BDB RID: 27611 RVA: 0x002C2694 File Offset: 0x002C0894
	public override void Init()
	{
		base.Init();
		base.xui.currentPopupMenu = this;
		this.gridView = (XUiV_Grid)base.GetChildById("list").ViewComponent;
		this.popupMenuItems = this.gridView.Controller.GetChildrenByType<XUiC_PopupMenuItem>(null);
		base.ViewComponent.IsVisible = false;
	}

	// Token: 0x06006BDC RID: 27612 RVA: 0x002C26F1 File Offset: 0x002C08F1
	public void Setup(Vector2i _offsetPosition, XUiView _originView)
	{
		this.ClearItems();
		this.IsOver = false;
		this.xuiPosition = base.xui.GetMouseXUIPosition();
		this.offset = _offsetPosition;
		this.originView = _originView;
		this.IsDirty = true;
	}

	// Token: 0x06006BDD RID: 27613 RVA: 0x002C2726 File Offset: 0x002C0926
	public void AddItem(XUiC_PopupMenuItem.Entry _newMenuItems)
	{
		this.menuItems.Add(_newMenuItems);
		this.IsDirty = true;
	}

	// Token: 0x06006BDE RID: 27614 RVA: 0x002C273B File Offset: 0x002C093B
	public void ClearItems()
	{
		this.menuItems.Clear();
		this.IsDirty = true;
	}

	// Token: 0x06006BDF RID: 27615 RVA: 0x002C2750 File Offset: 0x002C0950
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.menuItems.Count > 0 && !this.IsDirty)
		{
			if (PlatformManager.NativePlatform.Input.CurrentInputStyle == PlayerInputManager.InputStyle.Keyboard)
			{
				if ((base.xui.playerUI.CursorController.GetMouseButtonUp(UICamera.MouseButton.LeftButton) | base.xui.playerUI.playerInput.GUIActions.RightStick.WasReleased) && !this.IsOver)
				{
					this.ClearItems();
				}
			}
			else if (base.xui.playerUI.playerInput.GUIActions.Cancel.WasReleased)
			{
				this.ClearItems();
			}
		}
		if (this.IsDirty)
		{
			this.IsDirty = false;
			if (this.menuItems.Count == 0)
			{
				base.ViewComponent.IsVisible = false;
				if (base.xui.playerUI.CursorController.navigationTarget != null && base.xui.playerUI.CursorController.navigationTarget.Controller.IsChildOf(this))
				{
					base.xui.playerUI.CursorController.SetNavigationLockView(null, null);
					base.xui.playerUI.CursorController.SetNavigationTarget(this.originView);
					this.originView = null;
				}
				return;
			}
			int num = 0;
			int num2 = this.menuItems.Count - 1;
			for (int i = 0; i < this.popupMenuItems.Length; i++)
			{
				int num3 = this.popupMenuItems[i].SetEntry((i < this.menuItems.Count) ? this.menuItems[i] : null);
				if (num3 > num)
				{
					num = num3;
				}
				if (i < num2 && i < this.menuItems.Count && this.menuItems[i].IsEnabled)
				{
					num2 = i;
				}
			}
			num += this.maxWidthPadding;
			this.gridSize = new Vector2i(num, this.menuItems.Count * this.gridView.CellHeight);
			this.limitPositionToScreenBounds();
			base.ViewComponent.IsVisible = true;
			base.xui.playerUI.CursorController.SetNavigationLockView(this.viewComponent, this.popupMenuItems[num2].ViewComponent);
			base.RefreshBindings(false);
		}
	}

	// Token: 0x06006BE0 RID: 27616 RVA: 0x002C2988 File Offset: 0x002C0B88
	[PublicizedFrom(EAccessModifier.Private)]
	public void limitPositionToScreenBounds()
	{
		Vector2i vector2i = this.gridSize;
		vector2i.y = -vector2i.y;
		Vector2i vector2i2 = base.xui.GetXUiScreenSize() / 2;
		Vector2i vector2i3 = new Vector2i((int)((double)vector2i2.x * 0.97), (int)((double)vector2i2.y * 0.97));
		Vector2i vector2i4 = this.xuiPosition + this.offset;
		Vector2i vector2i5 = vector2i4;
		Vector2i vector2i6 = vector2i5 + vector2i;
		if (vector2i6.x >= vector2i3.x)
		{
			vector2i4.x = vector2i3.x - vector2i.x;
		}
		else if (vector2i5.x <= -vector2i3.x)
		{
			vector2i4.x = -vector2i3.x;
		}
		if (vector2i6.y <= -vector2i3.y)
		{
			vector2i4.y = -vector2i3.y - vector2i.y;
		}
		else if (vector2i5.y >= vector2i3.y)
		{
			vector2i4.y = vector2i3.y;
		}
		base.ViewComponent.Position = vector2i4;
	}

	// Token: 0x06006BE1 RID: 27617 RVA: 0x002C2A94 File Offset: 0x002C0C94
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "grid_width")
		{
			_value = this.gridSize.x.ToString();
			return true;
		}
		if (!(_bindingName == "grid_height"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		_value = this.gridSize.y.ToString();
		return true;
	}

	// Token: 0x06006BE2 RID: 27618 RVA: 0x002C2AF0 File Offset: 0x002C0CF0
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "element_padding_over_label")
		{
			this.maxWidthPadding = StringParsers.ParseSInt32(_value, 0, -1, NumberStyles.Integer);
			return true;
		}
		if (!(_name == "slider_min_width"))
		{
			return base.ParseAttribute(_name, _value, _parent);
		}
		this.SliderMinWidth = StringParsers.ParseSInt32(_value, 0, -1, NumberStyles.Integer);
		return true;
	}

	// Token: 0x06006BE3 RID: 27619 RVA: 0x002C2B44 File Offset: 0x002C0D44
	public override void OnVisibilityChanged(bool _isVisible)
	{
		base.OnVisibilityChanged(_isVisible);
		if (_isVisible)
		{
			base.ViewComponent.TryUpdatePosition();
		}
	}

	// Token: 0x04005219 RID: 21017
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<XUiC_PopupMenuItem.Entry> menuItems = new List<XUiC_PopupMenuItem.Entry>();

	// Token: 0x0400521A RID: 21018
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2i xuiPosition;

	// Token: 0x0400521B RID: 21019
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2i offset;

	// Token: 0x0400521C RID: 21020
	public bool IsOver;

	// Token: 0x0400521D RID: 21021
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Grid gridView;

	// Token: 0x0400521E RID: 21022
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_PopupMenuItem[] popupMenuItems;

	// Token: 0x0400521F RID: 21023
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiView originView;

	// Token: 0x04005220 RID: 21024
	[PublicizedFrom(EAccessModifier.Private)]
	public int maxWidthPadding = 60;

	// Token: 0x04005221 RID: 21025
	public int SliderMinWidth = 250;

	// Token: 0x04005222 RID: 21026
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2i gridSize = Vector2i.one;
}
