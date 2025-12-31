using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.Scripting;

// Token: 0x020003DC RID: 988
[Preserve]
public class EAIBlockIf : EAIBase
{
	// Token: 0x06001DFE RID: 7678 RVA: 0x000BAB17 File Offset: 0x000B8D17
	public override void Init(EntityAlive _theEntity)
	{
		base.Init(_theEntity);
		this.MutexBits = 1;
	}

	// Token: 0x06001DFF RID: 7679 RVA: 0x000BAB28 File Offset: 0x000B8D28
	public override void SetData(DictionarySave<string, string> data)
	{
		base.SetData(data);
		this.conditions = new List<EAIBlockIf.Condition>();
		string text;
		if (data.TryGetValue("condition", out text))
		{
			string[] array = text.Split(' ', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i += 3)
			{
				EAIBlockIf.Condition condition = new EAIBlockIf.Condition
				{
					type = EnumUtils.Parse<EAIBlockIf.eType>(array[i], true)
				};
				if (condition.type == EAIBlockIf.eType.None)
				{
					Log.Warning("{0} BlockIf type None", new object[]
					{
						this.theEntity.EntityName
					});
				}
				condition.op = EnumUtils.Parse<EAIBlockIf.eOp>(array[i + 1], true);
				if (condition.op == EAIBlockIf.eOp.None)
				{
					Log.Warning("{0} BlockIf op None", new object[]
					{
						this.theEntity.EntityName
					});
				}
				condition.value = StringParsers.ParseFloat(array[i + 2], 0, -1, NumberStyles.Any);
				this.conditions.Add(condition);
			}
		}
	}

	// Token: 0x06001E00 RID: 7680 RVA: 0x000BAC14 File Offset: 0x000B8E14
	public override bool CanExecute()
	{
		int count = this.conditions.Count;
		for (int i = 0; i < count; i++)
		{
			EAIBlockIf.Condition condition = this.conditions[i];
			float v = 0f;
			EAIBlockIf.eType type = condition.type;
			if (type != EAIBlockIf.eType.Alert)
			{
				if (type == EAIBlockIf.eType.Investigate)
				{
					v = (float)(this.theEntity.HasInvestigatePosition ? 1 : 0);
				}
			}
			else
			{
				v = (float)(this.theEntity.IsAlert ? 1 : 0);
			}
			if (this.Compare(condition.op, v, condition.value))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001E01 RID: 7681 RVA: 0x000BACA0 File Offset: 0x000B8EA0
	[PublicizedFrom(EAccessModifier.Private)]
	public bool Compare(EAIBlockIf.eOp op, float v1, float v2)
	{
		if (op != EAIBlockIf.eOp.e)
		{
			return op == EAIBlockIf.eOp.ne && v1 != v2;
		}
		return v1 == v2;
	}

	// Token: 0x06001E02 RID: 7682 RVA: 0x000BAA60 File Offset: 0x000B8C60
	public override bool Continue()
	{
		return this.CanExecute();
	}

	// Token: 0x04001497 RID: 5271
	public bool canExecute;

	// Token: 0x04001498 RID: 5272
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EAIBlockIf.Condition> conditions;

	// Token: 0x020003DD RID: 989
	[PublicizedFrom(EAccessModifier.Private)]
	public enum eType
	{
		// Token: 0x0400149A RID: 5274
		None,
		// Token: 0x0400149B RID: 5275
		Alert,
		// Token: 0x0400149C RID: 5276
		Investigate
	}

	// Token: 0x020003DE RID: 990
	[PublicizedFrom(EAccessModifier.Private)]
	public enum eOp
	{
		// Token: 0x0400149E RID: 5278
		None,
		// Token: 0x0400149F RID: 5279
		e,
		// Token: 0x040014A0 RID: 5280
		ne
	}

	// Token: 0x020003DF RID: 991
	[PublicizedFrom(EAccessModifier.Private)]
	public struct Condition
	{
		// Token: 0x040014A1 RID: 5281
		public EAIBlockIf.eType type;

		// Token: 0x040014A2 RID: 5282
		public EAIBlockIf.eOp op;

		// Token: 0x040014A3 RID: 5283
		public float value;
	}
}
