using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x0200164E RID: 5710
	[Preserve]
	public class ActionAddXP : ActionBaseClientAction
	{
		// Token: 0x0600AEDF RID: 44767 RVA: 0x00444664 File Offset: 0x00442864
		public override void OnClientPerform(Entity target)
		{
			EntityPlayer entityPlayer = target as EntityPlayer;
			if (entityPlayer != null)
			{
				this.xpAmount = GameEventManager.GetIntValue(entityPlayer, this.xpAmountText, 0);
				if (this.xpAmount < 0)
				{
					this.xpAmount = 0;
				}
				entityPlayer.Progression.AddLevelExp(this.xpAmount, "_xpOther", Progression.XPTypes.Other, true, true);
			}
		}

		// Token: 0x0600AEE0 RID: 44768 RVA: 0x004446B8 File Offset: 0x004428B8
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionAddXP.PropXPAmount, ref this.xpAmountText);
		}

		// Token: 0x0600AEE1 RID: 44769 RVA: 0x004446D2 File Offset: 0x004428D2
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionAddXP
			{
				xpAmountText = this.xpAmountText
			};
		}

		// Token: 0x040087DF RID: 34783
		[PublicizedFrom(EAccessModifier.Protected)]
		public string xpAmountText;

		// Token: 0x040087E0 RID: 34784
		[PublicizedFrom(EAccessModifier.Protected)]
		public int xpAmount;

		// Token: 0x040087E1 RID: 34785
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropXPAmount = "xp_amount";
	}
}
