using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x020015FA RID: 5626
	[Preserve]
	public class ChallengeObjectiveSpendSkillPoint : BaseChallengeObjective
	{
		// Token: 0x1700136F RID: 4975
		// (get) Token: 0x0600AD10 RID: 44304 RVA: 0x0043D2F4 File Offset: 0x0043B4F4
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.SpendSkillPoint;
			}
		}

		// Token: 0x17001370 RID: 4976
		// (get) Token: 0x0600AD11 RID: 44305 RVA: 0x0043D2F8 File Offset: 0x0043B4F8
		public override string DescriptionText
		{
			get
			{
				return string.Format(Localization.Get("ObjectiveSpendSkillPoints_keyword", false), Localization.Get("goAnyValue", false)) + ":";
			}
		}

		// Token: 0x0600AD12 RID: 44306 RVA: 0x0043D31F File Offset: 0x0043B51F
		public override void HandleOnCreated()
		{
			base.HandleOnCreated();
			this.CreateRequirements();
		}

		// Token: 0x0600AD13 RID: 44307 RVA: 0x0043D32D File Offset: 0x0043B52D
		[PublicizedFrom(EAccessModifier.Private)]
		public void CreateRequirements()
		{
			if (!this.ShowRequirements)
			{
				return;
			}
			this.Owner.SetRequirementGroup(new RequirementObjectiveGroupWindowOpen("Skills"));
		}

		// Token: 0x0600AD14 RID: 44308 RVA: 0x0043D34D File Offset: 0x0043B54D
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.SkillPointSpent += this.Current_SkillPointSpent;
		}

		// Token: 0x0600AD15 RID: 44309 RVA: 0x0043D365 File Offset: 0x0043B565
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.SkillPointSpent -= this.Current_SkillPointSpent;
		}

		// Token: 0x0600AD16 RID: 44310 RVA: 0x0043D380 File Offset: 0x0043B580
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_SkillPointSpent(string skillName)
		{
			if (this.progressionName == "" || this.progressionName.EqualsCaseInsensitive(skillName))
			{
				int num = base.Current;
				base.Current = num + 1;
			}
			if (base.Current >= this.MaxCount)
			{
				base.Current = this.MaxCount;
				this.CheckObjectiveComplete(true);
			}
		}

		// Token: 0x0600AD17 RID: 44311 RVA: 0x0043D3DF File Offset: 0x0043B5DF
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("skill_name"))
			{
				this.progressionName = e.GetAttribute("skill_name");
			}
		}

		// Token: 0x0600AD18 RID: 44312 RVA: 0x0043D410 File Offset: 0x0043B610
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveSpendSkillPoint
			{
				progressionName = this.progressionName
			};
		}

		// Token: 0x04008695 RID: 34453
		[PublicizedFrom(EAccessModifier.Private)]
		public string progressionName = "";
	}
}
