using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x0200169D RID: 5789
	[Preserve]
	public class ActionSetDayTime : BaseAction
	{
		// Token: 0x0600B041 RID: 45121 RVA: 0x0044DEAC File Offset: 0x0044C0AC
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			World world = GameManager.Instance.World;
			ulong worldTime = world.worldTime;
			int num = GameUtils.WorldTimeToDays(worldTime);
			int num2 = GameUtils.WorldTimeToHours(worldTime);
			int num3 = GameUtils.WorldTimeToMinutes(worldTime);
			ulong time = GameUtils.DayTimeToWorldTime((this.day < 1) ? num : this.day, (this.hours < 0) ? num2 : this.hours, (this.minutes < 0) ? num3 : this.minutes);
			world.SetTimeJump(time, true);
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600B042 RID: 45122 RVA: 0x0044DF21 File Offset: 0x0044C121
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseInt(ActionSetDayTime.PropDay, ref this.day);
			properties.ParseInt(ActionSetDayTime.PropHours, ref this.hours);
			properties.ParseInt(ActionSetDayTime.PropMinutes, ref this.minutes);
		}

		// Token: 0x0600B043 RID: 45123 RVA: 0x0044DF5D File Offset: 0x0044C15D
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionSetDayTime
			{
				day = this.day,
				hours = this.hours,
				minutes = this.minutes
			};
		}

		// Token: 0x040089C6 RID: 35270
		[PublicizedFrom(EAccessModifier.Protected)]
		public int day = -1;

		// Token: 0x040089C7 RID: 35271
		[PublicizedFrom(EAccessModifier.Protected)]
		public int hours = -1;

		// Token: 0x040089C8 RID: 35272
		[PublicizedFrom(EAccessModifier.Protected)]
		public int minutes = -1;

		// Token: 0x040089C9 RID: 35273
		public static string PropDay = "day";

		// Token: 0x040089CA RID: 35274
		public static string PropHours = "hours";

		// Token: 0x040089CB RID: 35275
		public static string PropMinutes = "minutes";
	}
}
