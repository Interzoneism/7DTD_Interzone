using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D25 RID: 3365
[Preserve]
public class XUiC_MapWaypointListEntry : XUiController
{
	// Token: 0x17000AA8 RID: 2728
	// (get) Token: 0x060068BE RID: 26814 RVA: 0x002A8CD3 File Offset: 0x002A6ED3
	// (set) Token: 0x060068BF RID: 26815 RVA: 0x002A8CDC File Offset: 0x002A6EDC
	public Waypoint Waypoint
	{
		get
		{
			return this._waypoint;
		}
		set
		{
			this._waypoint = value;
			this.Background.IsNavigatable = (this.Background.IsSnappable = (this._waypoint != null));
		}
	}

	// Token: 0x17000AA9 RID: 2729
	// (get) Token: 0x060068C0 RID: 26816 RVA: 0x002A8D12 File Offset: 0x002A6F12
	// (set) Token: 0x060068C1 RID: 26817 RVA: 0x002A8D1A File Offset: 0x002A6F1A
	public new bool Selected
	{
		get
		{
			return this.m_bSelected;
		}
		set
		{
			this.m_bSelected = value;
			this.updateSelected(false);
		}
	}

	// Token: 0x060068C2 RID: 26818 RVA: 0x002A8D2C File Offset: 0x002A6F2C
	public override void Init()
	{
		base.Init();
		this.waypointList = (XUiC_MapWaypointList)base.Parent.GetChildById("waypointList");
		this.Background = (XUiV_Sprite)base.GetChildById("Background").ViewComponent;
		this.Sprite = (XUiV_Sprite)base.GetChildById("Icon").ViewComponent;
		this.Tracking = (XUiV_Sprite)base.GetChildById("Tracking").ViewComponent;
		this.Name = (XUiV_Label)base.GetChildById("Name").ViewComponent;
		this.Distance = (XUiV_Label)base.GetChildById("Distance").ViewComponent;
		this.Background.Controller.OnHover += this.Controller_OnHover;
		this.Background.Controller.OnPress += this.Controller_OnPress;
		this.Background.Controller.OnScroll += this.Controller_OnScroll;
		this.Background.IsSnappable = false;
	}

	// Token: 0x060068C3 RID: 26819 RVA: 0x002A8E41 File Offset: 0x002A7041
	[PublicizedFrom(EAccessModifier.Private)]
	public void Controller_OnScroll(XUiController _sender, float _delta)
	{
		if (_delta > 0f)
		{
			XUiC_Paging pager = this.waypointList.pager;
			if (pager == null)
			{
				return;
			}
			pager.PageDown();
			return;
		}
		else
		{
			XUiC_Paging pager2 = this.waypointList.pager;
			if (pager2 == null)
			{
				return;
			}
			pager2.PageUp();
			return;
		}
	}

	// Token: 0x060068C4 RID: 26820 RVA: 0x002A8E78 File Offset: 0x002A7078
	[PublicizedFrom(EAccessModifier.Private)]
	public void Controller_OnHover(XUiController _sender, bool _isOver)
	{
		this.updateSelected(this.Waypoint != null && _isOver);
	}

	// Token: 0x060068C5 RID: 26821 RVA: 0x002A8E8C File Offset: 0x002A708C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Controller_OnPress(XUiController _sender, int _mouseButton)
	{
		if (this.Waypoint == null)
		{
			this.Selected = false;
			return;
		}
		for (int i = 0; i < base.Parent.Children.Count; i++)
		{
			if (base.Parent.Children[i] is XUiC_MapWaypointListEntry)
			{
				((XUiC_MapWaypointListEntry)base.Parent.Children[i]).Selected = false;
			}
		}
		this.waypointList.SelectedWaypoint = this.Waypoint;
		this.waypointList.SelectedWaypointEntry = this;
		this.Selected = true;
		if (InputUtils.ShiftKeyPressed && this.Waypoint != null)
		{
			this.waypointList.TrackedWaypoint = this.Waypoint;
			this.Waypoint.hiddenOnCompass = false;
			this.Waypoint.navObject.hiddenOnCompass = false;
			this.waypointList.UpdateWaypointsList(this.waypointList.SelectedWaypointEntry.Waypoint);
		}
	}

	// Token: 0x060068C6 RID: 26822 RVA: 0x002A8F74 File Offset: 0x002A7174
	public override void OnOpen()
	{
		base.OnOpen();
		this.Tracking.IsVisible = (this.Waypoint != null && this.Waypoint.bTracked);
	}

	// Token: 0x060068C7 RID: 26823 RVA: 0x002A8FA0 File Offset: 0x002A71A0
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateSelected(bool _bHover)
	{
		XUiV_Sprite background = this.Background;
		if (background != null)
		{
			if (this.m_bSelected)
			{
				background.Color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
				background.SpriteName = "ui_game_select_row";
			}
			else if (_bHover)
			{
				background.Color = new Color32(96, 96, 96, byte.MaxValue);
				background.SpriteName = "menu_empty";
			}
			else
			{
				background.Color = new Color32(64, 64, 64, byte.MaxValue);
				background.SpriteName = "menu_empty";
			}
		}
		this.Tracking.IsVisible = (this.Waypoint != null && this.Waypoint.bTracked);
	}

	// Token: 0x04004EFD RID: 20221
	public int Index;

	// Token: 0x04004EFE RID: 20222
	public XUiV_Sprite Background;

	// Token: 0x04004EFF RID: 20223
	public XUiV_Sprite Sprite;

	// Token: 0x04004F00 RID: 20224
	public XUiV_Label Name;

	// Token: 0x04004F01 RID: 20225
	public XUiV_Label Distance;

	// Token: 0x04004F02 RID: 20226
	public XUiV_Sprite Tracking;

	// Token: 0x04004F03 RID: 20227
	[PublicizedFrom(EAccessModifier.Private)]
	public Waypoint _waypoint;

	// Token: 0x04004F04 RID: 20228
	[PublicizedFrom(EAccessModifier.Private)]
	public bool m_bSelected;

	// Token: 0x04004F05 RID: 20229
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_MapWaypointList waypointList;
}
