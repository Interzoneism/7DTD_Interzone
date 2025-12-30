using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x0200165D RID: 5725
	[Preserve]
	public class ActionCloseWindow : ActionBaseClientAction
	{
		// Token: 0x0600AF28 RID: 44840 RVA: 0x00446534 File Offset: 0x00444734
		public override void OnClientPerform(Entity target)
		{
			EntityPlayerLocal entityPlayerLocal = target as EntityPlayerLocal;
			if (entityPlayerLocal != null)
			{
				if (this.windowName == "")
				{
					entityPlayerLocal.PlayerUI.windowManager.CloseAllOpenWindows(null, false);
					return;
				}
				entityPlayerLocal.PlayerUI.windowManager.Close(this.windowName);
			}
		}

		// Token: 0x0600AF29 RID: 44841 RVA: 0x00446587 File Offset: 0x00444787
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionCloseWindow.PropWindow, ref this.windowName);
		}

		// Token: 0x0600AF2A RID: 44842 RVA: 0x004465A1 File Offset: 0x004447A1
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionCloseWindow
			{
				targetGroup = this.targetGroup,
				windowName = this.windowName
			};
		}

		// Token: 0x04008846 RID: 34886
		[PublicizedFrom(EAccessModifier.Protected)]
		public string windowName = "";

		// Token: 0x04008847 RID: 34887
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropWindow = "window";
	}
}
