using System;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x0200058E RID: 1422
[Preserve]
public class LootEntryRequirementProgression : BaseOperationLootEntryRequirement
{
	// Token: 0x06002DC8 RID: 11720 RVA: 0x001312FF File Offset: 0x0012F4FF
	public override void Init(XElement e)
	{
		base.Init(e);
		e.ParseAttribute("name", ref this.progressionName);
		e.ParseAttribute("value", ref this.valueText);
	}

	// Token: 0x06002DC9 RID: 11721 RVA: 0x00131338 File Offset: 0x0012F538
	[PublicizedFrom(EAccessModifier.Protected)]
	public override float LeftSide(EntityPlayer player)
	{
		if (player != null)
		{
			ProgressionValue progressionValue = player.Progression.GetProgressionValue(this.progressionName);
			if (progressionValue != null)
			{
				return progressionValue.GetCalculatedLevel(player);
			}
		}
		return 0f;
	}

	// Token: 0x06002DCA RID: 11722 RVA: 0x00131370 File Offset: 0x0012F570
	[PublicizedFrom(EAccessModifier.Protected)]
	public override float RightSide(EntityPlayer player)
	{
		return StringParsers.ParseFloat(this.valueText, 0, -1, NumberStyles.Any);
	}

	// Token: 0x04002471 RID: 9329
	[PublicizedFrom(EAccessModifier.Protected)]
	public string progressionName;

	// Token: 0x04002472 RID: 9330
	[PublicizedFrom(EAccessModifier.Protected)]
	public string valueText;
}
