using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x0200058A RID: 1418
[Preserve]
public class BaseOperationLootEntryRequirement : BaseLootEntryRequirement
{
	// Token: 0x06002DBB RID: 11707 RVA: 0x00131128 File Offset: 0x0012F328
	public override void Init(XElement e)
	{
		base.Init(e);
		string value = "";
		if (e.ParseAttribute("operation", ref value))
		{
			this.operation = Enum.Parse<BaseOperationLootEntryRequirement.OperationTypes>(value);
		}
	}

	// Token: 0x06002DBC RID: 11708 RVA: 0x00131164 File Offset: 0x0012F364
	public override bool CheckRequirement(EntityPlayer player)
	{
		float num = this.LeftSide(player);
		float num2 = this.RightSide(player);
		switch (this.operation)
		{
		case BaseOperationLootEntryRequirement.OperationTypes.Equals:
		case BaseOperationLootEntryRequirement.OperationTypes.EQ:
		case BaseOperationLootEntryRequirement.OperationTypes.E:
			return num == num2;
		case BaseOperationLootEntryRequirement.OperationTypes.NotEquals:
		case BaseOperationLootEntryRequirement.OperationTypes.NEQ:
		case BaseOperationLootEntryRequirement.OperationTypes.NE:
			return num != num2;
		case BaseOperationLootEntryRequirement.OperationTypes.Less:
		case BaseOperationLootEntryRequirement.OperationTypes.LessThan:
		case BaseOperationLootEntryRequirement.OperationTypes.LT:
			return num < num2;
		case BaseOperationLootEntryRequirement.OperationTypes.Greater:
		case BaseOperationLootEntryRequirement.OperationTypes.GreaterThan:
		case BaseOperationLootEntryRequirement.OperationTypes.GT:
			return num > num2;
		case BaseOperationLootEntryRequirement.OperationTypes.LessOrEqual:
		case BaseOperationLootEntryRequirement.OperationTypes.LessThanOrEqualTo:
		case BaseOperationLootEntryRequirement.OperationTypes.LTE:
			return num <= num2;
		case BaseOperationLootEntryRequirement.OperationTypes.GreaterOrEqual:
		case BaseOperationLootEntryRequirement.OperationTypes.GreaterThanOrEqualTo:
		case BaseOperationLootEntryRequirement.OperationTypes.GTE:
			return num >= num2;
		default:
			return true;
		}
	}

	// Token: 0x06002DBD RID: 11709 RVA: 0x0002E7B0 File Offset: 0x0002C9B0
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual float LeftSide(EntityPlayer player)
	{
		return 0f;
	}

	// Token: 0x06002DBE RID: 11710 RVA: 0x0002E7B0 File Offset: 0x0002C9B0
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual float RightSide(EntityPlayer player)
	{
		return 0f;
	}

	// Token: 0x04002458 RID: 9304
	[PublicizedFrom(EAccessModifier.Protected)]
	public BaseOperationLootEntryRequirement.OperationTypes operation = BaseOperationLootEntryRequirement.OperationTypes.Equals;

	// Token: 0x04002459 RID: 9305
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropOperation = "operation";

	// Token: 0x0200058B RID: 1419
	[PublicizedFrom(EAccessModifier.Protected)]
	public enum OperationTypes
	{
		// Token: 0x0400245B RID: 9307
		None,
		// Token: 0x0400245C RID: 9308
		Equals,
		// Token: 0x0400245D RID: 9309
		EQ,
		// Token: 0x0400245E RID: 9310
		E,
		// Token: 0x0400245F RID: 9311
		NotEquals,
		// Token: 0x04002460 RID: 9312
		NEQ,
		// Token: 0x04002461 RID: 9313
		NE,
		// Token: 0x04002462 RID: 9314
		Less,
		// Token: 0x04002463 RID: 9315
		LessThan,
		// Token: 0x04002464 RID: 9316
		LT,
		// Token: 0x04002465 RID: 9317
		Greater,
		// Token: 0x04002466 RID: 9318
		GreaterThan,
		// Token: 0x04002467 RID: 9319
		GT,
		// Token: 0x04002468 RID: 9320
		LessOrEqual,
		// Token: 0x04002469 RID: 9321
		LessThanOrEqualTo,
		// Token: 0x0400246A RID: 9322
		LTE,
		// Token: 0x0400246B RID: 9323
		GreaterOrEqual,
		// Token: 0x0400246C RID: 9324
		GreaterThanOrEqualTo,
		// Token: 0x0400246D RID: 9325
		GTE
	}
}
