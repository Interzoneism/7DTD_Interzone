using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016A2 RID: 5794
	[Preserve]
	public class ActionSetInvestigationPosition : ActionBaseTargetAction
	{
		// Token: 0x0600B055 RID: 45141 RVA: 0x0044E1EC File Offset: 0x0044C3EC
		public override BaseAction.ActionCompleteStates PerformTargetAction(Entity target)
		{
			EntityAlive entityAlive = target as EntityAlive;
			if (entityAlive != null)
			{
				entityAlive.SetInvestigatePosition(base.Owner.TargetPosition, (int)(this.investigateTime * 20f), this.isAlert);
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600B056 RID: 45142 RVA: 0x0044E22E File Offset: 0x0044C42E
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseFloat(ActionSetInvestigationPosition.PropTime, ref this.investigateTime);
			properties.ParseBool(ActionSetInvestigationPosition.PropIsAlert, ref this.isAlert);
		}

		// Token: 0x0600B057 RID: 45143 RVA: 0x0044E259 File Offset: 0x0044C459
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionSetInvestigationPosition
			{
				investigateTime = this.investigateTime,
				isAlert = this.isAlert,
				targetGroup = this.targetGroup
			};
		}

		// Token: 0x040089D9 RID: 35289
		[PublicizedFrom(EAccessModifier.Protected)]
		public float investigateTime;

		// Token: 0x040089DA RID: 35290
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool isAlert;

		// Token: 0x040089DB RID: 35291
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropTime = "time";

		// Token: 0x040089DC RID: 35292
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropIsAlert = "is_alert";
	}
}
