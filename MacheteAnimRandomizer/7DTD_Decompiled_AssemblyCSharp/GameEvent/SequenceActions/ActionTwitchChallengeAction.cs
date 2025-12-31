using System;
using Challenges;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016E4 RID: 5860
	[Preserve]
	public class ActionTwitchChallengeAction : ActionBaseClientAction
	{
		// Token: 0x0600B18B RID: 45451 RVA: 0x00453BF0 File Offset: 0x00451DF0
		public override void OnClientPerform(Entity target)
		{
			QuestEventManager.Current.TwitchEventReceived(this.TwitchObjectiveType, this.param);
		}

		// Token: 0x0600B18C RID: 45452 RVA: 0x00453C08 File Offset: 0x00451E08
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseEnum<TwitchObjectiveTypes>(ActionTwitchChallengeAction.PropObjectiveType, ref this.TwitchObjectiveType);
			properties.ParseString(ActionTwitchChallengeAction.PropObjectiveParam, ref this.param);
		}

		// Token: 0x0600B18D RID: 45453 RVA: 0x00453C33 File Offset: 0x00451E33
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionTwitchChallengeAction
			{
				TwitchObjectiveType = this.TwitchObjectiveType,
				param = this.param
			};
		}

		// Token: 0x04008B04 RID: 35588
		[PublicizedFrom(EAccessModifier.Private)]
		public TwitchObjectiveTypes TwitchObjectiveType;

		// Token: 0x04008B05 RID: 35589
		[PublicizedFrom(EAccessModifier.Private)]
		public string param = "";

		// Token: 0x04008B06 RID: 35590
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropObjectiveType = "objective_type";

		// Token: 0x04008B07 RID: 35591
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropObjectiveParam = "objective_param";
	}
}
