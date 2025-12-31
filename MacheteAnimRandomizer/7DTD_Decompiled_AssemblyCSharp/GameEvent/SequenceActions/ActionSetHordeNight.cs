using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016A1 RID: 5793
	[Preserve]
	public class ActionSetHordeNight : BaseAction
	{
		// Token: 0x0600B050 RID: 45136 RVA: 0x0044E165 File Offset: 0x0044C365
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			if (GameManager.Instance != null && GameManager.Instance.World != null)
			{
				GameManager.Instance.World.aiDirector.BloodMoonComponent.SetForToday(this.keepBMDay);
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600B051 RID: 45137 RVA: 0x0044E1A1 File Offset: 0x0044C3A1
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseBool(ActionSetHordeNight.PropKeepBMDay, ref this.keepBMDay);
		}

		// Token: 0x0600B052 RID: 45138 RVA: 0x0044E1BB File Offset: 0x0044C3BB
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionSetHordeNight
			{
				keepBMDay = this.keepBMDay
			};
		}

		// Token: 0x040089D7 RID: 35287
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool keepBMDay = true;

		// Token: 0x040089D8 RID: 35288
		public static string PropKeepBMDay = "keep_bm_day";
	}
}
