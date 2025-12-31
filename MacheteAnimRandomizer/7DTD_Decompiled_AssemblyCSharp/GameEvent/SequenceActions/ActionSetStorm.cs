using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016A7 RID: 5799
	[Preserve]
	public class ActionSetStorm : BaseAction
	{
		// Token: 0x0600B069 RID: 45161 RVA: 0x0044E768 File Offset: 0x0044C968
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			float floatValue = GameEventManager.GetFloatValue(base.Owner.Target as EntityAlive, this.timeText, 1f);
			WeatherManager.Instance.SetStorm(null, (int)(floatValue * 1000f));
			WeatherManager.Instance.TriggerUpdate();
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600B06A RID: 45162 RVA: 0x0044E7B4 File Offset: 0x0044C9B4
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionSetStorm.PropTime, ref this.timeText);
		}

		// Token: 0x0600B06B RID: 45163 RVA: 0x0044E7CE File Offset: 0x0044C9CE
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionSetStorm
			{
				timeText = this.timeText
			};
		}

		// Token: 0x040089F1 RID: 35313
		[PublicizedFrom(EAccessModifier.Protected)]
		public string timeText;

		// Token: 0x040089F2 RID: 35314
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropTime = "hours";
	}
}
