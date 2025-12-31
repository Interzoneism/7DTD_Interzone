using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016BB RID: 5819
	[Preserve]
	public class ActionTimeChange : BaseAction
	{
		// Token: 0x0600B0C1 RID: 45249 RVA: 0x0044FDAC File Offset: 0x0044DFAC
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			World world = GameManager.Instance.World;
			ulong num = world.worldTime;
			ulong num2 = world.worldTime;
			switch (this.timePreset)
			{
			case ActionTimeChange.TimePresets.Current:
				num = world.worldTime;
				break;
			case ActionTimeChange.TimePresets.Morning:
				num = GameUtils.DayTimeToWorldTime(GameUtils.WorldTimeToDays(world.worldTime), (int)SkyManager.GetDuskTime(), 0);
				break;
			case ActionTimeChange.TimePresets.Noon:
				num = GameUtils.DayTimeToWorldTime(GameUtils.WorldTimeToDays(world.worldTime), 12, 0);
				break;
			case ActionTimeChange.TimePresets.Night:
				num = GameUtils.DayTimeToWorldTime(GameUtils.WorldTimeToDays(world.worldTime), 21, 45);
				break;
			case ActionTimeChange.TimePresets.NextMorning:
			{
				ulong worldTime = world.worldTime;
				int num3 = GameUtils.WorldTimeToDays(worldTime);
				int num4 = GameUtils.WorldTimeToHours(worldTime);
				int num5 = (int)SkyManager.GetDawnTime();
				if (num4 < num5)
				{
					num = GameUtils.DayTimeToWorldTime(num3, num5, 0);
				}
				else
				{
					num = GameUtils.DayTimeToWorldTime(num3 + 1, num5, 0);
				}
				break;
			}
			case ActionTimeChange.TimePresets.NextNoon:
			{
				ulong worldTime2 = world.worldTime;
				int num6 = GameUtils.WorldTimeToDays(worldTime2);
				if (GameUtils.WorldTimeToHours(worldTime2) < 12)
				{
					num = GameUtils.DayTimeToWorldTime(num6, 12, 0);
				}
				else
				{
					num = GameUtils.DayTimeToWorldTime(num6 + 1, 12, 0);
				}
				break;
			}
			case ActionTimeChange.TimePresets.NextNight:
			{
				ulong worldTime3 = world.worldTime;
				int num7 = GameUtils.WorldTimeToDays(worldTime3);
				if (GameUtils.WorldTimeToHours(worldTime3) < 22)
				{
					num = GameUtils.DayTimeToWorldTime(num7, 22, 0);
				}
				else
				{
					num = GameUtils.DayTimeToWorldTime(num7 + 1, 22, 0);
				}
				break;
			}
			case ActionTimeChange.TimePresets.HordeNight:
				num = GameUtils.DayTimeToWorldTime(GameStats.GetInt(EnumGameStats.BloodMoonDay), 21, 45);
				break;
			}
			int num8 = GameEventManager.GetIntValue(base.Owner.Target as EntityAlive, this.timeText, 60) * 1000 / 60;
			if (num8 < 0)
			{
				num2 = num + (ulong)((long)num8);
				if (num2 > world.worldTime)
				{
					num2 = 0UL;
				}
			}
			else if (num8 > 0)
			{
				num2 = num + (ulong)((long)num8);
				if (num2 < num)
				{
					num2 = num;
				}
			}
			else
			{
				num2 = num;
			}
			if (num2 != world.worldTime)
			{
				world.SetTimeJump(num2, true);
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600B0C2 RID: 45250 RVA: 0x0044FF7A File Offset: 0x0044E17A
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionTimeChange.PropTime, ref this.timeText);
			properties.ParseEnum<ActionTimeChange.TimePresets>(ActionTimeChange.PropTimePreset, ref this.timePreset);
		}

		// Token: 0x0600B0C3 RID: 45251 RVA: 0x0044FFA5 File Offset: 0x0044E1A5
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionTimeChange
			{
				timeText = this.timeText,
				timePreset = this.timePreset
			};
		}

		// Token: 0x04008A45 RID: 35397
		[PublicizedFrom(EAccessModifier.Protected)]
		public string timeText;

		// Token: 0x04008A46 RID: 35398
		[PublicizedFrom(EAccessModifier.Protected)]
		public ActionTimeChange.TimePresets timePreset;

		// Token: 0x04008A47 RID: 35399
		public static string PropTimePreset = "time_preset";

		// Token: 0x04008A48 RID: 35400
		public static string PropTime = "time";

		// Token: 0x020016BC RID: 5820
		[PublicizedFrom(EAccessModifier.Protected)]
		public enum TimePresets
		{
			// Token: 0x04008A4A RID: 35402
			Current,
			// Token: 0x04008A4B RID: 35403
			Morning,
			// Token: 0x04008A4C RID: 35404
			Noon,
			// Token: 0x04008A4D RID: 35405
			Night,
			// Token: 0x04008A4E RID: 35406
			NextMorning,
			// Token: 0x04008A4F RID: 35407
			NextNoon,
			// Token: 0x04008A50 RID: 35408
			NextNight,
			// Token: 0x04008A51 RID: 35409
			HordeNight
		}
	}
}
