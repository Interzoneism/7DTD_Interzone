using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016AA RID: 5802
	[Preserve]
	public class ActionShowMessageWindow : ActionBaseClientAction
	{
		// Token: 0x0600B078 RID: 45176 RVA: 0x0044EA3C File Offset: 0x0044CC3C
		public override void OnClientPerform(Entity target)
		{
			EntityPlayerLocal entityPlayerLocal = target as EntityPlayerLocal;
			if (entityPlayerLocal != null)
			{
				XUiC_TipWindow.ShowTip(this.message, this.title, entityPlayerLocal, null);
			}
		}

		// Token: 0x0600B079 RID: 45177 RVA: 0x0044EA66 File Offset: 0x0044CC66
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionShowMessageWindow.PropMessage, ref this.message);
			properties.ParseString(ActionShowMessageWindow.PropTitle, ref this.title);
		}

		// Token: 0x0600B07A RID: 45178 RVA: 0x0044EA91 File Offset: 0x0044CC91
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionShowMessageWindow
			{
				targetGroup = this.targetGroup,
				message = this.message,
				title = this.title
			};
		}

		// Token: 0x040089FF RID: 35327
		[PublicizedFrom(EAccessModifier.Private)]
		public static string PropMessage = "message";

		// Token: 0x04008A00 RID: 35328
		[PublicizedFrom(EAccessModifier.Private)]
		public static string PropTitle = "title";

		// Token: 0x04008A01 RID: 35329
		[PublicizedFrom(EAccessModifier.Private)]
		public string message = "";

		// Token: 0x04008A02 RID: 35330
		[PublicizedFrom(EAccessModifier.Private)]
		public string title = "";
	}
}
