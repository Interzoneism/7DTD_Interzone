using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016EC RID: 5868
	[Preserve]
	public class ActionModifyVarInt : ActionBaseClientAction
	{
		// Token: 0x0600B1B4 RID: 45492 RVA: 0x00454198 File Offset: 0x00452398
		public override BaseAction.ActionCompleteStates PerformTargetAction(Entity target)
		{
			int intValue = GameEventManager.GetIntValue(target as EntityAlive, this.valueText, 0);
			base.Owner.EventVariables.ModifyEventVariable(this.varName, this.operationType, intValue, this.minValue, this.maxValue);
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600B1B5 RID: 45493 RVA: 0x004541E4 File Offset: 0x004523E4
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionModifyVarInt.PropValue, ref this.valueText);
			properties.ParseString(ActionModifyVarInt.PropVarName, ref this.varName);
			properties.ParseEnum<GameEventVariables.OperationTypes>(ActionModifyVarInt.PropOperation, ref this.operationType);
			properties.ParseInt(ActionModifyVarInt.PropMinValue, ref this.minValue);
			properties.ParseInt(ActionModifyVarInt.PropMaxValue, ref this.maxValue);
		}

		// Token: 0x0600B1B6 RID: 45494 RVA: 0x00454250 File Offset: 0x00452450
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionModifyVarInt
			{
				varName = this.varName,
				valueText = this.valueText,
				operationType = this.operationType,
				minValue = this.minValue,
				maxValue = this.maxValue
			};
		}

		// Token: 0x04008B1F RID: 35615
		[PublicizedFrom(EAccessModifier.Protected)]
		public string valueText = "";

		// Token: 0x04008B20 RID: 35616
		[PublicizedFrom(EAccessModifier.Protected)]
		public string varName = "";

		// Token: 0x04008B21 RID: 35617
		[PublicizedFrom(EAccessModifier.Protected)]
		public GameEventVariables.OperationTypes operationType;

		// Token: 0x04008B22 RID: 35618
		[PublicizedFrom(EAccessModifier.Protected)]
		public int minValue = int.MinValue;

		// Token: 0x04008B23 RID: 35619
		[PublicizedFrom(EAccessModifier.Protected)]
		public int maxValue = int.MaxValue;

		// Token: 0x04008B24 RID: 35620
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropValue = "value";

		// Token: 0x04008B25 RID: 35621
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropVarName = "var_name";

		// Token: 0x04008B26 RID: 35622
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropOperation = "operation";

		// Token: 0x04008B27 RID: 35623
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropMinValue = "min_value";

		// Token: 0x04008B28 RID: 35624
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropMaxValue = "min_value";
	}
}
