using System;
using UnityEngine.Scripting;

// Token: 0x02000BDB RID: 3035
[Preserve]
public class CreativeActionEntryFavorite : BaseItemActionEntry
{
	// Token: 0x06005D69 RID: 23913 RVA: 0x0025DC31 File Offset: 0x0025BE31
	public CreativeActionEntryFavorite(XUiController controller, int stackID) : base(controller, "lblContextActionFavorite", "server_favorite", BaseItemActionEntry.GamepadShortCut.DPadRight, "crafting/craft_click_craft", "ui/ui_denied")
	{
		this.StackID = (ushort)stackID;
	}

	// Token: 0x06005D6A RID: 23914 RVA: 0x0025DC58 File Offset: 0x0025BE58
	public override void OnActivated()
	{
		EntityPlayer entityPlayer = base.ItemController.xui.playerUI.entityPlayer;
		if (entityPlayer.favoriteCreativeStacks.Contains(this.StackID))
		{
			entityPlayer.favoriteCreativeStacks.Remove(this.StackID);
		}
		else
		{
			entityPlayer.favoriteCreativeStacks.Add(this.StackID);
		}
		XUiC_Creative2Window childByType = base.ItemController.WindowGroup.Controller.GetChildByType<XUiC_Creative2Window>();
		if (childByType == null)
		{
			return;
		}
		childByType.RefreshView();
	}

	// Token: 0x040046AB RID: 18091
	[PublicizedFrom(EAccessModifier.Private)]
	public ushort StackID;
}
