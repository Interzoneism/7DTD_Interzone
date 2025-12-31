using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x020015F7 RID: 5623
	[Preserve]
	public class ChallengeObjectiveMeetTrader : BaseChallengeObjective
	{
		// Token: 0x17001369 RID: 4969
		// (get) Token: 0x0600ACF5 RID: 44277 RVA: 0x000ADB75 File Offset: 0x000ABD75
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.MeetTrader;
			}
		}

		// Token: 0x1700136A RID: 4970
		// (get) Token: 0x0600ACF6 RID: 44278 RVA: 0x0043CE5C File Offset: 0x0043B05C
		public override string DescriptionText
		{
			get
			{
				if (string.IsNullOrEmpty(this.TraderName))
				{
					return Localization.Get("challengeObjectiveMeetAnyTrader", false);
				}
				return Localization.Get("challengeObjectiveMeet", false) + " " + Localization.Get(this.TraderName, false) + ":";
			}
		}

		// Token: 0x0600ACF7 RID: 44279 RVA: 0x00002914 File Offset: 0x00000B14
		public override void Init()
		{
		}

		// Token: 0x0600ACF8 RID: 44280 RVA: 0x0043CEA8 File Offset: 0x0043B0A8
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.NPCMeet += this.Current_NPCMeet;
		}

		// Token: 0x0600ACF9 RID: 44281 RVA: 0x0043CEC0 File Offset: 0x0043B0C0
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.NPCMeet -= this.Current_NPCMeet;
		}

		// Token: 0x0600ACFA RID: 44282 RVA: 0x0043CED8 File Offset: 0x0043B0D8
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_NPCMeet(EntityNPC npc)
		{
			if (this.CheckBaseRequirements())
			{
				return;
			}
			if (this.TraderName == "" || npc.EntityName == this.TraderName)
			{
				int num = base.Current;
				base.Current = num + 1;
				if (base.Current >= this.MaxCount)
				{
					base.Current = this.MaxCount;
					this.CheckObjectiveComplete(true);
				}
			}
		}

		// Token: 0x0600ACFB RID: 44283 RVA: 0x0043CF45 File Offset: 0x0043B145
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("trader_name"))
			{
				this.TraderName = e.GetAttribute("trader_name");
			}
		}

		// Token: 0x0600ACFC RID: 44284 RVA: 0x0043CF76 File Offset: 0x0043B176
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveMeetTrader
			{
				TraderName = this.TraderName
			};
		}

		// Token: 0x0400868D RID: 34445
		[PublicizedFrom(EAccessModifier.Private)]
		public string TraderName = "";
	}
}
