using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x0200169E RID: 5790
	[Preserve]
	public class ActionSetEventFlag : ActionBaseTargetAction
	{
		// Token: 0x0600B046 RID: 45126 RVA: 0x0044DFC5 File Offset: 0x0044C1C5
		public override BaseAction.ActionCompleteStates PerformTargetAction(Entity target)
		{
			GameEventManager.Current.SetGameEventFlag(this.eventFlag, this.enable, GameEventManager.GetFloatValue(target as EntityAlive, this.durationText, 0f));
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600B047 RID: 45127 RVA: 0x0044DFF4 File Offset: 0x0044C1F4
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseEnum<GameEventManager.GameEventFlagTypes>(ActionSetEventFlag.PropEventFlag, ref this.eventFlag);
			properties.ParseBool(ActionSetEventFlag.PropEnable, ref this.enable);
			properties.ParseString(ActionSetEventFlag.PropDuration, ref this.durationText);
		}

		// Token: 0x0600B048 RID: 45128 RVA: 0x0044E030 File Offset: 0x0044C230
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionSetEventFlag
			{
				targetGroup = this.targetGroup,
				eventFlag = this.eventFlag,
				enable = this.enable,
				durationText = this.durationText
			};
		}

		// Token: 0x040089CC RID: 35276
		[PublicizedFrom(EAccessModifier.Protected)]
		public GameEventManager.GameEventFlagTypes eventFlag = GameEventManager.GameEventFlagTypes.Invalid;

		// Token: 0x040089CD RID: 35277
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool enable;

		// Token: 0x040089CE RID: 35278
		[PublicizedFrom(EAccessModifier.Protected)]
		public string durationText;

		// Token: 0x040089CF RID: 35279
		public static string PropEventFlag = "event_flag";

		// Token: 0x040089D0 RID: 35280
		public static string PropEnable = "enable";

		// Token: 0x040089D1 RID: 35281
		public static string PropDuration = "duration";
	}
}
