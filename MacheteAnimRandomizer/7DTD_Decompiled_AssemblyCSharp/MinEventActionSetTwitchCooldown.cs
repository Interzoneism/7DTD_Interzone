using System;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000646 RID: 1606
[Preserve]
public class MinEventActionSetTwitchCooldown : MinEventActionTargetedBase
{
	// Token: 0x170004B2 RID: 1202
	// (get) Token: 0x06003105 RID: 12549 RVA: 0x0014EB04 File Offset: 0x0014CD04
	// (set) Token: 0x06003106 RID: 12550 RVA: 0x0014EB0C File Offset: 0x0014CD0C
	public EntityPlayer.TwitchActionsStates state { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06003107 RID: 12551 RVA: 0x0014EB18 File Offset: 0x0014CD18
	public override void Execute(MinEventParams _params)
	{
		if (this.targets == null)
		{
			return;
		}
		for (int i = 0; i < this.targets.Count; i++)
		{
			EntityPlayer entityPlayer = this.targets[i] as EntityPlayer;
			if (entityPlayer != null && entityPlayer.TwitchActionsEnabled != EntityPlayer.TwitchActionsStates.Disabled)
			{
				entityPlayer.TwitchActionsEnabled = this.state;
			}
		}
	}

	// Token: 0x06003108 RID: 12552 RVA: 0x0014EB74 File Offset: 0x0014CD74
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "state")
		{
			this.state = (EntityPlayer.TwitchActionsStates)StringParsers.ParseSInt32(_attribute.Value, 0, -1, NumberStyles.Integer);
			if (this.state == EntityPlayer.TwitchActionsStates.Disabled)
			{
				this.state = EntityPlayer.TwitchActionsStates.TempDisabled;
			}
		}
		return flag;
	}
}
