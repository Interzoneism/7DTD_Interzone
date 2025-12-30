using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001631 RID: 5681
	[Preserve]
	public class RequirementVarFloat : BaseOperationRequirement
	{
		// Token: 0x0600AE56 RID: 44630 RVA: 0x00002914 File Offset: 0x00000B14
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600AE57 RID: 44631 RVA: 0x00441234 File Offset: 0x0043F434
		[PublicizedFrom(EAccessModifier.Protected)]
		public override float LeftSide(Entity target)
		{
			int num = 0;
			this.Owner.EventVariables.ParseVarInt(this.varName, ref num);
			return (float)num;
		}

		// Token: 0x0600AE58 RID: 44632 RVA: 0x0044125D File Offset: 0x0043F45D
		[PublicizedFrom(EAccessModifier.Protected)]
		public override float RightSide(Entity target)
		{
			return GameEventManager.GetFloatValue(target as EntityAlive, this.valueText, 0f);
		}

		// Token: 0x0600AE59 RID: 44633 RVA: 0x00441275 File Offset: 0x0043F475
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(RequirementVarFloat.PropVarName, ref this.varName);
			properties.ParseString(RequirementVarFloat.PropValue, ref this.valueText);
		}

		// Token: 0x0600AE5A RID: 44634 RVA: 0x004412A0 File Offset: 0x0043F4A0
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementVarFloat
			{
				Invert = this.Invert,
				operation = this.operation,
				varName = this.varName,
				valueText = this.valueText
			};
		}

		// Token: 0x0400873B RID: 34619
		[PublicizedFrom(EAccessModifier.Protected)]
		public string varName;

		// Token: 0x0400873C RID: 34620
		[PublicizedFrom(EAccessModifier.Protected)]
		public string valueText;

		// Token: 0x0400873D RID: 34621
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropVarName = "var_name";

		// Token: 0x0400873E RID: 34622
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropValue = "value";
	}
}
