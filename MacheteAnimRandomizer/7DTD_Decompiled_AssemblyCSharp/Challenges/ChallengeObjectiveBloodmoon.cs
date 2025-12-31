using System;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x020015E8 RID: 5608
	[Preserve]
	public class ChallengeObjectiveBloodmoon : BaseChallengeObjective
	{
		// Token: 0x17001349 RID: 4937
		// (get) Token: 0x0600AC47 RID: 44103 RVA: 0x00075C39 File Offset: 0x00073E39
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.Bloodmoon;
			}
		}

		// Token: 0x1700134A RID: 4938
		// (get) Token: 0x0600AC48 RID: 44104 RVA: 0x0043A09B File Offset: 0x0043829B
		public override string DescriptionText
		{
			get
			{
				return Localization.Get("challengeObjectiveBloodMoonCompleted", false) + ":";
			}
		}

		// Token: 0x0600AC49 RID: 44105 RVA: 0x00002914 File Offset: 0x00000B14
		public override void Init()
		{
		}

		// Token: 0x0600AC4A RID: 44106 RVA: 0x0043A0B2 File Offset: 0x004382B2
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.BloodMoonSurvive += this.Current_BloodMoonSurvive;
		}

		// Token: 0x0600AC4B RID: 44107 RVA: 0x0043A0CA File Offset: 0x004382CA
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.BloodMoonSurvive -= this.Current_BloodMoonSurvive;
		}

		// Token: 0x0600AC4C RID: 44108 RVA: 0x0043A0E4 File Offset: 0x004382E4
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_BloodMoonSurvive()
		{
			if (this.CheckBaseRequirements())
			{
				return;
			}
			int num = base.Current;
			base.Current = num + 1;
			this.CheckObjectiveComplete(true);
		}

		// Token: 0x0600AC4D RID: 44109 RVA: 0x0043A112 File Offset: 0x00438312
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveBloodmoon();
		}
	}
}
