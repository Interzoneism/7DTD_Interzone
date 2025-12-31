using System;
using System.Diagnostics;
using UnityEngine;

// Token: 0x02000924 RID: 2340
public class GameRandomManager
{
	// Token: 0x17000752 RID: 1874
	// (get) Token: 0x060045D3 RID: 17875 RVA: 0x001BDE85 File Offset: 0x001BC085
	public static GameRandomManager Instance
	{
		get
		{
			if (GameRandomManager.instance == null)
			{
				GameRandomManager.instance = new GameRandomManager();
				GameRandomManager.instance.SetBaseSeed((int)Stopwatch.GetTimestamp());
				GameRandomManager.instance.tempRandom = new GameRandom();
			}
			return GameRandomManager.instance;
		}
	}

	// Token: 0x060045D4 RID: 17876 RVA: 0x001BDEBC File Offset: 0x001BC0BC
	public void SetBaseSeed(int _baseSeed)
	{
		this.baseSeed = _baseSeed;
	}

	// Token: 0x17000753 RID: 1875
	// (get) Token: 0x060045D5 RID: 17877 RVA: 0x001BDEC5 File Offset: 0x001BC0C5
	public int BaseSeed
	{
		get
		{
			return this.baseSeed;
		}
	}

	// Token: 0x060045D6 RID: 17878 RVA: 0x001BDECD File Offset: 0x001BC0CD
	public GameRandom CreateGameRandom()
	{
		return this.CreateGameRandom(this.baseSeed);
	}

	// Token: 0x060045D7 RID: 17879 RVA: 0x001BDEDB File Offset: 0x001BC0DB
	public GameRandom CreateGameRandom(int _seed)
	{
		GameRandom gameRandom = this.pool.AllocSync(false);
		gameRandom.SetSeed(_seed);
		return gameRandom;
	}

	// Token: 0x060045D8 RID: 17880 RVA: 0x001BDEF0 File Offset: 0x001BC0F0
	public void FreeGameRandom(GameRandom _gameRandom)
	{
		if (_gameRandom == null)
		{
			return;
		}
		this.pool.FreeSync(_gameRandom);
	}

	// Token: 0x060045D9 RID: 17881 RVA: 0x001BDF02 File Offset: 0x001BC102
	public GameRandom GetTempGameRandom(int _seed)
	{
		this.tempRandom.SetSeed(_seed);
		return this.tempRandom;
	}

	// Token: 0x060045DA RID: 17882 RVA: 0x001BDF18 File Offset: 0x001BC118
	[PublicizedFrom(EAccessModifier.Private)]
	public static void log(string _format, params object[] _values)
	{
		float num = -1f;
		if (ThreadManager.IsMainThread())
		{
			num = Time.time;
			if (num != GameRandomManager.logTime)
			{
				if (GameRandomManager.logCount > 10)
				{
					Log.Warning("GameRandomManager {0} more...", new object[]
					{
						GameRandomManager.logCount - 10
					});
				}
				GameRandomManager.logTime = num;
				GameRandomManager.logCount = 0;
			}
			if (++GameRandomManager.logCount > 10)
			{
				return;
			}
		}
		Log.Warning(string.Format("{0} GameRandomManager ", num.ToCultureInvariantString()) + _format, _values);
	}

	// Token: 0x04003677 RID: 13943
	[PublicizedFrom(EAccessModifier.Private)]
	public static GameRandomManager instance;

	// Token: 0x04003678 RID: 13944
	[PublicizedFrom(EAccessModifier.Private)]
	public int baseSeed;

	// Token: 0x04003679 RID: 13945
	[PublicizedFrom(EAccessModifier.Private)]
	public MemoryPooledObject<GameRandom> pool = new MemoryPooledObject<GameRandom>(30);

	// Token: 0x0400367A RID: 13946
	[PublicizedFrom(EAccessModifier.Private)]
	public GameRandom tempRandom;

	// Token: 0x0400367B RID: 13947
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cLogMax = 10;

	// Token: 0x0400367C RID: 13948
	[PublicizedFrom(EAccessModifier.Private)]
	public static float logTime;

	// Token: 0x0400367D RID: 13949
	[PublicizedFrom(EAccessModifier.Private)]
	public static int logCount;
}
