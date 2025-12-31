using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D1A RID: 3354
[Preserve]
public class XUiC_MapInvitesListEntry : XUiController
{
	// Token: 0x17000AA4 RID: 2724
	// (get) Token: 0x0600687C RID: 26748 RVA: 0x002A73E7 File Offset: 0x002A55E7
	// (set) Token: 0x0600687D RID: 26749 RVA: 0x002A73F0 File Offset: 0x002A55F0
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

	// Token: 0x17000AA5 RID: 2725
	// (get) Token: 0x0600687E RID: 26750 RVA: 0x002A7426 File Offset: 0x002A5626
	// (set) Token: 0x0600687F RID: 26751 RVA: 0x002A7430 File Offset: 0x002A5630
	public new bool Selected
	{
		get
		{
			return this.m_bSelected;
		}
		set
		{
			if (this.m_bSelected && !value && this.Waypoint != null && this.Waypoint.navObject != null && this.Waypoint.navObject.NavObjectClass.NavObjectClassName == "waypoint_invite")
			{
				NavObjectManager.Instance.UnRegisterNavObject(this.Waypoint.navObject);
			}
			if (!this.m_bSelected && value)
			{
				this.Waypoint.navObject = NavObjectManager.Instance.RegisterNavObject("waypoint_invite", this.Waypoint.pos.ToVector3(), this.Waypoint.icon, false, -1, null);
				this.Waypoint.navObject.IsActive = false;
				this.Waypoint.navObject.name = GeneratedTextManager.GetDisplayTextImmediately(this.Waypoint.name, false, GeneratedTextManager.TextFilteringMode.Filter, GeneratedTextManager.BbCodeSupportMode.SupportedAndAddEscapes);
				this.Waypoint.navObject.usingLocalizationId = this.Waypoint.bUsingLocalizationId;
			}
			this.m_bSelected = value;
			this.updateSelected(false);
		}
	}

	// Token: 0x06006880 RID: 26752 RVA: 0x002A7538 File Offset: 0x002A5738
	[PublicizedFrom(EAccessModifier.Private)]
	public int makeKey()
	{
		return int.MaxValue - this.Index;
	}

	// Token: 0x06006881 RID: 26753 RVA: 0x002A7548 File Offset: 0x002A5748
	public override void Init()
	{
		base.Init();
		this.waypointList = (XUiC_MapInvitesList)base.Parent.GetChildById("invitesList");
		this.Background = (XUiV_Sprite)base.GetChildById("Background").ViewComponent;
		this.Sprite = (XUiV_Sprite)base.GetChildById("Icon").ViewComponent;
		this.Name = (XUiV_Label)base.GetChildById("Name").ViewComponent;
		this.Distance = (XUiV_Label)base.GetChildById("Distance").ViewComponent;
		this.Background.Controller.OnHover += this.Controller_OnHover;
		this.Background.Controller.OnPress += this.Controller_OnPress;
		this.Background.IsSnappable = false;
	}

	// Token: 0x06006882 RID: 26754 RVA: 0x002A7626 File Offset: 0x002A5826
	[PublicizedFrom(EAccessModifier.Private)]
	public void Controller_OnHover(XUiController _sender, bool _isOver)
	{
		this.updateSelected(this.Waypoint != null && _isOver);
	}

	// Token: 0x06006883 RID: 26755 RVA: 0x002A763C File Offset: 0x002A583C
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
			if (base.Parent.Children[i] is XUiC_MapInvitesListEntry)
			{
				((XUiC_MapInvitesListEntry)base.Parent.Children[i]).Selected = false;
			}
		}
		this.waypointList.SelectedInvite = this.Waypoint;
		this.waypointList.SelectedInviteEntry = this;
		this.Selected = true;
	}

	// Token: 0x06006884 RID: 26756 RVA: 0x002A76CC File Offset: 0x002A58CC
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
				return;
			}
			if (_bHover)
			{
				background.Color = new Color32(96, 96, 96, byte.MaxValue);
				background.SpriteName = "menu_empty";
				return;
			}
			background.Color = new Color32(64, 64, 64, byte.MaxValue);
			background.SpriteName = "menu_empty";
		}
	}

	// Token: 0x04004ED8 RID: 20184
	public int Index;

	// Token: 0x04004ED9 RID: 20185
	public XUiV_Sprite Background;

	// Token: 0x04004EDA RID: 20186
	public XUiV_Sprite Sprite;

	// Token: 0x04004EDB RID: 20187
	public XUiV_Label Name;

	// Token: 0x04004EDC RID: 20188
	public XUiV_Label Distance;

	// Token: 0x04004EDD RID: 20189
	public Action RefreshNameAction;

	// Token: 0x04004EDE RID: 20190
	[PublicizedFrom(EAccessModifier.Private)]
	public Waypoint _waypoint;

	// Token: 0x04004EDF RID: 20191
	[PublicizedFrom(EAccessModifier.Private)]
	public bool m_bSelected;

	// Token: 0x04004EE0 RID: 20192
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_MapInvitesList waypointList;
}
