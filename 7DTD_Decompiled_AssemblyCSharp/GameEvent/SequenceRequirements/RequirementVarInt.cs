using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001632 RID: 5682
	[Preserve]
	public class RequirementVarInt : BaseOperationRequirement
	{
		// Token: 0x0600AE5D RID: 44637 RVA: 0x00002914 File Offset: 0x00000B14
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600AE5E RID: 44638 RVA: 0x004412F0 File Offset: 0x0043F4F0
		[PublicizedFrom(EAccessModifier.Protected)]
		public override float LeftSide(Entity target)
		{
			int num = 0;
			this.Owner.EventVariables.ParseVarInt(this.varName, ref num);
			return (float)num;
		}

		// Token: 0x0600AE5F RID: 44639 RVA: 0x00441319 File Offset: 0x0043F519
		[PublicizedFrom(EAccessModifier.Protected)]
		public override float RightSide(Entity target)
		{
			return (float)GameEventManager.GetIntValue(target as EntityAlive, this.valueText, 0);
		}

		// Token: 0x0600AE60 RID: 44640 RVA: 0x0044132E File Offset: 0x0043F52E
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(RequirementVarInt.PropVarName, ref this.varName);
			properties.ParseString(RequirementVarInt.PropValue, ref this.valueText);
		}

		// Token: 0x0600AE61 RID: 44641 RVA: 0x00441359 File Offset: 0x0043F559
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementVarInt
			{
				Invert = this.Invert,
				operation = this.operation,
				varName = this.varName,
				valueText = this.valueText
			};
		}

		// Token: 0x0400873F RID: 34623
		[PublicizedFrom(EAccessModifier.Protected)]
		public string varName;

		// Token: 0x04008740 RID: 34624
		[PublicizedFrom(EAccessModifier.Protected)]
		public string valueText;

		// Token: 0x04008741 RID: 34625
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropVarName = "var_name";

		// Token: 0x04008742 RID: 34626
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropValue = "value";
	}
}
