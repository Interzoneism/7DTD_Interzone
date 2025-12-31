using System;
using UnityEngine.Scripting;

// Token: 0x02000BEC RID: 3052
[Preserve]
public class ItemActionEntryShowChallenge : BaseItemActionEntry
{
	// Token: 0x06005DA3 RID: 23971 RVA: 0x002611D4 File Offset: 0x0025F3D4
	public ItemActionEntryShowChallenge(XUiController controller, RecipeUnlockData unlockData) : base(controller, "Challenge", "ui_game_symbol_challenge", BaseItemActionEntry.GamepadShortCut.DPadUp, "crafting/craft_click_craft", "ui/ui_denied")
	{
		this.UnlockData = unlockData;
		base.ActionName = Localization.Get("challenge", false);
		base.IconName = "ui_game_symbol_challenge";
	}

	// Token: 0x06005DA4 RID: 23972 RVA: 0x0025F7CE File Offset: 0x0025D9CE
	public override void RefreshEnabled()
	{
		base.Enabled = true;
	}

	// Token: 0x06005DA5 RID: 23973 RVA: 0x00261220 File Offset: 0x0025F420
	public override void OnActivated()
	{
		XUi xui = base.ItemController.xui;
		xui.playerUI.windowManager.CloseIfOpen("looting");
		XUiC_WindowSelector.OpenSelectorAndWindow(xui.playerUI.entityPlayer, "challenges");
		XUiC_ChallengeEntryListWindow childByType = xui.GetChildByType<XUiC_ChallengeEntryListWindow>();
		if (childByType != null)
		{
			childByType.SetSelectedByUnlockData(this.UnlockData);
		}
	}

	// Token: 0x040046D5 RID: 18133
	public RecipeUnlockData UnlockData;
}
