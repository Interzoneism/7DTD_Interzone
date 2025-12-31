using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001589 RID: 5513
	public class TwitchVoteType
	{
		// Token: 0x0600A9AB RID: 43435 RVA: 0x0043063E File Offset: 0x0042E83E
		public bool IsInPreset(string preset)
		{
			return this.PresetNames == null || this.PresetNames.Contains(preset);
		}

		// Token: 0x0600A9AC RID: 43436 RVA: 0x00430656 File Offset: 0x0042E856
		public bool CanUse()
		{
			return this.Enabled && (this.MaxTimesPerDay == -1 || this.MaxTimesPerDay > this.CurrentDayCount);
		}

		// Token: 0x0600A9AD RID: 43437 RVA: 0x0043067C File Offset: 0x0042E87C
		public virtual void ParseProperties(DynamicProperties properties)
		{
			properties.ParseString(TwitchVoteType.PropTitle, ref this.Title);
			if (properties.Values.ContainsKey(TwitchVoteType.PropTitleKey))
			{
				this.Title = Localization.Get(properties.Values[TwitchVoteType.PropTitleKey], false);
			}
			properties.ParseString(TwitchVoteType.PropIcon, ref this.Icon);
			properties.ParseBool(TwitchVoteType.PropSpawnBlocked, ref this.SpawnBlocked);
			properties.ParseInt(TwitchVoteType.PropMaxTimesPerDay, ref this.MaxTimesPerDay);
			properties.ParseInt(TwitchVoteType.PropAllowedStartHour, ref this.AllowedStartHour);
			properties.ParseInt(TwitchVoteType.PropAllowedEndHour, ref this.AllowedEndHour);
			properties.ParseBool(TwitchVoteType.PropBloodMoonDay, ref this.BloodMoonDay);
			properties.ParseBool(TwitchVoteType.PropBloodMoonAllowed, ref this.BloodMoonAllowed);
			properties.ParseString(TwitchVoteType.PropGuaranteedGroups, ref this.GuaranteedGroup);
			properties.ParseBool(TwitchVoteType.PropCooldownOnEnd, ref this.CooldownOnEnd);
			properties.ParseBool(TwitchVoteType.PropUseMystery, ref this.UseMystery);
			properties.ParseBool(TwitchVoteType.PropActionLockout, ref this.ActionLockout);
			properties.ParseString(TwitchVoteType.PropGroup, ref this.Group);
			properties.ParseBool(TwitchVoteType.PropEnabled, ref this.Enabled);
			properties.ParseBool(TwitchVoteType.PropAllowedWithActions, ref this.AllowedWithActions);
			properties.ParseInt(TwitchVoteType.PropVoteChoiceCount, ref this.VoteChoiceCount);
			properties.ParseBool(TwitchVoteType.PropIsBoss, ref this.IsBoss);
			properties.ParseBool(TwitchVoteType.PropManualStart, ref this.ManualStart);
			if (properties.Values.ContainsKey(TwitchVoteType.PropPresets))
			{
				this.PresetNames = new List<string>();
				this.PresetNames.AddRange(properties.Values[TwitchVoteType.PropPresets].Split(',', StringSplitOptions.None));
			}
		}

		// Token: 0x04008453 RID: 33875
		public static string PropTitle = "title";

		// Token: 0x04008454 RID: 33876
		public static string PropTitleKey = "title_key";

		// Token: 0x04008455 RID: 33877
		public static string PropSpawnBlocked = "spawn_blocked";

		// Token: 0x04008456 RID: 33878
		public static string PropExcludeTimeIndex = "exclude_time_index";

		// Token: 0x04008457 RID: 33879
		public static string PropMaxTimesPerDay = "max_times_per_day";

		// Token: 0x04008458 RID: 33880
		public static string PropAllowedStartHour = "allowed_start_hour";

		// Token: 0x04008459 RID: 33881
		public static string PropAllowedEndHour = "allowed_end_hour";

		// Token: 0x0400845A RID: 33882
		public static string PropBloodMoonDay = "blood_moon_day";

		// Token: 0x0400845B RID: 33883
		public static string PropBloodMoonAllowed = "blood_moon_allowed";

		// Token: 0x0400845C RID: 33884
		public static string PropGuaranteedGroups = "guaranteed_group";

		// Token: 0x0400845D RID: 33885
		public static string PropCooldownOnEnd = "cooldown_on_end";

		// Token: 0x0400845E RID: 33886
		public static string PropUseMystery = "use_mystery";

		// Token: 0x0400845F RID: 33887
		public static string PropActionLockout = "action_lockout";

		// Token: 0x04008460 RID: 33888
		public static string PropGroup = "group";

		// Token: 0x04008461 RID: 33889
		public static string PropEnabled = "enabled";

		// Token: 0x04008462 RID: 33890
		public static string PropVoteChoiceCount = "vote_choice_count";

		// Token: 0x04008463 RID: 33891
		public static string PropAllowedWithActions = "allowed_with_actions";

		// Token: 0x04008464 RID: 33892
		public static string PropIsBoss = "is_boss";

		// Token: 0x04008465 RID: 33893
		public static string PropManualStart = "manual_start";

		// Token: 0x04008466 RID: 33894
		public static string PropIcon = "icon";

		// Token: 0x04008467 RID: 33895
		public static string PropPresets = "presets";

		// Token: 0x04008468 RID: 33896
		public string Name;

		// Token: 0x04008469 RID: 33897
		public string Title;

		// Token: 0x0400846A RID: 33898
		public string Icon;

		// Token: 0x0400846B RID: 33899
		public string Group;

		// Token: 0x0400846C RID: 33900
		public bool SpawnBlocked = true;

		// Token: 0x0400846D RID: 33901
		public bool BloodMoonDay = true;

		// Token: 0x0400846E RID: 33902
		public bool BloodMoonAllowed = true;

		// Token: 0x0400846F RID: 33903
		public bool CooldownOnEnd;

		// Token: 0x04008470 RID: 33904
		public bool UseMystery;

		// Token: 0x04008471 RID: 33905
		public bool ActionLockout;

		// Token: 0x04008472 RID: 33906
		public bool AllowedWithActions = true;

		// Token: 0x04008473 RID: 33907
		public int MaxTimesPerDay = -1;

		// Token: 0x04008474 RID: 33908
		public int AllowedStartHour;

		// Token: 0x04008475 RID: 33909
		public int AllowedEndHour = 24;

		// Token: 0x04008476 RID: 33910
		public int VoteChoiceCount = 3;

		// Token: 0x04008477 RID: 33911
		public int CurrentDayCount;

		// Token: 0x04008478 RID: 33912
		public string GuaranteedGroup = "";

		// Token: 0x04008479 RID: 33913
		public bool Enabled = true;

		// Token: 0x0400847A RID: 33914
		public bool ManualStart;

		// Token: 0x0400847B RID: 33915
		public bool IsBoss;

		// Token: 0x0400847C RID: 33916
		public List<string> PresetNames;
	}
}
