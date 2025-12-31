using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016A6 RID: 5798
	[Preserve]
	public class ActionSetScreenEffect : ActionBaseClientAction
	{
		// Token: 0x0600B064 RID: 45156 RVA: 0x0044E678 File Offset: 0x0044C878
		public override void OnClientPerform(Entity target)
		{
			EntityPlayerLocal entityPlayerLocal = target as EntityPlayerLocal;
			if (entityPlayerLocal != null)
			{
				entityPlayerLocal.ScreenEffectManager.SetScreenEffect(this.screenEffect, GameEventManager.GetFloatValue(entityPlayerLocal, this.intensityText, 0f), GameEventManager.GetFloatValue(entityPlayerLocal, this.fadeTimeText, 0f));
			}
		}

		// Token: 0x0600B065 RID: 45157 RVA: 0x0044E6C2 File Offset: 0x0044C8C2
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionSetScreenEffect.PropScreenEffect, ref this.screenEffect);
			properties.ParseString(ActionSetScreenEffect.PropIntensity, ref this.intensityText);
			properties.ParseString(ActionSetScreenEffect.PropFadeTime, ref this.fadeTimeText);
		}

		// Token: 0x0600B066 RID: 45158 RVA: 0x0044E6FE File Offset: 0x0044C8FE
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionSetScreenEffect
			{
				screenEffect = this.screenEffect,
				intensityText = this.intensityText,
				fadeTimeText = this.fadeTimeText,
				targetGroup = this.targetGroup
			};
		}

		// Token: 0x040089EB RID: 35307
		[PublicizedFrom(EAccessModifier.Protected)]
		public string screenEffect = "";

		// Token: 0x040089EC RID: 35308
		[PublicizedFrom(EAccessModifier.Protected)]
		public string intensityText;

		// Token: 0x040089ED RID: 35309
		[PublicizedFrom(EAccessModifier.Protected)]
		public string fadeTimeText;

		// Token: 0x040089EE RID: 35310
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropScreenEffect = "screen_effect";

		// Token: 0x040089EF RID: 35311
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropIntensity = "intensity";

		// Token: 0x040089F0 RID: 35312
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropFadeTime = "fade_time";
	}
}
