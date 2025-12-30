using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001612 RID: 5650
	[Preserve]
	public class RequirementEventActive : BaseRequirement
	{
		// Token: 0x0600ADC0 RID: 44480 RVA: 0x00002914 File Offset: 0x00000B14
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600ADC1 RID: 44481 RVA: 0x0043FEDC File Offset: 0x0043E0DC
		public override bool CanPerform(Entity target)
		{
			if (!EventsFromXml.Events.ContainsKey(this.EventName))
			{
				return this.Invert;
			}
			if (EventsFromXml.Events[this.EventName].Active)
			{
				return !this.Invert;
			}
			return this.Invert;
		}

		// Token: 0x0600ADC2 RID: 44482 RVA: 0x0043FF2C File Offset: 0x0043E12C
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(RequirementEventActive.PropEventName, ref this.EventName);
		}

		// Token: 0x0600ADC3 RID: 44483 RVA: 0x0043FF46 File Offset: 0x0043E146
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementEventActive
			{
				EventName = this.EventName,
				Invert = this.Invert
			};
		}

		// Token: 0x040086F8 RID: 34552
		[PublicizedFrom(EAccessModifier.Protected)]
		public string EventName = "";

		// Token: 0x040086F9 RID: 34553
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropEventName = "event_name";
	}
}
