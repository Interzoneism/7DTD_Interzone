using System;
using System.Collections.Generic;

namespace Challenges
{
	// Token: 0x02001604 RID: 5636
	public class RequirementGroupPhase
	{
		// Token: 0x0600AD72 RID: 44402 RVA: 0x0043E556 File Offset: 0x0043C756
		public void AddChallengeObjective(BaseChallengeObjective obj)
		{
			this.RequirementObjectiveList.Add(obj);
		}

		// Token: 0x0600AD73 RID: 44403 RVA: 0x0043E564 File Offset: 0x0043C764
		public void AddHooks()
		{
			for (int i = 0; i < this.RequirementObjectiveList.Count; i++)
			{
				for (int j = 0; j < this.RequirementObjectiveList.Count; j++)
				{
					this.RequirementObjectiveList[j].HandleAddHooks();
				}
			}
		}

		// Token: 0x0600AD74 RID: 44404 RVA: 0x0043E5B0 File Offset: 0x0043C7B0
		public bool HandleCheckStatus()
		{
			bool result = false;
			for (int i = 0; i < this.RequirementObjectiveList.Count; i++)
			{
				if (!this.RequirementObjectiveList[i].HandleCheckStatus())
				{
					result = true;
				}
				this.RequirementObjectiveList[i].UpdateStatus();
			}
			return result;
		}

		// Token: 0x0600AD75 RID: 44405 RVA: 0x0043E5FC File Offset: 0x0043C7FC
		public void HandleRemoveHooks()
		{
			for (int i = 0; i < this.RequirementObjectiveList.Count; i++)
			{
				this.RequirementObjectiveList[i].HandleRemoveHooks();
			}
		}

		// Token: 0x0600AD76 RID: 44406 RVA: 0x0043E630 File Offset: 0x0043C830
		public void ResetComplete()
		{
			this.IsComplete = false;
			for (int i = 0; i < this.RequirementObjectiveList.Count; i++)
			{
				this.RequirementObjectiveList[i].ResetComplete();
			}
		}

		// Token: 0x0600AD77 RID: 44407 RVA: 0x0043E66C File Offset: 0x0043C86C
		public virtual void UpdateStatus()
		{
			for (int i = 0; i < this.RequirementObjectiveList.Count; i++)
			{
				this.RequirementObjectiveList[i].UpdateStatus();
			}
		}

		// Token: 0x0600AD78 RID: 44408 RVA: 0x0043E6A0 File Offset: 0x0043C8A0
		public void Clone(RequirementGroupPhase phase)
		{
			for (int i = 0; i < phase.RequirementObjectiveList.Count; i++)
			{
				BaseChallengeObjective item = phase.RequirementObjectiveList[i].Clone();
				if (this.RequirementObjectiveList == null)
				{
					this.RequirementObjectiveList = new List<BaseChallengeObjective>();
				}
				this.RequirementObjectiveList.Add(item);
			}
		}

		// Token: 0x0600AD79 RID: 44409 RVA: 0x0043E6F4 File Offset: 0x0043C8F4
		public RequirementGroupPhase Clone()
		{
			RequirementGroupPhase requirementGroupPhase = new RequirementGroupPhase();
			for (int i = 0; i < this.RequirementObjectiveList.Count; i++)
			{
				BaseChallengeObjective item = this.RequirementObjectiveList[i].Clone();
				if (this.RequirementObjectiveList == null)
				{
					this.RequirementObjectiveList = new List<BaseChallengeObjective>();
				}
				requirementGroupPhase.RequirementObjectiveList.Add(item);
			}
			return requirementGroupPhase;
		}

		// Token: 0x0600AD7A RID: 44410 RVA: 0x0043E750 File Offset: 0x0043C950
		public Recipe GetItemRecipe()
		{
			for (int i = 0; i < this.RequirementObjectiveList.Count; i++)
			{
				Recipe recipeItem = this.RequirementObjectiveList[i].GetRecipeItem();
				if (recipeItem != null)
				{
					return recipeItem;
				}
			}
			return null;
		}

		// Token: 0x040086BA RID: 34490
		public List<BaseChallengeObjective> RequirementObjectiveList = new List<BaseChallengeObjective>();

		// Token: 0x040086BB RID: 34491
		public bool IsComplete;
	}
}
