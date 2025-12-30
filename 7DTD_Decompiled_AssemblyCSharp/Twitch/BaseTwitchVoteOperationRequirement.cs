using System;

namespace Twitch
{
	// Token: 0x02001591 RID: 5521
	public class BaseTwitchVoteOperationRequirement : BaseTwitchVoteRequirement
	{
		// Token: 0x0600A9EA RID: 43498 RVA: 0x00002914 File Offset: 0x00000B14
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600A9EB RID: 43499 RVA: 0x004323BC File Offset: 0x004305BC
		public override bool CanPerform(EntityPlayer player)
		{
			float num = this.LeftSide(player);
			float num2 = this.RightSide(player);
			switch (this.operation)
			{
			case BaseTwitchVoteOperationRequirement.OperationTypes.Equals:
			case BaseTwitchVoteOperationRequirement.OperationTypes.EQ:
			case BaseTwitchVoteOperationRequirement.OperationTypes.E:
				return num == num2;
			case BaseTwitchVoteOperationRequirement.OperationTypes.NotEquals:
			case BaseTwitchVoteOperationRequirement.OperationTypes.NEQ:
			case BaseTwitchVoteOperationRequirement.OperationTypes.NE:
				return num != num2;
			case BaseTwitchVoteOperationRequirement.OperationTypes.Less:
			case BaseTwitchVoteOperationRequirement.OperationTypes.LessThan:
			case BaseTwitchVoteOperationRequirement.OperationTypes.LT:
				return num < num2;
			case BaseTwitchVoteOperationRequirement.OperationTypes.Greater:
			case BaseTwitchVoteOperationRequirement.OperationTypes.GreaterThan:
			case BaseTwitchVoteOperationRequirement.OperationTypes.GT:
				return num > num2;
			case BaseTwitchVoteOperationRequirement.OperationTypes.LessOrEqual:
			case BaseTwitchVoteOperationRequirement.OperationTypes.LessThanOrEqualTo:
			case BaseTwitchVoteOperationRequirement.OperationTypes.LTE:
				return num <= num2;
			case BaseTwitchVoteOperationRequirement.OperationTypes.GreaterOrEqual:
			case BaseTwitchVoteOperationRequirement.OperationTypes.GreaterThanOrEqualTo:
			case BaseTwitchVoteOperationRequirement.OperationTypes.GTE:
				return num >= num2;
			default:
				return true;
			}
		}

		// Token: 0x0600A9EC RID: 43500 RVA: 0x0002E7B0 File Offset: 0x0002C9B0
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual float LeftSide(EntityPlayer player)
		{
			return 0f;
		}

		// Token: 0x0600A9ED RID: 43501 RVA: 0x0002E7B0 File Offset: 0x0002C9B0
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual float RightSide(EntityPlayer player)
		{
			return 0f;
		}

		// Token: 0x0600A9EE RID: 43502 RVA: 0x0043245A File Offset: 0x0043065A
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseEnum<BaseTwitchVoteOperationRequirement.OperationTypes>(BaseTwitchVoteOperationRequirement.PropOperation, ref this.operation);
		}

		// Token: 0x040084C4 RID: 33988
		[PublicizedFrom(EAccessModifier.Protected)]
		public BaseTwitchVoteOperationRequirement.OperationTypes operation;

		// Token: 0x040084C5 RID: 33989
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropOperation = "operation";

		// Token: 0x02001592 RID: 5522
		[PublicizedFrom(EAccessModifier.Protected)]
		public enum OperationTypes
		{
			// Token: 0x040084C7 RID: 33991
			None,
			// Token: 0x040084C8 RID: 33992
			Equals,
			// Token: 0x040084C9 RID: 33993
			EQ,
			// Token: 0x040084CA RID: 33994
			E,
			// Token: 0x040084CB RID: 33995
			NotEquals,
			// Token: 0x040084CC RID: 33996
			NEQ,
			// Token: 0x040084CD RID: 33997
			NE,
			// Token: 0x040084CE RID: 33998
			Less,
			// Token: 0x040084CF RID: 33999
			LessThan,
			// Token: 0x040084D0 RID: 34000
			LT,
			// Token: 0x040084D1 RID: 34001
			Greater,
			// Token: 0x040084D2 RID: 34002
			GreaterThan,
			// Token: 0x040084D3 RID: 34003
			GT,
			// Token: 0x040084D4 RID: 34004
			LessOrEqual,
			// Token: 0x040084D5 RID: 34005
			LessThanOrEqualTo,
			// Token: 0x040084D6 RID: 34006
			LTE,
			// Token: 0x040084D7 RID: 34007
			GreaterOrEqual,
			// Token: 0x040084D8 RID: 34008
			GreaterThanOrEqualTo,
			// Token: 0x040084D9 RID: 34009
			GTE
		}
	}
}
