using System;
using UnityEngine.Scripting;

namespace Quests.Requirements
{
	// Token: 0x020015B3 RID: 5555
	[Preserve]
	public class RequirementBuff : BaseRequirement
	{
		// Token: 0x0600AAAF RID: 43695 RVA: 0x00433CA4 File Offset: 0x00431EA4
		public override void SetupRequirement()
		{
			string arg = Localization.Get("RequirementBuff_keyword", false);
			base.Description = string.Format("{0} {1}", arg, BuffManager.GetBuff(base.ID).Name);
		}

		// Token: 0x0600AAB0 RID: 43696 RVA: 0x00433CDE File Offset: 0x00431EDE
		public override bool CheckRequirement()
		{
			return !base.OwnerQuest.Active || base.OwnerQuest.OwnerJournal.OwnerPlayer.Buffs.HasBuff(base.ID);
		}

		// Token: 0x0600AAB1 RID: 43697 RVA: 0x00433D14 File Offset: 0x00431F14
		public override BaseRequirement Clone()
		{
			return new RequirementBuff
			{
				ID = base.ID,
				Value = base.Value,
				Phase = base.Phase
			};
		}

		// Token: 0x0400854C RID: 34124
		[PublicizedFrom(EAccessModifier.Private)]
		public string name = "";
	}
}
