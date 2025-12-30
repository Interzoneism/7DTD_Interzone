using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000C11 RID: 3089
[Preserve]
public class XUiC_BuffInfoWindow : XUiC_InfoWindow
{
	// Token: 0x170009CC RID: 2508
	// (get) Token: 0x06005ED4 RID: 24276 RVA: 0x0026728E File Offset: 0x0026548E
	// (set) Token: 0x06005ED5 RID: 24277 RVA: 0x00267298 File Offset: 0x00265498
	public EntityUINotification Notification
	{
		get
		{
			return this.notification;
		}
		set
		{
			this.overridenBuff = null;
			this.notification = value;
			this.IsDirty = true;
			this.buffName = ((this.notification != null && this.notification.Buff != null) ? Localization.Get(this.notification.Buff.BuffClass.Name, false) : "");
			if (value != null && value.Buff != null)
			{
				EntityPlayer entityPlayer = base.xui.playerUI.entityPlayer;
				for (int i = 0; i < entityPlayer.Buffs.ActiveBuffs.Count; i++)
				{
					if (!entityPlayer.Buffs.ActiveBuffs[i].BuffClass.Hidden && !entityPlayer.Buffs.ActiveBuffs[i].Paused)
					{
						this.overridenBuff = entityPlayer.Buffs.ActiveBuffs[i];
						return;
					}
				}
			}
		}
	}

	// Token: 0x06005ED6 RID: 24278 RVA: 0x00267380 File Offset: 0x00265580
	public override void Init()
	{
		base.Init();
		this.itemPreview = base.GetChildById("itemPreview");
		this.windowName = base.GetChildById("windowName");
		this.windowIcon = base.GetChildById("windowIcon");
		this.description = base.GetChildById("descriptionText");
		this.stats = base.GetChildById("statText");
		this.actionItemList = (XUiC_ItemActionList)base.GetChildById("itemActions");
		this.statButton = base.GetChildById("statButton");
		if (this.statButton != null)
		{
			this.statButton.OnPress += this.StatButton_OnPress;
		}
		this.descriptionButton = base.GetChildById("descriptionButton");
		if (this.descriptionButton != null)
		{
			this.descriptionButton.OnPress += this.DescriptionButton_OnPress;
		}
	}

	// Token: 0x06005ED7 RID: 24279 RVA: 0x0026745E File Offset: 0x0026565E
	[PublicizedFrom(EAccessModifier.Private)]
	public void DescriptionButton_OnPress(XUiController _sender, int _mouseButton)
	{
		((XUiV_Button)this.statButton.ViewComponent).Selected = false;
		((XUiV_Button)this.descriptionButton.ViewComponent).Selected = true;
		this.showStats = false;
		this.IsDirty = true;
	}

	// Token: 0x06005ED8 RID: 24280 RVA: 0x0026749A File Offset: 0x0026569A
	[PublicizedFrom(EAccessModifier.Private)]
	public void StatButton_OnPress(XUiController _sender, int _mouseButton)
	{
		((XUiV_Button)this.statButton.ViewComponent).Selected = true;
		((XUiV_Button)this.descriptionButton.ViewComponent).Selected = false;
		this.showStats = true;
		this.IsDirty = true;
	}

	// Token: 0x06005ED9 RID: 24281 RVA: 0x002674D6 File Offset: 0x002656D6
	public override void Deselect()
	{
		if (this.selectedEntry != null)
		{
			this.selectedEntry.Selected = false;
		}
	}

	// Token: 0x06005EDA RID: 24282 RVA: 0x002674EC File Offset: 0x002656EC
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty)
		{
			if (this.emptyInfoWindow == null)
			{
				this.emptyInfoWindow = (XUiC_InfoWindow)base.xui.FindWindowGroupByName("backpack").GetChildById("emptyInfoPanel");
			}
			if (this.itemInfoWindow == null)
			{
				this.itemInfoWindow = (XUiC_ItemInfoWindow)base.xui.FindWindowGroupByName("backpack").GetChildById("itemInfoPanel");
			}
			base.RefreshBindings(true);
			this.IsDirty = false;
		}
	}

	// Token: 0x06005EDB RID: 24283 RVA: 0x00267570 File Offset: 0x00265770
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		bool flag = this.notification != null;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(bindingName);
		if (num <= 2465532423U)
		{
			if (num <= 1988587286U)
			{
				if (num != 1569673958U)
				{
					if (num == 1988587286U)
					{
						if (bindingName == "buffstatus")
						{
							value = ((flag && this.notification.Buff.Paused) ? Localization.Get("TwitchCooldownStatus_Paused", false) : "");
							return true;
						}
					}
				}
				else if (bindingName == "buffdescription")
				{
					value = (flag ? this.notification.Buff.BuffClass.Description : "");
					return true;
				}
			}
			else if (num != 2030550547U)
			{
				if (num == 2465532423U)
				{
					if (bindingName == "buffname")
					{
						value = (flag ? Localization.Get(this.notification.Buff.BuffClass.LocalizedName, false) : "");
						return true;
					}
				}
			}
			else if (bindingName == "buffcolor")
			{
				Color32 color = flag ? this.notification.GetColor() : Color.white;
				value = string.Format("{0},{1},{2},{3}", new object[]
				{
					color.r,
					color.g,
					color.b,
					color.a
				});
				return true;
			}
		}
		else if (num <= 3154262838U)
		{
			if (num != 2929532957U)
			{
				if (num == 3154262838U)
				{
					if (bindingName == "showdescription")
					{
						value = (!this.showStats).ToString();
						return true;
					}
				}
			}
			else if (bindingName == "bufficon")
			{
				value = (flag ? this.notification.Icon : "");
				return true;
			}
		}
		else if (num != 3257770903U)
		{
			if (num == 4276755783U)
			{
				if (bindingName == "buffstats")
				{
					value = (flag ? XUiM_PlayerBuffs.GetInfoFromBuff(base.xui.playerUI.entityPlayer, this.notification, this.overridenBuff) : "");
					return true;
				}
			}
		}
		else if (bindingName == "showstats")
		{
			value = this.showStats.ToString();
			return true;
		}
		return false;
	}

	// Token: 0x06005EDC RID: 24284 RVA: 0x002677FC File Offset: 0x002659FC
	public void SetBuff(XUiC_ActiveBuffEntry buffEntry)
	{
		if (this.emptyInfoWindow == null)
		{
			this.emptyInfoWindow = (XUiC_InfoWindow)base.xui.FindWindowGroupByName("backpack").GetChildById("emptyInfoPanel");
		}
		if (this.emptyInfoWindow != null && buffEntry == null)
		{
			if (!this.itemInfoWindow.ViewComponent.IsVisible)
			{
				this.emptyInfoWindow.ViewComponent.IsVisible = true;
			}
			return;
		}
		this.selectedEntry = buffEntry;
		this.Notification = buffEntry.Notification;
		EntityUINotification entityUINotification = this.notification;
		this.actionItemList.SetCraftingActionList(XUiC_ItemActionList.ItemActionListTypes.Buff, buffEntry);
		if (this.selectedEntry != null)
		{
			base.RefreshBindings(this.IsDirty);
		}
		this.IsDirty = true;
	}

	// Token: 0x06005EDD RID: 24285 RVA: 0x002678A7 File Offset: 0x00265AA7
	public void SetBuffInfo(XUiC_ActiveBuffEntry buff)
	{
		if (buff != null)
		{
			base.ViewComponent.IsVisible = true;
		}
		this.SetBuff(buff);
	}

	// Token: 0x04004785 RID: 18309
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityUINotification notification;

	// Token: 0x04004786 RID: 18310
	[PublicizedFrom(EAccessModifier.Private)]
	public BuffValue overridenBuff;

	// Token: 0x04004787 RID: 18311
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ActiveBuffEntry selectedEntry;

	// Token: 0x04004788 RID: 18312
	[PublicizedFrom(EAccessModifier.Private)]
	public string buffName = "";

	// Token: 0x04004789 RID: 18313
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController itemPreview;

	// Token: 0x0400478A RID: 18314
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController windowName;

	// Token: 0x0400478B RID: 18315
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController windowIcon;

	// Token: 0x0400478C RID: 18316
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController description;

	// Token: 0x0400478D RID: 18317
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController stats;

	// Token: 0x0400478E RID: 18318
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController craftingTime;

	// Token: 0x0400478F RID: 18319
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ItemActionList actionItemList;

	// Token: 0x04004790 RID: 18320
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_InfoWindow emptyInfoWindow;

	// Token: 0x04004791 RID: 18321
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ItemInfoWindow itemInfoWindow;

	// Token: 0x04004792 RID: 18322
	[PublicizedFrom(EAccessModifier.Private)]
	public bool showStats;

	// Token: 0x04004793 RID: 18323
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController statButton;

	// Token: 0x04004794 RID: 18324
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController descriptionButton;

	// Token: 0x04004795 RID: 18325
	[PublicizedFrom(EAccessModifier.Private)]
	public Color32 valueColor = new Color32(222, 206, 163, byte.MaxValue);
}
