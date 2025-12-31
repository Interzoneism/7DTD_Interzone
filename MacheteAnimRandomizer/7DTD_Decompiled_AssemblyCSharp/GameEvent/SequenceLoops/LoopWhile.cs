using System;
using GameEvent.SequenceActions;
using UnityEngine.Scripting;

namespace GameEvent.SequenceLoops
{
	// Token: 0x02001635 RID: 5685
	[Preserve]
	public class LoopWhile : BaseLoop
	{
		// Token: 0x17001387 RID: 4999
		// (get) Token: 0x0600AE70 RID: 44656 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public override bool UseRequirements
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600AE71 RID: 44657 RVA: 0x004417B8 File Offset: 0x0043F9B8
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			if (!this.runLoop)
			{
				this.runLoop = this.CheckCondition();
			}
			if (this.runLoop)
			{
				if (base.HandleActions() == BaseAction.ActionCompleteStates.Complete)
				{
					this.CurrentPhase = 0;
					for (int i = 0; i < this.Actions.Count; i++)
					{
						this.Actions[i].Reset();
					}
					this.runLoop = false;
				}
				return BaseAction.ActionCompleteStates.InComplete;
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600AE72 RID: 44658 RVA: 0x00441824 File Offset: 0x0043FA24
		[PublicizedFrom(EAccessModifier.Private)]
		public bool CheckCondition()
		{
			if (this.Requirements != null)
			{
				LoopWhile.ConditionTypes conditionType = this.ConditionType;
				if (conditionType == LoopWhile.ConditionTypes.Any)
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
				if (conditionType == LoopWhile.ConditionTypes.All)
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

		// Token: 0x0600AE73 RID: 44659 RVA: 0x004418C4 File Offset: 0x0043FAC4
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseEnum<LoopWhile.ConditionTypes>(LoopWhile.PropConditionType, ref this.ConditionType);
		}

		// Token: 0x0600AE74 RID: 44660 RVA: 0x004418DE File Offset: 0x0043FADE
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new LoopWhile
			{
				ConditionType = this.ConditionType
			};
		}

		// Token: 0x0400874A RID: 34634
		public LoopWhile.ConditionTypes ConditionType = LoopWhile.ConditionTypes.All;

		// Token: 0x0400874B RID: 34635
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropConditionType = "condition_type";

		// Token: 0x0400874C RID: 34636
		[PublicizedFrom(EAccessModifier.Private)]
		public bool runLoop;

		// Token: 0x02001636 RID: 5686
		public enum ConditionTypes
		{
			// Token: 0x0400874E RID: 34638
			Any,
			// Token: 0x0400874F RID: 34639
			All
		}
	}
}
