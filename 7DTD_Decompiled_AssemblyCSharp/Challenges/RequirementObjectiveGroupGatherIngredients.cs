using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x02001607 RID: 5639
	[Preserve]
	public class RequirementObjectiveGroupGatherIngredients : BaseRequirementObjectiveGroup
	{
		// Token: 0x0600AD88 RID: 44424 RVA: 0x0043EF60 File Offset: 0x0043D160
		public RequirementObjectiveGroupGatherIngredients(string itemID)
		{
			this.ItemID = itemID;
			this.itemRecipe = CraftingManager.GetRecipe(itemID);
		}

		// Token: 0x0600AD89 RID: 44425 RVA: 0x0043EF88 File Offset: 0x0043D188
		public override void CreateRequirements()
		{
			if (this.PhaseList == null)
			{
				this.PhaseList = new List<RequirementGroupPhase>();
			}
			RequirementGroupPhase requirementGroupPhase = new RequirementGroupPhase();
			int craftingTier = this.itemRecipe.GetOutputItemClass().HasQuality ? 1 : 0;
			for (int i = 0; i < this.itemRecipe.ingredients.Count; i++)
			{
				int num = this.itemRecipe.ingredients[i].count;
				if (this.itemRecipe.UseIngredientModifier)
				{
					num = (int)EffectManager.GetValue(PassiveEffects.CraftingIngredientCount, null, (float)num, this.Owner.Owner.Player, this.itemRecipe, FastTags<TagGroup.Global>.Parse(this.itemRecipe.ingredients[i].itemValue.ItemClass.GetItemName()), true, true, true, true, true, craftingTier, true, false);
				}
				if (num != 0)
				{
					ChallengeObjectiveGatherIngredient challengeObjectiveGatherIngredient = new ChallengeObjectiveGatherIngredient();
					challengeObjectiveGatherIngredient.Parent = this;
					challengeObjectiveGatherIngredient.Owner = this.Owner;
					challengeObjectiveGatherIngredient.IsRequirement = true;
					challengeObjectiveGatherIngredient.itemRecipe = this.itemRecipe;
					challengeObjectiveGatherIngredient.IngredientIndex = i;
					challengeObjectiveGatherIngredient.IngredientCount = num;
					challengeObjectiveGatherIngredient.NeededCount = ((this.CraftObj == null) ? 1 : this.CraftObj.MaxCount);
					challengeObjectiveGatherIngredient.MaxCount = num * challengeObjectiveGatherIngredient.NeededCount;
					challengeObjectiveGatherIngredient.Init();
					requirementGroupPhase.AddChallengeObjective(challengeObjectiveGatherIngredient);
				}
			}
			this.PhaseList.Add(requirementGroupPhase);
		}

		// Token: 0x0600AD8A RID: 44426 RVA: 0x000197A5 File Offset: 0x000179A5
		public override bool HasPrerequisiteCondition()
		{
			return true;
		}

		// Token: 0x0600AD8B RID: 44427 RVA: 0x0043F0E8 File Offset: 0x0043D2E8
		public override BaseRequirementObjectiveGroup Clone()
		{
			return new RequirementObjectiveGroupGatherIngredients(this.ItemID);
		}

		// Token: 0x040086C2 RID: 34498
		public string ItemID = "";

		// Token: 0x040086C3 RID: 34499
		[PublicizedFrom(EAccessModifier.Private)]
		public Recipe itemRecipe;

		// Token: 0x040086C4 RID: 34500
		public ChallengeObjectiveCraft CraftObj;
	}
}
