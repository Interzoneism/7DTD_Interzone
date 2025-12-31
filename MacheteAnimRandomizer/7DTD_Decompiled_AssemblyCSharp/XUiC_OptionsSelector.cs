using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000D57 RID: 3415
[Preserve]
public class XUiC_OptionsSelector : XUiController
{
	// Token: 0x140000B0 RID: 176
	// (add) Token: 0x06006AB3 RID: 27315 RVA: 0x002B6BAC File Offset: 0x002B4DAC
	// (remove) Token: 0x06006AB4 RID: 27316 RVA: 0x002B6BE4 File Offset: 0x002B4DE4
	public event XUiEvent_OnOptionSelectionChanged OnSelectionChanged;

	// Token: 0x17000AC5 RID: 2757
	// (get) Token: 0x06006AB5 RID: 27317 RVA: 0x002B6C19 File Offset: 0x002B4E19
	// (set) Token: 0x06006AB6 RID: 27318 RVA: 0x002B6C21 File Offset: 0x002B4E21
	public int SelectedIndex
	{
		get
		{
			return this.selectedIndex;
		}
		set
		{
			this.selectedIndex = value;
			this.IsDirty = true;
		}
	}

	// Token: 0x17000AC6 RID: 2758
	// (get) Token: 0x06006AB7 RID: 27319 RVA: 0x002B6C31 File Offset: 0x002B4E31
	// (set) Token: 0x06006AB8 RID: 27320 RVA: 0x002B6C3E File Offset: 0x002B4E3E
	public string Title
	{
		get
		{
			return this.lblTitle.Text;
		}
		set
		{
			this.lblTitle.Text = value;
		}
	}

	// Token: 0x06006AB9 RID: 27321 RVA: 0x002B6C4C File Offset: 0x002B4E4C
	public override void Init()
	{
		base.Init();
		this.leftArrow = base.GetChildById("leftArrow");
		this.rightArrow = base.GetChildById("rightArrow");
		this.textArea = base.GetChildById("textArea");
		this.clickable = base.GetChildById("clickable").ViewComponent;
		this.lblSelected = (this.textArea.GetChildById("lblText").ViewComponent as XUiV_Label);
		this.lblTitle = (base.GetChildById("lblTitle").ViewComponent as XUiV_Label);
		this.leftArrow.OnPress += this.HandleLeftArrowOnPress;
		this.rightArrow.OnPress += this.HandleRightArrowOnPress;
		this.rightArrow.ViewComponent.Position = new Vector2i(base.ViewComponent.Size.x - 30, this.rightArrow.ViewComponent.Position.y);
		this.textArea.ViewComponent.Size = new Vector2i(base.ViewComponent.Size.x - 80, this.textArea.ViewComponent.Size.y);
		this.clickable.IsNavigatable = (this.clickable.IsSnappable = true);
	}

	// Token: 0x06006ABA RID: 27322 RVA: 0x002B6DA5 File Offset: 0x002B4FA5
	[PublicizedFrom(EAccessModifier.Protected)]
	public void HandleSelectionChangedEvent()
	{
		if (this.OnSelectionChanged != null)
		{
			this.OnSelectionChanged(this, this.selectedIndex);
		}
	}

	// Token: 0x06006ABB RID: 27323 RVA: 0x002B6DC4 File Offset: 0x002B4FC4
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (base.xui.playerUI.CursorController.navigationTarget == this.clickable)
		{
			XUi.HandlePaging(base.xui, new Func<bool>(this.CycleRight), new Func<bool>(this.CycleLeft), false);
		}
		if (this.IsDirty)
		{
			if (this.items.Count > this.SelectedIndex)
			{
				this.lblSelected.Text = this.items[this.SelectedIndex];
			}
			else
			{
				this.lblSelected.Text = "";
			}
			this.IsDirty = false;
		}
	}

	// Token: 0x06006ABC RID: 27324 RVA: 0x002B6E69 File Offset: 0x002B5069
	[PublicizedFrom(EAccessModifier.Protected)]
	public void HandleLeftArrowOnPress(XUiController _sender, int _mouseButton)
	{
		this.CycleLeft();
	}

	// Token: 0x06006ABD RID: 27325 RVA: 0x002B6E72 File Offset: 0x002B5072
	[PublicizedFrom(EAccessModifier.Protected)]
	public void HandleRightArrowOnPress(XUiController _sender, int _mouseButton)
	{
		this.CycleRight();
	}

	// Token: 0x06006ABE RID: 27326 RVA: 0x002B6E7C File Offset: 0x002B507C
	public bool CycleLeft()
	{
		this.SelectedIndex -= this.Step;
		if (this.BoundsHandling == XUiC_OptionsSelector.BoundsHandlingTypes.Clamp)
		{
			if (this.SelectedIndex < 0)
			{
				this.SelectedIndex = 0;
				return false;
			}
		}
		else if (this.SelectedIndex < 0)
		{
			this.SelectedIndex = this.MaxCount - 1;
		}
		this.HandleSelectionChangedEvent();
		return true;
	}

	// Token: 0x06006ABF RID: 27327 RVA: 0x002B6ED4 File Offset: 0x002B50D4
	public bool CycleRight()
	{
		this.SelectedIndex += this.Step;
		if (this.BoundsHandling == XUiC_OptionsSelector.BoundsHandlingTypes.Clamp)
		{
			if (this.SelectedIndex >= this.MaxCount)
			{
				this.SelectedIndex = this.MaxCount - 1;
				return false;
			}
		}
		else if (this.SelectedIndex >= this.MaxCount)
		{
			this.SelectedIndex = 0;
		}
		this.HandleSelectionChangedEvent();
		return true;
	}

	// Token: 0x06006AC0 RID: 27328 RVA: 0x002B6F36 File Offset: 0x002B5136
	public void SetIndex(int newIndex)
	{
		if (this.SelectedIndex != newIndex)
		{
			this.SelectedIndex = newIndex;
			this.HandleSelectionChangedEvent();
		}
	}

	// Token: 0x06006AC1 RID: 27329 RVA: 0x002B6F4E File Offset: 0x002B514E
	public void ClearItems()
	{
		this.items.Clear();
		this.SelectedIndex = 0;
		this.MaxCount = 0;
		this.IsDirty = true;
	}

	// Token: 0x06006AC2 RID: 27330 RVA: 0x002B6F70 File Offset: 0x002B5170
	public int AddItem(string item)
	{
		this.items.Add(item);
		this.MaxCount = this.items.Count;
		this.IsDirty = true;
		return this.items.Count - 1;
	}

	// Token: 0x04005069 RID: 20585
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiController leftArrow;

	// Token: 0x0400506A RID: 20586
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiController rightArrow;

	// Token: 0x0400506B RID: 20587
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiController textArea;

	// Token: 0x0400506C RID: 20588
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiV_Label lblTitle;

	// Token: 0x0400506D RID: 20589
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiV_Label lblSelected;

	// Token: 0x0400506E RID: 20590
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiView clickable;

	// Token: 0x0400506F RID: 20591
	public XUiC_OptionsSelector.BoundsHandlingTypes BoundsHandling = XUiC_OptionsSelector.BoundsHandlingTypes.Wrap;

	// Token: 0x04005070 RID: 20592
	[PublicizedFrom(EAccessModifier.Private)]
	public int selectedIndex = 1;

	// Token: 0x04005071 RID: 20593
	public int MaxCount = 1;

	// Token: 0x04005072 RID: 20594
	public int Step = 1;

	// Token: 0x04005073 RID: 20595
	[PublicizedFrom(EAccessModifier.Private)]
	public List<string> items = new List<string>();

	// Token: 0x02000D58 RID: 3416
	public enum BoundsHandlingTypes
	{
		// Token: 0x04005075 RID: 20597
		Clamp,
		// Token: 0x04005076 RID: 20598
		Wrap
	}
}
