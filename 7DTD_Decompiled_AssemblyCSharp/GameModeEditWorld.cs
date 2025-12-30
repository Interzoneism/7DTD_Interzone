using System;
using UnityEngine.Scripting;

// Token: 0x02000F78 RID: 3960
[Preserve]
public class GameModeEditWorld : GameModeAbstract
{
	// Token: 0x06007E41 RID: 32321 RVA: 0x003324F9 File Offset: 0x003306F9
	public override string GetName()
	{
		return "gmEditWorld";
	}

	// Token: 0x06007E42 RID: 32322 RVA: 0x00332500 File Offset: 0x00330700
	public override string GetDescription()
	{
		return "gmEditWorldDesc";
	}

	// Token: 0x06007E43 RID: 32323 RVA: 0x000768E0 File Offset: 0x00074AE0
	public override int GetID()
	{
		return 8;
	}

	// Token: 0x06007E44 RID: 32324 RVA: 0x00332508 File Offset: 0x00330708
	public override GameMode.ModeGamePref[] GetSupportedGamePrefsInfo()
	{
		return new GameMode.ModeGamePref[]
		{
			new GameMode.ModeGamePref(EnumGamePrefs.GameDifficulty, GamePrefs.EnumType.Int, 1, null),
			new GameMode.ModeGamePref(EnumGamePrefs.ServerIsPublic, GamePrefs.EnumType.Bool, false, null),
			new GameMode.ModeGamePref(EnumGamePrefs.ServerVisibility, GamePrefs.EnumType.Int, 1, null),
			new GameMode.ModeGamePref(EnumGamePrefs.ServerMaxPlayerCount, GamePrefs.EnumType.Int, 8, null)
		};
	}

	// Token: 0x06007E45 RID: 32325 RVA: 0x00332578 File Offset: 0x00330778
	public override void Init()
	{
		base.Init();
		GameStats.Set(EnumGameStats.ShowSpawnWindow, false);
		GameStats.Set(EnumGameStats.TimeLimitActive, false);
		GameStats.Set(EnumGameStats.DayLimitActive, false);
		GameStats.Set(EnumGameStats.IsSpawnNearOtherPlayer, true);
		GameStats.Set(EnumGameStats.ShowWindow, "");
		GameStats.Set(EnumGameStats.TimeOfDayIncPerSec, 0);
		GameStats.Set(EnumGameStats.IsSpawnEnemies, false);
		GameStats.Set(EnumGameStats.IsTeleportEnabled, true);
		GameStats.Set(EnumGameStats.IsFlyingEnabled, true);
		GameStats.Set(EnumGameStats.IsCreativeMenuEnabled, true);
		GameStats.Set(EnumGameStats.IsPlayerDamageEnabled, false);
		GameStats.Set(EnumGameStats.AirDropFrequency, 0);
		GameStats.Set(EnumGameStats.AutoParty, true);
		GamePrefs.Set(EnumGamePrefs.DynamicSpawner, "");
	}

	// Token: 0x06007E46 RID: 32326 RVA: 0x000197A5 File Offset: 0x000179A5
	public override int GetRoundCount()
	{
		return 1;
	}

	// Token: 0x06007E47 RID: 32327 RVA: 0x00332188 File Offset: 0x00330388
	public override void StartRound(int _idx)
	{
		GameStats.Set(EnumGameStats.GameState, 1);
	}

	// Token: 0x06007E48 RID: 32328 RVA: 0x00002914 File Offset: 0x00000B14
	public override void EndRound(int _idx)
	{
	}

	// Token: 0x06007E49 RID: 32329 RVA: 0x00332600 File Offset: 0x00330800
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

	// Token: 0x0400606B RID: 24683
	public static readonly string TypeName = typeof(GameModeEditWorld).Name;
}
