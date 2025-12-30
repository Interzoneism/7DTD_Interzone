using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000C07 RID: 3079
[Preserve]
public class XUiC_ActiveBuffEntry : XUiC_SelectableEntry
{
	// Token: 0x170009BB RID: 2491
	// (get) Token: 0x06005E50 RID: 24144 RVA: 0x00263F0E File Offset: 0x0026210E
	// (set) Token: 0x06005E51 RID: 24145 RVA: 0x00263F18 File Offset: 0x00262118
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
			this.isDirty = true;
			EntityUINotification entityUINotification = this.notification;
			this.buffName = ((((entityUINotification != null) ? entityUINotification.Buff : null) != null) ? this.notification.Buff.BuffClass.LocalizedName : "");
			base.ViewComponent.Enabled = (value != null);
			if (((value != null) ? value.Buff : null) != null)
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

	// Token: 0x170009BC RID: 2492
	// (get) Token: 0x06005E52 RID: 24146 RVA: 0x00264006 File Offset: 0x00262206
	// (set) Token: 0x06005E53 RID: 24147 RVA: 0x0026400E File Offset: 0x0026220E
	public XUiC_BuffInfoWindow InfoWindow { get; set; }

	// Token: 0x06005E54 RID: 24148 RVA: 0x00264018 File Offset: 0x00262218
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void SelectedChanged(bool isSelected)
	{
		if (isSelected)
		{
			this.InfoWindow.SetBuffInfo(this);
		}
		if (this.background != null)
		{
			this.background.Color = (isSelected ? new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue) : new Color32(64, 64, 64, byte.MaxValue));
			this.background.SpriteName = (isSelected ? "ui_game_select_row" : "menu_empty");
		}
	}

	// Token: 0x06005E55 RID: 24149 RVA: 0x00264094 File Offset: 0x00262294
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		bool flag = this.notification != null;
		if (bindingName == "buffname")
		{
			value = (flag ? this.buffName : "");
			return true;
		}
		if (bindingName == "buffdisplayinfo")
		{
			value = (flag ? XUiM_PlayerBuffs.GetBuffDisplayInfo(this.notification, this.overridenBuff) : "");
			return true;
		}
		if (bindingName == "bufficon")
		{
			value = (flag ? this.notification.Icon : "");
			return true;
		}
		if (bindingName == "buffcolor")
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
		if (!(bindingName == "fontcolor"))
		{
			return false;
		}
		value = ((flag && this.notification.Buff.Paused) ? "128,128,128,255" : "255,255,255,255");
		return true;
	}

	// Token: 0x06005E56 RID: 24150 RVA: 0x002641CD File Offset: 0x002623CD
	public override void Init()
	{
		base.Init();
		this.background = (XUiV_Sprite)base.GetChildById("background").ViewComponent;
		base.OnScroll += this.HandleOnScroll;
		this.isDirty = true;
	}

	// Token: 0x06005E57 RID: 24151 RVA: 0x0026420C File Offset: 0x0026240C
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleOnScroll(XUiController _sender, float _delta)
	{
		if (_delta > 0f)
		{
			XUiC_Paging pager = ((XUiC_ActiveBuffList)base.Parent).pager;
			if (pager == null)
			{
				return;
			}
			pager.PageDown();
			return;
		}
		else
		{
			XUiC_Paging pager2 = ((XUiC_ActiveBuffList)base.Parent).pager;
			if (pager2 == null)
			{
				return;
			}
			pager2.PageUp();
			return;
		}
	}

	// Token: 0x06005E58 RID: 24152 RVA: 0x00264258 File Offset: 0x00262458
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnHovered(bool _isOver)
	{
		if (this.background != null && !base.Selected)
		{
			if (_isOver && this.notification != null)
			{
				this.background.Color = new Color32(96, 96, 96, byte.MaxValue);
			}
			else
			{
				this.background.Color = new Color32(64, 64, 64, byte.MaxValue);
			}
		}
		base.OnHovered(_isOver);
	}

	// Token: 0x06005E59 RID: 24153 RVA: 0x002642C9 File Offset: 0x002624C9
	public override void Update(float _dt)
	{
		base.RefreshBindings(this.isDirty);
		this.isDirty = false;
		base.Update(_dt);
	}

	// Token: 0x06005E5A RID: 24154 RVA: 0x002642E5 File Offset: 0x002624E5
	public void Refresh()
	{
		this.isDirty = true;
	}

	// Token: 0x04004729 RID: 18217
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;

	// Token: 0x0400472A RID: 18218
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite background;

	// Token: 0x0400472B RID: 18219
	[PublicizedFrom(EAccessModifier.Private)]
	public string buffName = "";

	// Token: 0x0400472C RID: 18220
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityUINotification notification;

	// Token: 0x0400472D RID: 18221
	[PublicizedFrom(EAccessModifier.Private)]
	public BuffValue overridenBuff;
}
