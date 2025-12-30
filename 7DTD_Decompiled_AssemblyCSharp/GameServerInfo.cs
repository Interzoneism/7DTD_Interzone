using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Platform;
using UnityEngine;

// Token: 0x020006CE RID: 1742
public class GameServerInfo
{
	// Token: 0x14000045 RID: 69
	// (add) Token: 0x0600335D RID: 13149 RVA: 0x0015C390 File Offset: 0x0015A590
	// (remove) Token: 0x0600335E RID: 13150 RVA: 0x0015C3C8 File Offset: 0x0015A5C8
	public event Action<GameServerInfo> OnChangedAny;

	// Token: 0x14000046 RID: 70
	// (add) Token: 0x0600335F RID: 13151 RVA: 0x0015C400 File Offset: 0x0015A600
	// (remove) Token: 0x06003360 RID: 13152 RVA: 0x0015C438 File Offset: 0x0015A638
	public event Action<GameServerInfo, GameInfoString> OnChangedString;

	// Token: 0x14000047 RID: 71
	// (add) Token: 0x06003361 RID: 13153 RVA: 0x0015C470 File Offset: 0x0015A670
	// (remove) Token: 0x06003362 RID: 13154 RVA: 0x0015C4A8 File Offset: 0x0015A6A8
	public event Action<GameServerInfo, GameInfoInt> OnChangedInt;

	// Token: 0x14000048 RID: 72
	// (add) Token: 0x06003363 RID: 13155 RVA: 0x0015C4E0 File Offset: 0x0015A6E0
	// (remove) Token: 0x06003364 RID: 13156 RVA: 0x0015C518 File Offset: 0x0015A718
	public event Action<GameServerInfo, GameInfoBool> OnChangedBool;

	// Token: 0x170004EA RID: 1258
	// (get) Token: 0x06003365 RID: 13157 RVA: 0x0015C54D File Offset: 0x0015A74D
	public bool IsValid
	{
		get
		{
			return !this.isBroken;
		}
	}

	// Token: 0x170004EB RID: 1259
	// (get) Token: 0x06003366 RID: 13158 RVA: 0x0015C558 File Offset: 0x0015A758
	public bool IsDedicated
	{
		get
		{
			return this.GetValue(GameInfoBool.IsDedicated);
		}
	}

	// Token: 0x170004EC RID: 1260
	// (get) Token: 0x06003367 RID: 13159 RVA: 0x0015C561 File Offset: 0x0015A761
	public bool IsDedicatedStock
	{
		get
		{
			return this.GetValue(GameInfoBool.IsDedicated) && this.GetValue(GameInfoBool.StockSettings) && !this.GetValue(GameInfoBool.ModdedConfig);
		}
	}

	// Token: 0x170004ED RID: 1261
	// (get) Token: 0x06003368 RID: 13160 RVA: 0x0015C582 File Offset: 0x0015A782
	public bool IsDedicatedModded
	{
		get
		{
			return this.GetValue(GameInfoBool.IsDedicated) && this.GetValue(GameInfoBool.ModdedConfig);
		}
	}

	// Token: 0x170004EE RID: 1262
	// (get) Token: 0x06003369 RID: 13161 RVA: 0x0015C597 File Offset: 0x0015A797
	public bool IsPeerToPeer
	{
		get
		{
			return !this.GetValue(GameInfoBool.IsDedicated) && !this.isNoResponse;
		}
	}

	// Token: 0x170004EF RID: 1263
	// (get) Token: 0x0600336A RID: 13162 RVA: 0x0015C5AD File Offset: 0x0015A7AD
	public bool AllowsCrossplay
	{
		get
		{
			return this.GetValue(GameInfoBool.AllowCrossplay);
		}
	}

	// Token: 0x170004F0 RID: 1264
	// (get) Token: 0x0600336B RID: 13163 RVA: 0x0015C5B7 File Offset: 0x0015A7B7
	public bool EACEnabled
	{
		get
		{
			return this.GetValue(GameInfoBool.EACEnabled);
		}
	}

	// Token: 0x170004F1 RID: 1265
	// (get) Token: 0x0600336C RID: 13164 RVA: 0x0015C5C0 File Offset: 0x0015A7C0
	public bool IgnoresSanctions
	{
		get
		{
			return this.GetValue(GameInfoBool.SanctionsIgnored);
		}
	}

	// Token: 0x170004F2 RID: 1266
	// (get) Token: 0x0600336D RID: 13165 RVA: 0x0015C5CC File Offset: 0x0015A7CC
	public EPlayGroup PlayGroup
	{
		get
		{
			EPlayGroup result;
			if (!EnumUtils.TryParse<EPlayGroup>(this.GetValue(GameInfoString.PlayGroup), out result, false))
			{
				return EPlayGroup.Unknown;
			}
			return result;
		}
	}

	// Token: 0x170004F3 RID: 1267
	// (get) Token: 0x0600336E RID: 13166 RVA: 0x0015C5EE File Offset: 0x0015A7EE
	// (set) Token: 0x0600336F RID: 13167 RVA: 0x0015C5F6 File Offset: 0x0015A7F6
	public bool IsFriends
	{
		get
		{
			return this.isFriends;
		}
		set
		{
			if (value != this.isFriends)
			{
				this.isFriends = value;
				Action<GameServerInfo> onChangedAny = this.OnChangedAny;
				if (onChangedAny == null)
				{
					return;
				}
				onChangedAny(this);
			}
		}
	}

	// Token: 0x170004F4 RID: 1268
	// (get) Token: 0x06003370 RID: 13168 RVA: 0x0015C619 File Offset: 0x0015A819
	public bool IsFavoriteHistory
	{
		get
		{
			return this.IsFavorite || this.IsHistory;
		}
	}

	// Token: 0x170004F5 RID: 1269
	// (get) Token: 0x06003371 RID: 13169 RVA: 0x0015C62B File Offset: 0x0015A82B
	// (set) Token: 0x06003372 RID: 13170 RVA: 0x0015C633 File Offset: 0x0015A833
	public bool IsFavorite
	{
		get
		{
			return this.isFavorite;
		}
		set
		{
			if (value != this.isFavorite)
			{
				this.isFavorite = value;
				Action<GameServerInfo> onChangedAny = this.OnChangedAny;
				if (onChangedAny == null)
				{
					return;
				}
				onChangedAny(this);
			}
		}
	}

	// Token: 0x170004F6 RID: 1270
	// (get) Token: 0x06003373 RID: 13171 RVA: 0x0015C656 File Offset: 0x0015A856
	public bool IsHistory
	{
		get
		{
			return this.lastPlayed > 0;
		}
	}

	// Token: 0x170004F7 RID: 1271
	// (get) Token: 0x06003374 RID: 13172 RVA: 0x0015C661 File Offset: 0x0015A861
	// (set) Token: 0x06003375 RID: 13173 RVA: 0x0015C669 File Offset: 0x0015A869
	public int LastPlayedLinux
	{
		get
		{
			return this.lastPlayed;
		}
		set
		{
			if (value != this.lastPlayed)
			{
				this.lastPlayed = value;
				Action<GameServerInfo> onChangedAny = this.OnChangedAny;
				if (onChangedAny == null)
				{
					return;
				}
				onChangedAny(this);
			}
		}
	}

	// Token: 0x170004F8 RID: 1272
	// (get) Token: 0x06003376 RID: 13174 RVA: 0x0015C68C File Offset: 0x0015A88C
	// (set) Token: 0x06003377 RID: 13175 RVA: 0x0015C694 File Offset: 0x0015A894
	public bool IsLAN
	{
		get
		{
			return this.isLan;
		}
		set
		{
			if (value != this.isLan)
			{
				this.isLan = value;
				Action<GameServerInfo> onChangedAny = this.OnChangedAny;
				if (onChangedAny == null)
				{
					return;
				}
				onChangedAny(this);
			}
		}
	}

	// Token: 0x170004F9 RID: 1273
	// (get) Token: 0x06003378 RID: 13176 RVA: 0x0015C6B7 File Offset: 0x0015A8B7
	// (set) Token: 0x06003379 RID: 13177 RVA: 0x0015C6BF File Offset: 0x0015A8BF
	public bool IsLobby
	{
		get
		{
			return this.isLobby;
		}
		set
		{
			if (value != this.isLobby)
			{
				this.isLobby = value;
				Action<GameServerInfo> onChangedAny = this.OnChangedAny;
				if (onChangedAny == null)
				{
					return;
				}
				onChangedAny(this);
			}
		}
	}

	// Token: 0x170004FA RID: 1274
	// (get) Token: 0x0600337A RID: 13178 RVA: 0x0015C6E2 File Offset: 0x0015A8E2
	// (set) Token: 0x0600337B RID: 13179 RVA: 0x0015C6EA File Offset: 0x0015A8EA
	public bool IsNoResponse
	{
		get
		{
			return this.isNoResponse;
		}
		set
		{
			if (value != this.isNoResponse)
			{
				this.isNoResponse = value;
				Action<GameServerInfo> onChangedAny = this.OnChangedAny;
				if (onChangedAny == null)
				{
					return;
				}
				onChangedAny(this);
			}
		}
	}

	// Token: 0x170004FB RID: 1275
	// (get) Token: 0x0600337C RID: 13180 RVA: 0x0015C70D File Offset: 0x0015A90D
	public VersionInformation Version
	{
		get
		{
			return this.version;
		}
	}

	// Token: 0x170004FC RID: 1276
	// (get) Token: 0x0600337D RID: 13181 RVA: 0x0015C715 File Offset: 0x0015A915
	public bool IsCompatibleVersion
	{
		get
		{
			return this.version.Major < 0 || this.version.EqualsMinor(Constants.cVersionInformation);
		}
	}

	// Token: 0x0600337E RID: 13182 RVA: 0x0015C738 File Offset: 0x0015A938
	public GameServerInfo()
	{
		this.OnChangedAny += this.RefreshServerDisplayTexts;
		this.Strings = new ReadOnlyDictionary<GameInfoString, string>(this.tableStrings);
		this.Ints = new ReadOnlyDictionary<GameInfoInt, int>(this.tableInts);
		this.Bools = new ReadOnlyDictionary<GameInfoBool, bool>(this.tableBools);
	}

	// Token: 0x0600337F RID: 13183 RVA: 0x0015C808 File Offset: 0x0015AA08
	public GameServerInfo(GameServerInfo _gsi)
	{
		foreach (KeyValuePair<GameInfoString, string> keyValuePair in _gsi.tableStrings)
		{
			this.SetValue(keyValuePair.Key, keyValuePair.Value);
		}
		foreach (KeyValuePair<GameInfoInt, int> keyValuePair2 in _gsi.tableInts)
		{
			this.SetValue(keyValuePair2.Key, keyValuePair2.Value);
		}
		foreach (KeyValuePair<GameInfoBool, bool> keyValuePair3 in _gsi.tableBools)
		{
			this.SetValue(keyValuePair3.Key, keyValuePair3.Value);
		}
		this.isBroken = _gsi.isBroken;
		this.isFriends = _gsi.isFriends;
		this.isFavorite = _gsi.isFavorite;
		this.lastPlayed = _gsi.lastPlayed;
		this.isLan = _gsi.isLan;
		this.isLobby = _gsi.isLobby;
		this.isNoResponse = _gsi.isNoResponse;
		this.RefreshServerDisplayTexts(this);
		this.OnChangedAny += this.RefreshServerDisplayTexts;
	}

	// Token: 0x06003380 RID: 13184 RVA: 0x0015C9F4 File Offset: 0x0015ABF4
	public GameServerInfo(string _serverInfoString)
	{
		if (_serverInfoString.Length == 0)
		{
			this.isBroken = true;
			return;
		}
		this.BuildInfoFromString(_serverInfoString);
		this.OnChangedAny += this.RefreshServerDisplayTexts;
	}

	// Token: 0x06003381 RID: 13185 RVA: 0x0015CAA8 File Offset: 0x0015ACA8
	public void Merge(GameServerInfo _gameServerInfo, EServerRelationType _source)
	{
		this.isFriends |= _gameServerInfo.IsFriends;
		this.isFavorite |= _gameServerInfo.IsFavorite;
		this.isLan |= _gameServerInfo.IsLAN;
		if (_source == EServerRelationType.History)
		{
			this.lastPlayed = _gameServerInfo.LastPlayedLinux;
		}
		foreach (KeyValuePair<GameInfoBool, bool> keyValuePair in _gameServerInfo.tableBools)
		{
			if (keyValuePair.Value)
			{
				this.SetValue(keyValuePair.Key, keyValuePair.Value);
			}
		}
		foreach (KeyValuePair<GameInfoString, string> keyValuePair2 in _gameServerInfo.tableStrings)
		{
			if (_source == EServerRelationType.LAN || keyValuePair2.Key != GameInfoString.IP)
			{
				this.SetValue(keyValuePair2.Key, keyValuePair2.Value);
			}
		}
		foreach (KeyValuePair<GameInfoInt, int> keyValuePair3 in _gameServerInfo.tableInts)
		{
			this.SetValue(keyValuePair3.Key, keyValuePair3.Value);
		}
	}

	// Token: 0x06003382 RID: 13186 RVA: 0x0015CC08 File Offset: 0x0015AE08
	[PublicizedFrom(EAccessModifier.Private)]
	public void BuildInfoFromString(string _serverInfoString)
	{
		if (_serverInfoString.Length == 0)
		{
			this.isBroken = true;
			return;
		}
		foreach (string text in _serverInfoString.Substring(0, _serverInfoString.Length - 1).Split(';', StringSplitOptions.None))
		{
			if (text.Length >= 2)
			{
				string[] array2 = text.Split(':', StringSplitOptions.None);
				if (array2.Length >= 2)
				{
					string key = array2[0].Trim(GameServerInfo.whiteSpaceChars);
					string value = array2[1];
					this.ParseAny(key, value);
				}
			}
		}
		this.RefreshServerDisplayTexts(this);
	}

	// Token: 0x06003383 RID: 13187 RVA: 0x0015CC90 File Offset: 0x0015AE90
	public bool ParseAny(string _key, string _value)
	{
		bool result;
		try
		{
			GameInfoString key;
			GameInfoInt key2;
			GameInfoBool key3;
			if (EnumUtils.TryParse<GameInfoString>(_key, out key, true))
			{
				this.SetValue(key, _value);
			}
			else if (EnumUtils.TryParse<GameInfoInt>(_key, out key2, true))
			{
				this.SetValue(key2, Convert.ToInt32(_value));
			}
			else if (EnumUtils.TryParse<GameInfoBool>(_key, out key3, true))
			{
				this.SetValue(key3, StringParsers.ParseBool(_value, 0, -1, true));
			}
			result = true;
		}
		catch (Exception)
		{
			string text = this.GetValue(GameInfoString.IP);
			if (string.IsNullOrEmpty(text))
			{
				text = "<unknown>";
			}
			int value = this.GetValue(GameInfoInt.Port);
			Log.Warning("GameServer {0}:{1} replied with invalid setting: {2}={3}", new object[]
			{
				text,
				value,
				_key,
				_value
			});
			this.isBroken = true;
			result = false;
		}
		return result;
	}

	// Token: 0x06003384 RID: 13188 RVA: 0x0015CD54 File Offset: 0x0015AF54
	public bool Parse(string _key, string _value)
	{
		GameInfoString key;
		if (EnumUtils.TryParse<GameInfoString>(_key, out key, true))
		{
			this.SetValue(key, _value);
			return true;
		}
		return false;
	}

	// Token: 0x06003385 RID: 13189 RVA: 0x0015CD7C File Offset: 0x0015AF7C
	public bool Parse(string _key, int _value)
	{
		GameInfoInt key;
		if (EnumUtils.TryParse<GameInfoInt>(_key, out key, true))
		{
			this.SetValue(key, _value);
			return true;
		}
		return false;
	}

	// Token: 0x06003386 RID: 13190 RVA: 0x0015CDA4 File Offset: 0x0015AFA4
	public bool Parse(string _key, bool _value)
	{
		GameInfoBool key;
		if (EnumUtils.TryParse<GameInfoBool>(_key, out key, true))
		{
			this.SetValue(key, _value);
			return true;
		}
		return false;
	}

	// Token: 0x06003387 RID: 13191 RVA: 0x0015CDCC File Offset: 0x0015AFCC
	public void SetValue(GameInfoString _key, string _value)
	{
		if (_value == null)
		{
			_value = "";
		}
		this.tableStrings[_key] = _value.Replace(':', '^').Replace(';', '*');
		if (_key == GameInfoString.IP || _key == GameInfoString.SteamID || _key == GameInfoString.UniqueId)
		{
			this.hashcode = long.MinValue;
		}
		if (_key == GameInfoString.ServerVersion)
		{
			VersionInformation versionInformation;
			if (VersionInformation.TryParseSerializedString(_value, out versionInformation))
			{
				this.version = versionInformation;
			}
			else
			{
				Log.Warning("Server browser: Could not parse version from received data (from entry: " + this.GetValue(GameInfoString.IP) + "): " + _value);
			}
		}
		this.cachedToString = null;
		this.cachedToStringLineBreaks = null;
		Action<GameServerInfo, GameInfoString> onChangedString = this.OnChangedString;
		if (onChangedString != null)
		{
			onChangedString(this, _key);
		}
		Action<GameServerInfo> onChangedAny = this.OnChangedAny;
		if (onChangedAny == null)
		{
			return;
		}
		onChangedAny(this);
	}

	// Token: 0x06003388 RID: 13192 RVA: 0x0015CE84 File Offset: 0x0015B084
	public string GetValue(GameInfoString _key)
	{
		string text;
		if (!this.tableStrings.TryGetValue(_key, out text))
		{
			return "";
		}
		return text.Replace('^', ':').Replace('*', ';');
	}

	// Token: 0x06003389 RID: 13193 RVA: 0x0015CEBC File Offset: 0x0015B0BC
	public void SetValue(GameInfoInt _key, int _value)
	{
		if (_key != GameInfoInt.Ping || !this.tableInts.ContainsKey(GameInfoInt.Ping) || _value >= 0)
		{
			this.tableInts[_key] = _value;
		}
		if (_key == GameInfoInt.Port)
		{
			this.hashcode = long.MinValue;
		}
		this.cachedToString = null;
		this.cachedToStringLineBreaks = null;
		Action<GameServerInfo, GameInfoInt> onChangedInt = this.OnChangedInt;
		if (onChangedInt != null)
		{
			onChangedInt(this, _key);
		}
		Action<GameServerInfo> onChangedAny = this.OnChangedAny;
		if (onChangedAny == null)
		{
			return;
		}
		onChangedAny(this);
	}

	// Token: 0x0600338A RID: 13194 RVA: 0x0015CF34 File Offset: 0x0015B134
	public int GetValue(GameInfoInt _key)
	{
		int result;
		if (!this.tableInts.TryGetValue(_key, out result))
		{
			return -1;
		}
		return result;
	}

	// Token: 0x0600338B RID: 13195 RVA: 0x0015CF54 File Offset: 0x0015B154
	public void SetValue(GameInfoBool _key, bool _value)
	{
		this.tableBools[_key] = _value;
		this.cachedToString = null;
		this.cachedToStringLineBreaks = null;
		Action<GameServerInfo, GameInfoBool> onChangedBool = this.OnChangedBool;
		if (onChangedBool != null)
		{
			onChangedBool(this, _key);
		}
		Action<GameServerInfo> onChangedAny = this.OnChangedAny;
		if (onChangedAny == null)
		{
			return;
		}
		onChangedAny(this);
	}

	// Token: 0x0600338C RID: 13196 RVA: 0x0015CFA0 File Offset: 0x0015B1A0
	public bool GetValue(GameInfoBool _key)
	{
		bool flag;
		return this.tableBools.TryGetValue(_key, out flag) && flag;
	}

	// Token: 0x0600338D RID: 13197 RVA: 0x0015CFBD File Offset: 0x0015B1BD
	public override string ToString()
	{
		return this.ToString(false);
	}

	// Token: 0x0600338E RID: 13198 RVA: 0x0015CFC8 File Offset: 0x0015B1C8
	public string ToString(bool _lineBreaks)
	{
		if ((!_lineBreaks && this.cachedToString == null) || (_lineBreaks && this.cachedToStringLineBreaks == null))
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<GameInfoString, string> keyValuePair in this.tableStrings)
			{
				stringBuilder.Append(keyValuePair.Key.ToStringCached<GameInfoString>());
				stringBuilder.Append(':');
				stringBuilder.Append(keyValuePair.Value);
				stringBuilder.Append(';');
				if (_lineBreaks)
				{
					stringBuilder.Append('\r');
					stringBuilder.Append('\n');
				}
			}
			foreach (KeyValuePair<GameInfoInt, int> keyValuePair2 in this.tableInts)
			{
				stringBuilder.Append(keyValuePair2.Key.ToStringCached<GameInfoInt>());
				stringBuilder.Append(':');
				stringBuilder.Append(keyValuePair2.Value);
				stringBuilder.Append(';');
				if (_lineBreaks)
				{
					stringBuilder.Append('\r');
					stringBuilder.Append('\n');
				}
			}
			foreach (KeyValuePair<GameInfoBool, bool> keyValuePair3 in this.tableBools)
			{
				stringBuilder.Append(keyValuePair3.Key.ToStringCached<GameInfoBool>());
				stringBuilder.Append(':');
				stringBuilder.Append(keyValuePair3.Value);
				stringBuilder.Append(';');
				if (_lineBreaks)
				{
					stringBuilder.Append('\r');
					stringBuilder.Append('\n');
				}
			}
			stringBuilder.Append('\r');
			stringBuilder.Append('\n');
			if (_lineBreaks)
			{
				this.cachedToStringLineBreaks = stringBuilder.ToString();
			}
			else
			{
				this.cachedToString = stringBuilder.ToString();
			}
		}
		if (!_lineBreaks)
		{
			return this.cachedToString;
		}
		return this.cachedToStringLineBreaks;
	}

	// Token: 0x0600338F RID: 13199 RVA: 0x0015D1C8 File Offset: 0x0015B3C8
	public override int GetHashCode()
	{
		if (this.hashcode == -9223372036854775808L)
		{
			string text = this.GetValue(GameInfoString.IP) + this.GetValue(GameInfoInt.Port).ToString();
			this.hashcode = (long)text.GetHashCode();
		}
		return (int)this.hashcode;
	}

	// Token: 0x06003390 RID: 13200 RVA: 0x0015D218 File Offset: 0x0015B418
	public override bool Equals(object _obj)
	{
		if (_obj == null)
		{
			return false;
		}
		GameServerInfo p = _obj as GameServerInfo;
		return this.Equals(p);
	}

	// Token: 0x06003391 RID: 13201 RVA: 0x0015D238 File Offset: 0x0015B438
	public bool Equals(GameServerInfo _p)
	{
		return _p != null && this.GetValue(GameInfoString.IP) == _p.GetValue(GameInfoString.IP) && this.GetValue(GameInfoInt.Port) == _p.GetValue(GameInfoInt.Port);
	}

	// Token: 0x06003392 RID: 13202 RVA: 0x0015D268 File Offset: 0x0015B468
	public void UpdateGameTimePlayers(ulong _time, int _players)
	{
		float time = Time.time;
		if (time - this.timeLastWorldTimeUpdate > 20f || this.GetValue(GameInfoInt.CurrentPlayers) != _players)
		{
			this.timeLastWorldTimeUpdate = time;
			if (PrefabEditModeManager.Instance.IsActive())
			{
				this.SetValue(GameInfoString.LevelName, PrefabEditModeManager.Instance.LoadedPrefab.Name);
			}
			this.SetValue(GameInfoInt.CurrentServerTime, (int)_time);
			this.SetValue(GameInfoInt.CurrentPlayers, _players);
			this.SetValue(GameInfoInt.FreePlayerSlots, this.GetValue(GameInfoInt.MaxPlayers) - _players);
			Action<GameServerInfo> onChangedAny = this.OnChangedAny;
			if (onChangedAny == null)
			{
				return;
			}
			onChangedAny(this);
		}
	}

	// Token: 0x06003393 RID: 13203 RVA: 0x0015D2F0 File Offset: 0x0015B4F0
	[PublicizedFrom(EAccessModifier.Private)]
	public void RefreshServerDisplayTexts(GameServerInfo gsi)
	{
		string value = gsi.GetValue(GameInfoString.CombinedPrimaryId);
		PlatformUserIdentifierAbs author = null;
		if (!string.IsNullOrEmpty(value))
		{
			author = PlatformUserIdentifierAbs.FromCombinedString(value, false);
		}
		string value2 = gsi.GetValue(GameInfoString.GameHost);
		string value3 = gsi.GetValue(GameInfoString.LevelName);
		string value4 = gsi.GetValue(GameInfoString.ServerDescription);
		string value5 = gsi.GetValue(GameInfoString.ServerWebsiteURL);
		string value6 = gsi.GetValue(GameInfoString.ServerLoginConfirmationText);
		if (!string.IsNullOrEmpty(value2) && string.IsNullOrEmpty(this.ServerDisplayName.Text))
		{
			this.ServerDisplayName.Update(value2, author);
		}
		if (!string.IsNullOrEmpty(value3) && string.IsNullOrEmpty(this.ServerWorldName.Text))
		{
			this.ServerWorldName.Update(value3, author);
		}
		if (!string.IsNullOrEmpty(value4) && string.IsNullOrEmpty(this.ServerDescription.Text))
		{
			this.ServerDescription.Update(value4.Replace("\\n", "\n"), author);
		}
		if (!string.IsNullOrEmpty(value5) && string.IsNullOrEmpty(this.ServerURL.Text))
		{
			this.ServerURL.Update(value5, author);
		}
		if (!string.IsNullOrEmpty(value6) && string.IsNullOrEmpty(this.ServerLoginConfirmationText.Text))
		{
			this.ServerLoginConfirmationText.Update(value6, author);
		}
	}

	// Token: 0x06003394 RID: 13204 RVA: 0x0015D41C File Offset: 0x0015B61C
	public void ClearOnChanged()
	{
		this.OnChangedAny = null;
		this.OnChangedString = null;
		this.OnChangedInt = null;
		this.OnChangedBool = null;
	}

	// Token: 0x06003395 RID: 13205 RVA: 0x0015D43A File Offset: 0x0015B63A
	public static bool IsSearchable(GameInfoString _gameInfoKey)
	{
		return GameServerInfo.SearchableStringInfosSet.Contains(_gameInfoKey);
	}

	// Token: 0x06003396 RID: 13206 RVA: 0x0015D447 File Offset: 0x0015B647
	public static bool IsSearchable(GameInfoInt _gameInfoKey)
	{
		return GameServerInfo.IntInfosInGameTagsSet.Contains(_gameInfoKey);
	}

	// Token: 0x06003397 RID: 13207 RVA: 0x0015D454 File Offset: 0x0015B654
	public static bool IsSearchable(GameInfoBool _gameInfoKey)
	{
		return GameServerInfo.BoolInfosInGameTagsSet.Contains(_gameInfoKey);
	}

	// Token: 0x06003398 RID: 13208 RVA: 0x0015D464 File Offset: 0x0015B664
	public static GameServerInfo BuildGameServerInfo()
	{
		GameServerInfo gameServerInfo = new GameServerInfo();
		bool @bool = GamePrefs.GetBool(EnumGamePrefs.ServerEnabled);
		if (GameManager.IsDedicatedServer && GamePrefs.GetBool(EnumGamePrefs.ServerAllowCrossplay))
		{
			bool flag = true;
			if (8 < GamePrefs.GetInt(EnumGamePrefs.ServerMaxPlayerCount))
			{
				Log.Warning(string.Format("CROSSPLAY INCOMPATIBLE VALUE: PLAYER COUNT GREATER THAN MAX OF {0}", 8));
				flag = false;
			}
			if (GamePrefs.GetBool(EnumGamePrefs.IgnoreEOSSanctions))
			{
				Log.Warning("CROSSPLAY INCOMPATIBLE VALUE: EOS SANCTIONS IGNORED");
				flag = false;
			}
			if (!flag)
			{
				Log.Warning("CROSSPLAY DISABLED FOR SESSION, CORRECT VALUES TO BE CROSSPLAY COMPATIBLE");
				GamePrefs.Set(EnumGamePrefs.ServerAllowCrossplay, false);
			}
		}
		gameServerInfo.SetValue(GameInfoString.GameType, "7DTD");
		gameServerInfo.SetValue(GameInfoString.GameName, GamePrefs.GetString(EnumGamePrefs.GameName));
		gameServerInfo.SetValue(GameInfoString.GameMode, GamePrefs.GetString(EnumGamePrefs.GameMode).Replace("GameMode", ""));
		gameServerInfo.SetValue(GameInfoString.GameHost, GameManager.IsDedicatedServer ? GamePrefs.GetString(EnumGamePrefs.ServerName) : GamePrefs.GetString(EnumGamePrefs.PlayerName));
		GameInfoString key = GameInfoString.LevelName;
		PrefabEditModeManager instance = PrefabEditModeManager.Instance;
		gameServerInfo.SetValue(key, (instance != null && instance.IsActive()) ? PrefabEditModeManager.Instance.LoadedPrefab.Name : GamePrefs.GetString(EnumGamePrefs.GameWorld));
		gameServerInfo.SetValue(GameInfoString.ServerDescription, GamePrefs.GetString(EnumGamePrefs.ServerDescription));
		gameServerInfo.SetValue(GameInfoString.ServerWebsiteURL, GamePrefs.GetString(EnumGamePrefs.ServerWebsiteURL));
		gameServerInfo.SetValue(GameInfoString.ServerLoginConfirmationText, GamePrefs.GetString(EnumGamePrefs.ServerLoginConfirmationText));
		gameServerInfo.SetValue(GameInfoBool.IsDedicated, GameManager.IsDedicatedServer);
		gameServerInfo.SetValue(GameInfoBool.IsPasswordProtected, !string.IsNullOrEmpty(GamePrefs.GetString(EnumGamePrefs.ServerPassword)));
		bool value = GameManager.IsDedicatedServer ? GamePrefs.GetBool(EnumGamePrefs.EACEnabled) : GamePrefs.GetBool(EnumGamePrefs.ServerEACPeerToPeer);
		gameServerInfo.SetValue(GameInfoBool.EACEnabled, value);
		gameServerInfo.SetValue(GameInfoBool.SanctionsIgnored, GameManager.IsDedicatedServer && GamePrefs.GetBool(EnumGamePrefs.IgnoreEOSSanctions));
		gameServerInfo.SetValue(GameInfoBool.AllowCrossplay, @bool && GamePrefs.GetBool(EnumGamePrefs.ServerAllowCrossplay) && PermissionsManager.IsCrossplayAllowed());
		gameServerInfo.SetValue(GameInfoString.PlayGroup, EPlayGroupExtensions.Current.ToStringCached<EPlayGroup>());
		gameServerInfo.SetValue(GameInfoInt.MaxPlayers, GamePrefs.GetInt(EnumGamePrefs.ServerMaxPlayerCount));
		gameServerInfo.SetValue(GameInfoInt.FreePlayerSlots, GamePrefs.GetInt(EnumGamePrefs.ServerMaxPlayerCount) - (GameManager.IsDedicatedServer ? 0 : 1));
		gameServerInfo.SetValue(GameInfoInt.CurrentPlayers, GameManager.IsDedicatedServer ? 0 : 1);
		gameServerInfo.SetValue(GameInfoInt.Port, GamePrefs.GetInt(EnumGamePrefs.ServerPort));
		gameServerInfo.SetValue(GameInfoString.ServerVersion, Constants.cVersionInformation.SerializableString);
		gameServerInfo.SetValue(GameInfoBool.Architecture64, !Constants.Is32BitOs);
		gameServerInfo.SetValue(GameInfoString.Platform, Application.platform.ToString());
		gameServerInfo.SetValue(GameInfoBool.IsPublic, GamePrefs.GetBool(EnumGamePrefs.ServerIsPublic));
		gameServerInfo.SetValue(GameInfoInt.ServerVisibility, PermissionsManager.IsMultiplayerAllowed() ? GamePrefs.GetInt(EnumGamePrefs.ServerVisibility) : 0);
		gameServerInfo.SetValue(GameInfoBool.StockSettings, GamePrefs.HasStockSettings());
		bool flag2 = StockFileHashes.HasStockXMLs();
		gameServerInfo.SetValue(GameInfoBool.StockFiles, flag2);
		gameServerInfo.SetValue(GameInfoBool.ModdedConfig, !flag2 || ModManager.AnyConfigModActive());
		gameServerInfo.SetValue(GameInfoString.Region, GamePrefs.GetString(EnumGamePrefs.Region));
		gameServerInfo.SetValue(GameInfoString.Language, GameManager.IsDedicatedServer ? GamePrefs.GetString(EnumGamePrefs.Language) : Localization.language);
		gameServerInfo.SetValue(GameInfoInt.GameDifficulty, GamePrefs.GetInt(EnumGamePrefs.GameDifficulty));
		gameServerInfo.SetValue(GameInfoInt.BlockDamagePlayer, GamePrefs.GetInt(EnumGamePrefs.BlockDamagePlayer));
		gameServerInfo.SetValue(GameInfoInt.BlockDamageAI, GamePrefs.GetInt(EnumGamePrefs.BlockDamageAI));
		gameServerInfo.SetValue(GameInfoInt.BlockDamageAIBM, GamePrefs.GetInt(EnumGamePrefs.BlockDamageAIBM));
		gameServerInfo.SetValue(GameInfoInt.XPMultiplier, GamePrefs.GetInt(EnumGamePrefs.XPMultiplier));
		gameServerInfo.SetValue(GameInfoBool.BuildCreate, GamePrefs.GetBool(EnumGamePrefs.BuildCreate));
		gameServerInfo.SetValue(GameInfoInt.DayNightLength, GamePrefs.GetInt(EnumGamePrefs.DayNightLength));
		gameServerInfo.SetValue(GameInfoInt.DayLightLength, GamePrefs.GetInt(EnumGamePrefs.DayLightLength));
		gameServerInfo.SetValue(GameInfoInt.DeathPenalty, GamePrefs.GetInt(EnumGamePrefs.DeathPenalty));
		gameServerInfo.SetValue(GameInfoInt.DropOnDeath, GamePrefs.GetInt(EnumGamePrefs.DropOnDeath));
		gameServerInfo.SetValue(GameInfoInt.DropOnQuit, GamePrefs.GetInt(EnumGamePrefs.DropOnQuit));
		gameServerInfo.SetValue(GameInfoInt.BedrollDeadZoneSize, GamePrefs.GetInt(EnumGamePrefs.BedrollDeadZoneSize));
		gameServerInfo.SetValue(GameInfoInt.BedrollExpiryTime, GamePrefs.GetInt(EnumGamePrefs.BedrollExpiryTime));
		gameServerInfo.SetValue(GameInfoInt.MaxSpawnedZombies, GamePrefs.GetInt(EnumGamePrefs.MaxSpawnedZombies));
		gameServerInfo.SetValue(GameInfoInt.MaxSpawnedAnimals, GamePrefs.GetInt(EnumGamePrefs.MaxSpawnedAnimals));
		gameServerInfo.SetValue(GameInfoBool.EnemySpawnMode, GamePrefs.GetBool(EnumGamePrefs.EnemySpawnMode));
		gameServerInfo.SetValue(GameInfoInt.EnemyDifficulty, GamePrefs.GetInt(EnumGamePrefs.EnemyDifficulty));
		gameServerInfo.SetValue(GameInfoInt.ZombieFeralSense, GamePrefs.GetInt(EnumGamePrefs.ZombieFeralSense));
		gameServerInfo.SetValue(GameInfoInt.ZombieMove, GamePrefs.GetInt(EnumGamePrefs.ZombieMove));
		gameServerInfo.SetValue(GameInfoInt.ZombieMoveNight, GamePrefs.GetInt(EnumGamePrefs.ZombieMoveNight));
		gameServerInfo.SetValue(GameInfoInt.ZombieFeralMove, GamePrefs.GetInt(EnumGamePrefs.ZombieFeralMove));
		gameServerInfo.SetValue(GameInfoInt.ZombieBMMove, GamePrefs.GetInt(EnumGamePrefs.ZombieBMMove));
		gameServerInfo.SetValue(GameInfoInt.BloodMoonFrequency, GamePrefs.GetInt(EnumGamePrefs.BloodMoonFrequency));
		gameServerInfo.SetValue(GameInfoInt.BloodMoonRange, GamePrefs.GetInt(EnumGamePrefs.BloodMoonRange));
		gameServerInfo.SetValue(GameInfoInt.BloodMoonWarning, GamePrefs.GetInt(EnumGamePrefs.BloodMoonWarning));
		gameServerInfo.SetValue(GameInfoInt.BloodMoonEnemyCount, GamePrefs.GetInt(EnumGamePrefs.BloodMoonEnemyCount));
		gameServerInfo.SetValue(GameInfoInt.LootAbundance, GamePrefs.GetInt(EnumGamePrefs.LootAbundance));
		int num = GamePrefs.GetInt(EnumGamePrefs.LootRespawnDays);
		if (num == 0)
		{
			num = -1;
		}
		gameServerInfo.SetValue(GameInfoInt.LootRespawnDays, num);
		gameServerInfo.SetValue(GameInfoInt.AirDropFrequency, GamePrefs.GetInt(EnumGamePrefs.AirDropFrequency));
		gameServerInfo.SetValue(GameInfoBool.AirDropMarker, GamePrefs.GetBool(EnumGamePrefs.AirDropMarker));
		gameServerInfo.SetValue(GameInfoInt.PartySharedKillRange, GamePrefs.GetInt(EnumGamePrefs.PartySharedKillRange));
		gameServerInfo.SetValue(GameInfoInt.PlayerKillingMode, GamePrefs.GetInt(EnumGamePrefs.PlayerKillingMode));
		gameServerInfo.SetValue(GameInfoInt.LandClaimCount, GamePrefs.GetInt(EnumGamePrefs.LandClaimCount));
		gameServerInfo.SetValue(GameInfoInt.LandClaimSize, GamePrefs.GetInt(EnumGamePrefs.LandClaimSize));
		gameServerInfo.SetValue(GameInfoInt.LandClaimDeadZone, GamePrefs.GetInt(EnumGamePrefs.LandClaimDeadZone));
		gameServerInfo.SetValue(GameInfoInt.LandClaimExpiryTime, GamePrefs.GetInt(EnumGamePrefs.LandClaimExpiryTime));
		gameServerInfo.SetValue(GameInfoInt.LandClaimDecayMode, GamePrefs.GetInt(EnumGamePrefs.LandClaimDecayMode));
		gameServerInfo.SetValue(GameInfoInt.LandClaimOnlineDurabilityModifier, GamePrefs.GetInt(EnumGamePrefs.LandClaimOnlineDurabilityModifier));
		gameServerInfo.SetValue(GameInfoInt.LandClaimOfflineDurabilityModifier, GamePrefs.GetInt(EnumGamePrefs.LandClaimOfflineDurabilityModifier));
		gameServerInfo.SetValue(GameInfoInt.LandClaimOfflineDelay, GamePrefs.GetInt(EnumGamePrefs.LandClaimOfflineDelay));
		gameServerInfo.SetValue(GameInfoInt.MaxChunkAge, GamePrefs.GetInt(EnumGamePrefs.MaxChunkAge));
		gameServerInfo.SetValue(GameInfoBool.ShowFriendPlayerOnMap, GamePrefs.GetBool(EnumGamePrefs.ShowFriendPlayerOnMap));
		gameServerInfo.SetValue(GameInfoInt.DayCount, GamePrefs.GetInt(EnumGamePrefs.DayCount));
		gameServerInfo.SetValue(GameInfoBool.AllowSpawnNearBackpack, GamePrefs.GetBool(EnumGamePrefs.AllowSpawnNearBackpack));
		gameServerInfo.SetValue(GameInfoInt.AllowSpawnNearFriend, GamePrefs.GetInt(EnumGamePrefs.AllowSpawnNearFriend));
		gameServerInfo.SetValue(GameInfoInt.QuestProgressionDailyLimit, GamePrefs.GetInt(EnumGamePrefs.QuestProgressionDailyLimit));
		gameServerInfo.SetValue(GameInfoBool.BiomeProgression, GamePrefs.GetBool(EnumGamePrefs.BiomeProgression));
		gameServerInfo.SetValue(GameInfoInt.StormFreq, GamePrefs.GetInt(EnumGamePrefs.StormFreq));
		return gameServerInfo;
	}

	// Token: 0x06003399 RID: 13209 RVA: 0x0015DA22 File Offset: 0x0015BC22
	public static void PrepareLocalServerInfo()
	{
		SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo = GameServerInfo.BuildGameServerInfo();
		GameInfoIntLimits.ValidateGameServerInfoCrossplaySettings(SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo);
	}

	// Token: 0x0600339A RID: 13210 RVA: 0x0015DA44 File Offset: 0x0015BC44
	public static void SetLocalServerWorldInfo()
	{
		SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.SetValue(GameInfoInt.CurrentServerTime, (int)GameManager.Instance.World.worldTime);
		Vector3i other;
		Vector3i one;
		GameManager.Instance.World.GetWorldExtent(out other, out one);
		Vector3i vector3i = one - other;
		SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.SetValue(GameInfoInt.WorldSize, vector3i.x);
	}

	// Token: 0x04002A2D RID: 10797
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

	// Token: 0x04002A2E RID: 10798
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<GameInfoString, string> tableStrings = new EnumDictionary<GameInfoString, string>();

	// Token: 0x04002A2F RID: 10799
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<GameInfoInt, int> tableInts = new EnumDictionary<GameInfoInt, int>();

	// Token: 0x04002A30 RID: 10800
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<GameInfoBool, bool> tableBools = new EnumDictionary<GameInfoBool, bool>();

	// Token: 0x04002A31 RID: 10801
	public readonly ReadOnlyDictionary<GameInfoString, string> Strings;

	// Token: 0x04002A32 RID: 10802
	public readonly ReadOnlyDictionary<GameInfoInt, int> Ints;

	// Token: 0x04002A33 RID: 10803
	public readonly ReadOnlyDictionary<GameInfoBool, bool> Bools;

	// Token: 0x04002A34 RID: 10804
	public AuthoredText ServerDisplayName = new AuthoredText();

	// Token: 0x04002A35 RID: 10805
	public AuthoredText ServerWorldName = new AuthoredText();

	// Token: 0x04002A36 RID: 10806
	public AuthoredText ServerDescription = new AuthoredText();

	// Token: 0x04002A37 RID: 10807
	public AuthoredText ServerURL = new AuthoredText();

	// Token: 0x04002A38 RID: 10808
	public AuthoredText ServerLoginConfirmationText = new AuthoredText();

	// Token: 0x04002A39 RID: 10809
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isBroken;

	// Token: 0x04002A3A RID: 10810
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isFriends;

	// Token: 0x04002A3B RID: 10811
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isFavorite;

	// Token: 0x04002A3C RID: 10812
	[PublicizedFrom(EAccessModifier.Private)]
	public int lastPlayed;

	// Token: 0x04002A3D RID: 10813
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isLan;

	// Token: 0x04002A3E RID: 10814
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isLobby;

	// Token: 0x04002A3F RID: 10815
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isNoResponse;

	// Token: 0x04002A40 RID: 10816
	[PublicizedFrom(EAccessModifier.Private)]
	public long hashcode = long.MinValue;

	// Token: 0x04002A41 RID: 10817
	[PublicizedFrom(EAccessModifier.Private)]
	public VersionInformation version = new VersionInformation(VersionInformation.EGameReleaseType.Alpha, -1, -1, -1);

	// Token: 0x04002A46 RID: 10822
	[PublicizedFrom(EAccessModifier.Private)]
	public float timeLastWorldTimeUpdate;

	// Token: 0x04002A47 RID: 10823
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly char[] whiteSpaceChars = new char[]
	{
		' ',
		'\r',
		'\n',
		'\t'
	};

	// Token: 0x04002A48 RID: 10824
	[PublicizedFrom(EAccessModifier.Private)]
	public string cachedToString;

	// Token: 0x04002A49 RID: 10825
	[PublicizedFrom(EAccessModifier.Private)]
	public string cachedToStringLineBreaks;

	// Token: 0x04002A4A RID: 10826
	public static readonly GameInfoString[] SearchableStringInfos = new GameInfoString[]
	{
		GameInfoString.LevelName,
		GameInfoString.GameHost,
		GameInfoString.SteamID,
		GameInfoString.Region,
		GameInfoString.Language,
		GameInfoString.UniqueId,
		GameInfoString.CombinedNativeId,
		GameInfoString.ServerVersion,
		GameInfoString.PlayGroup
	};

	// Token: 0x04002A4B RID: 10827
	public static readonly GameInfoInt[] IntInfosInGameTags = new GameInfoInt[]
	{
		GameInfoInt.GameDifficulty,
		GameInfoInt.DayNightLength,
		GameInfoInt.DeathPenalty,
		GameInfoInt.DropOnDeath,
		GameInfoInt.DropOnQuit,
		GameInfoInt.BloodMoonEnemyCount,
		GameInfoInt.EnemyDifficulty,
		GameInfoInt.PlayerKillingMode,
		GameInfoInt.CurrentServerTime,
		GameInfoInt.DayLightLength,
		GameInfoInt.AirDropFrequency,
		GameInfoInt.LootAbundance,
		GameInfoInt.LootRespawnDays,
		GameInfoInt.MaxSpawnedZombies,
		GameInfoInt.LandClaimCount,
		GameInfoInt.LandClaimSize,
		GameInfoInt.LandClaimExpiryTime,
		GameInfoInt.LandClaimDecayMode,
		GameInfoInt.LandClaimOnlineDurabilityModifier,
		GameInfoInt.LandClaimOfflineDurabilityModifier,
		GameInfoInt.MaxSpawnedAnimals,
		GameInfoInt.PartySharedKillRange,
		GameInfoInt.ZombieFeralSense,
		GameInfoInt.ZombieMove,
		GameInfoInt.ZombieMoveNight,
		GameInfoInt.ZombieFeralMove,
		GameInfoInt.ZombieBMMove,
		GameInfoInt.XPMultiplier,
		GameInfoInt.BlockDamagePlayer,
		GameInfoInt.BlockDamageAI,
		GameInfoInt.BlockDamageAIBM,
		GameInfoInt.BloodMoonFrequency,
		GameInfoInt.BloodMoonRange,
		GameInfoInt.BloodMoonWarning,
		GameInfoInt.BedrollExpiryTime,
		GameInfoInt.LandClaimOfflineDelay,
		GameInfoInt.Port,
		GameInfoInt.FreePlayerSlots,
		GameInfoInt.CurrentPlayers,
		GameInfoInt.MaxPlayers,
		GameInfoInt.WorldSize,
		GameInfoInt.MaxChunkAge,
		GameInfoInt.QuestProgressionDailyLimit,
		GameInfoInt.StormFreq
	};

	// Token: 0x04002A4C RID: 10828
	public static readonly GameInfoBool[] BoolInfosInGameTags = new GameInfoBool[]
	{
		GameInfoBool.IsDedicated,
		GameInfoBool.ShowFriendPlayerOnMap,
		GameInfoBool.BuildCreate,
		GameInfoBool.StockSettings,
		GameInfoBool.ModdedConfig,
		GameInfoBool.RequiresMod,
		GameInfoBool.AirDropMarker,
		GameInfoBool.EnemySpawnMode,
		GameInfoBool.IsPasswordProtected,
		GameInfoBool.AllowCrossplay,
		GameInfoBool.EACEnabled,
		GameInfoBool.SanctionsIgnored,
		GameInfoBool.BiomeProgression
	};

	// Token: 0x04002A4D RID: 10829
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly HashSet<GameInfoString> SearchableStringInfosSet = new HashSet<GameInfoString>(GameServerInfo.SearchableStringInfos);

	// Token: 0x04002A4E RID: 10830
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly HashSet<GameInfoInt> IntInfosInGameTagsSet = new HashSet<GameInfoInt>(GameServerInfo.IntInfosInGameTags);

	// Token: 0x04002A4F RID: 10831
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly HashSet<GameInfoBool> BoolInfosInGameTagsSet = new HashSet<GameInfoBool>(GameServerInfo.BoolInfosInGameTags);

	// Token: 0x020006CF RID: 1743
	public class UniqueIdEqualityComparer : IEqualityComparer<GameServerInfo>
	{
		// Token: 0x0600339C RID: 13212 RVA: 0x0000A7E3 File Offset: 0x000089E3
		[PublicizedFrom(EAccessModifier.Private)]
		public UniqueIdEqualityComparer()
		{
		}

		// Token: 0x0600339D RID: 13213 RVA: 0x0015DB53 File Offset: 0x0015BD53
		public bool Equals(GameServerInfo _x, GameServerInfo _y)
		{
			return _x == _y || (_x != null && _y != null && _x.GetValue(GameInfoString.UniqueId) == _y.GetValue(GameInfoString.UniqueId));
		}

		// Token: 0x0600339E RID: 13214 RVA: 0x0015DB78 File Offset: 0x0015BD78
		public int GetHashCode(GameServerInfo _obj)
		{
			return _obj.GetValue(GameInfoString.UniqueId).GetHashCode();
		}

		// Token: 0x04002A50 RID: 10832
		public static readonly GameServerInfo.UniqueIdEqualityComparer Instance = new GameServerInfo.UniqueIdEqualityComparer();
	}
}
