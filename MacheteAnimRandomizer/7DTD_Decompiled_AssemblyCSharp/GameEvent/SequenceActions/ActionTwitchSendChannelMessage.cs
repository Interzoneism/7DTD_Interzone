using System;
using Twitch;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016E6 RID: 5862
	[Preserve]
	public class ActionTwitchSendChannelMessage : ActionBaseClientAction
	{
		// Token: 0x0600B195 RID: 45461 RVA: 0x00453CE0 File Offset: 0x00451EE0
		public override void OnClientPerform(Entity target)
		{
			EntityPlayerLocal entityPlayerLocal = target as EntityPlayerLocal;
			if (entityPlayerLocal != null)
			{
				TwitchManager twitchManager = TwitchManager.Current;
				this.player = entityPlayerLocal;
				if (!twitchManager.TwitchActive)
				{
					return;
				}
				twitchManager.SendChannelMessage(base.GetTextWithElements(this.text), true);
			}
		}

		// Token: 0x0600B196 RID: 45462 RVA: 0x00453D20 File Offset: 0x00451F20
		[PublicizedFrom(EAccessModifier.Protected)]
		public override string ParseTextElement(string element)
		{
			if (element == "viewer")
			{
				return base.Owner.ExtraData;
			}
			if (!(element == "target"))
			{
				return element;
			}
			return this.player.EntityName;
		}

		// Token: 0x0600B197 RID: 45463 RVA: 0x00453D58 File Offset: 0x00451F58
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			if (properties.Values.ContainsKey(ActionTwitchSendChannelMessage.PropText))
			{
				this.text = properties.Values[ActionTwitchSendChannelMessage.PropText];
			}
			if (properties.Values.ContainsKey(ActionTwitchSendChannelMessage.PropTextKey))
			{
				this.textKey = properties.Values[ActionTwitchSendChannelMessage.PropTextKey];
				this.text = Localization.Get(this.textKey, false);
			}
		}

		// Token: 0x0600B198 RID: 45464 RVA: 0x00453DCE File Offset: 0x00451FCE
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionTwitchSendChannelMessage
			{
				targetGroup = this.targetGroup,
				textKey = this.textKey,
				text = this.text
			};
		}

		// Token: 0x04008B0A RID: 35594
		[PublicizedFrom(EAccessModifier.Protected)]
		public string textKey = "";

		// Token: 0x04008B0B RID: 35595
		[PublicizedFrom(EAccessModifier.Protected)]
		public string text = "";

		// Token: 0x04008B0C RID: 35596
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropText = "text";

		// Token: 0x04008B0D RID: 35597
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropTextKey = "text_key";

		// Token: 0x04008B0E RID: 35598
		[PublicizedFrom(EAccessModifier.Private)]
		public EntityPlayerLocal player;
	}
}
