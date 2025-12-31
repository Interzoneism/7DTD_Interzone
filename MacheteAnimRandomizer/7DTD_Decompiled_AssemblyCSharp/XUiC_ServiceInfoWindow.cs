using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000E20 RID: 3616
[Preserve]
public class XUiC_ServiceInfoWindow : XUiC_InfoWindow
{
	// Token: 0x06007139 RID: 28985 RVA: 0x002E234C File Offset: 0x002E054C
	public override void Init()
	{
		base.Init();
		this.servicePreview = base.GetChildById("servicePreview");
		this.windowName = base.GetChildById("windowName");
		this.windowIcon = base.GetChildById("windowIcon");
		this.description = base.GetChildById("descriptionText");
		this.stats = base.GetChildById("statText");
		this.mainActionItemList = (XUiC_ItemActionList)base.GetChildById("itemActions");
	}

	// Token: 0x0600713A RID: 28986 RVA: 0x00002914 File Offset: 0x00000B14
	public override void Deselect()
	{
	}

	// Token: 0x0600713B RID: 28987 RVA: 0x002E23CC File Offset: 0x002E05CC
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty && base.ViewComponent.IsVisible)
		{
			if (this.emptyInfoWindow == null)
			{
				this.emptyInfoWindow = (XUiC_InfoWindow)base.xui.FindWindowGroupByName("backpack").GetChildById("emptyInfoPanel");
			}
			this.IsDirty = false;
		}
	}

	// Token: 0x0600713C RID: 28988 RVA: 0x002E242C File Offset: 0x002E062C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(bindingName);
		if (num <= 2353034265U)
		{
			if (num <= 1380912366U)
			{
				if (num != 165025526U)
				{
					if (num == 1380912366U)
					{
						if (bindingName == "servicedescription")
						{
							value = ((this.service != null) ? this.service.Description : "");
							return true;
						}
					}
				}
				else if (bindingName == "servicegroupicon")
				{
					value = ((this.service != null) ? this.service.Icon : "");
					return true;
				}
			}
			else if (num != 2341642767U)
			{
				if (num == 2353034265U)
				{
					if (bindingName == "servicecost")
					{
						value = ((this.service != null) ? this.servicecostFormatter.Format(this.service.Price) : "");
						return true;
					}
				}
			}
			else if (bindingName == "servicestats")
			{
				value = ((this.service != null) ? this.stat1 : "");
				return true;
			}
		}
		else if (num <= 2418997840U)
		{
			if (num != 2390918988U)
			{
				if (num == 2418997840U)
				{
					if (bindingName == "pricelabel")
					{
						value = Localization.Get("xuiCost", false);
						return true;
					}
				}
			}
			else if (bindingName == "serviceicontint")
			{
				Color32 v = Color.white;
				value = this.serviceicontintcolorFormatter.Format(v);
				return true;
			}
		}
		else if (num != 3116710815U)
		{
			if (num == 3397569669U)
			{
				if (bindingName == "serviceicon")
				{
					value = ((this.service != null) ? this.service.Icon : "");
					return true;
				}
			}
		}
		else if (bindingName == "servicename")
		{
			value = ((this.service != null) ? this.service.Name : "");
			return true;
		}
		return false;
	}

	// Token: 0x0600713D RID: 28989 RVA: 0x002E2644 File Offset: 0x002E0844
	public void SetInfo(InGameService _service, XUiController controller)
	{
		this.service = _service;
		if (this.service == null)
		{
			if (this.emptyInfoWindow == null)
			{
				this.emptyInfoWindow = (XUiC_InfoWindow)base.xui.FindWindowGroupByName("backpack").GetChildById("emptyInfoPanel");
			}
			this.emptyInfoWindow.ViewComponent.IsVisible = true;
			return;
		}
		base.ViewComponent.IsVisible = true;
		if (this.servicePreview == null)
		{
			return;
		}
		string newValue = Utils.ColorToHex(this.valueColor);
		this.stat1 = XUiM_InGameService.GetServiceStats(base.xui, this.service).Replace("REPLACE_COLOR", newValue);
		this.mainActionItemList.SetServiceActionList(this.service, controller);
		base.RefreshBindings(false);
	}

	// Token: 0x0600713E RID: 28990 RVA: 0x002E26FB File Offset: 0x002E08FB
	public override void OnVisibilityChanged(bool _isVisible)
	{
		base.OnVisibilityChanged(_isVisible);
		if (this.service != null)
		{
			this.service.VisibleChangedHandler(_isVisible);
		}
	}

	// Token: 0x0600713F RID: 28991 RVA: 0x002E2720 File Offset: 0x002E0920
	public override void OnOpen()
	{
		base.OnOpen();
		if (this.service == null)
		{
			if (this.emptyInfoWindow == null)
			{
				this.emptyInfoWindow = (XUiC_InfoWindow)base.xui.FindWindowGroupByName("backpack").GetChildById("emptyInfoPanel");
			}
			this.emptyInfoWindow.ViewComponent.IsVisible = true;
			return;
		}
	}

	// Token: 0x06007140 RID: 28992 RVA: 0x002E277A File Offset: 0x002E097A
	public override void OnClose()
	{
		base.OnClose();
		this.service = null;
	}

	// Token: 0x04005611 RID: 22033
	[PublicizedFrom(EAccessModifier.Private)]
	public InGameService service;

	// Token: 0x04005612 RID: 22034
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController servicePreview;

	// Token: 0x04005613 RID: 22035
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController windowName;

	// Token: 0x04005614 RID: 22036
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController windowIcon;

	// Token: 0x04005615 RID: 22037
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController description;

	// Token: 0x04005616 RID: 22038
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController stats;

	// Token: 0x04005617 RID: 22039
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ItemActionList mainActionItemList;

	// Token: 0x04005618 RID: 22040
	[PublicizedFrom(EAccessModifier.Private)]
	public Color32 valueColor = new Color32(222, 206, 163, byte.MaxValue);

	// Token: 0x04005619 RID: 22041
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_InfoWindow emptyInfoWindow;

	// Token: 0x0400561A RID: 22042
	[PublicizedFrom(EAccessModifier.Private)]
	public string stat1 = "";

	// Token: 0x0400561B RID: 22043
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt servicecostFormatter = new CachedStringFormatterInt();

	// Token: 0x0400561C RID: 22044
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor serviceicontintcolorFormatter = new CachedStringFormatterXuiRgbaColor();
}
