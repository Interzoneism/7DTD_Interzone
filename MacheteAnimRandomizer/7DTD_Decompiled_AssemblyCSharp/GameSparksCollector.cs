using System;
using System.Collections.Generic;
using GameSparks.Core;

// Token: 0x02000F9A RID: 3994
public static class GameSparksCollector
{
	// Token: 0x17000D4A RID: 3402
	// (get) Token: 0x06007F3C RID: 32572 RVA: 0x0033A830 File Offset: 0x00338A30
	// (set) Token: 0x06007F3D RID: 32573 RVA: 0x0033A837 File Offset: 0x00338A37
	public static bool CollectGamePlayData { get; set; }

	// Token: 0x06007F3E RID: 32574 RVA: 0x0033A840 File Offset: 0x00338A40
	[PublicizedFrom(EAccessModifier.Private)]
	public static GSRequestData GetObject(string _keyString, GSRequestData _collection)
	{
		object obj = GameSparksCollector.lockObject;
		GSRequestData result;
		lock (obj)
		{
			GSRequestData gsrequestData = _collection.GetGSData(_keyString) as GSRequestData;
			if (gsrequestData != null)
			{
				result = gsrequestData;
			}
			else
			{
				gsrequestData = new GSRequestData();
				_collection.Add(_keyString, gsrequestData);
				result = gsrequestData;
			}
		}
		return result;
	}

	// Token: 0x06007F3F RID: 32575 RVA: 0x0033A8A0 File Offset: 0x00338AA0
	[PublicizedFrom(EAccessModifier.Private)]
	public static ValueTuple<GSRequestData, string> GetRequestDataAndKey(GameSparksCollector.GSDataCollection _collectionType, GameSparksCollector.GSDataKey _key, string _subKey = null)
	{
		string text = _key.ToStringCached<GameSparksCollector.GSDataKey>();
		GSRequestData gsrequestData = (_collectionType == GameSparksCollector.GSDataCollection.SessionUpdates) ? GameSparksCollector.dataUpdates : GameSparksCollector.dataSessionTotal;
		if (_subKey == null)
		{
			return new ValueTuple<GSRequestData, string>(gsrequestData, text);
		}
		return new ValueTuple<GSRequestData, string>(GameSparksCollector.GetObject(text, gsrequestData), _subKey);
	}

	// Token: 0x06007F40 RID: 32576 RVA: 0x0033A8E0 File Offset: 0x00338AE0
	public static void SetValue(GameSparksCollector.GSDataKey _key, string _subKey, int _value, bool _isGamePlay = true, GameSparksCollector.GSDataCollection _collectionType = GameSparksCollector.GSDataCollection.SessionUpdates)
	{
		if (!_isGamePlay || GameSparksCollector.CollectGamePlayData)
		{
			object obj = GameSparksCollector.lockObject;
			lock (obj)
			{
				ValueTuple<GSRequestData, string> requestDataAndKey = GameSparksCollector.GetRequestDataAndKey(_collectionType, _key, _subKey);
				GSRequestData item = requestDataAndKey.Item1;
				string item2 = requestDataAndKey.Item2;
				item.AddNumber(item2, _value);
			}
		}
	}

	// Token: 0x06007F41 RID: 32577 RVA: 0x0033A944 File Offset: 0x00338B44
	public static void SetValue(GameSparksCollector.GSDataKey _key, string _subKey, string _value, bool _isGamePlay = true, GameSparksCollector.GSDataCollection _collectionType = GameSparksCollector.GSDataCollection.SessionUpdates)
	{
		if (!_isGamePlay || GameSparksCollector.CollectGamePlayData)
		{
			object obj = GameSparksCollector.lockObject;
			lock (obj)
			{
				ValueTuple<GSRequestData, string> requestDataAndKey = GameSparksCollector.GetRequestDataAndKey(_collectionType, _key, _subKey);
				GSRequestData item = requestDataAndKey.Item1;
				string item2 = requestDataAndKey.Item2;
				item.AddString(item2, _value);
			}
		}
	}

	// Token: 0x06007F42 RID: 32578 RVA: 0x0033A9A8 File Offset: 0x00338BA8
	public static void IncrementCounter(GameSparksCollector.GSDataKey _key, string _subKey, int _increment, bool _isGamePlay = true, GameSparksCollector.GSDataCollection _collectionType = GameSparksCollector.GSDataCollection.SessionUpdates)
	{
		if (!_isGamePlay || GameSparksCollector.CollectGamePlayData)
		{
			object obj = GameSparksCollector.lockObject;
			lock (obj)
			{
				ValueTuple<GSRequestData, string> requestDataAndKey = GameSparksCollector.GetRequestDataAndKey(_collectionType, _key, _subKey);
				GSRequestData item = requestDataAndKey.Item1;
				string item2 = requestDataAndKey.Item2;
				int num = item.GetInt(item2).GetValueOrDefault();
				num += _increment;
				item.AddNumber(item2, num);
			}
		}
	}

	// Token: 0x06007F43 RID: 32579 RVA: 0x0033AA24 File Offset: 0x00338C24
	public static void IncrementCounter(GameSparksCollector.GSDataKey _key, string _subKey, float _increment, bool _isGamePlay = true, GameSparksCollector.GSDataCollection _collectionType = GameSparksCollector.GSDataCollection.SessionUpdates)
	{
		if (!_isGamePlay || GameSparksCollector.CollectGamePlayData)
		{
			object obj = GameSparksCollector.lockObject;
			lock (obj)
			{
				ValueTuple<GSRequestData, string> requestDataAndKey = GameSparksCollector.GetRequestDataAndKey(_collectionType, _key, _subKey);
				GSRequestData item = requestDataAndKey.Item1;
				string item2 = requestDataAndKey.Item2;
				float num = item.GetFloat(item2).GetValueOrDefault();
				num += _increment;
				item.AddNumber(item2, num);
			}
		}
	}

	// Token: 0x06007F44 RID: 32580 RVA: 0x0033AAA0 File Offset: 0x00338CA0
	public static void SetMax(GameSparksCollector.GSDataKey _key, string _subKey, int _currentValue, bool _isGamePlay = true, GameSparksCollector.GSDataCollection _collectionType = GameSparksCollector.GSDataCollection.SessionUpdates)
	{
		if (!_isGamePlay || GameSparksCollector.CollectGamePlayData)
		{
			object obj = GameSparksCollector.lockObject;
			lock (obj)
			{
				ValueTuple<GSRequestData, string> requestDataAndKey = GameSparksCollector.GetRequestDataAndKey(_collectionType, _key, _subKey);
				GSRequestData item = requestDataAndKey.Item1;
				string item2 = requestDataAndKey.Item2;
				int num = item.GetInt(item2) ?? int.MinValue;
				num = Math.Max(num, _currentValue);
				item.AddNumber(item2, num);
			}
		}
	}

	// Token: 0x06007F45 RID: 32581 RVA: 0x0033AB30 File Offset: 0x00338D30
	public static GSRequestData GetSessionUpdateDataAndReset()
	{
		object obj = GameSparksCollector.lockObject;
		GSRequestData result;
		lock (obj)
		{
			GSRequestData gsrequestData = GameSparksCollector.dataUpdates;
			GameSparksCollector.dataUpdates = new GSRequestData();
			result = gsrequestData;
		}
		return result;
	}

	// Token: 0x06007F46 RID: 32582 RVA: 0x0033AB7C File Offset: 0x00338D7C
	public static GSRequestData GetSessionTotalData(bool _reset)
	{
		if (!_reset)
		{
			return GameSparksCollector.dataSessionTotal;
		}
		object obj = GameSparksCollector.lockObject;
		GSRequestData result;
		lock (obj)
		{
			GSRequestData gsrequestData = GameSparksCollector.dataSessionTotal;
			GameSparksCollector.dataSessionTotal = new GSRequestData();
			result = gsrequestData;
		}
		return result;
	}

	// Token: 0x06007F47 RID: 32583 RVA: 0x0033ABD0 File Offset: 0x00338DD0
	public static void PlayerLevelUp(EntityPlayerLocal _localPlayer, int _level)
	{
		if (_level == 15)
		{
			GameSparksCollector.SendSaveTimePlayed(GameSparksCollector.GSDataKey.HoursPlayedAtLevel15, _localPlayer);
			GameSparksCollector.SendSkillStats(GameSparksCollector.GSDataKey.SkillsPurchasedAtLevel15, _localPlayer);
			return;
		}
		if (_level == 30)
		{
			GameSparksCollector.SendSaveTimePlayed(GameSparksCollector.GSDataKey.HoursPlayedAtLevel30, _localPlayer);
			GameSparksCollector.SendSkillStats(GameSparksCollector.GSDataKey.SkillsPurchasedAtLevel30, _localPlayer);
			return;
		}
		if (_level != 50)
		{
			return;
		}
		GameSparksCollector.SendSaveTimePlayed(GameSparksCollector.GSDataKey.HoursPlayedAtLevel50, _localPlayer);
		GameSparksCollector.SendSkillStats(GameSparksCollector.GSDataKey.SkillsPurchasedAtLevel50, _localPlayer);
	}

	// Token: 0x06007F48 RID: 32584 RVA: 0x0033AC10 File Offset: 0x00338E10
	[PublicizedFrom(EAccessModifier.Private)]
	public static void SendSkillStats(GameSparksCollector.GSDataKey _key, EntityPlayerLocal _localPlayer)
	{
		foreach (KeyValuePair<int, ProgressionValue> keyValuePair in _localPlayer.Progression.GetDict())
		{
			ProgressionClass progressionClass = keyValuePair.Value.ProgressionClass;
			for (int i = progressionClass.MinLevel + 1; i <= keyValuePair.Value.Level; i++)
			{
				string subKey;
				if (progressionClass.Parent == null || progressionClass.Parent == progressionClass)
				{
					subKey = string.Format("{0}_{1}", progressionClass.Name, i);
				}
				else
				{
					ProgressionClass parent = progressionClass.Parent;
					while (parent.Parent != null && parent.Parent != parent)
					{
						parent = parent.Parent;
					}
					subKey = string.Format("{0}_{1}_{2}", parent.Name, progressionClass.Name, i);
				}
				GameSparksCollector.SetValue(_key, subKey, 1, true, GameSparksCollector.GSDataCollection.SessionUpdates);
			}
		}
	}

	// Token: 0x06007F49 RID: 32585 RVA: 0x0033AD14 File Offset: 0x00338F14
	[PublicizedFrom(EAccessModifier.Private)]
	public static void SendSaveTimePlayed(GameSparksCollector.GSDataKey _key, EntityPlayerLocal _localPlayer)
	{
		int num = (int)(_localPlayer.totalTimePlayed / 60f);
		if (num > 0)
		{
			GameSparksCollector.SetValue(_key, null, num, true, GameSparksCollector.GSDataCollection.SessionUpdates);
		}
	}

	// Token: 0x0400621C RID: 25116
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly object lockObject = new object();

	// Token: 0x0400621D RID: 25117
	[PublicizedFrom(EAccessModifier.Private)]
	public static GSRequestData dataUpdates = new GSRequestData();

	// Token: 0x0400621E RID: 25118
	[PublicizedFrom(EAccessModifier.Private)]
	public static GSRequestData dataSessionTotal = new GSRequestData();

	// Token: 0x02000F9B RID: 3995
	public enum GSDataKey
	{
		// Token: 0x04006221 RID: 25121
		HoursPlayedAtLevel15,
		// Token: 0x04006222 RID: 25122
		HoursPlayedAtLevel30,
		// Token: 0x04006223 RID: 25123
		HoursPlayedAtLevel50,
		// Token: 0x04006224 RID: 25124
		SkillsPurchasedAtLevel15,
		// Token: 0x04006225 RID: 25125
		SkillsPurchasedAtLevel30,
		// Token: 0x04006226 RID: 25126
		SkillsPurchasedAtLevel50,
		// Token: 0x04006227 RID: 25127
		PlayerLevelAtHour,
		// Token: 0x04006228 RID: 25128
		XpEarnedBy,
		// Token: 0x04006229 RID: 25129
		PlayerDeathCauses,
		// Token: 0x0400622A RID: 25130
		ZombiesKilledBy,
		// Token: 0x0400622B RID: 25131
		CraftedItems,
		// Token: 0x0400622C RID: 25132
		TraderItemsBought,
		// Token: 0x0400622D RID: 25133
		VendingItemsBought,
		// Token: 0x0400622E RID: 25134
		TraderMoneySpentOn,
		// Token: 0x0400622F RID: 25135
		VendingMoneySpentOn,
		// Token: 0x04006230 RID: 25136
		TotalMoneySpentOn,
		// Token: 0x04006231 RID: 25137
		PeakConcurrentClients,
		// Token: 0x04006232 RID: 25138
		PeakConcurrentPlayers,
		// Token: 0x04006233 RID: 25139
		QuestTraderToTraderDistance,
		// Token: 0x04006234 RID: 25140
		QuestAcceptedDistance,
		// Token: 0x04006235 RID: 25141
		QuestOfferedDistance,
		// Token: 0x04006236 RID: 25142
		QuestStarterTraderDistance,
		// Token: 0x04006237 RID: 25143
		PlayerProfileIsCustom,
		// Token: 0x04006238 RID: 25144
		PlayerArchetypeName,
		// Token: 0x04006239 RID: 25145
		UsedTwitchIntegration
	}

	// Token: 0x02000F9C RID: 3996
	public enum GSDataCollection
	{
		// Token: 0x0400623B RID: 25147
		SessionTotal,
		// Token: 0x0400623C RID: 25148
		SessionUpdates
	}
}
