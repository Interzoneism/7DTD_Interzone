using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Platform;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x020015DD RID: 5597
	[Preserve]
	public class ChallengeClass
	{
		// Token: 0x17001338 RID: 4920
		// (get) Token: 0x0600ABD4 RID: 43988 RVA: 0x00438488 File Offset: 0x00436688
		// (set) Token: 0x0600ABD5 RID: 43989 RVA: 0x00438490 File Offset: 0x00436690
		public int OrderIndex { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001339 RID: 4921
		// (get) Token: 0x0600ABD6 RID: 43990 RVA: 0x00438499 File Offset: 0x00436699
		public bool HasNavType
		{
			get
			{
				return this.GetNavType() > ChallengeClass.UINavTypes.None;
			}
		}

		// Token: 0x0600ABD7 RID: 43991 RVA: 0x004384A4 File Offset: 0x004366A4
		public ChallengeClass(string name)
		{
			this.Name = name;
			this.OrderIndex = ChallengeClass.nextIndex++;
		}

		// Token: 0x0600ABD8 RID: 43992 RVA: 0x00438500 File Offset: 0x00436700
		public static ChallengeClass NewClass(string id)
		{
			if (ChallengeClass.s_Challenges.ContainsKey(id))
			{
				return null;
			}
			ChallengeClass challengeClass = new ChallengeClass(id.ToLower());
			ChallengeClass.s_Challenges[id] = challengeClass;
			return challengeClass;
		}

		// Token: 0x0600ABD9 RID: 43993 RVA: 0x00438535 File Offset: 0x00436735
		public static void Cleanup()
		{
			ChallengeClass.s_Challenges.Clear();
		}

		// Token: 0x0600ABDA RID: 43994 RVA: 0x00438544 File Offset: 0x00436744
		public static void InitChallenges()
		{
			foreach (string key in ChallengeClass.s_Challenges.Keys)
			{
				ChallengeClass.s_Challenges[key].Init();
			}
		}

		// Token: 0x0600ABDB RID: 43995 RVA: 0x004385A4 File Offset: 0x004367A4
		[PublicizedFrom(EAccessModifier.Private)]
		public void Init()
		{
			for (int i = 0; i < this.ObjectiveList.Count; i++)
			{
				this.ObjectiveList[i].BaseInit();
				if (this.ObjectiveList[i].NeedsConstantUIUpdate)
				{
					this.NeedsConstantUIUpdate = true;
				}
			}
		}

		// Token: 0x0600ABDC RID: 43996 RVA: 0x004385F2 File Offset: 0x004367F2
		public bool HasEventsOrPassives()
		{
			return this.Effects != null;
		}

		// Token: 0x0600ABDD RID: 43997 RVA: 0x004385FD File Offset: 0x004367FD
		public void ModifyValue(EntityAlive _ea, PassiveEffects _effect, ref float _base_value, ref float _perc_value, FastTags<TagGroup.Global> _tags = default(FastTags<TagGroup.Global>))
		{
			if (this.Effects == null)
			{
				return;
			}
			this.Effects.ModifyValue(_ea, _effect, ref _base_value, ref _perc_value, 0f, _tags, 1);
		}

		// Token: 0x0600ABDE RID: 43998 RVA: 0x00438620 File Offset: 0x00436820
		public static ChallengeClass GetChallenge(string name)
		{
			if (ChallengeClass.s_Challenges.ContainsKey(name))
			{
				return ChallengeClass.s_Challenges[name];
			}
			return null;
		}

		// Token: 0x0600ABDF RID: 43999 RVA: 0x0043863C File Offset: 0x0043683C
		public Challenge CreateChallenge(ChallengeJournal ownerJournal)
		{
			Challenge challenge = new Challenge();
			challenge.ChallengeClass = this;
			challenge.Owner = ownerJournal;
			for (int i = 0; i < this.ObjectiveList.Count; i++)
			{
				BaseChallengeObjective baseChallengeObjective = this.ObjectiveList[i];
				BaseChallengeObjective baseChallengeObjective2 = baseChallengeObjective.Clone();
				baseChallengeObjective2.Owner = challenge;
				baseChallengeObjective2.IsRequirement = baseChallengeObjective.IsRequirement;
				baseChallengeObjective2.MaxCount = baseChallengeObjective.MaxCount;
				baseChallengeObjective2.ShowRequirements = baseChallengeObjective.ShowRequirements;
				baseChallengeObjective2.Biome = baseChallengeObjective.Biome;
				baseChallengeObjective2.HandleOnCreated();
				challenge.ObjectiveList.Add(baseChallengeObjective2);
			}
			return challenge;
		}

		// Token: 0x0600ABE0 RID: 44000 RVA: 0x004386D1 File Offset: 0x004368D1
		public string GetNextChallengeName()
		{
			if (this.NextChallenge != null)
			{
				return this.NextChallenge.Name;
			}
			return "";
		}

		// Token: 0x0600ABE1 RID: 44001 RVA: 0x004386EC File Offset: 0x004368EC
		public void AddObjective(BaseChallengeObjective objective)
		{
			this.ObjectiveList.Add(objective);
			objective.OwnerClass = this;
		}

		// Token: 0x0600ABE2 RID: 44002 RVA: 0x00438704 File Offset: 0x00436904
		public bool ResetObjectives(Challenge challenge)
		{
			int count = challenge.ObjectiveList.Count;
			if (count > this.ObjectiveList.Count)
			{
				challenge.ObjectiveList.RemoveRange(this.ObjectiveList.Count, count - this.ObjectiveList.Count);
			}
			for (int i = 0; i < this.ObjectiveList.Count; i++)
			{
				BaseChallengeObjective baseChallengeObjective = this.ObjectiveList[i];
				if (i < count && baseChallengeObjective.GetType() != challenge.ObjectiveList[i].GetType())
				{
					return false;
				}
				BaseChallengeObjective baseChallengeObjective2 = baseChallengeObjective.Clone();
				baseChallengeObjective2.Owner = challenge;
				baseChallengeObjective2.IsRequirement = baseChallengeObjective.IsRequirement;
				baseChallengeObjective2.MaxCount = baseChallengeObjective.MaxCount;
				baseChallengeObjective2.ShowRequirements = baseChallengeObjective.ShowRequirements;
				baseChallengeObjective2.Biome = baseChallengeObjective.Biome;
				baseChallengeObjective2.HandleOnCreated();
				if (i < count)
				{
					BaseChallengeObjective obj = challenge.ObjectiveList[i];
					baseChallengeObjective2.CopyValues(obj, baseChallengeObjective);
					challenge.ObjectiveList[i] = baseChallengeObjective2;
					baseChallengeObjective2.Current = Utils.FastMin(baseChallengeObjective2.Current, baseChallengeObjective2.MaxCount);
				}
				else
				{
					challenge.ObjectiveList.Add(baseChallengeObjective2);
				}
				if (!challenge.IsActive)
				{
					baseChallengeObjective2.Current = baseChallengeObjective2.MaxCount;
					baseChallengeObjective2.Complete = true;
				}
			}
			return true;
		}

		// Token: 0x0600ABE3 RID: 44003 RVA: 0x00438848 File Offset: 0x00436A48
		public string GetHint(bool isPreReq)
		{
			if (this.ChallengeHint == null)
			{
				return "";
			}
			if (isPreReq)
			{
				if (PlatformManager.NativePlatform.Input.CurrentInputStyle != PlayerInputManager.InputStyle.Keyboard)
				{
					string key = this.PreReqChallengeHint + "_alt";
					if (Localization.Exists(key, false))
					{
						return Localization.Get(key, false);
					}
				}
				return Localization.Get(this.PreReqChallengeHint, false);
			}
			if (PlatformManager.NativePlatform.Input.CurrentInputStyle != PlayerInputManager.InputStyle.Keyboard)
			{
				string key2 = this.ChallengeHint + "_alt";
				if (Localization.Exists(key2, false))
				{
					return Localization.Get(key2, false);
				}
			}
			return Localization.Get(this.ChallengeHint, false);
		}

		// Token: 0x0600ABE4 RID: 44004 RVA: 0x004388E8 File Offset: 0x00436AE8
		public string GetDescription()
		{
			if (PlatformManager.NativePlatform.Input.CurrentInputStyle != PlayerInputManager.InputStyle.Keyboard)
			{
				string key = this.Description + "_alt";
				if (Localization.Exists(key, false))
				{
					return Localization.Get(key, false);
				}
			}
			return Localization.Get(this.Description, false);
		}

		// Token: 0x0600ABE5 RID: 44005 RVA: 0x00438935 File Offset: 0x00436B35
		public void FireEvent(MinEventTypes _eventType, MinEventParams _params)
		{
			if (this.Effects != null)
			{
				this.Effects.FireEvent(_eventType, _params);
			}
		}

		// Token: 0x0600ABE6 RID: 44006 RVA: 0x0043894C File Offset: 0x00436B4C
		public void ParseElement(XElement e)
		{
			if (e.HasAttribute("icon"))
			{
				this.Icon = e.GetAttribute("icon");
			}
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
			if (e.HasAttribute("group"))
			{
				ChallengeGroup challengeGroup = ChallengeGroup.s_ChallengeGroups[e.GetAttribute("group")];
				this.ChallengeGroup = challengeGroup;
				challengeGroup.AddChallenge(this);
			}
			if (e.HasAttribute("prerequisite_hint"))
			{
				this.PreReqChallengeHint = e.GetAttribute("prerequisite_hint");
			}
			if (e.HasAttribute("hint"))
			{
				this.ChallengeHint = e.GetAttribute("hint");
			}
			if (e.HasAttribute("short_description_key"))
			{
				this.ShortDescription = Localization.Get(e.GetAttribute("short_description_key"), false);
			}
			else if (e.HasAttribute("short_description"))
			{
				this.ShortDescription = e.GetAttribute("short_description");
			}
			if (e.HasAttribute("description_key"))
			{
				this.Description = e.GetAttribute("description_key");
			}
			else if (e.HasAttribute("description"))
			{
				this.Description = e.GetAttribute("description");
			}
			if (e.HasAttribute("reward_event"))
			{
				this.RewardEvent = e.GetAttribute("reward_event");
			}
			else
			{
				this.RewardEvent = ChallengesFromXml.DefaultRewardEvent;
			}
			if (e.HasAttribute("reward_text_key"))
			{
				this.RewardText = Localization.Get(e.GetAttribute("reward_text_key"), false);
			}
			else if (e.HasAttribute("reward_text"))
			{
				this.RewardText = e.GetAttribute("reward_text");
			}
			else
			{
				this.RewardText = ChallengesFromXml.DefaultRewardText;
			}
			if (e.HasAttribute("tags"))
			{
				this.TagName = e.GetAttribute("tags");
				this.Tags = FastTags<TagGroup.Global>.Parse(this.TagName);
			}
			if (e.HasAttribute("redeem_always"))
			{
				this.RedeemAlways = StringParsers.ParseBool(e.GetAttribute("redeem_always"), 0, -1, true);
			}
		}

		// Token: 0x0600ABE7 RID: 44007 RVA: 0x00438C20 File Offset: 0x00436E20
		public ChallengeClass.UINavTypes GetNavType()
		{
			for (int i = 0; i < this.ObjectiveList.Count; i++)
			{
				BaseChallengeObjective baseChallengeObjective = this.ObjectiveList[i];
				if (baseChallengeObjective.NavType != ChallengeClass.UINavTypes.None)
				{
					return baseChallengeObjective.NavType;
				}
			}
			return ChallengeClass.UINavTypes.None;
		}

		// Token: 0x040085EF RID: 34287
		public static Dictionary<string, ChallengeClass> s_Challenges = new CaseInsensitiveStringDictionary<ChallengeClass>();

		// Token: 0x040085F0 RID: 34288
		public string Name;

		// Token: 0x040085F1 RID: 34289
		public string Title;

		// Token: 0x040085F2 RID: 34290
		public string Icon;

		// Token: 0x040085F3 RID: 34291
		public ChallengeGroup ChallengeGroup;

		// Token: 0x040085F4 RID: 34292
		public string ShortDescription;

		// Token: 0x040085F5 RID: 34293
		public string Description;

		// Token: 0x040085F6 RID: 34294
		public string PreReqChallengeHint;

		// Token: 0x040085F7 RID: 34295
		public string ChallengeHint;

		// Token: 0x040085F8 RID: 34296
		public string RewardEvent;

		// Token: 0x040085F9 RID: 34297
		public string RewardText = "";

		// Token: 0x040085FA RID: 34298
		public string TagName = string.Empty;

		// Token: 0x040085FB RID: 34299
		public FastTags<TagGroup.Global> Tags = FastTags<TagGroup.Global>.none;

		// Token: 0x040085FC RID: 34300
		public List<BaseChallengeObjective> ObjectiveList = new List<BaseChallengeObjective>();

		// Token: 0x040085FD RID: 34301
		public ChallengeClass NextChallenge;

		// Token: 0x040085FE RID: 34302
		[PublicizedFrom(EAccessModifier.Private)]
		public static int nextIndex = 0;

		// Token: 0x04008600 RID: 34304
		public bool RedeemAlways;

		// Token: 0x04008601 RID: 34305
		public bool NeedsConstantUIUpdate;

		// Token: 0x04008602 RID: 34306
		public MinEffectController Effects;

		// Token: 0x020015DE RID: 5598
		public enum UINavTypes
		{
			// Token: 0x04008604 RID: 34308
			None,
			// Token: 0x04008605 RID: 34309
			Crafting,
			// Token: 0x04008606 RID: 34310
			TwitchActions
		}
	}
}
