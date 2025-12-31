using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Quests.Requirements
{
	// Token: 0x020015B4 RID: 5556
	[Preserve]
	public class RequirementGroup : BaseRequirement
	{
		// Token: 0x1700130F RID: 4879
		// (get) Token: 0x0600AAB3 RID: 43699 RVA: 0x00433D52 File Offset: 0x00431F52
		// (set) Token: 0x0600AAB4 RID: 43700 RVA: 0x00433D5A File Offset: 0x00431F5A
		public RequirementGroup.GroupOperator Operator { get; set; }

		// Token: 0x0600AAB5 RID: 43701 RVA: 0x00433D64 File Offset: 0x00431F64
		public override void SetupRequirement()
		{
			this.Operator = EnumUtils.Parse<RequirementGroup.GroupOperator>(base.Value, false);
			for (int i = 0; i < this.ChildRequirements.Count; i++)
			{
				this.ChildRequirements[i].OwnerQuest = base.OwnerQuest;
				this.ChildRequirements[i].SetupRequirement();
			}
			if (string.IsNullOrEmpty(base.ID))
			{
				if (this.ChildRequirements.Count > 0)
				{
					base.Description = this.ChildRequirements[0].Description;
					return;
				}
			}
			else
			{
				base.Description = Localization.Get(base.ID, false);
			}
		}

		// Token: 0x0600AAB6 RID: 43702 RVA: 0x00433E08 File Offset: 0x00432008
		public override bool CheckRequirement()
		{
			if (!base.OwnerQuest.Active)
			{
				return true;
			}
			bool result = this.Operator == RequirementGroup.GroupOperator.AND;
			for (int i = 0; i < this.ChildRequirements.Count; i++)
			{
				bool flag = this.ChildRequirements[i].CheckRequirement();
				if (this.Operator == RequirementGroup.GroupOperator.AND)
				{
					if (!flag)
					{
						return false;
					}
				}
				else if (this.Operator == RequirementGroup.GroupOperator.OR && flag)
				{
					return true;
				}
			}
			return result;
		}

		// Token: 0x0600AAB7 RID: 43703 RVA: 0x00433E74 File Offset: 0x00432074
		public override BaseRequirement Clone()
		{
			RequirementGroup requirementGroup = new RequirementGroup();
			requirementGroup.ID = base.ID;
			requirementGroup.Value = base.Value;
			requirementGroup.Phase = base.Phase;
			for (int i = 0; i < this.ChildRequirements.Count; i++)
			{
				requirementGroup.ChildRequirements.Add(this.ChildRequirements[i].Clone());
			}
			return requirementGroup;
		}

		// Token: 0x0400854D RID: 34125
		public List<BaseRequirement> ChildRequirements = new List<BaseRequirement>();

		// Token: 0x020015B5 RID: 5557
		public enum GroupOperator
		{
			// Token: 0x04008550 RID: 34128
			AND,
			// Token: 0x04008551 RID: 34129
			OR
		}
	}
}
