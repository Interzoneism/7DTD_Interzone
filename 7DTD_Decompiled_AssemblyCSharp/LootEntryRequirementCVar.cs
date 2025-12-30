using System;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x0200058D RID: 1421
[Preserve]
public class LootEntryRequirementCVar : BaseOperationLootEntryRequirement
{
	// Token: 0x06002DC4 RID: 11716 RVA: 0x0013128A File Offset: 0x0012F48A
	public override void Init(XElement e)
	{
		base.Init(e);
		e.ParseAttribute("cvar", ref this.cvar);
		e.ParseAttribute("value", ref this.valueText);
	}

	// Token: 0x06002DC5 RID: 11717 RVA: 0x001312C1 File Offset: 0x0012F4C1
	[PublicizedFrom(EAccessModifier.Protected)]
	public override float LeftSide(EntityPlayer player)
	{
		if (player != null)
		{
			return player.Buffs.GetCustomVar(this.cvar);
		}
		return 0f;
	}

	// Token: 0x06002DC6 RID: 11718 RVA: 0x001312E3 File Offset: 0x0012F4E3
	[PublicizedFrom(EAccessModifier.Protected)]
	public override float RightSide(EntityPlayer player)
	{
		return StringParsers.ParseFloat(this.valueText, 0, -1, NumberStyles.Any);
	}

	// Token: 0x0400246F RID: 9327
	[PublicizedFrom(EAccessModifier.Protected)]
	public string cvar;

	// Token: 0x04002470 RID: 9328
	[PublicizedFrom(EAccessModifier.Protected)]
	public string valueText;
}
