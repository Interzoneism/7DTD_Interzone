using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001611 RID: 5649
	[Preserve]
	public class RequirementCVar : BaseOperationRequirement
	{
		// Token: 0x0600ADB9 RID: 44473 RVA: 0x00002914 File Offset: 0x00000B14
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600ADBA RID: 44474 RVA: 0x0043FE08 File Offset: 0x0043E008
		[PublicizedFrom(EAccessModifier.Protected)]
		public override float LeftSide(Entity target)
		{
			EntityAlive entityAlive = target as EntityAlive;
			if (entityAlive == null)
			{
				return 0f;
			}
			return entityAlive.Buffs.GetCustomVar(this.cvar);
		}

		// Token: 0x0600ADBB RID: 44475 RVA: 0x0043FE36 File Offset: 0x0043E036
		[PublicizedFrom(EAccessModifier.Protected)]
		public override float RightSide(Entity target)
		{
			return GameEventManager.GetFloatValue(target as EntityAlive, this.valueText, 0f);
		}

		// Token: 0x0600ADBC RID: 44476 RVA: 0x0043FE4E File Offset: 0x0043E04E
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(RequirementCVar.PropCvar, ref this.cvar);
			properties.ParseString(RequirementCVar.PropValue, ref this.valueText);
		}

		// Token: 0x0600ADBD RID: 44477 RVA: 0x0043FE79 File Offset: 0x0043E079
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementCVar
			{
				Invert = this.Invert,
				operation = this.operation,
				cvar = this.cvar,
				valueText = this.valueText
			};
		}

		// Token: 0x040086F4 RID: 34548
		[PublicizedFrom(EAccessModifier.Protected)]
		public string cvar = "";

		// Token: 0x040086F5 RID: 34549
		[PublicizedFrom(EAccessModifier.Protected)]
		public string valueText;

		// Token: 0x040086F6 RID: 34550
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropCvar = "cvar";

		// Token: 0x040086F7 RID: 34551
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropValue = "value";
	}
}
