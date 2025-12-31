using System;
using UnityEngine.Scripting;

// Token: 0x02000BD9 RID: 3033
[Preserve]
public class BaseItemActionEntry
{
	// Token: 0x170009A4 RID: 2468
	// (get) Token: 0x06005D50 RID: 23888 RVA: 0x0025DAE4 File Offset: 0x0025BCE4
	// (set) Token: 0x06005D51 RID: 23889 RVA: 0x0025DAEC File Offset: 0x0025BCEC
	public string ActionName { get; set; }

	// Token: 0x170009A5 RID: 2469
	// (get) Token: 0x06005D52 RID: 23890 RVA: 0x0025DAF5 File Offset: 0x0025BCF5
	// (set) Token: 0x06005D53 RID: 23891 RVA: 0x0025DAFD File Offset: 0x0025BCFD
	public string IconName { get; set; }

	// Token: 0x170009A6 RID: 2470
	// (get) Token: 0x06005D54 RID: 23892 RVA: 0x0025DB06 File Offset: 0x0025BD06
	// (set) Token: 0x06005D55 RID: 23893 RVA: 0x0025DB0E File Offset: 0x0025BD0E
	public bool Enabled { get; set; }

	// Token: 0x170009A7 RID: 2471
	// (get) Token: 0x06005D56 RID: 23894 RVA: 0x0025DB17 File Offset: 0x0025BD17
	// (set) Token: 0x06005D57 RID: 23895 RVA: 0x0025DB1F File Offset: 0x0025BD1F
	public string SoundName { get; set; }

	// Token: 0x170009A8 RID: 2472
	// (get) Token: 0x06005D58 RID: 23896 RVA: 0x0025DB28 File Offset: 0x0025BD28
	// (set) Token: 0x06005D59 RID: 23897 RVA: 0x0025DB30 File Offset: 0x0025BD30
	public string DisabledSound { get; set; }

	// Token: 0x170009A9 RID: 2473
	// (get) Token: 0x06005D5A RID: 23898 RVA: 0x0025DB39 File Offset: 0x0025BD39
	// (set) Token: 0x06005D5B RID: 23899 RVA: 0x0025DB41 File Offset: 0x0025BD41
	public XUiController ItemController { get; set; }

	// Token: 0x170009AA RID: 2474
	// (get) Token: 0x06005D5C RID: 23900 RVA: 0x0025DB4A File Offset: 0x0025BD4A
	// (set) Token: 0x06005D5D RID: 23901 RVA: 0x0025DB52 File Offset: 0x0025BD52
	public XUiC_ItemActionEntry ParentItem { get; set; }

	// Token: 0x170009AB RID: 2475
	// (get) Token: 0x06005D5E RID: 23902 RVA: 0x0025DB5B File Offset: 0x0025BD5B
	// (set) Token: 0x06005D5F RID: 23903 RVA: 0x0025DB63 File Offset: 0x0025BD63
	public XUiC_ItemActionList ParentActionList { get; set; }

	// Token: 0x170009AC RID: 2476
	// (get) Token: 0x06005D60 RID: 23904 RVA: 0x0025DB6C File Offset: 0x0025BD6C
	// (set) Token: 0x06005D61 RID: 23905 RVA: 0x0025DB74 File Offset: 0x0025BD74
	public BaseItemActionEntry.GamepadShortCut ShortCut { get; set; }

	// Token: 0x06005D62 RID: 23906 RVA: 0x0025DB80 File Offset: 0x0025BD80
	public BaseItemActionEntry(XUiController itemController, string actionName, string spriteName, BaseItemActionEntry.GamepadShortCut shortcut = BaseItemActionEntry.GamepadShortCut.None, string soundName = "crafting/craft_click_craft", string disabledSoundName = "ui/ui_denied")
	{
		this.ItemController = itemController;
		this.ActionName = Localization.Get(actionName, false);
		this.IconName = spriteName;
		this.SoundName = soundName;
		this.DisabledSound = disabledSoundName;
		this.Enabled = true;
		this.ShortCut = shortcut;
	}

	// Token: 0x06005D63 RID: 23907 RVA: 0x0025DBCD File Offset: 0x0025BDCD
	public BaseItemActionEntry(string actionName, string spriteName, XUiController itemController, BaseItemActionEntry.GamepadShortCut shortcut = BaseItemActionEntry.GamepadShortCut.None, string soundName = "crafting/craft_click_craft", string disabledSoundName = "ui/ui_denied")
	{
		this.ItemController = itemController;
		this.ActionName = actionName;
		this.IconName = spriteName;
		this.SoundName = soundName;
		this.DisabledSound = disabledSoundName;
		this.Enabled = true;
		this.ShortCut = shortcut;
	}

	// Token: 0x06005D64 RID: 23908 RVA: 0x0025DC09 File Offset: 0x0025BE09
	public virtual void RefreshEnabled()
	{
		if (this.ItemController is XUiC_ItemStack)
		{
			this.Enabled = !((XUiC_ItemStack)this.ItemController).StackLock;
		}
	}

	// Token: 0x06005D65 RID: 23909 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnActivated()
	{
	}

	// Token: 0x06005D66 RID: 23910 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnDisabledActivate()
	{
	}

	// Token: 0x06005D67 RID: 23911 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnTimerCompleted()
	{
	}

	// Token: 0x06005D68 RID: 23912 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void DisableEvents()
	{
	}

	// Token: 0x02000BDA RID: 3034
	public enum GamepadShortCut
	{
		// Token: 0x040046A5 RID: 18085
		DPadUp,
		// Token: 0x040046A6 RID: 18086
		DPadLeft,
		// Token: 0x040046A7 RID: 18087
		DPadRight,
		// Token: 0x040046A8 RID: 18088
		DPadDown,
		// Token: 0x040046A9 RID: 18089
		None,
		// Token: 0x040046AA RID: 18090
		Max
	}
}
