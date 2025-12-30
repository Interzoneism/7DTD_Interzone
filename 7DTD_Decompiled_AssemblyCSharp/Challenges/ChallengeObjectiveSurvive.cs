using System;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x020015FB RID: 5627
	[Preserve]
	public class ChallengeObjectiveSurvive : BaseChallengeObjective
	{
		// Token: 0x17001371 RID: 4977
		// (get) Token: 0x0600AD1A RID: 44314 RVA: 0x00234AD1 File Offset: 0x00232CD1
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.Survive;
			}
		}

		// Token: 0x17001372 RID: 4978
		// (get) Token: 0x0600AD1B RID: 44315 RVA: 0x0043D436 File Offset: 0x0043B636
		public override string DescriptionText
		{
			get
			{
				return Localization.Get("challengeObjectiveSurvive", false) + ":";
			}
		}

		// Token: 0x17001373 RID: 4979
		// (get) Token: 0x0600AD1C RID: 44316 RVA: 0x0043D44D File Offset: 0x0043B64D
		public override string StatusText
		{
			get
			{
				return string.Format("{0}/{1}", XUiM_PlayerBuffs.GetTimeString((float)this.current * 60f), XUiM_PlayerBuffs.GetTimeString((float)this.MaxCount * 60f));
			}
		}

		// Token: 0x0600AD1D RID: 44317 RVA: 0x00002914 File Offset: 0x00000B14
		public override void Init()
		{
		}

		// Token: 0x0600AD1E RID: 44318 RVA: 0x0043D480 File Offset: 0x0043B680
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.TimeSurvive += this.Current_TimeSurvive;
			base.Current = (int)base.Player.longestLife;
			if (base.Current >= this.MaxCount)
			{
				base.Current = this.MaxCount;
				this.CheckObjectiveComplete(true);
			}
		}

		// Token: 0x0600AD1F RID: 44319 RVA: 0x0043D4D7 File Offset: 0x0043B6D7
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.TimeSurvive -= this.Current_TimeSurvive;
		}

		// Token: 0x0600AD20 RID: 44320 RVA: 0x0043D4EF File Offset: 0x0043B6EF
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_TimeSurvive(float val)
		{
			if (this.CheckBaseRequirements())
			{
				return;
			}
			base.Current = (int)val;
			if (base.Current >= this.MaxCount)
			{
				base.Current = this.MaxCount;
				this.CheckObjectiveComplete(true);
			}
		}

		// Token: 0x0600AD21 RID: 44321 RVA: 0x0043D524 File Offset: 0x0043B724
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveSurvive();
		}
	}
}
