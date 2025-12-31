using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001516 RID: 5398
	public class UpdateMessage
	{
		// Token: 0x0600A6AD RID: 42669 RVA: 0x0041CF7C File Offset: 0x0041B17C
		[PublicizedFrom(EAccessModifier.Private)]
		public static string DifficultyValueLocalized()
		{
			int num = GameStats.GetInt(EnumGameStats.GameDifficulty) + 1;
			return Localization.Get(string.Format("goDifficulty{0}", num) + ((num == 2) ? "_nodefault" : ""), false);
		}

		// Token: 0x0600A6AE RID: 42670 RVA: 0x0041CFC0 File Offset: 0x0041B1C0
		[PublicizedFrom(EAccessModifier.Private)]
		public static bool IsGameModded()
		{
			GameServerInfo gameServerInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer ? SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo : SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo;
			return gameServerInfo != null && gameServerInfo.GetValue(GameInfoBool.ModdedConfig);
		}

		// Token: 0x0600A6AF RID: 42671 RVA: 0x0041D000 File Offset: 0x0041B200
		[PublicizedFrom(EAccessModifier.Private)]
		public static string ModdedValueLocalized()
		{
			GameServerInfo gameServerInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer ? SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo : SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo;
			if (gameServerInfo == null)
			{
				return "--";
			}
			if (!gameServerInfo.GetValue(GameInfoBool.ModdedConfig))
			{
				return Localization.Get("xuiComboYesNoOff", false);
			}
			return Localization.Get("xuiComboYesNoOn", false);
		}

		// Token: 0x0600A6B0 RID: 42672 RVA: 0x0041D05C File Offset: 0x0041B25C
		[PublicizedFrom(EAccessModifier.Private)]
		public static string GetLocalizedPPRateValue()
		{
			if (TwitchManager.Current.ViewerData.PointRate == 1f)
			{
				return Localization.Get("xuiTwitchPointGenerationStandard", false);
			}
			if (TwitchManager.Current.ViewerData.PointRate == 2f)
			{
				return Localization.Get("xuiTwitchPointGenerationDouble", false);
			}
			if (TwitchManager.Current.ViewerData.PointRate == 3f)
			{
				return Localization.Get("xuiTwitchPointGenerationTriple", false);
			}
			if (TwitchManager.Current.ViewerData.PointRate == 0f)
			{
				return Localization.Get("goDisabled", false);
			}
			return Localization.Get("xuiTwitchPointGenerationStandard", false);
		}

		// Token: 0x0600A6B1 RID: 42673 RVA: 0x0041D0FC File Offset: 0x0041B2FC
		public UpdateMessage()
		{
			TwitchLeaderboardStats.StatEntry topKillerViewer = TwitchManager.LeaderboardStats.TopKillerViewer;
			this.TopKillerValue = (((topKillerViewer != null) ? topKillerViewer.Name : null) ?? "--");
			TwitchLeaderboardStats.StatEntry topGoodViewer = TwitchManager.LeaderboardStats.TopGoodViewer;
			this.TopGoodValue = (((topGoodViewer != null) ? topGoodViewer.Name : null) ?? "--");
			TwitchLeaderboardStats.StatEntry topBadViewer = TwitchManager.LeaderboardStats.TopBadViewer;
			this.TopBadValue = (((topBadViewer != null) ? topBadViewer.Name : null) ?? "--");
			TwitchLeaderboardStats.StatEntry currentGoodViewer = TwitchManager.LeaderboardStats.CurrentGoodViewer;
			this.BestHelperValue = (((currentGoodViewer != null) ? currentGoodViewer.Name : null) ?? "--");
			this.TotalGoodActionsValue = TwitchManager.LeaderboardStats.TotalGood.ToString();
			this.TotalBadActionsValue = TwitchManager.LeaderboardStats.TotalBad.ToString();
			this.LargestPimpPotValue = TwitchManager.LeaderboardStats.LargestPimpPot.ToString();
			this.DifficultyValue = UpdateMessage.DifficultyValueLocalized();
			this.DayCycleValue = string.Format(Localization.Get("goMinutes", false), GamePrefs.GetInt(EnumGamePrefs.DayNightLength));
			this.PPRateValue = UpdateMessage.GetLocalizedPPRateValue();
			this.ModdedValue = UpdateMessage.ModdedValueLocalized();
			base..ctor();
		}

		// Token: 0x040080E5 RID: 32997
		public string updateSignature;

		// Token: 0x040080E6 RID: 32998
		public string status;

		// Token: 0x040080E7 RID: 32999
		public int[] actionCooldowns;

		// Token: 0x040080E8 RID: 33000
		public Dictionary<string, int> bitBalances;

		// Token: 0x040080E9 RID: 33001
		public Dictionary<string, bool> hasChatted;

		// Token: 0x040080EA RID: 33002
		public string ActionPresetKey = TwitchManager.Current.CurrentActionPreset.Name;

		// Token: 0x040080EB RID: 33003
		public string VotePresetKey = TwitchManager.Current.CurrentVotePreset.Name;

		// Token: 0x040080EC RID: 33004
		public string EventPresetKey = TwitchManager.Current.CurrentEventPreset.Name;

		// Token: 0x040080ED RID: 33005
		public int Difficulty = GameStats.GetInt(EnumGameStats.GameDifficulty) + 1;

		// Token: 0x040080EE RID: 33006
		public int DayMinutes = GamePrefs.GetInt(EnumGamePrefs.DayNightLength);

		// Token: 0x040080EF RID: 33007
		public int PPRate = (int)TwitchManager.Current.ViewerData.PointRate;

		// Token: 0x040080F0 RID: 33008
		public bool IsModded = UpdateMessage.IsGameModded();

		// Token: 0x040080F1 RID: 33009
		public int GoodRewardTime = TwitchManager.LeaderboardStats.GoodRewardTime;

		// Token: 0x040080F2 RID: 33010
		public string ActionPresetValue = TwitchManager.Current.CurrentActionPreset.Title;

		// Token: 0x040080F3 RID: 33011
		public string VotePresetValue = TwitchManager.Current.CurrentVotePreset.Title;

		// Token: 0x040080F4 RID: 33012
		public string EventPresetValue = TwitchManager.Current.CurrentEventPreset.Title;

		// Token: 0x040080F5 RID: 33013
		public string TopKillerValue;

		// Token: 0x040080F6 RID: 33014
		public string TopGoodValue;

		// Token: 0x040080F7 RID: 33015
		public string TopBadValue;

		// Token: 0x040080F8 RID: 33016
		public string BestHelperValue;

		// Token: 0x040080F9 RID: 33017
		public string TotalGoodActionsValue;

		// Token: 0x040080FA RID: 33018
		public string TotalBadActionsValue;

		// Token: 0x040080FB RID: 33019
		public string LargestPimpPotValue;

		// Token: 0x040080FC RID: 33020
		public string DifficultyValue;

		// Token: 0x040080FD RID: 33021
		public string DayCycleValue;

		// Token: 0x040080FE RID: 33022
		public string PPRateValue;

		// Token: 0x040080FF RID: 33023
		public string ModdedValue;
	}
}
