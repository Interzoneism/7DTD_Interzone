using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x0200160E RID: 5646
	[Preserve]
	public class BaseOperationRequirement : BaseRequirement
	{
		// Token: 0x0600ADA9 RID: 44457 RVA: 0x00002914 File Offset: 0x00000B14
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600ADAA RID: 44458 RVA: 0x0043FC74 File Offset: 0x0043DE74
		public override bool CanPerform(Entity target)
		{
			float num = this.LeftSide(target);
			float num2 = this.RightSide(target);
			switch (this.operation)
			{
			case BaseOperationRequirement.OperationTypes.Equals:
			case BaseOperationRequirement.OperationTypes.EQ:
			case BaseOperationRequirement.OperationTypes.E:
				return num == num2;
			case BaseOperationRequirement.OperationTypes.NotEquals:
			case BaseOperationRequirement.OperationTypes.NEQ:
			case BaseOperationRequirement.OperationTypes.NE:
				return num != num2;
			case BaseOperationRequirement.OperationTypes.Less:
			case BaseOperationRequirement.OperationTypes.LessThan:
			case BaseOperationRequirement.OperationTypes.LT:
				return num < num2;
			case BaseOperationRequirement.OperationTypes.Greater:
			case BaseOperationRequirement.OperationTypes.GreaterThan:
			case BaseOperationRequirement.OperationTypes.GT:
				return num > num2;
			case BaseOperationRequirement.OperationTypes.LessOrEqual:
			case BaseOperationRequirement.OperationTypes.LessThanOrEqualTo:
			case BaseOperationRequirement.OperationTypes.LTE:
				return num <= num2;
			case BaseOperationRequirement.OperationTypes.GreaterOrEqual:
			case BaseOperationRequirement.OperationTypes.GreaterThanOrEqualTo:
			case BaseOperationRequirement.OperationTypes.GTE:
				return num >= num2;
			default:
				return true;
			}
		}

		// Token: 0x0600ADAB RID: 44459 RVA: 0x0002E7B0 File Offset: 0x0002C9B0
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual float LeftSide(Entity target)
		{
			return 0f;
		}

		// Token: 0x0600ADAC RID: 44460 RVA: 0x0002E7B0 File Offset: 0x0002C9B0
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual float RightSide(Entity target)
		{
			return 0f;
		}

		// Token: 0x0600ADAD RID: 44461 RVA: 0x0043FD12 File Offset: 0x0043DF12
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseEnum<BaseOperationRequirement.OperationTypes>(BaseOperationRequirement.PropOperation, ref this.operation);
		}

		// Token: 0x0600ADAE RID: 44462 RVA: 0x0043FD2C File Offset: 0x0043DF2C
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new BaseOperationRequirement
			{
				Invert = this.Invert,
				operation = this.operation
			};
		}

		// Token: 0x040086DA RID: 34522
		[PublicizedFrom(EAccessModifier.Protected)]
		public BaseOperationRequirement.OperationTypes operation = BaseOperationRequirement.OperationTypes.Equals;

		// Token: 0x040086DB RID: 34523
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropOperation = "operation";

		// Token: 0x0200160F RID: 5647
		[PublicizedFrom(EAccessModifier.Protected)]
		public enum OperationTypes
		{
			// Token: 0x040086DD RID: 34525
			None,
			// Token: 0x040086DE RID: 34526
			Equals,
			// Token: 0x040086DF RID: 34527
			EQ,
			// Token: 0x040086E0 RID: 34528
			E,
			// Token: 0x040086E1 RID: 34529
			NotEquals,
			// Token: 0x040086E2 RID: 34530
			NEQ,
			// Token: 0x040086E3 RID: 34531
			NE,
			// Token: 0x040086E4 RID: 34532
			Less,
			// Token: 0x040086E5 RID: 34533
			LessThan,
			// Token: 0x040086E6 RID: 34534
			LT,
			// Token: 0x040086E7 RID: 34535
			Greater,
			// Token: 0x040086E8 RID: 34536
			GreaterThan,
			// Token: 0x040086E9 RID: 34537
			GT,
			// Token: 0x040086EA RID: 34538
			LessOrEqual,
			// Token: 0x040086EB RID: 34539
			LessThanOrEqualTo,
			// Token: 0x040086EC RID: 34540
			LTE,
			// Token: 0x040086ED RID: 34541
			GreaterOrEqual,
			// Token: 0x040086EE RID: 34542
			GreaterThanOrEqualTo,
			// Token: 0x040086EF RID: 34543
			GTE
		}
	}
}
