using System;
using UnityEngine.Scripting;

// Token: 0x02000C30 RID: 3120
[Preserve]
public class XUiC_CharacterCosmeticWindowGroup : XUiController
{
	// Token: 0x06006003 RID: 24579 RVA: 0x0026F168 File Offset: 0x0026D368
	public override void Init()
	{
		base.Init();
		if (base.GetChildByType<XUiC_WindowNonPagingHeader>() != null)
		{
			this.nonPagingHeaderWindow = base.GetChildByType<XUiC_WindowNonPagingHeader>();
		}
		if (base.GetChildByType<XUiC_CharacterCosmeticWindow>() != null)
		{
			this.characterCosmeticWindow = base.GetChildByType<XUiC_CharacterCosmeticWindow>();
		}
		if (base.GetChildByType<XUiC_CharacterCosmeticsListWindow>() != null)
		{
			this.characterCosmeticListWindow = base.GetChildByType<XUiC_CharacterCosmeticsListWindow>();
		}
	}

	// Token: 0x06006004 RID: 24580 RVA: 0x0026F1B7 File Offset: 0x0026D3B7
	public static void Open(XUi xui, EquipmentSlots slot)
	{
		GUIWindowManager windowManager = xui.playerUI.windowManager;
		windowManager.Open("cosmetics", true, false, true);
		((XUiC_CharacterCosmeticWindowGroup)((XUiWindowGroup)windowManager.GetWindow("cosmetics")).Controller).SetEquipSlot(slot);
	}

	// Token: 0x06006005 RID: 24581 RVA: 0x0026F1F4 File Offset: 0x0026D3F4
	public override void OnOpen()
	{
		base.OnOpen();
		if (this.nonPagingHeaderWindow != null)
		{
			this.nonPagingHeaderWindow.SetHeader(Localization.Get("Cosmetics", false));
		}
		if (base.xui.PlayerEquipment != null)
		{
			base.xui.PlayerEquipment.IsOpen = true;
		}
	}

	// Token: 0x06006006 RID: 24582 RVA: 0x0026F243 File Offset: 0x0026D443
	public void SetEquipSlot(EquipmentSlots equipSlot)
	{
		if (equipSlot != EquipmentSlots.Count)
		{
			this.characterCosmeticListWindow.SetCategory(equipSlot);
		}
	}

	// Token: 0x06006007 RID: 24583 RVA: 0x0026F255 File Offset: 0x0026D455
	public override void OnClose()
	{
		base.OnClose();
		if (base.xui.PlayerEquipment != null)
		{
			base.xui.PlayerEquipment.IsOpen = false;
		}
	}

	// Token: 0x06006008 RID: 24584 RVA: 0x0026F27B File Offset: 0x0026D47B
	public void ResetPreview()
	{
		this.characterCosmeticWindow.MakePreview();
	}

	// Token: 0x04004859 RID: 18521
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_WindowNonPagingHeader nonPagingHeaderWindow;

	// Token: 0x0400485A RID: 18522
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_CharacterCosmeticWindow characterCosmeticWindow;

	// Token: 0x0400485B RID: 18523
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_CharacterCosmeticsListWindow characterCosmeticListWindow;
}
