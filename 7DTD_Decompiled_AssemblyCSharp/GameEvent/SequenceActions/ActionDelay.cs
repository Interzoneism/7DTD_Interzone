using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x0200165F RID: 5727
	[Preserve]
	public class ActionDelay : BaseAction
	{
		// Token: 0x0600AF32 RID: 44850 RVA: 0x004466C8 File Offset: 0x004448C8
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			if (this.currentTime == -999f)
			{
				this.currentTime = GameEventManager.GetFloatValue(base.Owner.Target as EntityAlive, this.delayTimeText, 5f);
			}
			this.currentTime -= Time.deltaTime;
			if (this.currentTime <= 0f)
			{
				return BaseAction.ActionCompleteStates.Complete;
			}
			return BaseAction.ActionCompleteStates.InComplete;
		}

		// Token: 0x0600AF33 RID: 44851 RVA: 0x0044672A File Offset: 0x0044492A
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnReset()
		{
			this.currentTime = -999f;
		}

		// Token: 0x0600AF34 RID: 44852 RVA: 0x00446737 File Offset: 0x00444937
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionDelay.PropTime, ref this.delayTimeText);
		}

		// Token: 0x0600AF35 RID: 44853 RVA: 0x00446751 File Offset: 0x00444951
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionDelay
			{
				delayTimeText = this.delayTimeText
			};
		}

		// Token: 0x0400884C RID: 34892
		[PublicizedFrom(EAccessModifier.Protected)]
		public string delayTimeText = "";

		// Token: 0x0400884D RID: 34893
		[PublicizedFrom(EAccessModifier.Protected)]
		public float delayTime = 5f;

		// Token: 0x0400884E RID: 34894
		[PublicizedFrom(EAccessModifier.Protected)]
		public float currentTime = -999f;

		// Token: 0x0400884F RID: 34895
		public static string PropTime = "time";
	}
}
