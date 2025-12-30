using System;
using System.Collections.Generic;
using UnityEngine;

namespace Twitch
{
	// Token: 0x02001583 RID: 5507
	public class TwitchVote
	{
		// Token: 0x170012D0 RID: 4816
		// (get) Token: 0x0600A994 RID: 43412 RVA: 0x0042FB74 File Offset: 0x0042DD74
		public string VoteDescription
		{
			get
			{
				switch (this.DisplayType)
				{
				case TwitchVote.VoteDisplayTypes.Single:
				case TwitchVote.VoteDisplayTypes.Special:
					return this.Title;
				case TwitchVote.VoteDisplayTypes.GoodBad:
					return this.Title + " / " + this.VoteLine1;
				case TwitchVote.VoteDisplayTypes.HordeBuffed:
					return this.Title + " (" + this.VoteLine1 + ")";
				default:
					return "";
				}
			}
		}

		// Token: 0x170012D1 RID: 4817
		// (get) Token: 0x0600A995 RID: 43413 RVA: 0x0042FBE0 File Offset: 0x0042DDE0
		public string VoteHeight
		{
			get
			{
				TwitchVote.VoteDisplayTypes displayType = this.DisplayType;
				if (displayType == TwitchVote.VoteDisplayTypes.Single || displayType == TwitchVote.VoteDisplayTypes.Special)
				{
					return "-50";
				}
				return "-90";
			}
		}

		// Token: 0x0600A996 RID: 43414 RVA: 0x0042FC06 File Offset: 0x0042DE06
		public bool IsInPreset(TwitchVotePreset preset)
		{
			return (this.PresetNames == null && !preset.IsEmpty) || (this.PresetNames != null && this.PresetNames.Contains(preset.Name));
		}

		// Token: 0x0600A997 RID: 43415 RVA: 0x0042FC38 File Offset: 0x0042DE38
		public bool CanUse(int hour, int gamestage, EntityPlayer player)
		{
			if ((this.StartGameStage != -1 && this.StartGameStage > gamestage) || (this.EndGameStage != -1 && this.EndGameStage < gamestage) || hour < this.AllowedStartHour || hour > this.AllowedEndHour || (this.MaxTimesPerDay != -1 && this.MaxTimesPerDay <= this.CurrentDayCount))
			{
				return false;
			}
			if (this.tempCooldown > 0f && TwitchManager.Current.CurrentUnityTime - this.tempCooldownSet < this.tempCooldown)
			{
				return false;
			}
			this.tempCooldown = 0f;
			this.tempCooldownSet = 0f;
			if (this.VoteRequirements == null)
			{
				return true;
			}
			for (int i = 0; i < this.VoteRequirements.Count; i++)
			{
				if (!this.VoteRequirements[i].CanPerform(player))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600A998 RID: 43416 RVA: 0x0042FD18 File Offset: 0x0042DF18
		public virtual void ParseProperties(DynamicProperties properties)
		{
			this.Properties = properties;
			string text = "";
			string text2 = "";
			properties.ParseLocalizedString(TwitchVote.PropTitleVarKey, ref text);
			properties.ParseLocalizedString(TwitchVote.PropDisplayVarKey, ref text2);
			if (text == "" && text2 != "")
			{
				text = text2;
			}
			else if (text != "" && text2 == "")
			{
				text2 = text;
			}
			properties.ParseLocalizedString(TwitchVote.PropTitleKey, ref this.Title);
			if (this.Title == "")
			{
				properties.ParseString(TwitchVote.PropTitle, ref this.Title);
			}
			string text3 = "";
			properties.ParseLocalizedString(TwitchVote.PropTitleFormatKey, ref text3);
			if (text3 != "")
			{
				this.Title = string.Format(text3, text);
			}
			properties.ParseLocalizedString(TwitchVote.PropDescriptionKey, ref this.Description);
			if (this.Description == "")
			{
				properties.ParseString(TwitchVote.PropDescription, ref this.Description);
			}
			text3 = "";
			properties.ParseLocalizedString(TwitchVote.PropDescriptionFormatKey, ref text3);
			if (text3 != "")
			{
				this.Description = string.Format(text3, text);
			}
			if (properties.Values.ContainsKey(TwitchVote.PropDisplayKey))
			{
				this.Display = Localization.Get(properties.Values[TwitchVote.PropDisplayKey], false);
			}
			else
			{
				properties.ParseString(TwitchVote.PropDisplay, ref this.Display);
			}
			text3 = "";
			properties.ParseLocalizedString(TwitchVote.PropDisplayFormatKey, ref text3);
			if (text3 != "")
			{
				this.Display = string.Format(text3, text2);
			}
			properties.ParseString(TwitchVote.PropEventName, ref this.GameEvent);
			properties.ParseString(TwitchVote.PropGroup, ref this.Group);
			properties.ParseInt(TwitchVote.PropStartGameStage, ref this.StartGameStage);
			properties.ParseInt(TwitchVote.PropEndGameStage, ref this.EndGameStage);
			properties.ParseInt(TwitchVote.PropAllowedStartHour, ref this.AllowedStartHour);
			properties.ParseInt(TwitchVote.PropAllowedEndHour, ref this.AllowedEndHour);
			properties.ParseString(TwitchVote.PropVoteLine1, ref this.VoteLine1);
			properties.ParseString(TwitchVote.PropVoteLine2, ref this.VoteLine2);
			properties.ParseEnum<TwitchVote.VoteDisplayTypes>(TwitchVote.PropDisplayType, ref this.DisplayType);
			properties.ParseInt(TwitchVote.PropMaxTimesPerDay, ref this.MaxTimesPerDay);
			string text4 = "";
			properties.ParseString(TwitchVote.PropVoteType, ref text4);
			if (text4 != "")
			{
				this.VoteTypes = text4.Split(',', StringSplitOptions.None);
				this.MainVoteType = TwitchManager.Current.VotingManager.GetVoteType(this.VoteTypes[0]);
			}
			properties.ParseBool(TwitchVote.PropEnabled, ref this.Enabled);
			this.OriginalEnabled = this.Enabled;
			properties.ParseString(TwitchVote.PropTitleColor, ref this.TitleColor);
			if (this.Display == "")
			{
				this.Display = this.Title;
			}
			this.Properties.ParseLocalizedString(TwitchVote.PropVoteLine1Key, ref this.VoteLine1);
			this.Properties.ParseLocalizedString(TwitchVote.PropVoteLine2Key, ref this.VoteLine2);
			this.VoteTip = this.Description;
			this.Properties.ParseString(TwitchVote.PropVoteTip, ref this.VoteTip);
			this.Properties.ParseLocalizedString(TwitchVote.PropVoteTipKey, ref this.VoteTip);
			if (properties.Values.ContainsKey(TwitchVote.PropPresets))
			{
				this.PresetNames = new List<string>();
				this.PresetNames.AddRange(properties.Values[TwitchVote.PropPresets].Split(',', StringSplitOptions.None));
			}
			if (!GameEventManager.GameEventSequences.ContainsKey(this.GameEvent))
			{
				Debug.LogError(string.Format("TwitchVote: Game Event Sequence '{0}' does not exist!", this.GameEvent));
			}
		}

		// Token: 0x0600A999 RID: 43417 RVA: 0x004300C9 File Offset: 0x0042E2C9
		public void AddCooldownAddition(TwitchActionCooldownAddition newCooldown)
		{
			if (this.CooldownAdditions == null)
			{
				this.CooldownAdditions = new List<TwitchActionCooldownAddition>();
			}
			this.CooldownAdditions.Add(newCooldown);
		}

		// Token: 0x0600A99A RID: 43418 RVA: 0x004300EA File Offset: 0x0042E2EA
		public void AddVoteRequirement(BaseTwitchVoteRequirement voteRequirement)
		{
			if (this.VoteRequirements == null)
			{
				this.VoteRequirements = new List<BaseTwitchVoteRequirement>();
			}
			this.VoteRequirements.Add(voteRequirement);
		}

		// Token: 0x0600A99B RID: 43419 RVA: 0x0043010C File Offset: 0x0042E30C
		public void HandleVoteComplete()
		{
			if (this.CooldownAdditions != null)
			{
				float actionCooldownModifier = TwitchManager.Current.ActionCooldownModifier;
				for (int i = 0; i < this.CooldownAdditions.Count; i++)
				{
					TwitchActionCooldownAddition twitchActionCooldownAddition = this.CooldownAdditions[i];
					if (twitchActionCooldownAddition.IsAction && TwitchActionManager.TwitchActions.ContainsKey(twitchActionCooldownAddition.ActionName))
					{
						TwitchAction twitchAction = TwitchActionManager.TwitchActions[twitchActionCooldownAddition.ActionName];
						twitchAction.tempCooldown = twitchActionCooldownAddition.CooldownTime * actionCooldownModifier;
						twitchAction.tempCooldownSet = Time.time;
					}
					else if (!twitchActionCooldownAddition.IsAction && TwitchActionManager.TwitchVotes.ContainsKey(twitchActionCooldownAddition.ActionName))
					{
						TwitchVote twitchVote = TwitchActionManager.TwitchVotes[twitchActionCooldownAddition.ActionName];
						twitchVote.tempCooldown = twitchActionCooldownAddition.CooldownTime * actionCooldownModifier;
						twitchVote.tempCooldownSet = Time.time;
					}
				}
			}
		}

		// Token: 0x040083FF RID: 33791
		public static string PropTitleVarKey = "title_var_key";

		// Token: 0x04008400 RID: 33792
		public static string PropDisplayVarKey = "display_var_key";

		// Token: 0x04008401 RID: 33793
		public static string PropTitle = "title";

		// Token: 0x04008402 RID: 33794
		public static string PropTitleKey = "title_key";

		// Token: 0x04008403 RID: 33795
		public static string PropDescription = "description";

		// Token: 0x04008404 RID: 33796
		public static string PropDescriptionKey = "description_key";

		// Token: 0x04008405 RID: 33797
		public static string PropDisplay = "display";

		// Token: 0x04008406 RID: 33798
		public static string PropDisplayKey = "display_key";

		// Token: 0x04008407 RID: 33799
		public static string PropEventName = "event_name";

		// Token: 0x04008408 RID: 33800
		public static string PropVoteType = "vote_type";

		// Token: 0x04008409 RID: 33801
		public static string PropGroup = "group";

		// Token: 0x0400840A RID: 33802
		public static string PropTitleFormatKey = "title_format_key";

		// Token: 0x0400840B RID: 33803
		public static string PropDescriptionFormatKey = "description_format_key";

		// Token: 0x0400840C RID: 33804
		public static string PropDisplayFormatKey = "display_format_key";

		// Token: 0x0400840D RID: 33805
		public static string PropStartGameStage = "start_gamestage";

		// Token: 0x0400840E RID: 33806
		public static string PropEndGameStage = "end_gamestage";

		// Token: 0x0400840F RID: 33807
		public static string PropAllowedStartHour = "allowed_start_hour";

		// Token: 0x04008410 RID: 33808
		public static string PropAllowedEndHour = "allowed_end_hour";

		// Token: 0x04008411 RID: 33809
		public static string PropDisplayType = "display_type";

		// Token: 0x04008412 RID: 33810
		public static string PropTitleColor = "title_color";

		// Token: 0x04008413 RID: 33811
		public static string PropVoteLine1 = "line1_desc";

		// Token: 0x04008414 RID: 33812
		public static string PropVoteLine1Key = "line1_desc_key";

		// Token: 0x04008415 RID: 33813
		public static string PropVoteLine2 = "line2_desc";

		// Token: 0x04008416 RID: 33814
		public static string PropVoteLine2Key = "line2_desc_key";

		// Token: 0x04008417 RID: 33815
		public static string PropEnabled = "enabled";

		// Token: 0x04008418 RID: 33816
		public static string PropMaxTimesPerDay = "max_times_per_day";

		// Token: 0x04008419 RID: 33817
		public static string PropVoteTip = "vote_tip";

		// Token: 0x0400841A RID: 33818
		public static string PropVoteTipKey = "vote_tip_key";

		// Token: 0x0400841B RID: 33819
		public static string PropPresets = "presets";

		// Token: 0x0400841C RID: 33820
		public static HashSet<string> ExtendsExcludes = new HashSet<string>
		{
			TwitchVote.PropStartGameStage,
			TwitchVote.PropEndGameStage
		};

		// Token: 0x0400841D RID: 33821
		public string VoteName;

		// Token: 0x0400841E RID: 33822
		public string Title;

		// Token: 0x0400841F RID: 33823
		public string Description;

		// Token: 0x04008420 RID: 33824
		public string Display = "";

		// Token: 0x04008421 RID: 33825
		public string GameEvent;

		// Token: 0x04008422 RID: 33826
		public string[] VoteTypes;

		// Token: 0x04008423 RID: 33827
		public TwitchVoteType MainVoteType;

		// Token: 0x04008424 RID: 33828
		public string Group = "";

		// Token: 0x04008425 RID: 33829
		public bool Enabled = true;

		// Token: 0x04008426 RID: 33830
		public bool OriginalEnabled;

		// Token: 0x04008427 RID: 33831
		public string TitleColor = "";

		// Token: 0x04008428 RID: 33832
		public int StartGameStage = -1;

		// Token: 0x04008429 RID: 33833
		public int EndGameStage = -1;

		// Token: 0x0400842A RID: 33834
		public int AllowedStartHour;

		// Token: 0x0400842B RID: 33835
		public int AllowedEndHour = 24;

		// Token: 0x0400842C RID: 33836
		public string VoteLine1 = "";

		// Token: 0x0400842D RID: 33837
		public string VoteLine2 = "";

		// Token: 0x0400842E RID: 33838
		public int MaxTimesPerDay = -1;

		// Token: 0x0400842F RID: 33839
		public int CurrentDayCount;

		// Token: 0x04008430 RID: 33840
		public float tempCooldownSet;

		// Token: 0x04008431 RID: 33841
		public float tempCooldown;

		// Token: 0x04008432 RID: 33842
		public string VoteTip = "";

		// Token: 0x04008433 RID: 33843
		public DynamicProperties Properties;

		// Token: 0x04008434 RID: 33844
		public List<TwitchActionCooldownAddition> CooldownAdditions;

		// Token: 0x04008435 RID: 33845
		public List<BaseTwitchVoteRequirement> VoteRequirements;

		// Token: 0x04008436 RID: 33846
		public TwitchVote.VoteDisplayTypes DisplayType = TwitchVote.VoteDisplayTypes.GoodBad;

		// Token: 0x04008437 RID: 33847
		public List<string> PresetNames;

		// Token: 0x02001584 RID: 5508
		public enum VoteDisplayTypes
		{
			// Token: 0x04008439 RID: 33849
			Single,
			// Token: 0x0400843A RID: 33850
			GoodBad,
			// Token: 0x0400843B RID: 33851
			Special,
			// Token: 0x0400843C RID: 33852
			HordeBuffed
		}
	}
}
