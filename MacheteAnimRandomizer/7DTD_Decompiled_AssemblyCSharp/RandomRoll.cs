using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020005C8 RID: 1480
[Preserve]
public class RandomRoll : TargetedCompareRequirementBase
{
	// Token: 0x06002F62 RID: 12130 RVA: 0x00144ED0 File Offset: 0x001430D0
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (this.cvarName != null)
		{
			this.value = this.target.Buffs.GetCustomVar(this.cvarName);
		}
		if (this.seedType == RandomRoll.SeedType.Item)
		{
			this.rand = GameRandomManager.Instance.CreateGameRandom(_params.Seed);
		}
		else if (this.seedType == RandomRoll.SeedType.Player)
		{
			this.rand = GameRandomManager.Instance.CreateGameRandom(_params.Self.entityId);
		}
		else
		{
			this.rand = GameRandomManager.Instance.CreateGameRandom(Environment.TickCount);
		}
		float randomFloat = this.rand.RandomFloat;
		GameRandomManager.Instance.FreeGameRandom(this.rand);
		return this.invert != RequirementBase.compareValues(Utils.FastLerp(this.minMax.x, this.minMax.y, randomFloat), this.operation, this.value);
	}

	// Token: 0x06002F63 RID: 12131 RVA: 0x00144FBC File Offset: 0x001431BC
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("roll[{0}-{1}] {2} {3}", new object[]
		{
			this.minMax.x.ToCultureInvariantString(),
			this.minMax.y.ToCultureInvariantString(),
			this.operation.ToStringCached<RequirementBase.OperationTypes>(),
			this.value.ToCultureInvariantString()
		}));
	}

	// Token: 0x06002F64 RID: 12132 RVA: 0x00145024 File Offset: 0x00143224
	public override bool ParseXAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (localName == "min_max")
			{
				this.minMax = StringParsers.ParseVector2(_attribute.Value);
				return true;
			}
			if (localName == "seed_type")
			{
				this.seedType = EnumUtils.Parse<RandomRoll.SeedType>(_attribute.Value, true);
				return true;
			}
			if (localName == "seed_additive")
			{
				this.seedAdditive = StringParsers.ParseSInt32(_attribute.Value, 0, -1, NumberStyles.Integer);
				return true;
			}
		}
		return flag;
	}

	// Token: 0x0400267D RID: 9853
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2 minMax;

	// Token: 0x0400267E RID: 9854
	[PublicizedFrom(EAccessModifier.Private)]
	public RandomRoll.SeedType seedType;

	// Token: 0x0400267F RID: 9855
	[PublicizedFrom(EAccessModifier.Private)]
	public int seedAdditive;

	// Token: 0x04002680 RID: 9856
	[PublicizedFrom(EAccessModifier.Private)]
	public GameRandom rand;

	// Token: 0x020005C9 RID: 1481
	[PublicizedFrom(EAccessModifier.Private)]
	public enum SeedType
	{
		// Token: 0x04002682 RID: 9858
		Item,
		// Token: 0x04002683 RID: 9859
		Player,
		// Token: 0x04002684 RID: 9860
		Random
	}
}
