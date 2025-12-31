using System;
using Twitch;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016E7 RID: 5863
	[Preserve]
	public class ActionTwitchStartCooldown : ActionBaseClientAction
	{
		// Token: 0x0600B19B RID: 45467 RVA: 0x00453E30 File Offset: 0x00452030
		public override void OnClientPerform(Entity target)
		{
			TwitchManager twitchManager = TwitchManager.Current;
			if (!twitchManager.TwitchActive)
			{
				return;
			}
			float floatValue = GameEventManager.GetFloatValue(target as EntityAlive, this.cooldownTimeLeft, 5f);
			twitchManager.SetCooldown(floatValue, TwitchManager.CooldownTypes.Time, false, true);
		}

		// Token: 0x0600B19C RID: 45468 RVA: 0x00453E6D File Offset: 0x0045206D
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionTwitchStartCooldown.PropTime, ref this.cooldownTimeLeft);
		}

		// Token: 0x0600B19D RID: 45469 RVA: 0x00453E87 File Offset: 0x00452087
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionTwitchStartCooldown
			{
				cooldownTimeLeft = this.cooldownTimeLeft
			};
		}

		// Token: 0x04008B0F RID: 35599
		[PublicizedFrom(EAccessModifier.Protected)]
		public string cooldownTimeLeft;

		// Token: 0x04008B10 RID: 35600
		public static string PropTime = "time";
	}
}
