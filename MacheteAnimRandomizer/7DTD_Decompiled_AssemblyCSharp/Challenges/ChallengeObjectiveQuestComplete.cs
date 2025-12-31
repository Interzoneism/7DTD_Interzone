using System;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x020015F8 RID: 5624
	[Preserve]
	public class ChallengeObjectiveQuestComplete : BaseChallengeObjective
	{
		// Token: 0x1700136B RID: 4971
		// (get) Token: 0x0600ACFE RID: 44286 RVA: 0x001666F0 File Offset: 0x001648F0
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.QuestComplete;
			}
		}

		// Token: 0x1700136C RID: 4972
		// (get) Token: 0x0600ACFF RID: 44287 RVA: 0x0043CF9C File Offset: 0x0043B19C
		public override string DescriptionText
		{
			get
			{
				if (this.questText == "")
				{
					this.questText = Localization.Get("challengeTargetAnyQuest", false);
				}
				return this.questText + " " + Localization.Get("challengeObjectiveQuestCompleted", false) + ":";
			}
		}

		// Token: 0x0600AD00 RID: 44288 RVA: 0x00002914 File Offset: 0x00000B14
		public override void Init()
		{
		}

		// Token: 0x0600AD01 RID: 44289 RVA: 0x0043CFEC File Offset: 0x0043B1EC
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.QuestComplete += this.Current_QuestComplete;
		}

		// Token: 0x0600AD02 RID: 44290 RVA: 0x0043D004 File Offset: 0x0043B204
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.QuestComplete -= this.Current_QuestComplete;
		}

		// Token: 0x0600AD03 RID: 44291 RVA: 0x0043D01C File Offset: 0x0043B21C
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_QuestComplete(FastTags<TagGroup.Global> questTags, QuestClass questClass)
		{
			if (this.questTag.IsEmpty || questTags.Test_AnySet(this.questTag))
			{
				if (this.CheckBaseRequirements())
				{
					return;
				}
				int num = base.Current;
				base.Current = num + 1;
				if (base.Current >= this.MaxCount)
				{
					base.Current = this.MaxCount;
					this.CheckObjectiveComplete(true);
				}
			}
		}

		// Token: 0x0600AD04 RID: 44292 RVA: 0x0043D080 File Offset: 0x0043B280
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("quest_tag"))
			{
				this.questTagText = e.GetAttribute("quest_tag");
				this.questTag = FastTags<TagGroup.Global>.Parse(this.questTagText);
			}
			else
			{
				this.questTag = FastTags<TagGroup.Global>.none;
			}
			if (e.HasAttribute("quest_text_key"))
			{
				this.questText = Localization.Get(e.GetAttribute("quest_text_key"), false);
			}
			if (e.HasAttribute("tier"))
			{
				this.tier = StringParsers.ParseSInt32(e.GetAttribute("tier"), 0, -1, NumberStyles.Integer);
			}
		}

		// Token: 0x0600AD05 RID: 44293 RVA: 0x0043D138 File Offset: 0x0043B338
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveQuestComplete
			{
				questTag = this.questTag,
				questText = this.questText,
				tier = this.tier
			};
		}

		// Token: 0x0400868E RID: 34446
		[PublicizedFrom(EAccessModifier.Private)]
		public string questTagText;

		// Token: 0x0400868F RID: 34447
		[PublicizedFrom(EAccessModifier.Private)]
		public FastTags<TagGroup.Global> questTag = FastTags<TagGroup.Global>.none;

		// Token: 0x04008690 RID: 34448
		[PublicizedFrom(EAccessModifier.Private)]
		public int tier;

		// Token: 0x04008691 RID: 34449
		[PublicizedFrom(EAccessModifier.Private)]
		public string questText = "";
	}
}
