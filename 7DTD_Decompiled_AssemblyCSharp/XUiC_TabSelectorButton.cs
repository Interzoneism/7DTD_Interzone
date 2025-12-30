using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000E5C RID: 3676
[Preserve]
public class XUiC_TabSelectorButton : XUiController
{
	// Token: 0x06007377 RID: 29559 RVA: 0x002F105C File Offset: 0x002EF25C
	public override void Init()
	{
		base.Init();
		this.parentSelector = base.GetParentByType<XUiC_TabSelector>();
		this.OuterDimensionsTransform = base.ViewComponent.UiTransform.FindRecursive("border");
		XUiC_SimpleButton childByType = base.GetChildByType<XUiC_SimpleButton>();
		if (childByType != null)
		{
			this.simpleButton = childByType;
			childByType.OnPressed += delegate(XUiController _, int _)
			{
				this.parentSelector.TabButtonClicked(this);
			};
			childByType.Button.IsSnappable = (childByType.Button.IsNavigatable = false);
			return;
		}
		this.interactionView = this.findClickableChild(this);
		if (this.interactionView == null)
		{
			Log.Error("[XUi] TabSelectorButton without SimpleButton or other view with 'on_press=true' in windowGroup '" + this.windowGroup.ID + "'");
			return;
		}
		this.interactionView.Controller.OnPress += delegate(XUiController _, int _)
		{
			this.parentSelector.TabButtonClicked(this);
		};
		this.interactionView.IsSnappable = (this.interactionView.IsNavigatable = false);
	}

	// Token: 0x06007378 RID: 29560 RVA: 0x002F1140 File Offset: 0x002EF340
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiView findClickableChild(XUiController _controller)
	{
		if (_controller.ViewComponent.EventOnPress)
		{
			return _controller.ViewComponent;
		}
		foreach (XUiController controller in _controller.Children)
		{
			XUiView xuiView = this.findClickableChild(controller);
			if (xuiView != null)
			{
				return xuiView;
			}
		}
		return null;
	}

	// Token: 0x06007379 RID: 29561 RVA: 0x002DED21 File Offset: 0x002DCF21
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty)
		{
			this.IsDirty = false;
			base.RefreshBindings(false);
		}
	}

	// Token: 0x0600737A RID: 29562 RVA: 0x002F11B4 File Offset: 0x002EF3B4
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		return base.ParseAttribute(_name, _value, _parent);
	}

	// Token: 0x0600737B RID: 29563 RVA: 0x002F11C0 File Offset: 0x002EF3C0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "tab_selected")
		{
			XUiC_TabSelectorTab tab = this.Tab;
			_value = (tab != null && tab.TabSelected).ToString();
			return true;
		}
		if (!(_bindingName == "tab_name_localized"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		XUiC_TabSelectorTab tab2 = this.Tab;
		_value = (((tab2 != null) ? tab2.TabHeaderText : null) ?? "");
		return true;
	}

	// Token: 0x0600737C RID: 29564 RVA: 0x002F122E File Offset: 0x002EF42E
	public void UpdateSelectionState()
	{
		if (this.simpleButton != null)
		{
			this.simpleButton.Button.Selected = this.Tab.TabSelected;
		}
		base.RefreshBindings(false);
	}

	// Token: 0x0600737D RID: 29565 RVA: 0x002F125A File Offset: 0x002EF45A
	public void UpdateVisibilityState()
	{
		base.ViewComponent.IsVisible = this.Tab.TabVisible;
	}

	// Token: 0x0600737E RID: 29566 RVA: 0x002F1272 File Offset: 0x002EF472
	public void PlayClickSound()
	{
		if (this.simpleButton != null)
		{
			this.simpleButton.Button.PlayClickSound();
			return;
		}
		if (this.interactionView != null)
		{
			this.interactionView.PlayClickSound();
		}
	}

	// Token: 0x040057E6 RID: 22502
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TabSelector parentSelector;

	// Token: 0x040057E7 RID: 22503
	public XUiC_TabSelectorTab Tab;

	// Token: 0x040057E8 RID: 22504
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton simpleButton;

	// Token: 0x040057E9 RID: 22505
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiView interactionView;

	// Token: 0x040057EA RID: 22506
	public Transform OuterDimensionsTransform;
}
