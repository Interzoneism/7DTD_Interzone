using System;

// Token: 0x02000F75 RID: 3957
public abstract class GameModeAbstract : GameMode
{
	// Token: 0x06007E29 RID: 32297 RVA: 0x00331E9C File Offset: 0x0033009C
	public override void Init()
	{
		GameStats.Set(EnumGameStats.IsSpawnEnemies, GamePrefs.GetBool(EnumGamePrefs.EnemySpawnMode));
		GameStats.Set(EnumGameStats.PlayerKillingMode, GamePrefs.GetInt(EnumGamePrefs.PlayerKillingMode));
		GameStats.Set(EnumGameStats.ShowAllPlayersOnMap, false);
		GameStats.Set(EnumGameStats.ShowFriendPlayerOnMap, GamePrefs.GetBool(EnumGamePrefs.ShowFriendPlayerOnMap));
		GameStats.Set(EnumGameStats.IsResetMapOnRestart, false);
		GameStats.Set(EnumGameStats.IsFlyingEnabled, GamePrefs.GetBool(EnumGamePrefs.BuildCreate));
		GameStats.Set(EnumGameStats.IsCreativeMenuEnabled, GamePrefs.GetBool(EnumGamePrefs.BuildCreate));
		GameStats.Set(EnumGameStats.IsTeleportEnabled, false);
		GameStats.Set(EnumGameStats.IsPlayerDamageEnabled, true);
		GameStats.Set(EnumGameStats.IsPlayerCollisionEnabled, true);
		GameStats.Set(EnumGameStats.TimeOfDayIncPerSec, 24000 / (GamePrefs.GetInt(EnumGamePrefs.DayNightLength) * 60));
		GameStats.Set(EnumGameStats.DayLimitActive, false);
		GameStats.Set(EnumGameStats.TimeLimitActive, false);
		GameStats.Set(EnumGameStats.FragLimitActive, false);
		GameStats.Set(EnumGameStats.GameDifficulty, GamePrefs.GetInt(EnumGamePrefs.GameDifficulty));
		GameStats.Set(EnumGameStats.GameDifficultyBonus, GameStageDefinition.DifficultyBonus);
		GameStats.Set(EnumGameStats.BlockDamagePlayer, GamePrefs.GetInt(EnumGamePrefs.BlockDamagePlayer));
		GameStats.Set(EnumGameStats.XPMultiplier, GamePrefs.GetInt(EnumGamePrefs.XPMultiplier));
		GameStats.Set(EnumGameStats.BloodMoonWarning, GamePrefs.GetInt(EnumGamePrefs.BloodMoonWarning));
		GameStats.Set(EnumGameStats.DayLightLength, GamePrefs.GetInt(EnumGamePrefs.DayLightLength));
		GameStats.Set(EnumGameStats.OptionsPOICulling, GamePrefs.GetInt(EnumGamePrefs.OptionsPOICulling));
		GameStats.Set(EnumGameStats.AllowedViewDistance, GamePrefs.GetInt(EnumGamePrefs.OptionsGfxViewDistance));
		GameStats.Set(EnumGameStats.QuestProgressionDailyLimit, GamePrefs.GetInt(EnumGamePrefs.QuestProgressionDailyLimit));
		GameStats.Set(EnumGameStats.StormFreq, GamePrefs.GetInt(EnumGamePrefs.StormFreq));
	}

	// Token: 0x06007E2A RID: 32298 RVA: 0x00331FD8 File Offset: 0x003301D8
	public override void ResetGamePrefs()
	{
		GameMode.ModeGamePref[] supportedGamePrefsInfo = this.GetSupportedGamePrefsInfo();
		for (int i = 0; i < supportedGamePrefsInfo.GetLength(0); i++)
		{
			EnumGamePrefs gamePref = supportedGamePrefsInfo[i].GamePref;
			GamePrefs.EnumType valueType = supportedGamePrefsInfo[i].ValueType;
			if (valueType == GamePrefs.EnumType.Int)
			{
				GamePrefs.Set(gamePref, (int)supportedGamePrefsInfo[i].DefaultValue);
			}
			else if (valueType == GamePrefs.EnumType.String)
			{
				GamePrefs.Set(gamePref, (string)supportedGamePrefsInfo[i].DefaultValue);
			}
			else if (valueType == GamePrefs.EnumType.Bool)
			{
				GamePrefs.Set(gamePref, (bool)supportedGamePrefsInfo[i].DefaultValue);
			}
		}
		this.Init();
	}

	// Token: 0x06007E2B RID: 32299 RVA: 0x00332072 File Offset: 0x00330272
	[PublicizedFrom(EAccessModifier.Protected)]
	public GameModeAbstract()
	{
	}
}
