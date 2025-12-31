using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000652 RID: 1618
[Preserve]
public class MinEventActionSetOverrideLoot : MinEventActionTargetedBase
{
	// Token: 0x0600312E RID: 12590 RVA: 0x0014F390 File Offset: 0x0014D590
	public override void Execute(MinEventParams _params)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return;
		}
		for (int i = 0; i < this.targets.Count; i++)
		{
			EntityPlayer entityPlayer = this.targets[i] as EntityPlayer;
			if (entityPlayer != null)
			{
				if (this.altLoot == "")
				{
					LootContainer.OverrideItems.Remove(entityPlayer);
				}
				else if (LootContainer.OverrideItems.ContainsKey(entityPlayer))
				{
					LootContainer.OverrideItems[entityPlayer] = this.altLoot.Split(',', StringSplitOptions.None);
				}
				else
				{
					LootContainer.OverrideItems.Add(entityPlayer, this.altLoot.Split(',', StringSplitOptions.None));
				}
			}
		}
	}

	// Token: 0x0600312F RID: 12591 RVA: 0x0014F43C File Offset: 0x0014D63C
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "items")
		{
			this.altLoot = _attribute.Value;
			return true;
		}
		return flag;
	}

	// Token: 0x04002785 RID: 10117
	[PublicizedFrom(EAccessModifier.Private)]
	public string altLoot = "";
}
