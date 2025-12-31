using System;

// Token: 0x02000C97 RID: 3223
public abstract class XUiC_DMBaseList<T> : XUiC_List<T> where T : XUiListEntry<T>
{
	// Token: 0x1400009C RID: 156
	// (add) Token: 0x0600636F RID: 25455 RVA: 0x00285244 File Offset: 0x00283444
	// (remove) Token: 0x06006370 RID: 25456 RVA: 0x0028527C File Offset: 0x0028347C
	public event XUiEvent_OnPressEventHandler OnEntryClicked;

	// Token: 0x1400009D RID: 157
	// (add) Token: 0x06006371 RID: 25457 RVA: 0x002852B4 File Offset: 0x002834B4
	// (remove) Token: 0x06006372 RID: 25458 RVA: 0x002852EC File Offset: 0x002834EC
	public event XUiEvent_OnPressEventHandler OnEntryDoubleClicked;

	// Token: 0x1400009E RID: 158
	// (add) Token: 0x06006373 RID: 25459 RVA: 0x00285324 File Offset: 0x00283524
	// (remove) Token: 0x06006374 RID: 25460 RVA: 0x0028535C File Offset: 0x0028355C
	public event XUiEvent_OnHoverEventHandler OnChildElementHovered;

	// Token: 0x06006375 RID: 25461 RVA: 0x00285394 File Offset: 0x00283594
	public override void Init()
	{
		base.Init();
		foreach (XUiC_ListEntry<T> xuiC_ListEntry in this.listEntryControllers)
		{
			xuiC_ListEntry.OnPress += this.EntryClicked;
			xuiC_ListEntry.OnDoubleClick += this.EntryDoubleClicked;
			xuiC_ListEntry.OnHover += this.ChildElementHovered;
		}
		foreach (XUiController xuiController in this.pager.Children)
		{
			xuiController.OnHover += this.ChildElementHovered;
		}
		base.PageContentsChanged += this.PageContentsChangedHandler;
		this.searchBox.OnHover += this.ChildElementHovered;
	}

	// Token: 0x06006376 RID: 25462 RVA: 0x00285474 File Offset: 0x00283674
	[PublicizedFrom(EAccessModifier.Protected)]
	public void PageContentsChangedHandler()
	{
		if (this.hoveredElement != null && this.OnChildElementHovered != null)
		{
			this.OnChildElementHovered(this.hoveredElement, false);
			this.OnChildElementHovered(this.hoveredElement, true);
		}
	}

	// Token: 0x06006377 RID: 25463 RVA: 0x002854AA File Offset: 0x002836AA
	[PublicizedFrom(EAccessModifier.Protected)]
	public void EntryClicked(XUiController _sender, int _mouseButton)
	{
		XUiEvent_OnPressEventHandler onEntryClicked = this.OnEntryClicked;
		if (onEntryClicked == null)
		{
			return;
		}
		onEntryClicked(_sender, _mouseButton);
	}

	// Token: 0x06006378 RID: 25464 RVA: 0x002854BE File Offset: 0x002836BE
	[PublicizedFrom(EAccessModifier.Protected)]
	public void EntryDoubleClicked(XUiController _sender, int _mouseButton)
	{
		XUiEvent_OnPressEventHandler onEntryDoubleClicked = this.OnEntryDoubleClicked;
		if (onEntryDoubleClicked == null)
		{
			return;
		}
		onEntryDoubleClicked(_sender, _mouseButton);
	}

	// Token: 0x06006379 RID: 25465 RVA: 0x002854D2 File Offset: 0x002836D2
	[PublicizedFrom(EAccessModifier.Protected)]
	public void ChildElementHovered(XUiController _sender, bool _isOver)
	{
		this.hoveredElement = (_isOver ? _sender : null);
		XUiEvent_OnHoverEventHandler onChildElementHovered = this.OnChildElementHovered;
		if (onChildElementHovered == null)
		{
			return;
		}
		onChildElementHovered(_sender, _isOver);
	}

	// Token: 0x0600637A RID: 25466 RVA: 0x002854F3 File Offset: 0x002836F3
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiC_DMBaseList()
	{
	}

	// Token: 0x04004ADC RID: 19164
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController hoveredElement;
}
