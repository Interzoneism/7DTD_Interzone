using System;
using UnityEngine.Scripting;

// Token: 0x0200091E RID: 2334
[Preserve]
public class RewardRecipe : BaseReward
{
	// Token: 0x0600459E RID: 17822 RVA: 0x001BD456 File Offset: 0x001BB656
	public override void SetupReward()
	{
		base.Description = Localization.Get("RewardRecipe_keyword", false);
		base.ValueText = base.ID;
		base.Icon = "ui_game_symbol_hammer";
	}

	// Token: 0x0600459F RID: 17823 RVA: 0x001BD480 File Offset: 0x001BB680
	public override void GiveReward(EntityPlayer player)
	{
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(player as EntityPlayerLocal);
		if (!XUiM_Recipes.GetRecipeIsUnlocked(uiforPlayer.xui, base.ID))
		{
			CraftingManager.UnlockRecipe(base.ID, uiforPlayer.entityPlayer);
		}
	}

	// Token: 0x060045A0 RID: 17824 RVA: 0x001BD4C0 File Offset: 0x001BB6C0
	public override BaseReward Clone()
	{
		RewardRecipe rewardRecipe = new RewardRecipe();
		base.CopyValues(rewardRecipe);
		return rewardRecipe;
	}

	// Token: 0x060045A1 RID: 17825 RVA: 0x001BD4DB File Offset: 0x001BB6DB
	public override void SetupGlobalRewardSettings()
	{
		CraftingManager.LockRecipe(base.ID, CraftingManager.RecipeLockTypes.Quest);
	}
}
