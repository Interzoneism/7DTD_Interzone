using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

// Token: 0x02000F4C RID: 3916
public class GameInfoIntLimits
{
	// Token: 0x17000D0B RID: 3339
	// (get) Token: 0x06007C5C RID: 31836 RVA: 0x003251A8 File Offset: 0x003233A8
	// (set) Token: 0x06007C5D RID: 31837 RVA: 0x003251B0 File Offset: 0x003233B0
	public int? min { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000D0C RID: 3340
	// (get) Token: 0x06007C5E RID: 31838 RVA: 0x003251B9 File Offset: 0x003233B9
	// (set) Token: 0x06007C5F RID: 31839 RVA: 0x003251C1 File Offset: 0x003233C1
	public int? max { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06007C60 RID: 31840 RVA: 0x003251CA File Offset: 0x003233CA
	public GameInfoIntLimits(int? min, int? max)
	{
		this.min = min;
		this.max = max;
	}

	// Token: 0x06007C61 RID: 31841 RVA: 0x003251E0 File Offset: 0x003233E0
	public bool IsValueValid(int value)
	{
		if (this.min != null)
		{
			int? num = this.min;
			if (!(value >= num.GetValueOrDefault() & num != null))
			{
				return false;
			}
		}
		if (this.max != null)
		{
			int? num = this.max;
			return value <= num.GetValueOrDefault() & num != null;
		}
		return true;
	}

	// Token: 0x06007C62 RID: 31842 RVA: 0x0032524C File Offset: 0x0032344C
	public string GetRangeDescription()
	{
		if (this.min != null && this.max != null)
		{
			int? min = this.min;
			int? max = this.max;
			if (min.GetValueOrDefault() == max.GetValueOrDefault() & min != null == (max != null))
			{
				return this.min.ToString();
			}
			return string.Format("{0} - {1}", this.min, this.max);
		}
		else
		{
			if (this.min != null)
			{
				return string.Format(">= {0}", this.min);
			}
			if (this.max != null)
			{
				return string.Format("<= {0}", this.max);
			}
			return "any value";
		}
	}

	// Token: 0x06007C63 RID: 31843 RVA: 0x00325334 File Offset: 0x00323534
	public static bool ValidateGameServerInfoCrossplaySettings(GameServerInfo _gsi)
	{
		if (!_gsi.GetValue(GameInfoBool.AllowCrossplay))
		{
			return true;
		}
		bool result = true;
		foreach (KeyValuePair<GameInfoInt, List<GameInfoIntLimits>> keyValuePair in GameInfoIntLimits.CrossplayLimits)
		{
			GameInfoInt gameInfoInt;
			List<GameInfoIntLimits> list;
			keyValuePair.Deconstruct(out gameInfoInt, out list);
			GameInfoInt gameInfoInt2 = gameInfoInt;
			int num;
			if (!_gsi.Ints.TryGetValue(gameInfoInt2, out num))
			{
				return true;
			}
			ValueTuple<bool, List<GameInfoIntLimits>> valueTuple = GameInfoIntLimits.IsWithinLimits(gameInfoInt2, num);
			bool item = valueTuple.Item1;
			List<GameInfoIntLimits> item2 = valueTuple.Item2;
			if (!item)
			{
				result = false;
				GameInfoIntLimits.logInvalidGameInfoInt(gameInfoInt2, num, item2);
			}
		}
		return result;
	}

	// Token: 0x06007C64 RID: 31844 RVA: 0x003253DC File Offset: 0x003235DC
	public static bool ValidateGamePrefsCrossplaySettings()
	{
		if (!GamePrefs.GetBool(EnumGamePrefs.ServerAllowCrossplay))
		{
			return true;
		}
		bool result = true;
		foreach (KeyValuePair<GameInfoInt, List<GameInfoIntLimits>> keyValuePair in GameInfoIntLimits.CrossplayLimits)
		{
			GameInfoInt gameInfoInt;
			List<GameInfoIntLimits> list;
			keyValuePair.Deconstruct(out gameInfoInt, out list);
			GameInfoInt gameInfoInt2 = gameInfoInt;
			EnumGamePrefs eProperty;
			if (!GameInfoIntLimits.GameInfoIntToGamePrefs.TryGetValue(gameInfoInt2, out eProperty))
			{
				return true;
			}
			int @int = GamePrefs.GetInt(eProperty);
			ValueTuple<bool, List<GameInfoIntLimits>> valueTuple = GameInfoIntLimits.IsWithinLimits(gameInfoInt2, @int);
			bool item = valueTuple.Item1;
			List<GameInfoIntLimits> item2 = valueTuple.Item2;
			if (!item)
			{
				result = false;
				GameInfoIntLimits.logInvalidGameInfoInt(gameInfoInt2, @int, item2);
			}
		}
		return result;
	}

	// Token: 0x06007C65 RID: 31845 RVA: 0x0032548C File Offset: 0x0032368C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void logInvalidGameInfoInt(GameInfoInt _gameInfoInt, int _value, List<GameInfoIntLimits> _violatedLimits)
	{
		string arg = GameInfoIntLimits.BuildAcceptableRangesString(_violatedLimits);
		Log.Warning(string.Format("Server setting {0} with value {1} outside of allowed range for cross-play ({2}).", _gameInfoInt.ToStringCached<GameInfoInt>(), _value, arg));
	}

	// Token: 0x06007C66 RID: 31846 RVA: 0x003254BC File Offset: 0x003236BC
	[return: TupleElementNames(new string[]
	{
		"isValid",
		"allLimits"
	})]
	public static ValueTuple<bool, List<GameInfoIntLimits>> IsWithinLimits(EnumGamePrefs _pref, out int _value)
	{
		_value = 0;
		GameInfoInt info;
		if (GameInfoIntLimits.GamePrefsToGameInfoInt.TryGetValue(_pref, out info))
		{
			int @int = GamePrefs.GetInt(_pref);
			return GameInfoIntLimits.IsWithinLimits(info, @int);
		}
		return new ValueTuple<bool, List<GameInfoIntLimits>>(true, null);
	}

	// Token: 0x06007C67 RID: 31847 RVA: 0x003254F4 File Offset: 0x003236F4
	[return: TupleElementNames(new string[]
	{
		"isValid",
		"allLimits"
	})]
	public static ValueTuple<bool, List<GameInfoIntLimits>> IsWithinLimits(GameInfoInt _info, int inValue)
	{
		List<GameInfoIntLimits> list;
		if (!GameInfoIntLimits.CrossplayLimits.TryGetValue(_info, out list))
		{
			return new ValueTuple<bool, List<GameInfoIntLimits>>(true, null);
		}
		using (List<GameInfoIntLimits>.Enumerator enumerator = list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.IsValueValid(inValue))
				{
					return new ValueTuple<bool, List<GameInfoIntLimits>>(true, null);
				}
			}
		}
		return new ValueTuple<bool, List<GameInfoIntLimits>>(false, list);
	}

	// Token: 0x06007C68 RID: 31848 RVA: 0x0032556C File Offset: 0x0032376C
	[PublicizedFrom(EAccessModifier.Private)]
	public static string BuildAcceptableRangesString(List<GameInfoIntLimits> limits)
	{
		if (limits == null || limits.Count == 0)
		{
			return "any value";
		}
		List<string> list = (from limit in limits
		select limit.GetRangeDescription()).ToList<string>();
		if (list.Count == 1)
		{
			return list[0];
		}
		if (list.Count == 2)
		{
			return list[0] + ", " + list[1];
		}
		return string.Join(", ", list);
	}

	// Token: 0x06007C69 RID: 31849 RVA: 0x003255F4 File Offset: 0x003237F4
	public static bool IsWithinIntValueLimits(GameServerInfo _gsi, out string _errorMessage)
	{
		_errorMessage = string.Empty;
		foreach (KeyValuePair<GameInfoInt, List<GameInfoIntLimits>> keyValuePair in GameInfoIntLimits.CrossplayLimits)
		{
			int num;
			if (!_gsi.Ints.TryGetValue(keyValuePair.Key, out num))
			{
				return true;
			}
			ValueTuple<bool, List<GameInfoIntLimits>> valueTuple = GameInfoIntLimits.IsWithinLimits(keyValuePair.Key, num);
			bool item = valueTuple.Item1;
			List<GameInfoIntLimits> item2 = valueTuple.Item2;
			if (!item)
			{
				GameInfoIntLimits.logInvalidGameInfoInt(keyValuePair.Key, num, item2);
				string arg = keyValuePair.Key.ToStringCached<GameInfoInt>();
				string arg2 = GameInfoIntLimits.BuildAcceptableRangesString(item2);
				_errorMessage = string.Format(Localization.Get("xuiNonStandardGameSettings", false), arg, num, arg2);
				return false;
			}
		}
		return true;
	}

	// Token: 0x04005F48 RID: 24392
	public static readonly Dictionary<GameInfoInt, EnumGamePrefs> GameInfoIntToGamePrefs = new Dictionary<GameInfoInt, EnumGamePrefs>
	{
		{
			GameInfoInt.XPMultiplier,
			EnumGamePrefs.XPMultiplier
		},
		{
			GameInfoInt.DayNightLength,
			EnumGamePrefs.DayNightLength
		},
		{
			GameInfoInt.BloodMoonEnemyCount,
			EnumGamePrefs.BloodMoonEnemyCount
		},
		{
			GameInfoInt.AirDropFrequency,
			EnumGamePrefs.AirDropFrequency
		},
		{
			GameInfoInt.BlockDamagePlayer,
			EnumGamePrefs.BlockDamagePlayer
		},
		{
			GameInfoInt.BlockDamageAI,
			EnumGamePrefs.BlockDamagePlayer
		},
		{
			GameInfoInt.BlockDamageAIBM,
			EnumGamePrefs.BlockDamageAIBM
		},
		{
			GameInfoInt.LootAbundance,
			EnumGamePrefs.LootAbundance
		},
		{
			GameInfoInt.LootRespawnDays,
			EnumGamePrefs.LootRespawnDays
		}
	};

	// Token: 0x04005F49 RID: 24393
	public static readonly Dictionary<EnumGamePrefs, GameInfoInt> GamePrefsToGameInfoInt = new Dictionary<EnumGamePrefs, GameInfoInt>
	{
		{
			EnumGamePrefs.XPMultiplier,
			GameInfoInt.XPMultiplier
		},
		{
			EnumGamePrefs.DayNightLength,
			GameInfoInt.DayNightLength
		},
		{
			EnumGamePrefs.BloodMoonEnemyCount,
			GameInfoInt.BloodMoonEnemyCount
		},
		{
			EnumGamePrefs.AirDropFrequency,
			GameInfoInt.AirDropFrequency
		},
		{
			EnumGamePrefs.BlockDamagePlayer,
			GameInfoInt.BlockDamagePlayer
		},
		{
			EnumGamePrefs.BlockDamageAI,
			GameInfoInt.BlockDamagePlayer
		},
		{
			EnumGamePrefs.BlockDamageAIBM,
			GameInfoInt.BlockDamageAIBM
		},
		{
			EnumGamePrefs.LootAbundance,
			GameInfoInt.LootAbundance
		},
		{
			EnumGamePrefs.LootRespawnDays,
			GameInfoInt.LootRespawnDays
		}
	};

	// Token: 0x04005F4A RID: 24394
	public static readonly Dictionary<GameInfoInt, List<GameInfoIntLimits>> CrossplayLimits = new Dictionary<GameInfoInt, List<GameInfoIntLimits>>
	{
		{
			GameInfoInt.XPMultiplier,
			new List<GameInfoIntLimits>
			{
				new GameInfoIntLimits(new int?(25), new int?(300))
			}
		},
		{
			GameInfoInt.DayNightLength,
			new List<GameInfoIntLimits>
			{
				new GameInfoIntLimits(new int?(10), null)
			}
		},
		{
			GameInfoInt.BloodMoonEnemyCount,
			new List<GameInfoIntLimits>
			{
				new GameInfoIntLimits(new int?(4), new int?(64))
			}
		},
		{
			GameInfoInt.AirDropFrequency,
			new List<GameInfoIntLimits>
			{
				new GameInfoIntLimits(new int?(0), new int?(0)),
				new GameInfoIntLimits(new int?(24), null)
			}
		},
		{
			GameInfoInt.BlockDamagePlayer,
			new List<GameInfoIntLimits>
			{
				new GameInfoIntLimits(new int?(25), new int?(300))
			}
		},
		{
			GameInfoInt.BlockDamageAI,
			new List<GameInfoIntLimits>
			{
				new GameInfoIntLimits(new int?(25), null)
			}
		},
		{
			GameInfoInt.BlockDamageAIBM,
			new List<GameInfoIntLimits>
			{
				new GameInfoIntLimits(new int?(25), null)
			}
		},
		{
			GameInfoInt.LootAbundance,
			new List<GameInfoIntLimits>
			{
				new GameInfoIntLimits(null, new int?(200))
			}
		},
		{
			GameInfoInt.LootRespawnDays,
			new List<GameInfoIntLimits>
			{
				new GameInfoIntLimits(new int?(-1), new int?(0)),
				new GameInfoIntLimits(new int?(5), null)
			}
		}
	};
}
