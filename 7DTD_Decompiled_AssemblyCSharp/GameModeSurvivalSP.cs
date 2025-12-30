using System;
using UnityEngine.Scripting;

// Token: 0x02000F7C RID: 3964
[Preserve]
public class GameModeSurvivalSP : GameModeAbstract
{
	// Token: 0x06007E6A RID: 32362 RVA: 0x003336A2 File Offset: 0x003318A2
	public override string GetName()
	{
		return "gmSurvivalSP";
	}

	// Token: 0x06007E6B RID: 32363 RVA: 0x003336A9 File Offset: 0x003318A9
	public override string GetDescription()
	{
		return "gmSurvivalSPDesc";
	}

	// Token: 0x06007E6C RID: 32364 RVA: 0x00076E19 File Offset: 0x00075019
	public override int GetID()
	{
		return 6;
	}

	// Token: 0x06007E6D RID: 32365 RVA: 0x003336B0 File Offset: 0x003318B0
	public override GameMode.ModeGamePref[] GetSupportedGamePrefsInfo()
	{
		return new GameMode.ModeGamePref[]
		{
			new GameMode.ModeGamePref(EnumGamePrefs.GameDifficulty, GamePrefs.EnumType.Int, 1, null),
			new GameMode.ModeGamePref(EnumGamePrefs.DayNightLength, GamePrefs.EnumType.Int, 60, null),
			new GameMode.ModeGamePref(EnumGamePrefs.DayLightLength, GamePrefs.EnumType.Int, 18, null),
			new GameMode.ModeGamePref(EnumGamePrefs.LootAbundance, GamePrefs.EnumType.Int, 100, null),
			new GameMode.ModeGamePref(EnumGamePrefs.LootRespawnDays, GamePrefs.EnumType.Int, 7, null),
			new GameMode.ModeGamePref(EnumGamePrefs.DropOnDeath, GamePrefs.EnumType.Int, 0, null),
			new GameMode.ModeGamePref(EnumGamePrefs.BloodMoonEnemyCount, GamePrefs.EnumType.Int, 8, null),
			new GameMode.ModeGamePref(EnumGamePrefs.EnemySpawnMode, GamePrefs.EnumType.Bool, true, null),
			new GameMode.ModeGamePref(EnumGamePrefs.EnemyDifficulty, GamePrefs.EnumType.Int, 0, null),
			new GameMode.ModeGamePref(EnumGamePrefs.AirDropFrequency, GamePrefs.EnumType.Int, 72, null),
			new GameMode.ModeGamePref(EnumGamePrefs.BuildCreate, GamePrefs.EnumType.Bool, false, null),
			new GameMode.ModeGamePref(EnumGamePrefs.PersistentPlayerProfiles, GamePrefs.EnumType.Bool, false, null),
			new GameMode.ModeGamePref(EnumGamePrefs.AirDropMarker, GamePrefs.EnumType.Bool, true, null),
			new GameMode.ModeGamePref(EnumGamePrefs.BedrollDeadZoneSize, GamePrefs.EnumType.Int, 15, null),
			new GameMode.ModeGamePref(EnumGamePrefs.MaxChunkAge, GamePrefs.EnumType.Int, -1, null)
		};
	}

	// Token: 0x06007E6E RID: 32366 RVA: 0x00333824 File Offset: 0x00331A24
	public override void Init()
	{
		base.Init();
		GameStats.Set(EnumGameStats.ShowSpawnWindow, false);
		GameStats.Set(EnumGameStats.TimeLimitActive, false);
		GameStats.Set(EnumGameStats.DayLimitActive, false);
		GameStats.Set(EnumGameStats.ShowWindow, "");
		GameStats.Set(EnumGameStats.IsSpawnEnemies, GamePrefs.GetBool(EnumGamePrefs.EnemySpawnMode));
		GameStats.Set(EnumGameStats.ScorePlayerKillMultiplier, 0);
		GameStats.Set(EnumGameStats.ScoreZombieKillMultiplier, 1);
		GameStats.Set(EnumGameStats.ScoreDiedMultiplier, -5);
		GameStats.Set(EnumGameStats.IsSpawnNearOtherPlayer, false);
		GameStats.Set(EnumGameStats.ZombieHordeMeter, true);
		GameStats.Set(EnumGameStats.DropOnDeath, GamePrefs.GetInt(EnumGamePrefs.DropOnDeath));
		GameStats.Set(EnumGameStats.DropOnQuit, 0);
		GameStats.Set(EnumGameStats.BloodMoonEnemyCount, GamePrefs.GetInt(EnumGamePrefs.BloodMoonEnemyCount));
		GameStats.Set(EnumGameStats.EnemySpawnMode, GamePrefs.GetBool(EnumGamePrefs.EnemySpawnMode));
		GameStats.Set(EnumGameStats.EnemyDifficulty, GamePrefs.GetInt(EnumGamePrefs.EnemyDifficulty));
		GameStats.Set(EnumGameStats.AirDropFrequency, GamePrefs.GetInt(EnumGamePrefs.AirDropFrequency));
		GameStats.Set(EnumGameStats.AirDropMarker, GamePrefs.GetBool(EnumGamePrefs.AirDropMarker));
		GameStats.Set(EnumGameStats.PartySharedKillRange, GamePrefs.GetInt(EnumGamePrefs.PartySharedKillRange));
		GamePrefs.Set(EnumGamePrefs.ServerMaxPlayerCount, 1);
		GamePrefs.Set(EnumGamePrefs.ServerIsPublic, false);
		GamePrefs.Set(EnumGamePrefs.ServerPort, Constants.cDefaultPort);
		GameStats.Set(EnumGameStats.IsFlyingEnabled, GamePrefs.GetBool(EnumGamePrefs.BuildCreate));
		GameStats.Set(EnumGameStats.BiomeProgression, GamePrefs.GetBool(EnumGamePrefs.BiomeProgression));
	}

	// Token: 0x06007E6F RID: 32367 RVA: 0x000197A5 File Offset: 0x000179A5
	public override int GetRoundCount()
	{
		return 1;
	}

	// Token: 0x06007E70 RID: 32368 RVA: 0x00332188 File Offset: 0x00330388
	public override void StartRound(int _idx)
	{
		GameStats.Set(EnumGameStats.GameState, 1);
	}

	// Token: 0x06007E71 RID: 32369 RVA: 0x00002914 File Offset: 0x00000B14
	public override void EndRound(int _idx)
	{
	}

	// Token: 0x04006070 RID: 24688
	public static readonly string TypeName = typeof(GameModeSurvivalSP).Name;
}
