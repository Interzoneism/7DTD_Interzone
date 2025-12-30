using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016AB RID: 5803
	[Preserve]
	public class ActionShowWindow : ActionBaseClientAction
	{
		// Token: 0x0600B07D RID: 45181 RVA: 0x0044EAF0 File Offset: 0x0044CCF0
		public override void OnClientPerform(Entity target)
		{
			EntityPlayerLocal entityPlayerLocal = target as EntityPlayerLocal;
			if (entityPlayerLocal != null)
			{
				entityPlayerLocal.PlayerUI.windowManager.OpenIfNotOpen(this.window, true, false, true);
			}
		}

		// Token: 0x0600B07E RID: 45182 RVA: 0x0044EB20 File Offset: 0x0044CD20
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionShowWindow.PropWindow, ref this.window);
		}

		// Token: 0x0600B07F RID: 45183 RVA: 0x0044EB3A File Offset: 0x0044CD3A
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionShowWindow
			{
				targetGroup = this.targetGroup,
				window = this.window
			};
		}

		// Token: 0x04008A03 RID: 35331
		[PublicizedFrom(EAccessModifier.Private)]
		public static string PropWindow = "window";

		// Token: 0x04008A04 RID: 35332
		[PublicizedFrom(EAccessModifier.Private)]
		public string window = "";
	}
}
