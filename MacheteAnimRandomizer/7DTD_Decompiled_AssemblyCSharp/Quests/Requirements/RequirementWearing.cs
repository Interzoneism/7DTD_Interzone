using System;
using UnityEngine.Scripting;

namespace Quests.Requirements
{
	// Token: 0x020015B8 RID: 5560
	[Preserve]
	public class RequirementWearing : BaseRequirement
	{
		// Token: 0x0600AAC1 RID: 43713 RVA: 0x00434134 File Offset: 0x00432334
		public override void SetupRequirement()
		{
			string arg = Localization.Get("RequirementWearing_keyword", false);
			this.expectedItem = ItemClass.GetItem(base.ID, false);
			this.expectedItemClass = ItemClass.GetItemClass(base.ID, false);
			base.Description = string.Format("{0} {1}", arg, this.expectedItemClass.GetLocalizedItemName());
		}

		// Token: 0x0600AAC2 RID: 43714 RVA: 0x0043418D File Offset: 0x0043238D
		public override bool CheckRequirement()
		{
			return !base.OwnerQuest.Active || LocalPlayerUI.GetUIForPlayer(base.OwnerQuest.OwnerJournal.OwnerPlayer).xui.PlayerEquipment.IsWearing(this.expectedItem);
		}

		// Token: 0x0600AAC3 RID: 43715 RVA: 0x004341C8 File Offset: 0x004323C8
		public override BaseRequirement Clone()
		{
			return new RequirementWearing
			{
				ID = base.ID,
				Value = base.Value,
				Phase = base.Phase
			};
		}

		// Token: 0x04008555 RID: 34133
		[PublicizedFrom(EAccessModifier.Private)]
		public ItemValue expectedItem = ItemValue.None.Clone();

		// Token: 0x04008556 RID: 34134
		[PublicizedFrom(EAccessModifier.Private)]
		public ItemClass expectedItemClass;
	}
}
