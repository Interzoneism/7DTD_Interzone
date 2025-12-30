using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001630 RID: 5680
	[Preserve]
	public class RequirementVarBool : BaseRequirement
	{
		// Token: 0x0600AE51 RID: 44625 RVA: 0x00441178 File Offset: 0x0043F378
		public override bool CanPerform(Entity target)
		{
			if (target is EntityAlive)
			{
				bool flag = false;
				this.Owner.EventVariables.ParseBool(this.varName, ref flag);
				if (flag == StringParsers.ParseBool(this.valueText, 0, -1, true))
				{
					return !this.Invert;
				}
			}
			return this.Invert;
		}

		// Token: 0x0600AE52 RID: 44626 RVA: 0x004411C8 File Offset: 0x0043F3C8
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(RequirementVarBool.PropVarName, ref this.varName);
			properties.ParseString(RequirementVarBool.PropValue, ref this.valueText);
		}

		// Token: 0x0600AE53 RID: 44627 RVA: 0x004411F3 File Offset: 0x0043F3F3
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementVarBool
			{
				Invert = this.Invert,
				varName = this.varName,
				valueText = this.valueText
			};
		}

		// Token: 0x04008737 RID: 34615
		[PublicizedFrom(EAccessModifier.Protected)]
		public string varName;

		// Token: 0x04008738 RID: 34616
		[PublicizedFrom(EAccessModifier.Protected)]
		public string valueText;

		// Token: 0x04008739 RID: 34617
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropVarName = "var_name";

		// Token: 0x0400873A RID: 34618
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropValue = "value";
	}
}
