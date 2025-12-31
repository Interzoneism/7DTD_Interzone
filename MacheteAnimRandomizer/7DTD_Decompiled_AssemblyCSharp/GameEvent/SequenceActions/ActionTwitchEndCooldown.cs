using System;
using Twitch;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016E5 RID: 5861
	[Preserve]
	public class ActionTwitchEndCooldown : ActionBaseClientAction
	{
		// Token: 0x0600B190 RID: 45456 RVA: 0x00453C7B File Offset: 0x00451E7B
		public override void OnClientPerform(Entity target)
		{
			if (TwitchManager.HasInstance)
			{
				TwitchManager.Current.ForceEndCooldown(this.playSound);
			}
		}

		// Token: 0x0600B191 RID: 45457 RVA: 0x00453C95 File Offset: 0x00451E95
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseBool(ActionTwitchEndCooldown.PropPlaySound, ref this.playSound);
		}

		// Token: 0x0600B192 RID: 45458 RVA: 0x00453CAF File Offset: 0x00451EAF
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionTwitchEndCooldown
			{
				playSound = this.playSound
			};
		}

		// Token: 0x04008B08 RID: 35592
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool playSound = true;

		// Token: 0x04008B09 RID: 35593
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropPlaySound = "play_sound";
	}
}
