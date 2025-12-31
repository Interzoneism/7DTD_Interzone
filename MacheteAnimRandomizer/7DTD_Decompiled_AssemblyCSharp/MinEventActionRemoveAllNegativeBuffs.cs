using System;
using UnityEngine.Scripting;

// Token: 0x02000628 RID: 1576
[Preserve]
public class MinEventActionRemoveAllNegativeBuffs : MinEventActionTargetedBase
{
	// Token: 0x06003098 RID: 12440 RVA: 0x0014BD90 File Offset: 0x00149F90
	public override void Execute(MinEventParams _params)
	{
		for (int i = 0; i < this.targets.Count; i++)
		{
			for (int j = 0; j < this.targets[i].Buffs.ActiveBuffs.Count; j++)
			{
				if (this.targets[i].Buffs.ActiveBuffs[j].BuffClass.DamageType != EnumDamageTypes.None)
				{
					this.targets[i].Buffs.ActiveBuffs[j].Remove = true;
				}
			}
		}
	}
}
