using System;
using GameEvent.SequenceActions;
using UnityEngine.Scripting;

namespace GameEvent.SequenceDecisions
{
	// Token: 0x0200163D RID: 5693
	[Preserve]
	public class DecisionIf : BaseDecision
	{
		// Token: 0x0600AE92 RID: 44690 RVA: 0x00442B51 File Offset: 0x00440D51
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			if (!this.runActions)
			{
				this.runActions = this.CheckCondition();
			}
			if (!this.runActions)
			{
				return BaseAction.ActionCompleteStates.Complete;
			}
			if (base.HandleActions() == BaseAction.ActionCompleteStates.Complete)
			{
				this.runActions = false;
				return BaseAction.ActionCompleteStates.Complete;
			}
			return BaseAction.ActionCompleteStates.InComplete;
		}

		// Token: 0x0600AE93 RID: 44691 RVA: 0x00442B84 File Offset: 0x00440D84
		[PublicizedFrom(EAccessModifier.Private)]
		public bool CheckCondition()
		{
			if (this.Requirements != null)
			{
				DecisionIf.ConditionTypes conditionType = this.ConditionType;
				if (conditionType == DecisionIf.ConditionTypes.Any)
				{
					bool result = false;
					for (int i = 0; i < this.Requirements.Count; i++)
					{
						if (this.Requirements[i].CanPerform(base.Owner.Target))
						{
							result = true;
							break;
						}
					}
					return result;
				}
				if (conditionType == DecisionIf.ConditionTypes.All)
				{
					for (int j = 0; j < this.Requirements.Count; j++)
					{
						if (!this.Requirements[j].CanPerform(base.Owner.Target))
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		// Token: 0x0600AE94 RID: 44692 RVA: 0x00442C24 File Offset: 0x00440E24
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseEnum<DecisionIf.ConditionTypes>(DecisionIf.PropConditionType, ref this.ConditionType);
		}

		// Token: 0x0600AE95 RID: 44693 RVA: 0x00442C3E File Offset: 0x00440E3E
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new DecisionIf
			{
				ConditionType = this.ConditionType
			};
		}

		// Token: 0x04008776 RID: 34678
		public DecisionIf.ConditionTypes ConditionType = DecisionIf.ConditionTypes.All;

		// Token: 0x04008777 RID: 34679
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropConditionType = "condition_type";

		// Token: 0x04008778 RID: 34680
		[PublicizedFrom(EAccessModifier.Private)]
		public bool runActions;

		// Token: 0x0200163E RID: 5694
		public enum ConditionTypes
		{
			// Token: 0x0400877A RID: 34682
			Any,
			// Token: 0x0400877B RID: 34683
			All
		}
	}
}
