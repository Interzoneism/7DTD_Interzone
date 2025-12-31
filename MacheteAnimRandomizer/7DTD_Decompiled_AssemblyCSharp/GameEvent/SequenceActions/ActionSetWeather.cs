using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016A9 RID: 5801
	[Preserve]
	public class ActionSetWeather : BaseAction
	{
		// Token: 0x0600B073 RID: 45171 RVA: 0x0044E988 File Offset: 0x0044CB88
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			float floatValue = GameEventManager.GetFloatValue(base.Owner.Target as EntityAlive, this.timeText, 60f);
			WeatherManager.Instance.ForceWeather(this.weatherGroup, floatValue);
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600B074 RID: 45172 RVA: 0x0044E9C8 File Offset: 0x0044CBC8
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionSetWeather.PropTime, ref this.timeText);
			properties.ParseString(ActionSetWeather.PropWeatherGroup, ref this.weatherGroup);
		}

		// Token: 0x0600B075 RID: 45173 RVA: 0x0044E9F3 File Offset: 0x0044CBF3
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionSetWeather
			{
				weatherGroup = this.weatherGroup,
				timeText = this.timeText
			};
		}

		// Token: 0x040089FB RID: 35323
		[PublicizedFrom(EAccessModifier.Protected)]
		public string timeText;

		// Token: 0x040089FC RID: 35324
		[PublicizedFrom(EAccessModifier.Protected)]
		public string weatherGroup = "default";

		// Token: 0x040089FD RID: 35325
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropTime = "time";

		// Token: 0x040089FE RID: 35326
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropWeatherGroup = "weather_group";
	}
}
