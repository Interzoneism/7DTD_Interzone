using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using UniLinq;

namespace Challenges
{
	// Token: 0x020015DF RID: 5599
	public class ChallengeGroup
	{
		// Token: 0x0600ABE9 RID: 44009 RVA: 0x00438C72 File Offset: 0x00436E72
		public ChallengeGroup(string name)
		{
			this.Name = name;
		}

		// Token: 0x0600ABEA RID: 44010 RVA: 0x00438CA8 File Offset: 0x00436EA8
		public static ChallengeGroup NewClass(string id)
		{
			if (ChallengeGroup.s_ChallengeGroups.ContainsKey(id))
			{
				return null;
			}
			ChallengeGroup challengeGroup = new ChallengeGroup(id.ToLower());
			ChallengeGroup.s_ChallengeGroups[id] = challengeGroup;
			return challengeGroup;
		}

		// Token: 0x0600ABEB RID: 44011 RVA: 0x00438CDD File Offset: 0x00436EDD
		public static ChallengeGroup GetGroup(string id)
		{
			if (!ChallengeGroup.s_ChallengeGroups.ContainsKey(id))
			{
				return null;
			}
			return ChallengeGroup.s_ChallengeGroups[id];
		}

		// Token: 0x0600ABEC RID: 44012 RVA: 0x00438CF9 File Offset: 0x00436EF9
		public void AddChallengeCount(string tag, int count)
		{
			if (this.ChallengeCounts == null)
			{
				this.ChallengeCounts = new List<ChallengeGroup.ChallengeCount>();
			}
			this.ChallengeCounts.Add(new ChallengeGroup.ChallengeCount
			{
				Tags = FastTags<TagGroup.Global>.Parse(tag),
				Count = count
			});
		}

		// Token: 0x0600ABED RID: 44013 RVA: 0x00438D31 File Offset: 0x00436F31
		public void AddChallenge(ChallengeClass challenge)
		{
			this.ChallengeClasses.Add(challenge);
		}

		// Token: 0x0600ABEE RID: 44014 RVA: 0x00438D40 File Offset: 0x00436F40
		public void ParseElement(XElement e)
		{
			if (e.HasAttribute("title_key"))
			{
				this.Title = Localization.Get(e.GetAttribute("title_key"), false);
			}
			else if (e.HasAttribute("title"))
			{
				this.Title = e.GetAttribute("title");
			}
			else
			{
				this.Title = this.Name;
			}
			if (e.HasAttribute("category"))
			{
				this.Category = e.GetAttribute("category");
			}
			if (e.HasAttribute("reward_event"))
			{
				this.RewardEvent = e.GetAttribute("reward_event");
			}
			if (e.HasAttribute("reward_text_key"))
			{
				this.RewardText = Localization.Get(e.GetAttribute("reward_text_key"), false);
			}
			else if (e.HasAttribute("reward_text"))
			{
				this.RewardText = e.GetAttribute("reward_text");
			}
			if (e.HasAttribute("objective_text_key"))
			{
				this.ObjectiveText = Localization.Get(e.GetAttribute("objective_text_key"), false);
			}
			else if (e.HasAttribute("objective_text"))
			{
				this.ObjectiveText = e.GetAttribute("objective_text");
			}
			if (e.HasAttribute("active_challenge_count"))
			{
				this.ActiveChallengeCount = StringParsers.ParseSInt32(e.GetAttribute("active_challenge_count"), 0, -1, NumberStyles.Integer);
			}
			if (e.HasAttribute("day_reset"))
			{
				this.DayReset = StringParsers.ParseSInt32(e.GetAttribute("day_reset"), 0, -1, NumberStyles.Integer);
			}
			if (e.HasAttribute("is_random"))
			{
				this.IsRandom = StringParsers.ParseBool(e.GetAttribute("is_random"), 0, -1, true);
			}
			if (e.HasAttribute("link_challenges"))
			{
				this.LinkChallenges = StringParsers.ParseBool(e.GetAttribute("link_challenges"), 0, -1, true);
			}
			if (e.HasAttribute("hidden_by"))
			{
				this.HiddenBy = e.GetAttribute("hidden_by");
			}
		}

		// Token: 0x0600ABEF RID: 44015 RVA: 0x00438F9C File Offset: 0x0043719C
		public bool IsVisible(EntityPlayer player)
		{
			if (this.HiddenBy == "")
			{
				return true;
			}
			if (ChallengeGroup.s_ChallengeGroups.ContainsKey(this.HiddenBy))
			{
				return ChallengeGroup.s_ChallengeGroups[this.HiddenBy].IsComplete;
			}
			return ChallengeCategory.s_ChallengeCategories[this.Category].CanShow(player);
		}

		// Token: 0x0600ABF0 RID: 44016 RVA: 0x00438FFB File Offset: 0x004371FB
		public bool HasEventsOrPassives()
		{
			return this.Effects != null;
		}

		// Token: 0x0600ABF1 RID: 44017 RVA: 0x00439006 File Offset: 0x00437206
		public void ModifyValue(EntityAlive _ea, PassiveEffects _effect, ref float _base_value, ref float _perc_value, FastTags<TagGroup.Global> _tags = default(FastTags<TagGroup.Global>))
		{
			if (this.Effects == null || !this.IsComplete)
			{
				return;
			}
			this.Effects.ModifyValue(_ea, _effect, ref _base_value, ref _perc_value, 0f, _tags, 1);
		}

		// Token: 0x0600ABF2 RID: 44018 RVA: 0x00439034 File Offset: 0x00437234
		public List<ChallengeClass> GetChallengeClassesForCreate()
		{
			List<ChallengeClass> list = new List<ChallengeClass>();
			for (int i = 0; i < this.ChallengeClasses.Count; i++)
			{
				list.Add(this.ChallengeClasses[i]);
			}
			GameRandom gameRandom = GameManager.Instance.World.GetGameRandom();
			for (int j = 0; j < list.Count * 2; j++)
			{
				int index = gameRandom.RandomRange(list.Count);
				int index2 = gameRandom.RandomRange(list.Count);
				ChallengeClass value = list[index];
				list[index] = list[index2];
				list[index2] = value;
			}
			if (this.ChallengeCounts != null)
			{
				for (int k = 0; k < this.ChallengeCounts.Count; k++)
				{
					ChallengeGroup.ChallengeCount challengeCount = this.ChallengeCounts[k];
					int num = challengeCount.Count;
					for (int l = list.Count - 1; l >= 0; l--)
					{
						if (list[l].Tags.Test_AnySet(challengeCount.Tags))
						{
							if (num == 0)
							{
								list.RemoveAt(l);
							}
							else
							{
								num--;
							}
						}
					}
				}
				list = (from c in list
				orderby c.TagName
				select c).ToList<ChallengeClass>();
			}
			return list;
		}

		// Token: 0x04008607 RID: 34311
		public static Dictionary<string, ChallengeGroup> s_ChallengeGroups = new CaseInsensitiveStringDictionary<ChallengeGroup>();

		// Token: 0x04008608 RID: 34312
		public string Name;

		// Token: 0x04008609 RID: 34313
		public string Title;

		// Token: 0x0400860A RID: 34314
		public bool IsComplete;

		// Token: 0x0400860B RID: 34315
		public string RewardEvent;

		// Token: 0x0400860C RID: 34316
		public string RewardText;

		// Token: 0x0400860D RID: 34317
		public string ObjectiveText;

		// Token: 0x0400860E RID: 34318
		public bool IsRandom;

		// Token: 0x0400860F RID: 34319
		public int ActiveChallengeCount = 10;

		// Token: 0x04008610 RID: 34320
		public int DayReset = -1;

		// Token: 0x04008611 RID: 34321
		public bool LinkChallenges;

		// Token: 0x04008612 RID: 34322
		public string Category;

		// Token: 0x04008613 RID: 34323
		public string HiddenBy = "";

		// Token: 0x04008614 RID: 34324
		public bool UIDirty;

		// Token: 0x04008615 RID: 34325
		public List<ChallengeGroup.ChallengeCount> ChallengeCounts;

		// Token: 0x04008616 RID: 34326
		public List<ChallengeClass> ChallengeClasses = new List<ChallengeClass>();

		// Token: 0x04008617 RID: 34327
		public MinEffectController Effects;

		// Token: 0x020015E0 RID: 5600
		public class ChallengeCount
		{
			// Token: 0x04008618 RID: 34328
			public FastTags<TagGroup.Global> Tags;

			// Token: 0x04008619 RID: 34329
			public int Count;
		}
	}
}
