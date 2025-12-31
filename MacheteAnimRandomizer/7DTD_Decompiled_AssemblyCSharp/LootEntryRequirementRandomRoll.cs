using System;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000590 RID: 1424
[Preserve]
public class LootEntryRequirementRandomRoll : BaseOperationLootEntryRequirement
{
	// Token: 0x06002DCF RID: 11727 RVA: 0x001313EA File Offset: 0x0012F5EA
	public override void Init(XElement e)
	{
		base.Init(e);
		e.ParseAttribute("min_max", ref this.minMax);
		e.ParseAttribute("value", ref this.valueText);
	}

	// Token: 0x06002DD0 RID: 11728 RVA: 0x00131424 File Offset: 0x0012F624
	[PublicizedFrom(EAccessModifier.Protected)]
	public override float LeftSide(EntityPlayer player)
	{
		float randomFloat = GameEventManager.Current.Random.RandomFloat;
		return Mathf.Lerp(this.minMax.x, this.minMax.y, randomFloat);
	}

	// Token: 0x06002DD1 RID: 11729 RVA: 0x0013145D File Offset: 0x0012F65D
	[PublicizedFrom(EAccessModifier.Protected)]
	public override float RightSide(EntityPlayer player)
	{
		return GameEventManager.GetFloatValue(player, this.valueText, 0f);
	}

	// Token: 0x04002474 RID: 9332
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vector2 minMax;

	// Token: 0x04002475 RID: 9333
	[PublicizedFrom(EAccessModifier.Protected)]
	public GameRandom rand;

	// Token: 0x04002476 RID: 9334
	[PublicizedFrom(EAccessModifier.Protected)]
	public string valueText;
}
