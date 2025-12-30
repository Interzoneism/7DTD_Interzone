using System;
using UnityEngine.Scripting;

namespace Quests.Requirements
{
	// Token: 0x020015B6 RID: 5558
	[Preserve]
	public class RequirementHolding : BaseRequirement
	{
		// Token: 0x0600AAB9 RID: 43705 RVA: 0x00433EF4 File Offset: 0x004320F4
		public override void SetupRequirement()
		{
			XUi xui = LocalPlayerUI.GetUIForPlayer(base.OwnerQuest.OwnerJournal.OwnerPlayer).xui;
			string arg = Localization.Get("RequirementHolding_keyword", false);
			this.expectedItem = ((base.ID != "" && base.ID != null) ? ItemClass.GetItem(base.ID, false) : xui.PlayerInventory.Toolbelt.GetBareHandItemValue());
			this.expectedItemClass = ((base.ID != "" && base.ID != null) ? ItemClass.GetItemClass(base.ID, false) : xui.PlayerInventory.Toolbelt.GetBareHandItem());
			if (base.ID == "" || base.ID == null)
			{
				base.Description = "Bare Hands";
				return;
			}
			base.Description = string.Format("{0} {1}", arg, this.expectedItemClass.GetLocalizedItemName());
		}

		// Token: 0x0600AABA RID: 43706 RVA: 0x00433FE8 File Offset: 0x004321E8
		public override bool CheckRequirement()
		{
			return !base.OwnerQuest.Active || LocalPlayerUI.GetUIForPlayer(base.OwnerQuest.OwnerJournal.OwnerPlayer).xui.PlayerInventory.Toolbelt.holdingItemStack.itemValue.type == this.expectedItem.type;
		}

		// Token: 0x0600AABB RID: 43707 RVA: 0x00434044 File Offset: 0x00432244
		public override BaseRequirement Clone()
		{
			return new RequirementHolding
			{
				ID = base.ID,
				Value = base.Value,
				Phase = base.Phase
			};
		}

		// Token: 0x04008552 RID: 34130
		[PublicizedFrom(EAccessModifier.Private)]
		public ItemValue expectedItem = ItemValue.None.Clone();

		// Token: 0x04008553 RID: 34131
		[PublicizedFrom(EAccessModifier.Private)]
		public ItemClass expectedItemClass;
	}
}
