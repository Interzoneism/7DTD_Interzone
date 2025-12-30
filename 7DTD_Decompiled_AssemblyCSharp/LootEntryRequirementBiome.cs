using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x0200058C RID: 1420
[Preserve]
public class LootEntryRequirementBiome : BaseLootEntryRequirement
{
	// Token: 0x06002DC1 RID: 11713 RVA: 0x00131220 File Offset: 0x0012F420
	public override void Init(XElement e)
	{
		base.Init(e);
		string text = "";
		if (e.ParseAttribute("biomes", ref text))
		{
			this.biomes = text.Split(',', StringSplitOptions.None);
			return;
		}
		this.biomes = new string[0];
	}

	// Token: 0x06002DC2 RID: 11714 RVA: 0x0013126A File Offset: 0x0012F46A
	public override bool CheckRequirement(EntityPlayer player)
	{
		return this.biomes.ContainsCaseInsensitive(player.biomeStandingOn.m_sBiomeName);
	}

	// Token: 0x0400246E RID: 9326
	[PublicizedFrom(EAccessModifier.Private)]
	public string[] biomes;
}
