using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016ED RID: 5869
	[Preserve]
	public class BaseWait : BaseAction
	{
		// Token: 0x170013AC RID: 5036
		// (get) Token: 0x0600B1B9 RID: 45497 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public override bool UseRequirements
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600B1BA RID: 45498 RVA: 0x00454308 File Offset: 0x00452508
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			if (this.Requirements != null)
			{
				BaseWait.ConditionTypes conditionType = this.ConditionType;
				if (conditionType == BaseWait.ConditionTypes.Any)
				{
					for (int i = 0; i < this.Requirements.Count; i++)
					{
						if (this.Requirements[i].CanPerform(base.Owner.Target))
						{
							return BaseAction.ActionCompleteStates.InComplete;
						}
					}
					return BaseAction.ActionCompleteStates.Complete;
				}
				if (conditionType == BaseWait.ConditionTypes.All)
				{
					for (int j = 0; j < this.Requirements.Count; j++)
					{
						if (!this.Requirements[j].CanPerform(base.Owner.Target))
						{
							return BaseAction.ActionCompleteStates.Complete;
						}
					}
					return BaseAction.ActionCompleteStates.InComplete;
				}
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600B1BB RID: 45499 RVA: 0x004543A1 File Offset: 0x004525A1
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseEnum<BaseWait.ConditionTypes>(BaseWait.PropConditionType, ref this.ConditionType);
		}

		// Token: 0x0600B1BC RID: 45500 RVA: 0x004543BB File Offset: 0x004525BB
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new BaseWait
			{
				ConditionType = this.ConditionType
			};
		}

		// Token: 0x04008B29 RID: 35625
		public BaseWait.ConditionTypes ConditionType = BaseWait.ConditionTypes.All;

		// Token: 0x04008B2A RID: 35626
		public static string PropConditionType = "condition_type";

		// Token: 0x020016EE RID: 5870
		public enum ConditionTypes
		{
			// Token: 0x04008B2C RID: 35628
			Any,
			// Token: 0x04008B2D RID: 35629
			All
		}
	}
}
