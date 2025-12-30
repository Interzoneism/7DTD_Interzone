using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000984 RID: 2436
public sealed class GameStageDefinition
{
	// Token: 0x0600496C RID: 18796 RVA: 0x001D0B67 File Offset: 0x001CED67
	public GameStageDefinition(string _name)
	{
		this.name = _name;
	}

	// Token: 0x0600496D RID: 18797 RVA: 0x001D0B81 File Offset: 0x001CED81
	public static void AddGameStage(GameStageDefinition gameStage)
	{
		gameStage.SortStages();
		GameStageDefinition.gameStages.Add(gameStage.name, gameStage);
	}

	// Token: 0x0600496E RID: 18798 RVA: 0x001D0B9A File Offset: 0x001CED9A
	public static GameStageDefinition GetGameStage(string name)
	{
		return GameStageDefinition.gameStages[name];
	}

	// Token: 0x0600496F RID: 18799 RVA: 0x001D0BA7 File Offset: 0x001CEDA7
	public static bool TryGetGameStage(string name, out GameStageDefinition definition)
	{
		return GameStageDefinition.gameStages.TryGetValue(name, out definition);
	}

	// Token: 0x06004970 RID: 18800 RVA: 0x001D0BB5 File Offset: 0x001CEDB5
	public static void Clear()
	{
		GameStageDefinition.gameStages.Clear();
	}

	// Token: 0x06004971 RID: 18801 RVA: 0x001D0BC1 File Offset: 0x001CEDC1
	public void AddStage(GameStageDefinition.Stage stage)
	{
		this.stages.Add(stage);
	}

	// Token: 0x06004972 RID: 18802 RVA: 0x001D0BCF File Offset: 0x001CEDCF
	public void SortStages()
	{
		this.stages.Sort((GameStageDefinition.Stage x, GameStageDefinition.Stage y) => x.stageNum.CompareTo(y.stageNum));
	}

	// Token: 0x06004973 RID: 18803 RVA: 0x001D0BFC File Offset: 0x001CEDFC
	public GameStageDefinition.Stage GetStage(int stage)
	{
		if (this.stages.Count < 1)
		{
			return null;
		}
		if (stage < this.stages[0].stageNum)
		{
			return null;
		}
		int num = GameStageDefinition.GetBoundIndex<GameStageDefinition.Stage>(this.stages, (GameStageDefinition.Stage s) => s.stageNum <= stage);
		num = Mathf.Clamp(num, 0, this.stages.Count - 1);
		return this.stages[num];
	}

	// Token: 0x06004974 RID: 18804 RVA: 0x001D0C7C File Offset: 0x001CEE7C
	public static int GetBoundIndex<T>(IList<T> sortedList, Func<T, bool> f)
	{
		int num = 0;
		int num2 = sortedList.Count;
		while (num + 1 < num2)
		{
			int num3 = (num + num2) / 2;
			if (f(sortedList[num3]))
			{
				num = num3;
			}
			else
			{
				num2 = num3;
			}
		}
		if (num2 < sortedList.Count && f(sortedList[num2]))
		{
			num = num2;
		}
		return num;
	}

	// Token: 0x06004975 RID: 18805 RVA: 0x001D0CD0 File Offset: 0x001CEED0
	public static int CalcPartyLevel(List<int> playerGameStages)
	{
		float num = 0f;
		playerGameStages.Sort();
		float num2 = GameStageDefinition.StartingWeight;
		for (int i = playerGameStages.Count - 1; i >= 0; i--)
		{
			num += (float)playerGameStages[i] * num2;
			num2 *= GameStageDefinition.DiminishingReturns;
		}
		return Mathf.FloorToInt(num);
	}

	// Token: 0x06004976 RID: 18806 RVA: 0x001D0D20 File Offset: 0x001CEF20
	public static int CalcGameStageAround(EntityPlayer player)
	{
		List<EntityPlayer> list = new List<EntityPlayer>();
		player.world.GetPlayersAround(player.position, 100f, list);
		List<int> list2 = new List<int>();
		for (int i = 0; i < list.Count; i++)
		{
			EntityPlayer entityPlayer = list[i];
			if (entityPlayer.prefab == player.prefab)
			{
				list2.Add(entityPlayer.gameStage);
			}
		}
		return GameStageDefinition.CalcPartyLevel(list2);
	}

	// Token: 0x040038B1 RID: 14513
	public static float DifficultyBonus = 1f;

	// Token: 0x040038B2 RID: 14514
	public static float StartingWeight = 1f;

	// Token: 0x040038B3 RID: 14515
	public static float DiminishingReturns = 0.5f;

	// Token: 0x040038B4 RID: 14516
	public static long DaysAliveChangeWhenKilled = 2L;

	// Token: 0x040038B5 RID: 14517
	public static int LootBonusEvery;

	// Token: 0x040038B6 RID: 14518
	public static int LootBonusMaxCount;

	// Token: 0x040038B7 RID: 14519
	public static float LootBonusScale;

	// Token: 0x040038B8 RID: 14520
	public static int LootWanderingBonusEvery;

	// Token: 0x040038B9 RID: 14521
	public static float LootWanderingBonusScale;

	// Token: 0x040038BA RID: 14522
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<string, GameStageDefinition> gameStages = new Dictionary<string, GameStageDefinition>();

	// Token: 0x040038BB RID: 14523
	public string name;

	// Token: 0x040038BC RID: 14524
	[PublicizedFrom(EAccessModifier.Private)]
	public List<GameStageDefinition.Stage> stages = new List<GameStageDefinition.Stage>();

	// Token: 0x02000985 RID: 2437
	public class Stage
	{
		// Token: 0x06004978 RID: 18808 RVA: 0x001D0DBA File Offset: 0x001CEFBA
		public Stage(int _stageNum)
		{
			this.stageNum = _stageNum;
		}

		// Token: 0x06004979 RID: 18809 RVA: 0x001D0DD4 File Offset: 0x001CEFD4
		public void AddSpawnGroup(GameStageDefinition.SpawnGroup spawn)
		{
			this.spawnGroups.Add(spawn);
		}

		// Token: 0x0600497A RID: 18810 RVA: 0x001D0DE2 File Offset: 0x001CEFE2
		public GameStageDefinition.SpawnGroup GetSpawnGroup(int index)
		{
			if (index >= 0 && index < this.spawnGroups.Count)
			{
				return this.spawnGroups[index];
			}
			return null;
		}

		// Token: 0x170007BB RID: 1979
		// (get) Token: 0x0600497B RID: 18811 RVA: 0x001D0E04 File Offset: 0x001CF004
		public int Count
		{
			get
			{
				return this.spawnGroups.Count;
			}
		}

		// Token: 0x040038BD RID: 14525
		public readonly int stageNum;

		// Token: 0x040038BE RID: 14526
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly List<GameStageDefinition.SpawnGroup> spawnGroups = new List<GameStageDefinition.SpawnGroup>();
	}

	// Token: 0x02000986 RID: 2438
	public class SpawnGroup
	{
		// Token: 0x0600497C RID: 18812 RVA: 0x001D0E11 File Offset: 0x001CF011
		public SpawnGroup(string _groupName, int _spawnCount, int _maxAlive, int _interval, int _duration)
		{
			this.groupName = _groupName;
			this.spawnCount = (ushort)_spawnCount;
			this.maxAlive = (ushort)_maxAlive;
			this.interval = (ushort)_interval;
			this.duration = (ushort)_duration;
		}

		// Token: 0x040038BF RID: 14527
		public readonly string groupName;

		// Token: 0x040038C0 RID: 14528
		public readonly ushort spawnCount;

		// Token: 0x040038C1 RID: 14529
		public readonly ushort maxAlive;

		// Token: 0x040038C2 RID: 14530
		public readonly ushort interval;

		// Token: 0x040038C3 RID: 14531
		public readonly ushort duration;
	}
}
