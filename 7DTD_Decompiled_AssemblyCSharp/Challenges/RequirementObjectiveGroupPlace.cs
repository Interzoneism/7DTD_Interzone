using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x02001609 RID: 5641
	[Preserve]
	public class RequirementObjectiveGroupPlace : BaseRequirementObjectiveGroup
	{
		// Token: 0x0600AD90 RID: 44432 RVA: 0x0043F20C File Offset: 0x0043D40C
		public RequirementObjectiveGroupPlace(string itemID)
		{
			this.ItemID = itemID;
		}

		// Token: 0x0600AD91 RID: 44433 RVA: 0x0043F228 File Offset: 0x0043D428
		public override void CreateRequirements()
		{
			if (this.PhaseList == null)
			{
				this.PhaseList = new List<RequirementGroupPhase>();
			}
			this.PhaseList.Add(this.AddIngredientGatheringReqs());
			RequirementGroupPhase requirementGroupPhase = new RequirementGroupPhase();
			ChallengeObjectiveCraft challengeObjectiveCraft = new ChallengeObjectiveCraft();
			challengeObjectiveCraft.Owner = this.Owner;
			challengeObjectiveCraft.SetupItem(this.ItemID);
			challengeObjectiveCraft.IsRequirement = true;
			challengeObjectiveCraft.MaxCount = 1;
			challengeObjectiveCraft.Init();
			requirementGroupPhase.AddChallengeObjective(challengeObjectiveCraft);
			this.PhaseList.Add(requirementGroupPhase);
			requirementGroupPhase = new RequirementGroupPhase();
			ChallengeObjectiveHold challengeObjectiveHold = new ChallengeObjectiveHold();
			challengeObjectiveHold.Owner = this.Owner;
			challengeObjectiveHold.itemClassID = this.ItemID;
			challengeObjectiveHold.IsRequirement = true;
			challengeObjectiveHold.MaxCount = 1;
			challengeObjectiveHold.Init();
			requirementGroupPhase.AddChallengeObjective(challengeObjectiveHold);
			this.PhaseList.Add(requirementGroupPhase);
		}

		// Token: 0x0600AD92 RID: 44434 RVA: 0x0043F2F0 File Offset: 0x0043D4F0
		[PublicizedFrom(EAccessModifier.Private)]
		public RequirementGroupPhase AddIngredientGatheringReqs()
		{
			Recipe recipe = CraftingManager.GetRecipe(this.ItemID);
			RequirementGroupPhase requirementGroupPhase = new RequirementGroupPhase();
			int craftingTier = recipe.GetOutputItemClass().HasQuality ? 1 : 0;
			for (int i = 0; i < recipe.ingredients.Count; i++)
			{
				int num = recipe.ingredients[i].count;
				if (recipe.UseIngredientModifier)
				{
					num = (int)EffectManager.GetValue(PassiveEffects.CraftingIngredientCount, null, (float)num, this.Owner.Owner.Player, recipe, FastTags<TagGroup.Global>.Parse(recipe.ingredients[i].itemValue.ItemClass.GetItemName()), true, true, true, true, true, craftingTier, true, false);
				}
				if (num != 0)
				{
					ChallengeObjectiveGatherIngredient challengeObjectiveGatherIngredient = new ChallengeObjectiveGatherIngredient();
					challengeObjectiveGatherIngredient.Owner = this.Owner;
					challengeObjectiveGatherIngredient.Parent = this;
					challengeObjectiveGatherIngredient.IsRequirement = true;
					challengeObjectiveGatherIngredient.itemRecipe = recipe;
					challengeObjectiveGatherIngredient.IngredientIndex = i;
					challengeObjectiveGatherIngredient.IngredientCount = num;
					challengeObjectiveGatherIngredient.NeededCount = 1;
					challengeObjectiveGatherIngredient.MaxCount = num;
					challengeObjectiveGatherIngredient.Init();
					requirementGroupPhase.AddChallengeObjective(challengeObjectiveGatherIngredient);
				}
			}
			return requirementGroupPhase;
		}

		// Token: 0x0600AD93 RID: 44435 RVA: 0x0043F404 File Offset: 0x0043D604
		public override bool HasPrerequisiteCondition()
		{
			EntityPlayerLocal player = this.Owner.Owner.Player;
			LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(this.Owner.Owner.Player);
			XUiM_PlayerInventory playerInventory = uiforPlayer.xui.PlayerInventory;
			ItemClass holdingItem = this.Owner.Owner.Player.inventory.holdingItem;
			return (!playerInventory.HasItem(ItemClass.GetItem(this.ItemID, false)) && !this.CheckDragDropItem(uiforPlayer.xui.dragAndDrop.CurrentStack, this.ItemID)) || holdingItem.Name != this.ItemID;
		}

		// Token: 0x0600AD94 RID: 44436 RVA: 0x0043F4A2 File Offset: 0x0043D6A2
		[PublicizedFrom(EAccessModifier.Private)]
		public bool CheckDragDropItem(ItemStack stack, string itemID)
		{
			return !stack.IsEmpty() && stack.itemValue.ItemClass.GetItemName() == itemID;
		}

		// Token: 0x0600AD95 RID: 44437 RVA: 0x0043F4C4 File Offset: 0x0043D6C4
		[PublicizedFrom(EAccessModifier.Protected)]
		public override bool CheckPhaseStatus(int index)
		{
			EntityPlayerLocal player = this.Owner.Owner.Player;
			LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(this.Owner.Owner.Player);
			XUiM_PlayerInventory playerInventory = LocalPlayerUI.GetUIForPlayer(player).xui.PlayerInventory;
			ItemClass holdingItem = this.Owner.Owner.Player.inventory.holdingItem;
			if (playerInventory.HasItem(ItemClass.GetItem(this.ItemID, false)) && holdingItem.Name == this.ItemID)
			{
				return false;
			}
			if (index > 1)
			{
				return index != 2 || ((playerInventory.HasItem(ItemClass.GetItem(this.ItemID, false)) || this.CheckDragDropItem(uiforPlayer.xui.dragAndDrop.CurrentStack, this.ItemID)) && holdingItem.Name != this.ItemID);
			}
			return !playerInventory.HasItem(ItemClass.GetItem(this.ItemID, false)) && !this.CheckDragDropItem(uiforPlayer.xui.dragAndDrop.CurrentStack, this.ItemID);
		}

		// Token: 0x0600AD96 RID: 44438 RVA: 0x0043F5D2 File Offset: 0x0043D7D2
		public override BaseRequirementObjectiveGroup Clone()
		{
			return new RequirementObjectiveGroupPlace(this.ItemID);
		}

		// Token: 0x040086C6 RID: 34502
		public string ItemID = "";
	}
}
