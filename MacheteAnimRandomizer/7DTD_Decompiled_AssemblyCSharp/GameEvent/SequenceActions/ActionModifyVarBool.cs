using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016EA RID: 5866
	[Preserve]
	public class ActionModifyVarBool : ActionBaseClientAction
	{
		// Token: 0x0600B1AA RID: 45482 RVA: 0x00453FFA File Offset: 0x004521FA
		public override BaseAction.ActionCompleteStates PerformTargetAction(Entity target)
		{
			base.Owner.EventVariables.SetEventVariable(this.varName, StringParsers.ParseBool(this.valueText, 0, -1, true));
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600B1AB RID: 45483 RVA: 0x00454021 File Offset: 0x00452221
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionModifyVarBool.PropValue, ref this.valueText);
			properties.ParseString(ActionModifyVarBool.PropVarName, ref this.varName);
		}

		// Token: 0x0600B1AC RID: 45484 RVA: 0x0045404C File Offset: 0x0045224C
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionModifyVarBool
			{
				varName = this.varName,
				valueText = this.valueText
			};
		}

		// Token: 0x04008B15 RID: 35605
		[PublicizedFrom(EAccessModifier.Protected)]
		public string valueText = "";

		// Token: 0x04008B16 RID: 35606
		[PublicizedFrom(EAccessModifier.Protected)]
		public string varName = "";

		// Token: 0x04008B17 RID: 35607
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropValue = "value";

		// Token: 0x04008B18 RID: 35608
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropVarName = "var_name";
	}
}
