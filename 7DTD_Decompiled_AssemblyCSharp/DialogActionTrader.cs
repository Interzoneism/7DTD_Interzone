using System;
using UnityEngine.Scripting;

// Token: 0x020002A1 RID: 673
[Preserve]
public class DialogActionTrader : BaseDialogAction
{
	// Token: 0x17000206 RID: 518
	// (get) Token: 0x0600130F RID: 4879 RVA: 0x00075CC0 File Offset: 0x00073EC0
	public override BaseDialogAction.ActionTypes ActionType
	{
		get
		{
			return BaseDialogAction.ActionTypes.Trader;
		}
	}

	// Token: 0x06001310 RID: 4880 RVA: 0x00075CC4 File Offset: 0x00073EC4
	public override void PerformAction(EntityPlayer player)
	{
		EntityNPC respondent = LocalPlayerUI.GetUIForPlayer(player as EntityPlayerLocal).xui.Dialog.Respondent;
		if (respondent != null)
		{
			if (base.ID.EqualsCaseInsensitive("restock"))
			{
				(respondent as EntityTrader).TileEntityTrader.TraderData.lastInventoryUpdate = 0UL;
				return;
			}
			if (base.ID.EqualsCaseInsensitive("trade"))
			{
				(respondent as EntityTrader).StartTrading(player);
				return;
			}
			if (base.ID.EqualsCaseInsensitive("reset_quests"))
			{
				if (respondent is EntityTrader)
				{
					(respondent as EntityTrader).ClearActiveQuests(player.entityId);
					return;
				}
			}
			else
			{
				if (base.ID.EqualsCaseInsensitive("drone_storage"))
				{
					(respondent as EntityDrone).OpenStorage(player);
					return;
				}
				if (base.ID.EqualsCaseInsensitive("drone_follow"))
				{
					(respondent as EntityDrone).FollowMode();
					return;
				}
				if (base.ID.EqualsCaseInsensitive("drone_sentry"))
				{
					(respondent as EntityDrone).SentryMode();
					return;
				}
				if (base.ID.EqualsCaseInsensitive("drone_heal"))
				{
					(respondent as EntityDrone).HealOwner();
					return;
				}
				if (base.ID.EqualsCaseInsensitive("drone_dont_heal_allies") || base.ID.EqualsCaseInsensitive("drone_heal_allies"))
				{
					(respondent as EntityDrone).ToggleHealAllies();
				}
			}
		}
	}

	// Token: 0x04000C91 RID: 3217
	[PublicizedFrom(EAccessModifier.Private)]
	public string name = "";
}
