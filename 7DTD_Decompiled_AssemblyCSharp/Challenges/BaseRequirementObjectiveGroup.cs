using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x02001603 RID: 5635
	[Preserve]
	public class BaseRequirementObjectiveGroup
	{
		// Token: 0x17001385 RID: 4997
		// (get) Token: 0x0600AD63 RID: 44387 RVA: 0x0043E2B3 File Offset: 0x0043C4B3
		public int Count
		{
			get
			{
				if (this.PhaseList == null)
				{
					return 0;
				}
				return this.PhaseList[this.currentIndex].RequirementObjectiveList.Count;
			}
		}

		// Token: 0x0600AD64 RID: 44388 RVA: 0x00002914 File Offset: 0x00000B14
		public virtual void CreateRequirements()
		{
		}

		// Token: 0x17001386 RID: 4998
		// (get) Token: 0x0600AD65 RID: 44389 RVA: 0x0043E2DA File Offset: 0x0043C4DA
		public List<BaseChallengeObjective> CurrentObjectiveList
		{
			get
			{
				if (this.PhaseList == null)
				{
					return null;
				}
				return this.PhaseList[this.currentIndex].RequirementObjectiveList;
			}
		}

		// Token: 0x0600AD66 RID: 44390 RVA: 0x0043E2FC File Offset: 0x0043C4FC
		public void HandleAddHooks()
		{
			if (this.PhaseList.Count == 0)
			{
				this.CreateRequirements();
			}
			for (int i = 0; i < this.PhaseList.Count; i++)
			{
				this.PhaseList[i].AddHooks();
			}
			this.CheckPrerequisites();
		}

		// Token: 0x0600AD67 RID: 44391 RVA: 0x0043E34C File Offset: 0x0043C54C
		public void CheckPrerequisites()
		{
			if (this.PhaseList.Count == 0)
			{
				this.CreateRequirements();
			}
			this.NeedsPreRequisites = false;
			for (int i = 0; i < this.PhaseList.Count; i++)
			{
				if (this.PhaseList[i].HandleCheckStatus())
				{
					this.currentIndex = i;
					this.NeedsPreRequisites = true;
					return;
				}
			}
		}

		// Token: 0x0600AD68 RID: 44392 RVA: 0x0043E3AC File Offset: 0x0043C5AC
		public void HandleRemoveHooks()
		{
			for (int i = 0; i < this.PhaseList.Count; i++)
			{
				this.PhaseList[i].HandleRemoveHooks();
			}
		}

		// Token: 0x0600AD69 RID: 44393 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public virtual bool HasPrerequisiteCondition()
		{
			return false;
		}

		// Token: 0x0600AD6A RID: 44394 RVA: 0x0043E3E0 File Offset: 0x0043C5E0
		public void ResetObjectives()
		{
			if (this.PhaseList != null)
			{
				for (int i = 0; i < this.PhaseList.Count; i++)
				{
					this.PhaseList[i].ResetComplete();
				}
			}
		}

		// Token: 0x0600AD6B RID: 44395 RVA: 0x0043E41C File Offset: 0x0043C61C
		public void ClonePhases(BaseRequirementObjectiveGroup group)
		{
			if (group.PhaseList != null)
			{
				for (int i = 0; i < group.PhaseList.Count; i++)
				{
					RequirementGroupPhase item = group.PhaseList[i].Clone();
					this.PhaseList.Add(item);
				}
			}
		}

		// Token: 0x0600AD6C RID: 44396 RVA: 0x0043E468 File Offset: 0x0043C668
		public virtual bool HandleCheckStatus()
		{
			if (this.PhaseList.Count == 0)
			{
				this.CreateRequirements();
			}
			this.ResetObjectives();
			for (int i = 0; i < this.PhaseList.Count; i++)
			{
				if (this.CheckPhaseStatus(i) && this.PhaseList[i].HandleCheckStatus())
				{
					this.currentIndex = i;
					this.NeedsPreRequisites = true;
					return true;
				}
				this.PhaseList[i].IsComplete = true;
			}
			return false;
		}

		// Token: 0x0600AD6D RID: 44397 RVA: 0x000197A5 File Offset: 0x000179A5
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual bool CheckPhaseStatus(int index)
		{
			return true;
		}

		// Token: 0x0600AD6E RID: 44398 RVA: 0x0043E4E4 File Offset: 0x0043C6E4
		public virtual void UpdateStatus()
		{
			for (int i = 0; i < this.PhaseList.Count; i++)
			{
				if (!this.PhaseList[i].IsComplete)
				{
					this.PhaseList[i].UpdateStatus();
				}
			}
		}

		// Token: 0x0600AD6F RID: 44399 RVA: 0x00019766 File Offset: 0x00017966
		public virtual BaseRequirementObjectiveGroup Clone()
		{
			return null;
		}

		// Token: 0x0600AD70 RID: 44400 RVA: 0x0043E52B File Offset: 0x0043C72B
		public Recipe GetItemRecipe()
		{
			return this.PhaseList[this.currentIndex].GetItemRecipe();
		}

		// Token: 0x040086B6 RID: 34486
		public Challenge Owner;

		// Token: 0x040086B7 RID: 34487
		public List<RequirementGroupPhase> PhaseList = new List<RequirementGroupPhase>();

		// Token: 0x040086B8 RID: 34488
		[PublicizedFrom(EAccessModifier.Protected)]
		public int currentIndex;

		// Token: 0x040086B9 RID: 34489
		public bool NeedsPreRequisites;
	}
}
