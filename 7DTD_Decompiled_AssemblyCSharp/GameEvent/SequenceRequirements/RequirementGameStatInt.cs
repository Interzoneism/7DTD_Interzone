using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001617 RID: 5655
	[Preserve]
	public class RequirementGameStatInt : BaseOperationRequirement
	{
		// Token: 0x0600ADDD RID: 44509 RVA: 0x00002914 File Offset: 0x00000B14
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600ADDE RID: 44510 RVA: 0x0044019C File Offset: 0x0043E39C
		[PublicizedFrom(EAccessModifier.Protected)]
		public override float LeftSide(Entity target)
		{
			return (float)GameStats.GetInt(this.GameStat);
		}

		// Token: 0x0600ADDF RID: 44511 RVA: 0x004401AA File Offset: 0x0043E3AA
		[PublicizedFrom(EAccessModifier.Protected)]
		public override float RightSide(Entity target)
		{
			return (float)GameEventManager.GetIntValue(target as EntityAlive, this.valueText, 0);
		}

		// Token: 0x0600ADE0 RID: 44512 RVA: 0x004401BF File Offset: 0x0043E3BF
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseEnum<EnumGameStats>(RequirementGameStatInt.PropGameStat, ref this.GameStat);
			properties.ParseString(RequirementGameStatInt.PropValue, ref this.valueText);
		}

		// Token: 0x0600ADE1 RID: 44513 RVA: 0x004401EA File Offset: 0x0043E3EA
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementGameStatInt
			{
				Invert = this.Invert,
				operation = this.operation,
				GameStat = this.GameStat,
				valueText = this.valueText
			};
		}

		// Token: 0x04008702 RID: 34562
		[PublicizedFrom(EAccessModifier.Protected)]
		public EnumGameStats GameStat = EnumGameStats.AnimalCount;

		// Token: 0x04008703 RID: 34563
		[PublicizedFrom(EAccessModifier.Protected)]
		public string valueText;

		// Token: 0x04008704 RID: 34564
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropGameStat = "gamestat";

		// Token: 0x04008705 RID: 34565
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropValue = "value";
	}
}
