using System;
using UnityEngine.Scripting;

// Token: 0x02000F76 RID: 3958
[Preserve]
public class GameModeCreative : GameModeAbstract
{
	// Token: 0x06007E2C RID: 32300 RVA: 0x0033207A File Offset: 0x0033027A
	public override string GetName()
	{
		return "gmCreative";
	}

	// Token: 0x06007E2D RID: 32301 RVA: 0x00332081 File Offset: 0x00330281
	public override string GetDescription()
	{
		return "gmCreativeDesc";
	}

	// Token: 0x06007E2E RID: 32302 RVA: 0x000282C0 File Offset: 0x000264C0
	public override int GetID()
	{
		return 2;
	}

	// Token: 0x06007E2F RID: 32303 RVA: 0x00332088 File Offset: 0x00330288
	public override GameMode.ModeGamePref[] GetSupportedGamePrefsInfo()
	{
		return new GameMode.ModeGamePref[]
		{
			new GameMode.ModeGamePref(EnumGamePrefs.ServerIsPublic, GamePrefs.EnumType.Bool, false, null),
			new GameMode.ModeGamePref(EnumGamePrefs.DayNightLength, GamePrefs.EnumType.Int, 60, null),
			new GameMode.ModeGamePref(EnumGamePrefs.DayLightLength, GamePrefs.EnumType.Int, 18, null)
		};
	}

	// Token: 0x06007E30 RID: 32304 RVA: 0x003320E0 File Offset: 0x003302E0
	public override void Init()
	{
		base.Init();
		GameStats.Set(EnumGameStats.ShowSpawnWindow, false);
		GameStats.Set(EnumGameStats.TimeLimitActive, false);
		GameStats.Set(EnumGameStats.DayLimitActive, false);
		GameStats.Set(EnumGameStats.IsSpawnNearOtherPlayer, true);
		GameStats.Set(EnumGameStats.ShowWindow, "");
		GameStats.Set(EnumGameStats.TimeOfDayIncPerSec, 4);
		GameStats.Set(EnumGameStats.IsSpawnEnemies, false);
		GameStats.Set(EnumGameStats.ShowAllPlayersOnMap, true);
		GameStats.Set(EnumGameStats.ZombieHordeMeter, false);
		GameStats.Set(EnumGameStats.DeathPenalty, 0);
		GameStats.Set(EnumGameStats.DropOnDeath, 0);
		GameStats.Set(EnumGameStats.DropOnQuit, 0);
		GameStats.Set(EnumGameStats.IsTeleportEnabled, true);
		GameStats.Set(EnumGameStats.IsFlyingEnabled, true);
		GameStats.Set(EnumGameStats.IsCreativeMenuEnabled, true);
		GameStats.Set(EnumGameStats.IsPlayerDamageEnabled, false);
		GameStats.Set(EnumGameStats.AutoParty, false);
		GamePrefs.Set(EnumGamePrefs.DynamicSpawner, "");
	}

	// Token: 0x06007E31 RID: 32305 RVA: 0x000197A5 File Offset: 0x000179A5
	public override int GetRoundCount()
	{
		return 1;
	}

	// Token: 0x06007E32 RID: 32306 RVA: 0x00332188 File Offset: 0x00330388
	public override void StartRound(int _idx)
	{
		GameStats.Set(EnumGameStats.GameState, 1);
	}

	// Token: 0x06007E33 RID: 32307 RVA: 0x00002914 File Offset: 0x00000B14
	public override void EndRound(int _idx)
	{
	}

	// Token: 0x06007E34 RID: 32308 RVA: 0x00332194 File Offset: 0x00330394
	public override string GetAdditionalGameInfo(World _world)
	{
		switch (GamePrefs.GetInt(EnumGamePrefs.SelectionOperationMode))
		{
		case 0:
			return "Player move";
		case 1:
			return "Selection move " + ((GamePrefs.GetInt(EnumGamePrefs.SelectionContextMode) == 0) ? "absolute" : "relative");
		case 2:
			return "Selection size";
		default:
			return "";
		}
	}

	// Token: 0x04006069 RID: 24681
	public static readonly string TypeName = typeof(GameModeCreative).Name;
}
