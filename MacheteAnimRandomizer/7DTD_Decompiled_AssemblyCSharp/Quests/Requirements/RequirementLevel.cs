using System;
using UnityEngine.Scripting;

namespace Quests.Requirements
{
	// Token: 0x020015B7 RID: 5559
	[Preserve]
	public class RequirementLevel : BaseRequirement
	{
		// Token: 0x0600AABD RID: 43709 RVA: 0x00434088 File Offset: 0x00432288
		public override void SetupRequirement()
		{
			string arg = Localization.Get("RequirementLevel_keyword", false);
			this.expectedLevel = Convert.ToInt32(base.Value);
			base.Description = string.Format("{0} {1}", arg, this.expectedLevel);
		}

		// Token: 0x0600AABE RID: 43710 RVA: 0x004340CE File Offset: 0x004322CE
		public override bool CheckRequirement()
		{
			return !base.OwnerQuest.Active || XUiM_Player.GetLevel(base.OwnerQuest.OwnerJournal.OwnerPlayer) >= this.expectedLevel;
		}

		// Token: 0x0600AABF RID: 43711 RVA: 0x004340FF File Offset: 0x004322FF
		public override BaseRequirement Clone()
		{
			return new RequirementLevel
			{
				ID = base.ID,
				Value = base.Value,
				Phase = base.Phase
			};
		}

		// Token: 0x04008554 RID: 34132
		[PublicizedFrom(EAccessModifier.Private)]
		public int expectedLevel;
	}
}
